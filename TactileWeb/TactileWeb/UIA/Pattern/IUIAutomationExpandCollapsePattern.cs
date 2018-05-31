using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("619be086-1f4e-4ee4-bafa-210128738730"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationExpandCollapsePattern
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee696046(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 3866

        [PreserveSig]  int Expand     ();
        //virtual HRESULT STDMETHODCALLTYPE Expand( void) = 0;
        
        [PreserveSig]  int Collapse   ();
        //virtual HRESULT STDMETHODCALLTYPE Collapse( void) = 0;
        
        [PreserveSig]  int get_CurrentExpandCollapseState   (out ExpandCollapseState retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentExpandCollapseState( 
        //    /* [retval][out] */ __RPC__out enum ExpandCollapseState *retVal) = 0;

        [PreserveSig]  int get_CachedExpandCollapseState    (out ExpandCollapseState retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedExpandCollapseState( 
        //    /* [retval][out] */ __RPC__out enum ExpandCollapseState *retVal) = 0;
        

    }
}