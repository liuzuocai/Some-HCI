using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Drawing
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Display every points that the mouse passes by
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Graphics g = this.CreateGraphics();
            g.FillEllipse(Brushes.DarkBlue, e.X, e.Y, 15, 15);
        }
    }
}
