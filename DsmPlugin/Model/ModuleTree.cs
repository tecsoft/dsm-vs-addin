using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tcdev.Collections.Generic;

namespace Tcdev.Dsm.Model
{
    public class ModuleTree
    {
        public Tree<Module> tree = new Tree<Module>();
        Dictionary<string, Tree<Module>.Node> branchLookup = new Dictionary<string, Tree<Module>.Node>();
        public bool Contains(string key)
        {
            return branchLookup.ContainsKey(key);
        }
        public Tree<Module>.Node Get(string key)
        {
            return branchLookup[key];
        }
        public Tree<Module>.Node Add(Module module, string key, Tree<Module>.Node parentNode, int buildNumber)
        {
            module.BuildNumber = buildNumber;
            Tree<Module>.Node n = tree.CreateNode(module);

            if (parentNode != null)
                n.Depth = parentNode.Depth + 1;

            tree.Add(parentNode, n);

            // new
            if (Contains(key) == false)
                branchLookup.Add(key, n);

            return n;
        }

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

                tree.Remove(node);
            }
        }

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

        public void RemoveOldItems(int buildNumber)
        {
            Tree<Module>.Node current = tree.Root;

            RemoveIfOld(buildNumber, current);

        }
    }
}
