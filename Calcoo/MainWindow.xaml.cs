using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private Cpu cpu;
        private LinkedList<Cpu> undoStack, redoStack;
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

        public MainWindow()
        {
            var settings = Settings.Load(InputLength);
            cpu = new Cpu(settings.CurrentMode, settings.CurrentAngleUnits, InputLength, ExpInputLength, NumBase, NMem,
                settings.CurrentEnterMode, settings.CurrentStackMode);
            undoStack = new LinkedList<Cpu>();
            redoStack = new LinkedList<Cpu>();

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

            body = new Body(MainGrid, displayCanvas, NumBase, InputLength, ExpInputLength);

            body.DisplayOnlyActiveButtonsForMode(settings.CurrentMode);
            body.ArcAutorelease = settings.ArcAutorelease;
            body.HypAutorelease = settings.HypAutorelease;
            body.CurrentPasteParsingAlgorithm = settings.CurrentPasteParsingAlgorithm;
            body.Round = settings.Round;
            body.RoundLength = settings.RoundLength;
            body.TruncateZeros = settings.TruncateZeros;
            body.CurrentDisplayFormat = settings.CurrentDisplayFormat;
            _customButtonCommand = settings.CustomButtonCommand;
            body.UndoEnabled = false;
            body.RedoEnabled = false;

            body.Refresh(cpu);
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
        }


        private const int WM_SIZING = 0x0214;
        private const int WM_SETTINGCHANGE = 0x001A;
        private const int WMSZ_TOP = 3;
        private const int WMSZ_TOPLEFT = 4;
        private const int WMSZ_TOPRIGHT = 5;
        private const int WMSZ_BOTTOM = 6;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SETTINGCHANGE)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is ToggleButton)
                return;

            if (e.Source is not Button b)
                return;

            if (Enum.TryParse(b.Name, out Command command))
                ProcessCommand(command);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
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
            // ignore illegal commands invoked by shortcuts
            if ((command == Command.StackDown || command == Command.StackUp) && cpu.CurrentMode == Settings.Mode.Alg)
                return;
            if ((command == Command.LeftParen || command == Command.RightParen) && cpu.CurrentMode == Settings.Mode.Rpn)
                return;

            switch (command)
            {
                case Command.Info:
                    var infoDialog = new InfoDialog();
                    infoDialog.Owner = this;
                    infoDialog.ShowDialog();
                    break;
                case Command.Settings:
                    var settings = new Settings(cpu.CurrentStackMode,
                        cpu.CurrentMode,
                        cpu.CurrentEnterMode,
                        body.Round,
                        body.RoundLength,
                        body.TruncateZeros,
                        body.ArcAutorelease,
                        body.HypAutorelease,
                        body.CurrentPasteParsingAlgorithm,
                        _customButtonCommand,
                        cpu.CurrentAngleUnits,
                        body.CurrentDisplayFormat);
                    var settingsDialog = new SettingsDialog(settings, InputLength);
                    settingsDialog.Owner = this;
                    settingsDialog.ShowDialog();
                    if (settingsDialog.WasChanged)
                    {
                        if (settingsDialog.NewSettings.CurrentEnterMode != cpu.CurrentEnterMode)
                            cpu.CurrentEnterMode = settingsDialog.NewSettings.CurrentEnterMode;
                        if (settingsDialog.NewSettings.CurrentStackMode != cpu.CurrentStackMode)
                            cpu.CurrentStackMode = settingsDialog.NewSettings.CurrentStackMode;
                        if (settingsDialog.NewSettings.CurrentMode != cpu.CurrentMode)
                        {
                            cpu.CurrentMode = settingsDialog.NewSettings.CurrentMode;
                            body.DisplayOnlyActiveButtonsForMode(settingsDialog.NewSettings.CurrentMode);
                        }
                        body.ArcAutorelease = settingsDialog.NewSettings.ArcAutorelease;
                        body.HypAutorelease = settingsDialog.NewSettings.HypAutorelease;
                        body.CurrentPasteParsingAlgorithm = settingsDialog.NewSettings.CurrentPasteParsingAlgorithm;
                        body.Round = settingsDialog.NewSettings.Round;
                        body.RoundLength = settingsDialog.NewSettings.RoundLength;
                        body.TruncateZeros = settingsDialog.NewSettings.TruncateZeros;
                        _customButtonCommand = settingsDialog.NewSettings.CustomButtonCommand;
                        settingsDialog.NewSettings.Save();
                    }
                    break;
                case Command.Undo:
                    if (undoStack.Any())
                    {
                        redoStack.AddFirst(cpu);
                        cpu = undoStack.First();
                        undoStack.RemoveFirst();

                        body.RedoEnabled = true;
                        if (!undoStack.Any())
                            body.UndoEnabled = false;
                    }
                    break;
                case Command.Redo:
                    if (redoStack.Any())
                    {
                        undoStack.AddFirst(cpu);
                        cpu = redoStack.First();
                        redoStack.RemoveFirst();

                        body.UndoEnabled = true;
                        if (!redoStack.Any())
                            body.RedoEnabled = false;
                    }
                    break;
                case Command.Format:
                    body.SwitchDisplayFormat();
                    Settings.SaveDisplayFormat(body.GetDisplayFormat());
                    break;
                case Command.DegRad:
                    cpu.Execute(Command.DegRad);
                    Settings.SaveAngleUnits(cpu.CurrentAngleUnits);
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
                        cpu.ExecutePaste(value);
                    }
                    catch { }
                    break;
                case Command.Custom:
                    if (!string.IsNullOrEmpty(_customButtonCommand))
                    {
                        PushUndo();
                        foreach (string token in _customButtonCommand.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (!double.IsFinite(cpu.X))
                                break;
                            if (Enum.TryParse(token, out Command parsed)
                                && !CommandExtensions.InvalidForCustomCommandSequence.Contains(parsed))
                                cpu.Execute(parsed);
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
                            cpu.Execute(dressedCommand);
                        }
                        else
                            cpu.Execute(command);
                        if (body.ArcAutorelease)
                            body.Arc = false;
                        if (body.HypAutorelease)
                            body.Hyp = false;
                    }
                    else if (command == Command.Enter && cpu.CurrentMode == Settings.Mode.Alg)
                        // "ENTER" and "EQ" share a shortcut which calls Command.Enter
                        cpu.Execute(Command.Eq);
                    else
                        // non-trigonometric buttons
                        cpu.Execute(command);

                    break;
            }

            body.Refresh(cpu);
        }

        private void PushUndo()
        {
            redoStack.Clear();
            undoStack.AddFirst(cpu.Clone());
            body.UndoEnabled = true;
            body.RedoEnabled = false;
            if (undoStack.Count > UndoStackSize)
                undoStack.RemoveLast();
        }
    }
}
