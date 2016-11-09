using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace StrategyWindows
{
    public class Windows_EventsMonitor
    {
        //Konstruktor
        Windows_EventsHandler eventHandlerWindows;
        public Windows_EventsMonitor(Windows_EventsHandler eventHandlerWindows)
        {
            this.eventHandlerWindows = eventHandlerWindows;
            
            //todo diese methode in globaler eventklasse aufrufen und festelegen, welche keymouseevents von welcher applikation abgefangen werden sollen!
            subscribeWindowsEvents();
        }

        #region mouseKeyboardEvents

        /// <summary>
        /// NuGET-Package: https://github.com/gmamaladze/globalmousekeyhook
        /// Use with WindowsForm: http://stackoverflow.com/questions/30678382/key-listener-not-firing-mousekeyhook und http://stackoverflow.com/questions/1730731/how-to-start-winform-app-minimized-to-tray/1732294#1732294
        /// http://stackoverflow.com/questions/22138795/from-idle-to-action-using-globalmousekeyhook-c
        /// Use with WPF: http://stackoverflow.com/questions/31366998/wpf-using-mousekeyhook-library-to-count-clicks-loaderlock-error
        /// ToDo: Ausführung in anderem Thread
        /// ToDo: Abfrage Groß/Kleinzeichen
        /// https://msdn.microsoft.com/de-de/library/system.windows.forms.keys(v=vs.110).aspx
        /// Problem: Kein Werfen eines Events nach Tastaturdruck
        /// Lsg.: Grund dafür evtl. Code aus MousekeyHook Demo-Sample; Hierin ist aber sichtbar, welche events abgefagen werden können!!!
        /// andere mögliche Gründe:
        /// Garbage Collector zerstört das delegateobjekt, evtl. wird dieses problem aber auch bereits in mousekeyhook-sln behandelt: http://stackoverflow.com/questions/9957544/callbackoncollecteddelegate-in-globalkeyboardhook-was-detected
        /// Fehlermeldung das der Thread mit Code 259 exited ist kein Grund, Events werden trotzdem geworfen: http://stackoverflow.com/questions/22395396/why-am-i-seeing-multiple-the-thread-0x22c8-has-exited-with-code-259-0x103-m

        /// ToDo: Events einer ausgewählten App abfangen und anderen Events ignorieren, Dies in eigenes EventMonitor-Projekt auslagern
        /// http://stackoverflow.com/questions/32119658/mouse-key-hook-key-listener-not-firing
        
        /// ToDo: Nutzung des Paket Screna: https://github.com/MathewSachin/Screna
        /// Capturing des Screen/Audio/Video/...
        /// </summary>
        
        private static IKeyboardMouseEvents m_MouseKeyEvents;

        //todo
        // array, oder reine stringliste mit üpbergabe der dinge die gehooed werden sollen
        public bool[] keyboardMouseEventSelection = new bool[10];
        public void initArray()
        {
            //keyboardMouseEventSelection = new bool[10];
            for (int i = 0; i < keyboardMouseEventSelection.Length; i++)
            {
                keyboardMouseEventSelection[i] = false;
            }
            keyboardMouseEventSelection[0] = true;
        }

        public void subscribeWindowsEvents()
        {
            subscribeGlobalKeyboardMouse();
            //subscribeApplicationKeyboardMouse();
            Console.WriteLine("Subscribe der Events");
        }

        public void unsubscribeWindowsEvents()
        {
            unsubscribeKeyboardMouse();
            Console.WriteLine("Unsubscribe der Events");
        }

        //Arbeit nur mit ApplicationEvents schwer möglich, 
        //da unklar, ob der Filter den Fokus hat oder die eigentliche zu filternde Applikation 
        public void subscribeApplicationKeyboardMouse()
        {
            unsubscribeKeyboardMouse();
            //todo
            //subscribeKeyboardMouse(prismEventAggregator, Hook.AppEvents(), keyboardMouseEventSelection);
        }

        private void subscribeGlobalKeyboardMouse()
        {
            unsubscribeKeyboardMouse();
            subscribeKeyboardMouse(Hook.GlobalEvents(), keyboardMouseEventSelection);
        }

        /// <summary>
        /// timestamp bei key?
        /// </summary>
        /// <param name="mouseKeyEvents"></param>
        private void subscribeKeyboardMouse(IKeyboardMouseEvents mouseKeyEvents, bool[] keyboardMouseEventSelection)
        {
            //from: http://stackoverflow.com/questions/9957544/callbackoncollecteddelegate-in-globalkeyboardhook-was-detected
            //if (m_Events != null) throw new InvalidOperationException("Can't hook more than once");
            m_MouseKeyEvents = mouseKeyEvents;

            //keyboard
            keyboardMouseEventSelection[0] = true;
            //wenn true dann machen
            if (keyboardMouseEventSelection[0]) m_MouseKeyEvents.KeyUp += eventHandlerWindows.onKeyUp;

            //mouse
            //m_Events.MouseUp += OnMouseUp;
            m_MouseKeyEvents.MouseUpExt += onMouseUpExt;
            //m_Events.MouseClick += OnMouseClick;
            //m_Events.MouseDoubleClick += OnMouseDoubleClick;
        }

        private void onMouseUpExt(object sender, MouseEventExtArgs e)
        {
            Console.WriteLine("MouseUp: \t{0}; \t System Timestamp: \t{1}", e.Button, e.Timestamp);

            // uncommenting the following line will suppress the middle mouse button click
            // if (e.Buttons == MouseButtons.Middle) { e.Handled = true; }
        }

        
        //todo hier können alle unsubscribet werden, auch wenn sie gar nicht subscribed wurden???
        public void unsubscribeKeyboardMouse()
        {
            //from: http://stackoverflow.com/questions/9957544/callbackoncollecteddelegate-in-globalkeyboardhook-was-detected
            if (m_MouseKeyEvents == null) return;

            //m_MouseKeyEvents.KeyDown -= OnKeyDown;
            // keypress wirft kein event bei pfeiltasten
            //m_MouseKeyEvents.KeyPress += GlobalHookKeyPress; 
            m_MouseKeyEvents.KeyUp -= eventHandlerWindows.onKeyUp;

            //m_Events.MouseUp -= OnMouseUp;
            m_MouseKeyEvents.MouseUpExt -= onMouseUpExt;
            //m_Events.MouseClick -= OnMouseClick;
            //m_Events.MouseDoubleClick -= OnMouseDoubleClick;

            //It is recommened to dispose it
            m_MouseKeyEvents.Dispose();
            m_MouseKeyEvents = null;
        }

        #endregion

        //Anmeldung der verschiedenen events, welche MouseKeyHook unterstützt
        public void Subscribe()
        {
            //from: http://stackoverflow.com/questions/9957544/callbackoncollecteddelegate-in-globalkeyboardhook-was-detected
            //if (m_GlobalHook != null) throw new InvalidOperationException("Can't hook more than once");

            // Note: for the application hook, use the Hook.AppEvents() instead
            m_MouseKeyEvents = Hook.GlobalEvents();

            m_MouseKeyEvents.MouseDownExt += GlobalHookMouseDownExt;
            // keypress wirft kein event bei pfeiltasten
            //m_GlobalHook.KeyPress += GlobalHookKeyPress; 
            m_MouseKeyEvents.KeyUp += OnKeyUp;

            Console.WriteLine("Subscribe des Hook");
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine("KeyPress: \t{0}", e.KeyChar.ToString());
        }

        //private void OnKeyDown(object sender, KeyPressEventArgs e)
        //{
        //    Console.WriteLine("KeyPress: \t{0}", e.KeyChar);
        //}

        private void OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Console.WriteLine("KeyUp  \t\t {0}\n", e.KeyValue);
            Console.WriteLine("KeyUp  \t\t {0}\n", e.KeyCode);
            //return e.KeyValue.ToString();
        }

        //todo wie event nach deren wurf behandeln?
        //für nutzung in grantexample eine globale variable festlegen, welche info über das geworfene event enthält
        // derzeit in grantmanager:         private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        public string keyValue(string key)
        {
            //OnKeyUp
            return key;
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            Console.WriteLine("MouseDown: \t{0}; \t System Timestamp: \t{1}", e.Button, e.Timestamp);

            // uncommenting the following line will suppress the middle mouse button click
            // if (e.Buttons == MouseButtons.Middle) { e.Handled = true; }
        }

        public void Unsubscribe()
        {
            //from: http://stackoverflow.com/questions/9957544/callbackoncollecteddelegate-in-globalkeyboardhook-was-detected
            if (m_MouseKeyEvents == null) return;

            m_MouseKeyEvents.MouseDownExt -= GlobalHookMouseDownExt;
            //m_GlobalHook.KeyPress -= GlobalHookKeyPress;
            m_MouseKeyEvents.KeyUp -= OnKeyUp;

            //It is recommened to dispose it
            m_MouseKeyEvents.Dispose();
        }

    }

}
