using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace homework_1
{
    public partial class Form1 : Form
    {
        Random random;
        private int server;
        private int attacker;
        private float probability;
        private int[] result;
        private float x_space;
        private float y_space;
        private int height;
        private int width;
        private const int START_X = 40;
        private const int START_Y = 100;
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;


            this.random = new Random();
            this.height = this.ClientSize.Height - 150;
            this.width = (this.ClientSize.Width/2) - 50;
            this.Paint += new PaintEventHandler(Create_Graphic);
            this.Paint += new PaintEventHandler(Paint_Attack);
            this.Paint += new PaintEventHandler(Create_Graphic_result);
            this.Paint += new PaintEventHandler(Paint_Result);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            int attacker, server;
            float probability;
            bool parseServer = int.TryParse(txtserver.Text, out server);
            bool parseAtk = int.TryParse(txtattacker.Text, out attacker);
            bool parseProb = float.TryParse(txtprobability.Text, out probability);
            if (parseServer && parseAtk && parseProb)
            {
                this.server = server;
                this.attacker = attacker;
                this.probability = probability;
                this.result = new int[server + 1];
                this.x_space = this.width / (float)this.server;
                this.y_space = this.height / (float)this.server;
                this.Invalidate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void checkvalue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void checkProbability_KeyPress(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox textbox = sender as System.Windows.Forms.TextBox;

            if (textbox.Text == "" && e.KeyChar != '0' && e.KeyChar != '1' && e.KeyChar != '\\')
            {
                e.Handled = true;
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\\')
            {
                e.Handled = true;
            }
            if (textbox.Text == "0" && e.KeyChar != '.' && e.KeyChar != '\\')
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && textbox.Text.Contains(".") && e.KeyChar != '\\')
            {
                e.Handled = true;
            }

        }

       

        


    private void Paint_Attack(object sender, PaintEventArgs e)
    {
        if (this.attacker ==0 || this.server == 0)
            {
                return;
            }
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        int count;
        Color color;
        PointF current_point, next_point;
        Pen pen;
        for (int i = 0; i < this.attacker; i++)
        {
            count = 0;
            color = Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
            current_point = new PointF(START_X, this.height+START_Y);
            next_point = new PointF(START_X, this.height+START_Y);
            pen = new Pen(color, 3);

            for (int j = 0; j < this.server; j++)
            {
                if (Attack())
                {
                    next_point = new PointF(current_point.X, current_point.Y - y_space);
                    g.DrawLine(pen, current_point, next_point);
                    current_point = next_point;
                    count++;
                }

                next_point = new PointF(current_point.X + x_space, current_point.Y);
                g.DrawLine(pen, current_point, next_point);
                current_point = next_point;
            }

            this.result[count] += 1;
        }
    }

    private Boolean Attack()
    {
        float randomNumber = (float)random.NextDouble();
        return this.probability >= randomNumber;
    }

    private void Create_Graphic(object sender, PaintEventArgs e)
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

        private void Paint_Result(object sender, PaintEventArgs e)
        {
            if (this.attacker == 0 || this.server == 0)
            {
                return;
            }
            float space = ((float)this.width / 2) / this.attacker;
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black, 1);
            PointF current_point, next_point;
            current_point = new PointF(START_X+ this.width, this.height + START_Y);
            Font font = new Font("Arial", 12);

            for (int i = 0; i < this.result.Length; i++)
            {

                if (this.result[i] > 0)
                {
                    next_point = new PointF(current_point.X + (this.result[i] * space), current_point.Y );
                    RectangleF rect = new RectangleF(current_point.X, current_point.Y, (this.result[i] * space),this.y_space);
                    g.FillRectangle(Brushes.Blue, rect); 

                    g.DrawRectangle(pen, Rectangle.Round(rect)); 
                }
                current_point.Y -= this.y_space;
            }
            float avg = average(0,0,0);
            g.DrawString($" The average is: {avg}", font, Brushes.Black, 700, 10);

        }
        private void Create_Graphic_result(object sender, PaintEventArgs e)
        {
            if (this.attacker == 0 || this.server == 0)
            {
                return;
            }
            Graphics g = e.Graphics;
            float space = ((float)this.width / 2) / this.attacker;
            Pen blackPen = new Pen(Color.Black, 2);
            blackPen.DashStyle = DashStyle.Dash;

            Rectangle rect = new Rectangle(this.width + START_X, START_Y, this.width / 2,this.height);
            g.DrawRectangle(blackPen, rect);
            Font font = new Font("Arial", 5);
            int len = this.result.Length;
            Pen grayPen = new Pen(Color.Gray, 1);
            float labelOffsetX = (this.width/2) / len;
            float labelOffsetY = (this.height) / this.attacker;
            float yPosition = this.height + START_Y;
            for (int i = 0; i <= this.attacker; i++)
            {
                float xPosition = this.width + START_X + (i * space);
                g.DrawLine(grayPen, xPosition, yPosition, xPosition, yPosition + 10);
                if (i % labelOffsetY == 0)
                {

                    g.DrawString($"{i}", font, Brushes.Black, xPosition, yPosition + 12);

                }

            }

           
        }

        private float average(int i, float cumulativeAverage, int numberOfAttempts)
        {
            if (i >= result.Length)
            {
                return numberOfAttempts > 0 ? cumulativeAverage : 0;
            }

            if (result[i] > 0)
            {
                numberOfAttempts++;
                cumulativeAverage += (result[i] - cumulativeAverage) / numberOfAttempts;
            }

            return average(i + 1, cumulativeAverage, numberOfAttempts);
        }

        private void txtserver_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtattacker_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtprobability_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
