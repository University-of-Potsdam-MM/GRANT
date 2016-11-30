using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager;
using GRANTManager.TreeOperations;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using OSMElement;
using System.Windows;
using System.Xml.Serialization;
using System.Diagnostics;

namespace FilteredTreeTest
{
    [TestClass]
    public class UnitTestSaveTree
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        SearchNodes searchNodes;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;


        [TestInitialize]
        public void Initialize()
        {
            strategyMgr = new StrategyManager();
            grantTrees = new GeneratedGrantTrees();
            Settings settings = new Settings();
            searchNodes = new SearchNodes(strategyMgr, grantTrees, treeOperation);
            treeOperation = new TreeOperation(strategyMgr, grantTrees);
            
            strategyMgr.setSpecifiedTree(settings.getPossibleTrees()[0].className);
            strategyMgr.setSpecifiedEventManager(settings.getPossibleEventManager()[0].className);
            strategyMgr.setSpecifiedFilter(settings.getPossibleFilters()[0].className);
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            List<GRANTManager.Strategy> posibleOS = settings.getPossibleOperationSystems();
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);

        }

        private TestContext m_testContext;

        public TestContext TestContext
        {
            // https://blogs.msdn.microsoft.com/vstsqualitytools/2006/01/09/using-testcontext-in-unit-tests/
            get { return m_testContext; }

            set { m_testContext = value; }

        }

        [TestMethod]
        public void TestSaveTree()
        {
            String applicationName = "calc.exe";
            String applicationPathName = @"C:\Windows\system32\calc.exe";
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(applicationName, applicationPathName);
            if (grantTrees.getFilteredTree() == null) { Assert.Fail("Es ist kein gefilterter Baum vorhanden"); return; }
            Console.WriteLine("m_testContext.DeploymentDirectory: " + m_testContext.DeploymentDirectory);
            String folderPathSave = System.IO.Path.Combine(m_testContext.DeploymentDirectory, "filter_test");
            guiFuctions.saveProject(folderPathSave);
            String projectLoadedPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SavedTrees");
            projectLoadedPath = System.IO.Path.Combine(projectLoadedPath, "filteredTree_RechnerUIA");
          //  String @folderPathSaveTree = Path.GetDirectoryName(@folderPathSave) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(@folderPathSave);
            //String @projectPathTree = Path.GetDirectoryName(projectLoadedPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(projectLoadedPath);


            if (!CompareToSavedFilteredTrees(System.IO.Path.Combine(folderPathSave, "filteredTree.xml"),System.IO.Path.Combine(projectLoadedPath, "filteredTree.xml")))
            {
                Assert.Fail("Der gerade gespeicherte Baum stimmt nicht mit den 'Test-Baum' überein!");
            }
        }



        /// <summary>
        /// vergleicht einen 'vorhandenen' gespeicherten Baum mit einen gerade erstellten gespeicherten Baum
        /// </summary>
        public bool CompareToSavedFilteredTrees(String path1, String path2)
        {
            if (!File.Exists(@path1))
            {
                Assert.Fail("Die Datei ({0}) existiert nicht!", path1);
                return false;
            }
            if (!File.Exists(@path2))
            {
                Assert.Fail("Die Datei ({0}) existiert nicht!", path2);
                return false;
            }
            Object loadedTree1;
            Object loadedTree2;
            using (System.IO.FileStream fs = System.IO.File.Open(path1, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
               loadedTree1 = strategyMgr.getSpecifiedTree().XmlDeserialize(fs);
            }
            using (System.IO.FileStream fs = System.IO.File.Open(path2, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                loadedTree2 = strategyMgr.getSpecifiedTree().XmlDeserialize(fs);
            }
            //  if (loadedTree1.Equals(loadedTree2)) { return true; } else { return false; } --> geht nicht da boundingRectangle unterschiedlich sein kann
            String node1Id;
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(loadedTree1))
            {
                node1Id = strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated;
                List<Object> associatedNodeList = searchNodes.getAssociatedNodeList(node1Id, loadedTree2);
                if (associatedNodeList.Count != 1) { Assert.Fail("Die Id '{0}' kommt mehr als ein mal oder keinmal in dem Baum ({1}) vor!", node1Id, path2); return false; }
                OSMElement.OSMElement osm1 = strategyMgr.getSpecifiedTree().GetData(node);
                GeneralProperties prop1 = osm1.properties;
                prop1.boundingRectangleFiltered = new Rect();
                prop1.fileName = null;
                //bei Textfeldern kann sich der Text ändern
                if (prop1.controlTypeFiltered.Equals("Text"))
                {
                    prop1.nameFiltered = "";
                }
                osm1.properties = prop1;
                strategyMgr.getSpecifiedTree().SetData(node, osm1);
                OSMElement.OSMElement osm2 = strategyMgr.getSpecifiedTree().GetData(associatedNodeList[0]);
                GeneralProperties prop2 = osm2.properties;
                prop2.boundingRectangleFiltered = new Rect();
                prop2.fileName = null;
                //bei Textfeldern kann sich der Text ändern
                if (prop2.controlTypeFiltered.Equals("Text"))
                {
                    prop2.nameFiltered = "";
                }
                osm2.properties = prop2;
                strategyMgr.getSpecifiedTree().SetData(associatedNodeList[0], osm2);
                Assert.AreEqual(true, hf.compareToNodes(node, associatedNodeList[0]), "Die beiden knoten stimmen nicht überein!");

                /* if (!strategyMgr.getSpecifiedTree().Equals(node, associatedNodeList[0]))
                 {
                     compareToNodes(node, associatedNodeList[0]);
                     Assert.Fail("Die folgenden beiden Knoten stimmen nicht überein:\n{0}\n{1}", node, associatedNodeList[0]);
                     return false;
                 }*/
            }
            return true;

        }

        /// <summary>
        /// Vergleicht zwei Projekt-Datein
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public bool CompareProjectFile(String path1, String path2)
        {
            if (!File.Exists(@path1))
            {
                Assert.Fail("Die Datei ({0}) existiert nicht!", path1);
                return false;
            }
            if (!File.Exists(@path2))
            {
                Assert.Fail("Die Datei ({0}) existiert nicht!", path2);
                return false;
            }

            XmlSerializer serializer;
            System.IO.FileStream fs = null;
            GrantProjectObject projectFile1;
            GrantProjectObject projectFile2;
            try
            {
                fs = System.IO.File.Open(path1, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                using (StreamReader reader = new StreamReader(fs))
                {
                    fs = null;
                    serializer = new XmlSerializer(typeof(GrantProjectObject));
                    projectFile1 = (GrantProjectObject)serializer.Deserialize(reader);
                }
            }
            finally
            {
                if (fs != null) { fs.Dispose(); }
            }
            try
            {
                fs = System.IO.File.Open(path2, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                using (StreamReader reader = new StreamReader(fs))
                {
                    fs = null;
                    serializer = new XmlSerializer(typeof(GrantProjectObject));
                    projectFile2 = (GrantProjectObject)serializer.Deserialize(reader);
                }
            }
            finally
            {
                if (fs != null) { fs.Dispose(); }
            }
            if (!projectFile1.Equals(projectFile2)) { Assert.Fail("Die beiden Projekt-Dateien sind nicht gleich!"); return false; }
            return true;
        }
    }
}
