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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSMElement.UiElements;

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
        public List<ITreeStrategy<T>> searchProperties(ITreeStrategy<T> tree, OSMElement.GeneralProperties properties, OperatorEnum oper) //TODO: properties sollten generisch sein
        {//TODO: hier fehlen noch viele Eigenschaften
            //TODO: was passiert, wenn T nicht vom Typ GeneralProperties ist?
            if (!(tree is ITreeStrategy<OSMElement.OSMElement>))
            {
                throw new InvalidOperationException("Falscher Baum-Type!");
            }
            GeneralProperties generalProperties = (GeneralProperties)Convert.ChangeType(properties, typeof(GeneralProperties));
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


                if (OperatorEnum.Equals(oper, OperatorEnum.and))
                {
                    if (propertieBoundingRectangle && propertieIsEnabled && propertieLocalizedControlType && propertieName && propertieIdGenerated & propertieAccessKey)
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
            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getFilteredTree();
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
            }
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
        /// und ggf. die OSM-Beziehungen
        /// Dabei wird erst mit dem Hauptfilter alles gefiltert und anschließend geprüft, bei welchem Knoten der filter gewechselt werden muss, dann wird dieser Knoten gesucht und neu gefiltert
        /// </summary>
        /// <param name="hwndNew"></param>
        public void updateTree(IntPtr hwndNew)
        {
            ITreeStrategy<OSMElement.OSMElement> treeLoaded = grantTrees.getFilteredTree().Copy();

            ITreeStrategy<OSMElement.OSMElement> treeNew = strategyMgr.getSpecifiedFilter().filtering(hwndNew, TreeScopeEnum.Application, -1);
            grantTrees.setFilteredTree(treeNew);

            if (treeNew.Equals(strategyMgr.getSpecifiedTree().NewNodeTree()) || !treeNew.HasChild) { throw new Exception("Anwendung kann nicht neu gefiltert werden."); }
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)treeLoaded).All.Nodes)
            {
                if (!node.Data.Equals(new OSMElement.OSMElement()) && !node.Data.properties.Equals(new GeneralProperties()))
                {
                    #region prüfen, ob für einen Knoten eine andere FilterStrategy eingestellt ist
                    if (node.Data.properties.grantFilterStrategyFullName != null &&
                        !node.Data.properties.grantFilterStrategyFullName.Equals(strategyMgr.getSpecifiedFilter().GetType().FullName) &&
                        !node.Data.properties.grantFilterStrategyNamespace.Equals(strategyMgr.getSpecifiedFilter().GetType().Namespace))
                    {
                        //Filter ändern
                        strategyMgr.setSpecifiedFilter(node.Data.properties.grantFilterStrategyFullName + ", " + node.Data.properties.grantFilterStrategyNamespace);
                        strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                            // Knoten in neuen Baum suchen + filtern und aktualisieren
                        OSMElement.OSMElement foundNewNode = getAssociatedNodeOfOldNode(treeLoaded, (ITreeStrategy<OSMElement.OSMElement>) node);
                        if (!foundNewNode.Equals(new OSMElement.OSMElement()))
                        {
                            OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(foundNewNode);
                            changePropertiesOfFilteredNode(properties, foundNewNode.properties.IdGenerated);
                        }
                       
                        //Filter zurückstellen ändern
                        strategyMgr.setSpecifiedFilter(grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName + ", " + grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyNamespace); //TODO: methode zum Erhalten des Standard-Filters
                        strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                    }
                    #endregion
                }
            }
            //ggf. Filterstrategie wieder zurücksetzen
            if (grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName != null && !grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName.Equals(strategyMgr.getSpecifiedFilter().GetType().FullName))
            {
                //ggf. Filter ändern
                strategyMgr.setSpecifiedFilter(grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName + ", " + grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyNamespace); //TODO: methode zum Erhalten des Standard-Filters
                strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            }
            #region Id in OSM-Beziehung neu setzen
            if (grantTrees.getOsmRelationship() != null)
            {
                List<OsmRelationship<String, String>> relationshipNew = new List<OsmRelationship<string, string>>();
                foreach (OsmRelationship<String, String> relationship in grantTrees.getOsmRelationship())
                {
                    String filteredTreeIdNew = getNewOsmRelationshipOfLoadedTreeId(treeLoaded, relationship);
                    OsmRelationship<String, String> r = new OsmRelationship<string, string>();
                    r.FilteredTree = filteredTreeIdNew;
                    r.BrailleTree = relationship.BrailleTree;
                    if (filteredTreeIdNew != null) { relationshipNew.Add(r); }
                }
                grantTrees.setOsmRelationship(relationshipNew);
            }
            #endregion
        }

        /// <summary>
        /// Sucht im neuen Baum nach dem Angegebenen Knoten des alten Baumes
        /// </summary>
        /// <param name="oldTree">gibt den alten (geladenen) Baum an</param>
        /// <param name="oldNode">gibt des gesuchten Knoten des alten Baums an</param>
        /// <returns>das <c>OSMElement</c> des neuen Baumes von dem zugehörigen Knoten</returns>
        private OSMElement.OSMElement getAssociatedNodeOfOldNode(ITreeStrategy<OSMElement.OSMElement> oldTree, ITreeStrategy<OSMElement.OSMElement> oldNode)
        {
            #region 1. festlegen der Eigenschaften, welche für die Suche berücksichtigt werden sollen
            GeneralProperties searchProperties = new GeneralProperties();
            searchProperties.acceleratorKeyFiltered = oldNode.Data.properties.acceleratorKeyFiltered;
            searchProperties.accessKeyFiltered = oldNode.Data.properties.accessKeyFiltered;
            searchProperties.autoamtionIdFiltered = oldNode.Data.properties.autoamtionIdFiltered;
            searchProperties.classNameFiltered = oldNode.Data.properties.classNameFiltered;
            searchProperties.controlTypeFiltered = oldNode.Data.properties.controlTypeFiltered;
            searchProperties.fileName = oldNode.Data.properties.fileName;
            searchProperties.frameWorkIdFiltered = oldNode.Data.properties.frameWorkIdFiltered;
          //  searchProperties.helpTextFiltered = oldNode.Data.properties.helpTextFiltered;
            searchProperties.isContentElementFiltered = oldNode.Data.properties.isContentElementFiltered;
            searchProperties.isControlElementFiltered = oldNode.Data.properties.isControlElementFiltered;
            //searchProperties.isEnabledFiltered = oldNode.Data.properties.isEnabledFiltered;
            searchProperties.isKeyboardFocusableFiltered = oldNode.Data.properties.isKeyboardFocusableFiltered;
           // searchProperties.isOffscreenFiltered = oldNode.Data.properties.isOffscreenFiltered;
            searchProperties.isPasswordFiltered = oldNode.Data.properties.isPasswordFiltered;
            searchProperties.isRequiredForFormFiltered = oldNode.Data.properties.isRequiredForFormFiltered;
            searchProperties.itemStatusFiltered = oldNode.Data.properties.itemStatusFiltered;
            searchProperties.itemTypeFiltered = oldNode.Data.properties.itemTypeFiltered;
            searchProperties.labeledbyFiltered = oldNode.Data.properties.labeledbyFiltered;
            //searchProperties.localizedControlTypeFiltered = oldNode.Data.properties.localizedControlTypeFiltered;
            searchProperties.moduleName = oldNode.Data.properties.moduleName;
           // searchProperties.suportedPatterns = oldNode.Data.properties.suportedPatterns;
            #endregion

            #region 2. Knoten in neuen Baum suchen
            List<ITreeStrategy<OSMElement.OSMElement>> treeNewAssociatedNodes = strategyMgr.getSpecifiedTreeOperations().searchProperties(grantTrees.getFilteredTree(), searchProperties, OperatorEnum.and);
            List<ITreeStrategy<OSMElement.OSMElement>> treeLoadedAssociatedNodes = strategyMgr.getSpecifiedTreeOperations().searchProperties(oldTree, searchProperties, OperatorEnum.and);
            if (treeNewAssociatedNodes.Count == 1 && treeLoadedAssociatedNodes.Count == 1)
            {
                return treeNewAssociatedNodes[0].Data;
            }
            else
            {
                //prüfen, ob die Tiefe, BranchCount + BranchIndex bei einem stimmen
                foreach (ITreeStrategy<OSMElement.OSMElement> nodeFound in treeNewAssociatedNodes)
                {
                    if (oldNode.BranchCount == nodeFound.BranchCount && oldNode.BranchIndex == nodeFound.BranchIndex && oldNode.Depth == nodeFound.Depth)
                    {
                        // 3.1 Knoten filtern + aktualisieren
                        return nodeFound.Data;
                    }
                }
                    Debug.WriteLine("Es könnte mehrer 'richtige' Knoten geben -> genauer Untersuchen -> TODO");
                    return new OSMElement.OSMElement();
            }
            #endregion
        }

        /// <summary>
        /// Ermittelt die neue id einer Beziehung zwischen den zwei Bäumen (Braille und gefiltert)
        /// </summary>
        /// <param name="oldTree">gibt den alten (geladenen) Baum an</param>
        /// <param name="relationship">gibt die alte Beziehung an</param>
        /// <returns>die Id des zugehörigen Knotens aus dem neuen Baum</returns>
        private String getNewOsmRelationshipOfLoadedTreeId(ITreeStrategy<OSMElement.OSMElement> oldTree, OsmRelationship<String, String> relationship)
        {
            if (relationship.FilteredTree == null) { Debug.WriteLine("keine Beziehung vorhanden!"); return null; }
            ITreeStrategy<OSMElement.OSMElement> associatedNodeOldTree = getAssociatedNode(relationship.FilteredTree, oldTree);
            if (associatedNodeOldTree == null || associatedNodeOldTree.Equals(default(ITreeStrategy<OSMElement.OSMElement>))) { Debug.WriteLine("Kein alten Knoten gefunden!"); return null; }

            OSMElement.OSMElement osmElementOfNewNode = getAssociatedNodeOfOldNode(oldTree, associatedNodeOldTree);
            if (osmElementOfNewNode.Equals(new OSMElement.OSMElement())) { Debug.WriteLine("Kein neuen Knoten gefunden!"); return null; }
            return osmElementOfNewNode.properties.IdGenerated;
        }


        /// <summary>
        /// Ändert einen Teilbaum des gefilterten Baums
        /// Achtung die Methode sollte nur genutzt werden, wenn von einem Element alle Kindelemente neu gefiltert wurden
        /// </summary>
        /// <param name="subTree">gibt den Teilbaum an</param>
        /// <remarks><c>true</c>, falls der Teilbaum geändert wurde; sonst <c> false</c></remarks>
        public bool changeSubTreeOfFilteredTree(ITreeStrategy<OSMElement.OSMElement> subTree)
        {
            if (subTree == strategyMgr.getSpecifiedTree().NewNodeTree() || subTree.HasChild == false) { Debug.WriteLine("Keine Elemente im Teilbaum!"); return false; }
            if (grantTrees.getFilteredTree() == null) { Debug.WriteLine("Kein Baum Vorhanden!"); return false; }
            String idFirstSubtreeNode = subTree.Child.Data.properties.IdGenerated;
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)grantTrees.getFilteredTree()).All.Nodes)
            {
                if (idFirstSubtreeNode.Equals(node.Data.properties.IdGenerated))
                {
                    Debug.WriteLine("Teilbum gefunden");
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
                    }
                    return true;
                }
            }
            Debug.WriteLine("Knoten im Teilbaum nicht gefunden!");
            return false;
        }
    }
}
