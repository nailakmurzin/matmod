using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab13
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // лаба 1.1 
            var imgs = Lab11();
            // гистограмма с функцией распределения L0,l1
            pb1.Image = imgs.Item1;
            pb2.Image = imgs.Item2;

            // лаба 1.2 - пример 1
            pbEx1.Image = Lab12();

            // лаба 1.2 - пример 2
            pbEx2.Image = Lab13();

            pbEx3.Image = Lab14();
        }

        private Image Lab14()
        {
            string diagrammPath = Application.StartupPath + @"\diagramm14.jpg";
            string fileZ1 = Application.StartupPath + @"\ex1Normal3tt.dat";
            string fileZ2 = Application.StartupPath + @"\ex1Normal3mo.dat";
            Tuple<float[], float[]> xy = new Tuple<float[], float[]>(LoadDataInMemory(fileZ1), LoadDataInMemory(fileZ2));
            if (xy.Item1 == null || xy.Item2 == null)
            {
                int t1 = 2000, n = 50;
                var mo = new float[n];
                var tt = new float[n];
                double al0 = 1 / ContextData.L0;
                Parallel.For(t1, t1 + n, T =>
                {
                    int N = 100000;
                    Random rnd = new Random();
                    var z = new double[N];
                    for (int i = 0; i < N; ++i)
                    {
                        double t = t1, x = 0;
                        while (t <= T)
                        {
                            double o = Math.PI * (rnd.NextDouble() - 0.5d);
                            double w = -Math.Log(rnd.NextDouble());
                            double a1 = Math.Sin(ContextData.L0 * o) / Math.Pow(Math.Cos(o), al0);
                            double b1 = Math.Pow(Math.Cos((ContextData.L0 - 1) * o) / w, (1 - ContextData.L0) * al0);
                            double th = Math.Abs(a1 * b1);
                            t += th;
                            x += API.TRAND.Normal(0, 1);
                        }
                        z[i] = x;
                    }
                    double m = 0;
                    for (int k = 0; k < N; ++k) { m += z[k]; }
                    int index = T - t1;
                    mo[index] = (float)Math.Log(Math.Abs(m / N));
                    tt[index] = (float)Math.Log(T);
                });
                xy = new Tuple<float[], float[]>(tt, mo);
                SaveData(tt, fileZ1);
                SaveData(mo, fileZ2);
            }
            var img = xy.DrawFunkFromPoints(1000, 600, new Pen(Color.Blue, 2f));
            img.Save(diagrammPath);
            return img;
        }

        public Tuple<Image, Image> Lab11()
        {
            string diagrammPath1 = Application.StartupPath + @"\diagramm11Z1.jpg";
            string diagrammPath2 = Application.StartupPath + @"\diagramm11Z2.jpg";

            string fileZ1 = Application.StartupPath + @"\generateZ1.dat";
            string fileZ2 = Application.StartupPath + @"\generateZ2.dat";

            Tuple<float[], float[]> z = new Tuple<float[], float[]>(LoadDataInMemory(fileZ1), LoadDataInMemory(fileZ2));
            if (z.Item1 == null || z.Item2 == null)
            {
                z = API.GetZ1Z2(10000000, ContextData.L0, ContextData.L1);
                SaveData(z.Item1, fileZ1);
                SaveData(z.Item2, fileZ2);
            }
            var hist1 = new Hist(z.Item1, 1000, 600, 2, -30, 30) { Brush = new SolidBrush(Color.Crimson) };
            var hist2 = new Hist(z.Item2, 1000, 600, 2, -20, 20) { Brush = new SolidBrush(Color.DarkOrange) };
            var x1 = GetMass(-30, 0, 0.1f);
            var y1 = API.GetFunkPL0(x1, ContextData.L0);
            var x2 = GetMass(-2.01f, 2.01f, 0.02f);
            var y2 = API.GetFunkPL1(x2, ContextData.L1);

            var img1 = hist1.DrawFuncScaleMaxFunkY(x1, y1, new Pen(Color.Blue, 2f), (ref double x, ref double y, Hist hist, float maxY, ref int moveX, ref int moveY) =>
            {
                y = (hist.Image.Height / (hist.MaxY * 10d / (hist.TrimVariables.Length * 1d)));
                moveX = 10;
            });
            img1.Save(diagrammPath1);
            var img2 = hist2.DrawFuncScaleMaxFunkY(x2, y2, new Pen(Color.Blue, 2f));
            img2.Save(diagrammPath2);
            return new Tuple<Image, Image>(img1, img2);
        }
        public Image Lab12()
        {
            string diagrammPath = Application.StartupPath + @"\diagramm12.jpg";
            string fileZ2 = Application.StartupPath + @"\ex1Normal.dat";
            float[] z1 = LoadDataInMemory(fileZ2);
            if (z1 == null)
            {
                int N = 10000000;
                z1 = new float[N];
                Parallel.For(0, N, i =>
                {
                    double t = 0;
                    int k = 0;
                    while (t < 2000)
                    {
                        t += API.TRAND.Exponential(1d);
                        k++;
                    }
                    double z = 0;
                    for (int j = 0; j < k; j++) { z += API.TRAND.Normal(0, 1); }
                    z1[i] = (float)z;
                });
                SaveData(z1, fileZ2);
            }
            var hist1 = new Hist(z1, 1000, 600, 2, -160, 160) { Brush = new SolidBrush(Color.DarkSlateBlue), Pen = new Pen(Color.Black, 0.001f) };
            float[] x1 = GetMass(-160, 160, 1f), y1 = API.NormV(x1, 1f / 2, 2000);
            var img = hist1.DrawFuncScaleMaxFunkY(x1, y1, new Pen(Color.Blue, 2f));
            img.Save(diagrammPath);
            return img;
        }
        public Image Lab13()
        {
            // если картинка диаграммы существует
            string diagrammPath = Application.StartupPath + @"\diagramm13.jpg";
            //Image img = LoadDiagramm(diagrammPath);
            //if (img != null) return img;

            int N = 10000000;
            string fileZ2 = Application.StartupPath + @"\ex2Z2.dat";
            float[] z1 = LoadDataInMemory(fileZ2);
            if (z1 == null)
            {
                z1 = new float[N];
                Parallel.For(0, N, i =>
                {
                    double t = 0;
                    int k = 0;
                    while (t < 2000)
                    {
                        t += API.TRAND.Exponential(1d);
                        k++;
                    }
                    float z = API.GetZ2Sample(k, ContextData.L1);
                    z1[i] = z;
                });
                SaveData(z1, fileZ2);
            }
            ContextData.Z1Lab13 = z1;
            var Z = Trim(z1, -5000, 5000);
            var hist = new Hist(z1, 1000, 600, 2, -5000, 5000) { Brush = new SolidBrush(Color.Red) };
            pictureBox1.Image = hist.Image;
            var sourceXy = API.GetFunkLog(hist.Histogram.Keys.ToArray(), hist.Histogram.Values.ToArray(), hist.MinX * hist.ScaleX);

            float scX, scY;
            var xy = sourceXy.DrawFunkFromPoints(1000, 600, new Pen(Color.Red, 2f), out scX, out scY);
            int l = sourceXy.Item1.Length;
            var mnk = HelperFunks.MNKDraw(xy, sourceXy.Item1, sourceXy.Item2, (1 * l / 20), (4 * l / 9) - 1, new Pen(Color.Green, 2f), scX, scY, 600, (float)(-1 - ContextData.L1));
            mnk.Save(diagrammPath);
            return mnk;
        }
        public Image DrawFuncTest(int width, int height, float[] xx, float[] yy, Pen pen, float correctX = 1, float correctY = 1)
        {
            Bitmap Image = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(Image);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawImage(ContextData.Lab11Img, new Point(0, 0));
            float h = width / 2f;
            float oldx = xx[0] * correctX + h, oldy = height - yy[0] * correctY;
            for (int i = 1, m = xx.Length; i < m; ++i)
            {
                float x = xx[i] * correctX + h, y = height - yy[i] * correctY;
                g.DrawLine(pen, oldx, oldy, x, y);
                oldx = x;
                oldy = y;
            }
            return Image;
        }
        public float[] LoadDataInMemory(string path)
        {
            float[] z1;
            if (!File.Exists(path)) return null;
            var memoryStream = new MemoryStream(File.ReadAllBytes(path));
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                z1 = new float[reader.BaseStream.Length / 4];
                int j = 0;
                while (reader.BaseStream.Position != reader.BaseStream.Length) { z1[j++] = reader.ReadSingle(); }
            }
            return z1;
        }
        public void SaveData(float[] data, string path)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
            {
                for (int i = 0, m = data.Length; i < m; ++i) { writer.Write(data[i]); }
            }
        }

        public float[] GetMass(float min, float max, float step)
        {
            var list = new List<float>();
            for (float i = min; i < max; i += step) { list.Add(i); }
            return list.ToArray();
        }
        public float[] Trim(float[] zV, int ogrXl, int ogrXr)
        {
            float[] temp = new float[zV.Length];
            int len = 0;
            for (int i = 0, m = zV.Length; i < m; ++i)
            {
                float z = zV[i];
                if (z < ogrXr && z > ogrXl) { temp[len++] = z; }
            }
            float[] zVariables = new float[len];
            for (int i = 0, m = len; i < m; ++i) { zVariables[i] = temp[i]; }
            return zVariables;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            var x2 = ContextData.Lab11.Item1;
            var y2 = ContextData.Lab11.Item2;
            pbEx11.Image = DrawFuncTest(1000, 600, x2, y2, new Pen(Color.Black, 2f), (int)numericUpDown1.Value, (int)numericUpDown2.Value);
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            var x2 = ContextData.Lab11.Item1;
            var y2 = ContextData.Lab11.Item2;
            pbEx11.Image = DrawFuncTest(1000, 600, x2, y2, new Pen(Color.Black), (int)numericUpDown1.Value, (int)numericUpDown2.Value);
        }
    }

    public static class ContextData
    {
        public static int Variant = 2;
        public static double L0 = 1d - 1d / (1d + Variant);
        public static double L1 = L0 + 1d;

        public static float[] Z1Lab13;
        public static float[] XLab12;
        public static Tuple<float[], float[]> TXlab14;
        public static float[] Z1Lab14;
        public static Tuple<float[], float[]> Lab11;
        public static Bitmap Lab11Img { get; set; }
    }


}
