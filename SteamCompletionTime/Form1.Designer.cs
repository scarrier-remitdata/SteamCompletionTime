namespace SteamCompletionTime
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
            this.label1 = new System.Windows.Forms.Label();
            this.steamIdTextBox = new System.Windows.Forms.TextBox();
            this.getListButton = new System.Windows.Forms.Button();
            this.resultsTextBox = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Steam User Id:";
            // 
            // steamIdTextBox
            // 
            this.steamIdTextBox.Location = new System.Drawing.Point(95, 9);
            this.steamIdTextBox.Name = "steamIdTextBox";
            this.steamIdTextBox.Size = new System.Drawing.Size(175, 20);
            this.steamIdTextBox.TabIndex = 1;
            // 
            // getListButton
            // 
            this.getListButton.Location = new System.Drawing.Point(276, 9);
            this.getListButton.Name = "getListButton";
            this.getListButton.Size = new System.Drawing.Size(75, 23);
            this.getListButton.TabIndex = 2;
            this.getListButton.Text = "Get List";
            this.getListButton.UseVisualStyleBackColor = true;
            this.getListButton.Click += new System.EventHandler(this.GetListButton_Click);
            // 
            // resultsTextBox
            // 
            this.resultsTextBox.Location = new System.Drawing.Point(15, 35);
            this.resultsTextBox.Multiline = true;
            this.resultsTextBox.Name = "resultsTextBox";
            this.resultsTextBox.ReadOnly = true;
            this.resultsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.resultsTextBox.Size = new System.Drawing.Size(444, 333);
            this.resultsTextBox.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 380);
            this.Controls.Add(this.resultsTextBox);
            this.Controls.Add(this.getListButton);
            this.Controls.Add(this.steamIdTextBox);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Steam Games Completion Time";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox steamIdTextBox;
        private System.Windows.Forms.Button getListButton;
        private System.Windows.Forms.TextBox resultsTextBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

