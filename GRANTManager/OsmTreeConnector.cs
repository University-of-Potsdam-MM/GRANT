using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OSMElement;

namespace GRANTManager
{   
    /* 
     * TODO: Die Klasse sollte woanders hin?
     */ 
    public class OsmTreeConnector
    {
        /// <summary>
        /// Adds an OSM connection (filtered Tree <--> braille tree)
        /// </summary>
        /// <param name="idFilteredTree">id of the filtered node</param>
        /// <param name="idBrailleTree">id of the braille node</param>
        /// <param name="osmConnection">(previous) OSM connections</param>
        public static void addOsmConnection(String idFilteredTree, String idBrailleTree, ref List<OsmConnector<String, String>> osmConnection)
        {   //TODO: evtl. noch prüfen, ob die Ids existieren

            if (idFilteredTree == null || idBrailleTree == null) { Debug.WriteLine("One of the ids dosn't exist! The relationship wasn't set."); return; }
            //checks whether the connection already exists
            if (!osmConnection.Exists(r => r.BrailleTree.Equals(idBrailleTree) && r.FilteredTree.Equals(idFilteredTree)))
            {
                OsmConnector<String, String> relationship = new OsmConnector<String, String>();
                relationship.BrailleTree = idBrailleTree;
                relationship.FilteredTree = idFilteredTree;
                osmConnection.Add(relationship);
            }
        }

        /// <summary>
        /// Sets OSM connections and delets the old ones (filtered Tree <--> braille tree)
        /// </summary>
        /// <param name="idFilteredTree">id of the filtered node</param>
        /// <param name="idBrailleTree">id of the braille node</param>
        /// <param name="osmConnection">(previous) OSM connections</param>
        public static void setOsmConnection(String idFilteredTree, String idBrailleTree, ref List<OsmConnector<String, String>> osmConnection)
        {
            if (idFilteredTree == null || idBrailleTree == null) { Debug.WriteLine("One of the ids dosn't exist! The relationship wasn't set."); return; }
            // deletes all old connections
            osmConnection.Clear();

                OsmConnector<String, String> relationship = new OsmConnector<String, String>();
                relationship.BrailleTree = idBrailleTree;
                relationship.FilteredTree = idFilteredTree;
                osmConnection.Add(relationship);
        }

        /// <summary>
        /// Delete an OSM connection
        /// </summary>
        /// <param name="idFilteredTree">id of the filtered node</param>
        /// <param name="idBrailleTree">id of the braille node</param>
        /// <param name="osmConnection">(previous) OSM connections</param>
        public static void removeOsmConnection(String idFilteredTree, String idBrailleTree, ref List<OsmConnector<String, String>> osmConnection)
        {
            if (osmConnection.Exists(r => r.BrailleTree.Equals(idBrailleTree) && r.FilteredTree.Equals(idFilteredTree)))
            {
                OsmConnector<String, String> relationshipToRemove = osmConnection.Find(r => r.BrailleTree.Equals(idBrailleTree) && r.FilteredTree.Equals(idFilteredTree));
                osmConnection.Remove(relationshipToRemove);
            }
            else
            {
                Debug.WriteLine("The connection dosn't exist!");
            }
        }

    }
}
