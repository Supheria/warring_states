/*
 * Created by SharpDevelop.
 * User: Burhan
 * Date: 11/05/2014
 * Time: 01:02 ص
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace DlaTest
{
	partial class VoronoiForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            pb = new PictureBox();
            label1 = new Label();
            numericUpDown1 = new NumericUpDown();
            numericUpDown2 = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)pb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(14, 239);
            button1.Margin = new Padding(4, 4, 4, 4);
            button1.Name = "button1";
            button1.Size = new Size(122, 35);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1Click;
            // 
            // pb
            // 
            pb.Location = new System.Drawing.Point(192, 17);
            pb.Margin = new Padding(4, 4, 4, 4);
            pb.Name = "pb";
            pb.Size = new Size(1225, 1488);
            pb.TabIndex = 1;
            pb.TabStop = false;
            pb.MouseMove += PbMouseMove;
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(16, 42);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(142, 23);
            label1.TabIndex = 2;
            label1.Text = "label1";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new System.Drawing.Point(13, 175);
            numericUpDown1.Margin = new Padding(4, 4, 4, 4);
            numericUpDown1.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(144, 23);
            numericUpDown1.TabIndex = 5;
            numericUpDown1.ValueChanged += NumericUpDown1ValueChanged;
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new System.Drawing.Point(13, 109);
            numericUpDown2.Margin = new Padding(4);
            numericUpDown2.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(144, 23);
            numericUpDown2.TabIndex = 6;
            numericUpDown2.ValueChanged += NumericUpDown2ValueChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1374, 1061);
            Controls.Add(numericUpDown2);
            Controls.Add(numericUpDown1);
            Controls.Add(label1);
            Controls.Add(pb);
            Controls.Add(button1);
            Margin = new Padding(4, 4, 4, 4);
            Name = "MainForm";
            Text = "Voronoi";
            ((System.ComponentModel.ISupportInitialize)pb).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pb;
		private System.Windows.Forms.Button button1;
        private NumericUpDown numericUpDown2;
    }
}
