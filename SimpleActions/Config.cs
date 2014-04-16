using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleActions
{
    static class Config
    {
        public static IList<ActionBatch> Init()
        {
            return new[]
            {
                new ActionBatch()
                {
                    Title = "DisplayName from xml comment",
                    Description = "Add DisplayName attribute from xml comment",
                    Actions = new IAction[]
                    {
                        new SearchReplaceAction()
                        {
                            IsRegExp = true,
                            MatchCase = false,
                            Pattern = @"\/\/\/\s+<summary>(.*)<\/summary>",
                            Replacement = @"$&\n\t\t[DisplayName(""$1"")]",
                        },
                        new VsAction("Edit.FormatDocument"), 
                    }
                }
            };
        }
    }
}
