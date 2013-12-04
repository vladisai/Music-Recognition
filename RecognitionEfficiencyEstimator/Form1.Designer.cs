namespace RecognitionEfficiencyEstimator
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
            this.StartButton = new System.Windows.Forms.Button();
            this.ResultLabel = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.logBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(12, 12);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(87, 36);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // ResultLabel
            // 
            this.ResultLabel.AutoSize = true;
            this.ResultLabel.Location = new System.Drawing.Point(36, 246);
            this.ResultLabel.Name = "ResultLabel";
            this.ResultLabel.Size = new System.Drawing.Size(0, 13);
            this.ResultLabel.TabIndex = 1;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 54);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(580, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // logBox
            // 
            this.logBox.FormattingEnabled = true;
            this.logBox.Location = new System.Drawing.Point(12, 83);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(580, 147);
            this.logBox.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 268);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.ResultLabel);
            this.Controls.Add(this.StartButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Label ResultLabel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ListBox logBox;
    }
}

