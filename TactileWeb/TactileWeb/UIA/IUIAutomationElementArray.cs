using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{
    [ComVisible(true), ComImport, Guid("14314595-b4bc-4055-95f2-58f2e42c9855"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationElementArray
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee671426(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 2247


        [PreserveSig]  int get_Length      (out int length);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Length( 
        //    /* [retval][out] */ __RPC__out int *length) = 0;
        
        [PreserveSig]  int GetElement      (int index, out IUIAutomationElement element);
        //virtual HRESULT STDMETHODCALLTYPE GetElement( 
        //    /* [in] */ int index,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **element) = 0;

    }
}