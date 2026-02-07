using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Calcoo.Test
{
    [TestFixture]
    internal class CommandTest
    {
        [Test]
        public void TrigBareToDressedTest()
        {
            ClassicAssert.AreEqual(Command.Sin, Command.Sin.TrigBareToDressed(false, false), "SIN");
            ClassicAssert.AreEqual(Command.Asin, Command.Sin.TrigBareToDressed(true, false), "ASIN");
            ClassicAssert.AreEqual(Command.Sinh, Command.Sin.TrigBareToDressed(false, true), "SINH");
            ClassicAssert.AreEqual(Command.Asinh, Command.Sin.TrigBareToDressed(true, true), "ASINH");

            ClassicAssert.AreEqual(Command.Cos, Command.Cos.TrigBareToDressed(false, false), "COS");
            ClassicAssert.AreEqual(Command.Acos, Command.Cos.TrigBareToDressed(true, false), "ACOS");
            ClassicAssert.AreEqual(Command.Cosh, Command.Cos.TrigBareToDressed(false, true), "COSH");
            ClassicAssert.AreEqual(Command.Acosh, Command.Cos.TrigBareToDressed(true, true), "ACOSH");

            ClassicAssert.AreEqual(Command.Tan, Command.Tan.TrigBareToDressed(false, false), "TAN");
            ClassicAssert.AreEqual(Command.Atan, Command.Tan.TrigBareToDressed(true, false), "ATAN");
            ClassicAssert.AreEqual(Command.Tanh, Command.Tan.TrigBareToDressed(false, true), "TANH");
            ClassicAssert.AreEqual(Command.Atanh, Command.Tan.TrigBareToDressed(true, true), "ATANH");
        }

        [Test]
        public void IsValidButtonTest()
        {
            ClassicAssert.AreEqual(false, Command.Eq.IsValidButton(Settings.Mode.Rpn), "EQ-RPN");
            ClassicAssert.AreEqual(false, Command.LeftParen.IsValidButton(Settings.Mode.Rpn), "LEFT_PAREN-RPN");
            ClassicAssert.AreEqual(false, Command.RightParen.IsValidButton(Settings.Mode.Rpn), "RIGHT_PAREN-RPN");

            ClassicAssert.AreEqual(true, Command.Enter.IsValidButton(Settings.Mode.Rpn), "ENTER-RPN");
            ClassicAssert.AreEqual(true, Command.StackDown.IsValidButton(Settings.Mode.Rpn), "STACK_DOWN-RPN");
            ClassicAssert.AreEqual(true, Command.StackUp.IsValidButton(Settings.Mode.Rpn), "STACK_UP-RPN");

            ClassicAssert.AreEqual(true, Command.Eq.IsValidButton(Settings.Mode.Alg), "EQ-ALG");
            ClassicAssert.AreEqual(true, Command.LeftParen.IsValidButton(Settings.Mode.Alg), "LEFT_PAREN-ALG");
            ClassicAssert.AreEqual(true, Command.RightParen.IsValidButton(Settings.Mode.Alg), "RIGHT_PAREN-ALG");

            ClassicAssert.AreEqual(false, Command.Enter.IsValidButton(Settings.Mode.Alg), "ENTER-ALG");
            ClassicAssert.AreEqual(false, Command.StackDown.IsValidButton(Settings.Mode.Alg), "STACK_DOWN-ALG");
            ClassicAssert.AreEqual(false, Command.StackUp.IsValidButton(Settings.Mode.Alg), "STACK_UP-ALG");
        }
    }
}
