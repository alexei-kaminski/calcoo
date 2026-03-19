using NUnit.Framework;

namespace Calcoo.Test
{
    [TestFixture]
    internal class CommandTest
    {
        [Test]
        public void TrigBareToDressedTest()
        {
            Assert.That(Command.Sin.TrigBareToDressed(false, false), Is.EqualTo(Command.Sin), "SIN");
            Assert.That(Command.Sin.TrigBareToDressed(true, false), Is.EqualTo(Command.Asin), "ASIN");
            Assert.That(Command.Sin.TrigBareToDressed(false, true), Is.EqualTo(Command.Sinh), "SINH");
            Assert.That(Command.Sin.TrigBareToDressed(true, true), Is.EqualTo(Command.Asinh), "ASINH");

            Assert.That(Command.Cos.TrigBareToDressed(false, false), Is.EqualTo(Command.Cos), "COS");
            Assert.That(Command.Cos.TrigBareToDressed(true, false), Is.EqualTo(Command.Acos), "ACOS");
            Assert.That(Command.Cos.TrigBareToDressed(false, true), Is.EqualTo(Command.Cosh), "COSH");
            Assert.That(Command.Cos.TrigBareToDressed(true, true), Is.EqualTo(Command.Acosh), "ACOSH");

            Assert.That(Command.Tan.TrigBareToDressed(false, false), Is.EqualTo(Command.Tan), "TAN");
            Assert.That(Command.Tan.TrigBareToDressed(true, false), Is.EqualTo(Command.Atan), "ATAN");
            Assert.That(Command.Tan.TrigBareToDressed(false, true), Is.EqualTo(Command.Tanh), "TANH");
            Assert.That(Command.Tan.TrigBareToDressed(true, true), Is.EqualTo(Command.Atanh), "ATANH");
        }

        [Test]
        public void IsValidButtonTest()
        {
            Assert.That(Command.Eq.IsValidButton(Settings.ModeType.Rpn), Is.EqualTo(false), "EQ-RPN");
            Assert.That(Command.LeftParen.IsValidButton(Settings.ModeType.Rpn), Is.EqualTo(false), "LEFT_PAREN-RPN");
            Assert.That(Command.RightParen.IsValidButton(Settings.ModeType.Rpn), Is.EqualTo(false), "RIGHT_PAREN-RPN");

            Assert.That(Command.Enter.IsValidButton(Settings.ModeType.Rpn), Is.EqualTo(true), "ENTER-RPN");
            Assert.That(Command.StackDown.IsValidButton(Settings.ModeType.Rpn), Is.EqualTo(true), "STACK_DOWN-RPN");
            Assert.That(Command.StackUp.IsValidButton(Settings.ModeType.Rpn), Is.EqualTo(true), "STACK_UP-RPN");

            Assert.That(Command.Eq.IsValidButton(Settings.ModeType.Alg), Is.EqualTo(true), "EQ-ALG");
            Assert.That(Command.LeftParen.IsValidButton(Settings.ModeType.Alg), Is.EqualTo(true), "LEFT_PAREN-ALG");
            Assert.That(Command.RightParen.IsValidButton(Settings.ModeType.Alg), Is.EqualTo(true), "RIGHT_PAREN-ALG");

            Assert.That(Command.Enter.IsValidButton(Settings.ModeType.Alg), Is.EqualTo(false), "ENTER-ALG");
            Assert.That(Command.StackDown.IsValidButton(Settings.ModeType.Alg), Is.EqualTo(false), "STACK_DOWN-ALG");
            Assert.That(Command.StackUp.IsValidButton(Settings.ModeType.Alg), Is.EqualTo(false), "STACK_UP-ALG");
        }
    }
}
