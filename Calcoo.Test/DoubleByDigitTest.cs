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
            DoubleByDigit dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 }, -1, -1, false);
            DoubleByDigit clonedDbd = dbd.Clone();
            CompareDoubleByDigit(clonedDbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 8, 9 }, -1, -1,
                false, "1");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9 }, 1, 1, false);
            clonedDbd = dbd.Clone();
            CompareDoubleByDigit(clonedDbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 0, 9 }, 1, 1, false,
                "2");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9, 0, 1 }, 1, -1, false);
            clonedDbd = dbd.Clone();
            CompareDoubleByDigit(clonedDbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 0, 1 }, 1, -1, false,
                "3");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9, 0, 0 }, -1, 1, false);
            clonedDbd = dbd.Clone();
            CompareDoubleByDigit(clonedDbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 0, 0 }, -1, 1, false,
                "4");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 }, -1, -1, true);
            clonedDbd = dbd.Clone();
            Assert.That(clonedDbd.IsOverflow(), Is.EqualTo(true), "overflown");
            // the content should not matter if overflown
        }

        [Test]
        public void ClearTest()
        {
            DoubleByDigit dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 }, -1, -1, false);
            dbd.Clear();
            CompareDoubleByDigit(dbd, new int[] { }, new int[] { }, new int[] { }, 1, 1, false, "1");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 }, -1, -1, true);
            dbd.Clear();
            CompareDoubleByDigit(dbd, new int[] { }, new int[] { }, new int[] { }, 1, 1, false, "2");
        }

        [Test]
        public void SetGetTest()
        {
            DoubleByDigit dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 }, -1, -1, false);
            CompareDoubleByDigit(dbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 8, 9 }, -1, -1, false, "1");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9 }, 1, 1, false);
            CompareDoubleByDigit(dbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 0, 9 }, 1, 1, false, "2");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9, 0, 1 }, 1, -1, false);
            CompareDoubleByDigit(dbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 0, 1 }, 1, -1, false, "3");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9, 0, 0 }, -1, 1, false);
            CompareDoubleByDigit(dbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 0, 0 }, -1, 1, false, "4");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 }, -1, -1, true);
            Assert.That(dbd.IsOverflow(), Is.EqualTo(true), "overflown"); // the content should not matter if overflown
        }

        [Test]
        public void ToDoubleTest()
        {
            DoubleByDigit dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 }, -1, -1, false);
            Assert.That(dbd.ToDouble(10), Is.EqualTo(-123.456e-89).Within(Precision * 123.456e-89), "1");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9 }, 1, 1, false);
            Assert.That(dbd.ToDouble(10), Is.EqualTo(123.456e9).Within(Precision * 123.456e9), "2");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9, 0, 1 }, 1, -1, false);
            Assert.That(dbd.ToDouble(10), Is.EqualTo(123.456e-1).Within(Precision * 123.456e-1), "3");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9, 0, 0 }, -1, 1, false);
            Assert.That(dbd.ToDouble(10), Is.EqualTo(-123.456).Within(Precision * 123.456), "4");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 }, -1, -1, true);
            Assert.That(double.IsNaN(dbd.ToDouble(10)), Is.EqualTo(true), "overflown");
        }

        [Test]
        public void FromDoubleTest()
        {
            // numbers causing overflow

            // not numbers
            DoubleByDigit dbd = DoubleByDigit.FromDouble(double.NaN, MantissaLength, ExpLength, false, 1, MantissaLength,
                false, 10);
            Assert.That(dbd.IsOverflow(), Is.EqualTo(true), "overflown"); // the content should not matter if overflown

            dbd = DoubleByDigit.FromDouble(double.PositiveInfinity, MantissaLength, ExpLength, false, 1, MantissaLength,
                false, 10);
            Assert.That(dbd.IsOverflow(), Is.EqualTo(true), "overflown"); // the content should not matter if overflown

            dbd = DoubleByDigit.FromDouble(double.NegativeInfinity, MantissaLength, ExpLength, false, 1, MantissaLength,
                false, 10);
            Assert.That(dbd.IsOverflow(), Is.EqualTo(true), "overflown"); // the content should not matter if overflown

            // too large to be displayed
            double d = 1e101;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.That(dbd.IsOverflow(), Is.EqualTo(true), d.ToString(CultureInfo.CurrentCulture) + "overflown");
            // the content should not matter if overflown

            d = -1e101;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.That(dbd.IsOverflow(), Is.EqualTo(true), d.ToString(CultureInfo.CurrentCulture) + "overflown");
            // the content should not matter if overflown

            d = 1e100;
            dbd = DoubleByDigit.FromDouble(1e100, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.That(dbd.IsOverflow(), Is.EqualTo(true), d.ToString(CultureInfo.CurrentCulture) + "overflown");
            // the content should not matter if overflown

            d = -1e100;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.That(dbd.IsOverflow(), Is.EqualTo(true), d.ToString(CultureInfo.CurrentCulture) + "overflown");
            // the content should not matter if overflown

            d = 9.99999999999999999999e99; // overflow caused by rounding
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.That(dbd.IsOverflow(), Is.EqualTo(true), d.ToString(CultureInfo.CurrentCulture) + "overflown");
            // the content should not matter if overflown

            d = -9.99999999999999999999e99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.That(dbd.IsOverflow(), Is.EqualTo(true), d.ToString(CultureInfo.CurrentCulture) + "overflown");
            // the content should not matter if overflown

            // numbers displayed in fixed format

            // so small it is zero
            d = 1e-100;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 0 }, new int[] { }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -1e-100;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 0 }, new int[] { }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = 0.4e-99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 0 }, new int[] { }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -0.4e-99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 0 }, new int[] { }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number - greater than 1, allows fixed without truncation
            d = 123.456;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -123.456;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new int[] { }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number - greater than 1, allows fixed, requires truncation up
            d = 123.4567891834;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6, 7, 8, 9, 2 }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -123.4567891834;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6, 7, 8, 9, 2 }, new int[] { }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number - greater than 1, allows fixed, requires truncation
            // down
            d = 123.4567891234;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6, 7, 8, 9, 1 }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -123.4567891234;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 1, 2, 3 }, new[] { 4, 5, 6, 7, 8, 9, 1 }, new int[] { }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number - less than 1, allows fixed without truncation
            d = 0.00456;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 0 }, new[] { 0, 0, 4, 5, 6 }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -0.00456;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 0 }, new[] { 0, 0, 4, 5, 6 }, new int[] { }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number - less than 1, allows fixed but required truncation up
            d = 0.0000456719;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 0 }, new[] { 0, 0, 0, 0, 4, 5, 6, 7, 2 }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -0.0000456719;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 0 }, new[] { 0, 0, 0, 0, 4, 5, 6, 7, 2 }, new int[] { }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number - less than 1, allows fixed, requires truncation down
            d = 0.0000456713;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 0 }, new[] { 0, 0, 0, 0, 4, 5, 6, 7, 1 }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -0.0000456713;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 0 }, new[] { 0, 0, 0, 0, 4, 5, 6, 7, 1 }, new int[] { }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // rounding changes the entire mantissa
            d = 0.9999999999999;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 1 }, new int[] { }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -0.9999999999999;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 1 }, new int[] { }, new int[] { }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // important tricky case - the number of significant digits is equal to
            // the mantissa length, but adding the zero before the decimal point
            // causes rounding, which changes the entire mantissa
            d = 0.9999999997;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 1 }, new int[] { }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -0.9999999997;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 1 }, new int[] { }, new int[] { }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // numbers requiring exponential format
            // regular number, mantissa shorter than MantissaLength
            d = 2.345e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5 }, new[] { 2, 3 }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            d = -2.345e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5 }, new[] { 2, 3 }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture));

            // regular number, mantissa longer than MantissaLength, round up
            d = 2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 3 }, new[] { 2, 3 }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture));

            d = -2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 3 }, new[] { 2, 3 }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture));

            // regular number, mantissa longer than MantissaLength, round down
            d = 2.3456789123e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 2 }, new[] { 2, 3 }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture));

            d = -2.3456789123e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 2 }, new[] { 2, 3 }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture));

            // very large number, barely fits
            d = 9.999999999e99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 9 }, new[] { 9, 9, 9, 9, 9, 9, 9, 9, 9 }, new[] { 9, 9 }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture));

            d = -9.999999999e99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 9 }, new[] { 9, 9, 9, 9, 9, 9, 9, 9, 9 }, new[] { 9, 9 }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture));

            // very small number, almost zero
            d = 1e-99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 1 }, new int[] { }, new[] { 9, 9 }, 1, -1,
                false, d.ToString(CultureInfo.CurrentCulture));

            d = -1e-99;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 1 }, new int[] { }, new[] { 9, 9 }, -1, -1,
                false, d.ToString(CultureInfo.CurrentCulture));

            // numbers forced into exponential format
            // regular number, mantissa shorter than MantissaLength
            // exp format on its own, just checking that forcing does not spoil
            // anything
            String testSubcase = ", forced into exp";
            d = 2.345e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5 }, new[] { 2, 3 }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.345e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5 }, new[] { 2, 3 }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round up
            // exp format on its own, just checking that forcing does not spoil
            // anything
            d = 2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 3 }, new[] { 2, 3 }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 3 }, new[] { 2, 3 }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round down
            // exp format on its own, just checking that forcing does not spoil
            // anything
            d = 2.3456789123e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 2 }, new[] { 2, 3 }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789123e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 2 }, new[] { 2, 3 }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round up
            // exp format on its own, just checking that forcing does not spoil
            // anything
            d = 2.3456789129e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 3 }, new[] { 2, 3 }, 1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789129e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 3 }, new[] { 2, 3 }, -1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round down
            // exp format on its own, just checking that forcing does not spoil
            // anything
            d = 2.3456789123e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 2 }, new[] { 2, 3 }, 1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789123e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 2 }, new[] { 2, 3 }, -1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // number greater than 1
            d = 23.45;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5 }, new[] { 0, 1 }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -23.45;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5 }, new[] { 0, 1 }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // number less than 1
            d = 0.2345;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5 }, new[] { 0, 1 }, 1, -1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -0.2345;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 1, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5 }, new[] { 0, 1 }, -1, -1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // numbers forced into engineering format
            // regular number, mantissa shorter than MantissaLength
            testSubcase = ", forced into eng";
            d = 2.345e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4 }, new[] { 5 }, new[] { 2, 1 }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.345e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4 }, new[] { 5 }, new[] { 2, 1 }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round up
            d = 2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4 }, new[] { 5, 6, 7, 8, 9, 1, 3 }, new[] { 2, 1 }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4 }, new[] { 5, 6, 7, 8, 9, 1, 3 }, new[] { 2, 1 }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round down
            d = 2.3456789123e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4 }, new[] { 5, 6, 7, 8, 9, 1, 2 }, new[] { 2, 1 }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789123e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4 }, new[] { 5, 6, 7, 8, 9, 1, 2 }, new[] { 2, 1 }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round up
            d = 2.3456789129e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3 }, new[] { 4, 5, 6, 7, 8, 9, 1, 3 }, new[] { 2, 4 }, 1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789129e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3 }, new[] { 4, 5, 6, 7, 8, 9, 1, 3 }, new[] { 2, 4 }, -1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // regular number, mantissa longer than MantissaLength, round down
            d = 2.3456789123e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3 }, new[] { 4, 5, 6, 7, 8, 9, 1, 2 }, new[] { 2, 4 }, 1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789123e-23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3 }, new[] { 4, 5, 6, 7, 8, 9, 1, 2 }, new[] { 2, 4 }, -1, -1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // number greater than 1
            d = 23.45;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3 }, new[] { 4, 5 }, new int[] { }, 1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -23.45;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3 }, new[] { 4, 5 }, new int[] { }, -1, 1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // number less than 1
            d = 0.2345;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4 }, new[] { 5 }, new[] { 0, 3 }, 1, -1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -0.2345;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, true, 3, MantissaLength, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4 }, new[] { 5 }, new[] { 0, 3 }, -1, -1, false,
                d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // truncation
            testSubcase = ", truncated";

            // exp number, round up
            d = 2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 7 }, new[] { 2, 3 }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456789129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 7 }, new[] { 2, 3 }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // exp number, round down
            d = 2.3456189129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6 }, new[] { 2, 3 }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -2.3456189129e23;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2 }, new[] { 3, 4, 5, 6 }, new[] { 2, 3 }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // fix number, round up after decimal point
            d = 234.56789129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4 }, new[] { 5, 7 }, new int[] { }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -234.56789129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4 }, new[] { 5, 7 }, new int[] { }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // fix number, round down after decimal point
            d = 234.56189129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4 }, new[] { 5, 6 }, new int[] { }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -234.56189129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4 }, new[] { 5, 6 }, new int[] { }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // fix number, round up before decimal point
            d = 23456789.129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4, 5, 7, 0, 0, 0 }, new int[] { }, new int[] { }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -23456789.129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4, 5, 7, 0, 0, 0 }, new int[] { }, new int[] { }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // fix number, round down before decimal point
            d = 23456189.129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4, 5, 6, 0, 0, 0 }, new int[] { }, new int[] { }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -23456189.129;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 4, 5, 6, 0, 0, 0 }, new int[] { }, new int[] { }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // fix number, round up after decimal point
            d = 230.0;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 0 }, new int[] { }, new int[] { }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -230.0;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, false, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 0 }, new int[] { }, new int[] { }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            // truncation
            testSubcase = ", truncated, trailing zeros kept";

            // fix number, round up after decimal point
            d = 230.0;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, true, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 0 }, new[] { 0, 0 }, new int[] { }, 1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);

            d = -230.0;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 5, true, 10);
            CompareDoubleByDigit(dbd, new[] { 2, 3, 0 }, new[] { 0, 0 }, new int[] { }, -1, 1,
                false, d.ToString(CultureInfo.CurrentCulture) + testSubcase);
        }

        [Test]
        public void AddExpDigitInvalidLengthTest()
        {
            var dbd = new DoubleByDigit();
            Assert.Throws<Exception>(() => dbd.AddExpDigit(1, 0), "length 0");
            Assert.Throws<Exception>(() => dbd.AddExpDigit(1, -1), "length -1");
        }

        [Test]
        public void ToStringOverflowTest()
        {
            var dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 }, -1, -1, true);
            Assert.That(dbd.ToString(), Is.EqualTo("error"), "overflow ToString");
        }

        [Test]
        public void FromDoubleInvalidParametersTest()
        {
            // expDivisor <= 0
            Assert.Throws<Exception>(() =>
                DoubleByDigit.FromDouble(1.0, MantissaLength, ExpLength, false, 0, MantissaLength, false, 10),
                "expDivisor = 0");
            // expDivisor >= base^expLength
            Assert.Throws<Exception>(() =>
                DoubleByDigit.FromDouble(1.0, MantissaLength, ExpLength, false, 100, MantissaLength, false, 10),
                "expDivisor = 100");
            // mantissaLength <= 0
            Assert.Throws<Exception>(() =>
                DoubleByDigit.FromDouble(1.0, 0, ExpLength, false, 1, 0, false, 10),
                "mantissaLength = 0");
            // mantissaLength < nDigitsToRoundTo
            Assert.Throws<Exception>(() =>
                DoubleByDigit.FromDouble(1.0, 5, ExpLength, false, 1, 6, false, 10),
                "nDigitsToRoundTo > mantissaLength");
        }

        [Test]
        public void FromDoubleRoundingPromotionFracTest()
        {
            // This value triggers rounding promotion in the x<1 fixed-format branch
            // where nSignifDigits < nDigitsToRoundTo (line 388-390)
            double d = 0.000999999995;
            var dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, MantissaLength, false, 10);
            Assert.That(dbd.IsOverflow(), Is.False, d + " not overflow");
            Assert.That(dbd.GetSign(), Is.EqualTo(1), d + " sign");
            // After rounding promotion: 0.0010000000
            Assert.That(dbd.GetIntDigit(0), Is.EqualTo(0), d + " int digit");
            Assert.That(dbd.GetFracDigit(0), Is.EqualTo(0), d + " frac 0");
            Assert.That(dbd.GetFracDigit(1), Is.EqualTo(0), d + " frac 1");
            Assert.That(dbd.GetFracDigit(2), Is.EqualTo(0), d + " frac 2");
            Assert.That(dbd.GetFracDigit(3), Is.EqualTo(1), d + " frac 3 (promoted)");

            // This value triggers the else branch (line 392-393) where
            // nSignifDigits == nDigitsToRoundTo, so abs2X is divided by base
            d = 0.009995;
            dbd = DoubleByDigit.FromDouble(d, MantissaLength, ExpLength, false, 1, 3, false, 10);
            Assert.That(dbd.IsOverflow(), Is.False, d + " not overflow");
            Assert.That(dbd.GetIntDigit(0), Is.EqualTo(0), d + " int digit");
            Assert.That(dbd.GetFracDigit(0), Is.EqualTo(0), d + " frac 0");
            Assert.That(dbd.GetFracDigit(1), Is.EqualTo(1), d + " frac 1 (promoted)");
        }

        [Test]
        public void ToStringTest()
        {
            var dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9 }, 1, 1, false);
            Assert.That(dbd.ToString(), Is.EqualTo("123.456e09"), "1");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9 }, -1, 1, false);
            Assert.That(dbd.ToString(), Is.EqualTo("-123.456e09"), "2");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9 }, 1, -1, false);
            Assert.That(dbd.ToString(), Is.EqualTo("123.456e-09"), "3");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 9 }, -1, -1, false);
            Assert.That(dbd.ToString(), Is.EqualTo("-123.456e-09"), "4");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new int[] { }, new int[] { }, 1, -1, false);
            Assert.That(dbd.ToString(), Is.EqualTo("123"), "5");

            dbd = InitDoubleByDigit(new[] { 1, 2, 3 }, new[] { 4, 5 }, new int[] { }, 1, -1, false);
            Assert.That(dbd.ToString(), Is.EqualTo("123.45"), "6");

            dbd = InitDoubleByDigit(new int[] { }, new int[] { }, new int[] { }, 1, 1, false);
            Assert.That(dbd.ToString(), Is.EqualTo("0"), "7");

            dbd = InitDoubleByDigit(new int[] { }, new int[] { }, new int[] { }, -1, 1, false);
            Assert.That(dbd.ToString(), Is.EqualTo("-0"), "8");
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
            Assert.That(dbd.GetNIntDigits(), Is.EqualTo(intDigits.Length), message + " intDigit length");
            for (int i = 0; i < intDigits.Length; ++i)
                Assert.That(dbd.GetIntDigit(i), Is.EqualTo(intDigits[i]), message + " intDigit " + i);

            Assert.That(dbd.GetNFracDigits(), Is.EqualTo(fracDigits.Length), message + " fracDigit length");
            for (int i = 0; i < fracDigits.Length; ++i)
                Assert.That(dbd.GetFracDigit(i), Is.EqualTo(fracDigits[i]), message + " fracDigit " + i);

            Assert.That(dbd.GetNExpDigits(), Is.EqualTo(expDigits.Length), message + " expDigit length");
            for (int i = 0; i < expDigits.Length; ++i)
                Assert.That(dbd.GetExpDigit(i), Is.EqualTo(expDigits[i]), message + " expDigit " + i);

            Assert.That(dbd.GetSign(), Is.EqualTo(sign), message + " sign");
            Assert.That(dbd.GetExpSign(), Is.EqualTo(expSign), message + " expSign");
            Assert.That(dbd.IsOverflow(), Is.EqualTo(overflow), message + " overflow");
        }
    }
}
