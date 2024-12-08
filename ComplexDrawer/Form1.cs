using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static TRsLib.MathEX;
using System.IO;

namespace ComplexDrawer
{
    public partial class MainForm : Form
    {
        public GraphBox inbox;
        public GraphBox outbox;
        public static MainForm mainForm;
        public CNumF cNumF;
        public Color color;
        int[] rgb = new int[3] { 255, 0, 0 };
        int rgbi = 1,rgbs=1;
        public static int penw = 1;
        public static bool engild = true;
        public MainForm()
        {
            InitializeComponent();
            progressBar1.Hide();
            BackColor = Color.Black;
            outbox = new GraphBox(pictureBox1);
            inbox = new GraphBox(pictureBox2);
            mainForm = this;
            color = Color.FromArgb(rgb[0],rgb[1],rgb[2]);
            
        }
        public void InstallMod()
        {
            cNumF = null;
        }
        public void ColorGo()
        {
            rgb[rgbi] += rgbs;
            if (rgb[rgbi] == 255||rgb[rgbi]==0)
            {
                rgbi = 2 - (3-rgbi) % 3;
                rgbs = -rgbs;
            }
            color= Color.FromArgb(rgb[0], rgb[1], rgb[2]);
        }
        public List<PointF> Func(PointF point)
        {
            if (Equals(cNumF, null)) return new List<PointF>() { point };
            CNum z = new CNum(point.X, point.Y);
            List<CNum> y = fc(z);
            List<PointF> r = new List<PointF>();
            foreach (CNum x in y)
                r.Add(new PointF((float)x.Re, (float)x.Im));
            return r;
        }
        public List<CNum> fc(CNum z)
        {
            return cNumF.Compute(new Dictionary<string, CNum>() {["z"]=z });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            double m = (double)numericUpDown1.Value;
            if (checkBox2.Checked)
            {
                double l = Convert.ToDouble(textBox10.Text), rl = Convert.ToDouble(textBox9.Text);
                Function fs = Function.ToFunctionWP(textBox4.Text),afs = Function.ToFunctionWP(textBox2.Text),bs= Function.ToFunctionWP(textBox3.Text);
                fs.Simpilify();
                Dictionary<string, double> dc = new Dictionary<string, double>();
                progressBar1.Maximum = (int)((rl - l) / m);
                progressBar1.Value = 0;
                progressBar1.Show();
                for (; l <= rl; l += m)
                {
                    dc["k"] = l;
                    MkCyc(afs.Compute(dc), bs.Compute(dc), fs.Compute(dc), m,false);
                    progressBar1.PerformStep();
                }
                progressBar1.Hide();
                return;
            }
            double a = Convert.ToDouble(textBox2.Text), b = Convert.ToDouble(textBox3.Text);
            double r = Convert.ToDouble(textBox4.Text);
            MkCyc(a, b, r, m);
        }
        public void UpdateBox()
        {
            outbox.Update();
            inbox.Update();
        }
        private void MkCyc(double a, double b, double r, double m,bool pg=true)
        {
            if (pg)
            {
                progressBar1.Maximum = (int)(Math.PI * 2 / m);
                progressBar1.Value = 0;
                progressBar1.Show();
            }
            
            for (double t = 0; t < Math.PI * 2; t += m)
            {
                inbox.DrawAC(new PointF((float)(a + r * Math.Cos(t)), (float)(b + r * Math.Sin(t))));
                if(pg)
                progressBar1.PerformStep();
            }
            if(pg)
            progressBar1.Hide();
            UpdateBox();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
                try
                {
                    cNumF = CNumF.ToCNumWP(textBox1.Text);
                    cNumF.Simpilify();
                button1.Enabled = false;
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.Message,"ERROR",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }
        void Save(Image image)
        {
            SaveFileDialog s = new SaveFileDialog
            {
                Filter = ".bmp|*.bmp|.png|*.png|.jpg|*.jpg"
            };
            if (s.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image.Save(s.FileName);
                    MessageBox.Show("OK");
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Save(inbox.bitmap);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Save(outbox.bitmap);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if( MessageBox.Show("+:加;-:减;*：乘;/:除;^:乘方$:开方\n可用函数:sin,cos,tan,sinh,cosh,tanh,asin,acos,atan,ln,exp,abs,coj.\n实函数可用:floor,ceiling,log.\nUse dnspy to create mod.\nInstall Mod?",
                "Help/Install Mod?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                InstallMod();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            inbox.MkMap();
            inbox.Update();
            outbox.MkMap();
            outbox.Update();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
          
            penw = (int)numericUpDown2.Value;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            engild = checkBox3.Checked;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            inbox.d = 50;
            outbox.d = 50;
            inbox.centre = new Point(inbox.bitmap.Width / 2, inbox.bitmap.Height / 2);
            outbox.centre = new Point(outbox.bitmap.Width / 2, outbox.bitmap.Height / 2);
            inbox.MkMap();
            outbox.MkMap();
            UpdateBox();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Function xt = Function.ToFunctionWP(textBox5.Text), yt = Function.ToFunctionWP(textBox6.Text);
            double left = Convert.ToDouble(textBox7.Text), right = Convert.ToDouble(textBox8.Text);
            xt.Simpilify();
            yt.Simpilify();
            Dictionary<string, double> dc = new Dictionary<string, double>();
            double m = (double)numericUpDown1.Value;
            if (checkBox2.Checked)
            {
                double l = Convert.ToDouble(textBox10.Text), rl = Convert.ToDouble(textBox9.Text);
                progressBar1.Maximum = (int)((rl - l) / m * (right - left) / m);
                progressBar1.Value = 0;
                progressBar1.Show();
                for (; l <= rl; l += m)
                {
                    for(double lt=left; lt <= right; lt += m)
                    {
                        dc["t"] = lt;
                        dc["k"] = l;
                        PointF pointF = new PointF((float)xt.Compute(dc), (float)yt.Compute(dc));
                        inbox.DrawAC(pointF);
                        progressBar1.PerformStep();
                    }
                }
                UpdateBox();
                progressBar1.Hide();
                return;
            }
            progressBar1.Maximum = (int)((right - left) / m);
            progressBar1.Value = 0;
            progressBar1.Show();
            for (; left <= right; left += m)
            {
                dc["t"] = left;
                PointF pointF = new PointF((float)xt.Compute(dc), (float)yt.Compute(dc));
                inbox.DrawAC(pointF);
                progressBar1.PerformStep();
            }
            UpdateBox();
            progressBar1.Hide();
        }
    }
}
