using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab13
{

    public class Hist
    {

        #region publicParams

        public Font Font
        {
            get
            {
                return _font;
            }
            set
            {
                _isDrow = false; _font = value;
            }
        }
        public SolidBrush ForeBrush
        {
            get { return _foreBrush; }
            set { _isDrow = false; _foreBrush = value; }
        }
        public Brush Brush
        {
            get { return _brush; }
            set { _isDrow = false; _brush = value; }
        }
        public Pen Pen
        {
            get { return _pen; }
            set { _isDrow = false; _pen = value; }
        }
        public Pen PenAxes
        {
            get { return _penAxes; }
            set { _isDrow = false; _penAxes = value; }
        }
        public Padding Padding
        {
            get { return _padding; }
            set { _isDrow = false; _padding = value; }
        }


        private bool _isDrow = false;
        private Font _font = new Font("Microsoft Sans Serif", 7F, FontStyle.Regular, GraphicsUnit.Point, 204);
        private SolidBrush _foreBrush = new SolidBrush(Color.Black);
        private Pen _pen = new Pen(Color.Black, 0.0001f);
        private Pen _penAxes = new Pen(Color.Black, 1f) { EndCap = LineCap.ArrowAnchor };
        private Padding _padding = new Padding(10);
        private Brush _brush = new SolidBrush(Color.Green);

        private Graphics g;
        private Image _image = null;
        #endregion

        public Image Image
        {
            get
            {
                if (!_isDrow) { DrawHist(); }
                return _image;
            }
        }

        public int Height => _image.Height;
        public int Width => _image.Width;

        public Dictionary<int, int> Histogram { get; } = new Dictionary<int, int>();
        public byte Step { get; protected set; }
        public float[] TrimVariables { get; protected set; }
        public float[] SourceVariables { get; protected set; }
        public float MaxX { get; protected set; }
        public float MaxY { get; protected set; }
        public float MinX { get; protected set; }
        public float DiapozonX { get; protected set; }
        public float ScaleX { get; protected set; }
        public float ScaleY { get; protected set; }
        public int CenterX { get; protected set; }
        public int TrimXMin { get; protected set; }
        public int TrimXMax { get; protected set; }

        public Hist(float[] variables, int width, int height, byte step, int trimXmin, int trimXmax)
        {
            Init(width, height, step, variables, trimXmin, trimXmax);
        }
        public void Init(int width, int height, byte step = 0, float[] variables = null, int trimXmin = 0, int trimXmax = 0)
        {
            if (variables != null)
            {
                SourceVariables = variables;
                TrimVariables = variables;
                if (trimXmin != trimXmax)
                {
                    TrimXMax = trimXmax;
                    TrimXMin = trimXmin;
                    TrimVariables = Trim(variables, trimXmin, trimXmax);
                }
                MaxX = TrimVariables.Max();
                MinX = TrimVariables.Min();
                DiapozonX = MaxX - MinX;
            }
            if (Step > 0) Step = step;
            Step = step;
            _image = new Bitmap(width, height);
            g = Graphics.FromImage(_image);
            g.Clear(Color.AntiqueWhite);
            g.SmoothingMode = SmoothingMode.HighQuality;
            Histogram.Clear();
            // calculate

            int h = height - 20, hstart = 10;
            int w = width - 20;
            int wStart = 10;

            // получаем число возможных столбцов
            int countColumns = w / Step;
            // соотношение столбцов к максимально возможному числу
            ScaleX = countColumns / DiapozonX;
            //fill Dict keys
            for (int i = 0; i <= countColumns; i++) { Histogram.Add(i, 0); }
            for (int i = 0, m = TrimVariables.Length; i < m; i++) { float x = ((TrimVariables[i] - MinX) * ScaleX); ++Histogram[(int)x]; }

            MaxY = Histogram.Values.Max();
            ScaleY = MaxY / h;
            CenterX = (int)(hstart + (-MinX / DiapozonX) * w);
        }
        public void DrawHist()
        {
            _isDrow = true;
            int width = _image.Width, height = _image.Height;
            int h = height - _padding.Top - _padding.Bottom, yAxe = h + _padding.Left;
            int w = width - _padding.Left - _padding.Right;

            var dv = Histogram.Values.ToArray();
            for (int i = 0, m = dv.Length; i < m; ++i)
            {
                if (dv[i] == 0) continue;
                var y = dv[i] / ScaleY;
                g.DrawRectangle(_pen, i * Step + _padding.Left, yAxe - y, Step, y);
                g.FillRectangle(_brush, i * Step + _padding.Left, yAxe - y, Step, y);
            }

            string maxXs = $"{(MaxX):#.#}", minXs = $"{(MinX):#.#}", maxYS = $"{(MaxY / (TrimVariables.Length * 1d)):0.000}";

            g.DrawLine(_penAxes, CenterX - 2, _padding.Top, CenterX + 2, _padding.Top);
            g.DrawString(maxYS, _font, _foreBrush, CenterX, 0);

            g.DrawLine(_penAxes, CenterX, 0, CenterX, height);
            g.DrawLine(_pen, 0, yAxe, width, yAxe);
            //черта слева по x
            g.DrawString(minXs, _font, _foreBrush, _padding.Left, yAxe);
            g.DrawLine(_pen, _padding.Left, h + 8, _padding.Left, h + 12);
            //черта справа по x
            g.DrawString(maxXs, _font, _foreBrush, _padding.Left + w - g.MeasureString(maxXs, _font).Width, yAxe);
            g.DrawLine(_pen, w + _padding.Left, h + 8, w + _padding.Left, h + 12);

            g.DrawString("0", _font, _foreBrush, CenterX, yAxe);
        }
        public delegate void ChangeScale(
            ref double defaultScaleX,
            ref double defaultScaleY, Hist hist, float maxY, ref int moveX, ref int moveY);
        public Image DrawFuncScaleMaxFunkY(float[] xx, float[] yy, Pen pen, ChangeScale action = null)
        {
            DrawHist();
            var maxY = yy.Max();
            double scalefunkY = (_image.Height - _padding.Top - _padding.Bottom) / maxY;
            double sc = ScaleX * Step;
            int moveX = 0, moveY = 0;
            action?.Invoke(ref sc, ref scalefunkY, this, maxY, ref moveX, ref moveY);
            float h = _image.Height - _padding.Bottom;

            float oldx = (float)(xx[0] * sc + CenterX), oldy = (float)(h - yy[0] * scalefunkY);
            g.DrawString(maxY.ToString(), _font, new SolidBrush(Color.Black), new PointF());
            for (int i = 1, m = xx.Length; i < m; ++i)
            {
                float x = (float)(xx[i] * sc + CenterX + moveX), y = (float)(h - yy[i] * scalefunkY - moveY);
                g.DrawLine(pen, oldx, oldy, x, y);
                oldx = x;
                oldy = y;
            }
            return _image;
        }

        public static float[] Trim(float[] zV, int ogrXl, int ogrXr)
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
    }
}
