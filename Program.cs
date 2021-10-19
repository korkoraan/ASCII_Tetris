using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Timers;
using System.Xml;
using Timer = System.Timers.Timer;

namespace ASCII_Tetris
{
    internal class Game
    {
        private const double DefaultSpeed = 1000.0; 
        private int[] gameField =           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // далее в качесте координат используется число, 
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // в котором последний знак - координата по Х, 
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // а все предыдущие - координата по Y
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                              0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        } ;

        private String screen = "";

        private static readonly Coords WellSize;
        static Game()
        {
            WellSize = new Coords(10, 15);
        }
        
        void CreateScreen()
        {
            screen += "-----------------\n";
            for (int i = 0; i < gameField.Length / 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    screen += gameField[i];
                }
                screen += "\n";
            }
            screen += "-----------------\n";    
        }

        private IFigure CurrentFigure;

        Boolean FigureHasFallen(IFigure figure)
        {
            if (figure.Coords != null)
                foreach (var coord in figure.Coords)
                {
                    if (gameField[coord.Y + 1 + coord.X] != 0 || gameField[coord.Y + 1 + coord.X] > gameField.Length) // 0 - пустая клетка
                    {
                        return true;
                    }
                }
            else throw new Exception("figure doesn't have coordinates");

            return false;
        }

        static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues (typeof (T));
            return (T) v.GetValue(new Random().Next(v.Length));
        }
        IFigure SpawnRandomFigure()
        {
            Random rnd = new Random();
            var f = RandomEnumValue<Figures>();
            IFigure result;
            switch (f)
            {
                case Figures.I:
                    result = new FigureI(new Coords(rnd.Next(4,WellSize.X - 1), 0)); // хардкод границ генерации
                    break;
                default:
                    result = new FigureI(new Coords(rnd.Next(4,WellSize.X - 1), 0)); // большинство фигур отсутствует
                    break;
            }

            return result;
        }

        static char ascii(int value)
        {
            return ((char) value);
        }
        
        String[] GetWell()
        {
            String[] result = new String[WellSize.Y];

            string tmp;
            for (int i = 0; i < WellSize.Y - 1; ++i)
            {
                tmp = "║";
                for (int j = 0; j < WellSize.X - 2; ++j)
                {
                    tmp += " ";
                }
                tmp += "║";
                result[i] = tmp;
            }

            tmp = "╚";
            for (int j = 0; j < WellSize.X - 2; ++j)
            {
                tmp += "═";
            }
            tmp += "╝";
            result[WellSize.Y - 1] = tmp;

            return result;
        } 
        
        void Update()
        {
        }

        private Timer timer = new Timer(DefaultSpeed);
        

        void Start()
        {
            CurrentFigure = SpawnRandomFigure();
        }

        void DrawFigure(IFigure figure)
        {
            foreach (var coord in figure.Coords)
            {
                Console.SetCursorPosition(coord.X, coord.Y);
                Console.Write("#");
                //░
            }
        }
        
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();

            while(true)
            {
                var backgroundColor = Console.BackgroundColor;
                var foregroundColor = Console.ForegroundColor; 
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                var well = game.GetWell();
                foreach (var s in well)
                {
                    Console.WriteLine(s);
                }

                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.Red;
                game.DrawFigure(game.CurrentFigure);

                var action = Console.ReadKey();
                switch (action.Key)
                {
                    case ConsoleKey.LeftArrow :
                        game.CurrentFigure.Move(Direction.Left);
                        break;
                    case ConsoleKey.RightArrow :
                        game.CurrentFigure.Move(Direction.Right);
                        break;
                    default:
                        game.CurrentFigure.Move(Direction.Down);
                        break;
                    
                }
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = foregroundColor; 
            }
        }
    }
}
