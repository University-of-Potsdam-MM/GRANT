using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

namespace GRANTManager.Interfaces
{
    public interface IOperationSystemStrategy
    {
        bool deliverCursorPosition();
        IntPtr getHWND();
        IntPtr getProcessHwndFromHwnd(int processId);
        //void paintRect(Rectangle rect, Graphics desktop);
        void paintRect(Rectangle rect);
        IntPtr deliverDesktopHWND();
        void getCursorPoint(out int x, out int y);
        Rectangle getRect(OSMElement.OSMElement osmElement);

        /// <summary>
        /// Ermittelt ob eine Anwendung geöffnet ist
        /// </summary>
        /// <param name="appMainModulName">gibt den MainModulName der gewünschten Anwendung an</param>
        /// <returns> Handle der Anwendung, falls die Anwendung geöffnet ist; sonst <c>IntPtr.Zero</c></returns>
        IntPtr isApplicationRunning(String appName);

        /// <summary>
        /// Ermittelt den Namen der Anwendung zurück
        /// </summary>
        /// <param name="name">Titel der Anwendung</param>
        /// <returns></returns>
        String getModulNameOfApplication(String name);

        /// <summary>
        /// Ermittelt Namen inkl. Pfad der gefilterten Anwendung an
        /// </summary>
        /// <param name="name">gibt den Titel der Anwendung an</param>
        /// <returns>Namen inkl. Pfad der gefilterten Anwendung</returns>
        String getFileNameOfApplicationByMainWindowTitle(String name);

        /// <summary>
        /// Ermittelt Namen inkl. Pfad der gefilterten Anwendung an
        /// </summary>
        /// <param name="name">gibt den Modul-Namen der Anwendung an</param>
        /// <returns>Namen inkl. Pfad der gefilterten Anwendung</returns>
        String getFileNameOfApplicationByModulName(String modulName);

        /// <summary>
        /// Startete eine Anwendung
        /// </summary>
        /// <param name="name">Gibt den Namen (inkl. Pfad) der Anwendung an</param>
        /// <returns><c>true</c> falls die Anwendung gestartet wurde; sonst <c>false</c></returns>
        bool openApplication(String name);

        /// <summary>
        /// Aktiviert eine Anwendung
        /// -> wird benötigt beim Filtern, falls die Anwendung minimiert ist
        /// </summary>
        /// <param name="hwnd">gibt den Handle der anwendung an</param>
        /// <returns><c>true</c>, falls die anwendung aktiviert wurde; sonst <c>false</c></returns>
        bool showWindow(IntPtr hwnd);

        void setForegroundWindow(IntPtr hWnd);

        void InitializeWindows_EventsMonitor();

        void setStrategyMgr(StrategyManager strategyManager);

    }
}
