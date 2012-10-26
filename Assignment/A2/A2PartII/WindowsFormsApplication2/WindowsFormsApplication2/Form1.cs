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
        private int index = 0;
		private int _maxIndex; //so we can loop. 
        
        public Form1()
        {
            InitializeComponent();
			_maxIndex = imageList1.Images.Count;

            timer1.Start();
            
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            //Image[] images = new Image[8]; //Never used...     
            pictureBox1.Image = imageList1.Images[index];
			index = (index + 1) % _maxIndex;
        }
    }
}
