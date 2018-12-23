using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communicator_LAN;

namespace Communicator_LAN
{
    public partial class Config_window : Form
    {
        public Config_window()
        {
            InitializeComponent();
            Warning_label.Text = "";

            ServerName_textBox.Name = VALID_FIELDS.SERVER_NAME;
            Password_textBox.Name   = VALID_FIELDS.SERVER_PASSWORD;
            MaxUsers_numeric.Name   = VALID_FIELDS.SERVER_MAXUSERS;
            Warning_label.Name      = VALID_FIELDS.WARNING_LABEL;
        }

        private void Proceed_button_Click(object sender, EventArgs e)
        {
            bool val = Validation.FieldsAreValid(this);
            if (val)
            {
                Main_window window = new Main_window(this);
                window.Show();
            }
        }

        private void TextBox_ContentChanged(object sender, EventArgs e)
        {
            Warning_label.Text = "OK";
            Warning_label.ForeColor = Color.DarkGreen;
            Validation.FieldsAreValid(this);
        }
    }
}
