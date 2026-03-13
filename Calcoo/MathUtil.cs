using System;

namespace Calcoo
{
    public static class MathUtil
    {
        private const double Epsilon = 1e-15;

        /**
         * Factorial using the Stirling formula for large arguments.
         */
        static public double Fact(double x,
                            int nSignificantDigits)
        {
            /*
             * if calcoo is unable to show all the meaningful digits of the result,
             * there is no point in the exact calculation, so we can use the
             * Stirling formula for the approximate calculation. Otherwise, we call
             * FactJr(x), which calculates the factorial of integer numbers exactly,
             * but is slower
             */
            // log_10 of the Stirling formula
            double log10XFact = (x + 0.5) * Math.Log10(x) - x * Math.Log10(Math.E);
            if (log10XFact > nSignificantDigits)
            {
                double a = 1.0 / (x + 1.0);
                return Math.Exp(-x - 1.0) * Math.Sqrt(2.0 * Math.PI) * Math.Pow(x + 1.0, (x + 0.5))
                       * (1.0
                       + a * ((1.0 / 12.0)
                         + a * ((1.0 / 288.0)
                           + a * ((-139.0 / 51840.0)
                             + a * ((-571.0 / 2488320.0)
                               )))));
            }

            return FactJr(x);
        }

        private static double FactJr(double x)
        {
            if (x >= 1.0)
                return x * FactJr(x - 1.0);

            // Abramowitz and Stegun, the error is < 3e-7
            return (1.0
            + x * ((-0.577191652)
              + x * ((0.988205891)
                + x * ((-0.897056937)
                  + x * ((0.918206857)
                    + x * ((-0.756704078)
                      + x * ((0.482199394)
                        + x * ((-0.193527818)
                          + x * (0.035868343)
                          ))))))));
        }

        /**
     * An estimate whether x can fit into display with nExpDigits for the exp part.
     */

        public static bool FactCanDo(double x,
            int nExpDigits)
        {
            /*
         * determines if the factorial is too large to be shown by calcoo this
         * is only an estimate, it still can turn out to be larger than the
         * calcoo's max number, but if it returns FALSE it is at least
         * guaranteed that the factorial will fit into double
         */

            if (x < 0.0)
                return false;
            if (x == 0.0)
                return true;
            double log10XFact = x * Math.Log10(x) - x * Math.Log10(Math.E) + 0.5 * Math.Log10(2 * Math.PI * x);
            return (log10XFact < Math.Pow(10, nExpDigits));
        }

        /**
     * We have to add up the numbers cleverly to have 100.1 - 100.0 - 0.1 == 0.
     */

        public static double SmartSum(double a,
            double b,
            int calcBase)
        {
            // to have 100.1 - 100.0 - 0.1 == 0
            double sum = a + b;
            double abssum = Math.Abs(sum);
            if (abssum == 0.0)
                return 0.0;
            double maxAbs = Math.Max(Math.Abs(a), Math.Abs(b));
            double log10Base = Math.Log10(calcBase);
            // ceil( log10( x ) / log10( base ) ) is the position before the decimal point
            // of the first significant digit of number x [ since log(a,b) = log(a,c)/log(b,c) for any c,
            // where log(a,b) is log of a base b; we chose c = 10 ]. Now the next condition checks
            // if the sum has the first significant digit at the same or higher place as
            // the bigger of the two added numbers.
            if (Math.Ceiling(Math.Log10(maxAbs) / log10Base) <= Math.Ceiling(Math.Log10(abssum) / log10Base))
                // If it does, then there is no need to worry about precision, because the error of the sum
                // due to the finite-precision machine arithmetics would be not less than the bigger error
                // of the two numbers being added, therefore any error in either of the two numbers will
                // be hidden by the error in the sum
                return sum;
            else
            {
                // If it does not, precision problems such as 100.1 - 100.0 - 0.1 != 0 may arise.
                // So here we deal with the case when some of the digits of the sum result from the rounded-off
                // portions of at least one of the numbers being added. In order to reduce the noise in the calculator's
                // presentation, we need to round off these digits. Here cutoff is the smallest number looking
                // like 0.00000000001, which is larger than the error of the sum which results from the
                // two added numbers being rounded.
                double cutoff = Math.Pow(calcBase, Math.Ceiling(Math.Log10(maxAbs * Epsilon) / log10Base));
                // We throw out all digits after the cutoff.
                return cutoff * Math.Round(sum / cutoff);
            }
        }
    }
}
