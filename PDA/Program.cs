using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushdownAutomaton;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {

            
            
        }

        public static void Show(bool isAdmitted)
        {
            Console.WriteLine();
            if (isAdmitted)
            {
                Console.WriteLine("String is recognized");
            }
            else
            {
                Console.WriteLine("String is not recognized");
            }
        }

        public static string GetString()
        {
            Console.Write("\nEnter string: ");
            return Console.ReadLine();
        }
    }
}
