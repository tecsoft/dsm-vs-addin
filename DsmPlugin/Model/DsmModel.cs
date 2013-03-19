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

namespace Tcdev.Dsm.Model
{
    /// <summary>
    /// DsmModel is based around the Module _hierarchy held in form of a tree.
    /// DsmModel provides methods for its contruction and modification
    /// </summary>
    public class DsmModel 
    {    
        private Dictionary<string, object> _sourceFiles;       
        private IList<Module>              _modules;
        private Tree<Module>.Node          _selectedNode;
        private Tree<Module>               _hierarchy;
        private bool                       _isModified;
        private DsmOptions                 _options;

        static Logger _log = new Logger(Path.Combine( Path.GetTempPath() ,"model.txt" ));
        
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        public DsmModel()
        {
            _modules      = new List<Module>();
            _sourceFiles  = new Dictionary<string, object>();
            _isModified   = false;
            _selectedNode = null;
            _options      = new DsmOptions();  // created with a default set of options
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// Tree of Modules representing the _hierarchy of types
        /// </summary>
        public Tree<Module> Hierarchy
        {
            get { return _hierarchy; }
        }
        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Get or set the DsmOptions
        /// </summary>
        public DsmOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Set to true if _model has been modified and would require the user to demand its serialization
        /// if they want to keep the changes
        /// </summary>
        public bool Modified
        {
            get { return _isModified; }
            set { _isModified = value; }
        }
        
        //-----------------------------------------------------------------------------------------

        //TODO Change so that it is done automatically in call CreateModule
        public void AddAssembly(string src)
        {
            if (!_sourceFiles.ContainsKey( src ) )
            {
                _sourceFiles.Add(src, null);
            }
        }
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates a module in the _model but without any particular placement in a _hierarchy
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="namespaceName"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public Module CreateModule(string typeName, string namespaceName, string assemblyName, bool isNested )
        {
            Module m = new Module(typeName, null, namespaceName, assemblyName, isNested );
            _modules.Add(m);
            return m;
        }
        
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Construct the module _hierarchy according to namespace ( logical _model) or by assembly
        /// (physical _model).  For the logical _model, modify the hierarchy so that a *.* namespace is
        /// added to contain the types at each level if desired
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="useAsterix"></param>
        public void BuildHierarchy()
        {
            if (this.Options.DsmModelType == DsmOptions.ModelType.Physical )
            {
                _hierarchy = PhysicalModelBuilder( _modules );
            }
            else
            {
                _hierarchy = LogicalModelBuilder( _modules );
                
                // clean up hanging leaf nodes to a * namespace
                BalanceLeafNodes(_hierarchy);
            }
        }

        //-------------------------------------------------------------------------------------------------

        Tree<Module> LogicalModelBuilder( IList<Module> typeModules )
        {
            // the tree hierarchy which represents the model
            Tree<Module> tree = new Tree<Module>();
            // dictionary to remember existing tree nodes 
            Dictionary<string, Tree<Module>.Node> branchLookup = new Dictionary<string, Tree<Module>.Node>();

            foreach( Module module in typeModules )
            {
                

                Tree<Module>.Node parentNode = null;
                if (branchLookup.ContainsKey(module.Namespace))
                {
                    parentNode = branchLookup[module.Namespace];
                }

                if (parentNode == null)
                {
                    // need to create the parts of this hierarchy that don't exist
                    string[] tokens = module.Namespace.Split('.');

                    if (tokens[0].Length > 0)  // ignore .<Module> for the moment
                    {
                        string namespacePortion = string.Empty;

                        foreach (string token in tokens)
                        {
                            if (namespacePortion.Length > 0)
                            {
                                namespacePortion += "." + token;
                            }
                            else
                            {
                                namespacePortion = token;
                            }

                            if (!branchLookup.ContainsKey(namespacePortion))
                            {
                                // create a new module
                                string nspace = null;
                                int pos = namespacePortion.LastIndexOf('.');
                                if (pos != -1)
                                {
                                    nspace = namespacePortion.Substring(0, pos);
                                }

                                Module m = new Module(token, null, nspace, null, false);

                                Tree<Module>.Node n = tree.CreateNode(m);
                                
                                if ( parentNode != null )
                                    n.Depth = parentNode.Depth + 1;
                                    
                                tree.Add(parentNode, n);
                                parentNode = n;
                                branchLookup.Add(namespacePortion, parentNode);
                            }
                            else
                            {
                                parentNode = branchLookup[namespacePortion];
                            }
                        }
                    }
                }

                Tree<Module>.Node node = tree.CreateNode(module);

                node.IsHidden = (module.IsNested == true && Options.HideNestedClasses == true);
                
                if (parentNode != null)
                {
                    node.Depth = parentNode.Depth + 1;
                }
                tree.Add(parentNode, node);
            }

            return tree;
        }

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
        void BalanceLeafNodes(Tree<Module> tree)
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
            Balance(tree, tree.Root);

            TreeIterator<Module> it = new TreeIterator<Module>(tree);
            Tree<Module>.Node node = it.Next();

            while (node != null)
            {
                if (node.HasChildren)
                {
                    Balance(tree, node);
                }

                node = it.Next();
            }
        }

        //-------------------------------------------------------------------------------------------------

        void Balance(Tree<Module> tree, Tree<Module>.Node parentNode)
        {
            // for the given node if it contains leafs and branches, create a new sub branch * and move
            // all leaf nodes into it
            Tree<Module>.Node node = parentNode.firstChild;

            IList<Tree<Module>.Node> list = new List<Tree<Module>.Node>();
            bool branchFound = false;
            while (node != null)
            {
                if (node.HasChildren == false)
                {
                    list.Add(node);
                }
                else
                {
                    branchFound = true;
                }

                node = node.nextSibling;
            }

            if (branchFound && list.Count > 0)
            {
                string nspace = null;
                if ( parentNode.NodeValue != null )
                {
                    nspace = parentNode.NodeValue.FullName;
                }

                Module m = new Module("*", null, nspace, null, false);

                Tree<Module>.Node asterixNode = tree.CreateNode(m);
                if (parentNode != tree.Root)
                {
                    asterixNode.Depth = parentNode.Depth + 1;
                }
                tree.Add(parentNode, asterixNode);
                foreach (Tree<Module>.Node leaf in list)
                {
                    tree.Remove(leaf);
                    tree.Add(asterixNode, leaf);
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
            return (_selectedNode != null && _selectedNode.nextSibling != null);
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
                Tree<Module>.Node node = _selectedNode.nextSibling;
                _hierarchy.Remove(node);
                _hierarchy.InsertBefore(node, _selectedNode);

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
            return (_selectedNode != null && _selectedNode.previousSibling != null);
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
                Tree<Module>.Node node = _selectedNode.previousSibling;
                _hierarchy.Remove(_selectedNode);
                _hierarchy.InsertBefore(_selectedNode, node);

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
            get { return _selectedNode; }
            set { _selectedNode = value; }
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
            TreeIterator<Module> it1 = new TreeIterator<Module>(this._hierarchy);
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
                    TreeIterator<Module> it2 = new TreeIterator<Module>(_hierarchy);
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
                    TreeIterator<Module> it2 = new TreeIterator<Module>(_hierarchy);
                    Tree<Module>.Node d2 = it2.Next();
                    while (d2 != null)
                    {
                        if (d2 != node1 && d2.HasChildren == false && DsmModel.IsDescendent(node1, d2))
                        {
                            // d2 is a descedent - find relations

                            TreeIterator<Module> it3 = new TreeIterator<Module>(_hierarchy);
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
                foreach (string src in this._sourceFiles.Keys)
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

            if (assemblies.Count < 1)
            {
                throw new DsmException("No assemblies found in project file");
            }

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

            newModel._hierarchy = new Tree<Module>();

            foreach (XmlNode node in modules)
            {
                bool isNested = false;
                if ( node.Attributes["nested"] != null )
                {
                    isNested = bool.Parse( node.Attributes["nested"].InnerText );
                }
                Module m = new Module(node.Attributes["name"].InnerText, null, null, null, isNested );

                //// Set hidden if nested classes are to be hidden
                //if (m.IsNested == true && newModel.Options.HideNestedClasses == true)
                //{
                //    m.IsHidden = true;
                //}

                Tree<Module>.Node newNode = newModel._hierarchy.CreateNode(m);
                
                

                string parentId = node.Attributes["parent"].InnerText;
                Tree<Module>.Node parentNode = null;
                if (nodeMap.ContainsKey(parentId))
                {
                    parentNode = nodeMap[parentId];
                }

                newModel._hierarchy.Add( parentNode, newNode );

                nodeMap.Add(node.Attributes["idref"].InnerText, newNode);
                
                newNode.IsHidden = (m.IsNested == true && newModel.Options.HideNestedClasses == true);
                if (parentNode != null)
                {
                    newNode.Depth = parentNode.Depth + 1;
                }            
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
            //if (modelType.Equals(ModelType.Logical))
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

                foreach (string src in this._sourceFiles.Keys)
                {
                    list.AppendChild( xhtml.CreateElement( "li" ) ).AppendChild( xhtml.CreateTextNode( src ) );                   
                }


                //
                // Cyclic dependencies
                //
                root.AppendChild(xhtml.CreateElement("h2")).AppendChild(xhtml.CreateTextNode("Cyclic Dependencies"));

                list = root.AppendChild(xhtml.CreateElement("ul"));

                TreeIterator<Module> it = new TreeIterator<Module>(_hierarchy);
                Tree<Module>.Node node = it.Next();
                while (node != null)
                {
                    if (node.HasChildren == false)
                    {
                        Module mod = node.NodeValue;

                        TreeIterator<Module> it2 = new TreeIterator<Module>(_hierarchy);
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

                it = new TreeIterator<Module>(_hierarchy);
                node = it.Next();
                while (node != null)
                {
                    if (node.HasChildren == false)
                    {
                        Module mod = node.NodeValue;

                        TreeIterator<Module> it2 = new TreeIterator<Module>(_hierarchy);
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
                Tree<Module> tree = this.Hierarchy;

                PartitionLoop(this.SelectedNode, true);

                _log.Trace("Partition done");

                this.AllocateIds();
            }
            catch (Exception e)
            {
                throw new DsmException("Matrix partitioning error", e);
            }
        }
        //-------------------------------------------------------------------------------------------------
        void PartitionLoop( Tree<Module>.Node parent, bool descend )
        {
            PartitionGroup(parent.Children);
        }
        //-------------------------------------------------------------------------------------------------
        void PartitionGroup( IList<Tree<Module>.Node>  nodes )
        {            
            if ( nodes.Count > 1)
            {
                SquareMatrix matrix = BuildPartitionMatrix( nodes );

                PartitionerMarkII p = new PartitionerMarkII(matrix);

                Vector v = p.Partition();
                
                ReorderNodes(nodes, v);

                _log.Trace( "reorder done" );
            }
        }
        //-------------------------------------------------------------------------------------------------
        SquareMatrix BuildPartitionMatrix(IList<Tree<Module>.Node> nodes)
        {
            SquareMatrix matrix = new SquareMatrix(nodes.Count);

            for (int i = 0; i < nodes.Count; i++)
            {
                Module provider = nodes[i].NodeValue;

                for (int j = 0; j < nodes.Count; j++)
                {
                    if (j != i)
                    {
                        Module consumer = nodes[j].NodeValue;

                        Relation relation = provider.GetRelation(consumer);

                        if (relation != null && relation.Weight > 0)
                        {
                            matrix.Set(i, j, 1);
                        }
                        else
                        {
                            matrix.Set(i, j, 0);
                        }
                    }
                }
            }

            return matrix;
        }
        //-------------------------------------------------------------------------------------------------
        void ReorderNodes(IList<Tree<Module>.Node> nodes, Vector permutationVector)
        {
            Tree<Module>.Node parentNode = nodes[0].Parent;
            
            foreach (Tree<Module>.Node node in parentNode.Children)
            {
                this.Hierarchy.Remove(node);
            }

            for (int i = 0; i < permutationVector.Size; i++)
            {
                this.Hierarchy.Add(parentNode, nodes[permutationVector.Get(i)]);
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
    }
}
