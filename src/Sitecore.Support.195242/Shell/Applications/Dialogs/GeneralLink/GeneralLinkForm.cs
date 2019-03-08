using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Sitecore.Support.Shell.Applications.Dialogs.GeneralLink
{
  public class GeneralLinkForm : Sitecore.Shell.Applications.Dialogs.GeneralLink.GeneralLinkForm
  {
    private Type _type = typeof(Sitecore.Shell.Applications.Dialogs.GeneralLink.GeneralLinkForm);

    protected override void OnLoad(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");

      base.OnLoad(e);
      if (Context.ClientPage.IsEvent)
      {
        return;
      }

      this._type.GetProperty("CurrentMode", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, base.LinkType ?? string.Empty);
      this.InitControls();
      this._type.GetMethod("SetModeSpecificControls", BindingFlags.NonPublic | BindingFlags.Instance)
        .Invoke(this, new object[] { });
      this._type.GetMethod("RegisterScripts", BindingFlags.NonPublic | BindingFlags.Static)
        .Invoke(this, new object[] { });
    }

    protected virtual void InitControls()
    {
      string customtarget = string.Empty;
      string target = this.LinkAttributes[LinkAttributeNames.Target];
      string targetValue = GetLinkTargetValue(target);
      if (targetValue == "Custom")
      {
        customtarget = target;
        this.CustomTarget.Disabled = false;
        this.Custom.Class = string.Empty;
      }
      else
      {
        this.CustomTarget.Disabled = true;
        this.Custom.Class = "disabled";
      }

      this.Text.Value = this.LinkAttributes[LinkAttributeNames.Text];
      this.Target.Value = targetValue;
      this.CustomTarget.Value = customtarget;
      this.Class.Value = this.LinkAttributes[LinkAttributeNames.Class];
      this.Querystring.Value = this.LinkAttributes[LinkAttributeNames.QueryString];
      this.Title.Value = this.LinkAttributes[LinkAttributeNames.Title];
      this._type.GetMethod("InitMediaLinkDataContext", BindingFlags.Instance | BindingFlags.NonPublic)
        .Invoke(this, new object[] { });
      this.InitInternalLinkDataContext();
    }
    protected virtual void InitInternalLinkDataContext()
    {
      this.InternalLinkDataContext.GetFromQueryString();
      string root = WebUtil.GetQueryString("ro");
      string itemId = this.LinkAttributes[LinkAttributeNames.Id];

      if (!string.IsNullOrEmpty(itemId) && ID.IsID(itemId))
      {
        var id = new ID(itemId);
        var itemUri = new ItemUri(id, Client.ContentDatabase);
        this.InternalLinkDataContext.SetFolder(itemUri);
      }

      if (root.Length > 0)
      {
        this.InternalLinkDataContext.Root = root;
      }
    }
  }
}