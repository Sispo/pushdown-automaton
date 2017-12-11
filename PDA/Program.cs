/*
 * Copyright (c) 2017 Tymofii Dolenko
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */


using System;
using System.Collections.Generic;
using PushdownAutomaton;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var stackElementO = "<0>";
            var alphabet = new string[]{ "(", ")" };
            //Stack alphabet consists of all non-terminals + the initial stack symbol
            var stackAlphabet = new string[] { stackElementO, PDA.initialStackSymbol };

            //Transition from state 0 to state 1 (first and second parameters)
            //We read ( from input (third parameter)
            //We pop Z0 (the initial stack symbol) from stack (fourth parameter)
            //We push <0> and Z0 to stack (last parameters, we can have more than one at a time)
            var transitionStart = new PDATransition(0, 1, "(", PDA.initialStackSymbol, stackElementO, PDA.initialStackSymbol);

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
            var transitionFinal = new PDATransition(1, 2, "", PDA.initialStackSymbol, "");

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
