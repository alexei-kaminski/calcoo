using System;
using NUnit.Framework;

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

            Assert.That(clonedStack.GetValue(), Is.EqualTo(3.0).Within(Precision), "Alg - two pushes - value");
            Assert.That(clonedStack.GetOp(), Is.EqualTo(Cpu.BinaryOp.Sub), "Alg - two pushes - op");
            Assert.That(clonedStack.HeadParenExists(), Is.EqualTo(false), "Alg - two pushes - paren");
            clonedStack.Pop();
            Assert.That(clonedStack.GetValue(), Is.EqualTo(2.0).Within(Precision), "Alg - two pushes, one pop - value");
            Assert.That(clonedStack.GetOp(), Is.EqualTo(Cpu.BinaryOp.Div), "Alg - two pushes, one pop - op");
            Assert.That(clonedStack.HeadParenExists(), Is.EqualTo(true), "Alg - two pushes, one pop - paren");
            clonedStack.Pop();
            Assert.That(clonedStack.IsEmpty(), Is.EqualTo(true), "Alg - two pushes, two pops - empty");
            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            stack.Push(3.0);
            clonedStack = stack.Clone();

            Assert.That(clonedStack.GetValue(), Is.EqualTo(3.0).Within(Precision), "RPN - two pushes");
            clonedStack.Pop();
            Assert.That(clonedStack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN - two pushes, one pop");
            clonedStack.Pop();
            Assert.That(clonedStack.IsEmpty(), Is.EqualTo(true), "RPN - two pushes, two pops");
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
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "Alg - one push - value");
            Assert.That(stack.GetOp(), Is.EqualTo(Cpu.BinaryOp.Div), "Alg - one push - op");
            Assert.That(stack.HeadParenExists(), Is.EqualTo(true), "Alg - one push - paren");
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            Assert.That(stack.GetValue(), Is.EqualTo(3.0).Within(Precision), "Alg - two pushes - value");
            Assert.That(stack.GetOp(), Is.EqualTo(Cpu.BinaryOp.Sub), "Alg - two pushes - op");
            Assert.That(stack.HeadParenExists(), Is.EqualTo(false), "Alg - two pushes - paren");
            stack.Pop();
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "Alg - two pushes, one pop - value");
            Assert.That(stack.GetOp(), Is.EqualTo(Cpu.BinaryOp.Div), "Alg - two pushes, one pop - op");
            Assert.That(stack.HeadParenExists(), Is.EqualTo(true), "Alg - two pushes, one pop - paren");
            // RPN, INFINITE
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(2.0);
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - one push");
            stack.Push(3.0);
            Assert.That(stack.GetValue(), Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - two pushes");
            stack.Push(4.0);
            Assert.That(stack.GetValue(), Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - three pushes");
            stack.Push(5.0);
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, INFINITE - four pushes");
            stack.Pop();
            Assert.That(stack.GetValue(), Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - four pushes, one pop");
            stack.Pop();
            Assert.That(stack.GetValue(), Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - four pushes, two pops");
            stack.Pop();
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - four pushes, three pops");
            stack.Pop();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - four pushes, four pops");
            // RPN, XYZT
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(2.0);
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - one push");
            stack.Push(3.0);
            Assert.That(stack.GetValue(), Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - two pushes");
            stack.Push(4.0);
            Assert.That(stack.GetValue(), Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - three pushes");
            stack.Push(5.0);
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, XYZT - four pushes");
            stack.Pop();
            Assert.That(stack.GetValue(), Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - four pushes, one pop");
            stack.Pop();
            Assert.That(stack.GetValue(), Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - four pushes, two pops");
            stack.Pop();
            // the fourth push chopped the tail off
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - four pushes, three pops");
            // testing for invalid inputs
            // ---------------------------------------------------------------
            stack = new CpuStack(Settings.Mode.Alg);
            Assert.That(() => stack.Push(2.0), Throws.InstanceOf<Exception>(), "RPN push Alg mode");

            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            Assert.That(() => stack.Push(2.0, Cpu.BinaryOp.Div), Throws.InstanceOf<Exception>(),
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
            Assert.That(stack.Pop(), Is.EqualTo(3.0).Within(Precision), "Alg - two pushes, one pop - value");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "Alg - two pushes, one pop - head value");
            Assert.That(stack.GetOp(), Is.EqualTo(Cpu.BinaryOp.Div), "Alg - two pushes, one pop - head op");
            Assert.That(stack.HeadParenExists(), Is.EqualTo(true), "Alg - two pushes, one pop - head paren");
            Assert.That(stack.Pop(), Is.EqualTo(2.0).Within(Precision), "Alg - two pushes, two pops - value");
            // RPN, INFINITE
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(2.0);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(5.0);
            Assert.That(stack.Pop(), Is.EqualTo(5.0).Within(Precision), "RPN, INFINITE - four pushes");
            Assert.That(stack.Pop(), Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - four pushes, one pop");
            Assert.That(stack.Pop(), Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - four pushes, two pops");
            Assert.That(stack.Pop(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - four pushes, three pops");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - four pushes, four pops");
            Assert.That(stack.Pop(), Is.EqualTo(0.0).Within(Precision), "RPN, INFINITE - empty stack pops 0");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - empty stack stays empty after pop");
            // RPN, XYZT
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(2.0);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(5.0);
            Assert.That(stack.Pop(), Is.EqualTo(5.0).Within(Precision), "RPN, XYZT - four pushes");
            Assert.That(stack.Pop(), Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - four pushes, one pop");
            Assert.That(stack.Pop(), Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - four pushes, two pops");
            // the fourth push chopped the tail off
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - four pushes, three pops");
            Assert.That(stack.Pop(), Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - empty stack pops 0");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - empty stack stays empty after pop");
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
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "ALG empty after one push, clear");
            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            stack.Clear();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN empty after one push, clear");
        }

        [Test]
        public void ExistOpenParenTest()
        {
            // normal functioning
            var stack = new CpuStack(Settings.Mode.Alg);
            Assert.That(stack.ExistOpenParen(), Is.EqualTo(false), "empty stack");
            stack.Push(2.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            Assert.That(stack.ExistOpenParen(), Is.EqualTo(true), "paren at head");
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            Assert.That(stack.ExistOpenParen(), Is.EqualTo(true), "paren deep");
            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            Assert.That(() =>
                stack.ExistOpenParen(), Throws.InstanceOf<Exception>(), "failed to throw at existOpenParen in RPN mode");
        }

        [Test]
        public void IsEmptyTest()
        {
            // ALG
            var stack = new CpuStack(Settings.Mode.Alg);
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "ALG empty new");
            stack.Push(2.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            Assert.That(stack.IsEmpty(), Is.EqualTo(false), "ALG empty after one push");
            stack.Pop();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "ALG empty after one push, one pop");
            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN empty new");
            stack.Push(2.0);
            Assert.That(stack.IsEmpty(), Is.EqualTo(false), "RPN empty after one push");
            stack.Pop();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN empty after one push, one pop");
        }

        [Test]
        public void GetOpTest()
        {
            // normal functioning
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(2.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            Assert.That(stack.GetOp(), Is.EqualTo(Cpu.BinaryOp.Div), "ALG - one push - op");
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            Assert.That(stack.GetOp(), Is.EqualTo(Cpu.BinaryOp.Sub), "ALG - two pushes - op");
            stack.Pop();
            Assert.That(stack.GetOp(), Is.EqualTo(Cpu.BinaryOp.Div), "ALG - two pushes, one pop - op");
            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Alg);
            Assert.That(() => stack.GetOp(), Throws.InstanceOf<Exception>(),
                "failed to throw at getOp of empty stack");


            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            Assert.That(() => stack.GetOp(), Throws.InstanceOf<Exception>(), "failed to throw at getOp in RPN mode");
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
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "ALG - one push");
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            Assert.That(stack.GetValue(), Is.EqualTo(3.0).Within(Precision), "ALG - two pushes");
            // testing for invalid inputs
            // ---------------------------------------------------------------
            stack = new CpuStack(Settings.Mode.Alg);
            Assert.That(() => stack.GetValue(), Throws.InstanceOf<Exception>(),
                "failed to throw at getValue of empty stack, ALG mode");

            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            Assert.That(() => stack.GetValue(), Throws.InstanceOf<Exception>(),
                "failed to throw at getValue of empty stack, RPN mode");
        }

        [Test]
        public void RollUpTest()
        {
            // RPN, INFINITE
            var stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            var x = stack.RollUp(2.0);
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - empty - obtained");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - empty - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            x = stack.RollUp(2.0);
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - one - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - one - one roll - head");
            x = stack.RollUp(4.0);
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - one - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - one - two rolls - head");
            stack.Pop();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - one - two rolls and pop - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            stack.Push(4.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 4.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - two - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - two - one roll - head");
            x = stack.RollUp(5.0); // stack is, head to tail: 5.0 2.0
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - two - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, INFINITE - two - two rolls - head");
            x = stack.RollUp(6.0); // stack is, head to tail: 6.0 5.0
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - two - three rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(6.0).Within(Precision), "RPN, INFINITE - two - three rolls - head");
            stack.Pop();
            stack.Pop();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - two - three rolls and two pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 7.0 4.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - three - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - three - one roll - head");
            x = stack.RollUp(5.0); // stack is, head to tail: 5.0 2.0 7.0
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - three - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, INFINITE - three - two rolls - head");
            x = stack.RollUp(6.0); // stack is, head to tail: 6.0 5.0 2.0
            Assert.That(x, Is.EqualTo(7.0).Within(Precision), "RPN, INFINITE - three - three rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(6.0).Within(Precision), "RPN, INFINITE - three - three rolls - head");
            x = stack.RollUp(8.0); // stack is, head to tail: 6.0 5.0 2.0
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - three - four rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(8.0).Within(Precision), "RPN, INFINITE - three - four rolls - head");
            stack.Pop();
            stack.Pop();
            stack.Pop();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - three - four rolls and three pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            stack.Push(9.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 9.0 7.0 4.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - four - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - four - one roll - head");
            x = stack.RollUp(5.0); // stack is, head to tail: 5.0 2.0 9.0 7.0
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - four - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, INFINITE - four - two rolls - head");
            x = stack.RollUp(6.0); // stack is, head to tail: 6.0 5.0 2.0 9.0
            Assert.That(x, Is.EqualTo(7.0).Within(Precision), "RPN, INFINITE - four - three rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(6.0).Within(Precision), "RPN, INFINITE - four - three rolls - head");
            x = stack.RollUp(8.0); // stack is, head to tail: 8.0 6.0 5.0 2.0
            Assert.That(x, Is.EqualTo(9.0).Within(Precision), "RPN, INFINITE - four - four rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(8.0).Within(Precision), "RPN, INFINITE - four - four rolls - head");
            x = stack.RollUp(10.0); // stack is, head to tail: 10.0 8.0 6.0 5.0
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - four - five rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(10.0).Within(Precision), "RPN, INFINITE - four - five rolls - head");
            stack.Pop();
            stack.Pop();
            stack.Pop();
            stack.Pop();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - four - five rolls and four pops - head");

            // RPN, XYZT
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 0.0 0.0
            Assert.That(x, Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - empty - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - empty - one roll - head");
            stack.Pop();
            Assert.That(stack.Pop(), Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - empty - one roll and one pop - head");
            Assert.That(stack.Pop(), Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - empty - one roll and two pops - head");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - empty - one roll and three pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(3.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 3.0 0.0
            Assert.That(x, Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - one - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - one - one roll - head");
            x = stack.RollUp(4.0); // stack is, head to tail: 4.0 2.0 3.0
            Assert.That(x, Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - one - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - one - two rolls - head");
            stack.Pop();
            Assert.That(stack.Pop(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - one - two rolls and two pops - head");
            Assert.That(stack.Pop(), Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - one - two rolls and three pops - head");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - one - two rollsl and four pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(3.0);
            stack.Push(4.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 4.0 3.0
            Assert.That(x, Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - two - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - two - one roll - head");
            x = stack.RollUp(5.0); // stack is, head to tail: 5.0 2.0 4.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - two - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, XYZT - two - two rolls - head");
            x = stack.RollUp(6.0); // stack is, head to tail: 6.0 5.0 2.0
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - two - three rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(6.0).Within(Precision), "RPN, XYZT - two - three rolls - head");
            stack.Pop();
            Assert.That(stack.Pop(), Is.EqualTo(5.0).Within(Precision), "RPN, XYZT - one - three rolls and two pops - head");
            Assert.That(stack.Pop(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - one - three rolls and three pops - head");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - two - three rolls and four pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 7.0 4.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - three - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - three - one roll - head");
            x = stack.RollUp(5.0); // stack is, head to tail: 5.0 2.0 7.0
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - three - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, XYZT - three - two rolls - head");
            x = stack.RollUp(6.0); // stack is, head to tail: 6.0 5.0 2.0
            Assert.That(x, Is.EqualTo(7.0).Within(Precision), "RPN, XYZT - three - three rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(6.0).Within(Precision), "RPN, XYZT - three - three rolls - head");
            x = stack.RollUp(8.0); // stack is, head to tail: 8.0 6.0 5.0
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - three - four rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(8.0).Within(Precision), "RPN, XYZT - three - four rolls - head");
            stack.Pop();
            Assert.That(stack.Pop(), Is.EqualTo(6.0).Within(Precision), "RPN, XYZT - one - four rolls and two pops - head");
            Assert.That(stack.Pop(), Is.EqualTo(5.0).Within(Precision), "RPN, XYZT - one - four rolls and three pops - head");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - three - four rolls and four pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(9.0); // will be washed away by pushing
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            x = stack.RollUp(2.0); // stack is, head to tail: 2.0 7.0 4.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - three - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - three - one roll - head");
            x = stack.RollUp(5.0); // stack is, head to tail: 5.0 2.0 7.0
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - three - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, XYZT - three - two rolls - head");
            x = stack.RollUp(6.0); // stack is, head to tail: 6.0 5.0 2.0
            Assert.That(x, Is.EqualTo(7.0).Within(Precision), "RPN, XYZT - three - three rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(6.0).Within(Precision), "RPN, XYZT - three - three rolls - head");
            x = stack.RollUp(8.0); // stack is, head to tail: 6.0 5.0 2.0
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - three - four rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(8.0).Within(Precision), "RPN, XYZT - three - four rolls - head");
            stack.Pop();
            stack.Pop();
            stack.Pop();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - three - four rolls and three pops - head");

            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(2.0, Cpu.BinaryOp.Div);
            Assert.That(() => stack.RollUp(3.0), Throws.InstanceOf<Exception>(),
                "failed to throw at rollUp in ALG mode");
        }

        [Test]
        public void RollDownTest()
        {
            // RPN, INFINITE
            var stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            var x = stack.RollDown(2.0);
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - empty - obtained");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - empty - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            x = stack.RollDown(2.0);
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - one - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - one - one roll - head");
            stack.Pop();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - one - one roll and pop - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            stack.Push(4.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 3.0 2.0
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - two - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - two - one roll - head");
            x = stack.RollDown(5.0); // stack is, head to tail: 2.0 5.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - two - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - two - two rolls - head");
            x = stack.RollDown(6.0); // stack is, head to tail: 5.0 6.0
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - two - three rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, INFINITE - two - three rolls - head");
            stack.Pop();
            stack.Pop();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - two - three rolls and two pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 4.0 3.0 2.0
            Assert.That(x, Is.EqualTo(7.0).Within(Precision), "RPN, INFINITE - three - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - three - one roll - head");
            x = stack.RollDown(5.0); // stack is, head to tail: 3.0 2.0 5.0
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - three - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - three - two rolls - head");
            x = stack.RollDown(6.0); // stack is, head to tail: 2.0 5.0 6.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - three - three rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - three - three rolls - head");
            x = stack.RollDown(8.0); // stack is, head to tail: 5.0 6.0 8.0
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - three - four rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, INFINITE - three - four rolls - head");
            stack.Pop();
            stack.Pop();
            stack.Pop();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - three - four rolls and three pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            stack.Push(9.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 7.0 4.0 3.0 2.0
            Assert.That(x, Is.EqualTo(9.0).Within(Precision), "RPN, INFINITE - four - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(7.0).Within(Precision), "RPN, INFINITE - four - one roll - head");
            x = stack.RollDown(5.0); // stack is, head to tail: 4.0 3.0 2.0 5.0
            Assert.That(x, Is.EqualTo(7.0).Within(Precision), "RPN, INFINITE - four - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - four - two rolls - head");
            x = stack.RollDown(6.0); // stack is, head to tail: 3.0 2.0 5.0 6.0
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - four - three rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - four - three rolls - head");
            x = stack.RollDown(8.0); // stack is, head to tail: 2.0 5.0 6.0 8.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - four - four rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - four - four rolls - head");
            x = stack.RollDown(10.0); // stack is, head to tail: 5.0 6.0 8.0 10.0
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - four - five rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, INFINITE - four - five rolls - head");
            stack.Pop();
            stack.Pop();
            stack.Pop();
            stack.Pop();
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, INFINITE - four - five rolls and four pops - head");

            // RPN, XYZT
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            x = stack.RollDown(2.0); // stack is, head to tail: 0.0 0.0 2.0
            Assert.That(x, Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - empty - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - empty - one roll - head");
            stack.Pop();
            Assert.That(stack.Pop(), Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - empty - one roll and one pop - head");
            Assert.That(stack.Pop(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - empty - one roll and two pops - head");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - empty - one roll and three pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(3.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 0.0 0.0 2.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - one - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - one - one roll - head");
            x = stack.RollDown(4.0); // stack is, head to tail: 0.0 2.0 4.0
            Assert.That(x, Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - one - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - one - two rolls - head");
            stack.Pop();
            Assert.That(stack.Pop(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - one - two rolls and two pops - head");
            Assert.That(stack.Pop(), Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - one - two rolls and three pops - head");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - one - two rolls and four pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(3.0);
            stack.Push(4.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 3.0 0.0 2.0
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - two - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - two - one roll - head");
            x = stack.RollDown(5.0); // stack is, head to tail: 0.0 2.0 5.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - two - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - two - two rolls - head");
            x = stack.RollDown(6.0); // stack is, head to tail: 2.0 5.0 6.0
            Assert.That(x, Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - two - three rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - two - three rolls - head");
            stack.Pop();
            Assert.That(stack.Pop(), Is.EqualTo(5.0).Within(Precision), "RPN, XYZT - two - three rolls and two pops - head");
            Assert.That(stack.Pop(), Is.EqualTo(6.0).Within(Precision), "RPN, XYZT - two - three rolls and three pops - head");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - two - three rolls and four pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 4.0 3.0 2.0
            Assert.That(x, Is.EqualTo(7.0).Within(Precision), "RPN, XYZT - three - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - three - one roll - head");
            x = stack.RollDown(5.0); // stack is, head to tail: 3.0 2.0 5.0
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - three - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - three - two rolls - head");
            x = stack.RollDown(6.0); // stack is, head to tail: 2.0 5.0 6.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - three - three rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - three - three rolls - head");
            x = stack.RollDown(8.0); // stack is, head to tail: 5.0 6.0 8.0
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - three - four rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, XYZT - three - four rolls - head");
            stack.Pop();
            Assert.That(stack.Pop(), Is.EqualTo(6.0).Within(Precision), "RPN, XYZT - three - three rolls and two pops - head");
            Assert.That(stack.Pop(), Is.EqualTo(8.0).Within(Precision), "RPN, XYZT - three - three rolls and three pops - head");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - three - four rolls and five pops - head");

            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            stack.Push(9.0); // will be washed away by pushing
            stack.Push(3.0);
            stack.Push(4.0);
            stack.Push(7.0);
            x = stack.RollDown(2.0); // stack is, head to tail: 4.0 3.0 2.0
            Assert.That(x, Is.EqualTo(7.0).Within(Precision), "RPN, XYZT - three - one roll - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - three - one roll - head");
            x = stack.RollDown(5.0); // stack is, head to tail: 3.0 2.0 5.0
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - three - two rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - three - two rolls - head");
            x = stack.RollDown(6.0); // stack is, head to tail: 2.0 5.0 6.0
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - three - three rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - three - three rolls - head");
            x = stack.RollDown(8.0); // stack is, head to tail: 5.0 6.0 8.0
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - three - four rolls - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, XYZT - three - four rolls - head");
            stack.Pop();
            Assert.That(stack.Pop(), Is.EqualTo(6.0).Within(Precision), "RPN, XYZT - three - three rolls and two pops - head");
            Assert.That(stack.Pop(), Is.EqualTo(8.0).Within(Precision), "RPN, XYZT - three - three rolls and three pops - head");
            Assert.That(stack.IsEmpty(), Is.EqualTo(true), "RPN, XYZT - three - four rolls and five pops - head");
            // normal functioning
            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(2.0, Cpu.BinaryOp.Div);
            Assert.That(() => stack.RollDown(3.0), Throws.InstanceOf<Exception>(),
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
            Assert.That(stack.HeadParenExists(), Is.EqualTo(true), "ALG - one push - op");
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            Assert.That(stack.HeadParenExists(), Is.EqualTo(false), "ALG - two pushes - op");
            stack.Pop();
            Assert.That(stack.HeadParenExists(), Is.EqualTo(true), "ALG - two pushes, one pop - op");
            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Alg);
            Assert.That(() => stack.HeadParenExists(), Throws.InstanceOf<Exception>(),
                "failed to throw at headParenExists of empty stack");

            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            Assert.That(() => stack.HeadParenExists(), Throws.InstanceOf<Exception>(),
                "failed to throw at headParenExists in RPN mode");
        }

        [Test]
        public void HeadParenAddTest()
        {
            // normal functioning
            var stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            stack.HeadParenAdd();
            Assert.That(stack.HeadParenExists(), Is.EqualTo(true), "ALG - one paren added - op");
            stack.HeadParenAdd();
            Assert.That(stack.HeadParenExists(), Is.EqualTo(true), "ALG - two parens added - op");
            stack.HeadParenRemove();
            Assert.That(stack.HeadParenExists(), Is.EqualTo(true), "ALG - two parens added, one removed - op");
            stack.HeadParenRemove();
            Assert.That(stack.HeadParenExists(), Is.EqualTo(false), "ALG - two parens added, two removed - op");
            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Alg);
            Assert.That(() => stack.HeadParenAdd(), Throws.InstanceOf<Exception>(),
                "failed to throw at headParenAdd of empty stack");

            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            Assert.That(() => stack.HeadParenAdd(), Throws.InstanceOf<Exception>(),
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
            Assert.That(stack.HeadParenExists(), Is.EqualTo(true), "ALG - two parens added, one removed - op");
            stack.HeadParenRemove();
            Assert.That(stack.HeadParenExists(), Is.EqualTo(false), "ALG - two parens added, two removed - op");
            // throws at invalid usage
            stack = new CpuStack(Settings.Mode.Alg);
            Assert.That(() => stack.HeadParenRemove(), Throws.InstanceOf<Exception>(),
                "failed to throw at headParenRemove of empty stack");

            stack = new CpuStack(Settings.Mode.Alg);
            stack.Push(3.0, Cpu.BinaryOp.Sub);
            Assert.That(() => stack.HeadParenRemove(), Throws.InstanceOf<Exception>(),
                "failed to throw at headParenRemove when none exist");

            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(2.0);
            Assert.That(() => stack.HeadParenRemove(), Throws.InstanceOf<Exception>(),
                "failed to throw at headParenRemove in RPN mode");
        }

        [Test]
        public void SwapHeadValueTest()
        {
            // ALG
            var stack = new CpuStack(Settings.Mode.Alg);
            var x = stack.SwapHeadValue(2.0);
            Assert.That(x, Is.EqualTo(2.0).Within(Precision), "ALG - empty");
            stack.Push(3.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            x = stack.SwapHeadValue(2.0);
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "ALG - one - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "ALG - one - stayed");
            stack.Push(4.0, Cpu.BinaryOp.Div);
            stack.HeadParenAdd();
            stack.HeadParenAdd();
            x = stack.SwapHeadValue(5.0);
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "ALG - two - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "ALG - two - stayed");
            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Infinite);
            x = stack.SwapHeadValue(2.0);
            Assert.That(x, Is.EqualTo(0.0).Within(Precision), "RPN, INFINITE - empty - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - empty - stayed");
            stack.Push(3.0);
            x = stack.SwapHeadValue(2.0);
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, INFINITE - one - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, INFINITE - one - stayed");
            stack.Push(4.0);
            x = stack.SwapHeadValue(5.0);
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, INFINITE - two - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, INFINITE - two - stayed");
            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, Settings.StackMode.Xyzt);
            x = stack.SwapHeadValue(2.0);
            Assert.That(x, Is.EqualTo(0.0).Within(Precision), "RPN, XYZT - empty - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - empty - stayed");
            stack.Push(3.0);
            x = stack.SwapHeadValue(2.0);
            Assert.That(x, Is.EqualTo(3.0).Within(Precision), "RPN, XYZT - one - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(2.0).Within(Precision), "RPN, XYZT - one - stayed");
            stack.Push(4.0);
            x = stack.SwapHeadValue(5.0);
            Assert.That(x, Is.EqualTo(4.0).Within(Precision), "RPN, XYZT - two - obtained");
            Assert.That(stack.GetValue(), Is.EqualTo(5.0).Within(Precision), "RPN, XYZT - two - stayed");
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
            Assert.That(stack.PeekValue(0), Is.EqualTo(4.0).Within(Precision), "ALG - 0");
            Assert.That(stack.PeekValue(1), Is.EqualTo(3.0).Within(Precision), "ALG - 1");
            Assert.That(stack.PeekValue(2), Is.EqualTo(0.0).Within(Precision), "ALG - 2");
            Assert.That(() => stack.PeekValue(-1), Throws.InstanceOf<Exception>(),
                "failed to throw at peekValue at negative depth in RPN mode");

            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(3.0);
            stack.Push(4.0);
            Assert.That(stack.PeekValue(0), Is.EqualTo(4.0).Within(Precision), "RPN - 0");
            Assert.That(stack.PeekValue(1), Is.EqualTo(3.0).Within(Precision), "RPN - 1");
            Assert.That(stack.PeekValue(2), Is.EqualTo(0.0).Within(Precision), "RPN - 2");
            Assert.That(() => stack.PeekValue(-1), Throws.InstanceOf<Exception>(),
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
            Assert.That(stack.PeekOp(0), Is.EqualTo(Cpu.BinaryOp.Sub), "ALG - 0");
            Assert.That(stack.PeekOp(1), Is.EqualTo(Cpu.BinaryOp.Div), "ALG - 1");
            Assert.That(stack.PeekOp(2), Is.EqualTo(null), "ALG - 2");
            Assert.That(() => stack.PeekOp(-1), Throws.InstanceOf<Exception>(),
                "failed to throw at peekOp at negative depth in RPN mode");

            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(3.0);
            stack.Push(4.0);
            Assert.That(stack.PeekOp(0), Is.EqualTo(null), "RPN - 0");
            Assert.That(stack.PeekOp(1), Is.EqualTo(null), "RPN - 1");
            Assert.That(stack.PeekOp(2), Is.EqualTo(null), "RPN - 2");
            Assert.That(() => stack.PeekOp(-1), Throws.InstanceOf<Exception>(),
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
            Assert.That(stack.PeekParenExists(0), Is.EqualTo(false), "ALG - 0");
            Assert.That(stack.PeekParenExists(1), Is.EqualTo(true), "ALG - 1");
            Assert.That(stack.PeekParenExists(2), Is.EqualTo(false), "ALG - 2");

            Assert.That(() => stack.PeekParenExists(-1), Throws.InstanceOf<Exception>(),
                "failed to throw at peekOp at negative depth in RPN mode");

            // RPN
            stack = new CpuStack(Settings.Mode.Rpn, DefaultStackMode);
            stack.Push(3.0);
            stack.Push(4.0);
            Assert.That(stack.PeekParenExists(0), Is.EqualTo(false), "RPN - 0");
            Assert.That(stack.PeekParenExists(1), Is.EqualTo(false), "RPN - 1");
            Assert.That(stack.PeekParenExists(2), Is.EqualTo(false), "RPN - 2");

            Assert.That(() => stack.PeekParenExists(-1), Throws.InstanceOf<Exception>(),
                "failed to throw at peekValue at negative depth in RPN mode");
        }
    }
}
