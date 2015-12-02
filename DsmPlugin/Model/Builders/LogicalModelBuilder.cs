using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tcdev.Collections.Generic;

namespace Tcdev.Dsm.Model.Builders
{
    public class LogicalModelBuilder : IModelBuilder
    {
        public void Build(DsmModel model, int BuildNumber, IEnumerable<Module> typeModules)
        {
            foreach (Module module in typeModules)
            {
                Tree<Module>.Node parentNode = null;

                bool exists = model.Contains(module.FullName);
                if (exists)
                {
                    parentNode = model.Get(module.FullName);
                    parentNode.NodeValue.BuildNumber = BuildNumber;

                    if (parentNode.Parent != null && parentNode.Parent.NodeValue != null
                        && "*".Equals(parentNode.Parent.NodeValue.Name))
                        parentNode.Parent.NodeValue.BuildNumber = BuildNumber;
                }
                string[] tokens = module.Namespace.Split('.');
                if (tokens[0].Length > 0)  // ignore .<Module> for the moment (this should be done by the analyser i.e. don't provider it
                {
                    string namespacePortion = string.Empty;

                    foreach (string token in tokens)
                    {
                        namespacePortion = (namespacePortion.Length > 0)
                            ? namespacePortion + "." + token : token;

                        if (exists || model.Contains(namespacePortion))
                        {
                            parentNode = model.Get(namespacePortion);
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

                            Module m = model.CreateModule(token, nspace, null, false);
                            parentNode = model.Add(m, m.FullName, parentNode, BuildNumber);
                        }
                    }

                    if (!exists)
                    {
                        Module copy = model.CreateModule(module.Name, module.Namespace, module.AssemblyName, module.IsNested);
                        Tree<Module>.Node node = model.Add(copy, copy.FullName, parentNode, BuildNumber);//tree.CreateNode(module);
                        node.IsHidden = (module.IsNested == true && model.Options.HideNestedClasses == true);
                    }
                }
            }
        }
    }
}