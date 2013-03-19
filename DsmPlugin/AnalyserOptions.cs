using System;

namespace Tcdev.Dsm
{
    public class DsmOptions
    {
        public enum ModelType
        {
            Physical = 1,
            Logical
        }

        public ModelType DsmModelType              = ModelType.Logical;
        public bool      ExcludeGlobalNamespace    = true;
        public bool      ExcludeCompilerNamespaces = true;
        public bool      HideNestedClasses         = false;
    }
}
