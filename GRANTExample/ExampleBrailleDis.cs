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
              /* setDauGui(fromGuiElement);
                ITreeStrategy<OSMElement.OSMElement> subtreeNav = strategyMgr.getSpecifiedTreeOperations().getSubtreeOfScreen("A1");
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
                if (grantTrees.getOsmRelationship() == null)
                {                    
                    List<OsmConnector<String, String>> relationship = ExampleTree.setOsmRelationship();
                    grantTrees.setOsmRelationship(relationship);
                    
                }
                else
                {
                    if (grantTrees.getFilteredTree() == null)
                    {
                        Console.WriteLine("Die Anwendung wurde noch nicht gefiltert - bitte 'F5' drücken");
                        return;
                    }
                    GeneralProperties propertiesForSearch = new GeneralProperties();
                        propertiesForSearch.controlTypeFiltered = "TextBox";
                        List<Object> treeElement = treeOperation.searchNodes.searchProperties(grantTrees.getBrailleTree(), propertiesForSearch, OperatorEnum.and);
                        String brailleId = "";
                        if (treeElement.Count > 0)
                        {
                            brailleId =strategyMgr.getSpecifiedTree().GetData(treeElement[0]).properties.IdGenerated;
                        }
                        if (brailleId.Equals("")) { return; }
                    OsmConnector<String, String> osmRelationships = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(brailleId) || r.FilteredTree.Equals(brailleId));
                    if (osmRelationships != null)
                    {
                        //strategyMgr.getSpecifiedFilter().updateNodeOfFilteredTree(osmRelationships.FilteredTree);
                        treeOperation.updateNodes.updateNodeOfFilteredTree(osmRelationships.FilteredTree);

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
            if (grantTrees.getFilteredTree() == null)
            {
                Console.WriteLine("Die Anwendung wurde noch nicht gefiltert - bitte 'F5' drücken");
                return;
            }
            String brailleId = "";
            GeneralProperties propertiesForSearch = new GeneralProperties();
            propertiesForSearch.controlTypeFiltered = "Screenshot";
                        List<Object> treeElement = treeOperation.searchNodes.searchProperties(grantTrees.getBrailleTree(), propertiesForSearch, OperatorEnum.and);
                        if (treeElement.Count > 0)
                        {
                            brailleId = strategyMgr.getSpecifiedTree().GetData(treeElement[0]).properties.IdGenerated;
                        }
                        if (brailleId.Equals("")) { return; }
            OsmConnector<String, String> osmRelationships = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(brailleId) || r.FilteredTree.Equals(brailleId)); 
            if(osmRelationships != null)
            {
                //strategyMgr.getSpecifiedFilter().updateNodeOfFilteredTree(osmRelationships.FilteredTree);
                treeOperation.updateNodes.updateNodeOfFilteredTree(osmRelationships.FilteredTree);

                OSMElement.OSMElement relatedBrailleTreeObject = treeOperation.searchNodes.getBrailleTreeOsmElementById(osmRelationships.BrailleTree);
                if (!relatedBrailleTreeObject.Equals(new OSMElement.OSMElement()))
                {
                  //  strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(relatedBrailleTreeObject.Data);
                    strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(ref relatedBrailleTreeObject);
                }
            }
        }

        public bool[,] getRendererExample()
        {
            if (grantTrees.getFilteredTree() == null)
            {
                Console.WriteLine("Die Anwendung wurde noch nicht gefiltert - bitte 'F5' drücken");
                return null;
            }
            if (grantTrees.getOsmRelationship() == null)
            {
                Console.WriteLine("Es sind noch keine OSM-Beziehungen vorhanden!");
                return null;
            }
            Settings settings = new Settings();
            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className);
            strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
            strategyMgr.getSpecifiedBrailleDisplay().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedBrailleDisplay().setTreeOperation(treeOperation);
   //         strategyMgr.getSpecifiedBrailleDisplay().initializedSimulator();
            setDauGui("nameFiltered");
            OSMElement.OSMElement osmElement = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Next(strategyMgr.getSpecifiedTree().Next(strategyMgr.getSpecifiedTree().Child(grantTrees.getBrailleTree())))); ;//strategyMgr.getBrailleTree().Child.Next.Next.Next.Next.Next.Next.Data;
            bool[,] result = strategyMgr.getSpecifiedBrailleDisplay().getRendererExampleRepresentation(osmElement);
            return result;
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

        #region Beispielobjekte
        private void setDauGui(String fromGuiElement3)
        {
            #region Element 1 Screenshot
            OSMElement.OSMElement osm1 = new OSMElement.OSMElement();
            BrailleRepresentation e1 = new BrailleRepresentation();
            GeneralProperties proper1 = new GeneralProperties();
            e1.screenName = "A1";
            e1.zoom = 1;
            e1.contrast = 120;
            e1.viewName = "v1";
           // c1.fromGuiElement = "nameFiltered";
            e1.showScrollbar = true;
            e1.isVisible = true;
            Rect p1 = new Rect(70,0,50,30);
            proper1.boundingRectangleFiltered = p1;
            
         //   proper1.IdGenerated = "braille123_1";
            proper1.controlTypeFiltered = "Screenshot";
            osm1.brailleRepresentation = e1;
            osm1.properties = proper1;
            treeOperation.updateNodes.addNodeInBrailleTree(osm1);
            #endregion

            #region Element 2 Text
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            GeneralProperties proper2 = new GeneralProperties();
            BrailleRepresentation e2 = new BrailleRepresentation();
            e2.screenName = "A2";
            proper2.valueFiltered = "Hallo 1 Hallo 2 Hallo 3 Hallo 4 Hallo 5";
          //  c2.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e2.showScrollbar = true;
            e2.viewName = "v2";
            e2.isVisible = true;
            Rect p2 = new Rect(90, 32, 29,15);
            proper2.boundingRectangleFiltered = p2;
            Padding padding = new Padding(1, 1, 1, 1);
            e2.padding = padding;
            Padding margin = new Padding(1, 1, 1, 1);
            e2.margin = margin;
            Padding boarder = new Padding(1, 1, 2, 1);
            e2.boarder = boarder;            
            
        //    proper2.IdGenerated = "braille123_2";
            proper2.controlTypeFiltered = "Text";
            osm2.brailleRepresentation = e2;
            osm2.properties = proper2;
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion

            #region Element 3 Text
            OSMElement.OSMElement osm3 = new OSMElement.OSMElement();
            BrailleRepresentation e3 = new BrailleRepresentation();
            GeneralProperties proper3 = new GeneralProperties();
            e3.screenName = "A1";
            e3.showScrollbar = false;
            //   c3.text = "Start Text";
            //c3.fromGuiElement = "nameFiltered";
            e3.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e3.viewName = "v3";
            e3.isVisible = false;
            Rect p3 = new Rect(70,30,30,20);
            proper3.boundingRectangleFiltered = p3;
            
        //    proper3.IdGenerated = "braille123_3";
            proper3.controlTypeFiltered = "Text";
            osm3.brailleRepresentation = e3;
            osm3.properties = proper3;
            treeOperation.updateNodes.addNodeInBrailleTree(osm3);
            #endregion

            #region Element 4 Matrix
            OSMElement.OSMElement osm4 = new OSMElement.OSMElement();
            BrailleRepresentation e4 = new BrailleRepresentation();
            GeneralProperties proper4 = new GeneralProperties();            
            e4.screenName = "A2";
            e4.matrix = new bool[,] { 
                {false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false},
                {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false},
                {false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false},
            };
            e4.viewName = "v4";
            Rect p4 = new Rect(40,50,20,7);
            proper4.boundingRectangleFiltered = p4;
       //     proper4.IdGenerated = "braille123_4";
            proper4.controlTypeFiltered = "Matrix";
            osm4.brailleRepresentation = e4;
            osm4.properties = proper4;
            treeOperation.updateNodes.addNodeInBrailleTree(osm4);
            #endregion

            #region Element 5 Button
            OSMElement.OSMElement osm5 = new OSMElement.OSMElement();
            BrailleRepresentation e5 = new BrailleRepresentation();
            GeneralProperties proper5 = new GeneralProperties();
            e5.screenName = "A1";
            proper5.valueFiltered = "Button";
           // c5.uiElementContent = "Hallo - Button";
           // c5.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e5.viewName = "v5";
            e5.isVisible = true;
            Rect p5 = new Rect(63,12,24,9);
            proper5.boundingRectangleFiltered = p5;
         //   proper5.IdGenerated = "braille123_5";
            proper5.controlTypeFiltered = "Button";
            proper5.isEnabledFiltered = true;
            osm5.brailleRepresentation = e5;
            osm5.properties = proper5;
            treeOperation.updateNodes.addNodeInBrailleTree(osm5);
            #endregion

            #region Element 6 TextBox
            OSMElement.OSMElement osm6 = new OSMElement.OSMElement();
            BrailleRepresentation e6 = new BrailleRepresentation();
            GeneralProperties proper6 = new GeneralProperties();
            e6.screenName = "A2";
            e6.showScrollbar = true;
           // c6.text = "Text 1 Text 2 Text 3 Text 4 Text 5 Text 6";
            //object[] otherContent6 = { UiObjectsEnum.TextBox, textBox6 };
            e6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            //c6.showScrollbar = true;
            
            e6.viewName = "v6";
            e6.isVisible = true;            
            Rect p6 = new Rect(40, 23,50,28);
           proper6.boundingRectangleFiltered = p6;
    //        proper6.IdGenerated = "braille123_6";
            proper6.controlTypeFiltered = "TextBox";
            proper6.isEnabledFiltered = true;
            osm6.brailleRepresentation = e6;
            osm6.properties = proper6;
            treeOperation.updateNodes.addNodeInBrailleTree(osm6);
            #endregion

            #region Element 7 DropDownMenu
            OSMElement.OSMElement osm7 = new OSMElement.OSMElement();
            GeneralProperties proper7 = new GeneralProperties();
            BrailleRepresentation e7 = new BrailleRepresentation();
            e7.isVisible = true;
            e7.screenName = "A1";
            //proper7.valueFiltered = "Text 1 Text 2 Text 3 Text 4 Text 5 Text 6";
            DropDownMenuItem dropDownMenu = new DropDownMenuItem();
            dropDownMenu.hasChild = true;
            dropDownMenu.hasNext = true;
            dropDownMenu.hasPrevious = false;
            dropDownMenu.isChild = false;
            dropDownMenu.isOpen = true;
            dropDownMenu.isVertical = true;
            proper7.valueFiltered = "Datei";
            // object[] otherContent7 = {UiObjectsEnum.DropDownMenu, uiElement};
            e7.uiElementSpecialContent = dropDownMenu;
            //c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e7.viewName = "v7";
            Rect p7 = new Rect(0,10,25,10);
            proper7.boundingRectangleFiltered = p7;
            proper7.isEnabledFiltered = true;
     //       proper7.IdGenerated = "braille123_7";
            proper7.controlTypeFiltered = "DropDownMenu";
            osm7.brailleRepresentation = e7;
            osm7.properties = proper7;
            treeOperation.updateNodes.addNodeInBrailleTree(osm7);
            #endregion

            #region Element 8 DropDownMenu
            OSMElement.OSMElement osm8 = new OSMElement.OSMElement();
            BrailleRepresentation e8 = new BrailleRepresentation();
            GeneralProperties proper8 = new GeneralProperties();
            e8.isVisible = true;
            e8.screenName = "A2";
            DropDownMenuItem dropDownMenu8 = new DropDownMenuItem();
            dropDownMenu8.hasChild = false;
            dropDownMenu8.hasNext = false;
            dropDownMenu8.hasPrevious = true;
            dropDownMenu8.isChild = false;
            dropDownMenu8.isOpen = false;
            dropDownMenu8.isVertical = true;
            proper8.valueFiltered = "Bearbeiten";
           // object[] otherContent8 = { UiObjectsEnum.DropDownMenu, dropDownMenu8 };
            e8.uiElementSpecialContent = dropDownMenu8;
            //c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e8.viewName = "v8";
            Rect p8 = new Rect(25, 10,35,10);
            proper8.boundingRectangleFiltered = p8;
    //        proper8.IdGenerated = "braille123_8";
            proper8.controlTypeFiltered = "DropDownMenu";
            proper8.isEnabledFiltered = true;
            osm8.brailleRepresentation = e8;
            osm8.properties = proper8;
            treeOperation.updateNodes.addNodeInBrailleTree(osm8);
            #endregion

            #region Element 9 DropDownMenu
            OSMElement.OSMElement osm9 = new OSMElement.OSMElement();
            BrailleRepresentation e9 = new BrailleRepresentation();
            GeneralProperties proper9 = new GeneralProperties();
            e9.isVisible = true;
            e9.screenName = "A1";
            DropDownMenuItem dropDownMenu9 = new DropDownMenuItem();
            dropDownMenu9.hasChild = true;
            dropDownMenu9.hasNext = true;
            dropDownMenu9.hasPrevious = false;
            dropDownMenu9.isChild = true;
            dropDownMenu9.isOpen = false;
            dropDownMenu9.isVertical = true;
            proper9.valueFiltered = "Neu";
           // object[] otherContent9 = { UiObjectsEnum.DropDownMenu, dropDownMenu9 };
            e9.uiElementSpecialContent = dropDownMenu9;
            //c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e9.viewName = "v9";
            Rect p9 = new Rect(0, 21, 30 , 8);
            proper9.boundingRectangleFiltered = p9;
            proper9.isEnabledFiltered = true;
 //           proper9.IdGenerated = "braille123_9";
            proper9.controlTypeFiltered = "DropDownMenu";
            osm9.brailleRepresentation = e9;
            osm9.properties = proper9;
            treeOperation.updateNodes.addNodeInBrailleTree(osm9);
            #endregion

            #region Element 10 DropDownMenu
            OSMElement.OSMElement osm10 = new OSMElement.OSMElement();
            BrailleRepresentation e10 = new BrailleRepresentation();
            GeneralProperties proper10 = new GeneralProperties();
            e10.isVisible = true;            
            e10.screenName = "A2";
            DropDownMenuItem dropDownMenu10 = new DropDownMenuItem();
            dropDownMenu10.hasChild = false;
            dropDownMenu10.hasNext = false;
            dropDownMenu10.hasPrevious = true;
            dropDownMenu10.isChild = true;
            proper10.isEnabledFiltered = false;
            dropDownMenu10.isOpen = false;
            dropDownMenu10.isVertical = true;
            proper10.valueFiltered = "Beenden";
          //  object[] otherContent10 = { UiObjectsEnum.DropDownMenu, dropDownMenu10 };
            e10.uiElementSpecialContent = dropDownMenu10;
            //c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e10.viewName = "v10";
            Rect p10 = new Rect( 0, 29, 30, 8);
            proper10.boundingRectangleFiltered = p10;
            
 //           proper10.IdGenerated = "braille123_10";
            proper10.controlTypeFiltered = "DropDownMenu";
            osm10.brailleRepresentation = e10;
            osm10.properties = proper10;
            treeOperation.updateNodes.addNodeInBrailleTree(osm10);
            #endregion

            #region Element 11 Button
            OSMElement.OSMElement osm11 = new OSMElement.OSMElement();
            BrailleRepresentation e11 = new BrailleRepresentation();
            GeneralProperties proper11 = new GeneralProperties();
            e11.screenName = "A1";

            proper11.valueFiltered = "Button 2";
            e11.viewName = "v11";
            e11.isVisible = true;
            Rect p11 = new Rect(89,12, 30, 9);
            proper11.boundingRectangleFiltered = p11;
            proper11.isEnabledFiltered = false;
    //        proper11.IdGenerated = "braille123_11";
            proper11.controlTypeFiltered = "Button";
            osm11.brailleRepresentation = e11;
            osm11.properties = proper11;
            treeOperation.updateNodes.addNodeInBrailleTree(osm11);
            #endregion

        }
        #endregion

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
           List<Object> screenshotNodes = treeOperation.searchNodes.searchProperties(subnodesOfScreen, prop);
           foreach (Object node in screenshotNodes)
           {
               OSMElement.OSMElement osmScreenshot = strategyMgr.getSpecifiedTree().GetData(node);
               strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(ref osmScreenshot);
               strategyMgr.getSpecifiedTree().SetData(node, osmScreenshot);
               grantTrees.setBrailleTree(strategyMgr.getSpecifiedTree().Root(node));
           }
        }

        public List<String> getPosibleScreens()
        {
            return treeOperation.searchNodes.getPosibleScreenNames();
        }
    }
}
