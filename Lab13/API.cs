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
            var z1 = new float[n];
            var z2 = new float[n];
            var u1 = new double[n];
            var u2 = new double[n];
            for (int i = 0; i < n; ++i)
            {
                u1[i] = rnd.NextDouble();
                u2[i] = rnd.NextDouble();
            }
            double al0 = 1 / l0;
            double al1 = 1 / l1;
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
            //for (int i = 0; i < n; ++i)
            //{
            //    double o = Math.PI * (u1[i] - 0.5d), w = Math.Log(u2[i]);

            //    double a1 = Math.Sin(l0 * o) / Math.Pow(Math.Cos(o), al0);
            //    double b1 = Math.Pow(Math.Cos((l0 - 1) * o) / w, (1 - l0) * al0);

            //    double a2 = Math.Sin(l1 * o) / Math.Pow(Math.Cos(o), al1);
            //    double b = Math.Cos((l1 - 1) * o) / w;
            //    double b2 = Math.Pow(b, (1 - l1) * al1);
            //    z[i] = new Z1Z2 { Z1 = (float)(a1 * b1), Z2 = (float)(a2 * b2) };
            //}
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
        static void Main1(string[] args)
        {
            int T = 2000, N = 1000000;
            double[] Z = new double[N];

            //for (int i = 0; i < N; ++i)
            //{
            //    double t = 0, x = 0;
            //    int k = 0;
            //    while (t < 2000)
            //    {
            //        t += TRAND.Exponential(1d);
            //        k++;
            //    }

            //    for (int j = 0; j < k; ++j)
            //    {
            //        x += TRAND.Normal(0, 1);
            //    }
            //    Z[i] = x;
            //}

            //var text = new StringBuilder();
            //for (int i = 0; i < N; ++i)
            //{
            //    text.AppendLine(Z[i].ToString());
            //}
            //File.WriteAllText(@"C:\Nail\WriteText2.txt", text.ToString());

        }
    }
}
