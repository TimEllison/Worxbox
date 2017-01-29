using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Workflows;

namespace CapTech.Modules.Worxbox.Foundation.Models
{
    public class WorxboxWorkflowState
    {
        public IWorkflow Workflow { get; set; }

        public WorkflowState WorkflowState { get; set; }

        public IEnumerable<WorkflowCommand> WorkflowCommands { get; set; }
    }
}
