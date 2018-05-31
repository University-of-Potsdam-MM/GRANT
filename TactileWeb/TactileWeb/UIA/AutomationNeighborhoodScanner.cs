using System;
using System.Collections;
using System.Threading;



namespace TactileWeb.UIA
{

    /// <summary>Scanner for neighbourhood UIA elements</summary>
    public class AutomationNeighborhoodScanner
    {
        /// <summary>The connected ScreenCapture object</summary>
        public readonly ScreenCapture   ScreenCapture;


        protected Thread    _thread;
        protected int       _xStart;
        protected int       _yStart;

        protected ArrayList _list;


        public AutomationNeighborhoodScanner(ScreenCapture   screenCapture)
        {
            ScreenCapture = screenCapture;
            _list         = new ArrayList();
        }

        ~AutomationNeighborhoodScanner()
        {
            if ( _thread != null )
            {
                _thread.Abort();
                _thread = null;
            }
        }

        public void Stop()
        {
            if ( _thread != null )
            {
                _thread.Abort();
                _thread = null;
            }

            _list.Clear();
        }

        public void Start(System.Drawing.Point pt)
        {
            Stop();

            _xStart = pt.X;
            _yStart = pt.Y;


            _thread      = new Thread( new ThreadStart( Thread_Event ) );
            _thread.Name = "UIA Automation Neighborhood Scanner";
            _thread.Start();
        }


        protected void Thread_Event()
        { 
            //System.Diagnostics.Debug.Print( "{0} Started", _thread.ManagedThreadId);

            try
            {
                //    _list.Clear();

                //    //todo: tactweb VirtualDevice
                //    //Program.VirtualDevice.PinAreaMain.SetAllPins(false);    // Clear the area
                //    //Program.VirtualDevice.PinControls.Clear();              // Remove all controls


                //    //int w = Program.VirtualDevice.PinAreaMain.Width;
                //    //int h = Program.VirtualDevice.PinAreaMain.Height;

                //    float sx = ScreenCapture.SX;

                //    w = (int)((float)w / sx);
                //    h = (int)((float)h / sx);


                //    int x1 = _xStart;
                //    int x2 = _xStart + w;

                //    int y1 = _yStart;
                //    int y2 = _yStart + h;


                //    for ( int y = y1; y <= y2; y+=10)
                //    {
                //        for ( int x = x1; x <= x2; x+=10)
                //        {
                //            AutomationElementInfo ele  = AutomationElementInfo.GetElementFromPoint(x,y);

                //            if ( ele.ControlType != ControlType.Pane)
                //            {
                //                if ( Exists(ele) == false )
                //                {
                //                    RECT r = ele.Rect;

                //                    PinControls.PinControl ctl = new PinControls.PinControl();
                //                    ctl.Left   = (int)( (float)(r.left   - _xStart   ) * sx);
                //                    ctl.Top    = (int)( (float)(r.top    - _yStart   ) * sx);
                //                    ctl.Width  = (int)( (float)(r.right  - r.left + 1) * sx);
                //                    ctl.Height = (int)( (float)(r.bottom - r.top  + 1) * sx);
                //                    ctl.Text   = ele.Name;

                //                    //todo: tactweb VirtualDevice
                //                    //Program.VirtualDevice.PinControls.Add (ctl);
                //                    //Program.VirtualDevice.Flush();

                //                    _list.Add(ele);
                //                }
                //            }

                //            Thread.Sleep(1);    // We need it for the abort
                //        }
                //    }


                //    //System.Diagnostics.Debug.Print( "{0} Fertig", _thread.ManagedThreadId);
            }
            catch (Exception)
            {
                // Thread abort
                //System.Diagnostics.Debug.Print( "{0} {1}", _thread.ManagedThreadId, ex.Message);
            }

        }



        protected bool Exists(AutomationElementInfo ele)
        {
            RECT r1 = ele.Rect;


            foreach (AutomationElementInfo itm in _list)
            {
                RECT r2 = itm.Rect;

                if ( (r1.left == r2.left) && (r1.right == r2.right) && (r1.top == r2.top) && (r1.bottom == r2.bottom) ) return true;    // -->
            }
        
            return false;
        }



    }
}