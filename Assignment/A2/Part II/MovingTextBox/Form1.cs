using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MovingTextBox
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			_textBox.TextChanged += new EventHandler(_textBox_TextChanged);
		}

		void _textBox_TextChanged(object sender, EventArgs e)
		{
			int shiftValue = 0;

			//Make sure we have at least a letter and a number
			if (_textBox.Text.Length >= 2)
			{
				//find out if we're shifting left or right
				if (_textBox.Text[0] == 'l' || _textBox.Text[0] == 'L')
				{
					shiftValue = -1;
				}
				else if(_textBox.Text[0] == 'r' || _textBox.Text[0] == 'R')
				{
					shiftValue = 1;
				}

				//If we're shifting
				if (shiftValue != 0)
				{
					try
					{
						shiftValue *= int.Parse(_textBox.Text.Substring(1));
						_textBox.Location = new Point(_textBox.Location.X + shiftValue, _textBox.Location.Y); //Location is of type point. You can't modify points and need to create new ones.
						_textBox.Text = ""; //try commenting this out and seeing how the behaviour changes
					}
					catch (FormatException)
					{ }
					catch (ArgumentNullException)
					{ }

				}
			}

		}
	}
}
