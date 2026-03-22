using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace Calcoo
{
    /// <summary>
    /// Interaction logic for InfoDialog.xaml
    /// </summary>
    public partial class InfoDialog : Window
    {
        public InfoDialog()
        {
            InitializeComponent();
            App.ApplyDialogTheme(this);
            SourceInitialized += (_, _) => MaxHeight = SystemParameters.WorkArea.Height;
            var version = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion.Split('+')[0] ?? "unknown";
            Title = $"Calcoo {version}";
            LicenseVersionRun.Text = version;
            AboutVersionRun.Text = version;
            var copyright = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyCopyrightAttribute>()
                ?.Copyright ?? "";
            LicenseCopyrightRun.Text = copyright;
            AboutCopyrightText.Text = copyright;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SizeToContent = SizeToContent.Manual;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true })?.Dispose();
            e.Handled = true;
        }
    }
}
