using System;
using System.Windows.Controls;

namespace Calcoo
{
    internal class LabelDisplay : BaseDisplay
    {
        public LabelDisplay(int xPos,
            int yPos,
            int xSize,
            int ySize,
            String icon,
            Canvas parent)
        {
            ShownGlyphs.Push(new DisplayGlyph(xPos, yPos, xSize, ySize, icon, parent));
            Refresh();
        }
    }
}
