using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager;
using StrategyManager.Interfaces;


namespace StrategyManager
{
    public class StrategyMgr
    {

        private IFilterStrategy specifiedFilter;
        private IOperationSystemStrategy specifiedOperationSystem;
        //private ITreeStrategy<GeneralProperties> specifiedTree;
        private ITreeStrategy<GeneralProperties> specifiedTree;

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
                Type[] typeArgs = { typeof(GeneralProperties) };
                var makeme = type.MakeGenericType(typeArgs);
                specifiedTree = (ITreeStrategy<GeneralProperties>)Activator.CreateInstance(makeme);
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

        public void setSpecifiedTree(ITreeStrategy<GeneralProperties> treeName)
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

        public ITreeStrategy<GeneralProperties> getSpecifiedTree()
        {
            return specifiedTree;
        }

    }

    }

