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
    public class MarkPropertiesFixture : FixtureHelper
    {
        class Consumer { public Provider Property { get; set; } }
        class Provider { }

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
        public void Test_Consumer_Is_Consumer_Of_Provider_As_Property_Type()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Consumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Provider"));
            
            Analyser.MarkProperties(consumer);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(1,  modProvider.Relations.Count );
            Assert.AreEqual(1, modProvider.Relations[modConsumer].Weight);
        }
    }
}
