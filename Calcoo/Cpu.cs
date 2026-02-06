using System;

namespace Calcoo
{
    public interface ICpuOutput
    {
        double X { get; }
        ICpuStackGetters GetStack();
        double GetMem(int i);
        int ActiveMemNum { get; }
        IDoubleByDigitGetters GetInput();
        bool IsInputInProgress();
        Settings.AngleUnits AngleUnits { get; }
    }

    public class Cpu : ICpuOutput
    {
        public const double CpuPrecision = 1e-15;

        public enum BinaryOp
        {
            Add,
            Sub,
            Mul,
            Div,
            Pow
        }

        private const int BinopPriorityMin = 0; // used for = and )

        public enum UnaryOp
        {
            // non-trigonometric
            Log10,
            TenToX,
            Ln,
            EtoX,
            Sqrt,
            Sqr,
            InvX,
            Fact,
            // trigonometric
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
            Atanh
        }

        public enum MemoryOp
        {
            MemToX,
            MemPlus,
            ExchXMem,
            XToMem
        }

        // ENTER is after an op or after Enter key in HP-28 mode, EnterPush is after Enter key in Traditional mode
        private enum Action
        {
            Clear,
            Enter,
            EnterPush,
            Input,
            Binop
        }

        private Action _lastAction;

        public bool IsInputInProgress()
        {
            return _lastAction == Action.Input || _lastAction == Action.Clear;
        }

        private Settings.Mode _Mode;

        public Settings.Mode Mode
        {
            get { return _Mode; }
            set
            {
                if (_Mode == value)
                    return;

                ResetRegisters();
                _Mode = value;
                _stack = new CpuStack(value, _stack.StackMode);
            }
        }

        private CpuStack _stack;

        public ICpuStackGetters GetStack()
        {
            return _stack;
        }

        // Input input;
        private DoubleByDigit _input;

        public IDoubleByDigitGetters GetInput()
        {
            return _input;
        }

        public double X { get; private set; }
 
        private readonly int _numBase;

        private readonly double[] _mem;

        public double GetMem(int i)
        {
            if (i > _mem.Length)
                throw new Exception("Requesting the content of memory register " + i + " while the max allowed is "
                                    + _mem.Length);
            return _mem[i];
        }

        public int ActiveMemNum { get; private set; }

        public Settings.AngleUnits AngleUnits { get; private set; }

        public Settings.EnterMode EnterMode { get; set; }

        public Settings.StackMode StackMode
        {
            get { return _stack.StackMode; }
            set { _stack.StackMode = value; }
        }

        private readonly int _inputLength;
        private readonly int _expInputLength;

        private enum InputField
        {
            Int,
            Frac,
            Exp
        }

        private InputField _inputField;

        private Cpu()
        {
            throw new Exception("Instantiating CPU without specifying the mode - RPN or Algebraic");
        }

        public Cpu(Settings.Mode mode,
            Settings.AngleUnits angleUnits,
            int inputLength,
            int expInputLength,
            int numBase,
            int nMem,
            Settings.EnterMode enterMode,
            Settings.StackMode stackMode)
        {
            // setting the s
            _numBase = numBase;

            // creating the structures
            _input = new DoubleByDigit();
            _inputLength = inputLength;
            _expInputLength = expInputLength;
            _inputField = InputField.Int;
            _stack = new CpuStack(mode, stackMode);
            _mem = new double[nMem];

            // setting the current settings
            Mode = mode;
            AngleUnits = angleUnits;
            EnterMode = enterMode;

            // resetting variables to the default state
            ActiveMemNum = 0;
            ResetRegisters();
            X = 0.0;
        }

        public Cpu Clone()
        {
            var clonedCpu = new Cpu(Mode, AngleUnits, _inputLength, _expInputLength, _numBase, _mem.Length, EnterMode,
                _stack.StackMode);
            clonedCpu._lastAction = _lastAction;
            clonedCpu.ActiveMemNum = ActiveMemNum;
            clonedCpu.X = X;
            for (int m = 0; m < _mem.Length; ++m)
                clonedCpu._mem[m] = _mem[m];
            clonedCpu._stack = _stack.Clone();
            clonedCpu._input = _input.Clone();
            clonedCpu._inputField = _inputField;
            return clonedCpu;
        }

        private void ResetRegisters()
        {
            _stack.Clear();
            _lastAction = Action.Clear;
            X = 0.0;
            _input.Clear();
        }

        public void Execute(Command command)
        {
            switch (command)
            {
                case Command.Digit0:
                case Command.Digit1:
                case Command.Digit2:
                case Command.Digit3:
                case Command.Digit4:
                case Command.Digit5:
                case Command.Digit6:
                case Command.Digit7:
                case Command.Digit8:
                case Command.Digit9:
                    ExecuteDigit(CommandToDigit(command));
                    break;
                case Command.Add:
                case Command.Sub:
                case Command.Mul:
                case Command.Div:
                case Command.Pow:
                    ExecuteBinaryOp(CommandToBinaryOp(command));
                    break;
                case Command.MemToX:
                case Command.MemPlus:
                case Command.ExchXMem:
                case Command.XToMem:
                    ExecuteMemoryOp(CommandToMemoryOp(command));
                    break;
                case Command.Mem0:
                case Command.Mem1:
                    ExecuteSwitchToMem(CommandToMemIndex(command));
                    break;
                case Command.Log10:
                case Command.TenToX:
                case Command.Ln:
                case Command.EtoX:
                case Command.Sqrt:
                case Command.Sqr:
                case Command.InvX:
                case Command.Fact:
                case Command.Sin:
                case Command.Cos:
                case Command.Tan:
                case Command.Asin:
                case Command.Acos:
                case Command.Atan:
                case Command.Sinh:
                case Command.Cosh:
                case Command.Tanh:
                case Command.Asinh:
                case Command.Acosh:
                case Command.Atanh:
                    ExecuteUnaryOp(CommandToUnaryOp(command));
                    break;
                case Command.Eq:
                    ExecuteEq();
                    break;
                case Command.Enter:
                    ExecuteEnter();
                    break;
                case Command.Dot:
                    ExecuteDot();
                    break;
                case Command.MantissaSign:
                    ExecuteMantissaSign();
                    break;
                case Command.Exp:
                    ExecuteExp();
                    break;
                case Command.ExpSign:
                    ExecuteExpSign();
                    break;
                case Command.Sign:
                    ExecuteCurrentSign();
                    break;
                case Command.Pi:
                    ExecutePi();
                    break;
                case Command.ExchXy:
                    ExecuteExchXy();
                    break;
                case Command.StackUp:
                    ExecuteStackUp();
                    break;
                case Command.StackDown:
                    ExecuteStackDown();
                    break;
                case Command.LeftParen:
                    ExecuteLeftParen();
                    break;
                case Command.RightParen:
                    ExecuteRightParen();
                    break;
                case Command.ClearAll:
                    ExecuteClearAll();
                    break;
                case Command.ClearX:
                    ExecuteClearX();
                    break;
                case Command.DegRad:
                    ExecuteDegRad();
                    break;
                default:
                    throw new Exception("unknown command " + command);
            }
        }

        private static BinaryOp CommandToBinaryOp(Command command)
        {
            switch (command)
            {
                case Command.Add:
                    return BinaryOp.Add;
                case Command.Sub:
                    return BinaryOp.Sub;
                case Command.Mul:
                    return BinaryOp.Mul;
                case Command.Div:
                    return BinaryOp.Div;
                case Command.Pow:
                    return BinaryOp.Pow;
                default:
                    throw new Exception("illegal BinaryOp command " + command);
            }
        }

        private static UnaryOp CommandToUnaryOp(Command command)
        {
            switch (command)
            {
                case Command.Log10:
                    return UnaryOp.Log10;
                case Command.TenToX:
                    return UnaryOp.TenToX;
                case Command.Ln:
                    return UnaryOp.Ln;
                case Command.EtoX:
                    return UnaryOp.EtoX;
                case Command.Sqrt:
                    return UnaryOp.Sqrt;
                case Command.Sqr:
                    return UnaryOp.Sqr;
                case Command.InvX:
                    return UnaryOp.InvX;
                case Command.Fact:
                    return UnaryOp.Fact;
                case Command.Sin:
                    return UnaryOp.Sin;
                case Command.Cos:
                    return UnaryOp.Cos;
                case Command.Tan:
                    return UnaryOp.Tan;
                case Command.Asin:
                    return UnaryOp.Asin;
                case Command.Sinh:
                    return UnaryOp.Sinh;
                case Command.Asinh:
                    return UnaryOp.Asinh;
                case Command.Acos:
                    return UnaryOp.Acos;
                case Command.Cosh:
                    return UnaryOp.Cosh;
                case Command.Acosh:
                    return UnaryOp.Acosh;
                case Command.Atan:
                    return UnaryOp.Atan;
                case Command.Tanh:
                    return UnaryOp.Tanh;
                case Command.Atanh:
                    return UnaryOp.Atanh;
                default:
                    throw new Exception("illegal UnaryOp command " + command);
            }
        }

        private static MemoryOp CommandToMemoryOp(Command command)
        {
            switch (command)
            {
                case Command.MemToX:
                    return MemoryOp.MemToX;
                case Command.MemPlus:
                    return MemoryOp.MemPlus;
                case Command.ExchXMem:
                    return MemoryOp.ExchXMem;
                case Command.XToMem:
                    return MemoryOp.XToMem;
                default:
                    throw new Exception("illegal MemoryOp command " + command);
            }
        }

        private int CommandToDigit(Command command)
        {
            switch (command)
            {
                case Command.Digit0:
                    return 0;
                case Command.Digit1:
                    return 1;
                case Command.Digit2:
                    return 2;
                case Command.Digit3:
                    return 3;
                case Command.Digit4:
                    return 4;
                case Command.Digit5:
                    return 5;
                case Command.Digit6:
                    return 6;
                case Command.Digit7:
                    return 7;
                case Command.Digit8:
                    return 8;
                case Command.Digit9:
                    return 9;
                default:
                    throw new Exception("illegal digit command " + command.ToString());
            }
        }

        private int CommandToMemIndex(Command command)
        {
            switch (command)
            {
                case Command.Mem0:
                    return 0;
                case Command.Mem1:
                    return 1;
                default:
                    throw new Exception("illegal mem index command " + command);
            }
        }

        // number input

        private void ExecuteDigit(int digitEntered)
        {
            switch (Mode)
            {
                case Settings.Mode.Alg:
                    switch (_lastAction)
                    {
                        case Action.Clear:
                        case Action.Enter:
                        case Action.Binop:
                            _input.Clear();
                            _inputField = InputField.Int;
                            break;
                        case Action.EnterPush:
                            throw new Exception("the cpu's last action may not be EnterPush in ALG mode");
                        case Action.Input:
                            break;
                    }
                    break;
                case Settings.Mode.Rpn:
                    switch (_lastAction)
                    {
                        case Action.Clear:
                            // no need to call cpu_to_output, because input routine works
                            // directly with the main display, and the other displays must
                            // have been taken care of before
                            _input.Clear();
                            _inputField = InputField.Int;
                            break;
                        case Action.Enter:
                            // this action type is set by the <Enter> button in the
                            // HP-28 enter mode, and by any operation that puts
                            // something into X, like Pi, M->x
                            _stack.Push(X);
                            _input.Clear();
                            _inputField = InputField.Int;
                            break;
                        case Action.EnterPush:
                            // this action type is set by the <Enter> button in the
                            // traditional enter mode
                            _input.Clear();
                            _inputField = InputField.Int;
                            break;
                        case Action.Input:
                            break;
                        case Action.Binop:
                            throw new Exception("the cpu's last action may not be Binop in RPN mode");
                    }
                    break;
                default:
                    throw new Exception("unknown cpu mode " + Mode);
            }

            switch (_inputField)
            {
                case InputField.Int:
                    if (_input.GetNIntDigits() < _inputLength)
                    {
                        if (digitEntered != 0 || _input.GetNIntDigits() != 0)
                            _input.AddIntDigit(digitEntered);
                    }
                    break;
                case InputField.Frac:
                    if (_input.GetNIntDigits() + _input.GetNFracDigits() < _inputLength)
                        _input.AddFracDigit(digitEntered);
                    break;
                case InputField.Exp:
                    _input.AddExpDigit(digitEntered, _expInputLength);
                    break;
                default:
                    throw new Exception("invalid current input field " + _inputField);
            }

            _lastAction = Action.Input;
        }

        private static double AngleToRad(double a,
            Settings.AngleUnits units)
        {
            switch (units)
            {
                case Settings.AngleUnits.Rad:
                    return a;
                case Settings.AngleUnits.Deg:
                    return a*Math.PI/180.0;
                default:
                    throw new Exception("unknown angle units " + units);
            }
        }

        private static double AngleFromRad(double a,
            Settings.AngleUnits units)
        {
            switch (units)
            {
                case Settings.AngleUnits.Rad:
                    return a;
                case Settings.AngleUnits.Deg:
                    return a*180.0/Math.PI;
                default:
                    throw new Exception("unknown angle units " + units);
            }
        }

        private void ExecuteDot()
        {
            if (_lastAction != Action.Input)
            {
                if (Mode == Settings.Mode.Rpn && _lastAction == Action.Enter)
                    _stack.Push(X);
                _input.Clear();
                _lastAction = Action.Input;
                _inputField = InputField.Int;
            }

            if (_inputField == InputField.Int)
            {
                if (_input.GetNIntDigits() == 0)
                    _input.AddIntDigit(0);
                _inputField = InputField.Frac;
            }
        }

        private void ExecuteMantissaSign()
        {
            if (IsInputInProgress())
                _input.InverseSign();
            else
                X = -X; // since sign change does not toggle INPUT_IN_PROGRESS
        }

        private void ExecuteExp()
        {
            if (_lastAction != Action.Input)
            {
                if (Mode == Settings.Mode.Rpn && _lastAction == Action.Enter)
                    _stack.Push(X);
                _input.Clear();
                _input.AddIntDigit(1);
                _lastAction = Action.Input;
                _inputField = InputField.Int;
            }

            if (_inputField != InputField.Exp)
            {
                _inputField = InputField.Exp;
                _input.AddExpDigit(0, _expInputLength);
            }
        }

        private void ExecuteExpSign()
        {
            if (_inputField != InputField.Exp || _lastAction != Action.Input)
                return;
            _input.InverseExpSign();
        }

        private void ExecuteCurrentSign()
        {
            if (IsInputInProgress())
                if (_inputField != InputField.Exp)
                    _input.InverseSign();
                else
                    _input.InverseExpSign();
            else
                X = -X; // since sign change does not toggle INPUT_IN_PROGRESS
        }

        private void ExecuteClearAll()
        {
            ResetRegisters();
        }

        private void ExecuteClearX()
        {
            _lastAction = Action.Clear;
            X = 0.0;
            _input.Clear();
        }

        private void ExecutePi()
        {
            if (Mode == Settings.Mode.Rpn)
            {
                if (_lastAction == Action.Input)
                    X = _input.ToDouble(_numBase);
                _stack.Push(X);
            }
            X = Math.PI;
            _lastAction = Action.Enter;
        }

        public void ExecutePaste(double z)
        {
            if (Mode == Settings.Mode.Rpn)
            {
                if (_lastAction == Action.Input)
                    X = _input.ToDouble(_numBase);
                _stack.Push(X);
            }
            X = z;
            _lastAction = Action.Enter;
        }

        // number operations

        private void ExecuteBinaryOp(BinaryOp binaryOp)
        {
            if (_lastAction == Action.Input)
                X = _input.ToDouble(10);

            switch (Mode)
            {
                case Settings.Mode.Rpn:
                    double y = _stack.Pop();
                    X = ComputeBinaryOp(y, X, binaryOp, _numBase);
                    _lastAction = Action.Enter; // no need to use ACTION_BINOP in RPN
                    break;
                case Settings.Mode.Alg:
                    if (!_stack.IsEmpty()
                        && BinaryOpPriority(_stack.GetOp()) >= BinaryOpPriority(binaryOp))
                        DoBinaryOpChain(BinaryOpPriority(binaryOp), false);
                    _stack.Push(X, binaryOp);
                    _lastAction = Action.Binop;
                    break;
                default:
                    throw new Exception("unknown cpu mode " + Mode);
            }
        }

        private static int BinaryOpPriority(BinaryOp binaryOp)
        {
            switch (binaryOp)
            {
                case BinaryOp.Add:
                case BinaryOp.Sub:
                    return BinopPriorityMin + 1;
                case BinaryOp.Mul:
                case BinaryOp.Div:
                    return BinopPriorityMin + 2;
                case BinaryOp.Pow:
                    return BinopPriorityMin + 3;
                default:
                    throw new Exception("unknown binaryOp " + binaryOp);
            }
        }

        private void DoBinaryOpChain(int initPriority,
            bool parenClosed)
        {
            if (Mode == Settings.Mode.Rpn)
                throw new Exception("DoBinaryOpChain called in RPN mode");
            // aux function; performs the chain of binary operations from the stack,
            // stopping the chain at a paren or at a lower-priority operation
            if (parenClosed)
            {
                // if paren_closed == true, it means there are open parens
                // currently open
                if (_stack.HeadParenExists())
                {
                    // handling the funny case of just one number in parens,
                    // like 2+(3)
                    _stack.HeadParenRemove();
                }
                else
                {
                    while (!_stack.HeadParenExists())
                    {
                        X = ComputeBinaryOp(_stack.GetValue(), X, _stack.GetOp(), _numBase);
                        _stack.Pop();
                    }
                    _stack.HeadParenRemove();
                }
            }
            else
            {
                while (!_stack.IsEmpty()
                       && (
                           (BinaryOpPriority(_stack.GetOp()) >= initPriority && !_stack.HeadParenExists())
                               // "=" effectively closes all parens
                           || initPriority == BinopPriorityMin
                           ))
                {
                    X = ComputeBinaryOp(_stack.GetValue(), X, _stack.GetOp(), _numBase);
                    _stack.Pop();
                }
            }
        }

        private static double ComputeBinaryOp(double a,
            double b,
            BinaryOp binaryOp,
            int baseForPrecision)
        {
            switch (binaryOp)
            {
                case BinaryOp.Add:
                    return MathUtil.SmartSum(a, b, baseForPrecision);
                case BinaryOp.Sub:
                    return MathUtil.SmartSum(a, -b, baseForPrecision);
                case BinaryOp.Mul:
                    return a*b;
                case BinaryOp.Div:
                    if (b != 0.0)
                        return a/b;
                    else
                        return Double.NaN;
                case BinaryOp.Pow:
                    if ((a == 0.0 && b <= 0.0) || (a < 0 && b != Math.Floor(b)))
                        return Double.NaN;
                    else
                        return Math.Pow(a, b);
                default:
                    throw new Exception("unknown binaryOp " + binaryOp);
            }
        }

        private void ExecuteEq()
        {
            switch (Mode)
            {
                case Settings.Mode.Rpn:
                    throw new Exception("ExecuteEq called in RPN mode");
                case Settings.Mode.Alg:
                    if (_lastAction == Action.Input)
                        X = _input.ToDouble(10);
                    DoBinaryOpChain(BinopPriorityMin, false);
                    _stack.Clear();
                    _lastAction = Action.Enter;
                    break;
                default:
                    throw new Exception("unknown cpu mode " + Mode);
            }
        }

        private void ExecuteEnter()
        {
            switch (Mode)
            {
                case Settings.Mode.Alg:
                    throw new Exception("ExecuteEnter called in ALG mode");
                case Settings.Mode.Rpn:
                    switch (EnterMode)
                    {
                        case Settings.EnterMode.Traditional:
                            if (_lastAction == Action.Input)
                                X = _input.ToDouble(10);
                            _stack.Push(X);
                            _lastAction = Action.EnterPush;
                            break;
                        case Settings.EnterMode.Hp28:
                            if (_lastAction == Action.Input)
                                X = _input.ToDouble(10);
                            else
                                _stack.Push(X);
                            _lastAction = Action.Enter;
                            break;
                        default:
                            throw new Exception("unknown enter mode " + EnterMode);
                    }
                    break;
                default:
                    throw new Exception("unknown cpu mode " + Mode);
            }
        }

        private void ExecuteExchXy()
        {
            if (_lastAction == Action.Input)
                X = _input.ToDouble(10);
            X = _stack.SwapHeadValue(X);
            _lastAction = Action.Enter;
        }

        private void ExecuteStackUp()
        {
            if (_lastAction == Action.Input)
                X = _input.ToDouble(10);
            X = _stack.RollUp(X);
            _lastAction = Action.Enter;
        }

        private void ExecuteStackDown()
        {
            if (_lastAction == Action.Input)
                X = _input.ToDouble(10);
            X = _stack.RollDown(X);
            _lastAction = Action.Enter;
        }

        private void ExecuteLeftParen()
        {
            if (Mode == Settings.Mode.Rpn)
                throw new Exception("cannot be called in RPN mode");

            if (_lastAction == Action.Binop)
                _stack.HeadParenAdd();
        }

        private void ExecuteRightParen()
        {
            if (Mode == Settings.Mode.Rpn)
                throw new Exception("cannot be called in RPN mode");

            if (_lastAction == Action.Input)
                X = _input.ToDouble(10);

            if (_stack.ExistOpenParen())
            {
                if (!_stack.IsEmpty())
                    DoBinaryOpChain(BinopPriorityMin, true);
            }
            else
            {
                // If an expression starts with an opening paren, it will be ignored
                // by cpu. However, the overall behavior will be adequate (as though
                // the opening paren was interpreted as a paren. This is because the
                // closing paren is made to act as "=". Test case: (2+3)/4
                DoBinaryOpChain(BinopPriorityMin, false);
            }

            _lastAction = Action.Enter;
        }

        private void ExecuteUnaryOp(UnaryOp unaryOp)
        {
            if (_lastAction == Action.Input)
                X = _input.ToDouble(10);

            switch (unaryOp)
            {
                case UnaryOp.Log10:
                    if (X > 0.0)
                        X = Math.Log10(X);
                    else
                        X = Double.NaN;
                    break;
                case UnaryOp.TenToX:
                    if (X < Math.Pow(10.0, _expInputLength))
                        X = Math.Pow(10.0, X);
                    else
                        X = Double.NaN;
                    break;
                case UnaryOp.Ln:
                    if (X > 0.0)
                        X = Math.Log(X);
                    else
                        X = Double.NaN;
                    break;
                case UnaryOp.EtoX:
                    if (X*Math.Log10(Math.E) < Math.Pow(10.0, _expInputLength))
                        X = Math.Exp(X);
                    else
                        X = Double.NaN;
                    break;
                case UnaryOp.Sqrt:
                    if (X >= 0.0)
                        X = Math.Sqrt(X);
                    else
                        X = Double.NaN;
                    break;
                case UnaryOp.Sqr:
                    X = X*X;
                    break;
                case UnaryOp.InvX:
                    if (X != 0.0)
                        X = 1.0/X;
                    else
                        X = Double.NaN;
                    break;
                case UnaryOp.Fact:
                    if (MathUtil.FactCanDo(X, _expInputLength))
                        X = MathUtil.Fact(X, _inputLength);
                    else
                        X = Double.NaN;
                    break;
                case UnaryOp.Sin:
                    X = AngleToRad(X, AngleUnits);
                    if (Math.Abs(X%Math.PI) < CpuPrecision*X)
                        // to have sin 180 == 0;
                        // the condition may be a bit too aggressive
                        X = 0.0;
                    else
                        X = Math.Sin(X);
                    break;
                case UnaryOp.Asin:
                    if (Math.Abs(X) <= 1.0)
                    {
                        X = Math.Asin(X);
                        X = AngleFromRad(X, AngleUnits);
                    }
                    else
                        X = Double.NaN;
                    break;
                case UnaryOp.Sinh:
                    X = Math.Sinh(X);
                    break;
                case UnaryOp.Asinh:
                    X = MathUtil.Asinh(X);
                    break;
                case UnaryOp.Cos:
                    X = AngleToRad(X, AngleUnits);
                    if (Math.Abs((X - Math.PI/2.0)%Math.PI) < CpuPrecision*X)
                        // to have cos 90 == 0;
                        // the condition may be a bit too aggressive
                        X = 0.0;
                    else
                        X = Math.Cos(X);
                    break;
                case UnaryOp.Acos:
                    if (Math.Abs(X) <= 1.0)
                    {
                        X = Math.Acos(X);
                        X = AngleFromRad(X, AngleUnits);
                    }
                    else
                        X = Double.NaN;
                    break;
                case UnaryOp.Cosh:
                    X = Math.Cosh(X);
                    break;
                case UnaryOp.Acosh:
                    if (X >= 1.0)
                        X = MathUtil.Acosh(X);
                    else
                        X = Double.NaN;
                    break;
                case UnaryOp.Tan:
                    X = AngleToRad(X, AngleUnits);
                    if (Math.Abs((X - Math.PI/2.0)%Math.PI) < CpuPrecision*X)
                        // to have tan 90 -> overflow;
                        // the condition may be a bit too aggressive
                        X = Double.NaN;
                    else if (Math.Abs(X%Math.PI) < CpuPrecision*X)
                        // to have tan 180 == 0;
                        // the condition may be a bit too aggressive
                        X = 0.0;
                    else
                        X = Math.Tan(X);
                    break;
                case UnaryOp.Atan:
                    X = Math.Atan(X);
                    X = AngleFromRad(X, AngleUnits);
                    break;
                case UnaryOp.Tanh:
                    X = Math.Tanh(X);
                    break;
                case UnaryOp.Atanh:
                    X = MathUtil.Atanh(X);
                    break;
                default:
                    throw new Exception("unknown unary op " + unaryOp);
            }

            _lastAction = Action.Enter;
        }

        private void ExecuteMemoryOp(MemoryOp memoryOp)
        {
            if (_lastAction == Action.Input)
                X = _input.ToDouble(_numBase);

            switch (memoryOp)
            {
                case MemoryOp.MemPlus:
                    _mem[ActiveMemNum] = MathUtil.SmartSum(X, _mem[ActiveMemNum], _numBase);
                    break;
                case MemoryOp.MemToX:
                    switch (Mode)
                    {
                        case Settings.Mode.Rpn:
                            _stack.Push(X);
                            break;
                        case Settings.Mode.Alg:
                            break;
                        default:
                            throw new Exception("unknown cpu mode " + Mode);
                    }

                    X = _mem[ActiveMemNum];
                    break;
                case MemoryOp.XToMem:
                    _mem[ActiveMemNum] = X;
                    break;
                case MemoryOp.ExchXMem:
                    double tmp = X;
                    X = _mem[ActiveMemNum];
                    _mem[ActiveMemNum] = tmp;
                    break;
                default:
                    throw new Exception("unknown memory op " + memoryOp);
            }

            _lastAction = Action.Enter;
        }

        private void ExecuteSwitchToMem(int memIndex)
        {
            ActiveMemNum = memIndex;
        }

        private void ExecuteDegRad()
        {
            switch (AngleUnits)
            {
                case Settings.AngleUnits.Deg:
                    AngleUnits = Settings.AngleUnits.Rad;
                    break;
                case Settings.AngleUnits.Rad:
                    AngleUnits = Settings.AngleUnits.Deg;
                    break;
                default:
                    throw new Exception("unknown current angle units " + AngleUnits);
            }
        }
    }
}
