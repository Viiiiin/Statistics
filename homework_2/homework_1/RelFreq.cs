using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace homework_1
{
    public class RelFreq
    {
        private Random random = new Random();
        private Graph graph;
        private float probability;
        private int server;
        private int attacker;
        private int t;
        public int[] result_t;
        public int[] result;

        public RelFreq(Graph graph, int server, int attacker, int t, float probability)
        {
            this.graph=graph;

            this.server = server;
            this.attacker = attacker;
            this.probability = probability;
            this.t = t;
            this.result = new int[(server + 1)];
            this.result_t = new int[(server + 1)];

        }

        public void Paint_Attack(object sender, PaintEventArgs e)
        {
            if (this.attacker == 0 || this.server == 0)
            {
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int jumps;
            float relative_score=0;
            Color color;
            PointF current_point, next_point;
            Pen pen;
            float x_space = this.graph.width / (float)this.server;
            for (int i = 0; i < this.attacker; i++)
            {
                jumps = 0;
                color = Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
                current_point = new PointF(graph.START_X, graph.height + graph.START_Y);
                next_point = new PointF(graph.START_X, graph.height + graph.START_Y);
                pen = new Pen(color, 3);

                for (int j = 0; j < this.server; j++)
                {
                    if (Attack())
                    {
                        jumps++;
                    }

                    relative_score = jumps / (float)(j + 1);

                    next_point = new PointF(current_point.X, graph.height + graph.START_Y - (relative_score * graph.height));
                    g.DrawLine(pen, current_point, next_point);
                    current_point = next_point;
                    next_point = new PointF(current_point.X + x_space, current_point.Y);
                    g.DrawLine(pen, current_point, next_point);
                    current_point = next_point;

                    // Aggiorna l'array result_t
                    if (j == t && (int)(relative_score * this.server) < result_t.Length)
                    {
                        result_t[(int)(relative_score * this.server)]++;
                    }
                }

                // Aggiorna l'array result_n
                int index = (int)(relative_score * this.server);
                if (index < result.Length)
                {
                    result[index]++;
                }
            }
        }
        private Boolean Attack()
        {
            float randomNumber = (float)random.NextDouble();
            return this.probability >= randomNumber;
        }

    }
}
