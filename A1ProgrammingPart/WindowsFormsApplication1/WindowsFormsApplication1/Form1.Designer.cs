namespace WindowsFormsApplication1
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS302",
            "HCI",
            "PPI",
            "80"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS320",
            "Database",
            "MIA",
            "95"}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS255",
            "Algorithms",
            "NKA",
            "45"}, -1);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS101",
            "Intro to CS",
            "John Braico",
            "101"}, -1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS102",
            "Intro to CS II",
            "John Braico",
            "98"}, -1);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS214",
            "Hellen Camera",
            "Data Structures",
            "82"}, -1);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS216",
            "Programming Practices",
            "John Braico",
            "78"}, -1);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS213",
            "Discrete Mathematics",
            "Terry Andress",
            "150"}, -1);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS208",
            "Analysis of Algorithms",
            "Randy Copper",
            "50"}, -1);
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS215",
            "Object Orientation",
            "John Anderson",
            "97"}, -1);
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS228",
            "Introduction to Computer Systems",
            "Randy Copper",
            "40"}, -1);
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS301",
            "Distributed Computing",
            "Michael Zapp",
            "30"}, -1);
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS303",
            "Automata Theory",
            "Helen Camera",
            "55"}, -1);
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS304",
            "Technical Communication in Computer Science",
            "Christina Penner",
            "59"}, -1);
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS317",
            "Analysis of Algorithms and Data Structures",
            "Neil Bruce",
            "70"}, -1);
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS335",
            "Software Engineering 1",
            "David Scuse",
            "60"}, -1);
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS337",
            "Computer Organization",
            "Parimala Thulasiraman, ",
            "100"}, -1);
            System.Windows.Forms.ListViewItem listViewItem18 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS338",
            "Databases: Concepts and Usage",
            "Leung, Carson Kai-Sang",
            "120"}, -1);
            System.Windows.Forms.ListViewItem listViewItem19 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS343",
            "Operating Systems",
            "Micheal Zapp",
            "66"}, -1);
            System.Windows.Forms.ListViewItem listViewItem20 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS372",
            "Computer Networks 1",
            "Ben Li",
            "88"}, -1);
            System.Windows.Forms.ListViewItem listViewItem21 = new System.Windows.Forms.ListViewItem(new string[] {
            "CS471",
            "Introduction to Data Mining",
            "Leung, Carson Kai-Sang",
            "100"}, -1);
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.GridLines = true;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15,
            listViewItem16,
            listViewItem17,
            listViewItem18,
            listViewItem19,
            listViewItem20,
            listViewItem21});
            this.listView1.Location = new System.Drawing.Point(0, 1);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(400, 258);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Course Number";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Instructor";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "# of Student";
            this.columnHeader4.Width = 100;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 260);
            this.Controls.Add(this.listView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}

