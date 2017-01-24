using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Pipelines;
using Sitecore.Web.UI.Sheer;
using Sitecore.Workflows;
using Sitecore.Workflows.Simple;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CapTech.Modules.Worxbox.Foundation.Repositories;
using Sitecore;
using Sitecore.Shell.Framework.Commands;

namespace CapTech.Modules.Worxbox.Foundation.Commands
{
    /// <summary>Represents the Workflow command.</summary>
    [Serializable]
    public class CompositeWorkflow : Command
    {
        /// <summary>Key used to identify the ID</summary>
        protected const string IDKey = "id";
        /// <summary>Key used to identify the language</summary>
        protected const string LanguageKey = "language";
        /// <summary>Key used to identify the version</summary>
        protected const string VersionKey = "version";
        /// <summary>Key used to identify the command ID</summary>
        protected const string CommandIdKey = "commandid";
        /// <summary>Key used to identify the workflow ID</summary>
        protected const string WorkflowIdKey = "workflowid";
        /// <summary>Key used to identify the UI setting</summary>
        protected const string UIKey = "ui";
        /// <summary>Key used to identify the 'check modified' setting</summary>
        protected const string CheckModifiedKey = "checkmodified";
        /// <summary>Key used to identify the 'suppress comment' setting</summary>
        protected const string SuppressCommentKey = "suppresscomment";

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
            string index = context.Parameters["id"];
            string name = context.Parameters["language"];
            string str = context.Parameters["version"];
            Item obj = Sitecore.Client.ContentDatabase.Items[index, Language.Parse(name), Sitecore.Data.Version.Parse(str)];
            if (obj == null || !this.CheckCommandValidity(obj, context.Parameters["commandid"]))
                return;
            Context.ClientPage.Start((object)this, "Run", new NameValueCollection()
      {
        {
          "id",
          index
        },
        {
          "language",
          name
        },
        {
          "version",
          str
        },
        {
          "commandid",
          context.Parameters["commandid"]
        },
        {
          "workflowid",
          context.Parameters["workflowid"]
        },
        {
          "ui",
          context.Parameters["ui"]
        },
        {
          "checkmodified",
          "1"
        },
        {
          "suppresscomment",
          context.Parameters["suppresscomment"]
        }
      });
        }

        /// <summary>Runs the specified args.</summary>
        /// <param name="args">The arguments.</param>
        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            bool flag1 = args.IsPostBack;
            bool flag2 = args.Parameters["checkmodified"] == "1";
            if (!this.CheckCommandValidity(Sitecore.Client.ContentDatabase.Items[args.Parameters["id"], Language.Parse(args.Parameters["language"]), Sitecore.Data.Version.Parse(args.Parameters["version"])], args.Parameters["commandid"]))
                return;
            if (flag2)
            {
                if (!flag1)
                {
                    if (Context.ClientPage.Modified)
                    {
                        SheerResponse.CheckModified(true);
                        args.WaitForPostBack();
                        return;
                    }
                }
                else if (args.Result == "cancel")
                    return;
                args.Parameters["checkmodified"] = (string)null;
                flag1 = false;
            }
            if (flag1 && args.Result == "cancel")
                return;
            Sitecore.Collections.StringDictionary commentFields = new Sitecore.Collections.StringDictionary();
            bool flag3 = StringUtil.GetString(new string[1]
            {
        args.Parameters["ui"]
            }) != "1";
            bool flag4 = StringUtil.GetString(new string[1]
            {
        args.Parameters["suppresscomment"]
            }) == "1";
            string commandId = args.Parameters["commandid"];
            string workflowId = args.Parameters["workflowid"];
            ItemUri itemUri = new ItemUri(args.Parameters["id"], Language.Parse(args.Parameters["language"]), Sitecore.Data.Version.Parse(args.Parameters["version"]), Sitecore.Client.ContentDatabase);
            if (!flag1 && flag3 && !flag4)
            {
                ID result = ID.Null;
                ID.TryParse(commandId, out result);
                WorkflowUIHelper.DisplayCommentDialog(itemUri, result);
                args.WaitForPostBack();
            }
            else
            {
                if (flag1)
                {
                    if (args.Result == "null" || args.Result == "undefined")
                        return;
                    commentFields = WorkflowUIHelper.ExtractFieldsFromFieldEditor(args.Result);
                }

                var workflow = Context.ContentDatabase.WorkflowProvider.GetWorkflow(workflowId);
                var repository = new WorxboxItemsRepository(workflow);
                var item = Context.ContentDatabase.GetItem(itemUri.ItemID, itemUri.Language, itemUri.Version);
                var workflowState = workflow.GetState(item);
                var completionCallback = new Processor("Workflow completed callback", (object)this, "WorkflowCompleteCallback");

                if (repository.IsWorxboxItem(workflowState, new DataUri(itemUri)) && repository.GetWorkflowCommandIDs().Any(x => x == ID.Parse(commandId)))
                {
                    var items =
                        repository.GetWorxboxItems(
                            repository.GetWorxboxWorkflowStates(workflow)
                                .First(x => x.WorkflowCommands.Any(y => y.CommandID == commandId)), item);
                    foreach (var compositeItem in items)
                    {
                        WorkflowUIHelper.ExecuteCommand(compositeItem.Uri, workflowId, commandId, commentFields, completionCallback);
                    }
                }
                WorkflowUIHelper.ExecuteCommand(itemUri, workflowId, commandId, commentFields, completionCallback);
            }
        }

        /// <summary>
        /// Processor delegate to be executed when workflow completes successfully.
        /// </summary>
        /// <param name="args">The arguments for the workflow invocation.</param>
        [UsedImplicitly]
        protected void WorkflowCompleteCallback(WorkflowPipelineArgs args)
        {
            Context.ClientPage.SendMessage((object)this, "item:refresh");
        }

        /// <summary>
        /// Checks if this command can be executed against current workflow state. This is mainly about concurrent workflow transitions.
        /// </summary>
        /// <param name="item">the item</param>
        /// <param name="commandId">workflow command</param>
        /// <returns></returns>
        private bool CheckCommandValidity(Item item, string commandId)
        {
            Assert.ArgumentNotNullOrEmpty(commandId, "commandId");
            Assert.ArgumentNotNull((object)item, "item");
            IWorkflow workflow = item.State.GetWorkflow();
            WorkflowState workflowState = item.State.GetWorkflowState();
            Assert.IsNotNull((object)workflow, "workflow");
            Assert.IsNotNull((object)workflowState, "state");
            if (((IEnumerable<WorkflowCommand>)workflow.GetCommands(workflowState.StateID)).Any<WorkflowCommand>((Func<WorkflowCommand, bool>)(a => a.CommandID == commandId)))
                return true;
            SheerResponse.Alert("The item has been moved to a different workflow state. Sitecore will therefore reload the item.");
            Context.ClientPage.SendMessage((object)this, "item:refresh");
            return false;
        }
    }
}
