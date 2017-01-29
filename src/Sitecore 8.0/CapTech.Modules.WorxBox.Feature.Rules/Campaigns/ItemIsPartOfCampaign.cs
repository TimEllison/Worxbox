using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace CapTech.Modules.WorxBox.Feature.Rules.Campaigns
{
    public class ItemIsPartOfCampaign<T> : WhenCondition<T> where T : RuleContext
    {
        public string CampaignItemId { get; set; }
        protected override bool Execute(T ruleContext)
        {
            var trackingField = new Sitecore.Analytics.Data.TrackingField(ruleContext.Item.Fields["__tracking"]);
            return trackingField.CampaignIds.Contains(CampaignItemId);
        }
    }
}
