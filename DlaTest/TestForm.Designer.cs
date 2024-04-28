namespace DlaTest
{
    partial class TestForm
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
            Progressor = new Label();
            SuspendLayout();
            // 
            // Progressor
            // 
            Progressor.AutoSize = true;
            Progressor.Location = new Point(373, 195);
            Progressor.Name = "Progressor";
            Progressor.Size = new Size(43, 17);
            Progressor.TabIndex = 0;
            Progressor.Text = "label1";
            // 
            // TestForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Progressor);
            Name = "TestForm";
            Text = "TestForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Progressor;
    }
}