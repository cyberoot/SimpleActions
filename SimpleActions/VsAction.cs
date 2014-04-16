using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE80;

namespace SimpleActions
{
    public class VsAction : IAction
    {
        private DTE2 _dte;

        public string Command { get; set; }

        public VsAction(string command)
        {
            Command = command;
        }

        public string GetKind()
        {
            return "VsAction";
        }

        public void Execute(DTE2 dte)
        {
            _dte = dte;
            _dte.ExecuteCommand(Command);
        }
    }
}
