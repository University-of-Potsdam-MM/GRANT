using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("59213f4f-7346-49e5-b120-80555987a148"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface  IUIAutomationRangeValuePattern
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee696207(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 4478

        [PreserveSig]  int SetValue     (double val);
        //virtual HRESULT STDMETHODCALLTYPE SetValue( 
        //    /* [in] */ double val) = 0;

        [PreserveSig]  int get_CurrentValue         (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentValue( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CurrentIsReadOnly    (out bool retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsReadOnly( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CurrentMaximum       (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentMaximum( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CurrentMinimum       (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentMinimum( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CurrentLargeChange   (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentLargeChange( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CurrentSmallChange   (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentSmallChange( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;




        [PreserveSig]  int get_CachedValue         (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedValue( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CachedIsReadOnly    (out bool retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsReadOnly( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedMaximum       (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedMaximum( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CachedMinimum       (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedMinimum( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CachedLargeChange   (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedLargeChange( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CachedSmallChange   (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedSmallChange( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;


    }
}