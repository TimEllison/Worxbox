using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapTech.Modules.Worxbox.Foundation.Models;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace CapTech.Modules.Worxbox.Foundation.Repositories
{
    public class WorxboxFieldFilterRepository
    {
        private readonly WorxboxSettingsRepository _settingsRepository;

        public WorxboxFieldFilterRepository()
        {
            _settingsRepository = new WorxboxSettingsRepository();
        }
    }
}
