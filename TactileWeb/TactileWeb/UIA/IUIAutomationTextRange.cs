using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("a543cc6a-f4ae-494b-8239-c814481187a8"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationTextRange
    {
        // https://msdn.microsoft.com/en-us/library/dd388227(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 6164


        [PreserveSig]  int Clone     (out IUIAutomationTextRange clonedRange);
        //virtual HRESULT STDMETHODCALLTYPE Clone( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTextRange **clonedRange) = 0;
        

        [PreserveSig]  int Compare     (IUIAutomationTextRange range, out bool areSame);
        //virtual HRESULT STDMETHODCALLTYPE Compare( 
        //    /* [in] */ __RPC__in_opt IUIAutomationTextRange *range,
        //    /* [retval][out] */ __RPC__out BOOL *areSame) = 0;
        

        [PreserveSig]  int CompareEndpoints     (TextPatternRangeEndpoint srcEndPoint, IUIAutomationTextRange range, TextPatternRangeEndpoint targetEndPoint, out int compValue);
        //virtual HRESULT STDMETHODCALLTYPE CompareEndpoints( 
        //    /* [in] */ enum TextPatternRangeEndpoint srcEndPoint,
        //    /* [in] */ __RPC__in_opt IUIAutomationTextRange *range,
        //    /* [in] */ enum TextPatternRangeEndpoint targetEndPoint,
        //    /* [retval][out] */ __RPC__out int *compValue) = 0;
        

        [PreserveSig]  int ExpandToEnclosingUnit     (TextUnit textUnit);
        //virtual HRESULT STDMETHODCALLTYPE ExpandToEnclosingUnit( 
        //    /* [in] */ enum TextUnit textUnit) = 0;
        

        [PreserveSig]  int FindAttribute     (TEXTATTRIBUTEID attr, IntPtr val, bool backward, out IUIAutomationTextRange found);
        //virtual HRESULT STDMETHODCALLTYPE FindAttribute( 
        //    /* [in] */ TEXTATTRIBUTEID attr,
        //    /* [in] */ VARIANT val,
        //    /* [in] */ BOOL backward,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTextRange **found) = 0;
        

        [PreserveSig]  int FindText     (IntPtr text, bool backward, bool ignoreCase, out IUIAutomationTextRange found);
        //virtual HRESULT STDMETHODCALLTYPE FindText( 
        //    /* [in] */ __RPC__in BSTR text,
        //    /* [in] */ BOOL backward,
        //    /* [in] */ BOOL ignoreCase,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTextRange **found) = 0;
        

        [PreserveSig]  int GetAttributeValue     (TEXTATTRIBUTEID attr, out IntPtr value);
        //virtual HRESULT STDMETHODCALLTYPE GetAttributeValue( 
        //    /* [in] */ TEXTATTRIBUTEID attr,
        //    /* [retval][out] */ __RPC__out VARIANT *value) = 0;
        

        [PreserveSig]  int GetBoundingRectangles     (out IntPtr boundingRects);
        //virtual HRESULT STDMETHODCALLTYPE GetBoundingRectangles( 
        //    /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *boundingRects) = 0;
        

        [PreserveSig]  int GetEnclosingElement     (out IUIAutomationElement enclosingElement);
        //virtual HRESULT STDMETHODCALLTYPE GetEnclosingElement( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **enclosingElement) = 0;
        

        [PreserveSig]  int GetText     (int maxLength, out IntPtr text);
        //virtual HRESULT STDMETHODCALLTYPE GetText( 
        //    /* [in] */ int maxLength,
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *text) = 0;
        

        [PreserveSig]  int Move     (TextUnit unit, int count, out int moved);
        //virtual HRESULT STDMETHODCALLTYPE Move( 
        //    /* [in] */ enum TextUnit unit,
        //    /* [in] */ int count,
        //    /* [retval][out] */ __RPC__out int *moved) = 0;
        

        [PreserveSig]  int MoveEndpointByUnit     (TextPatternRangeEndpoint endpoint, TextUnit unit, int count, out int moved);
        //virtual HRESULT STDMETHODCALLTYPE MoveEndpointByUnit( 
        //    /* [in] */ enum TextPatternRangeEndpoint endpoint,
        //    /* [in] */ enum TextUnit unit,
        //    /* [in] */ int count,
        //    /* [retval][out] */ __RPC__out int *moved) = 0;
        

        [PreserveSig]  int MoveEndpointByRange     (TextPatternRangeEndpoint srcEndPoint, IUIAutomationTextRange range, TextPatternRangeEndpoint targetEndPoint);
        //virtual HRESULT STDMETHODCALLTYPE MoveEndpointByRange( 
        //    /* [in] */ enum TextPatternRangeEndpoint srcEndPoint,
        //    /* [in] */ __RPC__in_opt IUIAutomationTextRange *range,
        //    /* [in] */ enum TextPatternRangeEndpoint targetEndPoint) = 0;
        

        [PreserveSig]  int Select     ();
        //virtual HRESULT STDMETHODCALLTYPE Select( void) = 0;
        

        [PreserveSig]  int AddToSelection     ();
        //virtual HRESULT STDMETHODCALLTYPE AddToSelection( void) = 0;
        

        [PreserveSig]  int RemoveFromSelection     ();
        //virtual HRESULT STDMETHODCALLTYPE RemoveFromSelection( void) = 0;
        

        [PreserveSig]  int ScrollIntoView     (bool alignToTop);
        //virtual HRESULT STDMETHODCALLTYPE ScrollIntoView( 
        //    /* [in] */ BOOL alignToTop) = 0;
        

        [PreserveSig]  int GetChildren     (out IUIAutomationElementArray children);
        //virtual HRESULT STDMETHODCALLTYPE GetChildren( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **children) = 0;





    }
}