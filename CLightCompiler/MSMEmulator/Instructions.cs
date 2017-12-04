namespace MSMEmulator
{
    public enum Instructions : short
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
}
