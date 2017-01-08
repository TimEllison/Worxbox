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
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;

namespace CapTech.Modules.Worxbox.Foundation.Repositories
{
    public class WorxboxSettingsRepository
    {
        internal static readonly ID WorxboxSettingsId = new ID("{2E5F1A78-7FD2-4CCA-BB06-F5EFDA1B16FB}");
        internal static readonly ID WorkflowCommandsFieldId = new ID("{ACAB8161-7015-4C01-9F86-066914EB2E16}");
        internal static readonly ID PageTemplatesFieldId = new ID("{A158C635-F52E-4CFB-9EBC-0B198E39FE6E}");
        internal static readonly ID FieldFilterFolderId = new ID("{442A9921-3D8C-41DB-A4F8-0848966FD6B3}");
        internal static readonly ID UserFilterFolderId = new ID("{6B1CE598-8238-4900-807B-798227443A4D}");
        internal static readonly ID UserFilterTemplateId = new ID("{9E9BD017-4AD3-464F-A695-9B284A32E457}");

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

        public Item GetUserFilter()
        {
            var userFilterFolder = Client.ContentDatabase.GetItem(UserFilterFolderId);
            var userFilter = userFilterFolder.GetChildren().ToArray()
                .FirstOrDefault(x => x["Owner"].Equals(User.Current.Name));
            if (userFilter == null)
            {
                using (new SecurityDisabler())
                {
                    var path = userFilterFolder.Paths.FullPath + $"/{User.Current.Name.Replace("\\", "-").Replace(" ","-")}";

                    userFilter = Client.ContentDatabase.CreateItemPath(path,
                        new TemplateItem(Client.ContentDatabase.GetItem(UserFilterTemplateId)));
                    userFilter.Editing.BeginEdit();
                    userFilter["Owner"] = User.Current.Name;
                    userFilter.Editing.EndEdit(true, false);
                }
            }
            return userFilter;
        }

        public void SaveUserRule(string ruleValue)
        {
            var userFilter = GetUserFilter();
            using (new SecurityDisabler())
            {
                userFilter.Editing.BeginEdit();
                userFilter.Fields["Filter Rule"].Value = ruleValue;
                userFilter.Editing.EndEdit(true, false);
            }
        }
    }
}
