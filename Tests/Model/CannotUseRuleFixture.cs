using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tcdev.Dsm.Model;
using Tcdev.Dsm.Model.Rules;

namespace Tcdev.Dsm.Tests.Model
{
    [TestFixture]
    public class CannotUseRuleFixture
    {
        [Test]
        public void IsViolated_True_When_Rule_Defined_For_A_To_B_And_Relation_Exists()
        {
            var model = new TestModel();
            var A = model.Add( "A" );
            var B = model.Add( "B" );
            model.SetRelation( A, B, 1 );

            var sut = new CannotUseRule( A, B );

            Assert.IsTrue( sut.IsViolated( A, B, model ) );
        }

        [Test]
        public void IsViolated_False_When_Rule_Defined_For_A_To_B_But_No_Relation_Exists()
        {
            var model = new TestModel();
            var A = model.Add( "A" );
            var B = model.Add( "B" );
            model.SetRelation( A, B, 0 );

            var sut = new CannotUseRule( A, B );

            Assert.IsFalse( sut.IsViolated( A, B, model ) );
        }

        [Test]
        public void IsViolated_False_When_Rule_Defined_For_A_To_B_Even_If_Relation_B_To_A()
        {
            var model = new TestModel();
            var A = model.Add( "A" );
            var B = model.Add( "B" );
            model.SetRelation( B, A, 1 );

            var sut = new CannotUseRule( A, B );

            Assert.IsFalse( sut.IsViolated( A, B, model ) );
        }

        [Test]
        public void IsViolated_True_When_Rule_Defined_For_A_To_B_For_ChildA_With_Relation()
        {
            var model = new TestModel();
            var A = model.Add( "A" );
            var B = model.Add( "B" );
            var ChildA = model.Add( "Child" );
            var nodeA = model.Add( A, null );
            model.Add( ChildA, nodeA );
            model.Add( B, null );

            model.SetRelation( ChildA, B, 1 );

            var sut = new CannotUseRule( A, B );

            Assert.IsTrue( sut.IsViolated( ChildA, B, model ) );
        }

        [Test]
        public void IsViolated_True_When_Rule_Defined_For_A_To_B_For_ChildB_With_Relation()
        {
            var model = new TestModel();
            var A = model.Add( "A" );
            var B = model.Add( "B" );
            var ChildB = model.Add( "Child" );
            var nodeA = model.Add( A, null );
            var nodeB= model.Add( B, null );
            model.Add( ChildB, nodeB );

            model.SetRelation( A, ChildB, 1 );

            var sut = new CannotUseRule( A, B );

            Assert.IsTrue( sut.IsViolated( A, ChildB, model ) );
        }

        [Test]
        public void IsViolated_True_When_Rule_Defined_For_A_To_Any_If_Relation_Exists()
        {
            var model = new TestModel();
            var A = model.Add( "A" );
            var B = model.Add( "B" );
            var C = model.Add( "C" );
            var nodeA = model.Add( A, null );
            var nodeB = model.Add( B, null );
            var nodeC = model.Add( C, null );
            model.SetRelation( A, C, 1 );

            var sut = new CannotUseRule( A, null );

            Assert.IsFalse( sut.IsViolated( A, B, model ) );
            Assert.IsTrue( sut.IsViolated( A, C, model ) );
        }

        [Test]
        public void IsViolated_True_When_Rule_Defined_For_Any_To_B_If_Relation_Exists()
        {
            var model = new TestModel();
            var A = model.Add( "A" );
            var B = model.Add( "B" );
            model.SetRelation( A, B, 1 );

            var sut = new CannotUseRule( null, B );

            Assert.IsTrue( sut.IsViolated( A, B, model ) );
        }
    }
}