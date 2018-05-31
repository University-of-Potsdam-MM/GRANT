using System;
using System.Runtime.InteropServices;


namespace TactileWeb.UIA
{

    [ComVisible(true), ComImport, Guid("30cbe57d-d9d0-452a-ab13-7ac5ac4825ee"), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IUIAutomation
    {
        // https://msdn.microsoft.com/en-us/library/dd319456(v=vs.85).aspx
        // UIAutomationClient.h  Zeile 11688

        [PreserveSig]  int CompareElements      (IntPtr el1, IntPtr el2, out bool areSame);
        [PreserveSig]  int CompareRuntimeIds    (IntPtr runtimeId1, IntPtr runtimeId2, out bool areSame);
        [PreserveSig]  int GetRootElement       (out IUIAutomationElement root);
        [PreserveSig]  int ElementFromHandle    (IntPtr hwnd, IntPtr el2, out IntPtr element);


        [PreserveSig]  int ElementFromPoint     (System.Drawing.Point pt, out IUIAutomationElement element);
        //virtual HRESULT STDMETHODCALLTYPE ElementFromPoint( 
        //    /* [in] */ POINT pt,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **element) = 0;

        [PreserveSig]  int GetFocusedElement     (out IUIAutomationElement element);
        //virtual HRESULT STDMETHODCALLTYPE GetFocusedElement( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **element) = 0;

        [PreserveSig]  int GetRootElementBuildCache     ();
        //virtual HRESULT STDMETHODCALLTYPE GetRootElementBuildCache( 
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **root) = 0;
        

        [PreserveSig]  int ElementFromHandleBuildCache     ();
        //virtual HRESULT STDMETHODCALLTYPE ElementFromHandleBuildCache( 
        //    /* [in] */ __RPC__in UIA_HWND hwnd,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **element) = 0;
        

        [PreserveSig]  int ElementFromPointBuildCache     ();
        //virtual HRESULT STDMETHODCALLTYPE ElementFromPointBuildCache( 
        //    /* [in] */ POINT pt,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **element) = 0;
        

        [PreserveSig]  int GetFocusedElementBuildCache     ();
        //virtual HRESULT STDMETHODCALLTYPE GetFocusedElementBuildCache( 
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **element) = 0;
        

        [PreserveSig]  int CreateTreeWalker     ();
        //virtual HRESULT STDMETHODCALLTYPE CreateTreeWalker( 
        //    /* [in] */ __RPC__in_opt IUIAutomationCondition *pCondition,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTreeWalker **walker) = 0;
        

        [PreserveSig]  int get_ControlViewWalker     (out IUIAutomationTreeWalker walker);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ControlViewWalker( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTreeWalker **walker) = 0;
        

        [PreserveSig]  int get_ContentViewWalker     (out IUIAutomationTreeWalker walker);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ContentViewWalker( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTreeWalker **walker) = 0;
        

        [PreserveSig]  int get_RawViewWalker     (out IUIAutomationTreeWalker walker);
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_RawViewWalker( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationTreeWalker **walker) = 0;
        

        [PreserveSig]  int get_RawViewCondition     ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_RawViewCondition( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **condition) = 0;
        

        [PreserveSig]  int get_ControlViewCondition     ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ControlViewCondition( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **condition) = 0;
        

        [PreserveSig]  int get_ContentViewCondition     ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ContentViewCondition( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **condition) = 0;
        

        [PreserveSig]  int CreateCacheRequest     ();
        //virtual HRESULT STDMETHODCALLTYPE CreateCacheRequest( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCacheRequest **cacheRequest) = 0;
        

        [PreserveSig]  int CreateTrueCondition     ();
        //virtual HRESULT STDMETHODCALLTYPE CreateTrueCondition( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **newCondition) = 0;
        

        [PreserveSig]  int CreateFalseCondition     ();
        //virtual HRESULT STDMETHODCALLTYPE CreateFalseCondition( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **newCondition) = 0;
        

        [PreserveSig]  int CreatePropertyCondition     ();
        //virtual HRESULT STDMETHODCALLTYPE CreatePropertyCondition( 
        //    /* [in] */ PROPERTYID propertyId,
        //    /* [in] */ VARIANT value,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **newCondition) = 0;
        

        [PreserveSig]  int CreatePropertyConditionEx     ();
        //virtual HRESULT STDMETHODCALLTYPE CreatePropertyConditionEx( 
        //    /* [in] */ PROPERTYID propertyId,
        //    /* [in] */ VARIANT value,
        //    /* [in] */ enum PropertyConditionFlags flags,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **newCondition) = 0;
        

        [PreserveSig]  int CreateAndCondition     ();
        //virtual HRESULT STDMETHODCALLTYPE CreateAndCondition( 
        //    /* [in] */ __RPC__in_opt IUIAutomationCondition *condition1,
        //    /* [in] */ __RPC__in_opt IUIAutomationCondition *condition2,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **newCondition) = 0;
        

        [PreserveSig]  int CreateAndConditionFromArray     ();
        //virtual HRESULT STDMETHODCALLTYPE CreateAndConditionFromArray( 
        //    /* [in] */ __RPC__in_opt SAFEARRAY * conditions,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **newCondition) = 0;
        

        [PreserveSig]  int CreateAndConditionFromNativeArray     ();
        //virtual HRESULT STDMETHODCALLTYPE CreateAndConditionFromNativeArray( 
        //    /* [size_is][in] */ __RPC__in_ecount_full(conditionCount) IUIAutomationCondition **conditions,
        //    /* [in] */ int conditionCount,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **newCondition) = 0;
        

        [PreserveSig]  int CreateOrCondition     ();
        //virtual HRESULT STDMETHODCALLTYPE CreateOrCondition( 
        //    /* [in] */ __RPC__in_opt IUIAutomationCondition *condition1,
        //    /* [in] */ __RPC__in_opt IUIAutomationCondition *condition2,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **newCondition) = 0;
        

        [PreserveSig]  int CreateOrConditionFromArray     ();
        //virtual HRESULT STDMETHODCALLTYPE CreateOrConditionFromArray( 
        //    /* [in] */ __RPC__in_opt SAFEARRAY * conditions,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **newCondition) = 0;
        

        [PreserveSig]  int CreateOrConditionFromNativeArray     ();
        //virtual HRESULT STDMETHODCALLTYPE CreateOrConditionFromNativeArray( 
        //    /* [size_is][in] */ __RPC__in_ecount_full(conditionCount) IUIAutomationCondition **conditions,
        //    /* [in] */ int conditionCount,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **newCondition) = 0;
        

        [PreserveSig]  int CreateNotCondition     ();
        //virtual HRESULT STDMETHODCALLTYPE CreateNotCondition( 
        //    /* [in] */ __RPC__in_opt IUIAutomationCondition *condition,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationCondition **newCondition) = 0;
        

        [PreserveSig]  int AddAutomationEventHandler     (EVENTID eventId, IUIAutomationElement element, TreeScope scope, IntPtr cacheRequest, IUIAutomationEventHandler handler);
        //virtual HRESULT STDMETHODCALLTYPE AddAutomationEventHandler( 
        //    /* [in] */ EVENTID eventId,
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ enum TreeScope scope,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [in] */ __RPC__in_opt IUIAutomationEventHandler *handler) = 0;
        

        [PreserveSig]  int RemoveAutomationEventHandler     (EVENTID eventId, IUIAutomationElement element, IUIAutomationEventHandler handler);
        //virtual HRESULT STDMETHODCALLTYPE RemoveAutomationEventHandler( 
        //    /* [in] */ EVENTID eventId,
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ __RPC__in_opt IUIAutomationEventHandler *handler) = 0;
        

        [PreserveSig]  int AddPropertyChangedEventHandlerNativeArray     ();
        //virtual HRESULT STDMETHODCALLTYPE AddPropertyChangedEventHandlerNativeArray( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ enum TreeScope scope,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [in] */ __RPC__in_opt IUIAutomationPropertyChangedEventHandler *handler,
        //    /* [size_is][in] */ __RPC__in_ecount_full(propertyCount) PROPERTYID *propertyArray,
        //    /* [in] */ int propertyCount) = 0;
        

        [PreserveSig]  int AddPropertyChangedEventHandler     ();
        //virtual HRESULT STDMETHODCALLTYPE AddPropertyChangedEventHandler( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ enum TreeScope scope,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [in] */ __RPC__in_opt IUIAutomationPropertyChangedEventHandler *handler,
        //    /* [in] */ __RPC__in SAFEARRAY * propertyArray) = 0;
        

        [PreserveSig]  int RemovePropertyChangedEventHandler     ();
        //virtual HRESULT STDMETHODCALLTYPE RemovePropertyChangedEventHandler( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ __RPC__in_opt IUIAutomationPropertyChangedEventHandler *handler) = 0;
        

        [PreserveSig]  int AddStructureChangedEventHandler     ();
        //virtual HRESULT STDMETHODCALLTYPE AddStructureChangedEventHandler( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ enum TreeScope scope,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [in] */ __RPC__in_opt IUIAutomationStructureChangedEventHandler *handler) = 0;
        

        [PreserveSig]  int RemoveStructureChangedEventHandler     ();
        //virtual HRESULT STDMETHODCALLTYPE RemoveStructureChangedEventHandler( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *element,
        //    /* [in] */ __RPC__in_opt IUIAutomationStructureChangedEventHandler *handler) = 0;
        

        [PreserveSig]  int AddFocusChangedEventHandler     (IUIAutomationCacheRequest cacheRequest, IUIAutomationFocusChangedEventHandler handler);
        //virtual HRESULT STDMETHODCALLTYPE AddFocusChangedEventHandler( 
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [in] */ __RPC__in_opt IUIAutomationFocusChangedEventHandler *handler) = 0;
        

        [PreserveSig]  int RemoveFocusChangedEventHandler     (IUIAutomationFocusChangedEventHandler handler);
        //virtual HRESULT STDMETHODCALLTYPE RemoveFocusChangedEventHandler( 
        //    /* [in] */ __RPC__in_opt IUIAutomationFocusChangedEventHandler *handler) = 0;
        

        [PreserveSig]  int RemoveAllEventHandlers     ();
        //virtual HRESULT STDMETHODCALLTYPE RemoveAllEventHandlers( void) = 0;
        

        [PreserveSig]  int IntNativeArrayToSafeArray     ();
        //virtual HRESULT STDMETHODCALLTYPE IntNativeArrayToSafeArray( 
        //    /* [size_is][in] */ __RPC__in_ecount_full(arrayCount) int *array,
        //    /* [in] */ int arrayCount,
        //    /* [retval][out] */ __RPC__deref_out_opt SAFEARRAY * *safeArray) = 0;
        

        [PreserveSig]  int IntSafeArrayToNativeArray     (IntPtr intArray, out IntPtr array, out int arrayCount);
        //virtual HRESULT STDMETHODCALLTYPE IntSafeArrayToNativeArray( 
        //    /* [in] */ __RPC__in SAFEARRAY * intArray,
        //    /* [size_is][size_is][out] */ __RPC__deref_out_ecount_full_opt(*arrayCount) int **array,
        //    /* [retval][out] */ __RPC__out int *arrayCount) = 0;
        

        [PreserveSig]  int RectToVariant     ();
        //virtual HRESULT STDMETHODCALLTYPE RectToVariant( 
        //    /* [in] */ RECT rc,
        //    /* [retval][out] */ __RPC__out VARIANT *var) = 0;
        

        [PreserveSig]  int VariantToRect     ();
        //virtual HRESULT STDMETHODCALLTYPE VariantToRect( 
        //    /* [in] */ VARIANT var,
        //    /* [retval][out] */ __RPC__out RECT *rc) = 0;
        

        [PreserveSig]  int SafeArrayToRectNativeArray     (IntPtr rects, out IntPtr rectArray, out int rectArrayCount);
        //virtual HRESULT STDMETHODCALLTYPE SafeArrayToRectNativeArray( 
        //    /* [in] */ __RPC__in SAFEARRAY * rects,
        //    /* [size_is][size_is][out] */ __RPC__deref_out_ecount_full_opt(*rectArrayCount) RECT **rectArray,
        //    /* [retval][out] */ __RPC__out int *rectArrayCount) = 0;
        

        [PreserveSig]  int CreateProxyFactoryEntry     ();
        //virtual HRESULT STDMETHODCALLTYPE CreateProxyFactoryEntry( 
        //    /* [in] */ __RPC__in_opt IUIAutomationProxyFactory *factory,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationProxyFactoryEntry **factoryEntry) = 0;
        

        [PreserveSig]  int get_ProxyFactoryMapping     ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ProxyFactoryMapping( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationProxyFactoryMapping **factoryMapping) = 0;
        

        [PreserveSig]  int GetPropertyProgrammaticName     ();
        //virtual HRESULT STDMETHODCALLTYPE GetPropertyProgrammaticName( 
        //    /* [in] */ PROPERTYID property,
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *name) = 0;
        

        [PreserveSig]  int GetPatternProgrammaticName     ();
        //virtual HRESULT STDMETHODCALLTYPE GetPatternProgrammaticName( 
        //    /* [in] */ PATTERNID pattern,
        //    /* [retval][out] */ __RPC__deref_out_opt BSTR *name) = 0;
        

        [PreserveSig]  int PollForPotentialSupportedPatterns     ();
        //virtual HRESULT STDMETHODCALLTYPE PollForPotentialSupportedPatterns( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *pElement,
        //    /* [out] */ __RPC__deref_out_opt SAFEARRAY * *patternIds,
        //    /* [out] */ __RPC__deref_out_opt SAFEARRAY * *patternNames) = 0;
        

        [PreserveSig]  int PollForPotentialSupportedProperties     ();
        //virtual HRESULT STDMETHODCALLTYPE PollForPotentialSupportedProperties( 
        //    /* [in] */ __RPC__in_opt IUIAutomationElement *pElement,
        //    /* [out] */ __RPC__deref_out_opt SAFEARRAY * *propertyIds,
        //    /* [out] */ __RPC__deref_out_opt SAFEARRAY * *propertyNames) = 0;
        

        [PreserveSig]  int CheckNotSupported     ();
        //virtual HRESULT STDMETHODCALLTYPE CheckNotSupported( 
        //    /* [in] */ VARIANT value,
        //    /* [retval][out] */ __RPC__out BOOL *isNotSupported) = 0;
        

        [PreserveSig]  int get_ReservedNotSupportedValue     ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ReservedNotSupportedValue( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUnknown **notSupportedValue) = 0;
        

        [PreserveSig]  int get_ReservedMixedAttributeValue     ();
        //virtual /* [propget] */ HRESULT STDMETHODCALLTYPE get_ReservedMixedAttributeValue( 
        //    /* [retval][out] */ __RPC__deref_out_opt IUnknown **mixedAttributeValue) = 0;
        

        [PreserveSig]  int ElementFromIAccessible     ();
        //virtual HRESULT STDMETHODCALLTYPE ElementFromIAccessible( 
        //    /* [in] */ __RPC__in_opt IAccessible *accessible,
        //    /* [in] */ int childId,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **element) = 0;
        

        [PreserveSig]  int ElementFromIAccessibleBuildCache     ();
        //virtual HRESULT STDMETHODCALLTYPE ElementFromIAccessibleBuildCache( 
        //    /* [in] */ __RPC__in_opt IAccessible *accessible,
        //    /* [in] */ int childId,
        //    /* [in] */ __RPC__in_opt IUIAutomationCacheRequest *cacheRequest,
        //    /* [retval][out] */ __RPC__deref_out_opt IUIAutomationElement **element) = 0;








    }
}
