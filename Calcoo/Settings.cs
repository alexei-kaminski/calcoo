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

        public StackMode stackMode;

        public enum Mode
        {
            Rpn,
            Alg
        }

        public Mode mode;


        public enum EnterMode
        {
            Traditional,
            Hp28
        }

        public EnterMode enterMode;

        public enum PasteParsingAlgorithm
        {
            LocaleBased,
            Heuristic
        }

        public PasteParsingAlgorithm pasteParsingAlgorithm;


        public bool round;
        public int roundLength;
        public bool truncateZeros;
        public bool arcAutorelease;
        public bool hypAutorelease;

        public String customButtonCommand;

        public AngleUnits angleUnits;
        public DisplayFormat displayFormat;

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

            public const String CustomButtonCommand = "";
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
            String customButtonCommand,
            AngleUnits angleUnits,
            DisplayFormat displayFormat)
        {
            this.stackMode = stackMode;
            this.mode = mode;
            this.enterMode = enterMode;
            this.round = round;
            this.roundLength = roundLength;
            this.truncateZeros = truncateZeros;
            this.arcAutorelease = arcAutorelease;
            this.hypAutorelease = hypAutorelease;
            this.pasteParsingAlgorithm = pasteParsingAlgorithm;
            this.customButtonCommand = customButtonCommand;
            this.angleUnits = angleUnits;
            this.displayFormat = displayFormat;
        }

        public Settings Clone()
        {
            return new Settings(
                stackMode,
                mode,
                enterMode,
                round,
                roundLength,
                truncateZeros,
                arcAutorelease,
                hypAutorelease,
                pasteParsingAlgorithm,
                customButtonCommand,
                angleUnits,
                displayFormat);
        }

        public Settings(int roundLength)
        {
            stackMode = Defaults.StackMode;
            mode = Defaults.Mode;
            enterMode = Defaults.EnterMode;
            round = Defaults.Round;
            this.roundLength = roundLength;
            truncateZeros = Defaults.TruncateZeros;
            arcAutorelease = Defaults.ArcAutorelease;
            hypAutorelease = Defaults.HypAutorelease;
            pasteParsingAlgorithm = Defaults.PasteParsingAlgorithm;
            customButtonCommand = Defaults.CustomButtonCommand;
            angleUnits = Defaults.AngleUnits;
            displayFormat = Defaults.DisplayFormat;
        }

        private static class Names
        {
            public static String registryPath = "Software\\Calcoo\\";
            public static String stackMode = "StackMode";
            public static String mode = "Mode";
            public static String enterMode = "EnterMode";
            public static String round = "Round";
            public static String roundLength = "RoundLength";
            public static String truncateZeros = "TruncateZeros";
            public static String arcAutorelease = "ArcAutorelease";
            public static String hypAutorelease = "HypAutorelease";
            public static String angleUnits = "AngleUnits";
            public static String displayFormat = "DisplayFormat";
            public static String pasteParsingAlgorithm = "PasteParsingAlgorithm";
            public static String customButtonCommand = "CustomButtonCommand";
        }

        private static String CleanUpCustomCommand(String customCommand)
        {
            String[] commandCandidates = customCommand.Trim().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (!commandCandidates.Any())
                return "";
            var cleanCustomCommand = new StringBuilder();

            foreach (String commandCandidate in commandCandidates)
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

            if (!Enum.TryParse((string)rk.GetValue(Names.mode, Defaults.Mode.ToString(), RegistryValueOptions.None), out settings.mode))
                settings.mode = Defaults.Mode;
            if (!Enum.TryParse((string)rk.GetValue(Names.stackMode, Defaults.StackMode.ToString(), RegistryValueOptions.None), out settings.stackMode))
                settings.stackMode = Defaults.StackMode;
            if (!Enum.TryParse((string)rk.GetValue(Names.enterMode, Defaults.EnterMode.ToString(), RegistryValueOptions.None), out settings.enterMode))
                settings.enterMode = Defaults.EnterMode;
            if (!Boolean.TryParse((string)rk.GetValue(Names.round, Defaults.Round.ToString(), RegistryValueOptions.None), out settings.round))
                settings.round = Defaults.Round;
            try
            {
                if (rk.GetValueKind(Names.roundLength) == RegistryValueKind.DWord)
                    settings.roundLength = (int)rk.GetValue(Names.roundLength, defaultRoundLength, RegistryValueOptions.None);
                else
                    settings.roundLength = defaultRoundLength;
            }
            catch (Exception)
            {
                settings.roundLength = defaultRoundLength;
            }
            if (!Boolean.TryParse((string)rk.GetValue(Names.truncateZeros, Defaults.TruncateZeros.ToString(), RegistryValueOptions.None), out settings.truncateZeros))
                settings.truncateZeros = Defaults.TruncateZeros;
            if (!Boolean.TryParse((string)rk.GetValue(Names.arcAutorelease, Defaults.ArcAutorelease.ToString(), RegistryValueOptions.None), out settings.arcAutorelease))
                settings.arcAutorelease = Defaults.ArcAutorelease;
            if (!Boolean.TryParse((string)rk.GetValue(Names.hypAutorelease, Defaults.HypAutorelease.ToString(), RegistryValueOptions.None), out settings.hypAutorelease))
                settings.hypAutorelease = Defaults.HypAutorelease;
            if (!Enum.TryParse((string)rk.GetValue(Names.pasteParsingAlgorithm, Defaults.PasteParsingAlgorithm.ToString(), RegistryValueOptions.None), out settings.pasteParsingAlgorithm))
                settings.pasteParsingAlgorithm = Defaults.PasteParsingAlgorithm;
            if (!Enum.TryParse((string)rk.GetValue(Names.angleUnits, Defaults.AngleUnits.ToString(), RegistryValueOptions.None), out settings.angleUnits))
                settings.angleUnits = Defaults.AngleUnits;
            if (!Enum.TryParse((string)rk.GetValue(Names.displayFormat, Defaults.DisplayFormat.ToString(), RegistryValueOptions.None), out settings.displayFormat))
                settings.displayFormat = Defaults.DisplayFormat;

            settings.customButtonCommand = CleanUpCustomCommand(rk.GetValue(Names.customButtonCommand, Defaults.CustomButtonCommand, RegistryValueOptions.None) as string ?? Defaults.CustomButtonCommand);

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

            rk.SetValue(Names.mode, mode.ToString());
            rk.SetValue(Names.stackMode, stackMode.ToString());
            rk.SetValue(Names.enterMode, enterMode.ToString());
            rk.SetValue(Names.round, round.ToString());
            rk.SetValue(Names.roundLength, roundLength, RegistryValueKind.DWord);
            rk.SetValue(Names.truncateZeros, truncateZeros.ToString());
            rk.SetValue(Names.arcAutorelease, arcAutorelease.ToString());
            rk.SetValue(Names.hypAutorelease, hypAutorelease.ToString());
            rk.SetValue(Names.pasteParsingAlgorithm, pasteParsingAlgorithm.ToString());
            rk.SetValue(Names.customButtonCommand, customButtonCommand);
            rk.SetValue(Names.angleUnits, angleUnits.ToString());
            rk.SetValue(Names.displayFormat, displayFormat.ToString());
        }
    }
}
