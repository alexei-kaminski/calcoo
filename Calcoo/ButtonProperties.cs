using System.Windows;

namespace Calcoo
{
    public static class ButtonProperties
    {
        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.RegisterAttached(
                "IsHighlighted",
                typeof(bool),
                typeof(ButtonProperties),
                new PropertyMetadata(false));

        public static bool GetIsHighlighted(DependencyObject obj) =>
            (bool)obj.GetValue(IsHighlightedProperty);

        public static void SetIsHighlighted(DependencyObject obj, bool value) =>
            obj.SetValue(IsHighlightedProperty, value);
    }
}
