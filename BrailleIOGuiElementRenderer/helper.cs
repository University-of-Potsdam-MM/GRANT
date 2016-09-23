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
            // getLength(0) ^= Anzahl der Zeilen
            // Length ^= Anzahl der Elemente insgesamt
            // Length / getLength ^= Pins pro Zeile (.GetLength(1) for the number of column)
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
        /// <param name="smallMatrix">ist die kleinere Matrix an</param>
        /// <param name="bigMatrix">ibt die größere Matrix an</param>
        /// <param name="startX">gibt an wieviele Pins nach Links frei bleiben sollen</param>
        /// <param name="startY">gibt an wieviele Pins nach oben frei bleiben sollen</param>
        public static void copyMatrixInMatrix(bool[,] smallMatrix, ref bool[,] bigMatrix, int startX = 0, int startY = 0)
        {
            for (int i = startX; (i < bigMatrix.GetLength(1)) && (i - startX < smallMatrix.GetLength(1)); i++)
            {
                for (int j = startY; (j < bigMatrix.GetLength(0)) && (j - startY < smallMatrix.GetLength(0)); j++)
                {
                    bigMatrix[j, i] = smallMatrix[j - startY, i - startX];
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
        /// Erstellt eine ausgewählte Box (alle Pins gesetzt) in der angegebenen Größe
        /// </summary>
        /// <param name="heightView">gibt die Höhe der Box an</param>
        /// <param name="widthView">gibt die Breite der Box an</param>
        /// <returns>gibt eine Bool-Matrix der Box zurück</returns>
        public static bool[,] createSelectedBox(int heightView, int widthView)
        {
            bool[,] viewMatrix = new bool[heightView, widthView];
            for (int height = 0; height < heightView; height++)
            {
                for (int width = 0; width < widthView; width++)
                {
                    viewMatrix[height, width] = true;
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
        /// entfernt von einer bool-Matrix den linken Rand
        /// </summary>
        /// <param name="viewMatrix">gibt die Referenz zur Matrix an</param>
        public static void RemoveRightBoarder(ref bool[,] viewMatrix)
        {
            for (int i = 1; i < viewMatrix.GetLength(0) - 1; i++)
            {
                viewMatrix[i, viewMatrix.GetLength(1)-1] = false;
            }
        }

        /// <summary>
        /// entfernt von einer bool-Matrix den unteren Rand
        /// </summary>
        /// <param name="viewMatrix">gibt die Referenz zur Matrix an</param>
        public static void RemoveBottomBoarder(ref bool[,] viewMatrix)
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
        public static void RemoveTopBoarder(ref bool[,] viewMatrix)
        {
            for (int i = 1; i < (viewMatrix.Length / viewMatrix.GetLength(0)) - 1; i++)
            {
                viewMatrix[0, i] = false;
            }
        }

        public static bool[,] createInterruptedLine(int width)
        {
            bool[,] line = new bool[1, width];
            for (int i = 0; i < width; i = i + 2)
            {
                line[0, i] = true;
            }
            return line;
        }

        public static bool[,] createDownBorder(int heightView, int widthView)
        {
            bool[,] viewMatrix = new bool[heightView, widthView];

            for (int width = 0; width < widthView; width++)
            {
                    viewMatrix[heightView-1, width] = true;
            }
            return viewMatrix;
        }

        public static bool[,] createLeftBorder(int heightView, int widthView)
        {
            bool[,] viewMatrix = new bool[heightView, widthView];
            for (int height = 0; height < heightView; height++)
            {
                    viewMatrix[height, 0] = true;
                
            }
            return viewMatrix;
        }

        internal static bool[,] createRightBorder(int heightView, int widthView)
        {
            bool[,] viewMatrix = new bool[heightView, widthView];
            for (int height = 0; height < heightView; height++)
            {
                viewMatrix[height, widthView-1] = true;

            }
            return viewMatrix;
        }

        internal static bool[,] createUpBorder(int heightView, int widthView)
        {
            bool[,] viewMatrix = new bool[heightView, widthView];

            for (int width = 0; width < widthView; width++)
            {
                viewMatrix[0,width]  = true;
            }
            return viewMatrix;
        }
    }
}
