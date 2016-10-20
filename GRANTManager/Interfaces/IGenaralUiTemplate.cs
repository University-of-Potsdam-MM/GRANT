﻿using System;
using System.Collections.Generic;
using GRANTManager.TreeOperations;

namespace GRANTManager.Interfaces
{
    public interface IGenaralUiTemplate
    {
        void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees);
        void setTreeOperation(TreeOperation treeOperation);
        void addNavigationbarForScreen(string pathToXml, Object subtree);
        void createUiElementsAllScreens(string pathToXml);
        void createUiElementsNavigationbarScreens(string pathToXml);
        void generatedUiFromTemplate(string pathToXml);
        void updateNavigationbarScreens(string pathToXml);

        void createUiElementFromTemplate(Object filteredSubtree, TempletUiObject templateObject, String brailleNodeId = null);
    }
    public struct TempletUiObject
    {
        public OSMElement.OSMElement osm { get; set; }
        public String groupImplementedClassTypeFullName { get; set; } //nötig?
        public String groupImplementedClassTypeDllName { get; set; } //nötig?
        public List<String> Screens { get; set; } //-> neu, da es nicht mit Screen in OSM (BR) zusammenpasst
        public String name { get; set; }
        public Boolean allElementsOfType { get; set;}
        public OSMElement.UiElements.Orientation orientation { get; set; }

        /// <summary>
        /// enthält Elemente, welche einem (OSM-)Element  als Gruppe zugeordnet sind
        /// die Elemente (Anzahl) dieser Gruppe verändern sich nicht (die inhalte dürfen sich aber ändern)
        /// </summary>
        /// [XmlIgnore]
        public List<OSMElement.OSMElement> groupElementsStatic { get; set; }
         
    }

}
