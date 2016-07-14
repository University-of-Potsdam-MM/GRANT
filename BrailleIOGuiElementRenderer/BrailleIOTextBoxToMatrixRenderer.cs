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
    public class BrailleIOTextBoxToMatrixRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer
    {
        public bool[,] RenderMatrix(IViewBoxModel view, object otherContent)
        {
            TextBox textBox;
            try
            {
                textBox = (TextBox)otherContent;
            }
            catch (InvalidCastException ice)
            {
                throw new InvalidCastException("Can't cast otherContent to TextBox! {0}", ice);
            }
            //der eigentliche Text wird seperat gerendert, da das Scrollen ansonsten unschöne Nebenefekte hätte (Box würde mitscrollen)
            RenderTextBoxTextView(view, textBox);
            return RenderTextBox(view, textBox);
        }

        private bool[,] RenderTextBoxTextView(IViewBoxModel view, TextBox textBox)
        {
            MatrixBrailleRenderer m = new MatrixBrailleRenderer();
            BrailleIOViewRange tmpTextBoxView = new BrailleIOViewRange(view.ViewBox.Left + 2, view.ViewBox.Top + 2, view.ViewBox.Width - 4, view.ViewBox.Height - 4);
            tmpTextBoxView.Name = "_T_" + textBox.screen + view.ViewBox.Left + view.ViewBox.Top + view.ViewBox.Width + view.ViewBox.Height;
            tmpTextBoxView.SetText(textBox.text);
            tmpTextBoxView.ShowScrollbars = textBox.showScrollbar;
            ((BrailleIOViewRange)view).GetYOffset();
            tmpTextBoxView.SetXOffset(((BrailleIOViewRange)view).GetXOffset());
            tmpTextBoxView.SetYOffset(((BrailleIOViewRange)view).GetYOffset());
            tmpTextBoxView.SetZIndex(3);
            bool[,] textMatrix = m.RenderMatrix(tmpTextBoxView, (textBox.text as object == null ? "" : textBox.text as object));

            BrailleIOMediator brailleIOMediator = BrailleIOMediator.Instance;
            BrailleIOScreen screen = (BrailleIOScreen)brailleIOMediator.GetView(textBox.screen);
            if (screen != null)
            {
                BrailleIOViewRange viewRange = screen.GetViewRange(tmpTextBoxView.Name);
                if (viewRange == null)
                {
                    ((BrailleIOScreen)brailleIOMediator.GetView(textBox.screen)).AddViewRange(tmpTextBoxView.Name, tmpTextBoxView);
                }
                else
                {
                    viewRange.SetXOffset(((BrailleIOViewRange)view).GetXOffset());
                    viewRange.SetYOffset(((BrailleIOViewRange)view).GetYOffset());
                    viewRange.SetText(textBox.text);
                }
            }

            return textMatrix;
        }


        private bool[,] RenderTextBox(IViewBoxModel view, TextBox textBox)
        {
            bool[,] viewMatrix;
            if (textBox.isDisabled)
            {
                viewMatrix = Helper.createBoxDeaktivatedUpDown(view.ViewBox.Height, view.ViewBox.Width);
            }
            else
            {
                viewMatrix = Helper.createBox(view.ViewBox.Height, view.ViewBox.Width); //erstmal eine eckige Matrix
            }

            //Ecke links oben abrunden
            viewMatrix[0, 0] = false;
            viewMatrix[1, 0] = false;
            viewMatrix[0, 1] = false;
            viewMatrix[1, 1] = true;

            BrailleIOViewRange tmpBoxView = new BrailleIOViewRange(view.ViewBox.Left, view.ViewBox.Top, view.ViewBox.Width, view.ViewBox.Height);
            tmpBoxView.Name = "_B-"+ textBox.screen + view.ViewBox.Left + view.ViewBox.Top + view.ViewBox.Width + view.ViewBox.Height;
            // tmpBoxView.SetText(textBoxText);
            tmpBoxView.SetMatrix(viewMatrix);
            // tmpBoxView.ShowScrollbars = true;
            tmpBoxView.SetYOffset(0);
            tmpBoxView.SetZIndex(2);

            object cM = textBox.text as object;
            IViewBoxModel tmpModel = tmpBoxView as IViewBoxModel;
            callAllPreHooks(ref tmpModel, ref cM);
            tmpBoxView = tmpBoxView as BrailleIOViewRange;
            BrailleIOMediator brailleIOMediator = BrailleIOMediator.Instance;
            BrailleIOScreen screen = (BrailleIOScreen)brailleIOMediator.GetView(textBox.screen);
            if (screen != null)
            {
                BrailleIOViewRange viewRange = screen.GetViewRange(tmpBoxView.Name);
                if (viewRange == null)
                {
                    ((BrailleIOScreen)brailleIOMediator.GetView(textBox.screen)).AddViewRange(tmpBoxView.Name, tmpBoxView);
                }

            }

            callAllPostHooks(tmpBoxView, cM, ref viewMatrix, false);
            return viewMatrix;

        }
    }
}
