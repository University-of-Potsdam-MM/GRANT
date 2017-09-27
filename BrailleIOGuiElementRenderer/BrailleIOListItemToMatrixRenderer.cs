using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleIO.Interface;
using BrailleIO.Renderer;
using BrailleIOGuiElementRenderer.UiElements;

namespace BrailleIOGuiElementRenderer
{
    public class BrailleIOListItemToMatrixRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer
    {
        public bool[,] RenderMatrix(IViewBoxModel view, object otherContent)
        {
            UiElement uiElement;
            Type typeOtherContent = otherContent.GetType();
            if (typeof(UiElement).Equals(typeOtherContent))
            {
                uiElement = (UiElement)otherContent;
            }
            else
            {
                throw new InvalidCastException("Can't cast otherContent to UiElement! {0}");
            }
            return RenderListItem(view, uiElement);
        }

        public bool[,] RenderListItem(IViewBoxModel view, UiElement uiElement)
        {
            //mehrere ListItems (als Gruppe zusammengefasst) bilden eine Liste

            ListMenuItem listmenuItem;
            if (uiElement.uiElementSpecialContent == null) { return new bool[0,0]; }
            Type typeSpecialContent = uiElement.uiElementSpecialContent.GetType();
            if (typeof(ListMenuItem).Equals(typeSpecialContent))
            {
                listmenuItem = (ListMenuItem)uiElement.uiElementSpecialContent;
            }
            else
            {
                throw new InvalidCastException("Can't cast uiElementSpecialContent to ListMenuItem! {0}");
            }
            bool[,] matrix = new bool[view.ViewBox.Height, view.ViewBox.Width];
            bool[,] text;
            MatrixBrailleRenderer m = new MatrixBrailleRenderer();
            
            if (listmenuItem.hasNext)
            {
                bool[,] seperatorLine = Helper.createInterruptedLine(view.ViewBox.Width);
                Helper.copyMatrixInMatrix(seperatorLine, ref matrix,0, view.ViewBox.Height -1 );
            }
            if (listmenuItem.isMultipleSelection)
            {
                bool [,] box;
                if (listmenuItem.isSelected)
                {
                    box =Helper.createSelectedBox(4, 4);
                }
                else
                {
                    box = Helper.createBox(4, 4);
                }
                Helper.copyMatrixInMatrix(box, ref matrix, 1,1);
                text = m.RenderMatrix(view.ViewBox.Width - 4, (uiElement.text as object == null ? "" : uiElement.text as object), false);
                Helper.copyMatrixInMatrix(text, ref matrix, 6, 1);
            }
            else
            {
                text = m.RenderMatrix(view.ViewBox.Width, (uiElement.text as object == null ? "" : uiElement.text as object), false);
                Helper.copyMatrixInMatrix(text, ref matrix,0, 1);
            }
            return matrix;
        }

    }
}
