using System;

namespace MSMEmulator
{
    public enum Instruction : byte
    {
        Halt,
        Drop, Dup, Pushi, Pushf,
        Get, Set, Read, Write,
        Addi, Subi, Muli, Divi,
        Modi, Addf, Subf, Mulf,
        Divf, Not, And, Or,
        Itof, Ftoi, Cmpeqi, Cmpnei,
        Cmplti, Cmplei, Cmpgti, Cmpgei,
        Cmpeqf, Cmpnef, Cmpltf, Cmplef,
        Cmpgtf, Cmpgef, Jump, Jumpt,
        Jumpf, Prep, Call, Ret,
        Outi, Outf, Outc
    }

    public class MSMEmulator
    {
        private Tuple<Instruction, int>[] _memory;
        private long _programIndex;
        private long _stackIndex;

        public MSMEmulator(long memorySize = 2^16)
        {
            _memory = new Tuple<Instruction, int>[memorySize];
            _programIndex = 0;
            _stackIndex = _memory.Length - 1;
        }

        public void Load(string asm)
        {
            string[] lines = asm.Split('\n');
            foreach (var line in lines)
            {
                string[] commentSplit = line.Split(';');
                if (commentSplit.Length > 0)
                {
                    string instruction = commentSplit[0];
                    string[] keywordSplit = instruction.Split(' ');
                    if (keywordSplit.Length > 0)
                    {
                        string keyword = keywordSplit[0];

                        switch (keyword)
                        {
                            case "halt":
                                _memory[_programIndex++] = new Tuple<Instruction, int>(Instruction.Halt, 0);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public string Run()
        {
            return null;
        }
    }
}
