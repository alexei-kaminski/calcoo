using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Calcoo
{
    public class TextUtil
    {
        private const char Unassigned = 'u';


        private static double TextToDoubleWithGuidance(String text,
            char decimalChar,
            char groupingChar)
        {
            String simplifiedText = text;
            if (groupingChar != Unassigned)
                simplifiedText = simplifiedText.Replace(Char.ToString(groupingChar), "");
            if (decimalChar != Unassigned)
                simplifiedText = simplifiedText.Replace(Char.ToString(decimalChar),
                    CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            return TextToDouble(simplifiedText, true);
        }

        public static double TextToDouble(String text,
            bool useNumberFormatParser)
        {
            if (useNumberFormatParser)
            {
                try
                {
                    return Double.Parse(text);
                }
                catch (Exception)
                {
                    return Double.NaN;
                }
            }
            else
            {
                // heuristic parser - it tries to figure out which character
                // between the two (',' and '.') is the decimal character, and
                // which is the grouping character
                String simplifiedText = text.Trim();

                var allowedChars =
                    new HashSet<char>(new[]
                    {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '-', '.', ',', 'e', 'E'});
                char[] textAsChars = simplifiedText.ToCharArray();

                if (!allowedChars.Contains(textAsChars[0]))
                    return Double.NaN;

                int parseableLength = textAsChars.Length;
                for (int i = 0; i < textAsChars.Length; ++i)
                {
                    if (!allowedChars.Contains(textAsChars[i]))
                    {
                        parseableLength = i;
                        break;
                    }
                }

                // Determine which character is the decimal char, and which is
                // the grouping char. First we figure out the positions of ','
                // and '.'
                var symPositions = new Dictionary<char, Queue<int>>();
                symPositions.Add('.', new Queue<int>());
                symPositions.Add(',', new Queue<int>());

                int ePosition = parseableLength;
                for (int i = 0; i < parseableLength; ++i)
                {
                    switch (textAsChars[i])
                    {
                        case '.':
                        case ',':
                            symPositions[textAsChars[i]].Enqueue(i);
                            break;
                        case 'e':
                        case 'E':
                            ePosition = i;
                            goto stringLoopExit;
                    }
                }
                stringLoopExit:

                // now we try to deduce the roles of ',' and '.' from their
                // positions
                char decimalChar;
                char groupingChar = Unassigned;
                // simple cases
                if (symPositions['.'].Count == 0 && symPositions[','].Count == 0)
                    return TextToDouble(simplifiedText, true);
                // no commas and dots - the parsing is trivial and can be delegated

                if (symPositions['.'].Count + symPositions[','].Count == 1)
                {
                    decimalChar = (symPositions['.'].Count == 1 ? '.' : ',');
                    return TextToDoubleWithGuidance(simplifiedText, decimalChar, groupingChar);
                }

                if (symPositions[','].Count > 1 && symPositions['.'].Count > 1)
                    return Double.NaN; // Parsing failed because encountered both '.' and ',' multiple times.
                // This can be improved by trying to make parsing possible by
                // discarding some ending portion of the string.

                if ((symPositions[','].Count > 1 && symPositions['.'].Count <= 1)
                    || (symPositions[','].Count <= 1 && symPositions['.'].Count > 1))
                {
                    // there is a clear candidate for the decimal char, but we must now verify if the other chars, also, present, are placed properly
                    if (symPositions['.'].Count <= 1)
                    {
                        decimalChar = '.';
                        groupingChar = ',';
                    }
                    else
                    {
                        decimalChar = ',';
                        groupingChar = '.';
                    }

                    int decimalCharPosition = (symPositions[decimalChar].Any()
                        ? symPositions[decimalChar].First()
                        : ePosition);

                    int firstSeparatorCharPosition = Math.Min(decimalCharPosition,
                        symPositions[groupingChar].First());
                    if (firstSeparatorCharPosition > 4
                        || (firstSeparatorCharPosition == 4 && textAsChars[0] != '-' && textAsChars[0] != '+'))
                        return Double.NaN;
                    // the number starts with more than three digits in a row even though grouping chars are present

                    // checking the grouping chars before the decimal char
                    int prevSeparatorCharPosition;
                    if (firstSeparatorCharPosition < decimalCharPosition)
                    {
                        prevSeparatorCharPosition = symPositions[groupingChar].Dequeue();
                        while (symPositions[groupingChar].Any() &&
                               symPositions[groupingChar].Peek() < decimalCharPosition)
                        {
                            if (symPositions[groupingChar].Peek() != prevSeparatorCharPosition + 4)
                                return Double.NaN;
                            // separator chars are separated by the number of digits other than 4
                            prevSeparatorCharPosition = symPositions[groupingChar].Dequeue();
                        }
                        if (decimalCharPosition != prevSeparatorCharPosition + 4)
                            return Double.NaN;
                        // separator char and decimal char are separated by the number of digits other than 4
                    }

                    // checking the grouping chars after the decimal char
                    prevSeparatorCharPosition = decimalCharPosition;
                    while (symPositions[groupingChar].Any())
                    {
                        if (symPositions[groupingChar].Peek() != prevSeparatorCharPosition + 4)
                            return Double.NaN; // separator chars are separated by the number of digits other than 4
                        prevSeparatorCharPosition = symPositions[groupingChar].Dequeue();
                    }
                    return TextToDoubleWithGuidance(simplifiedText, decimalChar, groupingChar);
                }
                else if (symPositions['.'].Count == 1 && symPositions[','].Count == 1)
                {
                    // the most difficult case, one of each potential separator
                    if (Math.Abs(symPositions['.'].Peek() - symPositions[','].Peek()) != 4)
                        return Double.NaN;
                    int firstPosition = Math.Min(symPositions['.'].Peek(), symPositions[','].Peek());
                    int secondPosition = Math.Max(symPositions['.'].Peek(), symPositions[','].Peek());
                    if (firstPosition == 0
                        ||
                        (firstPosition == 1 &&
                         (textAsChars[0] == '0' || textAsChars[0] == '-' || textAsChars[0] == '+'))
                        ||
                        (firstPosition == 2 && textAsChars[1] == '0' &&
                         (textAsChars[0] == '-' || textAsChars[0] == '+')))
                        return TextToDoubleWithGuidance(simplifiedText, textAsChars[firstPosition],
                            textAsChars[secondPosition]);

                    if (secondPosition == parseableLength - 3)
                        // in case the string represents a currency amount which is likely to have exactly two digits after the decimal point
                        return TextToDoubleWithGuidance(simplifiedText, textAsChars[secondPosition],
                            textAsChars[firstPosition]);

                    return Double.NaN;
                }
                return Double.NaN; // should not get here
            }
        }
    }
}

