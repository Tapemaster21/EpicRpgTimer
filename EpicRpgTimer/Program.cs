using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EpicRpgTimer
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
            Form1 f = new Form1();
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            f.Text = "EPIC RPG Timer - v" + v.Major + "." + v.Minor;
            f.Height = 325;
            Application.Run(f);

        }
    }
}
