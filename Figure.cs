using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ASCII_Tetris
{
    public enum Figures
    {
        J, I , O , L , Z, T, S
    }

    public class Coords
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    
    interface IFigure
    {
        void Turn();
        int State { get; set; }
        Coords[] Coords { get; set; }
        Coords Tail { get; set; }
    }

    public class FigureI : IFigure
    {
        public Coords[] Coords { get; set; } = new Coords[4];
        
        public Coords Tail { get; set; }

        public FigureI(Coords coord)
        {
            Tail = coord;
            Coords[0] = Tail;
            if (coord.X > 5)
            {
                Coords[1] = new Coords(Tail.X - 1, 0);
                Coords[2] = new Coords(Tail.X - 2, 0);
                Coords[3] = new Coords(Tail.X - 3, 0);
            } 
        }
        
        public void Turn()
        {
        }
        public int State { get; set; }
    }
}