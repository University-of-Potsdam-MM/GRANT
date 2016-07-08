using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleIOGuiElementRenderer
{
    /// <summary>
    /// Interface, welches alle eigenschaften Enthält, die für alle UI-Elemente zutreffend sind
    /// </summary>
    public interface IOtherContent
    {
        /// <summary>
        /// Enthält den darzustellenden Text eines UI-Elements
        /// </summary>
        String text { get; set; }

        /// <summary>
        /// Gibt an, ob das UI-Element deaktiviert ist
        /// </summary>
        bool isDisabled { get; set; }
    }
}
