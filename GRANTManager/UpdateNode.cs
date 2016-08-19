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
                //prüfen, ob der Knoten nicht mit dem standard-filterStrategies gefiltert werden soll und ggf. Filter kurzzeitig wechseln
                FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.getFilteredTree(), grantTrees.getFilterstrategiesOfNodes());
                FilterstrategyOfNode<String, String, String> nodeFilterstrategy = FilterstrategiesOfTree.getFilterstrategyOfNode(filteredTreeGeneratedId, grantTrees.getFilterstrategiesOfNodes());
                if (nodeFilterstrategy != null)
                {

                            strategyMgr.setSpecifiedFilter(nodeFilterstrategy.FilterstrategyFullName + ", " + nodeFilterstrategy.FilterstrategyDll);
                            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);

                }
                //Filtern + Knoten aktualisieren
                OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(treeElement.Data);
                strategyMgr.getSpecifiedTreeOperations().changePropertiesOfFilteredNode(properties);

                if (nodeFilterstrategy != null)
                {
                    //Filter wieder zurücksetzen
                    strategyMgr.setSpecifiedFilter(mainFilterstrategy.FilterstrategyFullName + ", " + mainFilterstrategy.FilterstrategyDll);
                    strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                }
            }
        }


        /// <summary>
        /// Filtert einen Knoten neu und nimmt dazu die gerade eingestellte Strategy,
        /// dadurch kann der Filter für einen Knoten geändert werden
        /// </summary>
        /// <param name="filteredTreeGeneratedId">gibt die generierte Id des Knotens an</param>
        public void filterNodeWithNewStrategy(String filteredTreeGeneratedId)
        {
            OSMElement.OSMElement relatedFilteredTreeObject = strategyMgr.getSpecifiedTreeOperations().getFilteredTreeOsmElementById(filteredTreeGeneratedId);
            if (relatedFilteredTreeObject.Equals(new OSMElement.OSMElement())) { return; }
                //prüfen, ob der Knoten nicht mit dem standard-filterStrategies gefiltert werden soll und ggf. Filter kurzzeitig wechseln
                FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.getFilteredTree(), grantTrees.getFilterstrategiesOfNodes());
                //Filtern
                OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(relatedFilteredTreeObject);
                
                if(!(mainFilterstrategy.FilterstrategyFullName.Equals(strategyMgr.getSpecifiedFilter().GetType().FullName) && mainFilterstrategy.FilterstrategyDll.Equals(strategyMgr.getSpecifiedFilter().GetType().Namespace)))
                {
                    // Filter für den Knoten merken
                    List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.getFilterstrategiesOfNodes();
                    FilterstrategiesOfTree.addFilterstrategyOfNode(filteredTreeGeneratedId,  strategyMgr.getSpecifiedFilter().GetType(), ref filterstrategies);
                    Settings settings = new Settings();
                    properties.grantFilterStrategy = settings.filterStrategyTypeToUserName(strategyMgr.getSpecifiedFilter().GetType());
                }
                else
                {
                    if (FilterstrategiesOfTree.getFilterstrategyOfNode(filteredTreeGeneratedId, grantTrees.getFilterstrategiesOfNodes()) != null)
                    {
                        //gemerkten Filter für den Knoten löschen -> Standardfilter wird genutzt
                        List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.getFilterstrategiesOfNodes();
                        bool isRemoved = FilterstrategiesOfTree.removeFilterstrategyOfNode(filteredTreeGeneratedId, strategyMgr.getSpecifiedFilter().GetType(), ref filterstrategies);
                        if (isRemoved)
                        {
                            properties.grantFilterStrategy = "";
                        }
                    }
                }
                // Knoten aktualisieren
                strategyMgr.getSpecifiedTreeOperations().changePropertiesOfFilteredNode(properties);
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
                changeFileName(fileNameNew);
            }
        }

        /// <summary>
        /// Ändert den Dateipfad der gefilterten Anwendung
        /// </summary>
        /// <param name="fileNameNew">gibt den neuen Dateipfad an</param>
        public void changeFileName(String fileNameNew)
        {
            if (!grantTrees.getFilteredTree().HasChild || grantTrees.getFilteredTree().Child.Data.properties.Equals(new GeneralProperties())) { return; }
            GeneralProperties properties = grantTrees.getFilteredTree().Child.Data.properties;
            properties.fileName = fileNameNew;
            strategyMgr.getSpecifiedTreeOperations().changePropertiesOfFilteredNode(properties);
        }
    }
}
