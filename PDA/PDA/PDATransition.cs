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

namespace PushdownAutomaton
{
    public class PDATransition
    {
        public int state { get; private set; }
        public int nextState { get; private set; }
        public string readFromInput { get; private set; }
        public string popFromStack { get; private set; }
        public string[] pushToStack { get; private set; }

        public PDATransition(int previousState, int nextState, string readFromInput, string popFromStack, params string[] pushToStack)
        {
            this.state = previousState;
            this.nextState = nextState;
            this.readFromInput = readFromInput;
            this.popFromStack = popFromStack;
            this.pushToStack = pushToStack;
        }

        public bool IsSuitable(PDACondition condition)
        {
            return (condition.stack.Count > 0 && condition.state == state && condition.stack.Peek() == popFromStack) && (readFromInput == "" || (condition.currentInput.Count > 0 && readFromInput == condition.currentInput.Peek()));
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
