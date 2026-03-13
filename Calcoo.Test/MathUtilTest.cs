using System;
using NUnit.Framework;

namespace Calcoo.Test
{
    [TestFixture]
    internal class MathUtilTest
    {
        [Test]
        public void FactTest()
        {
            Assert.That(MathUtil.Fact(0.0, 10), Is.EqualTo(1.0).Within(1e-10), "Zero");
            // the error of the used approximation per Abramowitz and Stegun is 3e-7
            Assert.That(MathUtil.Fact(0.5, 10), Is.EqualTo(Math.Sqrt(Math.PI) / 2.0).Within(3e-7), "Approximate - 0.5!");
            Assert.That(MathUtil.Fact(4.0, 10), Is.EqualTo(24.0).Within(1e-10), "Exact - 4!");
            Assert.That(MathUtil.Fact(15.0, 10), Is.EqualTo(2.0 * 3 * 4 * 5 * 6 * 7 * 8 * 9 * 10 * 11 * 12 * 13 * 14 * 15).Within(1e3),
                "Approximate - 15!");
            Assert.That(MathUtil.Fact(24.0, 10), Is.EqualTo(2.0 * 3 * 4 * 5 * 6 * 7 * 8 * 9 * 10 * 11 * 12 * 13 * 14 * 15 * 16 * 17 * 18
                            * 19 * 20 * 21 * 22 * 23 * 24).Within(1e14), "Approximate - 24!");
        }

        [Test]
        public void FactCanDoTest()
        {
            Assert.That(MathUtil.FactCanDo(0.0, 2), Is.EqualTo(true), "of 0");
            Assert.That(MathUtil.FactCanDo(24.0, 2), Is.EqualTo(true), "of 24");
            Assert.That(MathUtil.FactCanDo(69.0, 2), Is.EqualTo(true), "of 69");
            Assert.That(MathUtil.FactCanDo(70.0, 2), Is.EqualTo(false), "of 70");
            Assert.That(MathUtil.FactCanDo(124.0, 2), Is.EqualTo(false), "of 124");
        }

        [Test]
        public void SmartSumTest()
        {
            Assert.That(MathUtil.SmartSum(MathUtil.SmartSum(100.1, (-100.0), 10), (-0.1), 10),
                Is.EqualTo(0.0).Within(1e-20), "( 100.1 - 100 ) - 0.1 == 0");
            // Same-magnitude sum (no precision issue)
            Assert.That(MathUtil.SmartSum(1.5, 2.5, 10),
                Is.EqualTo(4.0).Within(1e-15), "1.5 + 2.5 == 4");
            // Zero result directly
            Assert.That(MathUtil.SmartSum(5.0, -5.0, 10),
                Is.EqualTo(0.0).Within(1e-20), "5 - 5 == 0");
        }

        [Test]
        public void FactCanDoNegativeTest()
        {
            Assert.That(MathUtil.FactCanDo(-1.0, 2), Is.EqualTo(false), "of -1");
            Assert.That(MathUtil.FactCanDo(-100.0, 2), Is.EqualTo(false), "of -100");
        }

    }
}
