using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Calcoo
{
    internal class OperationDisplay : BaseDisplay
    {
        private readonly Dictionary<Cpu.BinaryOp, DisplayGlyph> _ops;
        private readonly DisplayGlyph _paren;

        public OperationDisplay(int xMargin,
            int yMargin,
            int xSize,
            int ySize,
            string iconSet,
            Canvas parent)
        {
            _ops = new Dictionary<Cpu.BinaryOp, DisplayGlyph>();

            foreach (Cpu.BinaryOp op in Enum.GetValues(typeof(Cpu.BinaryOp)))
                _ops.Add(op, new DisplayGlyph(xMargin, yMargin, xSize, ySize, iconSet + op, parent));

            _paren = new DisplayGlyph(xMargin, yMargin, xSize, ySize, iconSet + "Paren", parent);
        }

        public void Show(Cpu.BinaryOp? op,
            bool showParen)
        {
            Clear();

            if (op != null)
            {
                if (showParen)
                    ShownGlyphs.Push(_paren);
                ShownGlyphs.Push(_ops[op.Value]);
            }

            Refresh();
        }
    }
}
