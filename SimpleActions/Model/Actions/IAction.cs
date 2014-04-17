using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE80;

namespace cyberoot.SimpleActions.Model.Actions
{
    public interface IAction
    {
        string GetKind();
        void Execute(DTE2 dte);
    }
}
