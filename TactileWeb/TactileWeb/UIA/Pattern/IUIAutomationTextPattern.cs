using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("32eba289-3583-42c9-9c59-3b6d9a1e9b6a"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationTextPattern
    {
        // https://msdn.microsoft.com/en-us/library/dd388237(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 6759

        [PreserveSig]  int RangeFromPoint     (System.Drawing.Point pt, out IUIAutomationTextRange range);
        //virtual HRESULT STDMETHODCALLTYPE RangeFromPoint( 
        //    /* [in] */ POINT pt,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTextRange **range) = 0;

        [PreserveSig]  int RangeFromChild     (IUIAutomationElement child, out IUIAutomationTextRange range);
        //virtual HRESULT STDMETHODCALLTYPE RangeFromChild( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *child,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTextRange **range) = 0;

        [PreserveSig]  int GetSelection     (out IUIAutomationTextRangeArray ranges);
        //virtual HRESULT STDMETHODCALLTYPE GetSelection( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTextRangeArray **ranges) = 0;

        [PreserveSig]  int GetVisibleRanges     (out IUIAutomationTextRangeArray ranges);
        //virtual HRESULT STDMETHODCALLTYPE GetVisibleRanges( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTextRangeArray **ranges) = 0;

        [PreserveSig]  int get_DocumentRange     (out IUIAutomationTextRange range);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_DocumentRange( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTextRange **range) = 0;

        [PreserveSig]  int get_SupportedTextSelection     (out SupportedTextSelection supportedTextSelection);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_SupportedTextSelection( 
        //    /* [retval][out] */ __RPC__out enum SupportedTextSelection *supportedTextSelection) = 0;


    }
}