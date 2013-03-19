using System;
using System.Collections.Generic;
using System.Text;

namespace Tcdev.Dsm.Matrix
{
    public class SquareMatrix : ICloneable
    {
        int[,] _matrix = null;
        int _size;

        //-----------------------------------------------------------------------------------------
        public SquareMatrix(int width)
        {
            _size = width;

            _matrix = new int[_size, _size];
        }

        //-----------------------------------------------------------------------------------------

        SquareMatrix()
        {
        }

        //-----------------------------------------------------------------------------------------

        public object Clone()
        {
            SquareMatrix sm = new SquareMatrix(this.Size);

            for (int i = 0; i < this.Size; i++)
            {
                for (int j = 0; j < this.Size; j++)
                {
                    sm.Set(i, j, this.Get(i, j) );
                }
            }

            return sm;
        }

        //-----------------------------------------------------------------------------------------

        public int Size
        {
            get { return _size; }
        }

        //-----------------------------------------------------------------------------------------

        public void Set(int i, int j, int value)
        {
            if (i >= _size || i < 0)
                throw new ArgumentOutOfRangeException("i");

            if (j >= _size || j < 0)
                throw new ArgumentOutOfRangeException("j");

            _matrix[i, j] = value;
        }

        //-----------------------------------------------------------------------------------------

        public int Get(int i, int j)
        {
            if (i >= _size || i < 0)
                throw new ArgumentOutOfRangeException("i");

            if (j >= _size || j < 0)
                throw new ArgumentOutOfRangeException("j");

            return _matrix[i,j];
        }

        //-----------------------------------------------------------------------------------------
    }
}
