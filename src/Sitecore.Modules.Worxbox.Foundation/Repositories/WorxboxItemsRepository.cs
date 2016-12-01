using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using CapTech.Modules.Worxbox.Foundation.Models;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Workflows;

namespace CapTech.Modules.Worxbox.Foundation.Repositories
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
            var referencedItems = Globals.LinkDatabase.GetReferences(item);
            var results = new List<Item>();
            foreach(var reference in referencedItems)
            {
                var refItem = Context.ContentDatabase.GetItem(reference.TargetItemID);

                if (refItem != null && refItem["__Workflow state"].Equals(state.WorkflowState.StateID))
                {
                    if (!results.Any(
                            x => x.ID == refItem.ID && x.Language == refItem.Language && x.Version == refItem.Version))
                    {
                        results.Add(refItem);
                    }
                    
                }
            }

            return results;
        }

        public IEnumerable<Item> GetWorkflowCommands()
        {
            var items = ((MultilistField) _settings.Fields[WorkflowCommandsFieldId]).GetItems();
            return items;
        }

        public IEnumerable<ID> GetWorkflowCommandIDs()
        {
            return GetWorkflowCommands().Select(x=>x.ID);
        }

        public IEnumerable<Item> GetWorxboxTemplates()
        {
            var items = ((MultilistField) _settings.Fields[PageTemplatesFieldId]).GetItems();
            return items;
        }

        public IEnumerable<ID> GetWorxboxTemplateIDs()
        {
            return GetWorxboxTemplates().Select(x => x.ID);
        }

        public bool IsWorxboxItem(WorkflowState state, DataUri item)
        {
            var contentItem = Context.ContentDatabase.GetItem(item.ItemID, item.Language, item.Version);
            var stateHasCommands =
                Context.ContentDatabase.GetItem(state.StateID).Children.Any(x => GetWorkflowCommandIDs().Contains(x.ID));
            var itemIsWorkboxTemplate = GetWorxboxTemplateIDs().Contains(contentItem.TemplateID);
            return stateHasCommands && itemIsWorkboxTemplate;
        }
    }
}
