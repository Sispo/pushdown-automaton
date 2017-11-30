using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDA
{
    enum PDAS { Example = 1 }
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
                    case PDAS.Example:

                        PushdownAutomaton examplePA = new PushdownAutomaton("../../../ex.txt");
                        examplePA.Show();

                        Show(examplePA.Run(examplePA.Split(GetString(), examplePA.inputAlphabet)));

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
