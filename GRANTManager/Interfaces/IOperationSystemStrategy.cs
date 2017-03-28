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

        /// <summary>
        /// Ermittelt ob eine Anwendung geöffnet ist
        /// </summary>
        /// <param name="appMainModulName">gibt den MainModulName der gewünschten Anwendung an</param>
        /// <returns> Handle der Anwendung, falls die Anwendung geöffnet ist; sonst <c>IntPtr.Zero</c></returns>
        IntPtr isApplicationRunning(String appName);

        /// <summary>
        /// Ermittelt den Namen der Anwendung zurück
        /// </summary>
        /// <param name="name">Prozess Id der Anwendung</param>
        /// <returns>gibt den Modul-Namen der Anwendung zurück</returns>
        String getModulNameOfApplication(int processId);

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
        /// Aktiviert eine Anwendung
        /// -> wird benötigt beim Filtern, falls die Anwendung minimiert ist
        /// </summary>
        /// <param name="hwnd">gibt den Handle der anwendung an</param>
        /// <returns><c>true</c>, falls die anwendung aktiviert wurde; sonst <c>false</c></returns>
        bool showWindow(IntPtr hwnd);

        /// <summary>
        /// Setzt eine Anwendung in den Vordergrund
        /// </summary>
        /// <param name="hWnd">gibt den Handle der Anwendung an</param>
        void setForegroundWindow(IntPtr hWnd);

        void InitializeWindows_EventsMonitor();

        void setStrategyMgr(StrategyManager strategyManager);

    }
}
