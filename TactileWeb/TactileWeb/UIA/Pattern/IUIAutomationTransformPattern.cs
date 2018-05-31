using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("a9b55844-a55d-4ef0-926d-569c16ff89bb"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationTransformPattern
    {
        // https://msdn.microsoft.com/en-us/library/dd388203(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 5660

         [PreserveSig]  int Move     (double x, double y);
         //virtual HRESULT STDMETHODCALLTYPE Move( 
         //    /* [in] */ double x,
         //    /* [in] */ double y) = 0;

         [PreserveSig]  int Resize     (double width, double height);
         //virtual HRESULT STDMETHODCALLTYPE Resize( 
         //    /* [in] */ double width,
         //    /* [in] */ double height) = 0;

         [PreserveSig]  int Rotate     (double degrees);
         //virtual HRESULT STDMETHODCALLTYPE Rotate( 
         //    /* [in] */ double degrees) = 0;



         [PreserveSig]  int get_CurrentCanMove     (out bool retVal);
         //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentCanMove( 
         //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

         [PreserveSig]  int get_CurrentCanResize     (out bool retVal);
         //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentCanResize( 
         //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

         [PreserveSig]  int get_CurrentCanRotate     (out bool retVal);
         //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentCanRotate( 
         //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

         [PreserveSig]  int get_CurrentContainingGrid     (out bool retVal);
         //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedCanMove( 
         //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;



         [PreserveSig]  int get_CachedCanResize     (out bool retVal);
         //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedCanResize( 
         //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

         [PreserveSig]  int get_CachedCanRotate     (out bool retVal);
         //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedCanRotate( 
         //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;


    }
}