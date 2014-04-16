using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;

namespace SimpleActions
{
    public class SearchReplaceAction : IAction
    {
        private DTE2 _dte;
        private Find2 findWin;

        public SearchReplaceAction()
        {
            MatchCase = false;
            IsRegExp = true;
        }

        public string Pattern { get; set; }
        public string Replacement { get; set; }
        public bool MatchCase { get; set; }
        public bool IsRegExp { get; set; }

        public string GetKind()
        {
            return "SearchReplaceAction";
        }

        public void Execute(DTE2 dte)
        {
            _dte = dte;
            findWin = (Find2)_dte.Find;

            findWin.WaitForFindToComplete = true;
            findWin.MatchCase = MatchCase;
            findWin.PatternSyntax = IsRegExp ? vsFindPatternSyntax.vsFindPatternSyntaxRegExpr : vsFindPatternSyntax.vsFindPatternSyntaxLiteral;
            findWin.Action = vsFindAction.vsFindActionReplaceAll;
            findWin.FindWhat = Pattern;
            findWin.ReplaceWith = Replacement;
            findWin.Execute();
        }
    }
}
