using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Win32;

namespace Calcoo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Cpu _cpu;
        private LinkedList<Cpu> _undoStack, _redoStack;
        private const int ExpInputLength = 2;
        private const int NumBase = 10;
        private const int NMem = 2;
        private const int NRegister = 3;
        private const int InputLength = 10;
        private const int UndoStackSize = 200;

        private readonly Body body;
        private double _aspectRatio;
        private int _chromeWidth, _chromeHeight;
        private string _customButtonCommand;
        private string _customButtonTooltip;

        public MainWindow()
        {
            var settings = Settings.Load(InputLength);
            _cpu = new Cpu(settings.CurrentMode, settings.CurrentAngleUnits, InputLength, ExpInputLength, NumBase, NMem,
                settings.CurrentEnterMode, settings.CurrentStackMode);
            _undoStack = new LinkedList<Cpu>();
            _redoStack = new LinkedList<Cpu>();

            InitializeComponent();

            using (var rk = Registry.CurrentUser.OpenSubKey("Software\\Calcoo\\", writable: true))
            {
                if (rk != null)
                {
                    if (rk.GetValue("WindowHeight") is int h && h > 0)
                        Height = h;
                    rk.DeleteValue("WindowWidth", throwOnMissingValue: false);
                }
            }

            var displayCanvas = new Body.DisplayCanvas(NMem, NRegister);
            displayCanvas.MainDisplay = MainDisplayCanvas;
            displayCanvas.DegRadDisplay = DegRadDisplayCanvas;
            displayCanvas.FormatDisplay = FormatDisplayCanvas;
            displayCanvas.MemDisplays[0] = Mem0DisplayCanvas;
            displayCanvas.MemDisplays[1] = Mem1DisplayCanvas;
            displayCanvas.RegisterLabelDisplays[0] = Register0LabelCanvas;
            displayCanvas.RegisterLabelDisplays[1] = Register1LabelCanvas;
            displayCanvas.RegisterLabelDisplays[2] = Register2LabelCanvas;
            displayCanvas.RegisterNumberDisplays[0] = Register0NumberCanvas;
            displayCanvas.RegisterNumberDisplays[1] = Register1NumberCanvas;
            displayCanvas.RegisterNumberDisplays[2] = Register2NumberCanvas;
            displayCanvas.RegisterOperationDisplays[0] = Register0OperationCanvas;
            displayCanvas.RegisterOperationDisplays[1] = Register1OperationCanvas;
            displayCanvas.RegisterOperationDisplays[2] = Register2OperationCanvas;

            body = new Body(MainGrid, displayCanvas, NumBase, InputLength, ExpInputLength, ProcessCommand);

            body.DisplayOnlyActiveButtonsForMode(settings.CurrentMode);
            body.ArcAutorelease = settings.ArcAutorelease;
            body.HypAutorelease = settings.HypAutorelease;
            body.CurrentPasteParsingAlgorithm = settings.CurrentPasteParsingAlgorithm;
            body.Round = settings.Round;
            body.RoundLength = settings.RoundLength;
            body.TruncateZeros = settings.TruncateZeros;
            body.CurrentDisplayFormat = settings.CurrentDisplayFormat;
            _customButtonCommand = settings.CustomButtonCommand;
            _customButtonTooltip = settings.CustomButtonTooltip;
            Topmost = settings.StayOnTop;
            body.UndoEnabled = false;
            body.RedoEnabled = false;

            body.Refresh(_cpu);
            body.SetCustomButtonTooltip(_customButtonTooltip);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            using (var rk = Registry.CurrentUser.CreateSubKey("Software\\Calcoo\\"))
            {
                if (rk != null)
                {
                    rk.SetValue("WindowHeight", (int)Math.Round(ActualHeight), RegistryValueKind.DWord);
                }
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;

            GetWindowRect(hwnd, out RECT windowRect);
            GetClientRect(hwnd, out RECT clientRect);

            var dpiScale = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            _chromeWidth = (windowRect.Right - windowRect.Left) - (clientRect.Right - clientRect.Left);
            _chromeHeight = (windowRect.Bottom - windowRect.Top) - (clientRect.Bottom - clientRect.Top);
            // Content aspect ratio: MainGrid is 25×16 + margins 8+8 = 416 wide, 14×16 + margins 8+8 = 240 tall
            _aspectRatio = 416.0 / 240.0;

            // Adjust window width to match content aspect ratio
            // GetClientRect returns physical pixels; WPF Width is in DIPs
            double clientHeight = (clientRect.Bottom - clientRect.Top) / dpiScale;
            double chromeWidth = _chromeWidth / dpiScale;
            Width = clientHeight * _aspectRatio + chromeWidth;

            App.ApplyDarkTitleBar(this);

            var source = HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);

            var sysMenu = GetSystemMenu(hwnd, false);
            InsertMenu(sysMenu, 0xF060, MF_BYCOMMAND | MF_STRING, IDM_STAY_ON_TOP, "Stay on top");
            InsertMenu(sysMenu, 0xF060, MF_BYCOMMAND | MF_SEPARATOR, 0, null);
            if (Topmost)
                CheckMenuItem(sysMenu, IDM_STAY_ON_TOP, MF_BYCOMMAND | MF_CHECKED);
        }


        private const int WM_SYSCOMMAND = 0x0112;
        private const int WM_SIZING = 0x0214;
        private const int WM_SETTINGCHANGE = 0x001A;
        private const int WMSZ_TOP = 3;
        private const int WMSZ_TOPLEFT = 4;
        private const int WMSZ_TOPRIGHT = 5;
        private const int WMSZ_BOTTOM = 6;
        private const uint IDM_STAY_ON_TOP = 0xA000;
        private const uint MF_SEPARATOR = 0x0800;
        private const uint MF_STRING = 0x0000;
        private const uint MF_BYCOMMAND = 0x0000;
        private const uint MF_CHECKED = 0x0008;
        private const uint MF_UNCHECKED = 0x0000;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, string? lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool InsertMenu(IntPtr hMenu, uint uPosition, uint uFlags, uint uIDNewItem, string? lpNewItem);

        [DllImport("user32.dll")]
        private static extern bool CheckMenuItem(IntPtr hMenu, uint uIDCheckItem, uint uCheck);


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SYSCOMMAND && (wParam.ToInt64() & 0xFFF0) == IDM_STAY_ON_TOP)
            {
                SetStayOnTop(!Topmost);
                handled = true;
            }
            else if (msg == WM_SETTINGCHANGE)
            {
                bool isDark = App.DetectDarkMode();
                if (isDark != App.IsDarkMode)
                {
                    App.ApplyTheme(isDark);
                    App.ApplyDarkTitleBar(this);
                }
            }
            else if (msg == WM_SIZING)
            {
                var rect = Marshal.PtrToStructure<RECT>(lParam);
                int edge = (int)wParam.ToInt64();
                int clientWidth = rect.Right - rect.Left - _chromeWidth;
                int clientHeight = rect.Bottom - rect.Top - _chromeHeight;

                if (edge == WMSZ_TOP || edge == WMSZ_BOTTOM)
                {
                    rect.Right = rect.Left + (int)(clientHeight * _aspectRatio) + _chromeWidth;
                }
                else
                {
                    int newHeight = (int)(clientWidth / _aspectRatio) + _chromeHeight;
                    if (edge == WMSZ_TOPLEFT || edge == WMSZ_TOPRIGHT)
                        rect.Top = rect.Bottom - newHeight;
                    else
                        rect.Bottom = rect.Top + newHeight;
                }

                Marshal.StructureToPtr(rect, lParam, false);
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                Key key = e.Key == Key.System ? e.SystemKey : e.Key;
                if (key == Key.Up)   { SetStayOnTop(true);  e.Handled = true; return; }
                if (key == Key.Down) { SetStayOnTop(false); e.Handled = true; return; }
            }

            Command? command = body.TranslateShortcut(e.Key, Keyboard.Modifiers.HasFlag(ModifierKeys.Control), Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            switch (command)
            {
                case null:
                    return;
                case Command.Arc:
                    body.HighlightButton(Command.Arc);
                    body.Arc = !body.Arc;
                    break;
                case Command.Hyp:
                    body.HighlightButton(Command.Hyp);
                    body.Hyp = !body.Hyp;
                    break;
                default:
                    body.HighlightButton(command.Value);
                    ProcessCommand((Command)command);
                    break;
            }
        }

        private void ProcessCommand(Command command)
        {
            // Enter and Eq share a shortcut: Enter maps to Eq in algebraic mode
            if (command == Command.Enter && _cpu.CurrentMode == Settings.Mode.Alg)
                command = Command.Eq;

            // ignore illegal commands invoked by shortcuts
            if (!command.IsValidButton(_cpu.CurrentMode)) return;

            switch (command)
            {
                case Command.Info:
                    var infoDialog = new InfoDialog();
                    infoDialog.Owner = this;
                    infoDialog.ShowDialog();
                    break;
                case Command.Settings:
                    var settings = new Settings(_cpu.CurrentStackMode,
                        _cpu.CurrentMode,
                        _cpu.CurrentEnterMode,
                        body.Round,
                        body.RoundLength,
                        body.TruncateZeros,
                        body.ArcAutorelease,
                        body.HypAutorelease,
                        body.CurrentPasteParsingAlgorithm,
                        _customButtonCommand,
                        _customButtonTooltip,
                        _cpu.CurrentAngleUnits,
                        body.CurrentDisplayFormat);
                    var settingsDialog = new SettingsDialog(settings, InputLength);
                    settingsDialog.Owner = this;
                    settingsDialog.ShowDialog();
                    if (settingsDialog.WasChanged)
                    {
                        if (settingsDialog.NewSettings.CurrentEnterMode != _cpu.CurrentEnterMode)
                            _cpu.CurrentEnterMode = settingsDialog.NewSettings.CurrentEnterMode;
                        if (settingsDialog.NewSettings.CurrentStackMode != _cpu.CurrentStackMode)
                            _cpu.CurrentStackMode = settingsDialog.NewSettings.CurrentStackMode;
                        if (settingsDialog.NewSettings.CurrentMode != _cpu.CurrentMode)
                        {
                            _cpu.CurrentMode = settingsDialog.NewSettings.CurrentMode;
                            body.DisplayOnlyActiveButtonsForMode(settingsDialog.NewSettings.CurrentMode);
                            _undoStack.Clear();
                            _redoStack.Clear();
                            body.UndoEnabled = false;
                            body.RedoEnabled = false;
                        }
                        body.ArcAutorelease = settingsDialog.NewSettings.ArcAutorelease;
                        body.HypAutorelease = settingsDialog.NewSettings.HypAutorelease;
                        body.CurrentPasteParsingAlgorithm = settingsDialog.NewSettings.CurrentPasteParsingAlgorithm;
                        body.Round = settingsDialog.NewSettings.Round;
                        body.RoundLength = settingsDialog.NewSettings.RoundLength;
                        body.TruncateZeros = settingsDialog.NewSettings.TruncateZeros;
                        _customButtonCommand = settingsDialog.NewSettings.CustomButtonCommand;
                        _customButtonTooltip = settingsDialog.NewSettings.CustomButtonTooltip;
                        body.SetCustomButtonTooltip(_customButtonTooltip);
                        settingsDialog.NewSettings.Save();
                    }
                    break;
                case Command.Undo:
                    if (_undoStack.Any())
                    {
                        _redoStack.AddFirst(_cpu);
                        _cpu = _undoStack.First();
                        _undoStack.RemoveFirst();

                        body.RedoEnabled = true;
                        if (!_undoStack.Any())
                            body.UndoEnabled = false;
                    }
                    break;
                case Command.Redo:
                    if (_redoStack.Any())
                    {
                        _undoStack.AddFirst(_cpu);
                        _cpu = _redoStack.First();
                        _redoStack.RemoveFirst();

                        body.UndoEnabled = true;
                        if (!_redoStack.Any())
                            body.RedoEnabled = false;
                    }
                    break;
                case Command.Format:
                    body.SwitchDisplayFormat();
                    Settings.SaveDisplayFormat(body.GetDisplayFormat());
                    break;
                case Command.DegRad:
                    _cpu.Execute(Command.DegRad);
                    Settings.SaveAngleUnits(_cpu.CurrentAngleUnits);
                    break;
                case Command.Copy:
                    try { Clipboard.SetText(body.GetMainDisplayString()); } catch { }
                    break;
                case Command.Paste:
                    try
                    {
                        if (!Clipboard.ContainsText())
                            break;
                        string textToPaste = Clipboard.GetText(TextDataFormat.Text);
                        double value = TextUtil.TextToDouble(textToPaste, body.CurrentPasteParsingAlgorithm == Settings.PasteParsingAlgorithm.LocaleBased);
                        if (double.IsNaN(value))
                            break;

                        PushUndo();
                        _cpu.ExecutePaste(value);
                    }
                    catch { }
                    break;
                case Command.Custom:
                    if (!string.IsNullOrEmpty(_customButtonCommand))
                    {
                        PushUndo();
                        foreach (string token in _customButtonCommand.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (!double.IsFinite(_cpu.X))
                                break;
                            if (Enum.TryParse(token, out Command parsed)
                                && !CommandExtensions.InvalidForCustomCommandSequence.Contains(parsed))
                                _cpu.Execute(parsed);
                        }
                    }
                    break;
                case Command.Exit:
                    Close();
                    break;
                default:
                    PushUndo();

                    if (CommandExtensions.TrigAll.Contains(command))
                    {
                        // special treatment for trigonometric buttons because
                        // they may autorelease arc and hyp
                        if (CommandExtensions.TrigBare.Contains(command))
                        {
                            Command dressedCommand = command.TrigBareToDressed(body.Arc, body.Hyp);
                            _cpu.Execute(dressedCommand);
                        }
                        else
                            _cpu.Execute(command);
                        if (body.ArcAutorelease)
                            body.Arc = false;
                        if (body.HypAutorelease)
                            body.Hyp = false;
                    }
                    else
                        _cpu.Execute(command);

                    break;
            }

            body.Refresh(_cpu);
        }

        private void SetStayOnTop(bool value)
        {
            Topmost = value;
            var hwnd = new WindowInteropHelper(this).Handle;
            var sysMenu = GetSystemMenu(hwnd, false);
            CheckMenuItem(sysMenu, IDM_STAY_ON_TOP, MF_BYCOMMAND | (Topmost ? MF_CHECKED : MF_UNCHECKED));
            Settings.SaveStayOnTop(value);
        }

        private void PushUndo()
        {
            _redoStack.Clear();
            _undoStack.AddFirst(_cpu.Clone());
            body.UndoEnabled = true;
            body.RedoEnabled = false;
            if (_undoStack.Count > UndoStackSize)
                _undoStack.RemoveLast();
        }
    }
}
