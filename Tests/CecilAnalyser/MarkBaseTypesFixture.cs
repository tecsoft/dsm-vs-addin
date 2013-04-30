using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tcdev.Dsm.Engine;
using Tcdev.Collections.Generic;
using Tcdev.Dsm.Model;

namespace Tcdev.Dsm.Tests
{

    [TestFixture]
    public class MarkBaseTypesFixture : FixtureHelper
    {
        private interface Ignore { void Method(); };
        private class Consumer : Provider, Ignore {}
        private class Provider { public void Method() { return; } }


        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            base.FixtureSetup();
        }

        [SetUp]
        public void Setup()
        {
            base.TestSetup();
        }

        //[TearDown]
        //public void TearDown()
        //{
        //    base.TearDown();
        //}

        [Test]
        public void Test_BaseType_Provider()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Consumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x,"Provider"));

            Analyser.MarkBaseType(consumer);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(1, modProvider.Relations.Count);
            Assert.AreEqual(1, modProvider.Relations[modConsumer].Weight);
        }
    }
}
