using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapTech.Modules.Worxbox.Foundation.Repositories;
using Newtonsoft.Json;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;

namespace CapTech.Modules.Worxbox.Feature.Client.Workbox
{
    public class WorxboxFilterForm : DialogForm
    {
        protected Literal FilterSection;

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");
            base.OnLoad(e);
            var repository = new WorxboxSettingsRepository();
            var fields = repository.GetFilterFields();
            FilterSection.Text = JsonConvert.SerializeObject(fields);
        }

        protected override void OnCancel(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull((object)args, "args");
            SheerResponse.CloseWindow();
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull((object)args, "args");
            SheerResponse.CloseWindow();
        }
    }
}
