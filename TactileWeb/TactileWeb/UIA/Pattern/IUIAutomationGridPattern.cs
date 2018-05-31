using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("414c3cdc-856b-4f5b-8538-3131c6302550"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationGridPattern
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee696064(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 3972

        [PreserveSig]  int GetItem     (int row, int column, out IUIAutomationElement element);
        //virtual HRESULT STDMETHODCALLTYPE GetItem( 
        //    /* [in] */ int row,
        //    /* [in] */ int column,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **element) = 0;



        [PreserveSig]  int get_CurrentRowCount    (out int retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentRowCount( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;

        [PreserveSig]  int get_CurrentColumnCount (out int retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentColumnCount( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;
        



        [PreserveSig]  int get_CachedRowCount     (out int retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedRowCount( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;

        [PreserveSig]  int get_CachedColumnCount  (out int retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedColumnCount( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;


    }
}