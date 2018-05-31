using System;
using System.Runtime.InteropServices;



namespace TactileWeb.UIA
{

    // https://msdn.microsoft.com/en-us/library/windows/desktop/dd162897(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int  left;
        public int  top;
        public int  right;
        public int  bottom;

        /// <summary>Cast to .NET Rectangle</summary>
        public static implicit operator System.Drawing.Rectangle(RECT rect)
        {
            return System.Drawing.Rectangle.FromLTRB( rect.left, rect.top, rect.right, rect.bottom );
        }

        /// <summary>Cast from .NET Rectangle</summary>
        public static implicit operator RECT (System.Drawing.Rectangle rectangle)
        {
            RECT r = new RECT();

            r.left   = rectangle.Left;
            r.top    = rectangle.Top;
            r.right  = rectangle.Right;
            r.bottom = rectangle.Bottom;

            return r;
        }


        public override string ToString()
        {
            return String.Format("X:{0}-{1} Y:{2}-{3}",  left, right, top,bottom );
        }

    }

}