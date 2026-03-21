using System;
using System.Windows.Controls;

namespace Calcoo
{
    internal class LabelDisplay : BaseDisplay
    {
        public LabelDisplay(int xPos,
            int yPos,
            int xSize,
            string icon,
            Canvas parent)
        {
            ShownGlyphs.Push(new DisplayGlyph(xPos, yPos, xSize, icon, parent));
            Refresh();
        }
    }
}
