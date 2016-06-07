using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager;
using StrategyManager.Interfaces;
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
        private List<osmRelationship.OsmRelationship<String, String>> osmRelationship;

        private IFilterStrategy specifiedFilter; //enthält die gewählte Filterstrategy (UIA, Java-Access-Bridge, ...)
        private IOperationSystemStrategy specifiedOperationSystem; // enthält die gewählte Betriebssystemklasse/-methoden (Windows, ...)
        private ITreeStrategy<OSMElement.OSMElement> specifiedTree; // enthält die gewählte Klasse der Baumdarstellung/-verarbeitung
        private IBrailleDisplayStrategy specifiedBrailleDisplay; // enthält die gewählte Klasse für das Ansprechen der Stiftplatte

        public void setFilteredTree(ITreeStrategy<OSMElement.OSMElement> tree)
        {
            filteredTree = tree;
        }

        public ITreeStrategy<OSMElement.OSMElement> getFilteredTree()
        {
            return filteredTree;
        }

        public void setBrailleTree(ITreeStrategy<OSMElement.OSMElement> tree)
        {
            brailleTree = tree;
        }

        public List<osmRelationship.OsmRelationship<String, String>> getOsmRelationship()
        {
            return osmRelationship;
        }


        public void setOsmRelationship(List<osmRelationship.OsmRelationship<String, String>> relationship)
        {
            osmRelationship = relationship;
        }

        public ITreeStrategy<OSMElement.OSMElement> getBrailleTree()
        {
            return brailleTree;
        }

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

        public IBrailleDisplayStrategy getSpecifiedBrailleDisplay()
        {
            return specifiedBrailleDisplay;
        }

        public void setSpecifiedFilter(String filterName)
        {
            try {
                Type type = Type.GetType(filterName);
                specifiedFilter = (IFilterStrategy)Activator.CreateInstance(type);
                specifiedFilter.setSpecifiedOperationSystem(specifiedOperationSystem);
                specifiedFilter.setSpecifiedTree(specifiedTree);
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
        
        public IFilterStrategy getSpecifiedFilter()
        {
            return specifiedFilter;
        }

         
        public void setSpecifiedOperationSystem(String operationSystemName)
        {
            try {
                Type type = Type.GetType(operationSystemName);
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

        public IOperationSystemStrategy getSpecifiedOperationSystem()
        {
            return specifiedOperationSystem;
        }

        

        public void setSpecifiedTree(String treeName)
        {
            try {
                Type type = Type.GetType(treeName);
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
            try {
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

    }

    }

