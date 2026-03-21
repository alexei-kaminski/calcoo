using System;
using System.Linq;
using System.Text;

using Microsoft.Win32;

namespace Calcoo
{
    public class Settings
    {
        public enum StackMode
        {
            Xyzt,
            Infinite
        }

        public StackMode CurrentStackMode;

        public enum Mode
        {
            Rpn,
            Alg
        }

        public Mode CurrentMode;

        public enum EnterMode
        {
            Traditional,
            Hp28
        }

        public EnterMode CurrentEnterMode;

        public enum PasteParsingAlgorithm
        {
            LocaleBased,
            Heuristic
        }

        public PasteParsingAlgorithm CurrentPasteParsingAlgorithm;


        public bool Round;
        public int RoundLength;
        public bool TruncateZeros;
        public bool ArcAutorelease;
        public bool HypAutorelease;

        public string CustomButtonCommand;

        public AngleUnits CurrentAngleUnits;
        public DisplayFormat CurrentDisplayFormat;

        private static class Defaults
        {
            public const StackMode StackMode = Settings.StackMode.Infinite;
            public const Mode Mode = Settings.Mode.Alg;
            public const EnterMode EnterMode = Settings.EnterMode.Traditional;
            public const bool Round = false;
            public const bool TruncateZeros = false;
            public const bool ArcAutorelease = true;
            public const bool HypAutorelease = true;
            public const AngleUnits AngleUnits = Settings.AngleUnits.Deg;
            public const DisplayFormat DisplayFormat = Settings.DisplayFormat.Fix;

            public const PasteParsingAlgorithm PasteParsingAlgorithm =
                Settings.PasteParsingAlgorithm.LocaleBased;

            public const string CustomButtonCommand = "";
        }

        // set by the buttons
        public enum AngleUnits
        {
            Deg,
            Rad
        }

        public enum DisplayFormat
        {
            Fix,
            Sci,
            Eng
        }

        public static int ExpDivisor(DisplayFormat displayFormat)
        {
            return (displayFormat == DisplayFormat.Eng ? 3 : 1);
        }

        public Settings(StackMode stackMode,
            Mode mode,
            EnterMode enterMode,
            bool round,
            int roundLength,
            bool truncateZeros,
            bool arcAutorelease,
            bool hypAutorelease,
            PasteParsingAlgorithm pasteParsingAlgorithm,
            string customButtonCommand,
            AngleUnits angleUnits,
            DisplayFormat displayFormat)
        {
            this.CurrentStackMode = stackMode;
            this.CurrentMode = mode;
            this.CurrentEnterMode = enterMode;
            this.Round = round;
            this.RoundLength = roundLength;
            this.TruncateZeros = truncateZeros;
            this.ArcAutorelease = arcAutorelease;
            this.HypAutorelease = hypAutorelease;
            this.CurrentPasteParsingAlgorithm = pasteParsingAlgorithm;
            this.CustomButtonCommand = customButtonCommand;
            this.CurrentAngleUnits = angleUnits;
            this.CurrentDisplayFormat = displayFormat;
        }

        public Settings Clone()
        {
            return new Settings(
                CurrentStackMode,
                CurrentMode,
                CurrentEnterMode,
                Round,
                RoundLength,
                TruncateZeros,
                ArcAutorelease,
                HypAutorelease,
                CurrentPasteParsingAlgorithm,
                CustomButtonCommand,
                CurrentAngleUnits,
                CurrentDisplayFormat);
        }

        public Settings(int roundLength)
        {
            CurrentStackMode = Defaults.StackMode;
            CurrentMode = Defaults.Mode;
            CurrentEnterMode = Defaults.EnterMode;
            Round = Defaults.Round;
            RoundLength = roundLength;
            TruncateZeros = Defaults.TruncateZeros;
            ArcAutorelease = Defaults.ArcAutorelease;
            HypAutorelease = Defaults.HypAutorelease;
            CurrentPasteParsingAlgorithm = Defaults.PasteParsingAlgorithm;
            CustomButtonCommand = Defaults.CustomButtonCommand;
            CurrentAngleUnits = Defaults.AngleUnits;
            CurrentDisplayFormat = Defaults.DisplayFormat;
        }

        private static class Names
        {
            public static string registryPath = "Software\\Calcoo\\";
            public static string stackMode = "StackMode";
            public static string mode = "Mode";
            public static string enterMode = "EnterMode";
            public static string round = "Round";
            public static string roundLength = "RoundLength";
            public static string truncateZeros = "TruncateZeros";
            public static string arcAutorelease = "ArcAutorelease";
            public static string hypAutorelease = "HypAutorelease";
            public static string angleUnits = "AngleUnits";
            public static string displayFormat = "DisplayFormat";
            public static string pasteParsingAlgorithm = "PasteParsingAlgorithm";
            public static string customButtonCommand = "CustomButtonCommand";
        }

        private static string CleanUpCustomCommand(string customCommand)
        {
            string[] commandCandidates = customCommand.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (!commandCandidates.Any())
                return "";
            var cleanCustomCommand = new StringBuilder();

            foreach (string commandCandidate in commandCandidates)
            {
                Command thisCommand;
                if (Enum.TryParse(commandCandidate, out thisCommand) &&
                    !CommandExtensions.InvalidForCustomCommandSequence.Contains(thisCommand))
                {
                    cleanCustomCommand.Append(thisCommand);
                    cleanCustomCommand.Append(" ");
                }
            }
            return cleanCustomCommand.ToString().Trim();
        }

        public static Settings Load(int defaultRoundLength)
        {
            var settings = new Settings(defaultRoundLength);
            using var rk = Registry.CurrentUser.OpenSubKey(Names.registryPath);
            if (rk == null)
                return settings;

            if (!Enum.TryParse((string)rk.GetValue(Names.mode, Defaults.Mode.ToString(), RegistryValueOptions.None), out settings.CurrentMode))
                settings.CurrentMode = Defaults.Mode;
            if (!Enum.TryParse((string)rk.GetValue(Names.stackMode, Defaults.StackMode.ToString(), RegistryValueOptions.None), out settings.CurrentStackMode))
                settings.CurrentStackMode = Defaults.StackMode;
            if (!Enum.TryParse((string)rk.GetValue(Names.enterMode, Defaults.EnterMode.ToString(), RegistryValueOptions.None), out settings.CurrentEnterMode))
                settings.CurrentEnterMode = Defaults.EnterMode;
            if (!bool.TryParse((string)rk.GetValue(Names.round, Defaults.Round.ToString(), RegistryValueOptions.None), out settings.Round))
                settings.Round = Defaults.Round;
            try
            {
                if (rk.GetValueKind(Names.roundLength) == RegistryValueKind.DWord)
                    settings.RoundLength = (int)rk.GetValue(Names.roundLength, defaultRoundLength, RegistryValueOptions.None);
                else
                    settings.RoundLength = defaultRoundLength;
            }
            catch (Exception)
            {
                settings.RoundLength = defaultRoundLength;
            }
            if (!bool.TryParse((string)rk.GetValue(Names.truncateZeros, Defaults.TruncateZeros.ToString(), RegistryValueOptions.None), out settings.TruncateZeros))
                settings.TruncateZeros = Defaults.TruncateZeros;
            if (!bool.TryParse((string)rk.GetValue(Names.arcAutorelease, Defaults.ArcAutorelease.ToString(), RegistryValueOptions.None), out settings.ArcAutorelease))
                settings.ArcAutorelease = Defaults.ArcAutorelease;
            if (!bool.TryParse((string)rk.GetValue(Names.hypAutorelease, Defaults.HypAutorelease.ToString(), RegistryValueOptions.None), out settings.HypAutorelease))
                settings.HypAutorelease = Defaults.HypAutorelease;
            if (!Enum.TryParse((string)rk.GetValue(Names.pasteParsingAlgorithm, Defaults.PasteParsingAlgorithm.ToString(), RegistryValueOptions.None), out settings.CurrentPasteParsingAlgorithm))
                settings.CurrentPasteParsingAlgorithm = Defaults.PasteParsingAlgorithm;
            if (!Enum.TryParse((string)rk.GetValue(Names.angleUnits, Defaults.AngleUnits.ToString(), RegistryValueOptions.None), out settings.CurrentAngleUnits))
                settings.CurrentAngleUnits = Defaults.AngleUnits;
            if (!Enum.TryParse((string)rk.GetValue(Names.displayFormat, Defaults.DisplayFormat.ToString(), RegistryValueOptions.None), out settings.CurrentDisplayFormat))
                settings.CurrentDisplayFormat = Defaults.DisplayFormat;

            settings.CustomButtonCommand = CleanUpCustomCommand(rk.GetValue(Names.customButtonCommand, Defaults.CustomButtonCommand, RegistryValueOptions.None) as string ?? Defaults.CustomButtonCommand);

            return settings;
        }

        public static void SaveDisplayFormat(DisplayFormat displayFormat)
        {
            using RegistryKey rk = Registry.CurrentUser.CreateSubKey(Names.registryPath);
            if (rk == null) return;
            rk.SetValue(Names.displayFormat, displayFormat.ToString());
        }

        public static void SaveAngleUnits(AngleUnits angleUnits)
        {
            using RegistryKey rk = Registry.CurrentUser.CreateSubKey(Names.registryPath);
            if (rk == null) return;
            rk.SetValue(Names.angleUnits, angleUnits.ToString());
        }

        public void Save()
        {
            using RegistryKey rk = Registry.CurrentUser.CreateSubKey(Names.registryPath);
            if (rk == null) return;

            rk.SetValue(Names.mode, CurrentMode.ToString());
            rk.SetValue(Names.stackMode, CurrentStackMode.ToString());
            rk.SetValue(Names.enterMode, CurrentEnterMode.ToString());
            rk.SetValue(Names.round, Round.ToString());
            rk.SetValue(Names.roundLength, RoundLength, RegistryValueKind.DWord);
            rk.SetValue(Names.truncateZeros, TruncateZeros.ToString());
            rk.SetValue(Names.arcAutorelease, ArcAutorelease.ToString());
            rk.SetValue(Names.hypAutorelease, HypAutorelease.ToString());
            rk.SetValue(Names.pasteParsingAlgorithm, CurrentPasteParsingAlgorithm.ToString());
            rk.SetValue(Names.customButtonCommand, CustomButtonCommand);
            rk.SetValue(Names.angleUnits, CurrentAngleUnits.ToString());
            rk.SetValue(Names.displayFormat, CurrentDisplayFormat.ToString());
        }
    }
}
