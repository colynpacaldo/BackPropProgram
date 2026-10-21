using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodingBackProp;

namespace BackPropForm
{
    public partial class Form1 : Form
    {
        private ComboBox cboError;
        private TextBox txtEpochs, txtLearn, txtMomentum, txtDecay, txtOutput;
        private Button btnRun;

        public Form1()
        {
            InitializeComponent(); 
            BuildUi();                      
            Console.SetOut(new TextBoxWriter(txtOutput)); 
        }

        private void BuildUi()
        {
            Text = "Back-Propagation Demo";
            ClientSize = new Size(860, 600);

            txtOutput = new TextBox();
            txtOutput.Multiline = true;
            txtOutput.ReadOnly = true;
            txtOutput.ScrollBars = ScrollBars.Both;
            txtOutput.WordWrap = false;
            txtOutput.Font = new Font("Consolas", 9.5f);
            txtOutput.Dock = DockStyle.Fill;
            Controls.Add(txtOutput);

            FlowLayoutPanel top = new FlowLayoutPanel();
            top.Dock = DockStyle.Top;
            top.Height = 78;
            top.Padding = new Padding(8);
            Controls.Add(top);

            cboError = new ComboBox();
            cboError.DropDownStyle = ComboBoxStyle.DropDownList;
            cboError.Items.Add("Mean squared error");
            cboError.Items.Add("Cross entropy error");
            cboError.SelectedIndex = 1;
            cboError.Width = 150;

            txtEpochs = new TextBox(); txtEpochs.Text = "1000"; txtEpochs.Width = 60;
            txtLearn = new TextBox(); txtLearn.Text = "0.05"; txtLearn.Width = 60;
            txtMomentum = new TextBox(); txtMomentum.Text = "0.01"; txtMomentum.Width = 60;
            txtDecay = new TextBox(); txtDecay.Text = "0.00001"; txtDecay.Width = 70;

            btnRun = new Button();
            btnRun.Text = "Train";
            btnRun.Width = 90;
            btnRun.Click += btnRun_Click;

            AddField(top, "Error:", cboError);
            AddField(top, "Epochs:", txtEpochs);
            AddField(top, "Learn rate:", txtLearn);
            AddField(top, "Momentum:", txtMomentum);
            AddField(top, "Weight decay:", txtDecay);
            top.Controls.Add(btnRun);

            txtOutput.BringToFront(); 
        }

        private static void AddField(FlowLayoutPanel panel, string label, Control c)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.AutoSize = true;
            lbl.Margin = new Padding(8, 7, 2, 0);
            panel.Controls.Add(lbl);
            panel.Controls.Add(c);
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            int epochs;
            double learnRate, momentum, decay;
            CultureInfo ci = CultureInfo.InvariantCulture; 
            if (!int.TryParse(txtEpochs.Text, NumberStyles.Integer, ci, out epochs) || epochs < 10 ||
                !double.TryParse(txtLearn.Text, NumberStyles.Float, ci, out learnRate) ||
                !double.TryParse(txtMomentum.Text, NumberStyles.Float, ci, out momentum) ||
                !double.TryParse(txtDecay.Text, NumberStyles.Float, ci, out decay))
            {
                MessageBox.Show("Please enter valid numbers (epochs must be at least 10).");
                return;
            }

            ErrorType et = (cboError.SelectedIndex == 0) ?
              ErrorType.MeanSquared : ErrorType.CrossEntropy;

            btnRun.Enabled = false;
            txtOutput.Clear();
            try
            {
                await Task.Run(() => CodingBackProp.BackPropProgram.RunDemo(et, decay, epochs, learnRate, momentum));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnRun.Enabled = true;
        }
    }

    public class TextBoxWriter : TextWriter
    {
        private readonly TextBox box;
        public TextBoxWriter(TextBox box) { this.box = box; }

        public override Encoding Encoding { get { return Encoding.UTF8; } }

        public override void Write(char value) { Write(value.ToString()); }

        public override void Write(char[] buffer, int index, int count)
        {
            Write(new string(buffer, index, count));
        }

        public override void Write(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            if (box.InvokeRequired)
                box.BeginInvoke(new Action(delegate { box.AppendText(value); }));
            else
                box.AppendText(value);
        }
    }
}