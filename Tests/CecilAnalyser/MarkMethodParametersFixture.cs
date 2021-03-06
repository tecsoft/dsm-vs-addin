﻿using System;
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
    public class MarkMethodParametersFixture : FixtureHelper
    {
        class Consumer {
            public void Method(Provider provider) { ; }
            void VoidMethod() { ; }
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
        public void Test_Consumer_Consumes_Method_With_Parameter_Provider()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Consumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Provider"));
            var method = consumer.Methods.FirstOrDefault( x => "Method".Equals(x.Name));

            Analyser.MarkMethodParameters(consumer, method);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(1,  modProvider.Relations.Count );
            Assert.AreEqual(1, modProvider.Relations[modConsumer].Weight);
        }

        [Test]
        public void Test_Consumer_Void_Parameter_Ignored()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Consumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Provider"));
            var method = consumer.Methods.FirstOrDefault(x => "VoidMethod".Equals(x.Name));

            Analyser.MarkMethodParameters(consumer, method);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(0, modProvider.Relations.Count);
        }
    }
}
