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
            var stackElementO = "<0>";
            var alphabet = new string[]{ "(", ")" };
            //Stack alphabet consists of all non-terminals + bottom stack element
            var stackAlphabet = new string[] { stackElementO, PDA.startStackElement };

            //Transition from state 0 to state 1 (first and second parameters)
            //We read ( from input (third parameter)
            //We pop Z0 (bottom element) from stack (fourth parameter)
            //We push <0> and Z0 to stack (last parameters, we can have more than one at a time)
            var transitionStart = new PDATransition(0, 1, "(", PDA.startStackElement, stackElementO, PDA.startStackElement);

            //Transition from state 1 to state 1
            //We read ( from input
            //We pop <0> from stack
            //We push <0><0> to stack
            var transitionOpen = new PDATransition(1, 1, "(", stackElementO, stackElementO, stackElementO);

            //Transition from state 1 to state 1
            //We read ) from input
            //We pop <0> from stack
            //We push nothing (empty string) to stack
            var transitionClose = new PDATransition(1, 1, ")", stackElementO, "");

            //Transition from state 1 to state 2 (final)
            //We read nothing (empty string) from input
            //We pop Z0 from stack
            //We push nothing (empty string) to stack
            //As a result both input and stack are now empty
            //We will successfully finish our recognition process
            var transitionFinal = new PDATransition(1, 2, "", PDA.startStackElement, "");

            var transitions = new PDATransition[]{ transitionStart, transitionOpen, transitionClose, transitionFinal };

            var states = new HashSet<int> { 0, 1, 2 };

            var pda = new PDA(alphabet, stackAlphabet, states, 0, transitions);

            //Recognizing valid string, expecting successfull result
            Show(pda.Recognize(pda.Split("(()())")));
            //Recognizing invalid string, expecting NotRecognized result
            Show(pda.Recognize(pda.Split("(()")));

            //Generating string using our pushdown automaton
            var generatedString = pda.Generate();

            Console.WriteLine(String.Join("",generatedString));

            //Recognizing generated string, obviously expecting successfull result
            Show(pda.Recognize(generatedString));
        }

        public static void Show(PDARecognitionResult result)
        {
            switch (result)
            {
                case PDARecognitionResult.InputIsNotValid:
                    Console.WriteLine("String is not valid");
                    break;
                case PDARecognitionResult.NotRecognized:
                    Console.WriteLine("String is not recognized");
                    break;
                case PDARecognitionResult.Recognized:
                    Console.WriteLine("String is recognized");
                    break;
            }
        }
    }
}
