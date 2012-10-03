using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
    }
}
