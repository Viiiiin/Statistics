using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace homework_1
{
    public partial class Form2 : Form
    {
        Random random;
        private int server;
        private int attacker;
        private float probability;
        private int t;
        private float x_space;
        private float y_space;
        private int height;
        private int width;
        private Dictionary<float, int> result_t;
        private Dictionary<float, int> result_n;
        private const int START_X = 40;
        private const int START_Y = 100;

       public Form2(int server, int attacker, float probability,int t)
        {
           InitializeComponent();
           this.server = server;
           this.attacker = attacker;
           this.probability = probability;
           this.t = t;
           this.result_t = new Dictionary<float, int>();
           this.result_n = new Dictionary<float, int>();
           this.height = this.Height - 200;
           this.width = (this.Width) - 500;
           this.x_space = this.width / (float)this.server;
           this.y_space = this.height / (float)(this.server * 2);
           this.FormBorderStyle = FormBorderStyle.FixedDialog;
           this.MaximizeBox = false;
           this.random = new Random();
           this.Paint += new PaintEventHandler(Create_Graphic);
           this.Paint += new PaintEventHandler(Paint_Attack);
           this.Paint += new PaintEventHandler(Create_Graphic_result);
           this.Paint += new PaintEventHandler(Paint_Result);
           this.Paint += new PaintEventHandler(Create_Graphic_t);
           this.Paint += new PaintEventHandler(Paint_t);
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        // RELATIVE PART
        private void Paint_Attack(object sender, PaintEventArgs e)
        {

            if (this.attacker == 0 || this.server == 0)
            {
                return;
            }
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int jumps;
            float  relative_score = 0f;
            Color color;
            PointF current_point, next_point;
            Pen pen;

            for (int i = 0; i < this.attacker; i++)
            {
         
                jumps = 0;
                color = Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
                current_point = new PointF(START_X, this.height + START_Y);
                next_point = new PointF(START_X, this.height + START_Y);
                pen = new Pen(color, 3);



                for (int j = 0; j <this.server; j++)
                {
                    if (Attack())
                    {
                        jumps++;
                    }
                    relative_score = jumps / (float)(j + 1);
                    
                    next_point = new PointF(current_point.X, this.height + START_Y - (relative_score * this.height)); //Mi posiziono al punto Y calcolato
                    g.DrawLine(pen, current_point, next_point);
                    current_point = next_point;
                    next_point = new PointF(current_point.X + x_space, current_point.Y);
                    g.DrawLine(pen, current_point, next_point);
                    current_point = next_point;
                    if (j == t)
                    {
                        if (result_t.ContainsKey(relative_score))
                        {
                            result_t[relative_score] += 1;
                        }
                        else
                        {
                            result_t.Add(relative_score, 1);
                        }
                    }

                }


                if (result_n.ContainsKey(relative_score))
                {
                    result_n[relative_score] += 1;
                }
                else
                {
                    result_n.Add(relative_score, 1);
                }
            }
        }


        private Boolean Attack()
        {
            float randomNumber = (float)random.NextDouble();
            return this.probability >= randomNumber;
        }

        private void Create_Graphic(object sender, PaintEventArgs e)
        {
            if (this.attacker == 0 || this.server == 0)
            {
                return;
            }
            Graphics g = e.Graphics;

            Pen blackPen = new Pen(Color.Black, 2);
            Rectangle rect = new Rectangle(START_X, START_Y, this.width, this.height);
            g.DrawRectangle(blackPen, rect);
        }

        private void Paint_Result(object sender, PaintEventArgs e)
        {
            if (this.attacker == 0 || this.server == 0)
            {
                return;
            }

            int max = this.result_n.Values.Max();
            float space = ((float)this.width / 5) / max;
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black, 1);
            PointF current_point;
            current_point = new PointF(START_X + this.width, this.height + START_Y);
            Font font = new Font("Arial", 12);

            foreach (KeyValuePair<float, int> solution in this.result_n)
            {

               float fixY = this.height + START_Y - (solution.Key * this.height);
               RectangleF rect = new RectangleF(current_point.X, fixY, (solution.Value * space), this.y_space);

               g.FillRectangle(Brushes.Blue, rect);

               g.DrawRectangle(pen, Rectangle.Round(rect));

            }

            CalculateMeanAndDeviation(this.result_n.Keys.ToArray(), out float mean, out float dev);
            g.DrawString($"Time N - Mean: {mean} - Dev {dev}", font, Brushes.Black, 700, 10);


        }
        private void Paint_t(object sender, PaintEventArgs e)
        {
            if (this.attacker == 0 || this.server == 0)
            {
                return;
            }

            int max = this.result_t.Values.Max();
            float space = ((float)this.width / 5) / max;
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black, 1);
            PointF current_point;
            current_point = new PointF(START_X + t * this.x_space, this.height + START_Y);
            Font font = new Font("Arial", 12);

            foreach (KeyValuePair<float, int> solution in result_t)
            {
                float fixY = this.height + START_Y - (solution.Key * this.height);
                RectangleF rect = new RectangleF(current_point.X, fixY, (solution.Value * space), this.y_space);
                g.FillRectangle(Brushes.Blue, rect);

                g.DrawRectangle(pen, Rectangle.Round(rect));

            }

            CalculateMeanAndDeviation(this.result_t.Keys.ToArray(), out float mean, out float dev);
            g.DrawString($"Time t - Mean: {mean} - Dev {dev}", font, Brushes.Black, 700, 30);


        }
        private void Create_Graphic_result(object sender, PaintEventArgs e)
        {
            if (this.attacker == 0 || this.server == 0)
            {
                return;
            }
            Graphics g = e.Graphics;
            int max = this.result_n.Values.Max();
            float space = ((float)this.width / 5) / max;
            Pen blackPen = new Pen(Color.Black, 2);
            blackPen.DashStyle = DashStyle.Dash;

            Rectangle rect = new Rectangle(this.width + START_X, START_Y, this.width / 5, this.height);
            g.DrawRectangle(blackPen, rect);
            Font font = new Font("Arial", 5);
            Pen grayPen = new Pen(Color.Gray, 1);
            float labelOffsetY = (this.height) / this.attacker;
            float yPosition = this.height + START_Y;
            for (int i = 0; i <= max; i++)
            {
                float xPosition = this.width + START_X + (i * space);
                g.DrawLine(grayPen, xPosition, yPosition, xPosition, yPosition + 10);
                if (i % labelOffsetY == 0)
                {

                    g.DrawString($"{i}", font, Brushes.Black, xPosition, yPosition + 12);

                }

            }


        }

        private void Create_Graphic_t(object sender, PaintEventArgs e)
        {

            if (this.attacker == 0 || this.server == 0)
            {
                return;
            }
            Graphics g = e.Graphics;
            int max = this.result_t.Values.Max();
            float space = ((float)this.width / 5) / max;
            Pen blackPen = new Pen(Color.Black, 2);
            blackPen.DashStyle = DashStyle.Dash;

            RectangleF rect = new RectangleF(this.t * this.x_space + START_X, START_Y, this.width / 5, this.height);
            g.DrawRectangle(blackPen, Rectangle.Round(rect));
            Font font = new Font("Arial", 5);
            Pen grayPen = new Pen(Color.Gray, 1);
            float labelOffsetY = (this.height) / this.attacker;
            float yPosition = this.height + START_Y;
            for (int i = 0; i <= max; i++)
            {
                float xPosition = this.t * this.x_space + START_X + (i * space);
                g.DrawLine(grayPen, xPosition, yPosition, xPosition, yPosition + 10);
                if (i % labelOffsetY == 0)
                {

                    g.DrawString($"{i}", font, Brushes.Black, xPosition, yPosition + 12);

                }

            }
        }


        public void CalculateMeanAndDeviation(float[] result, out float mean, out float dev)
        {
            float delta;
            int n = 1;
            mean = 0;
            dev = 0;

            for (int i = 0; i < result.Length; i++)
            {


               delta = result[i] - mean;
               mean += delta / n;
               dev += (result[i] - mean) * delta;
               n++;
                
            }
        }
    }
}
