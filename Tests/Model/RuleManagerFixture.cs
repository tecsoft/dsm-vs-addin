using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Tcdev.Dsm;
using Tcdev.Dsm.Model;
using Tcdev.Dsm.Model.Rules;

namespace Tcdev.Dsm.Tests.Model
{
    [TestFixture]
    public class RuleManagerFixture
    {
        [Test]
        public void at_Detect_Dependency_Default_Upper_Triangle_Infraction()
        {
            var model = new TestModel();
            var A = model.Add( "A" );
            var B = model.Add( "B" );
            var C = model.Add( "C" );

            var nodeA = model.Add( A, null );
            var nodeB = model.Add( B, null );
            var nodeC = model.Add( C, null );

            // C can provide to A and B
            // A and B cannot provide to C

            model.SetRelation( C, B, 1 );
            Assert.IsTrue( B.Id < C.Id );
            Assert.IsNull( model.GetInfraction( C, B ) ); // lower triangle relation

            Assert.IsTrue( A.Id < C.Id );
            Assert.IsNull( model.GetInfraction( A, C ) ); // upper triangle no relation

            model.SetRelation( A, B, 1 );
            Assert.IsTrue( A.Id < B.Id );
            Assert.IsNotNull( model.GetInfraction( A, B ) ); // upper tirangle infraction
        }

        [Test]
        public void at_Detect_Overriden_Default_Upper_Triangle_Infraction()
        {
            var model = new TestModel();
            var A = model.Add( "A" );
            var B = model.Add( "B" );

            var nodeA = model.Add( A, null );
            var nodeB = model.Add( B, null );

            Assert.IsTrue( A.Id < B.Id ); // A->B is upper triangle relation
            model.SetRelation( A, B, 1 );

            Assert.IsNotNull( model.GetInfraction( A, B ) );

            // add allow dependency

            model.AddAllowDependencyRule( A, B );

            Assert.IsNull( model.GetInfraction( A, B ) );
        }

        [Test]
        public void at_Detect_Infraction_For_Descendents()
        {
            var model = new TestModel();
            var A = model.Add( "A" );
            var B = model.Add( "B" );
            var A1 = model.Add( "A1" );
            var A11 = model.Add( "A11" );
            var B1 = model.Add( "B1" );
            var B11 = model.Add( "B11" );
            var C = model.Add( "C" );

            var nodeA = model.Add( A, null );
            var nodeB = model.Add( B, null );
            var nodeA1 = model.Add( A1, nodeA );
            var nodeA11 = model.Add( A11, nodeA1 );
            var nodeB1 = model.Add( B1, nodeB );
            var nodeB11 = model.Add( B1, nodeB1 );
            var nodeC = model.Add( C, null );

            model.SetRelation( C, B, 1 );
            model.SetRelation( C, A, 1 );
            model.SetRelation( B, A, 1 );
            model.SetRelation( C, B1, 1 );
            model.SetRelation( C, B11, 1 );
            model.SetRelation( C, A1, 1 );
            model.SetRelation( C, A11, 1 );
            model.SetRelation( B, A1, 1 );
            model.SetRelation( B, A11, 1 );
            model.SetRelation( B1, A1, 1 );
            model.SetRelation( B11, A1, 1 );
            model.SetRelation( B1, A11, 1 );

            model.AddDeniedDependencyRule( B1, A1 );

            model.Print();

            Assert.IsNotNull( model.GetInfraction( B1, A1 ) );
            Assert.IsNotNull( model.GetInfraction( B11, A1 ) );
            Assert.IsNotNull( model.GetInfraction( B1, A11 ) );
            Assert.IsNotNull( model.GetInfraction( B1, A1 ) );

            Assert.IsNull( model.GetInfraction( B1, A ) );
            Assert.IsNull( model.GetInfraction( B, A1 ) );
            Assert.IsNull( model.GetInfraction( C, A ) );
            Assert.IsNull( model.GetInfraction( C, A1 ) );
            Assert.IsNull( model.GetInfraction( C, A11 ) );
            Assert.IsNull( model.GetInfraction( C, B ) );
            Assert.IsNull( model.GetInfraction( C, B1 ) );
            Assert.IsNull( model.GetInfraction( C, B11 ) );
        }

        //[Test]
        //public void Test_Add_Disallowed_Dependency()
        //{
        //    DsmModel sut = new DsmModel();
        //    Module m1 = new Module( "type", null, "namespace1", "assembly", false );
        //    Module m2 = new Module( "type2", null, "namespace1", "assembly", false );
        //    Module m3 = new Module( "type3", null, "namespace2", "assembly", false );
        //    sut.BuildHierarchy( new List<Module>() { m1, m2, m3 } );

        //    sut.RuleManager.Add( new CannotUseRule( m1, m2 ) );

        //    Assert.AreEqual( 1, sut.RuleManager.Rules.Count );
        //    var rule = sut.RuleManager.Rules[0];

        //    Assert.AreEqual( m1.FullName, rule.Provider.FullName );
        //    Assert.AreEqual( m2.FullName, rule.Consumer.FullName );
        //}

        [Test]
        public void Disallowed_Dependency_Violated_When_Relation_Between_Provider_And_Consumer()
        {
        }
    }
}