using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("a94cd8b1-0844-4cd6-9d2d-640537ab39e9"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationValuePattern
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee671484(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 5824

        [PreserveSig]  int SetValue     (IntPtr val);
        //virtual HRESULT STDMETHODCALLTYPE SetValue( 
        //    /* [in] */ __RPC__in BSTR val) = 0;

        [PreserveSig]  int get_CurrentValue     (out IntPtr retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentValue( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CurrentIsReadOnly     (out bool retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsReadOnly( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;



        [PreserveSig]  int get_CachedValue     (out IntPtr retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedValue( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedIsReadOnly     (out bool retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsReadOnly( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;
        


    }
}