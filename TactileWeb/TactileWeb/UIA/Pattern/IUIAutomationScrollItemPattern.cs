using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("b488300f-d015-4f19-9c29-bb595e3645ef"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationScrollItemPattern
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee696165(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 4892

        [PreserveSig]  int ScrollIntoView     ();
        //virtual HRESULT STDMETHODCALLTYPE ScrollIntoView( void) = 0;



    }
}