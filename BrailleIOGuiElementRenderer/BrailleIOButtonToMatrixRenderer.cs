using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleIO.Interface;
using BrailleIO.Renderer;
using System.Drawing;

namespace BrailleIOGuiElementRenderer
{
    public class BrailleIOButtonToMatrixRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer
    {
        public bool[,] RenderMatrix(IViewBoxModel view, object otherContent)
        {
            String buttonText;
            try
            {
                buttonText = otherContent as String;
            }
            catch (InvalidCastException ice)
            {
                throw new InvalidCastException("Can't cast otherContent to String! {0}", ice);
            }

            return RenderButton(view, buttonText); 
         //   return RenderButton(view, "textBoxText");
        }

        public bool[,] RenderButton(IViewBoxModel view, String buttonText)
        {
            //call pre hooks  --> wie funktioniert das richtig?
            object cM = buttonText as object;
           callAllPreHooks(ref view, ref cM);

           bool[,] viewMatrix = Helper.createBox(view.ViewBox.Height, view.ViewBox.Width); //erstmal ein eckiger Button

            //Ecken abrunden
            //links oben
            viewMatrix[0, 0] = false;
            viewMatrix[1, 0] = false;
            viewMatrix[0, 1] = false;
            viewMatrix[1, 1] = true;
            //links unten
            viewMatrix[view.ViewBox.Height - 1, 0] = false;
            viewMatrix[view.ViewBox.Height - 1, 1] = false;
            viewMatrix[view.ViewBox.Height - 2, 0] = false;
            viewMatrix[view.ViewBox.Height - 2, 1] = true;
            //rechts oben
            viewMatrix[1, view.ViewBox.Width - 1] = false;
            viewMatrix[0, view.ViewBox.Width - 2] = false;
            viewMatrix[0, view.ViewBox.Width - 1] = false;
            viewMatrix[1, view.ViewBox.Width - 2] = true;
            //rechts unten
            viewMatrix[view.ViewBox.Height - 1, view.ViewBox.Width - 1] = false;
            viewMatrix[view.ViewBox.Height - 2, view.ViewBox.Width - 1] = false;
            viewMatrix[view.ViewBox.Height - 1, view.ViewBox.Width - 2] = false;
            viewMatrix[view.ViewBox.Height - 2, view.ViewBox.Width - 2] = true;

            //String to Braille/Matrix
            MatrixBrailleRenderer m = new MatrixBrailleRenderer();
            bool[,] textMatrix = m.RenderMatrix(view.ViewBox.Width - 4, (buttonText as object == null ? "" : buttonText as object), false);
            Helper.copyTextMatrixInMatrix(textMatrix, ref viewMatrix);

            //call post hooks --> wie funktioniert das richtig?
            callAllPostHooks(view, cM, ref viewMatrix, false);

            return viewMatrix;
        }

    }
}
