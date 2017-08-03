using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GRANTManager;
using System.Windows.Automation;
using System.Diagnostics;
using GRANTManager.Interfaces;
using OSMElement;
using System.Windows;

using Prism.Events;

//https://msdn.microsoft.com/en-us/library/ff649187.aspx
//https://github.com/PrismLibrary/Prism/blob/master/Documentation/WPF/70-CommunicatingBetweenLooselyCoupledComponents.md
//http://www.codeproject.com/Articles/355473/Prism-EventAggregator-Sample


namespace StrategyUIA
{
    #region Events
    /// <summary>
    /// todo
    /// Abfangen des events eines button funktioniert, auch für test-wpf-app
    /// next step: events verallgemeinern + als stratgy-pattern nutzen
    /// Events für Creation, Deletion, Update, Invoke (Aufruf), Focuswechsel
    /// </summary>
    public class UIAEventsMonitor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appHWND"></param>
        public void eventsUIA_withHWND(IntPtr appHWND)
        {
            try
            {
                //die Methode getProcessHwndFromHwnd liefert das GUElemtn der eigentlichen Anwendung
                //appHWND = operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points));

                Console.WriteLine("appHWND: '{0}'", appHWND.ToString());

                //todo apphwnd durchgehen mittels treescope und den ersten button der app geben lassen
                //todo//AutomationElement at = FilterStrategyUIA.deliverAutomationElementFromHWND(appHWND);

                AutomationElement at = FilterStrategyUIA.deliverAutomationElementFromHWND(appHWND);

                SubscribeToInvoke(at);

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: '{0}'", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appHWND"></param>
        public void eventsUIA_withAutomationElement(AutomationElement mainElement)
        {
            try
            {
                //die Methode getProcessHwndFromHwnd liefert das GUElemtn der eigentlichen Anwendung
                //appHWND = operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points));

                Console.WriteLine("AutomationElement: '{0}'", mainElement.Current.LocalizedControlType.ToString());

                //todo apphwnd durchgehen mittels treescope und den ersten button der app geben lassen
                //AutomationElement at = FilterStrategyUIA.deliverAutomationElementFromHWND(appHWND);

                SubscribeToInvoke(mainElement);

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: '{0}'", ex);
            }
        }


        #region UIA_Automation_Events_Automation
        //Automation.AddAutomationEventHandler Method (AutomationEvent, AutomationElement, TreeScope, AutomationEventHandler)
        //https://msdn.microsoft.com/en-us/library/system.windows.automation.automation.addautomationeventhandler%28v=vs.110%29.aspx

        // Member Variables
        AutomationElement ElementSubscribeButton;
        AutomationEventHandler UIAeventHandler;
        
        //Angabe in welcher Ebene des Baumes gesucht werden soll, bzw. hier von welchem automationelement in der GUI aus weitere elementereignisse betrachtete werden sollen
        TreeScope treeScope = TreeScope.Element;

        //public StrategyManager strategyMgr;

        //public EventAggregator_PRISM_UIA cea = new EventAggregator_PRISM_UIA(strategyMgr);

        public EventAggregator_PRISM_UIA cea = new EventAggregator_PRISM_UIA();

        /// <summary>
        /// Register an event handler for InvokedEvent on the specified element.
        /// 
        /// eventdoku 06.07.2016
        /// Für das Anmelden der "Aktivierung des Buttonelements"/den Klick auf den Button wird sich durch drücken der taste f7 mit dem button unter grantexample registriert 
        /// es wird nur das Automationelement unter dem mauszeiger gefiltert und in dieser subscribetoinvokemthode nur ei element für events berücksichtigt, auswahl über treescope
        /// unsubscribe für das event einbauen!
        /// </summary>
        /// <param name="elementButton">The automation element.</param>
        public void SubscribeToInvoke(AutomationElement elementButton)
        {

            //auswahl über treescope welche elemente für events betrachtete werden sollen
            //treeScope = TreeScope.Descendants;

            treeScope = TreeScope.Element;

            if (elementButton != null)
            {
                // hier wurde auf descendants/Nachkommen geändert und damit sollten alle events der eigentlichen hwnd anwendung berücksichtigt werden
                Automation.AddAutomationEventHandler(InvokePattern.InvokedEvent,
                     elementButton, treeScope,
                     UIAeventHandler = new AutomationEventHandler(OnUIAutomationEvent));
                ElementSubscribeButton = elementButton;
            }
        }

        /// <summary>
        /// AutomationEventHandler delegate.
        /// </summary>
        /// <param name="src">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnUIAutomationEvent(object src, AutomationEventArgs e)
        {
            // Make sure the element still exists. Elements such as tooltips
            // can disappear before the event is processed.
            AutomationElement sourceElement;
            try
            {
                sourceElement = src as AutomationElement;
            }
            catch (ElementNotAvailableException)
            {
                return;
            }
            if (e.EventId == InvokePattern.InvokedEvent)
            {
                // TODO Add handling code.
                Console.WriteLine("InvokedEvent raised '{0}'", sourceElement.ToString(), sourceElement.Current.LocalizedControlType.ToString());

                Console.WriteLine("Event ausgelöst durch Klick auf Button");

                //todo
                //classEventAggregator cea = new classEventAggregator();

                //nur einmaliges subscriben erlauben von einer klasse aus, nicht mehrmalig für dasselbe event anmelden!
                //Prism-subscribe passiert von der klasse aus, welche über das prism-event informiert weden möchte
                //dieses subscribe hier sollte natürlich nur einmal bei initialisierung in eventverarbeitender klasse erfolgen
                cea.aggSubscribe();
                
                //das publish passiert auch hier, das subscribe kann in einer anderen klasse sein, das publish muss/sollte nach dem grant-konzept dort sein, wo das egentliche event zuerst abgefangen wird, also bei uia in dem handler, 
                //welcher das event zuerst verarbeitet

                //cea.agg.GetEvent<stringOSMEvent>().Publish("Wurf aus UIAEventsMonitorKlasse");
                //kann genutzt werden
                //oder über den
                //aufruf des publisher in einer methode, wie eventOsmChangedHandler(); mit dieser art und weise könnte dann allerdings auch direkt der aufruf von methoden, welche das allererste event verarbeiten wollen, erfolgen
                // es soll aber so sein, dass nach dem ersten event ein neues event über publish geworfen wird und sämtliche unterschiedliche arten von events dieses event werfen 
                //und damit dann über prism eine ordentliche verarbeitung der events erfolgt, in der methode "eventosmchangedhandler" wird das neue event ge-published und die eigentliche verarbeitung über handler kann dann ganz woanders erfolgen
                // was den publisher eben nicht kümmert, wie und wo es verarbeitet wird, derzeit wird es in generateosm(string osm) verarbeitet
                cea.eventOsmChangedHandler();

            }
            else
            {
                // TODO Handle any other events that have been subscribed to.
            }
        }

        private void ShutdownUIA()
        {
            if (UIAeventHandler != null)
            {
                Automation.RemoveAutomationEventHandler(InvokePattern.InvokedEvent,
                    ElementSubscribeButton, UIAeventHandler);
            }
        }
        #endregion

        #region UIA_Automation_Events_Property
        //Automation.AddAutomationPropertyChangedEventHandler Method (AutomationElement, TreeScope, AutomationPropertyChangedEventHandler, AutomationProperty[])
        //https://msdn.microsoft.com/en-us/library/system.windows.automation.automation.addautomationpropertychangedeventhandler%28v=vs.110%29.aspx

        AutomationPropertyChangedEventHandler propChangeHandler;
        /// <summary>
        /// Adds a handler for property-changed event; in particular, a change in the enabled state.
        /// </summary>
        /// <param name="element">The UI Automation element whose state is being monitored.</param>
        public void SubscribePropertyChange(AutomationElement element)
        {
            Automation.AddAutomationPropertyChangedEventHandler(element,
                TreeScope.Element,
                propChangeHandler = new AutomationPropertyChangedEventHandler(OnPropertyChange),
                AutomationElement.IsEnabledProperty);

        }

        /// <summary>
        /// Handler for property changes.
        /// </summary>
        /// <param name="src">The source whose properties changed.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPropertyChange(object src, AutomationPropertyChangedEventArgs e)
        {
            AutomationElement sourceElement = src as AutomationElement;
            if (e.Property == AutomationElement.IsEnabledProperty)
            {
                bool enabled = (bool)e.NewValue;
                // TODO: Do something with the new value. 
                // The element that raised the event can be identified by its runtime ID property.
            }
            else
            {
                // TODO: Handle other property-changed events.
            }
        }

        public void UnsubscribePropertyChange(AutomationElement element)
        {
            if (propChangeHandler != null)
            {
                Automation.RemoveAutomationPropertyChangedEventHandler(element, propChangeHandler);
            }
        }

        #endregion

        #region UIA_Automation_events_Focus

        AutomationFocusChangedEventHandler focusHandler = null;

        /// <summary>
        /// Create an event handler and register it.
        /// </summary>
        public void SubscribeToFocusChange()
        {
            focusHandler = new AutomationFocusChangedEventHandler(OnFocusChange);
            Automation.AddAutomationFocusChangedEventHandler(focusHandler);
        }

        /// <summary>
        /// Handle the event.
        /// </summary>
        /// <param name="src">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnFocusChange(object src, AutomationFocusChangedEventArgs e)
        {
            // TODO Add event handling code.
            // The arguments tell you which elements have lost and received focus.
        }

        /// <summary>
        /// Cancel subscription to the event.
        /// </summary>
        public void UnsubscribeFocusChange()
        {
            if (focusHandler != null)
            {
                Automation.RemoveAutomationFocusChangedEventHandler(focusHandler);
            }
        }

        #endregion

    }
    #endregion
}
