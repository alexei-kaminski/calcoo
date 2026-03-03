using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calcoo
{
    /// <summary>
    /// Interaction logic for CustomButtonDialog.xaml
    /// </summary>
    public partial class CustomButtonDialog : Window
    {
        public string CommandText { get; private set; }

        public CustomButtonDialog(string currentCommand)
        {
            InitializeComponent();
            SourceInitialized += (_, _) => App.ApplyDarkTitleBar(this);
            CommandTextBox.Text = currentCommand ?? "";
            CommandText = "";
        }

        private void CommandLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var label = sender as Label;
            if (label == null) return;

            string commandName = label.Content.ToString();
            int caretIndex = CommandTextBox.CaretIndex;
            string text = CommandTextBox.Text;

            // Insert command name + space at caret position
            string insertion = commandName + " ";
            CommandTextBox.Text = text.Insert(caretIndex, insertion);
            CommandTextBox.CaretIndex = caretIndex + insertion.Length;
            CommandTextBox.Focus();
        }

        private void TidyUp()
        {
            CommandTextBox.Text = Regex.Replace(CommandTextBox.Text.Trim(), @"\s+", " ");
        }

        private bool Validate()
        {
            string text = CommandTextBox.Text.Trim();
            if (string.IsNullOrEmpty(text))
                return true;

            string[] tokens = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tokens.Length; i++)
            {
                Command parsed;
                if (!Enum.TryParse(tokens[i], out parsed))
                {
                    MessageBox.Show(this, "Unknown command: " + tokens[i], "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    SelectToken(tokens, i);
                    return false;
                }
                if (CommandExtensions.InvalidForCustomCommandSequence.Contains(parsed))
                {
                    MessageBox.Show(this, "Command not allowed in custom sequence: " + tokens[i],
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    SelectToken(tokens, i);
                    return false;
                }
            }
            return true;
        }

        private void SelectToken(string[] tokens, int tokenIndex)
        {
            int start = 0;
            for (int i = 0; i < tokenIndex; i++)
                start += tokens[i].Length + 1;

            CommandTextBox.Focus();
            CommandTextBox.Select(start, tokens[tokenIndex].Length);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            TidyUp();
            if (!Validate()) return;
            CommandText = CommandTextBox.Text;
            DialogResult = true;
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            TidyUp();
            if (Validate())
            {
                MessageBox.Show(this, "Command sequence is valid.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
