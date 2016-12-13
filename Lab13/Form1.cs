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
            // гистограмма с функцией распределения L0
            pb1.Image = imgs.Item1;
            // гистограмма с функцией распределения L1
            pb2.Image = imgs.Item2;

            // лаба 1.2 - пример 1
            pbEx1.Image = Lab12();

            // лаба 1.2 - пример 2
            pbEx2.Image = Lab13();
            pbEx2.Refresh();
        }
        public DataHist DrawHist(int width, int height, byte step, float[] zVariables, Brush brush, Pen pen)
        {
            int h = height - 20, hstart = 10, yAxe = h + hstart;
            int w = width - 20;
            int wStart = 10;
            int len = zVariables.Length;

            float minZ = float.MaxValue, maxZ = float.MinValue;
            for (int i = 0; i < len; ++i)
            {
                float z = zVariables[i];
                if (minZ > z) minZ = z;
                if (maxZ < z) maxZ = z;
            }
            // получаем максимально возможное число z от 0
            float maxX = maxZ - minZ;
            // получаем число возможных столбцов
            int countColumns = w / step;
            // соотношение столбцов к максимально возможному числу
            float ScaleX = countColumns / maxX;

            var dict = new Dictionary<int, int>();
            for (int i = 0; i <= countColumns; i++) { dict.Add(i, 0); }
            for (int i = 0, m = zVariables.Length; i < m; i++) { float x = ((zVariables[i] - minZ) * ScaleX); ++dict[(int)x]; }
            int maxY = int.MinValue;
            var dv = dict.Values.ToArray();
            for (int i = 0; i <= countColumns; i++) { int x = dv[i]; if (x > maxY) { maxY = x; } }

            float ScaleY = (maxY * 1f) / (h * 1f);

            for (int i = 0; i <= countColumns; i++)
            {
                dict[i] = (int)(dict[i] / ScaleY);
            }

            Bitmap bp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bp);
            g.Clear(Color.AntiqueWhite);
            g.SmoothingMode = SmoothingMode.HighQuality;
            int k = 0;

            int xCenter = (int)(hstart + (-minZ / (maxZ - minZ)) * w);
            var penArx = new Pen(Color.Black, 1);

            foreach (var i in dict.Values)
            {
                if (i != 0)
                {
                    g.DrawRectangle(pen, k * step + wStart, yAxe - i, step, i);
                    g.FillRectangle(brush, k * step + wStart, yAxe - i, step, i);
                }
                ++k;
            }
            var font = new Font("Microsoft Sans Serif", 7F, FontStyle.Regular, GraphicsUnit.Point, 204);

            string maxXs = $"{(maxZ):#.#}", minXs = $"{(minZ):#.#}", maxYS = $"{(maxY / (len * 1d)):0.000}";

            g.DrawLine(penArx, xCenter - 2, hstart, xCenter + 2, hstart);
            g.DrawString(maxYS, font, brush, xCenter, 0);

            g.DrawLine(penArx, xCenter, 0, xCenter, height);
            g.DrawLine(penArx, 0, yAxe, width, yAxe);
            //черта слева по x
            g.DrawString(minXs, font, brush, wStart, yAxe);
            g.DrawLine(penArx, wStart, h + 8, wStart, h + 12);
            //черта справа по x
            g.DrawString(maxXs, font, brush, wStart + w - g.MeasureString(maxXs, font).Width, yAxe);
            g.DrawLine(penArx, w + hstart, h + 8, w + hstart, h + 12);

            g.DrawString("0", font, brush, xCenter, yAxe);

            return new DataHist
            {
                Image = bp,
                MinX = minZ,
                Heigh = yAxe,
                MaxX = maxZ,
                MaxY = (maxY / (len * 1f)),
                ScaleY = (h / (maxY / (len * 1d))) / 10,
                //ScaleY = ScaleY,
                ScaleX = ScaleX * step,
                CenterX = xCenter,
                Dict = dict
            };
        }

        public DataHist DrawHist2(int width, int height, byte step, float[] zVariables, Brush brush, Pen pen)
        {
            int h = height - 20, hstart = 10, yAxe = h + hstart;
            int w = width - 20;
            int wStart = 10;
            int len = zVariables.Length;

            float minZ = zVariables.Min(), maxZ = zVariables.Max();
            // получаем максимально возможное число z от 0
            float maxX = maxZ - minZ;
            // получаем число возможных столбцов
            int countColumns = w / step;
            // соотношение столбцов к максимально возможному числу
            float ScaleX = countColumns / maxX;

            var dict = new Dictionary<int, int>();
            for (int i = 0; i <= countColumns; i++) { dict.Add(i, 0); }
            for (int i = 0, m = zVariables.Length; i < m; i++) { float x = ((zVariables[i] - minZ) * ScaleX); ++dict[(int)x]; }
            int maxY = int.MinValue;
            var dv = dict.Values.ToArray();
            for (int i = 0; i <= countColumns; i++) { int x = dv[i]; if (x > maxY) { maxY = x; } }

            float scaleYforDict = (maxY * 1f) / (h * 1f);

            for (int i = 0; i <= countColumns; i++) { dict[i] = (int)(dict[i] / scaleYforDict); }

            Bitmap bp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bp);
            g.Clear(Color.AntiqueWhite);
            g.SmoothingMode = SmoothingMode.HighQuality;
            int k = 0;

            int xCenter = (int)(hstart + (-minZ / (maxZ - minZ)) * w);
            var penArx = new Pen(Color.Black, 1);

            foreach (var i in dict.Values)
            {
                if (i != 0)
                {
                    g.DrawRectangle(pen, k * step + wStart, yAxe - i, step, i);
                    g.FillRectangle(brush, k * step + wStart, yAxe - i, step, i);
                }
                ++k;
            }
            var font = new Font("Microsoft Sans Serif", 7F, FontStyle.Regular, GraphicsUnit.Point, 204);

            string maxXs = $"{(maxZ):#.#}", minXs = $"{(minZ):#.#}", maxYS = $"{(maxY / (len * 1d)):0.000}";

            g.DrawLine(penArx, xCenter - 2, hstart, xCenter + 2, hstart);
            g.DrawString(maxYS, font, brush, xCenter, 0);

            g.DrawLine(penArx, xCenter, 0, xCenter, height);
            g.DrawLine(penArx, 0, yAxe, width, yAxe);
            //черта слева по x
            g.DrawString(minXs, font, brush, wStart, yAxe);
            g.DrawLine(penArx, wStart, h + 8, wStart, h + 12);
            //черта справа по x
            g.DrawString(maxXs, font, brush, wStart + w - g.MeasureString(maxXs, font).Width, yAxe);
            g.DrawLine(penArx, w + hstart, h + 8, w + hstart, h + 12);

            g.DrawString("0", font, brush, xCenter, yAxe);

            return new DataHist
            {
                Image = bp,
                MinX = minZ,
                Heigh = yAxe,
                MaxX = maxZ,
                MaxY = (maxY / (len * 1f)),
                ScaleY = (h / (maxY / (len * 1d))),
                //ScaleY = ScaleY,
                ScaleX = ScaleX * step,
                CenterX = xCenter,
                Dict = dict
            };
        }
        public Image DrawFunc(DataHist hist, float[] xx, float[] yy, Pen pen, double correctX = 0, double correctY = 1)
        {
            Graphics g = Graphics.FromImage(hist.Image);
            g.SmoothingMode = SmoothingMode.HighQuality;
            double scX = hist.ScaleX, scY = hist.ScaleY;
            int xCenter = hist.CenterX, height = hist.Heigh;
            float oldx = (float)(xx[0] * scX + xCenter), oldy = (float)(height - yy[0] * scY);
            for (int i = 1, m = xx.Length; i < m; ++i)
            {
                float x = (float)(xx[i] * scX + xCenter + 10 + correctX), y = (float)(height - yy[i] * scY * correctY);
                g.DrawLine(pen, oldx, oldy, x, y);
                oldx = x;
                oldy = y;
            }
            return hist.Image;
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
        public Image DrawFunc(Bitmap Image, float[] xx, float[] yy, Pen pen, float correctX = 1, float correctY = 1)
        {
            Graphics g = Graphics.FromImage(Image);
            g.SmoothingMode = SmoothingMode.HighQuality;
            float h = Image.Width / 2f;
            int height = Image.Height;
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

        public Image Lab12()
        {
            // если картинка диаграммы существует
            string diagrammPath = Application.StartupPath + @"\diagramm12.jpg";
            var img = LoadDiagramm(diagrammPath);
            if (img != null) return img;

            int N = 10000000;
            string fileZ2 = Application.StartupPath + @"\ex1Normal.dat";
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
                    double z = 0;
                    for (int j = 0; j < k; j++) { z += API.TRAND.Normal(0, 1); }
                    z1[i] = (float)z;
                });
                SaveData(z1, fileZ2);
            }
            ContextData.XLab12 = z1;
            var hist1 = DrawHist2(1000, 600, 2, z1, new SolidBrush(Color.DarkSlateBlue), new Pen(Color.Black, 0.001f));
            //hist1.Image.Save(diagrammPath);
            var x1 = GetMass(-98, 98, 1f);
            var y1 = API.NormV(x1, 0.5f / 2, 2000);
            ContextData.Lab11 = new Tuple<float[], float[]>(x1, y1);
            ContextData.Lab11Img = hist1.Image;
            return HelperFunks.DrawFuncScaleMaxFunkY(hist1, x1, y1, new Pen(Color.Blue, 2f), 2d);
            //return DrawFunc(hist1, x1, y1, new Pen(Color.Blue, 2f));
        }
        public Image Lab13()
        {
            // если картинка диаграммы существует
            string diagrammPath = Application.StartupPath + @"\diagramm13.jpg";
            var img = LoadDiagramm(diagrammPath);
            if (img != null) return img;

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
            var hist1 = DrawHist(1000, 600, 2, z1, new SolidBrush(Color.BlueViolet), new Pen(Color.Black, 0.001f));
            hist1.Image.Save(diagrammPath);
            return hist1.Image;
        }
        public Tuple<Image, Image> Lab11()
        {
            string fileZ1 = Application.StartupPath + @"\generateZ1.dat";
            string fileZ2 = Application.StartupPath + @"\generateZ2.dat";
            int n = 10000000;

            Tuple<float[], float[]> z = new Tuple<float[], float[]>(LoadDataInMemory(fileZ1), LoadDataInMemory(fileZ2));

            if (z.Item1 == null || z.Item2 == null)
            {
                z = API.GetZ1Z2(n, ContextData.L0, ContextData.L1);
                SaveData(z.Item1, fileZ1);
                SaveData(z.Item2, fileZ2);
            }
            ContextData.Lab11 = z;
            var img1 = DrawHist(1000, 600, 2, Trim(z.Item1, -30, 30), new SolidBrush(Color.Crimson), new Pen(Color.Black, 0.001f));
            var img2 = DrawHist(1000, 600, 2, Trim(z.Item2, -10, 10), new SolidBrush(Color.DarkOrange), new Pen(Color.Black, 0.001f));


            var x1 = GetMass(-30, 0, 0.2f);
            var y1 = API.GetFunkPL0(x1, ContextData.L0, img1.MaxY);
            var x2 = GetMass(-2.01f, 2.01f, 0.02f);
            var y2 = API.GetFunkPL1(x2, ContextData.L1, img2.MaxY);
            ContextData.Lab11 = new Tuple<float[], float[]>(x2, y2);
            ContextData.Lab11Img = img2.Image;
            //var img11 = DrawFunc(img1.Image, x1, y1, new Pen(Color.Blue, 2f), (float)img1.ScaleX, (float)img1.ScaleY);
            var img22 = DrawFunc(img2.Image, x2, y2, new Pen(Color.Blue, 2f), 50, 2090);
            return new Tuple<Image, Image>(DrawFunc(img1, x1, y1, new Pen(Color.Blue, 2f)), img22);
        }

        public Image LoadDiagramm(string path)
        {
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open)) return Image.FromStream(fs);
            }
            return null;
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
        public static Bitmap Lab11Img;
    }

    public struct DataHist
    {
        public Bitmap Image;
        public float MinX, MaxX;
        public float MaxY;
        public double ScaleX;
        public double ScaleY;
        public int CenterX;
        public int Heigh;
        public Dictionary<int, int> Dict;
    }
}
