using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;
namespace homework_1
{
    public class Graph
    {
        // Variabili di istanza
        private int server;
        private int attacker;
        public float x_space;
        public float y_space;
        public int height;
        public int width;
        public int START_X = 40;
        public int START_Y = 100;


        // Costruttore
        public Graph(int attacker, int server,int Fw,int Fh)
        {
            this.attacker = attacker;
            this.server = server;
            this.height = Fh - 200;
            this.width = Fw - 500;



        }

        // Metodi per il disegno
        public void Paint_Distribution(object sender, PaintEventArgs e, int[] result, float time)
        {
            if (this.attacker == 0 || this.server == 0)
            {
                return;
            }
            this.x_space = this.width / (float)this.server;
            this.y_space = this.height / (float)(result.Length-1);
            int max = result.Max();
            float space = ((float)this.width / 5) / max;
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black, 1);
            PointF current_point = new PointF(START_X + time * this.x_space, this.height + START_Y);
            Font font = new Font("Arial", 12);

            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] > 0)
                {
                    RectangleF rect = new RectangleF(current_point.X, current_point.Y, (result[i] * space), this.y_space);
                    g.FillRectangle(Brushes.Blue, rect);
                    g.DrawRectangle(pen, Rectangle.Round(rect));
                }
                current_point.Y -= this.y_space;
            }

            CalculateMeanAndDeviation(result, out float mean, out float var);
            if (time < this.server)
            {
                g.DrawString($"Time T - Mean: {mean} - Var {var}", font, Brushes.Black, 40, 590);
            }
            else
            {
                g.DrawString($"Time N - Mean: {mean} - Var {var}", font, Brushes.Black, 40, 610);
            }
        }

        // Metodo per calcolare la media e la deviazione
        public void CalculateMeanAndDeviation(int[] result, out float mean, out float var)
        {
            float delta, dev;
            int n = 0;
            mean = 0;
            dev = 0;
            int score,offset = 0;
            if (this.server +1  < result.Length) // ?????
            {
                offset = this.server;
            }

            for (int index = 0; index < result.Length; index++)
            {
                int count = result[index]; // Numero di persone con quello score
                if (count > 0) // Solo per score presenti
                {
                    score = index - offset; // Calcola lo score reale, considerando l'offset

                    for (int i = 0; i < count; i++) // Per ogni persona con quello score
                    {
                        n++;
                        delta = score - mean;
                        mean += delta / n; // Aggiorna la media
                        dev += delta * (score - mean); // Aggiorna la somma dei quadrati delle deviazioni
                    }
                }
            }
            var = dev / n;
            
        }

        // Metodo per creare il contorno dei grafici
        public void Create_Distribution_Graphic(object sender, PaintEventArgs e,  int[] result, float time)
        {

            if (this.attacker == 0 || this.server == 0)
            {
                return;
            }
            Graphics g = e.Graphics;
            int max = result.Max();
            float space = ((float)this.width / 5) / max; // RIPORTA INFINIY
            this.x_space = this.width / (float)this.server;
            this.y_space = this.height / (float)result.Length-1;
            Pen blackPen = new Pen(Color.Black, 2);
            blackPen.DashStyle = DashStyle.Dash;
            RectangleF rect;
            rect = new RectangleF(time * this.x_space + START_X, START_Y, this.width / 5, this.height);
            g.DrawRectangle(blackPen, Rectangle.Round(rect));
            Font font = new Font("Arial", 5);
            Pen grayPen = new Pen(Color.Gray, 1);
            float labelOffsetY = (this.height) / this.attacker;
            float yPosition = this.height + START_Y;
            for (int i = 0; i <= max; i++)
            {
                float xPosition = time * this.x_space + START_X + (i * space);
                g.DrawLine(grayPen, xPosition, yPosition, xPosition, yPosition + 10);
                if (i % labelOffsetY == 0)
                {

                    g.DrawString($"{i}", font, Brushes.Black, xPosition, yPosition + 12);

                }

            }
        }

        public void Create_Graphic(object sender, PaintEventArgs e)
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


    }
}


