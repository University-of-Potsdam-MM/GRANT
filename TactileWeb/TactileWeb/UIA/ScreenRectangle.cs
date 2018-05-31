using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace TactileWeb.UIA
{

    /// <summary>Shows a rectangle on the screen</summary>
    public class ScreenRectangle : IDisposable
    {
        protected Form          _frmLeft;
        protected Form          _frmTop;
        protected Form          _frmRight;
        protected Form          _frmBottom;

        protected Rectangle     _rectangle;
        protected bool          _visible;
        protected Color         _color;
        protected int           _thickness;


        public ScreenRectangle()
        {
            _visible    = false;
            _color      = Color.Blue;
            _thickness  = 3;

            _frmLeft    = CreateForm();
            _frmTop     = CreateForm();
            _frmRight   = CreateForm();
            _frmBottom  = CreateForm();
        }


        protected Form CreateForm()
        {
            Form f = new Form();

            f.FormBorderStyle = FormBorderStyle.None;
            f.ShowInTaskbar   = false;
            f.TopMost         = true;
            f.Width           = 1;
            f.Height          = 1;
            f.BackColor       = _color;

            int style = WinApi.GetWindowLong(f.Handle, WinApi.GWL_EXSTYLE);
            WinApi.SetWindowLong(f.Handle, WinApi.GWL_EXSTYLE,  style | WinApi.WS_EX_TOOLWINDOW);

            return f;
        }















        protected delegate void SetVisibleEventHandler   (bool value);
        protected delegate void SetColorEventHandler     (Color color);
        protected delegate void SetRectangleEventHandler ();


        /// <summary>The color of the rectangle</summary>
        public Rectangle Rectangle
        {
            get
            {
                return _rectangle;
            }
            set
            {
                _rectangle = value;

                try
                {
                    if ( _frmLeft.InvokeRequired )
                    { 
                        _frmLeft.Invoke( new SetRectangleEventHandler(SetRectangle) );  // Problem when we close the MVBD
                    }
                    else
                    {
                        SetRectangle();
                    }
                }
                catch(Exception)
                {}

            }
        }





        /// <summary>Is the rectangle visible</summary>
        public bool Visible
        {
            get
            {
                return _frmLeft.Visible;
            }
            set
            {
                if ( _frmLeft.InvokeRequired )
                {
                    try
                    {
                        _frmLeft.Invoke( new SetVisibleEventHandler(SetVisible), value);
                    }
                    catch(Exception)
                    {
                        // Nothing to do
                    }
                }
                else
                {
                    SetVisible(value);
                }

            }
        }









        /// <summary>The color of the rectangle</summary>
        public Color Color
        {
            get
            {
                return _frmLeft.BackColor;
            }
            set
            {

                if ( _frmLeft.InvokeRequired )
                { 
                    _frmLeft.Invoke( new SetColorEventHandler(SetColor), value);
                }
                else
                {
                    SetColor(value);
                }

            }
        }



        public int Thickness
        {
            get
            {
                return _thickness;
            }
            set
            {
                _thickness = value;
            }
        }




        public void Dispose()
        {
            _frmLeft.Dispose();
            _frmTop.Dispose();
            _frmRight.Dispose();
            _frmBottom.Dispose();
        }


        /// <summary>Call intern with UI thread only</summary>
        protected void SetRectangle ()
        {
            Rectangle r = _rectangle;
            int       t = _thickness;

            //             hWnd                             X                      Y                      Width                         Height
            WinApi.SetWindowPos( _frmLeft.Handle,   WinApi.HWND_TOPMOST,  r.Left - t        ,    r.Top         ,   t                    ,   r.Height   , WinApi.SWP_NOACTIVATE );   // left
            WinApi.SetWindowPos( _frmTop.Handle,    WinApi.HWND_TOPMOST,  r.Left - t        ,    r.Top - t     ,   t + r.Width + t      ,   t          , WinApi.SWP_NOACTIVATE );   // top
            WinApi.SetWindowPos( _frmRight.Handle,  WinApi.HWND_TOPMOST,  r.Right           ,    r.Top         ,   t                    ,   r.Height   , WinApi.SWP_NOACTIVATE );   // right
            WinApi.SetWindowPos( _frmBottom.Handle, WinApi.HWND_TOPMOST,  r.Left - t        ,    r.Bottom      ,   t + r.Width + t      ,   t          , WinApi.SWP_NOACTIVATE );   // bottom
        }


        /// <summary>Call intern with UI thread only</summary>
        protected void SetVisible   (bool value)
        {
            _frmLeft.Visible   = value;
            _frmTop.Visible    = value;
            _frmRight.Visible  = value;
            _frmBottom.Visible = value;
        }


        /// <summary>Call intern with UI thread only</summary>
        protected void SetColor     (Color color)
        {
            _frmLeft.BackColor   = color;
            _frmTop.BackColor    = color;
            _frmRight.BackColor  = color;
            _frmBottom.BackColor = color;
        }

    }
}