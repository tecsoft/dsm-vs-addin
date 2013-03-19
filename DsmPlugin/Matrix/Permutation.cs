using System;
using System.Collections.Generic;
using System.Text;

namespace Tcdev.Dsm.Matrix
{
    /// <summary>
    /// Represents a permutation of two values (order not important). 
    /// </summary>
    class Permutation
    {
        int _first;
        int _second;

        //----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        public Permutation(int value1, int value2 )
        {
            // as the order is not important we sort them to make comparing permutations a little easier
            if ( value1 < value2 )
            {
                _first = value1;
                _second = value2;
            }
            else
            {
                _first = value2;
                _second = value1;
            }
        }

        //----------------------------------------------------------------------------------------------------
        Permutation()
        {
        }

        //----------------------------------------------------------------------------------------------------
        int First
        {
            get { return _first; }
        }

        //----------------------------------------------------------------------------------------------------
        int Second
        {
            get { return _second; }
        }


        //----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Equals override so that permutations can be used as a key on a dictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals( object obj )
        {
            if ( obj != null && obj is Permutation)
            {
                Permutation test = obj as Permutation;

                return test._first == _first  && 
                       test._second == _second;
            }

            return false;
        }

        //----------------------------------------------------------------------------------------------------
        /// <summary>
        /// GetHashCode override so that permutations can be used as a key in a dictionary
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _first.GetHashCode() ^ _second.GetHashCode();
        }
        //----------------------------------------------------------------------------------------------------
        

    }
}
