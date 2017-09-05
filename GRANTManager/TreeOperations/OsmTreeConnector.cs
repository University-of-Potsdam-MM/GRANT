using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OSMElements;

namespace GRANTManager.TreeOperations
{   

    public class OsmTreeConnector
    {
        private GeneratedGrantTrees grantTrees;

        public OsmTreeConnector(GeneratedGrantTrees grantTrees)
        {
            this.grantTrees = grantTrees;
        }

        /// <summary>
        /// Adds an OSM connection (filtered Tree <--> braille tree)
        /// </summary>
        /// <param name="idFilteredTree">id of the filtered node</param>
        /// <param name="idBrailleTree">id of the braille node</param>
        public void addOsmConnection(String idFilteredTree, String idBrailleTree)
        {   //TODO: evtl. noch prüfen, ob die Ids existieren

            if (idFilteredTree == null || idBrailleTree == null) { Debug.WriteLine("One of the ids dosn't exist! The relationship wasn't set."); return; }
            //checks whether the connection already exists
            if (!grantTrees.osmTreeConnections.Exists(r => r.BrailleTreeId.Equals(idBrailleTree) && r.FilteredTreeId.Equals(idFilteredTree)))
            {
                OsmTreeConnectorTuple relationship = new OsmTreeConnectorTuple();
                relationship.BrailleTreeId = idBrailleTree;
                relationship.FilteredTreeId = idFilteredTree;
                grantTrees.osmTreeConnections.Add(relationship);
            }
        }

        /// <summary>
        /// Sets OSM connections and delets the old ones (filtered Tree <--> braille tree)
        /// </summary>
        /// <param name="idFilteredTree">id of the filtered node</param>
        /// <param name="idBrailleTree">id of the braille node</param>
        public void setOsmConnection(String idFilteredTree, String idBrailleTree)
        {
            if (idFilteredTree == null || idBrailleTree == null) { Debug.WriteLine("One of the ids dosn't exist! The relationship wasn't set."); return; }
            // deletes all old connections
            grantTrees.osmTreeConnections.Clear();

                OsmTreeConnectorTuple relationship = new OsmTreeConnectorTuple();
                relationship.BrailleTreeId = idBrailleTree;
                relationship.FilteredTreeId = idFilteredTree;
            grantTrees.osmTreeConnections.Add(relationship);
        }

        /// <summary>
        /// Delete an OSM connection
        /// </summary>
        /// <param name="idFilteredTree">id of the filtered node</param>
        /// <param name="idBrailleTree">id of the braille node</param>
        public void removeOsmConnection(String idFilteredTree, String idBrailleTree)
        {
            if (grantTrees.osmTreeConnections.Exists(r => r.BrailleTreeId.Equals(idBrailleTree) && r.FilteredTreeId.Equals(idFilteredTree)))
            {
                OsmTreeConnectorTuple relationshipToRemove = grantTrees.osmTreeConnections.Find(r => r.BrailleTreeId.Equals(idBrailleTree) && r.FilteredTreeId.Equals(idFilteredTree));
                grantTrees.osmTreeConnections.Remove(relationshipToRemove);
            }
            else
            {
                Debug.WriteLine("The connection dosn't exist!");
            }
        }

        internal void removeOsmConnection(OsmTreeConnectorTuple connectionToDel)
        {
            grantTrees.osmTreeConnections.Remove(connectionToDel);
        }
    }
}
