using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BrailleIO;

namespace StrategyBrailleIO
{
    class BrailleUserinterface
    {
        public static void createViewText(BrailleIOScreen screen, String text, String viewName, int left, int top, int width, int height)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(left, top, width, height, new bool[0, 0]);
            vr.SetText(text);
            vr.ShowScrollbars = true;
            //vr.SetPadding(2); //TODO
            //vr.SetMargin(2);
            //vr.SetBorder(1,0);

            screen.AddViewRange(viewName, vr);
        }

        public static void createViewMatrix(BrailleIOScreen screen, bool[,] matrix, String viewName, int left, int top, int width, int height)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(left, top, width, height, new bool[0, 0]);
            vr.SetMatrix(matrix);
            screen.AddViewRange(viewName, vr);
        }

        public static void createViewImage(BrailleIOScreen screen, Image image, String viewName, int left, int top, int width, int height)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(left, top, width, height, new bool[0, 0]);
            vr.SetBitmap(image);
            screen.AddViewRange(viewName, vr);
        }
    }
}
