using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Net;
using System.IO;
using System.Diagnostics;
using Tcdev.Dsm.Install;

namespace Tcdev.Dsm.Tests.Install
{
    [TestFixture]
    public class InstallerFixture
    {
        [Test]
        public void Test_Not_New_Old_Minor()
        {
            Installer sut = new Installer("","");
            Assert.IsFalse(sut.IsHigherVersion(122, 32, 122, 32));
        }

        [Test]
        public void Test_Not_New_Old_Major()
        {
            Installer sut = new Installer("", "");
            Assert.IsFalse(sut.IsHigherVersion(122, 32, 121, 33));
        }

        [Test]
        public void Test_New_Minor()
        {
            Installer sut = new Installer("", "");
            Assert.IsTrue(sut.IsHigherVersion(113, 11, 113, 12));
        }

        [Test]
        public void Test_Not_New_Major_And_Minor_Are_Same()
        {
            Installer sut = new Installer("", "");
            Assert.IsFalse(sut.IsHigherVersion(113, 11, 113, 11));
        }

        [Test]
        public void Test_Read_New_Version_File()
        {
            FileInfo fi = new FileInfo("Install//NewVersion.txt");
            Assert.IsTrue(fi.Exists);

            string url = "file://" + fi.FullName;

            Installer sut = new Installer(url, "");
            Assert.IsTrue (sut.NewVersion() );
        }

        [Test]
        public void Test_Read_Old_Version_File()
        {
            FileInfo fi = new FileInfo("Install//OldVersion.txt");
            Assert.IsTrue(fi.Exists);

            string url = "file://" + fi.FullName;

            Installer sut = new Installer(url, "");
            Assert.IsFalse(sut.NewVersion());
        }

        //[Test]
        //public void GetLatestVersion()
        //{
        //    string url = "http://www.tom-carter.net/DsmInstaller.msi";
        //    WebRequest req = WebRequest.Create(url );

        //    try
        //    {
        //        WebResponse reply = req.GetResponse();

        //        using (Stream reader = reply.GetResponseStream())
        //        {

        //            using (FileStream fs = new FileStream(
        //                "DsmInstaller.msi", FileMode.OpenOrCreate, FileAccess.Write))
        //            {

        //                Byte[] buffer = new Byte[32 * 1024];
        //                int read = reader.Read(buffer, 0, buffer.Length);

        //                while (read > 0)
        //                {
        //                    fs.Write(buffer, 0, read);
        //                    fs.Flush();
        //                    read = reader.Read(buffer, 0, buffer.Length);
        //                }
        //            }
        //        }

        //        Process process = new Process();
        //        process.StartInfo = new ProcessStartInfo(
        //            "msiexec", " /i DsmInstaller.msi REINSTALL=ALL REINSTALLMODE=vomus");
        //        process.Start();
        //    }
        //    catch(Exception e )
        //    {
        //        Console.WriteLine( e.ToString() );
        //    }
        //}

    }
}
