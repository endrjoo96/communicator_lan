namespace Communicator_LAN
{
    partial class Main_window
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
            this.IP_textBox = new System.Windows.Forms.Label();
            this.UsersPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Adres IP serwera:";
            // 
            // IP_textBox
            // 
            this.IP_textBox.AutoSize = true;
            this.IP_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.IP_textBox.Location = new System.Drawing.Point(13, 30);
            this.IP_textBox.Name = "IP_textBox";
            this.IP_textBox.Size = new System.Drawing.Size(124, 26);
            this.IP_textBox.TabIndex = 1;
            this.IP_textBox.Text = "Pobieram...";
            // 
            // UsersPanel
            // 
            this.UsersPanel.AutoScroll = true;
            this.UsersPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UsersPanel.Location = new System.Drawing.Point(12, 60);
            this.UsersPanel.Name = "UsersPanel";
            this.UsersPanel.Size = new System.Drawing.Size(215, 214);
            this.UsersPanel.TabIndex = 2;
            // 
            // Main_window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(239, 286);
            this.Controls.Add(this.UsersPanel);
            this.Controls.Add(this.IP_textBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(255, 325);
            this.Name = "Main_window";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main_window";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_window_FormClosing);
            this.Shown += new System.EventHandler(this.Main_window_Shown);
            this.Resize += new System.EventHandler(this.Main_window_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label IP_textBox;
        private System.Windows.Forms.Panel UsersPanel;
    }
}