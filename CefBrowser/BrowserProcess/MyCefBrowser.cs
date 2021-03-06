﻿//MIT, 2015-2017, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.CefBridge.Auto;

namespace LayoutFarm.CefBridge
{
    public class MyCefBrowser
    {
        public event EventHandler BrowserDisposed;

        MyCefCallback managedCallback;
        string currentUrl = "about:blank";
        IWindowControl parentControl;
        IWindowForm topForm;
        IWindowForm devForm;
        MyCefDevWindow cefDevWindow; 
        CefOsrListener cefOsrListener;
        List<MyCefCallback> keepAliveCallBack = new List<MyCefCallback>();
        //----
        //this is object that handle native req
        MyCefBwHandler myBwHandler;

        /// <summary>
        /// my custom MyCefBw
        /// </summary>
        readonly MyCefBw _myCefBw;
        public MyCefBrowser(IWindowControl parentControl,
            int x, int y, int w, int h, string initUrl, bool isOsr)
        {
            //global handler
            this.managedCallback = new MyCefCallback(this.HandleNativeReq);

            //cef-specific collection of cef-handler
            myBwHandler = new MyCefBwHandler(this);
            //---------------------------------------------------------------
            this.currentUrl = initUrl;
            //create cef browser view handler  
            this.parentControl = parentControl;
            this.topForm = parentControl.GetTopLevelControl() as IWindowForm;


            //for specific browser
            if (this.IsOsr = isOsr)
            {

                _myCefBw = new MyCefBw(Cef3Binder.MyCefCreateMyWebBrowserOSR(managedCallback));
                Cef3Binder.MyCefSetupBrowserHwndOSR(_myCefBw.ptr, parentControl.GetHandle(), x, y, w, h, initUrl, IntPtr.Zero);
            }
            else
            {
                _myCefBw = new MyCefBw(Cef3Binder.MyCefCreateMyWebBrowser(managedCallback));
                Cef3Binder.MyCefSetupBrowserHwnd(_myCefBw.ptr, parentControl.GetHandle(), x, y, w, h, initUrl, IntPtr.Zero);
            }


            Cef3Binder.MyCefBwCall(this._myCefBw.ptr, CefBwCallMsg.CefBw_MyCef_EnableKeyIntercept, 1);

            //register mycef browser
            RegisterCefWbControl(this);
        }
        public bool IsOsr
        {
            get;
            private set;
        }
        public CefOsrListener OsrListener
        {
            get { return cefOsrListener; }
            set { cefOsrListener = value; }
        }

        public IWindowControl ParentControl { get { return this.parentControl; } }
        public IWindowForm ParentForm { get { return this.topForm; } }

        public bool IsBrowserCreated
        {
            get;
            private set;
        }


        void HandleNativeReq(int met_id, IntPtr argsPtr)
        {

            //main raw msg switch table 

            if ((met_id >> 16) > 0)
            {
                //built in object
                CefNativeRequestHandlers.HandleNativeReq_I0(myBwHandler, met_id, argsPtr);
                return;
            }

            //else this is custom msg

            switch ((MyCefMsg)met_id)
            {
                default:

                    break;
                case MyCefMsg.CEF_MSG_ClientHandler_NotifyBrowserCreated:
                    {
                        IsBrowserCreated = true;

                    }
                    break;
                case MyCefMsg.CEF_MSG_ClientHandler_NotifyBrowserClosing:
                    {

                    }
                    break;

                case MyCefMsg.CEF_MSG_ClientHandler_NotifyBrowserClosed:
                    {
                        if (this.devForm != null)
                        {
                            this.devForm.Close();
                            ((IDisposable)this.devForm).Dispose();
                            this.devForm = null;
                        }
                        if (this.BrowserDisposed != null)
                        {
                            this.BrowserDisposed(this, EventArgs.Empty);
                        }
                    }
                    break;
                //case MyCefMsg.CEF_MSG_ClientHandler_OnBeforeContextMenu:
                //    {
                //    }
                //    break;

                //case MyCefMsg.CEF_MSG_ClientHandler_DownloadUpdated:
                //    {
                //        //this version we notify back 
                //        //when 
                //        NativeCallArgs metArgs = new NativeCallArgs(argsPtr);
                //        if (browserProcessListener != null)
                //        {
                //            browserProcessListener.OnDownloadCompleted(metArgs);
                //        }
                //    }
                //    break;
                //case MyCefMsg.CEF_MSG_ClientHandler_OnBeforePopup:
                //    {
                //        NativeCallArgs args = new NativeCallArgs(argsPtr);
                //        //open new form with specific url
                //        string url = args.GetArgAsString(0);
                //        Cef3Binder.SafeUIInvoke(() =>
                //        {
                //            IWindowForm form = Cef3Binder.CreateNewBrowserWindow(800, 600);
                //            form.Show();
                //            //and navigate to a specific url 
                //        });
                //    }
                //    break;
                //case MyCefMsg.CEF_MSG_ClientHandler_OnConsoleMessage:
                //    {
                //        //console.log ...

                //        if (browserProcessListener != null)
                //        {
                //            NativeCallArgs args = new NativeCallArgs(argsPtr);
                //            browserProcessListener.OnConsoleLog(args);
                //        }
                //    }
                //    break;
                //case MyCefMsg.CEF_MSG_ClientHandler_ShowDevTools:
                //    {
                //        //show dev tools
                //        //Cef3Binder.SafeUIInvoke(() =>
                //        //{
                //        //    IWindowForm newPopupForm = Cef3Binder.CreateNewBrowserWindow(800, 600);
                //        //    newPopupForm.Show();
                //        //});
                //    }
                //    break;
                //case MyCefMsg.CEF_MSG_ClientHandler_OnLoadError:
                //    {
                //        //load page error
                //        //ui process
                //        var args = new NativeCallArgs(argsPtr);
                //        IntPtr cefBrowser = args.GetArgAsNativePtr(0);
                //        IntPtr cefFrame = args.GetArgAsNativePtr(1);
                //        int errorCode = args.GetArgAsInt32(2);//error code
                //        string errorText = args.GetArgAsString(3);//errorText
                //        string failedUrl = args.GetArgAsString(4); //failedUrl
                //        //---------------------------                        
                //        //load error page
                //        LoadErrorPage(cefBrowser, cefFrame, errorCode, errorText, failedUrl);
                //    }
                //    break;
                //case MyCefMsg.CEF_MSG_ClientHandler_OnCertError:
                //    {
                //        var args = new NativeCallArgs(argsPtr);
                //        string certErrMsg = args.GetArgAsString(0);
                //        args.SetOutput(0, 1);//true
                //    }
                //    break;
                //case MyCefMsg.CEF_MSG_ClientHandler_ExecCustomProtocol:
                //    {
                //        //disable all protocol
                //        var args = new NativeCallArgs(argsPtr);
                //        if (browserProcessListener != null)
                //        {
                //            browserProcessListener.OnExecProtocol(args);
                //        }
                //        else
                //        {
                //            args.SetOutput(0, 0);//disable all protocol
                //        }
                //    }
                //    break;
                //----
                //TODO: review here again
                //case MyCefMsg.CEF_MSG_ClientHandler_SetResourceManager:
                //    {
                //        //setup resource mx
                //        if (browserProcessListener != null)
                //        {
                //            //INIT_MY_MET_ARGS(metArgs, 1) 
                //            // MyCefSetVoidPtr2(&vargs[1], resource_manager_);

                //            var args = new NativeCallArgs(argsPtr);
                //            var resourceMx = new NativeResourceMx(args.GetArgAsNativePtr(1));
                //            browserProcessListener.OnAddResourceMx(resourceMx);
                //        }
                //    }
                //    break;
                //case MyCefMsg.CEF_MSG_RequestUrlFilter2:
                //    {
                //        //filter url name
                //        if (browserProcessListener != null)
                //        {
                //            var args = new NativeCallArgs(argsPtr);
                //            browserProcessListener.OnFilterUrl(args);
                //        }
                //    }
                //    break;
                //case MyCefMsg.CEF_MSG_BinaryResouceProvider_OnRequest:
                //    {
                //        //request for binary resource
                //        if (browserProcessListener != null)
                //        {
                //            var args = new NativeCallArgs(argsPtr);
                //            browserProcessListener.OnRequestForBinaryResource(args);
                //        }
                //    }
                //    break;
                //------------------------------
                //eg. from cefQuery --> 
                case MyCefMsg.CEF_MSG_OnQuery:
                    {
                        //if (browserProcessListener != null)
                        //{
                        //    var args = new NativeCallArgs(argsPtr);
                        //    QueryRequestArgs reqArgs = QueryRequestArgs.CreateRequest(args.GetArgAsNativePtr(0));
                        //    browserProcessListener.OnCefQuery(args, reqArgs);
                        //}


                        //TODO: review here again
                        //QueryRequestArgs queryReq;
                        //memset(&queryReq, 0, sizeof(QueryRequestArgs));
                        //queryReq.browser = browser.get();
                        //queryReq.frame = frame.get();
                        //queryReq.query_id = query_id;


                        //MyCefStringHolder mystr;
                        //mystr.value = request;
                        //queryReq.request = &mystr;
                        //queryReq.persistent = persistent;
                        //queryReq.callback = callback.get();

                        //MethodArgs args;
                        //memset(&args, 0, sizeof(MethodArgs));
                        //args.SetArgAsNativeObject(0, &queryReq);
                        //this->mcallback_(CEF_MSG_OnQuery, &args);
                        ////auto result = args.ReadOutputAsString(0);
                        //CefString cefstr = args.ReadOutputAsString(0);
                        //callback->Success(cefstr);

                    }
                    break;

                //------------------------------
                case MyCefMsg.CEF_MSG_ClientHandler_NotifyTitle:
                    {
                        //INIT_MY_MET_ARGS(metArgs, 1) 
                        //SetCefStringToJsValue2(&vargs[1], string);

                        //title changed
                        var args = new NativeCallArgs(argsPtr);
                        string newtitle = args.GetArgAsString(1);
                        // Console.WriteLine("title changed:" + newtitle);
                    }
                    break;
                case MyCefMsg.CEF_MSG_ClientHandler_NotifyAddress:
                    {
                        //INIT_MY_MET_ARGS(metArgs, 1) 
                        //SetCefStringToJsValue2(&vargs[1], string);
                        //address changed
                        var args = new NativeCallArgs(argsPtr);
                        string newtitle = args.GetArgAsString(1);
                        // Console.WriteLine("address changed:" + newtitle);
                    }
                    break;
                //------------------------------
                case MyCefMsg.CEF_MSG_OSR_Render:
                    {
                        //receive rendere msg
                        var args = new NativeCallArgs(argsPtr);
                        //copy bits buffer and store to files  
                        if (cefOsrListener != null)
                        {
                            cefOsrListener.OnRender(args);
                        }

                    }
                    break;
            }
        }
        void LoadErrorPage(IntPtr cefBw, IntPtr cefFrame, int errorCode, string errorText, string failedUrl)
        {


            //ss << "<html><head><title>Page failed to load</title></head>" 
            //    "<body bgcolor=\"white\">" 
            //    "<h3>Page failed to load.</h3>" 
            //    "URL: <a href=\"" << failed_url << "\">" << failed_url << "</a>" 
            //    "<br/>Error: " << test_runner::GetErrorString(error_code) <<
            //    " (" << error_code << ")"; 
            //if (!other_info.empty())
            //    ss << "<br/>" << other_info;
            //ss << "</body></html>";
            //frame->LoadURL(test_runner::GetDataURI(ss.str(), "text/html"));
        }
        public string CurrentUrl
        {
            get { return this.currentUrl; }
        }
        public void NavigateTo(string url)
        {
            currentUrl = url;
            if (IsBrowserCreated)
            {
                using (var bw = _myCefBw.GetBrowser())
                using (var fr = bw.GetMainFrame())
                {
                    fr.LoadURL(url);
                }
            }
        }
        public void ExecJavascript(string src, string scriptUrl)
        {
            using (var fr = _myCefBw.GetMainFrame())
            {
                fr.ExecuteJavaScript(src, scriptUrl, 0);
            }
        }
        public void PostData(string url, byte[] data, int len)
        {
            JsValue a0 = new JsValue();
            JsValue a1 = new JsValue();
            JsValue ret;

            var v_url = NativeMyCefStringHolder.CreateHolder(url);
            a0.Ptr = v_url.nativePtr;
            //
            unsafe
            {

                fixed (byte* buffer = &data[0])
                {
                    a1.Ptr = new IntPtr(buffer);
                    a1.I32 = data.Length;

                    Cef3Binder.MyCefBwCall2(_myCefBw.ptr, (int)CefBwCallMsg.CefBw_PostData, out ret, ref a0, ref a1);
                }
            }


            v_url.Dispose();
        }
        public void PostData2(string url, byte[] data, int len)
        {

            //CefRefPtr<CefRequest> request(CefRequest::Create());
            //MyCefStringHolder* url = (MyCefStringHolder*)v1->ptr;
            //request->SetURL(url->value);
            ////Add post data to request, the correct method and content-type header will be set by CEF 
            //CefRefPtr<CefPostDataElement> postDataElement(CefPostDataElement::Create());


            //char* buffer1 = new char[v2->i32];
            //memcpy_s(buffer1, v2->i32, v2->ptr, v2->i32);
            //postDataElement->SetToBytes(v2->i32, buffer1);
            ////------

            //CefRefPtr<CefPostData> postData(CefPostData::Create());
            //postData->AddElement(postDataElement);
            //request->SetPostData(postData);

            ////add custom header (for test)
            //CefRequest::HeaderMap headerMap;
            //headerMap.insert(
            //    std::make_pair("X-My-Header", "My Header Value"));
            //request->SetHeaderMap(headerMap);

            ////load request
            //myBw->bwWindow->GetBrowser()->GetMainFrame()->LoadRequest(request);

            //delete buffer1; 
            JsValue a0 = new JsValue();
            JsValue a1 = new JsValue();
            JsValue ret;

            var v_url = NativeMyCefStringHolder.CreateHolder(url);
            a0.Ptr = v_url.nativePtr;
            //
            unsafe
            {

                fixed (byte* buffer = &data[0])
                {
                    a1.Ptr = new IntPtr(buffer);
                    a1.I32 = data.Length;

                    Cef3Binder.MyCefBwCall2(_myCefBw.ptr, (int)CefBwCallMsg.CefBw_PostData, out ret, ref a0, ref a1);
                }
            }


            v_url.Dispose();
        }
        public void SetSize(int w, int h)
        {

            JsValue a0 = new JsValue();
            a0.I32 = w;
            JsValue a1 = new JsValue();
            a1.I32 = h;
            JsValue ret;
            Cef3Binder.MyCefBwCall2(_myCefBw.ptr, (int)CefBwCallMsg.CefBw_SetSize, out ret, ref a0, ref a1);
        }

        public void GetText(Action<string> strCallback)
        {

            using (var bw = _myCefBw.GetBrowser())
            using (var frame1 = bw.GetMainFrame())
            {
                //List<long> idens = new List<long>();
                //bw.GetFrameIdentifiers(idens); //test only

                MyCefCallback visitorCallback = (int methodId, IntPtr nativeArgs) =>
                {
                    //wrap with the specific pars
                    //var pars = new Auto.CefStringVisitor(nativeArgs);
                    //string data = pars._string;

                    //MyCefNativeMetArgs metArgs = new MyCefNativeMetArgs(nativeArgs);
                    //if (metArgs.GetArgCount() == 1)
                    //{
                    //    JsValue value;
                    //    metArgs.GetArg(1, out value);
                    //    string data = Cef3Binder.MyCefJsReadString(ref value);

                    //}
                };
                Auto.CefStringVisitor visitor = Auto.CefStringVisitor.New(visitorCallback);


                frame1.GetText(visitor);

                //keep alive callback
                //InternalGetText((id, nativePtr) =>
                //{
                //    //INIT_MY_MET_ARGS(metArgs, 1) 
                //    //SetCefStringToJsValue2(&vargs[1], string);

                //    var args = new NativeCallArgs(nativePtr);
                //    strCallback(args.GetArgAsString(1));
                //});
                //Cef3Binder.MyCefDomGetTextWalk(this.myCefBrowser, strCallback);
            } 
        }
        public void GetSource(Action<string> strCallback)
        {
            ////keep alive callback
            //InternalGetSource((id, nativePtr) =>
            //{
            //    //INIT_MY_MET_ARGS(metArgs, 1) 
            //    //SetCefStringToJsValue2(&vargs[1], string);
            //    var args = new NativeCallArgs(nativePtr);
            //    strCallback(args.GetArgAsString(1));
            //});
        }
        public void GetSource2(Action<string> strCallback)
        {
            Auto.CefStringVisitor visitor = Auto.CefStringVisitor.New((id, ptr) =>
            {
                //NativeCallArgs args = new NativeCallArgs(ptr);
                //var text = args.GetArgAsString(1);
            });

            using (var bw = _myCefBw.GetBrowser())
            using (var myframe = bw.GetMainFrame())
            {
                string url = myframe.GetURL();
                myframe.GetText(visitor);

                Auto.CefStringVisitor visitor2 = Auto.CefStringVisitor.New((id, ptr) =>
                {
                    //NativeCallArgs args = new NativeCallArgs(ptr);
                    //var text = args.GetArgAsString(1);
                });

                myframe.GetSource(visitor2);
            }

        }
        public Auto.CefBrowser GetNativeBw()
        {
            return _myCefBw.GetBrowser();
        }
        public void LoadText(string text, string url)
        {
            using (var bw = _myCefBw.GetBrowser())
            using (var frame1 = bw.GetMainFrame())
            {
                frame1.LoadString(text, url);
            }
        }

         

        public void Stop()
        {
            using (var bw = _myCefBw.GetBrowser())
            {
                bw.StopLoad();
            }
        }
        public void GoBack()
        {
            using (var bw = _myCefBw.GetBrowser())
            {
                bw.GoBack();
            }
        }
        public void GoForward()
        {
            using (var bw = _myCefBw.GetBrowser())
            {
                bw.GoForward();
            }
        }
        public void Reload()
        {
            using (var bw = _myCefBw.GetBrowser())
            {
                bw.Reload();
            }
        }
        public void ReloadIgnoreCache()
        {
            using (var bw = _myCefBw.GetBrowser())
            {
                bw.ReloadIgnoreCache();
            }
        }
        public void dbugTest()
        {

#if DEBUG
            //JsValue ret;
            //JsValue a0 = new JsValue();
            //JsValue a1 = new JsValue();

            //Cef3Binder.MyCefBwCall2(_myCefBw.ptr, 2, out ret, ref a0, ref a1);

            ////----------- 
            //Cef3Binder.MyCefBwCall2(_myCefBw.ptr, 4, out ret, ref a0, ref a1);
            //int frameCount = ret.I32;

            //////-----------
            //////get CefBrowser
            ////Cef3Binder.MyCefBwCall2(myCefBrowser, 5, out ret, ref a0, ref a1);
            ////IntPtr cefBw = ret.Ptr;

            ////a0.Ptr = cefBw;
            ////a0.Type = JsValueType.Wrapped;
            ////Cef3Binder.MyCefBwCall2(myCefBrowser, 6, out ret, ref a0, ref a1);
            //////-----------
            ////create native list 
            ////Cef3Binder.MyCefBwCall2(myCefBrowser, 8, out ret, ref a0, ref a1);

            //////get framename
            ////a0.Ptr = nativelist;
            ////
            //Cef3Binder.MyCefBwCall2(_myCefBw.ptr, 7, out ret, ref a0, ref a1);
            //IntPtr nativelist = a0.Ptr;

            ////get list
            //unsafe
            //{
            //    int len = ret.I32;
            //    JsValue* unsafe_arr = (JsValue*)a0.Ptr;
            //    JsValue[] arr = new JsValue[len];
            //    for (int i = 0; i < len; ++i)
            //    {
            //        arr[i] = unsafe_arr[i];
            //    }
            //    //delete array result 
            //    Cef3Binder.MyCefDeletePtrArray(unsafe_arr);
            //}
            ////------------------


            ////list count
            //a0.Ptr = nativelist;
            //a0.Type = JsValueType.Wrapped;
            //Cef3Binder.MyCefBwCall2(_myCefBw.ptr, 9, out ret, ref a0, ref a1);
            ////list count
            //int list_count = ret.I32;
            ////delete native ptr
            ////Cef3Binder.MyCefDeletePtr(nativelist);
            ////
            ////list count
            //a0.Ptr = nativelist;
            //a0.Type = JsValueType.Wrapped;
            ////GetFrameIdentifiers
            //Cef3Binder.MyCefBwCall2(_myCefBw.ptr, 10, out ret, ref a0, ref a1);
            ////get list
            //unsafe
            //{
            //    int len = a0.I32;
            //    JsValue* unsafe_arr = (JsValue*)a0.Ptr;
            //    JsValue[] arr = new JsValue[len];
            //    for (int i = 0; i < len; ++i)
            //    {
            //        arr[i] = unsafe_arr[i];
            //    }
            //    //delete array result 
            //    Cef3Binder.MyCefDeletePtrArray(unsafe_arr);
            //}

            //Cef3Binder.MyCefBwCall2(_myCefBw.ptr, 21, out ret, ref a0, ref a1);

            //unsafe
            //{
            //    int len = ret.I32 + 1; //+1 for null terminated string
            //    char* buff = stackalloc char[len];
            //    int actualLen = 0;
            //    Cef3Binder.MyCefStringHolder_Read(ret.Ptr, buff, len, out actualLen);
            //    string value = new string(buff);
            //    Cef3Binder.MyCefDeletePtr(ret.Ptr);
            //}


            //IntPtr pdfSetting = Cef3Binder.MyCefCreatePdfPrintSetting("{\"header_footer_enabled\":true}");

            ////------------------
#endif
        }


        public void ShowDevTools()
        {
            if (devForm == null)
            {
                devForm = Cef3Binder.CreateBlankForm(800, 600);
                devForm.Text = "Developer Tool";
                devForm.Show();
                devForm.FormClosed += DevForm_FormClosed;
            }
            if (cefDevWindow == null)
            {
                cefDevWindow = new MyCefDevWindow();
                Cef3Binder.MyCefShowDevTools(_myCefBw.ptr,
                    cefDevWindow.GetMyCefBrowser(),
                    devForm.GetHandle());
            }
        }

        private void DevForm_FormClosed(object sender, EventArgs e)
        {
            devForm = null;
            cefDevWindow = null;
        }

        List<MyCefCallback> tmpCallbacks = new List<MyCefCallback>();

        public void PrintToPdf(string filename)
        {
            MyCefCallback cb = null;
            cb = new MyCefCallback((id, args) =>
            {
                //remove after finish
                var metArg = new NativeCallArgs(args);
                int isOK = metArg.GetArgAsInt32(1);
                tmpCallbacks.Remove(cb);
            });
            tmpCallbacks.Add(cb);
            //
            Cef3Binder.MyCefPrintToPdf(_myCefBw.ptr, IntPtr.Zero, filename, cb);

        }
        public void PrintToPdf(string pdfConfig, string filename)
        {
            IntPtr nativePdfConfig = Cef3Binder.MyCefCreatePdfPrintSetting(pdfConfig);
            MyCefCallback cb = null;
            cb = new MyCefCallback((id, args) =>
            {
                //remove after finish
                var metArg = new NativeCallArgs(args);
                int isOK = metArg.GetArgAsInt32(1);
                tmpCallbacks.Remove(cb);
            });
            tmpCallbacks.Add(cb);
            //
            Cef3Binder.MyCefPrintToPdf(_myCefBw.ptr, nativePdfConfig, filename, cb);
        }
        internal void NotifyCloseBw()
        {
            this.Stop();
            //
            JsValue ret;
            JsValue a0 = new JsValue();
            JsValue a1 = new JsValue();
            Cef3Binder.MyCefBwCall2(_myCefBw.ptr, (int)CefBwCallMsg.CefBw_CloseBw, out ret, ref a0, ref a1);
        }

        static Dictionary<IWindowForm, List<MyCefBrowser>> registerTopWindowForms =
                   new Dictionary<IWindowForm, List<MyCefBrowser>>();
        static readonly object sync_remove = new object();
        static void RegisterCefWbControl(MyCefBrowser cefWb)
        {
            // get top level of this cef browser control

            var ownerForm = (IWindowForm)cefWb.ParentControl.GetTopLevelControl();
            List<MyCefBrowser> foundList;
            if (!registerTopWindowForms.TryGetValue(ownerForm, out foundList))
            {
                foundList = new List<MyCefBrowser>();
                registerTopWindowForms.Add(ownerForm, foundList);
            }
            if (!foundList.Contains(cefWb))
            {
                foundList.Add(cefWb);
                cefWb.BrowserDisposed += new EventHandler(cefBrowserControl_Disposed);
            }
        }

        public static void DisposeAllChildWebBrowserControls(IWindowForm ownerForm)
        {

            //dispose all web browser (child) windows inside this window form             
            List<MyCefBrowser> foundList;
            if (registerTopWindowForms.TryGetValue(ownerForm, out foundList))
            {
                //remove webbrowser controls             
                for (int i = foundList.Count - 1; i >= 0; --i)
                {
                    MyCefBrowser mycefBw = foundList[i];
                    IWindowControl wb = mycefBw.ParentControl;
                    mycefBw.NotifyCloseBw();
                    //---------------------------------------
                    var parent = wb.GetParent();
                    parent.RemoveChild(wb);

                    //this Dispose() will terminate cef_life_time_handle *** 
                    //after native side dispose the wb control
                    //it will raise event BrowserDisposed
                    wb.Dispose();

                    //---------------------------------------
                }
                registerTopWindowForms.Remove(ownerForm);
            }
        }
        static void cefBrowserControl_Disposed(object sender, EventArgs e)
        {
            //web browser control is disposed 
            //TODO: review here 
            MyCefBrowser wb = sender as MyCefBrowser;
            if (wb != null)
            {
                IWindowForm winHandle = wb.ParentForm;
                List<MyCefBrowser> wblist;
                if (registerTopWindowForms.TryGetValue(winHandle, out wblist))
                {
                    lock (sync_remove)
                    {
                        wblist.Remove(wb);
                    }
                }
            }
            else
            {
                throw new NotSupportedException();
            }

        }
        public static bool IsReadyToClose(IWindowForm winForm)
        {
            //ready-to-close winform 
            lock (sync_remove)
            {
                List<MyCefBrowser> cefBrowserList;
                if (registerTopWindowForms.TryGetValue(winForm, out cefBrowserList))
                {
                    return cefBrowserList.Count == 0;
                }
                return true;
            }
        }


        //void OnUnmanagedPartCallBack(int id, IntPtr callBackArgs)
        //{

        //    switch ((MethodName)id)
        //    {
        //        case MethodName.MET_GetResourceHandler:
        //            {
        //                GetResourceHandler(new ResourceRequestArg(
        //                    new NativeCallArgs(callBackArgs)));
        //            }
        //            break;
        //        case MethodName.MET_TCALLBACK:
        //            {
        //                //Console.WriteLine("TCALLBACK");
        //            }
        //            break;
        //    }
        //}
        //protected virtual void GetResourceHandler(ResourceRequestArg req)
        //{
        //    //sample here
        //    string requestURL = req.RequestUrl;
        //    //test change content here 
        //    if (requestURL.StartsWith("http://www.google.com"))
        //    {
        //        req.SetResponseData("text/html", @"<http><body>Hello!</body></http>");
        //    }
        //}

        //public class ResourceRequestArg
        //{
        //    NativeCallArgs nativeArgs;
        //    internal ResourceRequestArg(NativeCallArgs nativeArgs)
        //    {
        //        this.nativeArgs = nativeArgs;
        //    }
        //    public string RequestUrl
        //    {
        //        get
        //        {
        //            return this.nativeArgs.GetArgAsString(0);
        //        }
        //    }
        //    public void SetResponseData(string mime, string str)
        //    {
        //        nativeArgs.SetOutput(0, mime);
        //        var utf8Buffer = Encoding.UTF8.GetBytes(str.ToCharArray());
        //        nativeArgs.SetOutput(1, utf8Buffer);
        //    }
        //    public void SetResponseData(string mime, byte[] dataBuffer)
        //    {
        //        nativeArgs.SetOutput(0, mime);
        //        nativeArgs.SetOutput(1, dataBuffer);
        //    }
        //}
    }

    //------------------         
    /// <summary>
    /// multiple (cpp-to-cs) req handlers
    /// </summary>
    public class MyCefBwHandler : CefDisplayHandler.I0,
                             CefLifeSpanHandler.I0,
                             CefLoadHandler.I0,
                             CefDownloadHandler.I0,
                             CefKeyboardHandler.I0,
                             CefRequestHandler.I0

    {
        MyCefBrowser _ownerBrowser;
        public MyCefBwHandler(MyCefBrowser owner)
        {
            this._ownerBrowser = owner;
        }

        void CefLifeSpanHandler.I0.DoClose(CefLifeSpanHandler.DoCloseArgs args)
        {

        }

        void CefLifeSpanHandler.I0.OnAfterCreated(CefLifeSpanHandler.OnAfterCreatedArgs args)
        {

        }

        void CefLifeSpanHandler.I0.OnBeforeClose(CefLifeSpanHandler.OnBeforeCloseArgs args)
        {

        }

        void CefLifeSpanHandler.I0.OnBeforePopup(CefLifeSpanHandler.OnBeforePopupArgs args)
        {
            //NativeCallArgs args = new NativeCallArgs(argsPtr);
            ////open new form with specific url
            //string url = args.GetArgAsString(0);
            //Cef3Binder.SafeUIInvoke(() =>
            //{
            //    IWindowForm form = Cef3Binder.CreateNewBrowserWindow(800, 600);
            //    form.Show();
            //    //and navigate to a specific url 
            //});


            //NativeCallArgs args = new NativeCallArgs(argsPtr);
            ////open new form with specific url
            //string url = args.GetArgAsString(0);
            //Cef3Binder.SafeUIInvoke(() =>
            //{
            //    IWindowForm form = Cef3Binder.CreateNewBrowserWindow(800, 600);
            //    form.Show();
            //    //and navigate to a specific url 
            //});
        }
        //--------------

        void CefDisplayHandler.I0.OnAddressChange(CefDisplayHandler.OnAddressChangeArgs args)
        {
            string url = args.url();
        }

        void CefDisplayHandler.I0.OnConsoleMessage(CefDisplayHandler.OnConsoleMessageArgs args)
        {
            string msg = args.message();

        }

        void CefDisplayHandler.I0.OnFaviconURLChange(CefDisplayHandler.OnFaviconURLChangeArgs args)
        {

        }

        void CefDisplayHandler.I0.OnFullscreenModeChange(CefDisplayHandler.OnFullscreenModeChangeArgs args)
        {

        }

        void CefDisplayHandler.I0.OnStatusMessage(CefDisplayHandler.OnStatusMessageArgs args)
        {
        }

        void CefDisplayHandler.I0.OnTitleChange(CefDisplayHandler.OnTitleChangeArgs args)
        {
            string title = args.title();

        }
        void CefDisplayHandler.I0.OnTooltip(CefDisplayHandler.OnTooltipArgs args)
        {

        }
        //------------------
        void CefLoadHandler.I0.OnLoadingStateChange(CefLoadHandler.OnLoadingStateChangeArgs args)
        {

        }

        void CefLoadHandler.I0.OnLoadStart(CefLoadHandler.OnLoadStartArgs args)
        {

        }

        void CefLoadHandler.I0.OnLoadEnd(CefLoadHandler.OnLoadEndArgs args)
        {

        }

        void CefLoadHandler.I0.OnLoadError(CefLoadHandler.OnLoadErrorArgs args)
        {

            ////load page error
            ////ui process
            //var args = new NativeCallArgs(argsPtr);
            //IntPtr cefBrowser = args.GetArgAsNativePtr(0);
            //IntPtr cefFrame = args.GetArgAsNativePtr(1);
            //int errorCode = args.GetArgAsInt32(2);//error code
            //string errorText = args.GetArgAsString(3);//errorText
            //string failedUrl = args.GetArgAsString(4); //failedUrl
            //                                           //---------------------------                        
            //                                           //load error page
            //LoadErrorPage(cefBrowser, cefFrame, errorCode, errorText, failedUrl); 
        }


        //--------------
        void CefDownloadHandler.I0.OnBeforeDownload(CefDownloadHandler.OnBeforeDownloadArgs args)
        {

            // case MyCefMsg.CEF_MSG_ClientHandler_BeforeDownload:
            //    {
            //    //handle download path here
            //    NativeCallArgs metArgs = new NativeCallArgs(argsPtr);
            //    string pathName = metArgs.GetArgAsString(2);

            //}
            //break;

            //		MethodArgs metArgs;
            //		memset(&metArgs, 0, sizeof(MethodArgs));
            //		metArgs.SetArgAsNativeObject(0, browser);
            //		metArgs.SetArgAsNativeObject(1, download_item);
            //		metArgs.SetArgAsString(2, suggested_name.c_str());
            //		this->mcallback_(CEF_MSG_ClientHandler_BeforeDownload, &metArgs); //tmp

            //		auto downloadPath = metArgs.ReadOutputAsString(0);
            //		callback->Continue(downloadPath, false);

            //throw new NotImplementedException();
        }
        void CefDownloadHandler.I0.OnDownloadUpdated(CefDownloadHandler.OnDownloadUpdatedArgs args)
        {

            //CefDownloadHandlerExt::OnDownloadUpdated(this->mcallback_, browser, download_item, callback);

            //MethodArgs metArgs;
            //memset(&metArgs, 0, sizeof(MethodArgs));
            //metArgs.SetArgAsNativeObject(0, browser);
            //metArgs.SetArgAsNativeObject(1, download_item);
            //auto fullPath = download_item->GetFullPath();
            //metArgs.SetArgAsString(2, fullPath.c_str());
            //this->mcallback_(CEF_MSG_ClientHandler_DownloadUpdated, &metArgs); //tmp	  
            //throw new NotImplementedException();
        }

        void CefKeyboardHandler.I0.OnPreKeyEvent(CefKeyboardHandler.OnPreKeyEventArgs args)
        {

        }

        void CefKeyboardHandler.I0.OnKeyEvent(CefKeyboardHandler.OnKeyEventArgs args)
        {

        }
        //------------------------------------
        void CefRequestHandler.I0.OnBeforeBrowse(CefRequestHandler.OnBeforeBrowseArgs args)
        {

        }

        void CefRequestHandler.I0.OnOpenURLFromTab(CefRequestHandler.OnOpenURLFromTabArgs args)
        {

        }

        void CefRequestHandler.I0.OnBeforeResourceLoad(CefRequestHandler.OnBeforeResourceLoadArgs args)
        {

        }

        void CefRequestHandler.I0.GetResourceHandler(CefRequestHandler.GetResourceHandlerArgs args)
        {

        }

        void CefRequestHandler.I0.OnResourceRedirect(CefRequestHandler.OnResourceRedirectArgs args)
        {

        }

        void CefRequestHandler.I0.OnResourceResponse(CefRequestHandler.OnResourceResponseArgs args)
        {

        }

        void CefRequestHandler.I0.GetResourceResponseFilter(CefRequestHandler.GetResourceResponseFilterArgs args)
        {

        }

        void CefRequestHandler.I0.OnResourceLoadComplete(CefRequestHandler.OnResourceLoadCompleteArgs args)
        {

        }

        void CefRequestHandler.I0.GetAuthCredentials(CefRequestHandler.GetAuthCredentialsArgs args)
        {

        }

        void CefRequestHandler.I0.OnQuotaRequest(CefRequestHandler.OnQuotaRequestArgs args)
        {

        }

        void CefRequestHandler.I0.OnProtocolExecution(CefRequestHandler.OnProtocolExecutionArgs args)
        {

        }

        void CefRequestHandler.I0.OnCertificateError(CefRequestHandler.OnCertificateErrorArgs args)
        {

        }

        void CefRequestHandler.I0.OnSelectClientCertificate(CefRequestHandler.OnSelectClientCertificateArgs args)
        {

        }

        void CefRequestHandler.I0.OnPluginCrashed(CefRequestHandler.OnPluginCrashedArgs args)
        {

        }

        void CefRequestHandler.I0.OnRenderViewReady(CefRequestHandler.OnRenderViewReadyArgs args)
        {

        }

        void CefRequestHandler.I0.OnRenderProcessTerminated(CefRequestHandler.OnRenderProcessTerminatedArgs args)
        {

        }
    }

}