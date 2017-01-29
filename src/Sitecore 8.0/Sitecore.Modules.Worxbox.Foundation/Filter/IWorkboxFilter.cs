using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Workflows;

namespace CapTech.Modules.Worxbox.Foundation.Filter
{
    public interface IWorkboxFilter
    {
        string FilterKey { get; }
        void Filter(IWorkflow workflow, IEnumerable<DataUri> items);
        IEnumerable<KeyValuePair<string, string>> GetFilterValues(IWorkflow workflow);
    }
}