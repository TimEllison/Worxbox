using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;
using Sitecore.Rules;

namespace CapTech.Modules.Worxbox.Foundation.Extensions
{
    public static class RulesExtensions
    {
        public static bool EvaluateConditions<T>(this Item root, string field, T ruleContext)
            where T : RuleContext
        {
            var stack = new RuleStack();
            foreach (Rule<T> rule in RuleFactory.GetRules<T>(new[] { root }, field).Rules)
            {
                if (rule.Condition != null)
                {
                    rule.Condition.Evaluate(ruleContext, stack);

                    if (ruleContext.IsAborted)
                    {
                        continue;
                    }
                }
            }
            return stack.Count != 0 && (bool) stack.Pop();
        }
    }
}
