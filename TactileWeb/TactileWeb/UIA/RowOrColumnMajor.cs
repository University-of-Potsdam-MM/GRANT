using System;

namespace TactileWeb.UIA
{

    // https://msdn.microsoft.com/en-us/library/windows/desktop/ee671604(v=vs.85).aspx
    public enum RowOrColumnMajor
    {
        /// <summary>Data in the table should be read row by row.</summary>
        RowMajor        = 0,

        /// <summary>Data in the table should be read column by column.</summary>
        ColumnMajor     = 1,

        /// <summary>The best way to present the data is indeterminate.</summary>
        Indeterminate   = 2,
    }
}