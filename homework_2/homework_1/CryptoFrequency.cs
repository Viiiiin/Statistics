using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System;

public class CryptoFrequency
{
    // Parametri per la visualizzazione
    private int[] frequencies;
    private int plotHeight;
    private int plotWidth;
    private const int MARGIN_X = 40;
    private const int MARGIN_Y = 100;
    private string text;
    // Componenti UI
    private TextBox output;
    private Label entropyLabel;

    // Aggiungiamo una costante per la lunghezza dell'alfabeto
    private const int ALPHABET_LENGTH = 25;

    public CryptoFrequency(
        string text,
        int formWidth,
        int formHeight,
        TextBox output,
        Label entropyLblB)
    {
        this.text = text;
        // Configurazione visualizzazione
        plotHeight = formHeight - 200;
        plotWidth = formWidth - 900;
        this.frequencies = new int[ALPHABET_LENGTH];
        // Riferimenti UI
        this.output = output;
        entropyLabel = entropyLblB;
    }

    public void AnalyzeAndDecrypt(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        Random rnd = new Random();
        int shift = rnd.Next(1, ALPHABET_LENGTH); // Usa ALPHABET_LENGTH

        string cipherText = CaesarShift(this.text, shift);
        // Calcola le frequenze delle lettere nel testo cifrato
        CalculateFrequency(cipherText);

        // Disegna l'istogramma della frequenza
        DrawHistogram(g, this.frequencies, ALPHABET_LENGTH, MARGIN_X, MARGIN_Y, "Frequency Distribution", Brushes.Blue);
        // Decifra il testo usando l'analisi delle frequenze
        string decryptedText = FrequencyAnalysisDecrypt(cipherText, frequencies);
        output.Text = decryptedText;
        double entropy = CalculateEntropy();
        double plainEntropy = CalcPlaintextEntropy(this.text);
        // Calcola l'entropia del testo cifrato

        entropyLabel.Text = "Entropy Plain/CTX: " + plainEntropy.ToString("F3") + "/" + entropy.ToString("F3");
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
    // Metodo per cifrare un testo con il cifrario di Cesare
    static string CaesarShift(string input, int shift)
    {
        StringBuilder result = new StringBuilder();

        foreach (char ch in input)
        {
            if (char.IsUpper(ch))
            {
                // Handle uppercase letters
                int offset = 'A';
                char shiftedChar = (char)((ch - offset + shift) % ALPHABET_LENGTH + offset); // Usa ALPHABET_LENGTH
                result.Append(shiftedChar);
            }
            else if (char.IsLower(ch))
            {
                // Handle lowercase letters
                int offset = 'a';
                char shiftedChar = (char)((ch - offset + shift) % ALPHABET_LENGTH + offset); // Usa ALPHABET_LENGTH
                result.Append(shiftedChar);
            }
            else if (char.IsDigit(ch))
            {
                // Handle digits (0-9)
                int offset = '0';
                char shiftedChar = (char)((ch - offset + shift) % 10 + offset);
                result.Append(shiftedChar);
            }
            else
            {
                // Non-letter characters are not shifted
                result.Append(ch);
            }
        }

        return result.ToString();
    }

    // Metodo per calcolare la frequenza delle lettere in un testo
    public void CalculateFrequency(string text)
    {
        int totalLetters = 0;

        foreach (char c in text.ToUpper())
        {
            if (char.IsLetter(c))
            {
                this.frequencies[c - 'A']++;
                totalLetters++;
            }
        }
    }

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

    // Metodo per tentare di decifrare un testo cifrato usando l'analisi delle frequenze
    public string FrequencyAnalysisDecrypt(string cipherText, int[] cipherFrequencies)
    {
    
        double[] englishFrequencies = {
            0.1270,  // E
            0.0906,  // T
            0.0817,  // A
            0.0751,  // O
            0.0700,  // I
            0.0675,  // N
            0.0633,  // S
            0.0609,  // H
            0.0599,  // R
            0.0425,  // D
            0.0403,  // L
            0.0278,  // C
            0.0276,  // U
            0.0241,  // M
            0.0236,  // W
            0.0223,  // F
            0.0197,  // Y
            0.0193,  // P
            0.0149,  // B
            0.0098,  // V
            0.0077,  // K
            0.0015,  // J
            0.0015,  // X
            0.0010,  // Q
            0.0007,  // Z
        };


        int bestShift = 0;
        double bestScore = double.MaxValue;

        for (int shift = 0; shift < ALPHABET_LENGTH; shift++)
        {
            // Calcola il "chi-squared statistic" per questo shift
            double score = 0;

            for (int i = 0; i < ALPHABET_LENGTH; i++)
            {
                // Indice dopo lo shift
                int shiftedIndex = (i + shift) % ALPHABET_LENGTH;

                // Frequenza osservata
                double observedFrequency = (double)cipherFrequencies[shiftedIndex] / cipherFrequencies.Sum();

                // Frequenza attesa
                double expectedFrequency = englishFrequencies[i];

                // Aggiungi il contributo al punteggio
                score += Math.Pow(observedFrequency - expectedFrequency, 2) / expectedFrequency;
            }

            // Trova lo shift con il punteggio più basso
            if (score < bestScore)
            {
                bestScore = score;
                bestShift = shift;
            }
        }

        // Decifra il testo cifrato usando lo shift migliore trovato
        return CaesarShift(cipherText, ALPHABET_LENGTH - bestShift);
    }

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
