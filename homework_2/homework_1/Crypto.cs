using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace homework_1
{
    public class CryptoDistribution
    {
        // Parametri di configurazione
        private Random random;
        private int modulus; // n
        private int maxU; // Valore massimo per l'esponente


        // Distribuzioni da salvare
        private double[] distributionA;
        private double[] distributionB;

        // Parametri per la visualizzazione
        private int plotHeight;
        private int plotWidth;
        private const int MARGIN_X = 40;
        private const int MARGIN_Y = 100;

        // Componenti UI
        private Label entropyLabelA;
        private Label entropyLabelB;

        public CryptoDistribution(
            int maxU,
            int formWidth,
            int formHeight,
            Label entropyLblA,
            Label entropyLblB)
        {
            random = new Random();

            // Configurazione delle distribuzioni
            distributionA = new double[19]; // n = 19
            distributionB = new double[15]; // n = 15
            this.maxU = maxU;
            // Configurazione visualizzazione
            plotHeight = formHeight - 200 ;
            plotWidth = formWidth - 900;

            // Riferimenti UI
            entropyLabelA = entropyLblA;
            entropyLabelB = entropyLblB;
        }

        /// <summary>
        /// Calcola la distribuzione di Y = g^U mod n
        /// </summary>
        private double[] ComputeDistribution(int n, int[] gValues, int maxU)
        {
            int[] frequency = new int[n];
            Array.Clear(frequency, 0, frequency.Length);

            foreach (int g in gValues)
            {
                
                    int exponent = random.Next(1, maxU + 1);
                    int result = ModularExponentiation(g, exponent, n);
                    frequency[result]++;
                
            }

            double total = frequency.Sum();
            double[] probabilities = new double[n];
            for (int i = 0; i < n; i++)
            {
                probabilities[i] = frequency[i] / total;
            }

            return probabilities;
        }

        /// <summary>
        /// Calcolo esponenziale modulare efficiente
        /// </summary>
        private int ModularExponentiation(int baseVal, int exponent, int modulus)
        {
            long result = 1;
            baseVal %= modulus;

            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result = (result * baseVal) % modulus;

                baseVal = (int)((long)baseVal * baseVal % modulus);
                exponent >>= 1;
            }

            return (int)result;
        }

        /// <summary>
        /// Disegna le due distribuzioni e calcola entropia/diversità
        /// </summary>
        public void DrawDistributions(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Calcola distribuzione A (n = 19, g = 2, 3, 10, 17)
            distributionA = ComputeDistribution(19, new int[] { 2, 3, 10, 17 }, maxU);
            DrawHistogram(g, distributionA, 19, MARGIN_X, MARGIN_Y, "Distribuzione A", Brushes.Blue);
            double entropyA = ComputeEntropy(distributionA);
            entropyLabelA.Text = $"Entropia (A): {entropyA:F2}";

            // Calcola distribuzione B (n = 15, g = 3, 6, 9, 12)
            distributionB = ComputeDistribution(15, new int[] { 3, 6, 9, 12 }, maxU);
            DrawHistogram(g, distributionB, 15, MARGIN_X + plotWidth + 50, MARGIN_Y, "Distribuzione B", Brushes.Red);
            double entropyB = ComputeEntropy(distributionB);
            entropyLabelB.Text = $"Entropia (B): {entropyB:F2}";
        }

        /// <summary>
        /// Disegna un istogramma con un'etichetta di titolo
        /// </summary>
        private void DrawHistogram(Graphics g, double[] probabilities, int modulus, float startX, float startY, string title, Brush brush)
        {
            float barWidth = plotWidth / (float)modulus;

            // Margini per spazio sopra e sotto
            float topMargin = 20; // Spazio extra sopra il grafico
            float barHeightFactor = plotHeight;

            // Aggiorna il punto di partenza verticale considerando il margine inferiore
            float adjustedStartY = plotHeight + startY;

            // Disegna il titolo
            using (Font titleFont = new Font("Arial", 12, FontStyle.Bold))
            {
                g.DrawString(title, titleFont, Brushes.Black, startX, adjustedStartY - plotHeight + 20);
            }

            // Disegna il rettangolo contenitore del grafico
            RectangleF graphArea = new RectangleF(startX - 10, startY, plotWidth + 50, plotHeight);
            g.DrawRectangle(Pens.Black, Rectangle.Round(graphArea));

            // Disegna le barre
            for (int i = 0; i < modulus; i++)
            {
                float barHeight = (float)(probabilities[i] * barHeightFactor);
                RectangleF bar = new RectangleF(
                    startX + (i * barWidth),
                    adjustedStartY - barHeight, // Regola la posizione verticale delle barre
                    barWidth - 2, // Spazio tra le barre per estetica
                    barHeight
                );

                g.FillRectangle(brush, bar);
                g.DrawRectangle(Pens.Black, Rectangle.Round(bar));
            }

            // Disegna i numeri sull'asse X
            using (Font axisFont = new Font("Arial", 10))
            {
                for (int i = 0; i < modulus; i++)
                {
                    string label = i.ToString();
                    float labelX = startX + (i * barWidth) + (barWidth / 2) - g.MeasureString(label, axisFont).Width / 2;
                    float labelY = adjustedStartY + 5; // Posiziona le etichette più in basso rispetto alle barre
                    g.DrawString(label, axisFont, Brushes.Black, labelX, labelY);
                }
            }


        }



        /// <summary>
        /// Calcola l'entropia di Shannon
        /// </summary>
        private double ComputeEntropy(double[] probabilities)
        {
            double entropy = 0.0;

            foreach (double p in probabilities)
            {
                if (p > 0)
                {
                    entropy -= p * Math.Log(p);
                }
            }

            return entropy;
        }
    }
}
