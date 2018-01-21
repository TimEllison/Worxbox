using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using CapTech.Modules.Worxbox.Foundation.Models;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Workflows;
using System;

namespace CapTech.Modules.Worxbox.Foundation.Repositories
{
    internal class ItemList
    {
        private Dictionary<ID, Item> itemList;
        public ItemList()
        {
            itemList = new Dictionary<ID, Item>();
        }
        public void Add(Item item)
        {
            if ( !itemList.Any( x => x.Key == item.ID))
            {
                itemList.Add(item.ID, item);
            }
        }

        public void AddRange(IEnumerable<Item> items)
        {
            foreach(var item in items)
            {
                if ( !itemList.Any (x => x.Key == item.ID ))
                {
                    itemList.Add(item.ID, item);
                }
            }
        }

        public IEnumerable<Item> ToList()
        {
            return itemList.Select(x => x.Value);
        }

        public bool Contains(Item item)
        {
            return ToList().Any(x => x.ID == item.ID && x.Language == item.Language && x.Version == item.Version);
        }
    }


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
        
        private IEnumerable<Item> GetWorxboxItems(WorxboxWorkflowState state, Item item, List<ID> visitedItems)
        {
            var referencedItems = (from x in Globals.LinkDatabase.GetReferences(item)
                                   select x.TargetItemID)
                                   .Distinct()
                                   .ToList();

            var results = new ItemList();

            foreach (var itemId in referencedItems)
            {
                var refItem = Context.ContentDatabase.GetItem(itemId);
                if (refItem != null && IsCompositable(state, refItem) && visitedItems.All(x=>x != refItem.ID))
                {
                    if (!results.Contains(refItem) )
                    {
                        results.Add(refItem);
                        visitedItems.Add(refItem.ID);
                        results.AddRange(GetParentItems(state, refItem, visitedItems));
                        results.AddRange(GetChildItems(state, refItem, visitedItems));
                        results.AddRange(GetWorxboxItems(state, refItem, visitedItems));
                    }
                }
            }

            foreach (var childItem in item.Axes.GetDescendants())
            {
                if (childItem != null && IsCompositable(state, childItem) && visitedItems.All(x => x != childItem.ID))
                {
                    if (!results.Contains(childItem))
                    {
                        results.Add(childItem);
                        visitedItems.Add(childItem.ID);
                        results.AddRange(GetWorxboxItems(state, childItem, visitedItems));
                    }
                }
            }
            return results.ToList();
        }

        private IEnumerable<Item> GetChildItems(WorxboxWorkflowState state, Item item, List<ID> visitedItems)
        {
            var items = item.Axes.GetDescendants();
            return items.Where(child => IsCompositable(state, child) && visitedItems.All(x => x != child.ID));
        }

        private IEnumerable<Item> GetParentItems(WorxboxWorkflowState state, Item item, List<ID> visitedItems)
        {
            var results = new List<Item>();
            var parent = item.Parent;
            while (parent != null && parent.ID != ItemIDs.RootID)
            {
                if (IsCompositable(state, parent) &&
                    visitedItems.All(x => x != parent.ID))
                {
                    results.Add(parent);
                    visitedItems.Add(parent.ID);
                }
                parent = parent.Parent;
            }
            return results;
        }

        private bool IsCompositable(WorxboxWorkflowState state, Item item)
        {
            return !this.IsWorxboxItem(state.WorkflowState, new DataUri(item.Uri)) &&
                   item["__Workflow state"].Equals(state.WorkflowState.StateID);
        }

        public IEnumerable<Item> GetWorxboxItems(WorxboxWorkflowState state, Item item)
        {
            return GetWorxboxItems(state, item, new List<ID>());
        }

        public IEnumerable<Item> GetWorkflowCommands()
        {
            var items = ((MultilistField) _settings.Fields[WorkflowCommandsFieldId]).GetItems();
            return items;
        }

        public IEnumerable<WorxboxWorkflowState> GetWorxboxWorkflowStates(IWorkflow workflow)
        {
            var worxBoxCommands = GetWorkflowCommands();
            foreach (var state in workflow.GetStates())
            {
                var commands = from cmd in worxBoxCommands
                    join wfCmd in workflow.GetCommands(state.StateID).ToList()
                        on cmd.ID equals ID.Parse(wfCmd.CommandID)
                    select wfCmd;

                if (commands.Any())
                {
                    yield return new WorxboxWorkflowState()
                    {
                        Workflow = workflow,
                        WorkflowState = state,
                        WorkflowCommands = commands
                    };
                }
            }
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
