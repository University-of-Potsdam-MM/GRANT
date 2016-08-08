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
    /// <summary>
    /// Die Klasse Kapselt update-Methode -> ectl. in ein anderes Paket verschieben
    /// </summary>
    public class UpdateNode
    {
        private StrategyManager strategyMgr;
        private GeneratedGrantTrees grantTrees;

        public UpdateNode(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
        }

        /// <summary>
        /// Aktualisiert einen Knoten des gefilterten Baums;
        /// dabei wird ggf. für den Knoten kurzzeitig die FilterMethode geändert
        /// </summary>
        /// <param name="filteredTreeGeneratedId">gibt die generierte Id des Knotens an</param>
        public void updateNodeOfFilteredTree(String filteredTreeGeneratedId)
        {
            List<ITreeStrategy<OSMElement.OSMElement>> relatedFilteredTreeObject = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeList(filteredTreeGeneratedId, grantTrees.getFilteredTree()); //TODO: in dem Kontext wollen wir eigentlich nur ein Element zurückbekommen
            foreach (ITreeStrategy<OSMElement.OSMElement> treeElement in relatedFilteredTreeObject)
            {
                Type interfaceOfNode = null;
                //prüfen, ob der Knoten nicht mit dem standard-filter gefiltert werden soll und ggf. Filter kurzzeitig wechseln
                if (treeElement.Data.properties.grantFilterStrategyFullName != null && treeElement.Data.properties.grantFilterStrategyNamespace != null)
                {
                    Type filterStrategyTypeTree = getTypeOfStrategy(grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName, grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyNamespace);
                    Type[] interfacesOfTree = filterStrategyTypeTree.GetInterfaces();
                    if (interfacesOfTree != null)
                    {
                        interfaceOfNode = filterStrategyTypeTree.GetInterface(interfacesOfTree[0].Name);
                        if (interfaceOfNode != null)
                        {
                            //TODO: prüfen, ob eine Änderung wirklich notwendig ist
                            //Filter kurzzeitig ändern
                            strategyMgr.setSpecifiedFilter(treeElement.Data.properties.grantFilterStrategyFullName + ", " + treeElement.Data.properties.grantFilterStrategyNamespace); //TODO: methode zum Erhalten des Standard-Filters
                            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                        }
                    }
                }
                //Filtern + Knoten aktualisieren
                OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(treeElement.Data);
                strategyMgr.getSpecifiedTreeOperations().changePropertiesOfFilteredNode(properties);
                if (grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName != null && grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyNamespace != null)
                {
                    strategyMgr.setSpecifiedFilter(grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName + ", " + grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyNamespace); //TODO: methode zum Erhalten des Standard-Filters
                    strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                }
                if (interfaceOfNode != null)
                {
                    //Filter wieder zurücksetzen
                    strategyMgr.setSpecifiedFilter(grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName + ", " + grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyNamespace); //TODO: methode zum Erhalten des Standard-Filters
                    strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                }
            }
        }

        private Type getTypeOfStrategy(String fullName, String ns)
        {
            return Type.GetType(fullName+", "+ns);
        }

        /// <summary>
        /// Vergleicht, ob der gespeicherte Anwendungspfad und der neu ermittelte Pfad übereinstimmen und passt den Pfad ggf. an
        /// </summary>
        public void compareAndChangeFileName()
        {
            if (!grantTrees.getFilteredTree().HasChild || grantTrees.getFilteredTree().Child.Data.properties.Equals(new GeneralProperties())) { return; }
            String fileNameNew = strategyMgr.getSpecifiedOperationSystem().getFileNameOfApplicationByModulName(grantTrees.getFilteredTree().Child.Data.properties.moduleName);
            if (!fileNameNew.Equals(grantTrees.getFilteredTree().Child.Data.properties.fileName))
            {
                Debug.WriteLine("Der Pfad der Anwendung muss amgepasst werden.");
                GeneralProperties properties = grantTrees.getFilteredTree().Child.Data.properties;
                properties.fileName = fileNameNew;
                strategyMgr.getSpecifiedTreeOperations().changePropertiesOfFilteredNode(properties);
            }
        }
    }
}
