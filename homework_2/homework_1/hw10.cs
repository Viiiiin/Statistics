using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

public class NumericalIntegration
{
    private int lowerBound;
    private int upperBound;
    private int intervals;
    private Func<double, double> function;
    private List<float> xValues;
    public int height;
    public int width;
    public int START_X = 40;
    public int START_Y = 100;
    private Label labelRiemann;
    private Label labelLebesgue;

    public NumericalIntegration(int lowerBound, int upperBound, int intervals, Func<double, double> function, Label labelRiemann, Label labelLebesgue, int formHeight, int formWidth)
    {
        if (intervals <= 0)
            throw new ArgumentException("Number of intervals must be greater than zero.");

        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
        this.intervals = intervals;
        this.function = function;
        this.height = formHeight - 200;
        this.width = formWidth - 500;
        this.labelRiemann = labelRiemann;
        this.labelLebesgue = labelLebesgue;
        this.xValues = GenerateXValues();
    }

    // Generate X values
    private List<float> GenerateXValues()
    {
        List<float> xValues = new List<float>();
        float delta = (float)(upperBound - lowerBound) / intervals;

        for (int i = 0; i <= intervals; i++)
        {
            xValues.Add(lowerBound + i * delta);
        }

        return xValues;
    }

    // Calculate Riemann Integral
    public float CalculateRiemannIntegral()
    {
        float delta = (float)(upperBound - lowerBound) / intervals;
        float sum = 0f;

        for (int i = 0; i < intervals; i++)
        {
            float x = xValues[i];
            sum +=(float) function(x) * delta;
        }

        return sum;
    }

    // Calculate Lebesgue Integral (simple approximation using measure weight)
    public float CalculateLebesgueIntegral()
    {
        float delta = (upperBound - lowerBound) / (float)intervals;

        // Raggruppa i valori della funzione in "livelli" (bucket) del codominio
        Dictionary<float, float> levelMeasures = new Dictionary<float, float>();

        for (int i = 0; i < intervals; i++)
        {
            float x = xValues[i];
            float y =(float) function(x);

            // Usa un'approssimazione per rappresentare livelli (bucket) nel codominio
            float roundedY = (float)Math.Round(y, 2); // Raggruppa i valori in bucket di precisione 0.01

            if (!levelMeasures.ContainsKey(roundedY))
                levelMeasures[roundedY] = 0f;

            // Aggiungi la misura dell'intervallo (Δx) al livello corrispondente
            levelMeasures[roundedY] += delta;
        }

        // Calcola l'integrale come somma dei valori del livello * misura
        float sum = 0f;
        foreach (var kvp in levelMeasures)
        {
            float level = kvp.Key;      // Valore del livello
            float measure = kvp.Value; // Misura corrispondente (somma delle larghezze)
            sum += level * measure;
        }

        return sum;
    }

    // Draw the function curve
    public void DrawFunction(Graphics g)
    {
        // Margini per il grafico
        const int margin = 20;

        // Calcolo di valori massimo e minimo della funzione
        float maxY =(float) xValues.Max(x => function(x));
        float minY = (float)xValues.Min(x => function(x));

        // Evita divisione per zero nei calcoli di scala
        float scaleX = (upperBound - lowerBound) != 0
            ? (width - 2 * margin) / (float)(upperBound - lowerBound)
            : 1;
        float scaleY = (maxY - minY) != 0
            ? (height - 2 * margin) / (float)(maxY - minY)
            : 1;

        // Disegna il rettangolo dell'area del grafico
        RectangleF graphArea = new RectangleF(START_X, START_Y, width, height);
        g.DrawRectangle(Pens.Black, Rectangle.Round(graphArea));

        // Disegna la griglia
        Pen gridPen = new Pen(Color.LightGray, 1);
        for (int i = 0; i <= intervals; i += intervals / 10) // 10 linee di griglia
        {
            float gridX = START_X + margin + (float)((xValues[i] - lowerBound) * scaleX);
            g.DrawLine(gridPen, gridX, START_Y, gridX, START_Y + height);

            float gridY = START_Y + height - margin - (float)((function(xValues[i]) - minY) * scaleY);
            g.DrawLine(gridPen, START_X, gridY, START_X + width, gridY);
        }

        // Disegna gli assi
        Pen axisPen = new Pen(Color.Black, 2);
        float yAxisX = START_X + margin - (float)(lowerBound * scaleX);
        float xAxisY = START_Y + height - margin + (float)(minY * scaleY);

        // Asse Y
        g.DrawLine(axisPen, yAxisX, START_Y, yAxisX, START_Y + height);

        // Asse X
        g.DrawLine(axisPen, START_X, xAxisY, START_X + width, xAxisY);

        // Penna per il disegno della curva
        Pen curvePen = new Pen(Color.Red, 2);

        // Disegna la funzione
        PointF? previousPoint = null;
        for (int i = 0; i <= intervals; i++)
        {
            float x = xValues[i];
            float y =(float) function(x);

            float plotX = START_X + margin + (float)((x - lowerBound) * scaleX);
            float plotY = START_Y + height - margin - (float)((y - minY) * scaleY);

            PointF currentPoint = new PointF(plotX, plotY);

            if (previousPoint.HasValue)
            {
                g.DrawLine(curvePen, previousPoint.Value, currentPoint);
            }

            previousPoint = currentPoint;
        }

        // Disegna il titolo del grafico
        Font titleFont = new Font("Arial", 12, FontStyle.Bold);
        string title = "Graph of the Function";
        SizeF titleSize = g.MeasureString(title, titleFont);
        g.DrawString(title, titleFont, Brushes.Black, START_X + (width - titleSize.Width) / 2, START_Y - margin);

        // Disegna le etichette sugli assi
        Font axisFont = new Font("Arial", 8);
        for (int i = 0; i <= intervals; i += intervals / 5) // Etichette ogni 1/5 degli intervalli
        {
            float labelX = START_X + margin + (float)((xValues[i] - lowerBound) * scaleX);
            g.DrawString($"{xValues[i]:F1}", axisFont, Brushes.Black, labelX - 10, START_Y + height);

            float labelY = START_Y + height - margin - (float)((function(xValues[i]) - minY) * scaleY);
            g.DrawString($"{function(xValues[i]):F1}", axisFont, Brushes.Black, START_X - 30, labelY - 5);
        }

        // Calcola gli integrali
        float lebesgue = (float)Math.Round(CalculateLebesgueIntegral(), 5);
        float riemann = (float)Math.Round(CalculateRiemannIntegral(), 5);

        // Aggiorna le etichette con i valori degli integrali
        labelRiemann.Text = $"Riemann: {riemann:F5}";
        labelLebesgue.Text = $"Lebesgue: {lebesgue:F5}";
    }

    // Paint method to hook into a Form or Panel
    public void Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        this.DrawFunction(g);
    }
}
