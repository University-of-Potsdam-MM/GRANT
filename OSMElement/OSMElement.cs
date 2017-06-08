using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OSMElement
{
    [Serializable]
    public class OSMElement : IEquatable<OSMElement>
    {
        public OSMElement()
        {
            properties = new GeneralProperties();
            brailleRepresentation = new BrailleRepresentation();
            events = new Events();
        }
        public GeneralProperties properties { get; set; }

        public Events events { get; set; }
        public BrailleRepresentation brailleRepresentation { get; set; }

        public override string ToString()
        {
            if (properties == null && brailleRepresentation == null) { return String.Format("GeneralProperties: null"); }
            if (properties == null) { return String.Format("GeneralProperties: null, BrailleRepresentation: {0}", brailleRepresentation.ToString()); }
            if (brailleRepresentation == null) { return String.Format("GeneralProperties: {0}, BrailleRepresentation: null", properties.ToString()); }
            return String.Format("GeneralProperties: {0}, BrailleRepresentation: {1}", properties.ToString(), brailleRepresentation.ToString());
        }

        public override bool Equals(object obj)
        {
            //TODO: Events
            if (obj == null || !obj.GetType().Equals(typeof(OSMElement))) { return false; }
            OSMElement osm = (OSMElement)obj;
            return this.Equals(osm);
        }

        public bool Equals(OSMElement osm)
        {
            if ((this.properties == null && osm.properties != null)
               || (this.properties != null && osm.properties == null)
               || (this.brailleRepresentation == null && osm.brailleRepresentation != null)
               || (this.brailleRepresentation != null && osm.brailleRepresentation == null))
            {
                return false;
            }
            bool result = true;
            if (this.properties != null)
            {
                result = this.properties.Equals(osm.properties);
            }
            if (this.brailleRepresentation != null)
            {
                result = result && this.brailleRepresentation.Equals(osm.brailleRepresentation);
            }
            return result;
        }

        /// <summary>
        /// Hash function.
        /// Attention: The object is mutable, the hash code can change! 
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        { //see: https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            unchecked // Overflow is fine, just wrap
            {
                int[] primeNumber = new int[] { 56467, 606241 };
                int hash = primeNumber[0]; // 	prime number 
                // Suitable nullity checks etc, of course :)
                hash = hash * primeNumber[1] + (this.properties != null ? properties.GetHashCode() : 0); 
                hash = hash * primeNumber[1] + (this.brailleRepresentation != null ? brailleRepresentation.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.events != null ? events.GetHashCode() : 0);
                return hash;
            }
        }

        /// <summary>
        /// Gives all Types of <see cref="OSMElement"/>
        /// </summary>
        /// <returns>list of all Types of <see cref="OSMElement"/></returns>
        public static List<String> getAllTypes()
        {
            List<String> displayedGuiElements = GeneralProperties.getAllTypes();
            displayedGuiElements.AddRange(BrailleRepresentation.getAllTypes());
            // displayedGuiElements.AddRange(Events.getAllTypes());
            return displayedGuiElements;
        }

        public static void setElement(String elementName, Object property, OSMElement osmElement)
        { // https://stackoverflow.com/questions/1089123/setting-a-property-by-reflection-with-a-string-value
            if (elementName != null && !osmElement.Equals(new OSMElement()))
            {
                PropertyInfo oInfo = osmElement.properties.GetType().GetProperty(elementName);
                if (oInfo != null)
                {
                    oInfo.SetValue(osmElement.properties, Convert.ChangeType(property, oInfo.PropertyType), null);
                    return;
                }
                else { oInfo = osmElement.brailleRepresentation.GetType().GetProperty(elementName); }
                if (oInfo != null)
                {
                    oInfo.SetValue(osmElement.brailleRepresentation, Convert.ChangeType(property, oInfo.PropertyType), null);
                    return;
                }
            }
            return;
        }

        public static object getElement(String elementName, OSMElement osmElement)
        {
            if (GeneralProperties.getAllTypes().Contains(elementName))
            {
                return GeneralProperties.getPropertyElement(elementName, osmElement.properties);
            }
            if (BrailleRepresentation.getAllTypes().Contains(elementName))
            {
                return BrailleRepresentation.getPropertyElement(elementName, osmElement.brailleRepresentation);
            }
            //TODO: Events
            return null;
        }
    }

}
