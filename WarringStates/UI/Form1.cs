using WarringStates.Loop;

namespace WarringStates.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TimeLoop.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TimeLoop.Start();
        }
    }
}
