namespace WindowsFormsApplication2
{
    partial class DialogBoxForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pb_Warning;
        private System.Windows.Forms.Button bt_Details;
        private System.Windows.Forms.Button bt_OK;
        private System.Windows.Forms.Button bt_Cancel;
        private System.Windows.Forms.Label lb_Warning;
        private System.Windows.Forms.Label lb_Description;

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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogBoxForm));
          this.pb_Warning = new System.Windows.Forms.PictureBox();
          this.bt_Details = new System.Windows.Forms.Button();
          this.bt_OK = new System.Windows.Forms.Button();
          this.bt_Cancel = new System.Windows.Forms.Button();
          this.lb_Warning = new System.Windows.Forms.Label();
          this.lb_Description = new System.Windows.Forms.Label();
          ((System.ComponentModel.ISupportInitialize)(this.pb_Warning)).BeginInit();
          this.SuspendLayout();
          // 
          // pb_Warning
          // 
          this.pb_Warning.Image = ((System.Drawing.Image)(resources.GetObject("pb_Warning.Image")));
          this.pb_Warning.Location = new System.Drawing.Point(12, 24);
          this.pb_Warning.Name = "pb_Warning";
          this.pb_Warning.Size = new System.Drawing.Size(60, 65);
          this.pb_Warning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
          this.pb_Warning.TabIndex = 0;
          this.pb_Warning.TabStop = false;
          // 
          // bt_Details
          // 
          this.bt_Details.Location = new System.Drawing.Point(16, 115);
          this.bt_Details.Name = "bt_Details";
          this.bt_Details.Size = new System.Drawing.Size(84, 27);
          this.bt_Details.TabIndex = 2;
          this.bt_Details.Text = "▼Details";
          this.bt_Details.UseVisualStyleBackColor = true;
          this.bt_Details.Click += new System.EventHandler(this.onClickDetail);
          // 
          // bt_OK
          // 
          this.bt_OK.Location = new System.Drawing.Point(179, 115);
          this.bt_OK.Name = "bt_OK";
          this.bt_OK.Size = new System.Drawing.Size(84, 27);
          this.bt_OK.TabIndex = 3;
          this.bt_OK.Text = "OK";
          this.bt_OK.UseVisualStyleBackColor = true;
          this.bt_OK.Click += new System.EventHandler(this.onClickExist);
          // 
          // bt_Cancel
          // 
          this.bt_Cancel.Location = new System.Drawing.Point(269, 115);
          this.bt_Cancel.Name = "bt_Cancel";
          this.bt_Cancel.Size = new System.Drawing.Size(84, 27);
          this.bt_Cancel.TabIndex = 4;
          this.bt_Cancel.Text = "Cancel";
          this.bt_Cancel.UseVisualStyleBackColor = true;
          this.bt_Cancel.Click += new System.EventHandler(this.onClickExist);
          // 
          // lb_Warning
          // 
          this.lb_Warning.AutoSize = true;
          this.lb_Warning.Location = new System.Drawing.Point(105, 49);
          this.lb_Warning.Name = "lb_Warning";
          this.lb_Warning.Size = new System.Drawing.Size(108, 13);
          this.lb_Warning.TabIndex = 5;
          this.lb_Warning.Text = "Invalid property value";
          // 
          // lb_Description
          // 
          this.lb_Description.Location = new System.Drawing.Point(14, 167);
          this.lb_Description.Name = "lb_Description";
          this.lb_Description.Size = new System.Drawing.Size(367, 90);
          this.lb_Description.TabIndex = 6;
          this.lb_Description.Text = resources.GetString("lb_Description.Text");
          // 
          // DialogBoxForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(407, 165);
          this.Controls.Add(this.lb_Description);
          this.Controls.Add(this.lb_Warning);
          this.Controls.Add(this.bt_Cancel);
          this.Controls.Add(this.bt_OK);
          this.Controls.Add(this.bt_Details);
          this.Controls.Add(this.pb_Warning);
          this.Name = "DialogBoxForm";
          this.Text = "Property Window";
          ((System.ComponentModel.ISupportInitialize)(this.pb_Warning)).EndInit();
          this.ResumeLayout(false);
          this.PerformLayout();

        }
        #endregion

    }
}

