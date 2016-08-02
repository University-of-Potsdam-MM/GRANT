using System;

using System.Diagnostics;
using GRANTManager.Interfaces;
using OSMElement;

using System.Collections.ObjectModel;
using System.Windows;

namespace GRANTManager
{

    public class GuiFunctions
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTree;

        public GuiFunctions(StrategyManager strategyMgr, GeneratedGrantTrees grantTree)
        {
            this.strategyMgr = strategyMgr;
            this.grantTree = grantTree;
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
        private void rootMenuItemCheckSibling(ref MenuItem root, ITreeStrategy<OSMElement.OSMElement> siblingNode)
        {
            if (siblingNode.HasParent && !siblingNode.Parent.Data.properties.IdGenerated.Equals(root.IdGenerated))
            {
                Console.WriteLine();
                if (root.parentMenuItem.IdGenerated.Equals(siblingNode.Parent.Data.properties.IdGenerated))
                {
                    root = root.parentMenuItem;
                    rootMenuItemCheckSibling(ref root, siblingNode);
                }
            }
        }

        public void treeIteration(ITreeStrategy<OSMElement.OSMElement> tree, ref GuiFunctions.MenuItem root)
        {

            ITreeStrategy<OSMElement.OSMElement> node1;

            while (tree.HasChild && !(tree.Count == 1 && tree.Depth == -1))
            {


                MenuItem child = new MenuItem();
                node1 = tree.Child;
                child.controlTypeFiltered = node1.Data.properties.controlTypeFiltered == null ? " " : node1.Data.properties.controlTypeFiltered;
                child.IdGenerated = node1.Data.properties.IdGenerated == null ? " " : node1.Data.properties.IdGenerated;
                String nameFiltered = node1.Data.properties.nameFiltered == null ? " " : node1.Data.properties.nameFiltered;
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


                if (!tree.IsRoot)
                {
                    child.parentMenuItem = root;

                    root.Items.Add(child);
                }
                else { root = child; };

                if (node1.HasChild)
                {
                    treeIteration(node1, ref child);
                }
                else
                {
                    if (node1.IsFirst && node1.IsLast)
                    {
                        root = root.parentMenuItem == null ? root : root.parentMenuItem;
                    }
                    treeIteration(node1, ref root);
                }


            }
            while (tree.HasNext)
            {
                MenuItem sibling = new MenuItem();
                node1 = tree.Next;
                //Pruefung, ob wir es ans richtige Element ranhängen und ggf. korrigieren
                rootMenuItemCheckSibling(ref root, tree);

                sibling.controlTypeFiltered = node1.Data.properties.controlTypeFiltered == null ? " " : node1.Data.properties.controlTypeFiltered;
                sibling.IdGenerated = node1.Data.properties.IdGenerated == null ? " " : node1.Data.properties.IdGenerated;
                String nameFiltered = node1.Data.properties.nameFiltered == null ? " " : node1.Data.properties.nameFiltered.ToString();
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

                if (node1.HasChild)
                {
                    treeIteration(node1, ref sibling);
                }
                else
                {
                    if (node1.IsFirst && node1.IsLast)
                    {
                        root = root.parentMenuItem == null ? root : root.parentMenuItem;
                    }
                    if (!node1.HasChild && !node1.HasNext)
                    {
                        root = root.parentMenuItem == null ? root : root.parentMenuItem;
                    }
                    treeIteration(node1, ref root);
                }
            }
            if (tree.Count == 1 && tree.Depth == -1)
            {
                
                return;
            }
            if (!tree.HasChild)
            {
                node1 = tree;
                if (tree.HasParent)
                {
                    node1.Remove();
                    return;
                }
            }
            if (tree.IsFirst && tree.IsLast)
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
            if (grantTree == null || grantTree.getFilteredTree() == null) { Console.WriteLine("Es ist kein gefilterter Baum vorhanden."); }
            System.IO.FileStream fs = System.IO.File.Create(filePath);
            grantTree.getFilteredTree().XmlSerialize(fs);
            fs.Close();
        }

        /// <summary>
        /// Lädt eine gefilterten Baum und speichert das Ergebnis im <c>GeneratedGrantTrees</c>
        /// </summary>
        /// <param name="filePath">gibt den Dateipfad + Name an</param>
        public void loadFilteredTree(String filePath)
        {
            System.IO.FileStream fs = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            ITreeStrategy<OSMElement.OSMElement> loadedTree = strategyMgr.getSpecifiedTree().XmlDeserialize(fs);
            fs.Close();
            //Baum setzen
            grantTree.setFilteredTree(loadedTree);

            //Filter-Strategy setzen
            if (grantTree.getFilteredTree() != null && grantTree.getFilteredTree().HasChild && !grantTree.getFilteredTree().Child.Data.Equals(new OSMElement.OSMElement()) && !grantTree.getFilteredTree().Child.Data.properties.Equals(new GeneralProperties()))
            {
                if (grantTree.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName != null && grantTree.getFilteredTree().Child.Data.properties.grantFilterStrategyNamespace != null)
                {
                    strategyMgr.setSpecifiedFilter(grantTree.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName + ", " + grantTree.getFilteredTree().Child.Data.properties.grantFilterStrategyNamespace);
                    strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTree);
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
            if (grantTree != null && grantTree.getFilteredTree() != null && grantTree.getFilteredTree().HasChild)
            {
                if (grantTree.getFilteredTree().Child.Data.properties.Equals(new GeneralProperties()) || grantTree.getFilteredTree().Child.Data.properties.moduleName == null)
                {
                    Debug.WriteLine("Kein Daten im 1. Knoten Vorhanden.");
                    return false;
                }
                IntPtr appIsRunnuing = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(grantTree.getFilteredTree().Child.Data.properties.moduleName);
                Debug.WriteLine("App ist gestartet: {0}", appIsRunnuing);
                if (appIsRunnuing.Equals(IntPtr.Zero))
                {
                    if (grantTree.getFilteredTree().Child.Data.properties.fileName != null)
                    {
                        bool openApp = strategyMgr.getSpecifiedOperationSystem().openApplication(grantTree.getFilteredTree().Child.Data.properties.fileName);
                        if (!openApp)
                        {
                            Debug.WriteLine("Anwendung konnte nicht geöffnet werden! Ggf. Pfad der Anwendung anpassen."); //TODO
                        }
                        else { return true; }
                    }
                }
                else { return true; }
            }
            return false;
        }

        public void filteredLoadedApplication()
        {
            //ist nur notwendig, wenn die Anwendung zwischendurch zu war (--> hwnd's vergleichen) oder die Anwendung verschoben wurde (--> Rect's vergleichen)
            ITreeStrategy<OSMElement.OSMElement> loadedTree = grantTree.getFilteredTree();
            IntPtr hwnd = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(loadedTree.Child.Data.properties.moduleName);
            if (hwnd.Equals(IntPtr.Zero)) { throw new Exception("Der HWND der Anwendung konnte nicht gefunden werden!"); }

            strategyMgr.getSpecifiedTreeOperations().updateTree(hwnd);

        }
    }
}
