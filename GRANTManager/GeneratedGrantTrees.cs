﻿using System;
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
        private ITreeStrategy<OSMElement.OSMElement> filteredTree; // enthält den gefilterten Baum
        private ITreeStrategy<OSMElement.OSMElement> brailleTree; // enthält die Baumdarstellung der UI auf der stiftplatte
        /// <summary>
        /// gibt die Beziehung zwischen <code>filteredTree</code> und <code>brailleTree</code> anhand der generierten Id an
        /// </summary>
        private List<OsmRelationship<String, String>> osmRelationship = new List<OsmRelationship<string, string>>();

        /// <summary>
        /// Setzt den aktuell gefilterten Baum
        /// </summary>
        /// <param name="tree">gibt den gefilterten Baum an</param>
        public void setFilteredTree(ITreeStrategy<OSMElement.OSMElement> tree)
        {
            filteredTree = tree;
        }

        /// <summary>
        /// Gibt den gefilterten Baum zurück
        /// </summary>
        /// <returns>Ger gefilterte Baum</returns>
        public ITreeStrategy<OSMElement.OSMElement> getFilteredTree()
        {
            return filteredTree;
        }

        /// <summary>
        /// Setzt die aktuelle Barille-UI-Darstellung.
        /// </summary>
        /// <param name="tree"></param>
        public void setBrailleTree(ITreeStrategy<OSMElement.OSMElement> tree)
        {
            brailleTree = tree;
        }

        /// <summary>
        /// Gibt die aktuelle Braille-UI-Darstellung zurück
        /// </summary>
        /// <returns>Braille-UI-Darstellung</returns>
        public ITreeStrategy<OSMElement.OSMElement> getBrailleTree()
        {
            return brailleTree;
        }

        /// <summary>
        /// Gibt die Beziehungen zwischen <value>brailleTree</value> und <value>filteredTree</value> an
        /// </summary>
        /// <returns>Beziehungen zwischen <value>brailleTree</value> und <value>filteredTree</value></returns>
        public List<OsmRelationship<String, String>> getOsmRelationship()
        {
            return osmRelationship;
        }

        /// <summary>
        /// Setzt die Beziehungen zwischen <value>brailleTree</value> und <value>filteredTree</value>
        /// </summary>
        /// <param name="relationship"></param>
        public void setOsmRelationship(List<OsmRelationship<String, String>> relationship)
        {
            osmRelationship = relationship;
        }
    }
}
