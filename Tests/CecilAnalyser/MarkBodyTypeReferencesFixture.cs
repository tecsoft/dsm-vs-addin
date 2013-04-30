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
    public class MarkBodyTypeReferencesFixture : FixtureHelper
    {
        class Consumer { 
            public void ConstructorCall() { Provider p = new Provider(); }
            public void MethodCall() { Provider p = null; p.MethodCall(); }
            public void StaticMethodCall() { Provider.StaticMethod(); }
            public void CastingMethod( Provider p ) { ChildConsumer c = (ChildConsumer)p; }
        }
        class Provider {
            public Provider() {;}
            public void MethodCall() { ; }
            public static void StaticMethod() { ; }
        }

        class ChildConsumer : Provider{
            public ChildConsumer() : base() { ; }
            public void BaseMethodCall() { MethodCall(); }
        }



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
        public void Test_Consumer_Is_Consumer_Of_Provider_Via_Constructor()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Consumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Provider"));
            var method = consumer.Methods.FirstOrDefault(x => "ConstructorCall".Equals(x.Name));

            Analyser.MarkBodyTypeReferences(consumer, method.Body) ;

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(1,  modProvider.Relations.Count );
            Assert.AreEqual(1, modProvider.Relations[modConsumer].Weight);
        }

        [Test]
        public void Test_Consumer_Is_Consumer_Of_Provider_Via_MethodCall()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Consumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Provider"));
            var method = consumer.Methods.FirstOrDefault(x => "MethodCall".Equals(x.Name));

            Analyser.MarkBodyTypeReferences(consumer, method.Body);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(1, modProvider.Relations.Count);
            Assert.AreEqual(1, modProvider.Relations[modConsumer].Weight);
        }

        [Test]
        public void Test_Consumer_Is_Consumer_Of_Provider_Via_StaticCall()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Consumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Provider"));
            var method = consumer.Methods.FirstOrDefault(x => "StaticMethodCall".Equals(x.Name));

            Analyser.MarkBodyTypeReferences(consumer, method.Body);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(1, modProvider.Relations.Count);
            Assert.AreEqual(1, modProvider.Relations[modConsumer].Weight);
        }

        [Test]
        public void Test_ChildConsumer_Is_Consumer_Of_Provider_Via_Base_Constructor_Call()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "ChildConsumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Provider"));
            var method = consumer.Methods.FirstOrDefault(x => ".ctor".Equals(x.Name));

            Analyser.MarkBodyTypeReferences(consumer, method.Body);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(1, modProvider.Relations.Count);
            Assert.AreEqual(1, modProvider.Relations[modConsumer].Weight);
        }

        [Test]
        public void Test_ChildConsumer_Is_Consumer_Of_Provider_Via_BaseMethod_Call()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "ChildConsumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Provider"));
            var method = consumer.Methods.FirstOrDefault(x => "BaseMethodCall".Equals(x.Name));

            Analyser.MarkBodyTypeReferences(consumer, method.Body);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(1, modProvider.Relations.Count);
            Assert.AreEqual(1, modProvider.Relations[modConsumer].Weight);
        }

        [Test]
        public void Test_Consumer_Is_Consumer_Of_ChildConsumer_Via_Cast()
        {
            var consumer = Analyser.Types.FirstOrDefault(x => IsNamed(x, "Consumer"));
            var provider = Analyser.Types.FirstOrDefault(x => IsNamed(x, "ChildConsumer"));
            var method = consumer.Methods.FirstOrDefault(x => "CastingMethod".Equals(x.Name));

            Analyser.MarkBodyTypeReferences(consumer, method.Body);

            var modProvider = Analyser.Modules[provider.FullName];
            Assert.IsNotNull(modProvider);

            var modConsumer = Analyser.Modules[consumer.FullName];
            Assert.IsNotNull(modConsumer);

            Assert.AreEqual(1, modProvider.Relations.Count);
            Assert.AreEqual(1, modProvider.Relations[modConsumer].Weight);
        }
    }
}
