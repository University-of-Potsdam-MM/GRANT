using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager.Interfaces;
using GRANTManager.AbstractClasses;
using OSMElement;


namespace GRANTManager
{
    public class StrategyManager
    {
        private IFilterStrategy specifiedFilter; //enthält die gewählte Filterstrategy (UIA, Java-Access-Bridge, ...)
        private IOperationSystemStrategy specifiedOperationSystem; // enthält die gewählte Betriebssystemklasse/-methoden (Windows, ...)
        private ITreeStrategy<OSMElement.OSMElement> specifiedTree; // enthält die gewählte Klasse der Baumdarstellung/-verarbeitung
        private IBrailleDisplayStrategy specifiedBrailleDisplay; // enthält die gewählte Klasse für das Ansprechen der Stiftplatte
        private AOutputManager specifiedDisplayStrategy; //enthält Methoden um  mögliche Ausgabegeräte zu erhalten etc.

        private IEventManagerStrategy eventManager;

        /// <summary>
        /// </summary>
        /// <param name="filterClassName">Gibt den Namen der der Klasse der Filterstrategie an (dieser muss in der Strategy.config vorhanden sein)</param>
        public void setSpecifiedEventManager(String eventManagerClassName)
        {
            try
            {
                Type type = Type.GetType(eventManagerClassName);
                eventManager = (IEventManagerStrategy)Activator.CreateInstance(type);
                eventManager.setStrategyMgr(this); //damit beim Manager-Wechsel nicht der Setter vergessen wird
            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Fehler bei StrategyManager_setSpecifiedEventManager: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Fehler bei StrategyManager_setSpecifiedEventManager: " + ae.Message);
            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei StrategyManager_setSpecifiedEventManager: " + e.Message);
            }
        }

        /// <summary>
        /// Gibt die verwendete Filterstrategie zurück (UIA, Java-Access-Bridge, ...)
        /// </summary>
        /// <returns></returns>
        public IEventManagerStrategy getSpecifiedEventManager()
        {
            return eventManager;
        }
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
                throw new InvalidCastException("Fehler bei StrategyManager_setSpecifiedTree: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Fehler bei StrategyManager_setSpecifiedTree: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei StrategyManager_setSpecifiedTree: " + e.Message);
            }
        }

        public void setSpecifiedTree(ITreeStrategy<OSMElement.OSMElement> treeName)
        {
            try
            {
                treeName.NewTree();
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
                if (specifiedDisplayStrategy != null)
                {                    
                    specifiedDisplayStrategy.Dispose(); //sorgt dafür, dass ggf. die alte TCP-Verbindung beendet wird
                  //  specifiedDisplayStrategy = null;
                }
                specifiedDisplayStrategy = (AOutputManager)Activator.CreateInstance(type, this);
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

        public AOutputManager getSpecifiedDisplayStrategy()
        {
            return specifiedDisplayStrategy;
        }

    }

}

