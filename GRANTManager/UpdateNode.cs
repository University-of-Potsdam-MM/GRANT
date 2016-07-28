using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager.Interfaces;

namespace GRANTManager
{
    /// <summary>
    /// Die Klasse Kapselt update-Methode -> ectl. in ein anderes Paket verschieben
    /// </summary>
    public class UpdateNode
    {
        private StrategyManager strategyMgr;

        public UpdateNode(StrategyManager strategyMgr)
        {
            this.strategyMgr = strategyMgr;
        }

        /// <summary>
        /// Aktualisiert einen Knoten des gefilterten Baums;
        /// dabei wird ggf. für den Knoten kurzzeitig die FilterMethode geändert
        /// </summary>
        /// <param name="filteredTreeGeneratedId">gibt die generierte Id des Knotens an</param>
        public void updateNodeOfFilteredTree(String filteredTreeGeneratedId)
        {
            List<ITreeStrategy<OSMElement.OSMElement>> relatedFilteredTreeObject = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeList(filteredTreeGeneratedId, strategyMgr.getFilteredTree()); //TODO: in dem Kontext wollen wir eigentlich nur ein Element zurückbekommen
            foreach (ITreeStrategy<OSMElement.OSMElement> treeElement in relatedFilteredTreeObject)
            {
                Type interfaceOfNode = null;
                //prüfen, ob der Knoten nicht mit dem standard-filter gefiltert werden soll und ggf. Filter kurzzeitig wechseln
                if (treeElement.Data.properties.grantFilterStrategy != null)
                {
                    Type[] interfacesOfTree = (strategyMgr.getFilteredTree().Child.Data.properties.grantFilterStrategy as Type).GetInterfaces();
                    if (interfacesOfTree != null)
                    {
                        interfaceOfNode = (treeElement.Data.properties.grantFilterStrategy as Type).GetInterface(interfacesOfTree[0].Name);
                        if (interfaceOfNode != null)
                        {
                            //TODO: prüfen, ob eine Änderung wirklich notwendig ist
                            //Filter kurzzeitig ändern
                            strategyMgr.setSpecifiedFilter((treeElement.Data.properties.grantFilterStrategy as Type).FullName + ", " + (treeElement.Data.properties.grantFilterStrategy as Type).Namespace); //TODO: methode zum Erhalten des Standard-Filters
                        }
                    }
                }
                //Filtern + Knoten aktualisieren
                OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(treeElement.Data);
                strategyMgr.getSpecifiedTreeOperations().changePropertiesOfFilteredNode(properties);
                if (strategyMgr.getFilteredTree().Child.Data.properties.grantFilterStrategy != null)
                {
                    strategyMgr.setSpecifiedFilter((strategyMgr.getFilteredTree().Child.Data.properties.grantFilterStrategy as Type).FullName + ", " + (strategyMgr.getFilteredTree().Child.Data.properties.grantFilterStrategy as Type).Namespace); //TODO: methode zum Erhalten des Standard-Filters
                }
                if (interfaceOfNode != null)
                {
                    //Filter wieder zurücksetzen
                    strategyMgr.setSpecifiedFilter((strategyMgr.getFilteredTree().Child.Data.properties.grantFilterStrategy as Type).FullName + ", " + (strategyMgr.getFilteredTree().Child.Data.properties.grantFilterStrategy as Type).Namespace); //TODO: methode zum Erhalten des Standard-Filters
                }
            }
        }


    }
}
