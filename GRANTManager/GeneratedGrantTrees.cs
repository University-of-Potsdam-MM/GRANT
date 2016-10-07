using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager.Interfaces;
using OSMElement;

namespace GRANTManager
{
    /// <summary>
    /// Diese Klasse enthält die erstellten Bäume und deren Beziehungen
    /// </summary>
    public class GeneratedGrantTrees
    {
        private Object filteredTree; // enthält den gefilterten Baum
        private Object brailleTree; // enthält die Baumdarstellung der UI auf der stiftplatte
        /// <summary>
        /// gibt die Beziehung zwischen <code>filteredTree</code> und <code>brailleTree</code> anhand der generierten Id an
        /// </summary>
        private List<OsmConnector<String, String>> osmRelationship = new List<OsmConnector<string, string>>();

        /// <summary>
        /// Gibt die Filterstrategien an;
        /// dabei wird diese nur für den Root-Knoten und für alle Knoten bei dennen die Strategy von Standardfilter abweicht angegeben
        /// </summary>
        private List<FilterstrategyOfNode<String, String, String>> filterstrategiesOfNodes = new List<FilterstrategyOfNode<string, string, string>>();

        /// <summary>
        /// Setzt den aktuell gefilterten Baum
        /// </summary>
        /// <param name="parentNode">gibt den gefilterten Baum an</param>
        public void setFilteredTree(Object tree)
        {
            filteredTree = tree;
        }

        /// <summary>
        /// Gibt den gefilterten Baum zurück
        /// </summary>
        /// <returns>Ger gefilterte Baum</returns>
        public Object getFilteredTree()
        {
            return filteredTree;
        }

        /// <summary>
        /// Setzt die aktuelle Barille-UI-Darstellung.
        /// </summary>
        /// <param name="parentNode"></param>
        public void setBrailleTree(Object tree)
        {
            brailleTree = tree;
        }

        /// <summary>
        /// Gibt die aktuelle Braille-UI-Darstellung zurück
        /// </summary>
        /// <returns>Braille-UI-Darstellung</returns>
        public Object getBrailleTree()
        {
            return brailleTree;
        }

        /// <summary>
        /// Gibt die Beziehungen zwischen <value>brailleTree</value> und <value>filteredTree</value> an
        /// </summary>
        /// <returns>Beziehungen zwischen <value>brailleTree</value> und <value>filteredTree</value></returns>
        public List<OsmConnector<String, String>> getOsmRelationship()
        {
            return osmRelationship;
        }

        /// <summary>
        /// Setzt die Beziehungen zwischen <value>brailleTree</value> und <value>filteredTree</value>
        /// </summary>
        /// <param name="relationship"></param>
        public void setOsmRelationship(List<OsmConnector<String, String>> relationship)
        {
            osmRelationship = relationship;
        }

        /// <summary>
        /// Gibt die FilterStrategien für die Knoten zurück;
        /// dabei wird diese nur für den Root-Knoten und für alle Knoten bei dennen die Strategy von Standardfilter abweicht angegeben
        /// </summary>
        /// <returns>Liste der Filterstrategien</returns>
        public List<FilterstrategyOfNode<String, String, String>> getFilterstrategiesOfNodes()
        {
            return filterstrategiesOfNodes;
        }

        /// <summary>
        /// Setzt die Filterstrategien für die Knoten
        /// </summary>
        /// <param name="filterstrategiesOfNodes">eine Liste von Filterstrategien</param>
        public void setFilterstrategiesOfNodes(List<FilterstrategyOfNode<String, String, String>> filterstrategiesOfNodes)
        {
            this.filterstrategiesOfNodes = filterstrategiesOfNodes;
        }
    }
}
