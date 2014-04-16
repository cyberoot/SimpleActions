using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE80;

namespace SimpleActions
{
    public interface IAction
    {
        string GetKind();
        void Execute(DTE2 dte);
    }
}
