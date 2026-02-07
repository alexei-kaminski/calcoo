using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;

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

            ClassicAssert.AreEqual(results[0], results[1], Cpu.CpuPrecision, "command sequence 0");
        }

        [Test]
        public void IsInputInProgressTest()
        {
            // RPN
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            ClassicAssert.AreEqual(true, cpu.IsInputInProgress(), "RPN - blank");
            cpu.Execute(Command.Digit1);
            ClassicAssert.AreEqual(true, cpu.IsInputInProgress(), "RPN - after digit");
            cpu.Execute(Command.Sqrt);
            ClassicAssert.AreEqual(false, cpu.IsInputInProgress(), "RPN - after unary op");
            cpu.Execute(Command.Digit2);
            ClassicAssert.AreEqual(true, cpu.IsInputInProgress(), "RPN - after digit again");
            cpu.Execute(Command.Enter);
            ClassicAssert.AreEqual(false, cpu.IsInputInProgress(), "RPN - after enter");
            cpu.Execute(Command.ClearAll);
            ClassicAssert.AreEqual(true, cpu.IsInputInProgress(), "RPN - after clear");
            // ALG
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            ClassicAssert.AreEqual(true, cpu.IsInputInProgress(), "ALG - blank");
            cpu.Execute(Command.Digit1);
            ClassicAssert.AreEqual(true, cpu.IsInputInProgress(), "ALG - after digit");
            cpu.Execute(Command.Sqrt);
            ClassicAssert.AreEqual(false, cpu.IsInputInProgress(), "ALG - after unary op");
            cpu.Execute(Command.Digit2);
            ClassicAssert.AreEqual(true, cpu.IsInputInProgress(), "ALG - after digit again");
            cpu.Execute(Command.Add);
            ClassicAssert.AreEqual(false, cpu.IsInputInProgress(), "ALG - after binop");
            cpu.Execute(Command.ClearAll);
            ClassicAssert.AreEqual(true, cpu.IsInputInProgress(), "ALG - after clear");
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
            ClassicAssert.AreEqual(7.23, cpu.GetStack().PeekValue(0), Cpu.CpuPrecision, "RPN");

            // ALG
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            String[] algSetupKeystrokes = {"Digit7", "Dot", "Digit2", "Digit3", "Add"};
            foreach (String ks in algSetupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            // just making sure we are getting access to the stack - the specific
            // stack functionality
            // is tested in its own tests
            ClassicAssert.AreEqual(7.23, cpu.GetStack().PeekValue(0), Cpu.CpuPrecision, "ALG");
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
            ClassicAssert.AreEqual(7.23, cpu.GetInput().ToDouble(NumBase), Cpu.CpuPrecision, "RPN");

            // ALG
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode,
                DefaultStackMode);

            String[] algSetupKeystrokes = {"Digit7", "Dot", "Digit2", "Digit3"};
            foreach (String ks in algSetupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            ClassicAssert.AreEqual(7.23, cpu.GetInput().ToDouble(NumBase), Cpu.CpuPrecision, "ALG");
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
            ClassicAssert.AreEqual(7.23, cpu.X, Cpu.CpuPrecision, "RPN");

            // ALG
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            String[] algSetupKeystrokes = {"Digit7", "Dot", "Digit2", "Digit3", "Eq"};
            foreach (String ks in algSetupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            ClassicAssert.AreEqual(7.23, cpu.X, Cpu.CpuPrecision, "ALG");
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
            ClassicAssert.AreEqual(7.23, cpu.GetMem(0), Cpu.CpuPrecision, "mem0");
            ClassicAssert.AreEqual(8.23, cpu.GetMem(1), Cpu.CpuPrecision, "mem1");

            // ALG testing is deemed unnecessary, since the mem operation does not
            // have mode-specific functionality
        }

        [Test]
        public void GetActiveMemNumTest()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            ClassicAssert.AreEqual(0, cpu.ActiveMemNum, "blank cpu");

            String[] rpnSetupKeystrokes =
            {
                "Digit7", "Dot", "Digit2", "Digit3", "XToMem", "Digit1", "Add",
                "Mem1", "XToMem"
            };
            foreach (String ks in rpnSetupKeystrokes)
                cpu.Execute((Command) Enum.Parse(typeof (Command), ks));
            ClassicAssert.AreEqual(1, cpu.ActiveMemNum, "switched to 1");
            cpu.Execute(Command.Mem0);
            ClassicAssert.AreEqual(0, cpu.ActiveMemNum, "switched back to 0");

            // ALG testing is deemed unnecessary, since the mem operation does not
            // have mode-specific functionality
        }

        [Test]
        public void GetAngleUnitsTest()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            ClassicAssert.AreEqual(Settings.AngleUnits.Deg, cpu.AngleUnits, "blank cpu");
            cpu.Execute(Command.DegRad);
            ClassicAssert.AreEqual(Settings.AngleUnits.Rad, cpu.AngleUnits, "switched once");
        }

        [Test]
        public void SetEnterModeTest()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            ClassicAssert.AreEqual(DefaultEnterMode, cpu.EnterMode, "blank cpu");
            cpu.EnterMode = Settings.EnterMode.Hp28;
            ClassicAssert.AreEqual(Settings.EnterMode.Hp28, cpu.EnterMode, "set to HP28");
            cpu.EnterMode = Settings.EnterMode.Traditional;
            ClassicAssert.AreEqual(Settings.EnterMode.Traditional, cpu.EnterMode, "set to TRADITIONAL");
            cpu.EnterMode = Settings.EnterMode.Hp28;
            ClassicAssert.AreEqual(Settings.EnterMode.Hp28, cpu.EnterMode, "set to HP28 again");

            ClassicAssert.AreEqual(18.0,
                RunCpu(new[]
                {
                    "Digit3", "Enter", "Digit3", "Enter", "Digit3", "Enter", "Digit3", "Enter",
                    "Digit3", "Enter", "Digit3", "Add", "Add", "Add", "Add", "Add"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                1e-7, "3<Enter>3<Enter>3<Enter>3<Enter>3<Enter>3+++++");
            ClassicAssert.AreEqual(12.0,
                RunCpu(new[]
                {
                    "Digit3", "Enter", "Digit3", "Enter", "Digit3", "Enter", "Digit3", "Enter",
                    "Digit3", "Enter", "Digit3", "Add", "Add", "Add", "Add", "Add"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt),
                1e-7, "3<Enter>3<Enter>3<Enter>3<Enter>3<Enter>3+++++");
        }

        [Test]
        public void GetEnterModeTest()
        {
            // identical to the tests in SetEnterModeTest - keeping both in case we
            // discover problems specific to the getter
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            ClassicAssert.AreEqual(DefaultEnterMode, cpu.EnterMode, "blank cpu");
            cpu.EnterMode = Settings.EnterMode.Hp28;
            ClassicAssert.AreEqual(Settings.EnterMode.Hp28, cpu.EnterMode, "set to HP28");
            cpu.EnterMode = Settings.EnterMode.Traditional;
            ClassicAssert.AreEqual(Settings.EnterMode.Traditional, cpu.EnterMode, "set to TRADITIONAL");
            cpu.EnterMode = Settings.EnterMode.Hp28;
            ClassicAssert.AreEqual(Settings.EnterMode.Hp28, cpu.EnterMode, "set to HP28 again");
        }

        [Test]
        public void SetStackModeTest()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            ClassicAssert.AreEqual(DefaultStackMode, cpu.StackMode, "blank cpu");
            cpu.StackMode = Settings.StackMode.Xyzt;
            ClassicAssert.AreEqual(Settings.StackMode.Xyzt, cpu.StackMode, "set to XYZT");
            cpu.StackMode = Settings.StackMode.Infinite;
            ClassicAssert.AreEqual(Settings.StackMode.Infinite, cpu.StackMode, "set to INFINITE");
            cpu.StackMode = Settings.StackMode.Xyzt;
            ClassicAssert.AreEqual(Settings.StackMode.Xyzt, cpu.StackMode, "set to XYZT again");

            ClassicAssert.AreEqual(6.0,
                RunCpu(new[] {"Digit3", "Enter", "Add"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                1e-7, "3<Enter>+");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit3", "Enter", "Add"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Hp28, Settings.StackMode.Infinite),
                1e-7, "3<Enter>+");
        }

        [Test]
        public void GetStackModeTest()
        {
            // identical to the tests in SetStackModeTest - keeping both in case we
            // discover problems specific to the getter
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            ClassicAssert.AreEqual(DefaultStackMode, cpu.StackMode, "blank cpu");
            cpu.StackMode = Settings.StackMode.Xyzt;
            ClassicAssert.AreEqual(Settings.StackMode.Xyzt, cpu.StackMode, "set to XYZT");
            cpu.StackMode = Settings.StackMode.Infinite;
            ClassicAssert.AreEqual(Settings.StackMode.Infinite, cpu.StackMode, "set to INFINITE");
            cpu.StackMode = Settings.StackMode.Xyzt;
            ClassicAssert.AreEqual(Settings.StackMode.Xyzt, cpu.StackMode, "set to XYZT again");
        }

        [Test]
        public void SetModeTest()
        {
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            ClassicAssert.AreEqual(Settings.Mode.Rpn, cpu.Mode, "blank cpu RPN");
            cpu.Mode = Settings.Mode.Alg;
            ClassicAssert.AreEqual(Settings.Mode.Alg, cpu.Mode, "RPN - set to ALG");
            cpu.Mode = Settings.Mode.Rpn;
            ClassicAssert.AreEqual(Settings.Mode.Rpn, cpu.Mode, "RPN - set to RPN again");

            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            ClassicAssert.AreEqual(Settings.Mode.Alg, cpu.Mode, "blank cpu ALG");
            cpu.Mode = Settings.Mode.Rpn;
            ClassicAssert.AreEqual(Settings.Mode.Rpn, cpu.Mode, "ALG - set to RPN");
            cpu.Mode = Settings.Mode.Alg;
            ClassicAssert.AreEqual(Settings.Mode.Alg, cpu.Mode, "ALG - set to ALG again");
        }

        [Test]
        public void GetModeTest()
        {
            // identical to the tests in SetModeTest - keeping both in case we
            // discover problems specific to the getter
            var cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            ClassicAssert.AreEqual(Settings.Mode.Rpn, cpu.Mode, "blank cpu RPN");
            cpu.Mode = Settings.Mode.Alg;
            ClassicAssert.AreEqual(Settings.Mode.Alg, cpu.Mode, "RPN - set to ALG");
            cpu.Mode = Settings.Mode.Rpn;
            ClassicAssert.AreEqual(Settings.Mode.Rpn, cpu.Mode, "RPN - set to RPN again");

            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            ClassicAssert.AreEqual(Settings.Mode.Alg, cpu.Mode, "blank cpu ALG");
            cpu.Mode = Settings.Mode.Rpn;
            ClassicAssert.AreEqual(Settings.Mode.Rpn, cpu.Mode, "ALG - set to RPN");
            cpu.Mode = Settings.Mode.Alg;
            ClassicAssert.AreEqual(Settings.Mode.Alg, cpu.Mode, "ALG - set to ALG again");
        }

        // indirect testing of private functions

        [Test]
        public void ExecuteDigitTest()
        {
            // trivial input
            ClassicAssert.AreEqual(86.0,
                RunCpu(new[] {"Digit8", "Digit6", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "86");

            // special cases
            // empty
            var cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            ClassicAssert.AreEqual(0, cpu.GetInput().GetNIntDigits(), "empty");
            // input starts with 0
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            cpu.Execute(Command.Digit0);
            ClassicAssert.AreEqual(0, cpu.GetInput().GetNIntDigits(), "input starts with 0 - 1");
            ClassicAssert.AreEqual(0, cpu.GetInput().GetNFracDigits(), "input starts with 0 - 1 - frac");
            cpu.Execute(Command.Digit1);
            ClassicAssert.AreEqual(1, cpu.GetInput().GetNIntDigits(), "input starts with 0 - 2");
            // input starts with dot
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            cpu.Execute(Command.Dot);
            ClassicAssert.AreEqual(1, cpu.GetInput().GetNIntDigits(), "input starts with dot - 1");
            ClassicAssert.AreEqual(0, cpu.GetInput().GetIntDigit(0), "input starts with dot - 1 - int digit");
            ClassicAssert.AreEqual(0, cpu.GetInput().GetNFracDigits(), "input starts with dot - 1 - frac");
            cpu.Execute(Command.Digit1);
            ClassicAssert.AreEqual(1, cpu.GetInput().GetNIntDigits(), "input starts with dot - 2 - int");
            ClassicAssert.AreEqual(0, cpu.GetInput().GetIntDigit(0), "input starts with dot - 2 - int digit");
            ClassicAssert.AreEqual(1, cpu.GetInput().GetNFracDigits(), "input starts with dot - 2 - frac");
            ClassicAssert.AreEqual(1, cpu.GetInput().GetFracDigit(0), "input starts with dot - 2 - frac digit");
            // input starts with exp
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            cpu.Execute(Command.Exp);
            ClassicAssert.AreEqual(1, cpu.GetInput().GetNIntDigits(), "input starts with exp - 1");
            ClassicAssert.AreEqual(1, cpu.GetInput().GetIntDigit(0), "input starts with exp - 1 - int digit");
            ClassicAssert.AreEqual(0, cpu.GetInput().GetNFracDigits(), "input starts with exp - 1 - frac");
            ClassicAssert.AreEqual(ExpInputLength, cpu.GetInput().GetNExpDigits(), "input starts with exp - 1 - exp");
            cpu.Execute(Command.Digit1);
            ClassicAssert.AreEqual(1, cpu.GetInput().GetNIntDigits(), "input starts with exp - 2 - int");
            ClassicAssert.AreEqual(1, cpu.GetInput().GetIntDigit(0), "input starts with exp - 2 - int digit");
            ClassicAssert.AreEqual(0, cpu.GetInput().GetNFracDigits(), "input starts with exp - 2 - frac");
            ClassicAssert.AreEqual(ExpInputLength, cpu.GetInput().GetNExpDigits(), "input starts with exp - 2 - exp");
            ClassicAssert.AreEqual(1, cpu.GetInput().GetExpDigit(ExpInputLength - 1), "input starts with exp - 2 - exp digit");
            // input starts with sign
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);
            cpu.Execute(Command.Sign);
            ClassicAssert.AreEqual(0, cpu.GetInput().GetNIntDigits(), "input starts with sign - 1");
            ClassicAssert.AreEqual(-1, cpu.GetInput().GetSign(), "input starts with sign - 1 - sign");
            cpu.Execute(Command.Digit1);
            ClassicAssert.AreEqual(1, cpu.GetInput().GetNIntDigits(), "input starts with sign - 2");
            ClassicAssert.AreEqual(1, cpu.GetInput().GetIntDigit(0), "input starts with sign - 2");
            // the test below is non-trivial. Contrary to what one may expect, one cannot start number input with the sign.
            // This behavior is the same as in a regular calculator.
            ClassicAssert.AreEqual(1, cpu.GetInput().GetSign(), "input starts with sign - 2 - sign");
        }

        [Test]
        public void ExecuteDotTest()
        {
            ClassicAssert.AreEqual(8.6,
                RunCpu(new[] {"Digit8", "Dot", "Digit6", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Cpu.CpuPrecision*8.6, "8.6");
            // number entry starting with a dot
            ClassicAssert.AreEqual(0.6,
                RunCpu(new[] {"Dot", "Digit6", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                Cpu.CpuPrecision*0.6, "0.6");
        }

        [Test]
        public void ExecuteMantissaSignTest()
        {
            ClassicAssert.AreEqual(-86.0,
                RunCpu(new[] {"Digit8", "Digit6", "MantissaSign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "-86");

            ClassicAssert.AreEqual(86.0,
                RunCpu(new[] {"Digit8", "Digit6", "MantissaSign", "MantissaSign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "--86");

            ClassicAssert.AreEqual(860,
                RunCpu(new[] {"Digit8", "Digit6", "Exp", "Digit1", "MantissaSign", "MantissaSign", "Eq"},
                    Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "--86e1");

            ClassicAssert.AreEqual(-860,
                RunCpu(new[] {"Digit8", "Digit6", "Exp", "Digit1", "MantissaSign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "-86e1");
        }

        [Test]
        public void ExecuteCurrentSignTest()
        {
            ClassicAssert.AreEqual(-86.0,
                RunCpu(new[] {"Digit8", "Digit6", "Sign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "-86");

            ClassicAssert.AreEqual(86.0,
                RunCpu(new[] {"Digit8", "Digit6", "Sign", "Sign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "--86");

            ClassicAssert.AreEqual(-86.3,
                RunCpu(new[] {"Digit8", "Digit6", "Dot", "Digit3", "Sign", "Eq"},
                    Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "-86.3");

            ClassicAssert.AreEqual(86.3,
                RunCpu(new[] {"Digit8", "Digit6", "Dot", "Digit3", "Sign", "Sign", "Eq"},
                    Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "--86.3");

            ClassicAssert.AreEqual(86.3e-2,
                RunCpu(new[] {"Digit8", "Digit6", "Dot", "Digit3", "Exp", "Digit2", "Sign", "Eq"},
                    Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "86.3e-2");

            ClassicAssert.AreEqual(86.3e2,
                RunCpu(new[]
                {
                    "Digit8", "Digit6", "Dot", "Digit3", "Exp", "Digit2", "Sign",
                    "Sign", "Eq"
                }, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "86.3e2");

            ClassicAssert.AreEqual(-86.3e-2,
                RunCpu(new[]
                {
                    "Digit8", "Digit6", "Dot", "Sign", "Digit3", "Exp", "Digit2",
                    "Sign", "Eq"
                }, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "-86.3e-2");
        }

        [Test]
        public void ExecuteExpTest()
        {
            ClassicAssert.AreEqual(80.0,
                RunCpu(new[] {"Digit8", "Exp", "Digit1", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "8e1");
        }

        [Test]
        public void ExecuteExpSignTest()
        {
            ClassicAssert.AreEqual(0.8,
                RunCpu(new[] {"Digit8", "Exp", "Digit1", "ExpSign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "8e-1");

            ClassicAssert.AreEqual(80.0,
                RunCpu(new[] {"Digit8", "Exp", "Digit1", "ExpSign", "ExpSign", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "8e--1");
        }

        [Test]
        public void ExecuteClearXTest()
        {
            ClassicAssert.AreEqual(0.0, RunCpu(new[] {"Pi", "ClearX"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                1e-17, "pi");

            ClassicAssert.AreEqual(8.0,
                RunCpu(new[] {"Digit8", "Add", "Digit6", "ClearX", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "8+6<Cx>=");
            ClassicAssert.AreEqual(13.0,
                RunCpu(new[] {"Digit8", "Add", "Digit6", "ClearX", "Digit5", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "8+6<Cx>5=");
        }

        [Test]
        public void ExecuteClearAllTest()
        {
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit8", "Add", "Digit6", "ClearAll", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "8+6<CA>=");
            ClassicAssert.AreEqual(5.0,
                RunCpu(new[] {"Digit8", "Add", "Digit6", "ClearAll", "Digit5", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad),
                1e-17, "8+6<CA>5=");
        }

        [Test]
        public void ExecutePiTest()
        {
            ClassicAssert.AreEqual(Math.PI, RunCpu(new[] {"Pi"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                Cpu.CpuPrecision*1e-5, "pi");
        }

        [Test]
        public void ExecutePasteTest()
        {
            // RPN
            Cpu cpu = new Cpu(Settings.Mode.Rpn, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            cpu.ExecutePaste(2.12);
            ClassicAssert.AreEqual(2.12, cpu.X, Cpu.CpuPrecision, "RPN on blank");
            cpu.Execute(Command.Digit1);
            cpu.Execute(Command.Add);
            ClassicAssert.AreEqual(3.12, cpu.X, Cpu.CpuPrecision, "RPN number on paste");
            cpu.ExecutePaste(3.12);
            cpu.Execute(Command.Add);
            ClassicAssert.AreEqual(6.24, cpu.X, Cpu.CpuPrecision, "RPN on op");
            cpu.Execute(Command.Digit1);
            cpu.ExecutePaste(3.12);
            cpu.Execute(Command.Add);
            ClassicAssert.AreEqual(4.12, cpu.X, Cpu.CpuPrecision, "RPN on input in progress 1");
            cpu.Execute(Command.Add);
            ClassicAssert.AreEqual(10.36, cpu.X, Cpu.CpuPrecision, "RPN on input in progress 2");

            // ALG
            cpu = new Cpu(Settings.Mode.Alg, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                DefaultEnterMode, DefaultStackMode);

            cpu.ExecutePaste(2.12);
            ClassicAssert.AreEqual(2.12, cpu.X, Cpu.CpuPrecision, "ALG on blank");
            cpu.Execute(Command.Add);
            cpu.Execute(Command.Digit1);
            cpu.Execute(Command.Add);
            cpu.ExecutePaste(5.12);
            cpu.Execute(Command.Add);
            ClassicAssert.AreEqual(8.24, cpu.X, Cpu.CpuPrecision, "ALG on op");
            cpu.Execute(Command.Digit1);
            cpu.ExecutePaste(1.12);
            cpu.Execute(Command.Eq);
            ClassicAssert.AreEqual(9.36, cpu.X, Cpu.CpuPrecision, "ALG on input in progress");
        }

        [Test]
        public void ExecuteBinaryOpTest()
        {
            // ALG mode
            // simple operations
            ClassicAssert.AreEqual((2.0 + 3.0),
                RunCpu(new[] {"Digit2", "Add", "Digit3", "Eq"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "2+3");
            ClassicAssert.AreEqual((2.0 - 3.0),
                RunCpu(new[] {"Digit2", "Sub", "Digit3", "Eq"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "2-3");
            ClassicAssert.AreEqual((2.0*3.0),
                RunCpu(new[] {"Digit2", "Mul", "Digit3", "Eq"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "2*3");
            ClassicAssert.AreEqual((2.0/3.0),
                RunCpu(new[] {"Digit2", "Div", "Digit3", "Eq"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "2/3");
            ClassicAssert.AreEqual(Math.Pow(2.0, 3.0),
                RunCpu(new[] {"Digit2", "Pow", "Digit3", "Eq"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "2^3");
            // binop priority
            ClassicAssert.AreEqual((2.0 + 3.0),
                RunCpu(new[] {"Digit2", "Add", "Digit3", "Add"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "2+3+");
            ClassicAssert.AreEqual((3.0),
                RunCpu(new[] {"Digit2", "Add", "Digit3", "Mul"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "2+3*");
            ClassicAssert.AreEqual((2.0*3.0),
                RunCpu(new[] {"Digit2", "Mul", "Digit3", "Mul"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "2*3*");
            ClassicAssert.AreEqual((3.0),
                RunCpu(new[] {"Digit2", "Mul", "Digit3", "Pow"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "2*3^");
            ClassicAssert.AreEqual((163.0),
                RunCpu(new[] {"Digit1", "Add", "Digit2", "Mul", "Digit3", "Pow", "Digit4", "Eq"},
                    Settings.Mode.Alg, Settings.AngleUnits.Deg), 1e-7, "1+2*3^4");
            // RPN mode - traditional stack
            // simple operations
            ClassicAssert.AreEqual((2.0 + 3.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Add"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite), 1e-7, "2 3 +");
            ClassicAssert.AreEqual((2.0 - 3.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Sub"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite), 1e-7, "2 3 -");
            ClassicAssert.AreEqual((2.0*3.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Mul"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite), 1e-7, "2 3 *");
            ClassicAssert.AreEqual((2.0/3.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Div"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite), 1e-7, "2 3 /");
            ClassicAssert.AreEqual(Math.Pow(2.0, 3.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Pow"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite), 1e-7, "2 3 ^");
            // enter followed by op
            ClassicAssert.AreEqual((2.0 + 2.0),
                RunCpu(new[] {"Digit2", "Enter", "Add"}, Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite), 1e-7, "2 +");
            ClassicAssert.AreEqual((2.0*2.0),
                RunCpu(new[] {"Digit2", "Enter", "Mul"}, Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite), 1e-7, "2 *");
            // operation chains
            ClassicAssert.AreEqual(2.0*Math.Log(3.0 + 4.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "Add", "Ln", "Mul"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite), 1e-7, "2 3 4 + ln *");
            // RPN mode - HP28 stack
            // simple operations - same as traditional stack
            ClassicAssert.AreEqual((2.0 + 3.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Add"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite), 1e-7, "2 3 +");
            ClassicAssert.AreEqual((2.0 - 3.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Sub"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite), 1e-7, "2 3 -");
            ClassicAssert.AreEqual((2.0*3.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Mul"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite), 1e-7, "2 3 *");
            ClassicAssert.AreEqual((2.0/3.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Div"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite), 1e-7, "2 3 /");
            ClassicAssert.AreEqual(Math.Pow(2.0, 3.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Pow"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite), 1e-7, "2 3 ^");
            // enter followed by op - behavior different from traditional stack
            ClassicAssert.AreEqual((2.0),
                RunCpu(new[] {"Digit2", "Enter", "Add"}, Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite), 1e-7, "2 +");
            ClassicAssert.AreEqual((0.0),
                RunCpu(new[] {"Digit2", "Enter", "Mul"}, Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite), 1e-7, "2 *");
            // operation chains - same as traditional stack
            ClassicAssert.AreEqual(2.0*Math.Log(3.0 + 4.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "Add", "Ln", "Mul"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite), 1e-7, "2 3 4 + ln *");
            // non-trivial cases - testing only RPN since the behavior is determined
            // by the low-level routines which are shared between RPN and ALG modes
            // less simple cases
            ClassicAssert.AreEqual(1.0,
                RunCpu(new[] {"Digit2", "Dot", "Digit3", "Enter", "Digit0", "Pow"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite), Cpu.CpuPrecision, "2.3 ^ 0.0");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit0", "Enter", "Digit4", "Dot", "Digit6", "Pow"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite), Cpu.CpuPrecision*1e-15, "0.0 ^ 4.6");
            // precision smart sum
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[]
                {
                    "Digit1", "Digit0", "Digit0", "Dot", "Digit1", "Enter",
                    "Digit1", "Digit0", "Digit0", "Sub",
                    "Digit0", "Dot", "Digit1", "Sub"
                },
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite), Cpu.CpuPrecision*1e-15, "100.1 - 100.0 - 0.1");
            // overflows
            ClassicAssert.True(Double.IsNaN(
                RunCpu(new[] {"Digit2", "Dot", "Digit3", "Enter", "Digit0", "Div"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite)
                ), "2.3 / 0.0");
            ClassicAssert.True(Double.IsNaN(
                RunCpu(new[] {"Digit0", "Enter", "Digit0", "Pow"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite)
                ), "0.0 ^ 0.0");
            ClassicAssert.True(Double.IsNaN(
                RunCpu(new[] {"Digit0", "Enter", "Digit1", "Sign", "Pow"},
                    Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite)
                ), "0.0 ^ -1.0");
            ClassicAssert.True(
                Double.IsNaN(
                    RunCpu(new[]
                    {
                        "Digit2", "Dot", "Digit3", "Sign", "Enter", "Digit1", "Dot",
                        "Digit1", "Pow"
                    },
                        Settings.Mode.Rpn, Settings.AngleUnits.Deg,
                        Settings.EnterMode.Hp28,
                        Settings.StackMode.Infinite)
                    ), "-2.3 ^ 1.1");
        }

        [Test]
        public void ExecuteEqTest()
        {
            ClassicAssert.AreEqual((2.0 + 3.0),
                RunCpu(new[] {"Digit2", "Add", "Digit3", "Eq"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "2+3");


            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)),
                () => RunCpu(new[] {"Eq"}, Settings.Mode.Rpn, Settings.AngleUnits.Deg),
                "  throw at EQ in RPN mode");
        }

        [Test]
        public void ExecuteEnterTest()
        {
            ClassicAssert.AreEqual((2.0 + 3.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Add"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite),
                1e-7, "2+3");


            ClassicAssert.Throws(Is.InstanceOf(typeof (Exception)),
                () => RunCpu(new[] {"Enter"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                "  throw at Enter in Alg mode");
        }

        [Test]
        public void ExecuteExchXyTest()
        {
            // ALG mode
            ClassicAssert.AreEqual((3.0/2.0),
                RunCpu(new[] {"Digit2", "Div", "Digit3", "ExchXy", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                1e-7, "2 / 3 ExchXY =");
            // RPN mode
            ClassicAssert.AreEqual((3.0/2.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "ExchXy", "Div"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Traditional,
                    Settings.StackMode.Infinite), 1e-7, "2 3 ExchXY /");
            ClassicAssert.AreEqual((3.0/2.0),
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "ExchXy", "Div"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg,
                    Settings.EnterMode.Hp28,
                    Settings.StackMode.Infinite), 1e-7, "2 3 ExchXY /");
        }

        [Test]
        public void ExecuteStackUpTest()
        {
            // infinite stack
            // one number in the stack
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 up");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 up up");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "StackUp", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 up up up");
            // two numbers in the stack
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 up");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 up up");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp", "StackUp", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 up up up");
            // three numbers in the stack
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 4 up");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 4 up up");
            ClassicAssert.AreEqual(4.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp",
                    "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 4 up up up");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp",
                    "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 4 up up up up");
            // xyzt stack
            // one number in the stack
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit2", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 up");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit2", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 up up");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit2", "StackUp", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 up up up");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "StackUp", "StackUp", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 up up up up");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit2", "StackUp", "StackUp", "StackUp", "StackUp", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 up up up up up");
            // two numbers in the stack
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 up");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp", "StackUp"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 up up");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp", "StackUp", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 up up up");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackUp", "StackUp", "StackUp", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 up up up up");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "StackUp", "StackUp", "StackUp", "StackUp",
                    "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 up up up up up");
            // three numbers in the stack
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 up");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 up up");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp",
                    "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 up up up");
            ClassicAssert.AreEqual(4.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp",
                    "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 up up up up");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackUp", "StackUp",
                    "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 up up up up up");
            // four numbers in the stack
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackUp"
                },
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 up");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 up up");
            ClassicAssert.AreEqual(4.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 up up up");
            ClassicAssert.AreEqual(5.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackUp", "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 up up up up");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackUp", "StackUp", "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 up up up up up");
            // five numbers in the stack - the highest number should not matter as
            // only the lowest four are rotated
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 7 2 3 4 5 up");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 up up");
            ClassicAssert.AreEqual(4.0,
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 up up up");
            ClassicAssert.AreEqual(5.0,
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackUp", "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 up up up up");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackUp", "StackUp", "StackUp", "StackUp", "StackUp"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 up up up up up");
        }

        [Test]
        public void ExecuteStackDownTest()
        {
            // infinite stack
            // one number in the stack
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 down");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "StackDown", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 down down");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "StackDown", "StackDown", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 down down down");
            // two numbers in the stack
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 down");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackDown", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 down down");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackDown", "StackDown", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 down down down");
            // three numbers in the stack
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 4 down");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 4 down down");
            ClassicAssert.AreEqual(4.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown",
                    "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 4 down down down");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown",
                    "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Infinite), 1e-7,
                "infinite: 2 3 4 down down down down");
            // xyzt stack
            // one number in the stack
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit2", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 down");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit2", "StackDown", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 down down");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit2", "StackDown", "StackDown", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 down down down");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "StackDown", "StackDown", "StackDown", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 down down down down");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[]
                {
                    "Digit2", "StackDown", "StackDown", "StackDown", "StackDown",
                    "StackDown"
                },
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 down down down down down");
            // two numbers in the stack
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 down");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackDown", "StackDown"}, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 down down");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "StackDown", "StackDown", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 down down down");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "StackDown", "StackDown", "StackDown",
                    "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 down down down down");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "StackDown", "StackDown", "StackDown",
                    "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 down down down down down");
            // three numbers in the stack
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 down");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[] {"Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown"},
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 down down");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown",
                    "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 down down down");
            ClassicAssert.AreEqual(4.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown",
                    "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 down down down down");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "StackDown", "StackDown",
                    "StackDown", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 down down down down down");
            // four numbers in the stack
            ClassicAssert.AreEqual(4.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 down");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 down down");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackDown", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 down down down");
            ClassicAssert.AreEqual(5.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackDown", "StackDown", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 down down down down");
            ClassicAssert.AreEqual(4.0,
                RunCpu(new[]
                {
                    "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter", "Digit5",
                    "StackDown", "StackDown", "StackDown", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 down down down down down");
            // five numbers in the stack - the highest number should not matter as
            // only the lowest four are rotated
            ClassicAssert.AreEqual(4.0,
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 7 2 3 4 5 down");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 down down");
            ClassicAssert.AreEqual(2.0,
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackDown", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 down down down");
            ClassicAssert.AreEqual(5.0,
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackDown", "StackDown", "StackDown", "StackDown"
                }, Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 down down down down");
            ClassicAssert.AreEqual(4.0,
                RunCpu(new[]
                {
                    "Digit7", "Enter", "Digit2", "Enter", "Digit3", "Enter", "Digit4", "Enter",
                    "Digit5", "StackDown", "StackDown", "StackDown", "StackDown", "StackDown"
                },
                    Settings.Mode.Rpn,
                    Settings.AngleUnits.Deg, Settings.EnterMode.Traditional, Settings.StackMode.Xyzt), 1e-7,
                "xyzt: 2 3 4 5 down down down down down");
        }

        [Test]
        public void ExecuteLeftParenTest()
        {
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"LeftParen", "Digit1", "Add", "Digit2", "Eq"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg), 1e-7, "EQ resets lone opening paren");
            // more tests in executeRightParenTest because the require both parens
        }

        [Test]
        public void ExecuteRightParenTest()
        {
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit1", "Add", "Digit2", "RightParen"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                1e-7, "unmatched closing paren acts as EQ");
            ClassicAssert.AreEqual(2.0*(3.0 + 4.0),
                RunCpu(new[]
                {
                    "Digit2", "Mul", "LeftParen", "Digit3", "Add", "Digit4",
                    "RightParen", "Eq"
                },
                    Settings.Mode.Alg, Settings.AngleUnits.Deg), 1e-7, "2*(3+4)");
            ClassicAssert.AreEqual(Math.Pow(2.0, (3.0 + 4.0)*5.0),
                RunCpu(new[]
                {
                    "Digit2", "Pow", "LeftParen", "LeftParen", "Digit3", "Add", "Digit4",
                    "RightParen", "Mul", "Digit5", "RightParen", "Eq"
                }, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg), 1e-7, "2^((3+4)*5)");
        }

        [Test]
        public void ExecuteUnaryOpTest()
        {
            // non-trigonometric unary ops
            ClassicAssert.AreEqual(Math.Log10(12.0),
                RunCpu(new[] {"Digit1", "Digit2", "Log10"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "log10(12)");
            ClassicAssert.AreEqual(Math.Log(10.0),
                RunCpu(new[] {"Digit1", "Digit0", "Ln"}, Settings.Mode.Alg, Settings.AngleUnits.Deg), 1e-7,
                "ln(10)");
            ClassicAssert.AreEqual(Math.Pow(10.0, 1.3),
                RunCpu(new[] {"Digit1", "Dot", "Digit3", "TenToX"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg), 1e-7, "10^1.3");
            ClassicAssert.AreEqual(Math.Exp(1.3),
                RunCpu(new[] {"Digit1", "Dot", "Digit3", "EtoX"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "e^1.3");
            ClassicAssert.AreEqual(Math.Sqrt(70.0),
                RunCpu(new[] {"Digit7", "Digit0", "Sqrt"}, Settings.Mode.Alg, Settings.AngleUnits.Deg), 1e-7,
                "sqrt(70)");
            ClassicAssert.AreEqual((1.3*1.3),
                RunCpu(new[] {"Digit1", "Dot", "Digit3", "Sqr"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "1.3^2");
            ClassicAssert.AreEqual((1.0/1.3),
                RunCpu(new[] {"Digit1", "Dot", "Digit3", "InvX"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "1/1.3");
            ClassicAssert.AreEqual(MathUtil.Fact(5.6, InputLength),
                RunCpu(new[] {"Digit5", "Dot", "Digit6", "Fact"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "5.6!");

            // trigonometric - degrees
            const double dr = Math.PI/180.0;
            ClassicAssert.AreEqual(Math.Sin(10.0*dr),
                RunCpu(new[] {"Digit1", "Digit0", "Sin"}, Settings.Mode.Alg, Settings.AngleUnits.Deg), 1e-7,
                "sin(10 deg)");
            ClassicAssert.AreEqual(Math.Cos(10.0*dr),
                RunCpu(new[] {"Digit1", "Digit0", "Cos"}, Settings.Mode.Alg, Settings.AngleUnits.Deg), 1e-7,
                "cos(10 deg)");
            ClassicAssert.AreEqual(Math.Tan(10.0*dr),
                RunCpu(new[] {"Digit1", "Digit0", "Tan"}, Settings.Mode.Alg, Settings.AngleUnits.Deg), 1e-7,
                "tan(10 deg)");
            // special cases
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit1", "Digit8", "Digit0", "Sin"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Cpu.CpuPrecision*1e-5, "sin(180 deg)");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit9", "Digit0", "Cos"}, Settings.Mode.Alg, Settings.AngleUnits.Deg),
                Cpu.CpuPrecision*1e-5, "cos(90 deg)");
            ClassicAssert.True(
                Double.IsNaN(RunCpu(new[] {"Digit9", "Digit0", "Tan"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg)), "tan(90 deg)");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Digit1", "Digit8", "Digit0", "Tan"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                Cpu.CpuPrecision*1e-5, "tan(180 deg)");

            // trigonometric - radians
            ClassicAssert.AreEqual(Math.Sin(10.0),
                RunCpu(new[] {"Digit1", "Digit0", "Sin"}, Settings.Mode.Alg, Settings.AngleUnits.Rad), 1e-7,
                "sin(10 rad)");
            ClassicAssert.AreEqual(Math.Cos(10.0),
                RunCpu(new[] {"Digit1", "Digit0", "Cos"}, Settings.Mode.Alg, Settings.AngleUnits.Rad), 1e-7,
                "cos(10 rad)");
            ClassicAssert.AreEqual(Math.Tan(10.0),
                RunCpu(new[] {"Digit1", "Digit0", "Tan"}, Settings.Mode.Alg, Settings.AngleUnits.Rad), 1e-7,
                "tan(10 rad)");
            // special cases
            ClassicAssert.AreEqual(0.0, RunCpu(new[] {"Pi", "Sin"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                Cpu.CpuPrecision*1e-5, "sin(pi)");

            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Pi", "Div", "Digit2", "Eq", "Cos"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                Cpu.CpuPrecision*1e-5, "cos(pi/2)");
            ClassicAssert.AreEqual(true,
                Double.IsNaN(RunCpu(new[] {"Pi", "Div", "Digit2", "Eq", "Tan"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad)), "tan(pi/2)");
            ClassicAssert.AreEqual(0.0,
                RunCpu(new[] {"Pi", "Tan"}, Settings.Mode.Alg, Settings.AngleUnits.Rad),
                Cpu.CpuPrecision*1e-5, "tan(180 deg)");
        }

        [Test]
        public void ExecuteMemoryOpTest()
        {
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit3", "XToMem", "ClearX", "MemToX"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                1e-7, "XToMem, CLEAR_X, MemToX");

            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit3", "XToMem", "Sqrt", "MemToX"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                1e-7, "XToMem, SQRT, MemToX");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit3", "XToMem", "Sqrt", "ExchXMem"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                1e-7, "XToMem, SQRT, EXCH_X_MEM");
            ClassicAssert.AreEqual(Math.Sqrt(3.0),
                RunCpu(new[] {"Digit3", "XToMem", "Sqrt", "ExchXMem", "ClearX", "ExchXMem"},
                    Settings.Mode.Alg, Settings.AngleUnits.Deg),
                1e-7, "XToMem, SQRT, EXCH_X_MEM, CLEAR_X, EXCH_X_MEM");
            ClassicAssert.AreEqual(3.0 + Math.Sqrt(3.0),
                RunCpu(new[] {"Digit3", "XToMem", "Sqrt", "MemPlus", "MemToX"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                1e-7, "XToMem, SQRT, MEM_PLUS, MemToX");
        }

        [Test]
        public void ExecuteSwitchToMemTest()
        {
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit3", "XToMem", "ClearX", "MemToX"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                1e-7, "XToMem, CLEAR_X, MemToX");
            ClassicAssert.AreEqual(3.0,
                RunCpu(new[] {"Digit3", "XToMem", "Sqrt", "Mem1", "XToMem", "Mem0", "MemToX"},
                    Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                1e-7, "XToMem, SQRT, Mem1, XToMem, Mem0, MemToX");
            ClassicAssert.AreEqual(Math.Sqrt(3.0),
                RunCpu(new[]
                {
                    "Digit3", "XToMem", "Sqrt", "Mem1", "XToMem", "Mem0", "MemToX", "Mem1",
                    "MemToX"
                }, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg),
                1e-7, "XToMem, SQRT, Mem1, XToMem, Mem0, MemToX, Mem1, MemToX");
        }

        [Test]
        public void ExecuteDegRadTest()
        {
            const double dr = Math.PI/180.0;
            ClassicAssert.AreEqual(Math.Sin(10.0),
                RunCpu(new[] {"Digit1", "Digit0", "DegRad", "Sin"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg), 1e-7, "sin(10 rad) - one deg/rad switch");
            ClassicAssert.AreEqual(Math.Sin(10.0*dr),
                RunCpu(new[] {"Digit1", "Digit0", "DegRad", "Sin"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad), 1e-7, "sin(10 deg) - one deg/rad switch");
            ClassicAssert.AreEqual(Math.Sin(10.0*dr),
                RunCpu(new[] {"Digit1", "Digit0", "DegRad", "DegRad", "Sin"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Deg), 1e-7, "sin(10 deg) - two deg/rad switches");
            ClassicAssert.AreEqual(Math.Sin(10.0),
                RunCpu(new[] {"Digit1", "Digit0", "DegRad", "DegRad", "Sin"}, Settings.Mode.Alg,
                    Settings.AngleUnits.Rad), 1e-7, "sin(10 rad) - two deg/rad switches");
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
    }
}
