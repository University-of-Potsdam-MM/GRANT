using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager.Interfaces;
using OSMElement;

namespace GRANTManager
{
    public class Load
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTree;

        public Load(StrategyManager strategyMgr, GeneratedGrantTrees grantTree)
        {
            this.strategyMgr = strategyMgr;
            this.grantTree = grantTree;
        }


        /// <summary>
        /// Lädt eine gefilterten Baum und speichert das ergebnis im <c>GeneratedGrantTrees</c>
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
            Console.WriteLine();
        }

        /// <summary>
        /// Öffnet falls nötig die gefilterte Anwendung
        /// </summary>
        /// <returns><c>true</c> falls die Anwendung (nun) geöffnet ist; sonst <c>false</c></returns>
        public bool openAppOfFilteredTree()
        {
            if (grantTree != null && grantTree.getFilteredTree() != null && grantTree.getFilteredTree().HasChild)
            {
                if (grantTree.getFilteredTree().Data.properties.Equals(new GeneralProperties()) || grantTree.getFilteredTree().Child.Data.properties.moduleName == null) { Console.WriteLine("Kein Daten im 1. Knoten Vorhanden."); return false; }
                IntPtr appIsRunnuing = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(grantTree.getFilteredTree().Child.Data.properties.moduleName);
                Console.WriteLine("App ist gestartet: {0}", appIsRunnuing);
                if (appIsRunnuing.Equals(IntPtr.Zero))
                {
                    if (grantTree.getFilteredTree().Child.Data.properties.fileName != null)
                    {
                        bool openApp = strategyMgr.getSpecifiedOperationSystem().openApplication(grantTree.getFilteredTree().Child.Data.properties.fileName);
                        if (!openApp)
                        {
                            Console.WriteLine("Anwendung konnte nicht geöffnet werden! Ggf. Pfad der Anwendung anpassen."); //TODO
                        }
                        else { return true; }
                    }
                }
            }
            return false;
        }
    }
}
