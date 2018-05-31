using System;
using System.Runtime.InteropServices;
using System.Collections;


namespace TactileWeb.RawInput
{
    public class RawInputDeviceCollection : IEnumerable
    {
        protected ArrayList _devices;


        public RawInputDeviceCollection()
        {
            _devices = new ArrayList();

            Refresh();
        }


        /// <summary>Füllt die Liste</summary>
        public void Refresh()
        {
            _devices.Clear();


            IntPtr pList = IntPtr.Zero;

            int count = 0;
            int size = Marshal.SizeOf( typeof(RAWINPUTDEVICELIST) );

            int stored = GetRawInputDeviceList(pList, ref count, size );
        
            pList = Marshal.AllocHGlobal(count * size);
            stored = GetRawInputDeviceList(pList, ref count, size );

            for (int i = 0; i < stored; i++)
            {
                RAWINPUTDEVICELIST itm = (RAWINPUTDEVICELIST) Marshal.PtrToStructure(pList + (i*size), typeof(RAWINPUTDEVICELIST) );

                IntPtr          hDevice = itm.hDevice;
                RawInputType    typ     = itm.dwType;
                string          name    = GetDeviceName(hDevice);
                string          info    = GetDeviceInfo(hDevice);

                RawInputDevice device = new RawInputDevice(hDevice, typ, name, info);
                _devices.Add(device);
            }

            Marshal.FreeHGlobal(pList);
        }



        private string GetDeviceName(IntPtr hDevice)
        {
            string name = null;

            IntPtr pInfo = Marshal.AllocHGlobal(1024);

            for (int i = 0; i < 5; i++)
            {
                int len = 1024;
            
                int ret = GetRawInputDeviceInfo(hDevice, RawInputDeviceInfoCommand.RIDI_DEVICENAME, pInfo, ref len );
            
                if (ret > 0 )
                {
                    name = Marshal.PtrToStringAnsi(pInfo);
                    break;
                }
            }

            Marshal.FreeHGlobal(pInfo);

            if (name == null)
            {
                name = "unbekannt";
            }

            return name;
        }


        private string GetDeviceInfo(IntPtr hDevice)
        {
            string info = null;

            int size = Marshal.SizeOf( typeof( RID_DEVICE_INFO ) );
            IntPtr pInfo = Marshal.AllocHGlobal(size);

            int read = GetRawInputDeviceInfo(hDevice, RawInputDeviceInfoCommand.RIDI_DEVICEINFO, pInfo, ref size );

            if ( read > 0 )
            {
                int             cbSize = Marshal.ReadInt32(pInfo, 0);
                RawInputType    dwType = (RawInputType)Marshal.ReadInt32(pInfo, 4);

                if (dwType == RawInputType.RIM_TYPEMOUSE)
                {
                    RID_DEVICE_INFO_MOUSE m = (RID_DEVICE_INFO_MOUSE)Marshal.PtrToStructure(pInfo+8, typeof(RID_DEVICE_INFO_MOUSE));
                    info = String.Format("Id: {0} Buttons:{1} SampleRate:{2}, HorizontalWheel:{3}", m.dwId, m.dwNumberOfButtons, m.dwSampleRate, m.fHasHorizontalWheel );
                }
                
                else if (dwType == RawInputType.RIM_TYPEKEYBOARD)
                {
                    RID_DEVICE_INFO_KEYBOARD k = (RID_DEVICE_INFO_KEYBOARD)Marshal.PtrToStructure(pInfo+8, typeof(RID_DEVICE_INFO_KEYBOARD));
                    info = String.Format("Type: {0} SubType:{1} Mode:{2}, Keys:{3}", k.dwType, k.dwSubType, k.dwKeyboardMode, k.dwNumberOfKeysTotal );
                }

                else if (dwType == RawInputType.RIM_TYPEHID)
                {
                    RID_DEVICE_INFO_HID h = (RID_DEVICE_INFO_HID)Marshal.PtrToStructure(pInfo+8, typeof(RID_DEVICE_INFO_HID));
                    info = String.Format("VID:0x{0:X4}, PID:0x{1:X4}, UsagePage:0x{2:X4} Usage:0x{3:X4} ", h.dwVendorId, h.dwProductId, h.usUsagePage, h.usUsage);
                }
            }
            
            
            Marshal.FreeHGlobal(pInfo);

            return info;
        }






        public int Count
        { 
            get 
            { 
                return _devices.Count;
            }
        }

        public RawInputDevice this[int index]
        { 
            get 
            { 
                return(RawInputDevice)_devices[index];
            }
        }


        public IEnumerator GetEnumerator()
        {
            return _devices.GetEnumerator();
        }











        private enum RawInputDeviceInfoCommand
        {
            RIDI_DEVICENAME  = 0x20000007,
            RIDI_DEVICEINFO  = 0x2000000b,
        }



        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTDEVICELIST
        {
            public IntPtr           hDevice;
            public RawInputType     dwType;
        }


        [StructLayout(LayoutKind.Explicit)]
        private struct RID_DEVICE_INFO
        {
            [FieldOffset(0)]    public int                          cbSize;
            [FieldOffset(4)]    public RawInputType                 dwType;
            [FieldOffset(8)]    public RID_DEVICE_INFO_MOUSE        mouse;
            [FieldOffset(8)]    public RID_DEVICE_INFO_KEYBOARD     keyboard;
            [FieldOffset(8)]    public RID_DEVICE_INFO_HID          hid;
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct RID_DEVICE_INFO_MOUSE
        {
            public int      dwId;
            public int      dwNumberOfButtons;
            public int      dwSampleRate;
            public bool     fHasHorizontalWheel;
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct RID_DEVICE_INFO_KEYBOARD
        {
            public int      dwType;
            public int      dwSubType;
            public int      dwKeyboardMode;
            public int      dwNumberOfFunctionKeys;
            public int      dwNumberOfIndicators;
            public int      dwNumberOfKeysTotal;
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct RID_DEVICE_INFO_HID
        {
            public int      dwVendorId;
            public int      dwProductId;
            public int      dwVersionNumber;
            public ushort   usUsagePage;
            public ushort   usUsage;
        }







        [DllImport("user32.dll")]   private static extern int  GetRawInputDeviceList (IntPtr                          pRawInputDeviceList, ref int puiNumDevices, int cbSize);
        [DllImport("user32.dll")]   private static extern int  GetRawInputDeviceList ([In, Out] RAWINPUTDEVICELIST[]  pRawInputDeviceList, ref int puiNumDevices, int cbSize);
        [DllImport("user32.dll")]   private static extern int  GetRawInputDeviceInfo (IntPtr hDevice, RawInputDeviceInfoCommand uiCommand, IntPtr              pData, ref int pcbSize);





    }
}
