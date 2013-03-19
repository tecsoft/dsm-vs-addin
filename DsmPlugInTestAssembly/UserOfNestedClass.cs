namespace Root4
{
	public class UserOfNestedClass
	{
		// field
		Root1.ParentOfNestedClass parent;                   // +1 ParentOfNestedClass -> UserOfNestedClass

		UserOfNestedClass()
		{
			// call of constructor
			parent = new Root1.ParentOfNestedClass();      //  +1 ParentOfNestedClass -> UserOfNestedClass

		}

		void Method_Calls_Function_Of_Nested_Class()
		{
			// local declaration and call of constructor

			Root1.ParentOfNestedClass.NestedClass nested = 
				new Root1.ParentOfNestedClass.NestedClass();  // +2 NestedClass -> UserOfNestedClass

			// nested method call
			nested.SetString( "world" );                     // +1 NestedClass -> UserOfNestedClass
		}			
	}
}
