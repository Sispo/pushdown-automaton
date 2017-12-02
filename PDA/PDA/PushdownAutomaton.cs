using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Extensions;

namespace PushdownAutomaton
{
    public enum PDARecognitionResult { InputIsNotValid, Recognized, NotRecognized };
    public class PDA
    {
        public static string startStackElement = "<Z0>";
        public IEnumerable<PDATransition> transitions { get; private set; }
        public int startState { get; private set; }
        public ISet<int> states { get; private set; }
        public IEnumerable<string> stackAlphabet { get; private set; }
        public IEnumerable<string> inputAlphabet { get; private set; }

        //Initiating random here to pick random transitions
        private Random random = new Random();

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
            string[] lowercased = Lowercased(input);

            if (IsInputValid(lowercased))
            {
                var inputStack = new Stack<string>();

                foreach(var element in lowercased.Reverse())
                {
                    inputStack.Push(element);
                }

                if (CanRecognize(inputStack))
                {
                    return PDARecognitionResult.Recognized;
                } else
                {
                    return PDARecognitionResult.NotRecognized;
                }

            } else
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

            string[] startStackArray = new string[] { startStackElement };

            var conditions = new List<PDACondition>() { new PDACondition(inputStack, new Stack<string>(startStackArray),0) };
            var hasFoundMatch = false;

            while(conditions.Count > 0 && !hasFoundMatch)
            {

                var newCondtionsSet = new HashSet<PDACondition>();

                foreach(var condition in conditions)
                {
                    var nextConditions = FindNextConditions(condition).ToList();

                    if (nextConditions.Count == 0)
                    {
                        results.Add(new PDAResult(false, condition));
                        continue;
                    }

                    foreach(var nextCondition in nextConditions)
                    {
                        if (nextCondition.currentInput.Count == 0 && nextCondition.stack.Count == 0)
                        {
                            results.Add(new PDAResult(true, nextCondition));
                            hasFoundMatch = true;
                            break;
                        } else
                        {
                            newCondtionsSet.Add(nextCondition);
                        }
                    }

                    if(hasFoundMatch)
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
            var newCurrentInput = condition.currentInput.Clone();
            var newStack = condition.stack.Clone();
            //We always pop something from stack
            newStack.Pop();

            //If we should read something from input, we remove it from current input
            if (transition.readFromInput != "")
            {
                newCurrentInput.Pop();
            }
            //Pushing elements to stack
            foreach(string elementToPush in transition.pushToStack.Reverse())
            {
                if (elementToPush != "")
                {
                    newStack.Push(elementToPush);
                }
            }

            return new PDACondition(newCurrentInput, newStack, transition.nextState);
        }

        private string[] Lowercased(string[] input)
        {
            return (from element in input
                   select element.ToLower()).ToArray();
        }

        private bool IsInputValid(string[] input)
        {
            foreach(var element in input)
            {
                if (!inputAlphabet.Contains(element))
                {
                    return false;
                }
            }
            return true;
        }

        public List<string> Generate(double lengthCoefficient)
        {

            List<string> generatedString = new List<string>();

            //Settings boundaries for coefficient [0,1]
            double coefficient = lengthCoefficient > 1 ? 1 : lengthCoefficient < 0 ? 0 : lengthCoefficient;

            var results = new HashSet<PDAResult>();

            string[] startStackArray = new string[] { startStackElement };

            var condition = new PDACondition(new Stack<string>(), new Stack<string>(startStackArray), 0);

            while (true)
            {
                var nextTransitions = FindNextTransitionsForGenerating(condition);

                if (nextTransitions.Count() > 0)
                {
                    int index = random.Next(0, nextTransitions.Count());

                    var nextTransition = nextTransitions.ElementAt(index);
                    
                    if (nextTransition.readFromInput != "")
                    {
                        generatedString.Add(nextTransition.readFromInput);
                        condition.currentInput.Push(nextTransition.readFromInput);
                    }

                    condition = ApplyTransition(nextTransition, condition);
                } else
                {
                    break;
                }
            }

            return generatedString;
        }
    }
}
