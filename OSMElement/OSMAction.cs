using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElements
{
    [Serializable]
    public class OSMAction
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public EventTypes Type { get; set; }
        public int Priority { get; set; }
    }

    public enum ActionTypes
    {
        FilterAction,
        OutputAction,
        ApplicationAction,
        SoundAction
    }
}
