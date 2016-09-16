using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleIO.Interface;
using BrailleIO.Renderer;
using System.Drawing;
using BrailleIOGuiElementRenderer.UiElements;

namespace BrailleIOGuiElementRenderer
{
    public class BrailleIODropDownMenuItemToMatrixRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer
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
            Type typeSpecialContent = uiElement.uiElementSpecialContent.GetType();
            DropDownMenuItem dropDownMenu;
            if(typeof(DropDownMenuItem).Equals(typeSpecialContent)){
                dropDownMenu = (DropDownMenuItem)uiElement.uiElementSpecialContent;
            }
            else { 
                throw new InvalidCastException("Can't cast uiElementSpecialContent to DropDownMenu! {0}"); 
            }
            if (!dropDownMenu.isChild) 
            {//bei der obersten "Leiste" im DropDownMenu muss bei der Ausrichtung nichts unterschieden werden
                return RenderDropDownMenuHorizontal(view, uiElement);
            }
            else
            {
                if (!dropDownMenu.isVertical)
                {// die horizontale Darstellung von Kindelementen entspricht den der obersten "Leiste"
                    return RenderDropDownMenuHorizontal(view, uiElement);
                }
                else
                {
                    return RenderDropDownMenuVertical(view, uiElement);
                }
            }
        }

        private bool[,] RenderDropDownMenuVertical(IViewBoxModel view, UiElement uiContent)
        {//TODO: Element muss eine Mindestgröße haben
            //call pre hooks
            object cM = uiContent.text as object;
            callAllPreHooks(ref view, ref cM);
            bool[,] boxMatrix;
            if(uiContent.isDisabled)
            {
                boxMatrix = Helper.createBoxDeaktivatedLeft(view.ViewBox.Height , view.ViewBox.Width-2); 
            }
            else
            {
                boxMatrix = Helper.createBox(view.ViewBox.Height, view.ViewBox.Width-2); //erstmal eine eckige Matrix // view.ViewBox.Width -2 => da open/close noch angezeigt werden muss
            }
            DropDownMenuItem dropDownMenu = (DropDownMenuItem)uiContent.uiElementSpecialContent; //Der Type mussan dieser Stelle vorher nicht geprüft werden, da das schon in der aufrufenden Methode gemacht wurde
            if (dropDownMenu.hasPrevious)
            {
                //Helper.RemoveDownBoarder(ref boxMatrix);
                Helper.RemoveUpBoarder(ref boxMatrix);
            }
            //String to Braille/Matrix
            MatrixBrailleRenderer m = new MatrixBrailleRenderer();
            bool[,] textMatrix = m.RenderMatrix(view.ViewBox.Width - 4, (uiContent.text as object == null ? "" : uiContent.text as object), false);
            Helper.copyTextMatrixInMatrix(textMatrix, ref boxMatrix, 2);
            if (dropDownMenu.hasNext) { SeparatorNextDropDownMenuElementDown(ref boxMatrix); }
            bool[,] viewMatrix = new bool[view.ViewBox.Height, view.ViewBox.Width];
            // bool[,] viewMatrix =  Helper.createBox(view.ViewBox.Height - 2, view.ViewBox.Width);
            Helper.copyMatrixInMatrix(boxMatrix, ref viewMatrix); // macht platz in der Matrix für open/close
            //Anpassungen je nach spezifischen DropDownMenu
            if (dropDownMenu.hasChild)
            {
                if (dropDownMenu.isOpen) { OpenDropDownMenuElementRight(ref viewMatrix); } else { CloseDropDownMenuElementRight(ref viewMatrix); }
            }
            //call post hooks
            callAllPostHooks(view, cM, ref viewMatrix, false);

            return viewMatrix;
        }

        private bool[,] RenderDropDownMenuHorizontal(IViewBoxModel view, UiElement uiContent)
        {//TODO: Element muss eine Mindestgröße haben
            //call pre hooks
            object cM = uiContent.text as object;
            callAllPreHooks(ref view, ref cM);

            bool[,] boxMatrix;
            if (uiContent.isDisabled)
            {
                boxMatrix = Helper.createBoxDeaktivatedUpDown(view.ViewBox.Height-2, view.ViewBox.Width);
            }
            else
            {
                boxMatrix = Helper.createBox(view.ViewBox.Height-2, view.ViewBox.Width); //erstmal eine eckige Matrix // view.ViewBox.Width -2 => da open/close noch angezeigt werden muss
            }
            DropDownMenuItem dropDownMenu = (DropDownMenuItem)uiContent.uiElementSpecialContent; //Der Type mussan dieser Stelle vorher nicht geprüft werden, da das schon in der aufrufenden Methode gemacht wurde#
            if (dropDownMenu.hasPrevious)
            {
                Helper.RemoveLeftBoarder(ref boxMatrix);
            }
            //String to Braille/Matrix
            MatrixBrailleRenderer m = new MatrixBrailleRenderer();
            bool[,] textMatrix = m.RenderMatrix(view.ViewBox.Width - 4, (uiContent.text as object == null ? "" : uiContent.text as object), false);
            Helper.copyTextMatrixInMatrix(textMatrix, ref boxMatrix, 2);
            if (dropDownMenu.hasNext) { SeparatorNextDropDownMenuElementRight(ref boxMatrix); }
            bool[,] viewMatrix = new bool[view.ViewBox.Height, view.ViewBox.Width];
            // bool[,] viewMatrix =  Helper.createBox(view.ViewBox.Height - 2, view.ViewBox.Width);
            Helper.copyMatrixInMatrix(boxMatrix, ref viewMatrix); // macht platz in der Matrix für open/close
            //Anpassungen je nach spezifischen DropDownMenu
            if (dropDownMenu.hasChild)
            {
                if (dropDownMenu.isOpen) { OpenDropDownMenuElementDown(ref viewMatrix); } else { CloseDropDownMenuElementDown(ref viewMatrix); }
            }
            //call post hooks
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
        {//gestrichelte Linie unten
            int x = (viewMatrix.Length / viewMatrix.GetLength(0)) ;
            for (int i = 2; i < x - 1; i = i + 2)
            {
                viewMatrix[ viewMatrix.GetLength(0)-1,i] = false;
            }
        }
    }
}
