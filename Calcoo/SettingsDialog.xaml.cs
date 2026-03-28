using System;
using System.Windows;
using System.Windows.Controls;

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

            (settings.CurrentStackMode switch
            {
                Settings.StackMode.Infinite => RpnStackInfinite,
                Settings.StackMode.Xyzt     => RpnStackXyzt,
                _ => throw new ArgumentOutOfRangeException(nameof(settings.CurrentStackMode), settings.CurrentStackMode, null)
            }).IsChecked = true;

            (settings.CurrentEnterMode switch
            {
                Settings.EnterMode.Traditional => RpnEnterTraditional,
                Settings.EnterMode.Hp28        => RpnEnterHp28,
                _ => throw new ArgumentOutOfRangeException(nameof(settings.CurrentEnterMode), settings.CurrentEnterMode, null)
            }).IsChecked = true;

            (settings.CurrentPasteParsingAlgorithm switch
            {
                Settings.PasteParsingAlgorithm.Heuristic   => PasteParsingHeuristic,
                Settings.PasteParsingAlgorithm.LocaleBased => PasteParsingLocaleBased,
                _ => throw new ArgumentOutOfRangeException(nameof(settings.CurrentPasteParsingAlgorithm), settings.CurrentPasteParsingAlgorithm, null)
            }).IsChecked = true;

            RoundingOutputCheckBox.IsChecked = settings.Round;
            for (int i = maxRoundLength; i >= 1; i--)
                RoundingDigitsComboBox.Items.Add(i);
            RoundingDigitsComboBox.SelectedItem = settings.RoundLength;
            TruncateZerosCheckBox.IsChecked = settings.TruncateZeros;
            UpdateRoundingControlsEnabled(settings.Round);

            (settings.CurrentRandomDistribution switch
            {
                Settings.RandomDistribution.Uniform => RandomDistributionUniform,
                Settings.RandomDistribution.Normal  => RandomDistributionNormal,
                _ => throw new ArgumentOutOfRangeException(nameof(settings.CurrentRandomDistribution), settings.CurrentRandomDistribution, null)
            }).IsChecked = true;

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

        private void ModeAlgebraic_Click(object sender, RoutedEventArgs e)
        {
            if (NewSettings.CurrentMode == Settings.Mode.Alg) return;
            RpnStackInfinite.IsEnabled = false;
            RpnStackXyzt.IsEnabled = false;
            RpnEnterTraditional.IsEnabled = false;
            RpnEnterHp28.IsEnabled = false;
            NewSettings.CurrentMode = Settings.Mode.Alg;
            WasChanged = true;
        }

        private void ModeRpn_Click(object sender, RoutedEventArgs e)
        {
            if (NewSettings.CurrentMode == Settings.Mode.Rpn) return;
            RpnStackInfinite.IsEnabled = true;
            RpnStackXyzt.IsEnabled = true;
            RpnEnterTraditional.IsEnabled = true;
            RpnEnterHp28.IsEnabled = true;
            NewSettings.CurrentMode = Settings.Mode.Rpn;
            WasChanged = true;
        }

        private void RpnEnterTraditional_Click(object sender, RoutedEventArgs e)
        {
            if (NewSettings.CurrentEnterMode == Settings.EnterMode.Traditional) return;
            NewSettings.CurrentEnterMode = Settings.EnterMode.Traditional;
            WasChanged = true;
        }

        private void RpnEnterHp28_Click(object sender, RoutedEventArgs e)
        {
            if (NewSettings.CurrentEnterMode == Settings.EnterMode.Hp28) return;
            NewSettings.CurrentEnterMode = Settings.EnterMode.Hp28;
            WasChanged = true;
        }

        private void RpnStackInfinite_Click(object sender, RoutedEventArgs e)
        {
            if (NewSettings.CurrentStackMode == Settings.StackMode.Infinite) return;
            NewSettings.CurrentStackMode = Settings.StackMode.Infinite;
            WasChanged = true;
        }

        private void RpnStackXyzt_Click(object sender, RoutedEventArgs e)
        {
            if (NewSettings.CurrentStackMode == Settings.StackMode.Xyzt) return;
            NewSettings.CurrentStackMode = Settings.StackMode.Xyzt;
            WasChanged = true;
        }

        private void PasteParsingLocaleBased_Click(object sender, RoutedEventArgs e)
        {
            if (NewSettings.CurrentPasteParsingAlgorithm == Settings.PasteParsingAlgorithm.LocaleBased) return;
            NewSettings.CurrentPasteParsingAlgorithm = Settings.PasteParsingAlgorithm.LocaleBased;
            WasChanged = true;
        }

        private void PasteParsingHeuristic_Click(object sender, RoutedEventArgs e)
        {
            if (NewSettings.CurrentPasteParsingAlgorithm == Settings.PasteParsingAlgorithm.Heuristic) return;
            NewSettings.CurrentPasteParsingAlgorithm = Settings.PasteParsingAlgorithm.Heuristic;
            WasChanged = true;
        }

        private void AutoreleaseArcButton_Click(object sender, RoutedEventArgs e)
        {
            bool value = AutoreleaseArcButton.IsChecked == true;
            if (NewSettings.ArcAutorelease == value) return;
            NewSettings.ArcAutorelease = value;
            WasChanged = true;
        }

        private void AutoreleaseHypButton_Click(object sender, RoutedEventArgs e)
        {
            bool value = AutoreleaseHypButton.IsChecked == true;
            if (NewSettings.HypAutorelease == value) return;
            NewSettings.HypAutorelease = value;
            WasChanged = true;
        }

        private void RoundingOutputCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool value = RoundingOutputCheckBox.IsChecked == true;
            if (NewSettings.Round == value) return;
            NewSettings.Round = value;
            UpdateRoundingControlsEnabled(NewSettings.Round);
            WasChanged = true;
        }

        private void TruncateZerosCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool value = TruncateZerosCheckBox.IsChecked == true;
            if (NewSettings.TruncateZeros == value) return;
            NewSettings.TruncateZeros = value;
            WasChanged = true;
        }

        private void RandomDistributionUniform_Click(object sender, RoutedEventArgs e)
        {
            if (NewSettings.CurrentRandomDistribution == Settings.RandomDistribution.Uniform) return;
            NewSettings.CurrentRandomDistribution = Settings.RandomDistribution.Uniform;
            WasChanged = true;
        }

        private void RandomDistributionNormal_Click(object sender, RoutedEventArgs e)
        {
            if (NewSettings.CurrentRandomDistribution == Settings.RandomDistribution.Normal) return;
            NewSettings.CurrentRandomDistribution = Settings.RandomDistribution.Normal;
            WasChanged = true;
        }

        private void SettingsOk_Click(object sender, RoutedEventArgs e) => Close();

        private void SettingsCancel_Click(object sender, RoutedEventArgs e)
        {
            WasChanged = false;
            Close();
        }

        private void CustomButton_Click(object sender, RoutedEventArgs e)
        {
            var customButtonDialog = new CustomButtonDialog(NewSettings.CustomButtonCommand, NewSettings.CustomButtonTooltip);
            customButtonDialog.Owner = this;
            if (customButtonDialog.ShowDialog() == true)
            {
                NewSettings.CustomButtonCommand = customButtonDialog.CommandText;
                NewSettings.CustomButtonTooltip = customButtonDialog.TooltipText;
                WasChanged = true;
            }
        }
    }
}
