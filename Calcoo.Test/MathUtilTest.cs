using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Calcoo.Test
{
    internal class MathUtilTest
    {
        [Test]
        public void FactTest()
        {
            ClassicAssert.AreEqual(1.0, MathUtil.Fact(0.0, 10), 1e-10, "Zero");
            // the error of the used approximation per Abramowitz and Stegun is 3e-7
            ClassicAssert.AreEqual(Math.Sqrt(Math.PI)/2.0, MathUtil.Fact(0.5, 10), 3e-7, "Approximate - 0.5!");
            ClassicAssert.AreEqual(24.0, MathUtil.Fact(4.0, 10), 1e-10, "Exact - 4!");
            ClassicAssert.AreEqual(2.0*3*4*5*6*7*8*9*10*11*12*13*14*15,
                MathUtil.Fact(15.0, 10), 1e3, "Approximate - 15!");
            ClassicAssert.AreEqual(2.0*3*4*5*6*7*8*9*10*11*12*13*14*15*16*17*18
                            *19*20*21*22*23*24, MathUtil.Fact(24.0, 10), 1e14, "Approximate - 24!");
        }

        [Test]
        public void FactCanDoTest()
        {
            ClassicAssert.AreEqual(true, MathUtil.FactCanDo(24.0, 2), "of 24");
            ClassicAssert.AreEqual(true, MathUtil.FactCanDo(69.0, 2), "of 69");
            ClassicAssert.AreEqual(false, MathUtil.FactCanDo(70.0, 2), "of 70");
            ClassicAssert.AreEqual(false, MathUtil.FactCanDo(124.0, 2), "of 124");
        }

        [Test]
        public void AsinhTest()
        {
            double[] testValues = {-20.0, -10.0, -1.0, -0.1, -0.0001, 0.0, 0.0001, 0.1, 1.0, 10.0, 20.0};
            const double precision = 1e-12;
            foreach (double testValue in testValues)
                ClassicAssert.AreEqual(testValue, MathUtil.Asinh(Math.Sinh(testValue)),
                    precision, "asinh of sinh of " + testValue);
        }

        [Test]
        public void AcoshTest()
        {
            double[] testValues = {-20.0, -10.0, -1.0, -0.1, -0.0001, 0.0, 0.0001, 0.1, 1.0, 10.0, 20.0};
            const double precision = 1e-12;
            foreach (double testValue in testValues)
                ClassicAssert.AreEqual(Math.Abs(testValue),
                    MathUtil.Acosh(Math.Cosh(testValue)), precision, "acosh of cosh of " + testValue);
        }

        [Test]
        public void AtanhTest()
        {
            double[] testValues = {-5.0, -1.0, -0.1, -0.0001, 0.0, 0.0001, 0.1, 1.0, 5.0};
            // cannot resolve +/-20 - returns infinity; precision for +/-10 is below
            // 1e-12 - it is Ok
            const double precision = 1e-12;
            foreach (double testValue in testValues)
                ClassicAssert.AreEqual(testValue, MathUtil.Atanh(Math.Tanh(testValue)),
                    precision, "atanh of tanh of " + testValue);
        }

        [Test]
        public void SmartSumTest()
        {
            ClassicAssert.AreEqual(0.0, MathUtil.SmartSum(MathUtil.SmartSum(100.1, (-100.0), 10), (-0.1), 10), 1e-20,
                "( 100.1 - 100 ) - 0.1 == 0");
        }
    }
}
