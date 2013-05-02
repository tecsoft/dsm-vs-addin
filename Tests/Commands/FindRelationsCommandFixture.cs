using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tcdev.Dsm.Model;
using Tcdev.Collections.Generic;
using DSM = Tcdev.Dsm;

namespace Tcdev.Dsm.Tests.Commands
{
    [TestFixture]
    public class FindRelationsCommandFixture
    {
        [Test]
        public void Test_FindRelation_At_Namespace_Level()
        {
            DSM.Model.DsmModel sut = new DSM.Model.DsmModel();
            Module m1 = new Module("type1", null, "namespace.subspace", "assembly", false);
            Module m2 = new Module("type2", null, "namespace1.subspace1", "assembly", false);

            sut.BuildHierarchy(new List<Module>() { m1,m2 });
            sut.AllocateIds();

            sut.FindNode(m1.FullName).NodeValue.AddRelation(
                sut.FindNode(m2.FullName).NodeValue, 1234);
            sut.CalculateParentWeights();

            var relations = sut.FindRelations( 
                sut.FindNode("namespace"), 
                sut.FindNode("namespace1") );

            Assert.AreEqual(1, relations.Count);
            var relation = relations[0];
            Assert.IsNotNull(relation);
            Assert.AreEqual(m1, relation.Provider);
            Assert.AreEqual(m2, relation.Consumer);
            Assert.AreEqual(1234, relation.Weight);
        }

        [Test]
        public void Test_FindRelation_At_Module_Level()
        {
            DSM.Model.DsmModel sut = new DSM.Model.DsmModel();
            Module m1 = new Module("type1", null, "namespace.subspace", "assembly", false);
            Module m2 = new Module("type2", null, "namespace1.subspace1", "assembly", false);

            sut.BuildHierarchy(new List<Module>() { m1, m2 });
            sut.AllocateIds();

            sut.FindNode(m1.FullName).NodeValue.AddRelation(
                sut.FindNode(m2.FullName).NodeValue, 1234);

            sut.CalculateParentWeights();

            var relations = sut.FindRelations(
                sut.FindNode("namespace.subspace.type1"),
                sut.FindNode("namespace1.subspace1.type2"));

            Assert.AreEqual(1, relations.Count);
            var relation = relations[0];
            Assert.IsNotNull(relation);
            Assert.AreEqual(m1, relation.Provider);
            Assert.AreEqual(m2, relation.Consumer);
            Assert.AreEqual(1234, relation.Weight);
        }

        [Test]
        public void Test_FindRelation_No_Relation_Empty_List()
        {
            DSM.Model.DsmModel sut = new DSM.Model.DsmModel();
            Module m1 = new Module("type1", null, "namespace.subspace", "assembly", false);
            Module m2 = new Module("type2", null, "namespace1.subspace1", "assembly", false);

            sut.BuildHierarchy(new List<Module>() { m1, m2 });
            sut.AllocateIds();
            sut.CalculateParentWeights();

            var relations = sut.FindRelations(
                sut.FindNode("namespace.subspace.type1"),
                sut.FindNode("namespace1"));

            Assert.AreEqual(0, relations.Count);
        }

        [Test]
        public void Test_FindRelation_At_Namespace_Level_Bidirectional()
        {
            DSM.Model.DsmModel sut = new DSM.Model.DsmModel();
            Module m1 = new Module("type1", null, "namespace.subspace", "assembly", false);
            Module m2 = new Module("type2", null, "namespace1.subspace1", "assembly", false);

            sut.BuildHierarchy(new List<Module>() { m1, m2 });
            sut.AllocateIds();

            sut.FindNode(m1.FullName).NodeValue.AddRelation(
                sut.FindNode(m2.FullName).NodeValue, 1234);

            sut.FindNode(m2.FullName).NodeValue.AddRelation(
                sut.FindNode(m1.FullName).NodeValue, 22);

            sut.CalculateParentWeights();

            var relations = sut.FindRelations(
                sut.FindNode("namespace"),
                sut.FindNode("namespace1"));

            Assert.AreEqual(1, relations.Count);

            var relation = relations[0];
            Assert.IsNotNull(relation);
            Assert.AreEqual(m1, relation.Provider);
            Assert.AreEqual(m2, relation.Consumer);
            Assert.AreEqual(1234, relation.Weight);
        }

        [Test]
        public void Test_FindRelation_At_Namespace_Level_Two_Consumers()
        {
            DSM.Model.DsmModel sut = new DSM.Model.DsmModel();
            Module m1 = new Module("type1", null, "namespace.subspace", "assembly", false);
            Module m2 = new Module("type2", null, "namespace1.subspace1", "assembly", false);
            Module m3 = new Module("type3", null, "namespace1.subspace1", "assembly", false);

            sut.BuildHierarchy(new List<Module>() { m1, m2,m3 });
            sut.AllocateIds();

            sut.FindNode(m1.FullName).NodeValue.AddRelation(
                sut.FindNode(m2.FullName).NodeValue, 1234);

            sut.FindNode(m1.FullName).NodeValue.AddRelation(
                sut.FindNode(m3.FullName).NodeValue, 22);

            sut.CalculateParentWeights();

            var relations = sut.FindRelations(
                sut.FindNode("namespace"),
                sut.FindNode("namespace1"));

            Assert.AreEqual(2, relations.Count);
            var relation = relations[0];
            Assert.IsNotNull(relation);
            Assert.AreEqual(m1, relation.Provider);
            Assert.AreEqual(m2, relation.Consumer);
            Assert.AreEqual(1234, relation.Weight);

            relation = relations[1];
            Assert.IsNotNull(relation);
            Assert.AreEqual(m1, relation.Provider);
            Assert.AreEqual(m3, relation.Consumer);
            Assert.AreEqual(22, relation.Weight);
        }

        
    }
}
