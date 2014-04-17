using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cyberoot.SimpleActions.Model.Actions;

namespace cyberoot.SimpleActions.Model
{
    public class ActionBatch
    {
        public IList<IAction> Actions { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
