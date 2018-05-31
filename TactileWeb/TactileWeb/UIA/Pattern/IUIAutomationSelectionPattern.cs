using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("5ed5202e-b2ac-47a6-b638-4b0bf140d78e"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationSelectionPattern
    {
        // https://msdn.microsoft.com/en-us/library/dd388264(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 4970

        [PreserveSig]  int GetCurrentSelection     (out IUIAutomationElementArray retVal);
         //    virtual HRESULT STDMETHODCALLTYPE GetCurrentSelection( 
        //        /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int get_CurrentCanSelectMultiple     (out bool retVal);
        //    virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentCanSelectMultiple( 
        //        /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CurrentIsSelectionRequired     (out bool retVal);
        //    virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsSelectionRequired( 
        //        /* [retval][out] */ __RPC__out BOOL *retVal) = 0;




        [PreserveSig]  int IUIAutomationElementArray     (out IUIAutomationElementArray retVal);
        //    virtual HRESULT STDMETHODCALLTYPE GetCachedSelection( 
        //        /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int get_CachedCanSelectMultiple     (out bool retVal);
        //    virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedCanSelectMultiple( 
        //        /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedIsSelectionRequired     (out bool retVal);
        //    virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsSelectionRequired( 
        //        /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

    }
}