using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{
    [ComVisible(true), ComImport, Guid("b32a92b5-bc25-4078-9c08-d7ee95c48e03"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationCacheRequest
    {
        // UIAutomationClient.h  Zeile 2878


        [PreserveSig]  int AddProperty      ();
        //virtual HRESULT STDMETHODCALLTYPE AddProperty( 
        //    /* [in] */ PROPERTYID propertyId) = 0;
        

        [PreserveSig]  int AddPattern      ();
        //virtual HRESULT STDMETHODCALLTYPE AddPattern( 
        //    /* [in] */ PATTERNID patternId) = 0;
        

        [PreserveSig]  int Clone      ();
        //virtual HRESULT STDMETHODCALLTYPE Clone( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCacheRequest **clonedRequest) = 0;
        

        [PreserveSig]  int get_TreeScope      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_TreeScope( 
        //    /* [retval][out] */ __RPC__out enum TreeScope *scope) = 0;
        

        [PreserveSig]  int put_TreeScope      ();
        //virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_TreeScope( 
        //    /* [in] */ enum TreeScope scope) = 0;
        

        [PreserveSig]  int get_TreeFilter      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_TreeFilter( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **filter) = 0;
        

        [PreserveSig]  int put_TreeFilter      ();
        //virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_TreeFilter( 
        //    /* [in] */ __RPC__in_opt IUIAutomationCondition *filter) = 0;
        

        [PreserveSig]  int get_AutomationElementMode      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_AutomationElementMode( 
        //    /* [retval][out] */ __RPC__out enum AutomationElementMode *mode) = 0;
        

        [PreserveSig]  int put_AutomationElementMode      ();
        //virtual /* [propput] */ HRESULT STDMETHODCALLTYPE put_AutomationElementMode( 
        //    /* [in] */ enum AutomationElementMode mode) = 0;
        




    }




}