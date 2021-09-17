using System;
using System.Windows.Forms;

namespace CloudConvertApp
{
    public class Logger
    {
        private RichTextBox textbox;

        public Logger(RichTextBox textbox)
        {
            this.textbox = textbox;
        }
        
        public void Log(string message, string args = null)
        {
            
            textbox.AppendText($"{(textbox.Text.Length == 0 ? "" : "\n")}{message} {(args == null ? "" : "(" + args + ")")}");
            
        }
    }
}