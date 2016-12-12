using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    public class TextviewObject
    {
        public List<TextviewElement> textviewElements { get; set; }
        public String viewCategory { get; set; }
        public String screenName { get; set; }
        public String itemEnumerate { get; set; }
        public List<AcronymsOfPropertyContent> acronymsOfPropertyContent { get; set; }
        public List<SpecialGroup> specialGroups { get; set; }

        public override string ToString()
        {
            return String.Format("viewCategory={0}, screenName={1}, textviewElements: {2}", viewCategory, screenName, String.Join("; ", textviewElements.ToArray().ToString()));
        }
    }

    public class TextviewElement
    {
        public int order { get; set; }
        public string property { get; set; }
        public int minWidth { get; set; }
        public String separator { get; set; }

        public override string ToString()
        {
            return String.Format("order={0}, property={1}, minWidth={2}, separator={3}", order, property, minWidth, separator);
        }
    }

    public class AcronymsOfPropertyContent
    {
        public String name { get; set; }
        public String acronym { get; set; }

        public override string ToString()
        {
            return String.Format("name={0}, acronym={1}", name, acronym);
        }
    }

    public class SpecialGroup
    {
        public String controltype { get; set; }
        public String separator { get; set; }

        public override string ToString()
        {
            return String.Format("controltype={0}, separator={1}", controltype, separator);
        }
    }
}
