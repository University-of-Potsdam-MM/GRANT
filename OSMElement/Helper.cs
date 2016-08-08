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
        /// <param name="elementName">gibt an welche eigenschaft geuscht ist</param>
        /// <param name="properties">gibt alle eigenschaften eines Knotens an</param>
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

        //Generiert eine ID --> richtiger Algorithmus muss noch gewählt werden
        public static String generatedId(GeneralProperties properties)
        {
            /* https://blogs.msdn.microsoft.com/csharpfaq/2006/10/09/how-do-i-calculate-a-md5-hash-from-a-string/
             * http://stackoverflow.com/questions/12979212/md5-hash-from-string
             * http://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file
             */
            String result = "filterdTree" + properties.autoamtionIdFiltered + properties.classNameFiltered + properties.controlTypeFiltered + properties.itemTypeFiltered + properties.accessKeyFiltered + String.Join(":", properties.runtimeIDFiltered.Select(p => p.ToString()).ToArray());
            // String result = "filterdTree" + properties.autoamtionIdFiltered + properties.classNameFiltered + properties.controlTypeFiltered + properties.itemTypeFiltered + properties.accessKeyFiltered; //Achtung noch nicht eindeutig
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(result));
            }
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }


    }
}
