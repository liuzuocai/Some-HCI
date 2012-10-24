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
        
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
            
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            Image[] images = new Image[8];     
            pictureBox1.Image = imageList1.Images[index]; 
            index++;
            if( index == 8){
                timer1.Stop();
            }
        }
    }
}
