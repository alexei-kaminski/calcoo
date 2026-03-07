using System.Windows;
using System.Windows.Controls;

namespace Calcoo
{
    public static class ThemedMessageBox
    {
        public static void Show(Window owner, string message, string title)
        {
            var dialog = new Window
            {
                Title = title,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = owner,
                Background = (System.Windows.Media.Brush)Application.Current.Resources["AppBackground"],
                Foreground = (System.Windows.Media.Brush)Application.Current.Resources["TextForeground"],
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI Variable, Segoe UI"),
                FontSize = 14
            };

            dialog.SourceInitialized += (_, _) =>
            {
                App.ApplyDarkTitleBar(dialog);
                App.ApplyMica(dialog);
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var textBlock = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(24, 20, 24, 20),
                MaxWidth = 340
            };
            Grid.SetRow(textBlock, 0);
            grid.Children.Add(textBlock);

            var buttonBorder = new Border
            {
                BorderBrush = (System.Windows.Media.Brush)Application.Current.Resources["CardBorderBrush"],
                BorderThickness = new Thickness(0, 1, 0, 0),
                Padding = new Thickness(16, 12, 16, 12)
            };
            var button = new Button
            {
                Content = "OK",
                HorizontalAlignment = HorizontalAlignment.Right,
                IsDefault = true,
                Background = (System.Windows.Media.Brush)Application.Current.Resources["AccentButtonBackground"],
                Foreground = (System.Windows.Media.Brush)Application.Current.Resources["AccentButtonForeground"],
                BorderThickness = new Thickness(0),
                Template = CreateAccentButtonTemplate()
            };
            button.Click += (_, _) => dialog.Close();
            buttonBorder.Child = button;
            Grid.SetRow(buttonBorder, 1);
            grid.Children.Add(buttonBorder);

            dialog.Content = grid;
            dialog.ShowDialog();
        }

        private static System.Windows.Controls.ControlTemplate CreateAccentButtonTemplate()
        {
            var template = new System.Windows.Controls.ControlTemplate(typeof(Button));
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new System.Windows.Data.Binding("Background")
                { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
            border.SetValue(Border.PaddingProperty, new Thickness(24, 6, 24, 6));
            border.SetValue(Border.SnapsToDevicePixelsProperty, true);

            var presenter = new FrameworkElementFactory(typeof(ContentPresenter));
            presenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            presenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            border.AppendChild(presenter);

            template.VisualTree = border;
            return template;
        }
    }
}
