using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using OSMElement;
using GRANTManager.Interfaces;
using System.Windows.Forms;
using GRANTApplication;
using BrailleIOGuiElementRenderer;
using OSMElement.UiElements;
using System.Windows;
using System.Diagnostics;
using TemplatesUi;
using GRANTManager.TreeOperations;

namespace GRANTExample
{
    public class ExampleBrailleDis
    {
        StrategyManager strategyMgr;
        TreeOperation treeOperation;
        GeneratedGrantTrees grantTrees;

        public ExampleBrailleDis(StrategyManager mgr, GeneratedGrantTrees grantTrees,  TreeOperation treeOperation)
        {
            strategyMgr = mgr;
            this.grantTrees = grantTrees;
            this.treeOperation = treeOperation;
        }

        /// <summary>
        /// Initialisiert, sofern nochnicht vorhanden, ein Ausgabegerät mit den angegebenen Ansichten;
        /// Aktualisiert die Darstellung des Knotens "braille123_6"
        /// </summary>
        /// <param name="fromGuiElement">gibt an welche <code>GeneralProperties</code>-Eigenschaft angezeigt werden soll</param>
        public void UiBrailleDis(String fromGuiElement)
        {
            try
            {               
                    // String path = @"Templates" + System.IO.Path.DirectorySeparatorChar + "TemplateUi.xml";
                String path = @"C:\Users\mkarlapp\Desktop\TemplateUi2.xml";
             //  setDauGui(displayedGuiElementType);
               /* ITreeStrategy<OSMElement.OSMElement> subtreeNav = strategyMgr.getSpecifiedTreeOperations().getSubtreeOfScreen("A1");
                if (subtreeNav != null && subtreeNav.Count > 0)
                {
                    ui.addNavigationbarForScreen(path, subtreeNav);
                }
                GuiFunctions guiFuctions = new GuiFunctions(strategyMgr, grantTrees);
                if (guiFuctions.isTemplateUsableForDevice(path))
                {
                    ui.createUiElementsAllScreensSymbolView(path);
                  //  ui.createUiElementsNavigationbarScreensSymbolView(path);
                    ui.updateNavigationbarScreens(path);
                    
                }*/
                treeOperation.updateNodes.updateBrailleGroups();
                if (strategyMgr.getSpecifiedBrailleDisplay() == null)
                {                   
                    Settings settings = new Settings();
                    strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden

                    strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
                    strategyMgr.getSpecifiedBrailleDisplay().setGeneratedGrantTrees(grantTrees);
                    strategyMgr.getSpecifiedBrailleDisplay().setTreeOperation(treeOperation);
                   // strategyMgr.getSpecifiedBrailleDisplay().initializedSimulator();
                    strategyMgr.getSpecifiedBrailleDisplay().setActiveAdapter();
                    strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
                }
                else
                {
                    if (!strategyMgr.getSpecifiedBrailleDisplay().isInitialized())
                    {
                        strategyMgr.getSpecifiedBrailleDisplay().setActiveAdapter();
                        strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
                    }
                }
                strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
                if (grantTrees.osmTreeConnections == null)
                {                    
                    List<OsmTreeConnectorTuple<String, String>> relationship = ExampleTree.setOsmRelationship();
                    grantTrees.osmTreeConnections =relationship;
                    
                }
                else
                {
                    if (grantTrees.filteredTree == null)
                    {
                        Console.WriteLine("Die Anwendung wurde noch nicht gefiltert - bitte 'F5' drücken");
                        return;
                    }
                    GeneralProperties propertiesForSearch = new GeneralProperties();
                        propertiesForSearch.controlTypeFiltered = "TextBox";
                        List<Object> treeElement = treeOperation.searchNodes.searchNodeByProperties(grantTrees.brailleTree, propertiesForSearch, OperatorEnum.and);
                        String brailleId = "";
                        if (treeElement.Count > 0)
                        {
                            brailleId =strategyMgr.getSpecifiedTree().GetData(treeElement[0]).properties.IdGenerated;
                        }
                        if (brailleId.Equals("")) { return; }
                    OsmTreeConnectorTuple<String, String> osmRelationships = grantTrees.osmTreeConnections.Find(r => r.BrailleTree.Equals(brailleId) || r.FilteredTree.Equals(brailleId));
                    if (osmRelationships != null)
                    {
                        //strategyMgr.getSpecifiedFilter().updateNodeOfFilteredTree(osmRelationships.FilteredTree);
                        treeOperation.updateNodes.filteredNodeElementOfApplication(osmRelationships.FilteredTree);

                    OSMElement.OSMElement relatedBrailleTreeObject = treeOperation.searchNodes.getBrailleTreeOsmElementById(osmRelationships.BrailleTree);
                    if (!relatedBrailleTreeObject.Equals(new OSMElement.OSMElement()))
                    {
                        treeOperation.updateNodes.updateNodeOfBrailleUi(ref relatedBrailleTreeObject);
                        strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(ref relatedBrailleTreeObject);
                        
                    }

                }
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: '{0}'", ex);
            }
            
        }

        public void updateImg()
        {
            if (grantTrees.filteredTree == null)
            {
                Console.WriteLine("Die Anwendung wurde noch nicht gefiltert - bitte 'F5' drücken");
                return;
            }
            String brailleId = "";
            GeneralProperties propertiesForSearch = new GeneralProperties();
            propertiesForSearch.controlTypeFiltered = "Screenshot";
                        List<Object> treeElement = treeOperation.searchNodes.searchNodeByProperties(grantTrees.brailleTree, propertiesForSearch, OperatorEnum.and);
                        if (treeElement.Count > 0)
                        {
                            brailleId = strategyMgr.getSpecifiedTree().GetData(treeElement[0]).properties.IdGenerated;
                        }
                        if (brailleId.Equals("")) { return; }
            OsmTreeConnectorTuple<String, String> osmRelationships = grantTrees.osmTreeConnections.Find(r => r.BrailleTree.Equals(brailleId) || r.FilteredTree.Equals(brailleId)); 
            if(osmRelationships != null)
            {
                //strategyMgr.getSpecifiedFilter().updateNodeOfFilteredTree(osmRelationships.FilteredTree);
                treeOperation.updateNodes.filteredNodeElementOfApplication(osmRelationships.FilteredTree);

                OSMElement.OSMElement relatedBrailleTreeObject = treeOperation.searchNodes.getBrailleTreeOsmElementById(osmRelationships.BrailleTree);
                if (!relatedBrailleTreeObject.Equals(new OSMElement.OSMElement()))
                {
                    strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(ref relatedBrailleTreeObject);
                }
            }
        }

        internal void update()
        {
            try
            {
                
                if (strategyMgr.getSpecifiedBrailleDisplay() == null || grantTrees == null || grantTrees.filteredTree == null || grantTrees.brailleTree == null || grantTrees.osmTreeConnections == null)
                {
                    return;
                }
                if(!strategyMgr.getSpecifiedTree().HasChild(grantTrees.filteredTree)) { return; }
                GeneralProperties prop1Node = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties;
                if(prop1Node.hWndFiltered == null || prop1Node.hWndFiltered.Equals(IntPtr.Zero)) { return; }
                treeOperation.updateNodes.filteredTreeOfApplication(prop1Node.hWndFiltered);
                treeOperation.updateNodes.updateBrailleGroups();
                Object tree = grantTrees.brailleTree;
                foreach (Object o in strategyMgr.getSpecifiedTree().AllChildrenNodes(tree))
                {
                    OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(o);
                    if ((!osm.brailleRepresentation.isGroupChild || osm.brailleRepresentation.groupelementsOfSameType.renderer == null) && !osm.properties.boundingRectangleFiltered.Equals(new Rect()))
                    {
                         treeOperation.updateNodes.updateNodeOfBrailleUi(ref osm);
                        strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(ref osm);
                    }
                }
                strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: '{0}'", ex);
            }
        }

        public bool[,] getRendererExample(String uiElementName)
        {
            if (uiElementName == null || uiElementName.Equals(""))
            {
                Console.WriteLine("kein Name angegeben");
                return null;
            }
            Settings settings = new Settings();
            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className);
            strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
            strategyMgr.getSpecifiedBrailleDisplay().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedBrailleDisplay().setGeneratedGrantTrees(grantTrees);
            bool[,] result = strategyMgr.getSpecifiedBrailleDisplay().getRendererExampleRepresentation(uiElementName);
            Console.WriteLine("Beispiel für " + uiElementName);
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for(int j = 0; j < (result.Length / result.GetLength(0)); j++)
                {
                 //   Console.Write(viewAtPoint[i,j]+ "\t");
                    if (result[i, j]) { Console.Write("x"); }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine("");
            }
                return result;
        }

       
        public List<String> getRendererList()
        {
            Settings settings = new Settings();
            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className);
            strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
            return strategyMgr.getSpecifiedBrailleDisplay().getUiElementRenderer();
        }

        /// <summary>
        /// Wechselt zwischen screen1 und screen2
        /// </summary>
        public void changeScreen(String name)
        {
            if (name.Equals("")) { return; }
            if (strategyMgr.getSpecifiedBrailleDisplay() == null) { return; }
           if( getPosibleScreens().Contains(name)){
               strategyMgr.getSpecifiedBrailleDisplay().setVisibleScreen(name);
            }
           // ITreeStrategy<OSMElement.OSMElement> subtreeFiltered = strategyMgr.getSpecifiedTreeOperations().getSubtreeOfScreen(visibleScreen);
           //Screenshots aktualisieren beim Screen-Wechsel
           Object subnodesOfScreen = treeOperation.searchNodes.getSubtreeOfScreen(name);
           GeneralProperties prop = new GeneralProperties();
           prop.controlTypeFiltered = "Screenshot";
           List<Object> screenshotNodes = treeOperation.searchNodes.searchNodeByProperties(subnodesOfScreen, prop);
           foreach (Object node in screenshotNodes)
           {
               OSMElement.OSMElement osmScreenshot = strategyMgr.getSpecifiedTree().GetData(node);
               strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(ref osmScreenshot);
               strategyMgr.getSpecifiedTree().SetData(node, osmScreenshot);
               grantTrees.brailleTree = strategyMgr.getSpecifiedTree().Root(node);
           }
        }

        public List<String> getPosibleScreens()
        {
            return treeOperation.searchNodes.getPosibleScreenNames();
        }
    }
}
