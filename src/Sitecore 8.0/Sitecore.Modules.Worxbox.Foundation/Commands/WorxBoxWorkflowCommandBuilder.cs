using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.CommandBuilders;
using Sitecore.Workflows;

namespace CapTech.Modules.Worxbox.Foundation.Commands
{
    public class WorxBoxWorkflowCommandBuilder : CommandBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Shell.Framework.CommandBuilders.WorkflowCommandBuilder" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="workflow">The workflow.</param>
        /// <param name="command">The command.</param>
        public WorxBoxWorkflowCommandBuilder(Item item, IWorkflow workflow, WorkflowCommand command)
          : base("item:compositeworkflow")
        {
            Assert.ArgumentNotNull((object)item, "item");
            Assert.ArgumentNotNull((object)workflow, "workflow");
            Assert.ArgumentNotNull((object)command, "command");
            this.Add("id", item.ID.ToString());
            this.Add("language", item.Language.Name);
            this.Add("version", item.Version.ToString());
            this.Add("commandid", command.CommandID);
            this.Add("workflowid", workflow.WorkflowID);
            this.Add("ui", command.HasUI);
            this.Add("suppresscomment", command.SuppressComment);
        }
    }
}
