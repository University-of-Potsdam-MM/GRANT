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
using GRANTManager.Interfaces;
using GRANTManager;
using OSMElement;
using BrailleIOGuiElementRenderer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSMElement.UiElements;
using System.Security.Cryptography;

namespace StrategyGenericTree
{
    public class TreeOperations<T> : ITreeOperations<T>
    {
        private StrategyManager strategyMgr;
        private GeneratedGrantTrees grantTrees;
        public StrategyManager getStrategyMgr() { return strategyMgr; }
        public void setStrategyMgr(StrategyManager manager) { strategyMgr = manager; }
        public void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees) { this.grantTrees = grantTrees; }


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
                        //printProperties(data);
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
                    //printProperties(data);
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
        public List<ITreeStrategy<T>> searchProperties(ITreeStrategy<T> tree, OSMElement.GeneralProperties generalProperties, OperatorEnum oper) //TODO: properties sollten generisch sein
        {//TODO: hier fehlen noch viele Eigenschaften
            //TODO: was passiert, wenn T nicht vom Typ GeneralProperties ist?
            if (!(tree is ITreeStrategy<OSMElement.OSMElement>))
            {
                throw new InvalidOperationException("Falscher Baum-Type!");
            }
           // GeneralProperties generalProperties = (GeneralProperties)Convert.ChangeType(properties, typeof(GeneralProperties));
            //printProperties(generalProperties);
            List<INode<OSMElement.OSMElement>> result = new List<INode<OSMElement.OSMElement>>();

            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                Boolean propertieLocalizedControlType = generalProperties.localizedControlTypeFiltered == null || node.Data.properties.localizedControlTypeFiltered.Equals(generalProperties.localizedControlTypeFiltered);
                Boolean propertieName = generalProperties.nameFiltered == null || node.Data.properties.nameFiltered.Equals(generalProperties.nameFiltered);
                Boolean propertieIsEnabled = generalProperties.isEnabledFiltered == null || node.Data.properties.isEnabledFiltered == generalProperties.isEnabledFiltered;
                Boolean propertieBoundingRectangle = generalProperties.boundingRectangleFiltered == new System.Windows.Rect() || node.Data.properties.boundingRectangleFiltered.Equals(generalProperties.boundingRectangleFiltered);
                Boolean propertieIdGenerated = generalProperties.IdGenerated == null || generalProperties.IdGenerated.Equals(node.Data.properties.IdGenerated);
                Boolean propertieAccessKey = generalProperties.accessKeyFiltered == null || generalProperties.accessKeyFiltered.Equals(node.Data.properties.accessKeyFiltered);
                Boolean acceleratorKey = generalProperties.acceleratorKeyFiltered == null || generalProperties.acceleratorKeyFiltered.Equals(node.Data.properties.acceleratorKeyFiltered);
                Boolean runtimeId = generalProperties.runtimeIDFiltered == null || Enumerable.SequenceEqual(generalProperties.runtimeIDFiltered, node.Data.properties.runtimeIDFiltered);
                Boolean automationId = generalProperties.autoamtionIdFiltered == null || generalProperties.autoamtionIdFiltered.Equals(node.Data.properties.autoamtionIdFiltered); //ist zumindest bei Skype für ein UI-Element nicht immer gleich
                Boolean controlType = generalProperties.controlTypeFiltered == null || generalProperties.controlTypeFiltered.Equals(node.Data.properties.controlTypeFiltered);
                if (OperatorEnum.Equals(oper, OperatorEnum.and))
                {
                    if (propertieBoundingRectangle && propertieLocalizedControlType &&  propertieIdGenerated && propertieAccessKey && acceleratorKey  &&  runtimeId && controlType)
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
            List<ITreeStrategy<T>> result2 = ListINodeToListITreeStrategy(result as List<INode<T>>);
            //printNodeList(result2);
            if (result2.Count == 0)
            {
                Debug.WriteLine("");
            }
            return result2;
        }

        /// <summary>
        /// Ermittelt zu einer View die Id des zugehörigen Knotens
        /// </summary>
        /// <param name="viewName">gibt den Namen der View an</param>
        /// <returns>falls der Knoten gefunden wurde, die generierte Id des Knotens; sonst <code>null</code> </returns>
        public String getIdOfView(String viewName)
        {
            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getBrailleTree();
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
            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getBrailleTree();
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
            //printNodeList(result);
            return result;
        }

        public OSMElement.OSMElement getFilteredTreeOsmElementById(String idGenerated)
        {
            return getAssociatedNodeElement(idGenerated, grantTrees.getFilteredTree());
        }

        public OSMElement.OSMElement getBrailleTreeOsmElementById(String idGenerated)
        {
            return getAssociatedNodeElement(idGenerated, grantTrees.getBrailleTree());
        }


        /// <summary>
        /// Sucht im Baum nach bestimmten Knoten anhand der IdGenerated 
        /// </summary>
        /// <param name="idGenerated">gibt die generierte Id des Knotens an</param>
        /// <param name="tree">gibt den Baum an, in dem gesucht werden soll</param>
        /// <returns>zugehöriger Knoten</returns>
        private OSMElement.OSMElement getAssociatedNodeElement(String idGenerated, ITreeStrategy<OSMElement.OSMElement> tree)
        {
            
            if (!(tree.GetType().BaseType == typeof(NodeTree<OSMElement.OSMElement>)))
            {
                throw new InvalidOperationException("Falscher Baum-Typ");
            }
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                if (node.Data.properties.IdGenerated != null && node.Data.properties.IdGenerated.Equals(idGenerated))
                {
                    return node.Data;
                }
            }
            return new OSMElement.OSMElement();

        }

        /// <summary>
        /// Sucht im Baum nach bestimmten Knoten anhand der IdGenerated 
        /// </summary>
        /// <param name="idGenerated">gibt die generierte Id des Knotens an</param>
        /// <param name="tree">gibt den Baum an, in dem gesucht werden soll</param>
        /// <returns>zugehöriger Knoten</returns>
        private ITreeStrategy<OSMElement.OSMElement> getAssociatedNode(String idGenerated, ITreeStrategy<OSMElement.OSMElement> tree)
        {

            if (!(tree.GetType().BaseType == typeof(NodeTree<OSMElement.OSMElement>)))
            {
                throw new InvalidOperationException("Falscher Baum-Typ");
            }
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                if (node.Data.properties.IdGenerated != null && node.Data.properties.IdGenerated.Equals(idGenerated))
                {
                    return (ITreeStrategy<OSMElement.OSMElement>)node;
                }
            }
            return default(ITreeStrategy<OSMElement.OSMElement>);

        }

        #endregion

        /// <summary>
        /// Ändert von einem Knoten die <code>GeneralProperties</code> ausgehend von der <code>IdGenerated</code>
        /// </summary>
        /// <param name="properties">gibt die neuen <code>GeneralProperties</code> an</param>
        public void changePropertiesOfFilteredNode(GeneralProperties properties)
        {
           /* ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getFilteredTree();
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                if (node.Data.properties.IdGenerated != null && node.Data.properties.IdGenerated.Equals(properties.IdGenerated))
                {
                    OSMElement.OSMElement osm = new OSMElement.OSMElement();
                    osm.brailleRepresentation = node.Data.brailleRepresentation;
                    osm.events = node.Data.events;
                    osm.properties = properties;
                    node.Data = osm;

                    break;
                }
            }*/
            List<ITreeStrategy<OSMElement.OSMElement>> node = strategyMgr.getSpecifiedTreeOperations().searchProperties(grantTrees.getFilteredTree(), properties, OperatorEnum.and);
            if (node.Count == 1)
            {
                OSMElement.OSMElement osm = new OSMElement.OSMElement();
                osm.brailleRepresentation = node[0].Data.brailleRepresentation;
                osm.events = node[0].Data.events;
                properties.IdGenerated = node[0].Data.properties.IdGenerated;
                osm.properties = properties;
                node[0].Data = osm;
                return;
            }
            Debug.WriteLine("TODO");
        }

        /// <summary>
        /// Ändert in einem Baum die Properties; dabei wird die <c>IdGenerated</c> mit aktualisiert
        /// </summary>
        /// <param name="properties">gibt die neuen <code>Generalproperties an</code></param>
        /// <param name="idGeneratedOld">gibt die alte <c>IdGenerated</c> an</param>
        public void changePropertiesOfFilteredNode(GeneralProperties properties, String idGeneratedOld)
        {
          //  ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getFilteredTree();
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)grantTrees.getFilteredTree()).All.Nodes)
            {
                if (node.Data.properties.IdGenerated != null && node.Data.properties.IdGenerated.Equals(idGeneratedOld))
                {
                    OSMElement.OSMElement osm = new OSMElement.OSMElement();
                    osm.brailleRepresentation = node.Data.brailleRepresentation;
                    osm.events = node.Data.events;
                    properties.IdGenerated = idGeneratedOld;
                    osm.properties = properties;
                    node.Data = osm;
                    break;
                }
            }
        }

        /// <summary>
        /// Ändert in der Braille-Darstellung das angegebene Element
        /// </summary>
        /// <param name="element">das geänderte Element für die Braille-Darstellung</param>
        private void changeBrailleRepresentation(ref OSMElement.OSMElement element)
        {
            ITreeStrategy<OSMElement.OSMElement> brailleTree = grantTrees.getBrailleTree();
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
        /// Ändert die Eigenschaften des angegebenen Knotens in StrategyManager.brailleRepresentation --> Momentan wird nur der anzuzeigende Text geändert und ob das Element deaktiviert ist!
        /// </summary>
        /// <param name="element">gibt den zu verändernden Knoten an</param>
        public void updateNodeOfBrailleUi(ref OSMElement.OSMElement element)
        {
            BrailleRepresentation updatedContentBR = element.brailleRepresentation;
            GeneralProperties updatetContentGP = element.properties;
            String updatedText = getTextForView(element);

           // updatedContentBR.text = updatedText;
            updatetContentGP.valueFiltered = updatedText;
            bool? isEnable = isUiElementEnable(element);
            if (isEnable != null)
            {
                updatetContentGP.isEnabledFiltered = (bool)isEnable;
            }
           // updatedContentBR.text = updatedText;

            element.brailleRepresentation = updatedContentBR;
            element.properties = updatetContentGP;
            changeBrailleRepresentation(ref element);//hier ist das Element schon geändert  

        }


        /// <summary>
        /// Ermittelt aufgrund der im StrategyManager angegebenen Beziehungen den anzuzeigenden Text
        /// </summary>
        /// <param name="osmElement">gibt das OSM-Element des anzuzeigenden GUI-Elementes an</param>
        /// <returns>den anzuzeigenden Text</returns>
        private String getTextForView(OSMElement.OSMElement osmElement)
        {
            OsmRelationship<String, String> osmRelationship = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(osmElement.properties.IdGenerated) || r.FilteredTree.Equals(osmElement.properties.IdGenerated)); //TODO: was machen wir hier, wenn wir mehrere Paare bekommen? (FindFirst?)
            if (osmRelationship == null)
            {
                Console.WriteLine("kein passendes objekt gefunden");
                return "";
            }
            OSMElement.OSMElement associatedNode = getFilteredTreeOsmElementById(osmRelationship.FilteredTree);
            //ITreeStrategy<OSMElement.OSMElement> associatedNode = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeElement(osmRelationship.FilteredTree, strategyMgr.getFilteredTree());
            String text = "";
            if (!associatedNode.Equals(new OSMElement.OSMElement()))
            {
                object objectText = OSMElement.Helper.getGeneralPropertieElement(osmElement.brailleRepresentation.fromGuiElement, associatedNode.properties);
                text = (objectText != null ? objectText.ToString() : "");
            }
            return text;
        }

        /// <summary>
        /// Ermittelt aufgrund der im StrategyManager angegebenen Beziehungen, ob das UI-Element aktiviert ist
        /// </summary>
        /// <param name="osmElement">gibt das OSM-Element des anzuzeigenden GUI-Elementes an</param>
        /// <returns><code>true</code> fals das UI-Element aktiviert ist; sonst <code>false</code> (falls der Wert nicht bestimmt werden kann, wird <code>null</code> zurückgegeben)</returns>
        private bool? isUiElementEnable(OSMElement.OSMElement osmElement)
        {
            OsmRelationship<String, String> osmRelationship = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(osmElement.properties.IdGenerated) || r.FilteredTree.Equals(osmElement.properties.IdGenerated)); //TODO: was machen wir hier, wenn wir mehrere Paare bekommen? (FindFirst?)
            if (osmRelationship == null)
            {
                Console.WriteLine("kein passendes objekt gefunden");
                return null;
            }
            OSMElement.OSMElement associatedNode = getFilteredTreeOsmElementById(osmRelationship.FilteredTree);
            //ITreeStrategy<OSMElement.OSMElement> associatedNode = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeElement(osmRelationship.FilteredTree, strategyMgr.getFilteredTree());
            bool? isEnable = null;
            if (!associatedNode.Equals(new OSMElement.OSMElement()))
            {
                object objectEnable = OSMElement.Helper.getGeneralPropertieElement("isEnabledFiltered", associatedNode.properties);
                isEnable = (objectEnable != null ? ((bool?)objectEnable) : null);
            }
            return isEnable;
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
        {//TODO: muss an der Stelle die 'IdGenerated' generiert werden oder passiert das vorher?
            if (grantTrees.getBrailleTree() == null)
            {
                grantTrees.setBrailleTree(strategyMgr.getSpecifiedTree().NewNodeTree());
            }
          
            //prüfen, ob der Knoten schon vorhanden ist
            OSMElement.OSMElement nodeToRemove = getBrailleTreeOsmElementById(brailleNode.properties.IdGenerated);
            if (nodeToRemove.Equals(new OSMElement.OSMElement()))
            {
                grantTrees.getBrailleTree().AddChild(brailleNode);
            }
            else
            {
                changeBrailleRepresentation(ref brailleNode);
            }            
        }

        /// <summary>
        /// entfernt einen Knoten vom Baum der Braille-Darstellung
        /// </summary>
        /// <param name="brailleNode">gibt das OSM-element des Knotens der entfernt werden soll an</param>
        public void removeNodeInBrailleTree(OSMElement.OSMElement brailleNode)
        {
            if (grantTrees.getBrailleTree() == null)
            {
                Console.WriteLine("Der Baum ist leer");
                return;
            }

            //prüfen, ob der Knoten vorhanden ist
            OSMElement.OSMElement nodeToRemove = getBrailleTreeOsmElementById(brailleNode.properties.IdGenerated);
            if (nodeToRemove.Equals(new OSMElement.OSMElement()))
            {
                Console.WriteLine("Der Knoten ist nicht vorhanden!");
                return;
            }
            grantTrees.getBrailleTree().Remove(nodeToRemove);
            
        }

        /// <summary>
        /// Aktualisiert den ganzen Baum (nach dem Laden)
        /// Dabei wird erst mit dem Hauptfilter alles gefiltert und anschließend geprüft, bei welchem Knoten der Filter gewechselt werden muss, dann wird dieser Knoten gesucht und neu gefiltert
        /// </summary>
        /// <param name="hwndNew"></param>
        public void updateTree(IntPtr hwndNew)
        {
            ITreeStrategy<OSMElement.OSMElement> treeLoaded = grantTrees.getFilteredTree().Copy();

            ITreeStrategy<OSMElement.OSMElement> treeNew = strategyMgr.getSpecifiedFilter().filtering(hwndNew, TreeScopeEnum.Application, -1);
            grantTrees.setFilteredTree(treeNew);

            if (treeNew.Equals(strategyMgr.getSpecifiedTree().NewNodeTree()) || !treeNew.HasChild) { throw new Exception("Anwendung kann nicht neu gefiltert werden."); }
            FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.getFilteredTree(), grantTrees.getFilterstrategiesOfNodes());
            foreach (FilterstrategyOfNode<String, String, String> nodeStrategy in grantTrees.getFilterstrategiesOfNodes())
            {
                //ersten Knoten ausschließen -> wurde mit der richtigen Strategy gefiltert
                if (!nodeStrategy.IdGenerated.Equals(mainFilterstrategy.IdGenerated))
                {
                    //Filter ändern
                    strategyMgr.setSpecifiedFilter(nodeStrategy.FilterstrategyFullName + ", " + nodeStrategy.FilterstrategyDll);
                    strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                    // Knoten in neuen Baum suchen + filtern und aktualisieren
                    OSMElement.OSMElement foundNewNode = getAssociatedNodeElement(nodeStrategy.IdGenerated, grantTrees.getFilteredTree());
                    if (!foundNewNode.Equals(new OSMElement.OSMElement()))
                    {
                        OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(foundNewNode);
                        Settings settings = new Settings();
                        properties.grantFilterStrategy = settings.filterStrategyTypeToUserName(strategyMgr.getSpecifiedFilter().GetType());                      
                        changePropertiesOfFilteredNode(properties, foundNewNode.properties.IdGenerated);
                    }
                }
            }
            //Filter zurückstellen ändern
            strategyMgr.setSpecifiedFilter(mainFilterstrategy.FilterstrategyFullName + ", " + mainFilterstrategy.FilterstrategyDll);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
        }


        /// <summary>
        /// Ändert einen Teilbaum des gefilterten Baums;
        /// Achtung die Methode sollte nur genutzt werden, wenn von einem Element alle Kindelemente neu gefiltert wurden
        /// </summary>
        /// <param name="subTree">gibt den Teilbaum an</param>
        /// <param name="idOfFirstNode">gibt die Id des esten Knotens des Teilbaumes an</param>
        /// <returns>die Id des Elternknotens des Teilbaumes oder <c>null</c></returns>
        public String changeSubTreeOfFilteredTree(ITreeStrategy<OSMElement.OSMElement> subTree, String idOfFirstNode)
        {
            if (subTree == strategyMgr.getSpecifiedTree().NewNodeTree() || subTree.HasChild == false) { Debug.WriteLine("Keine Elemente im Teilbaum!"); return null; }
            if (grantTrees.getFilteredTree() == null) { Debug.WriteLine("Kein Baum Vorhanden!"); return null; }
            if (idOfFirstNode == null || idOfFirstNode.Equals("")) { Debug.WriteLine("Keine Id des ersten Knotens im Teilbaum vorhanden!"); return null; }
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)grantTrees.getFilteredTree()).All.Nodes)
            {
                if (idOfFirstNode.Equals(node.Data.properties.IdGenerated))
                {                    
                    Debug.WriteLine("Teilbum gefunden");
                    if (node.HasParent)
                    {
                        //unterscheiden, ob der Knoten Geschwister (1.) oder Kinder (2.)  hat
                        if (subTree.Child.HasNext || subTree.Child.HasChild)
                        {
                            /*hat (auch) Geschwister
                             * zum Elternknoten gehen
                             *  - alte Kinder löschen
                             *  - neue Kinder hinzufügen
                             */
                            ITreeStrategy<OSMElement.OSMElement> parentNode = node.Parent;
                            while (parentNode.HasChild)
                            {
                                grantTrees.getFilteredTree().Remove(parentNode.Child.Data);
                            }
                            parentNode.AddChild(subTree);
                            return parentNode.Data.properties.IdGenerated;
                        }
                    }
                }
            }
            Debug.WriteLine("Knoten im Teilbaum nicht gefunden!");
            return null;
        }

        /// <summary>
        /// Generiert für den kompletten Baum die Ids
        /// </summary>
        /// <param name="tree">gibt eine referenz zu dem Baum an</param>
        public void generatedIdsOfTree(ref ITreeStrategy<OSMElement.OSMElement> tree)
        {
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                if (node.Data.properties.IdGenerated == null)
                {
                   OSMElement.OSMElement osm = node.Data;
                   GeneralProperties properties = node.Data.properties;
                   properties.IdGenerated =  generatedId(node);
                   osm.properties = properties;
                   node.Data = osm;
                   if (properties.IdGenerated.Trim().Equals(""))
                   {
                       Debug.WriteLine("");
                   }
                }
            }
        }

        /// <summary>
        /// Ermittelt und setzt die Ids in einem Teilbaum
        /// </summary>
        /// <param name="tree">gibt den Baum inkl. des Teilbaums ohne Ids an</param>
        /// <param name="idOfParent">gibt die Id des ersten Knotens des Teilbaums ohne Ids an</param>
        public void generatedIdsOfSubTree(ref ITreeStrategy<OSMElement.OSMElement> tree, String idOfParent)
        {
            //getFilteredTreeOsmElementById(idOfParent);
            ITreeStrategy<OSMElement.OSMElement> subtree =getAssociatedNode(idOfParent, tree);
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)subtree).All.Nodes)
            {
                if (node.Data.properties.IdGenerated == null)
                {
                    OSMElement.OSMElement osm = node.Data;
                    GeneralProperties properties = node.Data.properties;
                    properties.IdGenerated = generatedId(node);
                    osm.properties = properties;
                    node.Data = osm;
                }
            }
            tree = subtree.Root;
        }

        private static String generatedId(INode<OSMElement.OSMElement> node)
        {
            /* https://blogs.msdn.microsoft.com/csharpfaq/2006/10/09/how-do-i-calculate-a-md5-hash-from-a-string/
             * http://stackoverflow.com/questions/12979212/md5-hash-from-string
             * http://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file
             */
            GeneralProperties properties = node.Data.properties;
            String result = properties.controlTypeFiltered +
                properties.itemTypeFiltered +
                properties.accessKeyFiltered +
                properties.acceleratorKeyFiltered +
                properties.frameWorkIdFiltered +
                properties.isContentElementFiltered +
                properties.isControlElementFiltered +
                properties.isKeyboardFocusableFiltered +
                properties.isPasswordFiltered +
                properties.isRequiredForFormFiltered +
                properties.itemStatusFiltered +
                properties.itemTypeFiltered +
                properties.labeledbyFiltered +
                node.BranchCount +
                node.BranchIndex +
                node.Depth;
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(result));
            }
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }


        /// <summary>
        /// setzt bei allen Element ausgehend von der IdGenerated im Baum die angegebene Filterstrategie
        /// </summary>
        /// <param name="strategyType">gibt die zusetzende Strategie an</param>
        /// <param name="tree">gibt den (kompletten) Baum an</param>
        /// <param name="idOfParent">gibt die Id des Elternknotens, von denen die Kindknoten eine Filterstrategy gesetzt bekommen sollen</param>
        public void setFilterstrategyInPropertiesAndObject(Type strategyType, ref ITreeStrategy<OSMElement.OSMElement> tree, String idOfParent)
        {
            Settings settings = new Settings();
            List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.getFilterstrategiesOfNodes();
            ITreeStrategy<OSMElement.OSMElement> subtree = getAssociatedNode(idOfParent, tree);
            if (!subtree.HasChild) { return; }
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)subtree.Child).All.Nodes)
            {
                FilterstrategyOfNode<String, String, String> mainFilterstrategy =  FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.getFilteredTree(), filterstrategies);
                bool isAdded = FilterstrategiesOfTree.addFilterstrategyOfNode(node.Data.properties.IdGenerated, strategyType, ref filterstrategies);
                if (isAdded)
                {
                    GeneralProperties properties = node.Data.properties;
                    properties.grantFilterStrategy = settings.filterStrategyTypeToUserName(strategyType);
                    strategyMgr.getSpecifiedTreeOperations().changePropertiesOfFilteredNode(properties);
                }
            }
            tree = subtree.Root;
        }

    }
}
