using System;
using System.Windows.Forms;


namespace TactileWeb.RawInput
{

    public delegate void RawDeviceMouseEventHandler(object sender, RawInputDeviceMouseEventArgs e);


    /// <summary>Erweiterte MouseEventArgs mit dem Handle des Geräts und ob die Anwendung im Foredergrund war</summary>
    public class RawInputDeviceMouseEventArgs : MouseEventArgs
    {
        protected IntPtr    _rawDevice;
        protected bool      _foreground;

        public RawInputDeviceMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta,   IntPtr rawDevice, bool foreground) : base(button,clicks,x,y,delta)
        {
            _rawDevice  = rawDevice;
            _foreground = foreground;
        }

        /// <summary>Handle des Geräts</summary>
        public IntPtr RawDevice     { get { return _rawDevice; } }

        /// <summary>War die Anwendung im Fordergrund</summary>
        public bool   Foreground    { get { return _foreground; } }
    }
}