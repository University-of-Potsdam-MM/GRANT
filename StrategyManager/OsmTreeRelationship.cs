using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSMElement;

namespace StrategyManager
{   
    /* TODO: Brauchen wir hier auch ein Interface?
     * TODO: Die Klasse sollte woanders hin
     */ 
    public class OsmTreeRelationship
    {        
        /// <summary>
        /// Fügt eine Osm-Beziehung zwischen einem Knoten des gefilterten Baumes und dem Baum der Braille-Darstellung hinzu falls diese noch nicht vorhanden ist
        /// </summary>
        /// <param name="idFilteredTree">gibt die 'IdGenerated' des Knotens aus dem gefilterten Baum an</param>
        /// <param name="idBrailleTree">gibt die 'IdGenerated' des Knotens aus dem Baum mit der Braille-Darstellung an</param>
        /// <param name="osmRelationship">gibt eine Referenz zu den (bisherigen) Bezeihungen an</param>
        public static void addOsmRelationship(String idFilteredTree, String idBrailleTree, ref List<OsmRelationship<String, String>> osmRelationship)
        {   //TODO: evtl. noch prüfen, ob die Ids existieren

            //prüfen, ob die Beziehung schon vorhanden ist
            if (!osmRelationship.Exists(r => r.BrailleTree.Equals(idBrailleTree) && r.FilteredTree.Equals(idFilteredTree)))
            {
                OsmRelationship<String, String> relationship = new OsmRelationship<String, String>();
                relationship.BrailleTree = idBrailleTree;
                relationship.FilteredTree = idFilteredTree;
                osmRelationship.Add(relationship);
            }
        }

        /// <summary>
        /// Entfernt eine Beziehung zwischen einem Knoten des gefilterten Baumes und dem Baum der Braille-Darstellung 
        /// </summary>
        /// <param name="idFilteredTree">gibt die 'IdGenerated' des Knotens aus dem gefilterten Baum an</param>
        /// <param name="idBrailleTree">gibt die 'IdGenerated' des Knotens aus dem Baum mit der Braille-Darstellung an</param>
        /// <param name="osmRelationship">gibt eine Referenz zu den (bisherigen) Bezeihungen an</param>
        public static void removeOsmRelationship(String idFilteredTree, String idBrailleTree, ref List<OsmRelationship<String, String>> osmRelationship)
        {
            if (osmRelationship.Exists(r => r.BrailleTree.Equals(idBrailleTree) && r.FilteredTree.Equals(idFilteredTree)))
            {
                OsmRelationship<String, String> relationshipToRemove = osmRelationship.Find(r => r.BrailleTree.Equals(idBrailleTree) && r.FilteredTree.Equals(idFilteredTree));
                osmRelationship.Remove(relationshipToRemove);
            }
            else
            {
                Console.WriteLine("Die angegebene Beziehung war nicht vorhanden!");
            }
        }
    }
}
