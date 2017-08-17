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
// $hash=7d76e5dd6c0eccb57efb9a5202688705df7b5c16$
//

#include "libcef_dll/cpptoc/render_handler_cpptoc.h"
#include "libcef_dll/cpptoc/accessibility_handler_cpptoc.h"
#include "libcef_dll/ctocpp/browser_ctocpp.h"
#include "libcef_dll/ctocpp/drag_data_ctocpp.h"

namespace {

// MEMBER FUNCTIONS - Body may be edited by hand.

cef_accessibility_handler_t* CEF_CALLBACK
render_handler_get_accessibility_handler(struct _cef_render_handler_t* self) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return NULL;

  // Execute
  CefRefPtr<CefAccessibilityHandler> _retval =
      CefRenderHandlerCppToC::Get(self)->GetAccessibilityHandler();

  // Return type: refptr_same
  return CefAccessibilityHandlerCppToC::Wrap(_retval);
}

int CEF_CALLBACK
render_handler_get_root_screen_rect(struct _cef_render_handler_t* self,
                                    cef_browser_t* browser,
                                    cef_rect_t* rect) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return 0;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return 0;
  // Verify param: rect; type: simple_byref
  DCHECK(rect);
  if (!rect)
    return 0;

  // Translate param: rect; type: simple_byref
  CefRect rectVal = rect ? *rect : CefRect();

  // Execute
  bool _retval = CefRenderHandlerCppToC::Get(self)->GetRootScreenRect(
      CefBrowserCToCpp::Wrap(browser), rectVal);

  // Restore param: rect; type: simple_byref
  if (rect)
    *rect = rectVal;

  // Return type: bool
  return _retval;
}

int CEF_CALLBACK
render_handler_get_view_rect(struct _cef_render_handler_t* self,
                             cef_browser_t* browser,
                             cef_rect_t* rect) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return 0;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return 0;
  // Verify param: rect; type: simple_byref
  DCHECK(rect);
  if (!rect)
    return 0;

  // Translate param: rect; type: simple_byref
  CefRect rectVal = rect ? *rect : CefRect();

  // Execute
  bool _retval = CefRenderHandlerCppToC::Get(self)->GetViewRect(
      CefBrowserCToCpp::Wrap(browser), rectVal);

  // Restore param: rect; type: simple_byref
  if (rect)
    *rect = rectVal;

  // Return type: bool
  return _retval;
}

int CEF_CALLBACK
render_handler_get_screen_point(struct _cef_render_handler_t* self,
                                cef_browser_t* browser,
                                int viewX,
                                int viewY,
                                int* screenX,
                                int* screenY) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return 0;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return 0;
  // Verify param: screenX; type: simple_byref
  DCHECK(screenX);
  if (!screenX)
    return 0;
  // Verify param: screenY; type: simple_byref
  DCHECK(screenY);
  if (!screenY)
    return 0;

  // Translate param: screenX; type: simple_byref
  int screenXVal = screenX ? *screenX : 0;
  // Translate param: screenY; type: simple_byref
  int screenYVal = screenY ? *screenY : 0;

  // Execute
  bool _retval = CefRenderHandlerCppToC::Get(self)->GetScreenPoint(
      CefBrowserCToCpp::Wrap(browser), viewX, viewY, screenXVal, screenYVal);

  // Restore param: screenX; type: simple_byref
  if (screenX)
    *screenX = screenXVal;
  // Restore param: screenY; type: simple_byref
  if (screenY)
    *screenY = screenYVal;

  // Return type: bool
  return _retval;
}

int CEF_CALLBACK
render_handler_get_screen_info(struct _cef_render_handler_t* self,
                               cef_browser_t* browser,
                               struct _cef_screen_info_t* screen_info) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return 0;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return 0;
  // Verify param: screen_info; type: struct_byref
  DCHECK(screen_info);
  if (!screen_info)
    return 0;

  // Translate param: screen_info; type: struct_byref
  CefScreenInfo screen_infoObj;
  if (screen_info)
    screen_infoObj.AttachTo(*screen_info);

  // Execute
  bool _retval = CefRenderHandlerCppToC::Get(self)->GetScreenInfo(
      CefBrowserCToCpp::Wrap(browser), screen_infoObj);

  // Restore param: screen_info; type: struct_byref
  if (screen_info)
    screen_infoObj.DetachTo(*screen_info);

  // Return type: bool
  return _retval;
}

void CEF_CALLBACK
render_handler_on_popup_show(struct _cef_render_handler_t* self,
                             cef_browser_t* browser,
                             int show) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return;

  // Execute
  CefRenderHandlerCppToC::Get(self)->OnPopupShow(
      CefBrowserCToCpp::Wrap(browser), show ? true : false);
}

void CEF_CALLBACK
render_handler_on_popup_size(struct _cef_render_handler_t* self,
                             cef_browser_t* browser,
                             const cef_rect_t* rect) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return;
  // Verify param: rect; type: simple_byref_const
  DCHECK(rect);
  if (!rect)
    return;

  // Translate param: rect; type: simple_byref_const
  CefRect rectVal = rect ? *rect : CefRect();

  // Execute
  CefRenderHandlerCppToC::Get(self)->OnPopupSize(
      CefBrowserCToCpp::Wrap(browser), rectVal);
}

void CEF_CALLBACK render_handler_on_paint(struct _cef_render_handler_t* self,
                                          cef_browser_t* browser,
                                          cef_paint_element_type_t type,
                                          size_t dirtyRectsCount,
                                          cef_rect_t const* dirtyRects,
                                          const void* buffer,
                                          int width,
                                          int height) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return;
  // Verify param: dirtyRects; type: simple_vec_byref_const
  DCHECK(dirtyRectsCount == 0 || dirtyRects);
  if (dirtyRectsCount > 0 && !dirtyRects)
    return;
  // Verify param: buffer; type: simple_byaddr
  DCHECK(buffer);
  if (!buffer)
    return;

  // Translate param: dirtyRects; type: simple_vec_byref_const
  std::vector<CefRect> dirtyRectsList;
  if (dirtyRectsCount > 0) {
    for (size_t i = 0; i < dirtyRectsCount; ++i) {
      CefRect dirtyRectsVal = dirtyRects[i];
      dirtyRectsList.push_back(dirtyRectsVal);
    }
  }

  // Execute
  CefRenderHandlerCppToC::Get(self)->OnPaint(CefBrowserCToCpp::Wrap(browser),
                                             type, dirtyRectsList, buffer,
                                             width, height);
}

void CEF_CALLBACK render_handler_on_cursor_change(
    struct _cef_render_handler_t* self,
    cef_browser_t* browser,
    cef_cursor_handle_t cursor,
    cef_cursor_type_t type,
    const struct _cef_cursor_info_t* custom_cursor_info) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return;
  // Verify param: custom_cursor_info; type: struct_byref_const
  DCHECK(custom_cursor_info);
  if (!custom_cursor_info)
    return;

  // Translate param: custom_cursor_info; type: struct_byref_const
  CefCursorInfo custom_cursor_infoObj;
  if (custom_cursor_info)
    custom_cursor_infoObj.Set(*custom_cursor_info, false);

  // Execute
  CefRenderHandlerCppToC::Get(self)->OnCursorChange(
      CefBrowserCToCpp::Wrap(browser), cursor, type, custom_cursor_infoObj);
}

int CEF_CALLBACK
render_handler_start_dragging(struct _cef_render_handler_t* self,
                              cef_browser_t* browser,
                              cef_drag_data_t* drag_data,
                              cef_drag_operations_mask_t allowed_ops,
                              int x,
                              int y) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return 0;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return 0;
  // Verify param: drag_data; type: refptr_diff
  DCHECK(drag_data);
  if (!drag_data)
    return 0;

  // Execute
  bool _retval = CefRenderHandlerCppToC::Get(self)->StartDragging(
      CefBrowserCToCpp::Wrap(browser), CefDragDataCToCpp::Wrap(drag_data),
      allowed_ops, x, y);

  // Return type: bool
  return _retval;
}

void CEF_CALLBACK
render_handler_update_drag_cursor(struct _cef_render_handler_t* self,
                                  cef_browser_t* browser,
                                  cef_drag_operations_mask_t operation) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return;

  // Execute
  CefRenderHandlerCppToC::Get(self)->UpdateDragCursor(
      CefBrowserCToCpp::Wrap(browser), operation);
}

void CEF_CALLBACK
render_handler_on_scroll_offset_changed(struct _cef_render_handler_t* self,
                                        cef_browser_t* browser,
                                        double x,
                                        double y) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return;

  // Execute
  CefRenderHandlerCppToC::Get(self)->OnScrollOffsetChanged(
      CefBrowserCToCpp::Wrap(browser), x, y);
}

void CEF_CALLBACK render_handler_on_ime_composition_range_changed(
    struct _cef_render_handler_t* self,
    cef_browser_t* browser,
    const cef_range_t* selected_range,
    size_t character_boundsCount,
    cef_rect_t const* character_bounds) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return;
  // Verify param: selected_range; type: simple_byref_const
  DCHECK(selected_range);
  if (!selected_range)
    return;
  // Verify param: character_bounds; type: simple_vec_byref_const
  DCHECK(character_boundsCount == 0 || character_bounds);
  if (character_boundsCount > 0 && !character_bounds)
    return;

  // Translate param: selected_range; type: simple_byref_const
  CefRange selected_rangeVal = selected_range ? *selected_range : CefRange();
  // Translate param: character_bounds; type: simple_vec_byref_const
  std::vector<CefRect> character_boundsList;
  if (character_boundsCount > 0) {
    for (size_t i = 0; i < character_boundsCount; ++i) {
      CefRect character_boundsVal = character_bounds[i];
      character_boundsList.push_back(character_boundsVal);
    }
  }

  // Execute
  CefRenderHandlerCppToC::Get(self)->OnImeCompositionRangeChanged(
      CefBrowserCToCpp::Wrap(browser), selected_rangeVal, character_boundsList);
}

}  // namespace

// CONSTRUCTOR - Do not edit by hand.

CefRenderHandlerCppToC::CefRenderHandlerCppToC() {
  GetStruct()->get_accessibility_handler =
      render_handler_get_accessibility_handler;
  GetStruct()->get_root_screen_rect = render_handler_get_root_screen_rect;
  GetStruct()->get_view_rect = render_handler_get_view_rect;
  GetStruct()->get_screen_point = render_handler_get_screen_point;
  GetStruct()->get_screen_info = render_handler_get_screen_info;
  GetStruct()->on_popup_show = render_handler_on_popup_show;
  GetStruct()->on_popup_size = render_handler_on_popup_size;
  GetStruct()->on_paint = render_handler_on_paint;
  GetStruct()->on_cursor_change = render_handler_on_cursor_change;
  GetStruct()->start_dragging = render_handler_start_dragging;
  GetStruct()->update_drag_cursor = render_handler_update_drag_cursor;
  GetStruct()->on_scroll_offset_changed =
      render_handler_on_scroll_offset_changed;
  GetStruct()->on_ime_composition_range_changed =
      render_handler_on_ime_composition_range_changed;
}

template <>
CefRefPtr<CefRenderHandler> CefCppToCRefCounted<
    CefRenderHandlerCppToC,
    CefRenderHandler,
    cef_render_handler_t>::UnwrapDerived(CefWrapperType type,
                                         cef_render_handler_t* s) {
  NOTREACHED() << "Unexpected class type: " << type;
  return NULL;
}

#if DCHECK_IS_ON()
template <>
base::AtomicRefCount CefCppToCRefCounted<CefRenderHandlerCppToC,
                                         CefRenderHandler,
                                         cef_render_handler_t>::DebugObjCt = 0;
#endif

template <>
CefWrapperType CefCppToCRefCounted<CefRenderHandlerCppToC,
                                   CefRenderHandler,
                                   cef_render_handler_t>::kWrapperType =
    WT_RENDER_HANDLER;
