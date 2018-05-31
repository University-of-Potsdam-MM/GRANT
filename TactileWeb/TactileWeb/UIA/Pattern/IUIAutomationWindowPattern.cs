using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("0faef453-9208-43ef-bbb2-3b485177864f"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationWindowPattern
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee671492(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 5944

        [PreserveSig]  int Close     ();
        //virtual HRESULT STDMETHODCALLTYPE Close( void) = 0;

        [PreserveSig]  int WaitForInputIdle     (int milliseconds, out bool success);
        //virtual HRESULT STDMETHODCALLTYPE WaitForInputIdle( 
        //    /* [in] */ int milliseconds,
        //    /* [retval][out] */ __RPC__out BOOL *success) = 0;

        [PreserveSig]  int SetWindowVisualState     (WindowVisualState state);
        //virtual HRESULT STDMETHODCALLTYPE SetWindowVisualState( 
        //    /* [in] */ enum WindowVisualState state) = 0;

        [PreserveSig]  int get_CurrentCanMaximize     (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentCanMaximize( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CurrentCanMinimize     (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentCanMinimize( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CurrentIsModal     (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsModal( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CurrentIsTopmost     (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsTopmost( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CurrentWindowVisualState     (out WindowVisualState retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentWindowVisualState( 
        //    /* [retval][out] */ __RPC__out enum WindowVisualState *retVal) = 0;

        [PreserveSig]  int get_CurrentWindowInteractionState     (out WindowInteractionState retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentWindowInteractionState( 
        //    /* [retval][out] */ __RPC__out enum WindowInteractionState *retVal) = 0;





        [PreserveSig]  int get_CachedCanMaximize     (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedCanMaximize( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedCanMinimize     (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedCanMinimize( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedIsModal     (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsModal( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedIsTopmost     (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsTopmost( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedWindowVisualState     (out WindowVisualState retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedWindowVisualState( 
        //    /* [retval][out] */ __RPC__out enum WindowVisualState *retVal) = 0;

        [PreserveSig]  int get_CachedWindowInteractionState     (out WindowInteractionState retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedWindowInteractionState( 
        //    /* [retval][out] */ __RPC__out enum WindowInteractionState *retVal) = 0;

        

    }
}