using System;
using System.Collections.Generic;

namespace MSMEmulator
{
    public class MSMEmulator
    {
        private readonly short[] _memory;
        private short _programIndex;
        private short _programStartIndex;
        private short _stackIndex;
        private readonly Dictionary<string, short> _labels;

        public MSMEmulator(string asm = null)
        {
            _memory = new short[short.MaxValue + 1];
            _labels = new Dictionary<string, short>();

            if (asm != null)
                Load(asm);
        }

        public void Load(string asm)
        {
            _programIndex = 0;
            _programStartIndex = -1;

            string[] lines = asm.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.Length > 0 && line[0] == '.')
                {
                    line = line.Trim();
                    string[] commentSplit = line.Split(';');
                    string instruction = commentSplit[0];
                    string[] keywordSplit = instruction.Split(' ');
                    string keyword = keywordSplit[0].Trim();
                    string label = keyword.Substring(1);
                    if (_labels.ContainsKey(label))
                        throw new Exception($"Label {label} already defined");
                    _labels.Add(label, (short)i);
                }
            }

            for (long i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                line = line.Trim();
                string[] commentSplit = line.Split(';');
                if (commentSplit.Length > 0)
                {
                    string instruction = commentSplit[0];
                    string[] keywordSplit = instruction.Split(' ');
                    if (keywordSplit.Length > 0)
                    {
                        var keyword = keywordSplit[0].Trim();
                        short? arg = null;
                        if (keywordSplit.Length > 1)
                            if (short.TryParse(keywordSplit[1].Trim(), out var parsed))
                                arg = parsed;

                        switch (keyword)
                        {
                            case ".start":
                                if (_programStartIndex != -1)
                                    throw new Exception("Multiple .start declarations");
                                _programStartIndex = _programIndex;
                                break;
                            case "halt":
                                _memory[_programIndex++] = (short)Instructions.Halt;
                                _programIndex++;
                                break;
                            case "drop":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "dup":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "push.i":
                                _memory[_programIndex++] = (short)Instructions.Pushi;
                                _memory[_programIndex++] =
                                    arg ?? throw new Exception($"Expected instruction parameter at line {i}");
                                break;
                            case "push.f":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "get":
                                _memory[_programIndex++] = (short) Instructions.Get;
                                _memory[_programIndex++] =
                                    arg ?? throw new Exception($"Expected instruction parameter at line {i}");
                                break;
                            case "set":
                                _memory[_programIndex++] = (short)Instructions.Set;
                                _memory[_programIndex++] =
                                    arg ?? throw new Exception($"Expected instruction parameter at line {i}");
                                break;
                            case "add.i":
                                _memory[_programIndex++] = (short)Instructions.Addi;
                                _programIndex++;
                                break;
                            case "sub.i":
                                _memory[_programIndex++] = (short)Instructions.Subi;
                                _programIndex++;
                                break;
                            case "mul.i":
                                _memory[_programIndex++] = (short)Instructions.Muli;
                                _programIndex++;
                                break;
                            case "div.i":
                                _memory[_programIndex++] = (short)Instructions.Divi;
                                _programIndex++;
                                break;
                            case "mod.i":
                                _memory[_programIndex++] = (short)Instructions.Modi;
                                _programIndex++;
                                break;
                            case "add.f":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "sub.f":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "mul.f":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "div.f":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "not":
                                _memory[_programIndex++] = (short)Instructions.Not;
                                _programIndex++;
                                break;
                            case "and":
                                _memory[_programIndex++] = (short)Instructions.And;
                                _programIndex++;
                                break;
                            case "or":
                                _memory[_programIndex++] = (short)Instructions.Or;
                                _programIndex++;
                                break;
                            case "itof":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "ftoi":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "cmpeq.i":
                                _memory[_programIndex++] = (short)Instructions.Cmpeqi;
                                _programIndex++;
                                break;
                            case "cmpne.i":
                                _memory[_programIndex++] = (short)Instructions.Cmpnei;
                                _programIndex++;
                                break;
                            case "cmplt.i":
                                _memory[_programIndex++] = (short)Instructions.Cmplti;
                                _programIndex++;
                                break;
                            case "cmple.i":
                                _memory[_programIndex++] = (short)Instructions.Cmplei;
                                _programIndex++;
                                break;
                            case "cmpgt.i":
                                _memory[_programIndex++] = (short)Instructions.Cmpgti;
                                _programIndex++;
                                break;
                            case "cmpge.i":
                                _memory[_programIndex++] = (short)Instructions.Cmpgei;
                                _programIndex++;
                                break;
                            case "cmpeq.f":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "cmpne.f":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "cmplt.f":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "cmple.f":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "cmpgt.f":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "cmpge.f":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "jump":
                                {
                                    string label = keywordSplit[1].Trim();
                                    if (!_labels.ContainsKey(label))
                                        throw new Exception($"Unknown label {label} at line {i}");
                                    _memory[_programIndex++] = (short)Instructions.Jump;
                                    _memory[_programIndex++] = _labels[label];
                                }
                                break;
                            case "jumpt":
                                {
                                    string label = keywordSplit[1].Trim();
                                    if (!_labels.ContainsKey(label))
                                        throw new Exception($"Unknown label {label} at line {i}");
                                    _memory[_programIndex++] = (short)Instructions.Jumpt;
                                    _memory[_programIndex++] = _labels[label];
                                }
                                break;
                            case "jumpf":
                                {
                                    string label = keywordSplit[1].Trim();
                                    if (!_labels.ContainsKey(label))
                                        throw new Exception($"Unknown label {label} at line {i}");
                                    _memory[_programIndex++] = (short)Instructions.Jumpf;
                                    _memory[_programIndex++] = _labels[label];
                                }
                                break;
                            case "prep":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "call":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "ret":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "out.i":
                                _memory[_programIndex++] = (short)Instructions.Outi;
                                _programIndex++;
                                break;
                            case "out.f":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            case "out.c":
                                throw new NotImplementedException($"Instruction {keyword} is not implemented");
                            default:
                                if (keyword.Length > 1 && keyword[0] == '.')
                                {
                                    
                                }
                                else if (keyword.Length == 0)
                                {

                                }
                                else
                                    throw new InvalidOperationException($"Instruction {instruction} unknown");
                                break;
                        }
                    }
                }
            }
        }

        public string Run()
        {
            var output = string.Empty;

            _programIndex = 0;
            _stackIndex = (short)(_memory.Length - 1);

            while (_memory[_programIndex] != (short)Instructions.Halt)
            {
                var instruction = (Instructions) _memory[_programIndex];

                short a, b;

                switch (instruction)
                {
                    case Instructions.Drop:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Dup:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Pushi:
                        _memory[_stackIndex--] = _memory[_programIndex + 1];
                        break;
                    case Instructions.Pushf:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Get:
                        _memory[_stackIndex--] = _memory[_memory.Length - 1 + _memory[_programIndex + 1]];
                        break;
                    case Instructions.Set:
                        _memory[_memory.Length - 1 + _memory[_programIndex + 1]] = _memory[++_stackIndex];
                        break;
                    case Instructions.Read:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Write:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Addi:
                        a = _memory[++_stackIndex];
                        b = _memory[++_stackIndex];
                        _memory[_stackIndex--] = (short)(b + a);
                        break;
                    case Instructions.Subi:
                        a = _memory[++_stackIndex];
                        b = _memory[++_stackIndex];
                        _memory[_stackIndex--] = (short)(b - a);
                        break;
                    case Instructions.Muli:
                        a = _memory[++_stackIndex];
                        b = _memory[++_stackIndex];
                        _memory[_stackIndex--] = (short)(b * a);
                        break;
                    case Instructions.Divi:
                        a = _memory[++_stackIndex];
                        b = _memory[++_stackIndex];
                        _memory[_stackIndex--] = (short)(b / a);
                        break;
                    case Instructions.Modi:
                        a = _memory[++_stackIndex];
                        b = _memory[++_stackIndex];
                        _memory[_stackIndex--] = (short)(b % a);
                        break;
                    case Instructions.Addf:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Subf:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Mulf:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Divf:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Not:
                        _memory[_stackIndex--] = _memory[++_stackIndex] == 0 ? (short)1 : (short)0;
                        break;
                    case Instructions.And:
                        a = _memory[++_stackIndex];
                        b = _memory[++_stackIndex];
                        _memory[_stackIndex--] = a != 0 && b != 0 ? (short) 1 : (short) 0;
                        break;
                    case Instructions.Or:
                        a = _memory[++_stackIndex];
                        b = _memory[++_stackIndex];
                        _memory[_stackIndex--] = a != 0 || b != 0 ? (short)1 : (short)0;
                        break;
                    case Instructions.Itof:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Ftoi:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Cmpeqi:
                        break;
                    case Instructions.Cmpnei:
                        break;
                    case Instructions.Cmplti:
                        break;
                    case Instructions.Cmplei:
                        break;
                    case Instructions.Cmpgti:
                        break;
                    case Instructions.Cmpgei:
                        break;
                    case Instructions.Cmpeqf:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Cmpnef:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Cmpltf:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Cmplef:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Cmpgtf:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Cmpgef:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Jump:
                        break;
                    case Instructions.Jumpt:
                        break;
                    case Instructions.Jumpf:
                        break;
                    case Instructions.Prep:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Call:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Ret:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Outi:
                        break;
                    case Instructions.Outf:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    case Instructions.Outc:
                        throw new NotImplementedException($"{instruction} is not implemented");
                    default:
                        throw new Exception($"Unknown instruction");
                }

                _programIndex += 2;
            }

            return output;
        }
    }
}
