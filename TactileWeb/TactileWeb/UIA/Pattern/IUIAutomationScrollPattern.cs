using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("88f4d42a-e881-459d-a77c-73bbbb7e02dc"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationScrollPattern
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee696167(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 4678

        [PreserveSig]  int Scroll     (ScrollAmount horizontalAmount, ScrollAmount verticalAmount);
        //virtual HRESULT STDMETHODCALLTYPE Scroll( 
        //    /* [in] */ enum ScrollAmount horizontalAmount,
        //    /* [in] */ enum ScrollAmount verticalAmount) = 0;

        [PreserveSig]  int SetScrollPercent     (double horizontalPercent, double verticalPercent);
        //virtual HRESULT STDMETHODCALLTYPE SetScrollPercent( 
        //    /* [in] */ double horizontalPercent,
        //    /* [in] */ double verticalPercent) = 0;
        
        [PreserveSig]  int get_CurrentHorizontalScrollPercent     (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentHorizontalScrollPercent( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CurrentVerticalScrollPercent     (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentVerticalScrollPercent( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;
        
        [PreserveSig]  int get_CurrentHorizontalViewSize     (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentHorizontalViewSize( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CurrentVerticalViewSize     (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentVerticalViewSize( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CurrentHorizontallyScrollable     (out bool retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentHorizontallyScrollable( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CurrentVerticallyScrollable     (out bool retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentVerticallyScrollable( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;
        


        [PreserveSig]  int get_CachedHorizontalScrollPercent     (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedHorizontalScrollPercent( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CachedVerticalScrollPercent     (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedVerticalScrollPercent( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CachedHorizontalViewSize     (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedHorizontalViewSize( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CachedVerticalViewSize     (out double retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedVerticalViewSize( 
        //    /* [retval][out] */ __RPC__out double *retVal) = 0;

        [PreserveSig]  int get_CachedHorizontallyScrollable     (out bool retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedHorizontallyScrollable( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedVerticallyScrollable     (out bool retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedVerticallyScrollable( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;




    }
}