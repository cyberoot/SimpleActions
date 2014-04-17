using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE80;

namespace cyberoot.SimpleActions.Model.Actions
{
    public class VSCommandAction : BaseAction
    {
        public string Command { get; set; }

        public VSCommandAction()
        {
            Command = "";
        }

        public override void Execute(DTE2 dte)
        {
            DTE2 _dte = dte;
            _dte.ExecuteCommand(Command);
        }
    }
}
