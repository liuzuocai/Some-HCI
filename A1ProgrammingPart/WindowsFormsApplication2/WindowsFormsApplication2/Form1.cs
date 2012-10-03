using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        Size formsize;
        public Form1()
        {
            InitializeComponent();
            formsize = new Size(423, 190);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if( Size == formsize )
            {
                Size = new Size(423, 290);
                button1.Text = "▲Details";
            }
            else
            {
                Size = new Size(423, 190);
                button1.Text = "▼Details";
            }
        }
    }
}
