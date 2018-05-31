using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("828055ad-355b-4435-86d5-3b51c14a9b1b"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationLegacyIAccessiblePattern
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ee696074(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 7249

        [PreserveSig]  int Select     (long flagsSelect);
        //virtual HRESULT STDMETHODCALLTYPE Select( 
        //    long flagsSelect) = 0;

        [PreserveSig]  int DoDefaultAction     ();
        //virtual HRESULT STDMETHODCALLTYPE DoDefaultAction( void) = 0;

        [PreserveSig]  int SetValue     (IntPtr szValue);
        //virtual HRESULT STDMETHODCALLTYPE SetValue( 
        //    __RPC__in LPCWSTR szValue) = 0;

        [PreserveSig]  int get_CurrentChildId     (out int pRetVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentChildId( 
        //    /* [retval][out] */ __RPC__out int *pRetVal) = 0;

        [PreserveSig]  int get_CurrentName     (out IntPtr pszName);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentName( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *pszName) = 0;

        [PreserveSig]  int get_CurrentValue     (out IntPtr pszValue);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentValue( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *pszValue) = 0;

        [PreserveSig]  int get_CurrentDescription     (out IntPtr pszDescription);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentDescription( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *pszDescription) = 0;

        [PreserveSig]  int get_CurrentRole     (out int pdwRole);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentRole( 
        //    /* [retval][out] */ __RPC__out DWORD *pdwRole) = 0;

        [PreserveSig]  int get_CurrentState     (out int pdwState);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentState( 
        //    /* [retval][out] */ __RPC__out DWORD *pdwState) = 0;

        [PreserveSig]  int get_CurrentHelp     (out IntPtr pszHelp);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentHelp( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *pszHelp) = 0;

        [PreserveSig]  int get_CurrentKeyboardShortcut     (out IntPtr pszKeyboardShortcut);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentKeyboardShortcut( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *pszKeyboardShortcut) = 0;

        [PreserveSig]  int GetCurrentSelection     (out IUIAutomationElementArray pvarSelectedChildren);
        //virtual HRESULT STDMETHODCALLTYPE GetCurrentSelection( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **pvarSelectedChildren) = 0;

        [PreserveSig]  int get_CurrentDefaultAction     (out IntPtr pszDefaultAction);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentDefaultAction( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *pszDefaultAction) = 0;

        


        [PreserveSig]  int get_CachedChildId     (out int pRetVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedChildId( 
        //    /* [retval][out] */ __RPC__out int *pRetVal) = 0;

        [PreserveSig]  int get_CachedName     (out IntPtr pszName);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedName( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *pszName) = 0;

        [PreserveSig]  int get_CachedValue     (out IntPtr pszValue);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedValue( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *pszValue) = 0;

        [PreserveSig]  int get_CachedDescription     (out IntPtr pszDescription);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedDescription( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *pszDescription) = 0;

        [PreserveSig]  int get_CachedRole     (out int pdwRole);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedRole( 
        //    /* [retval][out] */ __RPC__out DWORD *pdwRole) = 0;

        [PreserveSig]  int get_CachedState     (out int pdwState);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedState( 
        //    /* [retval][out] */ __RPC__out DWORD *pdwState) = 0;

        [PreserveSig]  int get_CachedHelp     (out IntPtr pszHelp);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedHelp( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *pszHelp) = 0;

        [PreserveSig]  int get_CachedKeyboardShortcut     (out IntPtr pszKeyboardShortcut);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedKeyboardShortcut( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *pszKeyboardShortcut) = 0;

        [PreserveSig]  int GetCachedSelection     (out IUIAutomationElementArray pvarSelectedChildren);
        //virtual HRESULT STDMETHODCALLTYPE GetCachedSelection( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **pvarSelectedChildren) = 0;

        [PreserveSig]  int get_CachedDefaultAction     (out IntPtr pszDefaultAction);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedDefaultAction( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *pszDefaultAction) = 0;

        [PreserveSig]  int GetIAccessible     (out IntPtr ppAccessible);
        //virtual HRESULT STDMETHODCALLTYPE GetIAccessible( 
        //    /* [retval][out] */ __RPC__deref_out_opt IAccessible **ppAccessible) = 0;

        

    }
}