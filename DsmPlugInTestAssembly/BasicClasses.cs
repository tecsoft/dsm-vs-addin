using System;

namespace Root1
{


	public interface Interface
	{
		void InterfaceMethod();
	}
}



namespace Root1
{


	public abstract class AbstractClass
	{
		
		
		public AbstractClass()
		{
		}



		public void AbstractClassMethod()
		{
            Root3.SimpleClassA sca = new Root3.SimpleClassA();
			return;
		}
	}
}



namespace Root1
{


	public class ExceptionClass : ApplicationException
	{
	}
}


namespace Root1
{

	public class ParentOfNestedClass
	{
		public ParentOfNestedClass()
		{
		}

		public class NestedClass
		{
			private string x = "Hello";

			public NestedClass()
			{
			}

			public void SetString( string newX )
			{
				x = newX;
			}
		}
	}
}

namespace Root1
{
	public class ClassWithStaticConstructor
	{
		static ClassWithStaticConstructor()
		{
			string x = "intialised";
		}

		public static void StaticFunction()
		{
			string x = "called";
		}
	}
}

namespace Root1
{
	public struct SimpleStruct
	{
		private int x;
		private int y;

		public SimpleStruct( int x, int y )
		{
			this.x = x;
			this.y = y;
		}
	}
}

namespace Root1
{
	public delegate void DelegateFunction( string s );

	public class ClassRequiringUseOfItsDelegate
	{
		public ClassRequiringUseOfItsDelegate()
		{
		}

		public void Method_Using_Delegate_As_Parameter( string input, DelegateFunction func )
		{
			func(input);
		}
	}
}


