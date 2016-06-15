using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using StrategyManager.Interfaces;
using System.Windows;
using System.Drawing;
using OSMElement;

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

            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = false)]
            public static extern IntPtr GetDesktopWindow();
        }

        private CursorPoint cp = new CursorPoint();

        public CursorPoint Cp
        {
            get
            {
                return cp;
            }

            set
            {
                cp = value;
            }
        }

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

        public void getCursorPoint(out int x, out int y)
        {
            x = cp.X;
            y = cp.Y;

        }

        // ermittel Desktoppu
        public IntPtr deliverDesktopHWND()
        {
           try
            {
                return NativeMethods.GetDesktopWindow();
                
            }
            catch (Exception e) // 
            {
                throw new Exception("Fehler bei DesctopHWND: " + e.Message);
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
        // Fehlerbehandlung wie?
        public IntPtr getHWND()
        {
            try
            {
                IntPtr hwnd = NativeMethods.WindowFromPoint(Cp);
                return hwnd;
            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei getHWND: " + e.Message);
            }
        }

        // Main WindowHandle vom Prozess
        public IntPtr getProcessHwndFromHwnd(int processId)
        {
            try
            {
                Process p = Process.GetProcessById(processId);
                return p.MainWindowHandle;
            }
            catch (ArgumentException a)
            {
                throw new ArgumentException("Fehler bei MainWindowHandle: " +a.Message);
            }
            catch (InvalidOperationException i)
            {
                throw new InvalidOperationException("Fehler bei MainWindowHandle: " + i.Message);
            }
        }
        public void paintMouseRect(OSMElement.OSMElement osmElement)
        {
            int x = (int)osmElement.properties.boundingRectangleFiltered.TopLeft.X;
            int y = (int)osmElement.properties.boundingRectangleFiltered.TopLeft.Y;
            int x2 = (int)osmElement.properties.boundingRectangleFiltered.TopRight.X;
            int y2 = (int)osmElement.properties.boundingRectangleFiltered.BottomLeft.Y;
            //IntPtr points = osmElement.properties.hWndFiltered;
            int height = y2 - y;
            int width = x2 - x;


            Console.WriteLine("x: " + osmElement.properties.boundingRectangleFiltered.TopLeft.X);

            //IntPtr points = operationSystemStrategy.getHWND();
            //IntPtr MainHWND = operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points));

            // Create new graphics object using handle to window.
            Graphics newGraphics = Graphics.FromHwnd(deliverDesktopHWND());

            // Draw rectangle to screen.
            newGraphics.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 5), x, y, width, height);

            // Dispose of new graphics.
            newGraphics.Dispose();
        }
    }
    
}
