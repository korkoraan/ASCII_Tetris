using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;

namespace ASCII_Tetris
{
    public class Well
    {
        private char[,] Cells { get; }
        internal int Y { get; }
        internal int X { get; }
        public Well(int sizeX, int sizeY)
        {
            X = sizeX - 1;
            Y = sizeY - 1;
            Cells = new char[sizeX , sizeY];
            for (var i = 0; i < sizeY; i++)
            {
                for (var j = 0; j < sizeX; j++)
                {
                    Cells[j, i] = ' ';
                }
            }
        }

        public bool CanPut(Figure figure)
        {
            try
            {
                return !figure.Cells.Any(cell => Cells[cell.X, cell.Y] != ' ' & cell.C != ' ');
            }
            catch (IndexOutOfRangeException e)
            {
                return false;
            }
        }
        public bool Put(Figure figure)
        {
            foreach (var (x, y, c) in figure.Cells)
            {
                if(c != ' ')
                    Cells[x, y] = c;
            }

            return true;
        }

        private bool RowIsFull(int y)
        {
            for (var x = X; x >= 0; x--)
            {
                if (Cells[x, y] == ' ')
                {
                    return false;
                }
            }

            return true;
        } 
/// <summary>.
/// </summary>
/// <returns>number of removed rows</returns>
        private int RemoveFullRows()
        {
            var delRows = 0;
            for (var y = Y; y >= 0; y--)
            {
                if (!RowIsFull(y)) continue;
                RemoveRow(y);
                delRows++;
                delRows += RemoveFullRows();
            }

            return delRows;
        }

        private void RemoveRow(int y)
        { 
            var tmp = new List<char>();
            for (var _y = y - 1; _y >= 0; _y--)
            {
                for (var x = X; x >= 0; x--)
                {
                    tmp.Add(Cells[x,_y]);
                }
            }

            for (var _y = y; _y >= 0; _y--)
            {
                for (var x = X; x >= 0; x--)
                {
                    if (tmp.Count == 0)
                        break;
                    Cells[x,_y] = tmp.First();
                    tmp.Remove(tmp[0]);
                }
            }
        }
        
        internal int UpdateWell()
        {
            var score = RemoveFullRows();
            return score;
        }

        public char this[int x, int y] => Cells[x, y];
    }
}