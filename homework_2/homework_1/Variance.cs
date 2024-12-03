using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using static System.Windows.Forms.AxHost;

public class VarThAndEffective
{
    private Random random = new Random();
    private double[] probabilities;
    private int generation;
    private double[] result;
    private double[] means;
    private double[] variancesArr;
    private int n, m;
    public int height;
    public int width;
    public int START_X = 40;
    public int START_Y = 100;
    private Label labelMean;
    private Label labelVar;

    Dictionary<double, int> frequencies = new Dictionary<double, int>();

    public VarThAndEffective(int generation, int n, int Fw, int Fh, Label labelMean, Label labelVar)
    {
        this.n = n;
        this.probabilities = new double[n];
        this.result = new double[n];
        this.generation = generation;
        this.means = new double[generation];
        this.variancesArr = new double[generation];
        this.height = Fh - 200;
        this.width = Fw - 900;
        this.labelMean = labelMean;
        this.labelVar = labelVar;
    }

    private void Generate_Prob_array()
    {
        double sum = 0.0;
        for (int i = 0; i < probabilities.Length; i++)
        {
            this.probabilities[i] = random.NextDouble();
            sum += this.probabilities[i];
        }
        for (int i = 0; i < probabilities.Length; i++)
        {
            this.probabilities[i] /= sum;
        }
    }

    public void Paint_Effective_Distribution(Graphics g)
    {
        float x = START_X+this.width +10;
        float y = this.height + START_Y;
        RectangleF graphArea = new RectangleF(x, START_Y, width, height);
        g.DrawRectangle(Pens.Black, Rectangle.Round(graphArea));
        int[] array = frequencies.OrderBy(kvp => kvp.Key)
                                 .Select(kvp => kvp.Value)
                                 .ToArray();

        int l = array.Length;
        if (l == 0) return;
        double max = array.Max();
        float spacing = this.width * 0.05f / (l - 1);
        float rectWidth = (this.width ) /(float) l;

        for (int i = 0; i < l; i++)
        {
            float normalizedHeight = (float)(array[i] / max) * this.height;
            g.FillRectangle(Brushes.Blue, x, y - normalizedHeight, rectWidth, normalizedHeight);
            x += rectWidth ;
        }
    }

    public void Paint_Theoretic_Distribution(Graphics g)
    {
        float x = START_X;
        float y = this.height + START_Y;
        RectangleF graphArea = new RectangleF(x, START_Y, width, height);
        g.DrawRectangle(Pens.Black, Rectangle.Round(graphArea));
        float spacing = this.width * 0.05f / (n - 1);
        float rectWidth = (this.width * 0.95f) / n;

        for (int i = 0; i < n; i++)
        {
            float rectHeight = (float)(this.probabilities[i]) * this.height;
            g.FillRectangle(Brushes.Red, x, y - rectHeight, rectWidth, rectHeight);
            x += rectWidth + spacing;
        }
    }

    public void Create_Distribution(object sender, PaintEventArgs e)
    {
        Generate_Prob_array();
        Graphics g = e.Graphics;

        Paint_Theoretic_Distribution(g);

        double meanOfMeans = 0.0, varianceOfMeans = 0.0;
        double meanOfVariances = 0.0, varianceOfVariances = 0.0;



        CalculateMeanAndVariance(probabilities, out double meanTheoretical, out double varTheoretical);

        for (int i = 0; i < generation; i++)
        {
            double[] sample = GenerateSample(probabilities, this.n, this.n, random);

            double sampleMean = sample.Average();
            means[i] = sampleMean;

            double samplevariance = Calculatesamplevariance(sample, sampleMean);
            variancesArr[i] = samplevariance;

            UpdateWalford(ref meanOfMeans, ref varianceOfMeans, sampleMean, i + 1);
            UpdateWalford(ref meanOfVariances, ref varianceOfVariances, samplevariance, i + 1);

            double roundedVariance = Math.Round(samplevariance, 6); // Arrotonda a 2 decimali
            if (frequencies.ContainsKey(roundedVariance))
            {
                frequencies[roundedVariance]++;
            }
            else
            {
                frequencies[roundedVariance] = 1;
            }
        }

        varianceOfMeans /= generation;
        varianceOfVariances /= generation ;

        Paint_Effective_Distribution(g);

        labelMean.Text = $"Mean Th: {meanTheoretical:F2}, Mean Emp: {meanOfMeans:F2}";
        labelVar.Text = $"Var Th: {varTheoretical:F2}," +
                        $"Var-Sample Mean: {meanOfVariances:F2}, Var-Sample Var: {varianceOfVariances:F2}";
    }

    private void UpdateWalford(ref double mean, ref double variance, double value, int count)
    {
        double delta = value - mean;
        mean += delta / count;
        variance += delta * (value - mean);
    }

    private double Calculatesamplevariance(double[] sample, double sampleMean)
    {
        double sum = sample.Sum(x => (x - sampleMean) * (x - sampleMean));
        return sum / (sample.Length - 1);
    }

    private void CalculateMeanAndVariance(double[] array, out double mean, out double variance)
    {
        mean = 0;
        double M2 = 0;

        for (int i = 0; i < array.Length; i++) mean += i * array[i];
        for (int i = 0; i < array.Length; i++) M2 += (i - mean) * (i - mean) * array[i];

        variance = M2;
    }

    private double[] GenerateSample(double[] probabilities, int n, int intervals, Random random)
    {
        double[] sample = new double[n];
        for (int i = 0; i < n; i++)
        {
            double rand = random.NextDouble();
            double cumulative = 0;
            for (int j = 0; j < intervals; j++)
            {
                cumulative += probabilities[j];
                if (rand <= cumulative)
                {
                    sample[i] = j;
                    break;
                }
            }
        }
        return sample;
    }
}
