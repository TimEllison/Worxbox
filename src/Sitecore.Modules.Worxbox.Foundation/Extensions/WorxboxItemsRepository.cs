using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapTech.Modules.Worxbox.Foundation.Models;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Workflows;

namespace CapTech.Modules.Worxbox.Foundation.Extensions
{
    public class WorxboxItemsRepository
    {
        internal static readonly ID WorxboxSettingsId = new ID("{2E5F1A78-7FD2-4CCA-BB06-F5EFDA1B16FB}");
        internal static readonly ID WorkflowCommandsFieldId = new ID("{ACAB8161-7015-4C01-9F86-066914EB2E16}");
        internal static readonly ID PageTemplatesFieldId = new ID("{A158C635-F52E-4CFB-9EBC-0B198E39FE6E}");

        private readonly Item _settings;

        private readonly IWorkflow _workflow;
        public WorxboxItemsRepository(IWorkflow workflow)
        {
            Assert.ArgumentNotNull(workflow, "workflow");
            _workflow = workflow;
            _settings = Client.ContentDatabase.GetItem(WorxboxSettingsId);
        }

        public IEnumerable<Item> GetWorxboxItems(WorxboxWorkflowState state, Item item)
        {
            return null;
        }

        public IEnumerable<Item> GetWorkflowCommands()
        {
            var items = ((MultilistField) _settings.Fields[WorkflowCommandsFieldId]).GetItems();
            return items;
        }

        public bool IsWorxboxItem(WorkflowState state, DataUri item)
        {
            return false;
        }
    }
}
