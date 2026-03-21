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
        public static readonly Command[] Mem = { Command.Mem0, Command.Mem1 };

        public static readonly ReadOnlyCollection<Command> RpnOnly =
            new ReadOnlyCollection<Command>(new List<Command> { Command.Enter, Command.StackUp, Command.StackDown });

        public static readonly ReadOnlyCollection<Command> AlgOnly =
            new ReadOnlyCollection<Command>(new List<Command> { Command.Eq, Command.LeftParen, Command.RightParen });

        public static readonly ReadOnlyCollection<Command> TrigBare =
            new ReadOnlyCollection<Command>(new List<Command> { Command.Sin, Command.Cos, Command.Tan });

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

        public const string CustomButtonTooltip = "Custom command";

        private static readonly Dictionary<(Command, bool, bool), Command> _trigDressed = new()
        {
            { (Command.Sin, false, false), Command.Sin   },
            { (Command.Sin, true,  false), Command.Asin  },
            { (Command.Sin, false, true),  Command.Sinh  },
            { (Command.Sin, true,  true),  Command.Asinh },
            { (Command.Cos, false, false), Command.Cos   },
            { (Command.Cos, true,  false), Command.Acos  },
            { (Command.Cos, false, true),  Command.Cosh  },
            { (Command.Cos, true,  true),  Command.Acosh },
            { (Command.Tan, false, false), Command.Tan   },
            { (Command.Tan, true,  false), Command.Atan  },
            { (Command.Tan, false, true),  Command.Tanh  },
            { (Command.Tan, true,  true),  Command.Atanh },
        };

        public static Command TrigBareToDressed(this Command trigFunction, bool arcOn, bool hypOn)
        {
            if (_trigDressed.TryGetValue((trigFunction, arcOn, hypOn), out var result))
                return result;
            throw new Exception("Function " + trigFunction + " is not a bare trig function (sin, cos, tan)");
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
