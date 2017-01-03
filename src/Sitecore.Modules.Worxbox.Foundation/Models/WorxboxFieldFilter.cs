using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapTech.Modules.Worxbox.Foundation.Models
{
    public class WorxboxFieldFilter
    {
        public WorxboxFilterField Field { get; set; }
        public Operator Operator { get; set; }
        public string Value { get; set; }
    }
}
