using System;
using System.Collections.Generic;
using System.Text;


namespace Calcoo
{
    public interface IDoubleByDigitGetters
    {
        bool IsOverflow();
        int GetNIntDigits();
        int GetIntDigit(int i);
        int GetNFracDigits();
        int GetFracDigit(int i);
        int GetNExpDigits();
        int GetExpDigit(int i);
        int GetSign();
        int GetExpSign();
        double ToDouble(int numBase);
        string ToString();
    }

    public class DoubleByDigit : IDoubleByDigitGetters
    {
        private List<int> _intField;
        private List<int> _fracField;
        private List<int> _expField;
        private int _sign;
        private int _expSign;
        private bool _overflow;


        public DoubleByDigit()
        {
            _intField = new List<int>();
            _fracField = new List<int>();
            _expField = new List<int>();
            _sign = 1;
            _expSign = 1;
            _overflow = false;
        }

        public DoubleByDigit Clone()
        {
            var clonedSpelledDouble = new DoubleByDigit();
            clonedSpelledDouble._intField = new List<int>(_intField);
            clonedSpelledDouble._fracField = new List<int>(_fracField);
            clonedSpelledDouble._expField = new List<int>(_expField);
            clonedSpelledDouble._sign = _sign;
            clonedSpelledDouble._expSign = _expSign;
            clonedSpelledDouble._overflow = _overflow;
            return clonedSpelledDouble;
        }

        public void Clear()
        {
            _intField.Clear();
            _fracField.Clear();
            _expField.Clear();
            _sign = 1;
            _expSign = 1;
            _overflow = false;
        }

        public void AddIntDigit(int d)
        {
            _intField.Add(d);
        }

        public void AddFracDigit(int d)
        {
            _fracField.Add(d);
        }

        public void AddExpDigit(int d,
            int length)
        {
            if (length < 1)
                throw new Exception("Exp field length equals " + length + " while it must be positive");
            if (_expField.Count >= length)
                do
                {
                    _expField.RemoveAt(0);
                } while (_expField.Count >= length);
            else
                while (_expField.Count < length - 1)
                    _expField.Insert(0, 0);
            _expField.Add(d);
        }

        public void SetSign(int sign)
        {
            _sign = sign >= 0 ? 1 : -1;
        }

        public void SetExpSign(int expSign)
        {
            _expSign = expSign >= 0 ? 1 : -1;
        }

        public void InverseSign()
        {
            _sign = -_sign;
        }

        public void InverseExpSign()
        {
            _expSign = -_expSign;
        }

        public void SetOverflow(bool overflow)
        {
            _overflow = overflow;
        }

        public bool IsOverflow()
        {
            return _overflow;
        }

        public int GetNIntDigits()
        {
            return _intField.Count;
        }

        public int GetIntDigit(int i)
        {
            return _intField[i];
        }

        public int GetNFracDigits()
        {
            return _fracField.Count;
        }

        public int GetFracDigit(int i)
        {
            return _fracField[i];
        }

        public int GetNExpDigits()
        {
            return _expField.Count;
        }

        public int GetExpDigit(int i)
        {
            return _expField[i];
        }

        public int GetSign()
        {
            return _sign;
        }

        public int GetExpSign()
        {
            return _expSign;
        }

        public double ToDouble(int numBase)
        {
            if (_overflow)
                return double.NaN;
            // empty input returns zero by design
            double dBase = numBase;
            double x = 0.0;

            // int part
            for (int i = 0; i < _intField.Count; ++i)
                x = x * dBase + _intField[i];

            // frac part
            double placer = 1.0;
            for (int i = 0; i < _fracField.Count; ++i)
            {
                placer /= dBase;
                x += _fracField[i] * placer;
            }

            // sign
            if (_sign < 0)
                x = -x;

            // exp part
            int exponent = 0;
            for (int i = 0; i < _expField.Count; ++i)
                exponent = exponent * numBase + _expField[i];
            if (_expSign < 0)
                exponent = -exponent;
            x *= Math.Pow(dBase, exponent);

            return x;
        }

        public override string ToString()
        {
            if (_overflow)
                return "error";

            var stringBuffer = new StringBuilder();

            if (_sign < 0)
                stringBuffer.Append('-');

            if (_intField.Count == 0)
                stringBuffer.Append('0');

            foreach (int d in _intField)
                stringBuffer.Append(d);

            if (_fracField.Count > 0)
            {
                stringBuffer.Append('.');
                foreach (int d in _fracField)
                    stringBuffer.Append(d);
            }

            if (_expField.Count > 0)
            {
                stringBuffer.Append('e');
                if (_expSign < 0)
                    stringBuffer.Append('-');
                foreach (int d in _expField)
                    stringBuffer.Append(d);
            }

            return stringBuffer.ToString();
        }

        public static DoubleByDigit FromDouble(double x,
            int mantissaLength,
            int expLength,
            bool forceExp,
            int expDivisor,
            int nDigitsToRoundTo,
            bool forceRoundedMantissaLength,
            int numBase)
        {
            double dBase = numBase;
            var dbd = new DoubleByDigit();

            // sanity checks
            if (expDivisor <= 0 || expDivisor >= Math.Pow(dBase, expLength))
                throw new Exception("ExpDivisor = " + expDivisor + " but it must in interval [1, "
                                    + (Math.Pow(dBase, expLength) - 1) + "]");

            if (mantissaLength <= 0 || mantissaLength < nDigitsToRoundTo)
                throw new Exception("nDigitsToRoundTo = " + nDigitsToRoundTo
                                    + " but it must be greater than 0 and not greater than mantissa length "
                                    + mantissaLength);

            // Let's rock!

            // First, we consider some special cases of too large or too small x

            if (double.IsNaN(x) || double.IsInfinity(x))
            {
                dbd._overflow = true;
                return dbd;
            }

            // x is a valid number, continue

            double absX = Math.Abs(x);

            if (absX >= Math.Pow(dBase, Math.Pow(dBase, expLength)))
            {
                // x is so large that it cannot be displayed
                dbd._overflow = true;
                return dbd;
            }

            // x is small enough, continue

            if (absX < Math.Pow(dBase, -(Math.Pow(dBase, expLength) - 1)))
            {
                // x is effectively zero
                dbd._intField.Add(0);
                return dbd;
            }

            // x is not zero, continue

            /*
             * Now, the special cases have been considered, and we can proceed with
             * a regular case. This is not trivial, however. The main problem arises
             * when x=0.9999999999999...., so rounding of x for displaying may lead
             * to changes in the exp part, see the "!!!" comment
             */

            // sign - note the the sigh of zero is "+", like in all calculators
            dbd._sign = x >= 0.0 ? 1 : -1;

            int exponent = (int)Math.Floor(Math.Log(absX) / Math.Log(dBase));

            /* truncating all but first "nDigitsToRoundTo" digits of x */
            /*
             * for numbers <1 that can be displayed without exponent this
             * interpretation will be redone later, because in this case additional
             * rounding may be needed
             */

            long abs1X = (long)Math.Round((absX / Math.Pow(dBase, exponent + 1))
                                           * Math.Pow(dBase, nDigitsToRoundTo));

            if (abs1X >= Math.Pow(dBase, nDigitsToRoundTo))
            {
                /*
                 * !!! rounding has promoted x to the next order - this is what we
                 * mentioned before
                 */
                abs1X = (long)Math.Round(abs1X / dBase);
                exponent += 1;
            }

            if ((exponent == -1)
                && !forceExp
                && (Math.Round(abs1X / dBase) >= Math.Pow(dBase, nDigitsToRoundTo - 1)))
            {
                /*
                 * a very special case of, say, x=0.999999997, when the introduction
                 * of the zero before the dot changes the integer part from 0 to 1
                 * thus leading to switching between the types of format
                 * distinguished below
                 */
                dbd._intField.Add(1);
                return dbd;
            }

            /* padding with zeros to the full mantissaLength */
            abs1X *= (long)Math.Pow(dBase, mantissaLength - nDigitsToRoundTo);

            int[] digitsOfX = new int[mantissaLength];
            /* interpreting meaningful digits of x */
            for (int i = mantissaLength - 1; i >= 0; --i)
            {
                int lastDigit = (int)(abs1X % numBase);
                digitsOfX[i] = lastDigit;
                abs1X = (abs1X - lastDigit) / numBase;
            }

            /*
             * By this point, we have determined all the digits of x to display and
             * the exponent. Now we are going to figure out what digits to put
             * before and after the dot
             */

            if ((0 <= exponent) && (exponent < mantissaLength) && !forceExp)
            {
                /*
                 * x can be displayed without the exponent and x>=1, so there is NO
                 * need to introduce heading zeros
                 */
                int intLength = exponent + 1;

                for (int i = 0; i < intLength; ++i)
                    dbd._intField.Add(digitsOfX[i]);

                int fracLength = mantissaLength - intLength;
                for (int i = 0; i < fracLength; ++i)
                    dbd._fracField.Add(digitsOfX[intLength + i]);
            }
            else if ((-mantissaLength < exponent) && (exponent < 0)
                     && !forceExp)
            {
                /*
                 * x can be displayed without the exponent and x<1, so there IS a
                 * need to introduce heading zeros
                 */
                dbd._intField.Add(0);

                int nHeadingZeros = -exponent - 1;
                for (int i = 0; i < nHeadingZeros; ++i)
                    dbd._fracField.Add(0);

                int nSignifDigits = mantissaLength - 1 - nHeadingZeros;
                if (nSignifDigits > nDigitsToRoundTo)
                    nSignifDigits = nDigitsToRoundTo;

                /* separating first "signif_digit_num" digits of x */
                long abs2X = (long)Math.Round((absX / Math.Pow(dBase, exponent + 1))
                                               * Math.Pow(dBase, nSignifDigits));

                if (abs2X >= Math.Pow(dBase, nSignifDigits))
                {
                    /* rounding promoted x to the next order */
                    if (nSignifDigits < nDigitsToRoundTo)
                    {
                        nSignifDigits++;
                    }
                    else
                        abs2X = (long)Math.Round(abs2X / dBase);
                }

                for (int i = nSignifDigits - 1; i >= 0; i--)
                {
                    int lastDigit = (int)(abs2X % numBase);
                    digitsOfX[i] = lastDigit;
                    abs2X = (long)Math.Round((double)(abs2X - lastDigit) / numBase);
                }

                /* interpreting meaningful digits of x */
                for (int i = 0; i < nSignifDigits; i++)
                {
                    dbd._fracField.Add(digitsOfX[i]);
                }
            }
            else
            {
                /* exponent is needed to display x */

                if (expDivisor == 1)
                {
                    dbd._intField.Add(digitsOfX[0]);
                    for (int i = 0; i < mantissaLength - 1; i++)
                        dbd._fracField.Add(digitsOfX[i + 1]);
                }
                else
                {
                    /*
                     * engineering format: the exponent must be a multiple of
                     * ExpDivisor
                     */
                    int exponentAdj = Math.Abs(exponent) % expDivisor;
                    if (exponent < 0 && exponentAdj != 0)
                        exponentAdj = expDivisor - exponentAdj;
                    for (int i = 0; i < 1 + exponentAdj; ++i)
                        dbd._intField.Add(digitsOfX[i]);

                    int fracLength = mantissaLength - 1 - exponentAdj;
                    for (int i = 0; i < fracLength; ++i)
                        dbd._fracField.Add(digitsOfX[i + 1 + exponentAdj]);

                    exponent -= exponentAdj;
                }

                /* processing exp part */
                int tmp = Math.Abs(exponent);
                if (tmp != 0)
                {
                    /*
                     * when exponential format is enforced, the exp part may be
                     * equal to zero; there is no need to display it then
                     */
                    var digitsOfExpReversed = new List<int>();

                    while (tmp > 0)
                    {
                        digitsOfExpReversed.Add(tmp % numBase);
                        tmp = tmp / numBase;
                    }

                    for (int i = 0; i < expLength - digitsOfExpReversed.Count; ++i)
                        dbd._expField.Add(0);

                    for (int i = digitsOfExpReversed.Count - 1; i >= 0; --i)
                        dbd._expField.Add(digitsOfExpReversed[i]);

                    if (exponent > 0)
                        dbd._expSign = 1;
                    else
                        dbd._expSign = -1;
                }
            }

            /* getting rid of trailing zeros in frac */
            while ((dbd._fracField.Count > 0) // if nothing more to trim, stop
                   && (dbd._fracField[dbd._fracField.Count - 1] == 0) /*- if non-zero, stop */
                   && !(forceRoundedMantissaLength
                        && (dbd._intField.Count + dbd._fracField.Count <= nDigitsToRoundTo)))
                dbd._fracField.RemoveAt(dbd._fracField.Count - 1);

            return dbd;
        }
    }
}
