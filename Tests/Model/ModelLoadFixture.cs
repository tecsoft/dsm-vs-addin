using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tcdev.Collections.Generic;
using Tcdev.Dsm.Model;
using DSM=Tcdev.Dsm;

namespace Tcdev.Dsm.Tests.Model
{
    [TestFixture]
    public class ModelLoadFixture
    {
        [Test]
        public void Add_Assembles()
        {
            DsmModel sut = new DsmModel();
            sut.AddAssembly(new Target("assembly.src.dll", "assembly/src.dll"));
            Assert.IsTrue(sut.Assembles.Contains("assembly/src.dll"));
        }

        [Test]
        public void Save_Assemblies()
        {
            DsmModel sut = new DsmModel();
            sut.AddAssembly(new Target("assembly.src.dll", "assembly/src.dll"));
            sut.SaveModel(" test.dsm");
            DsmModel newModel = new DsmModel();
            DsmModel.LoadModel("test.dsm", newModel);

            Assert.IsTrue(newModel.Assembles.Contains("assembly/src.dll"));
        }

        [Test]
        public void Test_Load_From_File_Not_Null_Hierarchy()
        {
            DsmModel sut = new DsmModel();

            FileInfo fi = new FileInfo("DsmModel/ModelFile.dsm");
            Assert.IsTrue(fi.Exists);
            DsmModel.LoadModel(fi.FullName, sut);

            Assert.IsNotNull(sut.Hierarchy);
        }

        //[Test]
        //public void Test_Lookup_Module_In_Loaded_File()
        //{
        //    DsmModel sut = new DsmModel();
        //    Module m1 = sut.CreateModule("a", "b", "c", false);
        //    Assert.AreEqual(m1, sut.FindNode(m1.FullName));

        //    Module m2 = sut.CreateModule("j", "ezerze", "zefzafkzefk", true);
        //    Assert.AreEqual(m2, sut.FindNode(m2.FullName));
        //}

        [Test]
        public void Test_BuildHierarchy_Branch_Created()
        {
            DsmModel sut = new DsmModel();
            Module m1 = new Module("type", null, "namespace", "assembly", false);
            sut.BuildHierarchy(new List<Module>() { m1 });

            Assert.IsNotNull(sut.FindNode("namespace"));
            Assert.IsNotNull(sut.FindNode("namespace.type"));
        }

        [Test]
        public void Test_BuildHierarchy_Rebuild_Existing_Branch_No_Change()
        {
            DsmModel sut = new DsmModel();
            Module m1 = new Module("type", null, "namespace", "assembly", false);
            sut.BuildHierarchy(new List<Module>() { m1 });

            m1 = new Module("type", null, "namespace", "assembly", false);
            sut.BuildHierarchy(new List<Module>() { m1 });

            Assert.IsNotNull(sut.FindNode("namespace"));
            Assert.IsNotNull(sut.FindNode("namespace.type"));
        }

        [Test]
        public void Test_BuildHierarchy_Rebuild_Type_Moved_New_Branch()
        {
            DsmModel sut = new DsmModel();
            Module m1 = new Module("type", null, "namespace1", "assembly", false);
            sut.BuildHierarchy(new List<Module>() { m1 });

            m1 = new Module("type", null, "namespace2", "assembly", false);
            sut.BuildHierarchy(new List<Module>() { m1 });

            Assert.IsNotNull(sut.FindNode("namespace2"));
            Assert.IsNotNull(sut.FindNode("namespace2.type"));

            Assert.IsNull(sut.FindNode("namespace1"));
            Assert.IsNull(sut.FindNode("namespace1.type"));
        }

        [Test]
        public void Test_BuildHierarchy_Rebuild_Type_Removed_No_Branch()
        {
            DsmModel sut = new DsmModel();
            Module m1 = new Module("type", null, "namespace1", "assembly", false);
            Module m2 = new Module("type2", null, "namespace1", "assembly", false);
            Module m3 = new Module("type3", null, "namespace2", "assembly", false);

            sut.BuildHierarchy(new List<Module>() { m1, m2, m3 });

            Assert.IsNotNull(sut.FindNode("namespace1"));
            Assert.IsNotNull(sut.FindNode("namespace1.type"));
            Assert.IsNotNull(sut.FindNode("namespace1.type2"));
            Assert.IsNotNull(sut.FindNode("namespace2"));
            Assert.IsNotNull(sut.FindNode("namespace2.type3"));

            sut.BuildHierarchy(new List<Module>() { m1, m2 });

            Assert.IsNotNull(sut.FindNode("namespace1"));
            Assert.IsNotNull(sut.FindNode("namespace1.type"));
            Assert.IsNotNull(sut.FindNode("namespace1.type2"));

            Assert.IsNull(sut.FindNode("namespace2"));
            Assert.IsNull(sut.FindNode("namespace2.type3"));
        }

        [Test]
        public void Test_BuildHierarchy_Star_Branch_Created()
        {
            DsmModel sut = new DsmModel();
            Module m1 = new Module("type", null, "namespace", "assembly", false);
            Module m2 = new Module("type2", null, "namespace.sub", "assembly", false);

            sut.BuildHierarchy(new List<Module>() { m1, m2 });

            // namespace
            //  --- sub
            //      --- type2
            //  --- *
            //      --- type

            TreeIterator<Module> it = new TreeIterator<Module>(sut.Hierarchy);
            Tree<Module>.Node node = it.Next();

            while (node != null)
            {
                Console.WriteLine(node.NodeValue.FullName);

                node = it.Next();
            }

            Assert.IsNotNull(sut.FindNode("namespace"));
            Assert.IsNotNull(sut.FindNode("namespace.sub"));
            Assert.IsNotNull(sut.FindNode("namespace.sub.type2"));
            Assert.IsNotNull(sut.FindNode("namespace.type"));

            var starNode = sut.FindNode("namespace.type").Parent;
            var subNode = sut.FindNode("namespace.sub");

            // sub and * have the same parent = namespace
            Assert.AreEqual(starNode.Parent, subNode.Parent);
            Assert.AreEqual(starNode.Parent, sut.FindNode("namespace"));
        }

        [Test]
        public void Test_Model_Rebuild()
        {
            DsmModel sut = new DsmModel();

            Module m1= new Module("existing", null, "existing", "existing", false);
            Module m2 = new Module("deleted", null, "deleted", "deleted", false);
            Module m3 = new Module("moved", null, "from", "moved", false);

            sut.BuildHierarchy(new List<Module>() { m1, m2, m3 });

            TreeIterator<Module> it1 = new TreeIterator<Module>(sut.Hierarchy);
            Tree<Module>.Node node1 = it1.Next();
            Console.WriteLine("----");
            while (node1 != null)
            {
                Console.WriteLine(node1.NodeValue.FullName);

                node1 = it1.Next();
            }

            //Hierarchy contains:
            //existing and existing.existing
            //deleted and deleted.deleted
            //moved and from.moved

            Module m4 = new Module("existing", null, "existing", "existing", false);
            Module m5 = new Module("new", null, "new", "new", false);
            Module m6 = new Module("moved", null, "to", "moved", false);

            sut.BuildHierarchy(new List<Module>() { m4, m5, m6 });

            // now contains
            // existing and existing.existing
            // new and new.new
            // moved and to.moved
            Module existing = sut.FindNode("existing.existing").NodeValue;
            Module existingBranch = sut.FindNode("existing").NodeValue;
            Assert.AreEqual(1, existing.BuildNumber);
            Assert.AreEqual(1, existingBranch.BuildNumber);

            Module newM = sut.FindNode("new.new").NodeValue;
            Module newMBranch = sut.FindNode("new").NodeValue;
            Assert.AreEqual(1, newM.BuildNumber);
            Assert.AreEqual(1, newMBranch.BuildNumber);

            Module moved = sut.FindNode("to.moved").NodeValue;
            Module movedBranch = sut.FindNode("to").NodeValue;
            Assert.AreEqual(1, moved.BuildNumber);
            Assert.AreEqual(1, movedBranch.BuildNumber);

            Assert.IsNull(sut.FindNode("deleted"));
            Assert.IsNull(sut.FindNode("deleted.deleted"));
            Assert.IsNull(sut.FindNode("from"));
            Assert.IsNull(sut.FindNode("from.moved"));
            sut.AllocateIds(); // gives us the ordering so that we can that new branches are found at the top of the hierarchy

            Assert.IsTrue(newMBranch.Id < existingBranch.Id);
            Assert.IsTrue(movedBranch.Id < existingBranch.Id);

            TreeIterator<Module> it = new TreeIterator<Module>(sut.Hierarchy);
            Tree<Module>.Node node = it.Next();
            Console.WriteLine("----");
            while (node != null)
            {
                Console.WriteLine(node.NodeValue.FullName);

                node = it.Next();
            }
        }
    }
}