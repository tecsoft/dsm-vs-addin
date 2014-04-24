using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tcdev.Dsm.Model;
using Tcdev.Dsm.Model.DependencyRules;

namespace Tcdev.Dsm.Tests.DsmModel
{
    [TestFixture]
    public class CannotUseRuleFixture
    {
        [Test]
        public void IsViolated_True_When_A_Provides_To_B()
        {
            Module A = new Module("", "", "", "", false);
            Module B = new Module("", "", "", "", false);

            A.AddRelation(B, 1);

            var sut = new CannotUseRule(A, B);

            Assert.IsTrue(sut.IsViolated());
        }

        [Test]
        public void IsViolated_False_When_A_Does_Not_Provide_To_B()
        {
            Module A = new Module("", "", "", "", false);
            Module B = new Module("", "", "", "", false);

            A.AddRelation(B, 0);

            var sut = new CannotUseRule(A, B);

            Assert.IsFalse(sut.IsViolated());
        }

        [Test]
        public void IsViolated_False_If_B_Provides_To_A_But_Not_A_To_B()
        {
            Module A = new Module("", "", "", "", false);
            Module B = new Module("", "", "", "", false);

            A.AddRelation(B, 0);
            B.AddRelation(A, 1);

            var sut = new CannotUseRule(A, B);

            Assert.IsFalse(sut.IsViolated());
        }
    }
}