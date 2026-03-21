using System;
using System.Collections.Generic;

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
        Settings.AngleUnits CurrentAngleUnits { get; }
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

        private static readonly Dictionary<Settings.AngleUnits, double> HalfCircle = new()
        {
            { Settings.AngleUnits.Deg, 180.0 },
            { Settings.AngleUnits.Rad, Math.PI },
        };

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

        private Settings.Mode _currentMode;

        public Settings.Mode CurrentMode
        {
            get { return _currentMode; }
            set
            {
                if (_currentMode == value)
                    return;

                ResetRegisters();
                _currentMode = value;
                _stack = new CpuStack(value, _stack.CurrentStackMode);
            }
        }

        private CpuStack _stack;

        public ICpuStackGetters GetStack()
        {
            return _stack;
        }

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
            if (i < 0 || i >= _mem.Length)
                throw new Exception("Requesting the content of memory register " + i + " while the max allowed is "
                                    + _mem.Length);
            return _mem[i];
        }

        public int ActiveMemNum { get; private set; }

        public Settings.AngleUnits CurrentAngleUnits { get; private set; }

        public Settings.EnterMode CurrentEnterMode { get; set; }

        public Settings.StackMode CurrentStackMode
        {
            get { return _stack.CurrentStackMode; }
            set { _stack.CurrentStackMode = value; }
        }

        private readonly int _inputLength;
        private readonly int _expInputLength;

        private enum InputField
        {
            Int,
            Frac,
            Exp
        }

        private InputField _currentInputField;

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
            _numBase = numBase;

            // creating the structures
            _input = new DoubleByDigit();
            _inputLength = inputLength;
            _expInputLength = expInputLength;
            _currentInputField = InputField.Int;
            _stack = new CpuStack(mode, stackMode);
            _mem = new double[nMem];

            // setting the current settings
            _currentMode = mode;
            CurrentAngleUnits = angleUnits;
            CurrentEnterMode = enterMode;

            // resetting variables to the default state
            ActiveMemNum = 0;
            ResetRegisters();
            X = 0.0;
        }

        public Cpu Clone()
        {
            var clonedCpu = new Cpu(CurrentMode, CurrentAngleUnits, _inputLength, _expInputLength, _numBase, _mem.Length, CurrentEnterMode,
                _stack.CurrentStackMode);
            clonedCpu._lastAction = _lastAction;
            clonedCpu.ActiveMemNum = ActiveMemNum;
            clonedCpu.X = X;
            for (int m = 0; m < _mem.Length; ++m)
                clonedCpu._mem[m] = _mem[m];
            clonedCpu._stack = _stack.Clone();
            clonedCpu._input = _input.Clone();
            clonedCpu._currentInputField = _currentInputField;
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
                    ExecuteDigit(_commandToDigit[command]);
                    break;
                case Command.Add:
                case Command.Sub:
                case Command.Mul:
                case Command.Div:
                case Command.Pow:
                    ExecuteBinaryOp(_commandToBinaryOp[command]);
                    break;
                case Command.MemToX:
                case Command.MemPlus:
                case Command.ExchXMem:
                case Command.XToMem:
                    ExecuteMemoryOp(command);
                    break;
                case Command.Mem0:
                case Command.Mem1:
                    ExecuteSwitchToMem(_commandToMemIndex[command]);
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
                    ExecuteUnaryOp(command);
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

        private static readonly Dictionary<Command, BinaryOp> _commandToBinaryOp = new()
        {
            { Command.Add, BinaryOp.Add },
            { Command.Sub, BinaryOp.Sub },
            { Command.Mul, BinaryOp.Mul },
            { Command.Div, BinaryOp.Div },
            { Command.Pow, BinaryOp.Pow },
        };

        private static readonly Dictionary<Command, int> _commandToDigit = new()
        {
            { Command.Digit0, 0 },
            { Command.Digit1, 1 },
            { Command.Digit2, 2 },
            { Command.Digit3, 3 },
            { Command.Digit4, 4 },
            { Command.Digit5, 5 },
            { Command.Digit6, 6 },
            { Command.Digit7, 7 },
            { Command.Digit8, 8 },
            { Command.Digit9, 9 },
        };

        private static readonly Dictionary<Command, int> _commandToMemIndex = new()
        {
            { Command.Mem0, 0 },
            { Command.Mem1, 1 },
        };

        // number input

        private void ExecuteDigit(int digitEntered)
        {
            switch (CurrentMode)
            {
                case Settings.Mode.Alg:
                    switch (_lastAction)
                    {
                        case Action.Clear:
                        case Action.Enter:
                        case Action.Binop:
                            _input.Clear();
                            _currentInputField = InputField.Int;
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
                            _currentInputField = InputField.Int;
                            break;
                        case Action.Enter:
                            // this action type is set by the <Enter> button in the
                            // HP-28 enter mode, and by any operation that puts
                            // something into X, like Pi, M->x
                            _stack.Push(X);
                            _input.Clear();
                            _currentInputField = InputField.Int;
                            break;
                        case Action.EnterPush:
                            // this action type is set by the <Enter> button in the
                            // traditional enter mode
                            _input.Clear();
                            _currentInputField = InputField.Int;
                            break;
                        case Action.Input:
                            break;
                        case Action.Binop:
                            throw new Exception("the cpu's last action may not be Binop in RPN mode");
                    }
                    break;
                default:
                    throw new Exception("unknown cpu mode " + CurrentMode);
            }

            switch (_currentInputField)
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
                    throw new Exception("invalid current input field " + _currentInputField);
            }

            _lastAction = Action.Input;
        }

        private static double AngleToRad(double a, Settings.AngleUnits units)
        {
            return a * Math.PI / HalfCircle[units];
        }

        private static double AngleFromRad(double a, Settings.AngleUnits units)
        {
            return a * HalfCircle[units] / Math.PI;
        }

        // Checks whether angle/period is close to an integer.
        // Uses relative epsilon so it works for large angles.
        private static bool IsNearInteger(double ratio)
        {
            return Math.Abs(ratio - Math.Round(ratio)) < CpuPrecision * Math.Max(1.0, Math.Abs(ratio));
        }

        // Checks whether angle is a multiple of half-circle (180° or π).
        private static bool IsMultipleOf(double angle, Settings.AngleUnits units)
        {
            return IsNearInteger(angle / HalfCircle[units]);
        }

        // Checks whether angle is an odd multiple of quarter-circle (90°, 270°, ...).
        private static bool IsOddMultipleOfHalf(double angle, Settings.AngleUnits units)
        {
            double ratio = angle / (HalfCircle[units] / 2.0);
            return IsNearInteger(ratio) && Math.Abs(Math.Round(ratio) % 2.0) > 0.5;
        }

        private void ExecuteDot()
        {
            if (_lastAction != Action.Input)
            {
                if (CurrentMode == Settings.Mode.Rpn && _lastAction == Action.Enter)
                    _stack.Push(X);
                _input.Clear();
                _lastAction = Action.Input;
                _currentInputField = InputField.Int;
            }

            if (_currentInputField == InputField.Int)
            {
                if (_input.GetNIntDigits() == 0)
                    _input.AddIntDigit(0);
                _currentInputField = InputField.Frac;
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
                if (CurrentMode == Settings.Mode.Rpn && _lastAction == Action.Enter)
                    _stack.Push(X);
                _input.Clear();
                _input.AddIntDigit(1);
                _lastAction = Action.Input;
                _currentInputField = InputField.Int;
            }

            if (_currentInputField != InputField.Exp)
            {
                _currentInputField = InputField.Exp;
                _input.AddExpDigit(0, _expInputLength);
            }
        }

        private void ExecuteExpSign()
        {
            if (_currentInputField != InputField.Exp || _lastAction != Action.Input)
                return;
            _input.InverseExpSign();
        }

        private void ExecuteCurrentSign()
        {
            if (IsInputInProgress())
                if (_currentInputField != InputField.Exp)
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
            if (CurrentMode == Settings.Mode.Rpn)
            {
                FinalizeInput();
                _stack.Push(X);
            }
            X = Math.PI;
            _lastAction = Action.Enter;
        }

        public void ExecutePaste(double z)
        {
            if (CurrentMode == Settings.Mode.Rpn)
            {
                FinalizeInput();
                _stack.Push(X);
            }
            X = z;
            _lastAction = Action.Enter;
        }

        // number operations

        private void ExecuteBinaryOp(BinaryOp binaryOp)
        {
            FinalizeInput();

            switch (CurrentMode)
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
                    throw new Exception("unknown cpu mode " + CurrentMode);
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
            if (CurrentMode == Settings.Mode.Rpn)
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
                    return a * b;
                case BinaryOp.Div:
                    if (b != 0.0)
                        return a / b;
                    else
                        return double.NaN;
                case BinaryOp.Pow:
                    if ((a == 0.0 && b <= 0.0) || (a < 0 && b != Math.Floor(b)))
                        return double.NaN;
                    else
                        return Math.Pow(a, b);
                default:
                    throw new Exception("unknown binaryOp " + binaryOp);
            }
        }

        private void ExecuteEq()
        {
            switch (CurrentMode)
            {
                case Settings.Mode.Rpn:
                    throw new Exception("ExecuteEq called in RPN mode");
                case Settings.Mode.Alg:
                    FinalizeInput();
                    DoBinaryOpChain(BinopPriorityMin, false);
                    _stack.Clear();
                    _lastAction = Action.Enter;
                    break;
                default:
                    throw new Exception("unknown cpu mode " + CurrentMode);
            }
        }

        private void ExecuteEnter()
        {
            switch (CurrentMode)
            {
                case Settings.Mode.Alg:
                    throw new Exception("ExecuteEnter called in ALG mode");
                case Settings.Mode.Rpn:
                    switch (CurrentEnterMode)
                    {
                        case Settings.EnterMode.Traditional:
                            FinalizeInput();
                            _stack.Push(X);
                            _lastAction = Action.EnterPush;
                            break;
                        case Settings.EnterMode.Hp28:
                            if (_lastAction == Action.Input)
                                X = _input.ToDouble(_numBase);
                            else
                                _stack.Push(X);
                            _lastAction = Action.Enter;
                            break;
                        default:
                            throw new Exception("unknown enter mode " + CurrentEnterMode);
                    }
                    break;
                default:
                    throw new Exception("unknown cpu mode " + CurrentMode);
            }
        }

        private void ExecuteExchXy()
        {
            FinalizeInput();
            X = _stack.SwapHeadValue(X);
            _lastAction = Action.Enter;
        }

        private void ExecuteStackUp()
        {
            FinalizeInput();
            X = _stack.RollUp(X);
            _lastAction = Action.Enter;
        }

        private void ExecuteStackDown()
        {
            FinalizeInput();
            X = _stack.RollDown(X);
            _lastAction = Action.Enter;
        }

        private void ExecuteLeftParen()
        {
            if (CurrentMode == Settings.Mode.Rpn)
                throw new Exception("cannot be called in RPN mode");

            if (_lastAction == Action.Binop)
                _stack.HeadParenAdd();
        }

        private void ExecuteRightParen()
        {
            if (CurrentMode == Settings.Mode.Rpn)
                throw new Exception("cannot be called in RPN mode");

            FinalizeInput();

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

        private void ExecuteUnaryOp(Command command)
        {
            FinalizeInput();

            switch (command)
            {
                case Command.Log10:
                    if (X > 0.0)
                        X = Math.Log10(X);
                    else
                        X = double.NaN;
                    break;
                case Command.TenToX:
                    if (X < Math.Pow(10.0, _expInputLength))
                        X = Math.Pow(10.0, X);
                    else
                        X = double.NaN;
                    break;
                case Command.Ln:
                    if (X > 0.0)
                        X = Math.Log(X);
                    else
                        X = double.NaN;
                    break;
                case Command.EtoX:
                    if (X * Math.Log10(Math.E) < Math.Pow(10.0, _expInputLength))
                        X = Math.Exp(X);
                    else
                        X = double.NaN;
                    break;
                case Command.Sqrt:
                    if (X >= 0.0)
                        X = Math.Sqrt(X);
                    else
                        X = double.NaN;
                    break;
                case Command.Sqr:
                    X = X * X;
                    break;
                case Command.InvX:
                    if (X != 0.0)
                        X = 1.0 / X;
                    else
                        X = double.NaN;
                    break;
                case Command.Fact:
                    if (MathUtil.FactCanDo(X, _expInputLength))
                        X = MathUtil.Fact(X, _inputLength);
                    else
                        X = double.NaN;
                    break;
                case Command.Sin:
                    if (IsMultipleOf(X, CurrentAngleUnits))
                        X = 0.0;
                    else
                        X = Math.Sin(AngleToRad(X, CurrentAngleUnits));
                    break;
                case Command.Asin:
                    if (Math.Abs(X) <= 1.0)
                    {
                        X = Math.Asin(X);
                        X = AngleFromRad(X, CurrentAngleUnits);
                    }
                    else
                        X = double.NaN;
                    break;
                case Command.Sinh:
                    X = Math.Sinh(X);
                    break;
                case Command.Asinh:
                    X = Math.Asinh(X);
                    break;
                case Command.Cos:
                    if (IsOddMultipleOfHalf(X, CurrentAngleUnits))
                        X = 0.0;
                    else
                        X = Math.Cos(AngleToRad(X, CurrentAngleUnits));
                    break;
                case Command.Acos:
                    if (Math.Abs(X) <= 1.0)
                    {
                        X = Math.Acos(X);
                        X = AngleFromRad(X, CurrentAngleUnits);
                    }
                    else
                        X = double.NaN;
                    break;
                case Command.Cosh:
                    X = Math.Cosh(X);
                    break;
                case Command.Acosh:
                    X = Math.Acosh(X);
                    break;
                case Command.Tan:
                    if (IsOddMultipleOfHalf(X, CurrentAngleUnits))
                        X = double.NaN;
                    else if (IsMultipleOf(X, CurrentAngleUnits))
                        X = 0.0;
                    else
                        X = Math.Tan(AngleToRad(X, CurrentAngleUnits));
                    break;
                case Command.Atan:
                    X = Math.Atan(X);
                    X = AngleFromRad(X, CurrentAngleUnits);
                    break;
                case Command.Tanh:
                    X = Math.Tanh(X);
                    break;
                case Command.Atanh:
                    X = Math.Atanh(X);
                    break;
                default:
                    throw new Exception("unknown unary op " + command);
            }

            _lastAction = Action.Enter;
        }

        private void ExecuteMemoryOp(Command command)
        {
            FinalizeInput();

            switch (command)
            {
                case Command.MemPlus:
                    _mem[ActiveMemNum] = MathUtil.SmartSum(X, _mem[ActiveMemNum], _numBase);
                    break;
                case Command.MemToX:
                    switch (CurrentMode)
                    {
                        case Settings.Mode.Rpn:
                            _stack.Push(X);
                            break;
                        case Settings.Mode.Alg:
                            break;
                        default:
                            throw new Exception("unknown cpu mode " + CurrentMode);
                    }

                    X = _mem[ActiveMemNum];
                    break;
                case Command.XToMem:
                    _mem[ActiveMemNum] = X;
                    break;
                case Command.ExchXMem:
                    double tmp = X;
                    X = _mem[ActiveMemNum];
                    _mem[ActiveMemNum] = tmp;
                    break;
                default:
                    throw new Exception("unknown memory op " + command);
            }

            _lastAction = Action.Enter;
        }

        private void ExecuteSwitchToMem(int memIndex)
        {
            ActiveMemNum = memIndex;
        }

        private void FinalizeInput()
        {
            if (_lastAction == Action.Input)
                X = _input.ToDouble(_numBase);
        }

        private void ExecuteDegRad()
        {
            switch (CurrentAngleUnits)
            {
                case Settings.AngleUnits.Deg:
                    CurrentAngleUnits = Settings.AngleUnits.Rad;
                    break;
                case Settings.AngleUnits.Rad:
                    CurrentAngleUnits = Settings.AngleUnits.Deg;
                    break;
                default:
                    throw new Exception("unknown current angle units " + CurrentAngleUnits);
            }
        }
    }
}
