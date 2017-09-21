//---THIS-FILE-IS-PATCHED , org=D:\projects\cef_binary_3.3071.1647.win32\cpptoc\resolve_callback_cpptoc.cc
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
// $hash=2055b8206165fb8583b6d7659366d438913f3c25$
//

#include "libcef_dll/cpptoc/resolve_callback_cpptoc.h"
#include "libcef_dll/transfer_util.h"

//---kneadium-ext-begin
#include "../myext/ExportFuncAuto.h"
#include "../myext/InternalHeaderForExportFunc.h"
//---kneadium-ext-end

namespace {

// MEMBER FUNCTIONS - Body may be edited by hand.

void CEF_CALLBACK
resolve_callback_on_resolve_completed(struct _cef_resolve_callback_t* self,
                                      cef_errorcode_t result,
                                      cef_string_list_t resolved_ips) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return;
  // Unverified params: resolved_ips

  // Translate param: resolved_ips; type: string_vec_byref_const
  std::vector<CefString> resolved_ipsList;
  transfer_string_list_contents(resolved_ips, resolved_ipsList);

//---kneadium-ext-begin
auto me = CefResolveCallbackCppToC::Get(self);
const int CALLER_CODE=(CefResolveCallbackExt::_typeName << 16) | CefResolveCallbackExt::CefResolveCallbackExt_OnResolveCompleted_1;
auto m_callback= me->GetManagedCallBack(CALLER_CODE);
if(m_callback){
CefResolveCallbackExt::OnResolveCompletedArgs args1(result,&resolved_ipsList);
m_callback(CALLER_CODE, &args1.arg);
 if (((args1.arg.myext_flags >> 21) & 1) == 1){
return;
}
}
//---kneadium-ext-end

  // Execute
  CefResolveCallbackCppToC::Get(self)->OnResolveCompleted(result,
                                                          resolved_ipsList);
}

}  // namespace

// CONSTRUCTOR - Do not edit by hand.

CefResolveCallbackCppToC::CefResolveCallbackCppToC() {
  GetStruct()->on_resolve_completed = resolve_callback_on_resolve_completed;
}

template <>
CefRefPtr<CefResolveCallback> CefCppToCRefCounted<
    CefResolveCallbackCppToC,
    CefResolveCallback,
    cef_resolve_callback_t>::UnwrapDerived(CefWrapperType type,
                                           cef_resolve_callback_t* s) {
  NOTREACHED() << "Unexpected class type: " << type;
  return NULL;
}

#if DCHECK_IS_ON()
template <>
base::AtomicRefCount CefCppToCRefCounted<CefResolveCallbackCppToC,
                                         CefResolveCallback,
                                         cef_resolve_callback_t>::DebugObjCt =
    0;
#endif

template <>
CefWrapperType CefCppToCRefCounted<CefResolveCallbackCppToC,
                                   CefResolveCallback,
                                   cef_resolve_callback_t>::kWrapperType =
    WT_RESOLVE_CALLBACK;
