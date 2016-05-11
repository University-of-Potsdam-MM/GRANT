using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManager
{

    /* automatisch erstellt von einer Beispiel-XML 
     *
     * Inhalt der gewünschten XML-Datei kopieren
     * Bearbeiten -> Inhalt einfügen -> XML als Klasse einfügen
     * Vergleich vom XMLTestObjectText-Objekt (override Equals)
     */

    /*
     * 
     *                 public override bool Equals(object obj)
            {
                XMLDeviceObjectText textObject = obj as XMLDeviceObjectText;
                return this.dynamic == textObject.dynamic && this.fix == textObject.fix && this.order == textObject.order;
            }
     * 
     */



    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class XMLDevice
    {

        private XMLDeviceApplication applicationField;

        private XMLDeviceDevise deviseField;

        private string[] screensField;

        private XMLDeviceObject[] objectsField;

        private byte versionField;

        /// <remarks/>
        public XMLDeviceApplication Application
        {
            get
            {
                return this.applicationField;
            }
            set
            {
                this.applicationField = value;
            }
        }

        /// <remarks/>
        public XMLDeviceDevise Devise
        {
            get
            {
                return this.deviseField;
            }
            set
            {
                this.deviseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Screen", IsNullable = false)]
        public string[] Screens
        {
            get
            {
                return this.screensField;
            }
            set
            {
                this.screensField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Object", IsNullable = false)]
        public XMLDeviceObject[] Objects
        {
            get
            {
                return this.objectsField;
            }
            set
            {
                this.objectsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class XMLDeviceApplication
    {

        private string nameField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class XMLDeviceDevise
    {

        private string nameField;

        private byte heightField;

        private byte whidthField;

        private bool hasHardwareKeysField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public byte Height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }

        /// <remarks/>
        public byte Whidth
        {
            get
            {
                return this.whidthField;
            }
            set
            {
                this.whidthField = value;
            }
        }

        /// <remarks/>
        public bool hasHardwareKeys
        {
            get
            {
                return this.hasHardwareKeysField;
            }
            set
            {
                this.hasHardwareKeysField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class XMLDeviceObject
    {

        private string generatedIdField;

        private string screenField;

        private XMLDeviceObjectPosition positionField;

        private XMLDeviceObjectText textField;

        private XMLDeviceObjectBitmap bitmapField;

        private bool[][] matrixField;

        /// <remarks/>
        public string GeneratedId
        {
            get
            {
                return this.generatedIdField;
            }
            set
            {
                this.generatedIdField = value;
            }
        }

        /// <remarks/>
        public string Screen
        {
            get
            {
                return this.screenField;
            }
            set
            {
                this.screenField = value;
            }
        }

        /// <remarks/>
        public XMLDeviceObjectPosition Position
        {
            get
            {
                return this.positionField;
            }
            set
            {
                this.positionField = value;
            }
        }

        /// <remarks/>
        public XMLDeviceObjectText Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        public XMLDeviceObjectBitmap Bitmap
        {
            get
            {
                return this.bitmapField;
            }
            set
            {
                this.bitmapField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Line", IsNullable = false)]
        [System.Xml.Serialization.XmlArrayItemAttribute("value", IsNullable = false, NestingLevel = 1)]
        public bool[][] Matrix
        {
            get
            {
                return this.matrixField;
            }
            set
            {
                this.matrixField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class XMLDeviceObjectPosition
    {

        private string viewField;

        private XMLDeviceObjectPositionViewRange viewRangeField;

        private byte borderField;

        private byte marginField;

        /// <remarks/>
        public string View
        {
            get
            {
                return this.viewField;
            }
            set
            {
                this.viewField = value;
            }
        }

        /// <remarks/>
        public XMLDeviceObjectPositionViewRange ViewRange
        {
            get
            {
                return this.viewRangeField;
            }
            set
            {
                this.viewRangeField = value;
            }
        }

        /// <remarks/>
        public byte Border
        {
            get
            {
                return this.borderField;
            }
            set
            {
                this.borderField = value;
            }
        }

        /// <remarks/>
        public byte Margin
        {
            get
            {
                return this.marginField;
            }
            set
            {
                this.marginField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class XMLDeviceObjectPositionViewRange
    {

        private byte leftField;

        private byte topField;

        private byte widthField;

        private byte heightField;

        /// <remarks/>
        public byte Left
        {
            get
            {
                return this.leftField;
            }
            set
            {
                this.leftField = value;
            }
        }

        /// <remarks/>
        public byte Top
        {
            get
            {
                return this.topField;
            }
            set
            {
                this.topField = value;
            }
        }

        /// <remarks/>
        public byte Width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        public byte Height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class XMLDeviceObjectText
    {

        private string fixField;

        private string dynamicField;

        private string orderField;

        /// <remarks/>
        public string fix
        {
            get
            {
                return this.fixField;
            }
            set
            {
                this.fixField = value;
            }
        }

        /// <remarks/>
        public string dynamic
        {
            get
            {
                return this.dynamicField;
            }
            set
            {
                this.dynamicField = value;
            }
        }

        /// <remarks/>
        public string order
        {
            get
            {
                return this.orderField;
            }
            set
            {
                this.orderField = value;
            }
        }

        public override bool Equals(object obj)
        {
            XMLDeviceObjectText textObject = obj as XMLDeviceObjectText;
            return this.dynamic == textObject.dynamic && this.fix == textObject.fix && this.order == textObject.order;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class XMLDeviceObjectBitmap
    {

        private object pathField;

        private bool guiObjectField;

        /// <remarks/>
        public object path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        public bool guiObject
        {
            get
            {
                return this.guiObjectField;
            }
            set
            {
                this.guiObjectField = value;
            }
        }
    }



}
