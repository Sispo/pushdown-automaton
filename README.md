# pushdown-automaton

Pushdown automaton implementation in C#

Pushdown automaton consists of following:
- Input alphabet (∑)
```C#
IEnumerable<string> inputAlphabet
```
- Stack alphabet (Γ)
```C#
IEnumerable<string> stackAlphabet
```
- Set of states (Q)
```C#
ISet<int> states
```
- Set of transitions (δ)
```C#
IEnumerable<PDATransition> transitions
```

Where:
- There is start state ∈ Q
- Z0 is the initial stack symbol ∈ Γ
```C#
static string initialStackSymbol = "<Z0>";
```

**PDA recognizes input, if both input string and stack are _empty_.**

To initialize PDA:

```C#
var pda = new PDA(alphabet, stackAlphabet, states, startState, transitions);
```

To create transition from state **0** to state **1**, that reads string **a** from input, pops **A** from stack and pushes **B** and **C** to stack:
```C#
var transition = new PDATransition(0, 1, a, A, B, C);
```

**a** can be empty string. Then we will read nothing from input by applying this transition.
Element to push can likewise be empty string. Then we will push nothing to the stack.
For example:
```C#
var transition = new PDATransition(1, 1, "", A, "");
```
We remain in the state **1**, read **_nothing_** from the input, pop **A** from the stack and push **_nothing_** to the stack.

**Transition should always pop something from the stack.**

We can avoid this:
```C#
var transition = new PDATransition(1, 1, a, A, A);
```
We pop A and push it back to the stack.

You can easily recognize inputs
```C#
pda.Recognize(input);
```

You can even generate strings based on transitions you've provided to PDA
```C#
pda.Generate();
```
