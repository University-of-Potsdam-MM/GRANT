using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager;
using OSMElement;
using StrategyManager.Interfaces;
using System.Windows.Forms;
using GRANTApplication;
using BrailleIOGuiElementRenderer;
using OSMElement.UiElements;
using System.Windows;

namespace GRANTExample
{
    public class ExampleBrailleDis
    {
        StrategyMgr strategyMgr;
        UpdateNode updateNode;
        public ExampleBrailleDis(StrategyMgr mgr)
        {
            strategyMgr = mgr;
            updateNode = new UpdateNode(strategyMgr);
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
                setDauGui(fromGuiElement);
                if (strategyMgr.getSpecifiedBrailleDisplay() == null)
                {
                    Settings settings = new Settings();
                    strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden

                    strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
                   // strategyMgr.getSpecifiedBrailleDisplay().initializedSimulator();
                    strategyMgr.getSpecifiedBrailleDisplay().initializedBrailleDisplay();
                    strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
                }

                if (strategyMgr.getOsmRelationship() == null)
                {                    
                    List<OsmRelationship<String, String>> relationship = ExampleTree.setOsmRelationship();
                    strategyMgr.setOsmRelationship(relationship);
                    
                }
                else
                {
                    if (strategyMgr.getFilteredTree() == null)
                    {
                        Console.WriteLine("Die Anwendung wurde noch nicht gefiltert - bitte 'F5' drücken");
                        return;
                    }
                    String brailleId = "braille123_6";
                    OsmRelationship<String, String> osmRelationships = strategyMgr.getOsmRelationship().Find(r => r.BrailleTree.Equals(brailleId) || r.FilteredTree.Equals(brailleId)); //TODO: was machen wir hier, wenn wir mehrere Paare bekommen? (FindFirst?)
                    if (osmRelationships != null)
                    {
                        //strategyMgr.getSpecifiedFilter().updateNodeOfFilteredTree(osmRelationships.FilteredTree);
                        updateNode.updateNodeOfFilteredTree(osmRelationships.FilteredTree);

                    OSMElement.OSMElement relatedBrailleTreeObject = strategyMgr.getSpecifiedTreeOperations().getBrailleTreeOsmElementById(osmRelationships.BrailleTree);
                    if (!relatedBrailleTreeObject.Equals(new OSMElement.OSMElement()))
                    {
                        strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(ref relatedBrailleTreeObject);
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
            if (strategyMgr.getFilteredTree() == null)
            {
                Console.WriteLine("Die Anwendung wurde noch nicht gefiltert - bitte 'F5' drücken");
                return;
            }
            String brailleId = "braille123_1";
            OsmRelationship<String, String> osmRelationships = strategyMgr.getOsmRelationship().Find(r => r.BrailleTree.Equals(brailleId) || r.FilteredTree.Equals(brailleId)); 
            {
                //strategyMgr.getSpecifiedFilter().updateNodeOfFilteredTree(osmRelationships.FilteredTree);
                updateNode.updateNodeOfFilteredTree(osmRelationships.FilteredTree);

                OSMElement.OSMElement relatedBrailleTreeObject = strategyMgr.getSpecifiedTreeOperations().getBrailleTreeOsmElementById(osmRelationships.BrailleTree);
                if (!relatedBrailleTreeObject.Equals(new OSMElement.OSMElement()))
                {
                  //  strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(relatedBrailleTreeObject.Data);
                    strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(ref relatedBrailleTreeObject);
                }
            }
        }

        public bool[,] getRendererExample()
        {
            if (strategyMgr.getFilteredTree() == null)
            {
                Console.WriteLine("Die Anwendung wurde noch nicht gefiltert - bitte 'F5' drücken");
                return null;
            }
            if (strategyMgr.getOsmRelationship() == null)
            {
                Console.WriteLine("Es sind noch keine OSM-Beziehungen vorhanden!");
                return null;
            }
            Settings settings = new Settings();
            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className);
            strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
   //         strategyMgr.getSpecifiedBrailleDisplay().initializedSimulator();
            setDauGui("nameFiltered");
            OSMElement.OSMElement osmElement = strategyMgr.getBrailleTree().Child.Data;//strategyMgr.getBrailleTree().Child.Next.Next.Next.Next.Next.Next.Data;
            return strategyMgr.getSpecifiedBrailleDisplay().getRendererExampleRepresentation(osmElement);
        }

        #region Beispielobjekte
        private void setDauGui(String fromGuiElement3)
        {
            #region Element 1
            OSMElement.OSMElement osm1 = new OSMElement.OSMElement();
            BrailleRepresentation e1 = new BrailleRepresentation();
            GeneralProperties proper1 = new GeneralProperties();
            e1.screenName = "screen1";
            e1.zoom = 1;
            e1.contrast = 120;
            e1.viewName = "v1";
           // c1.fromGuiElement = "nameFiltered";
            e1.showScrollbar = true;
            e1.isVisible = true;
            Rect p1 = new Rect(70,0,50,30);
           /* p1.Height = 30;
            p1.Width = 50;
            p1.Left = 70;
            p1.Top = 0;*/
            proper1.boundingRectangleFiltered = p1;
            
            proper1.IdGenerated = "braille123_1";
            proper1.controlTypeFiltered = "Screenshot";
            osm1.brailleRepresentation = e1;
            osm1.properties = proper1;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm1);
            #endregion

            #region Element 2
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            GeneralProperties proper2 = new GeneralProperties();
            BrailleRepresentation e2 = new BrailleRepresentation();
            e2.screenName = "screen1";
            e2.text = "Hallo 1 Hallo 2 Hallo 3 Hallo 4 Hallo 5";
          //  c2.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e2.showScrollbar = true;
            e2.viewName = "v2";
            e2.isVisible = false;
            Rect p2 = new Rect(90, 42, 29,15);
           /* p2.Height = 15;
            p2.Width = 29;
            p2.Left = 90;
            p2.Top = 42;*/
            proper2.boundingRectangleFiltered = p2;
            Padding padding = new Padding(1, 1, 1, 1);
            e2.padding = padding;
            Padding margin = new Padding(1, 1, 1, 1);
            e2.margin = margin;
            Padding boarder = new Padding(1, 1, 2, 1);
            e2.boarder = boarder;            
            
            proper2.IdGenerated = "braille123_2";
            proper2.controlTypeFiltered = "Text";
            osm2.brailleRepresentation = e2;
            osm2.properties = proper2;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm2);
            #endregion

            #region Element 3
            OSMElement.OSMElement osm3 = new OSMElement.OSMElement();
            BrailleRepresentation e3 = new BrailleRepresentation();
            GeneralProperties proper3 = new GeneralProperties();
            e3.screenName = "screen1";
            e3.showScrollbar = false;
            //   c3.text = "Start Text";
            //c3.fromGuiElement = "nameFiltered";
            e3.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e3.viewName = "v3";
            e3.isVisible = false;
            Rect p3 = new Rect(70,30,30,20);
            /*p3.Height = 20;
            p3.Width = 30;
            p3.Left = 70;
            p3.Top = 30;*/
            proper3.boundingRectangleFiltered = p3;
            
            proper3.IdGenerated = "braille123_3";
            proper3.controlTypeFiltered = "Text";
            osm3.brailleRepresentation = e3;
            osm3.properties = proper3;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm3);
            #endregion

            #region Element 4
            OSMElement.OSMElement osm4 = new OSMElement.OSMElement();
            BrailleRepresentation e4 = new BrailleRepresentation();
            GeneralProperties proper4 = new GeneralProperties();            
            e4.screenName = "screen1";
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
         /*   p4.Height = 7;
            p4.Width = 20;
            p4.Left = 40;
            p4.Top = 50;*/
            proper4.boundingRectangleFiltered = p4;
            proper4.IdGenerated = "braille123_4";
            proper4.controlTypeFiltered = "Matrix";
            osm4.brailleRepresentation = e4;
            osm4.properties = proper4;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm4);
            #endregion

            #region Element 5
            OSMElement.OSMElement osm5 = new OSMElement.OSMElement();
            BrailleRepresentation e5 = new BrailleRepresentation();
            GeneralProperties proper5 = new GeneralProperties();
            e5.screenName = "screen1";
            e5.text = "Button";
           // c5.uiElementContent = "Hallo - Button";
           // c5.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e5.viewName = "v5";
            e5.isVisible = true;
            Rect p5 = new Rect(55,30,24,9);
          /*  p5.Height = 9;
            p5.Width = 24;
            p5.Left = 55;
            p5.Top = 30;*/
            proper5.boundingRectangleFiltered = p5;
            proper5.IdGenerated = "braille123_5";
            proper5.controlTypeFiltered = "Button";
            proper5.isEnabledFiltered = true;
            osm5.brailleRepresentation = e5;
            osm5.properties = proper5;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm5);
            #endregion

            #region Element 6
            OSMElement.OSMElement osm6 = new OSMElement.OSMElement();
            BrailleRepresentation e6 = new BrailleRepresentation();
            GeneralProperties proper6 = new GeneralProperties();
            e6.screenName = "screen1";
            e6.showScrollbar = true;
           // c6.text = "Text 1 Text 2 Text 3 Text 4 Text 5 Text 6";
            //object[] otherContent6 = { UiObjectsEnum.TextBox, textBox6 };
            e6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            //c6.showScrollbar = true;
            
            e6.viewName = "v6";
            e6.isVisible = true;            
            Rect p6 = new Rect(0, 29,50,28);
           /* p6.Height = 28;
            p6.Width = 50;
            p6.Left = 0;
            p6.Top = 29;*/
           proper6.boundingRectangleFiltered = p6;
            proper6.IdGenerated = "braille123_6";
            proper6.controlTypeFiltered = "TextBox";
            proper6.isEnabledFiltered = true;
            osm6.brailleRepresentation = e6;
            osm6.properties = proper6;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm6);
            #endregion

            #region Element 7
            OSMElement.OSMElement osm7 = new OSMElement.OSMElement();
            GeneralProperties proper7 = new GeneralProperties();
            BrailleRepresentation e7 = new BrailleRepresentation();
            e7.isVisible = true;
            e7.screenName = "screen1";
            e7.text = "Text 1 Text 2 Text 3 Text 4 Text 5 Text 6";
            DropDownMenu dropDownMenu = new DropDownMenu();
            dropDownMenu.hasChild = true;
            dropDownMenu.hasNext = true;
            dropDownMenu.hasPrevious = false;
            dropDownMenu.isChild = false;
            dropDownMenu.isOpen = true;
            dropDownMenu.isVertical = true;
            e7.text = "Datei";
            // object[] otherContent7 = {UiObjectsEnum.DropDownMenu, uiElement};
            e7.uiElementSpecialContent = dropDownMenu;
            //c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e7.viewName = "v7";
            Rect p7 = new Rect(0,0,25,10);
           /* p7.Height = 10;
            p7.Width = 25;
            p7.Left = 0;
            p7.Top = 0;*/
            proper7.boundingRectangleFiltered = p7;
            proper7.isEnabledFiltered = true;
            proper7.IdGenerated = "braille123_7";
            proper7.controlTypeFiltered = "DropDownMenu";
            osm7.brailleRepresentation = e7;
            osm7.properties = proper7;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm7);
            #endregion

            #region Element 8
            OSMElement.OSMElement osm8 = new OSMElement.OSMElement();
            BrailleRepresentation e8 = new BrailleRepresentation();
            GeneralProperties proper8 = new GeneralProperties();
            e8.isVisible = true;
            e8.screenName = "screen1";
            DropDownMenu dropDownMenu8 = new DropDownMenu();
            dropDownMenu8.hasChild = false;
            dropDownMenu8.hasNext = false;
            dropDownMenu8.hasPrevious = true;
            dropDownMenu8.isChild = false;
            dropDownMenu8.isOpen = false;
            dropDownMenu8.isVertical = true;
            e8.text = "Bearbeiten";
           // object[] otherContent8 = { UiObjectsEnum.DropDownMenu, dropDownMenu8 };
            e8.uiElementSpecialContent = dropDownMenu8;
            //c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e8.viewName = "v8";
            Rect p8 = new Rect(25, 0,35,10);
          /*  p8.Height = 10;
            p8.Width = 35;
            p8.Left = 25;
            p8.Top = 0;*/
            proper8.boundingRectangleFiltered = p8;
            proper8.IdGenerated = "braille123_8";
            proper8.controlTypeFiltered = "DropDownMenu";
            proper8.isEnabledFiltered = true;
            osm8.brailleRepresentation = e8;
            osm8.properties = proper8;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm8);
            #endregion

            #region Element 9
            OSMElement.OSMElement osm9 = new OSMElement.OSMElement();
            BrailleRepresentation e9 = new BrailleRepresentation();
            GeneralProperties proper9 = new GeneralProperties();
            e9.isVisible = true;
            e9.screenName = "screen1";
            DropDownMenu dropDownMenu9 = new DropDownMenu();
            dropDownMenu9.hasChild = true;
            dropDownMenu9.hasNext = true;
            dropDownMenu9.hasPrevious = false;
            dropDownMenu9.isChild = true;
            dropDownMenu9.isOpen = false;
            dropDownMenu9.isVertical = true;
            e9.text = "Neu";
           // object[] otherContent9 = { UiObjectsEnum.DropDownMenu, dropDownMenu9 };
            e9.uiElementSpecialContent = dropDownMenu9;
            //c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e9.viewName = "v9";
            Rect p9 = new Rect(0, 11, 30 , 8);
            /*p9.height = 8;
            p9.width = 30;
            p9.left = 0;
            p9.top = 11;*/
            proper9.boundingRectangleFiltered = p9;
            proper9.isEnabledFiltered = true;
            proper9.IdGenerated = "braille123_9";
            proper9.controlTypeFiltered = "DropDownMenu";
            osm9.brailleRepresentation = e9;
            osm9.properties = proper9;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm9);
            #endregion

            #region Element 10
            OSMElement.OSMElement osm10 = new OSMElement.OSMElement();
            BrailleRepresentation e10 = new BrailleRepresentation();
            GeneralProperties proper10 = new GeneralProperties();
            e10.isVisible = true;            
            e10.screenName = "screen1";
            DropDownMenu dropDownMenu10 = new DropDownMenu();
            dropDownMenu10.hasChild = false;
            dropDownMenu10.hasNext = false;
            dropDownMenu10.hasPrevious = true;
            dropDownMenu10.isChild = true;
            proper10.isEnabledFiltered = false;
            dropDownMenu10.isOpen = false;
            dropDownMenu10.isVertical = true;
            e10.text = "Beenden";
          //  object[] otherContent10 = { UiObjectsEnum.DropDownMenu, dropDownMenu10 };
            e10.uiElementSpecialContent = dropDownMenu10;
            //c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e10.viewName = "v10";
            Rect p10 = new Rect( 0, 19, 30, 8);
            /*p10.height = 8;
            p10.width = 30;
            p10.left = 0;
            p10.top = 19;*/
            proper10.boundingRectangleFiltered = p10;
            
            proper10.IdGenerated = "braille123_10";
            proper10.controlTypeFiltered = "DropDownMenu";
            osm10.brailleRepresentation = e10;
            osm10.properties = proper10;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm10);
            #endregion

            #region Element 11
            OSMElement.OSMElement osm11 = new OSMElement.OSMElement();
            BrailleRepresentation e11 = new BrailleRepresentation();
            GeneralProperties proper11 = new GeneralProperties();
            e11.screenName = "screen1";
            
            e11.text = "Button 2";
            e11.viewName = "v11";
            e11.isVisible = true;
            Rect p11 = new Rect(81,30, 30, 9);
            /* p11.height = 9;
            p11.width = 30;
            p11.left = 81;
            p11.top = 30; */
            proper11.boundingRectangleFiltered = p11;
            proper11.isEnabledFiltered = false;
            proper11.IdGenerated = "braille123_11";
            proper11.controlTypeFiltered = "Button";
            osm11.brailleRepresentation = e11;
            osm11.properties = proper11;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm11);
            #endregion

        }





        #endregion
    }
}
