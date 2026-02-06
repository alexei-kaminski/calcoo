using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;


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
       

        public MainWindow()
        {
            var settings = Settings.Load(InputLength);
            cpu = new Cpu(settings.mode, Settings.AngleUnits.Deg, InputLength, ExpInputLength, NumBase, NMem,
                settings.enterMode, settings.stackMode);
            undoStack = new LinkedList<Cpu>();
            redoStack = new LinkedList<Cpu>();

            InitializeComponent();

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
            body.UndoEnabled = false;
            body.RedoEnabled = false;

            body.Refresh(cpu);
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
                        false,
                        10,
                        false,
                        body.arcAutorelease,
                        body.hypAutorelease,
                        body.pasteParsingAlgorithm,
                        "");
                    var settingsDialog = new SettingsDialog(settings);
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
