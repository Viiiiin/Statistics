
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace homework_1
{
    public class SqrN
    {

        private Random random = new Random();
        private Graph graph;
        private float probability;
        private int server;
        private int attacker;
        private int t;
        public int[] result_t;
        public int[] result;

        public SqrN(Graph graph, int server, int attacker, int t, float probability)
        {
            this.graph = graph;
            this.server = server;
            this.attacker = attacker;
            this.probability = probability;
            this.t = t;
            this.result = new int[(server * 2) + 1];
            this.result_t = new int[(server * 2) + 1];

        }

        public void Paint_Attack(object sender, PaintEventArgs e)
        {
            if (this.attacker == 0 || this.server == 0)
            {
                return;
            }
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float x_space = this.graph.width / (float)this.server;
            float y_space = this.graph.height / (float)(result.Length - 1);
            int count;
            Color color;
            PointF current_point, next_point;
            Pen pen;
            float space = (float)Math.Sqrt(1.0 / this.server);

            for (int i = 0; i < this.attacker; i++)
            {
                count = 0;
                color = Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
                current_point = new PointF(graph.START_X, (this.graph.height / 2 + graph.START_Y));
                next_point = new PointF(graph.START_X, (this.graph.height / 2 + graph.START_Y));
                pen = new Pen(color, 3);

                for (int j = 0; j < this.server; j++)
                {
                    if (Attack())
                    {
                        next_point = new PointF(current_point.X, current_point.Y - (y_space * space));
                        count++;
                    }
                    else
                    {
                        next_point = new PointF(current_point.X, current_point.Y + (y_space * space));
                        count--;
                    }
                    g.DrawLine(pen, current_point, next_point);
                    current_point = next_point;
                    next_point = new PointF(current_point.X + x_space, current_point.Y);
                    g.DrawLine(pen, current_point, next_point);
                    current_point = next_point; 
                    if (j == t)
                    {
                        this.result_t[(int)((count * space) + this.server)] += 1;
                    }

                }

                this.result[(int)((count * space) + this.server)] += 1;
            }
        }

        private Boolean Attack()
        {
            float randomNumber = (float)random.NextDouble();
            return this.probability >= randomNumber;
        }
    }
}



