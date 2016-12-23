using System;
using System.Collections.Generic;
using System.Linq;
using CapTech.Modules.Worxbox.Foundation.Models;
using Newtonsoft.Json;
using NVelocity.Runtime.Directive;
using Sitecore;
using Sitecore.Data;
using Sitecore.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;

namespace CapTech.Modules.Worxbox.Foundation.Filter
{
    public class WorxBoxItemFilterer : IItemFilterer
    {
        public IEnumerable<DataUri> FilterItems(IEnumerable<DataUri> itemList)
        {
            var fieldFilter = Registry.GetString("/Current_User/Workbox/FieldFilter");
            if (string.IsNullOrEmpty(fieldFilter))
            {
                return itemList;
            }

            var filterList = JsonConvert.DeserializeObject<List<WorxboxFieldFilter>>(fieldFilter);
            var items = itemList.Select(x => Client.ContentDatabase.GetItem(x.ItemID, x.Language, x.Version));

            var result = new List<DataUri>();

            // Initial behavior is AND condition.
            foreach (var item in items)
            {
                var include = true;
                foreach (var filter in filterList)
                {
                    var filterInclude = false;
                    switch (filter.Operator)
                    {
                        case Operator.Contains:
                            filterInclude = item[filter.Field.FieldName].Contains(filter.Value);
                            break;
                        case Operator.EndsWith:
                            filterInclude = item[filter.Field.FieldName].EndsWith(filter.Value);
                            break;
                        case Operator.Equals:
                            filterInclude = item[filter.Field.FieldName].Equals(filter.Value);
                            break;
                        case Operator.NotEqual:
                            filterInclude = !item[filter.Field.FieldName].Equals(filter.Value);
                            break;
                        case Operator.StartsWith:
                            filterInclude = item[filter.Field.FieldName].StartsWith(filter.Value);
                            break;
                        default:
                            break;
                    }
                    include = include && filterInclude;
                }
                if (include)
                {
                    result.Add(new DataUri(item.ID, item.Language, item.Version));
                }
            }
            return result;
        }

        public void SetFilter()
        {
            var args = new ClientPipelineArgs();
            var urlString = new UrlString(UIUtil.GetUri("control:FilterForm"));
            Context.ClientPage.ClientResponse.ShowModalDialog(new ModalDialogOptions(urlString.ToString())
            {
                AutoIncreaseHeight = true,
                Closable = true,
                Header = "Filter",
                Height = "480px",
                Width = "720px"
            });
            args.WaitForPostBack();
            return;
        }

        public void ClearFilter()
        {
            Registry.SetString("/Current_User/Workbox/FieldFilter", String.Empty);
        }
    }
}
