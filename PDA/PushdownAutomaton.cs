using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PDA
{
    public class PushdownAutomaton
    {
        public static string emptySymbol = "ε";
        public static string bottomSymbol = "Δ";
        public static string endSymbol = "┤";
        public static int rowHeaderWidth = 5;
        public static int rowWidth = 25;
        public static int stackOutputWidth = 20;
        public static int stackOutputSpaceWidth = 5;
        public static int iterationCount = 100;

        public List<string> inputAlphabet;
        public List<string> stackAlphabet;
        public List<List<PDAOperation>> controlMatrix;

        public Stack<string> stack = new Stack<string>();
        public Stack<string> outputStack = new Stack<string>();
        public string[] initialStackContent;
        public PushdownAutomaton(string fileName)
        {
            Exception wrongFormatException =  new Exception("Wrong format");

            initLists();

            using (StreamReader sr = new StreamReader(fileName))
            {
                inputAlphabet.AddRange(sr.ReadLine().Split(' '));
                stackAlphabet.AddRange(sr.ReadLine().Split(' '));
                initialStackContent = sr.ReadLine().Split(' ');

                while (!sr.EndOfStream)
                {
                    string[] str = sr.ReadLine().Split(';');
                    List<string> buffer = new List<string>();
                    int count = 0;

                    List<PDAOperation> operationsList = new List<PDAOperation>();

                    foreach (string operationsString in str)
                    {
                        count++;
                        PDAOperation operation = null;

                        string[] operationsStringArray = operationsString.Split(',');
                        
                        if (operationsStringArray.Length == 2)
                        {
                            string trimmedFirst = operationsStringArray[0].Trim();

                            if (trimmedFirst == "pull")
                            {
                                operation = new PDAOperation(emptySymbol);
                            } else if (trimmedFirst.StartsWith("replace"))
                            {
                                operation = new PDAOperation(Split(trimmedFirst.Substring(8, trimmedFirst.Length - 9),stackAlphabet));
                            }

                            if (operation != null)
                            {
                                string trimmedSecond = operationsStringArray[1].Trim();

                                switch (trimmedSecond)
                                {
                                    case "shift":
                                        operation.type = PDAOperationType.ReplaceShift;
                                        break;
                                    case "keep":
                                        operation.type = PDAOperationType.ReplaceKeep;
                                        break;
                                    default:
                                        throw wrongFormatException;
                                }
                            }
                        }

                        operationsList.Add(operation);
                    }

                    if (count != inputAlphabet.Count + 1)
                    {
                        throw wrongFormatException;
                    }

                    controlMatrix.Add(operationsList);
                }

                if (controlMatrix.Count != stackAlphabet.Count)
                {
                    throw wrongFormatException;
                }
            }
        }

        void initLists()
        {
            inputAlphabet = new List<string>();
            inputAlphabet.Add(endSymbol);
            stackAlphabet = new List<string>();
            stackAlphabet.Add(bottomSymbol);
            controlMatrix = new List<List<PDAOperation>>();
        }

        public bool Run(string[] input)
        {
            stack.Clear();
            outputStack.Clear();
            stack.Push(bottomSymbol);
            outputStack.Push(endSymbol);

            foreach (string element in initialStackContent)
            {
                stack.Push(element);
            }

            for (int i = input.Length - 1; i >= 0; i--)
            {
                outputStack.Push(input[i]);
            }

            ShowStacks();

            int x = 0;
            
            while(x < iterationCount)
            {
                x++;

                if (stack.Peek() == bottomSymbol && outputStack.Peek() == endSymbol)
                {
                    return true;
                }

                PDAOperation current = null;

                if (stack.Peek() == bottomSymbol)
                {
                    current = controlMatrix[controlMatrix.Count - 1][inputAlphabet.IndexOf(outputStack.Peek()) - 1];
                } else if (outputStack.Peek() == endSymbol)
                {
                    current = controlMatrix[stackAlphabet.IndexOf(stack.Peek()) - 1][controlMatrix.First().Count - 1];
                } else
                {
                    current = controlMatrix[stackAlphabet.IndexOf(stack.Peek()) - 1][inputAlphabet.IndexOf(outputStack.Peek()) - 1];
                }

                if (current == null)
                {
                    return false;
                }

                Replace(current);
                ShowStacks();
            }

            return false;
        }

        void Replace(PDAOperation operation)
        {

            stack.Pop();
            if (operation.replacement[0] != emptySymbol)
            {
                for (int i = operation.replacement.Length - 1; i >= 0; i--)
                {
                    stack.Push(operation.replacement[i]);
                }
            }

            if (operation.type == PDAOperationType.ReplaceShift)
            {
                outputStack.Pop();
            }
        }

        void ShowStacks()
        {
            Console.WriteLine(Normalize(GetStackString(stack, true), stackOutputWidth) + Normalize("", stackOutputSpaceWidth) + Normalize(GetStackString(outputStack, false), stackOutputWidth));
        }

        string GetStackString(Stack<string> stack, bool reversed)
        {
            string stackString = "";
            
            if (reversed)
            {
                for(int i = stack.Count - 1; i >= 0; i--)
                {
                    stackString += stack.ElementAt(i);
                }
            } else
            {
                foreach(string element in stack)
                {
                    stackString += element;
                }
            }
            return stackString;
        }

        public string[] Split(string input, List<string> alphabet)
        {
            List<string> splitted = new List<string>();
            List<string> hypoStrings = new List<string>();
            string currentString = "";

            for (int i = 0; i < input.Length; i++)
            {

                char current = input[i];
                currentString += current;
                hypoStrings = alphabet.FindAll(x => x.Contains(currentString));

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

        public bool AreValid(string[] s)
        {
            foreach (string str in s)
                if (!inputAlphabet.Contains(str))
                    return false;
            return true;
        }

        public void Show()
        {
            Console.Write("Input alphabet: ");
            foreach (string str in inputAlphabet)
                Console.Write(str + " ");
            Console.WriteLine();
            Console.Write("Stack alphabet: ");
            foreach (string str in stackAlphabet)
                Console.Write(str + " ");
            Console.WriteLine();
            Console.WriteLine();

            string columns = Normalize("stack", rowHeaderWidth);
            columns += " | ";

            for (int j = 1; j < inputAlphabet.Count; j++)
            {
                string column = Normalize(inputAlphabet[j], rowWidth);
                column += " | ";
                columns += column;
            }

            columns += inputAlphabet.First();
            Console.WriteLine(columns);

            for (int i = 0; i < controlMatrix.Count - 1; i++)
            {
                string rowHeaderString = Normalize($"{stackAlphabet[i]}",rowHeaderWidth);
                rowHeaderString += " | ";
                Console.Write(rowHeaderString);

                for (int j = 0; j < inputAlphabet.Count - 1; j++)
                {
                    if (controlMatrix[i][j] != null)
                    {
                        string rowString = Normalize(controlMatrix[i][j].ToString(), rowWidth);
                        rowString += " | ";
                        Console.Write(rowString);
                    } else
                    {
                        string rowString = Normalize("reject", rowWidth);
                        rowString += " | ";
                        Console.Write(rowString);
                    }
                }
                    
                Console.Write($"reject");
                Console.WriteLine();
            }

            string lastRow = Normalize(stackAlphabet[0], rowHeaderWidth);
            lastRow += " | ";

            for (int j = 0; j < inputAlphabet.Count - 1; j++)
            {
                string column = Normalize("reject", rowWidth);
                column += " | ";
                lastRow += column;
            }

            lastRow += "admit";
            Console.WriteLine(lastRow);
            Console.WriteLine();
            Console.WriteLine();
        }

        public string Normalize(string input, int numberOfSymbols)
        {
            string str = input;
            if (str.Length > numberOfSymbols)
            {
                str = str.Substring(0, numberOfSymbols - 3);
                str += "...";
                return str;
            } else if (str.Length < numberOfSymbols)
            {
                for(int i = str.Length; i < numberOfSymbols; i++)
                {
                    str += " ";
                }
                return str;
            } else
            {
                return str;
            }
        } 
    }
}
