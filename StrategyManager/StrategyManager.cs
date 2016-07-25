using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager;
using StrategyManager.Interfaces;
using StrategyManager.AbstractClasses;
using OSMElement;


namespace StrategyManager
{
    public class StrategyMgr
    {
        //in extra Klasse auslagern?
        private ITreeStrategy<OSMElement.OSMElement> filteredTree; // enthält den gefilterten Baum
        private ITreeStrategy<OSMElement.OSMElement> brailleTree; // enthält die Baumdarstellung der UI auf der stiftplatte
        /// <summary>
        /// gibt die Beziehung zwischen <code>filteredTree</code> und <code>brailleTree</code> anhand der generierten Id an
        /// </summary>
        private List<OsmRelationship<String, String>> osmRelationship = new List<OsmRelationship<string,string>>();

        private IFilterStrategy specifiedFilter; //enthält die gewählte Filterstrategy (UIA, Java-Access-Bridge, ...)
        private IOperationSystemStrategy specifiedOperationSystem; // enthält die gewählte Betriebssystemklasse/-methoden (Windows, ...)
        private ITreeStrategy<OSMElement.OSMElement> specifiedTree; // enthält die gewählte Klasse der Baumdarstellung/-verarbeitung
        private IBrailleDisplayStrategy specifiedBrailleDisplay; // enthält die gewählte Klasse für das Ansprechen der Stiftplatte
        private AbstractDisplayStrategy specifiedDisplayStrategy; //enthält Methoden um  mögliche Ausgabegeräte zu erhalten etc.

        private ITreeOperations<OSMElement.OSMElement> specifiedTreeOperations;

        /// <summary>
        /// Setz die Klasse für spezielle Baum-Operationen
        /// </summary>
        /// <param name="treeClassNames"></param>
        public void setSpecifiedTreeOperations(String treeOperationsClassName)
        {
            try
            {
                Type type = Type.GetType(treeOperationsClassName);
                Type[] typeArgs = { typeof(OSMElement.OSMElement) };
                var makeme = type.MakeGenericType(typeArgs);
                specifiedTreeOperations = (ITreeOperations<OSMElement.OSMElement>)Activator.CreateInstance(makeme);
            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Fehler bei StrategyManager_setSpecifiedTreeOperationsr: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Fehler bei StrategyManager_setSpecifiedTreeOperations: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei StrategyManager_setSpecifiedTreeOperations: " + e.Message);
            }
        }

        public ITreeOperations<OSMElement.OSMElement> getSpecifiedTreeOperations()
        {
            return specifiedTreeOperations;
        }

        /// <summary>
        /// Setzt den aktuell gefilterten Baum
        /// </summary>
        /// <param name="tree">gibt den gefilterten Baum an</param>
        public void setFilteredTree(ITreeStrategy<OSMElement.OSMElement> tree)
        {
            filteredTree = tree;
        }

        /// <summary>
        /// Gibt den gefilterten Baum zurück
        /// </summary>
        /// <returns>Ger gefilterte Baum</returns>
        public ITreeStrategy<OSMElement.OSMElement> getFilteredTree()
        {
            return filteredTree;
        }

        /// <summary>
        /// Setzt die aktuelle Barille-UI-Darstellung.
        /// </summary>
        /// <param name="tree"></param>
        public void setBrailleTree(ITreeStrategy<OSMElement.OSMElement> tree)
        {
            brailleTree = tree;
        }

        /// <summary>
        /// Gibt die aktuelle Braille-UI-Darstellung zurück
        /// </summary>
        /// <returns>Braille-UI-Darstellung</returns>
        public ITreeStrategy<OSMElement.OSMElement> getBrailleTree()
        {
            return brailleTree;
        }

        /// <summary>
        /// Gibt die Beziehungen zwischen <value>brailleTree</value> und <value>filteredTree</value> an
        /// </summary>
        /// <returns>Beziehungen zwischen <value>brailleTree</value> und <value>filteredTree</value></returns>
        public List<OsmRelationship<String, String>> getOsmRelationship()
        {
            return osmRelationship;
        }

        /// <summary>
        /// Setzt die Beziehungen zwischen <value>brailleTree</value> und <value>filteredTree</value>
        /// </summary>
        /// <param name="relationship"></param>
        public void setOsmRelationship(List<OsmRelationship<String, String>> relationship)
        {
            osmRelationship = relationship;
        }

        /// <summary>
        /// Setzt die gewählte Klasse für die Braille-Ausgabe
        /// </summary>
        /// <param name="brailleDisplayName"></param>
        public void setSpecifiedBrailleDisplay(String brailleDisplayName)
        {
            try
            {
                Type type = Type.GetType(brailleDisplayName);
                specifiedBrailleDisplay = (IBrailleDisplayStrategy)Activator.CreateInstance(type);

            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Fehler bei StrategyManager_setSpecifiedBrailleDisplay: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Fehler bei StrategyManager_setSpecifiedBrailleDisplay: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei StrategyManager_setSpecifiedBrailleDisplay: " + e.Message);
            }
        }

        /// <summary>
        /// Gibt die gewählte Braille-ausgabe zurück.
        /// </summary>
        /// <returns></returns>
        public IBrailleDisplayStrategy getSpecifiedBrailleDisplay()
        {
            return specifiedBrailleDisplay;
        }

        /// <summary>
        /// Setzt die zuverwendene Filterstrategie (UIA, Java-Access-Bridge, ...)
        /// </summary>
        /// <param name="filterClassName">Gibt den Namen der der Klasse der Filterstrategie an (dieser muss in der Strategy.config vorhanden sein)</param>
        public void setSpecifiedFilter(String filterClassName)
        {
            try
            {
                Type type = Type.GetType(filterClassName);
                specifiedFilter = (IFilterStrategy)Activator.CreateInstance(type);
                specifiedFilter.setStrategyMgr(this); //damit beim Filter-Wechsel nicht der Setter vergessen wird
            }

            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Fehler bei StrategyManager_setSpecifiedFilter: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Fehler bei StrategyManager_setSpecifiedFilter: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei StrategyManager_setSpecifiedFilter: " + e.Message);
            }
        }

        /// <summary>
        /// Gibt die verwendete Filterstrategie zurück (UIA, Java-Access-Bridge, ...)
        /// </summary>
        /// <returns></returns>
        public IFilterStrategy getSpecifiedFilter()
        {
            return specifiedFilter;
        }


        /// <summary>
        /// Setzt die gewählte Betriebssystemklasse/-methoden (Windows, ...)
        /// </summary>
        /// <param name="operationSystemClassName">gibt den Namen der gewählten Betriebssystem-Klasse an (dieser muss in der Strategy.config vorhanden sein)</param>
        public void setSpecifiedOperationSystem(String operationSystemClassName)
        {
            try
            {
                Type type = Type.GetType(operationSystemClassName);
                specifiedOperationSystem = (IOperationSystemStrategy)Activator.CreateInstance(type);
            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Fehler bei StrategyManager_setSpecifiedFilter: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Fehler bei StrategyManager_setSpecifiedFilter: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei StrategyManager_setSpecifiedFilter: " + e.Message);
            }

        }

        /// <summary>
        /// gibt die gewählte Betriebssystemklasse/-methoden (Windows, ...) zurück
        /// </summary>
        /// <returns></returns>
        public IOperationSystemStrategy getSpecifiedOperationSystem()
        {
            return specifiedOperationSystem;
        }


        /// <summary>
        /// Setzt die gewählte Klasse der Baumdarstellung/-verarbeitung
        /// </summary>
        /// <param name="treeClassName">gibt den Namen der gewählten Baum-klasse an (dieser muss in der Strategy.config vorhanden sein)</param>
        public void setSpecifiedTree(String treeClassName)
        {
            try
            {
                Type type = Type.GetType(treeClassName);
                Type[] typeArgs = { typeof(OSMElement.OSMElement) };
                var makeme = type.MakeGenericType(typeArgs);
                specifiedTree = (ITreeStrategy<OSMElement.OSMElement>)Activator.CreateInstance(makeme);
            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Fehler bei StrategyManager_setSpecifiedFilter: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Fehler bei StrategyManager_setSpecifiedFilter: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei StrategyManager_setSpecifiedFilter: " + e.Message);
            }
        }

        public void setSpecifiedTree(ITreeStrategy<OSMElement.OSMElement> treeName)
        {
            try
            {
                treeName.NewNodeTree();
                specifiedTree = treeName;
            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei StrategyManager_setSpecifiedFilter: " + e.Message);
            }
        }

        public ITreeStrategy<OSMElement.OSMElement> getSpecifiedTree()
        {
            return specifiedTree;
        }

        public void setSpecifiedDisplayStrategy(String displayStrategyClassName)
        {
            try
            {
                Type type = Type.GetType(displayStrategyClassName);
                specifiedDisplayStrategy = (AbstractDisplayStrategy)Activator.CreateInstance(type, this);
            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Fehler bei StrategyManager_setSpecifieddisplayStrategy: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Fehler bei StrategyManager_setSpecifieddisplayStrategy: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei StrategyManager_setSpecifieddisplayStrategy: " + e.Message);
            }
        }

        public AbstractDisplayStrategy getSpecifiedDisplayStrategy()
        {
            return specifiedDisplayStrategy;
        }

    }

}

