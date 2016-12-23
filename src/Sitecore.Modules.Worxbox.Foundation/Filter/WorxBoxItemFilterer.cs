using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;

namespace CapTech.Modules.Worxbox.Foundation.Filter
{
    public class WorxBoxItemFilterer : IItemFilterer
    {
        public IEnumerable<DataUri> FilterItems(IEnumerable<DataUri> itemList)
        {
            return itemList;
        }

        public void SetFilter()
        {
            
        }

        public void ClearFilter()
        {
            
        }
    }
}
