﻿using System;
using System.Collections.Generic;
using System.Drawing;
namespace GRANTManager.Interfaces
{
    public interface ITreeOperations<T>
    {
        /// <summary>
        /// Ändert die Eigenschaften eines Elementes der Braille-UI --> Momentan wird nur der anzuzeigende Text geändert!
        /// </summary>
        /// <param name="element">gibt das zuändernde Element an</param>
        void updateNodeOfBrailleUi(ref OSMElement.OSMElement element);

        /// <summary>
        /// Ändert die Eigenschaften eines Knotens des gefilterten Baumes.
        /// </summary>
        /// <param name="properties">Gibt die neuen Eigenschaften an.</param>
        void changePropertiesOfFilteredNode(OSMElement.GeneralProperties properties);

        /// <summary>
        /// Ändert einen Teilbaum des gefilterten Baums;
        /// Achtung die Methode sollte nur genutzt werden, wenn von einem Element alle Kindelemente neu gefiltert wurden
        /// </summary>
        /// <param name="subTree">gibt den Teilbaum an</param>
        /// <param name="idOfFirstNode">gibt die Id des esten Knotens des Teilbaumes an</param>
        /// <returns>die Id des Elternknotens des Teilbaumes oder <c>null</c></returns>
        String changeSubTreeOfFilteredTree(Object subTree, String idOfFirstNode);

        /// <summary>
        /// Gibt zu der angegebenen generierten Id aus dem angegeben Baum einen zugehörigen Knoten an
        /// </summary>
        /// <param name="generatedId">Gibt die Id an zuder ein zugehöriger Knoten ermittelt werden soll</param>
        /// <param name="parentNode">gibt den Baum an, in welchem ein zugehöriger Knoten ermittelt werden soll</param>
        /// <returns>Gibt einen Knoten, bei denen die generierte Id übereinstimmt zurück</returns>
       // GRANTManager.Interfaces.ITreeStrategy<T> getAssociatedNode(string idGenerated, GRANTManager.Interfaces.ITreeStrategy<T> parentNode);

        OSMElement.OSMElement getFilteredTreeOsmElementById(String idGenerated);
        OSMElement.OSMElement getBrailleTreeOsmElementById(String idGenerated);

        /// <summary>
        /// Gibt zu der angegebenen generierten Id aus dem angegeben Baum alle zugehörigen Knoten an
        /// </summary>
        /// <param name="generatedId">Gibt die Id an zuder die zugehörigen Knoten ermittelt werden sollen</param>
        /// <param name="parentNode">gibt den Baum an, in welchem die zugehörigen Knoten ermittelt werden sollen </param>
        /// <returns>Gibt eine Liste mit den Knoten, bei denen die generierte Id übereinstimmt zurück</returns>
        List<Object> getAssociatedNodeList(string idGenereted, Object tree);

        /// <summary>
        /// Gibt die (einige) <code>GeneralProperties</code> des angegebenen Baumes aus
        /// </summary>
        /// <param name="parentNode">Gibt den auszugebenen Baum an</param>
        /// <param name="depth">Gibt die Tiefe der Ausgabe an; Wenn der gesamte Baum ausgegeben werden soll, so muss <value>-1</value> angegeben werden.</param>
       // void printTreeElements(Object tree, int depth);

        /// <summary>
        /// Sucht anhand der angegebenen <code>GeneralProperties</code> alle Knoten die diesen Eigenschaften entsprechen
        /// </summary>
        /// <param name="parentNode">Gibt den Baum an, in welchem gesucht werden soll</param>
        /// <param name="properties">gibt die zusuchenden Eigenschaften an</param>
        /// <param name="oper">gibt an wie die Eigenschaften verknüpft werden sollen</param>
        /// <returns>Eine Liste mit allen Knoten auf den die Eigenschaften zutreffen</returns>
        List<Object> searchProperties(Object tree, OSMElement.GeneralProperties properties, GRANTManager.OperatorEnum oper);

        /// <summary>
        /// Fügt einen Knoten dem Baum der  Braille-Darstellung hinzu;
        /// Falls ein Knoten mit der 'IdGenerated' schon vorhanden sein sollte, wird dieser aktualisiert
        /// </summary>
        /// <param name="brailleNode">gibt die Darstellung des Knotens an</param>
        /// <param name="parentId">falls diese gesetzt ist, so soll der Knoten als Kindknoten an diesem angehangen werden</param>
        /// <returns> die generierte Id, falls der Knoten hinzugefügt oder geupdatet wurde, sonst <c>null</c></returns>
        String addNodeInBrailleTree(OSMElement.OSMElement brailleNode, String parentId = null);

        /// <summary>
        /// entfernt einen Knoten vom Baum der Braille-Darstellung
        /// </summary>
        /// <param name="brailleNode">gibt das OSM-element des Knotens der entfernt werden soll an</param>
        void removeNodeInBrailleTree(OSMElement.OSMElement brailleNode);

        /// <summary>
        /// Aktualisiert den ganzen Baum (nach dem Laden)
        /// </summary>
        /// <param name="hwndNew"></param>
        void updateFilteredTree(IntPtr hwndNew);

        void generatedIdsOfFilteredTree(ref Object tree);


        /// <summary>
        /// setzt bei allen Element ausgehend von der IdGenerated im Baum die angegebene Filterstrategie
        /// </summary>
        /// <param name="strategyType">gibt die zusetzende Strategie an</param>
        /// <param name="subtree">gibt den Teilbaum an, bei dem die Strategy gesetzt werden soll</param>
        void setFilterstrategyInPropertiesAndObject(Type strategyType, Object subtree);

        /// <summary>
        /// Ermittelt und setzt die Ids in einem Teilbaum
        /// </summary>
        /// <param name="parentNode">gibt den Baum inkl. des Teilbaums ohne Ids an</param>
        /// <param name="idOfParent">gibt die Id des ersten Knotens des Teilbaums ohne Ids an</param>
        void generatedIdsOfFilteredSubtree(ref Object tree, String idOfParent);

                /// <summary>
        /// Gibt einen Teilbaum zurück, welcher nur die Views eines Screens enthält
        /// </summary>
        /// <param name="screenName">gibt den Namen des Screens an, zu dem der Teilbaum ermittelt werden soll</param>
        /// <returns>Teilbaum des Screens oder <c>null</c></returns>
        Object getSubtreeOfScreen(String screenName);

        /// <summary>
        /// Gibt die Namen der vorhandenen Screens im Braille-Baum an
        /// </summary>
        /// <returns>Eine Liste der Namen der Screens im Braille-Baum</returns>
        List<String> getPosibleScreenNames();

        /// <summary>
        /// Erstellt, aktuallisiert alle Gruppen im Braille-Baum
        /// </summary>
        void updateBrailleGroups();

        /// <summary>
        /// Löscht alle Kindelemente und deren OSM-Beziehungen von Gruppen im Braille-Baum
        /// </summary>
        void deleteChildsOfBrailleGroups();

        /// <summary>
        /// Entfernt alle Kinder des Knotens, aber nicht den Knoten selbst
        /// </summary>
        /// <param name="parentSubtree">gibt den Teilbaum an, bei dem alle Kinder entfernt werden sollen</param>
        void removeChildNodeInBrailleTree(Object parentSubtree);

        /// <summary>
        /// Ermittelt den Knoten des BrailleBaums zu einem Punkt
        /// bei Gruppenknoten wird "versucht" das entsprechende Kind zu ermitteln 
        /// </summary>
        /// <param name="pointX">gibt die x-Position des Punktes an</param>
        /// <param name="pointY">gibt die y-Position des Punktes an</param>
        /// <param name="groupViewName">gibt den View-Namen der Gruppen-View an</param>
        /// <param name="offsetX">gibt den x-Offset der Gruppen-View an</param>
        /// <param name="offsetY">ibt den x-Offset der Gruppen-View an</param>
        /// <returns>den knoten, welcher dem Element entspricht, welches auf der Stifftplatte geklickt wurde oder null</returns>
        Object getTreeElementOfViewAtPoint(int pointX, int pointY, String groupViewName, int offsetX, int offsetY);

        void setStrategyMgr(StrategyManager mamager);
        void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees);
        StrategyManager getStrategyMgr();

        void setPropertyForScreen(string screenName, bool isActiv, String navigationbarSubstring = "NavigationBarScreens");

        /// <summary>
        /// Ermittelt für jeden Screen-Teilbaum, den Knoten mit der Navigationsleiste
        /// </summary>
        /// <param name="navigationbarSubstring">gibt den Teil-String des Namenes für die view der Navigationsleiste an</param>
        /// <returns></returns>
        List<Object> getListOfNavigationbars(String navigationbarSubstring = "NavigationBarScreens");
    }
}
