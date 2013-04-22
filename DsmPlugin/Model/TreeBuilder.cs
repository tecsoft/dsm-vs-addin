using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tcdev.Collections.Generic;

namespace Tcdev.Dsm.Model
{
    internal class TreeBuilder
    {
        internal Tree<Module> Tree;
        internal Dictionary<string, Tree<Module>.Node> branchLookup;
        internal DsmOptions Options { get; private set; }
        public TreeBuilder(DsmOptions options)
        {
            Tree = new Tree<Module>();
            branchLookup = new Dictionary<string, Tree<Module>.Node>();
        }

        public Tree<Module>.Node NamespaceNode(Module module)
        {
            Tree<Module>.Node parentNode = null;
            if (branchLookup.ContainsKey(module.Namespace))
            {
                parentNode = branchLookup[module.Namespace];
            }

            return parentNode;
        }

        public Tree<Module>.Node BuildNamespace(Module module)
        {
            Tree<Module>.Node node = null;
            // need to create the parts of the hierarchy that don't exist
            string[] tokens = module.Namespace.Split('.');

            if (tokens[0].Length > 0)  // ignore .<Module> for the moment
            {
                string namespacePortion = string.Empty;

                foreach (string token in tokens)
                {
                    namespacePortion = (namespacePortion.Length > 0) ? namespacePortion + "." + token : token;

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

                        Tree<Module>.Node n = Tree.CreateNode(m);

                        if (node != null)
                            n.Depth = node.Depth + 1;

                        Tree.Add(node, n);
                        node = n;
                        branchLookup.Add(namespacePortion, node);
                    }
                    else
                    {
                        node = branchLookup[namespacePortion];
                    }
                }
            }

            Tree<Module>.Node node = Tree.CreateNode(module);

            node.IsHidden = (module.IsNested == true && Options.HideNestedClasses == true);

            if (node != null)
            {
                node.Depth = node.Depth + 1;
            }
            Tree.Add(node, node);

            return node;
        }
    }
}
