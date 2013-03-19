namespace Root6
{
	public class UserOfStaticClass
	{
		// +2 field dec + constructor   SimpleClassA -> UserOfStaticClass

		static Root3.SimpleClassA staticInstanceClassA = new Root3.SimpleClassA();

		public UserOfStaticClass()
		{
			Root1.ClassWithStaticConstructor.StaticFunction(); // +1 ClassWithStaticConstructor -> UserIfStaticClass
		}
	}
}