using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
//using Tcdev.Dsm.Adapters;
using System.IO;
using Tcdev.Dsm.Engine;

namespace Tcdev.Dsm.Tests.Adapters
{
    [TestFixture]
    public class ReflectorAdapterFixture
    {
        //[Test]
        //public void Test_No_Project_Specified()
        //{
        //    ReflectorAdapter sut = new ReflectorAdapter();
        //    using (IAnalyser analyser = sut.GetAnalyser())
        //        Assert.IsNull(analyser.ProjectFile);
        //}
        //[Test]
        //public void Test_Populate_Analyser_No_Project_File_Defined_Yet()
        //{
        //    ReflectorAdapter sut = new ReflectorAdapter();
        //    sut.ProjectPath = new DirectoryInfo("..");
        //    sut.ProjectName = "ReflectorAdapterFixture";

        //    using (IAnalyser analyser = sut.GetAnalyser())
        //        Assert.IsFalse(analyser.ProjectFile.Exists);
        //}

        //[Test]
        //public void Test_Populate_Analyser_Project_File_Defined()
        //{
        //    ReflectorAdapter sut = new ReflectorAdapter();
        //    sut.ProjectPath = new DirectoryInfo("..");
        //    sut.ProjectName = "ReflectorAdapterFixture";

        //    FileInfo fi = new FileInfo(Path.Combine(sut.ProjectPath.FullName, sut.ProjectName + ".dsm"));
        //    FileStream fs = fi.Create();
        //    fs.Dispose();

        //    IAnalyser analyser = null;

        //    try
        //    {
        //        analyser = sut.GetAnalyser();
        //        Assert.IsTrue(analyser.ProjectFile.Exists);
        //    }
        //    finally
        //    {
        //        analyser.Dispose();
        //        fi.Delete();
        //    }

        //}
    }
}
