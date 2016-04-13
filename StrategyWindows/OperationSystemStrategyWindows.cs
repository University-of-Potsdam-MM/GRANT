using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using StrategyManager.Interfaces;

namespace StrategyWindows
{
    public class OperationSystemStrategyWindows : IOperationSystemStrategy
    {
        internal static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            internal static extern bool SetProcessDPIAware();

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern IntPtr WindowFromPoint(CursorPoint lpPoint);



            [System.Runtime.InteropServices.DllImport("user32.dll")]
            internal static extern bool GetPhysicalCursorPos(ref CursorPoint lpPoint);

        }

        public CursorPoint cp = new CursorPoint();

        public struct CursorPoint
        {
            public int X;
            public int Y;

            public CursorPoint(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }


        // ermittel Cursor Position
        public bool deliverCursorPosition()
        {
            NativeMethods.SetProcessDPIAware();
            try
            {
                return NativeMethods.GetPhysicalCursorPos(ref cp);
            }
            catch (EntryPointNotFoundException) // Not Windows Vista
            {
                return false;
            }
        }

        //Gib Handle an CursorPostion zurück
        public IntPtr getHWND()
        {
            //GetPhysicalCursorPos(ref cp);
            IntPtr hwnd = NativeMethods.WindowFromPoint(cp);
            return hwnd;
        }

        // Main WindowHandle vom Prozess
        public IntPtr getProcessHwndFromHwnd(int processId)
        {
            Process p = Process.GetProcessById(processId);
            return p.MainWindowHandle;
        }
    }
}
