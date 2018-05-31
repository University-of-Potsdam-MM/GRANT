using System;
using System.Windows.Forms;


namespace TactileWeb.RawInput
{

    public delegate void RawDeviceKeyEventHandler(object sender, RawInputDeviceKeyEventArgs e);

    /// <summary>Erweiterte KeyEventArgs mit dem Handle des Geräts und ob die Anwendung im Foredergrund war</summary>
    public class RawInputDeviceKeyEventArgs : KeyEventArgs
    {
        protected IntPtr    _rawDevice;
        protected bool      _foreground;

        public RawInputDeviceKeyEventArgs(Keys keyData, IntPtr rawDevice, bool foreground) : base(keyData)
        {
            _rawDevice  = rawDevice;
            _foreground = foreground;
        }

        /// <summary>Handle des Geräts</summary>
        public IntPtr RawDevice     { get { return _rawDevice; } }

        /// <summary>War die Anwendung im Fordergrund</summary>
        public bool   Foreground    { get { return _foreground; } }

        public override string ToString()
        {
            return base.KeyCode.ToString();
        }

    }
}