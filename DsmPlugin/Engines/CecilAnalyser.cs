using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using Tcdev.Dsm.Model;
using Tcdev.Outil;
using System.Linq;

namespace Tcdev.Dsm.Engine
{
    /// <summary>
    /// One type of Analyser engine - one which uses the Reflector API to analyse the call _matrix
    /// betwen types in the Assemblies made known to it
    /// </summary>
    public  class CecilAnalyser : IAnalyser, IDisposable
    {
        private IList _assemblies;

        /*
         * Map for list of Types by internal DSM module
         */
        Dictionary< string, Tcdev.Dsm.Model.Module> _modules;
        IList<Mono.Cecil.TypeDefinition> _typeList;
        private Tcdev.Dsm.Model.DsmModel          _model;
        private static Logger                     _log;
        private DsmOptions                        _options;

        public Dictionary<string, Tcdev.Dsm.Model.Module> Modules
        {
            get { return _modules; }
        }

        public IList<Mono.Cecil.TypeDefinition> Types
        {
            get { return _typeList; }
        }

        //-------------------------------------------------------------------------------------------------
        public CecilAnalyser()
        {
            _log = new Logger(Path.Combine( Path.GetTempPath(), "log.txt" ) );
            _log.Trace("FRAMEWORK ANALYSER : New Analysis : " + DateTime.Now);

            _modules    = new Dictionary<string, Tcdev.Dsm.Model.Module>();
            _typeList = new List<Mono.Cecil.TypeDefinition>();
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

        public FileInfo ProjectFile
        {
            get;
            set;
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
        public IList<Module> LoadTypes()
        {
            var resolver = new DefaultAssemblyResolver();

            IDictionary<string, bool> paths = new Dictionary<string, bool>();

            foreach (Target target in _assemblies)
            {
                FileInfo fi = new FileInfo(target.FullPath );
                if (!fi.Exists)
                {
                    _log.Trace("Assembly not found: " + target.FullPath);
                }
                else
                {
                    if (paths.ContainsKey(fi.DirectoryName) == false)
                    {
                        paths.Add(fi.DirectoryName, true);
                        resolver.AddSearchDirectory(fi.DirectoryName);
                    }
                }
            }

            var parameters  =new ReaderParameters() { AssemblyResolver = resolver };

            foreach( Target target in _assemblies )
            {
                _log.Trace("Reading Assembly: " + target.FullPath);

                try
                {
                    Mono.Cecil.AssemblyDefinition assembly = 
                        Mono.Cecil.AssemblyDefinition.ReadAssembly( target.FullPath, parameters );

                    foreach( Mono.Cecil.ModuleDefinition module in assembly.Modules )
                    {
                        var moduleTypes = module.Types;
                        
                        foreach(Mono.Cecil.TypeDefinition typeDecl in moduleTypes )
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

            return _modules.Values.ToList();
        }

        private void LoadType( Target target, Mono.Cecil.TypeDefinition typeDecl )
        {
            if ( !ExcludeType( typeDecl ) )
            {
                _log.Trace( "Type found: " + typeDecl.ToString() );

                if ( !_modules.ContainsKey( /*typeDecl.GUID*/ typeDecl.FullName ) )
                {
                    Tcdev.Dsm.Model.Module newModule = _model.CreateModule(
                                            typeDecl.Name, typeDecl.Namespace, target.FullPath, false );
                    _modules.Add(typeDecl.FullName, newModule );

                    _typeList.Add( typeDecl );
                }

                // Nested classes are named OuterClassName.InnerClassName in the namespace of 
                // the outer class

                try
                {
                    foreach( Mono.Cecil.TypeDefinition nestedType in typeDecl.NestedTypes )
                    {
                        if ( !_modules.ContainsKey( nestedType.FullName) )
                        {
                            _log.Trace( " Nested type found: " + nestedType.ToString() );

                            _modules.Add(
                                nestedType.FullName,
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

            foreach( Mono.Cecil.TypeDefinition typeDecl in _typeList )
            {
                AnalyseType(typeDecl);
            }

            _log.Trace("AnalyseRelations completed : " + DateTime.Now);
        }

        //-----------------------------------------------------------------------------------------
        
        private void AnalyseType(Mono.Cecil.TypeDefinition typeDecl )
        {
            MarkInterfaces(typeDecl);
            MarkBaseType(typeDecl);
            MarkFields(typeDecl);
            MarkProperties(typeDecl);
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

        void AnalyseTypeMethods(Mono.Cecil.TypeDefinition typeDecl)
        {
            _log.Trace( typeDecl.Name + " Methods ...");
            
            try
            {  
                foreach( Mono.Cecil.MethodDefinition method in typeDecl.Methods )
                {
                    _log.Trace(method.Name );

                    AnalyseMethodBody(typeDecl, method);
                    MarkGenericMethodParameters(typeDecl, method);
                    MarkMethodParameters(typeDecl, method);
                    MarkMethodReturnType(typeDecl, method);
                }   
            }
            catch
            {
                _log.Trace( "Resolution Exception in AnalyseTypeMethods" );
            }
        }

        //-----------------------------------------------------------------------------------------

        public void MarkMethodReturnType( Mono.Cecil.TypeDefinition typeDecl, Mono.Cecil.MethodDefinition method )
        {
            _log.Trace("Return Type " + method.ReturnType);

            try
            {
                Mono.Cecil.TypeReference dec = method.ReturnType;

                MarkRelation(dec, typeDecl);
            }
            catch
            {
                _log.Trace( "Resolution exception MarkMethodReturnType" );
            }
        }

        //-----------------------------------------------------------------------------------------

        public void MarkMethodParameters( Mono.Cecil.TypeDefinition typeDecl, Mono.Cecil.MethodDefinition method )
        {
            _log.Trace("Parameters ...");
            
            try
            {
                foreach (Mono.Cecil.ParameterDefinition paramDecl in method.Parameters)
                {
                    _log.Trace(paramDecl.Name);

                    MarkRelation(paramDecl.ParameterType, typeDecl);
                }
            }
            catch
            {
                _log.Trace( "Resolution exception in MarkMethodParameters" );
            }
        }
        
        //-----------------------------------------------------------------------------------------

        public void MarkGenericMethodParameters( Mono.Cecil.TypeDefinition typeDecl, Mono.Cecil.MethodDefinition method )
        {
            _log.Trace("Generic Arguments of method ...");
            
            try
            {
                foreach (Mono.Cecil.GenericParameter genericArgument in method.GenericParameters)
                {
                    _log.Trace("Method.Generic argument: " + genericArgument.ToString());

                    foreach (var constraint in genericArgument.Constraints)
                    {
                        MarkRelation(constraint, typeDecl);
                    }
                }
            }
            catch
            {
                _log.Trace( "Resolution exception in MarkGenericMethodParametres" );
            }
        }

       
        //-----------------------------------------------------------------------------------------

        public void MarkProperties(Mono.Cecil.TypeDefinition typeDecl)
        {
            _log.Trace("Properties ...");
            try
            {
                foreach (Mono.Cecil.PropertyDefinition propertyDecl in  typeDecl.Properties)
                {
                    _log.Trace("Property:  " + propertyDecl.Name);
                    MarkRelation(propertyDecl.PropertyType, typeDecl);
                }
            }
            catch
            {
                _log.Trace( "ResolutionException in MarkProperties" );
            } 
        }

        //-----------------------------------------------------------------------------------------

        public void MarkFields(Mono.Cecil.TypeDefinition typeDecl)
        {
            try
            {
                foreach (Mono.Cecil.FieldDefinition fieldDecl in typeDecl.Fields )
                {
                    _log.Trace("Fields : " + fieldDecl.Name);

                    MarkRelation( fieldDecl.FieldType, typeDecl );
                }
            }
            catch
            {
                _log.Trace( "Resolution exception in MarkFields" );
            }
        }

        //-----------------------------------------------------------------------------------------

        public void MarkBaseType( Mono.Cecil.TypeDefinition typeDecl )
        {
            _log.Trace("Base Types ...");
            if (typeDecl.BaseType != null)
            {
                MarkRelation(typeDecl.BaseType, typeDecl);
            }
        }

        //-----------------------------------------------------------------------------------------
        public void MarkInterfaces( Mono.Cecil.TypeDefinition typeDecl )
        {
            _log.Trace("Interfaces ...");
            
            try
            {
                foreach ( Mono.Cecil.TypeReference interf in typeDecl.Interfaces )
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
        void AnalyseMethodBody( Mono.Cecil.TypeDefinition typeDecl, Mono.Cecil.MethodDefinition method )
        {
            _log.Trace("Method Body ..." + method.Name);

            try
            {
                Mono.Cecil.Cil.MethodBody body = method.Body;

                if ( body != null)
                {
                    MarkLocalVariables(typeDecl, body);

                   MarkBodyTypeReferences(typeDecl, body);
                }
                else
                {
                    _log.Trace("Abstract Method Declaration?");
                }
            } 
            catch( Exception ex )
            {
                _log.Trace( "Resolution error in GetMethodBody:" + ex.ToString() );
            }
        }

        //-----------------------------------------------------------------------------------------

        public void MarkBodyTypeReferences( 
            Mono.Cecil.TypeDefinition typeDecl,
            Mono.Cecil.Cil.MethodBody body)
        {
            int index = 0;
            
            var instructions = body.Instructions;
            while( index < instructions.Count )
            {
                var i = instructions[index];
                var opCode = i.OpCode;

                switch( opCode.OperandType )
                {
                    case Mono.Cecil.Cil.OperandType.InlineTok:
                    case Mono.Cecil.Cil.OperandType.InlineType:
                    case Mono.Cecil.Cil.OperandType.InlineMethod:
                    case Mono.Cecil.Cil.OperandType.InlineField:
                        {
                            object op = i.Operand;

                            if (op == null)
                            {
                                _log.Trace("unexpected null operand");
                            }
                            else
                            {
                                Mono.Cecil.TypeReference t = op as Mono.Cecil.TypeReference;
                                if (t != null)
                                {
                                    MarkRelation(t, typeDecl);
                                }
                                else
                                {
                                    Mono.Cecil.MemberReference m = op as Mono.Cecil.MemberReference;
                                    if (m != null)
                                    {
                                        MarkRelation(m.DeclaringType, typeDecl);
                                    }
                                    else
                                    {
                                        _log.Trace("Unhandled token type: " + op.ToString());
                                    }
                                }
                            }
                        }
                        
                        break;

                    default:
                        break;
                }

                index++;
            }
        }

        //-----------------------------------------------------------------------------------------
        public void MarkLocalVariables( Mono.Cecil.TypeDefinition typeDecl, Mono.Cecil.Cil.MethodBody body )
        {
            _log.Trace("Local variables ...");
            
            try
            {
                foreach (Mono.Cecil.Cil.VariableDefinition variable in body.Variables)
                {
                    MarkRelation(variable.VariableType, typeDecl);
                }
            }
            catch
            {
                _log.Trace( "Resolution exception in MarkLocalVariables" );
            }
            
        }

        //-------------------------------------------------------------------------------------------------
        private void MarkRelation( Mono.Cecil.TypeReference providerType, Mono.Cecil.TypeReference consumerType )
        {
            _log.Trace("Marking Relation");
            
            if (providerType == null || consumerType == null)
            {
                _log.Trace("At least one type has not been resolved - ignoring relation");
            }
            else
            {
                //Tcdev.Dsm.Model.Module consumer;

                //Tcdev.Dsm.Model.Module provider;
                if (_options.HideNestedClasses)
                {
                    providerType = NestedParentHelper(providerType);
                    consumerType = NestedParentHelper(consumerType);
                }

                _log.Trace(providerType.ToString() + " ---> " + consumerType.ToString());

                var consumer = _model.FindNode(consumerType.FullName);
                var provider = _model.FindNode(providerType.FullName);

                //if ( _model.H.TryGetValue(consumerType.FullName, out consumer) &&
                //    _modules.TryGetValue(providerType.FullName, out provider))
                if ( consumer != null && provider != null )
                {
                    _log.Trace("Relation found: " + providerType.Name);
                    provider.NodeValue.AddRelation(consumer.NodeValue, 1);
                }
                else
                {
                    _log.Trace("Relation NOT FOUND");
                    _log.Trace("consumer: " + consumerType.FullName);
                    _log.Trace("provider: " + providerType.FullName);
                }
                _log.Trace("-----------------------------------------------------");
            }
        }

        //-------------------------------------------------------------------------------------------------

        Mono.Cecil.TypeReference NestedParentHelper(Mono.Cecil.TypeReference typeDecl)
        {
            Mono.Cecil.TypeReference result = typeDecl;  //

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

        bool ExcludeType( Mono.Cecil.TypeDefinition typeDecl )
        {
            bool exclude = false;

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
    }
}
