using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager;
using GRANTManager.TreeOperations;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace FilteredTreeTest
{
    [TestClass]
    public class UnitTestUpdateNode
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;
        private String applicationName = "calc";
        private String applicationPathName = @"C:\Windows\system32\calc.exe";
        private String idTextNodeCalc = "F6BC5E5ADD3B17478743923733E4BC8C";
        private String treePathUia2;

        [TestInitialize]
        public void Initialize()
        {
            strategyMgr = new StrategyManager();
            grantTrees = new GeneratedGrantTrees();
            Settings settings = new Settings();
            treeOperation = new TreeOperation(strategyMgr, grantTrees);
            List<GRANTManager.Strategy> posibleOS = settings.getPossibleOperationSystems();

            strategyMgr.setSpecifiedTree(settings.getPossibleTrees()[0].className);
            strategyMgr.setSpecifiedEventStrategy(settings.getPossibleEventManager()[0].className);
            strategyMgr.setSpecifiedFilter(Settings.getPossibleFilters()[0].className);
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);
            String projectPath;
            projectPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SavedTrees");
            treePathUia2 = System.IO.Path.Combine(projectPath, "filteredTree_Rechner.grant");
        }


        [TestMethod]
        public void ChangeFilterForNodeTest()
        {
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(applicationName, applicationPathName);
            
            treeOperation.updateNodes.filterSubtreeWithCurrentFilterStrtegy(idTextNodeCalc);
            List<Strategy> filterStrategys = Settings.getPossibleFilters();
            Type currentFilter = strategyMgr.getSpecifiedFilter().GetType();
            int indexNewFilterStrategy = 0;
            foreach (Strategy strategy in filterStrategys)
            {
                if (!strategy.className.Contains(currentFilter.FullName) && !strategy.className.ToLower().Contains("java")) //kann raus wenn Java-Filter vorhanden ist
                {
                    break;
                }
                indexNewFilterStrategy++;
            }
            strategyMgr.setSpecifiedFilter(filterStrategys[indexNewFilterStrategy].className);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            treeOperation.updateNodes.filterSubtreeWithCurrentFilterStrtegy(idTextNodeCalc);
            OSMElement.OSMElement textNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(idTextNodeCalc);
            if (textNode.properties.grantFilterStrategy == null || !filterStrategys[indexNewFilterStrategy].userName.Equals(textNode.properties.grantFilterStrategy)) { Assert.Fail("Die Filterstrategie wurde für den Knoten (richtig) nicht geändert!\nBtrachtet wurde der Knoten  {0}\nDer filter hätte '{1}' sein sollen", textNode, filterStrategys[indexNewFilterStrategy].userName); }
        }

        /// <summary>
        /// Ändert die Anzeige bei Calc und ließt den Teilbaum neu ein
        /// </summary>
        [TestMethod]
        public void FilterSubtreeTest()
        {
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(applicationName, applicationPathName);

            treeOperation.updateNodes.filterSubtreeWithCurrentFilterStrtegy(idTextNodeCalc);
            
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(applicationName);                
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            //Send Key -> Inhalt im Textfeld soll sich ändern
            System.Windows.Forms.SendKeys.SendWait("{ESC}");
            System.Windows.Forms.SendKeys.SendWait("24");
            treeOperation.updateNodes.filterSubtreeWithCurrentFilterStrtegy(idTextNodeCalc);
            OSMElement.OSMElement textNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(idTextNodeCalc);
            if (!textNode.properties.nameFiltered.Equals("24")) { Assert.Fail("Der Knoten wurde nicht richtig geändert oder geupdatet!\nBetrachteter Knoten:\n{0}", textNode); }
        }

        /// <summary>
        /// Ändert die Anzeige bei Calc und prüft, ob die Änderung ausgelesen wird
        /// </summary>
        [TestMethod]
        public void UpdateNodeTest()
        {
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(applicationName, applicationPathName);

            treeOperation.updateNodes.filterSubtreeWithCurrentFilterStrtegy(idTextNodeCalc);

            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(applicationName);
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            //Send Key -> Inhalt im Textfeld soll sich ändern
            System.Windows.Forms.SendKeys.SendWait("{ESC}");
            System.Windows.Forms.SendKeys.SendWait("42");
            UpdateNodes up = new UpdateNodes(strategyMgr, grantTrees, treeOperation);
            up.filteredNodeElementOfApplication(idTextNodeCalc);
            OSMElement.OSMElement textNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(idTextNodeCalc);
            if (!textNode.properties.nameFiltered.Equals("42")) { Assert.Fail("Der Knoten wurde nicht richtig geändert oder geupdatet!\nBetrachteter Knoten:\n{0}", textNode); }
        }

        [TestMethod]
        public void UpdateNodeTest_grantFilterstrategyChildren()
        {
            String idPaneNode = "417F2ACC323396E993B4DC2AD2515D5E";
            guiFuctions.loadGrantProject(treePathUia2);
            OSMElement.OSMElement paneNodeOld = treeOperation.searchNodes.getFilteredTreeOsmElementById(idPaneNode).DeepCopy();

            UpdateNodes up = new UpdateNodes(strategyMgr, grantTrees, treeOperation);
            up.filteredNodeElementOfApplication(idPaneNode);
            OSMElement.OSMElement paneNodeNew = treeOperation.searchNodes.getFilteredTreeOsmElementById(idPaneNode);
            Assert.AreEqual(paneNodeOld.properties.grantFilterStrategy, paneNodeNew.properties.grantFilterStrategy);
            //Assert.AreEqual(paneNodeOld.properties.grantFilterStrategiesChildren, paneNodeNew.properties.grantFilterStrategiesChildren);
            if (!paneNodeOld.properties.grantFilterStrategiesChildren.All(p => paneNodeNew.properties.grantFilterStrategiesChildren.Contains(p))) // check whether 'osmParent.properties.grantFilterStrategiesChildren' is a subset of filterStrategiesChildren
            {
                Assert.Fail("The update of the 'grantFilterStrategiesChildren' wasn't correct!");
            }
        }

        [TestMethod]
        public void filteredTree_Children_test()
        {
            /*
             * chekcs whether the children was updatet --> change the textfeld but the textfeld isn't part of the children nodes (it's a descendants)
             * 
             */
            guiFuctions.loadGrantProject(treePathUia2);
            String id_WindowNode = "29567A6D5962C2D9DD9E359AECE86E39"; // = root
            String id_Pane = "417F2ACC323396E993B4DC2AD2515D5E";
            String id_titlebar = "41B73937D557B2AB5DA85001ABF0C423";
            String id_text = "F6BC5E5ADD3B17478743923733E4BC8C";
            OSMElement.OSMElement osm_textNodeOld = treeOperation.searchNodes.getFilteredTreeOsmElementById(id_text).DeepCopy();
            OSMElement.OSMElement osm_paneNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(id_Pane).DeepCopy();
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(applicationName);
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            System.Windows.Forms.SendKeys.SendWait("{ESC}");
            System.Windows.Forms.SendKeys.SendWait(osm_textNodeOld.properties.nameFiltered + "{+}" + "3" + "{ENTER}");
            treeOperation.updateNodes.filteredTree(id_WindowNode, TreeScopeEnum.Children);
            OSMElement.OSMElement osm_textNodeNew = treeOperation.searchNodes.getFilteredTreeOsmElementById(id_text);
            Assert.IsTrue(osm_textNodeOld.Equals(osm_textNodeNew), "this node shouldn't updated");
            OSMElement.OSMElement osm_paneNodeNew = treeOperation.searchNodes.getFilteredTreeOsmElementById(id_Pane);
            OSMElement.OSMElement osm_TitlebarNodeNew = treeOperation.searchNodes.getFilteredTreeOsmElementById(id_titlebar);
            List<String> grantFSList = new List<string>() { "UIA", "UIA2" };
            Assert.AreEqual(2, osm_paneNodeNew.properties.grantFilterStrategiesChildren.Count());
            Assert.AreEqual(1, osm_TitlebarNodeNew.properties.grantFilterStrategiesChildren.Count());
            if (!osm_paneNodeNew.properties.grantFilterStrategiesChildren.Contains("UIA")) { Assert.Fail(); }
            if (!osm_paneNodeNew.properties.grantFilterStrategiesChildren.Contains("UIA2")) { Assert.Fail(); }
            if (!osm_TitlebarNodeNew.properties.grantFilterStrategiesChildren.Contains("UIA")) { Assert.Fail(); }
        }


        [TestMethod]
        public void filteredTree_Element_test()
        {
            /*
             * chekcs whether the element was updatet --> open "Bearbeiten"
             * 
             */
            guiFuctions.loadGrantProject(treePathUia2);
            String id_edit = "C8B18DFACD2F1C1982BA19264FB6BC77";
            Object treeLoaded = grantTrees.filteredTree.DeepCopy();
            Object editNodeOld = treeOperation.searchNodes.getNode(id_edit, grantTrees.filteredTree).DeepCopy();
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(applicationName);
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            System.Windows.Forms.SendKeys.SendWait("%B"); // => Alt + B => opened the edit ("Bearbeiten") menu
            treeOperation.updateNodes.filteredTree(id_edit, TreeScopeEnum.Element);
            System.Windows.Forms.SendKeys.SendWait("{ESC}");
            Object editNodeNew = treeOperation.searchNodes.getNode(id_edit, grantTrees.filteredTree);
            Assert.AreEqual(strategyMgr.getSpecifiedTree().Count(treeLoaded), strategyMgr.getSpecifiedTree().Count(grantTrees.filteredTree));
            Assert.IsFalse(strategyMgr.getSpecifiedTree().Equals(editNodeOld, editNodeNew)); // hasKeyboardFocusFiltered has changed from "false" to "true"
            Assert.AreEqual(strategyMgr.getSpecifiedTree().Count(treeLoaded), strategyMgr.getSpecifiedTree().Count(grantTrees.filteredTree), "the numbers of nodes should be the same like before because only the 'edit' node was updated"); 
        }

        [TestMethod]
        public void filteredTree_Application_changeModus_test()
        {
            /*
             * chekcs whether the descendants was updatet --> change mode --> new nodes should be added
             * 
             */
            guiFuctions.loadGrantProject(treePathUia2);
            String id_edit = "C8B18DFACD2F1C1982BA19264FB6BC77";
            String id_Pane = "417F2ACC323396E993B4DC2AD2515D5E";
            Object treeLoaded = grantTrees.filteredTree.DeepCopy();
            Object paneNodeOld = treeOperation.searchNodes.getNode(id_Pane, grantTrees.filteredTree).DeepCopy();
            Object editNodeOld = treeOperation.searchNodes.getNode(id_edit, grantTrees.filteredTree).DeepCopy();
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(applicationName);
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            System.Windows.Forms.SendKeys.SendWait("^u"); // => Crlt + u => change calc modus
            System.Threading.Thread.Sleep(200);
            treeOperation.updateNodes.filteredTree(id_Pane, TreeScopeEnum.Application);
            System.Windows.Forms.SendKeys.SendWait("^{F4}"); // normal modus
            Object editNodeNew = treeOperation.searchNodes.getNode(id_edit, grantTrees.filteredTree);
            Object paneNodeNew = treeOperation.searchNodes.getNode(id_Pane, grantTrees.filteredTree);
            Assert.AreNotEqual(strategyMgr.getSpecifiedTree().Count(treeLoaded), strategyMgr.getSpecifiedTree().Count(grantTrees.filteredTree),  "some nodes should be added"); 
        }

        [TestMethod]
        public void filteredTree_Descendants_changeModus_test()
        {
            /*
             * chekcs whether the descendants was updatet --> change mode + select "Ansicht" (MenuBar) --> new nodes should be added But the MenuBar shouldn't filtered
             * 
             */
            guiFuctions.loadGrantProject(treePathUia2);
            String id_WindowNode = "29567A6D5962C2D9DD9E359AECE86E39"; // = root
            String id_menuBar = "6E62984FEFA9C92727332C5B6D08820F";
            String id_Pane = "417F2ACC323396E993B4DC2AD2515D5E";
            Object treeLoaded = grantTrees.filteredTree.DeepCopy();
            Object paneNodeOld = treeOperation.searchNodes.getNode(id_Pane, grantTrees.filteredTree).DeepCopy();
            Object menuBarNodeOld = treeOperation.searchNodes.getNode(id_menuBar, grantTrees.filteredTree).DeepCopy();
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(applicationName);
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            System.Windows.Forms.SendKeys.SendWait("^u"); // => Crlt + u => change calc modus
            System.Threading.Thread.Sleep(200);
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            System.Windows.Forms.SendKeys.SendWait("%"); // => Alt => focus menu
            System.Threading.Thread.Sleep(200);
            treeOperation.updateNodes.filteredTree(id_Pane, TreeScopeEnum.Descendants);
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            System.Windows.Forms.SendKeys.SendWait("^{F4}"); // normal modus
            Object menuBarNodeNew = treeOperation.searchNodes.getNode(id_menuBar, grantTrees.filteredTree);
            Object paneNodeNew = treeOperation.searchNodes.getNode(id_Pane, grantTrees.filteredTree);
            Assert.AreNotEqual(strategyMgr.getSpecifiedTree().Count(treeLoaded), strategyMgr.getSpecifiedTree().Count(grantTrees.filteredTree), "some nodes should be added");
            //Assert.AreEqual(strategyMgr.getSpecifiedTree().Count(treeLoaded), strategyMgr.getSpecifiedTree().Count(grantTrees.filteredTree), "the numbers of nodes should be the same like before because only the 'edit' node was updated");
            Assert.IsTrue( strategyMgr.getSpecifiedTree().Equals(menuBarNodeOld, menuBarNodeNew), "The MenuBar branch wasn't filtered, so there shouldn't be changes.");
        }

        [TestMethod]
        public void filteredTree_Descendants2_test()
        {
            /*
             * chekcs whether the descendants was updatet --> focus "Bearbeiten" --> new nodes should be added
             * 
             */
            guiFuctions.loadGrantProject(treePathUia2);
            String id_WindowNode = "29567A6D5962C2D9DD9E359AECE86E39"; // = root
            String id_edit = "C8B18DFACD2F1C1982BA19264FB6BC77";
            String id_menuBar = "6E62984FEFA9C92727332C5B6D08820F";
            Object treeLoaded = grantTrees.filteredTree.DeepCopy();
            Object editNodeOld = treeOperation.searchNodes.getNode(id_edit, grantTrees.filteredTree).DeepCopy();
            Object menuBarNodeOld = treeOperation.searchNodes.getNode(id_menuBar, grantTrees.filteredTree).DeepCopy();
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(applicationName);
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            System.Windows.Forms.SendKeys.SendWait("%");
            System.Windows.Forms.SendKeys.SendWait("{RIGHT}"); // => Alt + arrow right => focus the edit ("Bearbeiten") menu
            System.Threading.Thread.Sleep(200);
            treeOperation.updateNodes.filteredTree(id_WindowNode, TreeScopeEnum.Descendants);
            System.Windows.Forms.SendKeys.SendWait("{ESC}");
            Object editNodeNew = treeOperation.searchNodes.getNode(id_edit, grantTrees.filteredTree);
            Object menuBarNodeNew = treeOperation.searchNodes.getNode(id_menuBar, grantTrees.filteredTree);
            Assert.AreEqual(strategyMgr.getSpecifiedTree().Count(treeLoaded), strategyMgr.getSpecifiedTree().Count(grantTrees.filteredTree));
            Assert.IsFalse(strategyMgr.getSpecifiedTree().Equals(editNodeOld, editNodeNew)); // hasKeyboardFocusFiltered has changed from "false" to "true"
            Assert.AreEqual(strategyMgr.getSpecifiedTree().Count(treeLoaded), strategyMgr.getSpecifiedTree().Count(grantTrees.filteredTree), "the numbers of nodes should be the same like before because only the 'edit' node was updated");
            Assert.IsFalse(strategyMgr.getSpecifiedTree().Equals(menuBarNodeOld, menuBarNodeNew), "The MenuBar branch was filtered, so there should be changes.");
        }

        [TestMethod]
        public void filteredTree_Siblings_test()
        {
            /*
             * 1. change "Textfield"
             * 2. change modus
             * 3. filter siblings of "Textfild"
             * 4. new nodes from new modus should be added BUT textfilt contend shouldn't be changed
             */
            String id_text_filtered = "F6BC5E5ADD3B17478743923733E4BC8C";
            String idFiltered_VerlaufNode_filtered = "ED842B72B012E86CE468B73FA1378361";
            guiFuctions.loadGrantProject(treePathUia2);
            Object treeCopy = grantTrees.filteredTree.DeepCopy();
            OSMElement.OSMElement osm_editNode_old = treeOperation.searchNodes.getFilteredTreeOsmElementById(id_text_filtered).DeepCopy();
            OSMElement.OSMElement osm_VerlaufNode_old = treeOperation.searchNodes.getFilteredTreeOsmElementById(idFiltered_VerlaufNode_filtered).DeepCopy();
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.processName);
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            System.Windows.Forms.SendKeys.SendWait("{ESC}");
            System.Windows.Forms.SendKeys.SendWait(osm_editNode_old.properties.nameFiltered + "{+}" + osm_editNode_old.properties.nameFiltered + "1");
            System.Windows.Forms.SendKeys.SendWait("^u"); // => Crlt + u => change calc modus
            treeOperation.updateNodes.filteredTree(id_text_filtered, TreeScopeEnum.Sibling);
            Assert.IsFalse(strategyMgr.getSpecifiedTree().Equals(treeCopy, grantTrees.filteredTree), "The tree shold be changed.");
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(treeCopy) < strategyMgr.getSpecifiedTree().Count(grantTrees.filteredTree), "Some nodes should be added for the 'new' mode!");
            OSMElement.OSMElement osm_editNode_new = treeOperation.searchNodes.getFilteredTreeOsmElementById(id_text_filtered);
            OSMElement.OSMElement osm_VerlaufNode_new = treeOperation.searchNodes.getFilteredTreeOsmElementById(idFiltered_VerlaufNode_filtered);
            Assert.IsFalse(osm_VerlaufNode_old.Equals(osm_VerlaufNode_new));
            Assert.IsTrue(osm_editNode_old.Equals(osm_editNode_new));
        }

        [TestMethod]
        public void filteredTree_Siblings_checksFilterstrategy_test()
        {
            String id_text_filtered = "F6BC5E5ADD3B17478743923733E4BC8C";
            guiFuctions.loadGrantProject(treePathUia2);
            Object treeCopy = grantTrees.filteredTree.DeepCopy();
            OSMElement.OSMElement osm_editNode_old = treeOperation.searchNodes.getFilteredTreeOsmElementById(id_text_filtered).DeepCopy();
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.processName);
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            System.Windows.Forms.SendKeys.SendWait("{ESC}");
            System.Windows.Forms.SendKeys.SendWait(osm_editNode_old.properties.nameFiltered + "{+}" + osm_editNode_old.properties.nameFiltered + "1");
            System.Windows.Forms.SendKeys.SendWait("^u"); // => Crlt + u => change calc modus
            System.Threading.Thread.Sleep(200);
            treeOperation.updateNodes.filteredTree(id_text_filtered, TreeScopeEnum.Sibling);
            Assert.IsFalse(strategyMgr.getSpecifiedTree().Equals(treeCopy, grantTrees.filteredTree), "The tree shold be changed.");
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(treeCopy) < strategyMgr.getSpecifiedTree().Count(grantTrees.filteredTree), "Some nodes should be added for the 'new' mode!");

            foreach(Object nodeObject_old in strategyMgr.getSpecifiedTree().AllNodes(treeCopy))
            {
                OSMElement.OSMElement osm_old = strategyMgr.getSpecifiedTree().GetData(nodeObject_old);
                OSMElement.OSMElement osm_new = treeOperation.searchNodes.getFilteredTreeOsmElementById(osm_old.properties.IdGenerated);
                if(osm_new != null && !osm_new.Equals(new OSMElement.OSMElement()))
                {
                    Assert.AreEqual(osm_old.properties.grantFilterStrategy, osm_new.properties.grantFilterStrategy);
                    if (osm_old.properties.grantFilterStrategiesChildren != null)
                    {
                        Assert.IsTrue(osm_old.properties.grantFilterStrategiesChildren.All(osm_new.properties.grantFilterStrategiesChildren.Contains), "Both 'grantFilterStrategiesChildren' should have the same values! The node withe the id '" + osm_new.properties.IdGenerated + "' hasn't the correct 'grantFilterStrategiesChildren'!"); // Assert.AreEqual(osm_old.properties.grantFilterStrategiesChildren, osm_new.properties.grantFilterStrategiesChildren);
                    }
                }

            }
        }

        [TestCleanup]
             public void clean()
             {
                 if(grantTrees.filteredTree != null)
                 {
                     IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(applicationName);
                     strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
                     System.Windows.Forms.SendKeys.SendWait("{ESC}");
                     System.Threading.Thread.Sleep(1000);
                     System.Windows.Forms.SendKeys.SendWait("^{F4}"); // normal modus
                 }
             }
    }
}
