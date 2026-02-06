using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Calcoo.Test
{
    [TestFixture]
    public class DoubleByDigitTest
    {
        private const int ExpLength = 2;
        private const int MantissaLength = 10;
        private const double Precision = 1e-15;

        [Test]
        public void CloneTest()
        {
            DoubleByDigit dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {7, 8, 9}, -1, -1, false);
            DoubleByDigit clonedDbd = dbd.Clone();
            CompareDoubleByDigit(clonedDbd, new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {8, 9}, -1, -1,
                false, "1");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9}, 1, 1, false);
            clonedDbd = dbd.Clone();
            CompareDoubleByDigit(clonedDbd, new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {0, 9}, 1, 1, false,
                "2");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9, 0, 1}, 1, -1, false);
            clonedDbd = dbd.Clone();
            CompareDoubleByDigit(clonedDbd, new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {0, 1}, 1, -1, false,
                "3");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9, 0, 0}, -1, 1, false);
            clonedDbd = dbd.Clone();
            CompareDoubleByDigit(clonedDbd, new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {0, 0}, -1, 1, false,
                "4");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {7, 8, 9}, -1, -1, true);
            clonedDbd = dbd.Clone();
            Assert.AreEqual(true, clonedDbd.IsOverflow(), "overflown");
            /*- the content should not matter if overflown */
        }

        [Test]
        public void ClearTest()
        {
            DoubleByDigit dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {7, 8, 9}, -1, -1, false);
            dbd.Clear();
            CompareDoubleByDigit(dbd, new int[] {}, new int[] {}, new int[] {}, 1, 1, false, "1");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {7, 8, 9}, -1, -1, true);
            dbd.Clear();
            CompareDoubleByDigit(dbd, new int[] {}, new int[] {}, new int[] {}, 1, 1, false, "2");
        }

        [Test]
        public void SetGetTest()
        {
            DoubleByDigit dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {7, 8, 9}, -1, -1, false);
            CompareDoubleByDigit(dbd, new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {8, 9}, -1, -1, false, "1");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9}, 1, 1, false);
            CompareDoubleByDigit(dbd, new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {0, 9}, 1, 1, false, "2");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9, 0, 1}, 1, -1, false);
            CompareDoubleByDigit(dbd, new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {0, 1}, 1, -1, false, "3");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9, 0, 0}, -1, 1, false);
            CompareDoubleByDigit(dbd, new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {0, 0}, -1, 1, false, "4");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {7, 8, 9}, -1, -1, true);
            Assert.AreEqual(true, dbd.IsOverflow(), "overflown"); /*- the content should not matter if overflown */
        }

        [Test]
        public void ToDoubleTest()
        {
            DoubleByDigit dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {7, 8, 9}, -1, -1, false);
            Assert.AreEqual(-123.456e-89, dbd.ToDouble(10), Precision*123.456e-89, "1");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9}, 1, 1, false);
            Assert.AreEqual(123.456e9, dbd.ToDouble(10), Precision*123.456e9, "2");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9, 0, 1}, 1, -1, false);
            Assert.AreEqual(123.456e-1, dbd.ToDouble(10), Precision*123.456e-1, "3");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9, 0, 0}, -1, 1, false);
            Assert.AreEqual(-123.456, dbd.ToDouble(10), Precision*123.456, "4");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {7, 8, 9}, -1, -1, true);
            Assert.AreEqual(true, Double.IsNaN(dbd.ToDouble(10)), "overflown");
        }

        [Test]
        public void FromDoubleTest()
        {
            /*
         * numbers causing overflow
         */

            // not numbers
            DoubleByDigit dbd = DoubleByDigit.FromDouble(Double.NaN, MantissaLength, ExpLength, false, 1, MantissaLength,
                false, 10);
            Assert.AreEqual(true, dbd.IsOverflow(), "overflown"); /*- the content should not matter if overflown */

            dbd = DoubleByDigit.FromDouble(Double.PositiveInfinity, MantissaLength, ExpLength, false, 1, MantissaLength,
                false, 10);
            Assert.AreEqual(true, dbd.IsOverflow(), "overflown"); /*- the content should not matter if overflown */

            dbd = DoubleByDigit.FromDouble(Double.NegativeInfinity, MantissaLength, ExpLength, false, 1, MantissaLength,
                false, 10);
            Assert.AreEqual(true, dbd.IsOverflow(), "overflown"); /*- the content should not matter if overflown */

            // too large to be displayed
            double d = 1e101;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.AreEqual(true, dbd.IsOverflow(), d.ToString(CultureInfo.CurrentCulture) + "overflown");
            /*- the content should not matter if overflown */

            d = -1e101;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.AreEqual(true, dbd.IsOverflow(), d.ToString(CultureInfo.CurrentCulture) + "overflown");
            /*- the content should not matter if overflown */

            d = 1e100;
            dbd = DoubleByDigit.FromDouble(1e100, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.AreEqual(true, dbd.IsOverflow(), d.ToString(CultureInfo.CurrentCulture) + "overflown");
            /*- the content should not matter if overflown */

            d = -1e100;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.AreEqual(true, dbd.IsOverflow(), d.ToString(CultureInfo.CurrentCulture) + "overflown");
            /*- the content should not matter if overflown */

            d = 9.99999999999999999999e99; // overflow caused by rounding
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.AreEqual(true, dbd.IsOverflow(), d.ToString(CultureInfo.CurrentCulture) + "overflown");
            /*- the content should not matter if overflown */

            d = -9.99999999999999999999e99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.AreEqual(true, dbd.IsOverflow(), d.ToString(CultureInfo.CurrentCulture) + "overflown");
            /*- the content should not matter if overflown */

            /*
         * numbers displayed in fixed format
         */

            // so small it is zero
            d = 1e-100;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {0}, new int[] {}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -1e-100;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {0}, new int[] {}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = 0.4e-99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {0}, new int[] {}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -0.4e-99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {0}, new int[] {}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number - greater than 1, allows fixed without truncation
            d = 123.456;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {1, 2, 3}, new[] {4, 5, 6}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -123.456;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {1, 2, 3}, new[] {4, 5, 6}, new int[] {}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number - greater than 1, allows fixed, requires truncation up
            d = 123.4567891834;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {1, 2, 3}, new[] {4, 5, 6, 7, 8, 9, 2}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -123.4567891834;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {1, 2, 3}, new[] {4, 5, 6, 7, 8, 9, 2}, new int[] {}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number - greater than 1, allows fixed, requires truncation
            // down
            d = 123.4567891234;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {1, 2, 3}, new[] {4, 5, 6, 7, 8, 9, 1}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -123.4567891234;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {1, 2, 3}, new[] {4, 5, 6, 7, 8, 9, 1}, new int[] {}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number - less than 1, allows fixed without truncation
            d = 0.00456;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {0}, new[] {0, 0, 4, 5, 6}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -0.00456;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {0}, new[] {0, 0, 4, 5, 6}, new int[] {}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number - less than 1, allows fixed but required truncation up
            d = 0.0000456719;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {0}, new[] {0, 0, 0, 0, 4, 5, 6, 7, 2}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -0.0000456719;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {0}, new[] {0, 0, 0, 0, 4, 5, 6, 7, 2}, new int[] {}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number - less than 1, allows fixed, requires truncation down
            d = 0.0000456713;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {0}, new[] {0, 0, 0, 0, 4, 5, 6, 7, 1}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -0.0000456713;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {0}, new[] {0, 0, 0, 0, 4, 5, 6, 7, 1}, new int[] {}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // rounding changes the entire mantissa
            d = 0.9999999999999;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {1}, new int[] {}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -0.9999999999999;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {1}, new int[] {}, new int[] {}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // important tricky case - the number of significant digits is equal to
            // the mantissa length, but adding the zero before the decimal point
            // causes rounding, which changes the entire mantissa
            d = 0.9999999997;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {1}, new int[] {}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -0.9999999997;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {1}, new int[] {}, new int[] {}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            /*
         * numbers requiring exponential format
         */
            // regular number, mantissa shorter than MantissaLength
            d = 2.345e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5}, new[] {2, 3}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -2.345e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5}, new[] {2, 3}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number, mantissa longer than MantissaLength, round up
            d = 2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6, 7, 8, 9, 1, 3}, new[] {2, 3}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture));

            d = -2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6, 7, 8, 9, 1, 3}, new[] {2, 3}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture));

            // regular number, mantissa longer than MantissaLength, round down
            d = 2.3456789123e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6, 7, 8, 9, 1, 2}, new[] {2, 3}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture));

            d = -2.3456789123e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6, 7, 8, 9, 1, 2}, new[] {2, 3}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture));

            // very large number, barely fits
            d = 9.999999999e99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {9}, new[] {9, 9, 9, 9, 9, 9, 9, 9, 9}, new[] {9, 9}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture));

            d = -9.999999999e99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {9}, new[] {9, 9, 9, 9, 9, 9, 9, 9, 9}, new[] {9, 9}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture));

            // very small number, almost zero
            d = 1e-99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {1}, new int[] {}, new[] {9, 9}, 1, -1,
                false, d.ToString(CultureInfo.CurrentCulture));

            d = -1e-99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {1}, new int[] {}, new[] {9, 9}, -1, -1,
                false, d.ToString(CultureInfo.CurrentCulture));

            /*
         * numbers forced into exponential format
         */
            // regular number, mantissa shorter than MantissaLength
            // exp format on its own, just checking that forcing does not spoil
            // anything
            String testSubcase = ", forced into exp";
            d = 2.345e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5}, new[] {2, 3}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.345e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5}, new[] {2, 3}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round up
            // exp format on its own, just checking that forcing does not spoil
            // anything
            d = 2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6, 7, 8, 9, 1, 3}, new[] {2, 3}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6, 7, 8, 9, 1, 3}, new[] {2, 3}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round down
            // exp format on its own, just checking that forcing does not spoil
            // anything
            d = 2.3456789123e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6, 7, 8, 9, 1, 2}, new[] {2, 3}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789123e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6, 7, 8, 9, 1, 2}, new[] {2, 3}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round up
            // exp format on its own, just checking that forcing does not spoil
            // anything
            d = 2.3456789129e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6, 7, 8, 9, 1, 3}, new[] {2, 3}, 1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789129e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6, 7, 8, 9, 1, 3}, new[] {2, 3}, -1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round down
            // exp format on its own, just checking that forcing does not spoil
            // anything
            d = 2.3456789123e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6, 7, 8, 9, 1, 2}, new[] {2, 3}, 1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789123e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6, 7, 8, 9, 1, 2}, new[] {2, 3}, -1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // number greater than 1
            d = 23.45;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5}, new[] {0, 1}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -23.45;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5}, new[] {0, 1}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // number less than 1
            d = 0.2345;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5}, new[] {0, 1}, 1, -1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -0.2345;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5}, new[] {0, 1}, -1, -1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            /*
         * numbers forced into engineering format
         */
            // regular number, mantissa shorter than MantissaLength
            testSubcase = ", forced into eng";
            d = 2.345e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4}, new[] {5}, new[] {2, 1}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.345e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4}, new[] {5}, new[] {2, 1}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round up
            d = 2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4}, new[] {5, 6, 7, 8, 9, 1, 3}, new[] {2, 1}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4}, new[] {5, 6, 7, 8, 9, 1, 3}, new[] {2, 1}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round down
            d = 2.3456789123e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4}, new[] {5, 6, 7, 8, 9, 1, 2}, new[] {2, 1}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789123e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4}, new[] {5, 6, 7, 8, 9, 1, 2}, new[] {2, 1}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round up
            d = 2.3456789129e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3}, new[] {4, 5, 6, 7, 8, 9, 1, 3}, new[] {2, 4}, 1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789129e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3}, new[] {4, 5, 6, 7, 8, 9, 1, 3}, new[] {2, 4}, -1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round down
            d = 2.3456789123e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3}, new[] {4, 5, 6, 7, 8, 9, 1, 2}, new[] {2, 4}, 1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789123e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3}, new[] {4, 5, 6, 7, 8, 9, 1, 2}, new[] {2, 4}, -1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // number greater than 1
            d = 23.45;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3}, new[] {4, 5}, new int[] {}, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -23.45;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3}, new[] {4, 5}, new int[] {}, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // number less than 1
            d = 0.2345;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4}, new[] {5}, new[] {0, 3}, 1, -1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -0.2345;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4}, new[] {5}, new[] {0, 3}, -1, -1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            /*
         * truncation
         */
            testSubcase = ", truncated";

            // exp number, round up
            d = 2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 7}, new[] {2, 3}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 7}, new[] {2, 3}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // exp number, round down
            d = 2.3456189129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6}, new[] {2, 3}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456189129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2}, new[] {3, 4, 5, 6}, new[] {2, 3}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // fix number, round up after decimal point
            d = 234.56789129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4}, new[] {5, 7}, new int[] {}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -234.56789129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4}, new[] {5, 7}, new int[] {}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // fix number, round down after decimal point
            d = 234.56189129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4}, new[] {5, 6}, new int[] {}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -234.56189129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4}, new[] {5, 6}, new int[] {}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // fix number, round up before decimal point
            d = 23456789.129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4, 5, 7, 0, 0, 0}, new int[] {}, new int[] {}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -23456789.129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4, 5, 7, 0, 0, 0}, new int[] {}, new int[] {}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // fix number, round down before decimal point
            d = 23456189.129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4, 5, 6, 0, 0, 0}, new int[] {}, new int[] {}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -23456189.129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 4, 5, 6, 0, 0, 0}, new int[] {}, new int[] {}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // fix number, round up after decimal point
            d = 230.0;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 0}, new int[] {}, new int[] {}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -230.0;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 0}, new int[] {}, new int[] {}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            /*
         * truncation
         */
            testSubcase = ", truncated, trailing zeros kept";

            // fix number, round up after decimal point
            d = 230.0;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, true, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 0}, new[] {0, 0}, new int[] {}, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -230.0;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, true, 10);
            CompareDoubleByDigit(dbd, new[] {2, 3, 0}, new[] {0, 0}, new int[] {}, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);
        }

        [Test]
        public void ToStringTest()
        {
            var dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9}, 1, 1, false);
            Assert.AreEqual("123.456e09", dbd.ToString(), "1");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9}, -1, 1, false);
            Assert.AreEqual("-123.456e09", dbd.ToString(), "2");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9}, 1, -1, false);
            Assert.AreEqual("123.456e-09", dbd.ToString(), "3");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {9}, -1, -1, false);
            Assert.AreEqual("-123.456e-09", dbd.ToString(), "4");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new int[] {}, new int[] {}, 1, -1, false);
            Assert.AreEqual("123", dbd.ToString(), "5");

            dbd = InitDoubleByDigit(new[] {1, 2, 3}, new[] {4, 5}, new int[] {}, 1, -1, false);
            Assert.AreEqual("123.45", dbd.ToString(), "6");

            dbd = InitDoubleByDigit(new int[] {}, new int[] {}, new int[] {}, 1, 1, false);
            Assert.AreEqual("0", dbd.ToString(), "7");

            dbd = InitDoubleByDigit(new int[] {}, new int[] {}, new int[] {}, -1, 1, false);
            Assert.AreEqual("-0", dbd.ToString(), "8");
        }

        static private DoubleByDigit InitDoubleByDigit(IEnumerable<int> intDigits,
            IEnumerable<int> fracDigits,
            IEnumerable<int> expDigits,
            int sign,
            int expSign,
            bool overflow)
        {
            var dbd = new DoubleByDigit();
            foreach (int d in intDigits)
                dbd.AddIntDigit(d);
            foreach (int d in fracDigits)
                dbd.AddFracDigit(d);
            foreach (int d in expDigits)
                dbd.AddExpDigit(d, ExpLength);
            dbd.SetSign(sign);
            dbd.SetExpSign(expSign);
            dbd.SetOverflow(overflow);
            return dbd;
        }

        static private void CompareDoubleByDigit(DoubleByDigit dbd,
            int[] intDigits,
            int[] fracDigits,
            int[] expDigits,
            int sign,
            int expSign,
            bool overflow,
            String message)
        {
            Assert.AreEqual(intDigits.Length, dbd.GetNIntDigits(), message + " intDigit length");
            for (int i = 0; i < intDigits.Length; ++i)
                Assert.AreEqual(intDigits[i], dbd.GetIntDigit(i), message + " intDigit " + i);

            Assert.AreEqual(fracDigits.Length, dbd.GetNFracDigits(), message + " fracDigit length");
            for (int i = 0; i < fracDigits.Length; ++i)
                Assert.AreEqual(fracDigits[i], dbd.GetFracDigit(i), message + " fracDigit " + i);

            Assert.AreEqual(expDigits.Length, dbd.GetNExpDigits(), message + " expDigit length");
            for (int i = 0; i < expDigits.Length; ++i)
                Assert.AreEqual(expDigits[i], dbd.GetExpDigit(i), message + " expDigit " + i);

            Assert.AreEqual(sign, dbd.GetSign(), message + " sign");
            Assert.AreEqual(expSign, dbd.GetExpSign(), message + " expSign");
            Assert.AreEqual(overflow, dbd.IsOverflow(), message + " overflow");
        }
    }
}
