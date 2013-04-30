using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tcdev.Dsm.Model;
using Tcdev.Dsm.Commands;
using Tcdev.Dsm.Engine;
using System.Reflection;

namespace Tcdev.Dsm.Tests.Commands
{
    [TestFixture]
    public class AnalyseCommandFixture
    {
        IAnalyser _analyser;
        [SetUp]
        public void Setup()
        {
            _analyser = new CecilAnalyser();
        }

        //[TearDown]
        //public void TearDown()
        //{
        //    _analyser.Dispose();
        //}
        [Test]
        public void Test_Create()
        {
            _analyser.IncludeAssembly(new Target("DSMPLUGIN", @"D:\Perso\DSM\WorkDir\Tests\bin\Debug\TcDev.DsmPlugin.dll" ));
            
            Tcdev.Dsm.Model.DsmModel model = new Tcdev.Dsm.Model.DsmModel();
            _analyser.Model = model;

            
            //model.CreateModule("type1", "namespace", null, false);
            int i = model.BuildNumber;

            CommandAnalyse sut = new CommandAnalyse(_analyser, model);

            sut.Execute(null);

           //Assert.IsTrue(model.BuildNumber == i + 1);

            model.CreateModule("type2", "namespace", null, false);

            sut.Execute(null);

            //Assert.IsTrue(model.BuildNumber == i + 2);
        }
    }
}
