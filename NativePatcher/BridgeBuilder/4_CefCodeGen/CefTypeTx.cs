﻿//MIT, 2016-2017 ,WinterDev
using System;
using System.Collections.Generic;
namespace BridgeBuilder
{

    enum ImplWrapDirection
    {
        None,
        CppToC,
        CToCpp,
    }

    /// <summary>
    /// cef type transformer
    /// </summary>
    abstract class CefTypeTx
    {
        CodeTypeDeclaration _implDecl;
#if DEBUG
        protected int _dbug_cpp_count = 0;
#endif
        public CefTypeTx(CodeTypeDeclaration originalDecl)
        {
            this.OriginalDecl = originalDecl;
        }
        public SimpleTypeSymbol UnderlyingCType { get; set; }
        public CodeTypeDeclaration OriginalDecl { get; private set; }
        public CodeTypeDeclaration ImplTypeDecl
        {
            get { return _implDecl; }
            set
            {
                _implDecl = value;

            }
        }
        public override string ToString()
        {
            if (_implDecl != null)
            {
                return OriginalDecl.ToString() + " impl_by " + _implDecl.ToString();
            }
            else
            {
                return OriginalDecl.ToString();
            }
        }

        internal int MaxMethodParCount { get; set; }
        public int CsInterOpTypeNameId { get; set; }

        public abstract void GenerateCode(CefCodeGenOutput output);

        public string CppImplClassName { get; set; }
        public int CppImplClassNameId { get; set; }

        internal static string GetRawPtrMet(ImplWrapDirection wrapDirection)
        {

            //c-to-cpp
            //            template <class ClassName, class BaseName, class StructName>
            //CefRefPtr<BaseName> CefCToCppRefCounted<ClassName, BaseName, StructName>::Wrap(
            //    StructName* s)
            //        {
            //            if (!s)
            //                return NULL;


            //            template <class ClassName, class BaseName, class StructName>
            //StructName* CefCToCppRefCounted<ClassName, BaseName, StructName>::Unwrap(
            //    CefRefPtr<BaseName> c)
            //        {
            //            if (!c.get())
            //                return NULL;

            //------------------------------------
            //cpp-to-c
            //            // Wrap a C++ class with a C structure.  This is used when the class
            //            // implementation exists on this side of the DLL boundary but will have methods
            //            // called from the other side of the DLL boundary.
            //            template <class ClassName, class BaseName, class StructName>
            //class CefCppToCRefCounted : public CefBaseRefCounted {
            // public:
            //  // Create a new wrapper instance and associated structure reference for
            //  // passing an object instance the other side.
            //  static StructName* Wrap(CefRefPtr<BaseName> c)
            //        {
            //            if (!c.get())
            //                return NULL;

            //            // Wrap our object with the CefCppToCRefCounted class.
            //            ClassName* wrapper = new ClassName();
            //            wrapper->wrapper_struct_.object_ = c.get();
            //            // Add a reference to our wrapper object that will be released once our
            //            // structure arrives on the other side.
            //            wrapper->AddRef();
            //            // Return the structure pointer that can now be passed to the other side.
            //            return wrapper->GetStruct();
            //        }

            //        // Retrieve the underlying object instance for a structure reference passed
            //        // back from the other side.
            //        static CefRefPtr<BaseName> Unwrap(StructName* s)
            //        {
            //            if (!s)
            //                return NULL;

            //            // Cast our structure to the wrapper structure type.
            //            WrapperStruct* wrapperStruct = GetWrapperStruct(s);

            //            // If the type does not match this object then we need to unwrap as the
            //            // derived type.
            //            if (wrapperStruct->type_ != kWrapperType)
            //                return UnwrapDerived(wrapperStruct->type_, s);

            //            // Add the underlying object instance to a smart pointer.
            //            CefRefPtr<BaseName> objectPtr(wrapperStruct->object_);
            //            // Release the reference to our wrapper object that was added before the
            //            // structure was passed back to us.
            //            wrapperStruct->wrapper_->Release();
            //            // Return the underlying object instance.
            //            return objectPtr;
            //        }


            switch (wrapDirection)
            {
                default:
                    throw new NotSupportedException();
                case ImplWrapDirection.CppToC:
                    return "Wrap";
                case ImplWrapDirection.CToCpp:
                    return "Unwrap";
            }
        }
        internal static string GetSmartPointerMet(ImplWrapDirection wrapDirection)
        {
            switch (wrapDirection)
            {
                default:
                    throw new NotSupportedException();
                case ImplWrapDirection.CppToC:
                    return "Unwrap";
                case ImplWrapDirection.CToCpp:
                    return "Wrap";
            }

        }

        /// <summary>
        /// bring data from srcExpression and store to the destExpression
        /// </summary>
        /// <param name="par"></param>
        /// <param name="destExpression"></param>
        /// <param name="srcExpression"></param>
        internal static void PrepareDataFromNativeToCs(MethodParameterTxInfo par, string destExpression, string srcExpression, bool stackBased)
        {

            TypeSymbol ret = par.TypeSymbol;
            //check if we need some clean up code after return to client  
            switch (ret.TypeSymbolKind)
            {
                default:
                    break;
                case TypeSymbolKind.TypeDef:
                    {
                        CTypeDefTypeSymbol ctypedef = (CTypeDefTypeSymbol)ret;
                        par.ArgExtractCode = "MyCefSetInt32(" + destExpression + ",(int32_t)" + srcExpression + ");";
                        return;
                    }
                case TypeSymbolKind.ReferenceOrPointer:
                    {
                        ReferenceOrPointerTypeSymbol refOrPtr = (ReferenceOrPointerTypeSymbol)ret;
                        switch (refOrPtr.Kind)
                        {
                            default:
                                {

                                }
                                break;
                            case ContainerTypeKind.CefRawPtr:
                                {
                                    //raw pointer 
                                    par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," + srcExpression + ");";
                                    return;
                                }
                            case ContainerTypeKind.ByRef:
                                {
                                    TypeSymbol elemType = refOrPtr.ElementType;
                                    //what type that implement this elem
                                    if (elemType.TypeSymbolKind == TypeSymbolKind.Simple)
                                    {
                                        SimpleTypeSymbol simpleElem = (SimpleTypeSymbol)elemType;
                                        switch (simpleElem.PrimitiveTypeKind)
                                        {
                                            case PrimitiveTypeKind.CefString:
                                                if (stackBased)
                                                {
                                                    par.ArgExtractCode = "SetCefStringToJsValue2(" + destExpression + "," + srcExpression + ");";
                                                }
                                                else
                                                {
                                                    par.ArgExtractCode = "SetCefStringToJsValue(" + destExpression + "," + srcExpression + ");";
                                                    //need StringHolder cleanup
                                                    par.ArgPostExtractCode = "DeleteCefStringHolderFromJsValue(" + destExpression + ");";
                                                }

                                                return;
                                            case PrimitiveTypeKind.NaitveInt:
                                                par.ArgExtractCode = "MyCefSetInt32(" + destExpression + ",(int32_t)" + srcExpression + ");";
                                                return;
                                            case PrimitiveTypeKind.size_t:
                                                par.ArgExtractCode = "MyCefSetInt32(" + destExpression + ",(int32_t)" + srcExpression + ");";
                                                return;
                                            case PrimitiveTypeKind.Int64:
                                                par.ArgExtractCode = "MyCefSetInt64(" + destExpression + "," + srcExpression + ");";
                                                return;
                                            case PrimitiveTypeKind.UInt64:
                                                par.ArgExtractCode = "MyCefSetUInt64(" + destExpression + "," + srcExpression + ");";
                                                return;
                                            case PrimitiveTypeKind.Double:
                                                par.ArgExtractCode = "MyCefSetDouble(" + destExpression + "," + srcExpression + ");";
                                                return;
                                            case PrimitiveTypeKind.Float:
                                                par.ArgExtractCode = "MyCefSetFloat(" + destExpression + "," + srcExpression + ");";
                                                return;
                                            case PrimitiveTypeKind.Bool:
                                                //reference to bool
                                                //par.ArgExtractCode = "MyCefSetBool(" + destExpression + "," + srcExpression + ");";
                                                par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + ",&" + srcExpression + ");";
                                                return;
                                            case PrimitiveTypeKind.UInt32:
                                                par.ArgExtractCode = "MyCefSetUInt32(" + destExpression + "," + srcExpression + ");";
                                                return;
                                            case PrimitiveTypeKind.Int32:
                                                par.ArgExtractCode = "MyCefSetInt32(" + destExpression + "," + srcExpression + ");";
                                                return;
                                            case PrimitiveTypeKind.NotPrimitiveType:
                                                {
                                                    CefTypeTx txPlan = simpleElem.CefTxPlan;
                                                    if (txPlan == null)
                                                    {
                                                        if (par.IsConst)
                                                        {
                                                            par.ArgExtractCode = "MyCefSetVoidPtr2(" + destExpression + ",&" + srcExpression + ");";
                                                        }
                                                        else
                                                        {
                                                            par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," + srcExpression + ");";
                                                        }
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        //find what type that implement wrap/unwrap
                                                        CodeTypeDeclaration implBy = txPlan.ImplTypeDecl;

                                                        //c-to-cpp => from 'raw' pointer to 'smart' pointer
                                                        //cpp-to-c => from 'smart' pointer to 'raw' pointer

                                                        if (stackBased)
                                                        {

                                                            if (implBy.Name.Contains("CToCpp"))
                                                            {
                                                                //so if you want to send this to client lib
                                                                //you need to GET raw pointer , so =>


                                                                string auto_p = "p_" + par.Name;
                                                                par.ArgPreExtractCode = "auto " + auto_p + "=" + implBy.Name + "::Unwrap" + "(" + srcExpression + ");";
                                                                par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," + auto_p + "); ";//unwrap 
                                                                par.ArgPostExtractCode = implBy.Name + "::Wrap" + "(" + auto_p + ");";//wrap

                                                                return;

                                                            }
                                                            else if (implBy.Name.Contains("CppToC"))
                                                            {
                                                                string auto_p = "p_" + par.Name;
                                                                par.ArgPreExtractCode = "auto " + auto_p + "=" + implBy.Name + "::Wrap" + "(" + srcExpression + ")";//wrap
                                                                par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," + auto_p + ");";
                                                                par.ArgPostExtractCode = implBy.Name + "::Unwrap" + "(" + auto_p + ");";//unwrap
                                                                return;
                                                            }
                                                            else
                                                            {
                                                                throw new NotSupportedException();
                                                            }
                                                        }
                                                        else
                                                        {

                                                            if (implBy.Name.Contains("CToCpp"))
                                                            {
                                                                //so if you want to send this to client lib
                                                                //you need to GET raw pointer , so =>

                                                                par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," +
                                                                      implBy.Name + "::Unwrap" + "(" + srcExpression + "));";
                                                                return;

                                                            }
                                                            else if (implBy.Name.Contains("CppToC"))
                                                            {
                                                                par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," +
                                                                    implBy.Name + "::Wrap" + "(" + srcExpression + "));";
                                                                return;
                                                            }
                                                            else
                                                            {
                                                                throw new NotSupportedException();
                                                            }
                                                        }

                                                    }
                                                }
                                        }

                                    }
                                    else if (elemType.TypeSymbolKind == TypeSymbolKind.Vec)
                                    {
                                        par.ArgExtractCode = "MyCefSetVoidPtr2(" + destExpression + ",&" + srcExpression + ");";
                                        return;
                                    }
                                    else
                                    {
                                        if (par.IsConst)
                                        {
                                            par.ArgExtractCode = "MyCefSetVoidPtr2(" + destExpression + ",&" + srcExpression + ");";
                                        }
                                        else
                                        {
                                            par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," + srcExpression + ");";
                                        }
                                        return;
                                    }
                                }
                                break;
                            case ContainerTypeKind.CefRefPtr:
                                //return CefRefPtr => this need unwrap method for raw pointer before send this to client
                                {
                                    TypeSymbol elemType = refOrPtr.ElementType;
                                    //what type that implement this elem
                                    if (elemType.TypeSymbolKind == TypeSymbolKind.Simple)
                                    {
                                        SimpleTypeSymbol simpleElem = (SimpleTypeSymbol)elemType;
                                        if (simpleElem.PrimitiveTypeKind == PrimitiveTypeKind.NotPrimitiveType)
                                        {
                                            CefTypeTx txPlan = simpleElem.CefTxPlan;
                                            if (txPlan == null)
                                            {

                                                if (simpleElem.ToString() == "CefBaseRefCounted")
                                                {
                                                    par.ArgExtractCode = "!";
                                                    return;
                                                }
                                                throw new NotSupportedException();

                                            }
                                            else
                                            {
                                                //find what type that implement wrap/unwrap
                                                CodeTypeDeclaration implBy = txPlan.ImplTypeDecl;

                                                //c-to-cpp => from 'raw' pointer to 'smart' pointer
                                                //cpp-to-c => from 'smart' pointer to 'raw' pointer

                                                if (stackBased)
                                                {
                                                    if (implBy.Name.Contains("CToCpp"))
                                                    {
                                                        //so if you want to send this to client lib
                                                        //you need to GET raw pointer , so =>

                                                        //find result after extract 
                                                        string unwrapType = txPlan.UnderlyingCType.ToString();
                                                        //since this is CefRefPtr


                                                        //so after ElementType we should get pointer of the underlying element 
                                                        string auto_p = "p_" + par.Name;
                                                        par.CppUnwrapType = unwrapType + "*";
                                                        par.CppUnwrapMethod = implBy.Name + "::Unwrap";
                                                        par.CppWrapMethod = implBy.Name + "::Wrap";
                                                        //
                                                        par.ArgPreExtractCode = "auto " + auto_p + "=" + par.CppUnwrapMethod + "(" + srcExpression + ");"; //unwrap
                                                        par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," + auto_p + ");";
                                                        par.ArgPostExtractCode = implBy.Name + "::Wrap" + "(" + auto_p + ");"; //wrap                                                        

                                                    }
                                                    else if (implBy.Name.Contains("CppToC"))
                                                    {
                                                        string auto_p = "p_" + par.Name;
                                                        par.ArgPreExtractCode = "auto " + auto_p + "=" + implBy.Name + "::Wrap" + "(" + srcExpression + ");"; //wrap
                                                        par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," + auto_p + ");";
                                                        par.ArgPostExtractCode = implBy.Name + "::Unwrap" + "(" + auto_p + ");";//unwrap
                                                    }
                                                    else
                                                    {
                                                        throw new NotSupportedException();
                                                    }
                                                }
                                                else
                                                {
                                                    if (implBy.Name.Contains("CToCpp"))
                                                    {
                                                        //so if you want to send this to client lib
                                                        //you need to GET raw pointer , so =>

                                                        par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," +
                                                              implBy.Name + "::Unwrap" + "(" + srcExpression + "));";

                                                    }
                                                    else if (implBy.Name.Contains("CppToC"))
                                                    {
                                                        par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," +
                                                            implBy.Name + "::Wrap" + "(" + srcExpression + "));";
                                                    }
                                                    else
                                                    {
                                                        throw new NotSupportedException();
                                                    }
                                                }
                                                return;
                                            }
                                        }
                                        else
                                        {

                                        }
                                    }
                                    else
                                    {
                                        par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," + srcExpression + ");";
                                        return;
                                    }
                                }
                                break;
                            case ContainerTypeKind.Pointer:
                                {

                                    TypeSymbol elemType = refOrPtr.ElementType;
                                    if (elemType.TypeSymbolKind == TypeSymbolKind.Simple)
                                    {
                                        SimpleTypeSymbol simpleElem = (SimpleTypeSymbol)elemType;
                                        switch (simpleElem.PrimitiveTypeKind)
                                        {
                                            default:
                                                break;
                                            case PrimitiveTypeKind.NotPrimitiveType:
                                                {
                                                    CodeTypeDeclaration createdBy = simpleElem.CreatedByTypeDeclaration;
                                                    if (createdBy.Kind == TypeKind.Enum)
                                                    {
                                                        if (!par.IsConst)
                                                        {
                                                            par.ArgExtractCode = "MyCefSetInt32(" + destExpression + ",(int32_t)" + srcExpression + ");";
                                                            //TODO: set post 
                                                            return;
                                                        }
                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                                break;
                                            case PrimitiveTypeKind.Bool:
                                                {
                                                    //bool*
                                                    if (par.IsConst)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        par.ArgExtractCode = "MyCefSetBool(" + destExpression + ",*" + srcExpression + ");";
                                                        return;
                                                    }
                                                }
                                                break;
                                            case PrimitiveTypeKind.Void:
                                                {
                                                    //void*
                                                    if (par.IsConst)
                                                    {
                                                        par.ArgExtractCode = "MyCefSetVoidPtr2(" + destExpression + "," + srcExpression + ");";
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        par.ArgExtractCode = "MyCefSetVoidPtr(" + destExpression + "," + srcExpression + ");";
                                                        return;
                                                    }
                                                }
                                        }
                                    }
                                    else if (elemType.TypeSymbolKind == TypeSymbolKind.TypeDef)
                                    {


                                    }
                                    else
                                    {

                                    }
                                }
                                break;
                            case ContainerTypeKind.scoped_ptr:
                                break;
                        }
                    }
                    break;
                case TypeSymbolKind.Simple:
                    {
                        SimpleTypeSymbol simpleType = (SimpleTypeSymbol)ret;
                        switch (simpleType.PrimitiveTypeKind)
                        {
                            default:
                                break;
                            case PrimitiveTypeKind.Void:
                                par.ArgExtractCode = null;
                                return;
                            case PrimitiveTypeKind.NotPrimitiveType:
                                {
                                    SimpleTypeSymbol ss = (SimpleTypeSymbol)simpleType;
                                    if (ss.IsEnum)
                                    {
                                        //enum class 
                                        par.ArgExtractCode = "MyCefSetInt32(" + destExpression + ",(int32_t)" + srcExpression + ");";
                                        return;
                                    }
                                    else
                                    {
                                        switch (ss.Name)
                                        {
                                            default:
                                                {

                                                }
                                                break;
                                            case "CefTime":
                                            case "CefRect":
                                            case "CefPoint":
                                                {
                                                    //original code return the "value" type
                                                    //we have 2 choices
                                                    //1. copy-by-value
                                                    //2. copy-by-reference

                                                    //---test with copy by reference
                                                    //

                                                    par.ArgExtractCode = ss.Name + "* tmp_d1= new " + ss.Name + "(" + srcExpression + ");\r\n" +
                                                        "MyCefSetVoidPtr(" + destExpression + ",tmp_d1);\r\n";
                                                    return;
                                                }
                                        }
                                    }
                                }
                                break;
                            case PrimitiveTypeKind.CefString:
                                if (stackBased)
                                {
                                    par.ArgExtractCode = "SetCefStringToJsValue(" + destExpression + "," + srcExpression + ");";
                                }
                                else
                                {
                                    par.ArgExtractCode = "SetCefStringToJsValue(" + destExpression + "," + srcExpression + ");";
                                }
                                return;
                            case PrimitiveTypeKind.NaitveInt:
                                par.ArgExtractCode = "MyCefSetInt32(" + destExpression + ",(int32_t)" + srcExpression + ");";
                                return;
                            case PrimitiveTypeKind.size_t:
                                par.ArgExtractCode = "MyCefSetInt32(" + destExpression + ",(int32_t)" + srcExpression + ");";
                                return;
                            case PrimitiveTypeKind.Int64:
                                par.ArgExtractCode = "MyCefSetInt64(" + destExpression + "," + srcExpression + ");";
                                return;
                            case PrimitiveTypeKind.UInt64:
                                par.ArgExtractCode = "MyCefSetUInt64(" + destExpression + "," + srcExpression + ");";
                                return;
                            case PrimitiveTypeKind.Double:
                                par.ArgExtractCode = "MyCefSetDouble(" + destExpression + "," + srcExpression + ");";
                                return;
                            case PrimitiveTypeKind.Float:
                                par.ArgExtractCode = "MyCefSetFloat(" + destExpression + "," + srcExpression + ");";
                                return;
                            case PrimitiveTypeKind.Bool:
                                par.ArgExtractCode = "MyCefSetBool(" + destExpression + "," + srcExpression + ");";
                                return;
                            case PrimitiveTypeKind.UInt32:
                                par.ArgExtractCode = "MyCefSetUInt32(" + destExpression + "," + srcExpression + ");";
                                return;
                            case PrimitiveTypeKind.Int32:
                                par.ArgExtractCode = "MyCefSetInt32(" + destExpression + "," + srcExpression + ");";
                                return;
                        }
                    }
                    break;
            }
            throw new NotSupportedException();

        }

        internal static void PrepareCppMetArg(MethodParameterTxInfo par, string argName)
        {
            par.ClearExtractCode();
            TypeSymbol typeSymbol = par.TypeSymbol;
            TypeBridgeInfo bridge = typeSymbol.BridgeInfo;
            switch (typeSymbol.TypeSymbolKind)
            {
                default:

                    break;
                case TypeSymbolKind.Simple:
                    {
                        SimpleTypeSymbol simpleType = (SimpleTypeSymbol)typeSymbol;
                        if (simpleType.IsEnum)
                        {
                            //.net send enum as int32 
                            par.ArgExtractCode = "(" + simpleType.ToString() + ")" + argName + "->i32";//review here
                        }
                        else
                        {
                            switch (simpleType.PrimitiveTypeKind)
                            {
                                default:
                                    break;
                                case PrimitiveTypeKind.size_t: //uint32                                     
                                    par.ArgExtractCode = argName + "->" + bridge.CefCppSlotName;//review here
                                    break;
                                case PrimitiveTypeKind.Bool:
                                    par.ArgExtractCode = argName + "->" + bridge.CefCppSlotName + " !=0 ";//review here
                                    break;
                                case PrimitiveTypeKind.NotPrimitiveType:
                                    par.ArgExtractCode = "(" + simpleType.ToString() + "*)" + argName + "->" + bridge.CefCppSlotName;
                                    break;
                                case PrimitiveTypeKind.NaitveInt:
                                case PrimitiveTypeKind.Int32:
                                case PrimitiveTypeKind.Int64:
                                case PrimitiveTypeKind.UInt32:
                                case PrimitiveTypeKind.Float:
                                case PrimitiveTypeKind.Double:
                                    par.ArgExtractCode = argName + "->" + bridge.CefCppSlotName;
                                    break;
                            }
                        }
                    }
                    break;
                case TypeSymbolKind.TypeDef:
                    {
                        //eg. FileDialogMode
                        //check refer to 
                        //eg CefProcessId
                        CTypeDefTypeSymbol ctypedef = (CTypeDefTypeSymbol)typeSymbol;
                        TypeSymbol referTo = ctypedef.ReferToTypeSymbol;
                        if (referTo.TypeSymbolKind == TypeSymbolKind.Simple)
                        {
                            SimpleTypeSymbol ss = (SimpleTypeSymbol)referTo;
                            if (ss.IsEnum)
                            {
                                if (ctypedef.ParentType != null && !ctypedef.ParentType.IsGlobalCompilationUnitTypeDefinition)
                                {
                                    par.ArgExtractCode = "(" + ctypedef.ParentType + "::" + ctypedef.ToString() + ")" + argName + "->i32";
                                }
                                else
                                {
                                    par.ArgExtractCode = "(" + ctypedef.ToString() + ")" + argName + "->i32";
                                }

                            }
                            else if (ss.PrimitiveTypeKind == PrimitiveTypeKind.NotPrimitiveType)
                            {


                            }
                            else if (ss.PrimitiveTypeKind == PrimitiveTypeKind.UInt32)
                            {
                                //cef_color_t
                                par.ArgExtractCode = "(" + ctypedef.ToString() + ")" + argName + "->i32";//review here
                            }
                            else
                            {
                                par.ArgExtractCode = argName + "->" + bridge.CefCppSlotName;//review here
                            }
                        }
                        else
                        {

                        }

                    }
                    break;
                case TypeSymbolKind.ReferenceOrPointer:
                    {
                        ReferenceOrPointerTypeSymbol refOrPtr = (ReferenceOrPointerTypeSymbol)typeSymbol;
                        switch (refOrPtr.Kind)
                        {
                            default:
                                break;
                            case ContainerTypeKind.CefRawPtr:
                            case ContainerTypeKind.Pointer:
                                {
                                    TypeBridgeInfo bridgeInfo = refOrPtr.BridgeInfo;
                                    TypeSymbol elemtType = refOrPtr.ElementType;
                                    if (elemtType is SimpleTypeSymbol)
                                    {
                                        SimpleTypeSymbol ss = (SimpleTypeSymbol)elemtType;
                                        string elem_typename = ss.ToString();
                                        switch (elem_typename)
                                        {
                                            default:
                                                {

                                                }
                                                break;
                                            case "CefSchemeRegistrar":
                                                {
                                                    string slotName = bridge.CefCppSlotName.ToString();
                                                    par.ArgExtractCode = "(CefSchemeRegistrar*)" + argName + "->" + slotName;//direct cast
                                                }
                                                break;
                                            case "void":
                                                {
                                                    //void*
                                                    string slotName = bridge.CefCppSlotName.ToString();
                                                    par.ArgExtractCode = "(void*)" + argName + "->" + slotName;//direct cast
                                                }
                                                break;
                                            case "char":
                                                {
                                                    //char*
                                                    string slotName = bridge.CefCppSlotName.ToString();
                                                    par.ArgExtractCode = argName + "->" + slotName;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {

                                    }
                                }
                                break;
                            case ContainerTypeKind.CefRefPtr:
                                {
                                    //from cef 'smart' pointer

                                    TypeBridgeInfo bridgeInfo = refOrPtr.BridgeInfo;
                                    TypeSymbol elemtType = refOrPtr.ElementType;
                                    if (elemtType is SimpleTypeSymbol)
                                    {
                                        CefTypeTx txplan = ((SimpleTypeSymbol)elemtType).CefTxPlan;
                                        if (txplan == null)
                                        {
                                            if (elemtType.ToString() == "CefBaseRefCounted")
                                            {
                                                //bool SetUserData(CefRefPtr<CefBaseRefCounted> user_data)
                                                //only 1 
                                                string slotName = bridge.CefCppSlotName.ToString();
                                                par.ArgExtractCode = argName + "->" + slotName;
                                            }
                                            else
                                            {
                                                throw new NotSupportedException();
                                            }
                                        }
                                        else
                                        {
                                            CodeTypeDeclaration implTypeDecl = txplan.ImplTypeDecl;
                                            ImplWrapDirection implWrapDirection = ImplWrapDirection.None;
                                            if (implTypeDecl.Name.Contains("CToCpp"))
                                            {
                                                implWrapDirection = ImplWrapDirection.CToCpp;
                                                string met = GetSmartPointerMet(implWrapDirection);
                                                string slotName = bridge.CefCppSlotName.ToString();
                                                par.ArgExtractCode = implTypeDecl.Name + "::" + met + "(" + "(" + txplan.UnderlyingCType + "*)" + (argName + "->" + slotName) + ")";

                                            }
                                            else if (implTypeDecl.Name.Contains("CppToC"))
                                            {
                                                implWrapDirection = ImplWrapDirection.CppToC;
                                                string met = GetSmartPointerMet(implWrapDirection);
                                                string slotName = bridge.CefCppSlotName.ToString();
                                                par.ArgExtractCode = implTypeDecl.Name + "::" + met + "(" + "(" + txplan.UnderlyingCType + "*)" + (argName + "->" + slotName) + ")";
                                            }
                                            else
                                            {
                                                implWrapDirection = ImplWrapDirection.None;
                                                string met = GetSmartPointerMet(implWrapDirection);
                                                string slotName = bridge.CefCppSlotName.ToString();
                                                par.ArgExtractCode = implTypeDecl.Name + "::" + met + "(" + (argName + "->" + slotName) + ")";

                                            }
                                        }
                                    }
                                    else
                                    {
                                        //should not visit here
                                        throw new NotSupportedException();
                                    }
                                }
                                break;
                            case ContainerTypeKind.ByRef:
                                {
                                    TypeSymbol elemType = refOrPtr.ElementType;
                                    switch (elemType.TypeSymbolKind)
                                    {
                                        default:
                                            break;
                                        case TypeSymbolKind.Simple:
                                            {

                                                string elem_typename = refOrPtr.ElementType.ToString();
                                                switch (elem_typename)
                                                {
                                                    default:
                                                        break;
                                                    case "bool"://bool&
                                                        {
                                                            //eg. bool GetAccelerator(int command_id,int& key_code,bool& shift_pressed,bool& ctrl_pressed,bool& alt_pressed)


                                                            string slotName = bridge.CefCppSlotName.ToString();
                                                            par.ArgExtractCode = "*((bool*)" + argName + "->" + slotName + ")";

                                                            //par.ArgExtractCode = "&tmp_" + argName;
                                                            //if (!par.IsConst)
                                                            //{
                                                            //    par.ArgPreExtractCode = elem_typename + " tmp_" + argName + "=" + argName + "->" + slotName;
                                                            //    par.ArgPostExtractCode = PrepareCppReturnToCs(par.TypeSymbol, argName, " tmp_" + argName);
                                                            //}
                                                        }
                                                        break;
                                                    case "size_t":
                                                        {
                                                            //size_t&
                                                            //bool GetDataResource(int resource_id, void*&data,size_t & data_size)
                                                            //bool GetDataResourceForScale(int resource_id,ScaleFactor scale_factor,void*& data,size_t& data_size)

                                                            //string slotName = bridge.CefCppSlotName.ToString();

                                                            //par.ArgExtractCode = "&tmp_" + argName;
                                                            //if (!par.IsConst)
                                                            //{
                                                            //    par.ArgPreExtractCode = elem_typename + " tmp_" + argName + "=" + argName + "->" + slotName;
                                                            //    par.ArgPostExtractCode = PrepareCppReturnToCs(par.TypeSymbol, argName, " tmp_" + argName);
                                                            //}
                                                            string slotName = bridge.CefCppSlotName.ToString();
                                                            par.ArgExtractCode = "*((size_t*)" + argName + "->" + slotName + ")";

                                                        }
                                                        break;
                                                    case "float": //float&
                                                        {
                                                            //eg. bool GetRepresentationInfo(float scale_factor,float& actual_scale_factor,int& pixel_width,int& pixel_height)

                                                            //string slotName = bridge.CefCppSlotName.ToString();

                                                            //par.ArgExtractCode = "&tmp_" + argName;
                                                            //if (!par.IsConst)
                                                            //{
                                                            //    par.ArgPreExtractCode = elem_typename + " tmp_" + argName + "=" + argName + "->" + slotName;
                                                            //    par.ArgPostExtractCode = PrepareCppReturnToCs(par.TypeSymbol, argName, " tmp_" + argName);
                                                            //}
                                                            string slotName = bridge.CefCppSlotName.ToString();
                                                            par.ArgExtractCode = "*((float*)" + argName + "->" + slotName + ")";

                                                        }
                                                        break;
                                                    case "int":
                                                        {
                                                            //eg .bool GetRepresentationInfo(float scale_factor,float& actual_scale_factor,int& pixel_width,int& pixel_height)

                                                            //string slotName = bridge.CefCppSlotName.ToString();

                                                            //par.ArgExtractCode = "&tmp_" + argName;
                                                            //if (!par.IsConst)
                                                            //{
                                                            //    par.ArgPreExtractCode = elem_typename + " tmp_" + argName + "=" + argName + "->" + slotName;
                                                            //    par.ArgPostExtractCode = PrepareCppReturnToCs(par.TypeSymbol, argName, " tmp_" + argName);
                                                            //}
                                                            string slotName = bridge.CefCppSlotName.ToString();
                                                            par.ArgExtractCode = "*((int*)" + argName + "->" + slotName + ")";
                                                        }
                                                        break;
                                                    case "CefWindowInfo":
                                                    case "CefPoint":
                                                    case "CefSize":
                                                    case "CefRect":
                                                    case "CefRange":
                                                        {
                                                            //eg. void ShowDevTools(const CefWindowInfo& windowInfo,CefRefPtr<CefClient> client,const CefBrowserSettings& settings,const CefPoint& inspect_element_at)
                                                            //eg. void ImeSetComposition(const CefString& text,const std::vector<CefCompositionUnderline>& underlines,const CefRange& replacement_range,const CefRange& selection_range)

                                                            //string slotName = bridge.CefCppSlotName.ToString();
                                                            //par.ArgExtractCode = "&tmp_" + argName;

                                                            //if (!par.IsConst)
                                                            //{
                                                            //    par.ArgPreExtractCode = elem_typename + "* tmp_" + argName + "=" + "(*" + elem_typename + ")" + argName + "->" + slotName;
                                                            //    par.ArgPostExtractCode = PrepareCppReturnToCs(par.TypeSymbol, argName, " tmp_" + argName);
                                                            //}
                                                            string slotName = bridge.CefCppSlotName.ToString();
                                                            par.ArgExtractCode = "*((" + elem_typename + "*)" + argName + "->" + slotName + ")";
                                                        }
                                                        break;
                                                    case "CefString":
                                                        {
                                                            //CefString&
                                                            //known type names  
                                                            par.ArgExtractCode = "GetStringHolder(" + argName + ")->value"; //CefString          
                                                            //if (!par.IsConst)
                                                            //{

                                                            //}
                                                        }
                                                        break;
                                                }
                                            }
                                            break;
                                        case TypeSymbolKind.ReferenceOrPointer:
                                            {
                                                string elem_typename = refOrPtr.ElementType.ToString();
                                                switch (elem_typename)
                                                {
                                                    default:

                                                        break;
                                                    case "void*":
                                                        {
                                                            //eg. bool GetDataResource(int resource_id,void*& data,size_t& data_size)
                                                            //string slotName = bridge.CefCppSlotName.ToString();

                                                            //par.ArgExtractCode = "&tmp_" + argName;
                                                            //if (par.IsConst)
                                                            //{
                                                            //    par.ArgPreExtractCode = elem_typename + " tmp_" + argName + "=" + "(*" + elem_typename + ")" + argName + "->" + slotName;
                                                            //}
                                                            string slotName = bridge.CefCppSlotName.ToString();
                                                            par.ArgExtractCode = "*((" + elem_typename + "*)" + argName + "->" + slotName + ")";
                                                        }
                                                        break;
                                                    case "CefRefPtr<CefV8Value>":
                                                        {
                                                            //eg. bool Eval(const CefString& code,const CefString& script_url,int start_line,CefRefPtr<CefV8Value>& retval,CefRefPtr<CefV8Exception>& exception)
                                                            if (par.IsConst)
                                                            {

                                                            }
                                                            string slotName = bridge.CefCppSlotName.ToString();
                                                            par.ArgExtractCode = "*((" + elem_typename + "*)" + argName + "->" + slotName + ")";
                                                        }
                                                        break;
                                                    case "CefRefPtr<CefV8Exception>":
                                                        {
                                                            //eg. bool Eval(const CefString& code,const CefString& script_url,int start_line,CefRefPtr<CefV8Value>& retval,CefRefPtr<CefV8Exception>& exception)
                                                            if (par.IsConst)
                                                            {

                                                            }
                                                            string slotName = bridge.CefCppSlotName.ToString();
                                                            par.ArgExtractCode = "*((" + elem_typename + "*)" + argName + "->" + slotName + ")";
                                                        }
                                                        break;
                                                }
                                            }
                                            break;
                                        case TypeSymbolKind.Vec:
                                            {
                                                string elem_typename = refOrPtr.ElementType.ToString();
                                                string slotName = bridge.CefCppSlotName.ToString();
                                                par.ArgExtractCode = "*((" + elem_typename + "*)" + argName + "->" + slotName + ")";

                                                //switch (elem_typename)
                                                //{
                                                //    default:
                                                //        break;
                                                //    case "vec<CefString>":
                                                //        {
                                                //            //eg. void GetArgv(std::vector<CefString>& argv)
                                                //            //eg. bool GetDictionarySuggestions(std::vector<CefString>& suggestions)
                                                //            //eg. void SetSupportedSchemes(const std::vector<CefString>& schemes,CefRefPtr<CefCompletionCallback> callback)   
                                                //        }
                                                //        break;
                                                //    case "vec<int64>":
                                                //        {
                                                //            //eg. void GetFrameIdentifiers(std::vector<int64>& identifiers)
                                                //        }
                                                //        break;
                                                //    case "vec<CefCompositionUnderline>":
                                                //        {

                                                //        }
                                                //        break;
                                                //}
                                            }
                                            break;
                                        case TypeSymbolKind.Template:
                                            break;
                                        case TypeSymbolKind.TypeDef:
                                            {
                                                //eg. void ImeSetComposition(const CefString& text,const std::vector<CefCompositionUnderline>& underlines,const CefRange& replacement_range,const CefRange& selection_range)
                                                CTypeDefTypeSymbol ctypedef = (CTypeDefTypeSymbol)elemType;

                                                if (ctypedef.ParentType != null && !ctypedef.ParentType.IsGlobalCompilationUnitTypeDefinition)
                                                {
                                                    string elem_typename = refOrPtr.ElementType.ToString();
                                                    string slotName = bridge.CefCppSlotName.ToString();
                                                    par.ArgExtractCode = "*((" + ctypedef.ParentType + "::" + elem_typename + "*)" + argName + "->" + slotName + ")";

                                                }
                                                else
                                                {
                                                    string elem_typename = refOrPtr.ElementType.ToString();
                                                    string slotName = bridge.CefCppSlotName.ToString();
                                                    par.ArgExtractCode = "*((" + elem_typename + "*)" + argName + "->" + slotName + ")";
                                                }




                                                ////typedef 
                                                //string elem_typename = refOrPtr.ElementType.ToString();
                                                //switch (elem_typename)
                                                //{
                                                //    default:
                                                //        break;
                                                //    case "CefBrowserSettings":
                                                //    case "CefPdfPrintSettings":
                                                //    //eg. void PrintToPDF(const CefString& path,const CefPdfPrintSettings& settings,CefRefPtr<CefPdfPrintCallback> callback)
                                                //    case "CefKeyEvent":
                                                //    case "CefMouseEvent":
                                                //    //eg. void SendMouseWheelEvent(const CefMouseEvent& event,int deltaX,int deltaY)
                                                //    case "CefCookie":
                                                //    //eg. bool SetCookie(const CefString& url,const CefCookie& cookie,CefRefPtr<CefSetCookieCallback> callback)
                                                //    //

                                                //    case "AttributeMap":
                                                //    case "ElementVector":
                                                //    //eg. {bool GetColor(int command_id,cef_menu_color_type_t color_type,cef_color_t& color)}
                                                //    case "HeaderMap":
                                                //    case "SwitchMap":
                                                //    //eg. void GetSwitches(SwitchMap& switches)
                                                //    case "cef_color_t":
                                                //    //eg. bool GetColor(int command_id,cef_menu_color_type_t color_type,cef_color_t& color)
                                                //    case "ArgumentList":
                                                //    //eg. void GetArguments(ArgumentList& arguments)
                                                //    case "CefV8ValueList":
                                                //    //eg. CefRefPtr<CefV8Value> ExecuteFunction(CefRefPtr<CefV8Value> object,const CefV8ValueList& arguments)
                                                //    case "IssuerChainBinaryList":
                                                //    //eg {void GetDEREncodedIssuerChain(IssuerChainBinaryList& chain)}
                                                //    case "PageRangeList":
                                                //    case "KeyList":     //eg. {bool GetKeys(KeyList& keys)}                                                
                                                //        {

                                                //        }
                                                //        break;
                                                //}
                                            }
                                            break;

                                    }
                                }
                                break;
                        }
                    }
                    break;
            }

        }

        internal static string GetCsRetName(TypeSymbol retType)
        {
            //return type from cs
            switch (retType.TypeSymbolKind)
            {
                default:
                    break;
                case TypeSymbolKind.TypeDef:
                    {
                        CTypeDefTypeSymbol typedef = (CTypeDefTypeSymbol)retType;
                        TypeSymbol referToType = typedef.ReferToTypeSymbol;
                        if (referToType.TypeSymbolKind == TypeSymbolKind.Simple)
                        {
                            SimpleTypeSymbol ss = (SimpleTypeSymbol)referToType;
                            if (ss.CreatedByTypeDeclaration != null)
                            {
                                if (ss.CreatedByTypeDeclaration.Kind == TypeKind.Enum)
                                {
                                    return ss.CreatedByTypeDeclaration.Name;
                                }
                            }
                            else
                            {
                                return "IntPtr";
                            }
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                    break;
                case TypeSymbolKind.Simple:
                    {
                        SimpleTypeSymbol ss = (SimpleTypeSymbol)retType;
                        switch (ss.PrimitiveTypeKind)
                        {
                            default:
                                throw new NotSupportedException();
                            case PrimitiveTypeKind.Bool:
                                return "bool";
                            case PrimitiveTypeKind.CefString:
                                return "string";
                            case PrimitiveTypeKind.Double:
                                return "double";
                            case PrimitiveTypeKind.Float:
                                return "float";
                            case PrimitiveTypeKind.NaitveInt:
                                return "int";
                            case PrimitiveTypeKind.Int32:
                                return "int";
                            case PrimitiveTypeKind.Int64:
                                return "long";
                            case PrimitiveTypeKind.NotPrimitiveType:
                                {
                                    if (ss.IsEnum)
                                    {
                                        return ss.Name;
                                    }
                                    else
                                    {
                                        return ss.Name;
                                    }
                                }
                                break;
                            case PrimitiveTypeKind.size_t:
                                return "uint";
                            case PrimitiveTypeKind.UInt32:
                                return "uint";
                            case PrimitiveTypeKind.UInt64:
                                return "ulong";
                            case PrimitiveTypeKind.Void:
                                return "void";
                        }
                    }
                    break;
                case TypeSymbolKind.ReferenceOrPointer:
                    {
                        ReferenceOrPointerTypeSymbol refOrPtr = (ReferenceOrPointerTypeSymbol)retType;
                        TypeSymbol elemType = refOrPtr.ElementType;
                        switch (elemType.TypeSymbolKind)
                        {
                            default:
                                throw new NotSupportedException();
                            case TypeSymbolKind.ReferenceOrPointer:
                                {
                                    //eg. bool GetDataResource(int resource_id,void*& data,size_t& data_size)

                                    switch (elemType.ToString())
                                    {
                                        default:
                                            break;
                                        case "CefRefPtr<CefClient>":
                                            return "IntPtr";
                                        case "CefRefPtr<CefV8Value>":
                                            return "IntPtr";
                                        case "void*":
                                            return "IntPtr";
                                        case "CefRefPtr<CefV8Exception>":
                                            return "IntPtr";
                                    }
                                }
                                break;
                            case TypeSymbolKind.TypeDef:
                                {
                                    CTypeDefTypeSymbol typedef = (CTypeDefTypeSymbol)elemType;

                                    switch (typedef.Name)
                                    {
                                        default:
                                            throw new NotSupportedException();
                                        case "CefCursorInfo":
                                        case "CefPopupFeatures":
                                        case "cef_color_t":
                                        case "CefKeyEvent":
                                        case "CefMouseEvent":
                                        case "CefBrowserSettings":
                                        case "CefPdfPrintSettings":
                                        case "SwitchMap":
                                        case "ArgumentList":
                                        case "AttributeMap":
                                        case "CefCookie":
                                        case "PageRangeList":
                                        case "HeaderMap":
                                        case "ElementVector":
                                        case "CefV8ValueList":
                                        case "KeyList":
                                        case "IssuerChainBinaryList":
                                            return typedef.Name;
                                    }


                                }
                                break;
                            case TypeSymbolKind.Vec:
                                {
                                    VecTypeSymbol vec = (VecTypeSymbol)elemType;
                                    TypeSymbol elem = vec.ElementType;
                                    switch (elem.TypeSymbolKind)
                                    {
                                        default:
                                            throw new NotSupportedException();
                                        case TypeSymbolKind.ReferenceOrPointer:
                                            {
                                                string name = elem.ToString();
                                                switch (name)
                                                {
                                                    case "CefRefPtr<CefX509Certificate>":
                                                        return "List<CefCompositionUnderline>";
                                                }

                                                //list of a smart pointer object
                                            }
                                            break;
                                        case TypeSymbolKind.TypeDef:
                                            {
                                                CTypeDefTypeSymbol typedef = (CTypeDefTypeSymbol)elem;
                                                switch (typedef.Name)
                                                {
                                                    default:
                                                        throw new NotSupportedException();
                                                    case "CefCompositionUnderline":
                                                        return "List<CefCompositionUnderline>";
                                                }
                                            }
                                            break;
                                        case TypeSymbolKind.Simple:
                                            {
                                                SimpleTypeSymbol ss = (SimpleTypeSymbol)elem;
                                                switch (ss.PrimitiveTypeKind)
                                                {
                                                    default:
                                                        throw new NotSupportedException();
                                                    case PrimitiveTypeKind.NotPrimitiveType:
                                                        return "List<object>";
                                                    case PrimitiveTypeKind.Int64:
                                                        //list of unit
                                                        return "List<long>";
                                                    case PrimitiveTypeKind.String:
                                                        return "List<string>";
                                                    case PrimitiveTypeKind.CefString:
                                                        return "List<string>";

                                                }
                                            }
                                    }
                                }
                                break;
                            case TypeSymbolKind.Simple:
                                {
                                    SimpleTypeSymbol ss = (SimpleTypeSymbol)elemType;

                                    switch (ss.PrimitiveTypeKind)
                                    {
                                        default:
                                            throw new NotSupportedException();
                                        case PrimitiveTypeKind.Char:
                                            //pointer or reference of char
                                            return "IntPtr";
                                        case PrimitiveTypeKind.Bool:
                                            return "ref bool";
                                        case PrimitiveTypeKind.CefString:
                                            return "string";
                                        case PrimitiveTypeKind.Double:
                                            return "ref  double";
                                        case PrimitiveTypeKind.Float:
                                            return "ref float";
                                        case PrimitiveTypeKind.NaitveInt:
                                        case PrimitiveTypeKind.Int32:
                                            return "ref int";
                                        case PrimitiveTypeKind.Int64:
                                            return "long";
                                        case PrimitiveTypeKind.NotPrimitiveType:
                                            //ref of this simple type
                                            return ss.Name;
                                        case PrimitiveTypeKind.size_t:
                                            return "ref uint";
                                        case PrimitiveTypeKind.UInt32:
                                            return "ref uint";
                                        case PrimitiveTypeKind.UInt64:
                                            return "ref ulong";
                                        case PrimitiveTypeKind.Void:
                                            //request void* 
                                            //change this to IntPtr
                                            return "IntPtr";
                                    }

                                }
                                break;
                        }
                    }
                    break;

            }

            throw new NotSupportedException();
        }

        internal static string PrepareDataFromNativeToCs(TypeSymbol ret, string retName, string autoRetResultName)
        {

            //check if we need some clean up code after return to client  
            switch (ret.TypeSymbolKind)
            {
                default:
                    break;
                case TypeSymbolKind.TypeDef:
                    {
                        CTypeDefTypeSymbol ctypedef = (CTypeDefTypeSymbol)ret;
                        //from native type def  
                        TypeSymbol referToType = ctypedef.ReferToTypeSymbol;
                        if (referToType.TypeSymbolKind == TypeSymbolKind.Simple)
                        {
                            SimpleTypeSymbol ss = (SimpleTypeSymbol)referToType;
                            if (ss.IsEnum)
                            {
                                //return "var " + autoRetResultName + "= (" + referToType.ToString() + ")ret.I32;\r\n";
                                return " return (" + referToType.ToString() + ")" + retName + ".I32;\r\n";
                            }
                        }
                        return "return " + retName + ".Ptr;";
                        //return "IntPtr " + autoRetResultName + "= ret.Ptr;";

                    }
                case TypeSymbolKind.ReferenceOrPointer:
                    {
                        ReferenceOrPointerTypeSymbol refOrPtr = (ReferenceOrPointerTypeSymbol)ret;
                        switch (refOrPtr.Kind)
                        {
                            default:
                            case ContainerTypeKind.ByRef:
                                throw new NotSupportedException();
                            case ContainerTypeKind.CefRefPtr:
                                {
                                    //the result is inner pointer from cef 'smart' pointer
                                    TypeSymbol elemType = refOrPtr.ElementType;
                                    //return "var " + autoRetResultName + "= new " + elemType + "(ret.Ptr);";
                                    return "return new " + elemType + "(" + retName + ".Ptr);";
                                }
                        }
                    }
                case TypeSymbolKind.Simple:
                    {
                        SimpleTypeSymbol simpleType = (SimpleTypeSymbol)ret;
                        switch (simpleType.PrimitiveTypeKind)
                        {
                            default:
                                break;
                            case PrimitiveTypeKind.Void:
                                return null;
                            case PrimitiveTypeKind.NotPrimitiveType:
                                {
                                    SimpleTypeSymbol ss = (SimpleTypeSymbol)simpleType;
                                    if (ss.IsEnum)
                                    {
                                        //enum ,
                                        //cast from i32 to specific enum type
                                        //return "var " + autoRetResultName + "=(" + simpleType.Name + ")" + retName + ".I32;\r\n";
                                        return "return (" + simpleType.Name + ")" + retName + ".I32;\r\n";
                                    }
                                    else
                                    {
                                        switch (ss.Name)
                                        {
                                            default:
                                                {

                                                }
                                                break;
                                            case "CefTime":
                                            case "CefRect":
                                            case "CefPoint":
                                                {
                                                    //original code return the "value" type
                                                    //we have 2 choices
                                                    //1. copy-by-value
                                                    //2. copy-by-reference

                                                    //---test with copy by reference
                                                    //
                                                    // return "var " + autoRetResultName + "= new " + ss.Name + "(" + retName + ".Ptr);\r\n";
                                                    return "return new " + ss.Name + "(" + retName + ".Ptr);\r\n";
                                                }
                                        }
                                    }
                                }
                                break;
                            case PrimitiveTypeKind.CefString:
                                //get string from native side
                                //and we not need this CefString anymore,=> delete it too
                                //
                                //NativeMyCefStringHolder ret_str = new NativeMyCefStringHolder(ret.Ptr);
                                //string url = ret_str.ReadString(ret.I32);
                                //ret_str.Dispose();
                                //return "var " + autoRetResultName + "= " + "Cef3Binder.CopyStringAndDestroyNativeSide(ref " + retName + ");";
                                return "return Cef3Binder.CopyStringAndDestroyNativeSide(ref " + retName + ");";
                            case PrimitiveTypeKind.NaitveInt:
                                //return "var " + autoRetResultName + "= " + retName + ".I32;";
                                return "return " + retName + ".I32;";
                            case PrimitiveTypeKind.Int64:
                                //return "var " + autoRetResultName + "= " + retName + ".I64;";
                                return "return " + retName + ".I64;";
                            case PrimitiveTypeKind.UInt64:
                                //return "var " + autoRetResultName + "=  (ulong)" + retName + ".I64;";
                                return "return (ulong)" + retName + ".I64;";
                            case PrimitiveTypeKind.Double:
                                //return "var " + autoRetResultName + "=  " + retName + ".Num;";
                                return "return " + retName + ".Num;";
                            case PrimitiveTypeKind.Float:
                                //return "var " + autoRetResultName + "= (float)" + retName + ".Num;";
                                return "return (float)" + retName + ".Num;";
                            case PrimitiveTypeKind.Bool:
                                //return "var " + autoRetResultName + "=" + retName + ".I32 !=0;";
                                return "return " + retName + ".I32 !=0;";
                            case PrimitiveTypeKind.size_t:
                            case PrimitiveTypeKind.UInt32:
                                //return "var " + autoRetResultName + "= (uint)" + retName + ".I32;";
                                return "return (uint)" + retName + ".I32;";
                            case PrimitiveTypeKind.Int32:
                                //return "var " + autoRetResultName + "= " + retName + ".I32;";
                                return "return " + retName + ".I32;";
                        }
                    }
                    break;
            }
            throw new NotSupportedException();

        }


        protected static void AddComment(Token[] lineTokens, CodeStringBuilder builder)
        {
            CodeGenUtils.AddComment(lineTokens, builder);
        }
    }

    /// <summary>
    /// cef enum type transofmrer
    /// </summary>
    class CefEnumTx : CefTypeTx
    {
        string enum_base = "";
        public CefEnumTx(CodeTypeDeclaration typedecl)
            : base(typedecl)
        {

        }
        public override void GenerateCode(CefCodeGenOutput output)
        {
            //only CS 
            //check each field for proper enum base type 
            foreach (CodeFieldDeclaration field in this.OriginalDecl.GetFieldIter())
            {
                if (field.InitExpression != null)
                {
                    //cef specific
                    //some init expression need special treatment
                    string initExprString = field.InitExpression.ToString();

                    if (initExprString == "UINT_MAX")
                    {
                        enum_base = ":uint";
                        break;
                    }
                    else
                    {
                        initExprString = initExprString.ToLower();
                        if (initExprString.StartsWith("0x"))
                        {
                            uint uint1 = Convert.ToUInt32(initExprString.Substring(2), 16);
                            if (uint1 >= int.MaxValue)
                            {
                                enum_base = ":uint";
                                break;
                            }
                        }
                    }
                }
            }

            GenerateCsCode(output._csCode);
        }
        void GenerateCsCode(CodeStringBuilder stbuilder)
        {

            CodeStringBuilder codeBuilder = new CodeStringBuilder();
            CodeTypeDeclaration orgDecl = this.OriginalDecl;
            TypeTxInfo _typeTxInfo = orgDecl.TypeTxInfo;

            //
            AddComment(orgDecl.LineComments, codeBuilder);

            //for cef, if enum class end with flags_t 
            //we add FlagsAttribute to this enum type

            if (orgDecl.Name.EndsWith("flags_t"))
            {
                codeBuilder.AppendLine("[Flags]");
            }

            codeBuilder.AppendLine("public enum " + orgDecl.Name + enum_base + "{");
            //transform enum
            int i = 0;
            foreach (FieldTxInfo fieldTx in _typeTxInfo.fields)
            {

                if (i > 0)
                {
                    codeBuilder.AppendLine(",");
                }
                i++;
                CodeFieldDeclaration codeFieldDecl = fieldTx.fieldDecl;
                //
                AddComment(codeFieldDecl.LineComments, codeBuilder);
                //
                if (codeFieldDecl.InitExpression != null)
                {
                    string initExpr = codeFieldDecl.InitExpression.ToString();
                    //cef specific
                    if (initExpr == "UINT_MAX")
                    {
                        codeBuilder.Append(codeFieldDecl.Name + "=uint.MaxValue");
                    }
                    else
                    {
                        codeBuilder.Append(codeFieldDecl.Name + "=" + codeFieldDecl.InitExpression.ToString());
                    }
                }
                else
                {
                    codeBuilder.Append(codeFieldDecl.Name);
                }
            }
            codeBuilder.AppendLine("}");
            //
            stbuilder.Append(codeBuilder.ToString());
        }
    }


    /// <summary>
    /// tx for callback class
    /// </summary>
    class CefCallbackTx : CefTypeTx
    {
        TypeTxInfo _typeTxInfo;

        public CefCallbackTx(CodeTypeDeclaration typedecl)
            : base(typedecl)
        {
        }
        public override void GenerateCode(CefCodeGenOutput output)
        {
            CodeStringBuilder _cppHeaderExportFuncAuto = output._cppHeaderExportFuncAuto;

            CodeTypeDeclaration orgDecl = this.OriginalDecl;
            CodeTypeDeclaration implTypeDecl = this.ImplTypeDecl;

            GenerateCppCode(output._cppCode);
            GenerateCsCode(output._csCode);

            //-----------------------------------------------------------
            CppToCsMethodArgsClassGen cppMetArgClassGen = new CppToCsMethodArgsClassGen();
            //
            CodeStringBuilder cppArgClassStBuilder = new CodeStringBuilder();
            cppArgClassStBuilder.AppendLine("namespace " + orgDecl.Name + "Ext{");
            int j = _typeTxInfo.methods.Count;
            for (int i = 0; i < j; ++i)
            {
                MethodTxInfo met = _typeTxInfo.methods[i];
                cppMetArgClassGen.GenerateCppMethodArgsClass(met, cppArgClassStBuilder);
            }
            cppArgClassStBuilder.AppendLine("}");
            _cppHeaderExportFuncAuto.Append(cppArgClassStBuilder.ToString());

            //----------------------------------------------
            //InternalHeaderForExportFunc.h
            string namespaceName = orgDecl.Name + "Ext";
            var internalHeader = output._cppHeaderInternalForExportFuncAuto;
            internalHeader.AppendLine("namespace " + namespaceName);
            internalHeader.AppendLine("{");
            internalHeader.AppendLine("const int _typeName=" + "CefTypeName_" + orgDecl.Name + ";");
            internalHeader.AppendLine("}");
            //---------------------------------------------- 

        }
        void GenerateCppCode(CodeStringBuilder stbuilder)
        {

#if DEBUG
            _dbug_cpp_count++;
#endif
            //
            //create switch table for C#-interop
            //
            CodeTypeDeclaration orgDecl = this.OriginalDecl;
            CodeTypeDeclaration implTypeDecl = this.ImplTypeDecl;
            _typeTxInfo = orgDecl.TypeTxInfo;
            //-----------------------------------------------------------------------
            List<MethodTxInfo> onEventMethods = new List<MethodTxInfo>();

            int j = _typeTxInfo.methods.Count;
            int maxPar = 0;
            for (int i = 0; i < j; ++i)
            {
                MethodTxInfo metTx = _typeTxInfo.methods[i];
                if (metTx.metDecl.IsVirtual)
                {
                    //this method need a callback to .net side (.net-side event listener)
                    if (metTx.metDecl.Name.StartsWith("On"))
                    {
                        onEventMethods.Add(metTx);
                    }
                }
                metTx.CppMethodSwitchCaseName = orgDecl.Name + "_" + metTx.Name + "_" + (i + 1);
                if (metTx.pars.Count > maxPar)
                {
                    maxPar = metTx.pars.Count;
                }
            }

            MaxMethodParCount = maxPar;
            //----------
            CppHandleCsMethodRequestCodeGen cppHandleCsMetCodeGen2 = new CppHandleCsMethodRequestCodeGen();
            cppHandleCsMetCodeGen2.GenerateCppCode(
                this,
                orgDecl,
                ImplTypeDecl,
                this.UnderlyingCType,
                stbuilder
                );

            //----------
            if (onEventMethods.Count > 0)
            {
                //the callback need some method to call to C# side
                var eventListenerCodeGen = new CppEventListenerInstanceImplCodeGen();
                eventListenerCodeGen.GenerateCppImplClass(
                    this,
                    _typeTxInfo,
                    orgDecl,
                    onEventMethods,
                    stbuilder);
            }

        }

        void GenerateCsCode(CodeStringBuilder stbuilder)
        {

            CodeTypeDeclaration orgDecl = this.OriginalDecl;
            CodeTypeDeclaration implTypeDecl = this.ImplTypeDecl;

            CsCallToNativeCodeGen csCallToNativeCodeGen = new CsCallToNativeCodeGen();
            csCallToNativeCodeGen.GenerateCsCode(this, orgDecl, implTypeDecl, false, stbuilder);
        }

    }
    /// <summary>
    /// tx for instance element
    /// </summary>
    class CefInstanceElementTx : CefTypeTx
    {

        public CefInstanceElementTx(CodeTypeDeclaration typedecl)
            : base(typedecl)
        {

        }
        public override void GenerateCode(CefCodeGenOutput output)
        {


            GenerateCppCode(output._cppCode);
            GenerateCsCode(output._csCode);
        }
        void GenerateCppCode(CodeStringBuilder stbuilder)
        {

#if DEBUG
            _dbug_cpp_count++;
#endif
            //
            //create switch table for C#-interop
            //
            CodeTypeDeclaration orgDecl = this.OriginalDecl;
            CodeTypeDeclaration implTypeDecl = this.ImplTypeDecl;
            TypeTxInfo typeTxInfo;
            if (implTypeDecl.Name.Contains("CppToC"))
            {
                typeTxInfo = orgDecl.TypeTxInfo;
            }
            else
            {
                typeTxInfo = implTypeDecl.TypeTxInfo;
            }


            CppHandleCsMethodRequestCodeGen cppHandlerReqCodeGen = new CppHandleCsMethodRequestCodeGen();
            cppHandlerReqCodeGen.GenerateCppCode(this, orgDecl, implTypeDecl, this.UnderlyingCType, stbuilder);
            //
            if (cppHandlerReqCodeGen.callToDotNetMets.Count > 0)
            {
                CppInstanceImplCodeGen instanceImplCodeGen = new CppInstanceImplCodeGen();
                instanceImplCodeGen.GenerateCppImplClass(this,
                    typeTxInfo,
                    cppHandlerReqCodeGen.callToDotNetMets,
                    orgDecl,
                    stbuilder);
                //

            }

        }
        void GenerateCsCode(CodeStringBuilder stbuilder)
        {
            CodeTypeDeclaration orgDecl = this.OriginalDecl;
            CodeTypeDeclaration implTypeDecl = this.ImplTypeDecl;

            CodeGenUtils.AddComments(orgDecl, implTypeDecl);
            CsCallToNativeCodeGen callToNativeCs = new CsCallToNativeCodeGen();
            callToNativeCs.GenerateCsCode(this, orgDecl, implTypeDecl, true, stbuilder);
        }
    }

    /// <summary>
    /// tx for handler class
    /// </summary>
    class CefHandlerTx : CefTypeTx
    {
        TypeTxInfo _typeTxInfo;
        public CefHandlerTx(CodeTypeDeclaration typedecl)
            : base(typedecl)
        {

        }
        void GenerateCppImplMethodForNs(MethodTxInfo met, CodeStringBuilder stbuilder, bool useJsSlot)
        {
            CodeMethodDeclaration metDecl = met.metDecl;
            stbuilder.AppendLine("//gen! " + metDecl.ToString());
            //temp
            if (metDecl.ReturnType.ToString() == "FilterStatus")
            {
                stbuilder.Append(metDecl.ReturnType.ResolvedType + " " + metDecl.Name + "(");
            }
            else
            {
                stbuilder.Append(metDecl.ReturnType + " " + metDecl.Name + "(");
            }


            List<CodeMethodParameter> pars = metDecl.Parameters;

            //first par is managed callback
            stbuilder.Append("managed_callback mcallback");
            int j = pars.Count;
            for (int i = 0; i < j; ++i)
            {

                stbuilder.Append(",");
                CodeMethodParameter par = pars[i];

                if (par.IsConstPar)
                {
                    stbuilder.Append("const ");
                }
                //parameter type

                stbuilder.Append(par.ParameterType.ResolvedType.FullName + " ");
                stbuilder.Append(par.ParameterName);
            }
            stbuilder.AppendLine("){");
            //----------- 

            for (int i = 0; i < j; ++i)
            {
                MethodParameterTxInfo parTx = met.pars[i];
                parTx.ClearExtractCode();
                PrepareDataFromNativeToCs(parTx, "&vargs[" + (i + 1) + "]", parTx.Name, true);
            }
            //method body
            if (!useJsSlot)
            {
                stbuilder.AppendLine("if(mcallback){");
                string metArgsClassName = metDecl.Name + "Args";
                stbuilder.Append(metArgsClassName + " args1");
                //with ctors
                if (j == 0)
                {
                    stbuilder.AppendLine(";");
                }
                else
                {
                    stbuilder.Append("(");
                    for (int i = 0; i < j; ++i)
                    {
                        MethodParameterTxInfo par = met.pars[i];
                        if (i > 0) { stbuilder.Append(","); }
                        //temp
                        string parType = par.TypeSymbol.ToString();
                        if (parType.EndsWith("&"))
                        {
                            stbuilder.Append("&");
                        }
                        stbuilder.Append(par.Name);
                    }
                    stbuilder.AppendLine(");");
                }
                stbuilder.AppendLine("mcallback( (_typeName << 16) | " + met.CppMethodSwitchCaseName + ",&args1.arg);");
                stbuilder.AppendLine("}"); //if(this->mcallback){ 
            }
            else
            {
                stbuilder.AppendLine("if(mcallback){");
                //call to managed 
                stbuilder.AppendLine("MyMetArgsN args;");
                stbuilder.AppendLine("memset(&args, 0, sizeof(MyMetArgsN));");
                stbuilder.AppendLine("args.argCount=" + j + ";");
                int arrLen = j + 1;
                stbuilder.AppendLine("jsvalue vargs[" + arrLen + "];");
                stbuilder.AppendLine("memset(&vargs, 0, sizeof(jsvalue) * " + arrLen + ");");
                stbuilder.AppendLine("args.vargs=vargs;");
                PrepareCppMetArg(met.ReturnPlan, "vargs[0]");
                //
                for (int i = 0; i < j; ++i)
                {
                    MethodParameterTxInfo parTx = met.pars[i];
                    if (parTx.ArgPreExtractCode != null)
                    {
                        stbuilder.AppendLine(parTx.ArgPreExtractCode);
                    }
                }
                for (int i = 0; i < j; ++i)
                {
                    MethodParameterTxInfo parTx = met.pars[i];
                    stbuilder.AppendLine(parTx.ArgExtractCode);
                }
                //
                //call a method and get some result back 
                //
                stbuilder.AppendLine("mcallback( (_typeName << 16) | " + met.CppMethodSwitchCaseName + ",&args);");

                //post call
                for (int i = 0; i < j; ++i)
                {
                    MethodParameterTxInfo parTx = met.pars[i];
                    if (parTx.ArgPostExtractCode != null)
                    {
                        stbuilder.AppendLine(parTx.ArgPostExtractCode);
                    }
                }

                //temp fix, arg extract code 
                if (!met.ReturnPlan.IsVoid)
                {
                    stbuilder.AppendLine("return " + met.ReturnPlan.ArgExtractCode.Replace("->", ".") + ";");
                }
                //and return value
                stbuilder.AppendLine("}"); //if(this->mcallback){

                //-----------
            }

            //-------------------
            //default return if no managed callback
            if (!met.ReturnPlan.IsVoid)
            {
                string retTypeName = metDecl.ReturnType.ToString();
                if (retTypeName.StartsWith("CefRefPtr<"))
                {
                    stbuilder.Append("return nullptr;");
                }
                else
                {
                    switch (metDecl.ReturnType.ToString())
                    {
                        case "bool":
                            stbuilder.Append("return false;");
                            break;
                        case "FilterStatus": //TODO: revisit here
                            stbuilder.Append("return (FilterStatus)0;");
                            break;
                        case "ReturnValue":
                            stbuilder.Append("return (ReturnValue)0;");
                            break;
                        case "CefSize":
                            stbuilder.Append("throw new CefNotImplementException();");
                            break;
                        case "size_t":
                            stbuilder.Append("return 0;");
                            break;
                        case "int":
                            stbuilder.Append("return 0;");
                            break;
                        case "int64":
                            stbuilder.Append("return 0;");
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            stbuilder.AppendLine("}"); //method
        }

        void GenerateCppImplMethodDeclarationForNs(MethodTxInfo met, CodeStringBuilder stbuilder)
        {
            CodeMethodDeclaration metDecl = met.metDecl;
            stbuilder.AppendLine();
            stbuilder.AppendLine("//gen! " + metDecl.ToString());
            //temp
            if (metDecl.ReturnType.ToString() == "FilterStatus")
            {
                stbuilder.Append(metDecl.ReturnType.ResolvedType + " " + metDecl.Name + "(");
            }
            else
            {
                stbuilder.Append(metDecl.ReturnType + " " + metDecl.Name + "(");
            }
            List<CodeMethodParameter> pars = metDecl.Parameters;
            //first par is managed callback
            stbuilder.Append("managed_callback mcallback");
            int j = pars.Count;
            for (int i = 0; i < j; ++i)
            {

                stbuilder.Append(",");
                CodeMethodParameter par = pars[i];

                if (par.IsConstPar)
                {
                    stbuilder.Append("const ");
                }
                //parameter type

                stbuilder.Append(par.ParameterType.ResolvedType.FullName + " ");
                stbuilder.Append(par.ParameterName);
            }
            stbuilder.AppendLine(");");
        }

        void GenerateCppImplNamespace(CodeTypeDeclaration orgDecl,
            List<MethodTxInfo> callToDotNetMets,
            CodeStringBuilder stbuilder)
        {

            string namespaceName = orgDecl.Name + "Ext"; //namespace
            this.CppImplClassNameId = _typeTxInfo.CsInterOpTypeNameId;
            this.CppImplClassName = namespaceName;
            //----------------------------------------------
            //create a cpp namespace      
            stbuilder.Append("namespace " + namespaceName);
            stbuilder.AppendLine("{");


            int nn = callToDotNetMets.Count;
            for (int mm = 0; mm < nn; ++mm)
            {
                //implement on event notificationi
                MethodTxInfo met = callToDotNetMets[mm];
                met.CppMethodSwitchCaseName = namespaceName + "_" + met.Name + "_" + (mm + 1);
            }
            nn = callToDotNetMets.Count;
            for (int mm = 0; mm < nn; ++mm)
            {
                //implement on event notificationi
                MethodTxInfo met = callToDotNetMets[mm];
                //prepare data and call the callback
                GenerateCppImplMethodForNs(met, stbuilder, false);
            }
            stbuilder.AppendLine("}");
            //----------------------------------------------

            //----------------------------------------------
            //InternalHeaderForExportFunc.h
            _cppHeaderInternalForExportFuncAuto.AppendLine("namespace " + namespaceName);
            _cppHeaderInternalForExportFuncAuto.AppendLine("{");
            _cppHeaderInternalForExportFuncAuto.AppendLine("const int _typeName=" + "CefTypeName_" + orgDecl.Name + ";");
            for (int mm = 0; mm < nn; ++mm)
            {
                MethodTxInfo met = callToDotNetMets[mm];
                _cppHeaderInternalForExportFuncAuto.AppendLine("const int " + met.CppMethodSwitchCaseName + "=" + (mm + 1) + ";");
            }
            _cppHeaderInternalForExportFuncAuto.AppendLine("}");
            //----------------------------------------------
            //ExportFuncAuto.h
            _cppHeaderExportFuncAuto.AppendLine("namespace " + namespaceName);
            _cppHeaderExportFuncAuto.AppendLine("{");
            for (int mm = 0; mm < nn; ++mm)
            {
                //implement on event notificationi
                MethodTxInfo met = callToDotNetMets[mm];
                //prepare data and call the callback                 
                GenerateCppImplMethodDeclarationForNs(met, _cppHeaderExportFuncAuto);
            }
            _cppHeaderExportFuncAuto.AppendLine("}");

        }

        CodeStringBuilder _cppHeaderExportFuncAuto;
        CodeStringBuilder _cppHeaderInternalForExportFuncAuto;

        public override void GenerateCode(CefCodeGenOutput output)
        {
            _cppHeaderExportFuncAuto = output._cppHeaderExportFuncAuto;
            _cppHeaderInternalForExportFuncAuto = output._cppHeaderInternalForExportFuncAuto;

            GenerateCppCode(output._cppCode);
            GenerateCsCode(output._csCode);

        }
        void GenerateCppCode(CodeStringBuilder stbuilder)
        {
            CodeTypeDeclaration orgDecl = this.OriginalDecl;
            CodeTypeDeclaration implTypeDecl = this.ImplTypeDecl;
            CodeStringBuilder totalTypeMethod = new CodeStringBuilder();


            if (implTypeDecl.Name.Contains("CppToC"))
            {
                _typeTxInfo = orgDecl.TypeTxInfo;
            }
            else
            {
                _typeTxInfo = implTypeDecl.TypeTxInfo;
            }

            //-----------------------------------------------------------------------
            List<MethodTxInfo> callToDotNetMets = new List<MethodTxInfo>();
            CodeStringBuilder const_methodNames = new CodeStringBuilder();
            int maxPar = 0;
            int j = _typeTxInfo.methods.Count;

            for (int i = 0; i < j; ++i)
            {
                MethodTxInfo metTx = _typeTxInfo.methods[i];
                metTx.CppMethodSwitchCaseName = orgDecl.Name + "_" + metTx.Name + "_" + (i + 1);
                //-----------------
                CodeMethodDeclaration codeMethodDecl = metTx.metDecl;
                if (codeMethodDecl.IsAbstract || codeMethodDecl.IsVirtual)
                {
                    callToDotNetMets.Add(metTx);
                }
                //-----------------

                if (metTx.pars.Count > maxPar)
                {
                    maxPar = metTx.pars.Count;
                }
                const_methodNames.AppendLine("const int " + metTx.CppMethodSwitchCaseName + "=" + (i + 1) + ";");
            }
            totalTypeMethod.AppendLine(const_methodNames.ToString());
            //----------------------------------------------------------------------- 
            if (callToDotNetMets.Count > 0)
            {
                GenerateCppImplNamespace(orgDecl, callToDotNetMets, stbuilder);
            }

        }

        void GenerateCsCode(CodeStringBuilder stbuilder)
        {
            CodeTypeDeclaration orgDecl = this.OriginalDecl;
            CodeTypeDeclaration implTypeDecl = this.ImplTypeDecl;


            if (implTypeDecl.Name.Contains("CppToC"))
            {
                _typeTxInfo = orgDecl.TypeTxInfo;
            }
            else
            {
                _typeTxInfo = implTypeDecl.TypeTxInfo;
            }

            //-----------------------------------------------------------------------
            List<MethodTxInfo> callToDotNetMets = new List<MethodTxInfo>();

            int maxPar = 0;
            int j = _typeTxInfo.methods.Count;

            for (int i = 0; i < j; ++i)
            {
                MethodTxInfo metTx = _typeTxInfo.methods[i];
                //-----------------
                CodeMethodDeclaration codeMethodDecl = metTx.metDecl;
                if (codeMethodDecl.IsAbstract || codeMethodDecl.IsVirtual)
                {
                    callToDotNetMets.Add(metTx);
                }
                if (metTx.pars.Count > maxPar)
                {
                    maxPar = metTx.pars.Count;
                }
            }

            if (callToDotNetMets.Count > 0)
            {
                GenerateCsImplClass(orgDecl, callToDotNetMets, stbuilder);
            }

            CppToCsMethodArgsClassGen cppMetArgClassGen = new CppToCsMethodArgsClassGen();
            //------------------------------------------------------------------
            CodeStringBuilder cppArgClassStBuilder = new CodeStringBuilder();
            cppArgClassStBuilder.AppendLine("namespace " + orgDecl.Name + "Ext{");
            for (int i = 0; i < j; ++i)
            {
                MethodTxInfo met = _typeTxInfo.methods[i];
                cppMetArgClassGen.GenerateCppMethodArgsClass(met, cppArgClassStBuilder);
            }
            cppArgClassStBuilder.AppendLine("}");
            _cppHeaderExportFuncAuto.Append(cppArgClassStBuilder.ToString());

        }
        string GenerateCsMethodArgsClass_Native(MethodTxInfo met, CodeStringBuilder stbuilder)
        {
            //generate cs method pars
            CodeMethodDeclaration metDecl = (CodeMethodDeclaration)met.metDecl;
            List<CodeMethodParameter> pars = metDecl.Parameters;
            int j = pars.Count;
            //temp 
            string className = met.Name + "NativeArgs";
            stbuilder.AppendLine("[StructLayout(LayoutKind.Sequential)]");
            stbuilder.AppendLine("struct " + className + "{ "); //this is private struct with explicit layout
            stbuilder.AppendLine("public int argFlags;");
            //
            for (int i = 0; i < j; ++i)
            {
                //move this to method
                CodeMethodParameter par = pars[i];
                MethodParameterTxInfo parTx = met.pars[i];
                switch (parTx.Name)
                {
                    case "params":
                        parTx.Name = "_params";
                        break;
                    case "string":
                        parTx.Name = "_string";
                        break;
                    case "object":
                        parTx.Name = "_object";
                        break;
                    case "event":
                        parTx.Name = "_event";
                        break;
                }
                //
                stbuilder.Append("public ");

                string csParTypeName = GetCsRetName(parTx.TypeSymbol);
                string csSetterParTypeName = null;
                switch (csParTypeName)
                {
                    case "ref bool":
                        //provide both getter and setter method
                        //stbuilder.Append("bool");
                        parTx.ArgByRef = true;//temp
                        parTx.InnerTypeName = csSetterParTypeName = "bool";
                        break;
                    case "ref int":
                        //stbuilder.Append("int");
                        parTx.ArgByRef = true;//temp
                        parTx.InnerTypeName = csSetterParTypeName = "int";
                        break;
                    case "ref uint":
                        //stbuilder.Append("uint");
                        parTx.ArgByRef = true;//temp
                        parTx.InnerTypeName = csSetterParTypeName = "uint";
                        break;
                    default:
                        //stbuilder.Append(csParTypeName);
                        csSetterParTypeName = csParTypeName;
                        break;
                }

                //some cpp name can't be use in C#                 
                stbuilder.Append(" ");

                switch (csParTypeName)
                {
                    default:
                        {

                            if (csParTypeName.StartsWith("Cef"))
                            {
                                stbuilder.Append("IntPtr");
                            }
                            else if (csParTypeName.StartsWith("cef"))
                            {
                                stbuilder.Append(csParTypeName);
                            }
                            else
                            {
                                stbuilder.AppendLine(csParTypeName.ToString());
                                stbuilder.Append("IntPtr");
                            }
                        }
                        break;
                    case "IntPtr":
                        stbuilder.Append("IntPtr");
                        break;
                    case "List<object>":
                    case "List<string>":
                    case "List<CefCompositionUnderline>":
                        stbuilder.Append("IntPtr");
                        break;
                    case "CefValue":
                        stbuilder.Append("IntPtr");

                        break;
                    case "uint":
                        stbuilder.Append("uint");
                        break;
                    case "int":
                        stbuilder.Append("int");
                        break;
                    case "long":
                        stbuilder.Append("long");
                        break;
                    case "string":
                        stbuilder.Append("IntPtr");
                        break;
                    case "bool":
                        stbuilder.Append("bool");
                        break;
                    case "double":
                        stbuilder.Append("double");
                        break;
                    case "ref bool":
                        //provide both getter and setter method  
                        stbuilder.Append("double");
                        break;
                    case "ref int":
                        stbuilder.Append("int");
                        break;
                    case "ref uint":
                        stbuilder.Append("uint");
                        break;
                }
                stbuilder.Append(" ");
                stbuilder.Append(parTx.Name);
                stbuilder.AppendLine(";");
            }
            stbuilder.AppendLine("}"); //struct

            return className;
        }
        string GenerateCsMethodArgsClass_JsSlot(MethodTxInfo met, CodeStringBuilder stbuilder)
        {
            //generate cs method pars
            CodeMethodDeclaration metDecl = (CodeMethodDeclaration)met.metDecl;
            List<CodeMethodParameter> pars = metDecl.Parameters;
            int j = pars.Count;
            //temp 
            string className = met.Name + "Args";

            stbuilder.AppendLine("public struct " + className + "{ ");
            stbuilder.AppendLine("IntPtr nativePtr; //met arg native ptr");
            stbuilder.AppendLine("bool _isJsSlot;");

            stbuilder.AppendLine("internal " + className + "(IntPtr nativePtr){");

            stbuilder.AppendLine(@"int arg_flags;
                        this.nativePtr = MyMetArgs.GetNativeObjPtr(nativePtr,out arg_flags);
                        this._isJsSlot = ((arg_flags >> 18) & 1) ==0;
                        "
                        );

            stbuilder.AppendLine("}");

            int pos = 0;
            for (int i = 0; i < j; ++i)
            {
                pos = i + 1; //*** 

                //move this to method
                CodeMethodParameter par = pars[i];
                MethodParameterTxInfo parTx = met.pars[i];
                switch (parTx.Name)
                {
                    case "params":
                        parTx.Name = "_params";
                        break;
                    case "string":
                        parTx.Name = "_string";
                        break;
                    case "object":
                        parTx.Name = "_object";
                        break;
                    case "event":
                        parTx.Name = "_event";
                        break;
                }
                //
                stbuilder.Append("public ");

                string csParTypeName = GetCsRetName(parTx.TypeSymbol);
                string csSetterParTypeName = null;
                switch (csParTypeName)
                {
                    case "ref bool":
                        //provide both getter and setter method
                        stbuilder.Append("bool");
                        parTx.ArgByRef = true;//temp
                        parTx.InnerTypeName = csSetterParTypeName = "bool";
                        break;
                    case "ref int":
                        stbuilder.Append("int");
                        parTx.ArgByRef = true;//temp
                        parTx.InnerTypeName = csSetterParTypeName = "int";
                        break;
                    case "ref uint":
                        stbuilder.Append("uint");
                        parTx.ArgByRef = true;//temp
                        parTx.InnerTypeName = csSetterParTypeName = "uint";
                        break;
                    default:
                        stbuilder.Append(csParTypeName);
                        csSetterParTypeName = csParTypeName;
                        break;
                }

                //some cpp name can't be use in C#                 
                stbuilder.Append(" ");
                stbuilder.Append(parTx.Name);
                stbuilder.Append("()");
                stbuilder.AppendLine("{");

                string nativeArgClassName = met.Name + "NativeArgs";

                switch (csParTypeName)
                {
                    default:
                        {
                            stbuilder.AppendLine("unsafe{"); //open unsafe
                            stbuilder.Append("return _isJsSlot ? \r\n");
                            //is-js-slot= true
                            if (csParTypeName.StartsWith("Cef"))
                            {
                                stbuilder.Append("new " + csParTypeName + "(MyMetArgs.GetAsIntPtr(nativePtr," + pos + "))");
                                //if is not js-slot (this is native arg )
                                stbuilder.Append(":\r\n");
                                //cast native ptr to specific c-struct and get specific o
                                stbuilder.Append("new " + csParTypeName + "(((" + nativeArgClassName + "*)this.nativePtr)->" + parTx.Name + ");"); ;
                            }
                            else if (csParTypeName.StartsWith("cef"))
                            {
                                stbuilder.Append("(" + csParTypeName + ")" + "MyMetArgs.GetAsInt32(nativePtr," + pos + ")");
                                //if is not js-slot (this is native arg )
                                stbuilder.Append(":\r\n");
                                //cast native ptr to specific c-struct and get specific o
                                stbuilder.Append("(" + csParTypeName + ")" + "(((" + nativeArgClassName + "*)this.nativePtr)->" + parTx.Name + ");");
                            }
                            else
                            {
                                stbuilder.Append("throw new CefNotImplementedException();");
                            }
                            stbuilder.AppendLine("}"); //close unsafe context
                        }
                        break;
                    case "IntPtr":
                        stbuilder.Append("throw new CefNotImplementedException();");
                        break;
                    case "List<object>":
                    case "List<string>":
                    case "List<CefCompositionUnderline>":
                        stbuilder.Append("throw new CefNotImplementedException();");
                        break;
                    case "CefValue":
                        stbuilder.Append("throw new CefNotImplementedException();");
                        break;
                    case "uint":
                        {
                            stbuilder.AppendLine("unsafe{");
                            stbuilder.Append(" return _isJsSlot ? \r\n" + "MyMetArgs.GetAsUInt32(nativePtr," + pos + ") :\r\n");
                            stbuilder.Append("((" + nativeArgClassName + "*)this.nativePtr)->" + parTx.Name);
                            stbuilder.AppendLine(";");
                            stbuilder.AppendLine("}"); //close unsafe context
                        }
                        break;
                    case "int":
                        {
                            stbuilder.AppendLine("unsafe{");
                            stbuilder.Append("return _isJsSlot? \r\n" + "MyMetArgs.GetAsInt32(nativePtr," + pos + "):\r\n");
                            stbuilder.Append("((" + nativeArgClassName + "*)this.nativePtr)->" + parTx.Name);
                            stbuilder.AppendLine(";");
                            stbuilder.AppendLine("}"); //close unsafe context
                        }
                        break;
                    case "long":
                        {
                            stbuilder.AppendLine("unsafe{");
                            stbuilder.Append("return _isJsSlot ? \r\n" + "MyMetArgs.GetAsInt64(nativePtr," + pos + "):\r\n");
                            stbuilder.Append("((" + nativeArgClassName + "*)this.nativePtr)->" + parTx.Name);
                            stbuilder.AppendLine(";");
                            stbuilder.AppendLine("}"); //close unsafe context 
                        }
                        break;
                    case "string":
                        {
                            stbuilder.AppendLine("unsafe{");
                            stbuilder.Append("return _isJsSlot ?\r\n" + "MyMetArgs.GetAsString(nativePtr," + pos + "):\r\n");
                            stbuilder.Append("MyMetArgs.GetAsString(((" + nativeArgClassName + "*)this.nativePtr)->" + parTx.Name + ")");
                            stbuilder.AppendLine(";");
                            stbuilder.AppendLine("}"); //close unsafe context  
                        }
                        break;
                    case "bool":
                        {
                            stbuilder.AppendLine("unsafe{");
                            stbuilder.Append("return _isJsSlot?\r\n" + "MyMetArgs.GetAsBool(nativePtr," + pos + "):\r\n");
                            stbuilder.Append("((" + nativeArgClassName + "*)this.nativePtr)->" + parTx.Name);
                            stbuilder.AppendLine(";");
                            stbuilder.AppendLine("}"); //close unsafe context  
                        }
                        break;
                    case "double":
                        {
                            stbuilder.AppendLine("unsafe{");
                            stbuilder.Append("return _isJsSlot?\r\n" + "MyMetArgs.GetAsDouble(nativePtr," + pos + "):\r\n");
                            stbuilder.Append("((" + nativeArgClassName + "*)this.nativePtr)->" + parTx.Name);
                            stbuilder.AppendLine(";");
                            stbuilder.AppendLine("}"); //close unsafe context  
                        }
                        break;
                    case "ref bool":
                        //provide both getter and setter method
                        {
                            stbuilder.AppendLine("unsafe{");
                            stbuilder.Append("return " + "MyMetArgs.GetAsBool(nativePtr," + pos + ");");
                            stbuilder.Append("}");//close unsafe context  
                            stbuilder.Append("}"); //close method

                            //----------------------------------------------------------------------------------
                            //method
                            //generate setter part

                            stbuilder.AppendLine("public void " + parTx.Name + "(" + csSetterParTypeName + " value){");
                            stbuilder.AppendLine("MyMetArgs.SetBoolToAddress(nativePtr," + pos + ",value);");
                            stbuilder.AppendLine("}");
                            continue;
                        }

                    case "ref int":
                        {
                            stbuilder.Append("return " + "MyMetArgs.GetAsInt32(nativePtr," + pos + ");");
                            stbuilder.AppendLine("}");

                            //method
                            //generate setter part
                            stbuilder.AppendLine("public void " + parTx.Name + "(" + csSetterParTypeName + " value){");
                            stbuilder.AppendLine("MyMetArgs.SetInt32ToAddress(nativePtr," + pos + ",value);");
                            stbuilder.AppendLine("}");
                            continue;
                        }

                    case "ref uint":
                        {
                            stbuilder.Append("return " + "MyMetArgs.GetAsUInt32(nativePtr," + pos + ");");
                            stbuilder.AppendLine("}");
                            //method
                            //generate setter part
                            stbuilder.AppendLine("public void " + parTx.Name + "(" + csSetterParTypeName + " value){");
                            stbuilder.AppendLine("MyMetArgs.SetUInt32ToAddress(nativePtr," + pos + ",value);");
                            stbuilder.AppendLine("}");
                            continue;
                        }

                }
                stbuilder.AppendLine("}"); //method
            }
            stbuilder.AppendLine("}"); //struct

            return className;
        }

        void PrepareCsMetPars(MethodTxInfo met)
        {
            int j = met.pars.Count;
            for (int i = 0; i < j; ++i)
            {
                MethodParameterTxInfo parTx = met.pars[i];
                switch (parTx.Name)
                {
                    case "params":
                        {
                            parTx.Name = "_params";
                        }
                        break;
                    case "string":
                        {
                            parTx.Name = "_string";
                        }
                        break;
                    case "object":
                        {
                            parTx.Name = "_object";
                        }
                        break;
                    case "event":
                        {
                            parTx.Name = "_event";
                        }
                        break;
                }
            }

        }
        void GenerateCsExpandedArgsMethodForInterface(MethodTxInfo met, CodeStringBuilder stbuilder)
        {
            CodeMethodDeclaration metDecl = (CodeMethodDeclaration)met.metDecl;

            //temp             
            stbuilder.Append(GetCsRetName(met.ReturnPlan.TypeSymbol));
            stbuilder.Append(" ");
            stbuilder.Append(met.Name);
            stbuilder.Append("(");

            List<CodeMethodParameter> pars = metDecl.Parameters;
            int j = pars.Count;
            for (int i = 0; i < j; ++i)
            {
                if (i > 0)
                {
                    stbuilder.Append(",");
                }

                MethodParameterTxInfo parTx = met.pars[i];
                string parTypeName = GetCsRetName(parTx.TypeSymbol);
                stbuilder.Append(parTypeName);
                //some cpp name can't be use in C#
                stbuilder.Append(" ");
                stbuilder.Append(parTx.Name);
            }
            stbuilder.AppendLine(");");
        }

        void GenerateCsSingleArgMethodImplForI1(string argClassName, MethodTxInfo met, CodeStringBuilder stbuilder)
        {
            CodeMethodDeclaration metDecl = (CodeMethodDeclaration)met.metDecl;
            //temp 
            List<MethodParameterTxInfo> pars = met.pars;
            stbuilder.Append("public static void");
            stbuilder.Append(" ");
            stbuilder.Append(met.Name);
            stbuilder.Append("(I1 i1,");
            stbuilder.Append(argClassName + " args");
            stbuilder.AppendLine("){");
            GenerateExpandMethodContent(met, stbuilder);
            stbuilder.AppendLine("}"); //method
        }
        void GenerateCsSingleArgMethodImplForInterface(string argClassName, MethodTxInfo met, CodeStringBuilder stbuilder)
        {
            CodeMethodDeclaration metDecl = (CodeMethodDeclaration)met.metDecl;
            //temp 
            List<MethodParameterTxInfo> pars = met.pars;

            stbuilder.Append("void");
            stbuilder.Append(" ");

            stbuilder.Append(met.Name);
            stbuilder.Append("(");
            stbuilder.Append(argClassName + " args");
            stbuilder.AppendLine(");");
        }
        void GenerateCsImplClass(CodeTypeDeclaration orgDecl, List<MethodTxInfo> callToDotNetMets, CodeStringBuilder stbuilder)
        {
            int nn = callToDotNetMets.Count;
            //create interface for this handler
            //we provide 2 interfaces
            //1. singles arg interface
            //2. expanded args interface

            string className = orgDecl.Name;


            //create a cpp class              
            stbuilder.Append("public struct " + className);
            stbuilder.AppendLine("{");
            stbuilder.AppendLine("public const int _typeNAME=" + orgDecl.TypeTxInfo.CsInterOpTypeNameId + ";");


            for (int mm = 0; mm < nn; ++mm)
            {
                //implement on event notificationi
                MethodTxInfo met = callToDotNetMets[mm];
                PrepareCsMetPars(met);
                stbuilder.AppendLine("const int " + met.CppMethodSwitchCaseName + "= " + (mm + 1) + ";");
            }


            for (int mm = 0; mm < nn; ++mm)
            {
                //implement on event notificationi
                MethodTxInfo met = callToDotNetMets[mm];
                //prepare data and call the callback                
                stbuilder.AppendLine("//gen! " + met.metDecl.ToString());
                //
                //GenerateCsExpandedArgsMethodImpl(met, stbuilder);
                string argClassName = GenerateCsMethodArgsClass_JsSlot(met, stbuilder);
                CodeStringBuilder st2 = new CodeStringBuilder();
                GenerateCsMethodArgsClass_Native(met, st2);

                stbuilder.Append(st2.ToString());
                met.CsArgClassName = argClassName;
                //GenerateCsSingleArgMethodImpl(argClassName, met, stbuilder);                  
            }

            //------------------------------            
            stbuilder.Append("public interface I0");
            stbuilder.AppendLine("{");
            for (int mm = 0; mm < nn; ++mm)
            {
                //implement on event notificationi
                MethodTxInfo met = callToDotNetMets[mm];
                GenerateCsSingleArgMethodImplForInterface(met.CsArgClassName, met, stbuilder);
            }
            stbuilder.AppendLine("}");
            //-----------------

            stbuilder.Append("public interface I1");
            stbuilder.AppendLine("{");
            for (int mm = 0; mm < nn; ++mm)
            {
                //implement on event notificationi
                MethodTxInfo met = callToDotNetMets[mm];
                GenerateCsExpandedArgsMethodForInterface(met, stbuilder);
            }
            stbuilder.AppendLine("}");
            //-----------------


            stbuilder.AppendLine("public static void HandleNativeReq(I0 i0, I1 i1, int met_id, IntPtr nativeArgPtr)");
            stbuilder.AppendLine("{");
            stbuilder.AppendLine("int met_name = met_id & 0xffff;");
            stbuilder.AppendLine("switch (met_name){");
            //
            for (int mm = 0; mm < nn; ++mm)
            {
                //implement on event notificationi
                MethodTxInfo met = callToDotNetMets[mm];
                stbuilder.AppendLine("case " + met.CppMethodSwitchCaseName + ":{");
                stbuilder.AppendLine("var args=new " + met.CsArgClassName + "(nativeArgPtr);");
                //i0
                stbuilder.AppendLine("if(i0 != null){");
                stbuilder.AppendLine("i0." + met.Name + "(args);");
                stbuilder.AppendLine("}");
                //i1 expand interface
                stbuilder.AppendLine("if(i1 != null){");
                GenerateExpandMethodContent(met, stbuilder);
                stbuilder.AppendLine("}");
                stbuilder.AppendLine("}break;");//case 
            }

            stbuilder.AppendLine("}"); //end switch
            stbuilder.AppendLine("}"); //end method

            //-----------------
            //expansion version for i1

            for (int mm = 0; mm < nn; ++mm)
            {
                //implement on event notificationi
                MethodTxInfo met = callToDotNetMets[mm];
                GenerateCsSingleArgMethodImplForI1(met.CsArgClassName, met, stbuilder);
            }


            stbuilder.AppendLine("}"); //end class
        }
        void GenerateExpandMethodContent(MethodTxInfo met, CodeStringBuilder stbuilder)
        {

            //temp 
            List<MethodParameterTxInfo> pars = met.pars;
            //call 
            stbuilder.AppendLine("//expand args");
            int j = pars.Count;
            if (j > 0)
            {
                //arg expansion   
                for (int i = 0; i < j; ++i)
                {
                    MethodParameterTxInfo par = pars[i];
                    if (par.ArgByRef)
                    {
                        stbuilder.Append(par.InnerTypeName + " " + par.Name);
                        //with default value
                        if (par.InnerTypeName == "bool")
                        {
                            stbuilder.AppendLine("=false;");
                        }
                        else
                        {
                            stbuilder.AppendLine("=0;");
                        }
                    }
                }
            }
            //-------
            stbuilder.Append("i1.");//instant name
            stbuilder.Append(met.Name);
            stbuilder.Append("(");
            if (j > 0)
            {

                for (int i = 0; i < j; ++i)
                {
                    if (i > 0)
                    {
                        stbuilder.Append(",\r\n");
                    }
                    MethodParameterTxInfo par = pars[i];
                    if (par.ArgByRef)
                    {
                        stbuilder.Append("ref " + par.Name);
                    }
                    else
                    {
                        stbuilder.Append("args." + par.Name + "()");
                    }
                }
            }
            stbuilder.AppendLine(");");
            if (j > 0)
            {
                for (int i = 0; i < j; ++i)
                {
                    MethodParameterTxInfo par = pars[i];
                    if (par.ArgByRef)
                    {
                        stbuilder.AppendLine("args." + par.Name + "(" + par.Name + ");");
                    }
                }
            }
        }

    }


}