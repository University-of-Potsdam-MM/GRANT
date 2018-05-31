using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Reflection;

namespace TactileWeb
{
    public static class Program
    {
        /// <summary>Returns our name. MVBD</summary>
        public static string Name;

        /// <summary>Returns our version as string. 89</summary>
        public static string Version;

        //Capture des Screen
        public static ScreenCapture ScreenCapture;


        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Name = "TactileWeb";                                                               // Muss am Anfang gesetzt sein!!!
            Version = Assembly.GetEntryAssembly().GetName().Version.Revision.ToString();    // "89"

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ScreenCapture = new ScreenCapture();

            Application.Run(new Form_TactileWeb());
        }

        /// <summary>Gibt an ob die Anwendung (Control) gerade im VisualStudio (true) läuft oder als Anwendung (false) ausgeführt wird</summary>
        public static bool IsVisualStudio()
        {
            string name = System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower();
            return ((name == "devenv") || (name == "wdexpress"));
        }

    }
}
