using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using GRANTManager.Interfaces;
using System.Drawing;
using System.IO;
using System.Threading;
using GRANTManager;

namespace StrategyWindows
{
    public class OperationSystemStrategyWindows : IOperationSystemStrategy
    {
        private StrategyManager strategyMgr;
        Thread paintRecThread;
        
        /// <summary>
        /// Methode aus Interface IOperationSystemStrategy
        /// </summary>
        /// <param name="manager"></param>
        public void setStrategyMgr(StrategyManager manager) 
        { strategyMgr = manager; }

        //todo warum wird methode windowsEventsHandler.setStrategyMgr aufgerfuen und gleichzeitig übergabe des strategymgr in windowseventhandler? z38 auskommentiert
        /// <summary>
        /// Init des eventhandler, mit übergabe des tartagymanager für nutzung von diesem, und eventmonitor mit übergabe des eventhandler, da dieser in eventmonitor erzeugt und methoden daraus genutzt werden
        /// </summary>
        public OperationSystemStrategyWindows(StrategyManager manager)
        {
            strategyMgr = manager;
            windowsEventsHandler = new Windows_EventsHandler(strategyMgr);
            //windowsEventsHandler.setStrategyMgr(strategyMgr);
            windowsEventsMonitor = new Windows_EventsMonitor(windowsEventsHandler);
            paintRecThread = new Thread(delegate () { paintRecClear(null); });
        }

        Windows_EventsHandler windowsEventsHandler;
        Windows_EventsMonitor windowsEventsMonitor;

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
            public static extern Int32 SetForegroundWindow(IntPtr hWnd);

            [DllImport("User32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            [DllImport("User32.dll")]
            public static extern bool IsIconic(IntPtr hWnd);
        }

        private CursorPoint cp = new CursorPoint();          

        /// <summary>
        /// Gibt einen CursorPoint (Mauszeiger) an
        /// </summary>
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

        /// <summary>
        /// struktur eines CursorPoints (Mauszeigers)
        /// </summary>
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

        /// <summary>
        /// Gibt die position des Mauszeigers (CursorPostion) an
        /// </summary>
        /// <param name="x">x-Wert des Mauszeigers</param>
        /// <param name="y">y-Wert des Mauszeigers</param>
        public void getCursorPoint(out int x, out int y)
        {
            x = cp.X;
            y = cp.Y;
        }

        /// <summary>
        /// Ermittelt den Handle des Desktops
        /// </summary>
        /// <returns>Handle des Desktops</returns>
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

        /// <summary>
        /// Ermittelt die aktuelle Position der Maus (CursorPostion) und weißt sie <c>cp</c> zu
        /// </summary>
        /// <returns><c>true</c> falls die Position ermittelt wurde; <c>false</c> sonst</returns>
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

        // Fehlerbehandlung wie?
        /// <summary>
        /// Ermittelt den Handle der CursorPostion 
        /// </summary>
        /// <returns>Gib Handle an CursorPostion zurück</returns>
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
        /// <summary>
        /// Ermittelt den Main-Handle vom angegebenen Prozess
        /// </summary>
        /// <param name="processId">gibt die Id des Prozesses an</param>
        /// <returns>Main-Handel des angegebenen Prozesses</returns>
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

        /// <summary>
        /// Ermittelt aus einem <c>OSMElement.OSMElement</c> die zugehörige Position
        /// </summary>
        /// <param name="osmElement">gibt das <c>OSMElement an</c></param>
        /// <returns><c>Rectangle</c> mit der Position des Objektes</returns>
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
        
        /// <summary>
        /// Zeichnet ein Rechteck an der angegebenen Position
        /// </summary>
        /// <param name="rect">gibt die Position des Rechteckes an</param>
        public void paintRect(Rectangle rect)
        {            
            Graphics desktop = Graphics.FromHwnd(NativeMethods.GetDesktopWindow());           
            Graphics newGraphics = desktop;
            Pen redPen = new Pen(Color.Red, 5);
            newGraphics.DrawRectangle(redPen, rect);

            if (!paintRecThread.IsAlive)
            {
                paintRecThread = new Thread(delegate () { paintRecClear(newGraphics); });
                paintRecThread.Start();
            }
        }

        private void paintRecClear(Graphics graphics)
        {
            if(graphics == null) { return; }
            System.Threading.Thread.Sleep(2000);

            NativeMethods.InvalidateRect(IntPtr.Zero, IntPtr.Zero, true);

            //updatewindow säubert nicht ordentlich den screen
            //updateWindow(IntPtr.Zero);
            
            graphics.Dispose();
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

        /// <summary>
        /// Ermittelt den Namen der Anwendung 
        /// </summary>
        /// <param name="name">Prozess Id der Anwendung</param>
        /// <returns>gibt den Modul-Namen der Anwendung zurück</returns>
        public String getModulNameOfApplication(int processId)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                try
                {
                    if (!clsProcess.MainWindowHandle.Equals(IntPtr.Zero) && clsProcess.Id.Equals(processId))
                    {
                        return clsProcess.MainModule.ModuleName;
                    }
                }
                catch (System.ComponentModel.Win32Exception) { }
            }
            return null;
        }

        /// <summary>
        /// Ermittelt Namen inkl. Pfad der gefilterten Anwendung
        /// </summary>
        /// <param name="processId">Prozess Id der Anwendung</param>
        /// <returns>Namen inkl. Pfad der gefilterten Anwendung</returns>
        public String getFileNameOfApplicationByMainWindowTitle(int processId)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                try
                {
                      if (!clsProcess.MainWindowHandle.Equals(IntPtr.Zero) && clsProcess.Id.Equals(processId))
                    {
                        return clsProcess.MainModule.FileName;
                    }
                }
                catch (System.ComponentModel.Win32Exception) { }
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
        /// Startet eine Anwendung
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
                    int i = 20; // nur 20 mal testen ob die Anwendung läuft
                    IntPtr hwnd;
                    do
                    {
                        Thread.Sleep(500);
                        hwnd = isApplicationRunning(p.MainModule.ModuleName);
                        i--;
                    }
                    while ((hwnd == null || hwnd.Equals(IntPtr.Zero)) && i> 0);
                    if (i > 0)
                    {
                        Debug.WriteLine("Geöffnet; hwnd = "+ hwnd+" i= "+ i);
                        Thread.Sleep(500);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (FileNotFoundException) { }
            catch (System.ComponentModel.Win32Exception) { }
            return false;
        }

        /// <summary>
        /// Aktiviert eine Anwendung
        /// -> wird benötigt beim Filtern, falls die Anwendung minimiert ist
        /// </summary>
        /// <param name="hwnd">gibt den Handle der Anwendung an</param>
        /// <returns><c>true</c>, falls die Anwendung aktiviert wurde; sonst <c>false</c></returns>
        public bool showWindow(IntPtr hwnd)
        {
            if (NativeMethods.IsIconic(hwnd))
            {
                return NativeMethods.ShowWindow(hwnd, 9); // https://msdn.microsoft.com/de-de/library/windows/desktop/ms633548(v=vs.85).aspx
            }
            return true;
        }

        /// <summary>
        /// Setzt eine Anwendung in den Vordergrund
        /// </summary>
        /// <param name="hWnd">gibt den Handle der Anwendung an</param>
        public void setForegroundWindow(IntPtr hWnd)
        {
            NativeMethods.SetForegroundWindow(hWnd);
        }


        //todo: diese methode i prism event manager aufrufen in ihr wird festgelgt,
        //welche keyevents abgefragt werden sollen und wie... ohne festlegung, welche keyeventklasse genutzt wird 
        //global oder einer bestimmten app übergabe des hwnd und ob mouse und/oder key
        //in interface nehmen
        public void InitializeWindows_EventsMonitor()
        {
            //Windows_EventsMonitor wem = new Windows_EventsMonitor();
            
            ////wem.Unsubscribe();
            
            //wem.Subscribe();
            ////mouseKeyHookClass mk = new mouseKeyHookClass();
            ////mk.Subscribe();
        }


    }
    
}
