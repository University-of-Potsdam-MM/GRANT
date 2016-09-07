using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleIO.Interface;
using BrailleIO.Renderer;
using System.Drawing;
using BrailleIO;

namespace BrailleIOGuiElementRenderer
{
    public class BrailleIOGroupViewRangeToMatrixRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer
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


            return RenderGroup(view, groupViewRange); 
        }

        public bool[,] RenderGroup(IViewBoxModel view, UiElement groupViewRange)
        {
            //call pre hooks
            object cM = groupViewRange.text as object;
           callAllPreHooks(ref view, ref cM);
           bool[,] viewMatrix = new bool[0, 0];// = new bool[view.ViewBox.Height, view.ViewBox.Width]; //TODO: - Padding etc.
           int maxHeight = view.ViewBox.Height;
           int maxWidth = view.ViewBox.Width;
            
           if (groupViewRange.child!= null)
           {
               getMax(ref maxHeight, ref maxWidth, groupViewRange.child);
               viewMatrix = new bool[maxHeight, maxWidth]; 
               int i = 0;
              // Console.WriteLine("groupViewRange.viewName = {0}", groupViewRange.viewName);
               foreach (Groupelements child in groupViewRange.child)
               {
                   BrailleIOViewRange tmpChildView = new BrailleIOViewRange(Convert.ToInt32(child.childBoundingRectangle.Left), Convert.ToInt32(child.childBoundingRectangle.Top), Convert.ToInt32(child.childBoundingRectangle.Width), Convert.ToInt32(child.childBoundingRectangle.Height));
                   tmpChildView.Name = "_"+child.childBoundingRectangle.ToString(); //child.childUiElement.viewName;
                   tmpChildView.SetText(child.childUiElement.text);
                   tmpChildView.ShowScrollbars = child.childUiElement.showScrollbar;
                  // ((BrailleIOViewRange)view).GetYOffset();
                   bool[,] childMatrix;
                   if (child.renderer != null)
                   {
                       childMatrix = child.renderer.RenderMatrix(tmpChildView, child.childUiElement as object);
                   }
                   else
                   {
                       MatrixBrailleRenderer m = new MatrixBrailleRenderer();
                       childMatrix = m.RenderMatrix(tmpChildView, child.childUiElement.text as object);
                   }
                  // Console.WriteLine("i = "+i);
                   //Helper.copyMatrixInMatrix(childMatrix, ref viewMatrix,1+ i * Convert.ToInt32(child.childBoundingRectangle.Width));
                   //Helper.copyMatrixInMatrix(childMatrix, ref viewMatrix, Convert.ToInt32(child.childBoundingRectangle.TopLeft.X) - Convert.ToInt32(view.ViewBox.X), Convert.ToInt32(child.childBoundingRectangle.TopLeft.Y) - Convert.ToInt32(view.ViewBox.Y));
                   Helper.copyMatrixInMatrix(childMatrix, ref viewMatrix, Convert.ToInt32(child.childBoundingRectangle.TopLeft.X) - (Convert.ToInt32(view.ContentBox.X) + Convert.ToInt32(view.ViewBox.X)), Convert.ToInt32(child.childBoundingRectangle.TopLeft.Y) - (Convert.ToInt32(view.ContentBox.Y) + Convert.ToInt32(view.ViewBox.Y)));
                   i++;
               }
           }
           view.ContentHeight = maxHeight;
           view.ContentWidth = maxWidth;

            //call post hooks
            callAllPostHooks(view, cM, ref viewMatrix, false);

           return viewMatrix;
        }

        private void getMax(ref int maxHeight, ref int maxWidth, List<Groupelements> childs)
        {
            foreach (Groupelements c in childs)
            {
                maxHeight = Math.Max(maxHeight, Convert.ToInt32( c.childBoundingRectangle.Y + c.childBoundingRectangle.Height));
                maxWidth = Math.Max(maxWidth, Convert.ToInt32(c.childBoundingRectangle.X + c.childBoundingRectangle.Width));
            }
        }

    }
}
