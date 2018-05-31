using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

//todo notwendig für ansteuerung der platte ganzer ordner in mvbd
//using TactileWeb.PinControls;


namespace TactileWeb.UIA
{
    public class AutomationElement
    {
        public IUIAutomationElement             ele;
        public string                           Name;
        public ControlType                      ControlType;
        public string                           LocalizedControlType;
        public RECT                             Rect;
        public string                           RuntimeId;
        public bool                             IsEnabled;
        public IntPtr                           Handle;
        public string                           Text;
        public string                           CursorInfo;
        public string                           ItemInfo;

        public string                           PatternText;

        /// <summary>Speak UIA element when changed</summary>
        public bool                             SpeakElement;


        /// <summary>Show the UIA element in the brailleline of the device</summary>
        public bool                             ShowElementInBrailleline;


        /// <summary>The connected ScreenCapture object</summary>
        public readonly ScreenCapture           ScreenCapture;

        /// <summary>Scanner for neighbourhood elements</summary>
        public AutomationNeighborhoodScanner    NeighborhoodScanner;


        protected IUIAutomationTreeWalker       _controlWalker;
        protected IUIAutomationTreeWalker       _contentWalker;
        protected IUIAutomationTreeWalker       _rawViewWalker;


        protected IntPtr                        retVal;
        protected ScreenRectangle               _screenRectangle;
        protected PatternCollection             _allPattern;

        protected ActionThread                  _thread;

        protected string                        _lastName;
        protected float                         _lastSX;
        protected System.Drawing.Point          _lastMousePosition;



        public AutomationElement(ScreenCapture screenCapture)
        {
            ScreenCapture   = screenCapture;
            _screenRectangle = new ScreenRectangle();

            int ret;
            ret = ScreenCapture.UIA.get_ControlViewWalker (out _controlWalker); 
            ret = ScreenCapture.UIA.get_ContentViewWalker (out _contentWalker);
            ret = ScreenCapture.UIA.get_RawViewWalker(out _rawViewWalker);

            _allPattern = new PatternCollection();


            _thread = new ActionThread( Scan, "Automation Element" );
            _thread.Start();
        }


        protected string GetRuntimeId(IUIAutomationElement ele)
        {
            IntPtr intArray;

            if (ele.GetRuntimeId(out intArray) == 0)
            {
                IntPtr array;
                int count;

                if (ScreenCapture.UIA.IntSafeArrayToNativeArray(intArray, out array, out count) == 0)
                {
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < count; i++)
                    {
                        int v = Marshal.ReadInt32(array, i * 4);
                        sb.AppendFormat("{0} ", v);
                    }

                    if (sb.Length != 0) sb.Length -= 1;

                    return sb.ToString();
                }
            }

            return null;
        }


        public static RECT[] GetBoundingRectangles(IntPtr boundingRects)
        {
            IntPtr array;
            int    count;

            if ( ScreenCapture.UIA.SafeArrayToRectNativeArray(boundingRects, out array, out count) == 0 )
            {
                RECT[] ra = new RECT[count];

                int ofs = 0;
                for (int i = 0; i < count; i++)
                {
                    ra[i].left   = Marshal.ReadInt32(array, ofs); ofs+=4;
                    ra[i].top    = Marshal.ReadInt32(array, ofs); ofs+=4;
                    ra[i].right  = Marshal.ReadInt32(array, ofs); ofs+=4;
                    ra[i].bottom = Marshal.ReadInt32(array, ofs); ofs+=4;
                }

                return ra;
            }

            return new RECT[0];
        }




        protected void      Scan()
        {
            while (true)
            {

                if ( ScreenCapture.FollowMousePointer == true )
                {
                    System.Drawing.Point mousePosition = ScreenCapture.MousePosition;

                    if ( mousePosition != _lastMousePosition )
                    {
                        ScreenCapture.UIA.ElementFromPoint( mousePosition, out ele );

                        _lastMousePosition = mousePosition;
                    }
                }

                if ( ScreenCapture.FollowFocus == true )
                {
                    ele = ScreenCapture.FocusElement;
                }



                Name                 = null;
                ControlType          = ControlType.Unknown;
                LocalizedControlType = null;
                Rect                 = new RECT();
                RuntimeId            = null;
                CursorInfo           = null;
                ItemInfo             = null;
                IsEnabled            = true;
                PatternText          = null;
                Handle               = IntPtr.Zero;

                if ( ele == null )
                {
                    _screenRectangle.Visible = false;
                }
                else
                {
                    if ( ele.get_CurrentName                   (out retVal ) == 0 )   Name                  = Marshal.PtrToStringAuto(retVal);
                    if ( ele.get_CurrentLocalizedControlType   (out retVal ) == 0 )   LocalizedControlType  = Marshal.PtrToStringAuto(retVal);

                    ele.get_CurrentControlType                 (out ControlType  );
                    ele.get_CurrentBoundingRectangle           (out Rect         );
                    RuntimeId       = GetRuntimeId(ele);

                    _screenRectangle.Visible   = false;
                    _screenRectangle.Rectangle = Rect;

                    ele.get_CurrentIsEnabled          (out IsEnabled);
                    int ret = ele.get_CurrentNativeWindowHandle (out Handle);

                    if      ( ControlType == ControlType.ListItem )  ScanItem(ele, RuntimeId);
                    else if ( ControlType == ControlType.MenuItem )  ScanItem(ele, RuntimeId);
                    else if ( ControlType == ControlType.TreeItem )  ScanItem(ele, RuntimeId);
                    else if ( ControlType == ControlType.DataItem )  ScanDataItem();

                    else if ( ControlType == ControlType.Group    ) ScanGroupTest(ele, RuntimeId, _lastMousePosition);                       // https://docs.google.com/document/d/14Uth-f6IAZYfzGk-aKnPOU9QhU02k1ktpn7iRcH2nkw/edit#

                    else if ( ControlType == ControlType.Edit     )  ScanEdit( _lastMousePosition );

                    if ( Handle != IntPtr.Zero )
                    {
                        int Line = (int)SendMessage(Handle, EM_LINEFROMCHAR, -1, 0);
                        int sel  = (int)SendMessage(Handle, EM_GETSEL,        0, 0);
                
                        int curStart = sel & 0xffff;
                        int sel2     = (int)((uint)(sel & 0xffff0000) >> 4);

                        int lnStart  = (int)SendMessage(Handle, EM_LINEINDEX,  -1, 0);
                        int lnLength = (int)SendMessage(Handle, EM_LINELENGTH, -1, 0);

                        int Col = curStart - lnStart;

                        CursorInfo = String.Format("Z {0} S {1}  lnStart:{2} lnLength{3} ", Line, Col, lnStart, lnLength);
                    }

                    //PatternText = ScanPattern();

                }


                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Name:        {0}\r\n", Name);
                sb.AppendFormat("CT:          {0}\r\n", ControlType);    // TreeItem
                sb.AppendFormat("LCT:         {0}\r\n", LocalizedControlType);   // Strukturelement
                sb.AppendFormat("IsEnabled:   {0}\r\n", IsEnabled   );

                int width  = Rect.right  - Rect.left + 1;
                int height = Rect.bottom - Rect.top  + 1;
                sb.AppendFormat("RECT:        {0},{1}  {2}x{3}\r\n", Rect.left, Rect.top, width, height);
                sb.AppendFormat("ID:          {0}\r\n", RuntimeId   );
                sb.AppendFormat("HWND:        {0}\r\n", Handle      );
                sb.AppendFormat("Cursor:      {0}\r\n", CursorInfo  );

                sb.AppendFormat("ItemInfo:    \r\n"  );
                sb.AppendFormat("{0}\r\n", ItemInfo     );
                sb.AppendLine();

                //sb.AppendFormat("PatternText: \r\n"  );
                //sb.AppendLine();
                //sb.AppendFormat("{0}\r\n", PatternText     );


                Text = sb.ToString();


                // Speak and show information of the element
                if ( (Name != _lastName) || (ScreenCapture.SX != _lastSX) )
                {
                    string text;

                    if (Name == null)
                    {
                        text = LocalizedControlType;        // Schaltfläche
                    }
                    else
                    {
                        if ( ItemInfo == null ) { text = Name;                                          }   // "Ein Text"
                        else                    { text = String.Format("{0}\r\n{1}", Name, ItemInfo);   }   // "Ein Ordner... Element 3 von 5
                    }

                    if (SpeakElement == true)
                    {
                        //todo tactweb
                        //Speech.Speak(text);
                    }

                    //todo: tactweb VirtualDevice
                    if ( ShowElementInBrailleline == true )
                    {
                        //Program.VirtualDevice.PinAreaBrailleline.SetAllPins(false);             // Clear the area
                        //Program.VirtualDevice.PinAreaBrailleline.DrawBraillelineText(text);     // Draw the text in braille
                    }


                    if ( ScreenCapture.Active == false )
                    {
                        if ( NeighborhoodScanner != null )
                        {
                            NeighborhoodScanner.Stop();
                        }
                    }
                    else
                    {
                        if ( ScreenCapture.Mode == ScreenCaptureMode.UIA )
                        {
                            if ( NeighborhoodScanner == null )     NeighborhoodScanner = new AutomationNeighborhoodScanner(ScreenCapture);


                            if (ControlType == ControlType.Edit)
                            {
                                // Word Multiline
                                NeighborhoodScanner.Stop();

                                //todo: tactweb VirtualDevice
                                //Program.VirtualDevice.PinAreaMain.SetAllPins(false);
                                //Program.VirtualDevice.PinControls.Clear();

                                //Program.VirtualDevice.PinAreaMain.DrawBrailleTextMultiline ( text );
                                //Program.VirtualDevice.Flush();
                            }
                            else
                            {
                                NeighborhoodScanner.Start( new System.Drawing.Point(Rect.left, Rect.top) );                           // Scan the neighborhood-->
                            }
                        }
                    }


                    _lastName = Name;
                    _lastSX   = ScreenCapture.SX;
                }



                Thread.Sleep(200);
            }
        }

        protected void ScanPatternTest(IUIAutomationElement ele, string runtimeId)
        {
            //ele.GetCurrentPattern()
            ;
            ScanPattern();
        }

        protected void ScanGroupTest(IUIAutomationElement ele, string runtimeId, System.Drawing.Point _lastMousePosition)
        {

            ScanEditTest(_lastMousePosition);

            ScanPattern();
            ScanPattern();

            //PatternText = ScanPattern();
            //string helpString = ele.GetCurrentPattern().Text;// as string;

            //AutomationElement.EM_GETSEL

            //if(ele.get_CurrentControllerFor())

            //AutomationElementInfo_Test(ele);

            IUIAutomationTreeWalker walker = _rawViewWalker;//_contentWalker;//_controlWalker;
            //IUIAutomationElement parent = null;

            //if (walker.GetParentElement(ele, out parent) == 0)
            //{
            //    if (parent != null)
            //    {
                    StringBuilder sb = new StringBuilder();

                    int count = 0;
                    IUIAutomationElement child = null;
                    IUIAutomationElement next = null;

                    if (walker.GetFirstChildElement(ele, out child) == 0)
                    {
                        while (child != null)
                        {
                            count++;

                            string name;
                            if (child.get_CurrentName(out retVal) == 0)
                            {
                                name = Marshal.PtrToStringAuto(retVal);
                                sb.AppendFormat("{0}={1}", count, name);
                            }

                            if (walker.GetNextSiblingElement(child, out next) != 0) break; // -->

                            child = next;
                        }
                    //}

                    Name = sb.ToString();
                //}

                //AutomationElementInfo_Test(ele);

                //System.Diagnostics.Debug.WriteLine("test" + ele.get_CurrentControllerFor().ToString() + ControlType.ToString() + Name);
            }
        }

        protected void ScanEditTest(System.Drawing.Point pt)
        {
            IntPtr pUnk;

            if (ele.GetCurrentPattern(PATTERNID.UIA_TextPatternId, out pUnk) != 0) return; // -->
            if (pUnk == IntPtr.Zero) return; // -->

            IUIAutomationTextPattern p = (IUIAutomationTextPattern)Marshal.GetObjectForIUnknown(pUnk);


            int x = pt.X;
            int y = pt.Y;

            IUIAutomationTextRangeArray ranges;

            if (p.GetVisibleRanges(out ranges) != 0) return;

            int length;
            ranges.get_Length(out length);

            //for (int i = 0; i < length; i++)
            //{
            //    IUIAutomationTextRange range;

            //    if (ranges.GetElement(i, out range) == 0)
            //    {
            //        IntPtr boundingRects;
            //        if (range.GetBoundingRectangles(out boundingRects) == 0)
            //        {
            //            RECT[] ra = AutomationElement.GetBoundingRectangles(boundingRects);

            //            foreach (RECT r in ra)
            //            {
            //                if ((x >= r.left) && (x <= r.right) && (y >= r.top) && (y <= r.bottom))
            //                {
            //                    IntPtr ptr;
            //                    if (range.GetText(-1, out ptr) == 0)
            //                    {
            //                        Name = Marshal.PtrToStringAuto(ptr);
            //                    }
            //                }
            //            }

            //        }
            //    }
            //}

        }


        /// <summary>Creates infomation from the COM AutomationElement</summary>
        protected void AutomationElementInfo_Test(IUIAutomationElement ele)
        {
            IntPtr retVal;

            if (ele.get_CurrentName(out retVal) == 0)
            {
                Name = Marshal.PtrToStringAuto(retVal);
            }

            ele.get_CurrentControlType(out ControlType);
            ele.get_CurrentBoundingRectangle(out Rect);

            //if (ControlType == ControlType.DataItem)
            //{
            //    string text = GetDataItemText(ele);

            //    if (text != null)
            //    {
            //        Name = text;
            //    }

            //}

            //ele.get_CurrentControlType()
            string test = ControlType.GetHashCode().ToString();

            //ControlType..GetName()
            if (ControlType.ToString() == "Group")
            {
                System.Diagnostics.Debug.WriteLine("test 1 - " + ele.get_CurrentControllerFor().ToString() + " Q " + ControlType.ToString() + " Q " + Name + " Q " + test.ToString() + " Q ");// + ele.get.GetCurrentPropertyValueEx);
                //System.Diagnostics.Debug.WriteLine("test 2 - " + Text.ToString());

            }
        }


        protected void      ScanGroup()
        {
            IUIAutomationTreeWalker walker = _controlWalker;//_contentWalker;//_controlWalker;
            IUIAutomationElement        parent = null;

            if ( walker.GetParentElement(ele, out parent) == 0 )
            {
                if ( parent != null)
                {
                    StringBuilder sb = new StringBuilder();

                    int count = 0;
                    IUIAutomationElement child = null;
                    IUIAutomationElement next = null;

                    if ( walker.GetFirstChildElement(ele, out child) == 0 )
                    {
                        while(child != null)
                        {
                            count++;

                            string name;
                            if ( child.get_CurrentName (out retVal ) == 0 ) 
                            {
                                name = Marshal.PtrToStringAuto(retVal);
                                sb.AppendFormat("{0}={1}", count, name);
                            }

                            if ( walker.GetNextSiblingElement(child, out next) != 0 )   break; // -->

                            child = next;
                        }
                    }

                    Name = sb.ToString();
                }
            }


        }

        //    //if ( ele.GetCurrentPattern (PATTERNID.UIA_ItemContainerPatternId, out pUnk) != 0 )  return; // -->
        //    //if ( pUnk == IntPtr.Zero )                                                  return; // -->

        //    //IUIAutomationValuePattern  p = (IUIAutomationValuePattern)Marshal.GetObjectForIUnknown(pUnk);

        //    //IntPtr retval;
        //    //if ( p.get_CurrentValue(out retval) != 0 )  return; // -->
        //    //string text = Marshal.PtrToStringAuto(retval);

        //    //if ( text == null ) return; // -->

        //    //string[] sa = text.Split('\r');

        //    //if ( sa.Length == 1 )    Name = sa[0];          // -->
        //    //if ( sa.Length == 2 )    Name = sa[1].Trim();   // -->
        //}



        /// <summary>Scan as an Excel cell</summary>
        protected void      ScanDataItem()
        {
            IntPtr pUnk;

            if ( ele.GetCurrentPattern (PATTERNID.UIA_ValuePatternId, out pUnk) != 0 )  return; // -->
            if ( pUnk == IntPtr.Zero )                                                  return; // -->

            IUIAutomationValuePattern  p = (IUIAutomationValuePattern)Marshal.GetObjectForIUnknown(pUnk);

            IntPtr retval;
            if ( p.get_CurrentValue(out retval) != 0 )  return; // -->
            string text = Marshal.PtrToStringAuto(retval);

            if ( text == null ) return; // -->

            string[] sa = text.Split('\r');

            if ( sa.Length == 1 )    Name = sa[0];          // -->
            if ( sa.Length == 2 )    Name = sa[1].Trim();   // -->
        }


        /// <summary>Scan as a text in Word</summary>
        protected void      ScanEdit(System.Drawing.Point pt)
        {
            IntPtr pUnk;

            if ( ele.GetCurrentPattern (PATTERNID.UIA_TextPatternId, out pUnk) != 0 )   return; // -->
            if ( pUnk == IntPtr.Zero )                                                  return; // -->

            IUIAutomationTextPattern p = (IUIAutomationTextPattern)Marshal.GetObjectForIUnknown(pUnk);


            int x = pt.X;
            int y = pt.Y;

            IUIAutomationTextRangeArray ranges;

            if ( p.GetVisibleRanges(out ranges) != 0 )  return;

            int length;
            ranges.get_Length(out length);

            for (int i = 0; i < length; i++)
            {
                IUIAutomationTextRange      range;

                if ( ranges.GetElement(i, out range) == 0 )
                {
                    IntPtr boundingRects;
                    if ( range.GetBoundingRectangles(out boundingRects) == 0 )
                    {
                        RECT[] ra = AutomationElement.GetBoundingRectangles(boundingRects);

                        foreach(RECT r in ra)
                        {
                            if ( (x >= r.left) && (x <= r.right) && (y >= r.top) && (y <= r.bottom) )
                            {
                                IntPtr ptr;
                                if ( range.GetText(-1, out ptr) == 0 )
                                {
                                    Name = Marshal.PtrToStringAuto(ptr);
                                }
                            }
                        }

                    }
                }
            }

        }


        protected string    ScanPattern()
        {
            StringBuilder sb = new StringBuilder();

            foreach(Pattern p in _allPattern)
            {
                p.Refresh(this);
            }

            foreach(Pattern p in _allPattern)
            {
                System.Diagnostics.Debug.WriteLine(p.Id + p.Text);
                if ( p.Text != null )
                {
                    sb.AppendFormat("{0,-20}: {1}\r\n", p.Name, p.Text, p.Id);
                }

                if (p.Id.ToString()== "UIA_TextChildPatternId")
                {

                }

            }

            if (sb.Length != 0) sb.Length -= 2;

            if (sb.Length == 0)
            {
                return null;
            }
            else
            {
                return sb.ToString();
            }

        }

        //todo auskommentiert - ungenutzt tactweb 
        //AutomationElement GetListItemParent(AutomationElement listItem)
        //{
        //    if (listItem == null) throw new ArgumentException();
        //    SelectionItemPattern pattern = listItem.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
        //    if (pattern == null)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        SelectionItemPattern.SelectionItemPatternInformation properties = pattern.Current;
        //        return properties.SelectionContainer;
        //    }
        //}


        /// <summary>ListItem, MenuItem, TreeItem</summary>
        protected void      ScanItem(IUIAutomationElement ele, string runtimeId)
        {
            IUIAutomationTreeWalker walker = _controlWalker;
            IUIAutomationElement    parent = null;

            if ( walker.GetParentElement(ele, out parent) == 0 )
            {
                if ( parent != null)
                {

                    int found = 0;  // Element 5 von 10
                    int count = 0;


                    IUIAutomationElement item = null;
                    IUIAutomationElement next = null;

                    if ( walker.GetFirstChildElement(parent, out item) == 0 )
                    {
                        while(item != null)
                        {
                            count++;

                            string runtimeId2 = GetRuntimeId(item);

                            if ( runtimeId2 == runtimeId ) found = count;

                            if ( walker.GetNextSiblingElement(item, out next) != 0 )   break;

                            item = next;
                        }
                    }

                    if ( count != 0 )
                    {
                        //todo tactweb

                        //ItemInfo = Language.Format("Element {0} von {1}\r\n", "Element {0} of {1}\r\n", found, count);
                    }

                }
            }


        }

 

        private static int EM_LINEINDEX     = 0xBB;
        private static int EM_LINELENGTH    = 0xC1;

        private static int EM_LINEFROMCHAR  = 0xC9;
        private static int EM_GETSEL        = 0xB0;

        [DllImport("user32.dll")]   private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

    }
}