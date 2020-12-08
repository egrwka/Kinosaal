using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kinosaal_Oleynik
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1("TENET");
            form.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1("F9");
            form.Show();
        }
    }
}
