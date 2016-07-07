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

namespace GRANTExample
{
    public class ExampleBrailleDis
    {
        StrategyMgr strategyMgr;
        public ExampleBrailleDis(StrategyMgr mgr)
        {
            strategyMgr = mgr;
        }
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
                    strategyMgr.getSpecifiedBrailleDisplay().initializedSimulator();
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
                   String  brailleId =  "braille123_6";
                    OsmRelationship<String, String> osmRelationships = strategyMgr.getOsmRelationship().Find(r => r.BrailleTree.Equals(brailleId) || r.FilteredTree.Equals(brailleId)); //TODO: was machen wir hier, wenn wir mehrere Paare bekommen? (FindFirst?)
                    if (osmRelationships != null)
                    {
                        strategyMgr.getSpecifiedFilter().updateNodeOfFilteredTree(osmRelationships.FilteredTree);


                    ITreeStrategy<OSMElement.OSMElement> relatedBrailleTreeObject = strategyMgr.getSpecifiedTreeOperations().getAssociatedNode(osmRelationships.BrailleTree, strategyMgr.getBrailleTree());
                    if (relatedBrailleTreeObject != null)
                    {
                        strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(relatedBrailleTreeObject.Data);
                        strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(relatedBrailleTreeObject.Data);
                    }
                }
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: '{0}'", ex);
            }
            
        }

        #region Beispielobjekte
        private void setDauGui(String fromGuiElement3)
        {
            #region Element 1
            OSMElement.OSMElement osm1 = new OSMElement.OSMElement();
            BrailleRepresentation e1 = new BrailleRepresentation();
            e1.screenName = "screen1";
            Content c1 = new Content();
            c1.text = "Hallo";
            e1.viewName = "v1";
           // c1.fromGuiElement = "nameFiltered";
            c1.showScrollbar = true;
            e1.content = c1;
            Position p1 = new Position();
            p1.height = 8;
            p1.width = 23;
            p1.left = 0;
            p1.top = 0;
            e1.position = p1;
            GeneralProperties proper1 = new GeneralProperties();
            proper1.IdGenerated = "braille123_1";
            osm1.brailleRepresentation = e1;
            osm1.properties = proper1;
           // strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm1);
            #endregion

            #region Element 2
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            BrailleRepresentation e2 = new BrailleRepresentation();
            e2.screenName = "screen1";
            Content c2 = new Content();
            c2.text = "Hallo 2";
            e2.viewName = "v2";
            c2.showScrollbar = true;
            e2.content = c2;
            Position p2 = new Position();
            p2.height = 12;
            p2.width = 29;
            p2.left = 0;
            p2.top = 16;

            Padding padding = new Padding(1, 1, 1, 1);
            p2.padding = padding;
            Padding margin = new Padding(1, 1, 1, 1);
            p2.margin = margin;
            Padding boarder = new Padding(1, 1, 2, 1);
            p2.boarder = boarder;
            e2.position = p2;
            GeneralProperties proper2 = new GeneralProperties();
            proper2.IdGenerated = "braille123_2";
            osm2.brailleRepresentation = e2;
            osm2.properties = proper2;
           // strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm2);
            #endregion

            #region Element 3
            OSMElement.OSMElement osm3 = new OSMElement.OSMElement();
            BrailleRepresentation e3 = new BrailleRepresentation();
            e3.screenName = "screen1";
            Content c3 = new Content();
            //   c3.text = "Start Text";
            //c3.fromGuiElement = "nameFiltered";
            c3.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e3.viewName = "v3";
            e3.content = c3;
            e3.isVisible = false;
            Position p3 = new Position();
            p3.height = 20;
            p3.width = 30;
            p3.left = 70;
            p3.top = 30;
            e3.position = p3;
            GeneralProperties proper3 = new GeneralProperties();
            proper3.IdGenerated = "braille123_3";
            osm3.brailleRepresentation = e3;
            osm3.properties = proper3;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm3);
            #endregion

            #region Element 4
            OSMElement.OSMElement osm4 = new OSMElement.OSMElement();
            BrailleRepresentation e4 = new BrailleRepresentation();
            e4.screenName = "screen1";
            Content c4 = new Content();
            c4.matrix = new bool[,] { 
                {false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false},
                {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false},
                {false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false},
            };
            e4.viewName = "v4";
            e4.content = c4;
            Position p4 = new Position();
            p4.height = 7;
            p4.width = 20;
            p4.left = 40;
            p4.top = 50;
            e4.position = p4;
            GeneralProperties proper4 = new GeneralProperties();
            proper4.IdGenerated = "braille123_4";
            osm4.brailleRepresentation = e4;
            osm4.properties = proper4;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm4);
            #endregion

            #region Element 5
            OSMElement.OSMElement osm5 = new OSMElement.OSMElement();
            BrailleRepresentation e5 = new BrailleRepresentation();
            e5.screenName = "screen1";
            Content c5 = new Content();
            c5.text = "Button";
            BrailleIOGuiElementRenderer.Button button5 = new BrailleIOGuiElementRenderer.Button();
            button5.isDeactiveted = false;
            button5.text = "Button";
            object[] otherContent = {UiObjectsEnum.Button, button5};
            c5.otherContent = otherContent;
           // c5.otherContent = "Hallo - Button";
           // c5.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e5.viewName = "v5";
            e5.content = c5;
            e5.isVisible = true;
            Position p5 = new Position();
            p5.height = 9;
            p5.width = 24;
            p5.left = 55;
            p5.top = 30;
            e5.position = p5;
            GeneralProperties proper5 = new GeneralProperties();
            proper5.IdGenerated = "braille123_5";
            osm5.brailleRepresentation = e5;
            osm5.properties = proper5;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm5);
            #endregion

            #region Element 6
            OSMElement.OSMElement osm6 = new OSMElement.OSMElement();
            BrailleRepresentation e6 = new BrailleRepresentation();
            e6.screenName = "screen1";
            Content c6 = new Content();
           // c6.text = "Text 1 Text 2 Text 3 Text 4 Text 5 Text 6";
            c6.otherContent = UiObjectsEnum.TextBox;
            c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e6.viewName = "v6";
            e6.content = c6;
            e6.isVisible = true;
            Position p6 = new Position();
            p6.height = 28;
            p6.width = 50;
            p6.left = 0;
            p6.top = 29;
            e6.position = p6;
            GeneralProperties proper6 = new GeneralProperties();
            proper6.IdGenerated = "braille123_6";
            osm6.brailleRepresentation = e6;
            osm6.properties = proper6;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm6);
            #endregion

            #region Element 7
            OSMElement.OSMElement osm7 = new OSMElement.OSMElement();
            BrailleRepresentation e7 = new BrailleRepresentation();
            e7.screenName = "screen1";
            e7.isVisible = true;
            Content c7 = new Content();
            c7.text = "Text 1 Text 2 Text 3 Text 4 Text 5 Text 6";
            DropDownMenu dropDownMenu = new DropDownMenu();
            dropDownMenu.hasChild = true;
            dropDownMenu.hasNext = true;
            dropDownMenu.hasPrevious = false;
            dropDownMenu.isChild = false;
            dropDownMenu.isDeactiveted = false;
            dropDownMenu.isOpen = true;
            dropDownMenu.isVertical = true;
            dropDownMenu.text = "Datei";
             object[] otherContent7 = {UiObjectsEnum.DropDownMenu, dropDownMenu};
            c7.otherContent = otherContent7;
            //c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e7.viewName = "v7";
            e7.content = c7;
            Position p7 = new Position();
            p7.height = 10;
            p7.width = 25;
            p7.left = 0;
            p7.top = 0;
            e7.position = p7;
            GeneralProperties proper7 = new GeneralProperties();
            proper7.IdGenerated = "braille123_7";
            osm7.brailleRepresentation = e7;
            osm7.properties = proper7;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm7);
            #endregion

            #region Element 8
            OSMElement.OSMElement osm8 = new OSMElement.OSMElement();
            BrailleRepresentation e8 = new BrailleRepresentation();
            e8.screenName = "screen1";
            e8.isVisible = true;
            Content c8 = new Content();
            c8.text = "Text 1 Text 2 Text 3 Text 4 Text 5 Text 6";
            DropDownMenu dropDownMenu8 = new DropDownMenu();
            dropDownMenu8.hasChild = false;
            dropDownMenu8.hasNext = false;
            dropDownMenu8.hasPrevious = true;
            dropDownMenu8.isChild = false;
            dropDownMenu8.isDeactiveted = false;
            dropDownMenu8.isOpen = false;
            dropDownMenu8.isVertical = true;
            dropDownMenu8.text = "Bearbeiten";
            object[] otherContent8 = { UiObjectsEnum.DropDownMenu, dropDownMenu8 };
            c8.otherContent = otherContent8;
            //c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e8.viewName = "v8";
            e8.content = c8;
            Position p8 = new Position();
            p8.height = 10;
            p8.width = 35;
            p8.left = 25;
            p8.top = 0;
            e8.position = p8;
            GeneralProperties proper8 = new GeneralProperties();
            proper8.IdGenerated = "braille123_8";
            osm8.brailleRepresentation = e8;
            osm8.properties = proper8;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm8);
            #endregion

            #region Element 9
            OSMElement.OSMElement osm9 = new OSMElement.OSMElement();
            BrailleRepresentation e9 = new BrailleRepresentation();
            e9.screenName = "screen1";
            e9.isVisible = true;
            Content c9 = new Content();
            c9.text = "Text 1 Text 2 Text 3 Text 4 Text 5 Text 6";
            DropDownMenu dropDownMenu9 = new DropDownMenu();
            dropDownMenu9.hasChild = true;
            dropDownMenu9.hasNext = true;
            dropDownMenu9.hasPrevious = false;
            dropDownMenu9.isChild = true;
            dropDownMenu9.isDeactiveted = false;
            dropDownMenu9.isOpen = false;
            dropDownMenu9.isVertical = true;
            dropDownMenu9.text = "Neu";
            object[] otherContent9 = { UiObjectsEnum.DropDownMenu, dropDownMenu9 };
            c9.otherContent = otherContent9;
            //c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e9.viewName = "v9";
            e9.content = c9;
            Position p9 = new Position();
            p9.height = 8;
            p9.width = 30;
            p9.left = 0;
            p9.top = 11;
            e9.position = p9;
            GeneralProperties proper9 = new GeneralProperties();
            proper9.IdGenerated = "braille123_9";
            osm9.brailleRepresentation = e9;
            osm9.properties = proper9;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm9);
            #endregion

            #region Element 10
            OSMElement.OSMElement osm10 = new OSMElement.OSMElement();
            BrailleRepresentation e10 = new BrailleRepresentation();
            e10.screenName = "screen1";
            e10.isVisible = true;
            Content c10 = new Content();
            DropDownMenu dropDownMenu10 = new DropDownMenu();
            dropDownMenu10.hasChild = false;
            dropDownMenu10.hasNext = false;
            dropDownMenu10.hasPrevious = true;
            dropDownMenu10.isChild = true;
            dropDownMenu10.isDeactiveted = true;
            dropDownMenu10.isOpen = false;
            dropDownMenu10.isVertical = true;
            dropDownMenu10.text = "Beenden";
            object[] otherContent10 = { UiObjectsEnum.DropDownMenu, dropDownMenu10 };
            c10.otherContent = otherContent10;
            //c6.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e10.viewName = "v10";
            e10.content = c10;
            Position p10 = new Position();
            p10.height = 8;
            p10.width = 30;
            p10.left = 0;
            p10.top = 19;
            /*p10.height = 10;
            p10.width = 30;
            p10.left = 31;
            p10.top = 11;*/
            e10.position = p10;
            GeneralProperties proper10 = new GeneralProperties();
            proper10.IdGenerated = "braille123_10";
            osm10.brailleRepresentation = e10;
            osm10.properties = proper10;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm10);
            #endregion

            #region Element 11
            OSMElement.OSMElement osm11 = new OSMElement.OSMElement();
            BrailleRepresentation e11 = new BrailleRepresentation();
            e11.screenName = "screen1";
            Content c11 = new Content();
            c11.text = "Button 2";
            BrailleIOGuiElementRenderer.Button button11 = new BrailleIOGuiElementRenderer.Button();
            button11.isDeactiveted = true;
            button11.text = "Button 2";
            object[] otherContent11 = { UiObjectsEnum.Button, button11 };
            c11.otherContent = otherContent11;
            // c5.otherContent = "Hallo - Button";
            // c5.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e11.viewName = "v11";
            e11.content = c11;
            e11.isVisible = true;
            Position p11 = new Position();
            p11.height = 9;
            p11.width = 30;
            p11.left = 81;
            p11.top = 30;
            e11.position = p11;
            GeneralProperties proper11 = new GeneralProperties();
            proper11.IdGenerated = "braille123_11";
            osm11.brailleRepresentation = e11;
            osm11.properties = proper11;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm11);
            #endregion

        }



        #endregion
    }
}
