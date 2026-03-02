using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace Calcoo
{
    internal class Body
    {
        private readonly Dictionary<Command, Button> _buttons;
        private readonly Dictionary<Command, ToggleButton> _toggleButtons;
        private readonly Dictionary<Key, Command>[] _shortcuts;
        private readonly Dictionary<Command, DispatcherTimer> _highlightTimers = new();

        private readonly NumberDisplay _mainDisplay;
        private IDoubleByDigitGetters _mainDisplayContent;
        private readonly NumberDisplay[] _regNumDisplays;
        private readonly OperationDisplay[] _operationDisplays;
        private readonly LabelDisplay[] _regLabelDisplays;
        private readonly NumberDisplay[] _memDisplays;
        private readonly IndicatorDisplay<Settings.DisplayFormat> _displayFormatDisplay;
        private readonly IndicatorDisplay<Settings.AngleUnits> _angleUnitsDisplay;
        private readonly Frame _activeMemIcon;

        private readonly int _inputLength, _expInputLength, _numBase;
        public Settings.DisplayFormat displayFormat;
        public bool round;
        public int roundLength;
        public bool truncateZeros;

        public class DisplayCanvas
        {
            public Canvas MainDisplay, DegRadDisplay, FormatDisplay;
            public Canvas[] MemDisplays, RegisterDisplays;

            public DisplayCanvas(int nMemDisplays, int nRegisterDisplays)
            {
                MemDisplays = new Canvas[nMemDisplays];
                RegisterDisplays = new Canvas[nRegisterDisplays];
            }
        }

        public bool arcAutorelease;
        public bool hypAutorelease;
        public Settings.PasteParsingAlgorithm pasteParsingAlgorithm;

        public bool Arc
        {
            get { return _toggleButtons[Command.Arc].IsChecked.Equals(true); }
            set { _toggleButtons[Command.Arc].IsChecked = value; }
        }

        public bool Hyp
        {
            get { return _toggleButtons[Command.Hyp].IsChecked.Equals(true); }
            set { _toggleButtons[Command.Hyp].IsChecked = value; }
        }

        public bool UndoEnabled
        {
            get { return _buttons[Command.Undo].IsEnabled; }
            set { _buttons[Command.Undo].IsEnabled = value; }
        }

        public bool RedoEnabled
        {
            get { return _buttons[Command.Redo].IsEnabled; }
            set { _buttons[Command.Redo].IsEnabled = value; }
        }

        public Settings.DisplayFormat GetDisplayFormat()
        {
            return displayFormat;
        }

        public string GetMainDisplayString()
        {
            return _mainDisplayContent.ToString();
        }

        public void SwitchDisplayFormat()
        {
            switch (displayFormat)
            {
                case Settings.DisplayFormat.Fix:
                    displayFormat = Settings.DisplayFormat.Sci;
                    break;
                case Settings.DisplayFormat.Sci:
                    displayFormat = Settings.DisplayFormat.Eng;
                    break;
                case Settings.DisplayFormat.Eng:
                    displayFormat = Settings.DisplayFormat.Fix;
                    break;
            }
        }

        private void CreateButton(Command function, int xPos, int yPos, int xSize, int ySize, Key[][] shortcuts,
            string tooltip, string iconSet, Grid mainGrid, bool isToggle, bool hasIcon)
        {
            ButtonBase newButton;
            if (isToggle)
            {
                var tb = new ToggleButton();
                _toggleButtons.Add(function, tb);
                newButton = tb;
            }
            else
            {
                var b = new Button();
                _buttons.Add(function, b);
                newButton = b;
            }

            if (tooltip.Length != 0)
            {
                var tt = new ToolTip();
                tt.Content = tooltip;
                newButton.ToolTip = tt;
            }

            var frm = new Frame();
            newButton.Name = function.ToString();
            if (hasIcon)
            {
                frm.Source = new Uri("pack://application:,,,/Resources" + iconSet + newButton.Name + ".xaml",
                    UriKind.Absolute);
                newButton.Content = frm;
            }

            Grid.SetColumn(newButton, xPos);
            Grid.SetRow(newButton, yPos);
            Grid.SetColumnSpan(newButton, xSize);
            Grid.SetRowSpan(newButton, ySize);
            newButton.Focusable = false;

            mainGrid.Children.Add(newButton);
            AddCommandShortcuts(function, shortcuts);
        }

        private void AddCommandShortcuts(Command command, Key[][] shortcuts)
        {
            if (!shortcuts.Any()) return;
            for (int i = 0; i < Math.Min(4, shortcuts.Length); ++i)
                foreach (Key k in shortcuts[i])
                    _shortcuts[i].Add(k, command);
        }

        private int ShrotcutFlagsToIndex(bool ctrl, bool shift)
        {
            return (ctrl ? 2 : 0) + (shift ? 1 : 0);
        }

        public Command? TranslateShortcut(Key key, bool ctrl, bool shift)
        {
            Command command;
            if (_shortcuts[ShrotcutFlagsToIndex(ctrl, shift)].TryGetValue(key, out command))
                return command;
            return null;
        }

        // Buttons that share a grid position and swap visibility by mode
        private static readonly Dictionary<Command, Command> _modeAlternates = new()
        {
            { Command.Enter, Command.Eq },
            { Command.Eq, Command.Enter },
            { Command.StackDown, Command.LeftParen },
            { Command.LeftParen, Command.StackDown },
            { Command.StackUp, Command.RightParen },
            { Command.RightParen, Command.StackUp },
        };

        private ButtonBase FindButton(Command command)
        {
            if (_buttons.TryGetValue(command, out var btn))
                return btn;
            if (_toggleButtons.TryGetValue(command, out var tbtn))
                return tbtn;
            return null;
        }

        public void HighlightButton(Command command)
        {
            ButtonBase button = FindButton(command);

            // If button is hidden, try the mode alternate (e.g. Enter↔Eq)
            if ((button == null || button.Visibility != Visibility.Visible)
                && _modeAlternates.TryGetValue(command, out var alt))
                button = FindButton(alt);

            if (button == null || button.Visibility != Visibility.Visible) return;

            if (_highlightTimers.TryGetValue(command, out var existing))
                existing.Stop();

            button.Tag = "Highlighted";

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                button.Tag = null;
                _highlightTimers.Remove(command);
            };
            _highlightTimers[command] = timer;
            timer.Start();
        }

        public Body(Grid mainGrid, DisplayCanvas displayCanvas, int numBase, int inputLength, int expInputLength)
        {
            _buttons = new Dictionary<Command, Button>();
            _toggleButtons = new Dictionary<Command, ToggleButton>();
            _shortcuts = new Dictionary<Key, Command>[4];
            for(int i = 0; i < _shortcuts.Length; ++i)
                _shortcuts[i] = new Dictionary<Key, Command>();

            CreateButtons(mainGrid);

            _mainDisplay = new NumberDisplay(15, //int cellWidth
            0, //int dotOffsetX,
            -6, //int dotOffsetY,
            5, //int dotWidth,
            5, //int xMargin,
            5, //int yMargin,
            76, //int errorOffsetX,
            13, //int tickOffsetX,
            -3,  //int tickOffsetY,
            5, // int tickWidth
            inputLength,
            expInputLength,
            true,
            true,
            "/Icons/Displays/Main/",
            numBase,
            displayCanvas.MainDisplay)
            ;

            _memDisplays = new NumberDisplay[displayCanvas.MemDisplays.Length];

            for (int i = 0; i < _memDisplays.Length; ++i)
            {
                _memDisplays[i] = new NumberDisplay(
                        7, //int cellWidth,
                        0, //int dotOffsetX,
                        -6, //int dotOffsetY,
                        3, //int dotWidth,
                        1, //int xMargin,
                        2, //int yMargin,
                        0,//int errorOffsetX,
                        0,//int tickOffsetX,
                        0, //int tickOffsetY,
                        0, // int tickWidth
                        inputLength,
                        expInputLength,
                        false,
                        false,
                        "/Icons/Displays/Main/",
                        numBase,
                        displayCanvas.MemDisplays[i]
                        );
            }

            _regNumDisplays = new NumberDisplay[displayCanvas.RegisterDisplays.Length];
            _operationDisplays = new OperationDisplay[displayCanvas.RegisterDisplays.Length];
            _regLabelDisplays = new LabelDisplay[displayCanvas.RegisterDisplays.Length];

            for (int i = 0; i < _regNumDisplays.Length; ++i)
            {
                _regNumDisplays[i] = new NumberDisplay(
                    7, //int cellWidth,
                    0, //int dotOffsetX,
                    -6, //int dotOffsetY,
                    3, //int dotWidth,
                    1 + 10, //int xMargin,
                    2, //int yMargin,
                    0, //int errorOffsetX,
                    0, //int tickOffsetX,
                    0, //int tickOffsetY,
                    0, // int tickWidth
                    inputLength,
                    expInputLength,
                    false,
                    false,
                    "/Icons/Displays/Main/",
                    numBase,
                    displayCanvas.RegisterDisplays[i]
                    );

                _operationDisplays[i] = new OperationDisplay(120,
                    0,
                    20,
                    16,
                    "/Icons/Displays/Operation/",
                    displayCanvas.RegisterDisplays[i]);

                _regLabelDisplays[i] = new LabelDisplay(0,
                    0,
                    12,
                    16,
                    "/Icons/Displays/Label/Register" + i,
                    displayCanvas.RegisterDisplays[i]);
            }

            _displayFormatDisplay = new IndicatorDisplay<Settings.DisplayFormat>(0, 0, 32, 16,
                new[] {Settings.DisplayFormat.Eng, Settings.DisplayFormat.Sci, Settings.DisplayFormat.Fix},
                "/Icons/Displays/Indicator/", displayCanvas.FormatDisplay);
            _angleUnitsDisplay = new IndicatorDisplay<Settings.AngleUnits>(0, 0, 32, 16,
                new[] {Settings.AngleUnits.Deg, Settings.AngleUnits.Rad}, "/Icons/Displays/Indicator/",
                displayCanvas.DegRadDisplay);

            displayFormat = Settings.DisplayFormat.Fix;
            round = false;
            roundLength = inputLength;
            truncateZeros = false;

            _numBase = numBase;
            _expInputLength = expInputLength;
            _inputLength = inputLength;

            _activeMemIcon = new Frame();
            _activeMemIcon.Source = new Uri("pack://application:,,,/Resources/Icons/Buttons/ActiveMem.xaml",
                UriKind.Absolute);
        }

        private void CreateButtons(Grid mainGrid)
        {
            const String iconPath = "/Icons/Buttons/";

            CreateButton(Command.DegRad, 0, 4, 2, 1, new Key[][] {}, "Change angle units", iconPath, mainGrid, false, false);
            CreateButton(Command.Info, 0, 6, 2, 2, new[] { new Key[] {}, new[] { Key.Oem2 } }, "About/Help", iconPath, mainGrid, false, true);
            CreateButton(Command.Settings, 0, 8, 2, 2, new[] { new Key[] { }, new[] { Key.D1 } }, "Settings", iconPath, mainGrid, false, true);
            CreateButton(Command.Copy, 0, 10, 2, 2, new[] { new Key[] {}, new Key[] {}, new[] { Key.C } }, "Copy", iconPath, mainGrid, false, true);
            CreateButton(Command.Paste, 0, 12, 2, 2, new[] { new Key[] { }, new Key[] { }, new[] { Key.V } }, "Paste", iconPath, mainGrid, false, true);

            CreateButton(Command.Sin, 3, 4, 2, 2, new[] { new[] { Key.S } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Cos, 3, 6, 2, 2, new[] { new[] { Key.C } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Tan, 3, 8, 2, 2, new[] { new[] { Key.T } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Arc, 3, 10, 2, 2, new[] { new[] { Key.A } }, "", iconPath, mainGrid, true, true);
            CreateButton(Command.Hyp, 3, 12, 2, 2, new[] { new[] { Key.H } }, "", iconPath, mainGrid, true, true);
            CreateButton(Command.Sqr, 5, 4, 2, 2, new[] { new[] { Key.Q } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.EtoX, 5, 6, 2, 2, new[] { new[] { Key.X } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.TenToX, 5, 8, 2, 2, new[] { new[] { Key.D } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Pow, 5, 10, 2, 2, new[] { new[] { Key.R } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Pi, 5, 12, 2, 2, new[] { new[] { Key.P } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Sqrt, 7, 4, 2, 2, new[] { new[] { Key.W } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Ln, 7, 6, 2, 2, new[] { new[] { Key.N } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Log10, 7, 8, 2, 2, new[] { new[] { Key.G } }, "Base-10 log", iconPath, mainGrid, false, true);
            CreateButton(Command.InvX, 7, 10, 2, 2, new[] { new[] { Key.I } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Fact, 7, 12, 2, 2, new[] { new[] { Key.F } }, "", iconPath, mainGrid, false, true);

            CreateButton(Command.Digit7, 10, 3, 2, 2, new[] { new[] { Key.D7, Key.NumPad7 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Digit4, 10, 5, 2, 2, new[] { new[] { Key.D4, Key.NumPad4 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Digit1, 10, 7, 2, 2, new[] { new[] { Key.D1, Key.NumPad1 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Digit0, 10, 9, 2, 2, new[] { new[] { Key.D0, Key.NumPad0 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Exp, 10, 12, 2, 2, new[] { new[] { Key.E } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Digit8, 12, 3, 2, 2, new[] { new[] { Key.D8, Key.NumPad8 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Digit5, 12, 5, 2, 2, new[] { new[] { Key.D5, Key.NumPad5 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Digit2, 12, 7, 2, 2, new[] { new[] { Key.D2, Key.NumPad2 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Sign, 12, 9, 2, 2, new[] { new[] { Key.M } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Custom, 12, 12, 2, 2, new Key[][] {}, "Custom command", iconPath, mainGrid, false, true);
            CreateButton(Command.Digit9, 14, 3, 2, 2, new[] { new[] { Key.D9, Key.NumPad9 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Digit6, 14, 5, 2, 2, new[] { new[] { Key.D6, Key.NumPad6 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Digit3, 14, 7, 2, 2, new[] { new[] { Key.D3, Key.NumPad3 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Dot, 14, 9, 2, 2, new[] { new[] { Key.OemComma, Key.OemPeriod, Key.Decimal } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Format, 14, 12, 2, 1, new Key[][] {}, "Change display format", iconPath, mainGrid, false, false);

            CreateButton(Command.Add, 17, 3, 2, 2, new[] { new[] { Key.Add }, new[] { Key.OemPlus } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Mul, 17, 5, 2, 2, new[] { new[] { Key.Multiply }, new[] { Key.D8 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.ExchXy, 17, 7, 2, 2, new[] { new[] { Key.Y } }, "Swap X and Y", iconPath, mainGrid, false, true);
            CreateButton(Command.XToMem, 17, 10, 2, 2, new[] { new Key[] {}, new Key[] {}, new[] { Key.M } }, "STO", iconPath, mainGrid, false, true);
            CreateButton(Command.Mem0, 17, 12, 1, 1, new[] { new Key[] {}, new Key[] {}, new[] { Key.D1 } }, "", iconPath, mainGrid, false, false);
            CreateButton(Command.Mem1, 17, 13, 1, 1, new[] { new Key[] {}, new Key[] {}, new[] { Key.D2 } }, "", iconPath, mainGrid, false, false);
            CreateButton(Command.Sub, 19, 3, 2, 2, new[] { new[] { Key.Subtract, Key.OemMinus } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Div, 19, 5, 2, 2, new[] { new[] { Key.Divide, Key.Oem2 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.LeftParen, 19, 7, 2, 2, new[] { new[] { Key.Oem4 }, new[] { Key.D9 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.StackDown, 19, 7, 2, 2, new[] { new[] { Key.Down } }, "Scroll stack down", iconPath, mainGrid, false, true);
            CreateButton(Command.MemToX, 19, 10, 2, 2, new[] { new Key[] {}, new Key[] {}, new[] { Key.R } }, "RCL", iconPath, mainGrid, false, true);
            CreateButton(Command.ClearAll, 21, 3, 2, 2, new[] { new[] { Key.Escape } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Eq, 21, 5, 2, 2, new[] { new[] { Key.OemPlus } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.Enter, 21, 5, 2, 2, new[] { new[] { Key.Return } }, "Enter", iconPath, mainGrid, false, true);
            CreateButton(Command.RightParen, 21, 7, 2, 2, new[] { new[] { Key.Oem6 }, new[] { Key.D0 } }, "", iconPath, mainGrid, false, true);
            CreateButton(Command.StackUp, 21, 7, 2, 2, new[] { new[] { Key.Up } }, "Scroll stack up", iconPath, mainGrid, false, true);
            CreateButton(Command.MemPlus, 21, 10, 2, 2, new Key[][] {}, "Add X to memory", iconPath, mainGrid, false, true);
            CreateButton(Command.Undo, 23, 3, 2, 2, new[] { new[] { Key.Back, Key.Left }, new Key[] {}, new[] { Key.Z } }, "Undo", iconPath, mainGrid, false, true);
            CreateButton(Command.Redo, 23, 5, 2, 2, new[] { new[] { Key.Right }, new Key[] {}, new[] { Key.Y } }, "Redo", iconPath, mainGrid, false, true);
            CreateButton(Command.ClearX, 23, 7, 2, 2, new[] { new[] { Key.Delete } }, "Clear X", iconPath, mainGrid, false, true);
            CreateButton(Command.ExchXMem, 23, 10, 2, 2, new Key[][] {}, "Swap X and memory", iconPath, mainGrid, false, true);

            AddCommandShortcuts(Command.MantissaSign, new[] { new Key[] { }, new Key[] {}, new[] { Key.OemMinus } });
            AddCommandShortcuts(Command.ExpSign, new[] { new Key[] {}, new Key[] {}, new[] { Key.E } });
            AddCommandShortcuts(Command.Exit, new[] { new Key[] {}, new Key[] {}, new[] { Key.Q } });
        }

        public void DisplayOnlyActiveButtonsForMode(Settings.Mode mode)
        {
            switch (mode)
            {
                case Settings.Mode.Rpn:
                    foreach (var rpnFunc in CommandExtensions.RpnOnly)
                        _buttons[rpnFunc].Visibility = Visibility.Visible;
                    foreach (var algFunc in CommandExtensions.AlgOnly)
                        _buttons[algFunc].Visibility = Visibility.Hidden;
                    break;
                case Settings.Mode.Alg:
                    foreach (var rpnFunc in CommandExtensions.RpnOnly)
                        _buttons[rpnFunc].Visibility = Visibility.Hidden;
                    foreach (var algFunc in CommandExtensions.AlgOnly)
                        _buttons[algFunc].Visibility = Visibility.Visible;
                    break;
            }
        }

        public void Refresh(ICpuOutput cpuOutput)
        {
            // sanity checks
            if (_inputLength < cpuOutput.GetInput().GetNIntDigits() + cpuOutput.GetInput().GetNFracDigits())
                throw new Exception("mantissa input lengths don't match on the body (" + _inputLength + ") and cpu ("
                                    + (cpuOutput.GetInput().GetNIntDigits() + cpuOutput.GetInput().GetNFracDigits())
                                    + ")");
            if ((_expInputLength != cpuOutput.GetInput().GetNExpDigits() && cpuOutput.GetInput().GetNExpDigits() != 0))
                throw new Exception("exp input lengths don't match on the body (" + _expInputLength + ") and cpu("
                                    + cpuOutput.GetInput().GetNExpDigits() + ")");

            if (cpuOutput.IsInputInProgress())
            {
                _mainDisplayContent = cpuOutput.GetInput();
                if (_mainDisplayContent.GetNIntDigits() == 0)
                {
                    var tmp = new DoubleByDigit();
                    tmp.AddIntDigit(0);
                    tmp.SetSign(_mainDisplayContent.GetSign());
                    _mainDisplayContent = tmp;
                }
            }
            else
                _mainDisplayContent = DoubleByDigit.FromDouble(cpuOutput.X, _inputLength, _expInputLength,
                    displayFormat != Settings.DisplayFormat.Fix,
                    Settings.ExpDivisor(displayFormat), (round ? roundLength : _inputLength), round && !truncateZeros,
                    _numBase);

            _mainDisplay.Show(_mainDisplayContent);
            
            for (int i = 0; i < _regNumDisplays.Length; ++i)
            {
                _regNumDisplays[i].Show(DoubleByDigit.FromDouble(cpuOutput.GetStack().PeekValue(i), _inputLength,
                    _expInputLength,
                    displayFormat != Settings.DisplayFormat.Fix, Settings.ExpDivisor(displayFormat),
                    (round
                        ? roundLength
                        : _inputLength),
                    round && !truncateZeros, _numBase));
                _operationDisplays[i].Show(cpuOutput.GetStack().PeekOp(i), cpuOutput.GetStack().PeekParenExists(i));
            }

            for (int i = 0; i < _memDisplays.Length; ++i)
            {
                DoubleByDigit dbd = DoubleByDigit.FromDouble(cpuOutput.GetMem(i), _inputLength, _expInputLength,
                    displayFormat != Settings.DisplayFormat.Fix,
                    Settings.ExpDivisor(displayFormat), (round ? roundLength : _inputLength), round && !truncateZeros,
                    _numBase);
                _memDisplays[i].Show(dbd);
                if (i == cpuOutput.ActiveMemNum)
                    _buttons[CommandExtensions.Mem[i]].Content = _activeMemIcon;
                else
                    _buttons[CommandExtensions.Mem[i]].Content = null;
            }

            _angleUnitsDisplay.Show(cpuOutput.AngleUnits);
            _displayFormatDisplay.Show(displayFormat);

            if (_mainDisplayContent.IsOverflow())
            {
                foreach (var c in _buttons.Keys)
                    if (c != Command.Undo && c != Command.Redo && c != Command.ClearAll)
                        _buttons[c].IsEnabled = false;
                foreach (var c in _toggleButtons.Keys)
                    _toggleButtons[c].IsEnabled = false;
            }
            else
            {
                foreach (var c in _buttons.Keys)
                    if (c != Command.Undo && c != Command.Redo)
                        _buttons[c].IsEnabled = true;
                foreach (var c in _toggleButtons.Keys)
                    _toggleButtons[c].IsEnabled = true;
            }
        }

    }
}
