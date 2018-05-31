using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("506a921a-fcc9-409f-b23b-37eb74106872"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationTextPattern2
    {

        // IUIAutomationTextPattern
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




        // IUIAutomationTextPattern2
        // https://msdn.microsoft.com/en-us/library/windows/desktop/hh437299(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 6893

        [PreserveSig]  int RangeFromAnnotation      (IUIAutomationElement annotation, out IUIAutomationTextRange range);
        //virtual HRESULT STDMETHODCALLTYPE RangeFromAnnotation( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *annotation,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTextRange **range) = 0;

        [PreserveSig]  int GetCaretRange            (out bool isActive, out IUIAutomationTextRange range);
        //virtual HRESULT STDMETHODCALLTYPE GetCaretRange( 
        //    /* [out] */ __RPC__out BOOL *isActive,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTextRange **range) = 0; 


    }
}