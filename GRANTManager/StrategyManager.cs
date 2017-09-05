﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager.Interfaces;
using OSMElements;


namespace GRANTManager
{
    public class StrategyManager
    {
        private IFilterStrategy specifiedFilter; //enthält die gewählte Filterstrategy (UIA, Java-Access-Bridge, ...)
        private IOperationSystemStrategy specifiedOperationSystem; // enthält die gewählte Betriebssystemklasse/-methoden (Windows, ...)
        private ITreeStrategy<OSMElements.OSMElement> specifiedTree; // enthält die gewählte Klasse der Baumdarstellung/-verarbeitung
        private IBrailleDisplayStrategy specifiedBrailleDisplay; // enthält die gewählte Klasse für das Ansprechen der Stiftplatte
        private AOutputManager specifiedDisplayStrategy; //enthält Methoden um  mögliche Ausgabegeräte zu erhalten etc.

        private IEventAction specifiedEventAction;
        private IEventManager specifiedEventManager;
        private IEventProcessor specifiedEventProcessor;

        private IEventStrategy_PRISM eventStrategy_PRISM;

        private IGenaralUiTemplate generalUiTemplate;
        private IExternalScreenreader externalScreenreader;
        private IBrailleConverter brailleConverter;

        public void setSpecifiedBrailleConverter(String brailleConverterClassName)
        {
            try
            {
                Type type = Type.GetType(brailleConverterClassName);
                brailleConverter = (IBrailleConverter)Activator.CreateInstance(type);
            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Exception in StrategyManager.setBrailleConverter: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Exception in StrategyManager.setBrailleConverter: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Exception in StrategyManager.setBrailleConverter:: " + e.Message);
            }
        }

        public IBrailleConverter getSpecifiedBrailleConverter()
        {
            return brailleConverter;
        }

        public void setSpecifiedExternalScreenreader(String externalScreenreaderClassName)
        {
            try
            {
                Type type = Type.GetType(externalScreenreaderClassName);
                externalScreenreader = (IExternalScreenreader)Activator.CreateInstance(type, this);
            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Exception in StrategyManager.setExternalScreenreader: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Exception in StrategyManager.setExternalScreenreader: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Exception in StrategyManager.setExternalScreenreader:: " + e.Message);
            }
        }

        public IExternalScreenreader getSpecifiedExternalScreenreader()
        {
            return externalScreenreader;
        }


        public void setSpecifiedEventAction(String eventActionClassName)
        {
            try
            {
                Type type = Type.GetType(eventActionClassName);
                specifiedEventAction = (IEventAction)Activator.CreateInstance(type, this);
            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Exception in StrategyManager_setSpecifiedEventAction: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Exception in StrategyManager_setSpecifiedEventAction: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Exception in StrategyManager_setSpecifiedEventAction: " + e.Message);
            }
        }

        public IEventAction getSpecifiedEventAction() { return specifiedEventAction; }

        public void setSpecifiedEventProcessor(String eventProcessorClassName)
        {
            try
            {
                Type type = Type.GetType(eventProcessorClassName);
                specifiedEventProcessor = (IEventProcessor)Activator.CreateInstance(type, this);
            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Exception in StrategyManager_setSpecifiedEventProcessor: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Exception in StrategyManager_setSpecifiedEventProcessor: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Exception in StrategyManager_setSpecifiedEventProcessor: " + e.Message);
            }
        }

        public IEventProcessor getSpecifiedEventProcessor() { return specifiedEventProcessor; }

        public void setSpecifiedEventManager(String eventManagerClassName)
        {
            try
            {
                Type type = Type.GetType(eventManagerClassName);
                specifiedEventManager = (IEventManager)Activator.CreateInstance(type, this);
            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Exception in StrategyManager_setSpecifiedEventManager: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Exception in StrategyManager_setSpecifiedEventManager: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Exception in StrategyManager_setSpecifiedEventManager: " + e.Message);
            }
        }

        public IEventManager getSpecifiedEventManager()
        {
            return specifiedEventManager;
        }

        /// <summary>
        /// </summary>
        /// <param name="filterClassName">Gibt den Namen der der Klasse der Filterstrategie an (dieser muss in der Strategy.config vorhanden sein)</param>
        public void setSpecifiedEventStrategy(String eventManagerClassName)
        {
            try
            {
                Type type = Type.GetType(eventManagerClassName);
                eventStrategy_PRISM = (IEventStrategy_PRISM)Activator.CreateInstance(type);
                eventStrategy_PRISM.setStrategyMgr(this); //damit beim Manager-Wechsel nicht der Setter vergessen wird
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
        /// Gibt die verwendete EventManagerStrategy PRISM zurück
        /// </summary>
        /// <returns></returns>
        public IEventStrategy_PRISM getSpecifiedEventStrategy()
        {
            return eventStrategy_PRISM;
        }

        /// Setzt die gewählte Klasse für die Braille-Ausgabe
        /// </summary>
        /// <param name="brailleDisplayName"></param>
        public void setSpecifiedBrailleDisplay(String brailleDisplayName)
        {
            try
            {
                Type type = Type.GetType(brailleDisplayName);
                IBrailleDisplayStrategy strategy = getSpecifiedBrailleDisplay();
                if (strategy != null)
                {
                    Type oldAdapter = strategy.getActiveAdapter();
                    if(oldAdapter!= null)
                    {
                        Type newAdapter = Type.GetType(this.getSpecifiedDisplayStrategy().getActiveDevice().deviceClassTypeFullName + ", " + this.getSpecifiedDisplayStrategy().getActiveDevice().deviceClassTypeNamespace);
                        if (oldAdapter.Equals(newAdapter))
                        {
                            return;
                        }
                        else
                        {
                            this.getSpecifiedBrailleDisplay().removeActiveAdapter();
                        }
                    }
                    
                }
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
                specifiedOperationSystem = (IOperationSystemStrategy)Activator.CreateInstance(type, this);
                //specifiedOperationSystem.setStrategyMgr(this); //damit beim Filter-Wechsel nicht der Setter vergessen wird           
            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Fehler bei StrategyManager_setSpecifiedOperationSystem: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Fehler bei StrategyManager_setSpecifiedOperationSystem: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei StrategyManager_setSpecifiedOperationSystem: " + e.Message);
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
                Type[] typeArgs = { typeof(OSMElements.OSMElement) };
                var makeme = type.MakeGenericType(typeArgs);
                specifiedTree = (ITreeStrategy<OSMElements.OSMElement>)Activator.CreateInstance(makeme);
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

        public void setSpecifiedTree(ITreeStrategy<OSMElements.OSMElement> treeName)
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

        public ITreeStrategy<OSMElements.OSMElement> getSpecifiedTree()
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
                //falls MVBD als DisplayStrstegy gesetzt werden soll, muss geprüft werden, ob MVBD aktiv ist
                AOutputManager om = (AOutputManager)Activator.CreateInstance(type, this);
                if (getSpecifiedDisplayStrategy() == null || getSpecifiedDisplayStrategy().isDisplayStrategyAvailable(om))
                {
                    if (this.getSpecifiedBrailleDisplay() != null)
                    {
                        this.getSpecifiedBrailleDisplay().removeAllViews();
                    }
                    specifiedDisplayStrategy = om;

                }
                else
                {
                    Console.WriteLine("Die DisplayStrategy ist nicht verfügbar.");
                    System.Windows.Forms.MessageBox.Show("The chosen display strategy is not available!", "GRANT exception");
                    om.Dispose();
                }
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

        public void setSpecifiedGeneralTemplateUi(String generalUiTemplateClassName)
        {
            try
            {
                Type type = Type.GetType(generalUiTemplateClassName);
                generalUiTemplate = (IGenaralUiTemplate)Activator.CreateInstance(type, this);
            }
            catch (InvalidCastException ic)
            {
                throw new InvalidCastException("Fehler bei StrategyManager_setSpecifiedGeneralTemplateUi: " + ic.Message);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException("Fehler bei StrategyManager_setSpecifiedGeneralTemplateUi: " + ae.Message);

            }
            catch (Exception e)
            {
                throw new Exception("Fehler bei StrategyManager_setSpecifieddisplayStrategy: " + e.Message);
            }
        }

        public IGenaralUiTemplate getSpecifiedGeneralTemplateUi()
        {
            return generalUiTemplate;
        }

    }

}

