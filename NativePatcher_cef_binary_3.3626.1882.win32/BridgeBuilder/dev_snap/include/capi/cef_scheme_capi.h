// Copyright (c) 2019 Marshall A. Greenblatt. All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//    * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//    * Neither the name of Google Inc. nor the name Chromium Embedded
// Framework nor the names of its contributors may be used to endorse
// or promote products derived from this software without specific prior
// written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// ---------------------------------------------------------------------------
//
// This file was generated by the CEF translator tool and should not edited
// by hand. See the translator.README.txt file in the tools directory for
// more information.
//
// $hash=f83c72ec43f10f19eba82d9d3c284cc436cdbd23$
//

#ifndef CEF_INCLUDE_CAPI_CEF_SCHEME_CAPI_H_
#define CEF_INCLUDE_CAPI_CEF_SCHEME_CAPI_H_
#pragma once

#include "include/capi/cef_base_capi.h"
#include "include/capi/cef_browser_capi.h"
#include "include/capi/cef_frame_capi.h"
#include "include/capi/cef_request_capi.h"
#include "include/capi/cef_resource_handler_capi.h"
#include "include/capi/cef_response_capi.h"

#ifdef __cplusplus
extern "C" {
#endif

struct _cef_scheme_handler_factory_t;

///
// Structure that manages custom scheme registrations.
///
typedef struct _cef_scheme_registrar_t {
  ///
  // Base structure.
  ///
  cef_base_scoped_t base;

  ///
  // Register a custom scheme. This function should not be called for the built-
  // in HTTP, HTTPS, FILE, FTP, ABOUT and DATA schemes.
  //
  // If |is_standard| is true (1) the scheme will be treated as a standard
  // scheme. Standard schemes are subject to URL canonicalization and parsing
  // rules as defined in the Common Internet Scheme Syntax RFC 1738 Section 3.1
  // available at http://www.ietf.org/rfc/rfc1738.txt
  //
  // In particular, the syntax for standard scheme URLs must be of the form:
  // <pre>
  //  [scheme]://[username]:[password]@[host]:[port]/[url-path]
  // </pre> Standard scheme URLs must have a host component that is a fully
  // qualified domain name as defined in Section 3.5 of RFC 1034 [13] and
  // Section 2.1 of RFC 1123. These URLs will be canonicalized to
  // "scheme://host/path" in the simplest case and
  // "scheme://username:password@host:port/path" in the most explicit case. For
  // example, "scheme:host/path" and "scheme:///host/path" will both be
  // canonicalized to "scheme://host/path". The origin of a standard scheme URL
  // is the combination of scheme, host and port (i.e., "scheme://host:port" in
  // the most explicit case).
  //
  // For non-standard scheme URLs only the "scheme:" component is parsed and
  // canonicalized. The remainder of the URL will be passed to the handler as-
  // is. For example, "scheme:///some%20text" will remain the same. Non-standard
  // scheme URLs cannot be used as a target for form submission.
  //
  // If |is_local| is true (1) the scheme will be treated with the same security
  // rules as those applied to "file" URLs. Normal pages cannot link to or
  // access local URLs. Also, by default, local URLs can only perform
  // XMLHttpRequest calls to the same URL (origin + path) that originated the
  // request. To allow XMLHttpRequest calls from a local URL to other URLs with
  // the same origin set the CefSettings.file_access_from_file_urls_allowed
  // value to true (1). To allow XMLHttpRequest calls from a local URL to all
  // origins set the CefSettings.universal_access_from_file_urls_allowed value
  // to true (1).
  //
  // If |is_display_isolated| is true (1) the scheme can only be displayed from
  // other content hosted with the same scheme. For example, pages in other
  // origins cannot create iframes or hyperlinks to URLs with the scheme. For
  // schemes that must be accessible from other schemes set this value to false
  // (0), set |is_cors_enabled| to true (1), and use CORS "Access-Control-Allow-
  // Origin" headers to further restrict access.
  //
  // If |is_secure| is true (1) the scheme will be treated with the same
  // security rules as those applied to "https" URLs. For example, loading this
  // scheme from other secure schemes will not trigger mixed content warnings.
  //
  // If |is_cors_enabled| is true (1) the scheme can be sent CORS requests. This
  // value should be true (1) in most cases where |is_standard| is true (1).
  //
  // If |is_csp_bypassing| is true (1) the scheme can bypass Content-Security-
  // Policy (CSP) checks. This value should be false (0) in most cases where
  // |is_standard| is true (1).
  //
  // This function may be called on any thread. It should only be called once
  // per unique |scheme_name| value. If |scheme_name| is already registered or
  // if an error occurs this function will return false (0).
  ///
  int(CEF_CALLBACK* add_custom_scheme)(struct _cef_scheme_registrar_t* self,
                                       const cef_string_t* scheme_name,
                                       int is_standard,
                                       int is_local,
                                       int is_display_isolated,
                                       int is_secure,
                                       int is_cors_enabled,
                                       int is_csp_bypassing);
} cef_scheme_registrar_t;

///
// Structure that creates cef_resource_handler_t instances for handling scheme
// requests. The functions of this structure will always be called on the IO
// thread.
///
typedef struct _cef_scheme_handler_factory_t {
  ///
  // Base structure.
  ///
  cef_base_ref_counted_t base;

  ///
  // Return a new resource handler instance to handle the request or an NULL
  // reference to allow default handling of the request. |browser| and |frame|
  // will be the browser window and frame respectively that originated the
  // request or NULL if the request did not originate from a browser window (for
  // example, if the request came from cef_urlrequest_t). The |request| object
  // passed to this function will not contain cookie data.
  ///
  struct _cef_resource_handler_t*(CEF_CALLBACK* create)(
      struct _cef_scheme_handler_factory_t* self,
      struct _cef_browser_t* browser,
      struct _cef_frame_t* frame,
      const cef_string_t* scheme_name,
      struct _cef_request_t* request);
} cef_scheme_handler_factory_t;

///
// Register a scheme handler factory with the global request context. An NULL
// |domain_name| value for a standard scheme will cause the factory to match all
// domain names. The |domain_name| value will be ignored for non-standard
// schemes. If |scheme_name| is a built-in scheme and no handler is returned by
// |factory| then the built-in scheme handler factory will be called. If
// |scheme_name| is a custom scheme then you must also implement the
// cef_app_t::on_register_custom_schemes() function in all processes. This
// function may be called multiple times to change or remove the factory that
// matches the specified |scheme_name| and optional |domain_name|. Returns false
// (0) if an error occurs. This function may be called on any thread in the
// browser process. Using this function is equivalent to calling cef_request_tCo
// ntext::cef_request_context_get_global_context()->register_scheme_handler_fact
// ory().
///
CEF_EXPORT int cef_register_scheme_handler_factory(
    const cef_string_t* scheme_name,
    const cef_string_t* domain_name,
    cef_scheme_handler_factory_t* factory);

///
// Clear all scheme handler factories registered with the global request
// context. Returns false (0) on error. This function may be called on any
// thread in the browser process. Using this function is equivalent to calling c
// ef_request_tContext::cef_request_context_get_global_context()->clear_scheme_h
// andler_factories().
///
CEF_EXPORT int cef_clear_scheme_handler_factories();

#ifdef __cplusplus
}
#endif

#endif  // CEF_INCLUDE_CAPI_CEF_SCHEME_CAPI_H_
