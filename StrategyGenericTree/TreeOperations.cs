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
using System.Windows;
using System.Diagnostics;
using TemplatesUi;

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
        /// <param name="parentNode">gibt den Baum an</param>
        /// <param name="depth">gibt an bis in welche Tiefe die Knoten ausgegeben werden sollen; <code>-1</code> für den ganzen Baum</param>
        public void printTreeElements(ITreeStrategy<T> tree, int depth = -1)
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
                        Console.WriteLine( osmElement.brailleRepresentation.ToString());
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
        /// <param name="parentNode">gibt den Baum in welchem gesucht werden soll an</param>
        /// <param name="properties">gibt alle zu suchenden Eigenschaften an</param>
        /// <param name="oper">gibt an mit welchem Operator (and, or) die Eigenschaften verknüpft werden sollen</param>
        /// <returns>Eine Liste aus <code>ITreeStrategy</code>-Knoten mit den Eigenschaften</returns>
        public List<ITreeStrategy<T>> searchProperties(ITreeStrategy<T> tree, OSMElement.GeneralProperties generalProperties, OperatorEnum oper)
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
        /// Sucht im Baum nach bestimmten Knoten anhand der IdGenerated
        /// </summary>
        /// <param name="idGenereted">gibt die generierte Id des Knotens an</param>
        /// <param name="parentNode">gibt den Baum an, in dem gesucht werden soll</param>
        /// <returns>eine Liste mit allen Knoten, bei denen die Id übereinstimmt</returns>
        public List<ITreeStrategy<T>> getAssociatedNodeList(String idGenereted, ITreeStrategy<T> tree)
        {
            List<ITreeStrategy<T>> result = new List<ITreeStrategy<T>>();
            if (!(tree.GetType().BaseType == typeof(NodeTree<OSMElement.OSMElement>)))
            {
                throw new InvalidOperationException("Falscher Baum-Typ");
            }
           // Console.WriteLine("Gesuchte ID: {0}", idGenereted);
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                Boolean propertieIdGenerated = idGenereted == null || idGenereted.Equals(node.Data.properties.IdGenerated);
                //Console.WriteLine("ID = {0}", node.Data.properties.IdGenerated);
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
        /// <param name="parentNode">gibt den Baum an, in dem gesucht werden soll</param>
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
        /// <param name="parentNode">gibt den Baum an, in dem gesucht werden soll</param>
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
           /* ITreeStrategy<OSMElement.OSMElement> parentNode = grantTrees.getFilteredTree();
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)parentNode).All.Nodes)
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
          //  ITreeStrategy<OSMElement.OSMElement> parentNode = grantTrees.getFilteredTree();
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
            GeneralProperties updatedContentGP = element.properties;
            String updatedText = getTextForView(element);
            if (element.brailleRepresentation.uiElementSpecialContent != null && element.brailleRepresentation.uiElementSpecialContent.GetType().Equals(typeof(OSMElement.UiElements.ListMenuItem)))
            {
                OSMElement.UiElements.ListMenuItem listMenuItem = (ListMenuItem)element.brailleRepresentation.uiElementSpecialContent;
                updatedContentGP.isToggleStateOn = isCheckboxOfAssociatedElementSelected(element);
            }

           // updatedContentBR.text = updatedText;
            updatedContentGP.valueFiltered = updatedText;
            bool? isEnable = isUiElementEnable(element);
            if (isEnable != null)
            {
                updatedContentGP.isEnabledFiltered = (bool)isEnable;
            }
           // updatedContentBR.text = updatedText;

            element.brailleRepresentation = updatedContentBR;
            element.properties = updatedContentGP;
            changeBrailleRepresentation(ref element);//hier ist das Element schon geändert  

        }

        private bool isCheckboxOfAssociatedElementSelected(OSMElement.OSMElement element)
        {
            OsmRelationship<String, String> osmRelationship = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(element.properties.IdGenerated));
            if (osmRelationship == null)
            {
                Console.WriteLine("kein passendes objekt gefunden");
                return false;
            }
            //OSMElement.OSMElement associatedNode = getFilteredTreeOsmElementById(osmRelationship.FilteredTree);

            List<ITreeStrategy<OSMElement.OSMElement>> associatedNodeList = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeList(osmRelationship.FilteredTree, grantTrees.getFilteredTree());
            if (associatedNodeList != null && associatedNodeList.Count == 1 && associatedNodeList[0].HasChild)
            {
                //die Infos ob eine Checkbox ausgewählt ist stehen (zumindest bei meiner Beispielanwendung) im Kindelement
                return associatedNodeList[0].Child.Data.properties.isToggleStateOn != null ? (bool) associatedNodeList[0].Child.Data.properties.isToggleStateOn : false;
            }
            //return associatedNode.properties.isToggleStateOn != null ? (bool)associatedNode.properties.isToggleStateOn : false ;
            return false;
        }


        /// <summary>
        /// Ermittelt aufgrund der im StrategyManager angegebenen Beziehungen den anzuzeigenden Text
        /// </summary>
        /// <param name="filteredSubtree">gibt das OSM-Element des anzuzeigenden GUI-Elementes an</param>
        /// <returns>den anzuzeigenden Text</returns>
        private String getTextForView(OSMElement.OSMElement osmElement)
        {
            OsmRelationship<String, String> osmRelationship = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(osmElement.properties.IdGenerated));
            if (osmRelationship == null)
            {
                Console.WriteLine("kein passendes objekt gefunden");
                return "";
            }
            OSMElement.OSMElement associatedNode = getFilteredTreeOsmElementById(osmRelationship.FilteredTree);
            //ITreeStrategy<OSMElement.OSMElement> associatedNode = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeElement(osmRelationship.FilteredTree, strategyMgr.getFilteredTree());
            String text = "";
            if (!associatedNode.Equals(new OSMElement.OSMElement()) && !osmElement.brailleRepresentation.fromGuiElement.Trim().Equals(""))
            {
                object objectText = OSMElement.Helper.getGeneralPropertieElement(osmElement.brailleRepresentation.fromGuiElement, associatedNode.properties);
                text = (objectText != null ? objectText.ToString() : "");
            }
            return text;
        }

        /// <summary>
        /// Ermittelt aufgrund der im StrategyManager angegebenen Beziehungen, ob das UI-Element aktiviert ist
        /// </summary>
        /// <param name="filteredSubtree">gibt das OSM-Element des anzuzeigenden GUI-Elementes an</param>
        /// <returns><code>true</code> fals das UI-Element aktiviert ist; sonst <code>false</code> (falls der Wert nicht bestimmt werden kann, wird <code>null</code> zurückgegeben)</returns>
        private bool? isUiElementEnable(OSMElement.OSMElement osmElement)
        {
            OsmRelationship<String, String> osmRelationship = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(osmElement.properties.IdGenerated)); 
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
        /// <param name="parentId">falls diese gesetzt ist, so soll der Knoten als Kindknoten an diesem angehangen werden</param>
        /// <returns> die generierte Id, falls der Knoten hinzugefügt oder geupdatet wurde, sonst <c>null</c></returns>
        public String addNodeInBrailleTree(OSMElement.OSMElement brailleNode, String parentId = null)
        {
            if (grantTrees.getBrailleTree() == null)
            {
                grantTrees.setBrailleTree(strategyMgr.getSpecifiedTree().NewNodeTree());
            }
          
            //prüfen, ob der Knoten schon vorhanden ist
            if (brailleNode.properties.IdGenerated != null && !brailleNode.properties.IdGenerated.Equals(""))
            {
                OSMElement.OSMElement nodeToRemove = getBrailleTreeOsmElementById(brailleNode.properties.IdGenerated);
                if (!nodeToRemove.Equals(new OSMElement.OSMElement()))
                {
                    changeBrailleRepresentation(ref brailleNode);
                    return brailleNode.properties.IdGenerated;
                }
            }

            if (brailleNode.brailleRepresentation.screenName == null || brailleNode.brailleRepresentation.screenName.Equals(""))
            {
                Debug.WriteLine("Kein ScreenName angegeben. Es wurde nichts hinzugefügt!");
                return null;
            }
            // prüfen, ob es die View auf dem Screen schon gibt
            if (existViewInScreen(brailleNode.brailleRepresentation.screenName, brailleNode.brailleRepresentation.viewName))
            {
                return null;
            }
            addSubtreeOfScreen(brailleNode.brailleRepresentation.screenName);
            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getBrailleTree();
          //  foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)grantTrees.getBrailleTree()).All.Nodes)
            foreach(INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)grantTrees.getBrailleTree()).DirectChildren.Nodes)
            {   //ermittelt an welchen Screen-Knoten die View angehangen werden soll
                if (!node.Data.brailleRepresentation.Equals(new BrailleRepresentation()) && node.Data.brailleRepresentation.screenName.Equals(brailleNode.brailleRepresentation.screenName))
                {
                    OSMElement.OSMElement brailleNodeWithId = brailleNode;
                    GeneralProperties prop = brailleNode.properties;
                    prop.IdGenerated = generatedIdBrailleNode(brailleNode);
                    brailleNodeWithId.properties = prop;
                    if (parentId == null)
                    {
                        node.AddChild(brailleNodeWithId);
                        return prop.IdGenerated;
                    }
                    else
                    {
                        // es müssen von diesem Screen-Zweig alle Kinder durchsucht werden um die view mit der parentId zu finden
                        if (node.HasChild)
                        {
                            ITreeStrategy<OSMElement.OSMElement>  nodeScreen = node.Child;
                            if (nodeScreen.Data.properties.IdGenerated.Equals(parentId))
                            {
                                nodeScreen.AddChild(brailleNodeWithId);
                                return prop.IdGenerated;
                            }
                            while (nodeScreen.HasNext)
                            {
                                nodeScreen = nodeScreen.Next;
                                if (nodeScreen.Data.properties.IdGenerated.Equals(parentId))
                                {
                                    nodeScreen.AddChild(brailleNodeWithId);
                                    return prop.IdGenerated;
                                }
                            }
                            return null;
                        }
                        return null;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Prüft, ob die Angegebene View für den angegebenen Screen schon existiert
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        private bool existViewInScreen(String screenName, String viewName)
        {
            if (screenName == null || screenName.Equals("") || viewName == null || viewName.Equals("")) { return false; }
            OSMElement.OSMElement osmScreen = new OSMElement.OSMElement();
            BrailleRepresentation brailleScreen = new BrailleRepresentation();
            brailleScreen.screenName = screenName;
            osmScreen.brailleRepresentation = brailleScreen;
            if (!grantTrees.getBrailleTree().Contains(osmScreen)) { return false; }
            ITreeStrategy<OSMElement.OSMElement> treeCopy = grantTrees.getBrailleTree().Copy();
            if (!treeCopy.HasChild) { return false; }
            treeCopy = treeCopy.Child;
            bool hasNext = true;
            do
            {
                if (treeCopy.Data.brailleRepresentation.screenName.Equals(screenName))
                {
                    
                    //TODO: alle Kinder untersuchen
                    if (treeCopy.HasChild)
                    {
                        treeCopy = treeCopy.Child;
                        if (treeCopy.Data.brailleRepresentation.viewName.Equals(viewName)) { Debug.WriteLine("Achtung: für den Screen '" + screenName + "' existiert schon eine view mit dem Namen '" + viewName + "'!"); return true; }
                        while (treeCopy.HasNext)
                        {
                            treeCopy = treeCopy.Next;
                            if (treeCopy.Data.brailleRepresentation.viewName.Equals(viewName)) { Debug.WriteLine("Achtung: für den Screen '" + screenName + "' existiert schon eine view mit dem Namen '" + viewName + "'!"); return true; }
                        }
                    }
                    return false;
                }
                if (treeCopy.HasNext)
                {
                    treeCopy = treeCopy.Next;
                    hasNext = true;
                }
                else
                {
                    hasNext = false;
                }

            } while (hasNext);

            return false;

        }

        /// <summary>
        /// Fügt einen 'Zweig' für den Screen an den Root-Knoten an, falls der Screen im Baum noch nicht existiert
        /// </summary>
        /// <param name="screenName">gibt den Namen des Screens an</param>
        private void addSubtreeOfScreen(String screenName)
        {
            if (screenName == null || screenName.Equals(""))
            {
                Debug.WriteLine("Kein ScreenName angegeben. Es wurde nichts hinzugefügt!");
                return; 
            }
            OSMElement.OSMElement osmScreen = new OSMElement.OSMElement();
            BrailleRepresentation brailleScreen = new BrailleRepresentation();
            brailleScreen.screenName = screenName;
            osmScreen.brailleRepresentation = brailleScreen;
            if (!grantTrees.getBrailleTree().Contains(osmScreen))
            {
                grantTrees.getBrailleTree().AddChild(osmScreen);
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
        /// Entfernt alle Kinder des Knotens, aber nicht den Knoten selbst
        /// </summary>
        /// <param name="parentSubtree">gibt den Teilbaum an, bei dem alle Kinder entfernt werden sollen</param>
        public void removeChildNodeInBrailleTree(ITreeStrategy<OSMElement.OSMElement> parentSubtree)
        {
            if (parentSubtree != null && parentSubtree.HasChild && grantTrees.getBrailleTree() != null)
            {
                List<OSMElement.OSMElement> listToRemove = new List<OSMElement.OSMElement>();
                ITreeStrategy<OSMElement.OSMElement> childeens = parentSubtree.Child;
                //grantTrees.getBrailleTree().Remove(childeens.Data);
                listToRemove.Add(childeens.Data);
                while (childeens.HasNext)
                {
                    childeens = childeens.Next;
                    //grantTrees.getBrailleTree().Remove(childeens.Data);
                    listToRemove.Add(childeens.Data);
                }
                foreach (OSMElement.OSMElement osm in listToRemove)
                {
                    grantTrees.getBrailleTree().Remove(osm);
                }
            }
        }

        /// <summary>
        /// Aktualisiert den ganzen Baum (nach dem Laden)
        /// Dabei wird erst mit dem Hauptfilter alles gefiltert und anschließend geprüft, bei welchem Knoten der Filter gewechselt werden muss, dann wird dieser Knoten gesucht und neu gefiltert
        /// </summary>
        /// <param name="hwndNew"></param>
        public void updateFilteredTree(IntPtr hwndNew)
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
                      //  if (subTree.Child.HasNext || subTree.Child.HasChild)
                        {
                            ITreeStrategy<OSMElement.OSMElement> parentNode = node.Parent;

                            if (node.BranchIndex == 0)
                            {
                                
                                grantTrees.getFilteredTree().Remove(node.Data);
                                parentNode.InsertChild(subTree);
                            }
                            else
                            {
                                if (node.BranchIndex + 1 == node.BranchCount)
                                {
                                    //ITreeStrategy<OSMElement.OSMElement> parentNode = node.Parent;
                                    grantTrees.getFilteredTree().Remove(node.Data);
                                    parentNode.AddChild(subTree);
                                }
                                else
                                {
                                    ITreeStrategy<OSMElement.OSMElement> previousNode = node.Previous;
                                    grantTrees.getFilteredTree().Remove(node.Data);
                                    previousNode.InsertNext(subTree);
                                }
                            }
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
        /// <param name="parentNode">gibt eine referenz zu dem Baum an</param>
        public void generatedIdsOfFilteredTree(ref ITreeStrategy<OSMElement.OSMElement> tree)
        {
            Console.WriteLine("===== DEBUG BEGINN - HASH ======");
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)tree).All.Nodes)
            {
                if (node.Data.properties.IdGenerated == null)
                {
                   OSMElement.OSMElement osm = node.Data;
                   GeneralProperties properties = node.Data.properties;
                   properties.IdGenerated =  generatedIdFilteredNode(node);
                   osm.properties = properties;
                   node.Data = osm;
                   if (properties.IdGenerated.Trim().Equals(""))
                   {
                       Debug.WriteLine("");
                   }
                }
            }
            Console.WriteLine("===== DEBUG ENDE - HASH ======");
        }

        /// <summary>
        /// Ermittelt und setzt die Ids in einem Teilbaum
        /// </summary>
        /// <param name="parentNode">gibt den Baum inkl. des Teilbaums ohne Ids an</param>
        /// <param name="idOfParent">gibt die Id des ersten Knotens des Teilbaums ohne Ids an</param>
        public void generatedIdsOfFilteredSubtree(ref ITreeStrategy<OSMElement.OSMElement> tree, String idOfParent)
        {
            //getFilteredTreeOsmElementById(idOfParent);
            ITreeStrategy<OSMElement.OSMElement> subtree =getAssociatedNode(idOfParent, tree);
            if (subtree == null) { return; }
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)subtree).All.Nodes)
            {
                if (node.Data.properties.IdGenerated == null)
                {
                    OSMElement.OSMElement osm = node.Data;
                    GeneralProperties properties = node.Data.properties;
                    properties.IdGenerated = generatedIdFilteredNode(node);
                    osm.properties = properties;
                    node.Data = osm;
                }
            }
            tree = subtree.Root;
        }

        /// <summary>
        /// Erstellt eine Id für ein Knoten des gefilterten Baums
        /// </summary>
        /// <param name="node">gibt den Knoten des gefilterten Baums an</param>
        /// <returns>die generierte Id</returns>
        private static String generatedIdFilteredNode(INode<OSMElement.OSMElement> node)
        {
            /* https://blogs.msdn.microsoft.com/csharpfaq/2006/10/09/how-do-i-calculate-a-md5-hash-from-a-string/
             * http://stackoverflow.com/questions/12979212/md5-hash-from-string
             * http://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file
             */
            GeneralProperties properties = node.Data.properties;
            String result = 
                properties.controlTypeFiltered +
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
                properties.labeledbyFiltered +
                // node.BranchCount +
                node.BranchIndex *
                node.Depth;
            
            if (node.HasParent) { result += node.Parent.Data.properties.IdGenerated; }
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
            String tmpHash = String.Join(" : ", hash.Select(p => p.ToString()).ToArray());
           // Console.WriteLine("Node: name = {0}, \t controltype = {1}, \t hash[] = {2}, \t hashString = {3}, \t resultString = {4}", node.Data.properties.nameFiltered, node.Data.properties.controlTypeFiltered, tmpHash, sb.ToString(), result);
            return sb.ToString();
        }

        /// <summary>
        /// Erstellt die Id für einen Knoten des Braille-Baums
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static String generatedIdBrailleNode(OSMElement.OSMElement osmElement)
        {
            /* https://blogs.msdn.microsoft.com/csharpfaq/2006/10/09/how-do-i-calculate-a-md5-hash-from-a-string/
             * http://stackoverflow.com/questions/12979212/md5-hash-from-string
             * http://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file
             */
            GeneralProperties properties = osmElement.properties;
            BrailleRepresentation braille = osmElement.brailleRepresentation;
            String result = properties.controlTypeFiltered +
                braille.fromGuiElement +
                braille.screenName +
                braille.viewName;
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
        /// <param name="subtree">gibt den Teilbaum an, bei dem die Strategy gesetzt werden soll</param>
        public void setFilterstrategyInPropertiesAndObject(Type strategyType, ITreeStrategy<OSMElement.OSMElement> subtree)
        {
            FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.getFilteredTree(), grantTrees.getFilterstrategiesOfNodes());

            if (mainFilterstrategy.FilterstrategyFullName.Equals(strategyType.FullName) && mainFilterstrategy.FilterstrategyDll.Equals(strategyType.Namespace)) 
            {
                removeFilterStrategyofSubtree(strategyType, ref subtree);
                Debug.WriteLine("Die Strategy muss nicht ergänzt werden!");
                grantTrees.setFilteredTree(subtree.Root);
                return; 
            }
            Settings settings = new Settings();
            List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.getFilterstrategiesOfNodes();
           // ITreeStrategy<OSMElement.OSMElement> subtree = getAssociatedNode(idOfParent, tree);
           // if (!subtree.HasChild) { return; }
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)subtree).All.Nodes)
            {
                bool isAdded = FilterstrategiesOfTree.addFilterstrategyOfNode(node.Data.properties.IdGenerated, strategyType, ref filterstrategies);
                if (isAdded)
                {
                    GeneralProperties properties = node.Data.properties;
                    properties.grantFilterStrategy = settings.filterStrategyTypeToUserName(strategyType);
                    strategyMgr.getSpecifiedTreeOperations().changePropertiesOfFilteredNode(properties);
                }
            }
            grantTrees.setFilteredTree( subtree.Root);
        }

        private void removeFilterStrategyofSubtree(Type strategyType, ref ITreeStrategy<OSMElement.OSMElement> subtree)
        {
            Settings settings = new Settings();
            List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.getFilterstrategiesOfNodes();
            //ITreeStrategy<OSMElement.OSMElement> subtree = getAssociatedNode(idOfParent, tree);
            if (!subtree.HasChild) { return; }
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)subtree.Child).All.Nodes)
            {
                bool isRemoved = FilterstrategiesOfTree.removeFilterstrategyOfNode(node.Data.properties.IdGenerated, strategyType, ref filterstrategies);
                if (isRemoved)
                {
                    GeneralProperties properties = node.Data.properties;
                    properties.grantFilterStrategy = "";
                    strategyMgr.getSpecifiedTreeOperations().changePropertiesOfFilteredNode(properties);
                }
            }
        }

        /// <summary>
        /// Gibt einen Teilbaum zurück, welcher nur die Views eines Screens enthält
        /// </summary>
        /// <param name="screenName">gibt den Namen des Screens an, zu dem der Teilbaum ermittelt werden soll</param>
        /// <returns>Teilbaum des Screens oder <c>null</c></returns>
        public ITreeStrategy<OSMElement.OSMElement> getSubtreeOfScreen(String screenName)
        {
            if (screenName == null || screenName.Equals("")) { Debug.WriteLine("Kein Name des Screens angegeben"); return null; }
            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getBrailleTree().Copy();
            if (tree == null || !tree.HasChild) { return null; }
            tree = tree.Child;

            if (tree.Data.brailleRepresentation.screenName.Equals(screenName))
            {
                return tree;
            }
 
            while (tree.HasNext)
            {
                if (tree.Next.Data.brailleRepresentation.screenName.Equals(screenName))
                {
                    return tree.Next;
                }
                tree = tree.Next;
            }
            return null;
        }

        /// <summary>
        /// Gibt die Namen der vorhandenen Screens im Braille-Baum an
        /// </summary>
        /// <returns>Eine Liste der Namen der Screens im Braille-Baum</returns>
        public List<String> getPosibleScreenNames()
        {
            if (grantTrees == null || grantTrees.getBrailleTree() == null || !grantTrees.getBrailleTree().HasChild) { return null; }
            List<String> screens = new List<string>();
            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getBrailleTree().Copy();
            tree = tree.Child;
            screens.Add(tree.Data.brailleRepresentation.screenName);
            while (tree.HasNext)
            {
                tree = tree.Next;
                screens.Add(tree.Data.brailleRepresentation.screenName);
            }
            return screens;
        }

        /// <summary>
        /// aktuallisiert alle Gruppen im Braille-Baum
        /// </summary>
        public void updateBrailleGroups()
        {
            if (grantTrees == null || grantTrees.getBrailleTree() == null || !grantTrees.getBrailleTree().HasChild) { return; }
            ITree<OSMElement.OSMElement> copyOfBrailleTree = (ITree<OSMElement.OSMElement>)grantTrees.getBrailleTree().Copy();
            foreach (INode<OSMElement.OSMElement> node in copyOfBrailleTree.All.Nodes)
            {
                if (node.Data.properties.isContentElementFiltered == false && !node.Data.brailleRepresentation.isGroupChild)
                {
                    Type typeOfTemplate = Type.GetType(node.Data.brailleRepresentation.templateFullName + ", " + node.Data.brailleRepresentation.templateNamspace);
                    if (typeOfTemplate != null)
                    {
                       // ATemplateUi template = (ATemplateUi)Activator.CreateInstance(typeOfTemplate, strategyMgr, grantTrees);
                        TempletUiObject templateobject = new TempletUiObject();
                        templateobject.osm = node.Data;
                        templateobject.Screens = new List<string>();
                        templateobject.Screens.Add(node.Data.brailleRepresentation.screenName);
                        OsmRelationship<String, String> osmRelationship = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(node.Data.properties.IdGenerated) );
                        if (osmRelationship != null)
                        { 
                            ITreeStrategy<OSMElement.OSMElement> subtreeFiltered = getAssociatedNode(osmRelationship.FilteredTree, grantTrees.getFilteredTree());
                            if (subtreeFiltered == null) { return; }
                            String id = subtreeFiltered.Data.properties.IdGenerated;
                            subtreeFiltered = strategyMgr.getSpecifiedFilter().updateFiltering(subtreeFiltered.Data, TreeScopeEnum.Subtree);
                            String idParent = strategyMgr.getSpecifiedTreeOperations().changeSubTreeOfFilteredTree(subtreeFiltered, id);
                           ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getFilteredTree();
                           strategyMgr.getSpecifiedTreeOperations().generatedIdsOfFilteredSubtree(ref tree, idParent);
                           subtreeFiltered = getAssociatedNode(osmRelationship.FilteredTree, grantTrees.getFilteredTree());
                            //entfernen des "alten" Braille-Teilbaums
                           if (node.HasParent)
                           {
                               //List<OsmRelationship<String, String>> conector = grantTrees.getOsmRelationship();
                               //OsmTreeRelationship.removeOsmRelationship(osmRelationship.FilteredTree, osmRelationship.BrailleTree, ref conector);
                               removeChildNodeInBrailleTree((ITreeStrategy<OSMElement.OSMElement>)node);
                           }
                            if (subtreeFiltered != null)
                            {
                                IGenaralUiTemplate uiTemplate = new GenaralUI(strategyMgr, grantTrees);
                                uiTemplate.createUiElementFromTemplate(subtreeFiltered, templateobject, node.Data.properties.IdGenerated);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Löscht alle Kindelemente und deren OSM-Beziehungen von Gruppen im Braille-Baum
        /// </summary>
        public void deleteChildsOfBrailleGroups()
        {
            if (grantTrees == null || grantTrees.getBrailleTree() == null || !grantTrees.getBrailleTree().HasChild) { return; }
            ITree<OSMElement.OSMElement> copyOfBrailleTree = (ITree<OSMElement.OSMElement>)grantTrees.getBrailleTree().Copy();
            foreach (INode<OSMElement.OSMElement> node in copyOfBrailleTree.All.Nodes)
            {
                /*if (node.Data.properties.isContentElementFiltered == false)
                {
                   //für alle KinElemente
                    //strategyMgr.getSpecifiedTreeOperations().removeNodeInBrailleTree()
                    //OsmTreeRelationship.removeOsmRelationship()
                }*/
                if (node.HasParent && node.Parent.Data.properties.isContentElementFiltered == false)
                {
                    OsmRelationship<String, String> osmRelationship = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(node.Data.properties.IdGenerated));
                    if(osmRelationship != null)
                    {
                        Debug.WriteLine("Lösche Element ({0}, {1}) mit der ID {2} --> filteredTree-Id {3}", node.Data.properties.valueFiltered, node.Data.properties.controlTypeFiltered, node.Data.properties.IdGenerated, osmRelationship.FilteredTree);
                        List<OsmRelationship<String, String>> conector = grantTrees.getOsmRelationship();
                        OsmTreeRelationship.removeOsmRelationship(osmRelationship.FilteredTree, osmRelationship.BrailleTree, ref conector);
                        strategyMgr.getSpecifiedTreeOperations().removeNodeInBrailleTree(node.Data);
                    }
                }
            }
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
        public ITreeStrategy<OSMElement.OSMElement> getTreeElementOfViewAtPoint(int pointX, int pointY, String groupViewName, int offsetX, int offsetY)
        {
            if (grantTrees == null || groupViewName == null || groupViewName.Equals("")) { return null; }
            String visibleScreen = strategyMgr.getSpecifiedBrailleDisplay().getVisibleScreen();
            ITreeStrategy<OSMElement.OSMElement> screenTree = getSubtreeOfScreen(visibleScreen);
            screenTree = getSubtreeOfView(groupViewName, screenTree);
            if (screenTree == null || screenTree.Equals(strategyMgr.getSpecifiedTree().NewNodeTree())) { return null; }
           // screenTree = screenTree.Child;
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)screenTree).AllChildren.Nodes)
            {
               Rect rect = node.Data.properties.boundingRectangleFiltered;
               if (rect.Contains(pointX - offsetX, pointY - offsetY))
               {
                   return (ITreeStrategy<OSMElement.OSMElement>)node;
               }
               /*if ((rect.X + offsetX <= pointX && rect.X + offsetX + rect.Width >= pointX) && (rect.Y + offsetY <= pointY && rect.Y + offsetY + rect.Height >= pointY))
               {
                   Debug.WriteLine("");
               }
               Debug.WriteLine("");*/
            }
            return (ITreeStrategy<OSMElement.OSMElement>)screenTree; 
        }

        private ITreeStrategy<OSMElement.OSMElement> getSubtreeOfView(String viewName, ITreeStrategy<OSMElement.OSMElement> subtree)
        {
            if (subtree == null) { return null; }
            if (subtree.HasChild)
            {
                subtree = subtree.Child;
                if (subtree.Data.brailleRepresentation.viewName.Equals(viewName))
                {
                    return subtree;
                }
            }
            else { return null; }
            while (subtree.HasNext)
            {
                subtree = subtree.Next;
                if (subtree.Data.brailleRepresentation.viewName.Equals(viewName))
                {
                    return subtree;
                }
            }
            return null;
        }

        /// <summary>
        /// Ändert die Eigenschaft beim Braille-Knoten, welcher dem Screen entspricht,
        /// </summary>
        /// <param name="screenName">gibt den namen des Screens an</param>
        /// <param name="isActiv"></param>
        /// <param name="navigationbarSubstring">gibt einen (Teil-)String des Namens des Navigationsleiste an; wie diese im Baum benannt ist</param>
        public void setPropertyForScreen(string screenName, bool isActiv, String navigationbarSubstring = "NavigationBarScreens")
        {
            /*
             * 1. Knoten mit Screen suchen
             * 2. Navigationsleiset darin suchen
             * 2. Eigenschaft ändern
             * 3. Leiste neu Darstellen
             */
            ITreeStrategy<OSMElement.OSMElement> screenTree = strategyMgr.getSpecifiedTreeOperations().getSubtreeOfScreen(screenName);
            INode<OSMElement.OSMElement> navigationBarSubtree = (INode<OSMElement.OSMElement>)strategyMgr.getSpecifiedTree().NewNodeTree();
            foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)screenTree).AllChildren.Nodes)
            {
                if (node.Data.brailleRepresentation.viewName.Contains(navigationbarSubstring))
                { //Knoten mit Navigationsleiste gefunden
                    navigationBarSubtree = node;
                    break;
                }
            }
            if (navigationBarSubtree != null && navigationBarSubtree.Count > 0)
            {
                Debug.WriteLine("" + navigationBarSubtree.ToString());
                foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)navigationBarSubtree).AllChildren.Nodes)
                {
                    GeneralProperties prop = node.Data.properties;
                    if (node.Data.properties.valueFiltered.Equals(screenName))
                    {                        
                        prop.isEnabledFiltered = false;
                       // Debug.WriteLine("Tab '" + node.Data.properties.valueFiltered + "' ist disabled");
                    }
                    else
                    {
                        prop.isEnabledFiltered = true;
                        //Debug.WriteLine("Tab '" + node.Data.properties.valueFiltered + "' ist enabled");
                    }
                    OSMElement.OSMElement osm = node.Data;
                    osm.properties = prop;
                    
                    //changeBrailleRepresentation(ref osm);
                    node.Data = osm;
                }
                OSMElement.OSMElement osmScreen = navigationBarSubtree.Data;
                grantTrees.setBrailleTree(navigationBarSubtree.Root);
                //removeChildNodeInBrailleTree
               
                strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(ref osmScreen);
                navigationBarSubtree.Data = osmScreen;
                grantTrees.setBrailleTree(navigationBarSubtree.Root);
                
            }
        }

        /// <summary>
        /// Ermittelt für jeden Screen-Teilbaum, den Knoten mit der Navigationsleiste
        /// </summary>
        /// <param name="navigationbarSubstring">gibt den Teil-String des Namenes für die view der Navigationsleiste an</param>
        /// <returns></returns>
        public List<ITreeStrategy<OSMElement.OSMElement>> getListOfNavigationbars(String navigationbarSubstring = "NavigationBarScreens")
        {
            List<ITreeStrategy<OSMElement.OSMElement>> result = new List<ITreeStrategy<OSMElement.OSMElement>>();
           // ITreeStrategy<OSMElement.OSMElement> brailleTree = grantTrees.getBrailleTree();
            if (grantTrees.getBrailleTree() == null) { return result; }
           foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)grantTrees.getBrailleTree()).AllChildren.Nodes)
            {
                if(node.Data.brailleRepresentation.viewName != null && node.Data.brailleRepresentation.viewName.Contains(navigationbarSubstring) && node.HasChild)
                {
                    result.Add((ITreeStrategy<OSMElement.OSMElement>)node);
                }
            }
            return result;
        }

    }
}
