using System;
using System.Collections.Generic;

namespace Tcdev.Dsm.Model
{
	/// <summary>
	/// Represents a row in the DSM.  Modules are represent either a type (leaf node) or a namespace (branch node)
	/// </summary>
    /// <example>
    /// For a type with a full name of System.Drawing.Font we have 3 modules in the hierarchy
    ///     System
    ///         |--> Drawing
    ///                 |--> Font
    /// </example>
	public class Module
	{
        string name;
        string srcFile;
        string namespaceName;
        string assemblyName;
        bool   isNested;
        
        public int Id { get; set; }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get the name of the assembly where the type is defined
        /// </summary>
        internal string AssemblyName
        {
            get { return assemblyName; }
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get the namespace of the type
        /// </summary>
        internal string Namespace
        {
            get { return namespaceName; }
        }

        ////-------------------------------------------------------------------------------------------------
        //private bool collapsed;
        ///// <summary>
        ///// Get or set whether the module in the matrix is currently collapsed or not
        ///// </summary>
        //public bool IsCollapsed
        //{
        //    get { return collapsed; }
        //    set { collapsed = value; }
        //}

        

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets whether the type nested inside another or not
        /// </summary>
        public bool IsNested
        {
            get { return isNested; }
            set { isNested = value; }
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Relation information keyed by the consuming module
        /// </summary>
		public Dictionary<Module,Relation> Relations;

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Module constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="srcFile"></param>
        /// <param name="namespaceName"></param>
        /// <param name="assemblyName"></param>
        /// <param name="isNested"></param>
        public Module( string name, string srcFile, string namespaceName, string assemblyName, bool isNested )
        {
            this.name          = name;
            this.srcFile       = srcFile;
            this.namespaceName = namespaceName;
            this.assemblyName  = assemblyName;
            this.isNested      = isNested;

            Relations = new Dictionary<Module, Relation>();

            //Depth = 0;
            //IsCollapsed = true;
            //IsHidden = false;
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get th emodule name
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get the relation information for a given consumer of this module
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public Relation GetRelation(Module consumer)
        {
            Relation rel;
            Relations.TryGetValue(consumer, out rel);
            return rel;
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Add a consumer relationship for this module
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="weight"></param>
		public void AddRelation( Module consumer, int weight )
		{
            if (!Relations.ContainsKey(consumer))
            {
                Relations.Add(consumer, new Relation(this,consumer));
            }
            
            Relations[consumer].Weight += weight;
		}

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get the complete module name : Namespace._module_name_
        /// </summary>
        public string FullName
        {
            get
            {
                if (Namespace != null && Namespace.Length > 0)
                {
                    return Namespace + "." + Name;
                }

                return Name;
            }
        }

        public int BuildNumber { set; get; }

        public override int GetHashCode()
        {
            return FullName == null ? 0 : FullName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Module other = obj as Module;
            if (other == null) return false;

            if (this.FullName != null && other.FullName != null)
                return this.FullName.Equals(other.FullName);

            return this.FullName == other.FullName;
        }

        public override string ToString()
        {
            return FullName;
        }

	}
}
