using System;
using System.Collections;

namespace MatrixPartition
{
	/// <summary>
	/// Description résumée de Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// Point d'entrée principal de l'application.
		/// </summary>
		/// 

		/*static int _size = 4;
		static int [,] current =  
						{	{ 2,1,1,1 },
							{ 1,2,0,1 },
							{ 1,0,2,0 },
							{ 0,1,0,2 }
						};*/

		static int size = 6;
		static int [,] current =  
						{
							{ 2,1,0,1,0,0 },
							{ 0,2,1,0,1,0 },
							{ 0,0,2,0,1,1 },
							{ 0,1,1,2,0,0 },
							{ 0,0,0,0,2,1 },
							{ 0,0,0,0,1,2 }
						};
		static int accepted = 0;
		static int rejected = 0;

		class Permutation 
		{
			private int x;
			private int y;

			public Permutation( int col1, int col2 )
			{
				if ( col1 <= col2 )
				{
					x = col1;
					y = col2;
				}
				else
				{
					x = col2;
					y = col1;
				}
			}

			public override bool Equals(object obj)
			{
				if ( obj is Permutation )
				{
					Permutation test = obj as Permutation;

					return test.x == x && test.y == y;
				}

				return false;
			}

			public override int GetHashCode()
			{
				return String.Format("{0},{1}", x,y ).GetHashCode();
			}


		}


		[STAThread]
		static void Main(string[] args)
		{

			Console.WriteLine("----START ----");
			Print (current );

			Program1( ref current );

			Console.WriteLine();
			Console.WriteLine( "Permutations accepted: {0}", accepted );
			Console.WriteLine( "Permutations rejected: {0}", rejected );
			Print( current );
			
		}

		static void Program1( ref int[,] result )
		{
			int depth = 0;

			Loop(ref result, ref depth );

		}

		static void Loop( ref int[,] matrix, ref int depth )
		{
			// outer loop for finding all non zero cells in upper triangle

			Console.WriteLine( "ALGO DEPTH {0}", depth );
			depth++;

			Hashtable permMap = new Hashtable();

			for( int i = 0; i < size; i++ )
			{
				for( int j = size - 1; j >i; j--) // cols in upper triangle
				{
					int val = matrix[i,j];

					if ( val == 1 )
					{
						// now find first zero to left

						//Console.WriteLine( "NZ = {0},{1}", i, j );

						for ( int x = 0; x < size; x++ )
						{
							for( int y = 0; y < j; y++ )  // NOT worth changing if Z to right of NZ 
							{
								if ( y != j )
								{
									val = matrix[x,y];

									if ( val == 0 )
									{
										Permutation p = new Permutation( j, y );
										Permutation p1 = new Permutation( y,j );

										if ( permMap.Contains(p) /*|| permMap.Contains(p1)*/ )
										{
											// cols amready swapped this go
											Console.WriteLine("Already used: {0},{1}", j,y );
										}
										else
										{
											permMap.Add(p, null );

											Console.WriteLine("New : {0},{1}", j,y );

											int[,] temp = SwapColumns( matrix, j, y );

											int score = Score(temp);

											if ( score > Score( matrix) )
											{
												Console.WriteLine( "      Score improved - changed accepted" );
									
												matrix = temp;

												accepted++;

												Print( matrix );

												// restart Loop
												Loop( ref matrix, ref depth );
												
											}
											else
											{
												rejected++;
												Console.WriteLine( "      Permutation not accepted :score = {0}", score );
											}
										}
									
									}
									
								}
							}
						}
					}
				}
			}
		}


		static int Score( int[,] matrix )
		{
			int score = 0;

			for ( int i = 0; i < size - 1; i++ )
			{
				for( int j = i + 1; j < size; j++ )
				{
					if ( matrix[i,j] == 0 )
					{
						int x = (size - i );
						score += x * x * (j + 1) * (j + 1);
					}
				}
			}

			return score;

		}


		static int[,] SwapColumns( int[,] matrix, int col1, int col2 )
		{
			int[,] temp = new int[size,size];

			// copy of _matrix
			for (int i = 0; i < size; i++ )
			{
				for( int j = 0; j < size; j++ )
				{
					temp[i,j] = matrix[i,j];
				}
			}

			// swap cols on each row

			for( int i = 0; i< size; i++ )
			{
				int val1 = temp[i, col1 ];
				int val2 = temp[i, col2 ];

				temp[i, col1] = val2;
				temp[i, col2] = val1;
			}

			// swap rows for each column

			for( int j = 0; j < size; j++ )
			{
				int val1 = temp[col1,j];
				int val2 = temp[col2,j];

				temp[col1,j] = val2;
				temp[col2,j] = val1;
			}

			return temp;


		}

		static void  Print( int[,] matrix )
		{
			for( int i = 0; i < size; i++ )
			{
				for( int j = 0; j < size; j++ )
				{
					int val = matrix[i,j];

					if ( val == 2 ) 
					{
						Console.Write( 'X' );
					}
					else
					{
						Console.Write( val.ToString() );
					}
				}
				Console.WriteLine();
			}

			Console.WriteLine( "Score: {0}", Score(matrix) );
			Console.WriteLine();

		}
	}
}
