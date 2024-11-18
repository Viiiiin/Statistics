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
        private ThAndEffective tae;
        private int t,server;
        private int homework = 0;


        public Form1()
        {
            InitializeComponent();
            


        }


        private void button1_Click(object sender, EventArgs e)
        {
            int attacker, server, t, lambda,generation;
            float probability;

            bool parseServer = int.TryParse(txtserver.Text, out server);
            bool parseAtk = int.TryParse(txtattacker.Text, out attacker);
            bool parseProb = float.TryParse(txtprobability.Text, out probability);
            bool parseT = int.TryParse(txttime.Text, out t);
            bool parseLambda = int.TryParse(txtlambda.Text, out lambda);
            bool parseGen = int.TryParse(txtGen.Text, out generation);
            bool ver = t < server && t > 0;
            bool verlamnda = lambda < server;
            ValidateInputs();
            this.server = server;
            this.graph = new Graph(attacker, server, this.Width, this.Height);
            this.bernulli = new Bernulli(graph, server, attacker, t, probability);
            this.berlambda = new BerLambda(graph, server, attacker, t, probability,lambda);
            this.relfreq = new RelFreq(graph, server, attacker, t, probability);
            this.randomWalk = new RandomWalk(graph, server, attacker, t, probability);
            this.sqrn = new SqrN(graph, server, attacker, t,probability);
            this.tae = new ThAndEffective(generation, attacker, this.Width, this.Height,labelMean,labelVar);
            this.t = t;

            // Determina l'attacco selezionato
            if (radioButton1.Checked)
                this.homework = 1;
            else if (radioButton2.Checked)
                this.homework = 2;
            else if (radioButton3.Checked)
               this.homework = 3;
            else if(radioButton4.Checked)
                this.homework = 4;
            else if (radioButton5.Checked)
                this.homework = 5;
            else if (radioButton6.Checked)
                this.homework = 6;

            // Dissocia e riassocia l'evento Paint
            this.Paint -= Form1_Paint;
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.Invalidate(); // Richiama il Paint
            
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            int[] result = new int[1];
            int[] result_t = new int[1];
            this.graph.Create_Graphic(sender, e);

            switch (homework)
            {
                case 1: // Bernulli
                    this.bernulli.Paint_Attack(sender, e);
                    result = bernulli.result;
                    result_t = bernulli.result_t;
                    this.graph.Paint_Distribution(sender, e, result, this.server);
                    this.graph.Create_Distribution_Graphic(sender, e, result, this.server);
                    this.graph.Paint_Distribution(sender, e, result_t, t);
                    this.graph.Create_Distribution_Graphic(sender, e, result_t, t);
                    break;
                case 2: // RandomWalk
                    this.randomWalk.Paint_Attack(sender, e);
                    result = randomWalk.result;
                    result_t = randomWalk.result_t;
                    this.graph.Paint_Distribution(sender, e, result, this.server);
                    this.graph.Create_Distribution_Graphic(sender, e, result, this.server);
                    this.graph.Paint_Distribution(sender, e, result_t, t);
                    this.graph.Create_Distribution_Graphic(sender, e, result_t, t);
                    break;
                case 3: // RelFreq
                    this.relfreq.Paint_Attack(sender, e);
                    result = relfreq.result;
                    result_t = relfreq.result_t;
                    this.graph.Paint_Distribution(sender, e, result, this.server);
                    this.graph.Create_Distribution_Graphic(sender, e, result, this.server);
                    this.graph.Paint_Distribution(sender, e, result_t, t);
                    this.graph.Create_Distribution_Graphic(sender, e, result_t, t);
                    break;
                case 4: // Lambda
                    this.berlambda.Paint_Attack(sender, e);
                    result = berlambda.result;
                    result_t = berlambda.result_t;
                    this.graph.Paint_Distribution(sender, e, result, this.server);
                    this.graph.Create_Distribution_Graphic(sender, e, result, this.server);
                    this.graph.Paint_Distribution(sender, e, result_t, t);
                    this.graph.Create_Distribution_Graphic(sender, e, result_t, t);
                    break;

                case 5: // sqrN
                    this.sqrn.Paint_Attack(sender, e);
                    result = sqrn.result;
                    result_t = sqrn.result_t;
                    this.graph.Paint_Distribution(sender, e, result, this.server);
                    this.graph.Create_Distribution_Graphic(sender, e, result, this.server);
                    this.graph.Paint_Distribution(sender, e, result_t, t);
                    this.graph.Create_Distribution_Graphic(sender, e, result_t, t);
                    break;
                case 6:
                    this.tae.Create_Distribution(sender, e);
                    break;
            }

           

        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
 

        private void HandleInputFields()
        {
            // Disabilita/abilita i controlli in base al RadioButton selezionato
            txtprobability.Enabled = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked;
            txtlambda.Enabled = radioButton4.Checked;
            txttime.Enabled = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked;
            txtserver.Enabled = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked; 
            txtattacker.Enabled = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked || radioButton6.Checked; // Sempre abilitato
            txtGen.Enabled = radioButton6.Checked;

            // Mostra/nasconde i controlli in base al RadioButton selezionato
            txtprobability.Visible = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked;
            txtlambda.Visible = radioButton4.Checked;
            txttime.Visible = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked;
            txtserver.Visible = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked;
            txtattacker.Visible = txtattacker.Enabled; // Sempre visibile
            txtGen.Visible = radioButton6.Checked;

            // Aggiorna le etichette associate (se necessario)
            label3.Visible = txtprobability.Visible;
            label5.Visible = txtlambda.Visible;
            label4.Visible = txttime.Visible;
            label1.Visible = txtserver.Visible;
            label2.Visible = txtattacker.Visible;
            label6.Visible = txtGen.Visible;

 
        }
  

        private void ValidateInputs()
        {
            // Controlla i valori solo per le TextBox abilitate
            if (txtprobability.Enabled && !float.TryParse(txtprobability.Text, out _))
            {
                MessageBox.Show("Inserire un valore valido per Probability!");
                return;
            }

            if (txtlambda.Enabled && !int.TryParse(txtlambda.Text, out _))
            {
                MessageBox.Show("Inserire un valore valido per Lambda!");
                return;
            }

            if (txttime.Enabled && !int.TryParse(txttime.Text, out _))
            {
                MessageBox.Show("Inserire un valore valido per Time!");
                return;
            }

            if (txtGen.Enabled && !int.TryParse(txtGen.Text, out _))
            {
                MessageBox.Show("Inserire un valore valido per Generazione!");
                return;
            }
        }

        // Aggiungi questa chiamata in ogni evento CheckedChanged
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            HandleInputFields();
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
            radioButton1.CheckedChanged += radioButton_CheckedChanged;
            radioButton2.CheckedChanged += radioButton_CheckedChanged;
            radioButton3.CheckedChanged += radioButton_CheckedChanged;
            radioButton4.CheckedChanged += radioButton_CheckedChanged;
            radioButton5.CheckedChanged += radioButton_CheckedChanged;
            radioButton6.CheckedChanged += radioButton_CheckedChanged;

            HandleInputFields(); // Configura lo stato iniziale
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

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
