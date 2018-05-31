using System;

namespace TactileWeb.UIA
{

    // https://msdn.microsoft.com/en-us/library/windows/desktop/ee671662(v=vs.85).aspx
    public enum TEXTATTRIBUTEID
    {
        UIA_AnimationStyleAttributeId           = 40000,
        UIA_BackgroundColorAttributeId          = 40001,
        UIA_BulletStyleAttributeId              = 40002,
        UIA_CapStyleAttributeId                 = 40003,
        UIA_CultureAttributeId                  = 40004,
        UIA_FontNameAttributeId                 = 40005,
        UIA_FontSizeAttributeId                 = 40006,
        UIA_FontWeightAttributeId                = 40007,
        UIA_ForegroundColorAttributeId          = 40008,
        UIA_HorizontalTextAlignmentAttributeId  = 40009,
        UIA_IndentationFirstLineAttributeId     = 40010,
        UIA_IndentationLeadingAttributeId       = 40011,
        UIA_IndentationTrailingAttributeId      = 40012,
        UIA_IsHiddenAttributeId                 = 40013,
        UIA_IsItalicAttributeId                 = 40014,
        UIA_IsReadOnlyAttributeId               = 40015,
        UIA_IsSubscriptAttributeId              = 40016,
        UIA_IsSuperscriptAttributeId            = 40017,
        UIA_MarginBottomAttributeId             = 40018,
        UIA_MarginLeadingAttributeId            = 40019,
        UIA_MarginTopAttributeId                = 40020,
        UIA_MarginTrailingAttributeId           = 40021,
        UIA_OutlineStylesAttributeId            = 40022,
        UIA_OverlineColorAttributeId            = 40023,
        UIA_OverlineStyleAttributeId            = 40024,
        UIA_StrikethroughColorAttributeId       = 40025,
        UIA_StrikethroughStyleAttributeId       = 40026,
        UIA_TabsAttributeId                     = 40027,
        UIA_TextFlowDirectionsAttributeId       = 40028,
        UIA_UnderlineColorAttributeId           = 40029,
        UIA_UnderlineStyleAttributeId           = 40030,
        UIA_AnnotationTypesAttributeId          = 40031,
        UIA_AnnotationObjectsAttributeId        = 40032,
        UIA_StyleNameAttributeId                = 40033,
        UIA_StyleIdAttributeId                  = 40034,
        UIA_LinkAttributeId                     = 40035,
        UIA_IsActiveAttributeId                 = 40036,
        UIA_SelectionActiveEndAttributeId       = 40037,

        /// <summary>This attribute is specified as a value from the CaretPosition enumerated type. Supported starting with Windows 8. Variant type: VT_I4</summary>
        UIA_CaretPositionAttributeId            = 40038,
        UIA_CaretBidiModeAttributeId            = 40039
    }
}