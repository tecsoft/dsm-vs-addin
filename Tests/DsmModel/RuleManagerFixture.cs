using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Tcdev.Dsm;
using Tcdev.Dsm.Model;
using Tcdev.Dsm.Model.DependencyRules;

namespace Tcdev.Dsm.Tests.DsmModel
{
    [TestFixture]
    public class RuleManagerFixture
    {
        [Test]
        public void Test_Create_Disallowed_Dependency()
        {
            Model.DsmModel sut = new Model.DsmModel();
            Module m1 = new Module("type", null, "namespace1", "assembly", false);
            Module m2 = new Module("type2", null, "namespace1", "assembly", false);
            Module m3 = new Module("type3", null, "namespace2", "assembly", false);
            sut.BuildHierarchy(new List<Module>() { m1, m2, m3 });

            sut.RuleManager.Add(new CannotUseRule(m1, m2));

            Assert.AreEqual(1, sut.RuleManager.Rules(m1).Count);
            var rule = sut.RuleManager.Rules(m1)[0];

            Assert.AreEqual(m1.FullName, rule.Provider.FullName);
            Assert.AreEqual(m2.FullName, rule.Consumer.FullName);
        }

        [Test]
        public void Disallowed_Dependency_Violated_When_Relation_Between_Provider_And_Consumer()
        {
        }
    }
}