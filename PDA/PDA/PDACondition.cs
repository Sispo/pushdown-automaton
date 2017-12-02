using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace PushdownAutomaton
{
    public class PDACondition
    {
        public Stack<string> currentInput { get; private set; }
        public Stack<string> stack { get; private set; }
        public int state { get; private set; }

        public PDACondition(Stack<string> currentInput, Stack<string> stack, int state)
        {
            this.currentInput = currentInput;
            this.stack = stack;
            this.state = state;
        }

        public override bool Equals(object obj)
        {
            var condition = (PDACondition)obj;
            return currentInput.Equals(condition.currentInput) && stack.Equals(condition.stack) && state == state;
        }

        public override int GetHashCode()
        {
            return currentInput.GetHashCode() ^
                stack.GetHashCode() ^
                state.GetHashCode();
        }

    }
}
