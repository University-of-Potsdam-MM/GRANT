using System;
using System.Collections;

namespace TactileWeb.UIA
{
    public class PatternCollection : IEnumerable
    {
        protected ArrayList _list;


        public PatternCollection()
        { 
            _list = new ArrayList();

            _list.Add ( new Pattern(PATTERNID.UIA_InvokePatternId              ,"Invoke"            ) );
            _list.Add ( new Pattern(PATTERNID.UIA_SelectionPatternId           ,"Selection"         ) );
            _list.Add ( new Pattern(PATTERNID.UIA_ValuePatternId               ,"Value"             ) );
            _list.Add ( new Pattern(PATTERNID.UIA_RangeValuePatternId          ,"RangeValue"        ) );
            _list.Add ( new Pattern(PATTERNID.UIA_ScrollPatternId              ,"Scroll"            ) );
            _list.Add ( new Pattern(PATTERNID.UIA_ExpandCollapsePatternId      ,"Expand-Collapse"   ) );
            _list.Add ( new Pattern(PATTERNID.UIA_GridPatternId                ,"Grid"              ) );
            _list.Add ( new Pattern(PATTERNID.UIA_GridItemPatternId            ,"GridItem"          ) );
            _list.Add ( new Pattern(PATTERNID.UIA_MultipleViewPatternId        ,"MultipleView"      ) );
            _list.Add ( new Pattern(PATTERNID.UIA_WindowPatternId              ,"Window"            ) );
            _list.Add ( new Pattern(PATTERNID.UIA_SelectionItemPatternId       ,"SelectionItem"     ) );
            _list.Add ( new Pattern(PATTERNID.UIA_DockPatternId                ,"Dock"              ) );
            _list.Add ( new Pattern(PATTERNID.UIA_TablePatternId               ,"Table"             ) );
            _list.Add ( new Pattern(PATTERNID.UIA_TableItemPatternId           ,"TableItem"         ) );
            _list.Add ( new Pattern(PATTERNID.UIA_TextPatternId                ,"Text"              ) );
            _list.Add ( new Pattern(PATTERNID.UIA_TogglePatternId              ,"Toggle"            ) );
            _list.Add ( new Pattern(PATTERNID.UIA_TransformPatternId           ,"Transform"         ) );
            _list.Add ( new Pattern(PATTERNID.UIA_ScrollItemPatternId          ,"ScrollItem"        ) );
            _list.Add ( new Pattern(PATTERNID.UIA_LegacyIAccessiblePatternId   ,"LegacyIAccessible" ) );
            _list.Add ( new Pattern(PATTERNID.UIA_ItemContainerPatternId       ,"ItemContainer"     ) );
            _list.Add ( new Pattern(PATTERNID.UIA_VirtualizedItemPatternId     ,"VirtualizedItem"   ) );
            _list.Add ( new Pattern(PATTERNID.UIA_SynchronizedInputPatternId   ,"SynchronizedInput" ) );
            _list.Add ( new Pattern(PATTERNID.UIA_ObjectModelPatternId         ,"ObjectModel"       ) );
            _list.Add ( new Pattern(PATTERNID.UIA_AnnotationPatternId          ,"Annotation"        ) );
            _list.Add ( new Pattern(PATTERNID.UIA_TextPattern2Id               ,"Text2"             ) );
            _list.Add ( new Pattern(PATTERNID.UIA_StylesPatternId              ,"Styles"            ) );
            _list.Add ( new Pattern(PATTERNID.UIA_SpreadsheetPatternId         ,"Spreadsheet"       ) );
            _list.Add ( new Pattern(PATTERNID.UIA_SpreadsheetItemPatternId     ,"SpreadsheetItem"   ) );
            _list.Add ( new Pattern(PATTERNID.UIA_TransformPattern2Id          ,"Transform2"        ) );
            _list.Add ( new Pattern(PATTERNID.UIA_TextChildPatternId           ,"TextChild"         ) );
            _list.Add ( new Pattern(PATTERNID.UIA_DragPatternId                ,"Drag"              ) );
            _list.Add ( new Pattern(PATTERNID.UIA_DropTargetPatternId          ,"DropTarget"        ) );
            _list.Add ( new Pattern(PATTERNID.UIA_TextEditPatternId            ,"TextEdit"          ) );
            _list.Add ( new Pattern(PATTERNID.UIA_CustomNavigationPatternId    ,"CustomNavigation"  ) );
        }


        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public Pattern this[int index]
        { 
            get
            {
                return (Pattern)_list[index];
            }
        }


        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }



    }
}