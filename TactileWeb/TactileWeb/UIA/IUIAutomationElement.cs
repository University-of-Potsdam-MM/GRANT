using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{
    [ComVisible(true), ComImport, Guid("d22108aa-8ac5-49a5-837b-37bbb3d7591e"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomationElement
    {
        // https://msdn.microsoft.com/en-us/library/dd319271(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 1311


        [PreserveSig]  int SetFocus      ();
        // virtual HRESULT STDMETHODCALLTYPE SetFocus( void) = 0;


        [PreserveSig]  int GetRuntimeId  (out IntPtr runtimeId);
        //virtual HRESULT STDMETHODCALLTYPE GetRuntimeId( 
        //    /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *runtimeId) = 0;
        

        [PreserveSig]  int FindFirst      ();
        //virtual HRESULT STDMETHODCALLTYPE FindFirst( 
        //    /* [in] */ enum TreeScope scope,
        //    /* [in] */ __RPC__in_opt IUIAutomationCondition *condition,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **found) = 0;
        

        [PreserveSig]  int FindAll      ();
        //virtual HRESULT STDMETHODCALLTYPE FindAll( 
        //    /* [in] */ enum TreeScope scope,
        //    /* [in] */ __RPC__in_opt IUIAutomationCondition *condition,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **found) = 0;
        

        [PreserveSig]  int FindFirstBuildCache      ();
        //virtual HRESULT STDMETHODCALLTYPE FindFirstBuildCache( 
        //    /* [in] */ enum TreeScope scope,
        //    /* [in] */ __RPC__in_opt IUIAutomationCondition *condition,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **found) = 0;
        

        [PreserveSig]  int FindAllBuildCache      ();
        //virtual HRESULT STDMETHODCALLTYPE FindAllBuildCache( 
        //    /* [in] */ enum TreeScope scope,
        //    /* [in] */ __RPC__in_opt IUIAutomationCondition *condition,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **found) = 0;
        

        [PreserveSig]  int BuildUpdatedCache      ();
        //virtual HRESULT STDMETHODCALLTYPE BuildUpdatedCache( 
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **updatedElement) = 0;
        

        [PreserveSig]  int GetCurrentPropertyValue      ();
        //virtual HRESULT STDMETHODCALLTYPE GetCurrentPropertyValue( 
        //    /* [in] */ PROPERTYID propertyId,
        //    /* [retval][out] */ __RPC__out VARIANT *retVal) = 0;
        

        [PreserveSig]  int GetCurrentPropertyValueEx      ();
        //virtual HRESULT STDMETHODCALLTYPE GetCurrentPropertyValueEx( 
        //    /* [in] */ PROPERTYID propertyId,
        //    /* [in] */ BOOL ignoreDefaultValue,
        //    /* [retval][out] */ __RPC__out VARIANT *retVal) = 0;
        

        [PreserveSig]  int GetCachedPropertyValue      ();
        //virtual HRESULT STDMETHODCALLTYPE GetCachedPropertyValue( 
        //    /* [in] */ PROPERTYID propertyId,
        //    /* [retval][out] */ __RPC__out VARIANT *retVal) = 0;
        

        [PreserveSig]  int GetCachedPropertyValueEx      ();
        //virtual HRESULT STDMETHODCALLTYPE GetCachedPropertyValueEx( 
        //    /* [in] */ PROPERTYID propertyId,
        //    /* [in] */ BOOL ignoreDefaultValue,
        //    /* [retval][out] */ __RPC__out VARIANT *retVal) = 0;
        

        [PreserveSig]  int GetCurrentPatternAs      (PATTERNID patternId, ref Guid riid, out object patternObject);
        //virtual HRESULT STDMETHODCALLTYPE GetCurrentPatternAs( 
        //    /* [in] */ PATTERNID patternId,
        //    /* [in] */ __RPC__in REFIID riid,
        //    /* [retval][iid_is][out] */ __RPC__deref_out_opt void **patternObject) = 0;
        

        [PreserveSig]  int GetCachedPatternAs      ();
        //virtual HRESULT STDMETHODCALLTYPE GetCachedPatternAs( 
        //    /* [in] */ PATTERNID patternId,
        //    /* [in] */ __RPC__in REFIID riid,
        //    /* [retval][iid_is][out] */ __RPC__deref_out_opt void **patternObject) = 0;
        

        [PreserveSig]  int GetCurrentPattern      (PATTERNID patternId, out IntPtr patternObject);
        //virtual HRESULT STDMETHODCALLTYPE GetCurrentPattern( 
        //    /* [in] */ PATTERNID patternId,
        //    /* [retval][out] */ __RPC__deref_out_opt IUnknown **patternObject) = 0;
        

        [PreserveSig]  int GetCachedPattern      (PATTERNID patternId, out IntPtr patternObject);
        //virtual HRESULT STDMETHODCALLTYPE GetCachedPattern( 
        //    /* [in] */ PATTERNID patternId,
        //    /* [retval][out] */ __RPC__deref_out_opt IUnknown **patternObject) = 0;
        

        [PreserveSig]  int GetCachedParent      (out IUIAutomationElement parent);
        //virtual HRESULT STDMETHODCALLTYPE GetCachedParent( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **parent) = 0;
        

        [PreserveSig]  int GetCachedChildren      ();
        //virtual HRESULT STDMETHODCALLTYPE GetCachedChildren( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **children) = 0;
        

        [PreserveSig]  int get_CurrentProcessId      (out int retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentProcessId( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;
        

        [PreserveSig]  int get_CurrentControlType      (out ControlType retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentControlType( 
        //    /* [retval][out] */ __RPC__out CONTROLTYPEID *retVal) = 0;
        

        [PreserveSig]  int get_CurrentLocalizedControlType      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentLocalizedControlType( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;
        

        [PreserveSig]  int get_CurrentName      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentName( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;
        





        [PreserveSig]  int get_CurrentAcceleratorKey      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentAcceleratorKey( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;
        

        [PreserveSig]  int get_CurrentAccessKey      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentAccessKey( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;
        

        [PreserveSig]  int get_CurrentHasKeyboardFocus      (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentHasKeyboardFocus( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;
        

        [PreserveSig]  int get_CurrentIsKeyboardFocusable      (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsKeyboardFocusable( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;
        

        [PreserveSig]  int get_CurrentIsEnabled      (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsEnabled( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;
        

        [PreserveSig]  int get_CurrentAutomationId      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentAutomationId( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;
        

        [PreserveSig]  int get_CurrentClassName      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentClassName( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;
        

        [PreserveSig]  int get_CurrentHelpText      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentHelpText( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;
        

        [PreserveSig]  int get_CurrentCulture      (out int retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentCulture( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;
        

        [PreserveSig]  int get_CurrentIsControlElement      (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsControlElement( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;
        

        [PreserveSig]  int get_CurrentIsContentElement      (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsContentElement( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;
        

        [PreserveSig]  int get_CurrentIsPassword      (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsPassword( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;
        

        [PreserveSig]  int get_CurrentNativeWindowHandle      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentNativeWindowHandle( 
        //    /* [retval][out] */ __RPC__deref_out_opt UIA_HWND *retVal) = 0;
        

        [PreserveSig]  int get_CurrentItemType      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentItemType( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;
        

        [PreserveSig]  int get_CurrentIsOffscreen      (out bool retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsOffscreen( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;
        

        [PreserveSig]  int get_CurrentOrientation      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentOrientation( 
        //    /* [retval][out] */ __RPC__out enum OrientationType *retVal) = 0;
        

        [PreserveSig]  int get_CurrentFrameworkId      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentFrameworkId( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;
        

        [PreserveSig]  int get_CurrentIsRequiredForForm      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsRequiredForForm( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;
        

        [PreserveSig]  int get_CurrentItemStatus      (out IntPtr retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentItemStatus( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;
        

        [PreserveSig]  int get_CurrentBoundingRectangle      (out RECT retVal);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentBoundingRectangle( 
        //    /* [retval][out] */ __RPC__out RECT *retVal) = 0;
        



        [PreserveSig]  int get_CurrentLabeledBy      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentLabeledBy( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **retVal) = 0;

        [PreserveSig]  int get_CurrentAriaRole      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentAriaRole( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CurrentAriaProperties      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentAriaProperties( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CurrentIsDataValidForForm      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentIsDataValidForForm( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CurrentControllerFor      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentControllerFor( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int get_CurrentDescribedBy      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentDescribedBy( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int get_CurrentFlowsTo      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentFlowsTo( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int get_CurrentProviderDescription      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CurrentProviderDescription( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;






        [PreserveSig]  int get_CachedProcessId      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedProcessId( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;

        [PreserveSig]  int get_CachedControlType      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedControlType( 
        //    /* [retval][out] */ __RPC__out CONTROLTYPEID *retVal) = 0;

        [PreserveSig]  int get_CachedLocalizedControlType      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedLocalizedControlType( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedName      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedName( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedAcceleratorKey      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedAcceleratorKey( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedAccessKey      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedAccessKey( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedHasKeyboardFocus      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedHasKeyboardFocus( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedIsKeyboardFocusable      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsKeyboardFocusable( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedIsEnabled      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsEnabled( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedAutomationId      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedAutomationId( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedClassName      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedClassName( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedHelpText      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedHelpText( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedCulture      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedCulture( 
        //    /* [retval][out] */ __RPC__out int *retVal) = 0;

        [PreserveSig]  int get_CachedIsControlElement      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsControlElement( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedIsContentElement      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsContentElement( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedIsPassword      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsPassword( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedNativeWindowHandle      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedNativeWindowHandle( 
        //    /* [retval][out] */ __RPC__deref_out_opt UIA_HWND *retVal) = 0;

        [PreserveSig]  int get_CachedItemType      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedItemType( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedIsOffscreen      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsOffscreen( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedOrientation      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedOrientation( 
        //    /* [retval][out] */ __RPC__out enum OrientationType *retVal) = 0;

        [PreserveSig]  int get_CachedFrameworkId      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedFrameworkId( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedIsRequiredForForm      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsRequiredForForm( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedItemStatus      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedItemStatus( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedBoundingRectangle      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedBoundingRectangle( 
        //    /* [retval][out] */ __RPC__out RECT *retVal) = 0;

        [PreserveSig]  int get_CachedLabeledBy      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedLabeledBy( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **retVal) = 0;

        [PreserveSig]  int get_CachedAriaRole      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedAriaRole( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedAriaProperties      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedAriaProperties( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;

        [PreserveSig]  int get_CachedIsDataValidForForm      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedIsDataValidForForm( 
        //    /* [retval][out] */ __RPC__out BOOL *retVal) = 0;

        [PreserveSig]  int get_CachedControllerFor      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedControllerFor( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int get_CachedDescribedBy      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedDescribedBy( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int get_CachedFlowsTo      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedFlowsTo( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElementArray **retVal) = 0;

        [PreserveSig]  int get_CachedProviderDescription      ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_CachedProviderDescription( 
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *retVal) = 0;



        [PreserveSig]  int GetClickablePoint      (out System.Drawing.Point clickable, out bool gotClickable);
        //virtual HRESULT STDMETHODCALLTYPE GetClickablePoint( 
        //    /* [out] */ __RPC__out POINT *clickable,
        //    /* [retval][out] */ __RPC__out BOOL *gotClickable) = 0;





    }


}