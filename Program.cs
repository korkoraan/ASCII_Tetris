using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Timers;
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
                    result = new FigureI(new Coords(rnd.Next(9), 0));
                    break;
                default:
                    result = new FigureI(new Coords(rnd.Next(9), 0)); // большинство фигур отсутствует
                    break;
            }

            return result;
        }

        void Update(Object source, ElapsedEventArgs e)
        {
            if (FigureHasFallen(CurrentFigure))
            {
                CurrentFigure = SpawnRandomFigure();
            }
            
            foreach (var coord in CurrentFigure.Coords)
            {
                gameField[coord.Y + coord.X] = 1;
            }
            Console.WriteLine(screen);
            
            foreach (var coord in CurrentFigure.Coords)
            {
                gameField[coord.Y + coord.X] = 0;
            }
            CurrentFigure.Tail.Y--;
            timer.Start();
        }

        private Timer timer = new Timer(DefaultSpeed);
        

        void Start()
        {
            CreateScreen(); 
            timer.Elapsed += Update;
            Console.WriteLine(screen);
            timer.Start();
            CurrentFigure = SpawnRandomFigure();
        }

        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
            Console.ReadKey();
        }
    }
}
