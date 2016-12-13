using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

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
            float oldx = (float)(xx[0] * scX + xCenter), oldy = (float)(height - yy[0] * scY);
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
            xx[0] = x1 - ((-y1) * rx) / ry;
            yy[0] = 0;
            xx[1] = x1;
            yy[1] = y1;
            xx[2] = x2;
            yy[2] = y2;
            xx[3] = x1 - ((img.Height - y1) * rx) / ry;
            yy[3] = (ry * (xx[3] - x1)) / rx;


            //int m = xx.Length;
            //for (int i = 0; i < m; ++i) { yy[i] = (ry * (xx[i] - x1)) / rx; }
            var g = Graphics.FromImage(img);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawLine(pen, xx[0], yy[0], xx[3], yy[3]);
            return null;
        }

        public static float[] GetMass(float min, float max, float step)
        {
            var list = new List<float>();
            for (float i = min; i < max; i += step) { list.Add(i); }
            return list.ToArray();
        }
    }
}
