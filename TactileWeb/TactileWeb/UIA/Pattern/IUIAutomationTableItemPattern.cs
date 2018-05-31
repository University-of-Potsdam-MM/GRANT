using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("0b964eb3-ef2e-4464-9c79-61d61737a27e"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationTableItemPattern
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee696202(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 5452

        [PreserveSig]  int GetCurrentRowHeaderItems    (out IUIAutomationElementArray retval);
        //virtual HRESULT STDMETHODCALLTYPE GetCurrentRowHeaderItems( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int GetCurrentColumnHeaderItems (out IUIAutomationElementArray retval);
        //virtual HRESULT STDMETHODCALLTYPE GetCurrentColumnHeaderItems( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;


        [PreserveSig]  int GetCachedRowHeaderItems     (out IUIAutomationElementArray retval);
        //virtual HRESULT STDMETHODCALLTYPE GetCachedRowHeaderItems( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int GetCachedColumnHeaderItems  (out IUIAutomationElementArray retval);
        //virtual HRESULT STDMETHODCALLTYPE GetCachedColumnHeaderItems( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;
        

    }
}