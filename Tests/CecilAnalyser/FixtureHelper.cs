using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tcdev.Dsm.Engine;

namespace Tcdev.Dsm.Tests
{
    public class FixtureHelper
    {
        Target _testTarget = null;
        string _root;
        CecilAnalyser _analyser;

        protected string Name(string t)
        {
            return _root + t;
        }
        protected CecilAnalyser Analyser
        {
            get { return _analyser; }
        }

        protected bool IsNamed(Mono.Cecil.TypeDefinition type, string shortName)
        {
            return Name(shortName).Equals(type.FullName);
        }

        protected void TestSetup()
        {
            _analyser = new CecilAnalyser();
            _analyser.Model = new Tcdev.Dsm.Model.DsmModel();
            _analyser.IncludeAssembly(_testTarget);
            _analyser.LoadTypes();
        }

        protected void TearDown()
        {
            _analyser.Dispose();
        }

        protected void FixtureSetup()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            _testTarget = new Target(assembly.GetName().Name, assembly.Location);
            _root = this.GetType().FullName + "/";
        }
    }
}
