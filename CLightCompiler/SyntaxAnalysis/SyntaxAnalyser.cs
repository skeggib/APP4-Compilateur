﻿using LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalysis
{
    public class SyntaxAnalyser
    {
        private IList<Token> _tokens;
        private int _index;

        private Node _forPostOperation; // Sert a executer le post-action dans une boucle for avant un continue

        /// <summary>
        /// Convertit une liste de tokens en arbre
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public Node Convert(IList<Token> tokens)
        {
            _tokens = tokens;
            _index = 0;

            return Program();
        }

        // Z -> D*
        private Node Program()
        {
            var program = new Node(Nodes.Program, _tokens[0]);

            Node declarationFunc;
            do
            {
                declarationFunc = DeclarationFunc();
                if (declarationFunc != null)
                    program.Childs.Add(declarationFunc);
            } while (declarationFunc != null);

            return program;
        }

        // D -> 'int' ident '(' ('int' ident)* ')' S
        private Node DeclarationFunc()
        {
            if (_index < _tokens.Count && _tokens[_index].Category == Tokens.Int)
            {
                _index++; // On mange 'int'

                if (_index == _tokens.Count || _tokens[_index].Category != Tokens.Ident)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected identifier");

                var declarationFunc = new Node(Nodes.DeclFunc, _tokens[_index]);

                _index++; // On mange ident

                if (_index == _tokens.Count || _tokens[_index].Category != Tokens.OpeningParenthesis)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected '('");

                _index++; // On mange '('

                if (_index < _tokens.Count && _tokens[_index].Category == Tokens.Int)
                {
                    _index++; // On mange 'int'
                    if (_index < _tokens.Count && _tokens[_index].Category == Tokens.Ident)
                    {
                        declarationFunc.Tokens.Add(_tokens[_index]);
                        _index++; // On mange ident
                    }
                    else
                        throw new SyntaxException(_tokens[_index - 1].Offset, "Expected identifier");

                    while (_index < _tokens.Count && _tokens[_index].Category == Tokens.Comma)
                    {
                        _index++; // On mange ','

                        if (_index < _tokens.Count && _tokens[_index].Category == Tokens.Int)
                        {
                            _index++; // On mange 'int'
                            if (_index < _tokens.Count && _tokens[_index].Category == Tokens.Ident)
                            {
                                declarationFunc.Tokens.Add(_tokens[_index]);
                                _index++; // On mange ident
                            }
                            else
                                throw new SyntaxException(_tokens[_index - 1].Offset, "Expected identifier");
                        }
                        
                        else
                            throw new SyntaxException(_tokens[_index - 1].Offset, "Expected 'int'");
                    }
                }

                if (_index < _tokens.Count && _tokens[_index].Category == Tokens.ClosingParenthesis)
                    _index++; // On mange ')'
                else
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ')'");

                var statement = Statement();
                if (statement == null)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected statement");

                declarationFunc.Childs.Add(statement);

                return declarationFunc;
            }

            return null;
        }

        // S -> { S* } | A; | E; | if (E) S (else S)? | int ident; | while(E)S | do S while(E) | for(A; E; A)S | break; | continue; | return E;
        private Node Statement()
        {
            if (_index >= _tokens.Count)
                return null;

            Node a, e;

            if (_tokens[_index].Category == Tokens.OpeningBrace)
            {
                var tokenOpeningBrace = _tokens[_index];
                _index++;
                Node block = new Node(Nodes.Block, tokenOpeningBrace);

                Node s;
                do {
                    s = Statement();
                    if (s != null)
                        block.Childs.Add(s);
                } while (_index < _tokens.Count && _tokens[_index].Category != Tokens.ClosingBrace && s != null);
                if (_index >= _tokens.Count)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected '}'");
                _index++;
                return block;
            }

            else if ((a = Affectation()) != null) {
                if (_index >= _tokens.Count || _tokens[_index].Category != Tokens.Semicolon)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ';'");
                _index++;
                return a;
            }

            else if ((e = Expression()) != null)
            {
                if (_index >= _tokens.Count || _tokens[_index].Category != Tokens.Semicolon)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ';'");

                var tokenSemicolon = _tokens[_index];
                _index++;

                Node d = new Node(Nodes.Drop, tokenSemicolon, e);
                return d;
            }

            else if (_tokens[_index].Category == Tokens.Out)
            {
                var tokenOut = _tokens[_index];
                _index++;
                if ((e = Expression()) != null)
                {
                    if (_index >= _tokens.Count || _tokens[_index].Category != Tokens.Semicolon)
                        throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ';'");
                    _index++;

                    Node o = new Node(Nodes.Out, tokenOut, e);
                    return o;
                }

            }

            else if (_tokens[_index].Category == Tokens.If)
            {
                var tokenIf = _tokens[_index];
                _index++;
                Node p;
                if (_index >= _tokens.Count ||
                    _tokens[_index].Category != Tokens.OpeningParenthesis ||
                    (p = Primary()) == null)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected primary");

                Node sIf = Statement();
                if (_index >= _tokens.Count || sIf == null)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected if body");

                Node cond = new Node(Nodes.Condition, tokenIf, p, sIf);

                if (_index < _tokens.Count &&
                    _tokens[_index].Category == Tokens.Else)
                {
                    _index++;
                    Node sElse = Statement();
                    if (_index >= _tokens.Count || sElse == null)
                        throw new SyntaxException(_tokens[_index - 1].Offset, "Expected else body");

                    cond.Childs.Add(sElse);
                }

                return cond;
            }

            else if (_tokens[_index].Category == Tokens.Int)
            {
                _index++;
                if (_index >= _tokens.Count || _tokens[_index].Category != Tokens.Ident)
                    throw new SyntaxException(_tokens[_index].Offset, "Expected identifier");
                Token token = _tokens[_index];
                _index++;
                if (_index >= _tokens.Count || _tokens[_index].Category != Tokens.Semicolon)
                    throw new SyntaxException(_tokens[_index].Offset, "Expected ';'");
                _index++;

                return new Node(Nodes.DeclVar, token);
            }

            else if (_tokens[_index].Category == Tokens.While)
            {
                var tokenWhile = _tokens[_index];
                _index++; //On mange "while"
                Node p;
                if (_index >= _tokens.Count ||
                    _tokens[_index].Category != Tokens.OpeningParenthesis ||
                    (p = Primary()) == null)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected primary");

                Node s = Statement();
                if (_index >= _tokens.Count || s == null)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected while body");

                Node break_node = new Node(Nodes.Break, tokenWhile);
                Node cond = new Node(Nodes.Condition, tokenWhile, p, s, break_node);
                Node while_loop = new Node(Nodes.Loop, tokenWhile, cond);
                return while_loop;
            }

            else if (_tokens[_index].Category == Tokens.Do)
            {
                _index++;
                Node s;
                if (_index >= _tokens.Count || (s = Statement()) == null)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected primary");

                if (_tokens[_index].Category == Tokens.While)
                {
                    var currentToken = _tokens[_index];
                    _index++;
                    Node p;
                    if (_index >= _tokens.Count ||
                        _tokens[_index].Category != Tokens.OpeningParenthesis ||
                        (p = Primary()) == null)
                        throw new SyntaxException(_tokens[_index - 1].Offset, "Expected primary");

                    //if (_index >= _tokens.Count || _tokens[_index].Category != Tokens.Semicolon)
                    //    throw new SyntaxException(_tokens[_index].Offset, "Expected ';'");
                    //_index++;

                    Node break_node = new Node(Nodes.Break, currentToken);
                    Node continue_node = new Node(Nodes.Continue, currentToken);
                    Node cond = new Node(Nodes.Condition, currentToken, p, continue_node, break_node);
                    Node block_node = new Node(Nodes.Block, currentToken, s, cond);
                    Node loop_node = new Node(Nodes.Loop, currentToken, block_node);
                    return loop_node;
                }
            }

            else if (_tokens[_index].Category == Tokens.For)
            {
                var currentToken = _tokens[_index];
                _index++;
                if(_index>=_tokens.Count || _tokens[_index].Category != Tokens.OpeningParenthesis)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected opening parenthesis");

                _index++;
                Node init;
                if (_index >= _tokens.Count || (init = Affectation())==null )
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected initialisateur");

                if (_index >= _tokens.Count || _tokens[_index].Category != Tokens.Semicolon)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ';'");

                _index++;
                Node expre;
                if (_index >= _tokens.Count || (expre = Expression())==null )
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected expression");

                if (_index >= _tokens.Count || _tokens[_index].Category != Tokens.Semicolon)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ';'");

                _index++;
                Node post_op;
                if (_index >= _tokens.Count || (post_op = Affectation()) == null)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected initialisateur");

                if (_index >= _tokens.Count || _tokens[_index].Category != Tokens.ClosingParenthesis)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ')'");

                _index++;

                _forPostOperation = post_op;
                Node s;
                if (_index >= _tokens.Count || (s = Statement()) == null)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected for body");
                _forPostOperation = null;

                Node body_node = new Node(Nodes.Block, currentToken, s, post_op);
                Node cond_node = new Node(Nodes.Condition, currentToken, expre, body_node, new Node(Nodes.Break, currentToken));
                Node loop_node = new Node(Nodes.Loop, currentToken, cond_node);
                Node for_node = new Node(Nodes.Block, currentToken, init, loop_node);
                return for_node;
            }

            else if (_tokens[_index].Category == Tokens.Break)
            {
                var token = _tokens[_index];
                _index++; //On mange le "break"
                if (_index >= _tokens.Count ||_tokens[_index].Category != Tokens.Semicolon )
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ';'");
                _index++;
                return new Node(Nodes.Break, token);
            }       

            else if (_tokens[_index].Category == Tokens.Continue)
            {
                var token = _tokens[_index];
                _index++; //On mange le "continue"
                if (_index >= _tokens.Count || _tokens[_index].Category != Tokens.Semicolon)
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ';'");
                _index++;
                
                var node = new Node(Nodes.Continue, token);
                if (_forPostOperation != null)
                    node.Childs.Add(_forPostOperation);
                return node;
            }
                
            else if (_tokens[_index].Category == Tokens.Return)
            {
                var currentToken = _tokens[_index];
                _index++; // On mange 'return'
                var expression = Expression();
                if (expression == null)
                    throw new SyntaxException(_tokens[_index].Offset, "Expected expression");
                if (_index < _tokens.Count && _tokens[_index].Category == Tokens.Semicolon)
                {
                    _index++; // On mange ';'
                    return new Node(Nodes.Return, currentToken, expression);
                }
                
                throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ';'");
            }

            return null;
        }

        // A -> *ident = E | ident ('['E']')? = E
        private Node Affectation()
        {
            if (_index >= _tokens.Count)
                return null;

            if (_tokens[_index].Category == Tokens.Asterisk)
            {
                _index++; // On mange '*'

                if (_index < _tokens.Count && _tokens[_index].Category == Tokens.Ident)
                {
                    var indirSet = new Node(Nodes.IndirSet, _tokens[_index]);
                    _index++; // On mange l'ident

                    if (_index < _tokens.Count && _tokens[_index].Category == Tokens.Assign)
                    {
                        _index++; // On mange '='

                        var expression = Expression();
                        if (expression == null)
                            throw new SyntaxException(_tokens[_index - 1].Offset, "Expected expression");

                        indirSet.Childs.Add(expression);

                        return indirSet;
                    }
                    else
                        throw new SyntaxException(_tokens[_index - 1].Offset, "Expected '='");
                }
                else
                    throw new SyntaxException(_tokens[_index - 1].Offset, "Expected identifier");
            }

            else if (_tokens[_index].Category == Tokens.Ident)
            {
                Token token = _tokens[_index];
                _index++; // On mange l'ident

                Node node = null;

                if (_index < _tokens.Count && _tokens[_index].Category == Tokens.OpeningBracket)
                {
                    _index++; // On mange '['
                    var expression = Expression();
                    if (expression == null)
                        throw new SyntaxException(_tokens[_index - 1].Offset, "Expression expected");
                    if (_index < _tokens.Count && _tokens[_index].Category == Tokens.ClosingBracket)
                    {
                        _index++; // On mange ']'
                        node = new Node(Nodes.IndexSet, token, expression);
                    }
                    else
                        throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ']'");
                }

                else if (_index < _tokens.Count && _tokens[_index].Category == Tokens.Assign)
                    node = new Node(Nodes.Assign, token);

                else
                {
                    _index--;
                    return null;
                }

                if (_index < _tokens.Count && _tokens[_index].Category == Tokens.Assign)
                {
                    _index++; // On mange '='
                    Node e = Expression();
                    if (e == null)
                        throw new SyntaxException(_tokens[_index + 1].Offset, "Expression expected");
                    node.Childs.Add(e);
                }

                return node;
            }

            return null;
        }

        // P -> *ident | ident [ ('(' E? | E(,E)* ')') | '['E']' ]? | const | -P | !P | (E)
        private Node Primary()
        {
            if (_index >= _tokens.Count)
                return null;

            switch (_tokens[_index].Category)
            {
                case Tokens.Asterisk:
                    _index++; // On mange '*'

                    if (_index < _tokens.Count && _tokens[_index].Category == Tokens.Ident)
                        return new Node(Nodes.Indir, _tokens[_index++]);
                    else
                        throw new SyntaxException(_tokens[_index].Offset, "Expected identifier");

                case Tokens.Ident:
                    _index++;

                    // Si '(' apres identifiant
                    if (_index < _tokens.Count && _tokens[_index].Category == Tokens.OpeningParenthesis)
                    {
                        var call = new Node(Nodes.Call, _tokens[_index - 1]);

                        _index++; // On mange '('

                        var expression = Expression();

                        while (expression != null)
                        {
                            call.Childs.Add(expression);

                            if (_index < _tokens.Count && _tokens[_index].Category == Tokens.Comma)
                            {
                                _index++; // On mange ','
                                expression = Expression();
                                if (expression == null)
                                    throw new SyntaxException(_tokens[_index].Offset, "Expected expression");
                            }

                            else
                                break;
                        }

                        if (_index < _tokens.Count && _tokens[_index].Category == Tokens.ClosingParenthesis)
                            _index++;
                        else
                            throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ')'");

                        return call;
                    }

                    // Si '[' apres identifiant
                    else if (_index < _tokens.Count && _tokens[_index].Category == Tokens.OpeningBracket)
                    {
                        var index = new Node(Nodes.Index, _tokens[_index - 1]);

                        _index++; // On mange '['

                        var expression = Expression();

                        if (expression == null)
                            throw new SyntaxException(_tokens[_index - 1].Offset, "Expected expression");

                        index.Childs.Add(expression);

                        if (_index < _tokens.Count && _tokens[_index].Category == Tokens.ClosingBracket)
                            _index++; // On mange ']'
                        else
                            throw new SyntaxException(_tokens[_index - 1].Offset, "Expected ']'");

                        return index;
                    }

                    return new Node(Nodes.RefVar, _tokens[_index - 1]);

                case Tokens.Value:
                    _index++;
                    return new Node(Nodes.Const, _tokens[_index - 1]);

                case Tokens.Minus: // TODO Revoir si deux '-' a la suite sont valides
                    {
                        var tokenMinus = _tokens[_index];
                        _index++;
                        Node p = Primary();
                        if (p == null)
                            throw new SyntaxException(_tokens[_index - 1].Offset, "unexpected token after '-'");
                        return new Node(Nodes.Negative, tokenMinus, p);
                    }

                case Tokens.Not:
                    {
                        var tokenNot = _tokens[_index];
                        _index++;
                        Node p = Primary();
                        if (p == null)
                            throw new SyntaxException(_tokens[_index - 1].Offset, "unexpected token after '!'");
                        return new Node(Nodes.Not, tokenNot, p);
                    }

                case Tokens.OpeningParenthesis:
                    _index++;
                    Node e = Expression();
                    if (e == null)
                        throw new SyntaxException(_tokens[_index - 1].Offset, "unexpected expression");
                    if (_index >= _tokens.Count || _tokens[_index].Category != Tokens.ClosingParenthesis)
                        throw new SyntaxException(_tokens[_index-1].Offset, "expected ')'");
                    _index++;
                    return e;
            }

            return null;
        }

        // F -> P [ ( * | / ) F ]
        private Node Factor()
        {
            if (_index >= _tokens.Count)
                return null;

            Node p = Primary();
            if (p == null)
                return null;

            if (_index >= _tokens.Count)
                return p;

            Node op = null;
            if (_tokens[_index].Category == Tokens.Asterisk)
                op = new Node(Nodes.Multiplication, _tokens[_index], p);
            else if (_tokens[_index].Category == Tokens.Divide)
                op = new Node(Nodes.Division, _tokens[_index], p);
            else if (_tokens[_index].Category == Tokens.Modulo)
                op = new Node(Nodes.Modulo, _tokens[_index], p);
            else
                return p;

            _index++;
            if (_index >= _tokens.Count)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Missing operand");

            Node f = Factor();
            if (f == null)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Invalid operand");

            op.Childs.Add(f);
            return op;
        }
        
        // T -> F [ ( + | - ) T ]
        private Node Terme()
        {
            if (_index >= _tokens.Count)
                return null;

            Node f = Factor();
            if (f == null)
                return null;

            if (_index >= _tokens.Count)
                return f;

            Node op = null;
            if (_tokens[_index].Category == Tokens.Plus)
                op = new Node(Nodes.Addition, _tokens[_index], f);
            else if (_tokens[_index].Category == Tokens.Minus)
                op = new Node(Nodes.Substraction, _tokens[_index], f);
            else
                return f;

            _index++;
            if (_index >= _tokens.Count)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Missing operand");

            Node t = Terme();
            if (t == null)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Invalid operand");

            op.Childs.Add(t);
            return op;
        }

        // C -> T [ ( == | != | < | > | <= | >= ) C ]
        private Node Comparaison()
        {
            if (_index >= _tokens.Count)
                return null;

            Node t = Terme();
            if (t == null)
                return null;

            if (_index >= _tokens.Count)
                return t;

            Node op = null;
            switch (_tokens[_index].Category)
            {
                case Tokens.Equals:
                    op = new Node(Nodes.AreEqual, _tokens[_index], t);
                    break;

                case Tokens.NotEquals:
                    op = new Node(Nodes.AreNotEqual, _tokens[_index], t);
                    break;

                case Tokens.LowerThan:
                    op = new Node(Nodes.LowerThan, _tokens[_index], t);
                    break;

                case Tokens.GreaterThan:
                    op = new Node(Nodes.GreaterThan, _tokens[_index], t);
                    break;

                case Tokens.LowerOrEqual:
                    op = new Node(Nodes.LowerOrEqual, _tokens[_index], t);
                    break;

                case Tokens.GreaterOrEqual:
                    op = new Node(Nodes.GreaterOrEqual, _tokens[_index], t);
                    break;

                default:
                    return t;
            }

            _index++;
            if (_index >= _tokens.Count)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Missing operand");

            Node c = Comparaison();
            if (c == null)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Invalid operand");

            op.Childs.Add(c);
            return op;
        }

        // L -> C [ && L ]
        private Node Logic()
        {
            if (_index >= _tokens.Count)
                return null;

            Node c = Comparaison();
            if (c == null)
                return null;

            if (_index >= _tokens.Count)
                return c;

            Node op = null;
            if (_tokens[_index].Category == Tokens.And)
                op = new Node(Nodes.And, _tokens[_index], c);
            else
                return c;

            _index++;
            if (_index >= _tokens.Count)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Missing operand");

            Node l = Logic();
            if (l == null)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Invalid operand");

            op.Childs.Add(l);
            return op;
        }

        // E -> L [ || E ]
        private Node Expression()
        {
            if (_index >= _tokens.Count)
                return null;

            Node l = Logic();
            if (l == null)
                return null;

            if (_index >= _tokens.Count)
                return l;

            Node op = null;
            if (_tokens[_index].Category == Tokens.Or)
                op = new Node(Nodes.Or, _tokens[_index], l);
            else
                return l;

            _index++;
            if (_index >= _tokens.Count)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Missing operand");

            Node e = Expression();
            if (e == null)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Invalid operand");

            op.Childs.Add(e);
            return op;
        }
    }
}
