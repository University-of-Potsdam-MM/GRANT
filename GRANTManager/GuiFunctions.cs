using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GRANTManager.Interfaces;
using OSMElement;

namespace GRANTManager
{

    public class GuiFunctions
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTree;

        public GuiFunctions(StrategyManager strategyMgr, GeneratedGrantTrees grantTree)
        {
            this.strategyMgr = strategyMgr;
            this.grantTree = grantTree;
        }

        /// <summary>
        /// Speichert den gefilterten Baum aus dem <c>GeneratedGrantTrees</c>
        /// </summary>
        /// <param name="filePath">gibt den Dateipfad + Namen an</param>
        public void saveFilteredTree(String filePath)
        {
            if (grantTree == null || grantTree.getFilteredTree() == null) { Console.WriteLine("Es ist kein gefilterter Baum vorhanden."); }
            System.IO.FileStream fs = System.IO.File.Create(filePath);
            grantTree.getFilteredTree().XmlSerialize(fs);
            fs.Close();
        }

        /// <summary>
        /// Lädt eine gefilterten Baum und speichert das Ergebnis im <c>GeneratedGrantTrees</c>
        /// </summary>
        /// <param name="filePath">gibt den Dateipfad + Name an</param>
        public void loadFilteredTree(String filePath)
        {
            System.IO.FileStream fs = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            ITreeStrategy<OSMElement.OSMElement> loadedTree = strategyMgr.getSpecifiedTree().XmlDeserialize(fs);
            fs.Close();
            //Baum setzen
            grantTree.setFilteredTree(loadedTree);

            //Filter-Strategy setzen
            if (grantTree.getFilteredTree() != null && grantTree.getFilteredTree().HasChild && !grantTree.getFilteredTree().Child.Data.Equals(new OSMElement.OSMElement()) && !grantTree.getFilteredTree().Child.Data.properties.Equals(new GeneralProperties()))
            {
                if (grantTree.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName != null && grantTree.getFilteredTree().Child.Data.properties.grantFilterStrategyNamespace != null)
                {
                    strategyMgr.setSpecifiedFilter(grantTree.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName + ", " + grantTree.getFilteredTree().Child.Data.properties.grantFilterStrategyNamespace);
                }
                else
                {
                    throw new Exception("Keine FilterStrategy im ersten Knoten angegeben");
                }
            }
            else
            {
                throw new Exception("Baum nicht ausreichend spezifiziert!");
            }
            if (openAppOfFilteredTree())
            {
                //filteredLoadedApplication();
            }
        }

        /// <summary>
        /// Öffnet falls nötig die gefilterte Anwendung
        /// </summary>
        /// <returns><c>true</c> falls die Anwendung (nun) geöffnet ist; sonst <c>false</c></returns>
        public bool openAppOfFilteredTree()
        {
            if (grantTree != null && grantTree.getFilteredTree() != null && grantTree.getFilteredTree().HasChild)
            {
                if (grantTree.getFilteredTree().Data.properties.Equals(new GeneralProperties()) || grantTree.getFilteredTree().Child.Data.properties.moduleName == null)
                {
                    Console.WriteLine("Kein Daten im 1. Knoten Vorhanden.");
                   // return false;
                }
                IntPtr appIsRunnuing = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(grantTree.getFilteredTree().Child.Data.properties.moduleName);
                Debug.WriteLine("App ist gestartet: {0}", appIsRunnuing);
                if (appIsRunnuing.Equals(IntPtr.Zero))
                {
                    if (grantTree.getFilteredTree().Child.Data.properties.fileName != null)
                    {
                        bool openApp = strategyMgr.getSpecifiedOperationSystem().openApplication(grantTree.getFilteredTree().Child.Data.properties.fileName);
                        if (!openApp)
                        {
                            Debug.WriteLine("Anwendung konnte nicht geöffnet werden! Ggf. Pfad der Anwendung anpassen."); //TODO
                        }
                        else { return true; }
                    }
                }
            }
            return false;
        }

        public void filteredLoadedApplication()
        {
            //ist nur notwendig, wenn die Anwendung zwischendurch zu war (--> hwnd's vergleichen) oder die Anwendung verschoben wurde (--> Rect's vergleichen)
            ITreeStrategy<OSMElement.OSMElement> loadedTree = grantTree.getFilteredTree();
            IntPtr hwnd = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(loadedTree.Child.Data.properties.moduleName);
            if (hwnd.Equals(IntPtr.Zero)) { throw new Exception("Der HWND der Anwendung konnte nicht gefunden werden!"); }
            OSMElement.OSMElement firstNodeNew = strategyMgr.getSpecifiedFilter().filteringMainNode(hwnd);
            if (firstNodeNew.properties.hWndFiltered.Equals(loadedTree.Child.Data.properties.hWndFiltered))
            {
                Debug.WriteLine("Die Anwendung wurde zwischendurch nicht geschlossen");
                if (firstNodeNew.properties.boundingRectangleFiltered.Equals(loadedTree.Child.Data.properties.boundingRectangleFiltered))
                {
                    Debug.WriteLine("Die Anwendung befindet sich an der selben stelle und muss nicht neu gefiltert werden.");
                }
            }

        }
    }
}
