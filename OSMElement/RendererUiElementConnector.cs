using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    public class RendererUiElementConnector
    {
        public RendererUiElementConnector()
        {

        }

        public RendererUiElementConnector(String controlType, String rendererName, SizeUiElement size)
        {
            this.ControlType = controlType;
            this.RendererName = rendererName;
            this.SizeElement = size;
        }

        public String ControlType { get; set; }
        public String RendererName { get; set; }
        public SizeUiElement SizeElement { get; set; }

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
