using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System;

public class MThAndEffective
{
    private Random random = new Random();
    private double[] probabilities;
    private int generation;
    private double[] result;
    private double[] means;
    private double[] variance;
    private int n, m;
    public int height;
    public int width;
    public int START_X = 40;
    public int START_Y = 100;
    private Label labelMean;
    private Label labelVar;

    // Aggiungi una variabile per la frequenza
    private int[] frequencies; // Frequenze dei valori generati

    public MThAndEffective(int generation, int n, int Fw, int Fh, Label labelMean, Label labelVar)
    {
        this.n = n;
        this.probabilities = new double[n];
        this.result = new double[n];
        this.generation = generation;
        this.means = new double[generation];
        this.variance = new double[generation];
        this.height = Fh - 200;
        this.width = Fw - 500;
        this.labelMean = labelMean;
        this.labelVar = labelVar;

        // Inizializza l'array delle frequenze
        this.frequencies = new int[n];
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


    public void Paint_Effective_Distribution(Graphics g)
    {
        float x = START_X;
        float y = this.height + START_Y;

        // Calcolo della larghezza dei rettangoli e dello spazio tra di essi
        float totalSpacing = this.width * 0.05f; // Spazio totale (5% della larghezza complessiva)
        float spacing = totalSpacing / (n - 1); // Spazio tra i rettangoli
        float rect_width = (this.width - totalSpacing) / (float)n; // Larghezza dei rettangoli

        // Trova la frequenza massima per normalizzare
        int maxFrequency = frequencies.Max();
        int minFrequency = frequencies.Min();
        int range = maxFrequency - minFrequency;

        // Normalizzazione della frequenza e disegno dei rettangoli
        if (range > 0)
        {
            for (int i = 0; i < n; i++)
            {
                // Altezza del rettangolo proporzionale alla frequenza, normalizzata
                float rect_height = (float)((frequencies[i] - minFrequency) / (float)range) * this.height;

                // Disegna il rettangolo
                g.FillRectangle(Brushes.Blue, x, y - rect_height, rect_width, rect_height);

                // Aggiungi spazio tra i rettangoli
                x += rect_width + spacing; // Aggiungi spazio tra i rettangoli
            }
        }
    }


    public void Paint_Theoretic_Distribution(Graphics g)
    {
        float x = START_X;
        float y = this.height + START_Y;

        // Calcolo della larghezza dei rettangoli e dello spazio tra di essi
        float totalSpacing = this.width * 0.05f; // Spazio totale (5% della larghezza complessiva)
        float spacing = totalSpacing / (n - 1); // Spazio tra i rettangoli
        float rect_width = (this.width - totalSpacing) / (float)n; // Larghezza dei rettangoli

        for (int i = 0; i < n; i++)
        {
            // Altezza del rettangolo proporzionale alla probabilità teorica
            float rect_height = (float)(this.probabilities[i]) * this.height;

            // Disegna il rettangolo
            g.FillRectangle(Brushes.Red, x, y - rect_height, rect_width, rect_height);

            x += rect_width + spacing; // Aggiungi spazio tra i rettangoli
        }
    }

    public void Create_Distribution(object sender, PaintEventArgs e)
    {
        int intervals = this.n; // Numero di intervalli
        int generations = this.generation;
        Create_Graphic(sender, e); // Disegna il contorno dell'area
        Generate_Prob_array(); // Genera l'array delle probabilità teoriche
        Graphics g = e.Graphics;

        // Disegna la distribuzione teorica (rettangoli rossi)
        Paint_Theoretic_Distribution(g);

        // Variabili per calcolo delle medie e varianze empiriche
        double meanOfMeans = 0.0;
        double varianceOfMeans = 0.0;
        int generationCount = 0;

        // Inizializza array delle medie campionarie
        means = new double[this.generation];

        // Calcolo teorico di media e varianza iniziale
        CalculateMeanAndVariance(probabilities, out double meanTheoretical, out double varTheoretical);

        for (int i = 0; i < generations; i++)
        {
            // Genera la media campionaria
            double sampleMean = GenerateSampleMean(probabilities, this.n, intervals, random);
            means[i] = sampleMean;

            // Algoritmo di Walford per media
            generationCount++;
            double delta = sampleMean - meanOfMeans;
            meanOfMeans += delta / generationCount;

            // Algoritmo di Walford per varianza
            double delta2 = sampleMean - meanOfMeans;
            varianceOfMeans += delta * delta2;

            // Incrementa la frequenza del valore generato
            int index = (int)Math.Floor(sampleMean); // Usando il valore intero come indice
            if (index >= 0 && index < frequencies.Length)
            {
                frequencies[index]++;
            }
        }

        // Calcolo finale della varianza usando Walford
        varianceOfMeans = (generationCount > 1) ? varianceOfMeans / (generationCount - 1) : 0.0;

        // Disegna l'istogramma delle medie campionarie
        Paint_Effective_Distribution(g);

        // Aggiorna i label
        labelMean.Text = $"Mean Th: {meanTheoretical:F2}, Mean Emp: {meanOfMeans:F2}";
        labelVar.Text = $"Var Th: {varTheoretical:F2}, Var Emp: {varianceOfMeans:F2}";
    }

    private double GenerateSampleMean(double[] probabilities, int n, int intervals, Random random)
    {
        double sampleSum = 0;
        for (int i = 0; i < n; i++)
        {
            double rand = random.NextDouble();
            double cumulative = 0;
            for (int j = 0; j < intervals; j++)
            {
                cumulative += probabilities[j];
                if (rand <= cumulative)
                {
                    sampleSum += j;
                    break;
                }
            }
        }
        return sampleSum / n;
    }

    public void CalculateMeanAndVariance(double[] array, out double mean, out double variance)
    {
        mean = 0;
        double M2 = 0;

        // Calcola la media (expected value)
        for (int i = 0; i < array.Length; i++)
        {
            mean += i * array[i];
        }

        // Calcola la varianza
        for (int i = 0; i < array.Length; i++)
        {
            double delta = i - mean;
            M2 += delta * delta * array[i];
        }

        // La varianza è la somma dei quadrati delle deviazioni dalla media
        variance = M2/n;
    }


    public void Create_Graphic(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        Pen blackPen = new Pen(Color.Black, 2);
        Rectangle rect = new Rectangle(START_X, START_Y, this.width, this.height);
        g.DrawRectangle(blackPen, rect);
    }
}
