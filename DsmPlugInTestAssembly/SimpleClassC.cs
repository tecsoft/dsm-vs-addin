namespace Root3.Branch1
{
	public class SimpleClassC
	{
		public SimpleClassC()
		{
		}

		// +1 paremeter SimpleStruct -> SimpleClassC
		public bool Method_Uses_Structure( Root1.SimpleStruct input )
		{
			// +2 declaration + constructor  SimpleStruct -> SimpleClassC
			Root1.SimpleStruct mine = new Root1.SimpleStruct(0,0);

			if ( mine.Equals(input) )
				return true;
			else
				return false;
		}
	}
}