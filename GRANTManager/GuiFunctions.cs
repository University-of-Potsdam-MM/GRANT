using System;
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


            public String acceleratorKeyFiltered
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
            public String IdGenerated
            {
                get;
                set;
            }

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
            public String controlTypeFiltered
            {
                get;
                set;
            }

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
            public String labeledbyFiltered
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

            public String nameFiltered
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

            public String valueFiltered { get; set; }

            public MenuItem parentMenuItem
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
        /// prueft, ob der knoten an der richtigen Stelle dargestellt wird und korrigiert es ggf.
        /// </summary>
        /// <param name="root">gibt das (erwartete) Eltern-menuItem-element an</param>
        /// <param name="parentNode">gibt den vorgängert Knoten (linker Geschwisterknoten) an</param>
        private void rootMenuItemCheckSibling(ref MenuItem root, Object siblingNode)
        {
            if (root.IdGenerated.Trim().Equals("")) { return; }

            if (strategyMgr.getSpecifiedTree().HasParent(siblingNode) && !strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Parent(siblingNode)).properties.IdGenerated.Equals(root.IdGenerated))
            {
                if (root.parentMenuItem.IdGenerated.Equals(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Parent(siblingNode)).properties.IdGenerated))
                {
                    root = root.parentMenuItem;
                    rootMenuItemCheckSibling(ref root, siblingNode);
                }
            }
        }

        public void treeIteration(Object tree, ref GuiFunctions.MenuItem root)
        {

            Object node1;

            while (strategyMgr.getSpecifiedTree().HasChild(tree) && !(strategyMgr.getSpecifiedTree().Count(tree) == 1 && strategyMgr.getSpecifiedTree().Depth(tree) == -1))
            {


                MenuItem child = new MenuItem();
                node1 = strategyMgr.getSpecifiedTree().Child(tree);
                child.controlTypeFiltered = strategyMgr.getSpecifiedTree().GetData(node1).properties.controlTypeFiltered == null ? " " : strategyMgr.getSpecifiedTree().GetData(node1).properties.controlTypeFiltered;
                child.IdGenerated = strategyMgr.getSpecifiedTree().GetData(node1).properties.IdGenerated == null ? " " : strategyMgr.getSpecifiedTree().GetData(node1).properties.IdGenerated;
                String nameFiltered = strategyMgr.getSpecifiedTree().GetData(node1).properties.nameFiltered == null ? " " : strategyMgr.getSpecifiedTree().GetData(node1).properties.nameFiltered;
                if (nameFiltered.Length > 40)
                {
                    child.nameFiltered = nameFiltered.Substring(0, 40);
                }
                else
                {
                    child.nameFiltered = nameFiltered;
                }

                /*  child.acceleratorKeyFiltered = node1.Data.properties.acceleratorKeyFiltered == null ? " " : node1.Data.properties.acceleratorKeyFiltered;
                  child.accessKeyFiltered = node1.Data.properties.accessKeyFiltered == null ? " " : node1.Data.properties.accessKeyFiltered;
                  child.helpTextFiltered = node1.Data.properties.helpTextFiltered == null ? " " : node1.Data.properties.helpTextFiltered;
                  child.autoamtionIdFiltered = node1.Data.properties.autoamtionIdFiltered == null ? " " : node1.Data.properties.autoamtionIdFiltered;
                  child.classNameFiltered = node1.Data.properties.classNameFiltered == null ? " " : node1.Data.properties.classNameFiltered;
                  child.controlTypeFiltered = node1.Data.properties.controlTypeFiltered == null ? " " : node1.Data.properties.controlTypeFiltered;
                  child.frameWorkIdFiltered = node1.Data.properties.frameWorkIdFiltered == null ? " " : node1.Data.properties.frameWorkIdFiltered;
                  child.itemTypeFiltered = node1.Data.properties.itemTypeFiltered == null ? " " : node1.Data.properties.itemTypeFiltered;
                  child.itemStatusFiltered = node1.Data.properties.itemStatusFiltered == null ? " " : node1.Data.properties.itemStatusFiltered;
                  child.valueFiltered = node1.Data.properties.valueFiltered == null ? " " : node1.Data.properties.valueFiltered;
                  child.boundingRectangleFiltered = node1.Data.properties.boundingRectangleFiltered;
  */


                if (!strategyMgr.getSpecifiedTree().IsRoot(tree))
                {
                    child.parentMenuItem = root;

                    root.Items.Add(child);
                }
                else { root = child; };

                if (strategyMgr.getSpecifiedTree().HasChild(node1))
                {
                    treeIteration(node1, ref child);
                }
                else
                {
                    if (strategyMgr.getSpecifiedTree().IsFirst(node1) && strategyMgr.getSpecifiedTree().IsLast(node1))
                    {
                        root = root.parentMenuItem == null ? root : root.parentMenuItem;
                    }
                    treeIteration(node1, ref root);
                }


            }
            while (strategyMgr.getSpecifiedTree().HasNext(tree))
            {
                MenuItem sibling = new MenuItem();
                node1 = strategyMgr.getSpecifiedTree().Next(tree);
                //Pruefung, ob wir es ans richtige Element ranhängen und ggf. korrigieren
                rootMenuItemCheckSibling(ref root, tree);

                sibling.controlTypeFiltered = strategyMgr.getSpecifiedTree().GetData(node1).properties.controlTypeFiltered == null ? " " : strategyMgr.getSpecifiedTree().GetData(node1).properties.controlTypeFiltered;
                sibling.IdGenerated = strategyMgr.getSpecifiedTree().GetData(node1).properties.IdGenerated == null ? " " : strategyMgr.getSpecifiedTree().GetData(node1).properties.IdGenerated;
                String nameFiltered = strategyMgr.getSpecifiedTree().GetData(node1).properties.nameFiltered == null ? " " : strategyMgr.getSpecifiedTree().GetData(node1).properties.nameFiltered.ToString();
                if (nameFiltered.Length > 40)
                {
                    sibling.nameFiltered = nameFiltered.Substring(0, 20);
                }
                else
                {
                    sibling.nameFiltered = nameFiltered;
                }
                /*       sibling.acceleratorKeyFiltered = node1.Data.properties.acceleratorKeyFiltered == null ? " " : node1.Data.properties.acceleratorKeyFiltered;
                       sibling.accessKeyFiltered = node1.Data.properties.accessKeyFiltered == null ? " " : node1.Data.properties.accessKeyFiltered;

                       sibling.helpTextFiltered = node1.Data.properties.helpTextFiltered == null ? " " : node1.Data.properties.helpTextFiltered;
                       sibling.autoamtionIdFiltered = node1.Data.properties.autoamtionIdFiltered == null ? " " : node1.Data.properties.autoamtionIdFiltered;
                       sibling.classNameFiltered = node1.Data.properties.classNameFiltered == null ? " " : node1.Data.properties.classNameFiltered;
                       sibling.controlTypeFiltered = node1.Data.properties.controlTypeFiltered == null ? " " : node1.Data.properties.controlTypeFiltered;
                       sibling.frameWorkIdFiltered = node1.Data.properties.frameWorkIdFiltered == null ? " " : node1.Data.properties.frameWorkIdFiltered;



                       sibling.itemTypeFiltered = node1.Data.properties.itemTypeFiltered == null ? " " : node1.Data.properties.itemTypeFiltered;
                       sibling.itemStatusFiltered = node1.Data.properties.itemStatusFiltered == null ? " " : node1.Data.properties.itemStatusFiltered;


                       sibling.valueFiltered = node1.Data.properties.valueFiltered == null ? " " : node1.Data.properties.valueFiltered;

                       sibling.boundingRectangleFiltered = node1.Data.properties.boundingRectangleFiltered;
                       */
                sibling.parentMenuItem = root;
                root.Items.Add(sibling);

                if (strategyMgr.getSpecifiedTree().HasChild(node1))
                {
                    treeIteration(node1, ref sibling);
                }
                else
                {
                    if (strategyMgr.getSpecifiedTree().IsFirst(node1) && strategyMgr.getSpecifiedTree().IsLast(node1))
                    {
                        root = root.parentMenuItem == null ? root : root.parentMenuItem;
                    }
                    if (!strategyMgr.getSpecifiedTree().HasChild(node1) && !strategyMgr.getSpecifiedTree().HasNext(node1))
                    {
                        root = root.parentMenuItem == null ? root : root.parentMenuItem;
                    }
                    treeIteration(node1, ref root);
                }
            }
            if (strategyMgr.getSpecifiedTree().Count(tree) == 1 && strategyMgr.getSpecifiedTree().Depth(tree) == -1)
            {
                
                return;
            }
            if (!strategyMgr.getSpecifiedTree().HasChild(tree))
            {
                node1 = tree;
                if (strategyMgr.getSpecifiedTree().HasParent(tree))
                {
                    strategyMgr.getSpecifiedTree().Remove(node1);
                    return;
                }
            }
            if (strategyMgr.getSpecifiedTree().IsFirst(tree) && strategyMgr.getSpecifiedTree().IsLast(tree))
            {
                root = root.parentMenuItem == null ? root : root.parentMenuItem;
            }
        }

        /// <summary>
        /// Speichert den gefilterten Baum aus dem <c>GeneratedGrantTrees</c>
        /// </summary>
        /// <param name="filePath">gibt den Dateipfad + Namen an</param>
        public void saveFilteredTree(String filePath)
        {
            if (grantTrees == null || grantTrees.getFilteredTree() == null) { Console.WriteLine("Es ist kein gefilterter Baum vorhanden."); return; }
            using (System.IO.FileStream fs = System.IO.File.Create(filePath))
            {
                strategyMgr.getSpecifiedTree().XmlSerialize(grantTrees.getFilteredTree(), fs);
                //fs.Close(); <- nicht benötigt da using
            }
        }

        /// <summary>
        /// Speichert den Braille Baum aus dem <c>GeneratedGrantTrees</c>
        /// </summary>
        /// <param name="filePath">gibt den Dateipfad + Namen an</param>
        private void saveBrailleTree(String filePath)
        {
            if (grantTrees == null || grantTrees.getBrailleTree() == null) { Console.WriteLine("Es ist kein gefilterter Baum vorhanden."); }
            using (System.IO.FileStream fs = System.IO.File.Create(filePath))
            {
                strategyMgr.getSpecifiedTree().XmlSerialize(grantTrees.getBrailleTree(), fs);
                //fs.Close(); <- nicht benötigt da using
            }
        }

        /// <summary>
        /// Speichert das Projekt
        /// </summary>
        /// <param name="projectFilePath">gibt den Pfad + Dateinamen des Projektes an</param>
        public void saveProject(String projectFilePath)
        {
            if (grantTrees == null) { Debug.WriteLine("Grant-Tree ist null -- Projekt kann nicht gespeichert werden!"); return; }
            if (!Path.GetExtension(@projectFilePath).Equals(".grant", StringComparison.OrdinalIgnoreCase))
            {
                // .grant hinzufügen
                projectFilePath = @projectFilePath + ".grant";
            }
            String directoryPath = Path.GetDirectoryName(@projectFilePath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(@projectFilePath);

            Debug.WriteLine(directoryPath);
            GrantProjectObject projectObject = new GrantProjectObject();
            #region Speichern der Strategyn
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
            //Ordner für das Projekt erstellen
            DirectoryInfo di = Directory.CreateDirectory(directoryPath);
            if (di.Exists)
            {
                //löscht temporär alle kindelemente von Gruppen und deren Beziehungen im Braille-Baum
                treeOperation.updateNodes.deleteChildsOfBrailleGroups();
                saveFilteredTree(directoryPath + Path.DirectorySeparatorChar + filteredTreeSavedName);
                if (grantTrees.getBrailleTree() != null)
                {
                    saveBrailleTree(directoryPath + Path.DirectorySeparatorChar + brailleTreeSavedName);
                }
                XmlSerializer serializer;
                if (grantTrees.getOsmRelationship() != null && !grantTrees.getOsmRelationship().Equals(new OsmConnector<String, String>()) && grantTrees.getOsmRelationship().Count > 0)
                {
                    using (StreamWriter writer = new StreamWriter(directoryPath + Path.DirectorySeparatorChar + osmConectorName))
                    {
                        serializer = new XmlSerializer(typeof(List<OsmConnector<String, String>>));
                        serializer.Serialize(writer, grantTrees.getOsmRelationship());
                    }
                }
                if (grantTrees.getFilterstrategiesOfNodes() != null)
                {
                    using (StreamWriter writer = new StreamWriter(directoryPath + Path.DirectorySeparatorChar + filterstrategyFileName))
                    {
                        serializer = new XmlSerializer(typeof(List<FilterstrategyOfNode<String, String, String>>));
                        serializer.Serialize(writer, grantTrees.getFilterstrategiesOfNodes());
                    }
                }
                using (StreamWriter writer = new StreamWriter(projectFilePath))
                {
                    serializer = new XmlSerializer(typeof(GrantProjectObject));
                    serializer.Serialize(writer, projectObject);
                }
                // erstellt wieder die Kinelemente von Gruppen und deren Beziehungen im Braille-Baum
                treeOperation.updateNodes.updateBrailleGroups();
            }            
        }

        /// <summary>
        /// Lädt ein GRANT-Projekt
        /// </summary>
        /// <param name="projectFilePath">Gibt den Pfad zum Grant-Projekt an; die Dateiendung muss .gant sein</param>
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
            
            //setze OSM-Beziehungen
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
                            grantTrees.setOsmRelationship(osmConector);
                        }
                        catch (InvalidOperationException e) { Console.WriteLine("Fehler beim Laden der OSM-Beziehungen: {0}", e); }
                    }
                }
                finally { if (fs != null) { fs.Dispose(); } }
            }
            //fs.Close(); <- nicht benötigt da using
            #region Laden der Strategyn
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
            //lade FilteredTree + brailleTree -> ist im Unterordner welcher den Projektnamen trägt
            loadFilterstrategies(@projectDirectory + Path.DirectorySeparatorChar + filterstrategyFileName);
            loadFilteredTree(projectDirectory + Path.DirectorySeparatorChar + filteredTreeSavedName);
            loadBrailleTree(projectDirectory + Path.DirectorySeparatorChar + brailleTreeSavedName);
        }

        /// <summary>
        /// Lädt den Braille-Baum
        /// </summary>
        /// <param name="filePath">gibt den Pfad zum Braille-Baum an</param>
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
                // fs.Close();<- nicht benötigt da using
                //Baum setzen
                grantTrees.setBrailleTree(loadedTree);
            }
            
            updateConnectedBrailleNodes();
            treeOperation.updateNodes.updateBrailleGroups();
        }

        /// <summary>
        /// Aktualisiert die (mit dem gefilterten Baum verbundenen) Knoten im Braille-Baum
        /// </summary>
        private void updateConnectedBrailleNodes()
        {
            List<OsmConnector<String, String>> osmConector =  grantTrees.getOsmRelationship();
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
        /// Lädt die Filterstrategien für die Knoten
        /// </summary>
        /// <param name="filePath">gibt den Dateipfad + Name an</param>
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
                        grantTrees.setFilterstrategiesOfNodes(filterstrategies);
                    }
                    //fs.Close(); 
                }
                finally { if (fs != null) { fs.Dispose(); } }
            }
            else
            {
                Debug.WriteLine("Die Datei mit den FIlterstrategien exisitert nicht!");
            }
        }

        /// <summary>
        /// Lädt eine gefilterten Baum und speichert das Ergebnis
        /// </summary>
        /// <param name="filePath">gibt den Dateipfad + Name an</param>
        private void loadFilteredTree(String filePath)
        {
            if (!File.Exists(@filePath))
            {
                Debug.WriteLine("Die Datei Existiert nicht");
                return;
            }
            using (System.IO.FileStream fs = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                Object loadedTree = strategyMgr.getSpecifiedTree().XmlDeserialize(fs);
               // fs.Close();
                //Baum setzen
                grantTrees.setFilteredTree(loadedTree);
            }
            //Filter-Strategy setzen
            if (grantTrees.getFilteredTree() != null && strategyMgr.getSpecifiedTree().HasChild(grantTrees.getFilteredTree()) && !strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).Equals(new OSMElement.OSMElement()) && !strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).properties.Equals(new GeneralProperties()))
            {
                FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.getFilteredTree(), grantTrees.getFilterstrategiesOfNodes(), strategyMgr.getSpecifiedTree());
                if ( mainFilterstrategy != null)
                {
                    strategyMgr.setSpecifiedFilter(mainFilterstrategy.FilterstrategyFullName+ ", " + mainFilterstrategy.FilterstrategyDll);
                    strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                    strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
                }
                else
                {
                    throw new Exception("Keine FilterStrategy im ersten Knoten angegeben");
                }
            }
            else
            {
                throw new Exception("Baum nicht ausreichend spezifiziert!");
            }
            if (openAppOfFilteredTree())
            {
                filteredLoadedApplication();
            }
        }

        /// <summary>
        /// Öffnet falls nötig die gefilterte Anwendung
        /// </summary>
        /// <returns><c>true</c> falls die Anwendung (nun) geöffnet ist; sonst <c>false</c></returns>
        public bool openAppOfFilteredTree()
        {
            if (grantTrees != null && grantTrees.getFilteredTree() != null && strategyMgr.getSpecifiedTree().HasChild(grantTrees.getFilteredTree()))
            {
                if (strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).properties.Equals(new GeneralProperties()) || strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).properties.moduleName == null)
                {
                    Debug.WriteLine("Kein Daten im 1. Knoten Vorhanden.");
                    return false;
                }
                IntPtr appIsRunnuing = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).properties.moduleName);
                Debug.WriteLine("App ist gestartet: {0}", appIsRunnuing);
                if (appIsRunnuing.Equals(IntPtr.Zero))
                {

                    if (strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).properties.fileName != null)
                    {
                        bool openApp = strategyMgr.getSpecifiedOperationSystem().openApplication(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).properties.fileName);
                        if (!openApp)
                        {
                            Debug.WriteLine("Anwendung konnte nicht geöffnet werden! Ggf. Pfad der Anwendung anpassen."); //TODO
                            return false;
                        }
                        else { return true; }
                    }
                }
                else
                {
                    treeOperation.updateNodes.compareAndChangeFileName();
                    //aktiviert die Anwendung (nötig falls es minimiert war)
                   
                    strategyMgr.getSpecifiedOperationSystem().showWindow(appIsRunnuing);
                    return true;
                }
            }
            return false;
        }


        public void filteredLoadedApplication()
        {
            //ist nur notwendig, wenn die Anwendung zwischendurch zu war (--> hwnd's vergleichen) oder die Anwendung verschoben wurde (--> Rect's vergleichen)
            Object loadedTree = grantTrees.getFilteredTree();

            IntPtr hwnd = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(loadedTree)).properties.moduleName);
            if (hwnd.Equals(IntPtr.Zero)) { throw new Exception("Der HWND der Anwendung konnte nicht gefunden werden!"); }

            treeOperation.updateNodes.updateFilteredTree(hwnd);

        }

        public void deleteGrantTrees()
        {
            //grantTrees = new GeneratedGrantTrees();
            grantTrees.setOsmRelationship(new List<OsmConnector<String, String>>());
            grantTrees.setFilterstrategiesOfNodes(new List<FilterstrategyOfNode<String, String, String>>());
            Object treeNew = strategyMgr.getSpecifiedTree().NewTree();
            grantTrees.setFilteredTree(null);
            grantTrees.setBrailleTree(null);
        }

        /// <summary>
        /// Filtert einen Teilbaum und aktualisiert das Baumobjekt
        /// </summary>
        /// <param name="idGeneratedOfFirstNodeOfSubtree">gibt die Id des Knotens an, ab welcher der Teilbaum aktualisiert werden soll (inkl. diesem Knoten)</param>
        public void filterAndAddSubtreeOfApplication(String idGeneratedOfFirstNodeOfSubtree)
        {
            OSMElement.OSMElement osmElementOfFirstNodeOfSubtree = treeOperation.searchNodes.getFilteredTreeOsmElementById(idGeneratedOfFirstNodeOfSubtree);
            Object subtree = strategyMgr.getSpecifiedFilter().updateFiltering(osmElementOfFirstNodeOfSubtree, TreeScopeEnum.Subtree);
            String idParent = treeOperation.updateNodes.changeSubTreeOfFilteredTree(subtree, idGeneratedOfFirstNodeOfSubtree);
            Object tree = grantTrees.getFilteredTree();

            List<Object> searchResultTrees = treeOperation.searchNodes.searchProperties(tree, strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(subtree)).properties, OperatorEnum.and);
            if (searchResultTrees != null && searchResultTrees.Count == 1)
            {
                treeOperation.updateNodes.setFilterstrategyInPropertiesAndObject(strategyMgr.getSpecifiedFilter().GetType(), searchResultTrees[0]);
            }
            else
            {
                Debug.WriteLine("TODO");
            }
        }

        /// <summary>
        /// Prüft, ob eine Angegebene XML-Datei valide zu dem Schema TemplateUi.xsd ist
        /// </summary>
        /// <param name="pathToXml">Gibt den pfad zu der Xml Datei an</param>
        /// <returns><c>true</c> falls die Datei valide ist; sonst <c>false</c></returns>
        public static bool isTemplateValid(String pathToXml)
        {
            XDocument xDoc;
            if (!File.Exists(@pathToXml)) { Debug.WriteLine("Die XSD exisitert nicht"); return false; }
            try
            {
                xDoc = XDocument.Load(@pathToXml);
            }
            catch (XmlException e)
            {
                Debug.WriteLine("die XML ("+pathToXml+") ist nicht korrekt: " + e);
                return false;
            }
            bool isValid = true;
            String pathToXsd = @"TemplateUi.xsd";
            if (!File.Exists(@pathToXsd)) { Debug.WriteLine("Die XSD exisitert nicht"); return false; }

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
            catch (IOException e) { Debug.WriteLine("Auf die Datei kann nicht zugegriffen werden.\n" + e); return false; }
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
        /// Ermittelt zu einem Punkt den entsprechenden Braille-Knoten
        /// </summary>
        /// <param name="x">gibt die x-Koordinate des Punktes an</param>
        /// <param name="y">gibt die y-Koordinate des Punktes an</param>
        /// <returns></returns>
        public Object getBrailleNodeAtPoint(int x, int y)
        {
            int offsetX, offsetY;
            String viewAtPoint = strategyMgr.getSpecifiedBrailleDisplay().getBrailleUiElementViewNameAtPoint(x, y, out offsetX, out offsetY);
            return treeOperation.searchNodes.getTreeElementOfViewAtPoint(x, y, viewAtPoint, offsetX, offsetY);
        }

        /// <summary>
        /// Prüft, ob das angegebene Template für die gewählte Stiftplatte von der Größe her passend ist
        /// </summary>
        /// <param name="pathToXml">Gibt den pfad zu der Xml Datei an</param>
        /// <returns><c>true</c> falls das Template passend ist; sonst <c> false</c></returns>
        public bool isTemplateUsableForDevice(String pathToXml)
        {
            int tmpHeight, tmpWidth;
            return isTemplateUsableForDevice(@pathToXml, out tmpHeight, out tmpWidth);
        }

        /// <summary>
        /// Prüft, ob das angegebene Template für die gewählte Stiftplatte von der Größe her passend ist
        /// </summary>
        /// <param name="pathToXml">Gibt den pfad zu der Xml Datei an</param>
        /// <param name="minDeviceHeight">gibt die mindest Devicehöhe der Stiftplatte zurück</param>
        /// <param name="minDeviceWidth">gibt die mindest Devicebreite der Stiftplatte zurück</param>
        /// <returns><c>true</c> falls das Template passend ist; sonst <c> false</c></returns>
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
                Debug.WriteLine("die XML (" + pathToXml + ") ist nicht korrekt: " + e);
                return false;
            }
            String pathToXsd = @"TemplateUi.xsd";
            if (!File.Exists(@pathToXsd)) { Debug.WriteLine("Die XSD exisitert nicht"); return false; }
            minDeviceHeight = Convert.ToInt32( xElement.Element("MinDeviceHeight").Value);
            minDeviceWidth = Convert.ToInt32(xElement.Element("MinDeviceWidth").Value);
            Device activeDevice = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice();
            if (activeDevice.height >= minDeviceHeight && activeDevice.width >= minDeviceWidth)
            {
                return true;
            }
            return false;
        }

    }
}
