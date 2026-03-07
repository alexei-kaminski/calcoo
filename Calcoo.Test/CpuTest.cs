using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Calcoo.Test
{
    [TestFixture]
    internal class CpuTest
    {
        private const int InputLength = 10;
        private const int ExpInputLength = 2;
        private const int NumBase = 10;
        private const int NMem = 2;
        private const Settings.EnterMode DefaultEnterMode = Settings.EnterMode.Traditional;
        private const Settings.StackMode DefaultStackMode = Settings.StackMode.Infinite;

        // testing public functions

        [Test]
        public void CloneTest()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode,
                DefaultStackMode);

            String[] setupKeystrokes =
            {
                "Mem0", "Digit7", "XToMem", "Mem1", "Digit5", "Enter", "XToMem",
                "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5"
            };
            foreach (String ks in setupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            Cpu clonedCpu = cpu.Clone();

            var cpus = new List<Cpu> {cpu, clonedCpu};

            var results = new double[2];
            String[] testKeystrokes = {"Div", "Div", "Div", "Mem0", "MemToX", "Div", "Mem1", "MemToX", "Div"};

            for (int i = 0; i < 2; ++i)
            {
                foreach (String ks in testKeystrokes)
                    cpus[i].Execute((Command) Enum.Parse(typeof (Command), ks));
                results[i] = cpus[i].X;
            }

            Assert.That(results[1], Is.EqualTo(results[0]).Within(Cpu.CpuPrecision), "command sequence 0");
        }

        [Test]
        public void IsInputInProgressTest()
        {
            // RPN
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            Assert.That(cpu.IsInputInProgress(), Is.EqualTo(true), "RPN - blank");
            cpu.Execute(Command.Digit1);
            Assert.That(cpu.IsInputInProgress(), Is.EqualTo(true), "RPN - after digit");
            cpu.Execute(Command.Sqrt);
            Assert.That(cpu.IsInputInProgress(), Is.EqualTo(false), "RPN - after unary op");
            cpu.Execute(Command.Digit2);
            Assert.That(cpu.IsInputInProgress(), Is.EqualTo(true), "RPN - after digit again");
            cpu.Execute(Command.Enter);
            Assert.That(cpu.IsInputInProgress(), Is.EqualTo(false), "RPN - after enter");
            cpu.Execute(Command.ClearAll);
            Assert.That(cpu.IsInputInProgress(), Is.EqualTo(true), "RPN - after clear");
            // ALG
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            Assert.That(cpu.IsInputInProgress(), Is.EqualTo(true), "ALG - blank");
            cpu.Execute(Command.Digit1);
            Assert.That(cpu.IsInputInProgress(), Is.EqualTo(true), "ALG - after digit");
            cpu.Execute(Command.Sqrt);
            Assert.That(cpu.IsInputInProgress(), Is.EqualTo(false), "ALG - after unary op");
            cpu.Execute(Command.Digit2);
            Assert.That(cpu.IsInputInProgress(), Is.EqualTo(true), "ALG - after digit again");
            cpu.Execute(Command.Add);
            Assert.That(cpu.IsInputInProgress(), Is.EqualTo(false), "ALG - after binop");
            cpu.Execute(Command.ClearAll);
            Assert.That(cpu.IsInputInProgress(), Is.EqualTo(true), "ALG - after clear");
        }

        [Test]
        public void GetStackTest()
        {
            // RPN
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            String[] rpnSetupKeystrokes = {"Digit7", "Dot", "Digit2", "Digit3", "Enter"};
            foreach (String ks in rpnSetupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            // just making sure we are getting access to the stack - the specific
            // stack functionality
            // is tested in its own tests
            Assert.That(cpu.GetStack().PeekValue(0), Is.EqualTo(7.23).Within(Cpu.CpuPrecision), "RPN");

            // ALG
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            String[] algSetupKeystrokes = {"Digit7", "Dot", "Digit2", "Digit3", "Add"};
            foreach (String ks in algSetupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            // just making sure we are getting access to the stack - the specific
            // stack functionality
            // is tested in its own tests
            Assert.That(cpu.GetStack().PeekValue(0), Is.EqualTo(7.23).Within(Cpu.CpuPrecision), "ALG");
        }

        [Test]
        public void GetInputTest()
        {
            // RPN
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            String[] rpnSetupKeystrokes = {"Digit7", "Dot", "Digit2", "Digit3"};
            foreach (String ks in rpnSetupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            Assert.That(cpu.GetInput().ToDouble(NumBase), Is.EqualTo(7.23).Within(Cpu.CpuPrecision), "RPN");

            // ALG
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode,
                DefaultStackMode);

            String[] algSetupKeystrokes = {"Digit7", "Dot", "Digit2", "Digit3"};
            foreach (String ks in algSetupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            Assert.That(cpu.GetInput().ToDouble(NumBase), Is.EqualTo(7.23).Within(Cpu.CpuPrecision), "ALG");
        }

        [Test]
        public void GetXTest()
        {
            // RPN
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            String[] rpnSetupKeystrokes = {"Digit7", "Dot", "Digit2", "Digit3", "Enter"};
            foreach (String ks in rpnSetupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            Assert.That(cpu.X, Is.EqualTo(7.23).Within(Cpu.CpuPrecision), "RPN");

            // ALG
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            String[] algSetupKeystrokes = {"Digit7", "Dot", "Digit2", "Digit3", "Eq"};
            foreach (String ks in algSetupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            Assert.That(cpu.X, Is.EqualTo(7.23).Within(Cpu.CpuPrecision), "ALG");
        }

        [Test]
        public void GetMemTest()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            String[] rpnSetupKeystrokes =
            {
                "Digit7", "Dot", "Digit2", "Digit3", "XToMem", "Digit1", "Add",
                "Mem1", "XToMem"
            };
            foreach (String ks in rpnSetupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            // just making sure we are getting access to the stack - the specific
            // stack functionality
            // is tested in its own tests
            Assert.That(cpu.GetMem(0), Is.EqualTo(7.23).Within(Cpu.CpuPrecision), "mem0");
            Assert.That(cpu.GetMem(1), Is.EqualTo(8.23).Within(Cpu.CpuPrecision), "mem1");

            // ALG testing is deemed unnecessary, since the mem operation does not
            // have mode-specific functionality
        }

        [Test]
        public void GetActiveMemNumTest()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            Assert.That(cpu.ActiveMemNum, Is.EqualTo(0), "blank cpu");

            String[] rpnSetupKeystrokes =
            {
                "Digit7", "Dot", "Digit2", "Digit3", "XToMem", "Digit1", "Add",
                "Mem1", "XToMem"
            };
            foreach (String ks in rpnSetupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            Assert.That(cpu.ActiveMemNum, Is.EqualTo(1), "switched to 1");
            cpu.Execute(Command.Mem0);
            Assert.That(cpu.ActiveMemNum, Is.EqualTo(0), "switched back to 0");

            // ALG testing is deemed unnecessary, since the mem operation does not
            // have mode-specific functionality
        }

        [Test]
        public void GetAngleUnitsTest()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            Assert.That(cpu.AngleUnits, Is.EqualTo(Settings.AngleUnits.Deg), "blank cpu");
            cpu.Execute(Command.DegRad);
            Assert.That(cpu.AngleUnits, Is.EqualTo(Settings.AngleUnits.Rad), "switched once");
        }

        [Test]
        public void SetEnterModeTest()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            Assert.That(cpu.EnterMode, Is.EqualTo(DefaultEnterMode), "blank cpu");
            cpu.EnterMode = Settings.EnterMode.Hp28;
            Assert.That(cpu.EnterMode, Is.EqualTo(Settings.EnterMode.Hp28), "set to HP28");
            cpu.EnterMode = Settings.EnterMode.Traditional;
            Assert.That(cpu.EnterMode, Is.EqualTo(Settings.EnterMode.Traditional), "set to TRADITIONAL");
            cpu.EnterMode = Settings.EnterMode.Hp28;
            Assert.That(cpu.EnterMode, Is.EqualTo(Settings.EnterMode.Hp28), "set to HP28 again");

            Assert.That(
                RunCpu(new[]
                {
                    "Digit3", "Enter", "Digit3", "Enter", "Digit3", "Enter", "Digit3", "Enter",
                    "Digit3", "Enter", "Digit3", "Add", "Add", "Add", "Add", "Add"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(18.0).Within(1e-7), "3<Enter>3<Enter>3<Enter>3<Enter>3<Enter>3+++++");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit3", "Enter", "Digit3", "Enter", "Digit3", "Enter", "Digit3", "Enter",
                    "Digit3", "Enter", "Digit3", "Add", "Add", "Add", "Add", "Add"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(12.0).Within(1e-7), "3<Enter>3<Enter>3<Enter>3<Enter>3<Enter>3+++++");
        }

        [Test]
        public void GetEnterModeTest()
        {
            // identical to the tests in SetEnterModeTest - keeping both in case we
            // discover problems specific to the getter
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            Assert.That(cpu.EnterMode, Is.EqualTo(DefaultEnterMode), "blank cpu");
            cpu.EnterMode = Settings.EnterMode.Hp28;
            Assert.That(cpu.EnterMode, Is.EqualTo(Settings.EnterMode.Hp28), "set to HP28");
            cpu.EnterMode = Settings.EnterMode.Traditional;
            Assert.That(cpu.EnterMode, Is.EqualTo(Settings.EnterMode.Traditional), "set to TRADITIONAL");
            cpu.EnterMode = Settings.EnterMode.Hp28;
            Assert.That(cpu.EnterMode, Is.EqualTo(Settings.EnterMode.Hp28), "set to HP28 again");
        }

        [Test]
        public void SetStackModeTest()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            Assert.That(cpu.StackMode, Is.EqualTo(DefaultStackMode), "blank cpu");
            cpu.StackMode = Settings.StackMode.Xyzt;
            Assert.That(cpu.StackMode, Is.EqualTo(Settings.StackMode.Xyzt), "set to XYZT");
            cpu.StackMode = Settings.StackMode.Infinite;
            Assert.That(cpu.StackMode, Is.EqualTo(Settings.StackMode.Infinite), "set to INFINITE");
            cpu.StackMode = Settings.StackMode.Xyzt;
            Assert.That(cpu.StackMode, Is.EqualTo(Settings.StackMode.Xyzt), "set to XYZT again");

            Assert.That(
                RunCpu(new[] {"Digit3", "Enter", "Add"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(6.0).Within(1e-7), "3<Enter>+");
            Assert.That(
                RunCpu(new[] {"Digit3", "Enter", "Add"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Hp28, Settings.StackMode.Infinite),
                Is.EqualTo(3.0).Within(1e-7), "3<Enter>+");
        }

        [Test]
        public void GetStackModeTest()
        {
            // identical to the tests in SetStackModeTest - keeping both in case we
            // discover problems specific to the getter
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            Assert.That(cpu.StackMode, Is.EqualTo(DefaultStackMode), "blank cpu");
            cpu.StackMode = Settings.StackMode.Xyzt;
            Assert.That(cpu.StackMode, Is.EqualTo(Settings.StackMode.Xyzt), "set to XYZT");
            cpu.StackMode = Settings.StackMode.Infinite;
            Assert.That(cpu.StackMode, Is.EqualTo(Settings.StackMode.Infinite), "set to INFINITE");
            cpu.StackMode = Settings.StackMode.Xyzt;
            Assert.That(cpu.StackMode, Is.EqualTo(Settings.StackMode.Xyzt), "set to XYZT again");
        }

        [Test]
        public void SetModeTest()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            Assert.That(cpu.Mode, Is.EqualTo(Settings.Mode.Rpn), "blank cpu RPN");
            cpu.Mode = Settings.Mode.Alg;
            Assert.That(cpu.Mode, Is.EqualTo(Settings.Mode.Alg), "RPN - set to ALG");
            cpu.Mode = Settings.Mode.Rpn;
            Assert.That(cpu.Mode, Is.EqualTo(Settings.Mode.Rpn), "RPN - set to RPN again");

            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            Assert.That(cpu.Mode, Is.EqualTo(Settings.Mode.Alg), "blank cpu ALG");
            cpu.Mode = Settings.Mode.Rpn;
            Assert.That(cpu.Mode, Is.EqualTo(Settings.Mode.Rpn), "ALG - set to RPN");
            cpu.Mode = Settings.Mode.Alg;
            Assert.That(cpu.Mode, Is.EqualTo(Settings.Mode.Alg), "ALG - set to ALG again");
        }

        [Test]
        public void GetModeTest()
        {
            // identical to the tests in SetModeTest - keeping both in case we
            // discover problems specific to the getter
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            Assert.That(cpu.Mode, Is.EqualTo(Settings.Mode.Rpn), "blank cpu RPN");
            cpu.Mode = Settings.Mode.Alg;
            Assert.That(cpu.Mode, Is.EqualTo(Settings.Mode.Alg), "RPN - set to ALG");
            cpu.Mode = Settings.Mode.Rpn;
            Assert.That(cpu.Mode, Is.EqualTo(Settings.Mode.Rpn), "RPN - set to RPN again");

            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            Assert.That(cpu.Mode, Is.EqualTo(Settings.Mode.Alg), "blank cpu ALG");
            cpu.Mode = Settings.Mode.Rpn;
            Assert.That(cpu.Mode, Is.EqualTo(Settings.Mode.Rpn), "ALG - set to RPN");
            cpu.Mode = Settings.Mode.Alg;
            Assert.That(cpu.Mode, Is.EqualTo(Settings.Mode.Alg), "ALG - set to ALG again");
        }

        // indirect testing of private functions

        [Test]
        public void ExecuteDigitTest()
        {
            // trivial input
            Assert.That(
                RunCpu(new[] {"Digit8", "Digit6", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(86.0).Within(1e-17), "86");

            // special cases
            // empty
            var cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            Assert.That(cpu.GetInput().GetNIntDigits(), Is.EqualTo(0), "empty");
            // input starts with 0
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            cpu.Execute(Command.Digit0);
            Assert.That(cpu.GetInput().GetNIntDigits(), Is.EqualTo(0), "input starts with 0 - 1");
            Assert.That(cpu.GetInput().GetNFracDigits(), Is.EqualTo(0), "input starts with 0 - 1 - frac");
            cpu.Execute(Command.Digit1);
            Assert.That(cpu.GetInput().GetNIntDigits(), Is.EqualTo(1), "input starts with 0 - 2");
            // input starts with dot
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            cpu.Execute(Command.Dot);
            Assert.That(cpu.GetInput().GetNIntDigits(), Is.EqualTo(1), "input starts with dot - 1");
            Assert.That(cpu.GetInput().GetIntDigit(0), Is.EqualTo(0), "input starts with dot - 1 - int digit");
            Assert.That(cpu.GetInput().GetNFracDigits(), Is.EqualTo(0), "input starts with dot - 1 - frac");
            cpu.Execute(Command.Digit1);
            Assert.That(cpu.GetInput().GetNIntDigits(), Is.EqualTo(1), "input starts with dot - 2 - int");
            Assert.That(cpu.GetInput().GetIntDigit(0), Is.EqualTo(0), "input starts with dot - 2 - int digit");
            Assert.That(cpu.GetInput().GetNFracDigits(), Is.EqualTo(1), "input starts with dot - 2 - frac");
            Assert.That(cpu.GetInput().GetFracDigit(0), Is.EqualTo(1), "input starts with dot - 2 - frac digit");
            // input starts with exp
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            cpu.Execute(Command.Exp);
            Assert.That(cpu.GetInput().GetNIntDigits(), Is.EqualTo(1), "input starts with exp - 1");
            Assert.That(cpu.GetInput().GetIntDigit(0), Is.EqualTo(1), "input starts with exp - 1 - int digit");
            Assert.That(cpu.GetInput().GetNFracDigits(), Is.EqualTo(0), "input starts with exp - 1 - frac");
            Assert.That(cpu.GetInput().GetNExpDigits(), Is.EqualTo(ExpInputLength), "input starts with exp - 1 - exp");
            cpu.Execute(Command.Digit1);
            Assert.That(cpu.GetInput().GetNIntDigits(), Is.EqualTo(1), "input starts with exp - 2 - int");
            Assert.That(cpu.GetInput().GetIntDigit(0), Is.EqualTo(1), "input starts with exp - 2 - int digit");
            Assert.That(cpu.GetInput().GetNFracDigits(), Is.EqualTo(0), "input starts with exp - 2 - frac");
            Assert.That(cpu.GetInput().GetNExpDigits(), Is.EqualTo(ExpInputLength), "input starts with exp - 2 - exp");
            Assert.That(cpu.GetInput().GetExpDigit(ExpInputLength - 1), Is.EqualTo(1), "input starts with exp - 2 - exp digit");
            // input starts with sign
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            cpu.Execute(Command.Sign);
            Assert.That(cpu.GetInput().GetNIntDigits(), Is.EqualTo(0), "input starts with sign - 1");
            Assert.That(cpu.GetInput().GetSign(), Is.EqualTo(-1), "input starts with sign - 1 - sign");
            cpu.Execute(Command.Digit1);
            Assert.That(cpu.GetInput().GetNIntDigits(), Is.EqualTo(1), "input starts with sign - 2");
            Assert.That(cpu.GetInput().GetIntDigit(0), Is.EqualTo(1), "input starts with sign - 2");
            // the test below is non-trivial. Contrary to what one may expect, one cannot start number input with the sign.
            // This behavior is the same as in a regular calculator.
            Assert.That(cpu.GetInput().GetSign(), Is.EqualTo(1), "input starts with sign - 2 - sign");
        }

        [Test]
        public void ExecuteDotTest()
        {
            Assert.That(
                RunCpu(new[] {"Digit8", "Dot", "Digit6", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(8.6).Within(Cpu.CpuPrecision*8.6), "8.6");
            // number entry starting with a dot
            Assert.That(
                RunCpu(new[] {"Dot", "Digit6", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(0.6).Within(Cpu.CpuPrecision*0.6), "0.6");
        }

        [Test]
        public void ExecuteMantissaSignTest()
        {
            Assert.That(
                RunCpu(new[] {"Digit8", "Digit6", "MantissaSign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(-86.0).Within(1e-17), "-86");

            Assert.That(
                RunCpu(new[] {"Digit8", "Digit6", "MantissaSign", "MantissaSign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(86.0).Within(1e-17), "--86");

            Assert.That(
                RunCpu(new[] {"Digit8", "Digit6", "Exp", "Digit1", "MantissaSign", "MantissaSign", "Eq"},
                    Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(860).Within(1e-17), "--86e1");

            Assert.That(
                RunCpu(new[] {"Digit8", "Digit6", "Exp", "Digit1", "MantissaSign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(-860).Within(1e-17), "-86e1");
        }

        [Test]
        public void ExecuteCurrentSignTest()
        {
            Assert.That(
                RunCpu(new[] {"Digit8", "Digit6", "Sign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(-86.0).Within(1e-17), "-86");

            Assert.That(
                RunCpu(new[] {"Digit8", "Digit6", "Sign", "Sign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(86.0).Within(1e-17), "--86");

            Assert.That(
                RunCpu(new[] {"Digit8", "Digit6", "Dot", "Digit3", "Sign", "Eq"},
                    Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(-86.3).Within(1e-17), "-86.3");

            Assert.That(
                RunCpu(new[] {"Digit8", "Digit6", "Dot", "Digit3", "Sign", "Sign", "Eq"},
                    Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(86.3).Within(1e-17), "--86.3");

            Assert.That(
                RunCpu(new[] {"Digit8", "Digit6", "Dot", "Digit3", "Exp", "Digit2", "Sign", "Eq"},
                    Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(86.3e-2).Within(1e-17), "86.3e-2");

            Assert.That(
                RunCpu(new[]
                {
                    "Digit8", "Digit6", "Dot", "Digit3", "Exp", "Digit2", "Sign",
                    "Sign", "Eq"
                }, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(86.3e2).Within(1e-17), "86.3e2");

            Assert.That(
                RunCpu(new[]
                {
                    "Digit8", "Digit6", "Dot", "Sign", "Digit3", "Exp", "Digit2",
                    "Sign", "Eq"
                }, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(-86.3e-2).Within(1e-17), "-86.3e-2");
        }

        [Test]
        public void ExecuteExpTest()
        {
            Assert.That(
                RunCpu(new[] {"Digit8", "Exp", "Digit1", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(80.0).Within(1e-17), "8e1");
        }

        [Test]
        public void ExecuteExpSignTest()
        {
            Assert.That(
                RunCpu(new[] {"Digit8", "Exp", "Digit1", "ExpSign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(0.8).Within(1e-17), "8e-1");

            Assert.That(
                RunCpu(new[] {"Digit8", "Exp", "Digit1", "ExpSign", "ExpSign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(80.0).Within(1e-17), "8e--1");
        }

        [Test]
        public void ExecuteClearXTest()
        {
            Assert.That(RunCpu(new[] {"Pi", "ClearX"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                Is.EqualTo(0.0).Within(1e-17), "pi");

            Assert.That(
                RunCpu(new[] {"Digit8", "Add", "Digit6", "ClearX", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(8.0).Within(1e-17), "8+6<Cx>=");
            Assert.That(
                RunCpu(new[] {"Digit8", "Add", "Digit6", "ClearX", "Digit5", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(13.0).Within(1e-17), "8+6<Cx>5=");
        }

        [Test]
        public void ExecuteClearAllTest()
        {
            Assert.That(
                RunCpu(new[] {"Digit8", "Add", "Digit6", "ClearAll", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(0.0).Within(1e-17), "8+6<CA>=");
            Assert.That(
                RunCpu(new[] {"Digit8", "Add", "Digit6", "ClearAll", "Digit5", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(5.0).Within(1e-17), "8+6<CA>5=");
        }

        [Test]
        public void ExecutePiTest()
        {
            Assert.That(RunCpu(new[] {"Pi"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                Is.EqualTo(Math.PI).Within(Cpu.CpuPrecision*1e-5), "pi");
        }

        [Test]
        public void ExecutePasteTest()
        {
            // RPN
            Cpu cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            cpu.ExecutePaste(2.12);
            Assert.That(cpu.X, Is.EqualTo(2.12).Within(Cpu.CpuPrecision), "RPN on blank");
            cpu.Execute(Command.Digit1);
            cpu.Execute(Command.Add);
            Assert.That(cpu.X, Is.EqualTo(3.12).Within(Cpu.CpuPrecision), "RPN number on paste");
            cpu.ExecutePaste(3.12);
            cpu.Execute(Command.Add);
            Assert.That(cpu.X, Is.EqualTo(6.24).Within(Cpu.CpuPrecision), "RPN on op");
            cpu.Execute(Command.Digit1);
            cpu.ExecutePaste(3.12);
            cpu.Execute(Command.Add);
            Assert.That(cpu.X, Is.EqualTo(4.12).Within(Cpu.CpuPrecision), "RPN on input in progress 1");
            cpu.Execute(Command.Add);
            Assert.That(cpu.X, Is.EqualTo(10.36).Within(Cpu.CpuPrecision), "RPN on input in progress 2");

            // ALG
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            cpu.ExecutePaste(2.12);
            Assert.That(cpu.X, Is.EqualTo(2.12).Within(Cpu.CpuPrecision), "ALG on blank");
            cpu.Execute(Command.Add);
            cpu.Execute(Command.Digit1);
            cpu.Execute(Command.Add);
            cpu.ExecutePaste(5.12);
            cpu.Execute(Command.Add);
            Assert.That(cpu.X, Is.EqualTo(8.24).Within(Cpu.CpuPrecision), "ALG on op");
            cpu.Execute(Command.Digit1);
            cpu.ExecutePaste(1.12);
            cpu.Execute(Command.Eq);
            Assert.That(cpu.X, Is.EqualTo(9.36).Within(Cpu.CpuPrecision), "ALG on input in progress");
        }

        [Test]
        public void ExecuteBinaryOpTest()
        {
            // ALG mode
            // simple operations
            Assert.That(
                RunCpu(new[] {"Digit2", "Add", "Digit3", "Eq"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo((2.0 + 3.0)).Within(1e-7), "2+3");
            Assert.That(
                RunCpu(new[] {"Digit2", "Sub", "Digit3", "Eq"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo((2.0 - 3.0)).Within(1e-7), "2-3");
            Assert.That(
                RunCpu(new[] {"Digit2", "Mul", "Digit3", "Eq"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo((2.0*3.0)).Within(1e-7), "2*3");
            Assert.That(
                RunCpu(new[] {"Digit2", "Div", "Digit3", "Eq"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo((2.0/3.0)).Within(1e-7), "2/3");
            Assert.That(
                RunCpu(new[] {"Digit2", "Pow", "Digit3", "Eq"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Pow(2.0, 3.0)).Within(1e-7), "2^3");
            // binop priority
            Assert.That(
                RunCpu(new[] {"Digit2", "Add", "Digit3", "Add"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo((2.0 + 3.0)).Within(1e-7), "2+3+");
            Assert.That(
                RunCpu(new[] {"Digit2", "Add", "Digit3", "Mul"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo((3.0)).Within(1e-7), "2+3*");
            Assert.That(
                RunCpu(new[] {"Digit2", "Mul", "Digit3", "Mul"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo((2.0*3.0)).Within(1e-7), "2*3*");
            Assert.That(
                RunCpu(new[] {"Digit2", "Mul", "Digit3", "Pow"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo((3.0)).Within(1e-7), "2*3^");
            Assert.That(
                RunCpu(new[] {"Digit1", "Add", "Digit2", "Mul", "Digit3", "Pow", "Digit4", "Eq"},
                    Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo((163.0)).Within(1e-7), "1+2*3^4");
            // RPN mode - traditional stack
            // simple operations
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Add"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite),
                Is.EqualTo((2.0 + 3.0)).Within(1e-7), "2 3 +");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Sub"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite),
                Is.EqualTo((2.0 - 3.0)).Within(1e-7), "2 3 -");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Mul"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite),
                Is.EqualTo((2.0*3.0)).Within(1e-7), "2 3 *");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Div"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite),
                Is.EqualTo((2.0/3.0)).Within(1e-7), "2 3 /");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Pow"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite),
                Is.EqualTo(Math.Pow(2.0, 3.0)).Within(1e-7), "2 3 ^");
            // enter followed by op
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Add"}, Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite),
                Is.EqualTo((2.0 + 2.0)).Within(1e-7), "2 +");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Mul"}, Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite),
                Is.EqualTo((2.0*2.0)).Within(1e-7), "2 *");
            // operation chains
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "Add", "Ln", "Mul"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite),
                Is.EqualTo(2.0*Math.Log(3.0 + 4.0)).Within(1e-7), "2 3 4 + ln *");
            // RPN mode - HP28 stack
            // simple operations - same as traditional stack
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Add"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite),
                Is.EqualTo((2.0 + 3.0)).Within(1e-7), "2 3 +");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Sub"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite),
                Is.EqualTo((2.0 - 3.0)).Within(1e-7), "2 3 -");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Mul"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite),
                Is.EqualTo((2.0*3.0)).Within(1e-7), "2 3 *");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Div"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite),
                Is.EqualTo((2.0/3.0)).Within(1e-7), "2 3 /");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Pow"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite),
                Is.EqualTo(Math.Pow(2.0, 3.0)).Within(1e-7), "2 3 ^");
            // enter followed by op - behavior different from traditional stack
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Add"}, Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite),
                Is.EqualTo((2.0)).Within(1e-7), "2 +");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Mul"}, Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite),
                Is.EqualTo((0.0)).Within(1e-7), "2 *");
            // operation chains - same as traditional stack
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "Add", "Ln", "Mul"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite),
                Is.EqualTo(2.0*Math.Log(3.0 + 4.0)).Within(1e-7), "2 3 4 + ln *");
            // non-trivial cases - testing only RPN since the behavior is determined
            // by the low-level routines which are shared between RPN and ALG modes
            // less simple cases
            Assert.That(
                RunCpu(new[] {"Digit2", "Dot", "Digit3", "Enter", "Digit0", "Pow"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite),
                Is.EqualTo(1.0).Within(Cpu.CpuPrecision), "2.3 ^ 0.0");
            Assert.That(
                RunCpu(new[] {"Digit0", "Enter", "Digit4", "Dot", "Digit6", "Pow"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite),
                Is.EqualTo(0.0).Within(Cpu.CpuPrecision*1e-15), "0.0 ^ 4.6");
            // precision smart sum
            Assert.That(
                RunCpu(new[]
                {
                    "Digit1", "Digit0", "Digit0", "Dot", "Digit1", "Enter",
                    "Digit1", "Digit0", "Digit0", "Sub",
                    "Digit0", "Dot", "Digit1", "Sub"
                },
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite),
                Is.EqualTo(0.0).Within(Cpu.CpuPrecision*1e-15), "100.1 - 100.0 - 0.1");
            // overflows
            Assert.That(Double.IsNaN(
                RunCpu(new[] {"Digit2", "Dot", "Digit3", "Enter", "Digit0", "Div"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite)
                ), Is.True, "2.3 / 0.0");
            Assert.That(Double.IsNaN(
                RunCpu(new[] {"Digit0", "Enter", "Digit0", "Pow"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite)
                ), Is.True, "0.0 ^ 0.0");
            Assert.That(Double.IsNaN(
                RunCpu(new[] {"Digit0", "Enter", "Digit1", "Sign", "Pow"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite)
                ), Is.True, "0.0 ^ -1.0");
            Assert.That(
                Double.IsNaN(
                    RunCpu(new[]
                    {
                        "Digit2", "Dot", "Digit3", "Sign", "Enter", "Digit1", "Dot",
                        "Digit1", "Pow"
                    },
                        Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                        Settings.EnterMode.Hp28,
                        Settings.StackMode.Infinite)
                    ), Is.True, "-2.3 ^ 1.1");
        }

        [Test]
        public void ExecuteEqTest()
        {
            Assert.That(
                RunCpu(new[] {"Digit2", "Add", "Digit3", "Eq"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo((2.0 + 3.0)).Within(1e-7), "2+3");


            Assert.That(() => RunCpu(new[] {"Eq"}, Settings.Mode.Rpn, Settings.AngleUnits.Deg),
                Throws.InstanceOf<Exception>(), "  throw at EQ in RPN mode");
        }

        [Test]
        public void ExecuteEnterTest()
        {
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Add"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo((2.0 + 3.0)).Within(1e-7), "2+3");


            Assert.That(() => RunCpu(new[] {"Enter"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Throws.InstanceOf<Exception>(), "  throw at Enter in Alg mode");
        }

        [Test]
        public void ExecuteExchXyTest()
        {
            // ALG mode
            Assert.That(
                RunCpu(new[] {"Digit2", "Div", "Digit3", "ExchXy", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo((3.0/2.0)).Within(1e-7), "2 / 3 ExchXY =");
            // RPN mode
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "ExchXy", "Div"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite),
                Is.EqualTo((3.0/2.0)).Within(1e-7), "2 3 ExchXY /");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "ExchXy", "Div"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite),
                Is.EqualTo((3.0/2.0)).Within(1e-7), "2 3 ExchXY /");
        }

        [Test]
        public void ExecuteStackUpTest()
        {
            // infinite stack
            // one number in the stack
            Assert.That(
                RunCpu(new[] {"Digit2", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 up");
            Assert.That(
                RunCpu(new[] {"Digit2", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 up up");
            Assert.That(
                RunCpu(new[] {"Digit2", "StackUp", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 up up up");
            // two numbers in the stack
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 3 up");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(3.0).Within(1e-7), "infinite: 2 3 up up");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp", "StackUp", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 3 up up up");
            // three numbers in the stack
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 3 4 up");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(3.0).Within(1e-7), "infinite: 2 3 4 up up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp",
                    "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(4.0).Within(1e-7), "infinite: 2 3 4 up up up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp",
                    "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 3 4 up up up up");
            // xyzt stack
            // one number in the stack
            Assert.That(
                RunCpu(new[] {"Digit2", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 up");
            Assert.That(
                RunCpu(new[] {"Digit2", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 up up");
            Assert.That(
                RunCpu(new[] {"Digit2", "StackUp", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 up up up");
            Assert.That(
                RunCpu(new[] {"Digit2", "StackUp", "StackUp", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 2 up up up up");
            Assert.That(
                RunCpu(new[] {"Digit2", "StackUp", "StackUp", "StackUp", "StackUp", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 up up up up up");
            // two numbers in the stack
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 3 up");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 3 up up");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp", "StackUp", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 2 3 up up up");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp", "StackUp", "StackUp", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(3.0).Within(1e-7), "xyzt: 2 3 up up up up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "StackUp", "StackUp", "StackUp", "StackUp",
                    "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 3 up up up up up");
            // three numbers in the stack
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 3 4 up");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 2 3 4 up up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp",
                    "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(3.0).Within(1e-7), "xyzt: 2 3 4 up up up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp",
                    "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(4.0).Within(1e-7), "xyzt: 2 3 4 up up up up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp",
                    "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 3 4 up up up up up");
            // four numbers in the stack
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackUp"
                },
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 2 3 4 5 up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(3.0).Within(1e-7), "xyzt: 2 3 4 5 up up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(4.0).Within(1e-7), "xyzt: 2 3 4 5 up up up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackUp", "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(5.0).Within(1e-7), "xyzt: 2 3 4 5 up up up up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackUp", "StackUp", "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 2 3 4 5 up up up up up");
            // five numbers in the stack - the highest number should not matter as
            // only the lowest four are rotated
            Assert.That(
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 7 2 3 4 5 up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(3.0).Within(1e-7), "xyzt: 2 3 4 5 up up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(4.0).Within(1e-7), "xyzt: 2 3 4 5 up up up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackUp", "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(5.0).Within(1e-7), "xyzt: 2 3 4 5 up up up up");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackUp", "StackUp", "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 2 3 4 5 up up up up up");
        }

        [Test]
        public void ExecuteStackDownTest()
        {
            // infinite stack
            // one number in the stack
            Assert.That(
                RunCpu(new[] {"Digit2", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 down");
            Assert.That(
                RunCpu(new[] {"Digit2", "StackDown", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 down down");
            Assert.That(
                RunCpu(new[] {"Digit2", "StackDown", "StackDown", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 down down down");
            // two numbers in the stack
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 3 down");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackDown", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(3.0).Within(1e-7), "infinite: 2 3 down down");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackDown", "StackDown", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 3 down down down");
            // three numbers in the stack
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(3.0).Within(1e-7), "infinite: 2 3 4 down");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(2.0).Within(1e-7), "infinite: 2 3 4 down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown",
                    "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(4.0).Within(1e-7), "infinite: 2 3 4 down down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown",
                    "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                Is.EqualTo(3.0).Within(1e-7), "infinite: 2 3 4 down down down down");
            // xyzt stack
            // one number in the stack
            Assert.That(
                RunCpu(new[] {"Digit2", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 down");
            Assert.That(
                RunCpu(new[] {"Digit2", "StackDown", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 down down");
            Assert.That(
                RunCpu(new[] {"Digit2", "StackDown", "StackDown", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 down down down");
            Assert.That(
                RunCpu(new[] {"Digit2", "StackDown", "StackDown", "StackDown", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 2 down down down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "StackDown", "StackDown", "StackDown", "StackDown",
                    "StackDown"
                },
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 down down down down down");
            // two numbers in the stack
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 2 3 down");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackDown", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 3 down down");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackDown", "StackDown", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 3 down down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "StackDown", "StackDown", "StackDown",
                    "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(3.0).Within(1e-7), "xyzt: 2 3 down down down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "StackDown", "StackDown", "StackDown",
                    "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 2 3 down down down down down");
            // three numbers in the stack
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(3.0).Within(1e-7), "xyzt: 2 3 4 down");
            Assert.That(
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 2 3 4 down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown",
                    "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(0.0).Within(1e-7), "xyzt: 2 3 4 down down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown",
                    "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(4.0).Within(1e-7), "xyzt: 2 3 4 down down down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown",
                    "StackDown", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(3.0).Within(1e-7), "xyzt: 2 3 4 down down down down down");
            // four numbers in the stack
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(4.0).Within(1e-7), "xyzt: 2 3 4 5 down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(3.0).Within(1e-7), "xyzt: 2 3 4 5 down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackDown", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 2 3 4 5 down down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackDown", "StackDown", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(5.0).Within(1e-7), "xyzt: 2 3 4 5 down down down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackDown", "StackDown", "StackDown", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(4.0).Within(1e-7), "xyzt: 2 3 4 5 down down down down down");
            // five numbers in the stack - the highest number should not matter as
            // only the lowest four are rotated
            Assert.That(
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(4.0).Within(1e-7), "xyzt: 7 2 3 4 5 down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(3.0).Within(1e-7), "xyzt: 2 3 4 5 down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackDown", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(2.0).Within(1e-7), "xyzt: 2 3 4 5 down down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackDown", "StackDown", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(5.0).Within(1e-7), "xyzt: 2 3 4 5 down down down down");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackDown", "StackDown", "StackDown", "StackDown", "StackDown"
                },
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                Is.EqualTo(4.0).Within(1e-7), "xyzt: 2 3 4 5 down down down down down");
        }

        [Test]
        public void ExecuteLeftParenTest()
        {
            Assert.That(
                RunCpu(new[] {"LeftParen", "Digit1", "Add", "Digit2", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(3.0).Within(1e-7), "EQ resets lone opening paren");
            // more tests in executeRightParenTest because the require both parens
        }

        [Test]
        public void ExecuteRightParenTest()
        {
            Assert.That(
                RunCpu(new[] {"Digit1", "Add", "Digit2", "RightParen"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(3.0).Within(1e-7), "unmatched closing paren acts as EQ");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Mul", "LeftParen", "Digit3", "Add", "Digit4",
                    "RightParen", "Eq"
                },
                    Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo(2.0*(3.0 + 4.0)).Within(1e-7), "2*(3+4)");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit2", "Pow", "LeftParen", "LeftParen", "Digit3", "Add", "Digit4",
                    "RightParen", "Mul", "Digit5", "RightParen", "Eq"
                }, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Pow(2.0, (3.0 + 4.0)*5.0)).Within(1e-7), "2^((3+4)*5)");
        }

        [Test]
        public void ExecuteUnaryOpTest()
        {
            // non-trigonometric unary ops
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit2", "Log10"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Log10(12.0)).Within(1e-7), "log10(12)");
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit0", "Ln"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Log(10.0)).Within(1e-7), "ln(10)");
            Assert.That(
                RunCpu(new[] {"Digit1", "Dot", "Digit3", "TenToX"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Pow(10.0, 1.3)).Within(1e-7), "10^1.3");
            Assert.That(
                RunCpu(new[] {"Digit1", "Dot", "Digit3", "EtoX"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Exp(1.3)).Within(1e-7), "e^1.3");
            Assert.That(
                RunCpu(new[] {"Digit7", "Digit0", "Sqrt"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Sqrt(70.0)).Within(1e-7), "sqrt(70)");
            Assert.That(
                RunCpu(new[] {"Digit1", "Dot", "Digit3", "Sqr"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo((1.3*1.3)).Within(1e-7), "1.3^2");
            Assert.That(
                RunCpu(new[] {"Digit1", "Dot", "Digit3", "InvX"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo((1.0/1.3)).Within(1e-7), "1/1.3");
            Assert.That(
                RunCpu(new[] {"Digit5", "Dot", "Digit6", "Fact"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo(MathUtil.Fact(5.6, InputLength)).Within(1e-7), "5.6!");

            // trigonometric - degrees
            const double dr = Math.PI/180.0;
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit0", "Sin"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Sin(10.0*dr)).Within(1e-7), "sin(10 deg)");
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit0", "Cos"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Cos(10.0*dr)).Within(1e-7), "cos(10 deg)");
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit0", "Tan"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Tan(10.0*dr)).Within(1e-7), "tan(10 deg)");
            // special cases
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit8", "Digit0", "Sin"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(0.0).Within(Cpu.CpuPrecision*1e-5), "sin(180 deg)");
            Assert.That(
                RunCpu(new[] {"Digit9", "Digit0", "Cos"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo(0.0).Within(Cpu.CpuPrecision*1e-5), "cos(90 deg)");
            Assert.That(
                Double.IsNaN(RunCpu(new[] {"Digit9", "Digit0", "Tan"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg)), Is.True, "tan(90 deg)");
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit8", "Digit0", "Tan"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(0.0).Within(Cpu.CpuPrecision*1e-5), "tan(180 deg)");

            // trigonometric - radians
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit0", "Sin"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                Is.EqualTo(Math.Sin(10.0)).Within(1e-7), "sin(10 rad)");
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit0", "Cos"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                Is.EqualTo(Math.Cos(10.0)).Within(1e-7), "cos(10 rad)");
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit0", "Tan"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                Is.EqualTo(Math.Tan(10.0)).Within(1e-7), "tan(10 rad)");
            // special cases
            Assert.That(RunCpu(new[] {"Pi", "Sin"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                Is.EqualTo(0.0).Within(Cpu.CpuPrecision*1e-5), "sin(pi)");

            Assert.That(
                RunCpu(new[] {"Pi", "Div", "Digit2", "Eq", "Cos"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                Is.EqualTo(0.0).Within(Cpu.CpuPrecision*1e-5), "cos(pi/2)");
            Assert.That(
                Double.IsNaN(RunCpu(new[] {"Pi", "Div", "Digit2", "Eq", "Tan"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad)), Is.EqualTo(true), "tan(pi/2)");
            Assert.That(
                RunCpu(new[] {"Pi", "Tan"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                Is.EqualTo(0.0).Within(Cpu.CpuPrecision*1e-5), "tan(180 deg)");
        }

        [Test]
        public void ExecuteMemoryOpTest()
        {
            Assert.That(
                RunCpu(new[] {"Digit3", "XToMem", "ClearX", "MemToX"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(3.0).Within(1e-7), "XToMem, CLEAR_X, MemToX");

            Assert.That(
                RunCpu(new[] {"Digit3", "XToMem", "Sqrt", "MemToX"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(3.0).Within(1e-7), "XToMem, SQRT, MemToX");
            Assert.That(
                RunCpu(new[] {"Digit3", "XToMem", "Sqrt", "ExchXMem"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(3.0).Within(1e-7), "XToMem, SQRT, EXCH_X_MEM");
            Assert.That(
                RunCpu(new[] {"Digit3", "XToMem", "Sqrt", "ExchXMem", "ClearX", "ExchXMem"},
                    Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Sqrt(3.0)).Within(1e-7), "XToMem, SQRT, EXCH_X_MEM, CLEAR_X, EXCH_X_MEM");
            Assert.That(
                RunCpu(new[] {"Digit3", "XToMem", "Sqrt", "MemPlus", "MemToX"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(3.0 + Math.Sqrt(3.0)).Within(1e-7), "XToMem, SQRT, MEM_PLUS, MemToX");
        }

        [Test]
        public void ExecuteSwitchToMemTest()
        {
            Assert.That(
                RunCpu(new[] {"Digit3", "XToMem", "ClearX", "MemToX"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(3.0).Within(1e-7), "XToMem, CLEAR_X, MemToX");
            Assert.That(
                RunCpu(new[] {"Digit3", "XToMem", "Sqrt", "Mem1", "XToMem", "Mem0", "MemToX"},
                    Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(3.0).Within(1e-7), "XToMem, SQRT, Mem1, XToMem, Mem0, MemToX");
            Assert.That(
                RunCpu(new[]
                {
                    "Digit3", "XToMem", "Sqrt", "Mem1", "XToMem", "Mem0", "MemToX", "Mem1",
                    "MemToX"
                }, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Sqrt(3.0)).Within(1e-7), "XToMem, SQRT, Mem1, XToMem, Mem0, MemToX, Mem1, MemToX");
        }

        [Test]
        public void ExecuteDegRadTest()
        {
            const double dr = Math.PI/180.0;
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit0", "DegRad", "Sin"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Sin(10.0)).Within(1e-7), "sin(10 rad) - one deg/rad switch");
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit0", "DegRad", "Sin"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(Math.Sin(10.0*dr)).Within(1e-7), "sin(10 deg) - one deg/rad switch");
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit0", "DegRad", "DegRad", "Sin"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Is.EqualTo(Math.Sin(10.0*dr)).Within(1e-7), "sin(10 deg) - two deg/rad switches");
            Assert.That(
                RunCpu(new[] {"Digit1", "Digit0", "DegRad", "DegRad", "Sin"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Is.EqualTo(Math.Sin(10.0)).Within(1e-7), "sin(10 rad) - two deg/rad switches");
        }

        // framework for testing cpu operations
        private static double RunCpu(String[] keystrokes,
            Settings.Mode mode,
            Settings.AngleUnits angleUnits,
            Settings.EnterMode enterMode,
            Settings.StackMode stackMode)
        {
            var cpu = new Cpu(mode, angleUnits, InputLength, ExpInputLength, NumBase, NMem, enterMode, stackMode);
            for (int i = 0; i < keystrokes.Length; ++i)
                cpu.Execute((Command) Enum.Parse(typeof (Command), keystrokes[i]));
            return cpu.X;
        }

        private double RunCpu(String[] keystrokes,
            Settings.Mode mode,
            Settings.AngleUnits angleUnits)
        {
            if (mode != Settings.Mode.Alg)
                throw new Exception("must specify stack and enter mode explicitly for the RPN mode tests");
            return RunCpu(keystrokes, mode, angleUnits, DefaultEnterMode, DefaultStackMode);
        }

        [Test]
        public void GetMemAtBoundaryDoesNotThrow()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            // NMem is 2, so valid indices are 0 and 1. GetMem(NMem) should be caught
            // by the guard and throw Exception with a descriptive message.
            // Bug: guard "i > _mem.Length" lets i == _mem.Length slip through,
            // causing IndexOutOfRangeException instead of the descriptive Exception.
            Assert.DoesNotThrow(() => cpu.GetMem(NMem - 1));
            var ex = Assert.Throws<Exception>(() => cpu.GetMem(NMem));
        }
    }
}
