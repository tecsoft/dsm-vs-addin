using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tcdev.Dsm.Model.Builders
{
    public interface IModelBuilder
    {
        void Build(DsmModel model, int BuildNumber, IEnumerable<Module> modules);
    }
}