using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Calcoo
{
    public enum Command
    {
        DegRad,
        Info,
        Settings,
        Copy,
        Paste,
        Sin,
        Cos,
        Tan,
        Asin,
        Acos,
        Atan,
        Sinh,
        Cosh,
        Tanh,
        Asinh,
        Acosh,
        Atanh,
        Arc,
        Hyp,
        Sqr,
        EtoX,
        TenToX,
        Pow,
        Pi,
        Sqrt,
        Ln,
        Log10,
        InvX,
        Fact,
        Digit7,
        Digit4,
        Digit1,
        Digit0,
        Exp,
        Digit8,
        Digit5,
        Digit2,
        Sign,
        MantissaSign,
        ExpSign,
        Custom,
        Digit9,
        Digit6,
        Digit3,
        Dot,
        Format,
        Add,
        Mul,
        ExchXy,
        XToMem,
        Mem0,
        Mem1,
        Sub,
        Div,
        LeftParen,
        StackUp,
        MemToX,
        ClearAll,
        Eq,
        Enter,
        RightParen,
        StackDown,
        MemPlus,
        Undo,
        Redo,
        ClearX,
        ExchXMem,
        Exit
    }

    public static class CommandExtensions
    {
        public static readonly Command[] Mem = {Command.Mem0, Command.Mem1};

        public static readonly ReadOnlyCollection<Command> RpnOnly =
            new ReadOnlyCollection<Command>(new List<Command> {Command.Enter, Command.StackUp, Command.StackDown});

        public static readonly ReadOnlyCollection<Command> AlgOnly =
            new ReadOnlyCollection<Command>(new List<Command> {Command.Eq, Command.LeftParen, Command.RightParen});

        public static readonly ReadOnlyCollection<Command> TrigBare =
            new ReadOnlyCollection<Command>(new List<Command> {Command.Sin, Command.Cos, Command.Tan});

        public static readonly ReadOnlyCollection<Command> TrigAll = new ReadOnlyCollection<Command>(new List<Command>
        {
            Command.Sin,
            Command.Cos,
            Command.Tan,
            Command.Asin,
            Command.Acos,
            Command.Atan,
            Command.Sinh,
            Command.Cosh,
            Command.Tanh,
            Command.Asinh,
            Command.Acosh,
            Command.Atanh
        });

        public static readonly ReadOnlyCollection<Command> InvalidForCustomCommandSequence =
            new ReadOnlyCollection<Command>(new List<Command>
            {
                Command.Custom,
                Command.DegRad,
                Command.Format,
                Command.Undo,
                Command.Redo,
                Command.Info,
                Command.Settings,
                Command.Copy,
                Command.Paste
            });


        public static Command TrigBareToDressed(this Command trigFunction, bool arcOn, bool hypOn)
        {
            switch (trigFunction)
            {
                case Command.Sin:
                    if (arcOn)
                        if (hypOn)
                            return Command.Asinh;
                        else
                            return Command.Asin;
                    else if (hypOn)
                        return Command.Sinh;
                    else
                        return Command.Sin;
                case Command.Cos:
                    if (arcOn)
                        if (hypOn)
                            return Command.Acosh;
                        else
                            return Command.Acos;
                    else if (hypOn)
                        return Command.Cosh;
                    else
                        return Command.Cos;
                case Command.Tan:
                    if (arcOn)
                        if (hypOn)
                            return Command.Atanh;
                        else
                            return Command.Atan;
                    else if (hypOn)
                        return Command.Tanh;
                    else
                        return Command.Tan;
                default:
                    throw new Exception("Function " + trigFunction + " is not a bare trig function (sin, cos, tan)");
            }
        }

        public static bool IsValidButton(this Command function, Settings.Mode mode)
        {
            switch (mode)
            {
                case Settings.Mode.Rpn:
                    return !AlgOnly.Contains(function);
                case Settings.Mode.Alg:
                    return !RpnOnly.Contains(function);
                default:
                    throw new Exception("unknown mode " + mode);
            }
        }
    }
}
