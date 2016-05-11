using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace StrategyManager
{
    public class XMLSerialize
    {
        public static XMLDevice  XMLDeserialize(String filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLDevice));
            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            XMLDevice myXML = (XMLDevice)serializer.Deserialize(reader);
            fs.Close();
            return myXML;
        }
    }
}
