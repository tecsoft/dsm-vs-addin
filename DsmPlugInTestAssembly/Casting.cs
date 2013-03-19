
using System;
using Root3;
namespace Root5
{
	public class CastingClass
	{
		public CastingClass()
		{
		}

		public Root1.Interface Method_Cast_ToInterface( object parameter)
		{
			// +1 return type Interface -> CastingClass
Type x = typeof(SimpleClassA ); // + 1 SimpleClassA -> CastingClass
			
			if ( parameter is Root1.Interface )          // +1 Interface -> CastingClass
			{
				return (Root1.Interface)parameter;       // +1  Interface -> CastingClass
			}
			else
			{
				return parameter as Root1.Interface;    // +1 Interface -> CastingClass
			}
			
			
		}
	}
								  

}