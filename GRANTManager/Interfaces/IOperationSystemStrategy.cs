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
        /// <summary>
        /// Ermittelt die aktuelle Position der Maus (CursorPostion) und weißt sie <c>cp</c> zu
        /// </summary>
        /// <returns><c>true</c> falls die Position ermittelt wurde; <c>false</c> sonst</returns>
        bool deliverCursorPosition();

        // Fehlerbehandlung wie?
        /// <summary>
        /// Ermittelt den Handle der CursorPostion 
        /// </summary>
        /// <returns>Gib Handle an CursorPostion zurück</returns>
        IntPtr getHWND();

        // Main WindowHandle vom Prozess
        /// <summary>
        /// Ermittelt den Main-Handle vom angegebenen Prozess
        /// </summary>
        /// <param name="processId">gibt die Id des Prozesses an</param>
        /// <returns>Main-Handel des angegebenen Prozesses</returns>
        IntPtr getProcessHwndFromHwnd(int processId);

        /// <summary>
        /// Zeichnet ein Rechteck an der angegebenen Position
        /// </summary>
        /// <param name="rect">gibt die Position des Rechteckes an</param>
        void paintRect(Rectangle rect);

        /// <summary>
        /// Ermittelt den Handle des Desktops
        /// </summary>
        /// <returns>Handle des Desktops</returns>
        IntPtr deliverDesktopHWND();

        /// <summary>
        /// Gibt die position des Mauszeigers (CursorPostion) an
        /// </summary>
        /// <param name="x">x-Wert des Mauszeigers</param>
        /// <param name="y">y-Wert des Mauszeigers</param>
        void getCursorPoint(out int x, out int y);

        /// <summary>
        /// Ermittelt aus einem <c>OSMElement.OSMElement</c> die zugehörige Position
        /// </summary>
        /// <param name="osmElement">gibt das <c>OSMElement an</c></param>
        /// <returns><c>Rectangle</c> mit der Position des Objektes</returns>
        Rectangle getRect(OSMElement.OSMElement osmElement);

        Boolean isApplicationRunning(String processName);

        IntPtr getHandleOfApplication(string processName);

        /// <summary>
        /// Ermittelt den Namen der Anwendung zurück
        /// </summary>
        /// <param name="name">Prozess Id der Anwendung</param>
        /// <returns>gibt den Modul-Namen der Anwendung zurück</returns>
        String gerProcessNameOfApplication(int processId);

        /// <summary>
        /// Ermittelt Namen inkl. Pfad der gefilterten Anwendung an
        /// </summary>
        /// <param name="processId">Prozess Id der Anwendung</param>
        /// <returns>Namen inkl. Pfad der gefilterten Anwendung</returns>
        String getFileNameOfApplicationByMainWindowTitle(int processId);

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
        /// invoke the application if these was minimised
        /// </summary>
        /// <param name="hwnd">handle to the application</param>
        /// <returns><c>true</c>, if the application invoked; otherwiese<c>false</c></returns>
        bool showWindow(IntPtr hwnd);

        /// <summary>
        /// Hides a window
        /// </summary>
        /// <param name="hwnd">handle of the window which should be hiden</param>
        /// <returns><code>true</code> if the window is hiden, otherwise <code>false</code></returns>
        bool hideWindow(IntPtr hwnd);

        /// <summary>
        /// Setzt eine Anwendung in den Vordergrund
        /// </summary>
        /// <param name="hWnd">gibt den Handle der Anwendung an</param>
        void setForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Returns a handle to the foreground window (the window with which the user is currently working). 
        /// </summary>
        /// <returns>The return value is a handle to the foreground window or <c>null</c>.</returns>
        IntPtr getForegroundWindow();

        void InitializeWindows_EventsMonitor();

        void setStrategyMgr(StrategyManager strategyManager);

    }
}
