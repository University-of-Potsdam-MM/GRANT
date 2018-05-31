using System;



namespace TactileWeb.RawInput
{


    public class RawInputDevice
    {
        protected IntPtr        _hDevice;
        protected RawInputType  _dwType;
        protected string        _name;
        protected string        _info;


        public RawInputDevice(IntPtr hDevice, RawInputType dwType, string name, string info)
        {
            _hDevice    = hDevice;
            _dwType     = dwType;
            _name       = name;
            _info       = info;
        }

        public int              Id        { get { return _hDevice.ToInt32(); } }
        public IntPtr           Handle    { get { return _hDevice; } }
        public RawInputType     Typ       { get { return _dwType;  } }
        public string           Name      { get { return _name;    } }
        public string           Info      { get { return _info;    } }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3}", _dwType, _hDevice, _name, _info);
        }

    }
}
