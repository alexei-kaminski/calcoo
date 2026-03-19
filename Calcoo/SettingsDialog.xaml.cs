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

            CustomButton.ToolTip = CommandExtensions.CustomButtonTooltip;
            AutoreleaseArcButton.IsChecked = settings.ArcAutorelease;
            AutoreleaseHypButton.IsChecked = settings.HypAutorelease;

            switch (settings.Mode)
            {
                case Settings.ModeType.Alg:
                    RpnStackInfinite.IsEnabled = false;
                    RpnStackXyzt.IsEnabled = false;
                    RpnEnterTraditional.IsEnabled = false;
                    RpnEnterHp28.IsEnabled = false;
                    ModeAlgebraic.IsChecked = true;
                    break;
                case Settings.ModeType.Rpn:
                    ModeRpn.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.Mode), settings.Mode, null);
            }
            switch (settings.StackMode)
            {
                case Settings.StackModeType.Infinite:
                    RpnStackInfinite.IsChecked = true;
                    break;
                case Settings.StackModeType.Xyzt:
                    RpnStackXyzt.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.StackMode), settings.StackMode, null);
            }
            switch (settings.EnterMode)
            {
                case Settings.EnterModeType.Traditional:
                    RpnEnterTraditional.IsChecked = true;
                    break;
                case Settings.EnterModeType.Hp28:
                    RpnEnterHp28.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.EnterMode), settings.EnterMode, null);
            }
            switch (settings.PasteParsingAlgorithm)
            {
                case Settings.PasteParsingAlgorithmType.Heuristic:
                    PasteParsingHeuristic.IsChecked = true;
                    break;
                case Settings.PasteParsingAlgorithmType.LocaleBased:
                    PasteParsingLocaleBased.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.PasteParsingAlgorithm), settings.PasteParsingAlgorithm, null);
            }

            RoundingOutputCheckBox.IsChecked = settings.Round;
            for (int i = maxRoundLength; i >= 1; i--)
                RoundingDigitsComboBox.Items.Add(i);
            RoundingDigitsComboBox.SelectedItem = settings.RoundLength;
            TruncateZerosCheckBox.IsChecked = settings.TruncateZeros;
            UpdateRoundingControlsEnabled(settings.Round);
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
                NewSettings.RoundLength = value;
                WasChanged = true;
            }
        }

        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is RadioButton b)
            {
                switch (b.Name)
                {
                    case "ModeAlgebraic" when NewSettings.Mode != Settings.ModeType.Alg:
                        RpnStackInfinite.IsEnabled = false;
                        RpnStackXyzt.IsEnabled = false;
                        RpnEnterTraditional.IsEnabled = false;
                        RpnEnterHp28.IsEnabled = false;
                        NewSettings.Mode = Settings.ModeType.Alg;
                        WasChanged = true;
                        break;
                    case "ModeRpn" when NewSettings.Mode != Settings.ModeType.Rpn:
                        RpnStackInfinite.IsEnabled = true;
                        RpnStackXyzt.IsEnabled = true;
                        RpnEnterTraditional.IsEnabled = true;
                        RpnEnterHp28.IsEnabled = true;
                        NewSettings.Mode = Settings.ModeType.Rpn;
                        WasChanged = true;
                        break;
                    case "RpnEnterTraditional" when NewSettings.EnterMode != Settings.EnterModeType.Traditional:
                        NewSettings.EnterMode = Settings.EnterModeType.Traditional;
                        WasChanged = true;
                        break;
                    case "RpnEnterHp28" when NewSettings.EnterMode != Settings.EnterModeType.Hp28:
                        NewSettings.EnterMode = Settings.EnterModeType.Hp28;
                        WasChanged = true;
                        break;
                    case "RpnStackInfinite" when NewSettings.StackMode != Settings.StackModeType.Infinite:
                        NewSettings.StackMode = Settings.StackModeType.Infinite;
                        WasChanged = true;
                        break;
                    case "RpnStackXyzt" when NewSettings.StackMode != Settings.StackModeType.Xyzt:
                        NewSettings.StackMode = Settings.StackModeType.Xyzt;
                        WasChanged = true;
                        break;
                    case "PasteParsingHeuristic" when NewSettings.PasteParsingAlgorithm != Settings.PasteParsingAlgorithmType.Heuristic:
                        NewSettings.PasteParsingAlgorithm = Settings.PasteParsingAlgorithmType.Heuristic;
                        WasChanged = true;
                        break;
                    case "PasteParsingLocaleBased" when NewSettings.PasteParsingAlgorithm != Settings.PasteParsingAlgorithmType.LocaleBased:
                        NewSettings.PasteParsingAlgorithm = Settings.PasteParsingAlgorithmType.LocaleBased;
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
                    case "AutoreleaseArcButton" when NewSettings.ArcAutorelease != value:
                        NewSettings.ArcAutorelease = value;
                        WasChanged = true;
                        break;
                    case "AutoreleaseHypButton" when NewSettings.HypAutorelease != value:
                        NewSettings.HypAutorelease = value;
                        WasChanged = true;
                        break;
                    case "RoundingOutputCheckBox" when NewSettings.Round != value:
                        NewSettings.Round = value;
                        UpdateRoundingControlsEnabled(NewSettings.Round);
                        WasChanged = true;
                        break;
                    case "TruncateZerosCheckBox" when NewSettings.TruncateZeros != value:
                        NewSettings.TruncateZeros = value;
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
                        var customButtonDialog = new CustomButtonDialog(NewSettings.CustomButtonCommand);
                        customButtonDialog.Owner = this;
                        if (customButtonDialog.ShowDialog() == true)
                        {
                            NewSettings.CustomButtonCommand = customButtonDialog.CommandText;
                            WasChanged = true;
                        }
                        break;
                }
            }
        }
    }
}
