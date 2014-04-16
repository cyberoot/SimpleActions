using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleActions
{
    public class ActionBatch
    {
        public IList<IAction> Actions { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
