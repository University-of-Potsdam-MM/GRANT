using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{
    // https://msdn.microsoft.com/en-us/library/dd375576(v=vs.85).aspx

    [ComVisible(true), ComImport, Guid("146c3c17-f12e-4e22-8c27-f894b9b79c69"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationEventHandler
    {

        [PreserveSig]  int HandleAutomationEvent      (IUIAutomationElement sender, EVENTID eventId);
        //virtual HRESULT STDMETHODCALLTYPE HandleAutomationEvent( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *sender,
        //    /* [in] */ EVENTID eventId) = 0;
 

    }
}