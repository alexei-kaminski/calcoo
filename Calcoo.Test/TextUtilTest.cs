using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Calcoo.Test
{
    [TestFixture]
    internal class TextUtilTest
    {
        private class StringNumberPair
        {
            public readonly String S;
            public readonly double N;

            public StringNumberPair(double n,
                String s)
            {
                S = s;
                N = n;
            }
        }

        private const double Precision = 1e-12;

        [Test]
        public void TextToDoubleTest()
        {
            // native parser
            double[] testCasesD = { 1200, 1200.1, -1200.1 };

            foreach (var d in testCasesD)
            {
                var s = d.ToString(CultureInfo.CurrentCulture);
                Assert.That(TextUtil.TextToDouble(s, true), Is.EqualTo(d).Within(Precision * Math.Abs(d)), s + ", NFParser");
            }

            var testCases = new List<StringNumberPair>();

            // custom local parser - just one separator present
            testCases.Add(new StringNumberPair(31.24, "31,240"));
            testCases.Add(new StringNumberPair(31.24, "31.240"));
            testCases.Add(new StringNumberPair(312.4, "31,240e1"));
            testCases.Add(new StringNumberPair(312.4, "31.240e1"));
            testCases.Add(new StringNumberPair(312.4, "31,240e+1"));
            testCases.Add(new StringNumberPair(312.4, "31.240e+1"));
            testCases.Add(new StringNumberPair(3.124, "31,240e-1"));
            testCases.Add(new StringNumberPair(3.124, "31.240e-1"));
            testCases.Add(new StringNumberPair(31.24, "+31,240"));
            testCases.Add(new StringNumberPair(31.24, "+31.240"));
            testCases.Add(new StringNumberPair(312.4, "+31,240e1"));
            testCases.Add(new StringNumberPair(312.4, "+31.240e1"));
            testCases.Add(new StringNumberPair(312.4, "+31,240e+1"));
            testCases.Add(new StringNumberPair(312.4, "+31.240e+1"));
            testCases.Add(new StringNumberPair(3.124, "+31,240e-1"));
            testCases.Add(new StringNumberPair(3.124, "+31.240e-1"));
            testCases.Add(new StringNumberPair(-31.24, "-31,240"));
            testCases.Add(new StringNumberPair(-31.24, "-31.240"));
            testCases.Add(new StringNumberPair(-312.4, "-31,240e1"));
            testCases.Add(new StringNumberPair(-312.4, "-31.240e1"));
            testCases.Add(new StringNumberPair(-312.4, "-31,240e+1"));
            testCases.Add(new StringNumberPair(-312.4, "-31.240e+1"));
            testCases.Add(new StringNumberPair(-3.124, "-31,240e-1"));
            testCases.Add(new StringNumberPair(-3.124, "-31.240e-1"));

            testCases.Add(new StringNumberPair(0.24, ",240"));
            testCases.Add(new StringNumberPair(0.24, ".240"));
            testCases.Add(new StringNumberPair(2.4, ",240e1"));
            testCases.Add(new StringNumberPair(2.4, ".240e1"));
            testCases.Add(new StringNumberPair(2.4, ",240e+1"));
            testCases.Add(new StringNumberPair(2.4, ".240e+1"));
            testCases.Add(new StringNumberPair(0.024, ",240e-1"));
            testCases.Add(new StringNumberPair(0.024, ".240e-1"));
            testCases.Add(new StringNumberPair(0.24, "+,240"));
            testCases.Add(new StringNumberPair(0.24, "+.240"));
            testCases.Add(new StringNumberPair(2.4, "+,240e1"));
            testCases.Add(new StringNumberPair(2.4, "+.240e1"));
            testCases.Add(new StringNumberPair(2.4, "+,240e+1"));
            testCases.Add(new StringNumberPair(2.4, "+.240e+1"));
            testCases.Add(new StringNumberPair(0.024, "+,240e-1"));
            testCases.Add(new StringNumberPair(0.024, "+.240e-1"));
            testCases.Add(new StringNumberPair(-0.24, "-,240"));
            testCases.Add(new StringNumberPair(-0.24, "-.240"));
            testCases.Add(new StringNumberPair(-2.4, "-,240e1"));
            testCases.Add(new StringNumberPair(-2.4, "-.240e1"));
            testCases.Add(new StringNumberPair(-2.4, "-,240e+1"));
            testCases.Add(new StringNumberPair(-2.4, "-.240e+1"));
            testCases.Add(new StringNumberPair(-0.024, "-,240e-1"));
            testCases.Add(new StringNumberPair(-0.024, "-.240e-1"));

            testCases.Add(new StringNumberPair(31.0, "31,"));
            testCases.Add(new StringNumberPair(31.0, "31."));
            testCases.Add(new StringNumberPair(310, "31,e1"));
            testCases.Add(new StringNumberPair(310, "31.e1"));
            testCases.Add(new StringNumberPair(310, "31,e+1"));
            testCases.Add(new StringNumberPair(310, "31.e+1"));
            testCases.Add(new StringNumberPair(3.1, "31,e-1"));
            testCases.Add(new StringNumberPair(3.1, "31.e-1"));
            testCases.Add(new StringNumberPair(31.0, "+31,"));
            testCases.Add(new StringNumberPair(31.0, "+31."));
            testCases.Add(new StringNumberPair(310, "+31,e1"));
            testCases.Add(new StringNumberPair(310, "+31.e1"));
            testCases.Add(new StringNumberPair(310, "+31,e+1"));
            testCases.Add(new StringNumberPair(310, "+31.e+1"));
            testCases.Add(new StringNumberPair(3.1, "+31,e-1"));
            testCases.Add(new StringNumberPair(3.1, "+31.e-1"));
            testCases.Add(new StringNumberPair(-31.0, "-31,"));
            testCases.Add(new StringNumberPair(-31.0, "-31."));
            testCases.Add(new StringNumberPair(-310, "-31,e1"));
            testCases.Add(new StringNumberPair(-310, "-31.e1"));
            testCases.Add(new StringNumberPair(-310, "-31,e+1"));
            testCases.Add(new StringNumberPair(-310, "-31.e+1"));
            testCases.Add(new StringNumberPair(-3.1, "-31,e-1"));
            testCases.Add(new StringNumberPair(-3.1, "-31.e-1"));

            // custom local parser - several separators present, but no decimal point
            testCases.Add(new StringNumberPair(1234567, "1,234,567"));
            testCases.Add(new StringNumberPair(1234567, "1.234.567"));
            testCases.Add(new StringNumberPair(1234567, "+1,234,567"));
            testCases.Add(new StringNumberPair(1234567, "+1.234.567"));
            testCases.Add(new StringNumberPair(-1234567, "-1,234,567"));
            testCases.Add(new StringNumberPair(-1234567, "-1.234.567"));
            testCases.Add(new StringNumberPair(12345670, "1,234,567e1"));
            testCases.Add(new StringNumberPair(12345670, "1.234.567e1"));
            testCases.Add(new StringNumberPair(12345670, "+1,234,567e1"));
            testCases.Add(new StringNumberPair(12345670, "+1.234.567e1"));
            testCases.Add(new StringNumberPair(-12345670, "-1,234,567e1"));
            testCases.Add(new StringNumberPair(-12345670, "-1.234.567e1"));
            testCases.Add(new StringNumberPair(12345670, "1,234,567e+1"));
            testCases.Add(new StringNumberPair(12345670, "1.234.567e+1"));
            testCases.Add(new StringNumberPair(12345670, "+1,234,567e+1"));
            testCases.Add(new StringNumberPair(12345670, "+1.234.567e+1"));
            testCases.Add(new StringNumberPair(-12345670, "-1,234,567e+1"));
            testCases.Add(new StringNumberPair(-12345670, "-1.234.567e+1"));
            testCases.Add(new StringNumberPair(123456.7, "1,234,567e-1"));
            testCases.Add(new StringNumberPair(123456.7, "1.234.567e-1"));
            testCases.Add(new StringNumberPair(123456.7, "+1,234,567e-1"));
            testCases.Add(new StringNumberPair(123456.7, "+1.234.567e-1"));
            testCases.Add(new StringNumberPair(-123456.7, "-1,234,567e-1"));
            testCases.Add(new StringNumberPair(-123456.7, "-1.234.567e-1"));

            // custom local parser - several separators present, with decimal point        
            testCases.Add(new StringNumberPair(1234567.8, "1,234,567.8"));
            testCases.Add(new StringNumberPair(1234567.8, "1.234.567,8"));
            testCases.Add(new StringNumberPair(1234567.81234, "1,234,567.812,34"));
            testCases.Add(new StringNumberPair(1234567.81234, "1.234.567,812.34"));
            testCases.Add(new StringNumberPair(1234567.8123456, "1,234,567.812,345,6"));
            testCases.Add(new StringNumberPair(1234567.8123456, "1.234.567,812.345.6"));
            testCases.Add(new StringNumberPair(234567.8123456, "234,567.812,345,6"));
            testCases.Add(new StringNumberPair(234567.8123456, "234.567,812.345.6"));
            testCases.Add(new StringNumberPair(567.8123456, "567.812,345,6"));
            testCases.Add(new StringNumberPair(567.8123456, "567,812.345.6"));
            testCases.Add(new StringNumberPair(-1234567.8, "-1,234,567.8"));
            testCases.Add(new StringNumberPair(-1234567.8, "-1.234.567,8"));
            testCases.Add(new StringNumberPair(-1234567.81234, "-1,234,567.812,34"));
            testCases.Add(new StringNumberPair(-1234567.81234, "-1.234.567,812.34"));
            testCases.Add(new StringNumberPair(-1234567.8123456, "-1,234,567.812,345,6"));
            testCases.Add(new StringNumberPair(-1234567.8123456, "-1.234.567,812.345.6"));
            testCases.Add(new StringNumberPair(-234567.8123456, "-234,567.812,345,6"));
            testCases.Add(new StringNumberPair(-234567.8123456, "-234.567,812.345.6"));
            testCases.Add(new StringNumberPair(-567.8123456, "-567.812,345,6"));
            testCases.Add(new StringNumberPair(-567.8123456, "-567,812.345.6"));

            // custom local parser - several separators present, with decimal point and exponent
            testCases.Add(new StringNumberPair(12345678, "1,234,567.8e1"));
            testCases.Add(new StringNumberPair(12345678, "1.234.567,8e1"));
            testCases.Add(new StringNumberPair(12345678.1234, "1,234,567.812,34e1"));
            testCases.Add(new StringNumberPair(12345678.1234, "1.234.567,812.34e1"));
            testCases.Add(new StringNumberPair(12345678.123456, "1,234,567.812,345,6e1"));
            testCases.Add(new StringNumberPair(12345678.123456, "1.234.567,812.345.6e1"));
            testCases.Add(new StringNumberPair(2345678.123456, "234,567.812,345,6e1"));
            testCases.Add(new StringNumberPair(2345678.123456, "234.567,812.345.6e1"));
            testCases.Add(new StringNumberPair(5678.123456, "567.812,345,6e1"));
            testCases.Add(new StringNumberPair(5678.123456, "567,812.345.6e1"));
            testCases.Add(new StringNumberPair(-12345678, "-1,234,567.8e1"));
            testCases.Add(new StringNumberPair(-12345678, "-1.234.567,8e1"));
            testCases.Add(new StringNumberPair(-12345678.1234, "-1,234,567.812,34e1"));
            testCases.Add(new StringNumberPair(-12345678.1234, "-1.234.567,812.34e1"));
            testCases.Add(new StringNumberPair(-12345678.123456, "-1,234,567.812,345,6e1"));
            testCases.Add(new StringNumberPair(-12345678.123456, "-1.234.567,812.345.6e1"));
            testCases.Add(new StringNumberPair(-2345678.123456, "-234,567.812,345,6e1"));
            testCases.Add(new StringNumberPair(-2345678.123456, "-234.567,812.345.6e1"));
            testCases.Add(new StringNumberPair(-5678.123456, "-567.812,345,6e1"));
            testCases.Add(new StringNumberPair(-5678.123456, "-567,812.345.6e1"));

            testCases.Add(new StringNumberPair(12345678, "1,234,567.8e+1"));
            testCases.Add(new StringNumberPair(12345678, "1.234.567,8e+1"));
            testCases.Add(new StringNumberPair(12345678.1234, "1,234,567.812,34e+1"));
            testCases.Add(new StringNumberPair(12345678.1234, "1.234.567,812.34e+1"));
            testCases.Add(new StringNumberPair(12345678.123456, "1,234,567.812,345,6e+1"));
            testCases.Add(new StringNumberPair(12345678.123456, "1.234.567,812.345.6e+1"));
            testCases.Add(new StringNumberPair(2345678.123456, "234,567.812,345,6e+1"));
            testCases.Add(new StringNumberPair(2345678.123456, "234.567,812.345.6e+1"));
            testCases.Add(new StringNumberPair(5678.123456, "567.812,345,6e+1"));
            testCases.Add(new StringNumberPair(5678.123456, "567,812.345.6e+1"));
            testCases.Add(new StringNumberPair(-12345678, "-1,234,567.8e+1"));
            testCases.Add(new StringNumberPair(-12345678, "-1.234.567,8e+1"));
            testCases.Add(new StringNumberPair(-12345678.1234, "-1,234,567.812,34e+1"));
            testCases.Add(new StringNumberPair(-12345678.1234, "-1.234.567,812.34e+1"));
            testCases.Add(new StringNumberPair(-12345678.123456, "-1,234,567.812,345,6e+1"));
            testCases.Add(new StringNumberPair(-12345678.123456, "-1.234.567,812.345.6e+1"));
            testCases.Add(new StringNumberPair(-2345678.123456, "-234,567.812,345,6e+1"));
            testCases.Add(new StringNumberPair(-2345678.123456, "-234.567,812.345.6e+1"));
            testCases.Add(new StringNumberPair(-5678.123456, "-567.812,345,6e+1"));
            testCases.Add(new StringNumberPair(-5678.123456, "-567,812.345.6e+1"));

            testCases.Add(new StringNumberPair(123456.78, "1,234,567.8e-1"));
            testCases.Add(new StringNumberPair(123456.78, "1.234.567,8e-1"));
            testCases.Add(new StringNumberPair(123456.781234, "1,234,567.812,34e-1"));
            testCases.Add(new StringNumberPair(123456.781234, "1.234.567,812.34e-1"));
            testCases.Add(new StringNumberPair(123456.78123456, "1,234,567.812,345,6e-1"));
            testCases.Add(new StringNumberPair(123456.78123456, "1.234.567,812.345.6e-1"));
            testCases.Add(new StringNumberPair(23456.78123456, "234,567.812,345,6e-1"));
            testCases.Add(new StringNumberPair(23456.78123456, "234.567,812.345.6e-1"));
            testCases.Add(new StringNumberPair(56.78123456, "567.812,345,6e-1"));
            testCases.Add(new StringNumberPair(56.78123456, "567,812.345.6e-1"));
            testCases.Add(new StringNumberPair(-123456.78, "-1,234,567.8e-1"));
            testCases.Add(new StringNumberPair(-123456.78, "-1.234.567,8e-1"));
            testCases.Add(new StringNumberPair(-123456.781234, "-1,234,567.812,34e-1"));
            testCases.Add(new StringNumberPair(-123456.781234, "-1.234.567,812.34e-1"));
            testCases.Add(new StringNumberPair(-123456.78123456, "-1,234,567.812,345,6e-1"));
            testCases.Add(new StringNumberPair(-123456.78123456, "-1.234.567,812.345.6e-1"));
            testCases.Add(new StringNumberPair(-23456.78123456, "-234,567.812,345,6e-1"));
            testCases.Add(new StringNumberPair(-23456.78123456, "-234.567,812.345.6e-1"));
            testCases.Add(new StringNumberPair(-56.78123456, "-567.812,345,6e-1"));
            testCases.Add(new StringNumberPair(-56.78123456, "-567,812.345.6e-1"));

            // custom local parser - one of each separator - determined by the preceding zero
            testCases.Add(new StringNumberPair(0.1234, "0.123,4"));
            testCases.Add(new StringNumberPair(0.1234, "0,123.4"));
            testCases.Add(new StringNumberPair(-0.1234, "-0.123,4"));
            testCases.Add(new StringNumberPair(-0.1234, "-0,123.4"));
            testCases.Add(new StringNumberPair(0.1234, "+0.123,4"));
            testCases.Add(new StringNumberPair(0.1234, "+0,123.4"));
            testCases.Add(new StringNumberPair(0.1234, ".123,4"));
            testCases.Add(new StringNumberPair(0.1234, ",123.4"));
            testCases.Add(new StringNumberPair(-0.1234, "-.123,4"));
            testCases.Add(new StringNumberPair(-0.1234, "-,123.4"));
            testCases.Add(new StringNumberPair(0.1234, "+.123,4"));
            testCases.Add(new StringNumberPair(0.1234, "+,123.4"));

            // custom local parser - one of each separator - determined likelihood of being a currency amount
            testCases.Add(new StringNumberPair(1234.56, "1,234.56"));
            testCases.Add(new StringNumberPair(1234.56, "1.234,56"));
            testCases.Add(new StringNumberPair(-1234.56, "-1,234.56"));
            testCases.Add(new StringNumberPair(-1234.56, "-1.234,56"));

            foreach (var testCase in testCases)
            {
                Assert.That(TextUtil.TextToDouble(testCase.S, false), Is.EqualTo(testCase.N).Within(Precision * Math.Abs(testCase.N)),
                    testCase.S + ", heuristic parser");
            }

            // invalid strings
            var testCasesInvalid = new List<String>();
            testCasesInvalid.Add("--1.e-1");
            testCasesInvalid.Add("-+1.e-1");
            testCasesInvalid.Add("12,34,56");
            testCasesInvalid.Add("12.34.56");
            testCasesInvalid.Add("123,456,7.8");
            testCasesInvalid.Add("123.456.7,8");
            testCasesInvalid.Add("1.23,456,78");
            testCasesInvalid.Add("1,23.456.78");
            testCasesInvalid.Add("123,456.7,8");
            testCasesInvalid.Add("123.456,7.8");
            testCasesInvalid.Add("12,34.56");
            testCasesInvalid.Add("12.34,56");

            foreach (String testCase in testCasesInvalid)
            {
                Assert.That(Double.IsNaN(TextUtil.TextToDouble(testCase, false)), Is.EqualTo(true),
                    "invalid " + testCase + ", heuristic parser");
            }
        }

        [Test]
        public void TextToDoubleEmptyStringReturnsNaN()
        {
            Assert.That(Double.IsNaN(TextUtil.TextToDouble("", false)), Is.True, "empty string");
            Assert.That(Double.IsNaN(TextUtil.TextToDouble("   ", false)), Is.True, "whitespace only");
        }

        [Test]
        public void TextToDoubleHeuristicTruncatesUnexpectedChars()
        {
            // Input with trailing non-numeric chars — parseable portion is truncated (line 59-61)
            // The truncation limits separator scanning but the full string is still
            // passed to the locale parser, which fails on trailing letters → NaN
            Assert.That(Double.IsNaN(TextUtil.TextToDouble("42abc", false)), Is.True,
                "trailing letters cause NaN");
            Assert.That(Double.IsNaN(TextUtil.TextToDouble("123.45xyz", false)), Is.True,
                "trailing letters with dot cause NaN");
        }

        [Test]
        public void TextToDoubleHeuristicNoSeparatorsDelegatesToLocale()
        {
            // No dots or commas — delegates to locale-based parsing (line 95)
            Assert.That(TextUtil.TextToDouble("12345", false),
                Is.EqualTo(12345.0).Within(Precision * 12345), "plain integer, heuristic");
            Assert.That(TextUtil.TextToDouble("-678", false),
                Is.EqualTo(-678.0).Within(Precision * 678), "negative plain integer, heuristic");
        }

        [Test]
        public void TextToDoubleHeuristicBothMultipleReturnsNaN()
        {
            // Both '.' and ',' appear multiple times (line 105)
            Assert.That(Double.IsNaN(TextUtil.TextToDouble("1.000.000,000,00", false)), Is.True,
                "both . and , multiple times");
            Assert.That(Double.IsNaN(TextUtil.TextToDouble("1,000,000.000.00", false)), Is.True,
                "both , and . multiple times");
        }

        [Test]
        public void TextToDoubleHeuristicTooManyLeadingDigitsReturnsNaN()
        {
            // More than 3 digits before first separator with grouping chars present (line 132)
            // Need multiple of one separator type so line 109 branch is entered
            Assert.That(Double.IsNaN(TextUtil.TextToDouble("12345,678,901.23", false)), Is.True,
                "5 leading digits with grouping commas");
            Assert.That(Double.IsNaN(TextUtil.TextToDouble("12345.678.901,23", false)), Is.True,
                "5 leading digits with grouping dots");
        }

        [Test]
        public void TextToDoubleHeuristicAmbiguousSingleSeparatorsReturnsNaN()
        {
            // One dot and one comma, 4 apart, but positions don't match leading-zero
            // or currency patterns (line 185)
            Assert.That(Double.IsNaN(TextUtil.TextToDouble("12.345,6", false)), Is.True,
                "ambiguous 12.345,6");
            Assert.That(Double.IsNaN(TextUtil.TextToDouble("12,345.6", false)), Is.True,
                "ambiguous 12,345.6");
        }
    }
}
