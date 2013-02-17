using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace VideoReceiver
{
    public static class Program
    {
        public static Form1 f;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            f = new Form1();
            Application.Run(f);
        }
    }
}
