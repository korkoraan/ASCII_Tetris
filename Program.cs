using System;
using System.Linq;
using Timer = System.Timers.Timer;

namespace ASCII_Tetris
{
    internal class Game
    {
        private const double DefaultSpeed = 1000.0;
        private Engine _engine;
        private Well Well;
        private Well DebugScreen;
        private Well GameScreen;
        private bool DebugState;
        private static readonly int rightConstraint;
        private static readonly int leftConstraint;
        private static readonly int topConstraint;
        private static readonly int bottomConstraint;
        private static readonly Coord WellSize;
        private static int Score;
        private Figure CurrentFigure;
        private Figure NextFigure;
        static Game()
        {
            WellSize = new Coord(10, 15);
            leftConstraint = 5;
            rightConstraint = leftConstraint + WellSize.X;
            topConstraint = 5;
            bottomConstraint = topConstraint + WellSize.Y;
            Score = 0;
        }
        static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues (typeof (T));
            return (T) v.GetValue(new Random().Next(v.Length));
        }
        Figure RandomFigure()
        {
            var f = RandomEnumValue<Figures>();
            Figure result;
            switch (f)
            {
                case Figures.I:
                    FigureMatrix fm = new[]
                    {
                        "####"
                    };
                    result = fm.Create(new Coord(0,0));
                    break;
                case Figures.L:
                    fm = new[]
                    {
                        "#  ",
                        "###"
                    };
                    result = fm.Create(new Coord(0,0));
                    break;
                case Figures.O:
                    fm = new[]
                    {
                        "##",
                        "##"
                    };
                    result = fm.Create(new Coord(0,0));
                    break;
                case Figures.Z:
                    fm = new[]
                    {
                        "## ",
                        " ##"
                    };
                    result = fm.Create(new Coord(0,0));
                    break;
                case Figures.T:
                    fm = new[]
                    {
                        "###",
                        " # "
                    };
                    result = fm.Create(new Coord(0,0));
                    break;
                case Figures.S:
                    fm = new[]
                    {
                        " ##",
                        "## "
                    };
                    result = fm.Create(new Coord(0,0));
                    break;
                case Figures.J:
                    fm = new[]
                    {
                        "  #",
                        "###"
                    };
                    result = fm.Create(new Coord(0,0));
                    break;
                default:
                    fm = new[]
                    {
                        "## ",
                        " ##"
                    };
                    result = fm.Create(new Coord(0,0));
                    break;
            }

            return result;
        }
        string[] GetWell()
        {
            string[] result = new string[WellSize.Y];
            for(var i = 0; i < WellSize.Y; i++)
            {
                for (int j = 0; j < WellSize.X; j++)
                {
                    result[i] += Well[j, i];
                }
            }

            return result;
        }

        private Timer timer = new Timer(DefaultSpeed);

        void Start()
        {
            CurrentFigure = RandomFigure();
            NextFigure = RandomFigure();
            _engine = new Engine(WellSize);
            Well = _engine.Well;
            GameScreen = _engine.Well;
            DebugState = false;
        }
        static Boolean CanMove(Well well, Figure figure, Direction dir)
        {
            var f = figure.Clone() as Figure;
            if (f == null)
                throw new NullReferenceException($"{figure} reference is null");
            f.Move(dir);
            return well.CanPut(f);
        }

        static Boolean CanTurn(Well well, Figure figure)
        {
            var f = figure.Clone() as Figure;
            if (f == null)
                throw new NullReferenceException($"{figure} reference is null");
            f.Turn();
            return well.CanPut(f);
        }
        //debug function    
        static void writeF(Figure f)
        {
            Console.WriteLine("\n------------------");
            foreach (var s in f.Cells)
            {
                Console.WriteLine(s);
            }
            var coords = "";
            foreach (var c in f.Cells)
            {
                coords += "x" + c.X + "y" + c.Y + " ";
            }
            Console.WriteLine("Coords - " + coords);
        }
        
        private Well CreateDebugScreen()
        {
            var result = new Well(WellSize.X, WellSize.Y);
            var debugInfo = "Current figure coords are: ";
            foreach (var cell in CurrentFigure.Cells)
            {
                debugInfo += cell.X + cell.Y + " ";
            }
            debugInfo += ". ";
            var parsed = debugInfo.ToList();
            var x = 0;
            var y = 0;
            while (parsed.Count != 0)
            {
                // result[x, y] = parsed.First();
                parsed.Remove(parsed.First());
                x++;
                if (x > result.X)
                {
                    x = 0;
                    y++;
                    if (y > result.Y)
                        break;
                }
            }
            
            return result;
        }
        void DebugMode(Boolean state)
        {
            switch (state)
            {
                case true :
                    DebugState = true;
                    GameScreen = Well;
                    DebugScreen = CreateDebugScreen();
                    Well = DebugScreen;
                    break;
                case false :
                    DebugState = false;
                    Well = GameScreen;
                    break;
            }
        }

        bool OnMove(Figure figure, Direction direction)
        {
            if (figure == null)
            {
                throw new NullReferenceException("figure is null");
            }

            if (!CanMove(_engine.Well, figure, direction))
                return false;
            figure.Move(direction);
            return true;
        }
        void DrawRect(int leftX, int topY, int width, int height)
        {
            for (var x = leftX; x < leftX + width; x++)
            {
                Console.SetCursorPosition(x,topY);
                Console.Write("-");            
                Console.SetCursorPosition(x,topY + height);
                Console.Write("-");
            }

            for (var y = topY; y < topY + height; y++)
            {
                Console.SetCursorPosition(leftX,y);
                Console.Write("|");            
                Console.SetCursorPosition(leftX + width,y);
                Console.Write("|");
            }
            
            Console.SetCursorPosition(leftX,topY);
            Console.Write('*');
            Console.SetCursorPosition(leftX + width,topY);
            Console.Write("*");
            Console.SetCursorPosition(leftX,topY + height);
            Console.Write("*");
            Console.SetCursorPosition(leftX + width,topY + height);
            Console.Write("*");
        }

        void DrawFigure(Figure figure)
        {
            foreach (var cell in figure.Cells)
            {
                Console.SetCursorPosition(cell.X + rightConstraint,cell.Y + topConstraint);
                if(cell.C != ' ')
                    Console.Write(cell.C);
            }
        }

        void DrawScoreBoard ()
        {
            var width = 10 + NextFigure.Matrix[0].Length;
            var height = NextFigure.Matrix.Length + 3;
            
            DrawRect(rightConstraint + 13, topConstraint - 1, width, height);
            
            Console.SetCursorPosition(rightConstraint + 15, 5);
            Console.WriteLine("Score: " + Score);
            Console.SetCursorPosition(rightConstraint + 15, 6);
            Console.WriteLine("Next:");
            var y = 6;
            foreach (var str in NextFigure.Matrix)
            {
                Console.SetCursorPosition(rightConstraint + 15, ++y);
                Console.Write(str);
            }

        }
        void DrawFrame(string[] well)
        {
            for (var x = rightConstraint - 1; x < well[0].Length + rightConstraint + 1; x++)
            {
                Console.SetCursorPosition(x,topConstraint - 1);
                Console.Write("-");            
                Console.SetCursorPosition(x,topConstraint + well.Length);
                Console.Write("-");
            }

            for (var y = topConstraint - 1; y < well.Length + topConstraint + 1; y++)
            {
                Console.SetCursorPosition(rightConstraint - 1,y);
                Console.Write("|");            
                Console.SetCursorPosition(rightConstraint + well[0].Length,y);
                Console.Write("|");
            }

            var y_ = topConstraint;
            foreach (var s in well)
            { 
                Console.SetCursorPosition(rightConstraint,y_);
                Console.WriteLine(s);
                y_++; 
            }
        }

        private void GameOver()
        {
            
        }
        static void Main(string[] args)
        {
            Game game = new Game();

            game.Start();
            while(true)
            {
                // Console.Beep();
                var backgroundColor = Console.BackgroundColor;
                var foregroundColor = Console.ForegroundColor; 
                
                Console.Clear();
                
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                
                var well = game.GetWell();
                game.DrawFrame(well);
                game.DrawScoreBoard();
                
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.Red;
                
                game.DrawFigure(game.CurrentFigure);

                var action = Console.ReadKey();
                switch (action.Key)
                {
                    case ConsoleKey.LeftArrow :
                        if (!CanMove(game.Well, game.CurrentFigure, Direction.Left) && !game.DebugState)
                            break;
                        game.CurrentFigure.Move(Direction.Left);
                        break;
                    case ConsoleKey.RightArrow :
                        if (!CanMove(game.Well, game.CurrentFigure, Direction.Right) && !game.DebugState)
                            break;
                        game.CurrentFigure.Move(Direction.Right);
                        break;
                    case ConsoleKey.Spacebar :
                        if (!CanTurn(game.Well,game.CurrentFigure) && !game.DebugState)
                            break;
                        game.CurrentFigure.Turn();
                        break;
                    case ConsoleKey.D :
                        if (game.DebugState)
                            break;
                        else
                        {
                            game.DebugMode(true);
                            break;
                        }
                    case ConsoleKey.E :
                        game.DebugMode(false);
                        break;
                    case ConsoleKey.Q:
                        return;
                    default:
                        if (!CanMove(game.Well, game.CurrentFigure, Direction.Down) && !game.DebugState)
                        {
                            if (!game.Well.Put(game.CurrentFigure))
                                return;
                            game.CurrentFigure = (Figure)game.NextFigure.Clone();
                            game.NextFigure = game.RandomFigure();
                            Score += game.Well.UpdateWell();
                        }
                        game.CurrentFigure.Move(Direction.Down);
                        break;
                    
                }
                
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = foregroundColor;
            }
        }
    }
}
