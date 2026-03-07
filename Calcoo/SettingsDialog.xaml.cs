using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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
        private bool _initialized;

        public SettingsDialog(Settings settings, int maxRoundLength)
        {
            InitializeComponent();
            SourceInitialized += (_, _) =>
            {
                App.ApplyDarkTitleBar(this);
                App.ApplyMica(this);
            };

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
                    throw new ArgumentOutOfRangeException(nameof(settings.mode), settings.mode, null);
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
                    throw new ArgumentOutOfRangeException(nameof(settings.stackMode), settings.stackMode, null);
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
                    throw new ArgumentOutOfRangeException(nameof(settings.enterMode), settings.enterMode, null);
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
                    throw new ArgumentOutOfRangeException(nameof(settings.pasteParsingAlgorithm), settings.pasteParsingAlgorithm, null);
            }

            RoundingOutputCheckBox.IsChecked = settings.round;
            for (int i = maxRoundLength; i >= 1; i--)
                RoundingDigitsComboBox.Items.Add(i);
            RoundingDigitsComboBox.SelectedItem = settings.roundLength;
            TruncateZerosCheckBox.IsChecked = settings.truncateZeros;
            UpdateRoundingControlsEnabled(settings.round);
            _initialized = true;
        }

        private void UpdateRoundingControlsEnabled(bool roundingEnabled)
        {
            RoundingDigitsComboBox.IsEnabled = roundingEnabled;
            TruncateZerosCheckBox.IsEnabled = roundingEnabled;
        }

        private void RoundingDigitsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
                return;
            if (RoundingDigitsComboBox.SelectedItem is int value)
            {
                NewSettings.roundLength = value;
                WasChanged = true;
            }
        }

        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is RadioButton b)
            {
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

            if (e.Source is CheckBox cb)
            {
                WasChanged = true;
                switch (cb.Name)
                {
                    case"AutoreleaseArcButton":
                        NewSettings.arcAutorelease = AutoreleaseArcButton.IsChecked == true;
                        break;
                    case"AutoreleaseHypButton":
                        NewSettings.hypAutorelease = AutoreleaseHypButton.IsChecked == true;
                        break;
                    case "RoundingOutputCheckBox":
                        NewSettings.round = RoundingOutputCheckBox.IsChecked == true;
                        UpdateRoundingControlsEnabled(NewSettings.round);
                        break;
                    case "TruncateZerosCheckBox":
                        NewSettings.truncateZeros = TruncateZerosCheckBox.IsChecked == true;
                        break;
                }
                return;
            }

            if (e.Source is Button btn)
            {
                switch (btn.Name)
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
