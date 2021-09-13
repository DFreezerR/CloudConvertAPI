using System;
using System.Windows.Forms;

namespace ConvertAPI
{
    static class Program
    {
        public static Form1 formMain = new Form1();
        [STAThread]
        static void Main()
        {
            
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.Run(formMain);
        }
    }
}