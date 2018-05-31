using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("78f8ef57-66c3-4e09-bd7c-e79b2004894d"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationGridItemPattern
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee696053(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 4096

        [PreserveSig]  int get_CurrentContainingGrid     (out IUIAutomationElement retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentContainingGrid( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **retVal) = 0;


        [PreserveSig]  int get_CurrentRow     (out int retval);
        //[PreserveSig]  int GetItem     (out int retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentRow( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;


        [PreserveSig]  int get_CurrentColumn     (out int retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentColumn( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;


        [PreserveSig]  int get_CurrentRowSpan     (out int retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentRowSpan( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;

        [PreserveSig]  int get_CurrentColumnSpan     (out int retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentColumnSpan( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;


        [PreserveSig]  int get_CachedContainingGrid     (out IUIAutomationElement retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedContainingGrid( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **retVal) = 0;

        [PreserveSig]  int get_CachedRow     (out int retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedRow( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;


        [PreserveSig]  int get_CachedColumn     (out int retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedColumn( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;


        [PreserveSig]  int get_CachedRowSpan     (out int retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedRowSpan( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;


        [PreserveSig]  int get_CachedColumnSpan     (out int retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedColumnSpan( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;

    }
}