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
        //OSMElement.OSMElement filterElement(IntPtr hwnd);
        int deliverElementID(IntPtr hwnd);

        /// <summary>
        /// Ermittelt alle direkten Vorgänger des angegbenen Knotens
        /// </summary>
        /// <param name="node">Gibt den Knoten an, von dem die Vorgänger gesucht werden sollen</param>
        /// <param name="hwnd">Gibt den Handle des Knotens an, bis zu welchem die Vorgänger gesucht werden sollen</param>
        /// <returns>Ein Baum mit allen Vorgängern des Knotens</returns>
        ITreeStrategy<OSMElement.OSMElement> getParentsOfElement(ITreeStrategy<OSMElement.OSMElement> node, IntPtr hwnd);

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
