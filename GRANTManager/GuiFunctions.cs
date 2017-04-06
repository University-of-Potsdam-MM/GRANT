﻿using System;
using System.Threading;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using GRANTManager.TreeOperations;

using GRANTManager.Interfaces;
using OSMElement;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace GRANTManager
{

    public class GuiFunctions
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        private String filteredTreeSavedName = "filteredTree.xml";
        private String brailleTreeSavedName = "brailleTree.xml";
        private String osmConectorName = "osmConnector.xml";
        private String filterstrategyFileName = "filterstrategies.xml";

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiFunctions"/> class.
        /// </summary>
        /// <param name="strategyMgr"></param>
        /// <param name="grantTree"></param>
        /// <param name="treeOperation"></param>
        public GuiFunctions(StrategyManager strategyMgr, GeneratedGrantTrees grantTree, TreeOperation treeOperation)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTree;
            this.treeOperation = treeOperation;
        }

        public class MenuItem
        {
            public MenuItem()
            {
                this.Items = new ObservableCollection<MenuItem>();
            }

            public override string ToString()
            {
               // return String.Format("Id = {0}, controlType = {1}, name = {2}", IdGenerated, controlTypeFiltered, nameFiltered);
               return String.Format("{0} - {1}", controlTypeFiltered, nameFiltered); //so wird es in der GUI angezeigt
            }

            public String IdGenerated
            {
                get;
                set;
            }
            public String controlTypeFiltered
            {
                get;
                set;
            }
            public String nameFiltered
            {
                get;
                set;
            }
            public String viewName
            {
                get;
                set;
            }
            // falls nötig, können diese Properties aktiviert werden
          /*  public String screenName
            {
                get;
                set;
            }
            public String screenCategory
            {
                get;
                set;
            }*
            /*   public String acceleratorKeyFiltered
               {
                   get;
                   set;
               }

               public String accessKeyFiltered
               {
                   get;
                   set;
               }



               public Boolean? isKeyboardFocusableFiltered
               {
                   get;
                   set;
               }

               public int[] runtimeIDFiltered
               {
                   get;
                   set;
               }

               // STATE

               // Boolean? => true, false, null
               public Boolean? isEnabledFiltered
               {
                   get;
                   set;
               }

               public Boolean? hasKeyboardFocusFiltered
               {
                   get;
                   set;
               }

               // Visibility

               public Rect boundingRectangleFiltered
               {
                   get;
                   set;
               }

               public Boolean? isOffscreenFiltered
               {
                   get;
                   set;
               }

               public String helpTextFiltered
               {
                   get;
                   set;
               }


               //IDENTIFICATION/Elemttype

               //nicht von UIA


               public String autoamtionIdFiltered
               {
                   get;
                   set;
               }


               public String classNameFiltered
               {
                   get;
                   set;
               }

               //Anmerkung: ich habe den LocalizedControlType genommen
             

               public String frameWorkIdFiltered
               {
                   get;
                   set;
               }

               //typ?
               // Anmerkung: von String zu int geändert
               public IntPtr hWndFiltered
               {
                   get;
                   set;
               }

               public Boolean? isContentElementFiltered
               {
                   get;
                   set;
               }
               //typ?
               public String labeledByFiltered
               {
                   get;
                   set;
               }

               public Boolean? isControlElementFiltered
               {
                   get;
                   set;
               }

               public Boolean? isPasswordFiltered
               {
                   get;
                   set;
               }

               public String localizedControlTypeFiltered
               {
                   get;
                   set;
               }

              

               public int processIdFiltered
               {
                   get;
                   set;
               }

               public String itemTypeFiltered
               {
                   get;
                   set;
               }
               public String itemStatusFiltered
               {
                   get;
                   set;
               }
               public Boolean? isRequiredForFormFiltered
               {
                   get;
                   set;
               }

               public String valueFiltered { get; set; }*/

            public TreeViewItem parentMenuItem
            {
                get;
                set;
            }



            /// <summary>
            /// Enthält die unterstützten Pattern
            /// </summary>
            public object[] suportedPatterns { get; set; }

            public ObservableCollection<MenuItem> Items { get; set; }
        }

        /// <summary>
        /// flatten a tree
        /// </summary>
        /// <param name="node"> first node of a tree</param>
        /// <returns></returns>
        public static IEnumerable<TreeViewItem> Flatten(TreeViewItem node)
        {
            // http://stackoverflow.com/questions/17086190/linq-query-for-selecting-an-item-from-tree-structure-but-to-look-in-the-whole-d -> Antwort von Ben Reich
            yield return node;
            if (node.Items != null)
            {
                foreach (var child in node.Items)
                    foreach (var descendant in Flatten((TreeViewItem)child))
                        yield return descendant;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree">gibt den darzustellenden Baum an</param>
        /// <param name="root"></param>
        /// <param name="isFilteredTree">gibt an, ob es sich um den gefilterten Baum handelt</param>
        public void createTreeForOutput(Object tree, ref TreeViewItem root, bool isFilteredTree = true)
        {            
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                 MenuItem child = new MenuItem();
                TreeViewItem treeViewItem = new TreeViewItem();
                #region Werte hinzufügen
                child.controlTypeFiltered = strategyMgr.getSpecifiedTree().GetData(node).properties.controlTypeFiltered == null ? " " : strategyMgr.getSpecifiedTree().GetData(node).properties.controlTypeFiltered;
                child.IdGenerated = strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated == null ? " " : strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated;
              
                String nameFiltered;
                if (!isFilteredTree)
                {
                    if (strategyMgr.getSpecifiedTree().Depth(node) == 0)
                    {
                        nameFiltered = strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.typeOfView == null ? " " : strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.typeOfView;

                    }
                    else if (strategyMgr.getSpecifiedTree().Depth(node) == 1)
                    {
                        nameFiltered = strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.screenName == null ? " " : strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.screenName;

                    }
                    else
                    {
                        nameFiltered = strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName == null ? " " : strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName;

                    }
                }else
                {
                    nameFiltered = strategyMgr.getSpecifiedTree().GetData(node).properties.nameFiltered == null ? " " : strategyMgr.getSpecifiedTree().GetData(node).properties.nameFiltered;

                }
                if (nameFiltered.Length > 40)
                {
                    child.nameFiltered = nameFiltered.Substring(0, 40);
                }
                else
                {
                    child.nameFiltered = nameFiltered;
                }
                #endregion
                treeViewItem.Header = child;

                #region Tooltip für Knotenbeziehungen
                if (grantTrees.brailleTree != null)
                {
                    Object treeForSearch;
                    List<String> conIds = new List<string>();
                    String toolTip = "Connected node:";
                    if (!isFilteredTree)
                    {
                        treeForSearch = grantTrees.filteredTree;
                        conIds.Add(treeOperation.searchNodes.getConnectedFilteredTreenodeId(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated));
                    }
                    else
                    {
                        treeForSearch = grantTrees.brailleTree;
                        conIds = treeOperation.searchNodes.getConnectedBrailleTreenodeIds(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated);
                    }
                    if (conIds != null && conIds.Count > 0 && conIds[0] != null)
                    {
                        foreach (String id in conIds)
                        {
                            OSMElement.OSMElement conNode = treeOperation.searchNodes.getAssociatedNodeElement(id, treeForSearch);
                            if (!conNode.Equals(new OSMElement.OSMElement()))
                            {
                                if (isFilteredTree)
                                {
                                    toolTip += "\n" + conNode.brailleRepresentation.typeOfView +" "+ conNode.brailleRepresentation.screenName+" " + conNode.brailleRepresentation.viewName + " -" +conNode.properties.controlTypeFiltered +  " (" + conNode.properties.IdGenerated + ")";
                                }
                                else
                                {
                                    toolTip += "\n" + conNode.properties.controlTypeFiltered + " - " + conNode.properties.nameFiltered + " (" + conNode.properties.IdGenerated + ")";
                                }
                            }
                            treeViewItem.ToolTip = toolTip;
                        }
                    }
                }
                #endregion
                if (!strategyMgr.getSpecifiedTree().IsRoot(node))
                {
                    if (!(isFilteredTree && strategyMgr.getSpecifiedTree().IsTop(node)))
                    {
                        child.parentMenuItem = root;

                        root.Items.Add(treeViewItem);
                        if (strategyMgr.getSpecifiedTree().IsLast(node))
                        {
                            root = getRootForNextElement(node, treeViewItem);
                        }
                    }
                }

                if (strategyMgr.getSpecifiedTree().HasChild(node))
                {
                    root = treeViewItem;
                }
            }
            //geht zurück zum "Root"-MenuItem
            while (root.Header != null && root.Header is MenuItem && ((MenuItem)root.Header).parentMenuItem != null)
            {
                root = ((MenuItem)root.Header).parentMenuItem;
            }
        }

        /// <summary>
        /// Ermittelt zu einem Knoten das MenuItem, welches für den nächsten Knoten verwendet werden soll
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeItem"></param>
        /// <returns><c>GuiFunctions.MenuItem</c>, welches für den nächsten Knoten verwendet werden soll</returns>
        private TreeViewItem getRootForNextElement(Object node, TreeViewItem nodeItem)
        {
            if (!strategyMgr.getSpecifiedTree().HasNext(node) && strategyMgr.getSpecifiedTree().HasParent(node)) 
            {
                if (strategyMgr.getSpecifiedTree().HasNext(strategyMgr.getSpecifiedTree().Parent(node)))
                {
                    return (((MenuItem)((MenuItem)nodeItem.Header).parentMenuItem.Header).parentMenuItem);
                }
                else
                {
                    return getRootForNextElement(strategyMgr.getSpecifiedTree().Parent(node), ((MenuItem)nodeItem.Header).parentMenuItem);
                }
            }
            
            return nodeItem;
        }

        /// <summary>
        /// Speichert den gefilterten Baum aus dem <c>GeneratedGrantTrees</c>
        /// </summary>
        /// <param name="filePath">gibt den Dateipfad + Namen an</param>
        public void saveFilteredTree(String filePath)
        {
            if (grantTrees == null || grantTrees.filteredTree == null) { Console.WriteLine("Es ist kein gefilterter Baum vorhanden."); return; }
            using (System.IO.FileStream fs = System.IO.File.Create(filePath))
            {
                strategyMgr.getSpecifiedTree().XmlSerialize(grantTrees.filteredTree, fs);
                //fs.Close(); <- nicht benötigt da using
            }
        }

        /// <summary>
        /// Speichert den Braille Baum aus dem <c>GeneratedGrantTrees</c>
        /// </summary>
        /// <param name="filePath">gibt den Dateipfad + Namen an</param>
        private void saveBrailleTree(String filePath)
        {
            if (grantTrees == null || grantTrees.brailleTree == null) { Console.WriteLine("Es ist kein gefilterter Baum vorhanden."); }
            using (System.IO.FileStream fs = System.IO.File.Create(filePath))
            {
                strategyMgr.getSpecifiedTree().XmlSerialize(grantTrees.brailleTree, fs);
                //fs.Close(); <- nicht benötigt da using
            }
        }

        /// <summary>
        /// saves a project
        /// </summary>
        /// <param name="projectFilePath">path and filename of the project</param>
        public void saveProject(String projectFilePath)
        {
            if (grantTrees == null) { Debug.WriteLine("Grant-Tree ist null -- Projekt kann nicht gespeichert werden!"); return; }
            if (!Path.GetExtension(@projectFilePath).Equals(".grant", StringComparison.OrdinalIgnoreCase))
            {
                // .grant hinzufügen
                projectFilePath = @projectFilePath + ".grant";
            }
            String directoryPath = Path.GetDirectoryName(@projectFilePath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(@projectFilePath);
            
            GrantProjectObject projectObject = new GrantProjectObject();
            #region saves the strategies
            projectObject.grantBrailleStrategyFullName =  strategyMgr.getSpecifiedBrailleDisplay() == null ? null : strategyMgr.getSpecifiedBrailleDisplay().GetType().FullName;
            projectObject.grantBrailleStrategyNamespace = strategyMgr.getSpecifiedBrailleDisplay() == null ? null : strategyMgr.getSpecifiedBrailleDisplay().GetType().Namespace;
            projectObject.grantDisplayStrategyFullName = strategyMgr.getSpecifiedDisplayStrategy() == null ? null : strategyMgr.getSpecifiedDisplayStrategy().GetType().FullName;
            projectObject.grantDisplayStrategyNamespace = strategyMgr.getSpecifiedDisplayStrategy() == null ? null : strategyMgr.getSpecifiedDisplayStrategy().GetType().Namespace;
            projectObject.grantTreeStrategyFullName = strategyMgr.getSpecifiedTree() == null ? null : strategyMgr.getSpecifiedTree().GetType().Namespace + "." + strategyMgr.getSpecifiedTree().GetType().Name;
            projectObject.grantTreeStrategyNamespace = strategyMgr.getSpecifiedTree() == null ? null : strategyMgr.getSpecifiedTree().GetType().Namespace;
            projectObject.grantOperationSystemStrategyFullName = strategyMgr.getSpecifiedOperationSystem() == null ? null : strategyMgr.getSpecifiedOperationSystem().GetType().FullName;
            projectObject.grantOperationSystemStrategyNamespace = strategyMgr.getSpecifiedOperationSystem() == null ? null : strategyMgr.getSpecifiedOperationSystem().GetType().Namespace;
            projectObject.device = strategyMgr.getSpecifiedDisplayStrategy() == null ? default(Device) : strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice();
            #endregion
            //creates a directory for the project
            DirectoryInfo di = Directory.CreateDirectory(directoryPath);
            if (di.Exists)
            {
                //temporary deletes  all children from a group and their connections to the braille (output) tree 
                treeOperation.updateNodes.deleteChildsOfBrailleGroups();
                saveFilteredTree(directoryPath + Path.DirectorySeparatorChar + filteredTreeSavedName);
                if (grantTrees.brailleTree != null)
                {
                    saveBrailleTree(directoryPath + Path.DirectorySeparatorChar + brailleTreeSavedName);
                }
                XmlSerializer serializer;
                if (grantTrees.osmRelationship != null && !grantTrees.osmRelationship.Equals(new OsmConnector<String, String>()) && grantTrees.osmRelationship.Count > 0)
                {
                    using (StreamWriter writer = new StreamWriter(directoryPath + Path.DirectorySeparatorChar + osmConectorName))
                    {
                        serializer = new XmlSerializer(typeof(List<OsmConnector<String, String>>));
                        serializer.Serialize(writer, grantTrees.osmRelationship);
                    }
                }
                if (grantTrees.filterstrategiesOfNodes != null)
                {
                    using (StreamWriter writer = new StreamWriter(directoryPath + Path.DirectorySeparatorChar + filterstrategyFileName))
                    {
                        serializer = new XmlSerializer(typeof(List<FilterstrategyOfNode<String, String, String>>));
                        serializer.Serialize(writer, grantTrees.filterstrategiesOfNodes);
                    }
                }
                using (StreamWriter writer = new StreamWriter(projectFilePath))
                {
                    serializer = new XmlSerializer(typeof(GrantProjectObject));
                    serializer.Serialize(writer, projectObject);
                }
                //rebuilds all children elements from a group and their connections to the braille tree
                treeOperation.updateNodes.updateBrailleGroups();
            }            
        }

        /// <summary>
        /// Loads a GRANT project
        /// </summary>
        /// <param name="projectFilePath">path to the GRANT project file
        /// file extension must be ".grant"</param>
        public void loadGrantProject(String projectFilePath)
        {
            if (!Path.GetExtension(@projectFilePath).Equals(".grant", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("Falsche Dateiendung!");
                return;
            }
            if (!File.Exists(@projectFilePath))
            {
                Debug.WriteLine("Die Datei Existiert nicht");
                return;
            }
            deleteGrantTrees();
            GrantProjectObject grantProjectObject;
            String projectDirectory = Path.GetDirectoryName(@projectFilePath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(@projectFilePath);

            XmlSerializer serializer;
            System.IO.FileStream fs = null;
            try{ 
                fs = System.IO.File.Open(projectFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                using (StreamReader reader = new StreamReader(fs))
                {
                    fs = null;
                    serializer = new XmlSerializer(typeof(GrantProjectObject));
                    grantProjectObject = (GrantProjectObject)serializer.Deserialize(reader);
                }
            }finally{
                if(fs != null){fs.Dispose();}
            }
            
            //set the OSM connection
            if (File.Exists(@projectDirectory + Path.DirectorySeparatorChar + osmConectorName))
            {
                try
                {
                    fs = System.IO.File.Open(@projectDirectory + Path.DirectorySeparatorChar + osmConectorName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                    serializer = new XmlSerializer(typeof(List<OsmConnector<String, String>>));
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        fs = null;
                        try
                        {
                            List<OsmConnector<String, String>> osmConector = (List<OsmConnector<String, String>>)serializer.Deserialize(reader);
                            grantTrees.osmRelationship = osmConector;
                        }
                        catch (InvalidOperationException e) { Console.WriteLine("Exception when loading a OSM connection: {0}", e); }
                    }
                }
                finally { if (fs != null) { fs.Dispose(); } }
            }
            #region loads the strategies 
            if (grantProjectObject.grantBrailleStrategyFullName != null && grantProjectObject.grantBrailleStrategyNamespace != null)
            {
                strategyMgr.setSpecifiedBrailleDisplay(grantProjectObject.grantBrailleStrategyFullName + ", " + grantProjectObject.grantBrailleStrategyNamespace);
                strategyMgr.getSpecifiedBrailleDisplay().setGeneratedGrantTrees(grantTrees);
                strategyMgr.getSpecifiedBrailleDisplay().setTreeOperation(treeOperation);
                strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
            }
            if (grantProjectObject.grantDisplayStrategyFullName != null && grantProjectObject.grantDisplayStrategyNamespace != null)
            {
                strategyMgr.setSpecifiedDisplayStrategy(grantProjectObject.grantDisplayStrategyFullName + ", " + grantProjectObject.grantDisplayStrategyNamespace);
            }
            if (grantProjectObject.grantTreeStrategyFullName != null && grantProjectObject.grantTreeStrategyNamespace != null)
            {
                strategyMgr.setSpecifiedTree(grantProjectObject.grantTreeStrategyFullName + ", " + grantProjectObject.grantTreeStrategyNamespace);
            }
            if (grantProjectObject.grantOperationSystemStrategyFullName != null && grantProjectObject.grantOperationSystemStrategyNamespace != null)
            {
                strategyMgr.setSpecifiedOperationSystem(grantProjectObject.grantOperationSystemStrategyFullName + ", " + grantProjectObject.grantOperationSystemStrategyNamespace);
            }
            #endregion
            // if necessary set the output device
            strategyMgr.getSpecifiedDisplayStrategy().setActiveDevice(grantProjectObject.device);
            //load filtered tree and braille (output) tree -> it will be in a subfolder wich has the same name like the project
            loadFilterstrategies(@projectDirectory + Path.DirectorySeparatorChar + filterstrategyFileName);
            loadFilteredTree(projectDirectory + Path.DirectorySeparatorChar + filteredTreeSavedName);
            loadBrailleTree(projectDirectory + Path.DirectorySeparatorChar + brailleTreeSavedName);
        }

        /// <summary>
        /// Loads a braille (output) tree
        /// </summary>
        /// <param name="filePath">path to the braille tree</param>
        private void loadBrailleTree(String filePath)
        {
            
            if (!File.Exists(@filePath))
            {
                Debug.WriteLine("Die Datei Existiert nicht");
                return;
            }
            System.IO.FileStream fs;
            using (fs = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                Object loadedTree = strategyMgr.getSpecifiedTree().XmlDeserialize(fs);

                grantTrees.brailleTree = loadedTree;
            }
            
            updateConnectedBrailleNodes();
            treeOperation.updateNodes.updateBrailleGroups();
        }

        /// <summary>
        /// updates the nodes of the braille tree which are connectet with a node of the filtered tree
        /// </summary>
        private void updateConnectedBrailleNodes()
        {
            List<OsmConnector<String, String>> osmConector =  grantTrees.osmRelationship;
            if (osmConector != null)
            {
                foreach (OsmConnector<String, String> con in osmConector)
                {
                    OSMElement.OSMElement brailleNode = treeOperation.searchNodes.getBrailleTreeOsmElementById(con.BrailleTree);
                    treeOperation.updateNodes.updateNodeOfBrailleUi(ref brailleNode);
                }
            }
        }

        /// <summary>
        /// Loads the strategy of filtering for a node
        /// </summary>
        /// <param name="filePath">path of the directory and file inclusive the file extension</param>
        private void loadFilterstrategies(String filePath)
        {
            if (File.Exists(@filePath))
            {
                System.IO.FileStream fs = null;
                try{ fs = System.IO.File.Open(@filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                
                    XmlSerializer serializer = new XmlSerializer(typeof(List<FilterstrategyOfNode<String, String, String>>));
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        fs = null;
                        List<FilterstrategyOfNode<String, String, String>> filterstrategies = (List<FilterstrategyOfNode<String, String, String>>)serializer.Deserialize(reader);
                        grantTrees.filterstrategiesOfNodes = filterstrategies;
                    }
                }
                finally { if (fs != null) { fs.Dispose(); } }
            }
            else
            {
                Debug.WriteLine("The file with the strategies of filtering doesn't exist!");
            }
        }

        /// <summary>
        /// Loads a filtered tree and stores the result into the <c>GeneratedGrantTrees</c> object
        /// </summary>
        /// <param name="filePath">path of the directory and file inclusive the file extension</param>
        private void loadFilteredTree(String filePath)
        {
            if (!File.Exists(@filePath))
            {
                Debug.WriteLine("The file doesn't exist!");
                return;
            }
            using (System.IO.FileStream fs = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                Object loadedTree = strategyMgr.getSpecifiedTree().XmlDeserialize(fs);
                grantTrees.filteredTree =loadedTree;
            }

            if (grantTrees.filteredTree != null && strategyMgr.getSpecifiedTree().HasChild(grantTrees.filteredTree) && !strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).Equals(new OSMElement.OSMElement()) && !strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.Equals(new GeneralProperties()))
            {
                FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.filteredTree, grantTrees.filterstrategiesOfNodes, strategyMgr.getSpecifiedTree());
                if ( mainFilterstrategy != null)
                {
                    strategyMgr.setSpecifiedFilter(mainFilterstrategy.FilterstrategyFullName+ ", " + mainFilterstrategy.FilterstrategyDll);
                    strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                    strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
                }
                else
                {
                    throw new Exception("Their isn't a filtering strategy in the first node! It can not be filtered.");
                }
            }
            else
            {
                throw new Exception("The Tree isn't sufficiently specified!");
            }
            if (openAppOfFilteredTree())
            {
                filteredLoadedApplication();
            }
        }

        /// <summary>
        /// If necessary opens the applications which should be filtered.
        /// </summary>
        /// <returns><c>true</c> if the application now open; otherwise <c>false</c></returns>
        public bool openAppOfFilteredTree()
        {
            if (grantTrees != null && grantTrees.filteredTree != null && strategyMgr.getSpecifiedTree().HasChild(grantTrees.filteredTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.Equals(new GeneralProperties()) || strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.moduleName == null)
                {
                    Debug.WriteLine("No data in the first node.");
                    return false;
                }
                IntPtr appIsRunnuing = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.moduleName);
                if (appIsRunnuing.Equals(IntPtr.Zero))
                {

                    if (strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.fileName != null)
                    {
                        bool openApp = strategyMgr.getSpecifiedOperationSystem().openApplication(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.fileName);
                        if (!openApp)
                        {
                            Debug.WriteLine("Application cann't open! Maybe the path is wrong."); //TODO
                            return false;
                        }
                        else { return true; }
                    }
                }
                else
                {
                    treeOperation.updateNodes.compareAndChangeFileName();
                    //invoke the application if these was minimised

                    strategyMgr.getSpecifiedOperationSystem().showWindow(appIsRunnuing);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// reloads a whole filtered tree; 
        /// </summary>
        public void filteredLoadedApplication()
        {
            //it is just necassary when the application was closed  (--> compare hwnd)
            Object loadedTree = grantTrees.filteredTree;

            IntPtr hwnd = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(loadedTree)).properties.moduleName);
            if (hwnd.Equals(IntPtr.Zero)) { throw new Exception("Der HWND der Anwendung konnte nicht gefunden werden!"); }
            treeOperation.updateNodes.updateFilteredTree(hwnd);
        }

        /// <summary>
        /// Deletes the trees, OSM connections and strategies of filtering from the <c>GrantProjectObject</c>
        /// </summary>
        public void deleteGrantTrees()
        {
            //grantTrees = new GeneratedGrantTrees();
            grantTrees.osmRelationship =new List<OsmConnector<String, String>>();
            grantTrees.filterstrategiesOfNodes = new List<FilterstrategyOfNode<String, String, String>>();
            Object treeNew = strategyMgr.getSpecifiedTree().NewTree();
            grantTrees.filteredTree = null;
            grantTrees.brailleTree = null;
        }

        /// <summary>
        /// filtered a subtree and updates the tree object in the <c>GrantProjectObject</c> object
        /// </summary>
        /// <param name="idGeneratedOfFirstNodeOfSubtree">id of the node of the subtree to be updated</param>
        public void filterAndAddSubtreeOfApplication(String idGeneratedOfFirstNodeOfSubtree)
        {
            OSMElement.OSMElement osmElementOfFirstNodeOfSubtree = treeOperation.searchNodes.getFilteredTreeOsmElementById(idGeneratedOfFirstNodeOfSubtree);
            Object subtree = strategyMgr.getSpecifiedFilter().updateFiltering(osmElementOfFirstNodeOfSubtree, TreeScopeEnum.Subtree);
            String idParent = treeOperation.updateNodes.changeSubTreeOfFilteredTree(subtree, idGeneratedOfFirstNodeOfSubtree);
            Object tree = grantTrees.filteredTree;

            List<Object> searchResultTrees = treeOperation.searchNodes.searchProperties(tree, strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(subtree)).properties, OperatorEnum.and);

            if (searchResultTrees != null && searchResultTrees.Count == 1)
            {
                treeOperation.generatedIds.generatedIdsOfFilteredSubtree(searchResultTrees[0]);
                treeOperation.updateNodes.setFilterstrategyInPropertiesAndObject(strategyMgr.getSpecifiedFilter().GetType(), searchResultTrees[0]);
            }
            else
            {
                Debug.WriteLine("TODO");
            }
        }

        /// <summary>
        /// checks whether, a XML is valid to the schema (XSD -- TemplateUi.xsd)
        /// </summary>
        /// <param name="pathToXml">path to the XML file</param>
        /// <returns><c>true</c> if the XML falls die Datei valide ist; sonst <c>false</c></returns>
        public static bool isTemplateValid(String pathToXml)
        {
            XDocument xDoc;
            if (!File.Exists(@pathToXml)) { Debug.WriteLine("The XML doesn't exist!"); return false; }
            try
            {
                xDoc = XDocument.Load(@pathToXml);
            }
            catch (XmlException e)
            {
                Debug.WriteLine("the XML ("+pathToXml+") isn't valid: " + e);
                return false;
            }
            bool isValid = true;
            String pathToXsd = @"TemplateUi.xsd";
            if (!File.Exists(@pathToXsd)) { Debug.WriteLine("The XSD schema doesn't exist!"); return false; }

            try
            {
                FileStream fs = new FileStream(pathToXsd, FileMode.Open);

                XmlSchema xsd = XmlSchema.Read(fs, ValidationCallback);
                // xsd.Write(Console.Out);
                XmlSchemaSet xsdSet = new XmlSchemaSet();
                xsdSet.Add(xsd);
                xDoc.Validate(xsdSet, (o, e) =>
                {
                    Console.WriteLine("{0}", e.Message);
                    isValid = false;
                });
                System.Xml.Serialization.XmlSerializerNamespaces a = xsd.Namespaces;
                return isValid;
            }
            catch (IOException e) { Debug.WriteLine("The file cann't be accessed\n" + e); return false; }
        }

        static void ValidationCallback(object sender, ValidationEventArgs args)
        { // https://msdn.microsoft.com/de-de/library/04x694fe(v=vs.110).aspx
            if (args.Severity == XmlSeverityType.Warning)
                Console.Write("WARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Console.Write("ERROR: ");

            Console.WriteLine(args.Message);
        }

        /// <summary>
        /// Calculates the braille node to a point (on a braille device)
        /// </summary>
        /// <param name="x">x coordinat of the point</param>
        /// <param name="y">y coordinat of the point</param>
        /// <returns>the name of the view or <code>null</code></returns>
        public Object getBrailleNodeAtPoint(int x, int y)
        {
            int offsetX, offsetY;
            String viewAtPoint = strategyMgr.getSpecifiedBrailleDisplay().getBrailleUiElementViewNameAtPoint(x, y, out offsetX, out offsetY);
            return treeOperation.searchNodes.getTreeElementOfViewAtPoint(x, y, viewAtPoint, offsetX, offsetY);
        }

        /// <summary>
        /// Calculates to a point on a screenshot (on a braille device) the corresponding  point on the orginal "image" of the application
        /// </summary>
        /// <param name="brailleNode">the node with the screenshot (Controlltype == 'Screenshot') on which was clicked on the braille device</param>
        /// <param name="x">x coordinat of the point</param>
        /// <param name="y">y coordinat of the point</param>
        /// <param name="applicationX">x coordinat of the (normal) display OR <code>-1</code> </param>
        /// <param name="applicationY">y coordinat of the (normal) display OR <code>-1</code> </param>
        public void getScreenshotPointInApplication(Object brailleNode, int x, int y, out int applicationX, out int applicationY)
        {
            //attention: offset and zoom are still ignore
            applicationX = -1;
            applicationY = -1;
            OSMElement.OSMElement dataBraille = strategyMgr.getSpecifiedTree().GetData(brailleNode);
            if (dataBraille.Equals(new OSMElement.OSMElement())) { return; }
            if (!dataBraille.properties.controlTypeFiltered.Equals("Screenshot")) { Debug.WriteLine("Attention: This function should be only used if the controlltype of the node 'Screenshot'"); return; }


            #region calculates the click position of the screenshot on the braille device
            TactileNodeInfos nodeinfos = strategyMgr.getSpecifiedBrailleDisplay().getTactileNodeInfos(brailleNode);
            Device device = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice();
            int pointScreenshotBrailleX =  x - Convert.ToInt32( dataBraille.properties.boundingRectangleFiltered.TopLeft.X);

            int pointScreenshotBrailleY = y - Convert.ToInt32(dataBraille.properties.boundingRectangleFiltered.TopLeft.Y);
            //TODO: checks whether the point is into the rectange of the node
            #endregion
            #region mappes the point on the braille device to a point in the application
            OsmConnector<String, String> osmRelationships = grantTrees.osmRelationship.Find(r => r.BrailleTree.Equals(dataBraille.properties.IdGenerated));
            OSMElement.OSMElement dataFiltered = treeOperation.searchNodes.getFilteredTreeOsmElementById(osmRelationships.FilteredTree);
            if (osmRelationships == null) { return; }

            if (nodeinfos.Equals(new TactileNodeInfos()))
            {
                return;
            }
            int applicationNodeX = Convert.ToInt32(dataFiltered.properties.boundingRectangleFiltered.Width / nodeinfos.contentWidth * pointScreenshotBrailleX);
            int applicationNodeY = Convert.ToInt32(dataFiltered.properties.boundingRectangleFiltered.Height / nodeinfos.contentHeight * pointScreenshotBrailleY);
            // Debug.WriteLine("x = {0}, y = {1}", applicationNodeX, applicationNodeY);
            #endregion
            #region calculates the point with cover of the (normal) display
            //TODO Attention: currently it works only if the frame of the application starts in the left upper corner (on the (normal) display)
            applicationX = applicationNodeX;
            applicationY = applicationNodeY;
            #endregion
        }

        /// <summary>
        /// checks whether the template can be used for the choosen device 
        /// it depends on the dimensions of the braille device
        /// </summary>
        /// <param name="pathToXml">path to the XML file</param>
        /// <returns><c>true</c> if the template can be used; otherwise <c>false</c></returns>
        public bool isTemplateUsableForDevice(String pathToXml)
        {
            int tmpHeight, tmpWidth;
            return isTemplateUsableForDevice(@pathToXml, out tmpHeight, out tmpWidth);
        }

        /// <summary>
        /// checks whether the template can be used for the choosen device 
        /// it depends on the dimensions of the braille device
        /// </summary>
        /// <param name="pathToXml">path to the XML file</param>
        /// <param name="minDeviceHeight">the minimum number of pins of the braille display (height) for the template</param>
        /// <param name="minDeviceWidth">the minimum number of pins of the braille display (width) for the template</param>
        /// <returns><c>true</c> if the template can be used; otherwise<c> false</c></returns>
        public bool isTemplateUsableForDevice(String pathToXml, out int minDeviceHeight, out int minDeviceWidth)
        {
            minDeviceHeight = -1;
            minDeviceWidth = -1;
            XElement xElement;
            try
            {
                xElement = XElement.Load(@pathToXml);
            }
            catch (XmlException e)
            {
                Debug.WriteLine("the XML (" + pathToXml + ") isn't valide: " + e);
                return false;
            }
            String pathToXsd = @"TemplateUi.xsd";
            if (!File.Exists(@pathToXsd)) { Debug.WriteLine("The XSD schema doesn't exist"); return false; }
            minDeviceHeight = Convert.ToInt32( xElement.Element("MinDeviceHeight").Value);
            minDeviceWidth = Convert.ToInt32(xElement.Element("MinDeviceWidth").Value);
            Device activeDevice = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice();
            if (activeDevice.height >= minDeviceHeight && activeDevice.width >= minDeviceWidth)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///Removes invalide characters
        /// see: https://msdn.microsoft.com/library/844skk0h(v=vs.110).aspx
        /// </summary>
        /// <param name="input">gibt den String an, bei dem die unerwünschten Zeichen gelöscht werden sollen</param>
        /// <returns>string without the invalid characters</returns>
        public static String cleanInput(String input)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(input, @"[^\w\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }

        #region add filtered node in braille tree
        /// <summary>
        /// Adds a node to the braille (output) tree
        /// </summary>
        /// <param name="filteredNode">filtered node which should be added</param>
        public void addFilteredNodeToBrailleTree(Object filteredNode)
        {
            addFilteredNodeToBrailleTree("", filteredNode, null, null, null, null, new Rect());
        }

        /// <summary>
        /// Adds a node to the braille (output) tree
        /// </summary>
        /// <param name="controlleType">The controlle type which should be used for the tactile representation</param>
        /// <param name="filteredNode">corresponding filtered node which should be added</param>
        public void addFilteredNodeToBrailleTree(String controlleType, object filteredNode = null)
        {
            addFilteredNodeToBrailleTree(controlleType, filteredNode, null, null, null, null, new Rect());
        }

        /* nicht endeutig
         * public void addFilteredNodeToBrailleTree(String parentIdTactlie, String screenName, String screenCategory, String viewName = null, Rect positionTactile = new Rect())
        {
            addFilteredNodeToBrailleTree("", null, viewName, screenName, screenCategory, parentIdTactlie, positionTactile);
        }*/

        /// <summary>
        /// Adds a node to the braille (output) tree
        /// </summary>
        /// <param name="screenName">name of the screen to which the new node should be added</param>
        /// <param name="screenCategory">screen category of the new node</param>
        /// <param name="viewName">view name of the new node</param>
        /// <param name="positionTactile">tactile position from the new node</param>
        public void addFilteredNodeToBrailleTree( String screenName, String screenCategory, String viewName = null, Rect positionTactile = new Rect())
        {
            addFilteredNodeToBrailleTree("", null, viewName, screenName, screenCategory, null, positionTactile);
        }

        /// <summary>
        /// Adds a node to the braille (output) tree
        /// </summary>
        /// <param name = "controlleType" > The controlle type which should be used for the tactile representation</param>
        /// <param name="filteredNode">corresponding filtered node which should be added</param>
        /// <param name="viewName">view name of the new node</param>
        /// <param name="screenName">name of the screen to which the new node should be added</param>
        /// <param name="screenCategory">screen category of the new node</param>
        /// <param name="parentIdTactlie">id of the parent node in the braille (output) tree</param>
        /// <param name="positionTactile">tactile position from the new node</param>
        private void addFilteredNodeToBrailleTree(String controlleType, object filteredNode = null, String viewName = null, String screenName = null, String screenCategory = null, String parentIdTactlie = null, Rect positionTactile = new Rect())
        {
            OSMElement.OSMElement tactileOsm = new OSMElement.OSMElement();
            GeneralProperties tactileProp = new GeneralProperties();
            BrailleRepresentation tactlieRep = new BrailleRepresentation();
            if (controlleType == "" || !strategyMgr.getSpecifiedBrailleDisplay().getUiElementRenderer().Contains(controlleType))
            {
                Console.WriteLine("The controlltype does't exist (for this braille device) --> controlltype = 'Text'!");
                controlleType = "Text";
            }
            tactileProp.controlTypeFiltered = controlleType;
            tactileProp.boundingRectangleFiltered = positionTactile;
            if(filteredNode != null)
            {
                tactlieRep.displayedGuiElementType = "nameFiltered";
            }
            tactlieRep.viewName = viewName != null ? viewName : Guid.NewGuid().ToString();
            tactlieRep.typeOfView = screenCategory != null ? screenCategory : Guid.NewGuid().ToString();
            tactlieRep.screenName = screenName != null ?  screenName : tactlieRep.typeOfView;
            //Attention: when new Controlletypes are added, these should be added here!
            if (controlleType.Equals("DropDownMenuItem"))
            { 
                OSMElement.UiElements.DropDownMenuItem dropDownMenuItem = new OSMElement.UiElements.DropDownMenuItem();
                if (filteredNode != null)
                {
                    OSMElement.OSMElement dataNode = strategyMgr.getSpecifiedTree().GetData(filteredNode);
                    dropDownMenuItem.hasChild = strategyMgr.getSpecifiedTree().HasChild(filteredNode);
                    dropDownMenuItem.hasNext = strategyMgr.getSpecifiedTree().HasNext(filteredNode);
                }
                tactlieRep.uiElementSpecialContent = dropDownMenuItem;
            }
            if (controlleType.Equals("ListItem"))
            {
                OSMElement.UiElements.ListMenuItem listItem = new OSMElement.UiElements.ListMenuItem();
                if(filteredNode != null)
                {
                    if (strategyMgr.getSpecifiedTree().GetData(filteredNode).properties.controlTypeFiltered.Equals("CheckBox"))
                    {
                        listItem.isMultipleSelection = true;
                    }
                }
                tactlieRep.uiElementSpecialContent = listItem;
            }
            if (controlleType.Equals("TabItem"))
            {
                OSMElement.UiElements.TabItem tabItem = new OSMElement.UiElements.TabItem();
                tabItem.orientation = OSMElement.UiElements.Orientation.Top;
                tactlieRep.uiElementSpecialContent = tabItem;
            }
           

            tactileOsm.brailleRepresentation = tactlieRep;
            tactileOsm.properties = tactileProp;
            String idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(tactileOsm, parentIdTactlie);

            if (filteredNode != null)
            {
                List<OsmConnector<String, String>> relationship = grantTrees.osmRelationship;
                OsmTreeConnector.addOsmConnection(strategyMgr.getSpecifiedTree().GetData(filteredNode).properties.IdGenerated, idGenerated, ref relationship);
                treeOperation.updateNodes.updateNodeOfBrailleUi(ref tactileOsm);
            }
        }
        #endregion

    }
}
