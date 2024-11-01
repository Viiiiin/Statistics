using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;



namespace homework_1
{
    public partial class Form1 : Form
    {
        private Graph graph;
        private Bernulli bernulli;
        private RelFreq relfreq; 
        private RandomWalk randomWalk;
        private PaintEventArgs paintEventArgs;
        private BerLambda berlambda;
        private SqrN sqrn;
        private int t,server;
        private int selectedAttack = 0; // 0 = None, 1 = Bernulli, 2 = RandomWalk, 3 = RelFreq


        public Form1()
        {
            InitializeComponent();
            


        }


        private void button1_Click(object sender, EventArgs e)
        {
            int attacker, server, t, lambda;
            float probability;

            bool parseServer = int.TryParse(txtserver.Text, out server);
            bool parseAtk = int.TryParse(txtattacker.Text, out attacker);
            bool parseProb = float.TryParse(txtprobability.Text, out probability);
            bool parseT = int.TryParse(txttime.Text, out t);
            bool parseLambda = int.TryParse(txtlambda.Text, out lambda);
            bool ver = t < server && t > 0;
            bool verlamnda = lambda < server;
            
            if (parseServer && parseAtk && parseProb && parseT && ver && parseLambda)
            {
                this.server = server;
                this.graph = new Graph(attacker, server, this.Width, this.Height);
                this.bernulli = new Bernulli(graph, server, attacker, t, probability);
                this.berlambda = new BerLambda(graph, server, attacker, t, probability,lambda);
                this.relfreq = new RelFreq(graph, server, attacker, t, probability);
                this.randomWalk = new RandomWalk(graph, server, attacker, t, probability);
                this.sqrn = new SqrN(graph, server, attacker, t,probability);
                this.t = t;

                // Determina l'attacco selezionato
                if (radioButton1.Checked)
                    this.selectedAttack = 1;
                else if (radioButton2.Checked)
                    this.selectedAttack = 2;
                else if (radioButton3.Checked)
                   this.selectedAttack = 3;
                else if(radioButton4.Checked)
                    this.selectedAttack = 4;
                else if (radioButton5.Checked)
                    this.selectedAttack = 5;

                // Dissocia e riassocia l'evento Paint
                this.Paint -= Form1_Paint;
                this.Paint += new PaintEventHandler(Form1_Paint);
                this.Invalidate(); // Richiama il Paint
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            int[] result = new int[1];
            int[] result_t = new int[1];
            this.graph.Create_Graphic(sender, e);

            switch (selectedAttack)
            {
                case 1: // Bernulli
                    this.bernulli.Paint_Attack(sender, e);
                    result = bernulli.result;
                    result_t = bernulli.result_t;
                    break;
                case 2: // RandomWalk
                    this.randomWalk.Paint_Attack(sender, e);
                    result = randomWalk.result;
                    result_t = randomWalk.result_t;
                    break;
                case 3: // RelFreq
                    this.relfreq.Paint_Attack(sender, e);
                    result = relfreq.result;
                    result_t = relfreq.result_t;
                    break;
                case 4: // Lambda
                    this.berlambda.Paint_Attack(sender, e);
                    result = berlambda.result;
                    result_t = berlambda.result_t;
                    break;

                case 5: // sqrN
                    this.sqrn.Paint_Attack(sender, e);
                    result = sqrn.result;
                    result_t = sqrn.result_t;
                    break;
            }

            // Disegna le distribuzioni solo dopo aver dipinto l'attacco selezionato
            this.graph.Paint_Distribution(sender, e, result, this.server);
            this.graph.Create_Distribution_Graphic(sender, e, result, this.server);
            this.graph.Paint_Distribution(sender, e, result_t, t);
            this.graph.Create_Distribution_Graphic(sender, e, result_t, t);
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
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '\\')
            {
                e.Handled = true;
            }
            if (textbox.Text == "0" && e.KeyChar != ',' && e.KeyChar != '\\')
            {
                e.Handled = true;
            }
            if (e.KeyChar == ',' && textbox.Text.Contains(",") && e.KeyChar != '\\')
            {
                e.Handled = true;
            }

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


        private void txttime_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtlambda_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
