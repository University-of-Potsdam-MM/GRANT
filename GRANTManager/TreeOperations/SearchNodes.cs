﻿using OSMElement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GRANTManager.TreeOperations
{
    public class SearchNodes
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperation;

        public SearchNodes(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            this.treeOperation = treeOperation;

        }

        ///todo treeKlasse enthält eine Methode namens contains, ist diese in suche nutzbar?  -> Nein, da hier nicht nach einem Element im Baum gesucht werden soll, wo alle Eigenschaften übereinstimmen, sondern nur einige (die anderen haben sich in der zwischenzeit geändert)
        /// <summary>
        /// Sucht anhand der angegebenen Eigenschaften alle Knoten, welche der Bedingung entsprechen (Tiefensuche). Debei werden nur Eigenschften berücksichtigt, welche angegeben wurden.
        /// </summary>
        /// <param name="parentNode">gibt den Baum in welchem gesucht werden soll an</param>
        /// <param name="properties">gibt alle zu suchenden Eigenschaften an</param>
        /// <param name="oper">gibt an mit welchem Operator (and, or) die Eigenschaften verknüpft werden sollen</param>
        /// <returns>Eine Liste aus <code>ITreeStrategy</code>-Knoten mit den Eigenschaften</returns>
        public List<Object> searchProperties(Object tree, OSMElement.GeneralProperties generalProperties, OperatorEnum oper = OperatorEnum.and)
        {//TODO: hier fehlen noch viele Eigenschaften
            List<Object> result = new List<Object>();
            if (tree == null) { return result; }
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                Boolean propertieLocalizedControlType = generalProperties.localizedControlTypeFiltered == null || strategyMgr.getSpecifiedTree().GetData(node).properties.localizedControlTypeFiltered.Equals(generalProperties.localizedControlTypeFiltered);
                Boolean propertieName = generalProperties.nameFiltered == null || strategyMgr.getSpecifiedTree().GetData(node).properties.nameFiltered.Equals(generalProperties.nameFiltered);
                Boolean propertieIsEnabled = generalProperties.isEnabledFiltered == null || strategyMgr.getSpecifiedTree().GetData(node).properties.isEnabledFiltered == generalProperties.isEnabledFiltered;
                Boolean propertieBoundingRectangle = generalProperties.boundingRectangleFiltered == new System.Windows.Rect() || strategyMgr.getSpecifiedTree().GetData(node).properties.boundingRectangleFiltered.Equals(generalProperties.boundingRectangleFiltered);
                Boolean propertieIdGenerated = generalProperties.IdGenerated == null || generalProperties.IdGenerated.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated);
                Boolean propertieAccessKey = generalProperties.accessKeyFiltered == null || generalProperties.accessKeyFiltered.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.accessKeyFiltered);
                Boolean acceleratorKey = generalProperties.acceleratorKeyFiltered == null || generalProperties.acceleratorKeyFiltered.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.acceleratorKeyFiltered);
                Boolean runtimeId = generalProperties.runtimeIDFiltered == null || Enumerable.SequenceEqual(generalProperties.runtimeIDFiltered, strategyMgr.getSpecifiedTree().GetData(node).properties.runtimeIDFiltered);
                Boolean automationId = generalProperties.autoamtionIdFiltered == null || generalProperties.autoamtionIdFiltered.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.autoamtionIdFiltered); //ist zumindest bei Skype für ein UI-Element nicht immer gleich
                Boolean controlType = generalProperties.controlTypeFiltered == null || generalProperties.controlTypeFiltered.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.controlTypeFiltered);
                if (OperatorEnum.Equals(oper, OperatorEnum.and))
                {
                    if (propertieBoundingRectangle && propertieLocalizedControlType && propertieIdGenerated && propertieAccessKey && acceleratorKey && runtimeId && controlType)
                    {
                        result.Add(node);
                    }
                }
                if (OperatorEnum.Equals(oper, OperatorEnum.or))
                {//TODO: ergänzen
                    if ((generalProperties.localizedControlTypeFiltered != null && propertieLocalizedControlType) ||
                        (generalProperties.nameFiltered != null && propertieName) ||
                        (generalProperties.isEnabledFiltered != null && propertieIsEnabled) ||
                        (generalProperties.boundingRectangleFiltered != new System.Windows.Rect() && propertieBoundingRectangle) ||
                        (generalProperties.IdGenerated != null && propertieIdGenerated) ||
                        (generalProperties.accessKeyFiltered != null && propertieAccessKey)
                        )
                    {
                        result.Add(node);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Sucht im Baum nach bestimmten Knoten anhand der IdGenerated
        /// </summary>
        /// <param name="idGenereted">gibt die generierte Id des Knotens an</param>
        /// <param name="parentNode">gibt den Baum an, in dem gesucht werden soll</param>
        /// <returns>eine Liste mit allen Knoten, bei denen die Id übereinstimmt</returns>
        public List<Object> getAssociatedNodeList(String idGenereted, Object tree)
        {
            List<Object> result = new List<Object>();
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                Boolean propertieIdGenerated = idGenereted == null || idGenereted.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated);
                if (propertieIdGenerated)
                {
                    result.Add(node);
                }
            }
            //printNodeList(result);
            return result;
        }

        /// <summary>
        /// Sucht in dem gefilterten Baum der Anwendung nach einem Knoten mit einer speziellen Id
        /// </summary>
        /// <param name="idGenerated">gibt die Id des gesuchten Knotens an</param>
        /// <returns><c>OSMElement.OSMElement</c> mit der gesuchten Id</returns>
        public OSMElement.OSMElement getFilteredTreeOsmElementById(String idGenerated)
        {
            return getAssociatedNodeElement(idGenerated, grantTrees.filteredTree);
        }

        /// <summary>
        /// Sucht in dem Braille Baume nach einem Knoten mit einer speziellen Id
        /// </summary>
        /// <param name="idGenerated">gibt die Id des gesuchten Knotens an</param>
        /// <returns><c>OSMElement.OSMElement</c> mit der gesuchten Id</returns>
        public OSMElement.OSMElement getBrailleTreeOsmElementById(String idGenerated)
        {
            return getAssociatedNodeElement(idGenerated, grantTrees.brailleTree);
        }


        /// <summary>
        /// Sucht im Baum nach bestimmten Knoten anhand der IdGenerated 
        /// </summary>
        /// <param name="idGenerated">gibt die generierte Id des Knotens an</param>
        /// <param name="parentNode">gibt den Baum an, in dem gesucht werden soll</param>
        /// <returns>zugehöriger Knoten</returns>
        internal OSMElement.OSMElement getAssociatedNodeElement(String idGenerated, Object tree)
        {
            if (tree != null)
            {
                foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
                {
                    if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated != null && strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated.Equals(idGenerated))
                    {
                        return strategyMgr.getSpecifiedTree().GetData(node);
                    }
                }
            }
            return new OSMElement.OSMElement();

        }

        /// <summary>
        /// Sucht im Baum nach bestimmten Knoten anhand der IdGenerated 
        /// </summary>
        /// <param name="idGenerated">gibt die generierte Id des Knotens an</param>
        /// <param name="parentNode">gibt den Baum an, in dem gesucht werden soll</param>
        /// <returns>zugehöriger Knoten</returns>
        internal Object getAssociatedNode(String idGenerated, Object tree)
        {
            if (tree == null || idGenerated == null) { return null; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated != null && strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated.Equals(idGenerated))
                {
                    return node;
                }
            }
            return null;

        }

        /// <summary>
        /// Gibt einen Teilbaum zurück, welcher nur die Views eines Screens enthält
        /// </summary>
        /// <param name="screenName">gibt den Namen des Screens an, zu dem der Teilbaum ermittelt werden soll</param>
        /// <returns>Teilbaum des Screens oder <c>null</c></returns>
        public Object getSubtreeOfScreen(String screenName)
        {

            if (screenName == null || screenName.Equals("")) { Debug.WriteLine("Kein Name des Screens angegeben"); return null; }
            Object tree = strategyMgr.getSpecifiedTree().Copy(grantTrees.brailleTree);
            if (grantTrees.brailleTree == null ) { return null; }

            foreach(Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                foreach(Object screenNodes in strategyMgr.getSpecifiedTree().DirectChildrenNodes(vC))
                {
                    if (strategyMgr.getSpecifiedTree().GetData(screenNodes).brailleRepresentation.screenName.Equals(screenName))
                    {
                        return screenNodes;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gibt die Namen der vorhandenen Screens im Braille-Baum an
        /// </summary>
        /// <param name="typeOfView">gibt die typeOfView an</param>
        /// <returns>Eine Liste der Namen der Screens im Braille-Baum, falls <para>typeOfView</para> angegeben ist, werden nur die Screens dieser typeOfView zurückgegeben</returns>
        public List<String> getPosibleScreenNames(String typeOfView = null)
        {
            if (grantTrees == null || grantTrees.brailleTree == null || !strategyMgr.getSpecifiedTree().HasChild(grantTrees.brailleTree)) { return null; }
            List<String> screens = new List<string>();
            foreach(Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                if(typeOfView == null || typeOfView.Equals(strategyMgr.getSpecifiedTree().GetData(vC).brailleRepresentation.typeOfView))
                {
                    foreach(Object screenSubtree in strategyMgr.getSpecifiedTree().DirectChildrenNodes(vC))
                    {
                        screens.Add(strategyMgr.getSpecifiedTree().GetData(screenSubtree).brailleRepresentation.screenName);
                    }
                }
            }
            return screens;
        }

        /// <summary>
        /// Gibt eine Liste der tatsächlich genutzten Ansichten (typeOfView) zurück vgl <see cref="GRANTManager.Settings.getPossibleViewCategories"/>
        /// </summary>
        /// <returns></returns>
        public List<String> getUsedViewCategories()
        {
            if (grantTrees == null || grantTrees.brailleTree == null || !strategyMgr.getSpecifiedTree().HasChild(grantTrees.brailleTree)) { return null; }
            List<String> viewCategories = new List<string>();
            foreach (Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                viewCategories.Add(strategyMgr.getSpecifiedTree().GetData(vC).brailleRepresentation.typeOfView);
            }
            return viewCategories;
        }

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
        public Object getTreeElementOfViewAtPoint(int pointX, int pointY, String groupViewName, int offsetX, int offsetY)
        {
            if (grantTrees == null || groupViewName == null || groupViewName.Equals("")) { return null; }
            String visibleScreen = strategyMgr.getSpecifiedBrailleDisplay().getVisibleScreen();
            Object screenTree = getSubtreeOfScreen(visibleScreen);
            screenTree = getSubtreeOfView(groupViewName, screenTree);
            if (screenTree == null || screenTree.Equals(strategyMgr.getSpecifiedTree().NewTree())) { return null; }
            foreach(Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(screenTree))
            {
                Rect rect = strategyMgr.getSpecifiedTree().GetData(node).properties.boundingRectangleFiltered;
                if (rect.Contains(pointX - offsetX, pointY - offsetY))
                {
                    return node;
                }
            }
            return screenTree;
        }

        private Object getSubtreeOfView(String viewName, Object subtree)
        {
            //TODO. hier sollten auch die direkten Kinder von subtree reichen durchzugehen
            if (subtree == null) { return null; }
            if (strategyMgr.getSpecifiedTree().HasChild(subtree))
            {
                subtree = strategyMgr.getSpecifiedTree().Child(subtree);
                if (strategyMgr.getSpecifiedTree().GetData(subtree).brailleRepresentation.viewName.Equals(viewName))
                {
                    return subtree;
                }
            }
            else { return null; }
            while (strategyMgr.getSpecifiedTree().HasNext(subtree))
            {
                subtree = strategyMgr.getSpecifiedTree().Next(subtree);
                if (strategyMgr.getSpecifiedTree().GetData(subtree).brailleRepresentation.viewName.Equals(viewName))
                {
                    return subtree;
                }
            }
            return null;
        }

        /// <summary>
        /// Ermittelt für jeden Screen-Teilbaum, den Knoten mit der Navigationsleiste
        /// </summary>
        /// <param name="navigationbarSubstring">gibt den Teil-String des Namenes für die view der Navigationsleiste an</param>
        /// <returns></returns>
        public List<Object> getListOfNavigationbars(String navigationbarSubstring = "NavigationBarScreens")
        {
            List<Object> result = new List<Object>();
            // ITreeStrategy<OSMElement.OSMElement> brailleTree = grantTrees.brailleTree;
            if (grantTrees.brailleTree == null) { return result; }
            foreach(Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(grantTrees.brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName != null && strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName.Contains(navigationbarSubstring) && strategyMgr.getSpecifiedTree().HasChild(node))
                {
                    result.Add(node);
                }
            }
            return result;
        }

        /// <summary>
        /// Prüft, ob die Angegebene View für den angegebenen Screen schon existiert
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public bool existViewInScreen(String screenName, String viewName, String typeOfView)
        {
            if (screenName == null || screenName.Equals("") || viewName == null || viewName.Equals("") || typeOfView == null || typeOfView.Equals("")) { return false; }
            OSMElement.OSMElement osmScreen = new OSMElement.OSMElement();
            BrailleRepresentation brailleScreen = new BrailleRepresentation();
            brailleScreen.screenName = screenName;
            brailleScreen.typeOfView = typeOfView;
            osmScreen.brailleRepresentation = brailleScreen;
            GeneralProperties prop = new GeneralProperties();
            prop.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmScreen);
            osmScreen.properties = prop;
            if (!strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, osmScreen)) { return false; }

            if (!strategyMgr.getSpecifiedTree().HasChild(grantTrees.brailleTree)) { return false; }
            foreach(Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(vC).brailleRepresentation.typeOfView.Equals(typeOfView))
                {
                    foreach (Object node in strategyMgr.getSpecifiedTree().DirectChildrenNodes(vC))
                    {
                        OSMElement.OSMElement nodeData = strategyMgr.getSpecifiedTree().GetData(node);
                        if (nodeData.brailleRepresentation.screenName != null && nodeData.brailleRepresentation.screenName.Equals(screenName))
                        {
                            foreach (Object childScreen in strategyMgr.getSpecifiedTree().AllChildrenNodes(node))
                            {
                                OSMElement.OSMElement childScreenData = strategyMgr.getSpecifiedTree().GetData(childScreen);
                                if (childScreenData.brailleRepresentation.viewName != null && childScreenData.brailleRepresentation.viewName.Equals(viewName))
                                {
                                    Debug.WriteLine("Achtung: für den Screen '" + screenName + "' existiert schon eine view mit dem Namen '" + viewName + "'!");
                                    return true;
                                }
                            }
                            return false;
                        }
                    }
                    return false;
                }
            }

            
            return false;
        }

        /// <summary>
        /// Gibt die zugehörigen Ids des Braille-Baumes an
        /// </summary>
        /// <param name="idGeneratedFilteredNode">gibt die Id des Knotens des gefilterten Baumes an</param>
        /// <returns>eine Liste der Ids des Braille-Baumes oder <c>null</c></returns>
        public List<String> getConnectedBrailleTreenodeIds(String idGeneratedFilteredNode)
        {
            List<OsmConnector<String, String>> osmRelationships = grantTrees.osmRelationship.FindAll(r => r.FilteredTree.Equals(idGeneratedFilteredNode));
            if (osmRelationships != null)
            {
                List<String> result = new List<string>();
                foreach(OsmConnector<String, String> r in osmRelationships)
                {
                    result.Add(r.BrailleTree);
                }
                return result;
            }
            else { return null; }
        }

        /// <summary>
        /// Gibt die zugehörige Id des gefilterten Baumes zurück
        /// </summary>
        /// <param name="idGeneratedBrailleNode">gibt die Id des Knotens des Braille-Baumes an</param>
        /// <returns>die id des gefilterten Baumes oder <c>null</c></returns>
        public String getConnectedFilteredTreenodeId(String idGeneratedBrailleNode)
        {
            OsmConnector<String, String> osmRelationship = grantTrees.osmRelationship.Find(r => r.BrailleTree.Equals(idGeneratedBrailleNode));
            if(osmRelationship != null) { return osmRelationship.FilteredTree; }else { return null; }
        }


    }
}
