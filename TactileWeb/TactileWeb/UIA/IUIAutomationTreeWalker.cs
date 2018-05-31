using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("4042c624-389c-4afc-a630-9df854a541fc"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationTreeWalker
    {
        // https://msdn.microsoft.com/en-us/library/dd375281(v=vs.85).aspx
        // https://msdn.microsoft.com/en-us/library/dd319581(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 3038


        [PreserveSig]  int GetParentElement     (IUIAutomationElement element, out IUIAutomationElement parent);
        //virtual HRESULT STDMETHODCALLTYPE GetParentElement( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **parent) = 0;
        

        [PreserveSig]  int GetFirstChildElement     (IUIAutomationElement element, out IUIAutomationElement first);
        //virtual HRESULT STDMETHODCALLTYPE GetFirstChildElement( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **first) = 0;
        

        [PreserveSig]  int GetLastChildElement     (IUIAutomationElement element, out IUIAutomationElement last);
        //virtual HRESULT STDMETHODCALLTYPE GetLastChildElement( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **last) = 0;
        

        [PreserveSig]  int GetNextSiblingElement     (IUIAutomationElement element, out IUIAutomationElement next);
        //virtual HRESULT STDMETHODCALLTYPE GetNextSiblingElement( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **next) = 0;
        

        [PreserveSig]  int GetPreviousSiblingElement     (IUIAutomationElement element, out IUIAutomationElement previous);
        //virtual HRESULT STDMETHODCALLTYPE GetPreviousSiblingElement( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **previous) = 0;
        

        [PreserveSig]  int NormalizeElement     (IUIAutomationElement element, out IUIAutomationElement normalized);
        //virtual HRESULT STDMETHODCALLTYPE NormalizeElement( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **normalized) = 0;
        





        [PreserveSig]  int GetParentElementBuildCache     (IUIAutomationElement element);
        //virtual HRESULT STDMETHODCALLTYPE GetParentElementBuildCache( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **parent) = 0;
        

        [PreserveSig]  int GetFirstChildElementBuildCache     (IUIAutomationElement element);
        //virtual HRESULT STDMETHODCALLTYPE GetFirstChildElementBuildCache( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **first) = 0;
        

        [PreserveSig]  int GetLastChildElementBuildCache     (IUIAutomationElement element);
        //virtual HRESULT STDMETHODCALLTYPE GetLastChildElementBuildCache( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **last) = 0;
        

        [PreserveSig]  int GetNextSiblingElementBuildCache     (IUIAutomationElement element);
        //virtual HRESULT STDMETHODCALLTYPE GetNextSiblingElementBuildCache( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **next) = 0;
        

        [PreserveSig]  int GetPreviousSiblingElementBuildCache     (IUIAutomationElement element);
        //virtual HRESULT STDMETHODCALLTYPE GetPreviousSiblingElementBuildCache( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **previous) = 0;
        

        [PreserveSig]  int NormalizeElementBuildCache     (IUIAutomationElement element);
        //virtual HRESULT STDMETHODCALLTYPE NormalizeElementBuildCache( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **normalized) = 0;
        

        [PreserveSig]  int get_Condition     ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_Condition( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **condition) = 0;


    }
}