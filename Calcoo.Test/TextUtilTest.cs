using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Calcoo.Test
{
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
            double[] testCasesD = {1200, 1200.1, -1200.1};

            foreach (var d in testCasesD)
            {
                var s = d.ToString(CultureInfo.CurrentCulture);
                Assert.That(TextUtil.TextToDouble(s, true), Is.EqualTo(d).Within(Precision*Math.Abs(d)), s + ", NFParser");
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
                Assert.That(TextUtil.TextToDouble(testCase.S, false), Is.EqualTo(testCase.N).Within(Precision*Math.Abs(testCase.N)),
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
    }
}
