using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("94cf8058-9b8d-4ab9-8bfd-4cd0a33c8c70"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationTogglePattern
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee671456(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 5562

        [PreserveSig]  int Toggle     ();
        //virtual HRESULT STDMETHODCALLTYPE Toggle( void) = 0;

        [PreserveSig]  int get_CurrentToggleState     (out ToggleState retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentToggleState( 
        //    /* [retval][out] */ __RPC__out enum ToggleState *retVal) = 0;

        [PreserveSig]  int get_CachedToggleState     (out ToggleState retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedToggleState( 
        //    /* [retval][out] */ __RPC__out enum ToggleState *retVal) = 0;
        

    }
}