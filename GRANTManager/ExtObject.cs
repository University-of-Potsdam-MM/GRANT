using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager
{
    public static class ExtObject
    {
        /// <summary>
        /// Deep copying of an object (<see cref="https://softwarebydefault.com/2013/02/10/deep-copy-generics/"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToCopy"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the default value of a type (<see cref="https://stackoverflow.com/questions/325426/programmatic-equivalent-of-defaulttype"/>
        /// </summary>
        /// <param name="t">a type</param>
        /// <returns>default value of a type</returns>
        public static object GetDefault(Type t)
        {
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }
            return null;
        }
    }
}
