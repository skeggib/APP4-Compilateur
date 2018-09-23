using LexicalAnalysis;
using System.Collections.Generic;

namespace SyntaxAnalysis
{
    public class SyntaxAnalyser : ISyntaxAnalyser
    {
        private IList<Token> _tokensTemp;
        private int _indexTemp;

        private Node _forPostOperation; // Sert a executer le post-action dans une boucle for avant un continue
        
        private Token GetToken(Tokens category)
        {
            return _indexTemp < _tokensTemp.Count && _tokensTemp[_indexTemp].Category == category ? _tokensTemp[_indexTemp] : null;
        }

        private Token MoveToken(Tokens category)
        {
            if (_indexTemp >= _tokensTemp.Count)
                throw new SyntaxException(_tokensTemp[_indexTemp - 1].Offset, "No tokens left");
            if (_tokensTemp[_indexTemp].Category != category)
                throw new SyntaxException(_tokensTemp[_indexTemp].Offset, "Unexpected token");
            return _tokensTemp[_indexTemp++];
        }

        private Token LastToken()
        {
            return _indexTemp > 0 ? _tokensTemp[_indexTemp - 1] : null;
        }

        private bool TokensLeft()
        {
            return _indexTemp < _tokensTemp.Count;
        }

        /// <summary>
        /// Convertit une liste de tokens en arbre
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public Node Convert(IList<Token> tokens)
        {
            _tokensTemp = tokens;
            _indexTemp = 0;

            return Program();
        }

        // Z -> D*
        private Node Program()
        {
            var program = new Node(Nodes.Program, _tokensTemp[0]);

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
            if (GetToken(Tokens.Int) == null)
                return null;

            MoveToken(Tokens.Int);

            if (GetToken(Tokens.Ident) == null)
                throw new SyntaxException(LastToken().Offset, "Expected identifier");

            var declarationFunc = new Node(Nodes.DeclFunc, MoveToken(Tokens.Ident));

            if (GetToken(Tokens.OpeningParenthesis) == null)
                throw new SyntaxException(LastToken().Offset, "Expected '('");

            MoveToken(Tokens.OpeningParenthesis);

            if (GetToken(Tokens.Int) != null)
            {
                MoveToken(Tokens.Int);
                if (GetToken(Tokens.Ident) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected identifier");

                declarationFunc.Tokens.Add(GetToken(Tokens.Ident));
                MoveToken(Tokens.Ident);

                while (GetToken(Tokens.Comma) != null)
                {
                    MoveToken(Tokens.Comma);

                    if (GetToken(Tokens.Int) == null)
                        throw new SyntaxException(LastToken().Offset, "Expected 'int'");

                    MoveToken(Tokens.Int);

                    if (GetToken(Tokens.Ident) == null)
                        throw new SyntaxException(LastToken().Offset, "Expected identifier");
                        
                    declarationFunc.Tokens.Add(MoveToken(Tokens.Ident));
                }
            }

            if (GetToken(Tokens.ClosingParenthesis) == null)
                throw new SyntaxException(LastToken().Offset, "Expected ')'");
            MoveToken(Tokens.ClosingParenthesis);

            var statement = Statement();
            if (statement == null)
                throw new SyntaxException(LastToken().Offset, "Expected statement");

            declarationFunc.Childs.Add(statement);

            return declarationFunc;
        }

        // S -> { S* } | A; | E; | if (E) S (else S)? | int ident; | while(E)S | do S while(E) | for(A; E; A)S | break; | continue; | return E;
        private Node Statement()
        {
            if (!TokensLeft())
                return null;

            Node affectation, expression;

            var tokenOpeningBrace = GetToken(Tokens.OpeningBrace);
            if (tokenOpeningBrace != null)
            {
                MoveToken(Tokens.OpeningBrace);
                var block = new Node(Nodes.Block, tokenOpeningBrace);

                Node statement;
                do {
                    statement = Statement();
                    if (statement != null)
                        block.Childs.Add(statement);
                } while (GetToken(Tokens.ClosingBrace) == null && statement != null);
                if (GetToken(Tokens.ClosingBrace) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected '}'");
                MoveToken(Tokens.ClosingBrace);
                return block;
            }

            if ((affectation = Affectation()) != null) {
                if (GetToken(Tokens.Semicolon) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected ';'");
                MoveToken(Tokens.Semicolon);
                return affectation;
            }

            if ((expression = Expression()) != null)
            {
                var semicolon = GetToken(Tokens.Semicolon);
                if (semicolon == null)
                    throw new SyntaxException(LastToken().Offset, "Expected ';'");

                MoveToken(Tokens.Semicolon);

                var drop = new Node(Nodes.Drop, semicolon, expression);
                return drop;
            }

            var tokenOut = GetToken(Tokens.Out);
            if (tokenOut != null)
            {
                MoveToken(Tokens.Out);
                if ((expression = Expression()) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected expression");
                if (GetToken(Tokens.Semicolon) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected ';'");
                MoveToken(Tokens.Semicolon);

                var nodeOut = new Node(Nodes.Out, tokenOut, expression);
                return nodeOut;
            }

            var tokenIf = GetToken(Tokens.If);
            if (tokenIf != null)
            {
                MoveToken(Tokens.If);
                Node p;
                if (GetToken(Tokens.OpeningParenthesis) == null || (p = Primary()) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected primary");

                var sIf = Statement();
                if (sIf == null || !TokensLeft())
                    throw new SyntaxException(LastToken().Offset, "Expected statement");

                var cond = new Node(Nodes.Condition, tokenIf, p, sIf);

                if (GetToken(Tokens.Else) != null)
                {
                    MoveToken(Tokens.Else);
                    Node sElse = Statement();
                    if (sElse == null || !TokensLeft())
                        throw new SyntaxException(LastToken().Offset, "Expected statement");

                    cond.Childs.Add(sElse);
                }

                return cond;
            }

            if (GetToken(Tokens.Int) != null)
            {
                MoveToken(Tokens.Int);
                
                Token ident = GetToken(Tokens.Ident);
                if (ident == null)
                    throw new SyntaxException(LastToken().Offset, "Expected identifier");
                MoveToken(Tokens.Ident);
                if (GetToken(Tokens.Semicolon) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected ';'");
                MoveToken(Tokens.Semicolon);

                return new Node(Nodes.DeclVar, ident);
            }

            var tokenWhile = GetToken(Tokens.While);
            if (tokenWhile != null)
            {
                MoveToken(Tokens.While);
                Node primary;
                if (GetToken(Tokens.OpeningBrace) == null || (primary = Primary()) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected primary");

                var s = Statement();
                if (s == null || !TokensLeft())
                    throw new SyntaxException(LastToken().Offset, "Expected while body");

                var breakNode = new Node(Nodes.Break, tokenWhile);
                var cond = new Node(Nodes.Condition, tokenWhile, primary, s, breakNode);
                var whileLoop = new Node(Nodes.Loop, tokenWhile, cond);
                return whileLoop;
            }

            if (GetToken(Tokens.Do) != null)
            {
                MoveToken(Tokens.Do);
                Node s;
                if (!TokensLeft() || (s = Statement()) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected primary");

                var tokenWhile2 = GetToken(Tokens.While);
                if (tokenWhile2 == null)
                    throw new SyntaxException(LastToken().Offset, "Expected 'while'");

                MoveToken(Tokens.While);
                Node p;
                if (GetToken(Tokens.OpeningParenthesis) == null || (p = Primary()) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected primary");

                var breakNode = new Node(Nodes.Break, tokenWhile2);
                var continueNode = new Node(Nodes.Continue, tokenWhile2);
                var cond = new Node(Nodes.Condition, tokenWhile2, p, continueNode, breakNode);
                var blockNode = new Node(Nodes.Block, tokenWhile2, s, cond);
                return new Node(Nodes.Loop, tokenWhile2, blockNode);
            }

            var forToken = GetToken(Tokens.For);
            if (forToken != null)
            {
                MoveToken(Tokens.For);
                if(GetToken(Tokens.OpeningParenthesis) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected opening parenthesis");
                MoveToken(Tokens.OpeningParenthesis);

                var init = Affectation();
                if (init == null)
                    throw new SyntaxException(LastToken().Offset, "Expected initialisateur");

                if (GetToken(Tokens.Semicolon) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected ';'");
                MoveToken(Tokens.Semicolon);

                var expre = Expression();
                if (expre == null)
                    throw new SyntaxException(LastToken().Offset, "Expected expression");

                if (GetToken(Tokens.Semicolon) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected ';'");
                MoveToken(Tokens.Semicolon);

                var post_op = Affectation();
                if (post_op == null)
                    throw new SyntaxException(LastToken().Offset, "Expected initialisateur");

                if (GetToken(Tokens.ClosingParenthesis) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected ')'");
                MoveToken(Tokens.ClosingParenthesis);

                _forPostOperation = post_op;
                var s = Statement();
                if (s == null)
                    throw new SyntaxException(LastToken().Offset, "Expected for body");
                _forPostOperation = null;

                var bodyNode = new Node(Nodes.Block, forToken, s, post_op);
                var condNode = new Node(Nodes.Condition, forToken, expre, bodyNode, new Node(Nodes.Break, forToken));
                var loopNode = new Node(Nodes.Loop, forToken, condNode);

                return new Node(Nodes.Block, forToken, init, loopNode);
            }

            var breakToken = GetToken(Tokens.Break);
            if (breakToken != null)
            {
                MoveToken(Tokens.Break);

                if (GetToken(Tokens.Semicolon) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected ';'");
                MoveToken(Tokens.Semicolon);

                return new Node(Nodes.Break, breakToken);
            }

            var continueToken = GetToken(Tokens.Continue);
            if (continueToken != null)
            {
                MoveToken(Tokens.Continue);

                if (GetToken(Tokens.Semicolon) == null)
                    throw new SyntaxException(LastToken().Offset, "Expected ';'");
                MoveToken(Tokens.Semicolon);
                
                var node = new Node(Nodes.Continue, continueToken);
                if (_forPostOperation != null)
                    node.Childs.Add(_forPostOperation);
                return node;
            }

            var returnToken = GetToken(Tokens.Return);
            if (returnToken != null)
            {
                MoveToken(Tokens.Return);

                var e = Expression();
                if (e == null)
                    throw new SyntaxException(LastToken().Offset, "Expected expression");
                if (GetToken(Tokens.Semicolon) != null)
                {
                    MoveToken(Tokens.Semicolon);
                    return new Node(Nodes.Return, returnToken, e);
                }
                
                throw new SyntaxException(LastToken().Offset, "Expected ';'");
            }

            return null;
        }

        // A -> *ident = E | ident ('['E']')? = E
        private Node Affectation()
        {
            if (!TokensLeft())
                return null;

            if (GetToken(Tokens.Asterisk) != null)
            {
                MoveToken(Tokens.Asterisk);

                var identToken = GetToken(Tokens.Ident);
                if (identToken != null)
                {
                    var indirSet = new Node(Nodes.IndirSet, identToken);
                    MoveToken(Tokens.Ident);

                    if (GetToken(Tokens.Assign) != null)
                    {
                        MoveToken(Tokens.Assign);

                        var expression = Expression();
                        if (expression == null)
                            throw new SyntaxException(LastToken().Offset, "Expected expression");

                        indirSet.Childs.Add(expression);

                        return indirSet;
                    }

                    throw new SyntaxException(LastToken().Offset, "Expected '='");
                }

                throw new SyntaxException(LastToken().Offset, "Expected identifier");
            }

            var identToken2 = GetToken(Tokens.Ident);
            if (identToken2 != null)
            {
                MoveToken(Tokens.Ident);

                Node node;

                if (GetToken(Tokens.OpeningBracket) != null)
                {
                    MoveToken(Tokens.OpeningBracket);

                    var expression = Expression();
                    if (expression == null)
                        throw new SyntaxException(LastToken().Offset, "Expression expected");
                    if (GetToken(Tokens.ClosingBracket) != null)
                    {
                        MoveToken(Tokens.ClosingBracket);
                        node = new Node(Nodes.IndexSet, identToken2, expression);
                    }
                    else
                        throw new SyntaxException(LastToken().Offset, "Expected ']'");
                }

                else if (GetToken(Tokens.Assign) != null)
                    node = new Node(Nodes.Assign, identToken2);

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
