using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gaussian_Quick_Output
{
    class ParameterControl : Control
    {
        public virtual object DefaultValue { get; set; }
        public class StringTextBox : TextBox
        {
            public string DefaultValue { get; set; }
        }
        public class IntTextBox : TextBox
        {
            public int DefaultValue { get; set; }

            private static void sanitizeInput(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)Keys.Back) //The  character represents a backspace
                {
                    e.Handled = false; //Do not reject the input
                }
                else
                {
                    e.Handled = true; //Reject the input
                }
            }
        }
    }
}
