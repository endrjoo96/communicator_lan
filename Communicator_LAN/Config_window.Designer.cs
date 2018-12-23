namespace Communicator_LAN
{
    partial class Config_window
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.Proceed_button = new System.Windows.Forms.Button();
            this.ServerName_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Password_textBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.MaxUsers_numeric = new System.Windows.Forms.NumericUpDown();
            this.Warning_label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.MaxUsers_numeric)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14F);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "Konfiguracja serwera";
            // 
            // Proceed_button
            // 
            this.Proceed_button.Location = new System.Drawing.Point(215, 142);
            this.Proceed_button.Name = "Proceed_button";
            this.Proceed_button.Size = new System.Drawing.Size(75, 23);
            this.Proceed_button.TabIndex = 4;
            this.Proceed_button.Text = "Dalej...";
            this.Proceed_button.UseVisualStyleBackColor = true;
            this.Proceed_button.Click += new System.EventHandler(this.Proceed_button_Click);
            // 
            // ServerName_textBox
            // 
            this.ServerName_textBox.Location = new System.Drawing.Point(104, 39);
            this.ServerName_textBox.Name = "ServerName_textBox";
            this.ServerName_textBox.Size = new System.Drawing.Size(186, 20);
            this.ServerName_textBox.TabIndex = 1;
            this.ServerName_textBox.TextChanged += new System.EventHandler(this.TextBox_ContentChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Nazwa serwera:";
            // 
            // Password_textBox
            // 
            this.Password_textBox.Location = new System.Drawing.Point(104, 65);
            this.Password_textBox.Name = "Password_textBox";
            this.Password_textBox.PasswordChar = '•';
            this.Password_textBox.Size = new System.Drawing.Size(186, 20);
            this.Password_textBox.TabIndex = 2;
            this.Password_textBox.TextChanged += new System.EventHandler(this.TextBox_ContentChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Hasło:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Maks. klientów:";
            // 
            // MaxUsers_numeric
            // 
            this.MaxUsers_numeric.Location = new System.Drawing.Point(104, 91);
            this.MaxUsers_numeric.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.MaxUsers_numeric.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.MaxUsers_numeric.Name = "MaxUsers_numeric";
            this.MaxUsers_numeric.Size = new System.Drawing.Size(42, 20);
            this.MaxUsers_numeric.TabIndex = 3;
            this.MaxUsers_numeric.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // Warning_label
            // 
            this.Warning_label.AutoSize = true;
            this.Warning_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Warning_label.ForeColor = System.Drawing.Color.Black;
            this.Warning_label.Location = new System.Drawing.Point(12, 119);
            this.Warning_label.Name = "Warning_label";
            this.Warning_label.Size = new System.Drawing.Size(72, 15);
            this.Warning_label.TabIndex = 7;
            this.Warning_label.Text = "Ostrzeżenia";
            // 
            // Config_window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 177);
            this.Controls.Add(this.Warning_label);
            this.Controls.Add(this.MaxUsers_numeric);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Password_textBox);
            this.Controls.Add(this.ServerName_textBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Proceed_button);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimumSize = new System.Drawing.Size(318, 216);
            this.Name = "Config_window";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ekran konfiguracji";
            ((System.ComponentModel.ISupportInitialize)(this.MaxUsers_numeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Proceed_button;
        public System.Windows.Forms.TextBox ServerName_textBox;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox Password_textBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown MaxUsers_numeric;
        public System.Windows.Forms.Label Warning_label;
    }
}

