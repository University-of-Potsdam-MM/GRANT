using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("c270f6b5-5c69-4290-9745-7a7f97169468"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationFocusChangedEventHandler
    {

        [PreserveSig]  int HandleFocusChangedEvent      (IUIAutomationElement sender);
        //virtual HRESULT STDMETHODCALLTYPE HandleFocusChangedEvent( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *sender) = 0;

    }

}