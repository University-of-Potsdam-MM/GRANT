using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElements
{
    public struct TextviewObject
    {
       // public List<TextviewElement> textviewElements { get; set; }
        public String typeOfView { get; set; }
        public String screenName { get; set; }
        public String itemEnumerate { get; set; }
        public Orders orders { get; set; }
        public List<AcronymsOfPropertyContent> acronymsOfPropertyContent { get; set; }
       // public List<SpecialGroup> specialGroups { get; set; }

        public override string ToString()
        {
            return String.Format("typeOfView={0}, screenName={1}", typeOfView, screenName);
        }
    }

    /// <summary>
    /// Specified the order of objects of a text view element.
    /// </summary>
    public struct Orders
    {
        /// <summary>
        /// Default order of the objects.
        /// </summary>
        public List<TextviewElement> defaultOrder { get; set; }

        /// <summary>
        /// List of orders of special objects.
        /// </summary>
        public List<SpecialOrder> specialOrders { get; set; }
    }

    /// <summary>
    /// Specified the order of "special" objects of a text view element.
    /// </summary>
    public struct SpecialOrder
    {
        /// <summary>
        /// Name of the control type
        /// </summary>
        public String controltypeName { get; set; }
        /// <summary>
        /// List with the order.
        /// </summary>
        public List<TextviewElement> order { get; set; }

        public override string ToString()
        {
            return String.Format("controltypeName={0}", controltypeName);
        }
        
    }

    public struct TextviewElement
    {
        /// <summary>
        /// The order of objects (ascending order)
        /// </summary>
        public int order { get; set; }

        /// <summary>
        /// The name of the property to shown. Every controltype from <see cref="GeneralProperties"/> can be used. 
        /// </summary>
        public string property { get; set; }

        /// <summary>
        /// Minimum width of the view (in pins)
        /// </summary>
        public int minWidth { get; set; }

        /// <summary>
        /// String to separate objects
        /// </summary>
        public String separator { get; set; }

        public override string ToString()
        {
            return String.Format("order={0}, property={1}, minWidth={2}, separator={3}", order, property, minWidth, separator);
        }
    }

    public struct AcronymsOfPropertyContent
    {
        public String name { get; set; }
        public String acronym { get; set; }

        public override string ToString()
        {
            return String.Format("name={0}, acronym={1}", name, acronym);
        }
    }

/*    public struct SpecialGroup
    {
        public String controltype { get; set; }
        public String separator { get; set; }

        public override string ToString()
        {
            return String.Format("controltype={0}, separator={1}", controltype, separator);
        }
    }*/
}
