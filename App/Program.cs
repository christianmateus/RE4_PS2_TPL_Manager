using System;
using System.Windows.Forms;

namespace RE4_PS2_TPL_Manager
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0)
            {
                Application.Run(new FrmMain(args[0]));
            }
            else
            {
                Application.Run(new FrmMain());
            }
        }
    }
}
