using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using Sitecore.Security.Accounts;

namespace CapTech.Modules.WorxBox.Feature.Rules
{
    public class CreatedByMe<T> : WhenCondition<T> where T : RuleContext
    {
        protected override bool Execute(T ruleContext)
        {
            return ruleContext.Item.Statistics.CreatedBy.Equals(User.Current.Name.ToLowerInvariant());
        }
    }
}
