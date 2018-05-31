using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("620e691c-ea96-4710-a850-754b24ce2417"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationTablePattern
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee696207(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 5322

        [PreserveSig]  int GetCurrentRowHeaders     (out IUIAutomationElementArray retval);
        //virtual HRESULT STDMETHODCALLTYPE GetCurrentRowHeaders( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int GetCurrentColumnHeaders     (out IUIAutomationElementArray retval);
        //virtual HRESULT STDMETHODCALLTYPE GetCurrentColumnHeaders( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int get_CurrentRowOrColumnMajor     (out RowOrColumnMajor retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentRowOrColumnMajor( 
        //    /* [retval][out] */ __RPC__out enum RowOrColumnMajor *retVal) = 0;



        [PreserveSig]  int GetCachedRowHeaders     (out IUIAutomationElementArray retval);
        //virtual HRESULT STDMETHODCALLTYPE GetCachedRowHeaders( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int GetCachedColumnHeaders     (out IUIAutomationElementArray retval);
        //virtual HRESULT STDMETHODCALLTYPE GetCachedColumnHeaders( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int get_CachedRowOrColumnMajor     (out RowOrColumnMajor retval);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedRowOrColumnMajor( 
        //    /* [retval][out] */ __RPC__out enum RowOrColumnMajor *retVal) = 0;


    }
}