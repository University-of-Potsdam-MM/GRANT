using System;
using StrategyManager;
using OSMElement;
using System.Windows;
using System.Windows.Automation;

namespace StrategyManager.Interfaces
{
    public interface IFilterStrategy
    {
        /// <summary>
        /// Filtert ausgehend vom angegebenen Handle die Eigenschaften des GUI-Elementes und all deren Nachfolger
        /// </summary>
        /// <param name="hwnd">gibt den Handle an, von dem die Filterung starten soll</param>
        /// <returns>Ein Baum mit den gefilterten Eigenschaften</returns>
        ITreeStrategy<OSMElement.OSMElement> filtering(IntPtr hwnd);

        /// <summary>
        /// Filtert ausgehend vom angegebenen Punkt (<paramref name="pointX"/>, <paramref name="pointY"/>) unter Berücksichtigung des angegebenen <code>StrategyManager.TreeScopeEnum</code> Baum
        /// </summary>
        /// <param name="pointX">gibt die x-koordinate des zu filternden Elements an</param>
        /// <param name="pointY">gibt die Y-Koordinate des zu filternden Elements an</param>
        /// <param name="treeScope">gibt die 'Art' der Filterung an</param>
        /// <param name="depth">gibt für den <paramref name="treeScope"/> von 'Parent', 'Children' und 'Application' die Tiefe an, <code>-1</code> steht dabei für die 'komplette' Tiefe</param>
        /// <returns>der gefilterte (Teil-)Baum</returns>
        ITreeStrategy<OSMElement.OSMElement> filtering(int pointX, int pointY, StrategyManager.TreeScopeEnum treeScope, int depth);

        //OSMElement.OSMElement filterElement(IntPtr hwnd);
        int deliverElementID(IntPtr hwnd);

		//void getMouseRect(IntPtr hwnd, int pointX, int pointY, out int x, out int y, out int width, out int height);
        OSMElement.OSMElement setOSMElement(int pointX, int pointY);
        
        //AutomationElement ElementFromCursor(int pointsX, int pointsY);
        /// <summary>
        /// Filtert von dem angegebenen Knoten die Eigenschaften neu und ändert entsprechend das Baum-Objekt
        /// </summary>
        /// <param name="filteredTreeGeneratedId">gibt die generierte Id des Knotens an, welcher neu gefiltert werden soll</param>
        void updateNodeOfFilteredTree(String filteredTreeGeneratedId);
        void setStrategyMgr(StrategyMgr strategyMgr);
        StrategyMgr getStrategyMgr();    }
}
