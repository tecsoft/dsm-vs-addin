using System;
using System.Collections.Generic;
using System.Text;
using Tcdev.Collections.Generic;
using System.Collections;
using System.IO;
using System.Xml;
using Tcdev.Dsm;
using Tcdev.Dsm.Matrix;
using Tcdev.Dsm.Engine;
using Tcdev.Outil;
using Tcdev.Dsm.Model.DependencyRules;

namespace Tcdev.Dsm.Model
{
    /// <summary>
    /// DsmModel is based around the Module Hierarchy held in form of a tree.
    /// DsmModel provides methods for its contruction and modification
    /// </summary>
    public class DsmModel 
    {    
        //static Logger _log = new Logger(Path.Combine( Path.GetTempPath() ,"model.txt" ));

        public int BuildNumber { get; internal set; }

        Dictionary<string, Tree<Module>.Node> branchLookup = new Dictionary<string, Tree<Module>.Node>();

        //public IList<DependencyRule> Rules { get; protected set; }

        public RuleManager RuleManager { get; protected set; }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        public DsmModel()
        {
            SourceFiles  = new Dictionary<string, object>();
            IsModified   = false;
            Options      = new DsmOptions();  // created with a default set of options
            RuleManager = new RuleManager();
        }

        private Dictionary<string, object> SourceFiles
        {
            get;
            set;
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// Tree of Modules representing the Hierarchy of types
        /// </summary>
        public Tree<Module> Hierarchy
        {
            get;
            private set;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Get or set the DsmOptions
        /// </summary>
        public DsmOptions Options
        {
            get;
            set;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Set to true if _model has been modified and would require the user to demand its serialization
        /// if they want to keep the changes
        /// </summary>
        public bool IsModified
        {
            get;
            set;
        }
        
        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// Returns true if the Model contains a node with the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return branchLookup.ContainsKey(key);
        }

        /// <summary>
        /// Gets the node for the given key (requires contains check beforehand)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Tree<Module>.Node Get(string key)
        {
            return branchLookup[key];
        }

        public int NodeCount()
        {
            return branchLookup.Keys.Count;
        }
        /// <summary>
        /// Adds a module to the end list of children of parentNode
        /// </summary>
        /// <param name="module"></param>
        /// <param name="key"></param>
        /// <param name="parentNode"></param>
        /// <param name="buildNumber"></param>
        /// <returns></returns>
        public Tree<Module>.Node Add(Module module, string key, Tree<Module>.Node parentNode, int buildNumber)
        {
            module.BuildNumber = buildNumber;
            Tree<Module>.Node n = Hierarchy.CreateNode(module);

            if (parentNode != null)
                n.Depth = parentNode.Depth + 1;

            if (buildNumber == 0)
                Hierarchy.AddLast(parentNode, n);
            else
                Hierarchy.Add(parentNode, n);

            // new
            if (Contains(key) == false)
                branchLookup.Add(key, n);

            return n;
        }

        /// <summary>
        /// Remove node and its children from the model
        /// </summary>
        /// <param name="node"></param>
        void RemoveNode(Tree<Module>.Node node)
        {
            // Remove node from hierarchy and branch lookup and
            // recurse down doing the same for child nodes

            Module m = node.NodeValue;

            if (m != null)
            {
                branchLookup.Remove(m.FullName);
                foreach (var child in node.Children)
                {
                    RemoveNode(child);
                }

                Hierarchy.Remove(node);
            }
        }

        /// <summary>
        /// Recursive function to remove old nodes according to the current build number
        /// </summary>
        /// <param name="buildNumber"></param>
        /// <param name="current"></param>
        void RemoveIfOld(int buildNumber, Tree<Module>.Node current)
        {
            Module m = current.NodeValue;
            if (m != null && m.BuildNumber != buildNumber)
            {
                RemoveNode(current);
            }
            else
            {
                foreach (var child in current.Children)
                {
                    RemoveIfOld(buildNumber, child);
                }
            }
        }

        /// <summary>
        /// Remove old nodes according to build number
        /// </summary>
        /// <param name="buildNumber"></param>
        public void RemoveOldItems(int buildNumber)
        {
            Tree<Module>.Node current = Hierarchy.Root;

            RemoveIfOld(buildNumber, current);

        }

        //TODO Change so that it is done automatically in call CreateModule
        public void AddAssembly(string src)
        {
            if (!SourceFiles.ContainsKey( src ) )
            {
                SourceFiles.Add(src, null);
            }
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates a module in the _model but without any particular placement in a Hierarchy
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="namespaceName"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public Module CreateModule(string typeName, string namespaceName, string assemblyName, bool isNested )
        {
            Module m = new Module(typeName, null, namespaceName, assemblyName, isNested );
            m.BuildNumber = BuildNumber;
            return m;
        }
 
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Construct the module Hierarchy according to namespace ( logical _model) or by assembly
        /// (physical _model).  For the logical _model, modify the hierarchy so that a *.* namespace is
        /// added to contain the types at each level if desired
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="useAsterix"></param>
        public void BuildHierarchy( IList<Module> types)
        {
            // new
            if (Hierarchy == null)
            {
                Hierarchy = new Tree<Module>();
                BuildNumber = 0;
            }
            else
            {
                StartBuild();
            }

            if (this.Options.DsmModelType == DsmOptions.ModelType.Physical )
            {
                PhysicalModelBuilder( types );
            }
            else
            {
                LogicalModelBuilder2( types );

                if (BuildNumber > 0)
                {
                    RemoveOldItems(BuildNumber);
                }
                
                // clean up hanging leaf nodes to a * namespace
                BalanceLeafNodes();
            }
        }

        public Tree<Module>.Node FindNode(string key)
        {
            return Contains(key) ? Get(key) : null;
        }

        

        //-------------------------------------------------------------------------------------------------

        void LogicalModelBuilder2(IList<Module> typeModules)
        {
            foreach (Module module in typeModules)
            {
                Tree<Module>.Node parentNode = null;

                bool exists = Contains(module.FullName);
                if ( exists )
                {
                    parentNode = Get(module.FullName);
                    parentNode.NodeValue.BuildNumber = BuildNumber;

                    if (parentNode.Parent != null && parentNode.Parent.NodeValue != null
                        && "*".Equals(parentNode.Parent.NodeValue.Name))
                        parentNode.Parent.NodeValue.BuildNumber = BuildNumber;

                }
                string[] tokens = module.Namespace.Split('.');
                if (tokens[0].Length > 0)  // ignore .<Module> for the moment
                {
                    string namespacePortion = string.Empty;

                    foreach (string token in tokens)
                    {
                        namespacePortion = (namespacePortion.Length > 0)
                            ? namespacePortion + "." + token : token;

                        if (exists || Contains(namespacePortion ) )
                        {
                            parentNode = Get(namespacePortion);
                            parentNode.NodeValue.BuildNumber = BuildNumber;
                        }
                        else
                        {
                            // create a new module
                            string nspace = null;
                            int pos = namespacePortion.LastIndexOf('.');
                            if (pos != -1)
                            {
                                nspace = namespacePortion.Substring(0, pos);
                            }

                            Module m = CreateModule(token, nspace, null, false);
                            parentNode = Add(m, m.FullName, parentNode, BuildNumber);
                        }
                    }
                
                    if ( !exists )
                    {
                        Module copy = CreateModule(module.Name, module.Namespace, module.AssemblyName, module.IsNested);
                        Tree<Module>.Node node = Add(copy, copy.FullName, parentNode, BuildNumber);//tree.CreateNode(module);
                        node.IsHidden = (module.IsNested == true && Options.HideNestedClasses == true);
                   }
                }
            }

        }
//        void LogicalModelBuilder( IList<Module> typeModules )
//        {
//            //// the tree hierarchy which represents the model
//            //Tree<Module> tree = new Tree<Module>();
//            //// dictionary to remember existing tree nodes 
//            //Dictionary<string, Tree<Module>.Node> branchLookup = new Dictionary<string, Tree<Module>.Node>();

//            foreach( Module module in typeModules )
//            {
//                Tree<Module>.Node parentNode = null;

//                // descend each part to update or create each part of the missing branch

                



//                if ( Hierarchy.Contains(module.FullName) )
//                {
//                    parentNode = Hierarchy.Get(module.Namespace);
//                ////}

//                //if (parentNode == null)
//                //{
//                    // need to create the parts of this hierarchy that don't exist
//                    string[] tokens = module.Namespace.Split('.');

//                    if (tokens[0].Length > 0)  // ignore .<Module> for the moment
//                    {
//                        string namespacePortion = string.Empty;

//                        foreach (string token in tokens)
//                        {
//                            namespacePortion = (namespacePortion.Length > 0)
//                                ? namespacePortion + "." + token : token;

//                            //if (!branchLookup.ContainsKey(namespacePortion))
//                            if ( ! Hierarchy.Contains(namespacePortion ) )
//                            {
//                                // create a new module
//                                string nspace = null;
//                                int pos = namespacePortion.LastIndexOf('.');
//                                if (pos != -1)
//                                {
//                                    nspace = namespacePortion.Substring(0, pos);
//                                }

//                                //Module m = new Module(token, null, nspace, null, false);
//                                Module m = CreateModule(token, nspace, null, false);
//                                parentNode = Hierarchy.Add(m, m.FullName, parentNode, BuildNumber);

//                                //_lookup[m.FullName] = m;
                                

//                                //Tree<Module>.Node n = tree.CreateNode(m);
                                
//                                //if ( parentNode != null )
//                                //    n.Depth = parentNode.Depth + 1;
                                    
//                                //tree.Add(parentNode, n);
//                                //parentNode = n;
//                                //branchLookup.Add(namespacePortion, parentNode);
//                            }
//                            else
//                            {
//                                parentNode = Hierarchy.Get(namespacePortion);
//                            }
//                        }
//                    }
                

//                // add type to end of branch
//                Module copy = CreateModule(module.Name, module.Namespace, module.AssemblyName, module.IsNested);
//                Tree<Module>.Node node = Hierarchy.Add(copy, copy.FullName, parentNode, BuildNumber);//tree.CreateNode(module);

//                node.IsHidden = (module.IsNested == true && Options.HideNestedClasses == true);
//}
                
//                //_lookup[module.FullName] = module;
                
                
//                //if (parentNode != null)
//                //{
//                //    node.Depth = parentNode.Depth + 1;
//                //}
//                //tree.Add(parentNode, node);
//            }

//            //return tree;
//        }

        //-------------------------------------------------------------------------------------------------

        Tree<Module> PhysicalModelBuilder(IList<Module> typeModules)
        {
            Tree<Module> tree = new Tree<Module>();
            Dictionary<string, Tree<Module>.Node > branchLookup = new Dictionary<string, Tree<Module>.Node>();

            foreach (Module module in typeModules)
            {
                Tree<Module>.Node parentNode = null;

                if (branchLookup.ContainsKey(module.AssemblyName))
                {
                    parentNode = branchLookup[module.AssemblyName];
                }

                if (parentNode == null)
                {
                    Module m = new Module(module.AssemblyName, null, null, null, false);
                    parentNode = tree.CreateNode(m);
                    tree.Add(null, parentNode);
                    branchLookup.Add(module.AssemblyName, parentNode);
                }

                
                Tree<Module>.Node node = tree.CreateNode(module);
                node.Depth = parentNode.Depth + 1;
                node.IsHidden = ( module.IsNested == true && Options.HideNestedClasses == true);
                tree.Add(parentNode, node);
            }
            return tree;
        }

        //-------------------------------------------------------------------------------------------------
        void BalanceLeafNodes()
        {
            /*
             * In the DSM we do not display types and namespaces at the same level.
             * A tree node may have for children, either modules representing actual types or modules
             * representing namespace branches but not both
             * The aim then of this method is to search the tree looking leaf nodes and moving them
             * to a fabricated '*' namespace in the appropriate branch.
             * Example : there are 2 types : type A in namespace org.xxx and type B in namespace org.xxx.zzz
             * we therefore in the DSM after 'balancing' will have :
             *      org.xxx --> *   --> typeA
             *      org.xxx --> zzz --> typeB
             * */

            //Tree<Module> tree = Hierarchy.tree;

            Balance(Hierarchy, Hierarchy.Root);

            TreeIterator<Module> it = new TreeIterator<Module>(Hierarchy);
            Tree<Module>.Node node = it.Next();

            while (node != null)
            {
                if (node.HasChildren)
                {
                    Balance(Hierarchy, node);
                }

                node = it.Next();
            }
        }

        void Balance(Tree<Module> tree, Tree<Module>.Node parentNode)
        {
            // if parentNode contains leafs and branches - we copy leafs to a branch *
            // creating * if it does not yest exist

            //IList<Tree<Module>.Node> branches = new List<Tree<Module>.Node>();
            IList<Tree<Module>.Node> leaves   = new List<Tree<Module>.Node>();
            
            Tree<Module>.Node starNode = null;
            bool branchFound = false;

            foreach( var child in parentNode.Children )
            {
                Module m = child.NodeValue;
                if ("*".Equals(m.Name))
                    starNode = child;
                else
                {
                    if (child.HasChildren == false)
                    {
                        leaves.Add(child);
                    }
                    else
                    {
                        branchFound = true;
                    }
                }
            }

            if ( branchFound && leaves.Count > 0 )
            {
                if ( starNode == null )
                {
                    Module m = CreateModule( "*", 
                        parentNode.NodeValue == null ? null : parentNode.NodeValue.FullName, null, false );
                    starNode = Add(m, m.FullName, parentNode, BuildNumber);
                }

                foreach (Tree<Module>.Node leaf in leaves)
                {
                    tree.Remove(leaf);
                    tree.Add(starNode, leaf);
                    leaf.Depth++;
                }
            }
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true of the currently select node may be moved below its next sibling
        /// </summary>
        /// <returns></returns>
        public bool CanMoveNodeDown()
        {
            return (SelectedNode != null && SelectedNode.nextSibling != null);
        }
        
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Moves the currently select node below the next sibling
        /// </summary>
        /// <returns></returns>
        public bool MoveDown()
        {
            bool ok = false;
            if ( CanMoveNodeDown() )
            {
                // we actually move up the next sibling !
                Tree<Module>.Node node = SelectedNode.nextSibling;
                Hierarchy.Remove(node);
                Hierarchy.InsertBefore(node, SelectedNode);

                AllocateIds();
                ok = true;
            }

            return ok;
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// returns true if the selected node may be moved above the previous sibling
        /// </summary>
        /// <returns></returns>
        public bool CanMoveNodeUp()
        {
            return (SelectedNode != null && SelectedNode.previousSibling != null);
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Move the selected node above the previous sibling
        /// </summary>
        /// <returns></returns>
        public bool MoveUp()
        {
            bool ok = false;
            if ( CanMoveNodeUp() )
            {
                Tree<Module>.Node node = SelectedNode.previousSibling;
                Hierarchy.Remove(SelectedNode);
                Hierarchy.InsertBefore(SelectedNode, node);

                AllocateIds();

                ok = true;
            }
            return ok;
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Set or get the selected node
        /// </summary>
        public Tree<Module>.Node SelectedNode
        {
            get;
            set;
        }

        public Tree<Module>.Node SelectedConsumerNode
        {
            get;
            set;
        }

        public Tree<Module>.Node SelectedProviderNode
        {
            get;
            set;
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Allocate a simple integer ID to each node
        /// </summary>
        public void AllocateIds()
        {
            int i = 1;
            TreeIterator<Module> it = new TreeIterator<Module>(this.Hierarchy);

            Tree<Module>.Node node = it.Next();
            
            while (node != null)
            {
                node.NodeValue.Id = i;
                i++;
                node = it.Next();
            }
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// When called it sets the cyclic property of each module in a cyclic relationship
        /// </summary>
        public void AnalyseCyclicDepedencies()
        {
            TreeIterator<Module> it1 = new TreeIterator<Module>(this.Hierarchy);
            Tree<Module>.Node node1 = it1.Next();
            while (node1 != null)
            {
                TreeIterator<Module> it2 = new TreeIterator<Module>(this.Hierarchy);
                Tree<Module>.Node node2 = it2.Next();

                Module mod1 = node1.NodeValue;
                while (node2 != null)
                {
                    if (node1 != node2)
                    {
                        Module mod2 = node2.NodeValue;
                        Relation rel1 = mod1.GetRelation(mod2);
                        if (rel1 != null && rel1.Weight > 0)
                        {
                            Relation rel2 = mod2.GetRelation(mod1);
                            if (rel2 != null && rel2.Weight > 0)
                            {
                                rel1.IsCyclic = true;
                                rel2.IsCyclic = true;
                            }
                        }
                    }

                    node2 = it2.Next();
                }
                node1 = it1.Next();
            }
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// When called it rolls-up the dependency weights known at leaf module level to all branch modules
        /// higher up in the hierarchy (ascendants) creating the appropriate relation structures as required
        /// </summary>
        public void CalculateParentWeights()
        {
            TreeIterator<Module> it1 = new TreeIterator<Module>(this.Hierarchy);
            Tree<Module>.Node node1 = it1.Next(); // provider node
            while (node1 != null)
            {
                /*
                 * Calculate weight between provider (node1) and the ascendants of its consumers
                 */
                if (node1.HasChildren == false)
                {
                    // leaf node - find all other leaf nodes in relationship
                    // create relation with n1 and parents of n2
                    Module provider = node1.NodeValue;
                    TreeIterator<Module> it2 = new TreeIterator<Module>(Hierarchy);
                    Tree<Module>.Node n2 = it2.Next();
                    while (n2 != null)
                    {
                        if (n2 != node1 && n2.HasChildren == false)
                        {
                            Relation rel = provider.GetRelation(n2.NodeValue);
                            if (rel != null)
                            {
                                // relation found - add relation for each ascendents of rel.consumer
                                Tree<Module>.Node ascendent = n2.Parent;
                                while (ascendent != null && ascendent.NodeValue != null)
                                {
                                    provider.AddRelation(ascendent.NodeValue, rel.Weight);
                                    ascendent = ascendent.Parent;
                                }
                            }
                        }

                        n2 = it2.Next();
                    }
                }
                else
                {
                    Module branchProvider = node1.NodeValue;
                    // node1 is a branch - find all relations where the providers are descendents of this branch
                    TreeIterator<Module> it2 = new TreeIterator<Module>(Hierarchy);
                    Tree<Module>.Node d2 = it2.Next();
                    while (d2 != null)
                    {
                        if (d2 != node1 && d2.HasChildren == false && DsmModel.IsDescendent(node1, d2))
                        {
                            // d2 is a descedent - find relations

                            TreeIterator<Module> it3 = new TreeIterator<Module>(Hierarchy);
                            Tree<Module>.Node n3 = it3.Next();
                            while (n3 != null)
                            {
                                if (n3 != d2 && n3.HasChildren == false)
                                {
                                    Module provider = d2.NodeValue;
                                    Relation rel = provider.GetRelation(n3.NodeValue);

                                    if (rel != null)
                                    {
                                        // branch -> rel.consumer && branch to parent of each consumer
                                        branchProvider.AddRelation(rel.Consumer, rel.Weight);

                                        Tree<Module>.Node ascendent = n3.Parent;
                                        while (ascendent != null && ascendent.NodeValue != null)
                                        {
                                            branchProvider.AddRelation(ascendent.NodeValue, rel.Weight);
                                            ascendent = ascendent.Parent;

                                        }
                                    }
                                }

                                n3 = it3.Next();
                            }

                        }
                        d2 = it2.Next();
                    }
                }

                node1 = it1.Next();
            }
        }

        
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save the current model to an XML file
        /// </summary>
        /// <param name="filename"></param>
        public void SaveModel( string filename )
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(filename);
                sw.AutoFlush = true;

                XmlDocument doc = new XmlDocument();
                XmlElement root = doc.CreateElement("DSM");
                doc.AppendChild( root );

                root.SetAttribute( "type", ((int)Options.DsmModelType).ToString()  );

                root.SetAttribute("nestedhidden", Options.HideNestedClasses.ToString());

                XmlElement ass = doc.CreateElement("Assemblies");
                root.AppendChild( ass );

                int i = 1;
                foreach (string src in SourceFiles.Keys)
                {
                    XmlElement e = doc.CreateElement("Assembly");
                    e.SetAttribute( "idref", i.ToString());
                    e.SetAttribute( "name", src );
                    ass.AppendChild(e);
                    i++;
                }

                XmlElement mat = doc.CreateElement("Matrix");
                root.AppendChild( mat );

                TreeIterator<Module> it = new TreeIterator<Module>(this.Hierarchy);
                Tree<Module>.Node node = it.Next();

                while (node != null)
                {
                    Module module = node.NodeValue;

                    XmlElement e = doc.CreateElement("Module");
                    e.SetAttribute("idref", module.Id.ToString());
                    e.SetAttribute("name", module.Name);
                    e.SetAttribute("parent", 
                        node.parent.NodeValue == null ? "" : node.parent.NodeValue.Id.ToString());
                    e.SetAttribute("namespace", module.Namespace);
                    e.SetAttribute("nested", module.IsNested.ToString() );

                    mat.AppendChild(e);

                    foreach (Relation rel in module.Relations.Values)
                    {
                        XmlElement e2 = doc.CreateElement("Relation");
                        e2.SetAttribute("to", rel.Consumer.Id.ToString());
                        e2.SetAttribute("weight", rel.Weight.ToString());

                        e.AppendChild(e2);
                    }

                    node = it.Next();
                }
                sw.WriteLine(doc.OuterXml);
            }
            catch (Exception ex)
            {
                throw new DsmException("Error saving project to file", ex);
            }
            finally
            {
                if (sw != null) sw.Close();
            }
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Load a model from a previously saved DSM (XML) file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public static DsmModel LoadModel( string filename, DsmModel newModel)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            XmlNode attribute = xmlDoc.SelectSingleNode("//DSM/@type");

            if ( attribute != null )
            {
                newModel.Options.DsmModelType = (DsmOptions.ModelType)(int.Parse(attribute.InnerText));
            }

            attribute = xmlDoc.SelectSingleNode( "//DSM/@nestedhidden" );
            if ( attribute != null )
            {
                newModel.Options.HideNestedClasses = bool.Parse( attribute.InnerText );
            }

            XmlNodeList assemblies = xmlDoc.SelectNodes("//DSM/Assemblies/Assembly");

            //if (assemblies.Count < 1)
            //{
            //    throw new DsmException("No assemblies found in project file");
            //}

            XmlNodeList modules = xmlDoc.SelectNodes("//DSM/Matrix/Module");

            if (modules.Count < 1)
            {
                throw new DsmException( "No modules found in project file" );
            }

            foreach (XmlNode node in assemblies)
            {
                newModel.AddAssembly( node.Attributes["name"].InnerText);
            }

            Dictionary<string, Tree<Module>.Node> nodeMap = new Dictionary<string, Tree<Module>.Node>();

            newModel.Hierarchy = new Tree<Module>();
            newModel.BuildNumber = 0; // reset build number to zero
            //newModel.Hierarchy = new ModuleTree();

            foreach (XmlNode node in modules)
            {
                bool isNested = false;
                if ( node.Attributes["nested"] != null )
                {
                    isNested = bool.Parse( node.Attributes["nested"].InnerText );
                }

                Module m = newModel.CreateModule(node.Attributes["name"].InnerText, 
                   node.Attributes["namespace"].InnerText, null, isNested );
                
                //// Set hidden if nested classes are to be hidden
                //if (m.IsNested == true && newModel.Options.HideNestedClasses == true)
                //{
                //    m.IsHidden = true;
                //}

                string parentId = node.Attributes["parent"].InnerText;
                Tree<Module>.Node parentNode = null;
                if (nodeMap.ContainsKey(parentId))
                {
                    parentNode = nodeMap[parentId];
                }

                Tree<Module>.Node newNode = newModel.Add(m, m.FullName, parentNode, 0);
                nodeMap.Add(node.Attributes["idref"].InnerText, newNode);
            }

            // Once all _modules have been read we read in the relatio weights

            foreach (XmlNode node in modules)
            {
                string id = node.Attributes["idref"].InnerText;
                Module mod = nodeMap[id].NodeValue;

                XmlNodeList relations = node.SelectNodes("Relation");

                foreach (XmlNode relation in relations)
                {
                    string consumerId = relation.Attributes["to"].InnerText;
                    Module con = nodeMap[consumerId].NodeValue;

                    mod.AddRelation( con, int.Parse( relation.Attributes["weight"].InnerText ) );
                }
            }  

            return newModel;
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Build the model report
        /// </summary>
        /// <param name="sw"></param>
        public void DoReport(StreamWriter sw)
        {
            if( this.Options.DsmModelType.Equals( DsmOptions.ModelType.Logical ) )
            {
                DoLogicalReport(sw);
            }
            else
            {
                DoPhysicalReport(sw);
            }
        }

        //---------------------------------------------------------------------------------------------------
        void DoLogicalReport( StreamWriter sw )
        {
            try
            {
                XmlDocument xhtml = new XmlDocument();
                XmlNode root = xhtml.CreateNode(XmlNodeType.Element, "div", null );
                xhtml.AppendChild(root);

                XmlElement e = xhtml.CreateElement("h1");
                root.AppendChild(e)
                    .AppendChild(xhtml.CreateTextNode("Dependency Structure Report (Architecture Model)" ));

                //
                // Write out Assemblies currently being analysed
                //
                root.AppendChild( xhtml.CreateElement( "h2"))
                    .AppendChild( xhtml.CreateTextNode( "Assemblies Analysed" ) );

                XmlNode list = root.AppendChild(xhtml.CreateElement("ul"));

                foreach (string src in SourceFiles.Keys)
                {
                    list.AppendChild( xhtml.CreateElement( "li" ) ).AppendChild( xhtml.CreateTextNode( src ) );                   
                }


                //
                // Cyclic dependencies
                //
                root.AppendChild(xhtml.CreateElement("h2")).AppendChild(xhtml.CreateTextNode("Cyclic Dependencies"));

                list = root.AppendChild(xhtml.CreateElement("ul"));

                TreeIterator<Module> it = new TreeIterator<Module>(Hierarchy);
                Tree<Module>.Node node = it.Next();
                while (node != null)
                {
                    if (node.HasChildren == false)
                    {
                        Module mod = node.NodeValue;

                        TreeIterator<Module> it2 = new TreeIterator<Module>(Hierarchy);
                        Tree<Module>.Node n2 = it2.Next();
                        while( n2 != null )
                        {
                            if ( node != n2 && n2.HasChildren == false )
                            {
                                Module consumer = n2.NodeValue;

                                // This prevents us from writing out the same relation in the opposite direction 
                                if (mod.Id < consumer.Id)
                                {
                                    Relation rel = mod.GetRelation(n2.NodeValue);

                                    if (rel != null && rel.IsCyclic)
                                    {
                                        list.AppendChild(xhtml.CreateElement("li"))
                                        .AppendChild(xhtml.CreateTextNode(
                                            mod.FullName + " <---> " + rel.Consumer.FullName));
                                    }
                                }
                            }

                            n2 = it2.Next();
                        }
                        
                    }
                    node = it.Next();
                }

                if (list.SelectNodes("li").Count == 0)
                {
                    list.AppendChild(xhtml.CreateElement("li")).AppendChild(xhtml.CreateTextNode("N/A") );
                }
                //
                // Possible design breaks all relations in upper corner
                // if in same namespace - they are not reported for the moment
                //
                root.AppendChild(xhtml.CreateElement("h2"))
                    .AppendChild(xhtml.CreateTextNode("Potental Design Errors (upper triangle relations)"));

                root.AppendChild(xhtml.CreateElement("span"))
                    .AppendChild(xhtml.CreateTextNode("[Excludes relations between types in the same namespace]"));

                list = root.AppendChild(xhtml.CreateElement("ul"));

                it = new TreeIterator<Module>(Hierarchy);
                node = it.Next();
                while (node != null)
                {
                    if (node.HasChildren == false)
                    {
                        Module mod = node.NodeValue;

                        TreeIterator<Module> it2 = new TreeIterator<Module>(Hierarchy);
                        Tree<Module>.Node n2 = it2.Next();
                        while (n2 != null)
                        {
                            if (node != n2 && n2.HasChildren == false)
                            {
                                Module consumer = n2.NodeValue;

                                // Only if nodes are in top half of _matrix 
                                // and don't have the same parent (unless at top level )
                                if (mod.Id < consumer.Id &&
                                    !(node.Parent == n2.Parent && node.Depth > 1))
                                {
                                    Relation rel = mod.GetRelation(consumer);

                                    if (rel != null)
                                    {
                                        list.AppendChild(xhtml.CreateElement("li"))
                                        .AppendChild(xhtml.CreateTextNode(
                                            mod.FullName + " <--- " + rel.Consumer.FullName + " : " + rel.Weight));
                                    }
                                }
                            }

                            n2 = it2.Next();
                        }

                    }
                    node = it.Next();
                }

                if (list.SelectNodes("li").Count == 0)
                {
                    list.AppendChild(xhtml.CreateElement("li")).AppendChild(xhtml.CreateTextNode("N/A"));
                }

                sw.WriteLine(xhtml.OuterXml);
            }
            catch( Exception ex )
            {
                sw.WriteLine(ex.Message);
                throw;
            }
        }

        //--------------------------------------------------------------------------------------------------

        void DoPhysicalReport( StreamWriter sw )
        {
            //try
            //{
            //    XmlDocument xhtml = new XmlDocument();
            //    XmlNode root = xhtml.CreateNode(XmlNodeType.Element, "div", null );
            //    xhtml.AppendChild(root);

            //    XmlElement e = xhtml.CreateElement("h1");
            //    root.AppendChild(e)
            //        .AppendChild(xhtml.CreateTextNode("Dependency Structure Report (Deployment Model)"  ));

            //    //
            //    // Write out Assemblies currently being analysed
            //    //
            //    root.AppendChild( xhtml.CreateElement( "h2"))
            //        .AppendChild( xhtml.CreateTextNode( "Assemblies Analysed" ) );

            //    XmlNode list = root.AppendChild(xhtml.CreateElement("ul"));

            //    foreach (string src in this._sourceFiles.Keys)
            //    {
            //        list.AppendChild( xhtml.CreateElement( "li" ) ).AppendChild( xhtml.CreateTextNode( src ) );                   
            //    }

            //    root.AppendChild(xhtml.CreateElement("h2"))
            //        .AppendChild(xhtml.CreateTextNode("Assembly Dependencies"));

            //    Tree<Module>.Node node = Hierarchy.Root.FirstChild;

            //    while (node != null)
            //    {
            //        Module mod = node.NodeValue;
            //        if (mod.Relations.Count > 0)
            //        {
            //            root.AppendChild(xhtml.CreateElement("h3"))
            //                .AppendChild(xhtml.CreateTextNode(mod.FullName+ " is required by :"));

            //            if ( mod.Relations.Count > 0 )
            //            {
            //                list = root.AppendChild(xhtml.CreateElement("ul"));
            //                foreach (Relation rel in mod.Relations.Values)
            //                {
            //                    if (mod != rel.Consumer && rel.Consumer.Depth == mod.Depth)
            //                    {
            //                        list.AppendChild(xhtml.CreateElement("li"))
            //                            .AppendChild(xhtml.CreateTextNode(rel.Consumer.FullName));
            //                    }
            //                }
            //                if (list.SelectNodes("li").Count == 0)
            //                {
            //                    list.AppendChild(xhtml.CreateElement("li")).AppendChild(xhtml.CreateTextNode("N/A"));
            //                }
            //            }
            //        }

            //        node = node.NextSibling;
            //    }
                
            //    if (list.SelectNodes("li").Count == 0)
            //    {
            //        list.AppendChild(xhtml.CreateElement("li")).AppendChild(xhtml.CreateTextNode("N/A"));
            //    }

            //    sw.WriteLine(xhtml.OuterXml);
            //}
            //catch( Exception ex )
            //{
            //    sw.WriteLine(ex.Message);
            //    throw;
            //}
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Run a partition on the children of the currently selected module
        /// </summary>
        public void Partition()
        {
            try
            {
                Partitionner p = new Partitionner(Hierarchy);
                p.Partition(this.SelectedNode);

                this.AllocateIds();
            }
            catch (Exception e)
            {
                throw new DsmException("Matrix partitioning error", e);
            }
        }

        public IList<Relation> FindRelations(
            Tree<Module>.Node node1, Tree<Module>.Node node2)
        {
            IList<Relation> result = new List<Relation>();
            FindRelations(node1, node2, result);
            
            return result;
        }

        // Get a list of modules that make up the relation between provider and consumer
        void FindRelations(
            Tree<Module>.Node providerNode, Tree<Module>.Node consumerNode, IList<Relation> result )
        {

            var relations = providerNode.NodeValue.Relations;
            if (relations.Count == 0 || relations.ContainsKey(consumerNode.NodeValue) == false)
            {
                return;
            }
            else
            {
                IList<Tree<Module>.Node> leaves = new List<Tree<Module>.Node>();

                FindLeafNodes(providerNode, leaves);

                foreach (var leaf in leaves)
                {
                    foreach (var relation in leaf.NodeValue.Relations.Values)
                    {
                        var node = FindNode(relation.Consumer.FullName);

                        if ( node.HasChildren == false && ( consumerNode == node || IsDescendent(consumerNode, node ) ) )
                        {
                            result.Add(relation);
                        }
                    }
                }
            }
        }

        void FindLeafNodes(Tree<Module>.Node providerNode, IList<Tree<Module>.Node> nodes)
        {
            if (providerNode.HasChildren)
            {
                foreach (var node in providerNode.Children)
                {
                    FindLeafNodes(node, nodes);
                }
            }
            else
            {
                nodes.Add(providerNode);
            }
        }
       
        /******************************************************************************************/
        /* DsmModel Helper functions                                                                 */
        /******************************************************************************************/

        /// <summary>
        /// Returns true if node2 is a direct descendent of node1
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        static public bool IsDescendent(Tree<Module>.Node node1, Tree<Module>.Node node2)
        {
            if (node1 == null || node2 == null)
                throw new ArgumentNullException("Model::IsDescendent - Either node1 or node2 were null");

            if (node1 == node2)
                return false;

            Tree<Module>.Node testNode = node2;
            bool found = false;

            while ( testNode != null && ! found )
            {
                found = (node1 == testNode);
                testNode = testNode.parent;
            }
         
            return found;
        }

        //-------------------------------------------------------------------------------------------------
        static public bool HasCyclicRelation(Module module1, Module module2)
        {
            Relation relation = module1.GetRelation(module2);

            return (relation == null ? false : relation.IsCyclic);
        }
        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// Called before reanalysing to increment build number and clear existing relations
        /// </summary>
        public void StartBuild()
        {
            BuildNumber++;

            foreach (var node in branchLookup.Values)
            {
                if (node.NodeValue != null)
                    node.NodeValue.Relations.Clear();
            }
        }
    }
}
