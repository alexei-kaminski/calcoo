using NUnit.Framework;

namespace Calcoo.Test
{
    [TestFixture]
    internal class CommandTest
    {
        [Test]
        public void TrigBareToDressedTest()
        {
            Assert.AreEqual(Command.Sin, Command.Sin.TrigBareToDressed(false, false), "SIN");
            Assert.AreEqual(Command.Asin, Command.Sin.TrigBareToDressed(true, false), "ASIN");
            Assert.AreEqual(Command.Sinh, Command.Sin.TrigBareToDressed(false, true), "SINH");
            Assert.AreEqual(Command.Asinh, Command.Sin.TrigBareToDressed(true, true), "ASINH");

            Assert.AreEqual(Command.Cos, Command.Cos.TrigBareToDressed(false, false), "COS");
            Assert.AreEqual(Command.Acos, Command.Cos.TrigBareToDressed(true, false), "ACOS");
            Assert.AreEqual(Command.Cosh, Command.Cos.TrigBareToDressed(false, true), "COSH");
            Assert.AreEqual(Command.Acosh, Command.Cos.TrigBareToDressed(true, true), "ACOSH");

            Assert.AreEqual(Command.Tan, Command.Tan.TrigBareToDressed(false, false), "TAN");
            Assert.AreEqual(Command.Atan, Command.Tan.TrigBareToDressed(true, false), "ATAN");
            Assert.AreEqual(Command.Tanh, Command.Tan.TrigBareToDressed(false, true), "TANH");
            Assert.AreEqual(Command.Atanh, Command.Tan.TrigBareToDressed(true, true), "ATANH");
        }

        [Test]
        public void IsValidButtonTest()
        {
            Assert.AreEqual(false, Command.Eq.IsValidButton(Settings.Mode.Rpn), "EQ-RPN");
            Assert.AreEqual(false, Command.LeftParen.IsValidButton(Settings.Mode.Rpn), "LEFT_PAREN-RPN");
            Assert.AreEqual(false, Command.RightParen.IsValidButton(Settings.Mode.Rpn), "RIGHT_PAREN-RPN");

            Assert.AreEqual(true, Command.Enter.IsValidButton(Settings.Mode.Rpn), "ENTER-RPN");
            Assert.AreEqual(true, Command.StackDown.IsValidButton(Settings.Mode.Rpn), "STACK_DOWN-RPN");
            Assert.AreEqual(true, Command.StackUp.IsValidButton(Settings.Mode.Rpn), "STACK_UP-RPN");

            Assert.AreEqual(true, Command.Eq.IsValidButton(Settings.Mode.Alg), "EQ-ALG");
            Assert.AreEqual(true, Command.LeftParen.IsValidButton(Settings.Mode.Alg), "LEFT_PAREN-ALG");
            Assert.AreEqual(true, Command.RightParen.IsValidButton(Settings.Mode.Alg), "RIGHT_PAREN-ALG");

            Assert.AreEqual(false, Command.Enter.IsValidButton(Settings.Mode.Alg), "ENTER-ALG");
            Assert.AreEqual(false, Command.StackDown.IsValidButton(Settings.Mode.Alg), "STACK_DOWN-ALG");
            Assert.AreEqual(false, Command.StackUp.IsValidButton(Settings.Mode.Alg), "STACK_UP-ALG");
        }
    }
}
