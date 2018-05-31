using System;
using System.Runtime.InteropServices;



namespace TactileWeb.UIA
{

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;


        /// <summary>Cast to .NET Point</summary>
        public static implicit operator System.Drawing.Point(POINT point)
        {
            return new System.Drawing.Point(point.x, point.y);
        }

        /// <summary>Cast from .NET Point</summary>
        public static implicit operator POINT (System.Drawing.Point pt)
        {
            POINT p = new POINT();
            p.x = pt.X;
            p.y = pt.Y;

            return p;
        }




        public override string ToString()
        {
            return String.Format("{0},{1}", x,y);
        }
    }

}