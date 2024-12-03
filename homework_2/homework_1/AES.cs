using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System;
using System.Collections.Generic;

public class AES
{
    // Parametri per la visualizzazione
    private int[] frequencies;
    private int plotHeight;
    private int plotWidth;
    private const int MARGIN_X = 40;
    private const int MARGIN_Y = 100;
    private string text;
    private int[] plainFrequencies = new int[26];
    // Componenti UI
    private TextBox output;
    private Label entropyLabel;

    // Aggiungiamo una costante per la lunghezza dell'alfabeto
    private const int ALPHABET_LENGTH = 25;

    public AES(
        string text,
        int formWidth,
        int formHeight,
        TextBox output,
        Label entropyLblB)
    {
        this.text = text;
        // Configurazione visualizzazione
        plotHeight = formHeight - 400;
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

        string cipherText = Permutation(this.text, shift);
        // Calcola le frequenze delle lettere nel testo cifrato
        CalculateFrequency(text,this.plainFrequencies);
        CalculateFrequency(cipherText,this.frequencies);
        // Disegna l'istogramma della frequenza
        DrawHistogram(g, this.frequencies, ALPHABET_LENGTH, MARGIN_X, MARGIN_Y, "Distribution CTX", Brushes.Blue);
        DrawHistogram(g, this.plainFrequencies, ALPHABET_LENGTH, MARGIN_X+this.plotWidth+50, MARGIN_Y, "Distribution Plaintext", Brushes.Blue);
        // Decifra il testo usando l'analisi delle frequenze
        // string decryptedText = FrequencyAnalysisDecrypt(cipherText, frequencies);
        // output.Text = decryptedText;
        double entropy = CalculateEntropy(this.frequencies);
        double plainEntropy = CalculateEntropy(this.plainFrequencies);
        // Calcola l'entropia del testo cifrato

        entropyLabel.Text = "Entropy Plain/CTX: " + plainEntropy.ToString("F3") + "/" + entropy.ToString("F3");
    }



    // Metodo per cifrare un testo con il cifrario di Cesare
    static string Permutation(string input, int shift)
    {
        StringBuilder result = new StringBuilder();

            Dictionary<char, char> mapping = new Dictionary<char, char>
        {
            { 'A', 'Q' }, { 'B', 'Z' }, { 'C', 'X' }, { 'D', 'W' }, { 'E', 'V' },
            { 'F', 'U' }, { 'G', 'T' }, { 'H', 'R' }, { 'I', 'S' }, { 'J', 'P' },
            { 'K', 'O' }, { 'L', 'N' }, { 'M', 'M' }, { 'N', 'L' }, { 'O', 'K' },
            { 'P', 'J' }, { 'Q', 'I' }, { 'R', 'H' }, { 'S', 'G' }, { 'T', 'F' },
            { 'U', 'E' }, { 'V', 'D' }, { 'W', 'C' }, { 'X', 'B' }, { 'Y', 'A' },
            { 'Z', 'Y' }
        };
        foreach (char ch in input)
        {
            if (mapping.ContainsKey(ch))
            {
                result.Append(mapping[ch]);
            }
            else
            {
                result.Append(ch);
            }
        }

        return result.ToString();
    }

    // Metodo per calcolare la frequenza delle lettere in un testo
    public void CalculateFrequency(string text, int[] array)
    {
        int totalLetters = 0;

        foreach (char c in text.ToUpper())
        {
            if (char.IsLetter(c))
            {
                array[c - 'A']++;
                totalLetters++;
            }
        }
    }

    public double CalculateEntropy(int[] array) 
    {
        int totalLetters = array.Sum(); // Calcola il totale delle lettere
        double entropy = 0;

        // Per ogni frequenza, calcola la probabilità e aggiungi il contributo all'entropia
        foreach (var frequency in array)
        {
            if (frequency > 0)
            {
                double probability = (double)frequency / totalLetters; // Probabilità di ciascun carattere
                entropy -= probability * Math.Log(probability, 2); // Entropia di Shannon
            }
        }

        return entropy;
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
        RectangleF graphArea = new RectangleF(startX - 10, startY, plotWidth + 10, plotHeight);
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
