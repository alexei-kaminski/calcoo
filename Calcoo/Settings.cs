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
        public bool StayOnTop;

        public string CustomButtonCommand;
        public string CustomButtonTooltip;

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
            public const bool StayOnTop = false;
            public const AngleUnits AngleUnits = Settings.AngleUnits.Deg;
            public const DisplayFormat DisplayFormat = Settings.DisplayFormat.Fix;

            public const PasteParsingAlgorithm PasteParsingAlgorithm =
                Settings.PasteParsingAlgorithm.LocaleBased;

            public const string CustomButtonCommand = "";
            public const string CustomButtonTooltip = "Custom command";
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
            string customButtonTooltip,
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
            this.CustomButtonTooltip = customButtonTooltip;
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
                CustomButtonTooltip,
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
            CustomButtonTooltip = Defaults.CustomButtonTooltip;
            CurrentAngleUnits = Defaults.AngleUnits;
            CurrentDisplayFormat = Defaults.DisplayFormat;
        }

        private static class Names
        {
            public static string RegistryPath = "Software\\Calcoo";
            public static string StackMode = "StackMode";
            public static string Mode = "Mode";
            public static string EnterMode = "EnterMode";
            public static string Round = "Round";
            public static string RoundLength = "RoundLength";
            public static string TruncateZeros = "TruncateZeros";
            public static string ArcAutorelease = "ArcAutorelease";
            public static string HypAutorelease = "HypAutorelease";
            public static string AngleUnits = "AngleUnits";
            public static string DisplayFormat = "DisplayFormat";
            public static string PasteParsingAlgorithm = "PasteParsingAlgorithm";
            public static string CustomButtonCommand = "CustomButtonCommand";
            public static string CustomButtonTooltip = "CustomButtonTooltip";
            public static string StayOnTop = "StayOnTop";
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
            using var rk = Registry.CurrentUser.OpenSubKey(Names.RegistryPath);
            if (rk == null)
                return settings;

            if (!Enum.TryParse((string)rk.GetValue(Names.Mode, Defaults.Mode.ToString(), RegistryValueOptions.None), out settings.CurrentMode))
                settings.CurrentMode = Defaults.Mode;
            if (!Enum.TryParse((string)rk.GetValue(Names.StackMode, Defaults.StackMode.ToString(), RegistryValueOptions.None), out settings.CurrentStackMode))
                settings.CurrentStackMode = Defaults.StackMode;
            if (!Enum.TryParse((string)rk.GetValue(Names.EnterMode, Defaults.EnterMode.ToString(), RegistryValueOptions.None), out settings.CurrentEnterMode))
                settings.CurrentEnterMode = Defaults.EnterMode;
            if (!bool.TryParse((string)rk.GetValue(Names.Round, Defaults.Round.ToString(), RegistryValueOptions.None), out settings.Round))
                settings.Round = Defaults.Round;
            try
            {
                if (rk.GetValueKind(Names.RoundLength) == RegistryValueKind.DWord)
                    settings.RoundLength = (int)rk.GetValue(Names.RoundLength, defaultRoundLength, RegistryValueOptions.None);
                else
                    settings.RoundLength = defaultRoundLength;
            }
            catch (Exception)
            {
                settings.RoundLength = defaultRoundLength;
            }
            if (!bool.TryParse((string)rk.GetValue(Names.TruncateZeros, Defaults.TruncateZeros.ToString(), RegistryValueOptions.None), out settings.TruncateZeros))
                settings.TruncateZeros = Defaults.TruncateZeros;
            if (!bool.TryParse((string)rk.GetValue(Names.ArcAutorelease, Defaults.ArcAutorelease.ToString(), RegistryValueOptions.None), out settings.ArcAutorelease))
                settings.ArcAutorelease = Defaults.ArcAutorelease;
            if (!bool.TryParse((string)rk.GetValue(Names.HypAutorelease, Defaults.HypAutorelease.ToString(), RegistryValueOptions.None), out settings.HypAutorelease))
                settings.HypAutorelease = Defaults.HypAutorelease;
            if (!Enum.TryParse((string)rk.GetValue(Names.PasteParsingAlgorithm, Defaults.PasteParsingAlgorithm.ToString(), RegistryValueOptions.None), out settings.CurrentPasteParsingAlgorithm))
                settings.CurrentPasteParsingAlgorithm = Defaults.PasteParsingAlgorithm;
            if (!Enum.TryParse((string)rk.GetValue(Names.AngleUnits, Defaults.AngleUnits.ToString(), RegistryValueOptions.None), out settings.CurrentAngleUnits))
                settings.CurrentAngleUnits = Defaults.AngleUnits;
            if (!Enum.TryParse((string)rk.GetValue(Names.DisplayFormat, Defaults.DisplayFormat.ToString(), RegistryValueOptions.None), out settings.CurrentDisplayFormat))
                settings.CurrentDisplayFormat = Defaults.DisplayFormat;

            settings.CustomButtonCommand = CleanUpCustomCommand(rk.GetValue(Names.CustomButtonCommand, Defaults.CustomButtonCommand, RegistryValueOptions.None) as string ?? Defaults.CustomButtonCommand);
            settings.CustomButtonTooltip = rk.GetValue(Names.CustomButtonTooltip, Defaults.CustomButtonTooltip, RegistryValueOptions.None) as string ?? Defaults.CustomButtonTooltip;
            if (!bool.TryParse((string)rk.GetValue(Names.StayOnTop, Defaults.StayOnTop.ToString(), RegistryValueOptions.None), out settings.StayOnTop))
                settings.StayOnTop = Defaults.StayOnTop;

            return settings;
        }

        public static void SaveStayOnTop(bool stayOnTop)
        {
            using RegistryKey rk = Registry.CurrentUser.CreateSubKey(Names.RegistryPath);
            if (rk == null) return;
            rk.SetValue(Names.StayOnTop, stayOnTop.ToString());
        }

        public static void SaveDisplayFormat(DisplayFormat displayFormat)
        {
            using RegistryKey rk = Registry.CurrentUser.CreateSubKey(Names.RegistryPath);
            if (rk == null) return;
            rk.SetValue(Names.DisplayFormat, displayFormat.ToString());
        }

        public static void SaveAngleUnits(AngleUnits angleUnits)
        {
            using RegistryKey rk = Registry.CurrentUser.CreateSubKey(Names.RegistryPath);
            if (rk == null) return;
            rk.SetValue(Names.AngleUnits, angleUnits.ToString());
        }

        public void Save()
        {
            using RegistryKey rk = Registry.CurrentUser.CreateSubKey(Names.RegistryPath);
            if (rk == null) return;

            rk.SetValue(Names.Mode, CurrentMode.ToString());
            rk.SetValue(Names.StackMode, CurrentStackMode.ToString());
            rk.SetValue(Names.EnterMode, CurrentEnterMode.ToString());
            rk.SetValue(Names.Round, Round.ToString());
            rk.SetValue(Names.RoundLength, RoundLength, RegistryValueKind.DWord);
            rk.SetValue(Names.TruncateZeros, TruncateZeros.ToString());
            rk.SetValue(Names.ArcAutorelease, ArcAutorelease.ToString());
            rk.SetValue(Names.HypAutorelease, HypAutorelease.ToString());
            rk.SetValue(Names.PasteParsingAlgorithm, CurrentPasteParsingAlgorithm.ToString());
            rk.SetValue(Names.CustomButtonCommand, CustomButtonCommand);
            rk.SetValue(Names.CustomButtonTooltip, CustomButtonTooltip);
            rk.SetValue(Names.AngleUnits, CurrentAngleUnits.ToString());
            rk.SetValue(Names.DisplayFormat, CurrentDisplayFormat.ToString());
            rk.SetValue(Names.StayOnTop, StayOnTop.ToString());
        }
    }
}
