using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleIOGuiElementRenderer
{
    public class Helper
    {
        /// <summary>
        /// kopiert den Inhalt (Text) einer (kleineren) Matrix in eine größere; dabei werden möglicherweise Werte der größeren Matrix überschrieben
        /// die Ränder werden freigelassen
        /// </summary>
        /// <param name="smallMatrix">gibt die kleinere Matrix (mit Braille-Text an) an</param>
        /// <param name="bigMatrix">gibt die größere Matrix an</param>
        /// <param name="spaceLeft">gibt an wieviele pins nach Links (und  Rechts) frei bleiben sollen</param>
        public static void copyTextMatrixInMatrix(bool[,] smallMatrix, ref bool[,] bigMatrix, int spaceLeft)
        {
            //Console.WriteLine("GetLength = {0},  GetUpperBound = {1}", bigMatrix.GetLength(0), bigMatrix.GetUpperBound(0));
            // getLength ^= Anzahl der Zeilen
            // Length ^= Anzahl der Elemente insgesamt
            // Length / getLength ^= Pins pro Zeile
            for (int i = spaceLeft; (i < (bigMatrix.Length / bigMatrix.GetLength(0)) - 2) && (i < (smallMatrix.Length / smallMatrix.GetLength(0))); i++)//ab Pinreihe 'spaceLeft' soll der Text erscheinen
            {
                for (int j = 2; (j < bigMatrix.GetLength(0) - 2) && (j < smallMatrix.GetLength(0) + 2); j++)
                {
                    bigMatrix[j, i] = smallMatrix[j - 2, i - spaceLeft];
                }
            }
        }

        /// <summary>
        /// kopiert den Inhalt einer (kleineren) Matrix in eine größere; dabei werden möglicherweise Werte der größeren Matrix überschrieben
        /// </summary>
        /// <param name="smallMatrix">ibt die kleinere Matrix an</param>
        /// <param name="bigMatrix">ibt die größere Matrix an</param>
        public static void copyMatrixInMatrix(bool[,] smallMatrix, ref bool[,] bigMatrix)
        {
            for (int i = 0; (i < (bigMatrix.Length / bigMatrix.GetLength(0))) && (i < (smallMatrix.Length / smallMatrix.GetLength(0))); i++)
            {
                for (int j = 0; (j < bigMatrix.GetLength(0) ) && (j < smallMatrix.GetLength(0) ); j++)
                {
                    bigMatrix[j, i] = smallMatrix[j, i ];
                }
            }
        }

        /// <summary>
        /// Erstellt eine Box (Umrandung) in der angegebenen Größe
        /// </summary>
        /// <param name="heightView">gibt die Höhe der Box an</param>
        /// <param name="widthView">gibt die Breite der Box an</param>
        /// <returns>gibt eine Bool-Matrix der Box zurück</returns>
        public static bool[,] createBox(int heightView, int widthView)
        {
            bool[,] viewMatrix = new bool[heightView, widthView];
            for (int height = 0; height < heightView; height++)
            {
                for (int width = 0; width < widthView; width++)
                {
                    if (height == 0 || height == heightView - 1)
                    {
                        viewMatrix[height, width] = true;
                    }
                    else
                    {
                        if (width == 0 || width == widthView - 1)
                        {
                            viewMatrix[height, width] = true;
                        }
                    }
                }
            }
            return viewMatrix;
        }

        /// <summary>
        /// Erstellt eine Box mit gestrichelten Linien oben und unten --> Element ist deaktiviert
        /// </summary>
        /// <param name="heightView">gibt die Höhe der Box an</param>
        /// <param name="widthView">gibt die Breite der Box an</param>
        /// <returns>gibt eine Bool-Matrix der Box zurück</returns>
        public static bool[,] createBoxDeaktivatedUpDown(int heightView, int widthView)
        {
            bool[,] viewMatrix = new bool[heightView, widthView];
            for (int height = 0; height < heightView; height++)
            {
                for (int width = 0; width < widthView; width++)
                {
                    if (height == 0 || height == heightView - 1)
                    {
                        if (width % 4 == 0)
                        {
                            viewMatrix[height, width] = true;
                            if (width + 1 < widthView)
                            {
                                viewMatrix[height, width + 1] = true;
                            }

                        }
                        else
                        {
                            if (width == widthView-1)
                            {//damit das Menuelement noch Erkennbar ist, den letzten Pin mit setzen
                            //    viewMatrix[height, width + -1] = true;
                                viewMatrix[height, width ] = true; 
                            }
                        }                        
                    }
                    else
                    {
                        if (width == 0 || width == widthView - 1)
                        {
                            viewMatrix[height, width] = true;
                        }
                    }
                }
            }
            return viewMatrix;
        }

        /// <summary>
        /// Erstellt eine Box mit gestrichelten Linien links  --> Element ist deaktiviert
        /// </summary>
        /// <param name="heightView">gibt die Höhe der Box an</param>
        /// <param name="widthView">gibt die Breite der Box an</param>
        /// <returns>gibt eine Bool-Matrix der Box zurück</returns>
        public static bool[,] createBoxDeaktivatedLeft(int heightView, int widthView)
        {
            bool[,] viewMatrix = new bool[heightView, widthView];
            for (int height = 0; height < heightView; height++)
            {
                for (int width = 0; width < widthView; width++)
                {
                    if (height == 0 || height == heightView - 1)
                    {
                        viewMatrix[height, width] = true;
                    }
                    else
                    {
                        if (width == 0)
                        {
                            if (height % 4 == 0)
                            {
                                viewMatrix[height, width] = true;
                                if (height + 1 < heightView)
                                {
                                    viewMatrix[height+1, width ] = true;
                                }
                            }
                            else
                            {
                                if (height == heightView - 1)
                                {//damit das Menuelement noch Erkennbar ist, den letzten Pin mit setzen
                                    //    viewMatrix[height+1, width] = true;
                                    viewMatrix[height, width] = true;
                                }
                            }
                        }
                        else
                        {
                            if ( width == widthView - 1)
                            {
                                viewMatrix[height, width] = true;
                            }
                        }
                    }
                }
            }
            return viewMatrix;
        }


        /// <summary>
        /// entfernt von einer bool-Matrix den linken Rand
        /// </summary>
        /// <param name="viewMatrix">gibt die Referenz zur Matrix an</param>
        public static void RemoveLeftBoarder(ref bool[,] viewMatrix)
        {
            for (int i = 1; i < viewMatrix.GetLength(0)-1; i++)
            {
                viewMatrix[ i,0] = false;
            }
        }

        /// <summary>
        /// entfernt von einer bool-Matrix den unteren Rand
        /// </summary>
        /// <param name="viewMatrix">gibt die Referenz zur Matrix an</param>
        public static void RemoveDownBoarder(ref bool[,] viewMatrix)
        {
            for (int i = 1; i < (viewMatrix.Length /viewMatrix.GetLength(0))-1 ; i++)
            {
                viewMatrix[viewMatrix.GetLength(0)-1, i] = false;
            }
        }


        /// <summary>
        /// entfernt von einer bool-Matrix den oberen Rand
        /// </summary>
        /// <param name="viewMatrix">gibt die Referenz zur Matrix an</param>
        public static void RemoveUpBoarder(ref bool[,] viewMatrix)
        {
            for (int i = 1; i < (viewMatrix.Length / viewMatrix.GetLength(0)) - 1; i++)
            {
                viewMatrix[0, i] = false;
            }
        }
    }
}
