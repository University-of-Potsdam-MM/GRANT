using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OSMElement;

namespace GRANTManager
{   
    /* TODO: Brauchen wir hier auch ein Interface?
     * TODO: Die Klasse sollte woanders hin
     */ 
    public class OsmTreeConnector
    {        
        /// <summary>
        /// Fügt eine Osm-Beziehung zwischen einem Knoten des gefilterten Baumes und dem Baum der Braille-Darstellung hinzu falls diese noch nicht vorhanden ist
        /// </summary>
        /// <param name="idFilteredTree">gibt die 'IdGenerated' des Knotens aus dem gefilterten Baum an</param>
        /// <param name="idBrailleTree">gibt die 'IdGenerated' des Knotens aus dem Baum mit der Braille-Darstellung an</param>
        /// <param name="osmRelationship">gibt eine Referenz zu den (bisherigen) Bezeihungen an</param>
        public static void addOsmConnection(String idFilteredTree, String idBrailleTree, ref List<OsmConnector<String, String>> osmRelationship)
        {   //TODO: evtl. noch prüfen, ob die Ids existieren
            if (idFilteredTree == null || idBrailleTree == null) { Debug.WriteLine("Eine der Ids ist nicht angebene, die OSM-Beziehung wurde nicht erstellt!"); return; }
            //prüfen, ob die Beziehung schon vorhanden ist
            if (!osmRelationship.Exists(r => r.BrailleTree.Equals(idBrailleTree) && r.FilteredTree.Equals(idFilteredTree)))
            {
                OsmConnector<String, String> relationship = new OsmConnector<String, String>();
                relationship.BrailleTree = idBrailleTree;
                relationship.FilteredTree = idFilteredTree;
                osmRelationship.Add(relationship);
            }
        }

        /// <summary>
        /// Setzt eine Osm-Beziehung (die alten werden gelöscht)
        /// </summary>
        /// <param name="idFilteredTree">gibt die 'IdGenerated' des Knotens aus dem gefilterten Baum an</param>
        /// <param name="idBrailleTree">gibt die 'IdGenerated' des Knotens aus dem Baum mit der Braille-Darstellung an</param>
        /// <param name="osmRelationship">gibt eine Referenz zu den (bisherigen) Bezeihungen an</param>
        public static void setOsmConnection(String idFilteredTree, String idBrailleTree, ref List<OsmConnector<String, String>> osmRelationship)
        {
            if (idFilteredTree == null || idBrailleTree == null) { Debug.WriteLine("Eine der Ids ist nicht angebene, die OSM-Beziehung wurde nicht erstellt!"); return; }
                //alte Beziehungen löschen
            osmRelationship.Clear();

                OsmConnector<String, String> relationship = new OsmConnector<String, String>();
                relationship.BrailleTree = idBrailleTree;
                relationship.FilteredTree = idFilteredTree;
                osmRelationship.Add(relationship);
        }

        /// <summary>
        /// Entfernt eine Beziehung zwischen einem Knoten des gefilterten Baumes und dem Baum der Braille-Darstellung 
        /// </summary>
        /// <param name="idFilteredTree">gibt die 'IdGenerated' des Knotens aus dem gefilterten Baum an</param>
        /// <param name="idBrailleTree">gibt die 'IdGenerated' des Knotens aus dem Baum mit der Braille-Darstellung an</param>
        /// <param name="osmRelationship">gibt eine Referenz zu den (bisherigen) Bezeihungen an</param>
        public static void removeOsmConnection(String idFilteredTree, String idBrailleTree, ref List<OsmConnector<String, String>> osmRelationship)
        {
            if (osmRelationship.Exists(r => r.BrailleTree.Equals(idBrailleTree) && r.FilteredTree.Equals(idFilteredTree)))
            {
                OsmConnector<String, String> relationshipToRemove = osmRelationship.Find(r => r.BrailleTree.Equals(idBrailleTree) && r.FilteredTree.Equals(idFilteredTree));
                osmRelationship.Remove(relationshipToRemove);
            }
            else
            {
                Console.WriteLine("Die angegebene Beziehung war nicht vorhanden!");
            }
        }

    }
}
