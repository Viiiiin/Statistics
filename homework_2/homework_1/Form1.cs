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
using System.Xml.Serialization;
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
        private MThAndEffective mtae;
        private VarThAndEffective varMtae;
        private CryptoDistribution cr;
        private CryptoFrequency cf;
        private RSAcrypto rsa;
        private AES aes;
        private NumericalIntegration re;
        private Dictionary<string, Func<double, double>> functions;
        private int t,server;
        private int homework = 0;
        private bool calculationComplete = false;


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
            string selectedFunction = functionSelector.SelectedItem.ToString();
            Func<double, double> function = functions[selectedFunction];
            bool verlamnda = lambda < server;
            
            ValidateInputs();
            this.server = server;
            RadioButton[] radioButtons = new RadioButton[]
            {
                radioButton1, radioButton2, radioButton3, radioButton4, radioButton5,
                radioButton6, radioButton7, radioButton8, radioButton9, radioButton10,
                radioButton11, radioButton12, radioButton13
            };
            
            // Determina l'attacco selezionato
            this.homework = Array.FindIndex(radioButtons, rb => rb.Checked) + 1;
            switch (homework)
            {
                case 1:
                    this.bernulli = new Bernulli(graph, server, attacker, t, probability);
                    break;
                case 2:
                    this.randomWalk = new RandomWalk(graph, server, attacker, t, probability);
                    break;
                case 3:
                    this.relfreq = new RelFreq(graph, server, attacker, t, probability);
                    break;
                case 4:
                    this.berlambda = new BerLambda(graph, server, attacker, t, probability, lambda);
                    break;
                case 5:
                    this.sqrn = new SqrN(graph, server, attacker, t, probability);
                    break;
                case 6:
                    this.tae = new ThAndEffective(generation, attacker, this.Width, this.Height, labelMean, labelVar);
                    break;
                case 7:
                    this.mtae = new MThAndEffective(generation, attacker, this.Width, this.Height, labelMean, labelVar);
                    break;
                case 8:
                    this.cr = new CryptoDistribution(server, this.Width, this.Height, labelMean, labelVar);
                    break;
                case 9:
                    this.cf = new CryptoFrequency(txtPlain.Text, this.Width, this.Height, output, labelVar);
                    break;
                case 10:
                    this.rsa = new RSAcrypto(txtPlain.Text, t, generation, this.Width, this.Height, labelVar);
                    break;
                case 11:
                    this.varMtae = new VarThAndEffective(generation, attacker, this.Width, this.Height, labelMean, labelVar);
                    break;
                case 12:
                    this.aes = new AES(txtPlain.Text, this.Width, this.Height, output, labelVar);
                    break;
                case 13:
                    this.re = new NumericalIntegration(generation, attacker, server, function, labelMean, labelVar, this.Height, this.Width);
                    break;
            }
                    this.t = t;

            // Determina l'attacco selezionato
            // Creazione di un array o una lista per i RadioButton


            this.Invalidate();
            this.Paint += Form1_Paint;
            calculationComplete = true;

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (calculationComplete)
            {
                // Solo disegni necessari

            int[] result = new int[1];
                int[] result_t = new int[1];
             

                switch (homework)
                {
                    case 1: // Bernulli
                        this.graph.Create_Graphic(sender, e);
                        this.bernulli.Paint_Attack(sender, e);
                        result = bernulli.result;
                        result_t = bernulli.result_t;
                        this.graph.Paint_Distribution(sender, e, result, this.server);
                        this.graph.Create_Distribution_Graphic(sender, e, result, this.server);
                        this.graph.Paint_Distribution(sender, e, result_t, t);
                        this.graph.Create_Distribution_Graphic(sender, e, result_t, t);
                        break;
                    case 2: // RandomWalk
                        this.graph.Create_Graphic(sender, e);
                        this.randomWalk.Paint_Attack(sender, e);
                        result = randomWalk.result;
                        result_t = randomWalk.result_t;
                        this.graph.Paint_Distribution(sender, e, result, this.server);
                        this.graph.Create_Distribution_Graphic(sender, e, result, this.server);
                        this.graph.Paint_Distribution(sender, e, result_t, t);
                        this.graph.Create_Distribution_Graphic(sender, e, result_t, t);
                        break;
                    case 3: // RelFreq
                        this.graph.Create_Graphic(sender, e);
                        this.relfreq.Paint_Attack(sender, e);
                        result = relfreq.result;
                        result_t = relfreq.result_t;
                        this.graph.Paint_Distribution(sender, e, result, this.server);
                        this.graph.Create_Distribution_Graphic(sender, e, result, this.server);
                        this.graph.Paint_Distribution(sender, e, result_t, t);
                        this.graph.Create_Distribution_Graphic(sender, e, result_t, t);
                        break;
                    case 4: // Lambda
                        this.graph.Create_Graphic(sender, e);
                        this.berlambda.Paint_Attack(sender, e);
                        result = berlambda.result;
                        result_t = berlambda.result_t;
                        this.graph.Paint_Distribution(sender, e, result, this.server);
                        this.graph.Create_Distribution_Graphic(sender, e, result, this.server);
                        this.graph.Paint_Distribution(sender, e, result_t, t);
                        this.graph.Create_Distribution_Graphic(sender, e, result_t, t);
                        break;

                    case 5: // sqrN
                        this.graph.Create_Graphic(sender, e);
                        this.sqrn.Paint_Attack(sender, e);
                        result = sqrn.result;
                        result_t = sqrn.result_t;
                        this.graph.Paint_Distribution(sender, e, result, this.server);
                        this.graph.Create_Distribution_Graphic(sender, e, result, this.server);
                        this.graph.Paint_Distribution(sender, e, result_t, t);
                        this.graph.Create_Distribution_Graphic(sender, e, result_t, t);
                        break;
                    case 6:
                        this.graph.Create_Graphic(sender, e);
                        this.tae.Create_Distribution(sender, e);
                        break;
                    case 7:
                        this.graph.Create_Graphic(sender, e);
                        this.mtae.Create_Distribution(sender, e);
                        break;
                    case 8:
                        this.cr.DrawDistributions(sender, e);
                        break;
                    case 9:
                        
                        this.cf.AnalyzeAndDecrypt(sender, e);
                    break;
                    case 10:
                        
                        this.rsa.AnalyzeAndDecrypt(sender, e);
                        break;
                    case 11:
                        this.varMtae.Create_Distribution(sender, e);
                        break;
                    case 12:
                        this.aes.AnalyzeAndDecrypt(sender, e);
                        break;
                    case 13:

                        this.re.Paint(sender, e);
                        break;

                }

                calculationComplete = false;
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
            txttime.Enabled = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked || radioButton10.Checked;
            txtserver.Enabled = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked ||radioButton8.Checked || radioButton13.Checked; ;
            txtattacker.Enabled = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked || radioButton6.Checked || radioButton7.Checked || radioButton13.Checked|| radioButton11.Checked || radioButton13.Checked; // Sempre abilitato
            txtGen.Enabled = radioButton6.Checked || radioButton7.Checked || radioButton10.Checked || radioButton11.Checked || radioButton13.Checked; 
            txtPlain.Enabled = radioButton9.Checked || radioButton10.Checked || radioButton12.Checked;
            functionSelector.Enabled = radioButton13.Checked;
            // Mostra/nasconde i controlli in base al RadioButton selezionato
            txtprobability.Visible = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked ;
            txtlambda.Visible = radioButton4.Checked;
            txttime.Visible = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked || radioButton10.Checked; 
            txtserver.Visible = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked || radioButton8.Checked || radioButton13.Checked;
            txtattacker.Visible = radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked || radioButton6.Checked || radioButton7.Checked || radioButton11.Checked || radioButton13.Checked;
            txtGen.Visible = radioButton6.Checked || radioButton7.Checked || radioButton10.Checked || radioButton11.Checked || radioButton13.Checked; 
            txtPlain.Visible = txtPlain.Enabled;
            functionSelector.Visible = radioButton13.Checked;

            // Aggiorna le etichette associate (se necessario)
            label3.Visible = txtprobability.Visible;
            label5.Visible = txtlambda.Visible;
            label4.Visible = txttime.Visible;
            label1.Visible = txtserver.Visible;
            label2.Visible = txtattacker.Visible;
            label6.Visible = txtGen.Visible;
            label7.Visible = txtPlain.Visible;
            label8.Visible = functionSelector.Visible;
            output.Visible = radioButton9.Checked;

            // Modifica i valori delle etichette in base al RadioButton selezionato
            if (radioButton6.Checked || radioButton7.Checked || radioButton11.Checked)
            {
                label2.Text = "Samples";
            }
            else if (radioButton10.Checked)
            {
                label4.Text = "P";
                label6.Text = "Exponent";
            }
            else if (radioButton13.Checked)
            {
                label6.Text = "Lower";
                label2.Text = "Upper";
                label1.Text = "Intervals";
            }
            else
            {
                // Valori predefiniti per le etichette
                label2.Text = "Path";
                label4.Text = "T";
                label6.Text = "Generation";
                label1.Text = "Server"; // Predefinito per il server
            }

            // Configura la visualizzazione speciale per labelMean se radioButton9 è selezionato
            if (radioButton9.Checked)
            {
                labelMean.AutoSize = false;
                labelMean.MaximumSize = new Size(20, 10);
            }
            else
            {
                labelMean.AutoSize = true; // Torna al comportamento predefinito
                labelMean.MaximumSize = Size.Empty; // Nessuna dimensione massima
            }
        }

        private void InitializeFunctions()
        {
            // Map function names to actual functions
            functions = new Dictionary<string, Func<double, double>>()
            {
                { "Sin(x)", x => Math.Sin(x) },
                { "Cos(x)", x => Math.Cos(x) },
                { "Exp(x)", x => Math.Exp(x) },
                { "x^2", x => Math.Pow(x, 2) },
                { "Log(x)", x => Math.Log(x) }, // Logaritmo naturale
                { "Abs(x)", x => Math.Abs(x) }, // Valore assoluto<w
                { "Exp(-x)", x => Math.Exp(-x) }, // Esponenziale negativo
            };
            
            foreach (var key in functions.Keys)
            {
                functionSelector.Items.Add(key);
            }

            // Set default selection
            functionSelector.SelectedIndex = 0;
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
            radioButton7.CheckedChanged += radioButton_CheckedChanged;
            radioButton8.CheckedChanged += radioButton_CheckedChanged;
            radioButton9.CheckedChanged += radioButton_CheckedChanged;
            radioButton10.CheckedChanged += radioButton_CheckedChanged;
            radioButton11.CheckedChanged += radioButton_CheckedChanged;
            radioButton12.CheckedChanged += radioButton_CheckedChanged;
            radioButton13.CheckedChanged += radioButton_CheckedChanged;
            HandleInputFields(); // Configura lo stato iniziale
            InitializeFunctions();
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

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void output_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_ClickToShowFullText(object sender, EventArgs e)
        {
          
        }

    }
}
