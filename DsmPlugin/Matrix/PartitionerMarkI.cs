using System;
using System.Collections;
using Tcdev.Outil;

namespace Tcdev.Dsm.Matrix
{
	class PartitionerMarkI
	{
        SquareMatrix _sm;

        /*
         * DEBUG variables
         */
        static Logger _log = new Logger("Partitioner.txt");
        int _accepted = 0;
		int _rejected = 0;

        public PartitionerMarkI(SquareMatrix matrix)
        {
            _sm = matrix;         
        }

        PartitionerMarkI()
        {
        }

        public Vector Partition()
        {
            _accepted = 0;
            _rejected = 0;

            DateTime start = DateTime.Now;
            _log.Trace("Starting Partitioning " );

            Vector vector = new Vector( _sm.Size);

            DoPartitioning( ref _sm, ref vector );

            System.Text.StringBuilder b = new System.Text.StringBuilder();

            for (int i = 0; i < vector.Size; i++)
            {
                b.Append(vector.Get(i)).Append(", ");
            }

            _log.Trace(b.ToString());

          
            _log.Trace( String.Format("Permutations accepted: {0}", _accepted ) );
			_log.Trace( String.Format( "Permutations rejected: {0}", _rejected ) );

            TimeSpan t = DateTime.Now - start;
            _log.Trace("Partition completed in : " + t.TotalSeconds);

            return vector;
        }

		void DoPartitioning( ref SquareMatrix matrix, ref Vector partitionVector )
		{
//            Hashtable permMap = new Hashtable(); // Permutations already disregarded on this iteration
			
            // outer loop for finding all non zero cells in upper triangle

            bool doLoop;
            do
            {
                Hashtable permMap = new Hashtable(); // Permutations already disregarded on this iteration

                doLoop = false;

                for( int i = 0; i < matrix.Size; i++ )
                //for (int d = matrix.Size - 1; d > 1; d--)
                {
                //    int i = 0;
                  //  int j = d;
                    //while (j < matrix.Size)
                    for( int j = matrix.Size - 1; j > i; j--) // cols in upper triangle
                    {
                        int val = matrix.Get(i, j);

                        if (val != 0)
                        {
                            // now find first zero to left

  //                          for (int x = 0; x < matrix.Size; x++)
                            for (int x = matrix.Size - 1; x >= 0; x--) 
                            {
                                //for (int y = 0; y < j; y++)  // NOT worth changing if Z to right of NZ
                                for (int y = 0; y < matrix.Size && CellScore(x, y, matrix.Size) < CellScore(i, j, matrix.Size); y++)
                                {
                                    if (x != y)  // don't process whatever values are on diagonal
                                    {
                                        val = matrix.Get(x, y);

                                        if (val == 0)
                                        {
                                            Permutation p = new Permutation(j, y);

                                            if (!permMap.Contains(p))
                                            {
                                                permMap.Add(p, null);

                                                //_log.Trace(String.Format("New : {0},{1}", j,y ) );

                                                SquareMatrix temp = SwapColumns(matrix, j, y);

                                                long score = Score(temp);

                                                if (score > Score(matrix))
                                                {
                                                    //_log.Trace( "      Score improved - changed accepted" );

                                                    matrix = temp;
                                                    partitionVector.Swap(j, y);

                                                    _accepted++;

                                                    //DoPartitioning(ref matrix, ref partitionVector);
                                                    doLoop = true;

                                                }
                                                else
                                                {
                                                    _rejected++;
                                                    //_log.Trace( String.Format("      Permutation not accepted :score = {0}", score ) );
                                                }
                                            }
                                            else
                                            {
                                                //_log.Trace(String.Format("Already used: {0},{1}", j,y ) );
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //i++;
                        //j++;
                    }
                }
            }
            while (doLoop);
		}

		static long Score( SquareMatrix matrix )
		{
			int score = 0;

			for ( int i = 0; i < matrix.Size - 1; i++ )
			{
				for( int j = i + 1; j < matrix.Size; j++ )
				{
					if ( matrix.Get(i,j) == 0 )
					{
						int x = (matrix.Size - i );
						score += x * x * (j + 1) * (j + 1);
					}
				}
			}

			return score;

		}

        static long CellScore(int i, int j, int size)
        {
            int a = (size - i);
            int b = j + 1;

            return (a * a * b * b);
        }


		static SquareMatrix SwapColumns( SquareMatrix matrix, int col1, int col2 )
		{
            SquareMatrix temp = matrix.Clone() as SquareMatrix;

			// swap cols on each row

			for( int i = 0; i < temp.Size; i++ )
			{
				int val1 = temp.Get(i, col1);
				int val2 = temp.Get(i, col2 );

				temp.Set(i, col1, val2 );
				temp.Set(i, col2, val1 );
			}

			// swap rows for each column

			for( int j = 0; j < temp.Size; j++ )
			{
				int val1 = temp.Get(col1,j);
				int val2 = temp.Get(col2,j);

				temp.Set(col1, j, val2);
                temp.Set(col2, j, val1);
			}

			return temp;


		}


		/*static void Print( SquareMatrix<T> matrix )
		{
			for( int i = 0; i < matrix.Size; i++ )
			{
				for( int j = 0; j < matrix.Size; j++ )
				{
                    if ( i == j )
                    {
                        _log.T
					T val = matrix[i,j];

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

		}*/
	}
}
