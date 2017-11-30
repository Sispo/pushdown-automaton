using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDA
{
    public enum PDAOperationType { ReplaceShift, ReplaceKeep };
    public class PDAOperation
    {
        public PDAOperationType type;
        private string[] _replacement; 

        public string[] replacement
        {
            get
            {
                return _replacement;
            }
        }

        public PDAOperation(string[] replacement)
        {
            _replacement = replacement;
            Array.Reverse(_replacement);
        }

        public PDAOperation(string replacement)
        {
            _replacement = new string[1];
            _replacement[0] = replacement;
        }

        public override string ToString()
        {
            string desc = "";
            if (_replacement[0] == PushdownAutomaton.emptySymbol)
            {
                desc += "pull, ";
            } else
            {
                desc += $"replace ({String.Concat(_replacement)}), ";
            }

            switch(type)
            {
                case PDAOperationType.ReplaceKeep:
                    desc += "keep";
                    break;
                case PDAOperationType.ReplaceShift:
                    desc += "shift";
                    break;
            }

            return desc;
        }

    }
}
