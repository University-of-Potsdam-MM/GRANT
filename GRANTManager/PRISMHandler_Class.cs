using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GRANTManager;
using Prism.Events;

namespace GRANTManager
{
    public class PRISMHandler_Class
    {
        //So wie:
        //Kreierung des events direkt in wineventhanlderklasse
        //public class stringOSMEventTest : PubSubEvent<string> { }

        public class updateOSMEvent : PubSubEvent<string>
        {
            //void get()
            //{
            //    updateOSMEvent.
            //}
        }

        //bzw. die verarbeitung des events kann jedwede klasse selbst entscheiden, wie sie gemacht werden soll!!!
        public void generateOSM_PRISMHandler_Class(string osm)
        {
            Console.WriteLine("event verarbeitet, derzeit in EventAggregator_Prism, mit übergabe folgenden strings aus der publish-methode: " + osm);
            //osm = "werhers";
        }


    }
}
