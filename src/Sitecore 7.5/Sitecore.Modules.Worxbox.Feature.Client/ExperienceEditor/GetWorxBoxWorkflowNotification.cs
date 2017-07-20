using System.Linq;
using CapTech.Modules.Worxbox.Foundation.Commands;
using CapTech.Modules.Worxbox.Foundation.Repositories;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Pipelines.GetPageEditorNotifications;
using Sitecore.Shell.Framework.CommandBuilders;
using Sitecore.Workflows;

namespace CapTech.Modules.Worxbox.Feature.Client.ExperienceEditor
{
    public class GetWorxBoxWorkflowNotification : GetPageEditorNotificationsProcessor
    {
        public override void Process(GetPageEditorNotificationsArgs arguments)
        {
            Assert.ArgumentNotNull((object)arguments, "arguments");
            Item contextItem = arguments.ContextItem;
            Database database = contextItem.Database;
            IWorkflowProvider workflowProvider = database.WorkflowProvider;
            if (workflowProvider == null)
                return;
            IWorkflow workflow = workflowProvider.GetWorkflow(contextItem);
            if (workflow == null)
                return;
            WorkflowState state = workflow.GetState(contextItem);
            if (state == null)
                return;
            var repository = new WorxboxItemsRepository(workflow);

            var worxBoxIcon = "/~/icon/worxbox/32x32/worxbox.png";
            var displayIcon =
                $"<span><img src='{worxBoxIcon}'  style='vertical-align:middle; padding-right: 1px;'/></span>";



            string description = GetDescription(workflow, state, database);
            string icon = state.Icon;
            PageEditorNotification editorNotification = new PageEditorNotification(description, PageEditorNotificationType.Information)
            {
                Icon = icon
            };
            var commands = WorkflowFilterer.FilterVisibleCommands(workflow.GetCommands(contextItem));
            foreach (WorkflowCommand filterVisibleCommand in commands)
            {
                PageEditorNotificationOption notificationOption = new PageEditorNotificationOption(filterVisibleCommand.DisplayName, new WorkflowCommandBuilder(contextItem, workflow, filterVisibleCommand).ToString());
                editorNotification.Options.Add(notificationOption);
            }
            editorNotification.Options.Add(new PageEditorNotificationOption("|", ""));
            foreach (WorkflowCommand command in commands)
            {
                if (repository.IsWorxboxItem(state, new DataUri(contextItem.Uri)) &&
                    repository.GetWorkflowCommandIDs().Contains(ID.Parse(command.CommandID)))
                {
                    var notificationOption = new PageEditorNotificationOption(displayIcon + command.DisplayName, new WorxBoxWorkflowCommandBuilder(contextItem, workflow, command).ToString());
                    editorNotification.Options.Add(notificationOption);
                }
            }
            arguments.Notifications.Add(editorNotification);
        }

        private static bool CanShowCommands(Item item, WorkflowCommand[] commands)
        {
            Assert.ArgumentNotNull((object)item, "item");
            return !item.Appearance.ReadOnly && commands != null && commands.Length > 0 && (Context.IsAdministrator || item.Access.CanWriteLanguage() && (item.Locking.CanLock() || item.Locking.HasLock()));
        }

        private static string GetDescription(IWorkflow workflow, WorkflowState state, Database database)
        {
            Assert.ArgumentNotNull((object)workflow, "workflow");
            Assert.ArgumentNotNull((object)state, "state");
            Assert.ArgumentNotNull((object)database, "database");
            Item obj = database.GetItem(state.StateID);
            Assert.IsNotNull((object)obj, "Workflow state item not found.");
            string str = obj["Description"];
            if (!string.IsNullOrEmpty(str))
                return str;
            string displayName = workflow.Appearance.DisplayName;
            return Translate.Text("The item is in the '{0}' workflow state in the '{1}' workflow.", (object)obj.DisplayName, (object)displayName);
        }
    }
}
