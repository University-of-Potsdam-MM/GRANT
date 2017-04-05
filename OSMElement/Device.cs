﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    /// <summary>
    /// Structure for the discription of a braille display
    /// </summary>
    public struct Device
    {
        public int height { get; set; }
        public int width {get; set; }

        /// <summary>
        /// Orientation of a braille display
        /// </summary>
        public OrientationEnum orientation{ get; set; }

        /// <summary>
        /// Name of the Braille display
        /// </summary>
        public String name { get; set; }

        /// <summary>
        /// fully qualified name of the type for the used device class, including its namespace but not its assembly
        /// </summary>
        public String deviceClassTypeFullName { get; set; }

        /// <summary>
        /// namespace of the System.Type
        /// </summary>
        public String deviceClassTypeNamespace { get; set; }

        /// <summary>
        /// If exist the id of the device e.g. for MVBD
        /// </summary>
        public int id { get; set; }
        //TODO: + Toche, hardware keys

        public Device(int height, int width, OrientationEnum orientation, String name, Type deviceClassType)
            : this()
        {
            this.height = height;
            this.width = width;
            this.orientation = orientation;
            this.name = name;
            this.deviceClassTypeFullName = deviceClassType.FullName;
            this.deviceClassTypeNamespace = deviceClassType.Namespace;
        }

        public Device(Type deviceClassType):this()
        {
            this.deviceClassTypeFullName = deviceClassType.FullName;
            this.deviceClassTypeNamespace = deviceClassType.Namespace;
        }

        public override string ToString()
        {
            return (String.Format(" name: {0}, height = {1}, width = {2} ({3})", name, height, width, orientation));
        }
    }

    /// <summary>
    /// Orientation of a braille display
    /// </summary>
    public enum OrientationEnum {Unknown = -1, Front, Right, Rear, Left }

}
