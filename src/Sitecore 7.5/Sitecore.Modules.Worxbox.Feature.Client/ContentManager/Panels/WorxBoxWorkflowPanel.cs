using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Security.AccessControl;
using Sitecore.Security.Accounts;
using Sitecore.Shell.Framework.CommandBuilders;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Shell.Web.UI.WebControls;
using Sitecore.Web.UI.WebControls.Ribbons;
using Sitecore.Workflows;
using System.Web.UI;
using CapTech.Modules.Worxbox.Foundation.Commands;
using CapTech.Modules.Worxbox.Foundation.Repositories;
using Sitecore.Data;

namespace CapTech.Modules.Worxbox.Feature.Client.ContentManager.Panels
{
    public class WorxboxWorkflowPanel : RibbonPanel
    {
        /// <summary>The _check in item.</summary>
        private Item checkInItem;

        /// <summary>Renders the panel.</summary>
        /// <param name="output">The output.</param>
        /// <param name="ribbon">The ribbon.</param>
        /// <param name="button">The button.</param>
        /// <param name="context">The context.</param>
        public override void Render(HtmlTextWriter output, Ribbon ribbon, Item button, CommandContext context)
        {
            Assert.ArgumentNotNull((object)output, "output");
            Assert.ArgumentNotNull((object)ribbon, "ribbon");
            Assert.ArgumentNotNull((object)button, "button");
            Assert.ArgumentNotNull((object)context, "context");
            if (context.Items.Length < 1)
                return;
            Item obj = context.Items[0];
            if (!this.HasField(obj, FieldIDs.Workflow) || !Settings.Workflows.Enabled)
                return;
            IWorkflow workflow;
            WorkflowCommand[] commands;
            GetCommands(context.Items, out workflow, out commands);
            bool flag1 = this.IsCommandEnabled("item:checkout", obj);
            bool flag2 = CanShowCommands(obj, commands);
            bool flag3 = this.IsCommandEnabled("item:checkin", obj);
            this.RenderText(output, GetText(context.Items));
            if (workflow == null && !flag1 && (!flag2 && !flag3))
                return;

           

            Context.ClientPage.ClientResponse.DisableOutput();
            ribbon.BeginSmallButtons(output);
            if (flag1)
                this.RenderSmallButton(output, ribbon, string.Empty, Translate.Text("Edit"), "Office/24x24/edit_in_workflow.png", Translate.Text("Start editing this item."), "item:checkout", this.Enabled, false);
            if (flag3)
            {
                Item checkInItem = this.GetCheckInItem();
                if (checkInItem != null)
                    this.RenderSmallButton(output, ribbon, string.Empty, checkInItem["Phrase"], checkInItem.Appearance.Icon, Translate.Text("Check this item in."), "item:checkin(id=" + (object)obj.ID + ",language=" + obj.Language.Name + ",version=" + (object)obj.Version + ")", (this.Enabled ? 1 : 0) != 0, 0 != 0);
            }
            if (workflow != null)
                this.RenderSmallButton(output, ribbon, Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("B"), Translate.Text("History"), "Office/16x16/history.png", Translate.Text("Show the workflow history."), "item:workflowhistory", this.Enabled, false);
            if (flag2)
            {
                var repository = new WorxboxItemsRepository(workflow);

                foreach (WorkflowCommand command in commands)
                {
                    this.RenderSmallButton(output, ribbon, string.Empty, command.DisplayName, command.Icon, command.DisplayName, 
                        new WorkflowCommandBuilder(obj, workflow, command).ToString(), 
                        this.Enabled, 
                        false);

                    if (repository.IsWorxboxItem(obj.State.GetWorkflowState(), new DataUri(obj.Uri)) &&
                        repository.GetWorkflowCommandIDs().Contains(ID.Parse(command.CommandID)))
                    {
                        this.RenderSmallButton(output, ribbon, string.Empty, "WorxBox " + command.DisplayName, command.Icon, command.DisplayName,
                            new WorxBoxWorkflowCommandBuilder(obj, workflow, command).ToString(),
                            this.Enabled,
                            false);
                    }
                }
            }
            ribbon.EndSmallButtons(output);
            Context.ClientPage.ClientResponse.EnableOutput();
        }

        /// <summary>Determines whether this instance can show commands.</summary>
        /// <param name="item">The item to check.</param>
        /// <param name="commands">The commands.</param>
        /// <returns>
        /// <c>true</c> if this instance [can show commands] the specified item; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanShowCommands(Item item, WorkflowCommand[] commands)
        {
            Assert.ArgumentNotNull((object)item, "item");
            return !item.Appearance.ReadOnly && commands != null && commands.Length > 0 && (Context.IsAdministrator || item.Access.CanWriteLanguage() && (item.Locking.CanLock() || item.Locking.HasLock()));
        }

        /// <summary>Gets the commands.</summary>
        /// <param name="items">The items to get commands for.</param>
        /// <param name="workflow">The workflow.</param>
        /// <param name="commands">The commands.</param>
        private static void GetCommands(Item[] items, out IWorkflow workflow, out WorkflowCommand[] commands)
        {
            Assert.ArgumentNotNull((object)items, "items");
            Item obj = items[0];
            if (obj != null)
            {
                IWorkflowProvider workflowProvider = Context.ContentDatabase.WorkflowProvider;
                if (workflowProvider != null && workflowProvider.GetWorkflows().Length > 0)
                {
                    workflow = workflowProvider.GetWorkflow(obj);
                    if (workflow != null && workflow.GetState(obj) != null)
                    {
                        commands = WorkflowFilterer.FilterVisibleCommands(workflow.GetCommands(obj));
                        return;
                    }
                }
            }
            workflow = (IWorkflow)null;
            commands = (WorkflowCommand[])null;
        }

        /// <summary>Gets the text.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The get text.</returns>
        private static string GetText(Item[] items)
        {
            Assert.ArgumentNotNull((object)items, "items");
            if (items.Length <= 0 || items.Length != 1)
                return string.Empty;
            Item obj = items[0];
            if (obj.Appearance.ReadOnly)
                return string.Empty;
            if (AuthorizationManager.IsAllowed((ISecurable)obj, AccessRight.ItemWrite, (Account)Context.User))
            {
                if (obj.Locking.HasLock())
                    return Translate.Text("<b>You</b> have locked this item.");
                if (obj.Locking.IsLocked())
                    return Translate.Text("<b>\"{0}\"</b> has locked this item.", (object)StringUtil.GetString(new string[2]
                    {
            obj.Locking.GetOwnerWithoutDomain(),
            "?"
                    }));
                if (obj.Locking.CanLock())
                    return Translate.Text("Click Edit to lock and edit this item.");
                IWorkflow workflow = obj.State.GetWorkflow();
                WorkflowState workflowState = obj.State.GetWorkflowState();
                if (workflow == null || workflowState == null)
                    return Translate.Text("You do not have permission to<br/>edit the content of this item.");
                if (workflowState.FinalState)
                    return Translate.Text("This item has been approved.");
                return Translate.Text("The item is in the <b>{0}</b> state<br/>in the <b>{1}</b> workflow.", (object)StringUtil.GetString(new string[2]
                {
          workflowState.DisplayName,
          "?"
                }), (object)StringUtil.GetString(new string[2]
                {
          workflow.Appearance.DisplayName,
          "?"
                }));
            }
            if (obj.Access.CanWrite())
                return Translate.Text("Click Edit to lock and edit this item.");
            IWorkflow workflow1 = obj.State.GetWorkflow();
            WorkflowState workflowState1 = obj.State.GetWorkflowState();
            if (workflow1 == null || workflowState1 == null)
                return Translate.Text("You do not have permission to<br/>edit the content of this item.");
            if (workflowState1.FinalState)
                return Translate.Text("This item has been approved.");
            return Translate.Text("The item is in the <b>{0}</b> state<br/>in the <b>{1}</b> workflow.", (object)StringUtil.GetString(new string[2]
            {
        workflowState1.DisplayName,
        "?"
            }), (object)StringUtil.GetString(new string[2]
            {
        workflow1.Appearance.DisplayName,
        "?"
            }));
        }

        /// <summary>Gets the check in item.</summary>
        /// <returns>Check in workflow item</returns>
        private Item GetCheckInItem()
        {
            if (this.checkInItem == null)
                this.checkInItem = Context.Database.Items["/sitecore/system/Settings/Workflow/Check In"];
            return this.checkInItem;
        }

        /// <summary>
        /// Determines whether command enabled for the specified item.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// 	<c>true</c> if [is command enabled] [the specified command]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCommandEnabled(string command, Item item)
        {
            Assert.ArgumentNotNullOrEmpty(command, "command");
            Assert.ArgumentNotNull((object)item, "item");
            CommandState commandState = CommandManager.QueryState(command, item);
            if (commandState != CommandState.Down)
                return commandState == CommandState.Enabled;
            return true;
        }
    }
}
