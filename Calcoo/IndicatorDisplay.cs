using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Calcoo
{
    internal class IndicatorDisplay<T> : BaseDisplay
    {
        private readonly Dictionary<T, DisplayGlyph> _icons;

        public IndicatorDisplay(int xPos,
            int yPos,
            int xSize,
            int ySize,
            IEnumerable<T> values,
            String iconSet,
            Canvas parent)
        {
            _icons = new Dictionary<T, DisplayGlyph>();
            foreach (var value in values)
                _icons.Add(value, new DisplayGlyph(xPos, yPos, xSize, ySize, iconSet + value, parent));
        }

        public void Show(T value)
        {
            Clear();
            if (_icons.TryGetValue(value, out var icon))
                ShownGlyphs.Push(icon);
            else
                throw new Exception("Request to show unknown value " + value);
            Refresh();
        }
    }
}
