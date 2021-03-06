﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
//using Tcdev.Dsm.Adapters;
using Tcdev.Dsm.Engine;
using System.IO;
using Tcdev.DsmVsAddin;

namespace Tcdev.Dsm.Tests.Adapters
{
    [TestFixture]
    public class VisualStudioAdapterFixture
    {
        //[Test]
        //public void Test_Populate_Analyser_No_Project_File_Defined_Yet()
        //{
        //    VisualStudioAdapter sut = new VisualStudioAdapter();
        //    DirectoryInfo projectPath = new DirectoryInfo( "..");

        //    IAnalyser analyser = sut.GetAnalyser();
        //        Assert.IsFalse(analyser.ProjectFile.Exists);
        //}

        [Test]
        public void Test_Populate_Analyser_Project_File_Defined()
        {
            VisualStudioAdapter sut = new VisualStudioAdapter();
            DirectoryInfo projectPath = new DirectoryInfo("..");

            FileInfo fi = new FileInfo(Path.Combine(projectPath.FullName, "test.dsm"));
            FileStream fs = fi.Create();
            fs.Dispose();

            IAnalyser analyser = null;

            try
            {
                //analyser = sut.GetAnalyser();
                //Assert.IsTrue(analyser.ProjectFile.Exists);
            }
            finally
            {
                //analyser.Dispose();
                fi.Delete();
            }

        }
    }
}
