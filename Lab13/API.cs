using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troschuetz;
using Troschuetz.Random;

namespace Lab13
{
    static class API
    {
        public static float GetZ1(double l0, double o, double w)
        {
            double a1 = Math.Sin(l0 * o) / Math.Pow(Math.Cos(o), 1 / l0);
            double b1 = Math.Pow(Math.Cos((l0 - 1) * o) / w, (1 - l0) / l0);
            return (float)(a1 * b1);
        }
        public static float GetZ2(double l1, double o, double w)
        {
            double a2 = Math.Sin(l1 * o) / Math.Pow(Math.Cos(o), 1 / l1);
            double b = Math.Cos((l1 - 1) * o) / w;
            double bb = (1 - l1) / l1;
            double b2 = Math.Pow(b, bb);
            return (float)(a2 * b2);
        }
        public static Tuple<float[], float[]> GetZ1Z2(int n, double l0, double l1)
        {
            Random rnd = new Random();
            float[] z1 = new float[n], z2 = new float[n];
            double[] u1 = new double[n], u2 = new double[n];
            for (int i = 0; i < n; ++i)
            {
                u1[i] = rnd.NextDouble();
                u2[i] = rnd.NextDouble();
            }
            double al0 = 1 / l0, al1 = 1 / l1;
            Parallel.For(0, n, (i) =>
            {
                double o = Math.PI * (u1[i] - 0.5d), w = -Math.Log(u2[i]);

                double a1 = Math.Sin(l0 * o) / Math.Pow(Math.Cos(o), al0);
                double b1 = Math.Pow(Math.Cos((l0 - 1) * o) / w, (1 - l0) * al0);

                double a2 = Math.Sin(l1 * o) / Math.Pow(Math.Cos(o), al1);
                double b = Math.Cos((l1 - 1) * o) / w;
                double b2 = Math.Pow(b, (1 - l1) * al1);
                z1[i] = (float)(a1 * b1);
                z2[i] = (float)(a2 * b2);
            });
            return new Tuple<float[], float[]>(z1, z2);
        }
        public static float GetZ2Sample(int n, double l1)
        {
            Random rnd = new Random();
            float z2 = 0f;
            var u1 = new double[n];
            var u2 = new double[n];
            for (int i = 0; i < n; ++i)
            {
                u1[i] = rnd.NextDouble();
                u2[i] = rnd.NextDouble();
            }
            for (int i = 0; i < n; ++i)
            {
                double o = Math.PI * (u1[i] - 0.5d), w = -Math.Log(u2[i]);
                z2 += GetZ2(l1, o, w);
            }
            return z2;
        }

        public static TRandom TRAND = new TRandom();
        public static double GAMMA(double alpha)
        {
            try
            {
                double gamma = 0;
                if (alpha > 0)
                {
                    if (alpha > 0 && alpha < 1)
                    {
                        gamma = GAMMA(alpha + 1) / alpha;
                    }
                    else if (alpha >= 1 && alpha <= 2)
                    {
                        gamma = 1 - 0.577191652 * Math.Pow(alpha - 1, 1) + 0.988205891 * Math.Pow(alpha - 1, 2) -
                                0.897056937 * Math.Pow(alpha - 1, 3) + 0.918206857 * Math.Pow(alpha - 1, 4) -
                                0.756704078 * Math.Pow(alpha - 1, 5) + 0.482199394 * Math.Pow(alpha - 1, 6) -
                                0.193527818 * Math.Pow(alpha - 1, 7) + 0.03586843 * Math.Pow(alpha - 1, 8);
                    }
                    else
                    {
                        gamma = (alpha - 1) * GAMMA(alpha - 1);
                    }
                }
                if (alpha > 171)
                {
                    gamma = Math.Pow(10, 307);
                }
                return gamma;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static double GAMMA2(double x)
        {
            if (x <= 0.0)
            {
                string msg = string.Format("Invalid input argument {0}. Argument must be positive.", x);
                throw new ArgumentOutOfRangeException(msg);
            }

            // Split the function domain into three intervals:
            // (0, 0.001), [0.001, 12), and (12, infinity)

            ///////////////////////////////////////////////////////////////////////////
            // First interval: (0, 0.001)
            //
            // For small x, 1/Gamma(x) has power series x + gamma x^2  - ...
            // So in this range, 1/Gamma(x) = x + gamma x^2 with error on the order of x^3.
            // The relative error over this interval is less than 6e-7.

            const double gamma = 0.577215664901532860606512090; // Euler's gamma constant

            if (x < 0.001)
                return 1.0 / (x * (1.0 + gamma * x));

            ///////////////////////////////////////////////////////////////////////////
            // Second interval: [0.001, 12)

            if (x < 12.0)
            {
                // The algorithm directly approximates gamma over (1,2) and uses
                // reduction identities to reduce other arguments to this interval.

                double y = x;
                int n = 0;
                bool arg_was_less_than_one = (y < 1.0);

                // Add or subtract integers as necessary to bring y into (1,2)
                // Will correct for this below
                if (arg_was_less_than_one)
                {
                    y += 1.0;
                }
                else
                {
                    n = (int)(Math.Floor(y)) - 1;  // will use n later
                    y -= n;
                }

                // numerator coefficients for approximation over the interval (1,2)
                double[] p =
                {
                    -1.71618513886549492533811E+0,
                     2.47656508055759199108314E+1,
                    -3.79804256470945635097577E+2,
                     6.29331155312818442661052E+2,
                     8.66966202790413211295064E+2,
                    -3.14512729688483675254357E+4,
                    -3.61444134186911729807069E+4,
                     6.64561438202405440627855E+4
                };

                // denominator coefficients for approximation over the interval (1,2)
                double[] q =
                {
                    -3.08402300119738975254353E+1,
                     3.15350626979604161529144E+2,
                    -1.01515636749021914166146E+3,
                    -3.10777167157231109440444E+3,
                     2.25381184209801510330112E+4,
                     4.75584627752788110767815E+3,
                    -1.34659959864969306392456E+5,
                    -1.15132259675553483497211E+5
                };

                double num = 0.0;
                double den = 1.0;
                int i;

                double z = y - 1;
                for (i = 0; i < 8; i++)
                {
                    num = (num + p[i]) * z;
                    den = den * z + q[i];
                }
                double result = num / den + 1.0;

                // Apply correction if argument was not initially in (1,2)
                if (arg_was_less_than_one)
                {
                    // Use identity gamma(z) = gamma(z+1)/z
                    // The variable "result" now holds gamma of the original y + 1
                    // Thus we use y-1 to get back the orginal y.
                    result /= (y - 1.0);
                }
                else
                {
                    // Use the identity gamma(z+n) = z*(z+1)* ... *(z+n-1)*gamma(z)
                    for (i = 0; i < n; i++)
                        result *= y++;
                }

                return result;
            }

            ///////////////////////////////////////////////////////////////////////////
            // Third interval: [12, infinity)

            if (x > 171.624)
            {
                // Correct answer too large to display. 
                return double.PositiveInfinity;
            }

            return Math.Exp(LogGamma(x));
        }
        public static double LogGamma(double x)
        {
            if (x <= 0.0)
            {
                string msg = string.Format("Invalid input argument {0}. Argument must be positive.", x);
                throw new ArgumentOutOfRangeException(msg);
            }

            if (x < 12.0)
            {
                return Math.Log(Math.Abs(GAMMA2(x)));
            }

            // Abramowitz and Stegun 6.1.41
            // Asymptotic series should be good to at least 11 or 12 figures
            // For error analysis, see Whittiker and Watson
            // A Course in Modern Analysis (1927), page 252

            double[] c =
            {
                 1.0/12.0,
                -1.0/360.0,
                1.0/1260.0,
                -1.0/1680.0,
                1.0/1188.0,
                -691.0/360360.0,
                1.0/156.0,
                -3617.0/122400.0
            };
            double z = 1.0 / (x * x);
            double sum = c[7];
            for (int i = 6; i >= 0; i--)
            {
                sum *= z;
                sum += c[i];
            }
            double series = sum / x;

            double halfLogTwoPi = 0.91893853320467274178032973640562;
            double logGamma = (x - 0.5) * Math.Log(x) - x + halfLogTwoPi + series;
            return logGamma;
        }

        private static List<int> FactorialData = new List<int>(new[] { 1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800, 39916800, 479001600 });
        public static double Factorial(int x, double devision)
        {
            int count = FactorialData.Count;
            if (x < count) return devision / (FactorialData[x] * 1d);
            var last = FactorialData[count - 1];
            devision /= last;
            for (int i = FactorialData.Count; i < x; i++)
            {
                devision /= i;
            }
            return devision;
        }
        public static float[] GetFunkPL0(float[] xx, double L0, float maxV = 0)
        {
            float[] y = new float[xx.Length];
            for (int i = 0, m = xx.Length; i < m; ++i)
            {
                float x = xx[i];
                if (Math.Abs(x) < 0.01f)
                {
                    y[i] = 0;
                    continue;
                }
                double rez = 0;
                double p = Math.Pow(-x, -L0);
                double t = L0 * Math.PI;
                for (int j = 1; j < 100; ++j)
                {
                    double a = (Math.Abs(p) > 0.01) ? Math.Pow(p, j) : 0;
                    var b = Math.Sin(-(t * j) / 2d);
                    double c = GAMMA2(L0 * j + 1);
                    double g = a * b * c;
                    rez += Factorial(j, g);
                }
                rez /= Math.PI * x;
                y[i] = (float)rez;
            }
            return y;
        }
        public static float[] GetFunkPL1(float[] xx, double L1, float maxV = 0)
        {
            float[] y = new float[xx.Length];
            for (int i = 0, m = xx.Length; i < m; ++i)
            {
                float x = xx[i];
                if (Math.Abs(x) < 0.001f)
                {
                    y[i] = maxV;
                    continue;
                }
                double rez = 0;
                for (int j = 1; j < 270; ++j)
                {
                    double p = Math.Pow(-x, j);
                    double c = GAMMA2((j / L1) + 1);
                    var b = Math.Sin(-(Math.PI * j) / 2d);
                    double a = Factorial(j, c);
                    rez += a * p * b;
                }
                rez /= Math.PI * x;
                y[i] = (float)rez;
            }
            return y;
        }

        public static float[] NormV(float[] xx, float d, int t)
        {
            int m = xx.Length;
            float[] yy = new float[m];
            float s = 4 * d * t;
            for (int i = 0; i < m; ++i)
            {
                var x = xx[i];
                double c = -x * x / s;
                double a = Math.Exp(c);
                yy[i] = (float)(a / Math.Sqrt(Math.PI * s));
            }
            return yy;
        }

        public static Tuple<float[], float[]> GetFunkLog(int[] xx, int[] yy, float moveX)
        {
            int m = xx.Length;
            List<float> x = new List<float>(), y = new List<float>();
            var mx = (int)(moveX);
            for (int i = 0; i < m; ++i)
            {
                var xxt = xx[i] + mx;
                //if (xxt < 1) continue;
                if (xxt < 1 || yy[i] < 1) continue;
                var xt = (float)Math.Log(xxt);
                //if (yy[i] < 1)
                //{
                //    x.Add(xt);
                //    y.Add(0);
                //}
                var yt = (float)Math.Log(yy[i]);
                //if (float.IsInfinity(yt)) continue;
                x.Add(xt);
                y.Add(yt);
            }
            return new Tuple<float[], float[]>(x.ToArray(), y.ToArray());
        }
        public static float[] Copy(this float[] array)
        {
            int m = array.Length;
            var temp = new float[m];
            for (int i = 0; i < m; ++i) { temp[i] = array[i]; }
            return temp;
        }
        public static float[] CopyToFloat(this int[] array)
        {
            int m = array.Length;
            var temp = new float[m];
            for (int i = 0; i < m; ++i) { temp[i] = array[i]; }
            return temp;
        }

        //public static float[] MoveValues(this float[] array, float value)
        //{
        //    int m = array.Length;
        //    var temp = new float[m];
        //    for (int i = 0; i < m; ++i) { temp[i] = array[i] + value; }
        //    return temp;
        //}
        //public static float[] ScaleValues(this float[] array, float value)
        //{
        //    int m = array.Length;
        //    var temp = new float[m];
        //    for (int i = 0; i < m; ++i) { temp[i] = array[i] * value; }
        //    return temp;
        //}
    }
}
