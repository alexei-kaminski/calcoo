using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Calcoo
{
    internal class BaseDisplay
    {
        protected readonly Stack<DisplayGlyph> ShownGlyphs;

        protected BaseDisplay()
        {
            ShownGlyphs = new Stack<DisplayGlyph>();
        }

        protected void Clear()
        {
            foreach (var glyph in ShownGlyphs)
                glyph.Hide();
            ShownGlyphs.Clear();
        }

        protected void Refresh()
        {
            foreach (var glyph in ShownGlyphs)
                glyph.Show();
        }

        protected class DisplayGlyph
        {
            private readonly int _x, _y, _xSize, _ySize;
            private readonly string _icon;
            private readonly Canvas _parent;
            private ContentControl _box;

            public DisplayGlyph(int x,
                int y,
                int xSize,
                int ySize,
                string icon,
                Canvas parent)
            {
                _x = x;
                _y = y;
                _xSize = xSize;
                _ySize = ySize;
                _icon = icon;
                _parent = parent;
            }

            public void Show()
            {
                if (_box == null)
                {
                    _box = new ContentControl { };
                    _box.Width = _xSize;
                    //_box.Height = _ySize;
                    var uri = new Uri("Resources" + _icon + ".xaml", UriKind.Relative);
                    _box.Content = Application.LoadComponent(uri);
                    Canvas.SetLeft(_box, _x);
                    Canvas.SetTop(_box, _y);
                    _parent.Children.Add(_box);
                }
                _box.Visibility = Visibility.Visible;
            }

            public void Hide()
            {
                if (_box != null)
                    _box.Visibility = Visibility.Hidden;
            }
        }
    }
}
