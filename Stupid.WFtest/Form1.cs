using Stupid.WFtest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stupid.WFtest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public delegate void SetText(string text);

        public event SetText setText;

        public void GoSet(string text)
        {

            this.richTextBox1.Text = text;
        }


        public void GO(string text)
        {
            if (setText != null)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.Show();
            this.Hide();
        }
    }
}
