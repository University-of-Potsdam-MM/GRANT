using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;

using TactileWeb.UIA;
using TactileWeb.RawInput;


namespace TactileWeb
{


    /// <summary>Copys the screen and shows the pins in a virtual device</summary>
    public class ScreenCapture : IUIAutomationFocusChangedEventHandler
    {

        /// <summary>Activates the screen copy</summary>
        public bool                     Active;

        /// <summary>This value is obsolete and has no effect since version 119. Use Mode instead</summary>
        private bool                    ObsoleteEdgeDetection;

        /// <summary>What to show. 0=Thereshold, 1=EdgeDetection, 2=UIAElements</summary>
        public ScreenCaptureMode        Mode;



        ///// <summary>Filter gray colors</summary>
        public Filter                   FilterGray;

        ///// <summary>Filter red colors</summary>
        public Filter                   FilterRed;

        ///// <summary>Filter green colors</summary>
        public Filter                   FilterGreen;

        ///// <summary>Filter blue colors</summary>
        public Filter                   FilterBlue;



        /// <summary>X-Translate of the source</summary>
        public int                      DX;

        /// <summary>Y-Translate of the source</summary>
        public int                      DY;


        /// <summary>Zoomfactor of the source</summary>
        public float                    SX;

        /// <summary>The minimum value for zooming</summary>
        public float                    SXMinimum;

        /// <summary>The maximum value for zooming</summary>
        public float                    SXMaximum;



        /// <summary>The view follows the mouse pointer</summary>
        public bool                     FollowMousePointer;

        /// <summary>The view follows the focus element</summary>
        public bool                     FollowFocus;


        /// <summary>Shows the pins inverted</summary>
        public bool                     Invert;

        /// <summary>Is the ConfigMenu mode active</summary>
        public bool                     ConfigMenuMode;

        /// <summary>Speak the pressed Keys</summary>
        public bool                     SpeakKeys;

        /// <summary>Mouse Step when moving the mouse with cursor keys</summary>
        public int                      MouseStep;


        /// <summary>Counter for every scan</summary>
        public int                      Counter;



        /// <summary>Show the mousepointer with blinking dots</summary>
        public bool                     ShowMousepointer;

        /// <summary>The mousepointer is visible</summary>
        public bool                     ShowMousepointerVisible;


        /// <summary>Blink speed of the mousepointer (1-30). Saved a byte(0-255)</summary>
        public int                      ShowMousepointerVisibleTogglePrescaler;




        public IUIAutomationElement     FocusElement;
        public AutomationElement        UIAInfo;

        public Point                    MousePosition;
        public Point                    CaretPosition;
        public string                   CaretInfo;

        public RECT                     FocusRect;
        public static IUIAutomation     UIA;

        //todo warum ein zweites uia ???
        public static IUIAutomation2    UIA2;

        //todo - einbindung der virtuelle stiftplatte, bzw braillezeile mit anbindung an reale zeile platte
        /// <summary>Null, or the virtual device that shows the pins</summary>
        //public VirtualDeviceControl     VirtualDevice;


        protected int                       _pinCountX;
        protected int                       _pinCountY;

        protected byte[,]                   _red;
        protected byte[,]                   _green;
        protected byte[,]                   _blue;
        protected byte[,]                   _gray;

        protected IntPtr                    _pData;
        protected bool [,]                  _pins;

        //todo: threadverwaltung
        protected ActionThread              _thread;

        protected ScreenCaptureMode         _lastMode;
        protected bool                      _lastActive;


        /// <summary>Creates a new screencapture. Don't forget to set the VirtualDevice later.</summary>
        public ScreenCapture()
        {

            Active              = false;
            FollowMousePointer  = true;
            FollowFocus         = false;
            Mode                = ScreenCaptureMode.Threshold;
            Invert              = false;
            SpeakKeys           = false;
            ConfigMenuMode      = false;
            MouseStep           = 5;

            ShowMousepointer                        = true;
            ShowMousepointerVisible                 = false;
            ShowMousepointerVisibleTogglePrescaler  = 4;


            _pData              = Marshal.AllocHGlobal(1920*1080*4);


            _pinCountX          = 76;
            _pinCountY          = 48;


            _red                = new byte [258, 258];    // 256 + 2
            _green              = new byte [258, 258];
            _blue               = new byte [258, 258];
            
            _gray               = new byte [258, 258];
            _pins               = new bool [258, 258];

            DX                  = 0;
            DY                  = 0;

            SX                  = 1.0F;
            SXMinimum           = 0.01F;    // 1%
            SXMaximum           = 5.0F;     // 500%

            FilterGray          = new Filter( true  , true ,     80 , 120 );    // active, anchor, v1, v2
            FilterRed           = new Filter( false , true ,     80 , 120 );
            FilterGreen         = new Filter( false , true ,     80 , 120 );
            FilterBlue          = new Filter( false , true ,     80 , 120 );

            int version = 0;

            if ( version == 0 )
            {
                Guid CLSID_CUIAutomation  = new Guid("{ff48dba4-60ef-4201-aa87-54103eef594e}"); // XP
                Type type                 = Type.GetTypeFromCLSID(CLSID_CUIAutomation, true);
                UIA  = (IUIAutomation)Activator.CreateInstance(type);
                UIA2 = null;
            }
            else
            {
                Guid CLSID_CUIAutomation8 = new Guid("{e22ad333-b25f-460c-83d0-0581107395c9}"); // Windows 8
                Type type                = Type.GetTypeFromCLSID(CLSID_CUIAutomation8, true);

                UIA  = (IUIAutomation) Activator.CreateInstance(type);
                UIA2 = (IUIAutomation2)Activator.CreateInstance(type);

                // Test
                bool autoSetFocus;

                if ( UIA2.get_AutoSetFocus(out autoSetFocus) == 0 )
                {
                    System.Diagnostics.Debug.Print("UIA2 get_AutoSetFocus OK");

                    autoSetFocus = true;

                    if ( UIA2.put_AutoSetFocus(autoSetFocus) == 0 )
                    {
                        System.Diagnostics.Debug.Print("UIA2 put_AutoSetFocus OK");
                    }

                }
            }

            _thread         = new ActionThread( Thread_Run, "ScreenCapture" );
            _thread.Start();



            IUIAutomationCacheRequest cacheRequest = null;
            int ret = UIA.AddFocusChangedEventHandler(cacheRequest, this);

            UIAInfo = new AutomationElement( this );


            LoadSettings();


        }



        //todo: tactweb loadSettings

        /// <summary>Load the values from the file. Returns true if it was loaded successfully</summary>
        protected bool  LoadSettings()
        {
            bool success = false;

            //todo tactweb 
            //string       fileName = String.Format   (@"{0}ScreenCapture.dat",  ConfigOLD.Directory  );  // "C:\Users\baumueller\AppData\Roaming\MetecAG\MVBD\ScreenCapture.dat"

            FileStream fs = null;

            try
            {
                //fs = new FileStream(fileName,  FileMode.Open);
                //BinaryReader r = new BinaryReader(fs, Encoding.Unicode);

                //Active                  = r.ReadBoolean();      // 00
                //ObsoleteEdgeDetection   = r.ReadBoolean();      // 01 (old)
                //FollowFocus             = r.ReadBoolean();      // 02
                //FollowMousePointer      = r.ReadBoolean();      // 03
                //DX                      = r.ReadInt32();        // 04
                //DY                      = r.ReadInt32();        // 05
                //SX                      = r.ReadSingle();       // 06
                //int threshold           = r.ReadInt32();        // 07 (Unused Version 111)
                //Invert                  = r.ReadBoolean();      // 08
                //SpeakKeys               = r.ReadBoolean();      // 09

                //if ( fs.Position < fs.Length )  UIAInfo.SpeakElement                    = r.ReadBoolean();    // 10 (Version 109)
                //if ( fs.Position < fs.Length )  UIAInfo.ShowElementInBrailleline        = r.ReadBoolean();    // 11 (Version 110)
                //if ( fs.Position < fs.Length )  MouseStep                               = r.ReadInt32();      // 12 (Version 113)
                //if ( fs.Position < fs.Length )  ShowMousepointer                        = r.ReadBoolean();    // 13 (Version 113)
                //if ( fs.Position < fs.Length )  ShowMousepointerVisibleTogglePrescaler  = r.ReadByte();       // 14 (Version 113)

                //if ( fs.Position < fs.Length )  LoadSettingsFilter(r, FilterGray );                           // 15 (Version 113)
                //if ( fs.Position < fs.Length )  LoadSettingsFilter(r, FilterRed  );                           // 16 (Version 113)
                //if ( fs.Position < fs.Length )  LoadSettingsFilter(r, FilterGreen);                           // 17 (Version 113)
                //if ( fs.Position < fs.Length )  LoadSettingsFilter(r, FilterBlue );                           // 18 (Version 113)

                //if ( fs.Position < fs.Length )  Mode  = (ScreenCaptureMode)r.ReadByte();                      // 19 (Version 119)


                success = true; // Read was successful
            }

            catch(Exception ex)
            {
                //todo tactweb 
                //Debug.Print (ex.Message);
            }

            finally
            {
                if ( fs != null )
                {
                    fs.Close();
                }
            }


            return success;
        }


        /// <summary>Load data from file and set it in the filter</summary>
        protected void  LoadSettingsFilter(BinaryReader r, Filter f)
        {
            byte version = r.ReadByte();        // 00 Version for later changes
            f.Active     = r.ReadBoolean();     // 01
            f.Anchor     = r.ReadBoolean();     // 02
            f.V1         = r.ReadByte();        // 03
            f.V2         = r.ReadByte();        // 04
        }




        //todo: tactweb savesettings

        /// <summary>Saves the data in a file.</summary>
        public void     SaveSettings()
        {
            //string       fileName = String.Format   (@"{0}ScreenCapture.dat",  ConfigOLD.Directory  );  // "C:\Users\baumueller\AppData\Roaming\MetecAG\MVBD\ScreenCapture.dat"
            //FileStream   fs       = new FileStream  (fileName,  FileMode.Create);
            //BinaryWriter w        = new BinaryWriter(fs, Encoding.Unicode);

            //w.Write ( (bool) Active                                 );  // 00
            //w.Write ( (bool) ObsoleteEdgeDetection                  );  // 01 (old)
            //w.Write ( (bool) FollowFocus                            );  // 02
            //w.Write ( (bool) FollowMousePointer                     );  // 03
            //w.Write ( (int)  DX                                     );  // 04
            //w.Write ( (int)  DY                                     );  // 05
            //w.Write ( (float)SX                                     );  // 06
            //w.Write ( (int)  0                                      );  // 07 (Unused Version 111)
            //w.Write ( (bool) Invert                                 );  // 08
            //w.Write ( (bool) SpeakKeys                              );  // 09
            //w.Write ( (bool) UIAInfo.SpeakElement                   );  // 10 (Version 109)
            //w.Write ( (bool) UIAInfo.ShowElementInBrailleline       );  // 11 (Version 110)
            //w.Write ( (int)  MouseStep                              );  // 12 (Version 113)
            //w.Write ( (bool) ShowMousepointer                       );  // 13 (Version 113)
            //w.Write ( (byte) ShowMousepointerVisibleTogglePrescaler );  // 14 (Version 113)

            //SaveSettingsFilter (w, FilterGray                       );  // 15 (Version 113)
            //SaveSettingsFilter (w, FilterRed                        );  // 16 (Version 113)
            //SaveSettingsFilter (w, FilterGreen                      );  // 17 (Version 113)
            //SaveSettingsFilter (w, FilterBlue                       );  // 18 (Version 113)

            //w.Write ( (byte) Mode                                   );  // 19 (Version 113)


            //fs.Close();
        }

        /// <summary>Saves a filter on the filestream</summary>
        protected void  SaveSettingsFilter(BinaryWriter w, Filter f)
        {
            w.Write ( (byte) 0          );  // 00 Version for later changes
            w.Write ( (bool) f.Active   );  // 01
            w.Write ( (bool) f.Anchor   );  // 02
            w.Write ( (byte) f.V1       );  // 03
            w.Write ( (byte) f.V2       );  // 04
        }


        /// <summary>Event, when the focus was changed</summary>
        public int      HandleFocusChangedEvent(IUIAutomationElement sender)
        {
            FocusElement = sender;

            if (sender != null)
            {
                int ret = sender.get_CurrentBoundingRectangle (out FocusRect);
            }

            return 0;
        }


        /// <summary>Our running thread to do the work</summary>
        protected void  Thread_Run()
        {
            while (true)
            {
                Counter++;

                //todo: tactweb VirtualDevice
                //if (VirtualDevice == null)
                //{
                //    _pinCountX = 76;
                //    _pinCountY = 48;
                //}
                //else
                //{
                //    _pinCountX = VirtualDevice.PinAreaMain.Width;
                //    _pinCountY = VirtualDevice.PinAreaMain.Height;
                //}



                POINT lpPoint;
                if ( GetCursorPos(out lpPoint) == true )    { MousePosition = lpPoint; }

                GUITHREADINFO gui = new GUITHREADINFO();
                gui.cbSize = Marshal.SizeOf(gui);

                if ( GetGUIThreadInfo(0, ref gui) == true )
                {
                    CaretPosition = new Point( gui.rcCaret.left, gui.rcCaret.top );
                    
                    POINT ptScreen = new POINT();
                    ptScreen.x = gui.rcCaret.left;
                    ptScreen.y = gui.rcCaret.top;

                    ClientToScreen(gui.hwndCaret, ref ptScreen);

                    CaretInfo     = String.Format("{0}\r\n{1}\r\n{2}", gui.rcCaret, ptScreen, gui.flags);
                }

                //todo: tactweb VirtualDevice
                //// Clear screen when Mode changes
                //if ( Program.VirtualDevice != null )
                //{
                //    if ( ( Mode != _lastMode ) || ( Active != _lastActive ) )
                //    {
                //        Program.VirtualDevice.PinArray.SetAllPins(false);
                //        Program.VirtualDevice.PinControls.Clear();

                //        _lastMode   = Mode;
                //        _lastActive = Active;
                //    }
                //}



                if ( FollowMousePointer == true )
                {
                    DX = MousePosition.X;
                    DY = MousePosition.Y;
                }

                if ( FollowFocus == true )
                {
                    int width  = FocusRect.right  - FocusRect.left;
                    int height = FocusRect.bottom - FocusRect.top;

                    //_dx = _focusRect.left + (width  / 2);
                    //_dy = _focusRect.top  + (height / 2);

                    if ( (_pinCountX != 0) && (_pinCountY != 0) )
                    {
                        float sxX = (float)width  / (float)_pinCountX;
                        float sxY = (float)height / (float)_pinCountY;
                        float sx;

                        if ( sxX < sxY ) sx = sxX; else sx = sxY;

                        if ( (sx >= SXMinimum) && (sx <= SXMaximum) )
                        {
                            SX = sx;
                        }
                    }


                    DX = FocusRect.left + ( _pinCountX/2 );
                    DY = FocusRect.top  + ( _pinCountY/2 );
                }

                if ( Active == true )
                {
                    DoCapture();
                }

                System.Threading.Thread.Sleep(100);
            }
        }









        private bool _mouseLeft;

        protected void DoCapture()
        {
            IntPtr hWnd   = GetDesktopWindow();

            RECT lpRect = new RECT();
            bool ret = GetClientRect(hWnd, out lpRect);

            //todo: tactweb VirtualDevice
            //// Check the keys
            //if ( VirtualDevice != null )
            //{
            //    if ( ConfigMenuMode == true )   DoKeysConfigMenuMode();  else DoKeysRegular();  // -->

            //    // Check allways
            //    DeviceKeyCollection keys = VirtualDevice.Keys;

            //    ChangeConfigMenuMode ( keys.Alt.IsPressed  );    // Key 2 (Toggle ConfigMenuMode)
            //}


            if ( Mode != ScreenCaptureMode.UIA )
            {
                DoCapturePixel(hWnd);

            }

        }



        protected void DoCapturePixel(IntPtr hWnd)
        {
            int w1  = (int) ( _pinCountX / SX);
            int h1  = (int) ( _pinCountY / SX);
            int x1  = DX - w1 / 2;
            int y1  = DY - h1 / 2;


            IntPtr hDC    = GetDC ( hWnd );


            IntPtr hMemDC  = CreateCompatibleDC    (hDC);
            IntPtr hMemBM  = CreateCompatibleBitmap(hDC, _pinCountX + 2, _pinCountY + 2);
            IntPtr hOld    = SelectObject(hMemDC, hMemBM);

            SetStretchBltMode(hMemDC, HALFTONE);    // 4 = HALFTONE
            if( StretchBlt(hMemDC,   0,0, _pinCountX+2, _pinCountY+2,      hDC,  x1,y1, w1, h1,     SRCCOPY) == false)   return;


            BITMAPINFOHEADER   bi = new BITMAPINFOHEADER();
            bi.biSize           = 40;
            bi.biWidth          = _pinCountX+2;    // 76
            bi.biHeight         = _pinCountY+2;    // 48
            bi.biPlanes         = 1;
            bi.biBitCount       = 32;
            bi.biCompression    = BI_RGB;        // 0 = BI_RGB, 3 = BI_BITFIELDS


            if ( GetDIBits (hMemDC, hMemBM, 0, _pinCountY, _pData, ref bi, DIB_RGB_COLORS) == _pinCountY)
            {
                DoFilter();
            }


            // release all objects
            SelectObject (hMemDC, hOld);
            DeleteDC     (hMemDC);
            DeleteObject (hMemBM);
            ReleaseDC    (hWnd, hDC);

        }






        /// <summary>Regular Keys</summary>
        protected void DoKeysRegular()
        {
            //todo: tactweb VirtualDevice
            //if ( VirtualDevice == null )    return; // --> No keys available

            //DeviceKeyCollection keys = VirtualDevice.Keys;

            //if      ( keys.LZoomUp.IsPressed   )  { ChangeZoom( +1 ); }             // +
            //else if ( keys.LZoomDown.IsPressed )  { ChangeZoom( -1 ); }             // -

            //else if ( keys.LUp.IsPressed       )  { InputInjector.MouseMoveRelative( 0,          -MouseStep );  }     // U
            //else if ( keys.LDown.IsPressed     )  { InputInjector.MouseMoveRelative( 0,          +MouseStep );  }     // D
            //else if ( keys.LLeft.IsPressed     )  { InputInjector.MouseMoveRelative( -MouseStep, 0          );  }     // L
            //else if ( keys.LRight.IsPressed    )  { InputInjector.MouseMoveRelative( +MouseStep, 0          );  }     // R



            //else if ( keys.LGesture.IsPressed  )    // Key 1 = Touch
            //{
            //    if ( VirtualDevice.TouchInjector.Target == Rectangle.Empty )
            //    {
            //        VirtualDevice.TouchInjector.Target = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            //    }

            //    VirtualDevice.TouchInjector.Inject();
            //}

            //else if ( keys.LT.IsPressed         )    { ChangeThreshold( +1); }   // Key 3
            //else if ( keys.LSpace.IsPressed     )    { ChangeThreshold( -1); }   // Key 4

            //ChangeEdgeDetection  ( keys.Home.IsPressed );    // Key 5 (Toggle Edge)
            //ChangeInvert         ( keys.Back.IsPressed );    // Key 6 (Toggle Invert)



            //bool mouseLeft = keys.LSelect.IsPressed;    // Key X

            //if ( mouseLeft != _mouseLeft )
            //{
            //    if ( mouseLeft == true )    InputInjector.MouseDownLeft(); else InputInjector.MouseUpLeft();

            //    _mouseLeft = mouseLeft;
            //}
        }

        //todo: tactweb VirtualDevice
        /// <summary>Keys in ConfigMenuMode</summary>
        protected void DoKeysConfigMenuMode()
        {
            //if ( VirtualDevice == null )    return; // --> No keys available

            //DeviceKeyCollection keys = VirtualDevice.Keys;

            ////                      Shift,             Strg,                    Up,                 Down,                 Left,                 Right,                   Plus,                   Minus,                    Enter             
            //Program.ConfigMenu.Key( keys.LT.IsPressed, keys.LSpace.IsPressed,   keys.LUp.IsPressed, keys.LDown.IsPressed, keys.LLeft.IsPressed, keys.LRight.IsPressed,   keys.LZoomUp.IsPressed, keys.LZoomDown.IsPressed, keys.LSelect.IsPressed );


            //if ( ( keys.LT.IsPressed == false ) && ( keys.LSpace.IsPressed == false ) )
            //{
            //    if      ( keys.LZoomUp.IsPressed   )  { Program.ConfigMenu.KeyPlus();   }     // +
            //    else if ( keys.LZoomDown.IsPressed )  { Program.ConfigMenu.KeyMinus();  }     // -

            //    else if ( keys.LUp.IsPressed       )  { Program.ConfigMenu.KeyUp();     }     // U
            //    else if ( keys.LDown.IsPressed     )  { Program.ConfigMenu.KeyDown();   }     // D
            //    else if ( keys.LLeft.IsPressed     )  { Program.ConfigMenu.KeyLeft();   }     // L
            //    else if ( keys.LRight.IsPressed    )  { Program.ConfigMenu.KeyRight();  }     // R

            //    else if ( keys.LSelect.IsPressed   )  { Program.ConfigMenu.KeyEnter();  }     // X
            //}




        }






        private bool _cmm;
        private bool _invert;
        private bool _edgeDetection;

        //todo tactweb
        protected void ChangeConfigMenuMode (bool cmm)
        {
            //if ( cmm != _cmm )
            //{
            //    if ( cmm == true )
            //    {
            //        ConfigMenuMode = !ConfigMenuMode;

            //        if (ConfigMenuMode == true)
            //        {
            //            Speech.Speak( Language.Format("Config an", "Konfig an" ) );
            //            Program.ConfigMenu.SelectedItem.Speak();
            //        }
            //        else
            //        {
            //            Speech.Speak( Language.Format("Config off", "Konfig aus" ) );
            //        }
            //    }




            //    _cmm = cmm;
            //}
        }

        protected void ChangeInvert         (bool invert)
        {
            if ( invert != _invert )
            {
                if ( invert == true )
                {
                    Invert = !Invert;
                }

                _invert = invert;
            }
        }

        /// <summary>Toggle the Mode</summary>
        protected void ChangeEdgeDetection  (bool edgeDetection)
        {
            if ( edgeDetection != _edgeDetection )
            {
                if ( edgeDetection == true )
                {
                    if      ( Mode == ScreenCaptureMode.Threshold     )      Mode = ScreenCaptureMode.EdgeDetection;    // 0 --> 1
                    else if ( Mode == ScreenCaptureMode.EdgeDetection )      Mode = ScreenCaptureMode.UIA;              // 1 --> 2
                    else if ( Mode == ScreenCaptureMode.UIA           )      Mode = ScreenCaptureMode.Threshold;        // 2 --> 0
                }

                _edgeDetection = edgeDetection;
            }
        }

        /// <summary>Change the SX in a direction. Special steps.</summary>
        protected void ChangeZoom           (float direction)
        {
            float step1 = 0.01F;
            float step2 = 0.1F;

            float sx;


            if ( direction > 0 )
            {
                // Vergrößern
                if (SX < 0.1F) sx = SX + step1; else sx = SX + step2;
                if ( sx > SXMaximum ) SX = SXMaximum; else SX = sx;
            }
            else
            {
                // Verkleinern
                if (SX <= 0.11F) sx = SX - step1; else sx = SX - step2;
                if ( sx < SXMinimum ) SX = SXMinimum; else  SX = sx;
            }

        }

        /// <summary>Change the threshold in a direction. Only running if edgedetection is false</summary>
        protected void ChangeThreshold      (int direction)
        {
            if ( Mode != ScreenCaptureMode.Threshold )  return; // -->
            //if ( EdgeDetection == true )    return;


            int th;

            int stepThreshold = 1;

            if ( direction > 0 )
            {
                th = FilterGray.V1 + stepThreshold;
                if ( th > 255 )  FilterGray.V1 = 255; else FilterGray.V1 = th;
            }
            else
            {
                th = FilterGray.V1 - stepThreshold;
                if ( th < 0 )  FilterGray.V1 = 0; else FilterGray.V1 = th;
            }

        }






        protected void DoFilter              ()
        {

            // Teil 1
            int ofs = 0;

            for(int y = _pinCountY; y >= 0; y--)
            {
                for( int x = 0; x < _pinCountX+2; x++)
                {
                    byte blue  = Marshal.ReadByte( _pData, ofs+0 );
                    byte green = Marshal.ReadByte( _pData, ofs+1 );
                    byte red   = Marshal.ReadByte( _pData, ofs+2 );
                    ofs+=4;

                    byte gray  = (byte)(red * 0.30F + green * 0.59F + blue * 0.11F);

                    _red  [x,y] = red;
                    _green[x,y] = green;
                    _blue [x,y] = blue;
                    _gray [x,y] = gray;
                }
            }


            if      ( Mode == ScreenCaptureMode.Threshold     )    DoFilterThreshold();        // -->
            else if ( Mode == ScreenCaptureMode.EdgeDetection )    DoFilterEdgeDetection();    // -->

            //todo tactweb 
            //if ( VirtualDevice != null )
            //{
            //    if ( Invert == true )  { for(int y = 0; y < _pinCountY; y++)  { for( int x = 0; x < _pinCountX; x++)  { VirtualDevice.PinAreaMain[x,y] = !_pins[x+1,y+1];   }  } }
            //    else                   { for(int y = 0; y < _pinCountY; y++)  { for( int x = 0; x < _pinCountX; x++)  { VirtualDevice.PinAreaMain[x,y] =  _pins[x+1,y+1];   }  } }


            //    DoShowMousepointer();   // -->

            //    VirtualDevice.Flush();
            //}

        }


        protected void DoShowMousepointer()
        {
            if ( ShowMousepointer == false)  return;    // -->


            if ( (ShowMousepointerVisibleTogglePrescaler+1) <= 1 )  return; // --> Division by zero

            if ( (Counter % (ShowMousepointerVisibleTogglePrescaler+1) ) == 0 )
            {
                ShowMousepointerVisible = !ShowMousepointerVisible; // Toggle
            }

            //todo: tactweb VirtualDevice

            //if ( VirtualDevice == null )    return;     // -->

            int px = ( _pinCountX / 2 ) - 1;
            int py = ( _pinCountY / 2 ) - 1;


            //VirtualDevice.PinAreaMain[px+0,py+0] = ShowMousepointerVisible;     // Spitze
            //VirtualDevice.PinAreaMain[px+1,py+0] = ShowMousepointerVisible;     // 1. Rechts
            //VirtualDevice.PinAreaMain[px+0,py+1] = ShowMousepointerVisible;     // 1. Unten

            //VirtualDevice.PinAreaMain[px+2,py+0] = ShowMousepointerVisible;     // 2. Rechts
            //VirtualDevice.PinAreaMain[px+0,py+2] = ShowMousepointerVisible;     // 2. Unten
        }



        /// <summary>Filter with Edge Detection (Sobel Operator)</summary>
        protected void DoFilterEdgeDetection ()
        {
            int    limit  = 128 * 128;
            int[,] gx     = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0,   1 } };
            int[,] gy     = new int[,] { {  1, 2, 1 }, {  0, 0, 0 }, { -1, -2, -1 } };
            
            // Scharr
            //int[,] gx     = new int[,] { { 3,10,3 }, { 0,0,0 }, { -3,-10,-3 } };
            //int[,] gy     = new int[,] { { 3,0,-3 }, { 10, 0, -10 }, {  3,0,-3 } };




            for(int y = 1; y < _pinCountY + 2; y++)
            {
                for( int x = 1; x < _pinCountX + 2; x++)
                {
                    int new_rx = 0;
                    int new_ry = 0;
                    int new_gx = 0;
                    int new_gy = 0;
                    int new_bx = 0;
                    int new_by = 0;
                    int rc = 0;
                    int gc = 0;
                    int bc = 0;

                    for (int wi = -1; wi < 2; wi++)
                    {
                        for (int hw = -1; hw < 2; hw++)
                        {
                            rc = _red[x + hw, y + wi];
                            new_rx += gx[wi + 1, hw + 1] * rc;
                            new_ry += gy[wi + 1, hw + 1] * rc;

                            gc = _green[x + hw, y + wi];
                            new_gx += gx[wi + 1, hw + 1] * gc;
                            new_gy += gy[wi + 1, hw + 1] * gc;

                            bc = _blue[x + hw, y + wi];
                            new_bx += gx[wi + 1, hw + 1] * bc;
                            new_by += gy[wi + 1, hw + 1] * bc;
                        }
                    }
                        
                    if (new_rx * new_rx + new_ry * new_ry > limit || new_gx * new_gx + new_gy * new_gy > limit || new_bx * new_bx + new_by * new_by > limit)
                    {
                        _pins[x,y] = true;
                    }
                    else
                    {
                        _pins[x,y] = false;
                    }

                }
            }
        }

        /// <summary>Filter by a threshold</summary>
        protected void DoFilterThreshold     ()
        {

            for(int y = 0; y < _pinCountY + 2; y++)
            {
                for( int x = 0; x < _pinCountX + 2; x++)
                {
                    _pins[x,y] = GetThreshold(x,y);
                }
            }

        }


        protected bool GetThreshold(int x, int y)
        {
            if ( FilterGray.Active  == true )    { if ( ( _gray[x,y]  < FilterGray.V1   ) || ( _gray[x,y]  > FilterGray.V2  ) ) return false; }
            if ( FilterRed.Active   == true )    { if ( ( _red[x,y]   < FilterRed.V1    ) || ( _red[x,y]   > FilterRed.V2   ) ) return false; }
            if ( FilterGreen.Active == true )    { if ( ( _green[x,y] < FilterGreen.V1  ) || ( _green[x,y] > FilterGreen.V2 ) ) return false; }
            if ( FilterBlue.Active  == true )    { if ( ( _blue[x,y]  < FilterBlue.V1   ) || ( _blue[x,y]  > FilterBlue.V2  ) ) return false; }

            return true;
        }












        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;
        private const int SRCCOPY     = 13369376;

        //private struct SIZE
        //{
        //    public int cx;
        //    public int cy;
        //}


        private const int DIB_RGB_COLORS = 0;
        private const int HALFTONE       = 4;
        private const int BI_RGB         = 0;


        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFOHEADER
        {
            public int   biSize;
            public int   biWidth;
            public int   biHeight;
            public short biPlanes;
            public short biBitCount;
            public int   biCompression;
            public int   biSizeImage;
            public int   biXPelsPerMeter;
            public int   biYPelsPerMeter;
            public int   biClrUsed;
            public int   biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CURSORINFO
        {
            public int      cbSize;
            public int      flags;
            public IntPtr   hCursor;
            public POINT    ptScreenPos;
        }



        [StructLayout(LayoutKind.Sequential)]
        private struct GUITHREADINFO
        {
            public int      cbSize;
            public int      flags;
            public IntPtr   hwndActive;
            public IntPtr   hwndFocus;
            public IntPtr   hwndCapture;
            public IntPtr   hwndMenuOwner;
            public IntPtr   hwndMoveSize;
            public IntPtr   hwndCaret;
            public RECT     rcCaret;
        }

        [DllImport("user32.dll")]   private static extern bool      ClientToScreen   (IntPtr hWnd,  ref POINT lpPoint);
        [DllImport("user32.dll")]   private static extern bool      GetGUIThreadInfo (int idThread, ref GUITHREADINFO lpgui);
        [DllImport("user32.dll")]   private static extern bool      GetCursorInfo    (ref CURSORINFO pci);


        [DllImport("user32.dll")]   private static extern bool      GetKeyboardState(byte [] lpKeyState);
        [DllImport("user32.dll")]   private static extern short     GetKeyState(System.Windows.Forms.Keys nVirtKey);



        [DllImport("user32.dll")]   private static extern bool      GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]   private static extern bool      GetCaretPos (out POINT lpPoint);

        [DllImport("user32.dll")]   private static extern bool      SetCursorPos(int X, int Y);

        [DllImport("kernel32.dll")] private static extern int       GetCurrentThreadId    ();
        [DllImport("user32.dll")]   private static extern int       GetWindowThreadProcessId    (IntPtr hWnd, IntPtr lpdwProcessId);
        [DllImport("user32.dll")]   private static extern bool      AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);
        [DllImport("user32.dll")]   private static extern IntPtr    GetForegroundWindow    ();
        [DllImport("user32.dll")]   private static extern IntPtr    GetFocus    ();

        [DllImport("user32.dll")]   private static extern bool      GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]   private static extern bool      GetClientRect(IntPtr hWnd, out RECT lpRect);


        [DllImport("user32.dll")]   private static extern IntPtr    GetDC(IntPtr ptr);
        [DllImport("user32.dll")]   private static extern int       GetSystemMetrics(int abc);
        [DllImport("user32.dll")]   private static extern IntPtr    GetDesktopWindow();
        [DllImport("user32.dll")]   private static extern IntPtr    ReleaseDC(IntPtr hWnd, IntPtr hDc);


        [DllImport("gdi32.dll")]    private static extern bool      BitBlt                 (IntPtr hdcDest,  int nXDest,       int nYDest,       int nWidth,     int nHeight,          IntPtr hdcSrc, int nXSrc,       int nYSrc,                                           int dwRop);
        [DllImport("gdi32.dll")]    private static extern bool      StretchBlt             (IntPtr hdcDest,  int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest,      IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,      int dwRop);
        [DllImport("gdi32.dll")]    private static extern int       SetStretchBltMode      (IntPtr hdc, int iStretchMode);

        [DllImport("gdi32.dll")]    private static extern IntPtr    CreateCompatibleBitmap (IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]    private static extern IntPtr    CreateCompatibleDC     (IntPtr hdc);
        [DllImport("gdi32.dll")]    private static extern IntPtr    SelectObject           (IntPtr hdc, IntPtr bmp);
        [DllImport("gdi32.dll")]    private static extern IntPtr    DeleteDC               (IntPtr hDc);
        [DllImport("gdi32.dll")]    private static extern IntPtr    DeleteObject           (IntPtr hDc);

        [DllImport("gdi32.dll")]    private static extern int       GetDIBits              (IntPtr hdc, IntPtr hbmp, int uStartScan, int cScanLines, IntPtr lpvBits, ref BITMAPINFOHEADER lpbi, int uUsage);

    }
}