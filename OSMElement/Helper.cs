using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace OSMElement
{
    public class Helper
    {
        /// <summary> //TODO: nicht nur von den Properties ermitteln
        /// Gibt eine konkrete Eigenschaft zurück
        /// </summary>
        /// <param name="elementName">gibt an welche Eigenschaft gewünscht ist</param>
        /// <param name="properties">gibt alle Eigenschaften eines Knotens an</param>
        /// <returns>aus <code>properties</code> die gewünschte Eigenschaft</returns>
        public static object getGeneralPropertieElement(String elementName, GeneralProperties properties)
        {
            try
            { //http://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection-in-c-sharp#
                return properties.GetType().GetProperty(elementName).GetValue(properties, null);
            }
            catch
            {
                throw new Exception("Fehler in OSMElement.Helper: Es soll eine nicht vorhandene Eigenschaft zurückgegeben werden!");
            }
        }

    }
}
