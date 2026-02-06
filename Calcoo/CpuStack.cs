using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcoo
{
    public interface ICpuStackGetters
    {
        double PeekValue(int i);
        Cpu.BinaryOp? PeekOp(int i);
        bool PeekParenExists(int i);
    }

    public class CpuStack : ICpuStackGetters
    {
        public Settings.StackMode StackMode { get; set; }
    
        private readonly Settings.Mode _mode;

        private CpuStack()
        {
            throw new Exception("Instantiating modeless CpuStack");
        }

        public CpuStack(Settings.Mode mode)
        {
            if (mode != Settings.Mode.Alg)
                throw new Exception("stack mode must be specified in non-ALG mode stack construction "
                                    + mode.ToString());
            _stack = new LinkedList<StackElement>();
            _mode = mode;
        }

        public CpuStack(Settings.Mode mode,
            Settings.StackMode stackMode)
        {
            _stack = new LinkedList<StackElement>();
            _mode = mode;
            StackMode = stackMode;
        }

        public CpuStack Clone()
        {
            var clonedStack = new CpuStack(_mode, StackMode);

            foreach (StackElement e in _stack)
                clonedStack._stack.AddLast(e.Clone()); // deep copy of the stack
            return clonedStack;
        }

        private class StackElement
        {
            public double Z; // not readonly becuase may be changed in swap x-y
            public readonly Cpu.BinaryOp Op;
            public int NumberOfParens; // not readonly because may be incremented/decremented

            private StackElement()
            {
                throw new Exception("Instantiating empty StackElement");
            }

            // for Alg mode
            public StackElement(double z,
                Cpu.BinaryOp op,
                int numberOfParens)
            {
                Z = z;
                Op = op;
                NumberOfParens = numberOfParens;
            }

            // for RPN mode
            public StackElement(double z)
            {
                Z = z;
                // FIXME null out _op and _numberOfParens
            }

            public StackElement Clone()
            {
                return new StackElement(Z, Op, NumberOfParens);
            }
        }

        private readonly LinkedList<StackElement> _stack;
        // LinkedList because we need to be able to remove elements from both the beginning and the end

        public void Push(double z,
            Cpu.BinaryOp op)
        {
            if (_mode != Settings.Mode.Alg)
                throw new Exception("Alg stack push called in non-Alg mode " + _mode.ToString());
            _stack.AddFirst(new StackElement(z, op, 0));
        }


        public void Push(double z)
        {
            if (_mode != Settings.Mode.Rpn)
                throw new Exception("RPN stack push called in non-RPN mode");
            _stack.AddFirst(new StackElement(z));
            if (StackMode == Settings.StackMode.Xyzt && _stack.Count() > 3)
                _stack.RemoveLast();
        }

        public double Pop()
        {
            if (!_stack.Any())

                return 0.0;

            double val = _stack.First().Z;
            _stack.RemoveFirst();
            return val;
        }

        public void Clear()
        {
            _stack.Clear();
        }

        public bool ExistOpenParen()
        {
            if (_mode != Settings.Mode.Alg)
                throw new Exception("Alg stack existOpenParen called in non-Alg mode " + _mode.ToString());
            foreach (var stackElement in _stack)
            {
                if (stackElement.NumberOfParens > 0)
                    return true;
            }
            return false;
        }

        public bool IsEmpty()
        {
            return !_stack.Any();
        }

        public Cpu.BinaryOp GetOp()
        {
            if (_mode != Settings.Mode.Alg)
                throw new Exception("Alg stack GetOp called in non-Alg mode " + _mode.ToString());
            return _stack.First().Op;
        }

        private StackElement PeekElement(int i)
        {
            if (i < 0)
                throw new Exception("trying to peek stack at negative depth " + i);
            if (_stack.Count() > i)
            {
                LinkedList<StackElement>.Enumerator e = _stack.GetEnumerator();
                e.MoveNext();
                for (int j = 0; j < i; ++j)
                    e.MoveNext();
                return e.Current;
            }

            return null;
        }

        public double PeekValue(int i)
        {
            if (i < 0)
                throw new Exception("trying to peek value at negative depth " + i);
            if (_stack.Count() > i)
                return PeekElement(i).Z;

            return 0.0;
        }

        public Cpu.BinaryOp? PeekOp(int i)
        {
            if (i < 0)
                throw new Exception("trying to peek op at negative depth " + i);
            if (_stack.Count() > i && _mode == Settings.Mode.Alg)
                return PeekElement(i).Op;

            return null;
        }

        public bool PeekParenExists(int i)
        {
            if (i < 0)
                throw new Exception("trying to peek paren at negative depth " + i);
            if (_stack.Count() > i && _mode == Settings.Mode.Alg)
                return (PeekElement(i).NumberOfParens > 0);

            return false;
        }

        public double GetValue()
        {
            return _stack.First().Z;
        }

        public bool HeadParenExists()
        {
            if (_mode != Settings.Mode.Alg)
                throw new Exception("Alg stack HeadParenExists called in non-Alg mode " + _mode.ToString());
            return _stack.First().NumberOfParens > 0;
        }

        public void HeadParenAdd()
        {
            if (_mode != Settings.Mode.Alg)
                throw new Exception("Alg stack headParenAdd called in non-ALG mode " + _mode.ToString());
            _stack.First().NumberOfParens++;
        }

        public void HeadParenRemove()
        {
            if (_mode != Settings.Mode.Alg)
                throw new Exception("Alg stack headParenRemove called in non-ALG mode " + _mode.ToString());
            if (_stack.First().NumberOfParens == 0)
                throw new Exception("HeadParenRemove when none exist");
            _stack.First().NumberOfParens--;
        }

        public double RollUp(double x)
        {
            switch (_mode)
            {
                case Settings.Mode.Alg:
                    throw new Exception("stack RollUp called in Alg mode");
                case Settings.Mode.Rpn:
                    switch (StackMode)
                    {
                        case Settings.StackMode.Infinite:
                            if (!_stack.Any())
                                return x;

                            Push(x);
                            double val = _stack.Last().Z;
                            _stack.RemoveLast();
                            return val;

                        case Settings.StackMode.Xyzt:
                            // simple popping and re-pushing is less error-prone
                            // Note: Even if the stack had less than three elements,
                            // there will be exactly three elements after the roll.
                            // This does not spoil the functionality of the XYZT stack,
                            // because it is effectively padded with zeros anyway,
                            // see the implementation of Pop()
                            double oldY = Pop();
                            double oldZ = Pop();
                            double oldT = Pop();
                            Push(oldZ);
                            Push(oldY);
                            Push(x);
                            return oldT;

                        default:
                            throw new Exception("unknown stack mode " + StackMode.ToString());
                    }
                default:
                    throw new Exception("unknown cpu mode " + _mode.ToString());
            }
        }

        public double RollDown(double x)
        {
            switch (_mode)
            {
                case Settings.Mode.Alg:
                    throw new Exception("stack RollDown called in Alg mode");
                case Settings.Mode.Rpn:
                    switch (StackMode)
                    {
                        case Settings.StackMode.Infinite:
                            if (!_stack.Any())
                                return x;

                            _stack.AddLast(new StackElement(x));
                            return Pop();

                        case Settings.StackMode.Xyzt:
                            // simple popping and re-pushing is less error-prone
                            // Note: Even if the stack had less than three elements,
                            // there will be exactly three elements after the roll.
                            // This does not spoil the functionality of the XYZT stack,
                            // because it is effectively padded with zeros anyway,
                            // see the implementation of pop()
                            double oldY = Pop();
                            double oldZ = Pop();
                            double oldT = Pop();
                            Push(x);
                            Push(oldT);
                            Push(oldZ);
                            return oldY;
                        default:
                            throw new Exception("unknown stack mode " + StackMode.ToString());
                    }
                default:
                    throw new Exception("unknown cpu mode " + _mode.ToString());
            }
        }

        public double SwapHeadValue(double x)
        {
            switch (_mode)
            {
                case Settings.Mode.Rpn:
                    double varRpn = Pop();
                    Push(x);
                    return varRpn;
                case Settings.Mode.Alg:
                    if (!_stack.Any())
                        return x;

                    double varAlg = _stack.First().Z;
                    _stack.First().Z = x;
                    return varAlg;

                default:
                    throw new Exception("unknown cpu mode " + _mode.ToString());
            }
        }
    }
}

