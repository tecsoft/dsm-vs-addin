using System;
using System.Collections.Generic;
using System.Text;

namespace DsmPlugInTestAssembly
{
    class ClassEnumType
    {
        public enum EnumType
        {
            Undefined = 0,


            // Constants are hard coded during IL compilation phase
            Next = CONST.Constants.CONSTANT
        }
    }
}
