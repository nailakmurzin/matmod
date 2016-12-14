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
        public static Image DrawFuncScaleMaxFunkY(DataHist hist, float[] xx, float[] yy, Pen pen, double correctX = 1)
        {
            var maxY = yy.Max();
            double scalefunkY = hist.MaxY / maxY;

            Graphics g = Graphics.FromImage(hist.Image);
            g.SmoothingMode = SmoothingMode.HighQuality;
            double scX = hist.ScaleX, scY = hist.ScaleY;
            int xCenter = hist.CenterX, height = hist.Heigh;
            var font = new Font("Microsoft Sans Serif", 7F, FontStyle.Regular, GraphicsUnit.Point, 204);

            float oldx = (float)(xx[0] * scX + xCenter), oldy = (float)(height - yy[0] * scY);
            g.DrawString(maxY.ToString(), font, new SolidBrush(Color.Black), new PointF());
            for (int i = 1, m = xx.Length; i < m; ++i)
            {
                float x = (float)(xx[i] * scX * correctX + xCenter), y = (float)(height - yy[i] * scY * scalefunkY);
                g.DrawLine(pen, oldx, oldy, x, y);
                oldx = x;
                oldy = y;
            }
            return hist.Image;
        }

        public static float FindMax(float[] yy)
        {
            float maxY = float.MinValue;
            for (int i = 0, m = yy.Length; i < m; ++i) { if (maxY < yy[i]) { maxY = yy[i]; } }
            return maxY;
        }
        public static float FindMin(float[] yy)
        {
            float minY = float.MaxValue;
            for (int i = 0, m = yy.Length; i < m; ++i) { if (minY > yy[i]) { minY = yy[i]; } }
            return minY;
        }

        public static int GetIndexRabitMinimase(int[] y)
        {
            int oldY = y[0], max = int.MinValue, index = 0;
            for (int i = 1, m = y.Length; i < m; ++i)
            {
                int r = oldY - y[i];
                if (r > max)
                {
                    max = r;
                    index = i;
                }
                oldY = y[i];
            }
            return index;
        }

        public static Image DrawFunkFromTwoPoints(Image img, float x1, float y1, float x2, float y2, Pen pen)
        {
            var xx = new float[4];
            var yy = new float[4];
            var rx = (x2 - x1);
            var ry = (y2 - y1);
            xx[0] = x1 + ((-y1) * rx) / ry;
            yy[0] = 0;
            xx[1] = x1;
            yy[1] = y1;
            xx[2] = x2;
            yy[2] = y2;
            xx[3] = x1 + ((img.Height - y1) * rx) / ry;
            yy[3] = (ry * (xx[3] - x1)) / rx;


            //int m = xx.Length;
            //for (int i = 0; i < m; ++i) { yy[i] = (ry * (xx[i] - x1)) / rx; }

            var g = Graphics.FromImage(img);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawLine(pen, xx[0], yy[0], xx[3], yy[3]);
            return img;
        }

        public static Image DrawFunkFromPoints(Image img, float[] xx, float[] yy, float moveX, float scaleY, float moveY, Pen pen)
        {
            int m = xx.Length;
            float[] x = new float[m], y = new float[m];
            for (int i = 0; i < m; ++i)
            {
                x[i] = xx[i] + moveX;
                y[i] = yy[i] * scaleY + moveY;
            }
            var g = Graphics.FromImage(img);
            g.SmoothingMode = SmoothingMode.HighQuality;
            for (int i = 1; i < m; ++i) { g.DrawLine(pen, x[i - 1], y[i - 1], x[i], y[i]); }
            return img;
        }
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

        public static float[] GetMass(float min, float max, float step)
        {
            var list = new List<float>();
            for (float i = min; i < max; i += step) { list.Add(i); }
            return list.ToArray();
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
        public static Image RandomPointsDraw(Image img, float[] x, float[] y, int beginX, int endX, Pen pen, float scX, float scY, int h, float newA)
        {
            float summX = 0, summY = 0, summXY = 0, summXX = 0;

            float x1 = x[beginX], x2 = x[endX];
            float y1 = y[beginX], y2 = y[endX];

            var rx = (x2 - x1);
            var ry = (y2 - y1);
            var a = ry / rx;
            var b = -a * x1 + y1;
            var font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);

            var g = Graphics.FromImage(img);
            g.SmoothingMode = SmoothingMode.HighQuality;
            //line1
            g.DrawLine(pen, x1 * scX, h - (a * x1 + b) * scY, x2 * scX, h - (a * x2 + b) * scY);
            g.DrawString(a.ToString(), font, new SolidBrush(pen.Color), x[beginX] * scX, 10);
            //lene2
            int k = endX - beginX;
            var newB = ((summY - newA * summX) / k);
            g.DrawLine(new Pen(Color.DarkViolet, 2f), x[beginX] * scX, h - (newA * x[beginX] + newB) * scY, x[endX] * scX, h - (newA * x[endX] + newB) * scY);
            g.DrawString(newA.ToString(), font, new SolidBrush(Color.DarkViolet), x[beginX] * scX, 30);

            return img;
        }
    }
}
