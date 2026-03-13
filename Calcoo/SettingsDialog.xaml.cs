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
            App.ApplyDialogTheme(this);

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
                switch (b.Name)
                {
                    case "ModeAlgebraic" when NewSettings.mode != Settings.Mode.Alg:
                        RpnStackInfinite.IsEnabled = false;
                        RpnStackXyzt.IsEnabled = false;
                        RpnEnterTraditional.IsEnabled = false;
                        RpnEnterHp28.IsEnabled = false;
                        NewSettings.mode = Settings.Mode.Alg;
                        WasChanged = true;
                        break;
                    case "ModeRpn" when NewSettings.mode != Settings.Mode.Rpn:
                        RpnStackInfinite.IsEnabled = true;
                        RpnStackXyzt.IsEnabled = true;
                        RpnEnterTraditional.IsEnabled = true;
                        RpnEnterHp28.IsEnabled = true;
                        NewSettings.mode = Settings.Mode.Rpn;
                        WasChanged = true;
                        break;
                    case "RpnEnterTraditional" when NewSettings.enterMode != Settings.EnterMode.Traditional:
                        NewSettings.enterMode = Settings.EnterMode.Traditional;
                        WasChanged = true;
                        break;
                    case "RpnEnterHp28" when NewSettings.enterMode != Settings.EnterMode.Hp28:
                        NewSettings.enterMode = Settings.EnterMode.Hp28;
                        WasChanged = true;
                        break;
                    case "RpnStackInfinite" when NewSettings.stackMode != Settings.StackMode.Infinite:
                        NewSettings.stackMode = Settings.StackMode.Infinite;
                        WasChanged = true;
                        break;
                    case "RpnStackXyzt" when NewSettings.stackMode != Settings.StackMode.Xyzt:
                        NewSettings.stackMode = Settings.StackMode.Xyzt;
                        WasChanged = true;
                        break;
                    case "PasteParsingHeuristic" when NewSettings.pasteParsingAlgorithm != Settings.PasteParsingAlgorithm.Heuristic:
                        NewSettings.pasteParsingAlgorithm = Settings.PasteParsingAlgorithm.Heuristic;
                        WasChanged = true;
                        break;
                    case "PasteParsingLocaleBased" when NewSettings.pasteParsingAlgorithm != Settings.PasteParsingAlgorithm.LocaleBased:
                        NewSettings.pasteParsingAlgorithm = Settings.PasteParsingAlgorithm.LocaleBased;
                        WasChanged = true;
                        break;
                }
                return;
            }

            if (e.Source is CheckBox cb)
            {
                bool value = cb.IsChecked == true;
                switch (cb.Name)
                {
                    case "AutoreleaseArcButton" when NewSettings.arcAutorelease != value:
                        NewSettings.arcAutorelease = value;
                        WasChanged = true;
                        break;
                    case "AutoreleaseHypButton" when NewSettings.hypAutorelease != value:
                        NewSettings.hypAutorelease = value;
                        WasChanged = true;
                        break;
                    case "RoundingOutputCheckBox" when NewSettings.round != value:
                        NewSettings.round = value;
                        UpdateRoundingControlsEnabled(NewSettings.round);
                        WasChanged = true;
                        break;
                    case "TruncateZerosCheckBox" when NewSettings.truncateZeros != value:
                        NewSettings.truncateZeros = value;
                        WasChanged = true;
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
