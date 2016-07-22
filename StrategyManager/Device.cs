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
        public AdapterClass adapterClass { get; set; }
        public String name { get; set; }
        //TODO: + Toche, Hardwaretasten

        public Device(int height, int width, OrientationEnum orientation, String name, AdapterClass adapterClass) : this()
        {
            this.height = height;
            this.width = width;
            this.orientation = orientation;
            this.adapterClass = adapterClass;
            this.name = name;
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

    /// <summary>
    /// struktur, welche Namen und Namespace des Adapters fure das Device enthält
    /// </summary>
    public struct AdapterClass
    {
        public String name { get; set; }
        public String namespaceString {get; set; }
        public String dllName { get; set; }

        public AdapterClass(String name, String nsString, String dll) : this()
        {
            this.name = name;
            this.namespaceString = nsString;
            this.dllName = dll;
        }
    }
}
