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
            Button button;
            Type typeOtherContent = otherContent.GetType();
            if (typeof(Button).Equals(typeOtherContent))
            {
                button = (Button)otherContent;
            }
            else
            {
                throw new InvalidCastException("Can't cast otherContent to dropDownMenu! {0}");
            }


            return RenderButton(view, button); 
        }

        public bool[,] RenderButton(IViewBoxModel view, Button button)
        {
            //call pre hooks  --> wie funktioniert das richtig?
            object cM = button.text as object;
           callAllPreHooks(ref view, ref cM);

           bool[,] viewMatrix;
           if (button.isDeactiveted)
           {
               viewMatrix = Helper.createBoxDeaktivatedUpDown(view.ViewBox.Height, view.ViewBox.Width);
           }
           else
           {
               viewMatrix =   Helper.createBox(view.ViewBox.Height, view.ViewBox.Width); //erstmal ein eckiger Button
           }
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
            bool[,] textMatrix = m.RenderMatrix(view.ViewBox.Width - 4, (button.text as object == null ? "" : button.text as object), false);
            Helper.copyTextMatrixInMatrix(textMatrix, ref viewMatrix, 3);

            //call post hooks --> wie funktioniert das richtig?
            callAllPostHooks(view, cM, ref viewMatrix, false);

            return viewMatrix;
        }

    }
}
