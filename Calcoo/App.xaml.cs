using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Win32;

namespace Calcoo
{
    public partial class App : Application
    {
        public static bool IsDarkMode { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ApplyTheme(DetectDarkMode());
        }

        public static bool DetectDarkMode()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                if (key?.GetValue("AppsUseLightTheme") is int value)
                    return value == 0;
            }
            catch { }
            return false;
        }

        public static void ApplyTheme(bool isDark)
        {
            IsDarkMode = isDark;
            var res = Current.Resources;

            if (isDark)
            {
                res["AppBackground"] = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x2D));
                res["DisplayBackground"] = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E));
                res["BorderBrush"] = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
                res["IconForeground"] = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0));
                res["IconStroke"] = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0));
                res["ButtonHoverBg"] = new SolidColorBrush(Color.FromRgb(0x40, 0x40, 0x40));
                res["ButtonPressedBg"] = new SolidColorBrush(Color.FromRgb(0x50, 0x50, 0x50));
                res["TextForeground"] = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0));
                res["ButtonCheckedBg"] = new SolidColorBrush(Color.FromRgb(0x48, 0x48, 0x48));
                res["CardBackground"] = new SolidColorBrush(Color.FromArgb(0xB3, 0x2D, 0x2D, 0x2D));
                res["CardBorderBrush"] = new SolidColorBrush(Color.FromArgb(0x1A, 0xFF, 0xFF, 0xFF));
                res["ScrollBarThumbBrush"] = new SolidColorBrush(Color.FromRgb(0x6B, 0x6B, 0x6B));
                res["ScrollBarTrackBrush"] = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x2D));
                res["SubtleForeground"] = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                res["AccentButtonBackground"] = new SolidColorBrush(Color.FromRgb(0x60, 0xCD, 0xFF));
                res["AccentButtonForeground"] = new SolidColorBrush(Colors.Black);
            }
            else
            {
                res["AppBackground"] = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0));
                res["DisplayBackground"] = new SolidColorBrush(Colors.White);
                res["BorderBrush"] = new SolidColorBrush(Colors.Black);
                res["IconForeground"] = new SolidColorBrush(Colors.Black);
                res["IconStroke"] = new SolidColorBrush(Color.FromRgb(0x1F, 0x1A, 0x17));
                res["TextForeground"] = new SolidColorBrush(Colors.Black);
                res["ButtonHoverBg"] = new SolidColorBrush(Color.FromRgb(0xDA, 0xDA, 0xDA));
                res["ButtonPressedBg"] = new SolidColorBrush(Color.FromRgb(0xBA, 0xBA, 0xBA));
                res["ButtonCheckedBg"] = new SolidColorBrush(Color.FromRgb(0xCA, 0xCA, 0xCA));
                res["CardBackground"] = new SolidColorBrush(Color.FromArgb(0xB3, 0xFF, 0xFF, 0xFF));
                res["CardBorderBrush"] = new SolidColorBrush(Color.FromArgb(0x0F, 0x00, 0x00, 0x00));
                res["ScrollBarThumbBrush"] = new SolidColorBrush(Color.FromRgb(0xCD, 0xCD, 0xCD));
                res["ScrollBarTrackBrush"] = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0));
                res["SubtleForeground"] = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
                res["AccentButtonBackground"] = new SolidColorBrush(Color.FromRgb(0x00, 0x67, 0xC0));
                res["AccentButtonForeground"] = new SolidColorBrush(Colors.White);
            }

            foreach (Window window in Current.Windows)
                ApplyDarkTitleBar(window);
        }
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS
        {
            public int Left, Right, Top, Bottom;
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        public static void ApplyDarkTitleBar(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero) return;
            int value = IsDarkMode ? 1 : 0;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
        }

        public static bool ApplyMica(Window window)
        {
            try
            {
                var hwnd = new WindowInteropHelper(window).Handle;
                if (hwnd == IntPtr.Zero) return false;

                int backdropType = 2; // DWMSBT_MAINWINDOW (Mica)
                int hr = DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref backdropType, sizeof(int));
                return hr >= 0;
            }
            catch { }
            return false;
        }
    }
}
