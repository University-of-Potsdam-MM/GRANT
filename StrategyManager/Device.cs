using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManager
{
    /// <summary>
    /// Enthält Informationen/Beschreibung zu einer Stiftplatte
    /// </summary>
    public struct Device
    {
        public int height { get; set; }
        public int width {get; set; }
        public OrientationEnum orientation{ get; set; }
        public String name { get; set; }
        public Type deviceClassType { get; set; }
        //TODO: + Toche, Hardwaretasten

        public Device(int height, int width, OrientationEnum orientation, String name, Type deviceClassType)
            : this()
        {
            this.height = height;
            this.width = width;
            this.orientation = orientation;
            this.name = name;
            this.deviceClassType = deviceClassType;
        }

        public override string ToString()
        {
            return (String.Format(" name: {0}, height = {1}, width = {2} ({3})", name, height, width, orientation));
        }
    }

    /// <summary>
    /// Gibt die Ausrichtung der Stiftplatte an
    /// </summary>
    public enum OrientationEnum {Unknown = -1, Front, Right, Rear, Left }

}
