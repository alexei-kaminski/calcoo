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
        private bool _initialized;

        public SettingsDialog(Settings settings, int maxRoundLength)
        {
            InitializeComponent();
            App.ApplyDialogTheme(this);

            WasChanged = false;
            NewSettings = settings.Clone();
            CustomButton.ToolTip = CommandExtensions.CustomButtonTooltip;
            AutoreleaseArcButton.IsChecked = settings.ArcAutorelease;
            AutoreleaseHypButton.IsChecked = settings.HypAutorelease;

            switch (settings.CurrentMode)
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
                    throw new ArgumentOutOfRangeException(nameof(settings.CurrentMode), settings.CurrentMode, null);
            }
            switch (settings.CurrentStackMode)
            {
                case Settings.StackMode.Infinite:
                    RpnStackInfinite.IsChecked = true;
                    break;
                case Settings.StackMode.Xyzt:
                    RpnStackXyzt.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.CurrentStackMode), settings.CurrentStackMode, null);
            }
            switch (settings.CurrentEnterMode)
            {
                case Settings.EnterMode.Traditional:
                    RpnEnterTraditional.IsChecked = true;
                    break;
                case Settings.EnterMode.Hp28:
                    RpnEnterHp28.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.CurrentEnterMode), settings.CurrentEnterMode, null);
            }
            switch (settings.CurrentPasteParsingAlgorithm)
            {
                case Settings.PasteParsingAlgorithm.Heuristic:
                    PasteParsingHeuristic.IsChecked = true;
                    break;
                case Settings.PasteParsingAlgorithm.LocaleBased:
                    PasteParsingLocaleBased.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.CurrentPasteParsingAlgorithm), settings.CurrentPasteParsingAlgorithm, null);
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
                    case "ModeAlgebraic" when NewSettings.CurrentMode != Settings.Mode.Alg:
                        RpnStackInfinite.IsEnabled = false;
                        RpnStackXyzt.IsEnabled = false;
                        RpnEnterTraditional.IsEnabled = false;
                        RpnEnterHp28.IsEnabled = false;
                        NewSettings.CurrentMode = Settings.Mode.Alg;
                        WasChanged = true;
                        break;
                    case "ModeRpn" when NewSettings.CurrentMode != Settings.Mode.Rpn:
                        RpnStackInfinite.IsEnabled = true;
                        RpnStackXyzt.IsEnabled = true;
                        RpnEnterTraditional.IsEnabled = true;
                        RpnEnterHp28.IsEnabled = true;
                        NewSettings.CurrentMode = Settings.Mode.Rpn;
                        WasChanged = true;
                        break;
                    case "RpnEnterTraditional" when NewSettings.CurrentEnterMode != Settings.EnterMode.Traditional:
                        NewSettings.CurrentEnterMode = Settings.EnterMode.Traditional;
                        WasChanged = true;
                        break;
                    case "RpnEnterHp28" when NewSettings.CurrentEnterMode != Settings.EnterMode.Hp28:
                        NewSettings.CurrentEnterMode = Settings.EnterMode.Hp28;
                        WasChanged = true;
                        break;
                    case "RpnStackInfinite" when NewSettings.CurrentStackMode != Settings.StackMode.Infinite:
                        NewSettings.CurrentStackMode = Settings.StackMode.Infinite;
                        WasChanged = true;
                        break;
                    case "RpnStackXyzt" when NewSettings.CurrentStackMode != Settings.StackMode.Xyzt:
                        NewSettings.CurrentStackMode = Settings.StackMode.Xyzt;
                        WasChanged = true;
                        break;
                    case "PasteParsingHeuristic" when NewSettings.CurrentPasteParsingAlgorithm != Settings.PasteParsingAlgorithm.Heuristic:
                        NewSettings.CurrentPasteParsingAlgorithm = Settings.PasteParsingAlgorithm.Heuristic;
                        WasChanged = true;
                        break;
                    case "PasteParsingLocaleBased" when NewSettings.CurrentPasteParsingAlgorithm != Settings.PasteParsingAlgorithm.LocaleBased:
                        NewSettings.CurrentPasteParsingAlgorithm = Settings.PasteParsingAlgorithm.LocaleBased;
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
