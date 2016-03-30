using System;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FilterApplication
{
    class UIA_Filter
    {
      
        internal static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = false)]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsWindowVisible(IntPtr hWnd);

            [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
            public static extern IntPtr GetParent(IntPtr hWnd);


            #region window helper
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern IntPtr WindowFromPoint(CursorPoint lpPoint);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            internal static extern bool SetProcessDPIAware();

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


        // AutomationElement am CursorPoint
        public AutomationElement deliverAutomationElementFromPoint()
        {
            // Convert mouse position from System.Drawing.Point to System.Windows.Point.
            System.Windows.Point point = new System.Windows.Point(cp.X, cp.Y);
            
            AutomationElement element = AutomationElement.FromPoint(point);
            return element;
        }

        // AutomationElement vom HWND
        public static AutomationElement deliverAutomationElementFromHWND(IntPtr hwnd)
        {
            AutomationElement element = AutomationElement.FromHandle(hwnd);

            //element.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);
            return element;
        }

        // ProzessID vom AutomationElement
        public static int deliverAutomationElementID(IntPtr hwnd)
        {
            //window = WindowFromPoint(cp);
            AutomationElement element = AutomationElement.FromHandle(hwnd);

            int processIdentifier = (int)element.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);
            return processIdentifier;
        }

        // topLevel AutomationElement
        // hier wird Thread ermittelt, wird erstmal nicht genutzt
        public uint getProcessIdFromHwnd(IntPtr hwnd)
        {
            uint processID = 0;
            uint PID = NativeMethods.GetWindowThreadProcessId(hwnd, out processID);
            string PID_String = PID.ToString();
            return PID;
        }

        //Main WindowHandle vom Prozess
        public IntPtr getProcessHwndFromHwnd(int processId)
        {        
            Process p = Process.GetProcessById(processId);
            return p.MainWindowHandle;    
        }
        #endregion     
    }
}
