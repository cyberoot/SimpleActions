using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE80;

namespace cyberoot.SimpleActions.Model.Actions
{
    public abstract class BaseAction : IAction
    {
        public string GetKind()
        {
            return GetType().Name;
        }

        public abstract void Execute(DTE2 dte);
    }
}
