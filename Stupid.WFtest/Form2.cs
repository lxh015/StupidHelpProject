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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(this.textBox1.Text))
            {
                Form1 frm = new Form1();



                frm.setText += frm.GoSet;

                
                frm.Show();
            }
            else
            {
                MessageBox.Show("信息不能为空！");
            }
        }

        public void Close(string text)
        {

        }
    }
}
