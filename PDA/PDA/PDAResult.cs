using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushdownAutomaton
{
    public class PDAResult
    {
        public PDACondition condition { get; private set; }
        public bool isSuccessfull { get; private set; }

        public PDAResult(bool isSuccessfull, PDACondition condition)
        {
            this.isSuccessfull = isSuccessfull;
            this.condition = condition;
        }
    }
}
