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
    public class BrailleIOTextBoxToMatrixRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer
    {
        public bool[,] RenderMatrix(IViewBoxModel view, object otherContent)
        {
            String textBoxText;
            try
            {
                textBoxText = otherContent as String;
            }
            catch (InvalidCastException ice)
            {
                throw new InvalidCastException("Can't cast otherContent to String! {0}", ice);
            }
            return RenderTextBox(view, textBoxText);
        }

        private bool[,] RenderTextBox(IViewBoxModel view, String textBoxText)
        {
            //call pre hooks  --> wie funktioniert das richtig?
            object cM = textBoxText as object;
            callAllPreHooks(ref view, ref cM);

            bool[,] viewMatrix = Helper.createBox(view.ViewBox.Height, view.ViewBox.Width); //erstmal eine eckige Matrix
            //Ecke links oben abrunden
            viewMatrix[0, 0] = false;
            viewMatrix[1, 0] = false;
            viewMatrix[0, 1] = false;
            viewMatrix[1, 1] = true;

            //String to Braille/Matrix
            MatrixBrailleRenderer m = new MatrixBrailleRenderer();
            bool[,] textMatrix = m.RenderMatrix(view.ViewBox.Width - 4, (textBoxText as object == null ? "" : textBoxText as object), false);
            Helper.copyTextMatrixInMatrix(textMatrix, ref viewMatrix, 2);
            
            //call post hooks --> wie funktioniert das richtig?
            callAllPostHooks(view, cM, ref viewMatrix, false);

            return viewMatrix;
        }
    }
}
