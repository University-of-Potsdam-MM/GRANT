using System;
using GRANTManager;
using OSMElement;
using System.Windows;
using System.Windows.Automation;

namespace GRANTManager.Interfaces
{
    /// <summary>
    /// Das Interface IFilterStrategy enthält die Methoden-Signaturen für die Filterung der Anwendungsdaten. 
    /// </summary>
    public interface IFilterStrategy
    {
        /// <summary>
        /// Filtert ausgehend vom angegebenen Handle die Eigenschaften des GUI-Elementes und all deren Nachfolger
        /// </summary>
        /// <param name="hwnd">gibt den Handle an, von dem die Filterung starten soll</param>
        /// <returns>Ein Baum mit den gefilterten Eigenschaften</returns>
        ITreeStrategy<OSMElement.OSMElement> filtering(IntPtr hwnd);

        /// <summary>
        /// Filtert ausgehend vom angegebenen Punkt (<paramref name="pointX"/>, <paramref name="pointY"/>) unter Berücksichtigung des angegebenen <code>GRANTManager.TreeScopeEnum</code> Baum
        /// </summary>
        /// <param name="pointX">gibt die x-koordinate des zu filternden Elements an</param>
        /// <param name="pointY">gibt die Y-Koordinate des zu filternden Elements an</param>
        /// <param name="treeScope">gibt die 'Art' der Filterung an</param>
        /// <param name="depth">gibt für den <paramref name="treeScope"/> von 'Parent', 'Children' und 'Application' die Tiefe an, <code>-1</code> steht dabei für die 'komplette' Tiefe</param>
        /// <returns>der gefilterte (Teil-)Baum</returns>
        ITreeStrategy<OSMElement.OSMElement> filtering(int pointX, int pointY, GRANTManager.TreeScopeEnum treeScope, int depth);

        //OSMElement.OSMElement filterElement(IntPtr hwnd);
        int deliverElementID(IntPtr hwnd);

		//void getMouseRect(IntPtr hwnd, int pointX, int pointY, out int x, out int y, out int width, out int height);
        OSMElement.OSMElement setOSMElement(int pointX, int pointY);
        
        //AutomationElement ElementFromCursor(int pointsX, int pointsY);

        /// <summary>
        /// Ermittelt aus dem alten <code>OSMElement</code> eines Knotens die aktualisierten Properties
        /// </summary>
        /// <param name="osmElement">gibt das OSM-Element an welches aktualisiert werden soll</param>
        /// <returns>gibt für einen Knoten die aktualisierten Properties zurück</returns>
        GeneralProperties updateNodeContent(OSMElement.OSMElement osmElement);


        /// <summary>
        /// Filtert eine Anwendung/Teilanwendung ausgehend vom hwnd
        /// </summary>
        /// <param name="hwnd">gibt den Handle der zu filternden Anwendung/Element an</param>
        /// <param name="treeScope">gibt die 'Art' der Filterung an</param>
        /// <param name="depth">gibt für den <paramref name="treeScope"/> von 'Parent', 'Children' und 'Application' die Tiefe an, <code>-1</code> steht dabei für die 'komplette' Tiefe</param>
        /// <returns>der gefilterte (Teil-)Baum</returns>
        ITreeStrategy<OSMElement.OSMElement> filtering(IntPtr hwnd, TreeScopeEnum treeScope, int depth);

        void setStrategyMgr(StrategyManager manager);
        void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees);
        StrategyManager getStrategyMgr();
        ITreeStrategy<OSMElement.OSMElement> updateFiltering(OSMElement.OSMElement osmElementOfFirstNodeOfSubtree, TreeScopeEnum treeScopeEnum);
    };
}
