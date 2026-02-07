using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Calcoo.Test
{
    [TestFixture]
    public class CpuStackTest
    {
        private const double Precision = 1e-15;
        private const Settings.StackMode DefaultStackMode = Settings.StackMode.Infinite;

        [Test]
        public void CloneTest()
        {
            // Alg
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(2.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            CpuStack clonedStack = stack.Clone();

            ClassicAssert.AreEqual(3.0, clonedStack.GetValue(), Precision, "Alg - two pushes - value");
            ClassicAssert.AreEqual(Cpu.BinaryOp.Sub, clonedStack.GetOp(), "Alg - two pushes - op");
            ClassicAssert.AreEqual(false, clonedStack.HeadParenExists(), "Alg - two pushes - paren");
            clonedStack.Pop();
            ClassicAssert.AreEqual(2.0, clonedStack.GetValue(), Precision, "Alg - two pushes, one pop - value");
            ClassicAssert.AreEqual(Cpu.BinaryOp.Div, clonedStack.GetOp(), "Alg - two pushes, one pop - op");
            ClassicAssert.AreEqual(true, clonedStack.HeadParenExists(), "Alg - two pushes, one pop - paren");
            clonedStack.Pop();
            ClassicAssert.AreEqual(true, clonedStack.IsEmpty(), "Alg - two pushes, two pops - empty");
            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            stack.Push(3.0);
            clonedStack = stack.Clone();

            ClassicAssert.AreEqual(3.0, clonedStack.GetValue(), Precision, "RPN - two pushes");
            clonedStack.Pop();
            ClassicAssert.AreEqual(2.0, clonedStack.GetValue(), Precision, "RPN - two pushes, one pop");
            clonedStack.Pop();
            ClassicAssert.AreEqual(true, clonedStack.IsEmpty(), "RPN - two pushes, two pops");
        }

        [Test]
        public void PushTest()
        {
            // normal functioning
            // ------------------------------------------------------
            // Alg
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(2.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "Alg - one push - value");
            ClassicAssert.AreEqual(Cpu.BinaryOp.Div, stack.GetOp(), "Alg - one push - op");
            ClassicAssert.AreEqual(true, stack.HeadParenExists(), "Alg - one push - paren");
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            ClassicAssert.AreEqual(3.0, stack.GetValue(), Precision, "Alg - two pushes - value");
            ClassicAssert.AreEqual(Cpu.BinaryOp.Sub, stack.GetOp(), "Alg - two pushes - op");
            ClassicAssert.AreEqual(false, stack.HeadParenExists(), "Alg - two pushes - paren");
            stack.Pop();
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "Alg - two pushes, one pop - value");
            ClassicAssert.AreEqual(Cpu.BinaryOp.Div, stack.GetOp(), "Alg - two pushes, one pop - op");
            ClassicAssert.AreEqual(true, stack.HeadParenExists(), "Alg - two pushes, one pop - paren");
            // RPN, INFINITE
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(2.0);
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, INFINITE - one push");
            stack.Push(3.0);
            ClassicAssert.AreEqual(3.0, stack.GetValue(), Precision, "RPN, INFINITE - two pushes");
            stack.Push(4.0);
            ClassicAssert.AreEqual(4.0, stack.GetValue(), Precision, "RPN, INFINITE - three pushes");
            stack.Push(5.0);
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, INFINITE - four pushes");
            stack.Pop();
            ClassicAssert.AreEqual(4.0, stack.GetValue(), Precision, "RPN, INFINITE - four pushes, one pop");
            stack.Pop();
            ClassicAssert.AreEqual(3.0, stack.GetValue(), Precision, "RPN, INFINITE - four pushes, two pops");
            stack.Pop();
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, INFINITE - four pushes, three pops");
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - four pushes, four pops");
            // RPN, XYZT
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(2.0);
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, XYZT - one push");
            stack.Push(3.0);
            ClassicAssert.AreEqual(3.0, stack.GetValue(), Precision, "RPN, XYZT - two pushes");
            stack.Push(4.0);
            ClassicAssert.AreEqual(4.0, stack.GetValue(), Precision, "RPN, XYZT - three pushes");
            stack.Push(5.0);
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, XYZT - four pushes");
            stack.Pop();
            ClassicAssert.AreEqual(4.0, stack.GetValue(), Precision, "RPN, XYZT - four pushes, one pop");
            stack.Pop();
            ClassicAssert.AreEqual(3.0, stack.GetValue(), Precision, "RPN, XYZT - four pushes, two pops");
            stack.Pop();
            // the fourth push chopped the tail off
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - four pushes, three pops");
            // testing for invalid inputs
            // ---------------------------------------------------------------
            stack = new CpuStack(Settings.Mode.Alg);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.Push(2.0), "RPN push Alg mode");

            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.Push(2.0, Cpu.BinaryOp.Div),
                " Alg push RPN mode");
        }

        [Test]
        public void PopTest()
        {
            // normal functioning
            // ------------------------------------------------------
            // Alg
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(2.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            ClassicAssert.AreEqual(3.0, stack.Pop(), Precision, "Alg - two pushes, one pop - value");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "Alg - two pushes, one pop - head value");
            ClassicAssert.AreEqual(Cpu.BinaryOp.Div, stack.GetOp(), "Alg - two pushes, one pop - head op");
            ClassicAssert.AreEqual(true, stack.HeadParenExists(), "Alg - two pushes, one pop - head paren");
            ClassicAssert.AreEqual(2.0, stack.Pop(), Precision, "Alg - two pushes, two pops - value");
            // RPN, INFINITE
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(2.0);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(5.0);
            ClassicAssert.AreEqual(5.0, stack.Pop(), Precision, "RPN, INFINITE - four pushes");
            ClassicAssert.AreEqual(4.0, stack.Pop(), Precision, "RPN, INFINITE - four pushes, one pop");
            ClassicAssert.AreEqual(3.0, stack.Pop(), Precision, "RPN, INFINITE - four pushes, two pops");
            ClassicAssert.AreEqual(2.0, stack.Pop(), Precision, "RPN, INFINITE - four pushes, three pops");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - four pushes, four pops");
            ClassicAssert.AreEqual(0.0, stack.Pop(), Precision, "RPN, INFINITE - empty stack pops 0");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - empty stack stays empty after pop");
            // RPN, XYZT
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(2.0);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(5.0);
            ClassicAssert.AreEqual(5.0, stack.Pop(), Precision, "RPN, XYZT - four pushes");
            ClassicAssert.AreEqual(4.0, stack.Pop(), Precision, "RPN, XYZT - four pushes, one pop");
            ClassicAssert.AreEqual(3.0, stack.Pop(), Precision, "RPN, XYZT - four pushes, two pops");
            // the fourth push chopped the tail off
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - four pushes, three pops");
            ClassicAssert.AreEqual(0.0, stack.Pop(), Precision, "RPN, XYZT - empty stack pops 0");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - empty stack stays empty after pop");
        }

        [Test]
        public void ClearTest()
        {
            // ALG
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(2.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            stack.Clear();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "ALG empty after one push, clear");
            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            stack.Clear();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN empty after one push, clear");
        }

        [Test]
        public void ExistOpenParenTest()
        {
            // normal functioning
            var stack = new CpuStack(Settings.Mode.Alg);
            ClassicAssert.AreEqual(false, stack.ExistOpenParen(), "empty stack");
            stack.Push(2.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            ClassicAssert.AreEqual(true, stack.ExistOpenParen(), "paren at head");
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            ClassicAssert.AreEqual(true, stack.ExistOpenParen(), "paren deep");
            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () =>
                stack.ExistOpenParen(), "failed to throw at existOpenParen in RPN mode");
        }

        [Test]
        public void IsEmptyTest()
        {
            // ALG
            var stack = new CpuStack(Settings.Mode.Alg);
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "ALG empty new");
            stack.Push(2.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            ClassicAssert.AreEqual(false, stack.IsEmpty(), "ALG empty after one push");
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "ALG empty after one push, one pop");
            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN empty new");
            stack.Push(2.0);
            ClassicAssert.AreEqual(false, stack.IsEmpty(), "RPN empty after one push");
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN empty after one push, one pop");
        }

        [Test]
        public void GetOpTest()
        {
            // normal functioning
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(2.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            ClassicAssert.AreEqual(Cpu.BinaryOp.Div, stack.GetOp(), "ALG - one push - op");
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            ClassicAssert.AreEqual(Cpu.BinaryOp.Sub, stack.GetOp(), "ALG - two pushes - op");
            stack.Pop();
            ClassicAssert.AreEqual(Cpu.BinaryOp.Div, stack.GetOp(), "ALG - two pushes, one pop - op");
            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Alg);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.GetOp(),
                "failed to throw at getOp of empty stack");


            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.GetOp(), "failed to throw at getOp in RPN mode");
        }

        [Test]
        public void GetValueTest()
        {
            // normal functioning
            // ------------------------------------------------------
            // ALG
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(2.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "ALG - one push");
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            ClassicAssert.AreEqual(3.0, stack.GetValue(), Precision, "ALG - two pushes");
            // testing for invalid inputs
            // ---------------------------------------------------------------
            stack = new CpuStack(Settings.Mode.Alg);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.GetValue(),
                "failed to throw at getValue of empty stack, ALG mode");

            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.GetValue(),
                "failed to throw at getValue of empty stack, RPN mode");
        }

        [Test]
        public void RollUpTest()
        {
            // RPN, INFINITE
            var stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            var x = stack.RollUp(2.0);
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, INFINITE - empty - obtained");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - empty - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            x = stack.RollUp(2.0);
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, INFINITE - one - one roll - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, INFINITE - one - one roll - head");
            x = stack.RollUp(4.0);
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, INFINITE - one - two rolls - obtained");
            ClassicAssert.AreEqual(4.0, stack.GetValue(), Precision, "RPN, INFINITE - one - two rolls - head");
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - one - two rolls and pop - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            stack.Push(4.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 4.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, INFINITE - two - one roll - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, INFINITE - two - one roll - head");
            x = stack.RollUp(5.0); // stack is, head to tail: 5.0 2.0
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, INFINITE - two - two rolls - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, INFINITE - two - two rolls - head");
            x = stack.RollUp(6.0); // stack is, head to tail: 6.0 5.0
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, INFINITE - two - three rolls - obtained");
            ClassicAssert.AreEqual(6.0, stack.GetValue(), Precision, "RPN, INFINITE - two - three rolls - head");
            stack.Pop();
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - two - three rolls and two pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 7.0 4.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, INFINITE - three - one roll - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, INFINITE - three - one roll - head");
            x = stack.RollUp(5.0); // stack is, head to tail: 5.0 2.0 7.0
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, INFINITE - three - two rolls - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, INFINITE - three - two rolls - head");
            x = stack.RollUp(6.0); // stack is, head to tail: 6.0 5.0 2.0
            ClassicAssert.AreEqual(7.0, x, Precision, "RPN, INFINITE - three - three rolls - obtained");
            ClassicAssert.AreEqual(6.0, stack.GetValue(), Precision, "RPN, INFINITE - three - three rolls - head");
            x = stack.RollUp(8.0); // stack is, head to tail: 6.0 5.0 2.0
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, INFINITE - three - four rolls - obtained");
            ClassicAssert.AreEqual(8.0, stack.GetValue(), Precision, "RPN, INFINITE - three - four rolls - head");
            stack.Pop();
            stack.Pop();
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - three - four rolls and three pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            stack.Push(9.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 9.0 7.0 4.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, INFINITE - four - one roll - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, INFINITE - four - one roll - head");
            x = stack.RollUp(5.0); // stack is, head to tail: 5.0 2.0 9.0 7.0
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, INFINITE - four - two rolls - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, INFINITE - four - two rolls - head");
            x = stack.RollUp(6.0); // stack is, head to tail: 6.0 5.0 2.0 9.0
            ClassicAssert.AreEqual(7.0, x, Precision, "RPN, INFINITE - four - three rolls - obtained");
            ClassicAssert.AreEqual(6.0, stack.GetValue(), Precision, "RPN, INFINITE - four - three rolls - head");
            x = stack.RollUp(8.0); // stack is, head to tail: 8.0 6.0 5.0 2.0
            ClassicAssert.AreEqual(9.0, x, Precision, "RPN, INFINITE - four - four rolls - obtained");
            ClassicAssert.AreEqual(8.0, stack.GetValue(), Precision, "RPN, INFINITE - four - four rolls - head");
            x = stack.RollUp(10.0); // stack is, head to tail: 10.0 8.0 6.0 5.0
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, INFINITE - four - five rolls - obtained");
            ClassicAssert.AreEqual(10.0, stack.GetValue(), Precision, "RPN, INFINITE - four - five rolls - head");
            stack.Pop();
            stack.Pop();
            stack.Pop();
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - four - five rolls and four pops - head");

            // RPN, XYZT
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 0.0 0.0
            ClassicAssert.AreEqual(0.0, x, Precision, "RPN, XYZT - empty - one roll - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, XYZT - empty - one roll - head");
            stack.Pop();
            ClassicAssert.AreEqual(0.0, stack.Pop(), Precision, "RPN, XYZT - empty - one roll and one pop - head");
            ClassicAssert.AreEqual(0.0, stack.Pop(), Precision, "RPN, XYZT - empty - one roll and two pops - head");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - empty - one roll and three pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(3.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 3.0 0.0
            ClassicAssert.AreEqual(0.0, x, Precision, "RPN, XYZT - one - one roll - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, XYZT - one - one roll - head");
            x = stack.RollUp(4.0); // stack is, head to tail: 4.0 2.0 3.0
            ClassicAssert.AreEqual(0.0, x, Precision, "RPN, XYZT - one - two rolls - obtained");
            ClassicAssert.AreEqual(4.0, stack.GetValue(), Precision, "RPN, XYZT - one - two rolls - head");
            stack.Pop();
            ClassicAssert.AreEqual(2.0, stack.Pop(), Precision, "RPN, XYZT - one - two rolls and two pops - head");
            ClassicAssert.AreEqual(3.0, stack.Pop(), Precision, "RPN, XYZT - one - two rolls and three pops - head");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - one - two rollsl and four pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(3.0);
            stack.Push(4.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 4.0 3.0
            ClassicAssert.AreEqual(0.0, x, Precision, "RPN, XYZT - two - one roll - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, XYZT - two - one roll - head");
            x = stack.RollUp(5.0); // stack is, head to tail: 5.0 2.0 4.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, XYZT - two - two rolls - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, XYZT - two - two rolls - head");
            x = stack.RollUp(6.0); // stack is, head to tail: 6.0 5.0 2.0
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, XYZT - two - three rolls - obtained");
            ClassicAssert.AreEqual(6.0, stack.GetValue(), Precision, "RPN, XYZT - two - three rolls - head");
            stack.Pop();
            ClassicAssert.AreEqual(5.0, stack.Pop(), Precision, "RPN, XYZT - one - three rolls and two pops - head");
            ClassicAssert.AreEqual(2.0, stack.Pop(), Precision, "RPN, XYZT - one - three rolls and three pops - head");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - two - three rolls and four pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 7.0 4.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, XYZT - three - one roll - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, XYZT - three - one roll - head");
            x = stack.RollUp(5.0); // stack is, head to tail: 5.0 2.0 7.0
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, XYZT - three - two rolls - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, XYZT - three - two rolls - head");
            x = stack.RollUp(6.0); // stack is, head to tail: 6.0 5.0 2.0
            ClassicAssert.AreEqual(7.0, x, Precision, "RPN, XYZT - three - three rolls - obtained");
            ClassicAssert.AreEqual(6.0, stack.GetValue(), Precision, "RPN, XYZT - three - three rolls - head");
            x = stack.RollUp(8.0); // stack is, head to tail: 8.0 6.0 5.0
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, XYZT - three - four rolls - obtained");
            ClassicAssert.AreEqual(8.0, stack.GetValue(), Precision, "RPN, XYZT - three - four rolls - head");
            stack.Pop();
            ClassicAssert.AreEqual(6.0, stack.Pop(), Precision, "RPN, XYZT - one - four rolls and two pops - head");
            ClassicAssert.AreEqual(5.0, stack.Pop(), Precision, "RPN, XYZT - one - four rolls and three pops - head");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - three - four rolls and four pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(9.0); // will be washed away by pushing
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 7.0 4.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, XYZT - three - one roll - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, XYZT - three - one roll - head");
            x = stack.RollUp(5.0); // stack is, head to tail: 5.0 2.0 7.0
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, XYZT - three - two rolls - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, XYZT - three - two rolls - head");
            x = stack.RollUp(6.0); // stack is, head to tail: 6.0 5.0 2.0
            ClassicAssert.AreEqual(7.0, x, Precision, "RPN, XYZT - three - three rolls - obtained");
            ClassicAssert.AreEqual(6.0, stack.GetValue(), Precision, "RPN, XYZT - three - three rolls - head");
            x = stack.RollUp(8.0); // stack is, head to tail: 6.0 5.0 2.0
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, XYZT - three - four rolls - obtained");
            ClassicAssert.AreEqual(8.0, stack.GetValue(), Precision, "RPN, XYZT - three - four rolls - head");
            stack.Pop();
            stack.Pop();
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - three - four rolls and three pops - head");

            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(2.0, Cpu.BinaryOp.Div);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.RollUp(3.0),
                "failed to throw at rollUp in ALG mode");
        }

        [Test]
        public void RollDownTest()
        {
            // RPN, INFINITE
            var stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            var x = stack.RollDown(2.0);
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, INFINITE - empty - obtained");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - empty - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            x = stack.RollDown(2.0);
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, INFINITE - one - one roll - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, INFINITE - one - one roll - head");
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - one - one roll and pop - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            stack.Push(4.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 3.0 2.0
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, INFINITE - two - one roll - obtained");
            ClassicAssert.AreEqual(3.0, stack.GetValue(), Precision, "RPN, INFINITE - two - one roll - head");
            x = stack.RollDown(5.0); // stack is, head to tail: 2.0 5.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, INFINITE - two - two rolls - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, INFINITE - two - two rolls - head");
            x = stack.RollDown(6.0); // stack is, head to tail: 5.0 6.0
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, INFINITE - two - three rolls - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, INFINITE - two - three rolls - head");
            stack.Pop();
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - two - three rolls and two pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 4.0 3.0 2.0
            ClassicAssert.AreEqual(7.0, x, Precision, "RPN, INFINITE - three - one roll - obtained");
            ClassicAssert.AreEqual(4.0, stack.GetValue(), Precision, "RPN, INFINITE - three - one roll - head");
            x = stack.RollDown(5.0); // stack is, head to tail: 3.0 2.0 5.0
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, INFINITE - three - two rolls - obtained");
            ClassicAssert.AreEqual(3.0, stack.GetValue(), Precision, "RPN, INFINITE - three - two rolls - head");
            x = stack.RollDown(6.0); // stack is, head to tail: 2.0 5.0 6.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, INFINITE - three - three rolls - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, INFINITE - three - three rolls - head");
            x = stack.RollDown(8.0); // stack is, head to tail: 5.0 6.0 8.0
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, INFINITE - three - four rolls - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, INFINITE - three - four rolls - head");
            stack.Pop();
            stack.Pop();
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - three - four rolls and three pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            stack.Push(9.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 7.0 4.0 3.0 2.0
            ClassicAssert.AreEqual(9.0, x, Precision, "RPN, INFINITE - four - one roll - obtained");
            ClassicAssert.AreEqual(7.0, stack.GetValue(), Precision, "RPN, INFINITE - four - one roll - head");
            x = stack.RollDown(5.0); // stack is, head to tail: 4.0 3.0 2.0 5.0
            ClassicAssert.AreEqual(7.0, x, Precision, "RPN, INFINITE - four - two rolls - obtained");
            ClassicAssert.AreEqual(4.0, stack.GetValue(), Precision, "RPN, INFINITE - four - two rolls - head");
            x = stack.RollDown(6.0); // stack is, head to tail: 3.0 2.0 5.0 6.0
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, INFINITE - four - three rolls - obtained");
            ClassicAssert.AreEqual(3.0, stack.GetValue(), Precision, "RPN, INFINITE - four - three rolls - head");
            x = stack.RollDown(8.0); // stack is, head to tail: 2.0 5.0 6.0 8.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, INFINITE - four - four rolls - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, INFINITE - four - four rolls - head");
            x = stack.RollDown(10.0); // stack is, head to tail: 5.0 6.0 8.0 10.0
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, INFINITE - four - five rolls - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, INFINITE - four - five rolls - head");
            stack.Pop();
            stack.Pop();
            stack.Pop();
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, INFINITE - four - five rolls and four pops - head");

            // RPN, XYZT
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            x = stack.RollDown(2.0); // stack is, head to tail: 0.0 0.0 2.0
            ClassicAssert.AreEqual(0.0, x, Precision, "RPN, XYZT - empty - one roll - obtained");
            ClassicAssert.AreEqual(0.0, stack.GetValue(), Precision, "RPN, XYZT - empty - one roll - head");
            stack.Pop();
            ClassicAssert.AreEqual(0.0, stack.Pop(), Precision, "RPN, XYZT - empty - one roll and one pop - head");
            ClassicAssert.AreEqual(2.0, stack.Pop(), Precision, "RPN, XYZT - empty - one roll and two pops - head");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - empty - one roll and three pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(3.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 0.0 0.0 2.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, XYZT - one - one roll - obtained");
            ClassicAssert.AreEqual(0.0, stack.GetValue(), Precision, "RPN, XYZT - one - one roll - head");
            x = stack.RollDown(4.0); // stack is, head to tail: 0.0 2.0 4.0
            ClassicAssert.AreEqual(0.0, x, Precision, "RPN, XYZT - one - two rolls - obtained");
            ClassicAssert.AreEqual(0.0, stack.GetValue(), Precision, "RPN, XYZT - one - two rolls - head");
            stack.Pop();
            ClassicAssert.AreEqual(2.0, stack.Pop(), Precision, "RPN, XYZT - one - two rolls and two pops - head");
            ClassicAssert.AreEqual(4.0, stack.Pop(), Precision, "RPN, XYZT - one - two rolls and three pops - head");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - one - two rolls and four pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(3.0);
            stack.Push(4.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 3.0 0.0 2.0
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, XYZT - two - one roll - obtained");
            ClassicAssert.AreEqual(3.0, stack.GetValue(), Precision, "RPN, XYZT - two - one roll - head");
            x = stack.RollDown(5.0); // stack is, head to tail: 0.0 2.0 5.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, XYZT - two - two rolls - obtained");
            ClassicAssert.AreEqual(0.0, stack.GetValue(), Precision, "RPN, XYZT - two - two rolls - head");
            x = stack.RollDown(6.0); // stack is, head to tail: 2.0 5.0 6.0
            ClassicAssert.AreEqual(0.0, x, Precision, "RPN, XYZT - two - three rolls - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, XYZT - two - three rolls - head");
            stack.Pop();
            ClassicAssert.AreEqual(5.0, stack.Pop(), Precision, "RPN, XYZT - two - three rolls and two pops - head");
            ClassicAssert.AreEqual(6.0, stack.Pop(), Precision, "RPN, XYZT - two - three rolls and three pops - head");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - two - three rolls and four pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 4.0 3.0 2.0
            ClassicAssert.AreEqual(7.0, x, Precision, "RPN, XYZT - three - one roll - obtained");
            ClassicAssert.AreEqual(4.0, stack.GetValue(), Precision, "RPN, XYZT - three - one roll - head");
            x = stack.RollDown(5.0); // stack is, head to tail: 3.0 2.0 5.0
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, XYZT - three - two rolls - obtained");
            ClassicAssert.AreEqual(3.0, stack.GetValue(), Precision, "RPN, XYZT - three - two rolls - head");
            x = stack.RollDown(6.0); // stack is, head to tail: 2.0 5.0 6.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, XYZT - three - three rolls - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, XYZT - three - three rolls - head");
            x = stack.RollDown(8.0); // stack is, head to tail: 5.0 6.0 8.0
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, XYZT - three - four rolls - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, XYZT - three - four rolls - head");
            stack.Pop();
            ClassicAssert.AreEqual(6.0, stack.Pop(), Precision, "RPN, XYZT - three - three rolls and two pops - head");
            ClassicAssert.AreEqual(8.0, stack.Pop(), Precision, "RPN, XYZT - three - three rolls and three pops - head");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - three - four rolls and five pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(9.0); // will be washed away by pushing
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 4.0 3.0 2.0
            ClassicAssert.AreEqual(7.0, x, Precision, "RPN, XYZT - three - one roll - obtained");
            ClassicAssert.AreEqual(4.0, stack.GetValue(), Precision, "RPN, XYZT - three - one roll - head");
            x = stack.RollDown(5.0); // stack is, head to tail: 3.0 2.0 5.0
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, XYZT - three - two rolls - obtained");
            ClassicAssert.AreEqual(3.0, stack.GetValue(), Precision, "RPN, XYZT - three - two rolls - head");
            x = stack.RollDown(6.0); // stack is, head to tail: 2.0 5.0 6.0
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, XYZT - three - three rolls - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, XYZT - three - three rolls - head");
            x = stack.RollDown(8.0); // stack is, head to tail: 5.0 6.0 8.0
            ClassicAssert.AreEqual(2.0, x, Precision, "RPN, XYZT - three - four rolls - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, XYZT - three - four rolls - head");
            stack.Pop();
            ClassicAssert.AreEqual(6.0, stack.Pop(), Precision, "RPN, XYZT - three - three rolls and two pops - head");
            ClassicAssert.AreEqual(8.0, stack.Pop(), Precision, "RPN, XYZT - three - three rolls and three pops - head");
            ClassicAssert.AreEqual(true, stack.IsEmpty(), "RPN, XYZT - three - four rolls and five pops - head");
            // normal functioning
            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(2.0, Cpu.BinaryOp.Div);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.RollDown(3.0),
                "failed to throw at rollDown in ALG mode");
        }

        [Test]
        public void HeadParenExistsTest()
        {
            // normal functioning
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(2.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            ClassicAssert.AreEqual(true, stack.HeadParenExists(), "ALG - one push - op");
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            ClassicAssert.AreEqual(false, stack.HeadParenExists(), "ALG - two pushes - op");
            stack.Pop();
            ClassicAssert.AreEqual(true, stack.HeadParenExists(), "ALG - two pushes, one pop - op");
            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Alg);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.HeadParenExists(),
                "failed to throw at headParenExists of empty stack");

            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.HeadParenExists(),
                "failed to throw at headParenExists in RPN mode");
        }

        [Test]
        public void HeadParenAddTest()
        {
            // normal functioning
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            stack.HeadParenAdd();
            ClassicAssert.AreEqual(true, stack.HeadParenExists(), "ALG - one paren added - op");
            stack.HeadParenAdd();
            ClassicAssert.AreEqual(true, stack.HeadParenExists(), "ALG - two parens added - op");
            stack.HeadParenRemove();
            ClassicAssert.AreEqual(true, stack.HeadParenExists(), "ALG - two parens added, one removed - op");
            stack.HeadParenRemove();
            ClassicAssert.AreEqual(false, stack.HeadParenExists(), "ALG - two parens added, two removed - op");
            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Alg);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.HeadParenAdd(),
                "failed to throw at headParenAdd of empty stack");

            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.HeadParenAdd(),
                "failed to throw at headParenAdd in RPN mode");
        }

        [Test]
        public void HeadParenRemoveTest()
        {
            // normal functioning
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            stack.HeadParenRemove();
            ClassicAssert.AreEqual(true, stack.HeadParenExists(), "ALG - two parens added, one removed - op");
            stack.HeadParenRemove();
            ClassicAssert.AreEqual(false, stack.HeadParenExists(), "ALG - two parens added, two removed - op");
            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Alg);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.HeadParenRemove(),
                "failed to throw at headParenRemove of empty stack");

            stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.HeadParenRemove(),
                "failed to throw at headParenRemove when none exist");

            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.HeadParenRemove(),
                "failed to throw at headParenRemove in RPN mode");
        }

        [Test]
        public void SwapHeadValueTest()
        {
            // ALG
            var stack = new CpuStack(Settings.Mode.Alg);
            var x = stack.SwapHeadValue(2.0);
            ClassicAssert.AreEqual(2.0, x, Precision, "ALG - empty");
            stack.Push(3.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            x = stack.SwapHeadValue(2.0);
            ClassicAssert.AreEqual(3.0, x, Precision, "ALG - one - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "ALG - one - stayed");
            stack.Push(4.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            x = stack.SwapHeadValue(5.0);
            ClassicAssert.AreEqual(4.0, x, Precision, "ALG - two - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "ALG - two - stayed");
            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            x = stack.SwapHeadValue(2.0);
            ClassicAssert.AreEqual(0.0, x, Precision, "RPN, INFINITE - empty - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, INFINITE - empty - stayed");
            stack.Push(3.0);
            x = stack.SwapHeadValue(2.0);
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, INFINITE - one - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, INFINITE - one - stayed");
            stack.Push(4.0);
            x = stack.SwapHeadValue(5.0);
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, INFINITE - two - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, INFINITE - two - stayed");
            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            x = stack.SwapHeadValue(2.0);
            ClassicAssert.AreEqual(0.0, x, Precision, "RPN, XYZT - empty - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, XYZT - empty - stayed");
            stack.Push(3.0);
            x = stack.SwapHeadValue(2.0);
            ClassicAssert.AreEqual(3.0, x, Precision, "RPN, XYZT - one - obtained");
            ClassicAssert.AreEqual(2.0, stack.GetValue(), Precision, "RPN, XYZT - one - stayed");
            stack.Push(4.0);
            x = stack.SwapHeadValue(5.0);
            ClassicAssert.AreEqual(4.0, x, Precision, "RPN, XYZT - two - obtained");
            ClassicAssert.AreEqual(5.0, stack.GetValue(), Precision, "RPN, XYZT - two - stayed");
        }

        [Test]
        public void PeekValueTest()
        {
            // ALG
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(3.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            stack.Push(4.0, Cpu.BinaryOp.Sub);
            ClassicAssert.AreEqual(4.0, stack.PeekValue(0), Precision, "ALG - 0");
            ClassicAssert.AreEqual(3.0, stack.PeekValue(1), Precision, "ALG - 1");
            ClassicAssert.AreEqual(0.0, stack.PeekValue(2), Precision, "ALG - 2");
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.PeekValue(-1),
                "failed to throw at peekValue at negative depth in RPN mode");

            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(3.0);
            stack.Push(4.0);
            ClassicAssert.AreEqual(4.0, stack.PeekValue(0), Precision, "RPN - 0");
            ClassicAssert.AreEqual(3.0, stack.PeekValue(1), Precision, "RPN - 1");
            ClassicAssert.AreEqual(0.0, stack.PeekValue(2), Precision, "RPN - 2");
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.PeekValue(-1),
                "failed to throw at peekValue at negative depth in RPN mode");
        }

        [Test]
        public void PeekOpTest()
        {
            // ALG
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(3.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            stack.Push(4.0, Cpu.BinaryOp.Sub);
            ClassicAssert.AreEqual(Cpu.BinaryOp.Sub, stack.PeekOp(0), "ALG - 0");
            ClassicAssert.AreEqual(Cpu.BinaryOp.Div, stack.PeekOp(1), "ALG - 1");
            ClassicAssert.AreEqual(null, stack.PeekOp(2), "ALG - 2");
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.PeekOp(-1),
                "failed to throw at peekOp at negative depth in RPN mode");

            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(3.0);
            stack.Push(4.0);
            ClassicAssert.AreEqual(null, stack.PeekOp(0), "RPN - 0");
            ClassicAssert.AreEqual(null, stack.PeekOp(1), "RPN - 1");
            ClassicAssert.AreEqual(null, stack.PeekOp(2), "RPN - 2");
            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.PeekOp(-1),
                "failed to throw at peekValue at negative depth in RPN mode");
        }

        [Test]
        public void PeekParenExistsTest()
        {
            // ALG
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(3.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            stack.Push(4.0, Cpu.BinaryOp.Sub);
            ClassicAssert.AreEqual(false, stack.PeekParenExists(0), "ALG - 0");
            ClassicAssert.AreEqual(true, stack.PeekParenExists(1), "ALG - 1");
            ClassicAssert.AreEqual(false, stack.PeekParenExists(2), "ALG - 2");

            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.PeekParenExists(-1),
                "failed to throw at peekOp at negative depth in RPN mode");

            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(3.0);
            stack.Push(4.0);
            ClassicAssert.AreEqual(false, stack.PeekParenExists(0), "RPN - 0");
            ClassicAssert.AreEqual(false, stack.PeekParenExists(1), "RPN - 1");
            ClassicAssert.AreEqual(false, stack.PeekParenExists(2), "RPN - 2");

            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)), () => stack.PeekParenExists(-1),
                "failed to throw at peekValue at negative depth in RPN mode");
        }
    }
}
