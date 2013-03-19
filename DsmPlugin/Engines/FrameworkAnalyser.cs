using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Tcdev.Dsm.Model;
using System.IO;
using Tcdev.Outil;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace Tcdev.Dsm.Engine
{
    /// <summary>
    /// One type of Analyser engine - one which uses the Reflector API to analyse the call _matrix
    /// betwen types in the Assemblies made known to it
    /// </summary>
    internal class FrameworkAnalyser : IAnalyser
    {
        private IList _assemblies;

        /*
         * Map for list of Types by internal DSM module
         */
        Dictionary< Guid, Tcdev.Dsm.Model.Module> _modules;
        IList< Type >                             _typeList;
        private Tcdev.Dsm.Model.DsmModel          _model;
        private static Logger                     _log;
        private DsmOptions                        _options;

        /*
         * Internal maps for determining whether opcodes are coded in one or two bytes
         */
        IDictionary<Byte, OpCode> _OneByteOpCodes = new Dictionary<Byte, OpCode>();
        IDictionary<Byte, OpCode> _TwoByteOpCodes = new Dictionary<Byte, OpCode>();

        //-------------------------------------------------------------------------------------------------
        public FrameworkAnalyser()
        {
            InitialiseOpCodeResources();

            _log = new Logger("log.txt");
            _log.Trace("FRAMEWORK ANALYSER : New Analysis : " + DateTime.Now);

            _modules    = new Dictionary<Guid, Tcdev.Dsm.Model.Module>();
            _typeList   = new List<Type>();
            _assemblies = new ArrayList();
            _options    = new DsmOptions();
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get or set the analysis options
        /// </summary>
        public DsmOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Set the model structure that the analyser should fill in
        /// </summary>
        public DsmModel Model
        {
            set { _model = value; }
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// This engine expects an assembly of type Target
        /// </summary>
        /// <param name="assembly"></param>
        public void IncludeAssembly( Target assembly )
        {
            _assemblies.Add(assembly);
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Identifiy the individual Types that can be analysed
        /// </summary>
        public void LoadTypes()
        {
            foreach( Target target in _assemblies )
            {
                _log.Trace("Reading Assembly: " + target.FullPath);

                try
                {
                    Assembly assembly = target.AssemblyObject as Assembly;

                    foreach (System.Reflection.Module module in assembly.GetModules() )
                    {
                        Type[] moduleTypes = null;
                        try
                        {
                            moduleTypes = module.GetTypes();
                        } 
                        catch( ReflectionTypeLoadException rtle )
                        {
                            // in case some types cannot be found we just carry on with those that are
                           _log.Trace( rtle.ToString() );
                            moduleTypes = rtle.Types; 
                        }
                        
                        foreach( Type typeDecl in moduleTypes )
                        {
                            if ( typeDecl != null ) // may be null if failed to load the type properly
                            {
                                LoadType( target, typeDecl );
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private void LoadType( Target target, Type typeDecl )
        {
            if ( !ExcludeType( typeDecl ) )
            {
                _log.Trace( "Type found: " + typeDecl.ToString() );

                if ( !_modules.ContainsKey( typeDecl.GUID ) )
                {
                    Tcdev.Dsm.Model.Module newModule = _model.CreateModule(
                                            typeDecl.Name, typeDecl.Namespace, target.FullPath, false );
                    _modules.Add(typeDecl.GUID, newModule );

                    _typeList.Add( typeDecl );
                }

                // Nested classes are named OuterClassName.InnerClassName in the namespace of 
                // the outer class

                try
                {
                    foreach ( Type nestedType in typeDecl.GetNestedTypes() )
                    {
                        if ( !_modules.ContainsKey( nestedType.GUID ) )
                        {
                            _log.Trace( " Nested type found: " + nestedType.ToString() );

                            _modules.Add(
                                nestedType.GUID,
                                _model.CreateModule(
                                    typeDecl.Name + "." + nestedType.Name,
                                    typeDecl.Namespace,
                                    target.FullPath,
                                    true )
                            );

                            _typeList.Add( nestedType );

                            // TODO classes nested within nested classes are not processed currently !
                        }
                    }
                }
                catch
                {
                    _log.Trace( "Resolution exception of GetNestedTypes" );
                }
            }
        }
        
        //-------------------------------------------------------------------------------------------------
        public void AnalyseRelations()
        {
            _log.Trace("Starting Analysis ...");

            foreach( Type typeDecl in _typeList )
            {
                AnalyseType(typeDecl);
            }

            _log.Trace("AnalyseRelations completed : " + DateTime.Now);
        }

        //-----------------------------------------------------------------------------------------
        
        private void AnalyseType(Type typeDecl )
        {
            MarkInterfaces(typeDecl);
            MarkBaseType(typeDecl);
            MarkFields(typeDecl);
            MarkProperties(typeDecl);
            AnalyseTypeConstructors( typeDecl );
            AnalyseTypeMethods(typeDecl);
            
            /*
             * Attributes - Currently not supported
             *
             * Events declared by class itself are not analysed
             * 
             * Generic Arguments - not needed just describes the place holders for generic types
             * */

            /*
             * TODO Generic Module  ???
             * */  
        }

        //-----------------------------------------------------------------------------------------
        void AnalyseTypeConstructors( Type typeDecl )
        {
            _log.Trace( typeDecl.Name + " Constructors ..." );
            
            try
            {
                ConstructorInfo[] constructors = typeDecl.GetConstructors( BindingFlags.DeclaredOnly | 
                                                                            BindingFlags.Instance    | 
                                                                            BindingFlags.NonPublic   | 
                                                                            BindingFlags.Public      |
                                                                            BindingFlags.Static );
                foreach( ConstructorInfo constructor in constructors )
                {
                    _log.Trace(constructor.Name );
                    
                    AnalyseConstructorBody( typeDecl, constructor );
                    MarkConstructorParameters( typeDecl, constructor );
                }
            }
            catch
            {
                _log.Trace( "Resolution exception in AnalyseTypeConstructors" );
            }
        }
        //-----------------------------------------------------------------------------------------
        void AnalyseConstructorBody( Type typeDecl, ConstructorInfo constructor )
        {
            try
            {
                MethodBody body = constructor.GetMethodBody();
                if ( body != null )
                {
                    MarkLocalVariables( typeDecl, body );
                    
                    // TODO error with constructor.GetGenericArguments see MSDN reflection does not support them???
                    AnalyseMethodOpCodes( typeDecl, body, null, constructor.Module );
                }
                else
                {
                    _log.Trace( "Abstract constructor Declaration?" );
                }
                  
            }
            catch 
            {
                _log.Trace( "Resolution exception in AnalyseConstructorBody" );
            }
        }


        //-----------------------------------------------------------------------------------------

        void AnalyseTypeMethods(Type typeDecl)
        {
            _log.Trace( typeDecl.Name + " Methods ...");
            
            try
            {
                MethodInfo[] methods = typeDecl.GetMethods(  BindingFlags.DeclaredOnly | 
                                                             BindingFlags.Instance   | 
                                                             BindingFlags.NonPublic  | 
                                                             BindingFlags.Public     | 
                                                             BindingFlags.Static );
                                                             
                foreach (MethodInfo method in  methods)     
                {
                    _log.Trace(method.Name);

                    AnalyseMethodBody(typeDecl, method);
                    MarkGenericMethodParameters(typeDecl, method);
                    MarkMethodParameters(typeDecl, method);
                    MarkMethodReturnType(typeDecl, method);
                    
                    /*
                     * Generic Method TODO not supported by reflection API ?
                     * */
                }   
            }
            catch
            {
                _log.Trace( "Resolution Exception in AnalyseTypeMethods" );
            }
        }

        //-----------------------------------------------------------------------------------------

        private void MarkMethodReturnType (Type typeDecl, MethodInfo method)
        {
            _log.Trace("Return Type " + method.ReturnType);

            try
            {
                Type dec = method.ReturnType;

                MarkRelation(dec, typeDecl);
            }
            catch
            {
                _log.Trace( "Resolution exception MarkMethodReturnType" );
            }
        }

        //-----------------------------------------------------------------------------------------

        private void MarkMethodParameters(Type typeDecl, MethodInfo method)
        {
            _log.Trace("Parameters ...");
            
            try
            {
                ParameterInfo[] parameters = method.GetParameters();
                foreach (ParameterInfo paramDecl in parameters )
                {
                    _log.Trace(paramDecl.Name);

                    Type it = paramDecl.ParameterType;

                    MarkRelation(it, typeDecl);
                }
            }
            catch
            {
                _log.Trace( "Resolution exception in MarkMethodParameters" );
            }
        }
        //-----------------------------------------------------------------------------------------

        private void MarkConstructorParameters( Type typeDecl, ConstructorInfo constructor )
        {
            _log.Trace( "Parameters ..." );
            
            try
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                foreach ( ParameterInfo paramDecl in parameters )
                {
                    _log.Trace( paramDecl.Name );

                    Type it = paramDecl.ParameterType;

                    MarkRelation( it, typeDecl );
                }
            }
            catch
            {
                _log.Trace( "Resolution exception in MarkConstructorParameters" );
            }
        }

        //-----------------------------------------------------------------------------------------

        private void MarkGenericMethodParameters(Type typeDecl, MethodInfo method)
        {
            _log.Trace("Generic Arguments of method ...");
            
            try
            {
                Type[] arguments = method.GetGenericArguments();
                foreach (Type genericArgument in arguments)
                {
                    _log.Trace("Method.Generic argument: " + genericArgument.ToString());

                    Type gadec = genericArgument.GetType();
                    MarkRelation(gadec, typeDecl);
                }
            }
            catch
            {
                _log.Trace( "Resolution exception in MarkGenericMethodParametres" );
            }
        }

        //-----------------------------------------------------------------------------------------

        private void MarkGenericConstructorParameters( Type typeDecl, ConstructorInfo constructor )
        {
            // GetGenericArguments not supported for ConstructorInfo !!!
            _log.Trace( "Generic Arguments of method ..." );
            
            try
            {
                Type[] arguments = constructor.GetGenericArguments();

                foreach ( Type genericArgument in arguments )
                {
                    _log.Trace( "Method.Generic argument: " + genericArgument.ToString() );

                    Type gadec = genericArgument.GetType();
                    MarkRelation( gadec, typeDecl );
                }
            }
            catch
            {
                _log.Trace("Resolution exception in MarkGenericConstructorParameters" );
            }
        }

        //-----------------------------------------------------------------------------------------

        private void MarkProperties(Type typeDecl)
        {
            _log.Trace("Properties ...");
            try
            {
                PropertyInfo[] properties = typeDecl.GetProperties();
                foreach (PropertyInfo propertyDecl in  properties )
                {
                    _log.Trace("Property:  " + propertyDecl.Name);
                    Type pdec = propertyDecl.PropertyType;
                    MarkRelation(pdec, typeDecl);
                }
            }
            catch
            {
                _log.Trace( "ResolutionException in MarkProperties" );
            } 
        }

        //-----------------------------------------------------------------------------------------

        private void MarkFields(Type typeDecl)
        {
            try
            {
                FieldInfo[] fields = typeDecl.GetFields();
                foreach (FieldInfo fieldDecl in fields )
                {
                    _log.Trace("Fields : " + fieldDecl.Name);
                    
                    Type fdec = fieldDecl.FieldType;

                    MarkRelation(fdec, typeDecl);
                }
            }
            catch
            {
                _log.Trace( "Resolution exception in MarkFields" );
            }
        }

        //-----------------------------------------------------------------------------------------

        private void MarkBaseType(Type typeDecl)
        {
            _log.Trace("Base Types ...");
            if (typeDecl.BaseType != null)
            {
                MarkRelation(typeDecl.BaseType, typeDecl);
            }
        }

        //-----------------------------------------------------------------------------------------
        private void MarkInterfaces(Type typeDecl)
        {
            _log.Trace("Interfaces ...");
            
            try
            {
                Type[] interfaces = typeDecl.GetInterfaces();
                foreach ( Type interf in interfaces )
                {
                    MarkRelation( interf, typeDecl );
                }
            }
            catch
            {
                _log.Trace("Resolution exception in MarkInterfaces" );
            }
            
        }

        //-------------------------------------------------------------------------------------------------
        void AnalyseMethodBody(Type typeDecl, MethodInfo method)
        {
            _log.Trace("Method Body ..." + method.Name);

            try
            {
                MethodBody body = method.GetMethodBody();

                if ( body != null)
                {
                    MarkLocalVariables(typeDecl, body);

                    AnalyseMethodOpCodes(typeDecl, body, method.GetGenericArguments(),  method.Module );
                }
                else
                {
                    _log.Trace("Abstract Method Declaration?");
                }
            } 
            catch
            {
                _log.Trace( "Resolution error in GetMethodBody" );
            }
        }

        //-----------------------------------------------------------------------------------------
        
        void AnalyseMethodOpCodes(Type typeDecl, MethodBody body, Type[] genericArguments, System.Reflection.Module module )
        {
            Int32 index = 0;
            Byte[] il = body.GetILAsByteArray();

            while (index < il.Length )
            {
                Byte b = ReadByte(il, ref index);
                OpCode opCode = OpCodes.Nop;

                if (b == 0xfe)
                {
                    Byte b2 = ReadByte(il, ref index);
                    opCode = _TwoByteOpCodes[b2];

                }
                else
                {
                    opCode = _OneByteOpCodes[b];
                }

                _log.Trace(String.Format("{0:x} ", opCode));

                Int32 token = 0x0;

                switch (opCode.OperandType)
                {
                    case OperandType.ShortInlineBrTarget: // 8 bit signed integer
                        ReadByte(il, ref index );
                        break;

                    case OperandType.ShortInlineVar: // 8 bit integer						
                    case OperandType.ShortInlineI: // 8bit integer
                        ReadByte( il, ref index);
                        break;

                    case OperandType.InlineVar: // 16 bit integer ordinal
                        ReadUInt16( il, ref index);
                        break;

                    case OperandType.InlineBrTarget: // 32 bit integer	
                    case OperandType.InlineI: // 32 bit integer
                        ReadInt32( il, ref index);
                        break;

                    case OperandType.InlineI8: // 64 bit integer
                        ReadInt64( il, ref index);
                        break;

                    case OperandType.InlineMethod: // 32 bit meta data token
                        token = ReadInt32( il, ref index );

                        try
                        {
                            MethodBase method = module.ResolveMethod(
                                token, typeDecl.GetGenericArguments(),genericArguments);
                            
                            MarkRelation( method.DeclaringType, typeDecl);
                        }
                        catch
                        {
                            _log.Trace( "Resolution exception on InlineMethod in AnalyseOpCodes" );
                        }

                        break;

                    case OperandType.InlineField: // 32 bit meta data token
                        token = ReadInt32(il, ref index);
                        _log.Trace( "*** Inline Field Ignored ****");
    
                        break;

                    case OperandType.InlineType: // 32 bit meta data token
                        token = ReadInt32(il, ref index);
                        try
                        {
                            Type inlineType = module.ResolveType(token, typeDecl.GetGenericArguments(), genericArguments);
                            MarkRelation( inlineType, typeDecl);
                        }
                        catch
                        {
                            _log.Trace( "Resolution error of InlineType in AnalyseOpCodes" );
                        }
                        break;

                    case OperandType.InlineNone: //Nop
                        break;

                    case OperandType.InlineR: // 64bit IEEE float
                        ReadDouble( il, ref index );
                        break;

                    case OperandType.InlineSig: // 32 bit metadata signature token	
                        token = ReadInt32( il, ref index );
                        _log.Trace("Inline Sig NOT PROCESSED size: " + module.ResolveSignature(token).Length);
                        break;

                    case OperandType.InlineString: // 32 bit meta data string
                        ReadInt32( il, ref index);
                        break;

                    case OperandType.InlineSwitch: // 32 bit integer argument to switch
                        Int32 nb = ReadInt32( il, ref index );
                        for (Int32 i = 0; i < nb; i++)
                        {
                            ReadInt32( il, ref index);
                        }
                        break;

                    case OperandType.InlineTok: // inline field, method or type reference token
                        token = ReadInt32( il, ref index );
                        
                        Type inlineToken = module.ResolveType( token, typeDecl.GetGenericArguments(), genericArguments );
                        MarkRelation( inlineToken, typeDecl );
                        //type = _module.ResolveType( token );
                        _log.Trace( " INLINE_TOK DON'T KNOW HOW TO RESOLVE ");
                        break;

                    case OperandType.ShortInlineR: // 32bit IEEE float
                        ReadSingle( il, ref index);
                        break;

                    case OperandType.InlinePhi: //reserved not assembly not valid
                        break;
                    default:
                        throw new ApplicationException(
                            String.Format("Unknown operand type found: {0:x}", opCode));
                }
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// TODO currently unused (helpful for resolving INLINE_TOK ???)
        /// </summary>
        /// <param name="module"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Type ResolveField(System.Reflection.Module module, int token)
        {
            Type fieldType = null;
            try
            {
                fieldType = module.ResolveField(token).FieldType;
            }
            catch (Exception e)
            {
                _log.Trace("ResolveFieldType: " + e.ToString());
            }

            return fieldType;
        }

        //-----------------------------------------------------------------------------------------
        private void MarkLocalVariables(Type typeDecl, MethodBody body)
        {
            _log.Trace("Local variables ...");
            
            try
            {
                IList<LocalVariableInfo> variables  = body.LocalVariables;
                foreach (LocalVariableInfo variable in variables )
                {
                    Type vdecl = variable.LocalType;
                    MarkRelation(vdecl, typeDecl);
                }
            }
            catch
            {
                _log.Trace( "Resolution exception in MarkLocalVariables" );
            }
            
        }

        //-------------------------------------------------------------------------------------------------
        private void MarkRelation(Type providerType, Type consumerType)
        {
            _log.Trace("Marking Relation");
            
            if (providerType == null || consumerType == null)
            {
                _log.Trace("At least one type has not been resolved - ignoring relation");
            }
            else
            {
                Tcdev.Dsm.Model.Module consumer;

                Tcdev.Dsm.Model.Module provider;
                if (_options.HideNestedClasses)
                {
                    providerType = NestedParentHelper(providerType);
                    consumerType = NestedParentHelper(consumerType);
                }

                _log.Trace(providerType.ToString() + " ---> " + consumerType.ToString());

                if (_modules.TryGetValue(consumerType.GUID, out consumer) &&
                    _modules.TryGetValue(providerType.GUID, out provider))
                {
                    _log.Trace("Relation found: " + providerType.Name);
                    provider.AddRelation(consumer, 1);
                }
                else
                {
                    _log.Trace("Relation NOT FOUND");
                }
                _log.Trace("-----------------------------------------------------");
            }
        }

        //-------------------------------------------------------------------------------------------------

        Type NestedParentHelper(Type typeDecl)
        {
            Type result = typeDecl;  //

            if (typeDecl.DeclaringType != null)
            {
                result = typeDecl.DeclaringType;
            }
            
            return result;

        }

        //-------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            if (_log != null) _log.Dispose();
        }
        
        //-------------------------------------------------------------------------------------------------

        bool ExcludeType( Type typeDecl )
        {
            bool exclude = false;

            // TODO private implementation details
            
            try
            {
                if ( _options.ExcludeGlobalNamespace && 
                    ( typeDecl.Namespace == null || typeDecl.Namespace.Length == 0 ) )
                {
                    exclude = true;
                }
                else if ( _options.ExcludeCompilerNamespaces &&
                          ( typeDecl.Namespace.Equals( "<CppImplementationDetails>" ) ||
                            typeDecl.Namespace.Equals( "<CrtImplementationDetails>")  ) )
                {
                    exclude = true;
                }
            }
            catch(Exception err )
            {
                // Using the Reflection API for loading assemblies can result in exceptions when trying to access
                // the Namespace property.  We can ignore these assuming that since the assembly has not been
                // preloaded we are not interested in its analysis
                _log.Trace("Reflection error : " + err.ToString());
                exclude = true;
            }
            return exclude;
        }


        //-------------------------------------------------------------------------------------------------

        void InitialiseOpCodeResources()
        {
            try
            {
                FieldInfo[] fields = typeof( OpCodes ).GetFields( BindingFlags.DeclaredOnly |
                                                                    BindingFlags.Instance   |
                                                                    BindingFlags.NonPublic  |
                                                                    BindingFlags.Public     |
                                                                    BindingFlags.Static );
                foreach (FieldInfo fi in fields )
                {
                    OpCode opCode = (OpCode)(fi.GetValue(null));
                    Int16 val = (opCode.Value);

                    if ((val & 0xff00) == 0xfe00)
                    {
                        Byte b;
                        unchecked
                        {
                            b = (Byte)(val - 0xfe00);
                        }

                        _TwoByteOpCodes.Add(b, opCode);
                    }
                    else
                    {
                        Byte b = (Byte)val;
                        _OneByteOpCodes.Add(b, opCode);
                    }
                } 
            }
            catch
            {
                _log.Trace("Resolution error in InitialiseOpCodeResources" );
            }

         }   

        //------------------------------------------------------------------------------------------

        #region Field Readers for IL Code
        Byte PeakByte( Byte[] bytes, ref Int32 index) 
        { 
            return (Byte)bytes[index];
        }
        //------------------------------------------------------------------------------------------
        Byte ReadByte(Byte[] bytes, ref Int32 index) 
        {
            return (Byte)bytes[index++]; 
        }
        //------------------------------------------------------------------------------------------
        SByte ReadSByte(Byte[] bytes, ref Int32 index) 
        { 
            return (SByte)ReadByte( bytes, ref index); 
        }
        //------------------------------------------------------------------------------------------

        UInt16 ReadUInt16(Byte[] bytes, ref Int32 index) 
        { 
            index += 2; 
            return BitConverter.ToUInt16( bytes, index - 2); 
        }
        //------------------------------------------------------------------------------------------

        UInt32 ReadUInt32(Byte[] bytes, ref Int32 index) 
        { 
            index += 4; 
            return BitConverter.ToUInt32( bytes, index - 4); 
        }
        //------------------------------------------------------------------------------------------
        UInt64 ReadUInt64(Byte[] bytes, ref Int32 index) 
        {
            index += 8; 
            return BitConverter.ToUInt64( bytes, index - 8); 
        }
        //------------------------------------------------------------------------------------------
        Int16 ReadInt16(Byte[] bytes, ref Int32 index) 
        { 
            index += 2; 
            return BitConverter.ToInt16( bytes, index - 2); 
        }
        //------------------------------------------------------------------------------------------
        Int32 ReadInt32(Byte[] bytes, ref Int32 index) 
        { 
            index += 4; 
            return BitConverter.ToInt32( bytes, index - 4); 
        }
        //------------------------------------------------------------------------------------------
        Int64 ReadInt64(Byte[] bytes, ref Int32 index) 
        { 
            index += 8; 
            return BitConverter.ToInt64( bytes, index - 8); 
        }
        //------------------------------------------------------------------------------------------
        Single ReadSingle(Byte[] bytes, ref Int32 index) 
        { 
            index += 4; 
            return BitConverter.ToSingle( bytes, index - 4); 
        }
        //------------------------------------------------------------------------------------------
        Double ReadDouble(Byte[] bytes, ref Int32 index) 
        { 
            index += 8; 
            return BitConverter.ToDouble(bytes, index - 8); 
        }
        //------------------------------------------------------------------------------------------
        #endregion
        
        //------------------------------------------------------------------------------------------
    }
}
