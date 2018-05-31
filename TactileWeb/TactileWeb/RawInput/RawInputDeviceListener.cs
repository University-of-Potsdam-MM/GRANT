using System;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;



namespace TactileWeb.RawInput
{

    /// <summary>
    /// Globale Events für Maus, Tastatur und Joystick basierend auf RawInput.
    /// Die Events sollten sich wie in der Windows Forms klasse verhalten.
    /// Es kann zwischen mehreren Tastaturen unterschieden werden.
    /// </summary>
    public class RawInputDeviceListener
    {

        /// <summary>Soll beim gedrückt halten einer Taste der KeyDown event wiederholend geschickt werden. Bei false wird er nur einmal geschickt. Bei true werden die Wiederholungen geschickt. (Standard)</summary>
        public bool              SendKeyDownRepeats = true;

        protected Type              _tHeader;
        protected int               _szHeader;

        protected CallbackWindow    _wnd;
        protected bool[]            _isKeyDown;


        public RawInputDeviceListener()
        {
            _tHeader  = typeof(RAWINPUTHEADER);
            _szHeader = Marshal.SizeOf( _tHeader );

            _isKeyDown          = new bool[256];    // Key Down Wiederholungen sollen verhindert werden
            SendKeyDownRepeats  = true;

            _wnd = new CallbackWindow ( WndProc );

            RAWINPUTDEVICE[] devices = new RAWINPUTDEVICE[3];
            devices[0].usUsagePage  = HIDUsagePage.Generic;
            devices[0].usUsage      = HIDUsage.Mouse;
            devices[0].dwFlags      = RawInputDeviceFlags.InputSink;
            devices[0].hwndTarget   = _wnd.Handle;

            devices[1].usUsagePage  = HIDUsagePage.Generic;
            devices[1].usUsage      = HIDUsage.Keyboard;
            devices[1].dwFlags      = RawInputDeviceFlags.InputSink;
            devices[1].hwndTarget   = _wnd.Handle;

            devices[2].usUsagePage  = HIDUsagePage.Generic;
            devices[2].usUsage      = HIDUsage.Joystick;
            devices[2].dwFlags      = RawInputDeviceFlags.InputSink;
            devices[2].hwndTarget   = _wnd.Handle;

            int cbSize =  Marshal.SizeOf( typeof(RAWINPUTDEVICE) );

            if ( RegisterRawInputDevices( devices, devices.Length, cbSize ) == false )
            {
                throw new Exception("RegisterRawInputDevices");
            }

        }




        /// <summary>Hier erhalten wir die WM_INPUT Meldungen zurück</summary>
        protected void WndProc         (ref Message m)
        {
            bool foreground = (m.WParam.ToInt32() == 0);

            int    pcbSize      = 1024;
            IntPtr pData        = Marshal.AllocHGlobal(pcbSize);

            int outSize = GetRawInputData( m.LParam, RawInputCommand.RID_INPUT, pData, ref pcbSize , _szHeader );

            if (outSize < 1)
            {
                System.Diagnostics.Debug.Print ("GetRawInputData: Puffer zu klein!");
                return; // Fehler
            }

            RAWINPUTHEADER  header = (RAWINPUTHEADER) Marshal.PtrToStructure(pData, _tHeader );

            if      ( header.dwType == RawInputType.RIM_TYPEMOUSE     )    { WndProcMouse    ( foreground, header.hDevice, pData ); }
            else if ( header.dwType == RawInputType.RIM_TYPEKEYBOARD  )    { WndProcKeyboard ( foreground, header.hDevice, pData ); }
            else if ( header.dwType == RawInputType.RIM_TYPEHID       )    { WndProcHID      ( foreground, header.hDevice, pData ); }


            Marshal.FreeHGlobal(pData);
        }


        protected void WndProcMouse    (bool foreground, IntPtr hDevice, IntPtr pData )
        {
            RAWMOUSE m = (RAWMOUSE)Marshal.PtrToStructure( pData + _szHeader,  typeof(RAWMOUSE) );

            int x = 0;
            int y = 0;

            System.Drawing.Point ptMouse = Control.MousePosition;
            x = ptMouse.X;
            y = ptMouse.Y;

            //x = input.mouse.lLastX;
            //y = input.mouse.lLastY;

            //System.Diagnostics.Debug.Print ("Mouse Device:0x{0:X4,-5} FG:{1,-4}  X:{2,-3} Y:{3,-3}   Buttons:{4,-12} Wheel:{5}", hDevice, foreground,     x,y,  mouse.usButtonFlags, mouse.usButtonData );
            
            if      ( m.usButtonFlags == RawMouseButtonFlags.None        ) { OnMouseMove   ( new RawInputDeviceMouseEventArgs(MouseButtons.None,     0, x, y, 0, hDevice, foreground) ); }

            if      ( m.usButtonFlags == RawMouseButtonFlags.LeftDown    ) { OnMouseDown   ( new RawInputDeviceMouseEventArgs(MouseButtons.Left,     1, x, y, 0, hDevice, foreground) ); }
            else if ( m.usButtonFlags == RawMouseButtonFlags.LeftUp      ) { OnMouseUp     ( new RawInputDeviceMouseEventArgs(MouseButtons.Left,     1, x, y, 0, hDevice, foreground) ); }

            else if ( m.usButtonFlags == RawMouseButtonFlags.RightDown   ) { OnMouseDown   ( new RawInputDeviceMouseEventArgs(MouseButtons.Right,    1, x, y, 0, hDevice, foreground) ); }
            else if ( m.usButtonFlags == RawMouseButtonFlags.RightUp     ) { OnMouseUp     ( new RawInputDeviceMouseEventArgs(MouseButtons.Right,    1, x, y, 0, hDevice, foreground) ); }

            else if ( m.usButtonFlags == RawMouseButtonFlags.MiddleDown  ) { OnMouseDown   ( new RawInputDeviceMouseEventArgs(MouseButtons.Middle,   1, x, y, 0, hDevice, foreground) ); }
            else if ( m.usButtonFlags == RawMouseButtonFlags.MiddleUp    ) { OnMouseUp     ( new RawInputDeviceMouseEventArgs(MouseButtons.Middle,   1, x, y, 0, hDevice, foreground) ); }

            else if ( m.usButtonFlags == RawMouseButtonFlags.Button4Down ) { OnMouseDown  ( new RawInputDeviceMouseEventArgs(MouseButtons.XButton1, 1, x, y, 0, hDevice, foreground) ); }
            else if ( m.usButtonFlags == RawMouseButtonFlags.Button4Up   ) { OnMouseUp    ( new RawInputDeviceMouseEventArgs(MouseButtons.XButton1, 1, x, y, 0, hDevice, foreground) ); }

            else if ( m.usButtonFlags == RawMouseButtonFlags.Button5Down ) { OnMouseDown  ( new RawInputDeviceMouseEventArgs(MouseButtons.XButton2, 1, x, y, 0, hDevice, foreground) ); }
            else if ( m.usButtonFlags == RawMouseButtonFlags.Button5Up   ) { OnMouseUp    ( new RawInputDeviceMouseEventArgs(MouseButtons.XButton2, 1, x, y, 0, hDevice, foreground) ); }

            else if ( m.usButtonFlags == RawMouseButtonFlags.MouseWheel  ) 
            { 
                short delta = (short)m.usButtonData;

                OnMouseWheel  ( new RawInputDeviceMouseEventArgs(MouseButtons.None, 0, x, y, delta, hDevice, foreground) );
            }


        }


        protected void WndProcKeyboard (bool foreground, IntPtr hDevice, IntPtr pData)
        {
            RAWKEYBOARD keyboard = (RAWKEYBOARD)Marshal.PtrToStructure( pData + _szHeader,  typeof(RAWKEYBOARD) );

            int  k   = keyboard.VKey;
            Keys key = (Keys)keyboard.VKey;

            if      ( ( keyboard.Message == WM_KEYDOWN ) || ( keyboard.Message == WM_SYSKEYDOWN ) )   
            { 
                if (  ( SendKeyDownRepeats == true ) || ( _isKeyDown[k] == false )  )
                {
                    _isKeyDown[k] = true;

                    if ( ( _isKeyDown[ (int)Keys.LShiftKey   ] == true ) || ( _isKeyDown[ (int)Keys.RShiftKey   ] == true ) )   { key |= Keys.Shift;   }
                    if ( ( _isKeyDown[ (int)Keys.LControlKey ] == true ) || ( _isKeyDown[ (int)Keys.RControlKey ] == true ) )   { key |= Keys.Control; }
                    if ( ( _isKeyDown[ (int)Keys.LMenu       ] == true ) || ( _isKeyDown[ (int)Keys.LMenu       ] == true ) )   { key |= Keys.Alt;     }


                    OnKeyDown( new RawInputDeviceKeyEventArgs(key, hDevice, foreground) );
                }
            }
            
            else if ( ( keyboard.Message == WM_KEYUP   ) || ( keyboard.Message == WM_SYSKEYUP   ) )
            {
                _isKeyDown[k] = false;
                OnKeyUp  ( new RawInputDeviceKeyEventArgs(key, hDevice, foreground) );
            }
            
            else 
            {
                System.Diagnostics.Debug.Print("TODO WndProcKeyboard 0x{0:X8}", keyboard.Message);
            }


           // System.Diagnostics.Debug.Print ("Key   Device:0x{0:X4} FG:{1,-4}  VKey:{2} Flags:{3} Message:0x{4:X4} ScanCode:{5}",         hDevice.ToInt32(), foreground,     keyData, keyboard.Flags, keyboard.Message, keyboard.MakeCode + 1 );
        }


        protected void WndProcHID      (bool foreground, IntPtr hDevice, IntPtr pData)
        {
            RAWHID h = (RAWHID)Marshal.PtrToStructure( pData + _szHeader, typeof(RAWHID) );


            int count  = h.dwCount;     // 1
            int length = h.dwSizeHid;   // 7

            byte[] ba = new byte[length];

            for (int i = 0; i < count; i++)
            {
                Marshal.Copy(pData+16+8+ i*length, ba, 0, ba.Length);

                OnHid( new RawInputDeviceHidEventArgs(ba, hDevice, foreground) );

                //System.Diagnostics.Debug.Print ("H {0} VID:{1:X4} PID:{2:X4} Data:{3}", deviceName, info.hid.dwVendorId, info.hid.dwProductId, sb.ToString() );
            }

        }





        /// <summary>Klasse um Windows Messages auch ohne eine Form zu erhalten</summary>
        protected class CallbackWindow : NativeWindow
        {
            public delegate void Callback(ref Message m);


            protected Callback              _callback; 

            /// <summary>Erstellt ein neues Benachrichtigungsfenster wür WM_INPUT Messages</summary>
            public CallbackWindow(Callback callback)
            {
                _callback = callback;
                CreateHandle(new CreateParams() );
            }

            protected override void WndProc(ref Message m)
            {
                if ( m.Msg == WM_INPUT )
                {
                    _callback.Invoke(ref m);
                }

                base.WndProc(ref m);
            }
        }




        public event RawDeviceMouseEventHandler     MouseDown;
        public event RawDeviceMouseEventHandler     MouseUp;
        public event RawDeviceMouseEventHandler     MouseMove;
        public event RawDeviceMouseEventHandler     MouseWheel;
        public event RawDeviceKeyEventHandler       KeyDown;
        public event RawDeviceKeyEventHandler       KeyUp;
        public event RawDeviceHidEventHandler       Hid;


        protected virtual void OnMouseMove  (RawInputDeviceMouseEventArgs e)  { if (MouseMove  != null) { MouseMove (this, e); } }
        protected virtual void OnMouseWheel (RawInputDeviceMouseEventArgs e)  { if (MouseWheel != null) { MouseWheel(this, e); } }
        protected virtual void OnMouseDown  (RawInputDeviceMouseEventArgs e)  { if (MouseDown  != null) { MouseDown (this, e); } }
        protected virtual void OnMouseUp    (RawInputDeviceMouseEventArgs e)  { if (MouseUp    != null) { MouseUp   (this, e); } }

        protected virtual void OnKeyDown    (RawInputDeviceKeyEventArgs   e)  { if (KeyDown    != null) { KeyDown   (this, e); } }
        protected virtual void OnKeyUp      (RawInputDeviceKeyEventArgs   e)  { if (KeyUp      != null) { KeyUp     (this, e); } }

        protected virtual void OnHid        (RawInputDeviceHidEventArgs   e)  { if (Hid        != null) { Hid       (this, e); } }














        private const int WM_INPUT      = 0x00FF;

        private const int WM_KEYDOWN    = 0x0100;
        private const int WM_KEYUP      = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP   = 0x0105;

        private enum HIDUsagePage : ushort
        {
            Undefined       = 0x00,
            Generic         = 0x01,
            Simulation      = 0x02,
            VR              = 0x03,
            Sport           = 0x04,
            Game            = 0x05,
            Keyboard        = 0x07,
            LED             = 0x08,
            Button          = 0x09,
            Ordinal         = 0x0A,
            Telephony       = 0x0B,
            Consumer        = 0x0C,
            Digitizer       = 0x0D,
            PID             = 0x0F,
            Unicode         = 0x10,
            AlphaNumeric    = 0x14,
            Medical         = 0x40,
            MonitorPage0    = 0x80,
            MonitorPage1    = 0x81,
            MonitorPage2    = 0x82,
            MonitorPage3    = 0x83,
            PowerPage0      = 0x84,
            PowerPage1      = 0x85,
            PowerPage2      = 0x86,
            PowerPage3      = 0x87,
            BarCode         = 0x8C,
            Scale           = 0x8D,
            MSR             = 0x8E,
            Touch           = 0xFF00
        }


        private enum HIDUsage : ushort
        {
            Pointer                         = 0x01,
            Mouse                           = 0x02,
            Joystick                        = 0x04,
            Gamepad                         = 0x05,
            Keyboard                        = 0x06,
            Keypad                          = 0x07,
            SystemControl                   = 0x80,
            X                               = 0x30,
            Y                               = 0x31,
            Z                               = 0x32,
            RelativeX                       = 0x33,
            RelativeY                       = 0x34,
            RelativeZ                       = 0x35,
            Slider                          = 0x36,
            Dial                            = 0x37,
            Wheel                           = 0x38,
            HatSwitch                       = 0x39,
            CountedBuffer                   = 0x3A,
            ByteCount                       = 0x3B,
            MotionWakeup                    = 0x3C,
            VX                              = 0x40,
            VY                              = 0x41,
            VZ                              = 0x42,
            VBRX                            = 0x43,
            VBRY                            = 0x44,
            VBRZ                            = 0x45,
            VNO                             = 0x46,
            SystemControlPower              = 0x81,
            SystemControlSleep              = 0x82,
            SystemControlWake               = 0x83,
            SystemControlContextMenu        = 0x84,
            SystemControlMainMenu           = 0x85,
            SystemControlApplicationMenu    = 0x86,
            SystemControlHelpMenu           = 0x87,
            SystemControlMenuExit           = 0x88,
            SystemControlMenuSelect         = 0x89,
            SystemControlMenuRight          = 0x8A,
            SystemControlMenuLeft           = 0x8B,
            SystemControlMenuUp             = 0x8C,
            SystemControlMenuDown           = 0x8D,
            KeyboardNoEvent                 = 0x00,
            KeyboardRollover                = 0x01,
            KeyboardPostFail                = 0x02,
            KeyboardUndefined               = 0x03,
            KeyboardaA                      = 0x04,
            KeyboardzZ                      = 0x1D,
            Keyboard1                       = 0x1E,
            Keyboard0                       = 0x27,
            KeyboardLeftControl             = 0xE0,
            KeyboardLeftShift               = 0xE1,
            KeyboardLeftALT                 = 0xE2,
            KeyboardLeftGUI                 = 0xE3,
            KeyboardRightControl            = 0xE4,
            KeyboardRightShift              = 0xE5,
            KeyboardRightALT                = 0xE6,
            KeyboardRightGUI                = 0xE7,
            KeyboardScrollLock              = 0x47,
            KeyboardNumLock                 = 0x53,
            KeyboardCapsLock                = 0x39,
            KeyboardF1                      = 0x3A,
            KeyboardF12                     = 0x45,
            KeyboardReturn                  = 0x28,
            KeyboardEscape                  = 0x29,
            KeyboardDelete                  = 0x2A,
            KeyboardPrintScreen             = 0x46,
            LEDNumLock                      = 0x01,
            LEDCapsLock                     = 0x02,
            LEDScrollLock                   = 0x03,
            LEDCompose                      = 0x04,
            LEDKana                         = 0x05,
            LEDPower                        = 0x06,
            LEDShift                        = 0x07,
            LEDDoNotDisturb                 = 0x08,
            LEDMute                         = 0x09,
            LEDToneEnable                   = 0x0A,
            LEDHighCutFilter                = 0x0B,
            LEDLowCutFilter                 = 0x0C,
            LEDEqualizerEnable              = 0x0D,
            LEDSoundFieldOn                 = 0x0E,
            LEDSurroundFieldOn              = 0x0F,
            LEDRepeat                       = 0x10,
            LEDStereo                       = 0x11,
            LEDSamplingRateDirect           = 0x12,
            LEDSpinning                     = 0x13,
            LEDCAV                          = 0x14,
            LEDCLV                          = 0x15,
            LEDRecordingFormatDet           = 0x16,
            LEDOffHook                      = 0x17,
            LEDRing                         = 0x18,
            LEDMessageWaiting               = 0x19,
            LEDDataMode                     = 0x1A,
            LEDBatteryOperation             = 0x1B,
            LEDBatteryOK                    = 0x1C,
            LEDBatteryLow                   = 0x1D,
            LEDSpeaker                      = 0x1E,
            LEDHeadset                      = 0x1F,
            LEDHold                         = 0x20,
            LEDMicrophone                   = 0x21,
            LEDCoverage                     = 0x22,
            LEDNightMode                    = 0x23,
            LEDSendCalls                    = 0x24,
            LEDCallPickup                   = 0x25,
            LEDConference                   = 0x26,
            LEDStandBy                      = 0x27,
            LEDCameraOn                     = 0x28,
            LEDCameraOff                    = 0x29,
            LEDOnLine                       = 0x2A,
            LEDOffLine                      = 0x2B,
            LEDBusy                         = 0x2C,
            LEDReady                        = 0x2D,
            LEDPaperOut                     = 0x2E,
            LEDPaperJam                     = 0x2F,
            LEDRemote                       = 0x30,
            LEDForward                      = 0x31,
            LEDReverse                      = 0x32,
            LEDStop                         = 0x33,
            LEDRewind                       = 0x34,
            LEDFastForward                  = 0x35,
            LEDPlay                         = 0x36,
            LEDPause                        = 0x37,
            LEDRecord                       = 0x38,
            LEDError                        = 0x39,
            LEDSelectedIndicator            = 0x3A,
            LEDInUseIndicator               = 0x3B,
            LEDMultiModeIndicator           = 0x3C,
            LEDIndicatorOn                  = 0x3D,
            LEDIndicatorFlash               = 0x3E,
            LEDIndicatorSlowBlink           = 0x3F,
            LEDIndicatorFastBlink           = 0x40,
            LEDIndicatorOff                 = 0x41,
            LEDFlashOnTime                  = 0x42,
            LEDSlowBlinkOnTime              = 0x43,
            LEDSlowBlinkOffTime             = 0x44,
            LEDFastBlinkOnTime              = 0x45,
            LEDFastBlinkOffTime             = 0x46,
            LEDIndicatorColor               = 0x47,
            LEDRed                          = 0x48,
            LEDGreen                        = 0x49,
            LEDAmber                        = 0x4A,
            LEDGenericIndicator             = 0x3B,
            TelephonyPhone                  = 0x01,
            TelephonyAnsweringMachine       = 0x02,
            TelephonyMessageControls        = 0x03,
            TelephonyHandset                = 0x04,
            TelephonyHeadset                = 0x05,
            TelephonyKeypad                 = 0x06,
            TelephonyProgrammableButton     = 0x07,
            SimulationRudder                = 0xBA,
            SimulationThrottle              = 0xBB
        }

        [Flags()]
        private enum RawInputDeviceFlags
        {
            None            = 0,
            Remove          = 0x00000001,
            Exclude         = 0x00000010,
            PageOnly        = 0x00000020,
            NoLegacy        = 0x00000030,
            InputSink       = 0x00000100,
            CaptureMouse    = 0x00000200,
            NoHotKeys       = 0x00000200,
            AppKeys         = 0x00000400
        }





        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTDEVICE
        {
            public HIDUsagePage             usUsagePage;
            public HIDUsage                 usUsage;
            public RawInputDeviceFlags      dwFlags;
            public IntPtr                   hwndTarget;
        }


        private enum RawInputCommand
        {
            RID_HEADER  = 0x10000005,
            RID_INPUT   = 0x10000003,
        }


        //private enum RawInputType
        //{
        //    RIM_TYPEMOUSE       = 0,
        //    RIM_TYPEKEYBOARD    = 1,
        //    RIM_TYPEHID         = 2
        //}


        //[StructLayout(LayoutKind.Sequential)]
        //private struct RAWINPUTDEVICELIST
        //{
        //    public IntPtr           hDevice;
        //    public RawInputType     dwType;

        //    public override string ToString()
        //    {
        //        return String.Format("{0} {1}", hDevice, dwType);
        //    }
        //}




        private enum RawMousFlags : ushort
        {
            MOUSE_MOVE_RELATIVE         = 0,
            MOUSE_MOVE_ABSOLUTE         = 1,
            MOUSE_VIRTUAL_DESKTOP       = 2,
            MOUSE_ATTRIBUTES_CHANGED    = 4
        }

        [Flags()]
        private enum RawMouseButtonFlags : ushort
        {
            None           = 0,
            LeftDown       = 0x0001,
            LeftUp         = 0x0002,
            RightDown      = 0x0004,
            RightUp        = 0x0008,
            MiddleDown     = 0x0010,
            MiddleUp       = 0x0020,
            Button4Down    = 0x0040,
            Button4Up      = 0x0080,
            Button5Down    = 0x0100,
            Button5Up      = 0x0200,
            MouseWheel     = 0x0400
        }



        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTHEADER       // 16 od 24 (32 od 64Bit)
        {
            public RawInputType dwType;     // 4      4
            public int          dwSize;     // 4      4
            public IntPtr       hDevice;    // 4      8
            public IntPtr       wParam;     // 4      8
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct RAWMOUSE
        {
            public RawMousFlags          usFlags;               // 2
            public ushort                ulButtons;             // 2
            public RawMouseButtonFlags   usButtonFlags;         // 2
            public ushort                usButtonData;          // 2
            public uint                  ulRawButtons;          // 4
            public int                   lLastX;                // 4
            public int                   lLastY;                // 4
            public uint                  ulExtraInformation;    // 4
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWKEYBOARD              // 16 Bytes 
        {
            public short    MakeCode;           // 2
            public ushort   Flags;              // 2
            public short    Reserved;           // 2
            public ushort   VKey;               // 2
            public uint     Message;            // 4
            public int      ExtraInformation;   // 4
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWHID
        {
            public int      dwSizeHid;  // 4 Bytes
            public int      dwCount;    // 4 Bytes
        }



        [DllImport("user32.dll")]   private static extern bool RegisterRawInputDevices ([MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] RAWINPUTDEVICE[] pRawInputDevices, int uiNumDevices, int cbSize);
        [DllImport("user32.dll")]   private static extern int  GetRawInputData         (IntPtr hRawInput, RawInputCommand uiCommand, IntPtr pData, ref int pcbSize, int cbSizeHeader);


    }
}