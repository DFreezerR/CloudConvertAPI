using System;
using System.Windows.Forms;

namespace CloudConvertApp
{
    static class Program
    {
        public static FMain formMain = new FMain();
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