using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace homework_1
{
    public class RSAcrypto
    {
        // Parametri per la visualizzazione
        private int[] frequencies;
        private int plotHeight;
        private int plotWidth;
        private const int MARGIN_X = 40;
        private const int MARGIN_Y = 100;
        private string text;
        // Componenti UI
        private Label output;
        private Label entropyLabel;

        // Parametri RSA
        private int e ;  // esponente
        private int P; // modulo (piano esempio, si dovrebbe usare un numero primo)

        public RSAcrypto(
            string text,
            int e,
            int P,
            int formWidth,
            int formHeight,

            Label entropyLbl)
        {
            this.P = P;
            this.e = e;
            this.text = text;
            // Configurazione visualizzazione
            plotHeight = formHeight - 200;
            plotWidth = formWidth - 900;
            this.frequencies = new int[26];
            // Riferimenti UI

            entropyLabel = entropyLbl;
        }

        // Analizza e decifra il testo cifrato usando RSA e l'analisi delle frequenze
        public void AnalyzeAndDecrypt(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Converti ogni lettera del testo in una rappresentazione numerica (A = 0, B = 1, ..., Z = 25)
            string cipherText = RSAEncrypt(this.text);

            // Calcola le frequenze delle lettere nel testo cifrato
            CalculateFrequency(cipherText);

            // Disegna l'istogramma della frequenza
            DrawHistogram(g, this.frequencies, 26, MARGIN_X, MARGIN_Y, "Frequency Distribution", Brushes.Blue);
            double plainEntropy = CalcPlaintextEntropy(this.text);
            // Calcola l'entropia del testo cifrato
            double entropy = CalculateEntropy();
            entropyLabel.Text = "Entropy Plain/CTX: " + plainEntropy.ToString("F3") +"/"+  entropy.ToString("F3");
        }

        // Metodo per cifrare un testo usando RSA (E = L^e mod P)
        public string RSAEncrypt(string input)
        {
            StringBuilder encryptedText = new StringBuilder();

            foreach (char ch in input)
            {
                if (char.IsLetter(ch))
                {
                    // Convert letter to number (A = 0, B = 1, ..., Z = 25)
                    int letterValue = char.ToUpper(ch) - 'A';

                    // Calculate (letterValue^e) % P using modular exponentiation
                    int encryptedValue = ModExp(letterValue, e, P) % 26; ;

                    // Convert back to character (A = 0, B = 1, ..., Z = 25)
                    encryptedText.Append((char)(encryptedValue + 'A'));
                }
                else
                {
                    encryptedText.Append(ch); // Keep non-alphabetic characters unchanged
                }
            }

            return encryptedText.ToString();
        }

        // Efficient modular exponentiation method
        private int ModExp(int baseValue, int exponent, int modulus)
        {
            int result = 1;
            baseValue = baseValue % modulus;

            while (exponent > 0)
            {
                if ((exponent & 1) == 1) // Se l'esponente è dispari
                    result = (result * baseValue) % modulus;

                exponent = exponent >> 1; // Divisione intera per 2
                baseValue = (baseValue * baseValue) % modulus;
            }

            return result;
        }

        // Metodo per calcolare la frequenza delle lettere in un testo
        public void CalculateFrequency(string text)
        {
            int totalLetters = 0;

            foreach (char c in text.ToUpper())
            {
                if (char.IsLetter(c))
                {
                    this.frequencies[char.ToUpper(c) - 'A']++;
                    totalLetters++;
                }
            }
        }
        public double CalcPlaintextEntropy(string text)
        {
            int[] localFrequencies = new int[26];
            int totalLetters = 0;

            // Calcola le frequenze delle lettere
            foreach (char c in text.ToUpper())
            {
                if (char.IsLetter(c))
                {
                    localFrequencies[char.ToUpper(c) - 'A']++;
                    totalLetters++;
                }
            }

            // Calcola l'entropia basandosi sulle frequenze
            double entropy = 0;
            foreach (var frequency in localFrequencies)
            {
                if (frequency > 0)
                {
                    double probability = (double)frequency / totalLetters;
                    entropy -= probability * Math.Log(probability, 2);
                }
            }

            return entropy;
        }
        // Metodo per calcolare l'entropia del testo cifrato
        public double CalculateEntropy()
        {
            int totalLetters = this.frequencies.Sum(); // Calcola il totale delle lettere
            double entropy = 0;

            // Per ogni frequenza, calcola la probabilità e aggiungi il contributo all'entropia
            foreach (var frequency in this.frequencies)
            {
                if (frequency > 0)
                {
                    double probability = (double)frequency / totalLetters; // Probabilità di ciascun carattere
                    entropy -= probability * Math.Log(probability, 2); // Entropia di Shannon
                }
            }

            return entropy;
        }

        // Metodo per visualizzare il testo cifrato e l'entropia in una nuova finestra
 

        // Metodo per disegnare l'istogramma
        private void DrawHistogram(Graphics g, int[] frequency, int modulus, float startX, float startY, string title, Brush brush)
        {
            float barWidth = plotWidth / (float)modulus;

            // Margini per spazio sopra e sotto
            float topMargin = 20; // Spazio extra sopra il grafico
            float barHeightFactor = plotHeight / (float)frequency.Max();

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
                float barHeight = (float)(frequency[i] * barHeightFactor);
                RectangleF bar = new RectangleF(
                    startX + (i * barWidth),
                    adjustedStartY - barHeight, // Regola la posizione verticale delle barre
                    barWidth - 2, // Spazio tra le barre per estetica
                    barHeight
                );

                g.FillRectangle(brush, bar);
                g.DrawRectangle(Pens.Black, Rectangle.Round(bar));
            }

            // Disegna le lettere sull'asse X
            using (Font axisFont = new Font("Arial", 10))
            {
                for (int i = 0; i < modulus; i++)
                {
                    string label = ((char)(i + 'A')).ToString();
                    float labelX = startX + (i * barWidth) + (barWidth / 2) - g.MeasureString(label, axisFont).Width / 2;
                    float labelY = adjustedStartY + 5; // Posiziona le etichette più in basso rispetto alle barre
                    g.DrawString(label, axisFont, Brushes.Black, labelX, labelY);
                }
            }
        }
    }
}
