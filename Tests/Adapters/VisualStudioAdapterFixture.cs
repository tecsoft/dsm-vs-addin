using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tcdev.Dsm.Adapters;
using Tcdev.Dsm.Engine;
using System.IO;

namespace Tcdev.Dsm.Tests.Adapters
{
    [TestFixture]
    public class VisualStudioAdapterFixture
    {
        [Test]
        public void Test_Populate_Analyser_No_Project_File_Defined_Yet()
        {
            VisualStudioAdapter sut = new VisualStudioAdapter();
            sut.ProjectPath = new DirectoryInfo( "..");
            sut.ProjectName = "VisualStudioAdapterFixture";

            IAnalyser analyser = sut.GetAnalyser();
                Assert.IsFalse(analyser.ProjectFile.Exists);
        }

        [Test]
        public void Test_Populate_Analyser_Project_File_Defined()
        {
            VisualStudioAdapter sut = new VisualStudioAdapter();
            sut.ProjectPath = new DirectoryInfo("..");
            sut.ProjectName = "VisualStudioAdapterFixture";

            FileInfo fi = new FileInfo(Path.Combine(sut.ProjectPath.FullName, sut.ProjectName + ".dsm"));
            FileStream fs = fi.Create();
            fs.Dispose();

            IAnalyser analyser = null;

            try
            {
                analyser = sut.GetAnalyser();
                Assert.IsTrue(analyser.ProjectFile.Exists);
            }
            finally
            {
                //analyser.Dispose();
                fi.Delete();
            }

        }
    }
}
