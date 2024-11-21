using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace homework_1
{
    public class ThAndEffective
    {
        private Random random = new Random();
        private double[] probabilities;
        private int generation;
        private double[] result;
        private int n;
        public int height;
        public int width;
        public int START_X = 40;
        public int START_Y = 100;
        private Label labelMean;
        private Label labelVar;

        public ThAndEffective(int generation, int n, int Fw, int Fh, Label labelMean, Label labelVar)
        {
            this.n = n;
            this.probabilities = new double[n];
            this.result = new double[n];
            this.generation = generation;
            this.height = Fh - 200;
            this.width = Fw - 500;
            this.labelMean = labelMean;
            this.labelVar = labelVar;
        }

        private void Generate_Prob_array()
        {
            double sum = 0.0;
            double rand;
            for (int i = 0; i < probabilities.Length; i++)
            {
                this.probabilities[i] = random.NextDouble();
                sum += this.probabilities[i];

            }
            for (int i = 0; i < probabilities.Length; i++)
            {
                this.probabilities[i] = this.probabilities[i] / sum;

            }
        }

        public void Create_Distribution(object sender, PaintEventArgs e)
        {
            Create_Graphic(sender, e);
            Generate_Prob_array();
            Graphics g = e.Graphics;
            Paint_Theoretic_Distribution(g);

            // Calcolo delle somme cumulative
            Double[] cumulativeSums = new Double[this.n];
            cumulativeSums[0] = this.probabilities[0];
            for (int i = 1; i < probabilities.Length; i++)
            {
                cumulativeSums[i] = cumulativeSums[i - 1] + probabilities[i];
            }

            // Azzera l'array dei risultati
            Array.Clear(result, 0, result.Length);

            // Variabili per il Walford Algorithm
            double meanEmpirical = 0;
            double varianceEmpirical = 0;
            int count = 0; // Numero totale di valori generati

            // Genera i valori casuali e calcola direttamente media e varianza
            for (int i = 0; i < this.generation; i++)
            {
                double randomValue = random.NextDouble();

                // Trova l'intervallo corretto
                for (int j = 0; j < this.n; j++)
                {
                    if (randomValue < cumulativeSums[j])
                    {
                        result[j]++;
                        count++;

                        // Aggiornamento incrementale di media e varianza (Walford Algorithm)
                        double delta = j - meanEmpirical;
                        meanEmpirical += delta / count;
                        double delta2 = j - meanEmpirical;
                        varianceEmpirical += delta * delta2;

                        break;
                    }
                }

                Paint_Effective_Distribution(g);
            }

            // Normalizza la varianza empirica
            varianceEmpirical /= count;

            // Calcola media e varianza teoriche
            CalculateMeanAndVariance(probabilities, out double meanTheoretical, out double varTheoretical);

            // Aggiorna le Label con entrambe le medie e varianze
            labelMean.Text = $"Mean Th/Emp: {meanTheoretical:F2}/{meanEmpirical:F2}";
            labelVar.Text = $"Var Th/Emp: {varTheoretical:F2}/{varianceEmpirical:F2}";
        }

        public void CalculateMeanAndVariance(double[] probabilities, out double mean, out double variance)
        {
            mean = 0;
            double M2 = 0;

            // Calcola la media (expected value)
            for (int i = 0; i < probabilities.Length; i++)
            {
                mean += i * probabilities[i];
            }

            // Calcola la varianza
            for (int i = 0; i < probabilities.Length; i++)
            {
                double delta = i - mean;
                M2 += delta * delta * probabilities[i];
            }

            variance = M2;
        }


        public void Paint_Effective_Distribution(Graphics g)
        {
            float x = START_X;
            float y = this.height + START_Y;
            float rect_width = this.width / (float)n;
            Pen pen = new Pen(Color.Blue, 1);

            double sum = this.result.Sum();
            if (sum > 0)
            {
                for (int i = 0; i < this.n; i++)
                {
                    float redHeight = (float)(this.probabilities[i]) * this.height;
                    g.FillRectangle(Brushes.White, x, START_Y, rect_width, y - START_Y - redHeight);
                    float rect_height = (float)(this.result[i] / sum) * this.height;
                    RectangleF rect = new RectangleF(x, y - rect_height, rect_width, rect_height);
                    g.FillRectangle(Brushes.Blue, rect);
                    g.DrawRectangle(pen, Rectangle.Round(rect));
                    x += rect_width;
                }
            }
        }


        public void Create_Graphic(object sender, PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            Pen blackPen = new Pen(Color.Black, 2);
            Rectangle rect = new Rectangle(START_X, START_Y, this.width, this.height);
            g.DrawRectangle(blackPen, rect);
        }


        public void Paint_Theoretic_Distribution(Graphics g)
        {

            float x = START_X;
            float y = this.height + START_Y;
            float rect_width = this.width / (float)n;
            float rect_height;
            Pen pen = new Pen(Color.Black, 1);
            for (int i = 0; i < this.n; i++)
            {
                // punto di inizio x=0 y=height+Start_y
                rect_height = (float)(this.probabilities[i]) * this.height;
                RectangleF rect = new RectangleF(x, y - rect_height, rect_width, rect_height);
                g.FillRectangle(Brushes.Red, rect);
                g.DrawRectangle(pen, Rectangle.Round(rect));
                x += rect_width;

            }
        }
    }
}