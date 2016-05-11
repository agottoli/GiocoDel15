using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiocoDel15
{
    class Board
    {
        int side;
        int size;
        int holePosition;
        int[] table;
        string holeSpaces;
        int numeroMosse = 0;

        public Board(int size)
        {
            this.size = size;
            side = AssertSquareSize(size);

            table = new int[size];
            for (int i = 0; i < size - 1;)
            {
                table[i] = ++i;
            }

            holePosition = size - 1;

            CalcolaHoleSpaces(size);
        }

        public Board(int[] conf, int holeIndex)
        {

            size = conf.Length;
            side = AssertSquareSize(size);

            table = conf;

            holePosition = holeIndex;

            CalcolaHoleSpaces(size);
        }

        public Board(int[] conf) : this(conf, FindHoleIndex(conf))
        { }

        private int AssertSquareSize(int size)
        {
            double sqrt = Math.Sqrt(size);
            if (sqrt % 1 != 0)
            {
                throw new System.ArgumentException(size + " must be a square.");
            }
            return Convert.ToInt32(sqrt);
        }

        private static int FindHoleIndex(int[] conf)
        {
            for (int i = 0; i < conf.Length; i++)
            {
                if (conf[i] == 0) // suppongo che il buco sia identificato dal valore 0
                {
                    return i;
                }
            }

            throw new ArgumentException("La configurazione passata non contiene lo 0 (che identifica il buco).");
        }

        private void CalcolaHoleSpaces(int size)
        {
            holeSpaces = " ";
            int i2 = size;
            while ((i2 /= 10) != 0)
            {
                holeSpaces += " ";
            }
        }

        /// <exception cref="InvalidOperationException">Se il buco è in prima riga</exception>
        public void MoveHoleUp()
        {
            if (holePosition < side)
                throw new System.InvalidOperationException("Hole is in the first row.");

            int newHolePosition = holePosition - side;

            table[holePosition] = table[newHolePosition];

            numeroMosse++;

            holePosition = newHolePosition;

            //Console.Clear();
            //Console.WriteLine(ToString());
        }

        /// <exception cref="InvalidOperationException">Se il buco è in ultima riga</exception>
        public void MoveHoleDown()
        {
            int newHolePosition = holePosition + side;

            if (newHolePosition >= table.Length)
                throw new System.InvalidOperationException("Hole is in the last row.");

            table[holePosition] = table[newHolePosition];

            numeroMosse++;

            holePosition = newHolePosition;

            //Console.Clear();
            //Console.WriteLine(ToString());
        }

        /// <exception cref="InvalidOperationException">Se il buco è in prima colonna</exception>
        public void MoveHoleLeft()
        {
            if (holePosition % side == 0)
                throw new System.InvalidOperationException("Hole is in the first column.");

            table[holePosition] = table[--holePosition];

            numeroMosse++;

            //Console.Clear();
            //Console.WriteLine(ToString());
        }

        /// <exception cref="InvalidOperationException">Se il buco è in ultima colonna</exception>
        public void MoveHoleRight()
        {
            if (holePosition % side == side - 1)
                throw new System.InvalidOperationException("Hole is in the last column.");

            table[holePosition] = table[++holePosition];

            numeroMosse++;

            //Console.Clear();
            //Console.WriteLine(ToString());
        }

        // 0 1 2 3
        // 4 5 6 7
        // ...
        public int CoordinateToIndex(int x, int y)
        {
            AssertValidCoodinate(x, true);
            AssertValidCoodinate(y, false);

            return x * side + y;
        }

        public int[] IndexToCoordinate(int index)
        {
            AssertValidIndex(index);

            int row = index / side;
            int col = index - (row * side);
            return new int[] { row, col };
        }

        private void AssertValidCoodinate(int i, Boolean isRow)
        {
            if (i < 0 || i >= side)
            {
                throw new System.ArgumentOutOfRangeException(i + " is not a valid " + (isRow ? "row" : "column") + " value.");
            }
        }

        private void AssertValidIndex(int i)
        {
            if (i < 0 || i >= table.Length)
            {
                throw new System.ArgumentOutOfRangeException(i + " is not a valid index.");
            }
        }

        public bool IsSolved()
        {
            for (int i = 1; i < table.Length; i++)
            {
                if (table[i - 1] != i)
                    return false;
            }

            return true;
        }

        public bool HasSameConfiguration(Board other)
        {
            if (other == null)
                return false;

            if (other.Size() != table.Length)
                return false;

            if (holePosition != other.GetHolePosition())
                return false;

            for (int i = 0; i < table.Length; i++)
            {
                if (i != holePosition && table[i] != other.GetNumberFromIndex(i))
                    return false; 
            }

            return true;
        }

        public int Size()
        {
            return size;
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            for (int x = 0; x < side; x++)
            {
                sb.Append("|");
                for (int y = 0; y < side; y++, i++)
                {
                    sb.Append("| ");
                    if (i == holePosition)
                        sb.Append(holeSpaces);
                    else {
                        int n = 1;
                        int numero = table[i];
                        int tmp = numero;
                        while ((tmp /= 10) != 0)
                        {
                            n++;
                        }
                        sb.Append(holeSpaces.Substring(n));
                        sb.Append(numero);
                    }

                    sb.Append(" |");
                }
                sb.Append("|\n");
            }

            return sb.ToString();
        }

        public int GetIndexOfNumber(int number)
        {
            if (number >= Size() || number <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < table.Length; i++)
            {
                if (table[i] == number && i != holePosition)
                    return i;
            }

            throw new Exception(number + " non è contenuto nella table!");
        }

        public int GetNumberFromIndex(int index)
        {
            if (index >= Size() || index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (index == holePosition)
                throw new Exception(index + " è la posizione del buco!");

            return table[index];
        }

        public int GetHolePosition()
        {
            return holePosition;
        }

        public int GetSide()
        {
            return side;
        }

        public void ResetNumeroMosse()
        {
            numeroMosse = 0;
        }

        public int GetNumeroMosse()
        {
            return numeroMosse;
        }
    }
}
