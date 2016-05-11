using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiocoDel15
{
    class Program
    {
        static void Main(string[] args)
        {
            int x = 0;

            if (args.Length == 0 || !Int32.TryParse(args[0], out x))
            {
                //Console.WriteLine("Specificare il numero di elementi!");
                //return;
                x = 16;
            }

            Board b = new Board(x);

            Console.WriteLine("## Gioco del 15 applicato a " + b.GetSide() + "x" + b.GetSide() + " ##\n");

            Caos(b, 500);

            Console.WriteLine("Scombinato con " + b.GetNumeroMosse() + " mosse.");
            b.ResetNumeroMosse();

            Console.WriteLine("Risolvi se sei capace!!!");

            Gioca(b);

        }

        // gioco manuale utilizzando le frecce
        static void Gioca(Board b)
        {
            int numeroMosse = 0;
            do
            {
                ConsoleKeyInfo key = Console.ReadKey();
                try
                {
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            b.MoveHoleUp();
                            break;
                        case ConsoleKey.LeftArrow:
                            b.MoveHoleLeft();
                            break;
                        case ConsoleKey.DownArrow:
                            b.MoveHoleDown();
                            break;
                        case ConsoleKey.RightArrow:
                            b.MoveHoleRight();
                            break;
                    }
                }
                catch (System.InvalidOperationException e)
                {
                    // DO NOTHING
                }
                numeroMosse++;
                Console.Clear();
                Console.WriteLine(b.ToString());
            } while (!b.IsSolved());
            Console.WriteLine("Risolto in " + numeroMosse + " mosse.");
            Console.ReadLine();
        }

        static void Caos(Board b, int numeroDiMosse)
        {
            Random rnd = new Random();
            for (int i = 0; i < numeroDiMosse; i++)
            {
                int mossa = rnd.Next(0, 4);
                try
                {
                    switch (mossa)
                    {
                        case 0:
                            b.MoveHoleUp();
                            break;
                        case 1:
                            b.MoveHoleDown();
                            break;
                        case 2:
                            b.MoveHoleLeft();
                            break;
                        default:
                            //case 3:
                            b.MoveHoleRight();
                            break;
                    }

                    //Console.Clear();
                    //Console.WriteLine(b.ToString());
                }
                catch (System.InvalidOperationException e)
                {
                    i--;
                }
            }
        }
    }
}
