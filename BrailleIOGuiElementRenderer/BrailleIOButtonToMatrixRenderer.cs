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
            catch(InvalidCastException ice) 
            {
                throw new InvalidCastException("Can't cast otherContent to String! {0}", ice);
            }

            return RenderButton(view, buttonText);
        }

        public bool[,] RenderButton(IViewBoxModel view, String buttonText)
        {
            //call pre hooks  --> wie funktioniert das richtig?
            object cM = buttonText as object;
          //  callAllPreHooks(ref view, ref cM);

            bool[,] viewMatrix = new bool[view.ViewBox.Height, view.ViewBox.Width];
            
            //TODO String to Braille/Matrix
            MatrixBrailleRenderer m = new MatrixBrailleRenderer();
            bool[,] textMatrix = m.RenderMatrix(view.ViewBox.Width - 4, buttonText as object, false);
            copyMatrixInmatrix(textMatrix, ref viewMatrix);
            //erstmal ein eckiger Button
            for (int height = 0; height < view.ViewBox.Height; height++)
            {
                for (int width = 0; width < view.ViewBox.Width; width++)
                {
                    if (height == 0 || height == view.ViewBox.Height - 1)
                    {
                        viewMatrix[height, width] = true;
                    }
                    else
                    {
                        if (width == 0 || width == view.ViewBox.Width - 1)
                        {
                            viewMatrix[height, width] = true;
                        }
                    }
                }
            }
            //Ecken abrunden
            viewMatrix[0, 0] = false;
            viewMatrix[view.ViewBox.Height - 1, 0] = false;
            viewMatrix[0, view.ViewBox.Width - 1] = false;
            viewMatrix[view.ViewBox.Height - 1, view.ViewBox.Width - 1] = false;

            //call post hooks --> wie funktioniert das richtig?
         //   callAllPostHooks(view, cM, ref viewMatrix, false);

            return viewMatrix;
        }

        private void copyMatrixInmatrix(bool[,] smallMatrix, ref bool[,] bigMatrix)
        {
            //Console.WriteLine("GetLength = {0},  GetUpperBound = {1}", bigMatrix.GetLength(0), bigMatrix.GetUpperBound(0));
            // getLength ^= Anzahl der Zeilen
            // Length ^= Anzahl der Elemente insgesamt
            // Length / getLength ^= Pins pro Zeile
            for (int i = 2; (i < (bigMatrix.Length / bigMatrix.GetLength(0)) - 2) && (i < (smallMatrix.Length / smallMatrix.GetLength(0))); i++)//ab Zeile 2 soll der Text erscheinen
            {
                for (int j = 2; (j < bigMatrix.GetLength(0) - 2) && (j < smallMatrix.GetLength(0) + 2); j++)
                {
                    bigMatrix[j, i] = smallMatrix[j - 2, i - 2];
                }
            }
            Console.WriteLine();
        }
    }
}
