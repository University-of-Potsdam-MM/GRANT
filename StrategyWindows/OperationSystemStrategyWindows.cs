using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GRANTManager.Interfaces;
using System.Windows;
using System.Drawing;
using OSMElement;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

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

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool UpdateWindow(IntPtr hWnd);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool InvalidateRect(IntPtr hwnd, IntPtr lpRect, bool
            bErase);

            [DllImport("User32.dll")]
            public static extern Int32 SetForegroundWindow(int hWnd);

            [DllImport("User32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            [DllImport("User32.dll")]
            public static extern bool IsIconic(IntPtr hWnd);
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

        // refresht Ansicht des übergebenen hwnd
        public bool updateWindow(IntPtr hwnd)
        {
            try
            {
                NativeMethods.UpdateWindow(hwnd);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei UpdateWindow: " + e.Message);
            }
        }

        public Rectangle getRect(OSMElement.OSMElement osmElement)
        {
            int x = (int)osmElement.properties.boundingRectangleFiltered.TopLeft.X;
            int y = (int)osmElement.properties.boundingRectangleFiltered.TopLeft.Y;
            int x2 = (int)osmElement.properties.boundingRectangleFiltered.TopRight.X;
            int y2 = (int)osmElement.properties.boundingRectangleFiltered.BottomLeft.Y;
            int height = y2 - y;
            int width = x2 - x;
            // Create rectangle.
            return new Rectangle(x,y,width,height);
        }
        
        public void paintRect(Rectangle rect)
        {            
            Graphics desktop = Graphics.FromHwnd(NativeMethods.GetDesktopWindow());           
            Graphics newGraphics = desktop;
            Pen redPen = new Pen(Color.Red, 5);
            newGraphics.DrawRectangle(redPen, rect);

            System.Threading.Thread.Sleep(1000);
            
            NativeMethods.InvalidateRect(IntPtr.Zero, IntPtr.Zero , true);

            //updatewindow säubert nicht ordentlich den screen
            //updateWindow(IntPtr.Zero);

            newGraphics.Dispose();
        }

        public void paintScreenWithoutRect(Rectangle rect)
        {
            //System.Windows//.Automation.Automationelement screenHWND = NativeMethods.GetDesktopWindow();

              //              OSMElement.OSMElement osmElement = new OSMElement.OSMElement();

            IntPtr screen = NativeMethods.GetDesktopWindow();
            

            //osmElement.properties = setProperties(mouseElement);

            Graphics desktop = Graphics.FromHwnd(screen);
            Graphics newGraphics = desktop;

            //Rectangle rect = operationSystemStrategy.getRect(osmElement);
        }


        public void paintRect_Test(Rectangle rect)
        {            
            Graphics desktop = Graphics.FromHwnd(NativeMethods.GetDesktopWindow());           
           
            Graphics newGraphics = desktop;

         //   newGraphics.SetClip(rect);
//            System.Drawing.Drawing2D.GraphicsContainer cont;

//            cont = newGraphics.BeginContainer();

            //Graphics newGraphics2 = newGraphics;
            //newGraphics2.beg
            
            Pen redPen = new Pen(Color.Red, 5);
            //redPen.
            newGraphics.DrawRectangle(redPen, rect);
            //newGraphics.Dispose();



            System.Threading.Thread.Sleep(1000);

            //newGraphics.Clear(System.Drawing.Color.Transparent);

       //     newGraphics.ResetClip();


//           newGraphics.EndContainer(cont);

            //newGraphics.ResetTransform();
            
            //NativeMethods.InvalidateRect(NativeMethods.GetDesktopWindow(),null, true);
            NativeMethods.InvalidateRect(IntPtr.Zero ,IntPtr.Zero , true);

           //updateWindow(NativeMethods.GetDesktopWindow());

            ///newGraphics.ResetClip();

           
//Console.WriteLine("x: " + osmElement.properties.boundingRectangleFiltered.TopLeft.X);

            //IntPtr points = operationSystemStrategy.getHWND();
            //IntPtr MainHWND = operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points));

            // Create new graphics object using handle to window.
  //          Graphics newGraphics = Graphics.FromHwnd(deliverDesktopHWND());

    //        System.Drawing.Size s = this.Size;


      //      Graphics newGraphics2 = Graphics.FromHwnd(deliverDesktopHWND());

        //    newGraphics2.CopyFromScreen();

            // Draw rectangle to screen.
          //  newGraphics.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 5), x, y, width, height);

            // Dispose of new graphics.
            newGraphics.Dispose();

        }


        /// <summary>
        /// Ermittelt ob eine Anwendung geöffnet ist
        /// </summary>
        /// <param name="appMainModulName">gibt den MainModulName der gewünschten Anwendung an</param>
        /// <returns> Handle der Anwendung, falls die Anwendung geöffnet ist; sonst <c>IntPtr.Zero</c></returns>
        public IntPtr isApplicationRunning(string appMainModulName)
        {
            if (appMainModulName == null) { return IntPtr.Zero; }
            foreach (Process clsProcess in Process.GetProcesses())
            {
                try
                {
                    if (!clsProcess.MainWindowHandle.Equals(IntPtr.Zero) && clsProcess.MainModule.ModuleName.Equals(appMainModulName))
                    {
                        return clsProcess.MainWindowHandle;
                    }
                }
                catch (System.ComponentModel.Win32Exception) { }
                catch (InvalidOperationException) { }
            }
            return IntPtr.Zero;
        }

      //  public bool isApplicationInForeground(String appMainModulName){


      //  }

        /// <summary>
        /// Ermittelt den Namen der Anwendung zurück
        /// </summary>
        /// <param name="name">Titel der Anwendung</param>
        /// <returns></returns>
        public String getModulNameOfApplication(String name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.MainWindowTitle.Contains(name))
                {
                    return clsProcess.MainModule.ModuleName;
                }
            }
            return null;
        }

        /// <summary>
        /// Ermittelt Namen inkl. Pfad der gefilterten Anwendung an
        /// </summary>
        /// <param name="name">gibt den Titel der Anwendung an</param>
        /// <returns>Namen inkl. Pfad der gefilterten Anwendung</returns>
        public String getFileNameOfApplicationByMainWindowTitle(String name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.MainWindowTitle.Contains(name))
                {
                    return clsProcess.MainModule.FileName;
                }
            }
            return null;
        }

        /// <summary>
        /// Ermittelt Namen inkl. Pfad der gefilterten Anwendung an
        /// </summary>
        /// <param name="name">gibt den Modul-Namen der Anwendung an</param>
        /// <returns>Namen inkl. Pfad der gefilterten Anwendung</returns>
        public String getFileNameOfApplicationByModulName(String modulName)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                try
                {
                    if (!clsProcess.MainWindowHandle.Equals(IntPtr.Zero) && clsProcess.MainModule.ModuleName.Contains(modulName))
                    {
                        return clsProcess.MainModule.FileName;
                    }
                }
                catch (System.ComponentModel.Win32Exception){}
            }
            return null;
        }

        /// <summary>
        /// Startete eine Anwendung
        /// </summary>
        /// <param name="name">Gibt den Namen (inkl. Pfad) der Anwendung an</param>
        /// <returns><c>true</c> falls die Anwendung gestartet wurde; sonst <c>false</c></returns>
        public bool openApplication(string name)
        {
            if (name == null || name.Equals("")) { Console.WriteLine("Kein Name vorhanden!"); return false; }
            try
            {
                Process p = Process.Start(@name);
                if (p != null) 
                {
                    Thread.Sleep(250);
                    return true;
                }
            }
            catch (ObjectDisposedException) { }
            catch (FileNotFoundException) { }

            return false;
        }

        /// <summary>
        /// Aktiviert eine Anwendung
        /// -> wird benötigt beim Filtern, falls die Anwendung minimiert ist
        /// </summary>
        /// <param name="hwnd">gibt den Handle der anwendung an</param>
        /// <returns><c>true</c>, falls die anwendung aktiviert wurde; sonst <c>false</c></returns>
        public bool showWindow(IntPtr hwnd)
        {
            if (NativeMethods.IsIconic(hwnd))
            {
                return NativeMethods.ShowWindow(hwnd, 9); // https://msdn.microsoft.com/de-de/library/windows/desktop/ms633548(v=vs.85).aspx
            }
            return true;
        }
    }
    
}
