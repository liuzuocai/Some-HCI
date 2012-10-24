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
		private int _currentImage = 0;
		private int _numberOfImages;

		public Form1()
		{
			InitializeComponent();
			_numberOfImages = _imageList.Images.Count;

			_timer.Enabled = true;
			_timer.Tick += new EventHandler(_timer_Tick);

			LoadCurrentImage();
		}

		private void LoadCurrentImage()
		{
			_pictureBox.Image = _imageList.Images[_currentImage];
		}

		void _timer_Tick(object sender, EventArgs e)
		{
			_currentImage = (_currentImage + 1) % _numberOfImages;
			LoadCurrentImage();
		}
	}
}
