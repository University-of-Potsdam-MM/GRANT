using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    /// <summary>
    /// Spezifies the structure for connection between a (Braille) rendere and a GUI controllType
    /// </summary>
    public class RendererUiElementConnector
    {
        #region Constructor
        public RendererUiElementConnector()
        {

        }

        public RendererUiElementConnector(String controlType, String rendererName, SizeUiElement size)
        {
            this.ControlType = controlType;
            this.RendererName = rendererName;
            this.SizeElement = size;
        }
        #endregion

        /// <summary>
        /// The ControlType of the renderer
        /// </summary>
        public String ControlType { get; set; }

        /// <summary>
        /// The name of the renderer
        /// </summary>
        public String RendererName { get; set; }

        /// <summary>
        /// Size of the rendered element
        /// </summary>
        public SizeUiElement SizeElement { get; set; }

        public override string ToString()
        {
            return String.Format("RendererName: {0},   ControlType: {1}, SizeElement: {2}", RendererName, ControlType, SizeElement);
        }

        /// <summary>
        /// Specifies the size of an element to render
        /// </summary>
        public struct SizeUiElement
        {
            public SizeUiElement(int height, int width)
            {
                this.height = height;
                this.width = width;
            }

            public int height { get; set; }
            public int width { get; set; }
            //TODO: Padding etc.

            public override string ToString()
            {
                return String.Format("height = {0},   width = {1}", height, width);
            }
        }

    }
}
