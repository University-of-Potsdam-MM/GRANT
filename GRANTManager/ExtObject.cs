using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager
{
    // see:https://softwarebydefault.com/2013/02/10/deep-copy-generics/

    public static class ExtObject
    {
        public static T DeepCopy<T>(this T objectToCopy)
        { 
            if(objectToCopy == null) { return default(T); }
            MemoryStream mStream = new MemoryStream();
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(mStream, objectToCopy);

            mStream.Position = 0;
            T returnValue = (T)bFormatter.Deserialize(mStream);

            mStream.Close();
            mStream.Dispose();

            return returnValue;
        }
    }
}
