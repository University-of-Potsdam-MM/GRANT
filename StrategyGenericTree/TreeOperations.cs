using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Permissions;
using StrategyManager.Interfaces;
using StrategyManager;
using OSMElement;
using BrailleIOGuiElementRenderer;

namespace StrategyGenericTree
{
    public class TreeOperations<T> : ITreeOperations<T>
    {
        private StrategyMgr strategyMgr;
        public StrategyMgr getStrategyMgr() { return strategyMgr; }
        public void setStrategyMgr(StrategyMgr mamager) { strategyMgr = mamager; }


        #region Print
        /// <summary>
        /// Gibt alle Knoten eines Baumes auf der Konsole aus.
        /// </summary>
        /// <param name="tree">gibt den Baum an</param>
        /// <param name="depth">gibt an bis in welche Tiefe die Knoten ausgegeben werden sollen; <code>-1</code> für den ganzen Baum</param>
        public void printTreeElements(ITreeStrategy<T> tree, int depth)
        {
            foreach (ITreeStrategy<T> node in ((ITree<T>)tree).All.Nodes)
            {
                if (node.Depth <= depth || depth == -1)
                {
                    Console.Write("Node -  Anz. Kinder: {0},  Depth: {1},   hasNext: {2}, hasChild: {3}", node.DirectChildCount, node.Depth, node.HasNext, node.HasChild);
                    if (node.HasParent)
                    {
                        if (node.Parent.Data is OSMElement.OSMElement)
                        {
                            OSMElement.OSMElement osmElement = (OSMElement.OSMElement)Convert.ChangeType(node.Parent.Data, typeof(OSMElement.OSMElement));
                            Console.Write(", Parent: {0}", osmElement.properties.nameFiltered);
                        }
                    }
                    //TODO: ist es hier okay, dass wir von GeneralProperties "ausgehen"?
                    if (node.Data is OSMElement.OSMElement)
                    {
                        OSMElement.OSMElement osmElement = (OSMElement.OSMElement)Convert.ChangeType(node.Data, typeof(OSMElement.OSMElement));
                        GeneralProperties data = osmElement.properties;
                        printProperties(data);
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Gibt eine Liste von Knoten auf der Konsole aus.
        /// </summary>
        /// <param name="nodes">gibt die Liste der auszugebenden Knoten an</param>
        private void printNodeList(List<ITreeStrategy<T>> nodes)
        {
            foreach (ITreeStrategy<T> r in nodes)
            {
                OSMElement.OSMElement osmElement1 = (OSMElement.OSMElement)Convert.ChangeType(r.Data, typeof(OSMElement.OSMElement));
                Console.Write("Node - Name: {3}, Depth: {0}, hasNext: {1}, hasChild: {2}", r.Depth, r.HasNext, r.HasChild, osmElement1.properties.nameFiltered);
                if (r.HasParent)
                {
                    if (r.Parent.Data is OSMElement.OSMElement)
                    {
                        OSMElement.OSMElement osmElement = (OSMElement.OSMElement)Convert.ChangeType(r.Parent.Data, typeof(OSMElement.OSMElement));
                        GeneralProperties parentData = osmElement.properties;
                        Console.Write(", Parent: {0}", parentData.nameFiltered);
                    }

                }
                if (r.Data is GeneralProperties)
                {
                    OSMElement.OSMElement osmElement = (OSMElement.OSMElement)Convert.ChangeType(r.Parent.Data, typeof(OSMElement.OSMElement));
                    GeneralProperties data = osmElement.properties;
                    printProperties(data);
                }
                Console.WriteLine();
            }
        }

        #region print GeneralProperties
        /// <summary>
        /// Gibt allr gesetzten Properties auf der Konsole aus.
        /// </summary>
        /// <param name="properties">gibt fie Properies an</param>
        /// TODO: Die Methode könnte eigentlich für alle Baumklassen genutzt werden
        private void printProperties(GeneralProperties properties)
        {
            Console.WriteLine("\nProperties:");
            if (properties.localizedControlTypeFiltered != null)
            {
                Console.WriteLine("localizedControlTypeFiltered: {0}", properties.localizedControlTypeFiltered);
            }
            if (properties.nameFiltered != null)
            {
                Console.WriteLine("nameFiltered: {0}", properties.nameFiltered);
            }
            if (properties.acceleratorKeyFiltered != null)
            {
                Console.WriteLine("acceleratorKeyFiltered: {0}", properties.acceleratorKeyFiltered);
            }
            if (properties.accessKeyFiltered != null)
            {
                Console.WriteLine("accessKeyFiltered: {0}", properties.accessKeyFiltered);
            }
            if (properties.isKeyboardFocusableFiltered != null)
            {
                Console.WriteLine("isKeyboardFocusableFiltered: {0}", properties.isKeyboardFocusableFiltered);
            }
            if (properties.isEnabledFiltered != null)
            {
                Console.WriteLine("isEnabledFiltered: {0}", properties.isEnabledFiltered);
            }
            if (properties.hasKeyboardFocusFiltered != null)
            {
                Console.WriteLine("hasKeyboardFocusFiltered: {0}", properties.hasKeyboardFocusFiltered);
            }
            if (properties.boundingRectangleFiltered != null && properties.boundingRectangleFiltered != new System.Windows.Rect())
            {
                Console.WriteLine("boundingRectangleFiltered: {0}", properties.boundingRectangleFiltered);
            }
            if (properties.isOffscreenFiltered != null)
            {
                Console.WriteLine("isOffscreenFiltered: {0}", properties.isOffscreenFiltered);
            }
            if (properties.helpTextFiltered != null)
            {
                Console.WriteLine("helpTextFiltered: {0}", properties.helpTextFiltered);
            }
            if (properties.IdGenerated != null)
            {
                Console.WriteLine("IdGenerated: {0}", properties.IdGenerated);
            }
            if (properties.autoamtionIdFiltered != null)
            {
                Console.WriteLine("autoamtionIdFiltered: {0}", properties.autoamtionIdFiltered);
            }
            if (properties.controlTypeFiltered != null)
            {
                Console.WriteLine("controlTypeFiltered: {0}", properties.controlTypeFiltered);
            }
            if (properties.frameWorkIdFiltered != null)
            {
                Console.WriteLine("frameWorkIdFiltered: {0}", properties.frameWorkIdFiltered);
            }
            if (properties.hWndFiltered != null)
            {
                Console.WriteLine("hWndFiltered: {0}", properties.hWndFiltered);
            }
            if (properties.isContentElementFiltered != null)
            {
                Console.WriteLine("isContentElementFiltered: {0}", properties.isContentElementFiltered);
            }
            if (properties.labeledbyFiltered != null)
            {
                Console.WriteLine("labeledbyFiltered: {0}", properties.labeledbyFiltered);
            }
            if (properties.isControlElementFiltered != null)
            {
                Console.WriteLine("isControlElementFiltered: {0}", properties.isControlElementFiltered);
            }
            if (properties.isPasswordFiltered != null)
            {
                Console.WriteLine("isPasswordFiltered: {0}", properties.isPasswordFiltered);
            }
            if (properties.processIdFiltered != 0)
            {
                Console.WriteLine("processIdFiltered: {0}", properties.processIdFiltered);
            }
            if (properties.itemTypeFiltered != null)
            {
                Console.WriteLine("itemTypeFiltered: {0}", properties.itemTypeFiltered);
            }
            if (properties.itemStatusFiltered != null)
            {
                Console.WriteLine("itemStatusFiltered: {0}", properties.itemStatusFiltered);
            }
            if (properties.isRequiredForFormFiltered != null)
            {
                Console.WriteLine("isRequiredForFormFiltered: {0}", properties.isRequiredForFormFiltered);
            }
            if (properties.valueFiltered != null)
            {
                Console.WriteLine("valueFiltered: {0}", properties.valueFiltered);
            }
            Console.WriteLine();
        }
        #endregion
        #endregion

        #region search
        ///todo treeKlasse enthält eine Methode namens contains, ist diese in suche nutzbar?  -> Nein, da hier nicht nach einem Element im Baum gesucht werden soll, wo alle Eigenschaften übereinstimmen, sondern nur einige (die anderen haben sich in der zwischenzeit geändert)
        /// <summary>
        /// Sucht anhand der angegebenen Eigenschaften alle Knoten, welche der Bedingung entsprechen (Tiefensuche). Debei werden nur Eigenschften berücksichtigt, welche angegeben wurden.
        /// </summary>
        /// <param name="tree">gibt den Baum in welchem gesucht werden soll an</param>
        /// <param name="properties">gibt alle zu suchenden Eigenschaften an</param>
        /// <param name="oper">gibt an mit welchem Operator (and, or) die Eigenschaften verknüpft werden sollen</param>
        /// <returns>Eine Liste aus <code>ITreeStrategy</code>-Knoten mit den Eigenschaften</returns>
        public List<ITreeStrategy<T>> searchProperties(ITreeStrategy<T> tree, OSMElement.GeneralProperties properties, OperatorEnum oper) //TODO: properties sollten generisch sein
        {//TODO: hier fehlen noch viele Eigenschaften
            //TODO: was passiert, wenn T nicht vom Typ GeneralProperties ist?
            if (!(tree is ITreeStrategy<OSMElement.OSMElement>))
            {
                throw new InvalidOperationException("Falscher Baum-Type!");
            }
            GeneralProperties generalProperties = (GeneralProperties)Convert.ChangeType(properties, typeof(GeneralProperties));
            printProperties(generalProperties);
            List<INode<OSMElement.OSMElement>> result = new List<INode<OSMElement.OSMElement>>();

            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                Boolean propertieLocalizedControlType = generalProperties.localizedControlTypeFiltered == null || node.Data.properties.localizedControlTypeFiltered.Equals(generalProperties.localizedControlTypeFiltered);
                Boolean propertieName = generalProperties.nameFiltered == null || node.Data.properties.nameFiltered.Equals(generalProperties.nameFiltered);
                Boolean propertieIsEnabled = generalProperties.isEnabledFiltered == null || node.Data.properties.isEnabledFiltered == generalProperties.isEnabledFiltered;
                Boolean propertieBoundingRectangle = generalProperties.boundingRectangleFiltered == new System.Windows.Rect() || node.Data.properties.boundingRectangleFiltered.Equals(generalProperties.boundingRectangleFiltered);
                Boolean propertieIdGenerated = generalProperties.IdGenerated == null || generalProperties.IdGenerated.Equals(node.Data.properties.IdGenerated);

                if (OperatorEnum.Equals(oper, OperatorEnum.and))
                {
                    if (propertieBoundingRectangle && propertieIsEnabled && propertieLocalizedControlType && propertieName && propertieIdGenerated)
                    {
                        result.Add(node);
                    }
                }
                if (OperatorEnum.Equals(oper, OperatorEnum.or))
                {
                    if ((generalProperties.localizedControlTypeFiltered != null && propertieLocalizedControlType) ||
                        (generalProperties.nameFiltered != null && propertieName) ||
                        (generalProperties.isEnabledFiltered != null && propertieIsEnabled) ||
                        (generalProperties.boundingRectangleFiltered != new System.Windows.Rect() && propertieBoundingRectangle) ||
                        (generalProperties.IdGenerated != null && propertieIdGenerated)
                        )
                    {
                        result.Add(node);
                    }
                }
            }
            List<ITreeStrategy<T>> result2 = ListINodeToListITreeStrategy(result as List<INode<T>>);
            printNodeList(result2);
            return result2;
        }

        /// <summary>
        /// Ermittelt zu einer View die Id des zugehörigen Knotens
        /// </summary>
        /// <param name="viewName">gibt den Namen der View an</param>
        /// <returns>falls der Knoten gefunden wurde, die generierte Id des Knotens; sonst <code>null</code> </returns>
        public String getIdOfView(String viewName)
        {
            ITreeStrategy<OSMElement.OSMElement> tree = strategyMgr.getBrailleTree();
            if (!(tree.GetType().BaseType == typeof(NodeTree<OSMElement.OSMElement>)))
            {
                throw new InvalidOperationException("Falscher Baum-Typ");
            }
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                if (node.Data.properties.IdGenerated != null && node.Data.brailleRepresentation.viewName.Equals(viewName))
                {
                    return node.Data.properties.IdGenerated;
                }
            }
            return null;
            
        }

        /// <summary>
        /// Ermittelt zu einer View den zugehörigen Knoten
        /// </summary>
        /// <param name="viewName">gibt den Namen der View an</param>
        /// <returns></returns>
        public OSMElement.OSMElement getNodeOfView(String viewName)
        {
            ITreeStrategy<OSMElement.OSMElement> tree = strategyMgr.getBrailleTree();
            if (!(tree.GetType().BaseType == typeof(NodeTree<OSMElement.OSMElement>)))
            {
                throw new InvalidOperationException("Falscher Baum-Typ");
            }
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                if (node.Data.properties.IdGenerated != null && node.Data.brailleRepresentation.viewName.Equals(viewName))
                {
                    return node.Data;
                }
            }
            return default(OSMElement.OSMElement);

        }

        /// <summary>
        /// Sucht im Baum nach bestimmten Knoten anhand der IdGenerated
        /// </summary>
        /// <param name="idGenereted">gibt die generierte Id des Knotens an</param>
        /// <param name="tree">gibt den Baum an, in dem gesucht werden soll</param>
        /// <returns>eine Liste mit allen Knoten, bei denen die Id übereinstimmt</returns>
        public List<ITreeStrategy<T>> getAssociatedNodeList(String idGenereted, ITreeStrategy<T> tree)
        {
            List<ITreeStrategy<T>> result = new List<ITreeStrategy<T>>();
            if (!(tree.GetType().BaseType == typeof(NodeTree<OSMElement.OSMElement>)))
            {
                throw new InvalidOperationException("Falscher Baum-Typ");
            }

            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                Boolean propertieIdGenerated = idGenereted == null || idGenereted.Equals(node.Data.properties.IdGenerated);

                if (propertieIdGenerated)
                {
                    result.Add((ITreeStrategy<T>)node);
                }
            }
            printNodeList(result);
            return result;
        }


        /// <summary>
        /// Sucht im Baum nach bestimmten Knoten anhand der IdGenerated 
        /// </summary>
        /// <param name="idGenerated">gibt die generierte Id des Knotens an</param>
        /// <param name="tree">gibt den Baum an, in dem gesucht werden soll</param>
        /// <returns>zugehöriger Knoten</returns>
        public ITreeStrategy<T> getAssociatedNode(String idGenerated, ITreeStrategy<T> tree)
        {
            if (!(tree.GetType().BaseType == typeof(NodeTree<OSMElement.OSMElement>)))
            {
                throw new InvalidOperationException("Falscher Baum-Typ");
            }
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                if (node.Data.properties.IdGenerated != null && node.Data.properties.IdGenerated.Equals(idGenerated))
                {
                    return (ITreeStrategy<T>)node;
                }
            }
            return default(ITreeStrategy<T>);

        }

        #endregion

        /// <summary>
        /// Ändert von einem Knoten die <code>GeneralProperties</code> ausgehend von der <code>IdGenerated</code>
        /// </summary>
        /// <param name="properties">gibt die neuen <code>Generalproperties an</code></param>
        public void changePropertiesOfFilteredNode(GeneralProperties properties)
        {
            ITreeStrategy<OSMElement.OSMElement> tree = strategyMgr.getFilteredTree();
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                if (node.Data.properties.IdGenerated != null && node.Data.properties.IdGenerated.Equals(properties.IdGenerated))
                {
                    OSMElement.OSMElement osm = new OSMElement.OSMElement();
                    osm.brailleRepresentation = node.Data.brailleRepresentation;
                    osm.events = node.Data.events;
                    osm.interaction = node.Data.interaction;
                    osm.properties = properties;
                    node.Data = osm;

                    break;
                }
            }
            System.Console.WriteLine();
        }

        /// <summary>
        /// Ändert in der Braille-Darstellung das angegebene Element
        /// </summary>
        /// <param name="element">das geänderte Element für die Braille-Darstellung</param>
        private void changeBrailleRepresentation(OSMElement.OSMElement element)
        {
            ITreeStrategy<OSMElement.OSMElement> brailleTree = strategyMgr.getBrailleTree();
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)brailleTree).All.Nodes)
            {
                if (node.Data.properties.IdGenerated != null && node.Data.properties.IdGenerated.Equals(element.properties.IdGenerated))
                {
                    node.Data = element;

                    break;
                }
            }
        }

        /// <summary>
        /// Ändert die Eigenschaften des angegebenen Knotens in StrategyMgr.brailleRepresentation --> Momentan wird nur der anzuzeigende Text geändert und ob das Element deaktiviert ist!
        /// </summary>
        /// <param name="element">gibt den zu verändernden Knoten an</param>
        public void updateNodeOfBrailleUi(OSMElement.OSMElement element)
        {
            Content updatedContent = element.brailleRepresentation.content;
            String updatedText = getTextForView(element);            
            if (element.brailleRepresentation.content.otherContent != null && !typeof(BrailleIOGuiElementRenderer.UiObjectsEnum).Equals(element.brailleRepresentation.content.otherContent.GetType()))
            {//immer wenn 'OtherContent' ein Array und nicht nur den namen des UI-Elementes (als Enum) enthält, muss der Text hierdrinn geupdatet werden
                updatedContent.otherContent = element.brailleRepresentation.content.otherContent;
                IOtherContent otherContent = (element.brailleRepresentation.content.otherContent as object[])[1] as IOtherContent;
                otherContent.text = updatedText;
                bool? isDisable = isUiElementDisable(element);
                if(isDisable != null){
                    otherContent.isDisabled = (bool)isDisable;
                }
            }
            else
            {
                updatedContent.text = updatedText;
            }
            BrailleRepresentation updatedBrailleReprasentation = element.brailleRepresentation;
            updatedBrailleReprasentation.content = updatedContent;
            element.brailleRepresentation = updatedBrailleReprasentation;
            changeBrailleRepresentation(element);//hier ist das Element schon geändert                

        }


        /// <summary>
        /// Ermittelt aufgrund der im StrategyMgr angegebenen Beziehungen den anzuzeigenden Text
        /// </summary>
        /// <param name="osmElement">gibt das OSM-Element des anzuzeigenden GUI-Elementes an</param>
        /// <returns>den anzuzeigenden Text</returns>
        private String getTextForView(OSMElement.OSMElement osmElement)
        {
            OsmRelationship<String, String> osmRelationship = strategyMgr.getOsmRelationship().Find(r => r.BrailleTree.Equals(osmElement.properties.IdGenerated) || r.FilteredTree.Equals(osmElement.properties.IdGenerated)); //TODO: was machen wir hier, wenn wir mehrere Paare bekommen? (FindFirst?)
            if (osmRelationship == null)
            {
                Console.WriteLine("kein passendes objekt gefunden");
                return "";
            }
            ITreeStrategy<OSMElement.OSMElement> associatedNode =  getAssociatedNode(osmRelationship.FilteredTree, strategyMgr.getFilteredTree() as ITreeStrategy<T>) as ITreeStrategy<OSMElement.OSMElement>;
            //ITreeStrategy<OSMElement.OSMElement> associatedNode = strategyMgr.getSpecifiedTreeOperations().getAssociatedNode(osmRelationship.FilteredTree, strategyMgr.getFilteredTree());
            String text = "";
            if (associatedNode != null)
            {
                object objectText = OSMElement.Helper.getGeneralPropertieElement(osmElement.brailleRepresentation.content.fromGuiElement, associatedNode.Data.properties);
                text = (objectText != null ? objectText.ToString() : "");
            }
            return text;
        }

        /// <summary>
        /// Ermittelt aufgrund der im StrategyMgr angegebenen Beziehungen, ob das UI-Element deaktiviert ist
        /// </summary>
        /// <param name="osmElement">gibt das OSM-Element des anzuzeigenden GUI-Elementes an</param>
        /// <returns><code>true</code> fals das UI-Element deaktiviert ist; sonst <code>false</code> (falls der Wert nicht bestimmt werden kann, wird <code>null</code> zurückgegeben)</returns>
        private bool? isUiElementDisable(OSMElement.OSMElement osmElement)
        {
            OsmRelationship<String, String> osmRelationship = strategyMgr.getOsmRelationship().Find(r => r.BrailleTree.Equals(osmElement.properties.IdGenerated) || r.FilteredTree.Equals(osmElement.properties.IdGenerated)); //TODO: was machen wir hier, wenn wir mehrere Paare bekommen? (FindFirst?)
            if (osmRelationship == null)
            {
                Console.WriteLine("kein passendes objekt gefunden");
                return null;
            }
            ITreeStrategy<OSMElement.OSMElement> associatedNode = getAssociatedNode(osmRelationship.FilteredTree, strategyMgr.getFilteredTree() as ITreeStrategy<T>) as ITreeStrategy<OSMElement.OSMElement>;
            //ITreeStrategy<OSMElement.OSMElement> associatedNode = strategyMgr.getSpecifiedTreeOperations().getAssociatedNode(osmRelationship.FilteredTree, strategyMgr.getFilteredTree());
            bool? isDisable = null;
            if (associatedNode != null)
            {
                object objectEnable = OSMElement.Helper.getGeneralPropertieElement("isEnabledFiltered", associatedNode.Data.properties);
                isDisable = (objectEnable != null ? !((bool?)objectEnable) : null);
            }
            return isDisable;
        }

        /// <summary>
        /// Wandelt eine Liste von <code>INode</code> in eine Liste von <code>ITreeStrategy</code> um
        /// </summary>
        /// <param name="list">gibt die <code>INode</code>-Liste an</param>
        /// <returns>eine <code>ITreeStrategy</code>-Liste</returns>
        private List<ITreeStrategy<T>> ListINodeToListITreeStrategy(List<INode<T>> list)
        {
            List<ITreeStrategy<T>> result = new List<ITreeStrategy<T>>();
            foreach (INode<T> node in list)
            {
                result.Add((ITreeStrategy<T>)node);
            }
            return result;
        }

        /// <summary>
        /// Fügt einen Knoten dem Baum der  Braille-Darstellung hinzu;
        /// Falls ein Knoten mit der 'IdGenerated' schon vorhanden sein sollte, wird dieser aktualisiert
        /// </summary>
        /// <param name="brailleNode">gibt die Darstellung des Knotens an</param>
        public void addNodeInBrailleTree(OSMElement.OSMElement brailleNode)
        {//TODO: muss an der Stelle die 'IdGenerated' eneriert werden oder passiert das vorher?
            if (strategyMgr.getBrailleTree() == null)
            {
                strategyMgr.setBrailleTree( strategyMgr.getSpecifiedTree().NewNodeTree());
            }
          
            //prüfen, ob der Knoten schon vorhanden ist
            ITreeStrategy<OSMElement.OSMElement> nodeToRemove = getAssociatedNode(brailleNode.properties.IdGenerated, strategyMgr.getBrailleTree() as ITreeStrategy<T>) as ITreeStrategy<OSMElement.OSMElement>;
            if (nodeToRemove == null || nodeToRemove.Equals(strategyMgr.getSpecifiedTree().NewNodeTree()))
            {
                strategyMgr.getBrailleTree().AddChild(brailleNode);
            }
            else
            {
                changeBrailleRepresentation(brailleNode);
            }            
        }

        /// <summary>
        /// entfernt einen Knoten vom Baum der Braille-Darstellung
        /// </summary>
        /// <param name="brailleNode">gibt das OSM-element des Knotens der entfernt werden soll an</param>
        public void removeNodeInBrailleTree(OSMElement.OSMElement brailleNode)
        {
            if (strategyMgr.getBrailleTree() == null)
            {
                Console.WriteLine("Der Baum ist leer");
                return;
            }

            //prüfen, ob der Knoten vorhanden ist
            ITreeStrategy<OSMElement.OSMElement> nodeToRemove =  getAssociatedNode(brailleNode.properties.IdGenerated, strategyMgr.getBrailleTree() as ITreeStrategy<T>) as ITreeStrategy<OSMElement.OSMElement>;
            if (nodeToRemove == null || nodeToRemove.Equals(strategyMgr.getSpecifiedTree().NewNodeTree()))
            {
                Console.WriteLine("Der Knoten ist nicht vorhanden!");
                return;
            }
            strategyMgr.getBrailleTree().Remove(nodeToRemove.Data);
            
        }
    }
}
