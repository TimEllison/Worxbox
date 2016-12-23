using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapTech.Modules.Worxbox.Foundation.Models;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace CapTech.Modules.Worxbox.Foundation.Repositories
{
    public class WorxboxSettingsRepository
    {
        internal static readonly ID WorxboxSettingsId = new ID("{2E5F1A78-7FD2-4CCA-BB06-F5EFDA1B16FB}");
        internal static readonly ID WorkflowCommandsFieldId = new ID("{ACAB8161-7015-4C01-9F86-066914EB2E16}");
        internal static readonly ID PageTemplatesFieldId = new ID("{A158C635-F52E-4CFB-9EBC-0B198E39FE6E}");
        internal static readonly ID FieldFilterFolderId = new ID("{442A9921-3D8C-41DB-A4F8-0848966FD6B3}");

        private readonly Item _settings;

        public WorxboxSettingsRepository()
        {
            _settings = Client.ContentDatabase.GetItem(WorxboxSettingsId);
        }

        public IEnumerable<Item> GetWorxboxTemplates()
        {
            var items = ((MultilistField)_settings.Fields[PageTemplatesFieldId]).GetItems();
            return items;
        }

        public IEnumerable<WorxboxFilterField> GetFilterFields()
        {
            var items = Client.ContentDatabase.GetItem(FieldFilterFolderId).Children;
            var result = items.ToArray().Select(item => new WorxboxFilterField
            {
                FieldName = item["Field Name"],
                FriendlyName = item["Friendly Name"]
            }).ToList();
            return result;
        }
    }
}
