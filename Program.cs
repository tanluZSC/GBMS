using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GBMSAPI_CS_Example
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GBMASPI_CsExampleMainForm());
        }
    }
}