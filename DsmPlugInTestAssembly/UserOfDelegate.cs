namespace Root4.Branch2
{
	public class UserOfDelegate
	{
		public UserOfDelegate()
		{
		}

		private void DelegateInstance( string s )
		{
			System.Console.WriteLine( s );
		}


		public void Method_Pass_Delegate_Instance()
		{
			// +2 dec + constructeur   ClassReqquiringUseOfItsDelegate -> UserOfDElegate
			Root1.ClassRequiringUseOfItsDelegate classType = 
				new Root1.ClassRequiringUseOfItsDelegate();

			// +2 dec + ctor  DelegateFunction -> UserOfDelegate

			Root1.DelegateFunction func = new Root1.DelegateFunction( DelegateInstance );

			// +1 ClassRequiringUseOfItsDelegate --> UserOfDelegate
			classType.Method_Using_Delegate_As_Parameter( "hello world", func );
		}
	}
}


