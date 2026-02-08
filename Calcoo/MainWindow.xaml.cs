using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
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
            cpu = new Cpu(settings.mode, settings.angleUnits, InputLength, ExpInputLength, NumBase, NMem,
                settings.enterMode, settings.stackMode);
            undoStack = new LinkedList<Cpu>();
            redoStack = new LinkedList<Cpu>();

            InitializeComponent();

            var rk = Registry.CurrentUser.OpenSubKey("Software\\Calcoo\\");
            if (rk != null)
            {
                if (rk.GetValue("WindowWidth") is int w && rk.GetValue("WindowHeight") is int h && w > 0 && h > 0)
                {
                    Width = w;
                    Height = h;
                }
            }

            var displayCanvas = new Body.DisplayCanvas(NMem, NRegister);
            displayCanvas.MainDisplay = MainDisplayCanvas;
            displayCanvas.DegRadDisplay = DegRadDisplayCanvas;
            displayCanvas.FormatDisplay = FormatDisplayCanvas;
            displayCanvas.MemDisplays[0] = Mem0DisplayCanvas;
            displayCanvas.MemDisplays[1] = Mem1DisplayCanvas;
            displayCanvas.RegisterDisplays[0] = Register0DisplayCanvas;
            displayCanvas.RegisterDisplays[1] = Register1DisplayCanvas;
            displayCanvas.RegisterDisplays[2] = Register2DisplayCanvas;

            body = new Body(MainGrid, displayCanvas, NumBase, InputLength, ExpInputLength);
            
            body.DisplayOnlyActiveButtonsForMode(settings.mode);
            body.arcAutorelease = settings.arcAutorelease;
            body.hypAutorelease = settings.hypAutorelease;
            body.pasteParsingAlgorithm = settings.pasteParsingAlgorithm;
            body.round = settings.round;
            body.roundLength = settings.roundLength;
            body.truncateZeros = settings.truncateZeros;
            body.displayFormat = settings.displayFormat;
            _customButtonCommand = settings.customButtonCommand;
            body.UndoEnabled = false;
            body.RedoEnabled = false;

            body.Refresh(cpu);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            var rk = Registry.CurrentUser.CreateSubKey("Software\\Calcoo\\");
            if (rk != null)
            {
                rk.SetValue("WindowWidth", (int)ActualWidth, RegistryValueKind.DWord);
                rk.SetValue("WindowHeight", (int)ActualHeight, RegistryValueKind.DWord);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;

            GetWindowRect(hwnd, out RECT windowRect);
            GetClientRect(hwnd, out RECT clientRect);

            _chromeWidth = (windowRect.Right - windowRect.Left) - (clientRect.Right - clientRect.Left);
            _chromeHeight = (windowRect.Bottom - windowRect.Top) - (clientRect.Bottom - clientRect.Top);
            _aspectRatio = (double)(clientRect.Right - clientRect.Left) / (clientRect.Bottom - clientRect.Top);

            var source = HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
        }

        private const int WM_SIZING = 0x0214;
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
            if (msg == WM_SIZING)
            {
                var rect = Marshal.PtrToStructure<RECT>(lParam);
                int edge = wParam.ToInt32();
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
            if (e.Source.GetType() == typeof (ToggleButton))
                return;

            var b = e.Source as Button;

            if (b == null)
                return;

            ProcessCommand( (Command) Enum.Parse(typeof (Command), b.Name));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Command? command = body.TranslateShortcut(e.Key, Keyboard.Modifiers == ModifierKeys.Control, Keyboard.Modifiers == ModifierKeys.Shift);
            switch (command) { 
                case null:
                    return;
                case Command.Arc:
                    body.Arc = !body.Arc;
                    break;
                case Command.Hyp:
                    body.Hyp = !body.Hyp;
                    break;
                default:
                    ProcessCommand((Command)command);
                    break;
            }
        }

        private void ProcessCommand(Command command)
        {
            // ignore illegal commands invoked by shortcuts
            if ((command == Command.StackDown || command == Command.StackUp) && cpu.Mode == Settings.Mode.Alg)
                return;
            if ((command == Command.LeftParen || command == Command.RightParen) && cpu.Mode == Settings.Mode.Rpn)
                return;

            switch (command)
            {
                case Command.Info:
                    var infoDialog = new InfoDialog();
                    infoDialog.Owner = this;
                    infoDialog.ShowDialog();
                    break;
                case Command.Settings:
                    var settings = new Settings(cpu.StackMode,
                        cpu.Mode,
                        cpu.EnterMode,
                        body.round,
                        body.roundLength,
                        body.truncateZeros,
                        body.arcAutorelease,
                        body.hypAutorelease,
                        body.pasteParsingAlgorithm,
                        _customButtonCommand,
                        cpu.AngleUnits,
                        body.displayFormat);
                    var settingsDialog = new SettingsDialog(settings, InputLength);
                    settingsDialog.Owner = this;
                    settingsDialog.ShowDialog();
                    if (settingsDialog.WasChanged)
                    {
                        if (settingsDialog.NewSettings.enterMode != cpu.EnterMode)
                            cpu.EnterMode = settingsDialog.NewSettings.enterMode;
                        if (settingsDialog.NewSettings.stackMode != cpu.StackMode)
                            cpu.StackMode = settingsDialog.NewSettings.stackMode;
                        if (settingsDialog.NewSettings.mode != cpu.Mode)
                        {
                            cpu.Mode = settingsDialog.NewSettings.mode;
                            body.DisplayOnlyActiveButtonsForMode(settingsDialog.NewSettings.mode);
                        }
                        body.arcAutorelease = settingsDialog.NewSettings.arcAutorelease;
                        body.hypAutorelease = settingsDialog.NewSettings.hypAutorelease;
                        body.pasteParsingAlgorithm = settingsDialog.NewSettings.pasteParsingAlgorithm;
                        body.round = settingsDialog.NewSettings.round;
                        body.roundLength = settingsDialog.NewSettings.roundLength;
                        body.truncateZeros = settingsDialog.NewSettings.truncateZeros;
                        _customButtonCommand = settingsDialog.NewSettings.customButtonCommand;
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
                    //Settings.saveDisplayFormat(body.getDisplayFormat()); FIXME
                    break;
                case Command.Copy:
                    //Clipboard.SetText(mainDisplay.Content.ToString()); FIXME
                    break;
                case Command.Paste:
                    if (!Clipboard.ContainsText())
                        break;
                    String textToPaste = Clipboard.GetText(TextDataFormat.Text);
                    double value = TextUtil.TextToDouble(textToPaste, body.pasteParsingAlgorithm == Settings.PasteParsingAlgorithm.LocaleBased);
                    if (Double.IsNaN(value))
                        break;

                    redoStack.Clear();
                    undoStack.AddFirst(cpu.Clone());
                    body.UndoEnabled = true;
                    body.RedoEnabled = false;
                    if (undoStack.Count > UndoStackSize)
                        undoStack.RemoveLast();

                    cpu.ExecutePaste(value);
                    break;
                case Command.Custom:
                    if (!string.IsNullOrEmpty(_customButtonCommand))
                    {
                        redoStack.Clear();
                        undoStack.AddFirst(cpu.Clone());
                        body.UndoEnabled = true;
                        body.RedoEnabled = false;
                        if (undoStack.Count > UndoStackSize)
                            undoStack.RemoveLast();

                        foreach (string token in _customButtonCommand.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            Command parsed;
                            if (Enum.TryParse(token, out parsed))
                                cpu.Execute(parsed);
                        }
                    }
                    break;
                case Command.Exit:
                    Close();
                    break;
                default:
                    redoStack.Clear();
                    undoStack.AddFirst(cpu.Clone());
                    body.UndoEnabled = true;
                    body.RedoEnabled = false;
                    if (undoStack.Count > UndoStackSize)
                        undoStack.RemoveLast();

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
                        if (body.arcAutorelease)
                            body.Arc = false;
                        if (body.hypAutorelease)
                            body.Hyp = false;
                    }
                    else if (command == Command.Enter && cpu.Mode == Settings.Mode.Alg)
                        // "ENTER" and "EQ" share a shortcut which calls Command.Enter
                        cpu.Execute(Command.Eq);
                    else
                        // non-trigonometric buttons
                        cpu.Execute(command);

                    break;
            }

            body.Refresh(cpu);
        }
    }
}
