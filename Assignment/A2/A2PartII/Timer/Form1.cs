using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Timer
{
    public partial class Form1 : Form
    {
        private int _index = 0; //index of each picture
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000; // set interval to 10 seconds
            Image[] images = new Image[8];
            pictureBox1.Image = imageList1.Images[_index];
            _index++;
            if (_index == 8)
            {
                timer1.Stop();
            }
        }
    }
}
