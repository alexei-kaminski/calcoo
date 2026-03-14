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

        private static SolidColorBrush Frozen(SolidColorBrush brush)
        {
            brush.Freeze();
            return brush;
        }

        public static void ApplyTheme(bool isDark)
        {
            IsDarkMode = isDark;
            var res = Current.Resources;

            if (isDark)
            {
                res["AppBackground"] = Frozen(new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x2D)));
                res["DisplayBackground"] = Frozen(new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E)));
                res["BorderBrush"] = Frozen(new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66)));
                res["IconForeground"] = Frozen(new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0)));
                res["ButtonHoverBg"] = Frozen(new SolidColorBrush(Color.FromRgb(0x40, 0x40, 0x40)));
                res["ButtonPressedBg"] = Frozen(new SolidColorBrush(Color.FromRgb(0x50, 0x50, 0x50)));
                res["TextForeground"] = Frozen(new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0)));
                res["ButtonCheckedBg"] = Frozen(new SolidColorBrush(Color.FromRgb(0x48, 0x48, 0x48)));
                res["CardBackground"] = Frozen(new SolidColorBrush(Color.FromArgb(0xB3, 0x2D, 0x2D, 0x2D)));
                res["CardBorderBrush"] = Frozen(new SolidColorBrush(Color.FromArgb(0x1A, 0xFF, 0xFF, 0xFF)));
                res["ButtonBorderBrush"] = Frozen(new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF)));
                res["ScrollBarThumbBrush"] = Frozen(new SolidColorBrush(Color.FromRgb(0x6B, 0x6B, 0x6B)));
                res["ScrollBarTrackBrush"] = Frozen(new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x2D)));
                res["SubtleForeground"] = Frozen(new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99)));
                res["AccentButtonBackground"] = Frozen(new SolidColorBrush(Color.FromRgb(0x60, 0xCD, 0xFF)));
                res["AccentButtonForeground"] = Frozen(new SolidColorBrush(Colors.Black));
            }
            else
            {
                res["AppBackground"] = Frozen(new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0)));
                res["DisplayBackground"] = Frozen(new SolidColorBrush(Colors.White));
                res["BorderBrush"] = Frozen(new SolidColorBrush(Colors.Black));
                res["IconForeground"] = Frozen(new SolidColorBrush(Colors.Black));
                res["TextForeground"] = Frozen(new SolidColorBrush(Colors.Black));
                res["ButtonHoverBg"] = Frozen(new SolidColorBrush(Color.FromRgb(0xDA, 0xDA, 0xDA)));
                res["ButtonPressedBg"] = Frozen(new SolidColorBrush(Color.FromRgb(0xBA, 0xBA, 0xBA)));
                res["ButtonCheckedBg"] = Frozen(new SolidColorBrush(Color.FromRgb(0xCA, 0xCA, 0xCA)));
                res["CardBackground"] = Frozen(new SolidColorBrush(Color.FromArgb(0xB3, 0xFF, 0xFF, 0xFF)));
                res["CardBorderBrush"] = Frozen(new SolidColorBrush(Color.FromArgb(0x0F, 0x00, 0x00, 0x00)));
                res["ButtonBorderBrush"] = Frozen(new SolidColorBrush(Color.FromArgb(0x40, 0x00, 0x00, 0x00)));
                res["ScrollBarThumbBrush"] = Frozen(new SolidColorBrush(Color.FromRgb(0xCD, 0xCD, 0xCD)));
                res["ScrollBarTrackBrush"] = Frozen(new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0)));
                res["SubtleForeground"] = Frozen(new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66)));
                res["AccentButtonBackground"] = Frozen(new SolidColorBrush(Color.FromRgb(0x00, 0x67, 0xC0)));
                res["AccentButtonForeground"] = Frozen(new SolidColorBrush(Colors.White));
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

        public static void ApplyDialogTheme(Window window)
        {
            window.SourceInitialized += (_, _) =>
            {
                ApplyDarkTitleBar(window);
                ApplyMica(window);
            };
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
