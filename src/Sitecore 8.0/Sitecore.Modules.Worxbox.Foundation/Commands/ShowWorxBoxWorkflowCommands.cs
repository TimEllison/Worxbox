using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.CommandBuilders;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Workflows;
using System;
using System.Linq;
using CapTech.Modules.Worxbox.Foundation.Repositories;
using Sitecore.Shell.Framework.Commands;

namespace CapTech.Modules.Worxbox.Foundation.Commands
{
    [Serializable]
    public class ShowWorxBoxWorkflowCommands : Command
    {
        /// <summary>Queries the state of the command.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The state of the command.</returns>
        public override CommandState QueryState(CommandContext context)
        {
            if (!Settings.Workflows.Enabled)
                return CommandState.Hidden;
            return base.QueryState(context);
        }

        /// <summary>Executes the command in the specified context.</summary>
        /// <param name="context">The context.</param>
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, "context");
            string name1 = context.Parameters["database"];
            string path = context.Parameters["id"];
            string name2 = context.Parameters["language"];
            string str = context.Parameters["version"];
            Database database = Factory.GetDatabase(name1);

            if (database == null)
                return;
            Item obj = database.GetItem(path, Language.Parse(name2), Sitecore.Data.Version.Parse(str));
            if (obj == null)
                return;
            IWorkflow workflow = obj.Database.WorkflowProvider.GetWorkflow(obj);
            if (workflow == null)
                return;
            WorkflowCommand[] workflowCommandArray = WorkflowFilterer.FilterVisibleCommands(workflow.GetCommands(obj), obj);

            var repository = new WorxboxItemsRepository(workflow);

            if (workflowCommandArray == null || workflowCommandArray.Length == 0)
                return;
            Menu menu = new Menu();
            SheerResponse.DisableOutput();
            foreach (WorkflowCommand command in workflowCommandArray)
            {
                string @string = new WorkflowCommandBuilder(obj, workflow, command).ToString();
                menu.Add("C" + command.CommandID, command.DisplayName, command.Icon, string.Empty, @string, false, string.Empty, MenuItemType.Normal).Disabled = 
                    !Context.User.IsAdministrator && !obj.Locking.HasLock();

                if (repository.IsWorxboxItem(obj.State.GetWorkflowState(), new DataUri(obj.Uri)) &&
                    repository.GetWorkflowCommandIDs().Contains(ID.Parse(command.CommandID)))
                {
                    @string = new WorxBoxWorkflowCommandBuilder(obj, workflow, command).ToString();
                    menu.Add("C" + command.CommandID, "WorxBox " + command.DisplayName, command.Icon, string.Empty, @string, false, string.Empty, MenuItemType.Normal).Disabled = 
                        !Context.User.IsAdministrator && !obj.Locking.HasLock();
                }
            }
            SheerResponse.EnableOutput();
            SheerResponse.ShowContextMenu(Context.ClientPage.ClientRequest.Control, "right", (Control)menu);
        }
    }
}
