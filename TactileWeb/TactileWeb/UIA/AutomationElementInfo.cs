using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{
    /// <summary>A small information os a UIA Element</summary>
    public class AutomationElementInfo
    {

        /// <summary>Returns null or the element on this position</summary>
        public static AutomationElementInfo GetElementFromPoint(int x, int y)
        {
            System.Drawing.Point  pt      = new System.Drawing.Point(x,y);
            IUIAutomationElement  element = null;

            if ( ScreenCapture.UIA.ElementFromPoint( pt, out element ) == 0 )
            {
                return new AutomationElementInfo(element);
            }
        
            return null;
        }


        public static AutomationElementInfo GetParentElement(IUIAutomationTreeWalker walker, IUIAutomationElement ele)
        {
            IUIAutomationElement parent = null;

            if ( walker.GetParentElement(ele, out parent) == 0 )
            {
                if ( parent != null)
                {
                    return new AutomationElementInfo(parent);
                }
            }

            return null;
        }



        public string                   Name;
        public ControlType              ControlType;
        public RECT                     Rect;

        /// <summary>Creates infomation from the COM AutomationElement</summary>
        public AutomationElementInfo (IUIAutomationElement ele)
        {

            IntPtr retVal;

            if ( ele.get_CurrentName                   (out retVal ) == 0 )
            {
                Name                  = Marshal.PtrToStringAuto(retVal);
            }

            ele.get_CurrentControlType                 (out ControlType  );
            ele.get_CurrentBoundingRectangle           (out Rect         );



            if ( ControlType == ControlType.DataItem )
            {
                string text = GetDataItemText(ele);

                if ( text != null)
                {
                    Name = text;
                }

            }


        }


        /// <summary>Returns null or the text of a Excel cell</summary>
        protected string GetDataItemText (IUIAutomationElement ele)
        {
            IntPtr pUnk;

            if ( ele.GetCurrentPattern (PATTERNID.UIA_ValuePatternId, out pUnk) == 0 )
            {
                if ( pUnk != IntPtr.Zero )
                {
                    IUIAutomationValuePattern  p = (IUIAutomationValuePattern)Marshal.GetObjectForIUnknown(pUnk);

                    IntPtr retval;
                    if ( p.get_CurrentValue(out retval) == 0 )
                    {
                        string text = Marshal.PtrToStringAuto(retval);

                        if ( text != null )
                        {
                            string[] sa = text.Split('\r');

                            if ( sa.Length == 1 )    return sa[0];          // -->
                            if ( sa.Length == 2 )    return sa[1].Trim();   // -->
                        }
                    }
                }
            }

            return null;
        }






        public override string ToString()
        {
            return String.Format("X:{0}-{1} Y:{2}-{3}", Rect.left, Rect.right,  Rect.top, Rect.bottom);
        }


    }
}