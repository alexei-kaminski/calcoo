using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Calcoo
{
    /// <summary>
    /// Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {
        public Settings NewSettings;
        public bool WasChanged;
        private readonly int _maxRoundLength;

        public SettingsDialog(Settings settings, int maxRoundLength)
        {
            InitializeComponent();
            SourceInitialized += (_, _) => App.ApplyDarkTitleBar(this);

            WasChanged = false;
            NewSettings = settings.Clone();
            _maxRoundLength = maxRoundLength;

            AutoreleaseArcButton.IsChecked = settings.arcAutorelease;
            AutoreleaseHypButton.IsChecked = settings.hypAutorelease;

            switch (settings.mode)
            {
                case Settings.Mode.Alg:
                    RpnStackInfinite.IsEnabled = false;
                    RpnStackXyzt.IsEnabled = false;
                    RpnEnterTraditional.IsEnabled = false;
                    RpnEnterHp28.IsEnabled = false;
                    ModeAlgebraic.IsChecked = true;
                    break;
                case Settings.Mode.Rpn:
                    ModeRpn.IsChecked = true;
                    break;
                default:
                    throw new Exception("Unknown mode " + settings.mode);
            }
            switch (settings.stackMode)
            {
                case Settings.StackMode.Infinite:
                    RpnStackInfinite.IsChecked = true;
                    break;
                case Settings.StackMode.Xyzt:
                    RpnStackXyzt.IsChecked = true;
                    break;
                default:
                    throw new Exception("Unknown stack mode " + settings.stackMode);
            }
            switch (settings.enterMode)
            {
                case Settings.EnterMode.Traditional:
                    RpnEnterTraditional.IsChecked = true;
                    break;
                case Settings.EnterMode.Hp28:
                    RpnEnterHp28.IsChecked = true;
                    break;
                default:
                    throw new Exception("Unknown enter mode " + settings.enterMode);
            }
            switch (settings.pasteParsingAlgorithm)
            {
                case Settings.PasteParsingAlgorithm.Heuristic:
                    PasteParsingHeuristic.IsChecked = true;
                    break;
                case Settings.PasteParsingAlgorithm.LocaleBased:
                    PasteParsingLocaleBased.IsChecked = true;
                    break;
                default:
                    throw new Exception("Unknown paste parsing algorithm " + settings.pasteParsingAlgorithm);
            }

            RoundingOutputCheckBox.IsChecked = settings.round;
            RoundingDigitsTextBox.Text = settings.roundLength.ToString();
            TruncateZerosCheckBox.IsChecked = settings.truncateZeros;
            UpdateRoundingControlsEnabled(settings.round);
        }

        private void UpdateRoundingControlsEnabled(bool roundingEnabled)
        {
            RoundingDigitsTextBox.IsEnabled = roundingEnabled;
            RoundingDigitsUp.IsEnabled = roundingEnabled;
            RoundingDigitsDown.IsEnabled = roundingEnabled;
            TruncateZerosCheckBox.IsEnabled = roundingEnabled;
        }

        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source.GetType() == typeof (RadioButton))
            {
                RadioButton b = e.Source as RadioButton;
                if (b == null) return;
                WasChanged = true;
                switch (b.Name)
                {
                    case "ModeAlgebraic":
                        RpnStackInfinite.IsEnabled = false;
                        RpnStackXyzt.IsEnabled = false;
                        RpnEnterTraditional.IsEnabled = false;
                        RpnEnterHp28.IsEnabled = false;
                        NewSettings.mode = Settings.Mode.Alg;
                        break;
                    case "ModeRpn":
                        RpnStackInfinite.IsEnabled = true;
                        RpnStackXyzt.IsEnabled = true;
                        RpnEnterTraditional.IsEnabled = true;
                        RpnEnterHp28.IsEnabled = true;
                        NewSettings.mode = Settings.Mode.Rpn;
                        break;
                    case "RpnEnterTraditional":
                        NewSettings.enterMode = Settings.EnterMode.Traditional;
                        break;
                    case "RpnEnterHp28":
                        NewSettings.enterMode = Settings.EnterMode.Hp28;
                        break;
                    case "RpnStackInfinite":
                        NewSettings.stackMode = Settings.StackMode.Infinite;
                        break;
                    case "RpnStackXyzt":
                        NewSettings.stackMode = Settings.StackMode.Xyzt;
                        break;
                    case "PasteParsingHeuristic":
                        NewSettings.pasteParsingAlgorithm = Settings.PasteParsingAlgorithm.Heuristic;
                        break;
                    case "PasteParsingLocaleBased":
                        NewSettings.pasteParsingAlgorithm = Settings.PasteParsingAlgorithm.LocaleBased;
                        break;
                }
                return;
            }

            if (e.Source.GetType() == typeof(CheckBox))
            {
                CheckBox b = e.Source as CheckBox;
                if (b == null) return;
                WasChanged = true;
                switch (b.Name)
                {
                    case"AutoreleaseArcButton":
                        NewSettings.arcAutorelease = AutoreleaseArcButton.IsChecked.Equals(true);
                        break;
                    case"AutoreleaseHypButton":
                        NewSettings.hypAutorelease = AutoreleaseHypButton.IsChecked.Equals(true);
                        break;
                    case "RoundingOutputCheckBox":
                        NewSettings.round = RoundingOutputCheckBox.IsChecked.Equals(true);
                        UpdateRoundingControlsEnabled(NewSettings.round);
                        break;
                    case "TruncateZerosCheckBox":
                        NewSettings.truncateZeros = TruncateZerosCheckBox.IsChecked.Equals(true);
                        break;
                }
                return;
            }

            if (e.Source.GetType() == typeof(RepeatButton))
            {
                RepeatButton b = e.Source as RepeatButton;
                if (b == null) return;
                WasChanged = true;
                switch (b.Name)
                {
                    case "RoundingDigitsUp":
                        if (NewSettings.roundLength < _maxRoundLength)
                        {
                            NewSettings.roundLength++;
                            RoundingDigitsTextBox.Text = NewSettings.roundLength.ToString();
                        }
                        break;
                    case "RoundingDigitsDown":
                        if (NewSettings.roundLength > 1)
                        {
                            NewSettings.roundLength--;
                            RoundingDigitsTextBox.Text = NewSettings.roundLength.ToString();
                        }
                        break;
                }
                return;
            }

            if (e.Source.GetType() == typeof (Button))
            {
                Button b = e.Source as Button;
                if (b == null) return;
                switch (b.Name)
                {
                    case "SettingsOk":
                        Close();
                        break;
                    case "SettingsCancel":
                        WasChanged = false;
                        Close();
                        break;
                    case "CustomButton":
                        var customButtonDialog = new CustomButtonDialog(NewSettings.customButtonCommand);
                        customButtonDialog.Owner = this;
                        if (customButtonDialog.ShowDialog() == true)
                        {
                            NewSettings.customButtonCommand = customButtonDialog.CommandText;
                            WasChanged = true;
                        }
                        break;
                }
            }
        }
    }
}
