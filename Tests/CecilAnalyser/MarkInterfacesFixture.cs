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
    public class MarkInterfacesFixture : FixtureHelper
    { 
        interface Provider { void Method(); };
        class Consumer : Provider {public void Method(){return;}}
        class Independent { public void Method() { return; } }
        
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
        public void Test_Independent_Class_Has_No_Relation_To_Interface()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Independent"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Provider"));

            Analyser.MarkInterfaces(consumer);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(0, modProvider.Relations.Count);
        }

        [Test]
        public void Test_Class_Is_Consumer_Of_Interface()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Consumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Provider"));
                
            Analyser.MarkInterfaces(consumer);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(1,  modProvider.Relations.Count );
            Assert.AreEqual(1, modProvider.Relations[modConsumer].Weight);
        }
    }
}
