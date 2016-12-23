using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapTech.Modules.Worxbox.Foundation.Repositories;
using Sitecore;
using Sitecore.Data;
using Sitecore.Globalization;
using Sitecore.Shell.Web.UI.WebControls;
using Sitecore.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Workflows;

namespace CapTech.Modules.Worxbox.Foundation.Filter
{
    public class WorxBoxItemFilterer : IItemFilterer
    {
        public IEnumerable<DataUri> FilterItems(IEnumerable<DataUri> itemList)
        {
            return itemList;
        }

        public ClientCommand SetFilter()
        {
            var args = new ClientPipelineArgs();
            var urlString = new UrlString(UIUtil.GetUri("control:FilterForm"));
            var command = Context.ClientPage.ClientResponse.ShowModalDialog(new ModalDialogOptions(urlString.ToString())
            {
                AutoIncreaseHeight = true,
                Closable = true,
                Header = "Filter",
                Height = "480px",
                Width = "720px"
            });
            
            args.WaitForPostBack();
            return command;
        }

        public void ClearFilter()
        {
            
        }
    }
}
