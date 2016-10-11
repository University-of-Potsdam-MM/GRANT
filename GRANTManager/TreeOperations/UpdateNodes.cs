using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GRANTManager.Interfaces;
using OSMElement;
using OSMElement.UiElements;

namespace GRANTManager.TreeOperations
{
    /// <summary>
    /// Die Klasse Kapselt update-Methode -> ectl. in ein anderes Paket verschieben
    /// </summary>
    public class UpdateNodes
    {
        private StrategyManager strategyMgr;
        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperation;
        public UpdateNodes(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            this.treeOperation = treeOperation;
        }

        /// <summary>
        /// Aktualisiert einen Knoten des gefilterten Baums;
        /// dabei wird ggf. für den Knoten kurzzeitig die FilterMethode geändert
        /// </summary>
        /// <param name="filteredTreeGeneratedId">gibt die generierte Id des Knotens an</param>
        public void updateNodeOfFilteredTree(String filteredTreeGeneratedId)
        {
            List<Object> relatedFilteredTreeObject = treeOperation.searchNodes.getAssociatedNodeList(filteredTreeGeneratedId, grantTrees.getFilteredTree()); //TODO: in dem Kontext wollen wir eigentlich nur ein Element zurückbekommen
            foreach (ITreeStrategy<OSMElement.OSMElement> treeElement in relatedFilteredTreeObject)
            {
                //prüfen, ob der Knoten nicht mit dem standard-filterStrategies gefiltert werden soll und ggf. Filter kurzzeitig wechseln
                FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.getFilteredTree(), grantTrees.getFilterstrategiesOfNodes(), strategyMgr.getSpecifiedTree());
                FilterstrategyOfNode<String, String, String> nodeFilterstrategy = FilterstrategiesOfTree.getFilterstrategyOfNode(filteredTreeGeneratedId, grantTrees.getFilterstrategiesOfNodes());
                if (nodeFilterstrategy != null)
                {

                            strategyMgr.setSpecifiedFilter(nodeFilterstrategy.FilterstrategyFullName + ", " + nodeFilterstrategy.FilterstrategyDll);
                            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);

                }
                //Filtern + Knoten aktualisieren
                OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(strategyMgr.getSpecifiedTree().GetData(treeElement));
                changePropertiesOfFilteredNode(properties);

                if (nodeFilterstrategy != null)
                {
                    //Filter wieder zurücksetzen
                    strategyMgr.setSpecifiedFilter(mainFilterstrategy.FilterstrategyFullName + ", " + mainFilterstrategy.FilterstrategyDll);
                    strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                    strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
                }
            }
        }


        /// <summary>
        /// Filtert einen Knoten neu und nimmt dazu die gerade eingestellte Strategy,
        /// dadurch kann der Filter für einen Knoten geändert werden
        /// </summary>
        /// <param name="filteredTreeGeneratedId">gibt die generierte Id des Knotens an</param>
        public void filterNodeWithNewStrategy(String filteredTreeGeneratedId)
        {
            OSMElement.OSMElement relatedFilteredTreeObject = treeOperation.searchNodes.getFilteredTreeOsmElementById(filteredTreeGeneratedId);
            if (relatedFilteredTreeObject.Equals(new OSMElement.OSMElement())) { return; }
                //prüfen, ob der Knoten nicht mit dem standard-filterStrategies gefiltert werden soll und ggf. Filter kurzzeitig wechseln
                FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.getFilteredTree(), grantTrees.getFilterstrategiesOfNodes(), strategyMgr.getSpecifiedTree());
                //Filtern
                OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(relatedFilteredTreeObject);
                
                if(!(mainFilterstrategy.FilterstrategyFullName.Equals(strategyMgr.getSpecifiedFilter().GetType().FullName) && mainFilterstrategy.FilterstrategyDll.Equals(strategyMgr.getSpecifiedFilter().GetType().Namespace)))
                {
                    // Filter für den Knoten merken
                    List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.getFilterstrategiesOfNodes();
                    FilterstrategiesOfTree.addFilterstrategyOfNode(filteredTreeGeneratedId,  strategyMgr.getSpecifiedFilter().GetType(), ref filterstrategies);
                    Settings settings = new Settings();
                    properties.grantFilterStrategy = settings.filterStrategyTypeToUserName(strategyMgr.getSpecifiedFilter().GetType());
                }
                else
                {
                    if (FilterstrategiesOfTree.getFilterstrategyOfNode(filteredTreeGeneratedId, grantTrees.getFilterstrategiesOfNodes()) != null)
                    {
                        //gemerkten Filter für den Knoten löschen -> Standardfilter wird genutzt
                        List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.getFilterstrategiesOfNodes();
                        bool isRemoved = FilterstrategiesOfTree.removeFilterstrategyOfNode(filteredTreeGeneratedId, strategyMgr.getSpecifiedFilter().GetType(), ref filterstrategies);
                        if (isRemoved)
                        {
                            properties.grantFilterStrategy = "";
                        }
                    }
                }
                // Knoten aktualisieren
                changePropertiesOfFilteredNode(properties);
        }

        private Type getTypeOfStrategy(String fullName, String ns)
        {
            return Type.GetType(fullName+", "+ns);
        }

        /// <summary>
        /// Vergleicht, ob der gespeicherte Anwendungspfad und der neu ermittelte Pfad übereinstimmen und passt den Pfad ggf. an
        /// </summary>
        public void compareAndChangeFileName()
        {
            if (!strategyMgr.getSpecifiedTree().HasChild(grantTrees.getFilteredTree()) || strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child( grantTrees.getFilteredTree())).properties.Equals(new GeneralProperties())) { return; }
            String fileNameNew = strategyMgr.getSpecifiedOperationSystem().getFileNameOfApplicationByModulName(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).properties.moduleName);
            if (!fileNameNew.Equals(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).properties.fileName))
            {
                Debug.WriteLine("Der Pfad der Anwendung muss amgepasst werden.");
                changeFileName(fileNameNew);
            }
        }

        /// <summary>
        /// Ändert den Dateipfad der gefilterten Anwendung
        /// </summary>
        /// <param name="fileNameNew">gibt den neuen Dateipfad an</param>
        public void changeFileName(String fileNameNew)
        {
            if (!strategyMgr.getSpecifiedTree().HasChild(grantTrees.getFilteredTree()) || strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).properties.Equals(new GeneralProperties())) { return; }
            GeneralProperties properties = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).properties;
            properties.fileName = fileNameNew;
            changePropertiesOfFilteredNode(properties);
        }

        #region Kopie aus TreeOperations
        /// <summary>
        /// Ändert von einem Knoten die <code>GeneralProperties</code> ausgehend von der <code>IdGenerated</code>
        /// </summary>
        /// <param name="properties">gibt die neuen <code>GeneralProperties</code> an</param>
        public void changePropertiesOfFilteredNode(GeneralProperties properties)
        {
            List<Object> node = treeOperation.searchNodes.searchProperties(grantTrees.getFilteredTree(), properties);
            if (node.Count == 1)
            {
                OSMElement.OSMElement osm = new OSMElement.OSMElement();
                osm.brailleRepresentation = strategyMgr.getSpecifiedTree().GetData(node[0]).brailleRepresentation;
                osm.events = strategyMgr.getSpecifiedTree().GetData(node[0]).events;
                properties.IdGenerated = strategyMgr.getSpecifiedTree().GetData(node[0]).properties.IdGenerated;
                osm.properties = properties;
                strategyMgr.getSpecifiedTree().SetData(node[0], osm);
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
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.getFilteredTree()))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated != null && strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated.Equals(idGeneratedOld))
                {
                    OSMElement.OSMElement osm = new OSMElement.OSMElement();
                    osm.brailleRepresentation = strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation;
                    osm.events = strategyMgr.getSpecifiedTree().GetData(node).events;
                    properties.IdGenerated = idGeneratedOld;
                    osm.properties = properties;
                    strategyMgr.getSpecifiedTree().SetData(node, osm);
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
            Object brailleTree = grantTrees.getBrailleTree();
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated != null && strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated.Equals(element.properties.IdGenerated))
                {
                    strategyMgr.getSpecifiedTree().SetData(node, element);
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
                ListMenuItem listMenuItem = (ListMenuItem)element.brailleRepresentation.uiElementSpecialContent;
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
            OsmConnector<String, String> osmRelationship = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(element.properties.IdGenerated));
            if (osmRelationship == null)
            {
                Console.WriteLine("kein passendes objekt gefunden");
                return false;
            }
            //OSMElement.OSMElement associatedNode = getFilteredTreeOsmElementById(osmRelationship.FilteredTree);

            List<Object> associatedNodeList = treeOperation.searchNodes.getAssociatedNodeList(osmRelationship.FilteredTree, grantTrees.getFilteredTree());
            if (associatedNodeList != null && associatedNodeList.Count == 1 && strategyMgr.getSpecifiedTree().HasChild(associatedNodeList[0]))
            {
                //die Infos ob eine Checkbox ausgewählt ist stehen (zumindest bei meiner Beispielanwendung) im Kindelement
                return strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(associatedNodeList[0])).properties.isToggleStateOn != null ? (bool)strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(associatedNodeList[0])).properties.isToggleStateOn : false;
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
            OsmConnector<String, String> osmRelationship = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(osmElement.properties.IdGenerated));
            if (osmRelationship == null)
            {
                Console.WriteLine("kein passendes objekt gefunden");
                return "";
            }
            OSMElement.OSMElement associatedNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(osmRelationship.FilteredTree);
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
            OsmConnector<String, String> osmRelationship = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(osmElement.properties.IdGenerated));
            if (osmRelationship == null)
            {
                Console.WriteLine("kein passendes objekt gefunden");
                return null;
            }
            OSMElement.OSMElement associatedNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(osmRelationship.FilteredTree);
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
                grantTrees.setBrailleTree(strategyMgr.getSpecifiedTree().NewTree());
            }

            //prüfen, ob der Knoten schon vorhanden ist
            if (brailleNode.properties.IdGenerated != null && !brailleNode.properties.IdGenerated.Equals(""))
            {
                OSMElement.OSMElement nodeToRemove = treeOperation.searchNodes.getBrailleTreeOsmElementById(brailleNode.properties.IdGenerated);
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
            Object tree = grantTrees.getBrailleTree();
            Type t = grantTrees.getBrailleTree().GetType();
            //  foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)grantTrees.getBrailleTree()).All.Nodes)
            if (strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.getBrailleTree()) < 1)
            {
                Console.WriteLine();
            }
            Console.WriteLine("strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.getBrailleTree()) = {0}", strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.getBrailleTree()));
            Object children = strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.getBrailleTree());
            Console.WriteLine();
            foreach (Object node in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.getBrailleTree()))
            {
                 //ermittelt an welchen Screen-Knoten die View angehangen werden soll
                if (!strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.Equals(new BrailleRepresentation()) && strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.screenName.Equals(brailleNode.brailleRepresentation.screenName))
                {
                    OSMElement.OSMElement brailleNodeWithId = brailleNode;
                    GeneralProperties prop = brailleNode.properties;
                    prop.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(brailleNode);
                    brailleNodeWithId.properties = prop;
                    if (parentId == null)
                    {
                        strategyMgr.getSpecifiedTree().AddChild(node, brailleNodeWithId);
                        return prop.IdGenerated;
                    }
                    else
                    {
                        // es müssen von diesem Screen-Zweig alle Kinder durchsucht werden um die view mit der parentId zu finden
                        if (strategyMgr.getSpecifiedTree().HasChild(node))
                        {
                            Object nodeScreen = strategyMgr.getSpecifiedTree().Child(node);
                            if (strategyMgr.getSpecifiedTree().GetData(nodeScreen).properties.IdGenerated.Equals(parentId))
                            {
                                strategyMgr.getSpecifiedTree().AddChild(nodeScreen, brailleNodeWithId);
                                return prop.IdGenerated;
                            }
                            while (strategyMgr.getSpecifiedTree().HasNext(nodeScreen))
                            {
                                nodeScreen = strategyMgr.getSpecifiedTree().Next(nodeScreen);
                                if (strategyMgr.getSpecifiedTree().GetData(nodeScreen).properties.IdGenerated.Equals(parentId))
                                {
                                    strategyMgr.getSpecifiedTree().AddChild(nodeScreen, brailleNodeWithId);
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
            if (!strategyMgr.getSpecifiedTree().Contains(grantTrees.getBrailleTree(), osmScreen)) { return false; }
            Object treeCopy = strategyMgr.getSpecifiedTree().Copy(grantTrees.getBrailleTree());
            if (!strategyMgr.getSpecifiedTree().HasChild(treeCopy)) { return false; }
            treeCopy = strategyMgr.getSpecifiedTree().Child(treeCopy);
            bool hasNext = true;
            do
            {
                if (strategyMgr.getSpecifiedTree().GetData(treeCopy).brailleRepresentation.screenName.Equals(screenName))
                {
                    //TODO: alle Kinder untersuchen
                    if (strategyMgr.getSpecifiedTree().HasChild(treeCopy))
                    {
                        treeCopy = strategyMgr.getSpecifiedTree().Child(treeCopy);
                        if (strategyMgr.getSpecifiedTree().GetData(treeCopy).brailleRepresentation.viewName.Equals(viewName)) { Debug.WriteLine("Achtung: für den Screen '" + screenName + "' existiert schon eine view mit dem Namen '" + viewName + "'!"); return true; }
                        while (strategyMgr.getSpecifiedTree().HasNext(treeCopy))
                        {
                            treeCopy = strategyMgr.getSpecifiedTree().Next(treeCopy);
                            if (strategyMgr.getSpecifiedTree().GetData(treeCopy).brailleRepresentation.viewName.Equals(viewName)) { Debug.WriteLine("Achtung: für den Screen '" + screenName + "' existiert schon eine view mit dem Namen '" + viewName + "'!"); return true; }
                        }
                    }
                    return false;
                }
                if (strategyMgr.getSpecifiedTree().HasNext(treeCopy))
                {
                    treeCopy = strategyMgr.getSpecifiedTree().Next(treeCopy);
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
            if (!strategyMgr.getSpecifiedTree().Contains(grantTrees.getBrailleTree(), osmScreen))
            {
                strategyMgr.getSpecifiedTree().AddChild(grantTrees.getBrailleTree(), osmScreen);
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
            OSMElement.OSMElement nodeToRemove = treeOperation.searchNodes.getBrailleTreeOsmElementById(brailleNode.properties.IdGenerated);
            if (nodeToRemove.Equals(new OSMElement.OSMElement()))
            {
                Console.WriteLine("Der Knoten ist nicht vorhanden!");
                return;
            }
            strategyMgr.getSpecifiedTree().Remove(grantTrees.getBrailleTree(), nodeToRemove);

        }

        /// <summary>
        /// Entfernt alle Kinder des Knotens, aber nicht den Knoten selbst
        /// </summary>
        /// <param name="parentSubtree">gibt den Teilbaum an, bei dem alle Kinder entfernt werden sollen</param>
        public void removeChildNodeInBrailleTree(Object parentSubtree)
        {
            if (parentSubtree != null && strategyMgr.getSpecifiedTree().HasChild(parentSubtree) && grantTrees.getBrailleTree() != null)
            {
                List<OSMElement.OSMElement> listToRemove = new List<OSMElement.OSMElement>();
                Object childeens = strategyMgr.getSpecifiedTree().Child(parentSubtree);
                //grantTrees.getBrailleTree().Remove(childeens.Data);
                listToRemove.Add(strategyMgr.getSpecifiedTree().GetData(childeens));
                while (strategyMgr.getSpecifiedTree().HasNext(childeens))
                {
                    childeens = strategyMgr.getSpecifiedTree().Next(childeens);
                    //grantTrees.getBrailleTree().Remove(childeens.Data);
                    listToRemove.Add(strategyMgr.getSpecifiedTree().GetData(childeens));
                }
                foreach (OSMElement.OSMElement osm in listToRemove)
                {
                    strategyMgr.getSpecifiedTree().Remove(grantTrees.getBrailleTree(), osm);
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
            Object treeLoaded = strategyMgr.getSpecifiedTree().Copy(grantTrees.getFilteredTree());

            Object treeNew = strategyMgr.getSpecifiedFilter().filtering(hwndNew, TreeScopeEnum.Application, -1);
            grantTrees.setFilteredTree(treeNew);

            if (treeNew.Equals(strategyMgr.getSpecifiedTree().NewTree()) || !strategyMgr.getSpecifiedTree().HasChild(treeNew)) { throw new Exception("Anwendung kann nicht neu gefiltert werden."); }
            FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.getFilteredTree(), grantTrees.getFilterstrategiesOfNodes(), strategyMgr.getSpecifiedTree());
            foreach (FilterstrategyOfNode<String, String, String> nodeStrategy in grantTrees.getFilterstrategiesOfNodes())
            {
                //ersten Knoten ausschließen -> wurde mit der richtigen Strategy gefiltert
                if (!nodeStrategy.IdGenerated.Equals(mainFilterstrategy.IdGenerated))
                {
                    //Filter ändern
                    strategyMgr.setSpecifiedFilter(nodeStrategy.FilterstrategyFullName + ", " + nodeStrategy.FilterstrategyDll);
                    strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                    strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
                    // Knoten in neuen Baum suchen + filtern und aktualisieren
                    OSMElement.OSMElement foundNewNode = treeOperation.searchNodes.getAssociatedNodeElement(nodeStrategy.IdGenerated, grantTrees.getFilteredTree());
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
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
        }


        /// <summary>
        /// Ändert einen Teilbaum des gefilterten Baums;
        /// Achtung die Methode sollte nur genutzt werden, wenn von einem Element alle Kindelemente neu gefiltert wurden
        /// </summary>
        /// <param name="subTree">gibt den Teilbaum an</param>
        /// <param name="idOfFirstNode">gibt die Id des esten Knotens des Teilbaumes an</param>
        /// <returns>die Id des Elternknotens des Teilbaumes oder <c>null</c></returns>
        public String changeSubTreeOfFilteredTree(Object subTree, String idOfFirstNode)
        {
            if (subTree == strategyMgr.getSpecifiedTree().NewTree() || strategyMgr.getSpecifiedTree().HasChild(subTree) == false) { Debug.WriteLine("Keine Elemente im Teilbaum!"); return null; }
            if (grantTrees.getFilteredTree() == null) { Debug.WriteLine("Kein Baum Vorhanden!"); return null; }
            if (idOfFirstNode == null || idOfFirstNode.Equals("")) { Debug.WriteLine("Keine Id des ersten Knotens im Teilbaum vorhanden!"); return null; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.getFilteredTree()))
            { 
                if (idOfFirstNode.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated))
                {
                    Debug.WriteLine("Teilbum gefunden");
                    if (strategyMgr.getSpecifiedTree().HasParent(node))
                    {
                        //  if (subTree.Child.HasNext || subTree.Child.HasChild)
                        {
                            Object parentNode = strategyMgr.getSpecifiedTree().Parent(node);

                            if (strategyMgr.getSpecifiedTree().BranchIndex(node) == 0)
                            {
                                strategyMgr.getSpecifiedTree().Remove(grantTrees.getFilteredTree(), strategyMgr.getSpecifiedTree().GetData(node));
                                strategyMgr.getSpecifiedTree().InsertChild(parentNode, subTree);
                            }
                            else
                            {
                                if (strategyMgr.getSpecifiedTree().BranchIndex(node) +1 == strategyMgr.getSpecifiedTree().BranchCount(node))
                                {
                                    strategyMgr.getSpecifiedTree().Remove(grantTrees.getFilteredTree(), strategyMgr.getSpecifiedTree().GetData(node));
                                    strategyMgr.getSpecifiedTree().AddChild(parentNode, subTree);
                                }
                                else
                                {
                                    Object previousNode = strategyMgr.getSpecifiedTree().Previous(node);
                                    strategyMgr.getSpecifiedTree().Remove(grantTrees.getFilteredTree(), strategyMgr.getSpecifiedTree().GetData(node));
                                    strategyMgr.getSpecifiedTree().InsertNext(previousNode, subTree);
                                }
                            }
                            return strategyMgr.getSpecifiedTree().GetData(parentNode).properties.IdGenerated;
                        }

                    }
                }
            }
            Debug.WriteLine("Knoten im Teilbaum nicht gefunden!");
            return null;
        }

        /// <summary>
        /// setzt bei allen Element ausgehend von der IdGenerated im Baum die angegebene Filterstrategie
        /// </summary>
        /// <param name="strategyType">gibt die zusetzende Strategie an</param>
        /// <param name="subtree">gibt den Teilbaum an, bei dem die Strategy gesetzt werden soll</param>
        public void setFilterstrategyInPropertiesAndObject(Type strategyType, Object subtree)
        {
            FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.getFilteredTree(), grantTrees.getFilterstrategiesOfNodes(), strategyMgr.getSpecifiedTree());

            if (mainFilterstrategy.FilterstrategyFullName.Equals(strategyType.FullName) && mainFilterstrategy.FilterstrategyDll.Equals(strategyType.Namespace))
            {
                removeFilterStrategyofSubtree(strategyType, ref subtree);
                Debug.WriteLine("Die Strategy muss nicht ergänzt werden!");
                grantTrees.setFilteredTree(strategyMgr.getSpecifiedTree().Root(subtree));
                return;
            }
            Settings settings = new Settings();
            List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.getFilterstrategiesOfNodes();
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(subtree))
            {
                bool isAdded = FilterstrategiesOfTree.addFilterstrategyOfNode(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated, strategyType, ref filterstrategies);
                if (isAdded)
                {
                    GeneralProperties properties = strategyMgr.getSpecifiedTree().GetData(node).properties;
                    properties.grantFilterStrategy = settings.filterStrategyTypeToUserName(strategyType);
                    changePropertiesOfFilteredNode(properties);
                }
            }
            grantTrees.setFilteredTree(strategyMgr.getSpecifiedTree().Root(subtree));
        }

        private void removeFilterStrategyofSubtree(Type strategyType, ref Object subtree)
        {
            Settings settings = new Settings();
            List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.getFilterstrategiesOfNodes();
            //ITreeStrategy<OSMElement.OSMElement> subtree = getAssociatedNode(idOfParent, tree);
            if (!strategyMgr.getSpecifiedTree().HasChild(subtree)) { return; }
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(strategyMgr.getSpecifiedTree().Child(subtree)))
            {
                bool isRemoved = FilterstrategiesOfTree.removeFilterstrategyOfNode(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated, strategyType, ref filterstrategies);
                if (isRemoved)
                {
                    GeneralProperties properties = strategyMgr.getSpecifiedTree().GetData(node).properties;
                    properties.grantFilterStrategy = "";
                    changePropertiesOfFilteredNode(properties);
                }
            }
        }
        
        /// <summary>
        /// aktuallisiert alle Gruppen im Braille-Baum
        /// </summary>
        public void updateBrailleGroups()
        {
            if (grantTrees == null || grantTrees.getBrailleTree() == null || !strategyMgr.getSpecifiedTree().HasChild(grantTrees.getBrailleTree())) { return; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(strategyMgr.getSpecifiedTree().Copy(grantTrees.getBrailleTree())) )
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.isContentElementFiltered == false && !strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.isGroupChild)
                {
                    Type typeOfTemplate = Type.GetType(strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.templateFullName + ", " + strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.templateNamspace);
                    if (typeOfTemplate != null)
                    {
                        TempletUiObject templateobject = new TempletUiObject();
                        templateobject.osm = strategyMgr.getSpecifiedTree().GetData(node);
                        templateobject.Screens = new List<string>();
                        templateobject.Screens.Add(strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.screenName);
                        OsmConnector<String, String> osmRelationship = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated));
                        if (osmRelationship != null)
                        {
                            Object subtreeFiltered = treeOperation.searchNodes.getAssociatedNode(osmRelationship.FilteredTree, grantTrees.getFilteredTree());
                            if (subtreeFiltered == null) { return; }
                            String id = strategyMgr.getSpecifiedTree().GetData(subtreeFiltered).properties.IdGenerated;
                            subtreeFiltered = strategyMgr.getSpecifiedFilter().updateFiltering(strategyMgr.getSpecifiedTree().GetData(subtreeFiltered), TreeScopeEnum.Subtree);
                            String idParent = changeSubTreeOfFilteredTree(subtreeFiltered, id);
                            Object tree = grantTrees.getFilteredTree();
                           tree=  treeOperation.generatedIds.generatedIdsOfFilteredSubtree(treeOperation.searchNodes.getAssociatedNode(idParent, tree));
                           grantTrees.setFilteredTree(tree);
                            subtreeFiltered = treeOperation.searchNodes.getAssociatedNode(osmRelationship.FilteredTree, grantTrees.getFilteredTree());
                            //entfernen des "alten" Braille-Teilbaums
                            if (strategyMgr.getSpecifiedTree().HasParent(node))
                            {
                                removeChildNodeInBrailleTree(node);
                            }
                            if (subtreeFiltered != null)
                            {
                                strategyMgr.getSpecifiedGeneralTemplateUi().createUiElementFromTemplate(subtreeFiltered, templateobject, strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated);
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
            if (grantTrees == null || grantTrees.getBrailleTree() == null || !strategyMgr.getSpecifiedTree().HasChild(grantTrees.getBrailleTree())) { return; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(strategyMgr.getSpecifiedTree().Copy(grantTrees.getBrailleTree())))
            {
                if (strategyMgr.getSpecifiedTree().HasParent(node) && strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Parent(node)).properties.isContentElementFiltered == false)
                {
                    OsmConnector<String, String> osmRelationship = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated));
                    if (osmRelationship != null)
                    {
                        Debug.WriteLine("Lösche Element ({0}, {1}) mit der ID {2} --> filteredTree-Id {3}", strategyMgr.getSpecifiedTree().GetData(node).properties.valueFiltered, strategyMgr.getSpecifiedTree().GetData(node).properties.controlTypeFiltered, strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated, osmRelationship.FilteredTree);
                        List<OsmConnector<String, String>> conector = grantTrees.getOsmRelationship();
                        OsmTreeConnector.removeOsmConnection(osmRelationship.FilteredTree, osmRelationship.BrailleTree, ref conector);
                        removeNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(node));
                    }
                }
            }
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
            Object screenTree = treeOperation.searchNodes.getSubtreeOfScreen(screenName);
            Object navigationBarSubtree = strategyMgr.getSpecifiedTree().NewTree();
            foreach(Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(screenTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName.Contains(navigationbarSubstring))
                { //Knoten mit Navigationsleiste gefunden
                    navigationBarSubtree = node;
                    break;
                }
            }
            if (navigationBarSubtree != null && strategyMgr.getSpecifiedTree().Count(navigationBarSubtree) > 0)
            {
                //Debug.WriteLine("" + navigationBarSubtree.ToString());
                foreach( Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(navigationBarSubtree))
                {
                    GeneralProperties prop = strategyMgr.getSpecifiedTree().GetData(node).properties;
                    if (strategyMgr.getSpecifiedTree().GetData(node).properties.valueFiltered.Equals(screenName))
                    {
                        prop.isEnabledFiltered = false;
                        // Debug.WriteLine("Tab '" + node.Data.properties.valueFiltered + "' ist disabled");
                    }
                    else
                    {
                        prop.isEnabledFiltered = true;
                        //Debug.WriteLine("Tab '" + node.Data.properties.valueFiltered + "' ist enabled");
                    }
                    OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                    osm.properties = prop;
                    strategyMgr.getSpecifiedTree().SetData(node, osm);
                }
                OSMElement.OSMElement osmScreen = strategyMgr.getSpecifiedTree().GetData(navigationBarSubtree);
                grantTrees.setBrailleTree(strategyMgr.getSpecifiedTree().Root(navigationBarSubtree));
                //removeChildNodeInBrailleTree

                strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(ref osmScreen);
                strategyMgr.getSpecifiedTree().SetData(navigationBarSubtree, osmScreen);
                grantTrees.setBrailleTree(strategyMgr.getSpecifiedTree().Root(navigationBarSubtree));

            }
        }
        #endregion
    }
}
