using System;
using System.Linq;
using System.Text;

using Microsoft.Win32;

namespace Calcoo
{
    public class Settings
    {
        public enum StackModeType
        {
            Xyzt,
            Infinite
        }

        public StackModeType StackMode;

        public enum ModeType
        {
            Rpn,
            Alg
        }

        public ModeType Mode;


        public enum EnterModeType
        {
            Traditional,
            Hp28
        }

        public EnterModeType EnterMode;

        public enum PasteParsingAlgorithmType
        {
            LocaleBased,
            Heuristic
        }

        public PasteParsingAlgorithmType PasteParsingAlgorithm;


        public bool Round;
        public int RoundLength;
        public bool TruncateZeros;
        public bool ArcAutorelease;
        public bool HypAutorelease;

        public string CustomButtonCommand;

        public AngleUnitsType AngleUnits;
        public DisplayFormatType DisplayFormat;

        private static class Defaults
        {
            public const StackModeType StackMode = Settings.StackModeType.Infinite;
            public const ModeType Mode = Settings.ModeType.Alg;
            public const EnterModeType EnterMode = Settings.EnterModeType.Traditional;
            public const bool Round = false;
            public const bool TruncateZeros = false;
            public const bool ArcAutorelease = true;
            public const bool HypAutorelease = true;
            public const AngleUnitsType AngleUnits = Settings.AngleUnitsType.Deg;
            public const DisplayFormatType DisplayFormat = Settings.DisplayFormatType.Fix;

            public const PasteParsingAlgorithmType PasteParsingAlgorithm =
                Settings.PasteParsingAlgorithmType.LocaleBased;

            public const string CustomButtonCommand = "";
        }

        // set by the buttons
        public enum AngleUnitsType
        {
            Deg,
            Rad
        }

        public enum DisplayFormatType
        {
            Fix,
            Sci,
            Eng
        }

        public static int ExpDivisor(DisplayFormatType displayFormat)
        {
            return (displayFormat == DisplayFormatType.Eng ? 3 : 1);
        }

        public Settings(StackModeType stackMode,
            ModeType mode,
            EnterModeType enterMode,
            bool round,
            int roundLength,
            bool truncateZeros,
            bool arcAutorelease,
            bool hypAutorelease,
            PasteParsingAlgorithmType pasteParsingAlgorithm,
            string customButtonCommand,
            AngleUnitsType angleUnits,
            DisplayFormatType displayFormat)
        {
            this.StackMode = stackMode;
            this.Mode = mode;
            this.EnterMode = enterMode;
            this.Round = round;
            this.RoundLength = roundLength;
            this.TruncateZeros = truncateZeros;
            this.ArcAutorelease = arcAutorelease;
            this.HypAutorelease = hypAutorelease;
            this.PasteParsingAlgorithm = pasteParsingAlgorithm;
            this.CustomButtonCommand = customButtonCommand;
            this.AngleUnits = angleUnits;
            this.DisplayFormat = displayFormat;
        }

        public Settings Clone()
        {
            return new Settings(
                StackMode,
                Mode,
                EnterMode,
                Round,
                RoundLength,
                TruncateZeros,
                ArcAutorelease,
                HypAutorelease,
                PasteParsingAlgorithm,
                CustomButtonCommand,
                AngleUnits,
                DisplayFormat);
        }

        public Settings(int roundLength)
        {
            StackMode = Defaults.StackMode;
            Mode = Defaults.Mode;
            EnterMode = Defaults.EnterMode;
            Round = Defaults.Round;
            RoundLength = roundLength;
            TruncateZeros = Defaults.TruncateZeros;
            ArcAutorelease = Defaults.ArcAutorelease;
            HypAutorelease = Defaults.HypAutorelease;
            PasteParsingAlgorithm = Defaults.PasteParsingAlgorithm;
            CustomButtonCommand = Defaults.CustomButtonCommand;
            AngleUnits = Defaults.AngleUnits;
            DisplayFormat = Defaults.DisplayFormat;
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

            if (!Enum.TryParse((string)rk.GetValue(Names.mode, Defaults.Mode.ToString(), RegistryValueOptions.None), out settings.Mode))
                settings.Mode = Defaults.Mode;
            if (!Enum.TryParse((string)rk.GetValue(Names.stackMode, Defaults.StackMode.ToString(), RegistryValueOptions.None), out settings.StackMode))
                settings.StackMode = Defaults.StackMode;
            if (!Enum.TryParse((string)rk.GetValue(Names.enterMode, Defaults.EnterMode.ToString(), RegistryValueOptions.None), out settings.EnterMode))
                settings.EnterMode = Defaults.EnterMode;
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
            if (!Enum.TryParse((string)rk.GetValue(Names.pasteParsingAlgorithm, Defaults.PasteParsingAlgorithm.ToString(), RegistryValueOptions.None), out settings.PasteParsingAlgorithm))
                settings.PasteParsingAlgorithm = Defaults.PasteParsingAlgorithm;
            if (!Enum.TryParse((string)rk.GetValue(Names.angleUnits, Defaults.AngleUnits.ToString(), RegistryValueOptions.None), out settings.AngleUnits))
                settings.AngleUnits = Defaults.AngleUnits;
            if (!Enum.TryParse((string)rk.GetValue(Names.displayFormat, Defaults.DisplayFormat.ToString(), RegistryValueOptions.None), out settings.DisplayFormat))
                settings.DisplayFormat = Defaults.DisplayFormat;

            settings.CustomButtonCommand = CleanUpCustomCommand(rk.GetValue(Names.customButtonCommand, Defaults.CustomButtonCommand, RegistryValueOptions.None) as string ?? Defaults.CustomButtonCommand);

            return settings;
        }

        public static void SaveDisplayFormat(DisplayFormatType displayFormat)
        {
            using RegistryKey rk = Registry.CurrentUser.CreateSubKey(Names.registryPath);
            if (rk == null) return;
            rk.SetValue(Names.displayFormat, displayFormat.ToString());
        }

        public static void SaveAngleUnits(AngleUnitsType angleUnits)
        {
            using RegistryKey rk = Registry.CurrentUser.CreateSubKey(Names.registryPath);
            if (rk == null) return;
            rk.SetValue(Names.angleUnits, angleUnits.ToString());
        }

        public void Save()
        {
            using RegistryKey rk = Registry.CurrentUser.CreateSubKey(Names.registryPath);
            if (rk == null) return;

            rk.SetValue(Names.mode, Mode.ToString());
            rk.SetValue(Names.stackMode, StackMode.ToString());
            rk.SetValue(Names.enterMode, EnterMode.ToString());
            rk.SetValue(Names.round, Round.ToString());
            rk.SetValue(Names.roundLength, RoundLength, RegistryValueKind.DWord);
            rk.SetValue(Names.truncateZeros, TruncateZeros.ToString());
            rk.SetValue(Names.arcAutorelease, ArcAutorelease.ToString());
            rk.SetValue(Names.hypAutorelease, HypAutorelease.ToString());
            rk.SetValue(Names.pasteParsingAlgorithm, PasteParsingAlgorithm.ToString());
            rk.SetValue(Names.customButtonCommand, CustomButtonCommand);
            rk.SetValue(Names.angleUnits, AngleUnits.ToString());
            rk.SetValue(Names.displayFormat, DisplayFormat.ToString());
        }
    }
}
