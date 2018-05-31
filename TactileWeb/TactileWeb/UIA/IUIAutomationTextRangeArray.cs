using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("ce4ae76a-e717-4c98-81ea-47371d028eb6"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationTextRangeArray
    {
        // https://msdn.microsoft.com/en-us/library/dd388212(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 6667


        [PreserveSig]  int get_Length     (out int length);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Length( 
        //    /* [retval][out] */ __RPC__out int *length) = 0;

        [PreserveSig]  int GetElement     (int index, out IUIAutomationTextRange element);
        //virtual HRESULT STDMETHODCALLTYPE GetElement( 
        //    /* [in] */ int index,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTextRange **element) = 0;


    }
}