using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml.Schema;

namespace Lab13
{
    static class HelperFunks
    {
        public static Image DrawFunkFromPoints(this Tuple<float[], float[]> xy, int w, int h, Pen pen, out float scX, out float scY)
        {
            Bitmap img = new Bitmap(w, h);
            var xx = xy.Item1;
            var yy = xy.Item2;
            int m = xx.Length;

            var maxY = yy.Max();
            var maxX = xx[m - 1];
            scY = h / maxY;
            scX = w / maxX;


            float[] x = new float[m], y = new float[m];
            for (int i = 0; i < m; ++i)
            {
                x[i] = xx[i] * scX;
                y[i] = h - yy[i] * scY;
            }
            var g = Graphics.FromImage(img);
            g.Clear(Color.AliceBlue);
            g.SmoothingMode = SmoothingMode.HighQuality;

            for (int i = 1; i < m; ++i) { g.DrawLine(pen, x[i - 1], y[i - 1], x[i], y[i]); }
            return img;
        }
        public static Image DrawFunkFromPoints(this Tuple<float[], float[]> xy, int w, int h, Pen pen)
        {
            Bitmap img = new Bitmap(w, h);
            var xx = xy.Item1;
            var yy = xy.Item2;
            int m = xx.Length;

            float maxY = yy.Max(), minY = yy.Min();
            float maxX = xx.Max(), minX = xx.Min();
            float cx = maxX - minX, cy = maxY - minY;
            float scY = h / cy;
            float scX = w / cx;

            float[] x = new float[m], y = new float[m];
            for (int i = 0; i < m; ++i)
            {
                x[i] = (xx[i] - minX) * scX;
                y[i] = h + minY * scY - yy[i] * scY;
            }
            var g = Graphics.FromImage(img);
            g.Clear(Color.AliceBlue);
            g.SmoothingMode = SmoothingMode.HighQuality;
            var font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            g.DrawString(minX.ToString(), font, new SolidBrush(Color.Black), 0, h - 30);
            g.DrawString(maxX.ToString(), font, new SolidBrush(Color.Black), w - 50, h - 30);

            g.DrawString(minY.ToString(), font, new SolidBrush(Color.Black), w / 2f, h - 30);
            g.DrawString(maxY.ToString(), font, new SolidBrush(Color.Black), w / 2f, 0);

            for (int i = 1; i < m; ++i) { g.DrawLine(pen, x[i - 1], y[i - 1], x[i], y[i]); }
            return img;
        }
        public static Image MNKDraw(Image img, float[] x, float[] y, int beginX, int endX, Pen pen, float scX, float scY, int h, float newA)
        {
            float summX = 0, summY = 0, summXY = 0, summXX = 0;



            int k = 0;
            for (int i = beginX; i < endX; ++i)
            {
                summX += x[i];
                summY += y[i];
                summXY += x[i] * y[i];
                summXX += x[i] * x[i];
                k++;
            }
            var a = (k * summXY - summX * summY) / (k * summXX - summX * summX);
            var b = (summY - a * summX) / k;
            var font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);

            var g = Graphics.FromImage(img);
            g.SmoothingMode = SmoothingMode.HighQuality;
            //line1
            g.DrawLine(pen, x[beginX] * scX, h - (a * x[beginX] + b) * scY, x[endX] * scX, h - (a * x[endX] + b) * scY);
            g.DrawString(a.ToString(), font, new SolidBrush(pen.Color), x[beginX] * scX, 10);
            //lene2
            var newB = ((summY - newA * summX) / k);
            g.DrawLine(new Pen(Color.DarkViolet, 2f), x[beginX] * scX, h - (newA * x[beginX] + newB) * scY, x[endX] * scX, h - (newA * x[endX] + newB) * scY);
            g.DrawString(newA.ToString(), font, new SolidBrush(Color.DarkViolet), x[beginX] * scX, 30);

            return img;
        }
    }
}
