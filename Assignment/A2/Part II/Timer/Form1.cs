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

		/// <summary>
		/// Get or set the index of the current image
		/// </summary>
		private int CurrentImage
		{
			get { return _currentImage; }
			set
			{
				_currentImage = (_currentImage + 1) % _numberOfImages;
			}
		}

		public Form1()
		{
			//Initialize
			InitializeComponent();
			_numberOfImages = _imageList.Images.Count;

			_timer.Enabled = true;
			_timer.Tick += new EventHandler(_timer_Tick);

			//Load first image
			LoadCurrentImage();
		}

		/// <summary>
		/// Loads the imgae indicated by the class variable "_currentImage". This value be >= _number of images
		/// </summary>
		private void LoadCurrentImage()
		{
			_pictureBox.Image = _imageList.Images[_currentImage];
		}

		void _timer_Tick(object sender, EventArgs e)
		{
			CurrentImage++;
			LoadCurrentImage();
		}
	}
}
