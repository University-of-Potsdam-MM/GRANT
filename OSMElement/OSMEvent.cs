using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElements
{
    //TODO
    [Serializable]
    public class OSMEvent
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public EventTypes Type { get; set; }
        public int Priority { get; set; }
    }

    public enum EventTypes
    {
        Maus,
        Keyboard,
        Application,
        BrailleDisplay,
        Sound
    }
}
