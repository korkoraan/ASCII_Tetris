using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCII_Tetris
{
    public enum Figures
    {
        J, I , O , L , Z, T, S
    }
    public enum Direction
    {
        Down, Left, Right
    }
    
    public class FigureMatrix
    {
        private string[] Matrix { get; }

        private FigureMatrix(string[] matrix)
        {
            // TODO: check dimension!
            Matrix = matrix;
        }

        public Figure Create(Coord center) => new((string[])Matrix.Clone(), center);

        public static implicit operator FigureMatrix(string[] data) => new FigureMatrix(data);
    }

    public record Cell(int X, int Y, char C);
    public class Figure : ICloneable
    {
        internal string[] Matrix { get; private set; }
        public List<Cell> Cells { get; private set; }
        public Cell this[int x, int y]
        {
            get
            {
                foreach (var cell in Cells)
                {
                    if (cell.X == Center.X + x && cell.Y == Center.Y + y)
                        return cell;
                }

                throw new ArgumentException($"coordinate {x},{y} not found in {this}");
            }
        } 
        List<Cell> MatrixToCells(string[] matrix, Coord center)
        {
            var data = new List<Cell>();
            var x = 0;
            var y = 0;
            foreach (var str in matrix)
            {
                foreach(var c in str)
                {
                    data.Add(new Cell(center.X + x, center.Y + y, c));
                    x++;
                }

                x = 0;
                y++;
            }

            return data;
        }
        public Figure(string[] matrix, Coord center)
        {
            Matrix = matrix;
            Cells = new List<Cell>();
            Cells = MatrixToCells(matrix, center);
        }

        private Figure(List<Cell> cells)
        {
            Cells = new List<Cell>(cells);
        }
        
        public void Turn()
        {
            var result = new List<string>();
            for (var i = Matrix.Length - 1; i >= 0 ; --i)
            {
                var s = Matrix[i];
                for (var j = 0; j < s.Length; ++j)
                {
                    if (result.Count <= j)
                    {
                        result.Add("");
                    }
                    result[j] += s[j];
                }
            }
            Matrix = result.ToArray();
            Cells = MatrixToCells(Matrix, Center);
        }

        public void Move(Direction dir)
        {
            switch (dir)
            {
                case Direction.Down:
                    Cells = Cells.Select(cell => cell with {Y = cell.Y + 1}).ToList();
                    break;
                case Direction.Left:
                    Cells = Cells.Select(cell => cell with {X = cell.X - 1}).ToList();
                    break;
                case Direction.Right:
                    Cells =  Cells.Select(cell => cell with {X = cell.X + 1}).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }

        private Coord Center => new(Cells[0].X, Cells[0].Y);

        public object Clone()
        {
            var f = new Figure(Matrix, Center)
            {
                Cells = new List<Cell>(Cells)
            };
            return f;
        }
    }
}