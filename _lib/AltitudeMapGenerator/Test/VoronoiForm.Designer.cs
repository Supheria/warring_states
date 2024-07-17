/*
 * Created by SharpDevelop.
 * User: Burhan
 * Date: 11/05/2014
 * Time: 01:02 ص
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace AltitudeMapGenerator.Test
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
            numericUpDown3 = new NumericUpDown();
            numericUpDown4 = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)pb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(14, 239);
            button1.Margin = new Padding(4);
            button1.Name = "button1";
            button1.Size = new Size(122, 35);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1Click;
            // 
            // pb
            // 
            pb.Location = new Point(192, 17);
            pb.Margin = new Padding(4);
            pb.Name = "pb";
            pb.Size = new Size(1225, 1488);
            pb.TabIndex = 1;
            pb.TabStop = false;
            pb.MouseMove += PbMouseMove;
            // 
            // label1
            // 
            label1.Location = new Point(16, 42);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(142, 23);
            label1.TabIndex = 2;
            label1.Text = "label1";
            label1.AutoSize = true;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(13, 175);
            numericUpDown1.Margin = new Padding(4);
            numericUpDown1.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(144, 23);
            numericUpDown1.TabIndex = 5;
            numericUpDown1.ValueChanged += NumericUpDown1ValueChanged;
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new Point(13, 109);
            numericUpDown2.Margin = new Padding(4);
            numericUpDown2.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(144, 23);
            numericUpDown2.TabIndex = 6;
            numericUpDown2.ValueChanged += NumericUpDown2ValueChanged;
            // 
            // numericUpDown3
            // 
            numericUpDown3.Location = new Point(13, 358);
            numericUpDown3.Margin = new Padding(4);
            numericUpDown3.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown3.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.Size = new Size(72, 23);
            numericUpDown3.TabIndex = 7;
            numericUpDown3.ValueChanged += NumericUpDown3_ValueChanged;
            // 
            // numericUpDown4
            // 
            numericUpDown4.Location = new Point(96, 358);
            numericUpDown4.Margin = new Padding(4);
            numericUpDown4.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown4.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown4.Name = "numericUpDown4";
            numericUpDown4.Size = new Size(72, 23);
            numericUpDown4.TabIndex = 8;
            numericUpDown4.ValueChanged += NumericUpDown3_ValueChanged;
            // 
            // VoronoiForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1374, 1061);
            Controls.Add(numericUpDown4);
            Controls.Add(numericUpDown3);
            Controls.Add(numericUpDown2);
            Controls.Add(numericUpDown1);
            Controls.Add(label1);
            Controls.Add(pb);
            Controls.Add(button1);
            Margin = new Padding(4);
            Name = "VoronoiForm";
            Text = "Voronoi";
            ((System.ComponentModel.ISupportInitialize)pb).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pb;
		private System.Windows.Forms.Button button1;
        private NumericUpDown numericUpDown2;
        private NumericUpDown numericUpDown3;
        private NumericUpDown numericUpDown4;
    }
}
