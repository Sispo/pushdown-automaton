using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushdownAutomaton
{
    public class PDATransition
    {
        public int state { get; private set; }
        public int nextState { get; private set; }
        public string readFromInput { get; private set; }
        public string popFromStack { get; private set; }
        public string[] pushToStack { get; private set; }

        public PDATransition(int previousState, int nextState, string readFromInput, string popFromStack, string[] pushToStack)
        {
            this.state = previousState;
            this.nextState = nextState;
            this.readFromInput = readFromInput;
            this.popFromStack = popFromStack;
            this.pushToStack = pushToStack;
        }

        public PDATransition(int previousState, int nextState, string readFromInput, string popFromStack, string pushToStack)
        {
            this.state = previousState;
            this.nextState = nextState;
            this.readFromInput = readFromInput;
            this.popFromStack = popFromStack;
            this.pushToStack = new string[1]{ pushToStack };
        }

        public bool IsSuitable(PDACondition condition)
        {
            return (condition.stack.Count > 0 && condition.state == state && condition.stack.Peek() == popFromStack) && (readFromInput == "" || readFromInput == condition.currentInput.Peek());
        }

        public bool IsSuitableForGenerating(PDACondition condition)
        {
            return condition.stack.Count > 0 && condition.state == state && condition.stack.Peek() == popFromStack;
        }

        public override string ToString()
        {
            string readFromInputString = readFromInput == "" ? "ε" : readFromInput;
            string pushString = pushToStack.Length > 0 ? String.Join(",", pushToStack) : "ε";
            return $"({state},{readFromInputString},{popFromStack}) = ({nextState},{{{pushString}}})";
        }
    }
}
