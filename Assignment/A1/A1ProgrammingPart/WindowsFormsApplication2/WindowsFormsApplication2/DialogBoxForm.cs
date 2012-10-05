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
    public partial class DialogBoxForm : Form
    {
        Size formsize;

        public DialogBoxForm()
        {
            InitializeComponent();
            formsize = new Size(423, 190);
        }

        private void onClickExist(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void onClickDetail(object sender, EventArgs e)
        {
            if( Size == formsize )
            {
                Size = new Size(423, 290);
                bt_Details.Text = "▲Details";
            }
            else
            {
                Size = new Size(423, 190);
                bt_Details.Text = "▼Details";
            }
        }
    }
}
