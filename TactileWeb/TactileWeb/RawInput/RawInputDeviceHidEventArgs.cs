using System;
using System.Text;



namespace TactileWeb.RawInput
{
    public delegate void RawDeviceHidEventHandler(object sender, RawInputDeviceHidEventArgs e);


    /// <summary>Daten eines HID Geräts</summary>
    public class RawInputDeviceHidEventArgs : EventArgs
    {
        protected byte[]    _data;
        protected IntPtr    _rawDevice;
        protected bool      _foreground;

        public RawInputDeviceHidEventArgs(byte[] data, IntPtr rawDevice, bool foreground)
        {
            _data       = data;
            _rawDevice  = rawDevice;
            _foreground = foreground;
        }


        /// <summary>Vom HID Gerät empfangene Daten</summary>
        public byte[] Data { get { return _data; } }

        /// <summary>Handle des Geräts</summary>
        public IntPtr RawDevice     { get { return _rawDevice; } }

        /// <summary>War die Anwendung im Fordergrund</summary>
        public bool   Foreground    { get { return _foreground; } }



        /// <summary>Gibt die Daten als String zurück</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < _data.Length; i++)
            {
                sb.AppendFormat("{0} ", _data[i]);
            }

            if (sb.Length != 0) sb.Length -= 1;


            return base.ToString();
        }



    }
}