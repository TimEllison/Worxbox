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
using Sitecore.Exceptions;

namespace CapTech.Modules.Worxbox.Foundation.Commands
{
    [Serializable]
    public class CompositeWorkflow : Command
    {
        public override CommandState QueryState(CommandContext context)
        {
            if (!Settings.Workflows.Enabled)
                return CommandState.Hidden;
            return base.QueryState(context);
        }

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, "context");
            string parameter1 = context.Parameters["id"];
            string parameter2 = context.Parameters["language"];
            string parameter3 = context.Parameters["version"];
            if (Sitecore.Client.ContentDatabase.Items[parameter1, Language.Parse(parameter2), Sitecore.Data.Version.Parse(parameter3)] == null)
                return;
            Context.ClientPage.Start((object)this, "Run", new NameValueCollection()
      {
        {
          "id",
          parameter1
        },
        {
          "language",
          parameter2
        },
        {
          "version",
          parameter3
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

        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            bool flag1 = args.IsPostBack;
            if (args.Parameters["checkmodified"] == "1")
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
            string comments = string.Empty;
            bool flag2 = StringUtil.GetString(new string[1]
            {
        args.Parameters["ui"]
            }) != "1";
            bool flag3 = StringUtil.GetString(new string[1]
            {
        args.Parameters["suppresscomment"]
            }) == "1";
            if (!flag1 && flag2 && !flag3)
            {
                SheerResponse.Input("Enter a comment:", "");
                args.WaitForPostBack();
            }
            else
            {
                if (flag1)
                {
                    if (args.Result == "null")
                        return;
                    comments = args.Result;
                }
                IWorkflowProvider workflowProvider = Sitecore.Client.ContentDatabase.WorkflowProvider;
                if (workflowProvider != null)
                {
                    IWorkflow workflow = workflowProvider.GetWorkflow(args.Parameters["workflowid"]);
                    if (workflow != null)
                    {
                        Item obj = Sitecore.Client.ContentDatabase.Items[args.Parameters["id"],
                            Language.Parse(args.Parameters["language"]),
                            Sitecore.Data.Version.Parse(args.Parameters["version"])];
                        if (obj != null)
                        {
                            var workflowState = workflow.GetState(obj);
                            var repository = new WorxboxItemsRepository(workflow);
                            try
                            {
                                if (repository.IsWorxboxItem(workflowState, new DataUri(obj.Uri)) && repository.GetWorkflowCommandIDs().Any(x => x == ID.Parse(args.Parameters["commandid"])))
                                {
                                    var items =
                                        repository.GetWorxboxItems(
                                            repository.GetWorxboxWorkflowStates(workflow)
                                                .First(x => x.WorkflowCommands.Any(y => y.CommandID == args.Parameters["commandid"])), obj);
                                    foreach (var compositeItem in items)
                                    {
                                        var compositeWorkflowResult = workflow.Execute(args.Parameters["commandid"], compositeItem, comments, true);
                                        if (!compositeWorkflowResult.Succeeded)
                                        {
                                            if (!string.IsNullOrEmpty(compositeWorkflowResult.Message))
                                                SheerResponse.Alert(compositeWorkflowResult.Message);
                                        }
                                    }
                                }

                                WorkflowResult workflowResult = workflow.Execute(args.Parameters["commandid"], obj, comments, true);
                                if (!workflowResult.Succeeded)
                                {
                                    if (!string.IsNullOrEmpty(workflowResult.Message))
                                        SheerResponse.Alert(workflowResult.Message);
                                }
                            }
                            catch (WorkflowStateMissingException ex)
                            {
                                SheerResponse.Alert("One or more items could not be processed because their workflow state does not specify the next step.");
                            }
                        }
                    }
                }
                Context.ClientPage.SendMessage((object)this, "item:refresh");
                SheerResponse.Eval("window.top.location.reload();");
            }
        }
    }
}
