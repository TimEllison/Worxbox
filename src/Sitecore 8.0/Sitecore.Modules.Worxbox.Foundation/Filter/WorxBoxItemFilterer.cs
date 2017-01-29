using System;
using System.Collections.Generic;
using System.Linq;
using CapTech.Modules.Worxbox.Foundation.Extensions;
using CapTech.Modules.Worxbox.Foundation.Models;
using CapTech.Modules.Worxbox.Foundation.Repositories;
//using Newtonsoft.Json;
//using NVelocity.Runtime.Directive;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Rules;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.Dialogs.RulesEditor;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;

namespace CapTech.Modules.Worxbox.Foundation.Filter
{
    public class WorxBoxItemFilterer : IItemFilterer
    {
        private readonly WorxboxSettingsRepository _repository;

        public WorxBoxItemFilterer()
        {
            _repository = new WorxboxSettingsRepository();
        }

        public IEnumerable<DataUri> FilterItems(IEnumerable<DataUri> itemList)
        {
            var isSet = Registry.GetBool("/Current_User/Workbox/FieldFilterEnabled");
            var filter = Registry.GetString("/Current_User/Workbox/FieldFilter");
            var items = new List<DataUri>();

            if (isSet && !string.IsNullOrEmpty(filter))
            {
                var filterItem = Client.ContentDatabase.GetItem(ID.Parse(filter));
                if (filterItem != null)
                {
                    var context = new RuleContext();
                    foreach (var item in itemList)
                    {
                        context.Item = Client.ContentDatabase.GetItem(item);
                        if (filterItem.EvaluateConditions("Filter Rule", context))
                        {
                            items.Add(item);
                        }
                    }
                    return items;
                }
                else
                {
                    return itemList;
                }
            }
            else
            {
                return itemList;
            }
        }

        public void SetFilter()
        {
            var item = _repository.GetUserFilter();
            var args = new ClientPipelineArgs();
            args.Parameters["ContextItemId"] = item.ID.ToString();
            args.Parameters["Value"] = item.Fields["Filter Rule"].Value;
            args.Parameters["RulesPath"] = "/sitecore/system/Settings/Rules/WorxBox";
            args.Parameters["UrlPath"] = "/sitecore/shell/~/xaml/CapTech.WorxBox.FilterForm.aspx";

            Context.ClientPage.Start(this, "Run", args);
        }

        protected void Run(ClientPipelineArgs args)
        {
            if (!args.IsPostBack)
            {
                var options = new RulesEditorOptions()
                {
                    ContextItemID = args.Parameters["ContextItemId"],
                    IncludeCommon = true,
                    HideActions = true,
                    RulesPath = args.Parameters["RulesPath"],
                    Value = args.Parameters["Value"]
                };

                options.AllowMultiple = true;
                var urlString = options.ToUrlString();
                urlString.Path = args.Parameters["UrlPath"];
                urlString["sc_content"] = Client.ContentDatabase.Name;
                SheerResponse.ShowModalDialog(urlString.ToString(), "1024px", "768px", string.Empty, true);
                args.WaitForPostBack();
            }
            else
            {
                if (args.HasResult)
                {
                    var rulesValue = args.Result;
                    if (!string.IsNullOrEmpty(rulesValue))
                    {
                        _repository.SaveUserRule(rulesValue);
                        Registry.SetBool("/Current_User/Workbox/FieldFilterEnabled", true);
                        Registry.SetString("/Current_User/Workbox/FieldFilter", _repository.GetUserFilter().ID.ToString());
                    }
                }
            }
        }

        public void ClearFilter()
        {
            Registry.SetBool("/Current_User/Workbox/FieldFilterEnabled", false);
        }
    }
}
