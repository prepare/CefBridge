//---THIS-FILE-IS-PATCHED , org=D:\projects\cef_binary_3.3071.1647.win32\cpptoc\focus_handler_cpptoc.cc
// Copyright (c) 2017 The Chromium Embedded Framework Authors. All rights
// reserved. Use of this source code is governed by a BSD-style license that
// can be found in the LICENSE file.
//
// ---------------------------------------------------------------------------
//
// This file was generated by the CEF translator tool. If making changes by
// hand only do so within the body of existing method and function
// implementations. See the translator.README.txt file in the tools directory
// for more information.
//
// $hash=d3d4fbf680412ca26d5803ffd269dea1e06ed388$
//

#include "libcef_dll/cpptoc/focus_handler_cpptoc.h"
#include "libcef_dll/ctocpp/browser_ctocpp.h"

//---kneadium-ext-begin
#include "../myext/ExportFuncAuto.h"
#include "../myext/InternalHeaderForExportFunc.h"
//---kneadium-ext-end

namespace {

// MEMBER FUNCTIONS - Body may be edited by hand.

void CEF_CALLBACK focus_handler_on_take_focus(struct _cef_focus_handler_t* self,
                                              cef_browser_t* browser,
                                              int next) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return;

//---kneadium-ext-begin
auto me = CefFocusHandlerCppToC::Get(self);
const int CALLER_CODE=(CefFocusHandlerExt::_typeName << 16) | CefFocusHandlerExt::CefFocusHandlerExt_OnTakeFocus_1;
auto m_callback= me->GetManagedCallBack(CALLER_CODE);
if(m_callback){
CefFocusHandlerExt::OnTakeFocusArgs args1(browser,next);
m_callback(CALLER_CODE, &args1.arg);
 if (((args1.arg.myext_flags >> 21) & 1) == 1){
return;
}
}
//---kneadium-ext-end

  // Execute
  CefFocusHandlerCppToC::Get(self)->OnTakeFocus(CefBrowserCToCpp::Wrap(browser),
                                                next ? true : false);
}

int CEF_CALLBACK focus_handler_on_set_focus(struct _cef_focus_handler_t* self,
                                            cef_browser_t* browser,
                                            cef_focus_source_t source) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return 0;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return 0;

//---kneadium-ext-begin
auto me = CefFocusHandlerCppToC::Get(self);
const int CALLER_CODE=(CefFocusHandlerExt::_typeName << 16) | CefFocusHandlerExt::CefFocusHandlerExt_OnSetFocus_2;
auto m_callback= me->GetManagedCallBack(CALLER_CODE);
if(m_callback){
CefFocusHandlerExt::OnSetFocusArgs args1(browser,source);
m_callback(CALLER_CODE, &args1.arg);
 if (((args1.arg.myext_flags >> 21) & 1) == 1){
 return args1.arg.myext_ret_value;
}
}
//---kneadium-ext-end

  // Execute
  bool _retval = CefFocusHandlerCppToC::Get(self)->OnSetFocus(
      CefBrowserCToCpp::Wrap(browser), source);

  // Return type: bool
  return _retval;
}

void CEF_CALLBACK focus_handler_on_got_focus(struct _cef_focus_handler_t* self,
                                             cef_browser_t* browser) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return;

//---kneadium-ext-begin
auto me = CefFocusHandlerCppToC::Get(self);
const int CALLER_CODE=(CefFocusHandlerExt::_typeName << 16) | CefFocusHandlerExt::CefFocusHandlerExt_OnGotFocus_3;
auto m_callback= me->GetManagedCallBack(CALLER_CODE);
if(m_callback){
CefFocusHandlerExt::OnGotFocusArgs args1(browser);
m_callback(CALLER_CODE, &args1.arg);
 if (((args1.arg.myext_flags >> 21) & 1) == 1){
return;
}
}
//---kneadium-ext-end

  // Execute
  CefFocusHandlerCppToC::Get(self)->OnGotFocus(CefBrowserCToCpp::Wrap(browser));
}

}  // namespace

// CONSTRUCTOR - Do not edit by hand.

CefFocusHandlerCppToC::CefFocusHandlerCppToC() {
  GetStruct()->on_take_focus = focus_handler_on_take_focus;
  GetStruct()->on_set_focus = focus_handler_on_set_focus;
  GetStruct()->on_got_focus = focus_handler_on_got_focus;
}

template <>
CefRefPtr<CefFocusHandler> CefCppToCRefCounted<
    CefFocusHandlerCppToC,
    CefFocusHandler,
    cef_focus_handler_t>::UnwrapDerived(CefWrapperType type,
                                        cef_focus_handler_t* s) {
  NOTREACHED() << "Unexpected class type: " << type;
  return NULL;
}

#if DCHECK_IS_ON()
template <>
base::AtomicRefCount CefCppToCRefCounted<CefFocusHandlerCppToC,
                                         CefFocusHandler,
                                         cef_focus_handler_t>::DebugObjCt = 0;
#endif

template <>
CefWrapperType CefCppToCRefCounted<CefFocusHandlerCppToC,
                                   CefFocusHandler,
                                   cef_focus_handler_t>::kWrapperType =
    WT_FOCUS_HANDLER;
