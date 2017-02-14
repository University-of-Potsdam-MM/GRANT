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
    /// Die Klasse Kapselt update-Methode -> evtl. in ein anderes Paket verschieben
    /// </summary>
    public class UpdateNodes
    {
        /// <summary>
        /// Enthält die zur Verfügung stehenden Strategien
        /// </summary>
        private StrategyManager strategyMgr;
        /// <summary>
        /// Enthält die generierten Bäume und dessen Beziehungen
        /// </summary>
        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperation;

        private String VIEWCATEGORY_SYMBOLVIEW;

        public UpdateNodes(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            this.treeOperation = treeOperation;
            
            List<String> viewCategories = Settings.getPossibleViewCategories();
            if(viewCategories != null) { VIEWCATEGORY_SYMBOLVIEW = viewCategories[0]; }
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
            if(brailleTree == null) { return; }
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
        /// Ändert die Eigenschaften des angegebenen Knotens in StrategyManager.brailleRepresentation 
        /// </summary>
        /// <param name="element">gibt den zu verändernden Knoten an</param>
        public void updateNodeOfBrailleUi(ref OSMElement.OSMElement element)
        {            
            BrailleRepresentation updatedContentBR = element.brailleRepresentation;
            GeneralProperties updatedContentGP = element.properties;
            if (element.brailleRepresentation.isGroupChild && element.brailleRepresentation.groupelementsOfSameType.renderer != null)
            {
                updatedContentGP.controlTypeFiltered = element.brailleRepresentation.groupelementsOfSameType.renderer; //TODO: passt das so?
            }
            if (!strategyMgr.getSpecifiedBrailleDisplay().getAllUiElementRenderer().Contains(updatedContentGP.controlTypeFiltered))
            {
                Debug.Print("Achtung: Der ausgewählte Renderer existiert nicht! Er wurde auf 'Text' gesetzt.");
                updatedContentGP.controlTypeFiltered = "Text";
            }
            if (element.brailleRepresentation.fromGuiElement != null)
            {
                updatedContentGP.valueFiltered = getTextForView(element);
            }
            if (element.brailleRepresentation.uiElementSpecialContent != null && element.brailleRepresentation.uiElementSpecialContent.GetType().Equals(typeof(OSMElement.UiElements.ListMenuItem)))
            {
                ListMenuItem listMenuItem = (ListMenuItem)element.brailleRepresentation.uiElementSpecialContent;
                updatedContentGP.isToggleStateOn = isCheckboxOfAssociatedElementSelected(element);
            }

            
            bool? isEnable = isUiElementEnable(element);
            if (isEnable != null)
            {
                updatedContentGP.isEnabledFiltered = (bool)isEnable;
            }
            // updatedContentBR.text = updatedText;
            if (updatedContentBR.fromGuiElement != "" && !GeneralProperties.getAllStringsFor_fromGuiElement().Contains(updatedContentBR.fromGuiElement))
            {
                Debug.WriteLine("Achtung: Es wurde ein falscher Wert bei 'fromGuiElement' ausgewählt! Deshalb wurd er auf 'Text' gesetzt.");
                updatedContentBR.fromGuiElement = "Text";
            }

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
        /// Ermittelt aufgrund der im StrategyManager angegebenen Beziehungen den anzuzeigenden Text, sollte es für den Text eine Abkürzung geben, so wird diese genutzt
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
            return useAcronymForText( text);
        }

        /// <summary>
        /// Ersetzt einen Text durch eine Abkürzung (sofern vorhanden)
        /// </summary>
        /// <param name="text">gibt den Text, der abgekürzt werdn soll an</param>
        /// <returns> eine abgekürzte Version des Textes oder der Text </returns>
        public String useAcronymForText(String text)
        {
            if (grantTrees.TextviewObject == null || grantTrees.TextviewObject.acronymsOfPropertyContent == null || text == null || text.Equals("")){ return ""; }
            foreach(AcronymsOfPropertyContent aopc in grantTrees.TextviewObject.acronymsOfPropertyContent)
            {
                if (aopc.name.Equals(text))
                {
                    return aopc.acronym;
                }
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
            if (grantTrees.getBrailleTree() == null) { grantTrees.setBrailleTree(strategyMgr.getSpecifiedTree().NewTree()); }
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
                Debug.WriteLine("Kein ScreenName angegeben. Es wurde nichts hinzugefügt!"); return null;
            }
            // prüfen, ob es die View auf dem Screen schon gibt
            if (treeOperation.searchNodes.existViewInScreen(brailleNode.brailleRepresentation.screenName, brailleNode.brailleRepresentation.viewName, brailleNode.brailleRepresentation.screenCategory))
            {
                return null;
            }
            addSubtreeOfScreen(brailleNode.brailleRepresentation.screenName, brailleNode.brailleRepresentation.screenCategory);

            //1. richtige ViewCategorie finden
            foreach (Object viewCategoryNode in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.getBrailleTree()))
            {
                if (strategyMgr.getSpecifiedTree().GetData(viewCategoryNode).brailleRepresentation.screenCategory.Equals(brailleNode.brailleRepresentation.screenCategory))
                {
                    foreach (Object node in strategyMgr.getSpecifiedTree().DirectChildrenNodes(viewCategoryNode))
                    {
                        //2. ermittelt an welchen Screen-Knoten die View angehangen werden soll
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
                                foreach (object childOfNode in strategyMgr.getSpecifiedTree().DirectChildrenNodes(node))
                                {
                                    if (strategyMgr.getSpecifiedTree().GetData(childOfNode).properties.IdGenerated.Equals(parentId))
                                    {
                                        strategyMgr.getSpecifiedTree().AddChild(childOfNode, brailleNodeWithId);
                                        return prop.IdGenerated;
                                    }
                                }
                                strategyMgr.getSpecifiedTree().AddChild(node, brailleNodeWithId);
                                return prop.IdGenerated;
                                //return null;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Fügt einen 'Zweig' für den Screen an den Root-Knoten an, falls der Screen im Baum noch nicht existiert
        /// </summary>
        /// <param name="screenName">gibt den Namen des Screens an</param>
        /// <param name="viewCategory">gibt die Kategorie der View an</param>
        private void addSubtreeOfScreen(String screenName, String viewCategory)
        {
            if (screenName == null || screenName.Equals(""))
            {
                Debug.WriteLine("Kein ScreenName angegeben. Es wurde nichts hinzugefügt!");
                return;
            }
            OSMElement.OSMElement osmScreen = new OSMElement.OSMElement();
            BrailleRepresentation brailleScreen = new BrailleRepresentation();
            brailleScreen.screenName = screenName;
            brailleScreen.screenCategory = viewCategory;
            osmScreen.brailleRepresentation = brailleScreen;
            GeneralProperties propOsmScreen = new GeneralProperties();
            propOsmScreen.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmScreen);
            osmScreen.properties = propOsmScreen;
            

            //der Screenexistiert noch nicht
            if (!strategyMgr.getSpecifiedTree().Contains(grantTrees.getBrailleTree(), osmScreen))
            {
                OSMElement.OSMElement osmViewCategory = new OSMElement.OSMElement();
                BrailleRepresentation brailleViewCategory = new BrailleRepresentation();
                brailleViewCategory.screenCategory = viewCategory;
                osmViewCategory.brailleRepresentation = brailleViewCategory;
                GeneralProperties propViewCategory = new GeneralProperties();
                propViewCategory.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmViewCategory);
                osmViewCategory.properties = propViewCategory;
                Object viewCategorySubtree = null;
                //die viewCategory existert schon
                if(strategyMgr.getSpecifiedTree().Contains(grantTrees.getBrailleTree(), osmViewCategory))
                {
                    //viewCategory suchen
                    foreach(Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes( grantTrees.getBrailleTree()))
                    {
                        if (strategyMgr.getSpecifiedTree().GetData(vC).brailleRepresentation.screenCategory.Equals(viewCategory))
                        {
                            viewCategorySubtree = vC;
                        }
                    }
                }
                //die viewCategory existiert noch nicht
                else
                {
                    viewCategorySubtree = strategyMgr.getSpecifiedTree().AddChild(grantTrees.getBrailleTree(), osmViewCategory);
                }                
                if(viewCategorySubtree == null ) { throw new Exception("Die Containsmethode gab an, dass die ViewCategory schon vorhanden ist, sie konnte im Baum aber nicht gefunden werden!"); }
                
                strategyMgr.getSpecifiedTree().AddChild(viewCategorySubtree, osmScreen);

            }else
            {
                Debug.WriteLine("Der Screen exisitert schon!");
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
                        TemplateUiObject templateobject = new TemplateUiObject();
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
                            if (idParent == null || tree == null) { return; }
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
        /// <param name="navigationbarSubstring">gibt einen (Teil-)String des Namens der Navigationsleiste an; wie diese im Baum benannt ist</param>
        public void setPropertyInNavigationbarForScreen(string screenName, bool isActiv, String navigationbarSubstring = "NavigationBarScreens")
        {
            /*
             * 1. Knoten mit Screen suchen
             * 2. Navigationsleiset darin suchen
             * 2. Eigenschaft ändern
             * 3. Leiste neu Darstellen
             */
            Object screenTree = treeOperation.searchNodes.getSubtreeOfScreen(screenName);
            if (screenTree == null)
            {
                Debug.WriteLine("Gesuchter ScreenName: " + screenName + "\t Baum: \n" + strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.getBrailleTree()));
                throw new Exception("Der Screen konnte nicht gefunden werden, deshalb konnten die Eigenschaften der Navigationsleiste nicht geändert werden!");  
            }
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
                foreach( Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(navigationBarSubtree))
                {
                    GeneralProperties prop = strategyMgr.getSpecifiedTree().GetData(node).properties;
                    if (strategyMgr.getSpecifiedTree().GetData(node).properties.valueFiltered.Equals(screenName))
                    {
                        prop.isEnabledFiltered = false;
                    }
                    else
                    {
                        prop.isEnabledFiltered = true;
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
    }
}
