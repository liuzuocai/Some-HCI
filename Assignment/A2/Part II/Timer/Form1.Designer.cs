namespace Timer
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this._pictureBox = new System.Windows.Forms.PictureBox();
			this._timer = new System.Windows.Forms.Timer(this.components);
			this._imageList = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// _pictureBox
			// 
			this._pictureBox.Location = new System.Drawing.Point(43, 40);
			this._pictureBox.Name = "_pictureBox";
			this._pictureBox.Size = new System.Drawing.Size(256, 192);
			this._pictureBox.TabIndex = 0;
			this._pictureBox.TabStop = false;
			// 
			// _timer
			// 
			this._timer.Interval = 1000;
			// 
			// _imageList
			// 
			this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
			this._imageList.TransparentColor = System.Drawing.Color.Transparent;
			this._imageList.Images.SetKeyName(0, "Chrysanthemum.jpg");
			this._imageList.Images.SetKeyName(1, "Desert.jpg");
			this._imageList.Images.SetKeyName(2, "Hydrangeas.jpg");
			this._imageList.Images.SetKeyName(3, "Jellyfish.jpg");
			this._imageList.Images.SetKeyName(4, "Koala.jpg");
			this._imageList.Images.SetKeyName(5, "Lighthouse.jpg");
			this._imageList.Images.SetKeyName(6, "Penguins.jpg");
			this._imageList.Images.SetKeyName(7, "Tulips.jpg");
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(383, 264);
			this.Controls.Add(this._pictureBox);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox _pictureBox;
		private System.Windows.Forms.Timer _timer;
		private System.Windows.Forms.ImageList _imageList;
	}
}

