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
using System.Linq;

namespace PushdownAutomaton
{
    public enum PDARecognitionResult { InputIsNotValid, Recognized, NotRecognized };
    public class PDA
    {
        public static string initialStackSymbol = "<Z0>";
        public IEnumerable<PDATransition> transitions { get; private set; }
        public int startState { get; private set; }
        public ISet<int> states { get; private set; }
        public IEnumerable<string> stackAlphabet { get; private set; }
        public IEnumerable<string> inputAlphabet { get; private set; }

        //Initiating random here to pick random transitions
        private readonly Random random = new Random();

        public PDA(IEnumerable<string> inputAlphabet,
                   IEnumerable<string> stackAlphabet,
                   ISet<int> states, int startState,
                   IEnumerable<PDATransition> transitions)
        {
            this.inputAlphabet = inputAlphabet;
            this.stackAlphabet = stackAlphabet;
            this.states = states;
            this.startState = startState;
            this.transitions = transitions;
        }

        public PDARecognitionResult Recognize(string[] input)
        {
            if (IsInputValid(input))
            {
                var inputStack = new Stack<string>();

                foreach (var element in input.Reverse())
                {
                    inputStack.Push(element);
                }

                if (CanRecognize(inputStack))
                {
                    return PDARecognitionResult.Recognized;
                }
                else
                {
                    return PDARecognitionResult.NotRecognized;
                }

            }
            else
            {
                return PDARecognitionResult.InputIsNotValid;
            }
        }

        public bool CanRecognize(Stack<string> input)
        {
            return GetResults(input).Any(result => result.isSuccessfull);
        }

        public IEnumerable<PDAResult> GetResults(Stack<string> inputStack)
        {

            var results = new HashSet<PDAResult>();

            string[] startStackArray = new string[] { initialStackSymbol };

            var conditions = new List<PDACondition>() { new PDACondition(inputStack, new Stack<string>(startStackArray), 0) };
            var hasFoundMatch = false;

            while (conditions.Count > 0 && !hasFoundMatch)
            {

                var newCondtionsSet = new HashSet<PDACondition>();

                foreach (var condition in conditions)
                {
                    var nextConditions = FindNextConditions(condition).ToList();

                    if (nextConditions.Count == 0)
                    {
                        results.Add(new PDAResult(false, condition));
                        continue;
                    }

                    foreach (var nextCondition in nextConditions)
                    {
                        if (nextCondition.currentInput.Count == 0 && nextCondition.stack.Count == 0)
                        {
                            results.Add(new PDAResult(true, nextCondition));
                            hasFoundMatch = true;
                            break;
                        }
                        else
                        {
                            newCondtionsSet.Add(nextCondition);
                        }
                    }

                    if (hasFoundMatch)
                    {
                        break;
                    }
                }

                conditions = newCondtionsSet.ToList();
            }

            return results;
        }

        private IEnumerable<PDACondition> FindNextConditions(PDACondition condition)
        {
            return from transition in transitions
                   where transition.IsSuitable(condition)
                   select ApplyTransition(transition, condition);
        }

        private IEnumerable<PDATransition> FindNextTransitionsForGenerating(PDACondition condition)
        {
            return from transition in transitions
                   where transition.IsSuitableForGenerating(condition)
                   select transition;
        }

        private static PDACondition ApplyTransition(PDATransition transition, PDACondition condition)
        {
            var newCurrentInput = new Stack<string>(new Stack<string>(condition.currentInput));
            var newStack = new Stack<string>(new Stack<string>(condition.stack));
            //We always pop something from stack
            newStack.Pop();

            //If we should read something from input, we remove it from current input
            if (transition.readFromInput != "")
            {
                newCurrentInput.Pop();
            }
            //Pushing elements to stack
            foreach (string elementToPush in transition.pushToStack.Reverse())
            {
                if (elementToPush != "")
                {
                    newStack.Push(elementToPush);
                }
            }

            return new PDACondition(newCurrentInput, newStack, transition.nextState);
        }

        public string[] Generate()
        {

            List<string> generatedString = new List<string>();

            var results = new HashSet<PDAResult>();

            string[] startStackArray = new string[] { initialStackSymbol };

            var condition = new PDACondition(new Stack<string>(), new Stack<string>(startStackArray), 0);

            while (true)
            {
                var nextTransitions = FindNextTransitionsForGenerating(condition);

                if (nextTransitions.Any())
                {
                    int index = random.Next(0, nextTransitions.Count());

                    var nextTransition = nextTransitions.ElementAt(index);

                    if (nextTransition.readFromInput != "")
                    {
                        generatedString.Add(nextTransition.readFromInput);
                        condition.currentInput.Push(nextTransition.readFromInput);
                    }

                    condition = ApplyTransition(nextTransition, condition);
                }
                else
                {
                    break;
                }
            }

            return generatedString.ToArray();
        }

        //Use this split method only if input is not separated by any character
        //and there is no such alphabet element that is the left side of another element
        //Example "word" and "wordA" - are not accepted
        //Whereas "word" and "Aword" - accepted
        public string[] Split(string input)
        {
            List<string> splitted = new List<string>();
            List<string> hypoStrings = new List<string>();
            string currentString = "";

            for (int i = 0; i < input.Length; i++)
            {

                char current = input[i];
                currentString += current;
                hypoStrings = inputAlphabet.ToList().FindAll(x => x.Contains(currentString));

                if (hypoStrings.Count == 0)
                {
                    throw new Exception("Invalid string.");
                }
                else
                {
                    if (hypoStrings.Any(x => x == currentString))
                    {
                        splitted.Add(currentString);
                        currentString = "";
                    }
                }


            }

            return splitted.ToArray();
        }

        private bool IsInputValid(string[] input)
        {
            foreach (var element in input)
            {
                if (!inputAlphabet.Contains(element))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
