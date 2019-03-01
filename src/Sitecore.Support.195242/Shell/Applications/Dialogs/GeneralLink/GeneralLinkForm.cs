using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.Dialogs;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using Sitecore.Xml;
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

      var flags = BindingFlags.NonPublic | BindingFlags.Instance;

      if (!Context.ClientPage.IsEvent)
      {
        this._type.GetProperty("CurrentMode", flags).SetValue(this, base.LinkType ?? string.Empty);
        InitControls();

        this._type.GetMethod("SetModeSpecificControls", flags).Invoke(this, new object[] { });
        this._type.GetMethod("RegisterScripts", BindingFlags.NonPublic | BindingFlags.Static).Invoke(this, new object[] { });
      }
    }
    
    protected virtual void InitControls()
    {
      string value = string.Empty;
      string text = base.LinkAttributes["target"];
      string linkTargetValue = LinkForm.GetLinkTargetValue(text);
      if (linkTargetValue == "Custom")
      {
        value = text;
        CustomTarget.Disabled = false;
        Custom.Class = string.Empty;
      }
      else
      {
        CustomTarget.Disabled = true;
        Custom.Class = "disabled";
      }
      Text.Value = base.LinkAttributes["text"];
      Target.Value = linkTargetValue;
      CustomTarget.Value = value;
      Class.Value = base.LinkAttributes["class"];
      Querystring.Value = base.LinkAttributes["querystring"];
      Title.Value = base.LinkAttributes["title"];

      this._type.GetMethod("InitMediaLinkDataContext", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, new object[] { });
      InitInternalLinkDataContext();
    }

    protected virtual void InitInternalLinkDataContext()
    {
      InternalLinkDataContext.GetFromQueryString();
      string queryString = WebUtil.GetQueryString("ro");
      string text = base.LinkAttributes["id"];
      if (!string.IsNullOrEmpty(text) && ID.IsID(text))
      {
        ItemUri folder = new ItemUri(new ID(text), Language.Parse(WebUtil.GetQueryString("la")), Client.ContentDatabase);
        InternalLinkDataContext.SetFolder(folder);
      }
      if (queryString.Length > 0)
      {
        InternalLinkDataContext.Root = queryString;
      }
    }
  }
}