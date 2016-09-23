using BrailleIO.Interface;
using BrailleIO.Renderer;
using BrailleIOGuiElementRenderer.UiElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleIOGuiElementRenderer
{
    public class BrailleIOTabItemToMatrixRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer
    {
        public bool[,] RenderMatrix(IViewBoxModel view, object otherContent)
        {
            UiElement groupViewRange;
            Type typeOtherContent = otherContent.GetType();
            if (typeof(UiElement).Equals(typeOtherContent))
            {
                groupViewRange = (UiElement)otherContent;
            }
            else
            {
                throw new InvalidCastException("Can't cast otherContent to UiContent! {0}");
            }


            return RenderTabView(view, groupViewRange);
        }

        private bool[,] RenderTabView(IViewBoxModel view, UiElement groupViewRange)
        {
            if (groupViewRange.uiElementSpecialContent == null)
            {
                throw new Exception("Kein 'uiElementSpecialContent' angegeben!");
            }
            Type typeSpecialContent = groupViewRange.uiElementSpecialContent.GetType();
            Console.WriteLine("Type = {0}", typeSpecialContent.Name);

            TabItem tabview;
            if (typeof(TabItem).Equals(typeSpecialContent))
            {
                tabview = (TabItem)groupViewRange.uiElementSpecialContent;
            }
            else
            {
                throw new InvalidCastException("Can't cast uiElementSpecialContent to Tabview! {0}");
            }
            bool[,] viewMatrix = new bool[view.ViewBox.Height, view.ViewBox.Width];
            MatrixBrailleRenderer m = new MatrixBrailleRenderer();
            bool[,] textMatrix = m.RenderMatrix(view.ViewBox.Width - 4, (groupViewRange.text as object == null ? "" : groupViewRange.text as object), false);
            
            bool[,] box = new bool[0,0];

                box = Helper.createBox(view.ViewBox.Height, view.ViewBox.Width);
                int spceLeft = 1;
                if (tabview.orientation.Equals(UiElements.Orientation.Bottom))
                {
                    Helper.RemoveBottomBoarder(ref box);
                }
                if (tabview.orientation.Equals(UiElements.Orientation.Top))
                {
                    Helper.RemoveTopBoarder(ref box);
                }
                if (tabview.orientation.Equals(UiElements.Orientation.Left))
                {
                    Helper.RemoveLeftBoarder(ref box);
                }
                if (tabview.orientation.Equals(UiElements.Orientation.Right))
                {
                    Helper.RemoveRightBoarder(ref box);
                    spceLeft = 2;
                }  

            if (groupViewRange.isDisabled)
            {// isDisabled -> ist gerade der aktive Tab und kann daher nicht mehr "aktiviert" werden
                if (tabview.orientation.Equals(UiElements.Orientation.Bottom))
                {
                    Helper.RemoveTopBoarder(ref box);
                }
                if (tabview.orientation.Equals(UiElements.Orientation.Top))
                {
                    Helper.RemoveBottomBoarder(ref box);
                }
                if (tabview.orientation.Equals(UiElements.Orientation.Left))
                {
                    Helper.RemoveRightBoarder(ref box);
                }
                if (tabview.orientation.Equals(UiElements.Orientation.Right))
                {
                    Helper.RemoveLeftBoarder(ref box);
                } 
            }
            Helper.copyMatrixInMatrix(box, ref viewMatrix);
            Helper.copyTextMatrixInMatrix(textMatrix, ref viewMatrix, spceLeft);
             return viewMatrix;
        }
    }
}
