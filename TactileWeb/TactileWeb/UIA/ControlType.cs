using System;

namespace TactileWeb.UIA
{

    // https://msdn.microsoft.com/en-us/library/dd757486(v=vs.85).aspx
    public enum ControlType
    {
        Unknown       = 0,
        Button        = 50000,
        Calendar      = 50001,
        CheckBox      = 50002,
        ComboBox      = 50003,
        Custom        = 50025,
        DataGrid      = 50028,
        DataItem      = 50029,
        Document      = 50030,
        Edit          = 50004,
        Group         = 50026,
        Header        = 50034,
        HeaderItem    = 50035,
        Hyperlink     = 50005,
        Image         = 50006,
        List          = 50008,
        ListItem      = 50007,
        MenuBar       = 50010,
        Menu          = 50009,
        MenuItem      = 50011,
        Pane          = 50033,
        ProgressBar   = 50012,
        RadioButton   = 50013,
        ScrollBar     = 50014,
        Separator     = 50038,
        Slider        = 50015,
        Spinner       = 50016,
        SplitButton   = 50031,
        StatusBar     = 50017,
        Tab           = 50018,
        TabItem       = 50019,
        Table         = 50036,
        Text          = 50020,
        Thumb         = 50027,
        TitleBar      = 50037,
        ToolBar       = 50021,
        ToolTip       = 50022,
        Tree          = 50023,
        TreeItem      = 50024,
        Window        = 50032

    }
}