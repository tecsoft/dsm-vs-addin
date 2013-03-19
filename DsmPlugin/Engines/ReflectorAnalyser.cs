using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Tcdev.Dsm.Model;
using Reflector;
using Reflector.CodeModel;
using System.IO;
using Tcdev.Outil;
using System.Windows.Forms;

namespace Tcdev.Dsm.Engine
{
    /// <summary>
    /// One type of Analyser engine - one which uses the Reflector API to analyse the call matrix
    /// between types in the Assemblies made known to it
    /// </summary>
    internal class ReflectorAnalyser : IAnalyser
    {
        private IList<Target>                        _assemblies;
        private Dictionary<ITypeDeclaration, Module> _modules;
        private Tcdev.Dsm.Model.DsmModel             _model;
        private static Logger                        _log;
        private DsmOptions                           _options;

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        public ReflectorAnalyser()
        {
            _log = new Logger("log.txt");
            _log.Trace("New Analysis : " + DateTime.Now);

            _modules    = new Dictionary<ITypeDeclaration, Module>();
            _assemblies = new List<Target>();
            _options    = new DsmOptions();
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get or set the DSM options
        /// </summary>
        public DsmOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the DSM model hierarchy
        /// </summary>
        public DsmModel Model
        {
            set { _model = value; }
        }

        //-------------------------------------------------------------------------------------------------
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
            foreach (Target target in _assemblies)
            {
                _log.Trace("Reading Assembly: " + target.FullPath);
                
                _model.AddAssembly( target.FullPath);
                
                IAssembly assembly = target.AssemblyObject as IAssembly;

                foreach (IModule module in assembly.Modules)
                {
                    foreach (ITypeDeclaration typeDecl in module.Types)
                    {
                        LoadType( assembly, typeDecl );
                    }
                }
            }
        }

        //-------------------------------------------------------------------------------------------------
        private void LoadType( IAssembly assembly, ITypeDeclaration typeDecl )
        {
            if ( ! ExcludeType( typeDecl ) )
            {
                _log.Trace("Type found: " + typeDecl.ToString() );

                if (!_modules.ContainsKey(typeDecl))
                {
                    Module newModule = _model.CreateModule( typeDecl.ToString(),
                                                    typeDecl.Namespace,
                                                    assembly.Name,
                                                    false );
                    _modules.Add( typeDecl, newModule );
                }

                foreach ( ITypeDeclaration nestedType in typeDecl.NestedTypes )
                {
                    if ( !_modules.ContainsKey( nestedType ) )
                    {
                        _log.Trace( " Nested type found: " + nestedType.ToString() );

                        Module newModule = _model.CreateModule(
                                typeDecl.ToString() + "." + nestedType.ToString(),
                                typeDecl.Namespace, assembly.Name, true );
                                
                        _modules.Add( nestedType, newModule );

                        // TODO classes nested within nested classes are not processed currently !
                    }
                } 
            }
        }

        
        //-------------------------------------------------------------------------------------------------
        public void AnalyseRelations()
        {
            _log.Trace("Starting Analysis ...");

            foreach( ITypeDeclaration typeDecl in _modules.Keys )
            {
                AnalyseType(typeDecl);
            }

            _log.Trace("AnalyseRelations completed : " + DateTime.Now);
        }

        //-----------------------------------------------------------------------------------------
        
        private void AnalyseType(ITypeDeclaration typeDecl )
        {
            MarkInterfaces(typeDecl);
            MarkBaseType(typeDecl);
            MarkFields(typeDecl);
            MarkProperties(typeDecl);

            /*
             * Attributes TODO - Currently not supported
             *
             * Events declared by class itself are not analysed
             * 
             * Generic Arguments - not needed just describes the place holders for generic types
             * */

            /*
             * TODO Generic Module  ???
             * */

         
            AnalyseTypeMethods(typeDecl);
        }

        //-----------------------------------------------------------------------------------------

        private void AnalyseTypeMethods(ITypeDeclaration typeDecl)
        {
            _log.Trace( typeDecl.Name + " Methods ...");

            foreach (IMethodDeclaration method in typeDecl.Methods)
            {
                _log.Trace(method.Name);

                AnalyseMethodBody(typeDecl, method);
                MarkGenericMethodParameters(typeDecl, method);
                MarkMethodParameters(typeDecl, method);
                MarkMethodReturnType(typeDecl, method);
                
                /*
                 * Generic Method TODO ???
                 * */
            }
        }

        //-----------------------------------------------------------------------------------------

        private void MarkMethodReturnType(ITypeDeclaration typeDecl, IMethodDeclaration method)
        {
            _log.Trace("Return Type " + method.ReturnType);

            ITypeDeclaration dec = ToTypeDeclaration(method.ReturnType.Type);

            MarkRelation(dec, typeDecl);
        }

        //-----------------------------------------------------------------------------------------

        private void MarkMethodParameters(ITypeDeclaration typeDecl, IMethodDeclaration method)
        {
            _log.Trace("Parameters ...");
            foreach (IParameterDeclaration paramDecl in method.Parameters)
            {
                _log.Trace(paramDecl.Name);

                ITypeDeclaration it = ToTypeDeclaration(paramDecl.ParameterType);

                MarkRelation(it, typeDecl);
            }
        }

        //-----------------------------------------------------------------------------------------

        private void MarkGenericMethodParameters(ITypeDeclaration typeDecl, IMethodDeclaration method)
        {
            _log.Trace("Generic Arguments of method ...");

            foreach (IType genericArgument in method.GenericArguments)
            {
                _log.Trace("Method.Generic argument: " + genericArgument.ToString());

                ITypeDeclaration gadec = ToTypeDeclaration(genericArgument);
                MarkRelation(gadec, typeDecl);
            }
        }

        //-----------------------------------------------------------------------------------------

        private void MarkProperties(ITypeDeclaration typeDecl)
        {
            _log.Trace("Properties ...");
            foreach (IPropertyDeclaration propertyDecl in typeDecl.Properties)
            {
                _log.Trace("Property:  " + propertyDecl.Name);

                ITypeDeclaration pdec = ToTypeDeclaration(propertyDecl.PropertyType);

                MarkRelation(pdec, typeDecl);
            }
        }

        //-----------------------------------------------------------------------------------------

        private void MarkFields(ITypeDeclaration typeDecl)
        {
            foreach (IFieldDeclaration fieldDecl in typeDecl.Fields)
            {
                _log.Trace("Fields : " + fieldDecl.Name);
                ITypeDeclaration fdec = ToTypeDeclaration(fieldDecl.FieldType);

                MarkRelation(fdec, typeDecl);
            }
        }

        //-----------------------------------------------------------------------------------------

        private void MarkBaseType(ITypeDeclaration typeDecl)
        {
            _log.Trace("Base Types ...");
            if (typeDecl.BaseType != null)
            {
                MarkRelation(InternalResolve(typeDecl.BaseType), typeDecl);
            }
        }

        //-----------------------------------------------------------------------------------------
        
        private void MarkInterfaces(ITypeDeclaration typeDecl)
        {
            _log.Trace("Interfaces ...");
            foreach (ITypeReference interf in typeDecl.Interfaces)
            {
                MarkRelation(InternalResolve(interf), typeDecl);
            }
        }

        //-------------------------------------------------------------------------------------------------

        static ITypeDeclaration ToTypeDeclaration( IType o )
        {
            if (o != null)
            {
                ITypeDeclaration decl;

                if (o is ITypeReference)
                {
                    decl = InternalResolve(o as ITypeReference);
                }
                else
                {
                    decl = o as ITypeDeclaration;
                }

                if (decl != null && decl.GenericType != null)
                {
                    decl = InternalResolve(decl.GenericType);
                }

                return decl;
            }

            return null;
        }

        //-------------------------------------------------------------------------------------------------
        void AnalyseMethodBody(ITypeDeclaration typeDecl, IMethodDeclaration method)
        {
            _log.Trace("Method Body ..." + method.Name);

            try
            {
                if (method.Body != null)
                {
                    IMethodBody body = method.Body as IMethodBody;

                    MarkLocalVariables(typeDecl, body);

                    AnalyseMethodOpCodes(typeDecl, body);
                }
                else
                {
                    _log.Trace("Abstract Method Declaration?");
                }
            }
            catch (Exception e)
            {
                _log.Trace("ERROR: " + e.Message + System.Environment.NewLine + e.StackTrace);
                throw;
            }
        }
        //-------------------------------------------------------------------------------------------------
        private void AnalyseMethodOpCodes(ITypeDeclaration typeDecl, IMethodBody body)
        {
            foreach (IInstruction instruction in body.Instructions)
            {
                if (instruction.Value != null)
                {
                    _log.Trace(String.Format("Opcode: {0:x}", instruction.Code));
                    object val = instruction.Value;
                    switch (instruction.Code)
                    {
                        //
                        // Casting && IsInst
                        //
                        case 0x0074: // Opcode castclass
                        case 0x0075: // Opcode isinst
                            MarkCastOpCpde(typeDecl, instruction, val);
                            break;
                        //
                        // New Array
                        //
                        case 0x008d: // opcodenewarr
                            MarkArrayConstructorOpCode(typeDecl, instruction, val);
                            break;
                        //
                        // Method Invocations
                        //
                        case 0x0027:  // Opcode jmp (to method)
                        case 0x0028:  // OpCode call
                        case 0x0029:   // Opcode calli
                        case 0x006f:  // OpCode virtCall
                        case 0x0073:  // OpCode newObj        
                            MarkMethodCalls(typeDecl, instruction, val);
                            break;
                    }
                }
            }
        }

        //-------------------------------------------------------------------------------------------------
        private void MarkMethodCalls(ITypeDeclaration typeDecl, IInstruction instruction, object val)
        {
            _log.Trace("call " + instruction.ToString());

            IMethodReference m = val as IMethodReference;
            if (m != null)
            {
                ITypeDeclaration mdec = ToTypeDeclaration(m.DeclaringType);
                MarkRelation(mdec, typeDecl);
            }
        }
        //-------------------------------------------------------------------------------------------------
        private void MarkArrayConstructorOpCode(ITypeDeclaration typeDecl, IInstruction instruction, object val)
        {
            _log.Trace("Array constructor ..." + instruction.ToString());

            ITypeDeclaration arrdecl = ToTypeDeclaration(val as IType);
            MarkRelation(arrdecl, typeDecl);
        }
        //-------------------------------------------------------------------------------------------------
        private void MarkCastOpCpde(ITypeDeclaration typeDecl, IInstruction instruction, object val)
        {
            _log.Trace("Cast " + instruction.ToString());
            ITypeDeclaration castdecl = ToTypeDeclaration(val as IType);

            MarkRelation(castdecl, typeDecl);
        }

        //-------------------------------------------------------------------------------------------------
        private void MarkLocalVariables(ITypeDeclaration typeDecl, IMethodBody body)
        {
            _log.Trace("Local variables ...");
            foreach (IVariableDeclaration variable in body.LocalVariables)
            {
                ITypeDeclaration vdecl = ToTypeDeclaration(variable.VariableType);

                MarkRelation(vdecl, typeDecl);
            }
        }

        // ---------------------------------------------------------------------------------------------

        private static ITypeDeclaration InternalResolve( ITypeReference typeRef )
        {
            // In the case of a type reference we must call the resolve method to extract the actual type defintion
            ITypeDeclaration typeDec = null;
            try
            {
                typeDec = typeRef.Resolve();
            }
            catch (Exception ex)
            {
                _log.Trace( "InternalResolve: " + ex.Message );
            }

            return typeDec;
        }

        //-------------------------------------------------------------------------------------------------
        private void MarkRelation(ITypeDeclaration providerType, ITypeDeclaration consumerType)
        {
            // If the provider and the consumer are both being tracked we increment the dependency the
            // count by one
            if (providerType == null || consumerType == null)
            {
                _log.Trace("At least one type has not been resolved - ignoring relation");
            }
            else
            {
                Module consumer;                
                Module provider;
                
                // if we are not displaying nested classes we find which types should be used to count the depdencies
                if (_options.HideNestedClasses)
                {
                    providerType = NestedParentHelper(providerType);
                    consumerType = NestedParentHelper(consumerType);
                }

                _log.Trace(providerType.ToString() + " ---> " + consumerType.ToString());

                if (_modules.TryGetValue(consumerType, out consumer) &&
                    _modules.TryGetValue(providerType, out provider))
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

        ITypeDeclaration NestedParentHelper(ITypeDeclaration typeDecl)
        {
            ITypeDeclaration result = typeDecl;  //

            if (typeDecl.Owner != null) // type is a nested type
            {
                if (typeDecl.Owner is ITypeDeclaration)
                {
                    _log.Trace("Owner class is TypeDeclaration: " + typeDecl.Owner.ToString());

                    result = typeDecl.Owner as ITypeDeclaration;
                }
                else if (typeDecl.Owner is ITypeReference)
                {
                    _log.Trace("Owner class is TypeReference: " + typeDecl.Owner.ToString());

                    result = InternalResolve(typeDecl.Owner as ITypeReference);
                }
            }
            
            _log.Trace("Not nested class " );

            return result;

        }

        //-------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            if (_log != null) _log.Dispose();
        }
        
        //-------------------------------------------------------------------------------------------------

        bool ExcludeType( ITypeDeclaration typeDecl )
        {
            bool exclude = false;

            // TODO private implementation details

            if ( _options.ExcludeGlobalNamespace && typeDecl.Namespace.Length == 0 )
            {
                exclude = true;
            }
            else if ( _options.ExcludeCompilerNamespaces &&
                      ( typeDecl.Namespace.Equals( "<CppImplementationDetails>" ) ||
                        typeDecl.Namespace.Equals( "<CrtImplementationDetails>")  ) )
            {
                exclude = true;
            }

            return exclude;
        }
    }
}
