using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ComplexDrawer
{
    public class GraphBox
    {
        public PictureBox pictureBox;
        public int d = 50;
        public Bitmap bitmap;
        public Graphics graphics;
        public bool mousedown = false;
        public Point mouseDownLoc;
        public Point centre;
        public GraphBox(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            bitmap = new Bitmap(pictureBox.Size.Width, pictureBox.Size.Height);
            graphics = Graphics.FromImage(bitmap);
            pictureBox.SizeChanged += PictureBox_SizeChanged;
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;
            pictureBox.MouseWheel += PictureBox_MouseWheel;
            centre = new Point(bitmap.Width / 2, bitmap.Height / 2);
            MkMap();
            pictureBox.Image = bitmap;
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if(e.Delta>0)
                d = (int)(e.Delta / 100.0 * d);
            else
            {
                int di= (int)(-e.Delta / 144.0 * d);
                if (di != 0) d = (int)(-e.Delta / 144.0 * d);
                else return;
            }
                
            MkMap();
            Update();
            
            
        }
        public static int RangeLimit(int a, int x, int b)
        {
            if (x < a) return a;
            if (x > b) return b;
            return x;
        }
        public void MkMap()
        {
            graphics.Clear(Color.Black);
            if (MainForm.engild)
            {
                int di = d;
                while (di >= 100) di /= 2;
                while (di <= 25) di *= 2;
                for (int x = centre.X % di; x < bitmap.Width; x += di)
                    for (int y = centre.Y % di; y < bitmap.Height; y += di)
                    {
                        graphics.DrawLine(Pens.Gray, new Point(x, 0), new Point(x, bitmap.Height));
                        graphics.DrawLine(Pens.Gray, new Point(0, y), new Point(bitmap.Width, y));
                        graphics.DrawString(Math.Round(((x - centre.X) / (float)d), 2, MidpointRounding.AwayFromZero).ToString(), SystemFonts.DefaultFont, Brushes.Gainsboro, new Point(x, RangeLimit(0, centre.Y, bitmap.Height - 14)));
                        string sa = Math.Round(((centre.Y - y) / (float)d), 2, MidpointRounding.AwayFromZero).ToString();
                        graphics.DrawString(sa, SystemFonts.DefaultFont, Brushes.Gainsboro, new Point(RangeLimit(0, centre.X, bitmap.Width - sa.Length * 7 - 1), y));
                    }
                graphics.DrawLine(Pens.Silver, new Point(centre.X, 0), new Point(centre.X, bitmap.Height));
                graphics.DrawLine(Pens.Silver, new Point(0, centre.Y), new Point(bitmap.Width, centre.Y));
            }
            

        }
        public PointF ToRealPoint(Point point) => new PointF((point.X - centre.X) / (float)d, (centre.Y - point.Y) / (float)d);
        public Point ToMapPoint(PointF point) => new Point((int)(point.X * d) + centre.X,centre.Y - (int)(point.Y * d));
        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
            if (e.Button == MouseButtons.Right)
            {
                centre = new Point(centre.X + e.X-mouseDownLoc.X, centre.Y + e.Y - mouseDownLoc.Y);
                Bitmap od = new Bitmap(bitmap);
                MkMap();
                graphics.DrawImage(od, e.X - mouseDownLoc.X, e.Y - mouseDownLoc.Y);
                Update();
                return;
            }
            if (e.Button == MouseButtons.Middle)
            {
                MkMap();
                Update();
            }
        }
        public void DrawPoint(PointF Rpoint,Color color)
        {
            DrawPoint(ToMapPoint(Rpoint), color);
        }
        public void DrawPoint(Point Mpoint,Color color)
        {
            bitmap.SetPixel(Mpoint.X, Mpoint.Y, color);
        }
        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (MainForm.penw == 1)
                    {
                        DrawAC(e.Location);
                        MainForm.mainForm.UpdateBox();
                    }
                    else
                    {
                        bool er = false;
                        for (int x = 0; x < MainForm.penw; x++)
                            for (int y = 0; y < MainForm.penw; y++)
                            {
                                try
                                {
                                    Point mPoint = new Point(e.X + x, e.Y + y);
                                    List<PointF> r = MainForm.mainForm.Func(ToRealPoint(mPoint));
                                    foreach (PointF t in r)
                                    {
                                        MainForm.mainForm.outbox.DrawPoint(t, MainForm.mainForm.color);
                                    }
                                    MainForm.mainForm.outbox.Update();
                                    DrawPoint(mPoint, MainForm.mainForm.color);
                                    Update();
                                    er = true;
                                }
                                catch
                                {
                                    continue;

                                }
                            }
                        if(er)
                            MainForm.mainForm.ColorGo();
                    }
                    return;
                }
                
            }
            MainForm.mainForm.label2.Text = ToRealPoint(e.Location).ToString();
        }
        public void DrawAC(Point mPoint)
        {
            try
            {
                List<PointF> r = MainForm.mainForm.Func(ToRealPoint(mPoint));
                foreach (PointF x in r)
                {
                    MainForm.mainForm.outbox.DrawPoint(x, MainForm.mainForm.color);
                }
                DrawPoint(mPoint, MainForm.mainForm.color);
            }
            catch
            {
                return;
            }
            
            MainForm.mainForm.ColorGo();
        }

        public void DrawAC(PointF rPoint)
        {
            try
            {
                List<PointF> r = MainForm.mainForm.Func(rPoint);
                foreach (PointF x in r)
                {
                    MainForm.mainForm.outbox.DrawPoint(x, MainForm.mainForm.color);
                }
                DrawPoint(rPoint, MainForm.mainForm.color);
            }
            catch
            {
                return;
            }
            MainForm.mainForm.ColorGo();
        }
        public void Update()
        {
            pictureBox.Image = bitmap;
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            mouseDownLoc = e.Location;
        }

        private void PictureBox_SizeChanged(object sender, EventArgs e)
        {
            if (pictureBox.Size.Width == 0 || pictureBox.Size.Height == 0) return;
            bitmap = new Bitmap(pictureBox.Size.Width, pictureBox.Size.Height);
            graphics = Graphics.FromImage(bitmap);
            MkMap();
            Update();
        }
    }
}
