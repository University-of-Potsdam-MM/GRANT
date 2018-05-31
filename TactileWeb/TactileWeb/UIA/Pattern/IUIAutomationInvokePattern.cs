using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("fb377fbe-8ea6-46d5-9c73-6499642d3059"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationInvokePattern
    {
        // https://msdn.microsoft.com/en-us/library/dd375399(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 3683

        [PreserveSig]  int Invoke     ();
        // virtual HRESULT STDMETHODCALLTYPE Invoke( void) = 0;

    }
}