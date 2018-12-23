using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Communicator_LAN
{
    static class VALID_FIELDS
    {
        public const string SERVER_NAME = "ServerName_textBox";
        public const string SERVER_PASSWORD = "Password_textBox";
        public const string SERVER_MAXUSERS = "MaxUsers_numeric";
        public const string WARNING_LABEL = "Warning_label";
    }
    class Validation
    {
        internal static bool FieldsAreValid(Form parentForm)
        {
            Label warning = null;
            foreach (Control c in parentForm.Controls)
            {
                if (c.GetType() == typeof(Label))
                {
                    if (c.Name == VALID_FIELDS.WARNING_LABEL)
                    {
                        warning = c as Label;
                        break;
                    }
                }
            }
            foreach (Control c in parentForm.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    switch (c.Name)
                    {
                        case VALID_FIELDS.SERVER_PASSWORD:
                        {
                            if ((c as TextBox).Text == "")
                            {
                                warning.Text = "Serwer nie będzie zabezpieczony hasłem.";
                                warning.ForeColor = System.Drawing.Color.DarkOrange;
                            }
                            break;
                        }
                        case VALID_FIELDS.SERVER_NAME:
                        {
                            if((c as TextBox).Text=="")
                            {
                                warning.Text = "Nie ustawiono nazwy serwera.";
                                warning.ForeColor = System.Drawing.Color.Red;
                                return false;
                            }
                            break;
                        }
                    }
                }
            }
            return true;
        }
    }
}
