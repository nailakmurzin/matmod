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
            var imgs = Lab11();
            pb1.Image = imgs.Item1;
            pb2.Image = imgs.Item2;

            pbEx2.Image = Lab13();
            pbEx2.Image.Save(diagrammPath);
            pbEx2.Refresh();
        }
        public Bitmap DrawHist(int width, int height, byte step, float[] zVariables, Brush brush, Pen pen)
        {
            int h = height - 20, hstart = 10, hh = h + hstart;
            int w = width - 20;
            int wStart = 10;
            var dict = new Dictionary<int, int>();
            int len = zVariables.Length;
            float min = float.MaxValue, max = float.MinValue;
            for (int i = 0; i < len; ++i)
            {
                float z = zVariables[i];
                if (min > z) min = z;
                if (max < z) max = z;
            }
            float maxX = max - min;
            int count = w / step;
            float kk = count / maxX;
            for (int i = 0; i <= count; i++) { dict.Add(i, 0); }
            for (int i = 0, m = zVariables.Length; i < m; i++)
            {
                float x = ((zVariables[i] - min) * kk); // от 0 до 500
                ++dict[(int)x];
            }
            int maxY = int.MinValue;
            var dv = dict.Values.ToArray();
            for (int i = 0; i <= count; i++)
            {
                int x = dv[i];
                if (x > maxY)
                {
                    maxY = x;
                }
            }

            float kkk = maxY * 1f / h * 1f;

            for (int i = 0; i <= count; i++)
            {
                dict[i] = (int)(dict[i] / kkk);
            }

            Bitmap bp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bp);
            g.Clear(Color.AntiqueWhite);
            g.SmoothingMode = SmoothingMode.HighQuality;
            int k = 0;

            int xCenter = 10 + (int)((-min / (max - min)) * w);
            var penArx = new Pen(Color.Black, 1);

            foreach (var i in dict.Values)
            {
                if (i != 0)
                {
                    g.DrawRectangle(pen, k * step + wStart, hh - i, step, i);
                    g.FillRectangle(brush, k * step + wStart, hh - i, step, i);
                }
                ++k;
            }
            var font = new Font("Microsoft Sans Serif", 7F, FontStyle.Regular, GraphicsUnit.Point, 204);


            string maxXs = $"{(max):#.#}", minXs = $"{(min):#.#}";


            g.DrawLine(penArx, xCenter - 2, hstart, xCenter + 2, hstart);
            g.DrawString($"{(maxY / (len * 1d)):0.000}", font, brush, xCenter, 0);
            g.DrawLine(penArx, xCenter, hstart, xCenter, hh);
            g.DrawLine(penArx, hstart, hh, w + hstart, hh);

            g.DrawString(minXs, font, brush, wStart, hh);
            g.DrawLine(penArx, wStart, h + 8, wStart, h + 12);

            g.DrawString(maxXs, font, brush, wStart + w - g.MeasureString(maxXs, font).Width, hh);
            g.DrawLine(penArx, w + hstart, h + 8, w + hstart, h + 12);
            g.DrawString("0", font, brush, xCenter, hh);
            return bp;
        }

        public float[] Trim(float[] zV, int ogrXL, int ogrXR)
        {
            float[] temp = new float[zV.Length];
            int len = 0;
            for (int i = 0, m = zV.Length; i < m; ++i)
            {
                float z = zV[i];
                if (z < ogrXR && z > ogrXL) { temp[len++] = z; }
            }
            float[] zVariables = new float[len];
            for (int i = 0, m = len; i < m; ++i) { zVariables[i] = temp[i]; }
            return zVariables;
        }

        
        string diagrammPath = Application.StartupPath + @"\diagramm13.jpg";
        public Image Lab13()
        {
            if (File.Exists(diagrammPath))
            {
                using (FileStream fs = new FileStream(diagrammPath, FileMode.Open))
                {
                    return Image.FromStream(fs);
                }
            }

            string fileZ2 = Application.StartupPath + @"\ex2Z2.dat";
            int N = 10000000;
            double l0 = 1 - 1 / (1 + 2);
            double l1 = l0 + 1;
            float[] z1;

            if (File.Exists(fileZ2))
            {
                //var lines = File.ReadAllLines(filePath);
                //N = lines.Length;
                //z1 = new float[N];
                //for (int i = 0; i < N; ++i) { z1[i] = float.Parse(lines[i]); }
                using (BinaryReader reader = new BinaryReader(File.Open(fileZ2, FileMode.Open)))
                {
                    z1 = new float[reader.BaseStream.Length / 4];
                    int j = 0;
                    while (reader.BaseStream.Position != reader.BaseStream.Length) { z1[j++] = reader.ReadSingle(); }
                }
            }
            else
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
                    float z = API.GetZ2Sample(k, l1);
                    z1[i] = z;
                });
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileZ2, FileMode.OpenOrCreate)))
                {
                    for (int i = 0, m = z1.Length; i < m; ++i) { writer.Write(z1[i]); }
                }
                //var text = new StringBuilder();
                //for (int i = 0; i < N; ++i) { text.AppendLine(z1[i].ToString()); }
                //File.WriteAllText(Application.StartupPath + @"\WriteText2.txt", text.ToString());
            }
            return DrawHist(1000, 600, 2, z1, new SolidBrush(Color.BlueViolet), new Pen(Color.Black, 0.001f));
        }
        public Tuple<Image, Image> Lab11()
        {
            string fileZ1 = Application.StartupPath + @"\generateZ1.dat";
            string fileZ2 = Application.StartupPath + @"\generateZ2.dat";
            int n = 10000000;
            int N = 2;
            double l0 = 1 - 1 / (1 + N);
            double l1 = l0 + 1;
            Tuple<float[], float[]> z;
            if (File.Exists(fileZ1) && File.Exists(fileZ2))
            {
                float[] z1, z2;
                using (BinaryReader reader = new BinaryReader(File.Open(fileZ1, FileMode.Open)))
                {
                    z1 = new float[reader.BaseStream.Length / 4];
                    z2 = new float[z1.Length];
                    int j = 0;
                    while (reader.BaseStream.Position != reader.BaseStream.Length) { z1[j++] = reader.ReadSingle(); }
                }
                using (BinaryReader reader = new BinaryReader(File.Open(fileZ2, FileMode.Open)))
                {
                    int j = 0;
                    while (reader.BaseStream.Position != reader.BaseStream.Length) { z2[j++] = reader.ReadSingle(); }
                }
                z = new Tuple<float[], float[]>(z1, z2);
            }
            else
            {
                z = API.GetZ1Z2(n, l0, l1);
                float[] z1 = z.Item1, z2 = z.Item2;
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileZ1, FileMode.OpenOrCreate)))
                {
                    for (int i = 0, m = z1.Length; i < m; ++i) { writer.Write(z1[i]); }
                }
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileZ2, FileMode.OpenOrCreate)))
                {
                    for (int i = 0, m = z2.Length; i < m; ++i) { writer.Write(z2[i]); }
                }
            }
            var img1 = DrawHist(1000, 600, 2, Trim(z.Item1, -30, 30), new SolidBrush(Color.DarkRed), new Pen(Color.Black, 0.001f));
            var img2 = DrawHist(1000, 600, 2, z.Item2, new SolidBrush(Color.DarkOrange), new Pen(Color.Black, 0.001f));
            return new Tuple<Image, Image>(img1, img2);
        }
    }
}
