using System;
using System.Collections.Generic;
using System.Text;

namespace Tcdev.Dsm.Matrix
{
    /// <summary>
    /// The vector represents the permutation of the original indexes 0 to N after the partitioning
    /// has been accomplished : ex : original 0 1 2 3 4 could be permuted to 3 2 1 0 4.  We use this to
    /// reorganise the real matric after the calculation has completed
    /// </summary>
    public class Vector
    {
        int   _size;
        int[] _vector = null;

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="width"></param>
        public Vector( int width)
        {
            _size = width;
            _vector = new int[_size];

            for (int i = 0; i < _size; i++)
            {
                _vector[i] = i;
            }
        }

        //-----------------------------------------------------------------------------------------

        private Vector()
        {
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the size of this vector
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Set the value of index i
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        public void Set(int i, int value)
        {
            if (i >= _size || i < 0 )
                throw new ArgumentOutOfRangeException("i");
                
            _vector[i] = value;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Get the value at index i
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int Get(int i)
        {
            if (i >= _size || i < 0)
                throw new ArgumentOutOfRangeException("i");

            return _vector[i];
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Swap two indexes around
        /// </summary>
        /// <param name="idx1"></param>
        /// <param name="idx2"></param>
        public void Swap( int idx1, int idx2 )
        {
            if (idx1 >= _size || idx1 < 0)
                throw new ArgumentOutOfRangeException("idx1");

            if (idx2 >= _size || idx2 < 0)
                throw new ArgumentOutOfRangeException("idx2");

            int temp = _vector[idx1];
            _vector[idx1] = _vector[idx2];
            _vector[idx2] = temp;
        }

        //-----------------------------------------------------------------------------------------
    }
}
