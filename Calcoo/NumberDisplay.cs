using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Calcoo
{
    internal class NumberDisplay : BaseDisplay
    {
        private readonly DisplayGlyph[] _minusSign;
        private readonly DisplayGlyph[,] _intDigits;
        private readonly DisplayGlyph[,] _fracDigits;
        private readonly DisplayGlyph[] _intTicks;
        private readonly DisplayGlyph[] _fracTicks;
        private readonly DisplayGlyph[] _dot;
        private readonly DisplayGlyph _e;
        private readonly DisplayGlyph _expPlusSign, _expMinusSign, _error;
        private readonly DisplayGlyph[,] _expDigits;

        private readonly int _inputLength, _expInputLength;
        private readonly bool _hasTicks, _hasError;

        private const int TickFrequency = 3;

        public NumberDisplay(int cellWidth, int dotOffsetX, int dotOffsetY, int dotWidth, int xMargin, int yMargin,
            int errorOffsetX, int tickOffsetX, int tickOffsetY, int tickWidth, int inputLength, int expInputLength,
            bool hasTicks, bool hasError, string iconSet, int numBase, Canvas parent)
        {
            _inputLength = inputLength;
            _expInputLength = expInputLength;
            _hasTicks = hasTicks;
            _hasError = hasError;

            int cellHeight = 2;

            _minusSign = new DisplayGlyph[inputLength];
            _intDigits = new DisplayGlyph[inputLength, numBase];
            _fracDigits = new DisplayGlyph[inputLength, numBase];
            _intTicks = new DisplayGlyph[inputLength];
            _fracTicks = new DisplayGlyph[inputLength];
            _dot = new DisplayGlyph[inputLength];
            _expDigits = new DisplayGlyph[expInputLength, numBase];

            int displayExpOffsetX = cellWidth*(inputLength + 1) + dotWidth;

            /* Things to show */
            for (int n = 0; n < numBase; ++n)
            {
                String thisDigitIcon = iconSet + "Digit" + n;

                for (int i = 0; i < inputLength; ++i)
                {
                    _intDigits[i, n] = new DisplayGlyph(xMargin + cellWidth*(i + 1), yMargin, cellWidth, cellHeight,
                        thisDigitIcon, parent);
                    _fracDigits[i, n] = new DisplayGlyph(xMargin + dotWidth + cellWidth*(i + 1), yMargin, cellWidth,
                        cellHeight, thisDigitIcon, parent);
                }

                for (int i = 0; i < expInputLength; ++i)
                    _expDigits[i, n] = new DisplayGlyph(xMargin + displayExpOffsetX + cellWidth*(i + 2),
                        yMargin, cellWidth, cellHeight, thisDigitIcon, parent);
            }

            String dotIcon = iconSet + "Dot";
            String minusIcon = iconSet + "Minus";

            for (int i = 0; i < inputLength; i++)
            {
                _dot[i] = new DisplayGlyph(xMargin + cellWidth*(i + 2) + dotOffsetX, yMargin + dotOffsetY, dotWidth,
                    dotWidth, dotIcon, parent);
                _minusSign[i] = new DisplayGlyph(xMargin + cellWidth*i, yMargin, cellWidth, cellHeight, minusIcon,
                    parent);
            }

            if (hasTicks)
            {
                String tickIcon = iconSet + "Tick";
                for (int i = 0; i < inputLength; ++i)
                {
                    _intTicks[i] = new DisplayGlyph(xMargin + cellWidth*i + tickOffsetX,
                        yMargin + tickOffsetY, tickWidth, tickWidth, tickIcon, parent);
                    _fracTicks[i] = new DisplayGlyph(xMargin + cellWidth*i + dotWidth + tickOffsetX,
                        yMargin + tickOffsetY, tickWidth, tickWidth, tickIcon, parent);
                }
            }

            _expMinusSign = new DisplayGlyph(xMargin + displayExpOffsetX + cellWidth, yMargin, cellWidth, cellHeight,
                minusIcon, parent);
            _expPlusSign = new DisplayGlyph(xMargin + displayExpOffsetX + cellWidth, yMargin, cellWidth, cellHeight,
                iconSet + "Plus", parent);
            _e = new DisplayGlyph(xMargin + displayExpOffsetX, yMargin, cellWidth, cellHeight,
                iconSet + "E", parent);
            if (hasError)
                _error = new DisplayGlyph(xMargin + errorOffsetX, yMargin, cellWidth, cellHeight, iconSet + "Error",
                    parent);
            else
                _error = null;
        }

        public void Show(IDoubleByDigitGetters content)
        {
            Clear();

            if (content.IsOverflow())
            {
                if (_hasError)
                    ShownGlyphs.Push(_error);
                Refresh();
                return;
            }

            int nMantissaDigits = content.GetNIntDigits() + content.GetNFracDigits();
            int startPos = _inputLength - nMantissaDigits;

            if (nMantissaDigits > _inputLength)
                throw new Exception("More digits in the mantissa, " + nMantissaDigits
                                    + ", than can be shown," + _inputLength);

            if (content.GetSign() < 0)
                ShownGlyphs.Push(_minusSign[startPos]);

            for (int i = 0; i < content.GetNIntDigits(); ++i)
                ShownGlyphs.Push(_intDigits[startPos + i, content.GetIntDigit(i)]);

            ShownGlyphs.Push(_dot[startPos + content.GetNIntDigits() - 1]);

            for (int i = 0; i < content.GetNFracDigits(); ++i)
                ShownGlyphs.Push(_fracDigits[startPos + content.GetNIntDigits() + i, content.GetFracDigit(i)]);

            if (content.GetNExpDigits() > 0)
            {
                if (content.GetNExpDigits() > _expInputLength)
                    throw new Exception("More digits in the exponent, " + content.GetNExpDigits()
                                        + ", than can be shown," + _expInputLength);
                ShownGlyphs.Push(_e);
                if (content.GetExpSign() > 0)
                    ShownGlyphs.Push(_expPlusSign);
                else
                    ShownGlyphs.Push(_expMinusSign);
                for (int i = 0; i < content.GetNExpDigits(); ++i)
                    ShownGlyphs.Push(_expDigits[i, content.GetExpDigit(i)]);
            }

            if (_hasTicks)
            {
                for (int i = content.GetNIntDigits() - TickFrequency; i > 0; i -= TickFrequency)
                    ShownGlyphs.Push(_intTicks[startPos + i]);

                for (int i = TickFrequency; i < content.GetNFracDigits(); i += TickFrequency)
                    ShownGlyphs.Push(_fracTicks[startPos + content.GetNIntDigits() + i]);
            }
            Refresh();
        }
    }
}
