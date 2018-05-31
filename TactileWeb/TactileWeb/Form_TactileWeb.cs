using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TactileWeb
{
    public partial class Form_TactileWeb : Form
    {
        public Form_TactileWeb()
        {
            InitializeComponent();
        }

        private void Form_TactileWeb_Load(object sender, EventArgs e)
        {
            // Try: Icon ruft nie das Load auf. Alles in den Konstruktor!
        }

        private int _tmrCounter;

        private void tmrRefresh_Tick(object sender, EventArgs e)
        {
            if (Program.IsVisualStudio() == true) return; // Problems in FormMain

            _tmrCounter++;

            if (_tmrCounter > 5)
            {
                //// Refresh the UI (controls)
                Timer_Tick();
            }
        }

        public void Timer_Tick()
        {
            UIAText_Changer();
            //txtUIAInfo.Text = "aa";
        }

        public void UIAText_Changer()
        {
            // UIA Info
            //if (txtUIAInfo.Text != "dd") txtUIAInfo.Text = "dd";

            //Program.ScreenCapture.UIAInfo.Text) txtUIAInfo.Text = Program.ScreenCapture.UIAInfo.Text;

            // UIA Info
            if (txtUIAInfo.Text != Program.ScreenCapture.UIAInfo.Text) txtUIAInfo.Text = Program.ScreenCapture.UIAInfo.Text;

        }

    }
}
