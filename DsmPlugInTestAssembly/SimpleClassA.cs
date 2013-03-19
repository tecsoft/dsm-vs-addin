
namespace Root3
{
	public class SimpleClassA
	{
		// Declaration of field
		private Root1.Interface interfaceMember;                                 // +1 interface -> SimpleClassA



		public SimpleClassA()
		{
			// call to Constructor
			interfaceMember = new Root2.ImplementsInterfaceClass();              // +1 ImplementsInterfaceClass -> SimpleClassA
		}




		public void Method_Parameter_Interface( Root1.Interface interfaceCall )  // +1 Interface -> SimpleClassA
		{
			// virtual call to interface method

			interfaceCall.InterfaceMethod();                                     // +1 Interface -> SimpleClassA

			return;
		}




		public void Method_Throws_Exception()
		{
			// constructor of exception called
			// object thrown - throw not counted

			throw new Root1.ExceptionClass();                                    // +1 ExceptionClass -> SimpleClassA
		}




		private void Method_Catches_Exception()
		{
			// Exception is caught - no method is called on it
			// Exception is rethrown
			try
			{
				string x = "Hello";
			}
			// declares local variable ex          ????????????????????????
			catch( Root1.ExceptionClass ex)										// +1 ExceptionClass -> SimpleClassA
			{
				throw ex;
			}
		}

		

	}
}