
namespace Root4
{
	public class SimpleClassB
	{


		//field declaration
		private Root3.SimpleClassA simpleClassAInstance;       // +1 SimpleClassA -> SimpleClassB




		public SimpleClassB()
		{
			// Constructor of class
			simpleClassAInstance = new Root3.SimpleClassA();   // +1 SimpleClassA -> SimpleClassB
		}




		// +1 SimpleClassA -> SimpleClassB
		public Root3.SimpleClassA Method_Returns_SimpleClassA()      // +1 SimpleClassA -> SimpleClassB ?????????
		{
			// value returned, not recount
			return simpleClassAInstance;
		}



		// + 1 Return type SimpleClassA -> SimpleClassB
		public Root3.SimpleClassA PropertyClassA
		{
			// get and set properties
			get{ return simpleClassAInstance; }						
			set{ simpleClassAInstance = value; }
		}




		public void Method_Takes_SimpleClassA( Root3.SimpleClassA classA )  // +1 SimpleClassA -> SimpleClassB
		{
			// classA not used
			return;
		}



		public void Method_Uses_Local_ClassA()
		{
			// Local declaration + call to constructor
			Root3.SimpleClassA local = new Root3.SimpleClassA(); // +2 SimpleClassA -> SimpleClassB

			// call direct method on local
			local.Method_Throws_Exception();                       // +1 SimpleCLassA -> SimpleClassB
		}


	}
}