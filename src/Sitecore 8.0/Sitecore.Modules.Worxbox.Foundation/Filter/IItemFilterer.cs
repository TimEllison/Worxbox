using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.Web.UI.HtmlControls;

namespace CapTech.Modules.Worxbox.Foundation.Filter
{
    public interface IItemFilterer
    {
        IEnumerable<DataUri> FilterItems(IEnumerable<DataUri> itemList);
        void SetFilter();
        void ClearFilter();
    }
}
