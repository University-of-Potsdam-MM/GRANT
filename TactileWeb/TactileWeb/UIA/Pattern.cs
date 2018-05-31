using System;
using System.Runtime.InteropServices;
using System.Text;


namespace TactileWeb.UIA
{
    public class Pattern
    {

        public readonly string      Name;
        public readonly PATTERNID   Id;
        public string               Text;

        public Pattern ( PATTERNID patternId, string name )
        {
            Id   = patternId;
            Name = name;
        }

        public override string ToString()
        {
            return String.Format("{0}", Name);
        }

        public void Refresh (AutomationElement element)
        {
            Text = CreateText(element);
        }

        private string CreateText(AutomationElement element)
        {
            IUIAutomationElement ele = element.ele;
            IntPtr pUnk;

            if ( ele.GetCurrentPattern (Id, out pUnk) != 0 )    return null;    // -->
            if ( pUnk == IntPtr.Zero )                          return null;    // -->

            object o = Marshal.GetObjectForIUnknown(pUnk);




            // 10000 Invoke
            if ( Id == PATTERNID.UIA_InvokePatternId               )
            {
                IUIAutomationInvokePattern   p = (IUIAutomationInvokePattern)o;
                return "Es kann angeklickt werden";
            }


            // 10001 Selection (Multiselect Liste)
            else if ( Id == PATTERNID.UIA_SelectionPatternId            )
            {
                IUIAutomationSelectionPattern p = (IUIAutomationSelectionPattern)o;

                bool isMultiselect; p.get_CurrentCanSelectMultiple(out isMultiselect);

                IUIAutomationElementArray elements;

                p.GetCurrentSelection(out elements);
                int length = 0; elements.get_Length(out length);

                return String.Format("Ausgewählt={0} Multiselect={1}", length, isMultiselect);
            }


            // 10002 Value
            else if ( Id == PATTERNID.UIA_ValuePatternId                )    
            {
                IUIAutomationValuePattern    p = (IUIAutomationValuePattern)o;

                string text       = null;
                bool   isReadonly = false;

                IntPtr retval;
                if ( p.get_CurrentValue(out retval) == 0 )  text = Marshal.PtrToStringAuto(retval);
                p.get_CurrentIsReadOnly(out isReadonly);

                //string[] sa = System.Text.RegularExpressions.Regex.Split(text, "\r\n");

                //int maxlen = 5;
                //if (text.Length > maxlen)   text = String.Format("{0}...", text.Substring(0,maxlen) );

                return String.Format("IsReadOnly={0}\r\n{1}\r\n", isReadonly, text);
            }


            // 10003 Range (Minimum, Maximum, Value)
            else if ( Id == PATTERNID.UIA_RangeValuePatternId           )
            {
                IUIAutomationRangeValuePattern   p = (IUIAutomationRangeValuePattern)o;

                double min;     p.get_CurrentMinimum(out min  );
                double value;   p.get_CurrentValue  (out value);
                double max;     p.get_CurrentMaximum(out max  );

                return String.Format("Min:{0} Value:{1} Max:{2}", min,value,max);
            }


            // 10004 Scroll (Fenster mit Scrollbars)
            else if ( Id == PATTERNID.UIA_ScrollPatternId               )
            {
                IUIAutomationScrollPattern   p = (IUIAutomationScrollPattern)o;

                double percentX; p.get_CurrentHorizontalScrollPercent (out percentX);
                double percentY; p.get_CurrentVerticalScrollPercent   (out percentY);

                return String.Format("Kann gescrollt werden. Aktuelle Position: X={0}% Y={1}%", percentX, percentY);
            }

            // 10005 Expand / Collaps (TreeView)
            else if ( Id == PATTERNID.UIA_ExpandCollapsePatternId       )
            {
                IUIAutomationExpandCollapsePattern p = (IUIAutomationExpandCollapsePattern)o;

                ExpandCollapseState state; p.get_CurrentExpandCollapseState(out state);

                if      ( state == ExpandCollapseState.ExpandCollapseState_Collapsed          ) return "Nicht erweitert, geschlossen";
                else if ( state == ExpandCollapseState.ExpandCollapseState_Expanded           ) return "Aufgeklappt";
                else if ( state == ExpandCollapseState.ExpandCollapseState_PartiallyExpanded  ) return "Aufgeklappt, aber nicht alle sichtbar";
                else if ( state == ExpandCollapseState.ExpandCollapseState_LeafNode           ) return "Nicht aufgeklappbar. Kein Knoten";
                else                                                                            return "Unbekannt";
            }


            // 10006 Grid
            else if ( Id == PATTERNID.UIA_GridPatternId                 )
            {
                IUIAutomationGridPattern     p = (IUIAutomationGridPattern)o;
                int cols;   p.get_CurrentColumnCount(out cols);
                int rows;   p.get_CurrentRowCount   (out rows);
                return String.Format("Spalten: {0}, Zeilen: {1}", cols, rows);
            }


            // 10007 Grid Item
            else if ( Id == PATTERNID.UIA_GridItemPatternId             )
            {
                IUIAutomationGridItemPattern p = (IUIAutomationGridItemPattern)o;
                int x;   p.get_CurrentColumn(out x);
                int y;   p.get_CurrentRow   (out y);
                return String.Format("Zelle {0} / {1}", x,y);
            }


            // 10008
            else if ( Id == PATTERNID.UIA_MultipleViewPatternId         )
            {
                return "vorhanden";
            }

            // 10009 Window
            else if ( Id == PATTERNID.UIA_WindowPatternId               )
            {
                IUIAutomationWindowPattern p = (IUIAutomationWindowPattern)o;

                WindowVisualState state;    p.get_CurrentWindowVisualState(out state);

                if      ( state == WindowVisualState.WindowVisualState_Normal    )  return "Normal";
                else if ( state == WindowVisualState.WindowVisualState_Maximized )  return "Maximiert";
                else if ( state == WindowVisualState.WindowVisualState_Minimized )  return "Minimiert";
                else                                                                return "Unbekannt";
            }

            // 10010 Selection Item
            else if ( Id == PATTERNID.UIA_SelectionItemPatternId        )
            {
                return "vorhanden";
            }


            // 10011 Dock
            else if      ( Id == PATTERNID.UIA_DockPatternId                 )
            {
                return "vorhanden";
            }

            // 10012 Table
            else if ( Id == PATTERNID.UIA_TablePatternId                )
            {
                IUIAutomationTablePattern    p = (IUIAutomationTablePattern)o;
                return "vorhanden";
            }

            // 10013 TableItem
            else if ( Id == PATTERNID.UIA_TableItemPatternId            )
            {
                IUIAutomationTableItemPattern p = (IUIAutomationTableItemPattern)o;

                IUIAutomationElementArray a;

                p.GetCurrentColumnHeaderItems(out a);

                if ( a == null )
                {
                    return "Headers are null";
                }
                else
                {
                    int c1; a.get_Length(out c1);

                    p.GetCurrentRowHeaderItems(out a);
                    int c2; a.get_Length(out c2);

                    return String.Format("ColumnHeader={0} RowHeaders={1}", c1,c2);
                }
            }


            //// 10024 Text2
            //else if ( Id == PATTERNID.UIA_TextPattern2Id    )
            //{
            //    return "vorhanden";
            //}

            //// 10032 TextEdit
            //else if ( Id == PATTERNID.UIA_TextEditPatternId    )
            //{
            //    return "vorhanden";
            //}


            // 10014 Text (Visual Studio Editor)
            else if ( ( Id == PATTERNID.UIA_TextPatternId ) || ( Id == PATTERNID.UIA_TextPattern2Id ) || ( Id == PATTERNID.UIA_TextEditPatternId ) )
            {
                IntPtr ptr;

                //IUIAutomationTextRangeArray ranges;
                IUIAutomationTextRange      range;
                //int length;
                string text = null;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine();


                if ( Id == PATTERNID.UIA_TextPattern2Id )
                {
                    IUIAutomationTextPattern2 p2 = (IUIAutomationTextPattern2)o;
                    bool isActive;
                    if ( p2.GetCaretRange(out isActive, out range) == 0 )
                    {
                        sb.AppendFormat("Text2: isActive:{0}\r\n", isActive );

                        if ( range != null)
                        {
                            if (range.GetText(-1, out ptr) == 0)
                            {
                                text = Marshal.PtrToStringAuto(ptr);
                                sb.AppendFormat("Caret-Text: {0}\r\n", text );

                            }
                        }
                    }
                }
                else if ( Id == PATTERNID.UIA_TextEditPatternId )
                {
                    IUIAutomationTextEditPattern pe = (IUIAutomationTextEditPattern)o;

                    if ( pe.GetConversionTarget( out range) == 0 )
                    {
                        if ( range != null)
                        {
                            if (range.GetText(-1, out ptr) == 0)
                            {
                                text = Marshal.PtrToStringAuto(ptr);
                                sb.AppendFormat("ConversionTarget: {0}\r\n", text );
                            }
                        }
                    }


                    if ( pe.GetActiveComposition( out range) == 0 )
                    {
                        if ( range != null)
                        {
                            if (range.GetText(-1, out ptr) == 0)
                            {
                                text = Marshal.PtrToStringAuto(ptr);
                                sb.AppendFormat("ActiveCompositiont: {0}\r\n", text );
                            }
                        }
                    }
                }


                IUIAutomationTextPattern p = (IUIAutomationTextPattern)o;



                System.Drawing.Point pt;
                //pt = element.ScreenCapture.CaretPosition;
                pt = element.ScreenCapture.MousePosition;

                sb.AppendFormat("pt={0}\r\n", pt  );





                sb.AppendFormat("{0}\r\n",    GetRangeText(p, pt) );

                //if ( p.RangeFromPoint( element._screenCapture.MousePosition, out range) == 0 )
                //{
                //    sb.Append("MousePosition");

                //    if ( range.GetText(-1, out ptr) == 0 )
                //    {
                //        text = Marshal.PtrToStringAuto(ptr);
                //        sb.AppendFormat("Text={0}", text);
                //    }

                //    if ( range.GetBoundingRectangles(out ptr) == 0 )
                //    {
                //        RECT[] rects = AutomationElement.GetBoundingRectangles(ptr);

                //        foreach(RECT rect in rects)
                //        {
                //            sb.AppendFormat("  X:{0}-{1} Y:{2}-{3}\r\n",  rect.left,rect.right, rect.top, rect.bottom);
                //        }
                //    }
                //    sb.AppendLine();

                //}




                //if ( p.GetSelection(out ranges) == 0 )
                //{
                //    ranges.get_Length(out length);

                //    sb.AppendFormat("Selections: {0}\r\n", length);

                //    for (int i = 0; i < length; i++)
                //    {
                //        if ( ranges.GetElement(i, out range) == 0 )
                //        {

                //            // Test
                //            //ptr = IntPtr.Zero;

                //            //if ( range.GetAttributeValue(TEXTATTRIBUTEID.UIA_CaretPositionAttributeId, out ptr) == 0 )
                //            //{
                //            //     int variantType = Marshal.ReadInt32(ptr, 0);
                //            //     int i4          = Marshal.ReadInt32(ptr, 8);
                //            //}







                //            int childCount = 0;

                //            IUIAutomationElementArray children;
                //            if ( range.GetChildren(out children) == 0 )
                //            {
                //                children.get_Length(out childCount);
                //            }

                //            if ( range.GetText(-1, out ptr) == 0 )
                //            {
                //                text = Marshal.PtrToStringAuto(ptr);
                //            }

                //            sb.AppendFormat(" Sel{0} Children:{1} Text:{2}\r\n", i, childCount, text);



                //            if ( range.GetBoundingRectangles(out ptr) == 0 )
                //            {
                //                RECT[] rects = AutomationElement.GetBoundingRectangles(ptr);

                //                foreach(RECT rect in rects)
                //                {
                //                    sb.AppendFormat("  X:{0}-{1} Y:{2}-{3}\r\n",  rect.left,rect.right, rect.top, rect.bottom);
                //                }
                //            }

                //        }
                //    }
                //}


                //if ( p.GetVisibleRanges(out ranges) == 0 )
                //{
                //    ranges.get_Length(out length);
                //    sb.AppendFormat("VisibleRanges: {0}\r\n", length);

                //    for (int i = 0; i < length; i++)
                //    {
                //        if ( ranges.GetElement(i, out range) == 0 )
                //        {
                //            if ( range.GetText(-1, out ptr) == 0 )
                //            {
                //                text = Marshal.PtrToStringAuto(ptr);
                //                sb.AppendFormat("{0}= {1}\r\n",  i, text);

                //                IntPtr boundingRects;
                //                if ( range.GetBoundingRectangles(out boundingRects) == 0 )
                //                {
                //                    AutomationElement.GetBoundingRectangles(boundingRects);
                //                }

                //            }
                //        }
                //    }
                //}




                return sb.ToString();
            }



            // 10015 Toggle (Checkbox)
            else if ( Id == PATTERNID.UIA_TogglePatternId               )
            {
                IUIAutomationTogglePattern p = (IUIAutomationTogglePattern)o;

                ToggleState state;  p.get_CurrentToggleState(out state);

                if      ( state == ToggleState.ToggleState_Off           )      return "Aus";
                else if ( state == ToggleState.ToggleState_On            )      return "An";
                else if ( state == ToggleState.ToggleState_Indeterminate )      return "Inaktiv";
                else                                                            return "Unbekannt";
            }



            // 10016 Transform (Can move, resize, rotate)
            else if ( Id == PATTERNID.UIA_TransformPatternId            )
            {
                IUIAutomationTransformPattern p = (IUIAutomationTransformPattern)o;

                StringBuilder sb = new StringBuilder();

                bool retVal;
                p.get_CurrentCanMove   (out retVal);   if (retVal == true) sb.AppendFormat("verschiebbar, ");
                p.get_CurrentCanResize (out retVal);   if (retVal == true) sb.AppendFormat("vergrößerbar, ");
                p.get_CurrentCanRotate (out retVal);   if (retVal == true) sb.AppendFormat("rotierbar, ");

                if (sb.Length == 0) sb.Append("Nichts"); else sb.Length -=2;

                return sb.ToString();
            }



            // 10017
            else if ( Id == PATTERNID.UIA_ScrollItemPatternId           )
            {
                IUIAutomationScrollItemPattern   p = (IUIAutomationScrollItemPattern)o;
                return "Das Element kann in den sichtbaren bereich gescrollt werden";
            }



            // 10018 Legacy
            else if ( Id == PATTERNID.UIA_LegacyIAccessiblePatternId    )
            {
                IUIAutomationLegacyIAccessiblePattern p = (IUIAutomationLegacyIAccessiblePattern)o;
                IntPtr ptr;
                string name  = null;    if ( p.get_CurrentName (out ptr) == 0 ) name  = Marshal.PtrToStringAuto(ptr);
                string value = null;    if ( p.get_CurrentValue(out ptr) == 0 ) value = Marshal.PtrToStringAuto(ptr);

                return null;
                //return String.Format("Name={0} Value={1}", name, value);
            }



            // 10019
            else if ( Id == PATTERNID.UIA_ItemContainerPatternId        )
            {
                return "vorhanden";
            }




            // 10020
            else if ( Id == PATTERNID.UIA_VirtualizedItemPatternId      )
            {
                return "vorhanden";
            }


            // 10021
            else if ( Id == PATTERNID.UIA_SynchronizedInputPatternId    )
            {
                return "vorhanden";
            }












            // 10022 Object Model
            else if ( Id == PATTERNID.UIA_ObjectModelPatternId    )
            {
                return "vorhanden";
            }

            // 10023 Annotation
            else if ( Id == PATTERNID.UIA_AnnotationPatternId    )
            {
                return "vorhanden";
            }







            // 10025 Styles
            else if ( Id == PATTERNID.UIA_StylesPatternId    )
            {
                return "vorhanden";
            }

            // 10026 Spreadsheet
            else if ( Id == PATTERNID.UIA_SpreadsheetPatternId    )
            {
                return "vorhanden";
            }

            // 10027 Spreadsheet Item
            else if ( Id == PATTERNID.UIA_SpreadsheetItemPatternId    )
            {
                return "vorhanden";
            }

            // 10028 Transform2
            else if ( Id == PATTERNID.UIA_TransformPattern2Id    )
            {
                return "vorhanden";
            }

            // 10029 TextChild
            else if ( Id == PATTERNID.UIA_TextChildPatternId    )
            {
                return "vorhanden";
            }

            // 10030 Drag
            else if ( Id == PATTERNID.UIA_DragPatternId    )
            {
                return "vorhanden";
            }

            // 10031 Drop Target
            else if ( Id == PATTERNID.UIA_DropTargetPatternId    )
            {
                return "vorhanden";
            }



            // 10033 UIA_Custom Navigation
            else if ( Id == PATTERNID.UIA_CustomNavigationPatternId    )
            {
                return "vorhanden";
            }


            else
            {
                return "vorhanden";
            }




        }


        private string GetRangeText (IUIAutomationTextPattern p, System.Drawing.Point pt)
        {
            StringBuilder sb = new StringBuilder();

            int x = pt.X;
            int y = pt.Y;

            IUIAutomationTextRangeArray ranges;

            if ( p.GetVisibleRanges(out ranges) == 0 )
            {
                int length; ranges.get_Length(out length);

                for (int i = 0; i < length; i++)
                {
                    IUIAutomationTextRange      range;

                    if ( ranges.GetElement(i, out range) == 0 )
                    {
                        IntPtr boundingRects;
                        if ( range.GetBoundingRectangles(out boundingRects) == 0 )
                        {
                            RECT[] ra = AutomationElement.GetBoundingRectangles(boundingRects);

                            foreach(RECT r in ra)
                            {
                                if ( (x >= r.left) && (x <= r.right) && (y >= r.top) && (y <= r.bottom) )
                                {
                                    IntPtr ptr;
                                    if ( range.GetText(-1, out ptr) == 0 )
                                    {
                                        string text = Marshal.PtrToStringAuto(ptr);
                                        return text;
                                    }
                                }
                            }

                        }
                    }
                }
            }

            return null;
        }
 




    }
}