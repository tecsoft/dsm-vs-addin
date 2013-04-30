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
    public class MarkGenericMethodParametersFixture : FixtureHelper
    {
        class Consumer {
            public void Method<T>(T t ) where T : Provider{ ; }
            public void IgnoreMethod<T>(T t) where T : class { ; }
        }
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
        public void Test_Consumer_Consumes_Method_With_Generic_Parameter_Constrained_As_Provider()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Consumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Provider"));
            var method = consumer.Methods.FirstOrDefault( x => "Method".Equals(x.Name));

            Analyser.MarkGenericMethodParameters(consumer, method);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(1,  modProvider.Relations.Count );
            Assert.AreEqual(1, modProvider.Relations[modConsumer].Weight);
        }

        [Test]
        public void Test_Consumer_Class_Constrained_GenericParameter_Ignored()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Consumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Provider"));
            var method = consumer.Methods.FirstOrDefault(x => "IgnoreMethod".Equals(x.Name));

            Analyser.MarkGenericMethodParameters(consumer, method);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(0, modProvider.Relations.Count);
        }
    }
}
