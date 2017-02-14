using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleIO.Interface;
using BrailleIO.Renderer;
using System.Drawing;
using BrailleIO;
using System.Diagnostics;

namespace BrailleIOGuiElementRenderer
{
    public class BrailleIOTextBoxToMatrixRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer
    {
        public bool[,] RenderMatrix(IViewBoxModel view, object otherContent)
        {
            UiElement textBoxContent;
            try
            {
                textBoxContent = (UiElement)otherContent;
            }
            catch (InvalidCastException ice)
            {
                throw new InvalidCastException("Can't cast uiElementContent to BrailleRepresentation! {0}", ice);
            }
            //der eigentliche Text wird seperat gerendert, da das Scrollen ansonsten unschöne Nebenefekte hätte (Box würde mitscrollen)
            RenderTextBoxTextView(view, textBoxContent);
            return RenderTextBox(view, textBoxContent);
        }

        private bool[,] RenderTextBoxTextView(IViewBoxModel view, UiElement textBoxContent)
        {
            MatrixBrailleRenderer m = new MatrixBrailleRenderer();
            BrailleIOViewRange tmpTextBoxView;
            BrailleIOMediator brailleIOMediator = BrailleIOMediator.Instance;
            BrailleIOScreen screen = brailleIOMediator.GetView(textBoxContent.screenName) as BrailleIOScreen;
            if (screen.GetViewRange("_TextBoxText_" + textBoxContent.viewName) as BrailleIOViewRange != null)
            {
                tmpTextBoxView = screen.GetViewRange("_TextBoxText_" + textBoxContent.viewName) as BrailleIOViewRange;
            }else
            {
                tmpTextBoxView = new BrailleIOViewRange(view.ViewBox.Left + 3, view.ViewBox.Top + 2, view.ViewBox.Width - 5, view.ViewBox.Height - 4);
                tmpTextBoxView.Name = "_TextBoxText_" + textBoxContent.viewName;
                tmpTextBoxView.SetText(textBoxContent.text);
                tmpTextBoxView.ShowScrollbars = textBoxContent.showScrollbar;
            }
            
            tmpTextBoxView.SetZIndex(3);
            bool[,] textMatrix;
            if (tmpTextBoxView.ContentBox.Height <= 0 || tmpTextBoxView.ContentBox.Width <= 0)
            {
                textMatrix = new bool[0, 0];
            }
            else
            {
                textMatrix = m.RenderMatrix(tmpTextBoxView, (textBoxContent.text as object == null ? "" : textBoxContent.text as object));
            }
            if (screen != null)
            {
                BrailleIOViewRange viewRange = screen.GetViewRange(tmpTextBoxView.Name);
                if (viewRange == null)
                {
                    ((BrailleIOScreen)brailleIOMediator.GetView(textBoxContent.screenName)).AddViewRange(tmpTextBoxView.Name, tmpTextBoxView);
                    viewRange = screen.GetViewRange(tmpTextBoxView.Name);
                }
                viewRange.SetText(textBoxContent.text);
            }

            return textMatrix;
        }


        private bool[,] RenderTextBox(IViewBoxModel view, UiElement textBoxContent)
        {
            bool[,] viewMatrix;
            if (textBoxContent.isDisabled)
            {
                viewMatrix = Helper.createBoxDeaktivatedUpDown(view.ViewBox.Height, view.ViewBox.Width);
            }
            else
            {
                viewMatrix = Helper.createBox(view.ViewBox.Height, view.ViewBox.Width); //erstmal eine eckige Matrix
            }

            //Ecke links oben abrunden
            Debug.Print(viewMatrix.GetLength(0).ToString());
            Debug.Print(viewMatrix.GetLength(1).ToString());
            if(viewMatrix.GetLength(0) <=0 || viewMatrix.GetLength(1) <= 0) { return new bool[0, 0]; }
            viewMatrix[0, 0] = false;
            if (viewMatrix.GetLength(1) > 1)
            {
                viewMatrix[1, 0] = false;
            }
            if (viewMatrix.GetLength(0) > 1)
            {
                viewMatrix[0, 1] = false;
            }
            if (viewMatrix.GetLength(0) > 1 && viewMatrix.GetLength(1) > 1)
            {
                viewMatrix[1, 1] = true;
            }

            BrailleIOViewRange tmpBoxView = new BrailleIOViewRange(view.ViewBox.Left, view.ViewBox.Top, view.ViewBox.Width, view.ViewBox.Height);
            tmpBoxView.Name = "_B_"+ textBoxContent.screenName + view.ViewBox.Left + view.ViewBox.Top + view.ViewBox.Width + view.ViewBox.Height;
            // tmpBoxView.SetText(textBoxText);
            tmpBoxView.SetMatrix(viewMatrix);
            // tmpBoxView.ShowScrollbars = true;
            tmpBoxView.SetYOffset(0);
            tmpBoxView.SetZIndex(2);

            object cM = textBoxContent.text as object;
            IViewBoxModel tmpModel = tmpBoxView as IViewBoxModel;
            callAllPreHooks(ref tmpModel, ref cM);
            tmpBoxView = tmpBoxView as BrailleIOViewRange;
            BrailleIOMediator brailleIOMediator = BrailleIOMediator.Instance;
            BrailleIOScreen screen = (BrailleIOScreen)brailleIOMediator.GetView(textBoxContent.screenName);
            if (screen != null)
            {
                BrailleIOViewRange viewRange = screen.GetViewRange(tmpBoxView.Name);
                if (viewRange == null)
                {
                    ((BrailleIOScreen)brailleIOMediator.GetView(textBoxContent.screenName)).AddViewRange(tmpBoxView.Name, tmpBoxView);
                }

            }

            callAllPostHooks(tmpBoxView, cM, ref viewMatrix, false);
            return viewMatrix;

        }
    }
}
