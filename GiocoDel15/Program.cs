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
                x = 25;
            }

            Board b = new Board(x);

            Console.WriteLine("## Gioco del 15 applicato a " + b.GetSide() + "x" + b.GetSide() + " ##\n");

            Caos(b, 500);

            Console.WriteLine("Scombinato con " + b.GetNumeroMosse() + " mosse.");
            b.ResetNumeroMosse();

            string scelta;
            do
            {
                Console.Write("Risoluzione automatica [1], Gioco manuale [2]: ");
            }
            while (!(scelta = Console.ReadLine()).Equals("1") && !scelta.Equals("2"));

            if (scelta.Equals("2"))
            {
                Console.Clear();
                Console.WriteLine(b.ToString());
                Console.WriteLine("Risolvi se sei capace!!!");
                Gioca(b);
            }
            else
            {
                do
                {
                    Console.Write("Classica [1], Copia una configurazione [2]: ");
                }
                while (!(scelta = Console.ReadLine()).Equals("1") && !scelta.Equals("2"));

                Board target = null;
                if (scelta.Equals("2"))
                {
                    target = new Board(x);
                    Caos(target, 500);

                    Console.WriteLine("da così:\n" + b.ToString());
                    Console.WriteLine("deve diventare così:\n" + target.ToString());
                    Console.ReadLine();
                }

                Risolvi(b, target);

                Console.WriteLine(b.ToString());

                bool res;
                if (scelta.Equals("2"))
                {
                    Console.WriteLine("MIO:\n" + b.ToString());
                    Console.WriteLine("TARGET:\n" + target.ToString());
                    res = b.HasSameConfiguration(target);
                } 
                else
                {
                    res = b.IsSolved();
                }
                    

                if (res)
                    Console.WriteLine("Risolto con " + b.GetNumeroMosse() + " mosse.");
                else
                    Console.WriteLine("Cannato (" + b.GetNumeroMosse() + " mosse).");
            }

            Console.ReadLine();
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

        static void Risolvi(Board b, Board bTarget)
        {
            int finalRowHole = b.GetSide() - 1;
            int finalColHole = b.GetSide() - 1;
            if (bTarget != null)
            {
                if (b.Size() != bTarget.Size())
                {
                    Console.WriteLine("Hanno dimensioni deverse!!!");
                    return;
                }
                int[] hole = bTarget.IndexToCoordinate(bTarget.GetHolePosition());
                finalRowHole = hole[0];
                finalColHole = hole[1];
            }

            RisolviTranneUltime2Righe(b, bTarget, finalRowHole, finalColHole);
            RisolviUltime2Righe(b, bTarget, finalRowHole, finalColHole);

            for (int i = b.GetSide() - 1; i > finalRowHole; i--)
            {
                b.MoveHoleUp();
            }
            for (int i = b.GetSide() - 1; i > finalColHole; i--)
            {
                b.MoveHoleLeft();
            }
        }

        private static int GetNumberWithHoleTrick(Board bTarget, int index, int finalRowHole, int finalColHole)
        {
            int[] c = bTarget.IndexToCoordinate(index);

            // sono sull'utima colonna e su righe sotto il buco
            if (c[0] >= finalRowHole && c[1] == bTarget.GetSide() - 1)
                return bTarget.GetNumberFromIndex(index + bTarget.GetSide());

            if (c[0] == finalRowHole && c[1] >= finalColHole)
                return bTarget.GetNumberFromIndex(index + 1);

            return bTarget.GetNumberFromIndex(index);
        }

        private static void RisolviTranneUltime2Righe(Board b, Board bTarget, int finalRowHole, int finalColHole)
        {
            for (int numero = 1; numero <= b.Size() - 2 * b.GetSide(); numero++)
            {
                int index;
                if (bTarget != null)
                {
                    int numeroLetto = GetNumberWithHoleTrick(bTarget, numero - 1, finalRowHole, finalColHole);

                    index = b.GetIndexOfNumber(numeroLetto); // MOD
                }
                else
                {
                    index = b.GetIndexOfNumber(numero);
                }

                //Console.WriteLine("Tocca al numero " + numero);


                int[] num = b.IndexToCoordinate(index);
                int rowN = num[0];
                int colN = num[1];
                int[] hole = b.IndexToCoordinate(b.GetHolePosition());
                int rowB = hole[0];
                int colB = hole[1];

                //Console.WriteLine("Mi posizione a destra del numero!");
                if (rowB < rowN)
                {
                    {
                        if (colB == colN)
                        {
                            if (colB == b.GetSide() - 1)
                            {
                                // significa che dovrei tirare su li il numero quindi posso andare giu
                                // se però ci sono altre righe in mezzo devo andare giù e riposizionarmi sulla riga sotto al numero
                                b.MoveHoleDown();
                                rowB++;
                                if (rowB == rowN) // ok ho posizionato
                                {
                                    rowN--;
                                    continue;
                                }

                                // mi scosto di uno a sinistra
                                b.MoveHoleLeft();
                                colB--;
                            }
                            else
                            {
                                // mi scosto di uno a destra (che non da problemi)
                                b.MoveHoleRight();
                                colB++;
                            }
                        }
                        // adesso sono scostato di uno (destra o sinistra)
                        // posso spostare in giu fino alla riga del numero
                        while (rowB < rowN)
                        {
                            b.MoveHoleDown();
                            rowB++;
                        }
                    }
                }
                // sono sotto oppure sulla stessa riga del numero!

                PosizionaBuco(b, ref rowN, ref colN, ref rowB, ref colB);

                int[] destinazione = b.IndexToCoordinate(numero - 1);
                int rowDest = destinazione[0];
                int colDest = destinazione[1];

                // sposto in orizzontale fino alla colonna di destinazione
                //Console.WriteLine("Posiziono il numero nella colonna giusta.");
                if (colN != colDest)
                {
                    // NOTA: se entro qua dentro il buco va a finire sotto!!!

                    if (colN > colDest)
                    {
                        // sposto il numero a sinistra
                        //Console.WriteLine("sposto il numero a sinistra");
                        while (colN != colDest)
                        {
                            if (rowN < b.GetSide() - 1)
                            {
                                b.MoveHoleDown();
                                b.MoveHoleLeft();
                                b.MoveHoleLeft();
                                b.MoveHoleUp();
                            }
                            else {
                                b.MoveHoleUp();
                                b.MoveHoleLeft();
                                b.MoveHoleLeft();
                                b.MoveHoleDown();
                            }

                            b.MoveHoleRight();
                            colB--;
                            colN--;
                        }
                    }
                    else
                    {
                        do
                        {
                            // sposto il numero a destra
                            //Console.WriteLine("sposto il numero a destra");
                            b.MoveHoleLeft();
                            colB--;
                            colN++;


                            if (colN < b.GetSide() - 1)
                            {
                                if (rowN < b.GetSide() - 1)
                                {
                                    b.MoveHoleDown();
                                    b.MoveHoleRight();
                                    b.MoveHoleRight();
                                    b.MoveHoleUp();
                                }
                                else
                                {
                                    b.MoveHoleUp();
                                    b.MoveHoleRight();
                                    b.MoveHoleRight();
                                    b.MoveHoleDown();
                                }

                                colB += 2;
                            }
                        } while (colN != colDest);

                    }

                }

                // il buco può essere a destra o a sinistra!!
                // --> sinistra: è l'ultima riga!!!
                //               oppure ho spostato a destra!
                // --> destra: tutti gli altri casi.

                if (rowN == rowDest)
                {
                    continue;
                }

                // controllo il caso di completamento riga!!!
                if (colN == b.GetSide() - 1)
                {
                    //Console.WriteLine("Posiziono il fine riga.");
                    // il buco è sempre a sinistra!!!
                    // per tirare su il numero bisogna spostare i primi numeri
                    b.MoveHoleLeft();
                    while (rowB != rowDest)
                    {
                        b.MoveHoleUp();
                        rowB--;
                    }
                    b.MoveHoleRight();

                    b.MoveHoleRight();
                    colB++;

                    while (rowB != rowN)
                    {
                        b.MoveHoleDown();
                        rowB++;
                    }
                    rowN--;

                    while (rowN != rowDest)
                    {
                        b.MoveHoleLeft();
                        b.MoveHoleUp();
                        b.MoveHoleUp();
                        b.MoveHoleRight();
                        b.MoveHoleDown();
                        rowB--;
                        rowN--;
                    }

                    // POSIZIONATO!!!

                    // ripristino i due elementi
                    b.MoveHoleLeft();
                    b.MoveHoleUp();
                    b.MoveHoleLeft();
                    b.MoveHoleDown();
                    colB -= 2;

                    continue;
                }
                else
                {
                    // devo tirare su il numero e non è al fine riga
                    do
                    {
                        b.MoveHoleUp();
                        b.MoveHoleLeft();
                        b.MoveHoleDown();
                        b.MoveHoleRight();
                        b.MoveHoleUp();
                        rowB--;
                        rowN--;
                    } while (rowN != rowDest);
                }

            }
        }

        // posiziona il buco nella cella a destra del numero che si vuole processare
        private static void PosizionaBuco(Board b, ref int rowN, ref int colN, ref int rowB, ref int colB)
        {
            if (rowB == rowN)
            {
                if (colB < colN)
                {
                    // sposto in orizzontale a destra (fino a destra del numero)
                    do
                    {
                        b.MoveHoleRight();
                        colB++;
                    } while (colB < colN);
                    colN--;
                }
                else {
                    // sposto in orizzontale a sinistra (fino a destra del numero)
                    while (colB > colN + 1)
                    {
                        b.MoveHoleLeft();
                        colB--;
                    }
                }
            }
            else {

                // sposto in orizzontale (fino a fianco al numero)
                int colTarget;
                if (colN < b.GetSide() - 1)
                    colTarget = colN + 1;
                else // siamo sul bordo destro
                    colTarget = colN - 1;
                while (colB != colTarget)
                {
                    if (colB > colTarget)
                    {
                        b.MoveHoleLeft();
                        colB--;
                    }
                    else
                    {
                        b.MoveHoleRight();
                        colB++;
                    }
                }

                // sposto in verticale (fino alla riga del numero)
                if (rowB >= rowN)
                {
                    while (rowB != rowN)
                    {
                        b.MoveHoleUp();
                        rowB--;
                    }
                }
                else
                {
                    do
                    {
                        b.MoveHoleDown();
                        rowB++;
                    } while (rowB != rowN);
                }

                if (colB < colN)
                {
                    b.MoveHoleRight();
                    colB++;
                    colN--;
                }
            }
        }

        private static void RisolviUltime2Righe(Board b, Board bTarget, int finalRowHole, int finalColHole)
        {
            int numero = b.Size() - 2 * b.GetSide() + 1;
            for (int i = 0; i < b.GetSide() - 1; i++, numero++) // occhio che l'ultima iterazione è solo perché entra in casi perticolari gestiti che non da eccezione!!!
            {
                int index;
                if (bTarget != null)
                {
                    int numeroLetto = GetNumberWithHoleTrick(bTarget, numero - 1, finalRowHole, finalColHole);
                    index = b.GetIndexOfNumber(numeroLetto); // MOD
                }
                else
                {
                    index = b.GetIndexOfNumber(numero);
                }

                int[] num = b.IndexToCoordinate(index);
                int rowN = num[0];
                int colN = num[1];
                int[] hole = b.IndexToCoordinate(b.GetHolePosition());
                int rowB = hole[0];
                int colB = hole[1];

                int[] destinazione = b.IndexToCoordinate(numero - 1);
                int rowDest = destinazione[0];
                int colDest = destinazione[1];



                PosizionaBuco(b, ref rowN, ref colN, ref rowB, ref colB);

                // muovo il numero a sinistra fino alla propria colonna
                while (colN != colDest)
                {
                    if (rowN < b.GetSide() - 1)
                    {

                        b.MoveHoleDown();
                        b.MoveHoleLeft();
                        b.MoveHoleLeft();
                        b.MoveHoleUp();
                    }
                    else {
                        b.MoveHoleUp();
                        b.MoveHoleLeft();
                        b.MoveHoleLeft();
                        b.MoveHoleDown();
                    }

                    b.MoveHoleRight();
                    colB--;
                    colN--;
                }

                // numero con una riga in più
                int index2;
                if (bTarget != null)
                {
                    int numeroLetto = GetNumberWithHoleTrick(bTarget, numero - 1 + b.GetSide(), finalRowHole, finalColHole);
                    index2 = b.GetIndexOfNumber(numeroLetto);
                }
                else
                {
                    index2 = b.GetIndexOfNumber(numero + b.GetSide());
                }
                num = b.IndexToCoordinate(index2);
                int rowN2 = num[0];
                int colN2 = num[1];

                //posiziona in basso!
                if (rowN == rowDest)
                {
                    if (colN2 == colDest)
                    {
                        // significa che sono posizionati giusti;
                        continue;
                    }

                    b.MoveHoleDown();
                    b.MoveHoleLeft();
                    b.MoveHoleUp();
                    b.MoveHoleRight();
                    b.MoveHoleDown();

                    if (colN2 == colDest + 1)
                    {
                        // allora ho messo in posizione sopra a N
                        rowN2--;
                        colN2--;
                    }

                    rowN++;
                    rowB++;
                }

                if (colN2 == colDest)
                {
                    // situazione
                    // N2 .
                    // N  h
                    VerticalSwitch(b);
                    rowN--;
                    rowN2++;
                }
                else
                {
                    // devo portare il numero
                    if (colN2 == colDest + 1)
                    {
                        b.MoveHoleUp();
                        rowB--;
                        rowN2++;
                    }
                    else
                    {
                        if (rowN2 == rowDest)
                        {
                            b.MoveHoleUp();
                            rowB--;
                        }

                        do
                        {
                            b.MoveHoleRight();
                            colB++;
                        } while (colB != colN2);

                        colN2--;

                        // muovo il numero2 a sinistra fino alla colonna desiderata (colDest + 1)
                        // muovo il numero a sinistra fino alla propria colonna
                        while (colN2 != colDest + 1)
                        {
                            if (rowN2 < b.GetSide() - 1)
                            {

                                b.MoveHoleDown();
                                b.MoveHoleLeft();
                                b.MoveHoleLeft();
                                b.MoveHoleUp();
                            }
                            else {
                                b.MoveHoleUp();
                                b.MoveHoleLeft();
                                b.MoveHoleLeft();
                                b.MoveHoleDown();
                            }

                            b.MoveHoleRight();
                            colB--;
                            colN2--;
                        }

                        // .      oppure . N2 b
                        // N N2 b        N 
                        if (rowN2 == rowDest + 1)
                        {
                            b.MoveHoleUp();
                            b.MoveHoleLeft();
                            rowB--;
                            colB--;
                        }
                        else
                        {
                            b.MoveHoleDown();
                            b.MoveHoleLeft();
                            b.MoveHoleUp();
                            colB--;
                            rowN2++;
                        }
                    }

                    // da         a
                    // . b    ->  N  .
                    // N N2       N2 b
                    b.MoveHoleLeft();
                    b.MoveHoleDown();
                    b.MoveHoleRight();

                    rowN--;
                    colN2--;
                    rowB++;
                }

            }

            // posiziona il buco nella cella [Side - 1][Side - 1]
            if (b.GetHolePosition() != b.Size() - 1)
            {
                b.MoveHoleDown();
                //rowB++;
            }
        }

        // from  NUM1         to  NUM2
        //       NUM2 hole        NUM1 hole
        // usa come supporto le celle:
        //     [rowHole - 1][colHole]
        //     [rowHole - 1][colHole + 1]
        //     [rowHole][colHole + 1]
        static void VerticalSwitch(Board b)
        {
            b.MoveHoleUp();
            b.MoveHoleLeft();
            b.MoveHoleDown();
            b.MoveHoleRight();
            b.MoveHoleRight();
            b.MoveHoleUp();
            b.MoveHoleLeft();
            b.MoveHoleLeft();
            b.MoveHoleDown();
            b.MoveHoleRight();
            b.MoveHoleUp();
            b.MoveHoleLeft();
            b.MoveHoleDown();
            b.MoveHoleRight();
            b.MoveHoleUp();
            b.MoveHoleRight();
            b.MoveHoleDown();
            b.MoveHoleLeft();
            b.MoveHoleUp();
            b.MoveHoleLeft();
            b.MoveHoleDown();
            b.MoveHoleRight();
        }
    }
}
