using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDA
{
    enum PDAS { NullNOneN = 1 }
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                Console.WriteLine($"\n1) Example PDA\n");

                PDAS selection = (PDAS)Convert.ToInt32(Console.ReadLine());

                switch (selection)
                {
                    case PDAS.NullNOneN:

                        PushdownAutomaton pa0n1n = new PushdownAutomaton("ex.txt");
                        pa0n1n.Show();

                        Show(pa0n1n.Run(pa0n1n.Split(GetString(), pa0n1n.inputAlphabet)));

                        break;
                }
            }
        }

        public static void Show(bool isAdmitted)
        {
            Console.WriteLine();
            if (isAdmitted)
            {
                Console.WriteLine("String is admitted");
            }
            else
            {
                Console.WriteLine("String is rejected");
            }
        }

        public static string GetString()
        {
            Console.Write("\nEnter string: ");
            return Console.ReadLine();
        }
    }
}
