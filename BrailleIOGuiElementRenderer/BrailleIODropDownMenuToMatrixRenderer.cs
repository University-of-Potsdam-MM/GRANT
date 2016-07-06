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
    public class BrailleIODropDownMenuToMatrixRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer
    {
        public bool[,] RenderMatrix(IViewBoxModel view, object otherContent)
        {
            DropDownMenu dropDownMenu;
            Type typeOtherContent = otherContent.GetType();
            if (typeof(DropDownMenu).Equals(typeOtherContent))
            {
                dropDownMenu = (DropDownMenu)otherContent;
            }
            else
            {
                throw new InvalidCastException("Can't cast otherContent to dropDownMenu! {0}");
            }
            if (!dropDownMenu.isChild) 
            {//bei der obersten "Leiste" im DropDownMenu muss bei der Ausrichtung nichts unterschieden werden
                return RenderDropDownMenuHorizontal(view, dropDownMenu);
            }
            else
            {
                if (!dropDownMenu.isVertical)
                {// die horizontale Darstellung von Kindelementen entspricht den der obersten "Leiste"
                    return RenderDropDownMenuHorizontal(view, dropDownMenu);
                }
                else
                {
                    return RenderDropDownMenuVertical(view, dropDownMenu);
                }
            }
        }

        private bool[,] RenderDropDownMenuVertical(IViewBoxModel view, DropDownMenu dropDownMenu)
        {//TODO: Element muss eine Mindestgröße haben
            //call pre hooks  --> wie funktioniert das richtig?
            object cM = dropDownMenu.text as object;
            callAllPreHooks(ref view, ref cM);

            bool[,] boxMatrix = Helper.createBox(view.ViewBox.Height, view.ViewBox.Width - 2); //erstmal eine eckige Matrix // view.ViewBox.Width -2 => da open/close noch angezeigt werden muss
            if (dropDownMenu.hasPrevious)
            {
                Helper.RemoveDownBoarder(ref boxMatrix);
            }
            //String to Braille/Matrix
            MatrixBrailleRenderer m = new MatrixBrailleRenderer();
            bool[,] textMatrix = m.RenderMatrix(view.ViewBox.Width - 4, (dropDownMenu.text as object == null ? "" : dropDownMenu.text as object), false);
            Helper.copyTextMatrixInMatrix(textMatrix, ref boxMatrix);
            if (dropDownMenu.hasNext) { SeparatorNextDropDownMenuElementDown(ref boxMatrix); }
            bool[,] viewMatrix = new bool[view.ViewBox.Height, view.ViewBox.Width];
            // bool[,] viewMatrix =  Helper.createBox(view.ViewBox.Height - 2, view.ViewBox.Width);
            Helper.copyMatrixInMatrix(boxMatrix, ref viewMatrix); // macht platz in der Matrix für open/close
            //Anpassungen je nach spezifischen DropDownMenu
            if (dropDownMenu.hasChild)
            {
                if (dropDownMenu.isOpen) { OpenDropDownMenuElementRight(ref viewMatrix); } else { CloseDropDownMenuElementRight(ref viewMatrix); }
            }
            //call post hooks --> wie funktioniert das richtig?
            callAllPostHooks(view, cM, ref viewMatrix, false);

            return viewMatrix;
        }

        private bool[,] RenderDropDownMenuHorizontal(IViewBoxModel view, DropDownMenu dropDownMenu)
        {//TODO: Element muss eine Mindestgröße haben
            //call pre hooks  --> wie funktioniert das richtig?
            object cM = dropDownMenu.text as object;
            callAllPreHooks(ref view, ref cM);
            
            bool[,] boxMatrix = Helper.createBox(view.ViewBox.Height - 2, view.ViewBox.Width); //erstmal eine eckige Matrix // view.ViewBox.Height -2 => da open/close noch angezeigt werden muss
            if (dropDownMenu.hasPrevious)
            {
                Helper.RemoveLeftBoarder(ref boxMatrix);
            }
            //String to Braille/Matrix
            MatrixBrailleRenderer m = new MatrixBrailleRenderer();
            bool[,] textMatrix = m.RenderMatrix(view.ViewBox.Width - 4, (dropDownMenu.text as object == null ? "" : dropDownMenu.text as object), false);
            Helper.copyTextMatrixInMatrix(textMatrix, ref boxMatrix);
            if (dropDownMenu.hasNext) { SeparatorNextDropDownMenuElementRight(ref boxMatrix); }
            bool[,] viewMatrix = new bool[view.ViewBox.Height, view.ViewBox.Width];
            // bool[,] viewMatrix =  Helper.createBox(view.ViewBox.Height - 2, view.ViewBox.Width);
            Helper.copyMatrixInMatrix(boxMatrix, ref viewMatrix); // macht platz in der Matrix für open/close
            //Anpassungen je nach spezifischen DropDownMenu
            if (dropDownMenu.hasChild)
            {
                if (dropDownMenu.isOpen) { OpenDropDownMenuElementDown(ref viewMatrix); } else { CloseDropDownMenuElementDown(ref viewMatrix); }
            }
            //call post hooks --> wie funktioniert das richtig?
            callAllPostHooks(view, cM, ref viewMatrix, false);

            return viewMatrix;
        }

        private void OpenDropDownMenuElementRight(ref bool[,] viewMatrix)
        {
            int y =( viewMatrix.Length / viewMatrix.GetLength(0)) -1;
            int center = viewMatrix.GetLength(0) / 2;
            viewMatrix[center -1, y-2] = false;
            viewMatrix[center +1, y-2] = false;
            viewMatrix[center, y-2] = false;
            viewMatrix[center -1, y - 1] = true;
            viewMatrix[center+1, y-1] = true;
            viewMatrix[center, y] = true;

        }

        private void CloseDropDownMenuElementRight(ref bool[,] viewMatrix)
        {
         /*   int x = (viewMatrix.Length / viewMatrix.GetLength(0)) - 1;
            for (int i = 2; i < viewMatrix.GetLength(0) - 2; i++) 
            {
                viewMatrix[i, x - 2] = false;
                viewMatrix[i, x - 1] = true;
            }*/
            int y = (viewMatrix.Length / viewMatrix.GetLength(0)) - 1;
            int center = viewMatrix.GetLength(0) / 2;
            viewMatrix[center - 1, y - 2] = false;
            viewMatrix[center + 1, y - 2] = false;
            viewMatrix[center, y - 2] = false;
            viewMatrix[center - 1, y - 1] = true;
            viewMatrix[center + 1, y - 1] = true;
            viewMatrix[center, y-1] = true;
        }


        private void OpenDropDownMenuElementDown(ref bool[,] viewMatrix)
        {//unten
            viewMatrix[viewMatrix.GetLength(0) - 3, 3] = false;
            viewMatrix[viewMatrix.GetLength(0) - 3, 4] = false;
            viewMatrix[viewMatrix.GetLength(0) - 3, 5] = false;
            viewMatrix[viewMatrix.GetLength(0) - 2, 3] = true;
            viewMatrix[viewMatrix.GetLength(0) - 2, 5] = true;
            viewMatrix[viewMatrix.GetLength(0) - 1, 4] = true;
        }

        private void CloseDropDownMenuElementDown(ref bool[,] viewMatrix)
        {//unten
            viewMatrix[viewMatrix.GetLength(0) - 3, 3] = false;
            viewMatrix[viewMatrix.GetLength(0) - 3, 4] = false;
            viewMatrix[viewMatrix.GetLength(0) - 3, 5] = false;
            viewMatrix[viewMatrix.GetLength(0) - 2, 3] = true;
            viewMatrix[viewMatrix.GetLength(0) - 2, 5] = true;
            viewMatrix[viewMatrix.GetLength(0) - 2, 4] = true;
        }

        private void SeparatorNextDropDownMenuElementRight(ref bool[,] viewMatrix)
        {//gestrichelte Linie rechts
            int length = (viewMatrix.Length / viewMatrix.GetLength(0)) - 1;
            for (int i = 2; i < viewMatrix.GetLength(0) -1; i = i + 2)
            {
                viewMatrix[i, length] = false;
            }
        }

        private void SeparatorNextDropDownMenuElementDown(ref bool[,] viewMatrix)
        {//gestrichelte Linie rechts
            int x = (viewMatrix.Length / viewMatrix.GetLength(0)) ;
            for (int i = 2; i < x - 1; i = i + 2)
            {
                viewMatrix[ viewMatrix.GetLength(0)-1,i] = false;
            }
        }
    }
}
