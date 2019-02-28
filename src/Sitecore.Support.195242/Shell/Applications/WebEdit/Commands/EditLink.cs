using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.WebEdit.Commands;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI;
using Sitecore.Web.UI.Sheer;
using Sitecore.Xml.Xsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Sitecore.Support.Shell.Applications.WebEdit.Commands
{
  public class EditLink : Sitecore.Shell.Applications.WebEdit.Commands.EditLink
  {
    public override void Execute(CommandContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      string formValue = WebUtil.GetFormValue("scPlainValue");
      context.Parameters.Add("fieldValue", formValue);
      Context.ClientPage.Start(this, "Run", context.Parameters);
    }

    private new static void Run(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Item item = Context.ContentDatabase.GetItem(args.Parameters["itemid"]);
      Assert.IsNotNull(item, typeof(Item));
      Field field = item.Fields[args.Parameters["fieldid"]];
      Assert.IsNotNull(field, typeof(Field));
      string value = args.Parameters["controlid"];

      if (args.IsPostBack)
      {
        if (args.HasResult)
        {

          string text = (typeof(Sitecore.Shell.Applications.WebEdit.Commands.EditLink)
            .GetMethod("RenderLink", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(null, new object[] { args })
            as RenderFieldResult).ToString();
          
          SheerResponse.SetAttribute("scHtmlValue", "value", string.IsNullOrEmpty(text) ? WebEditLinkCommand.GetDefaultText() : text);
          SheerResponse.SetAttribute("scPlainValue", "value", args.Result);
          ScriptInvokationBuilder scriptInvokationBuilder = new ScriptInvokationBuilder("scSetHtmlValue");
          scriptInvokationBuilder.AddString(value, Array.Empty<object>());
          if (!string.IsNullOrEmpty(text) && string.IsNullOrEmpty(StringUtil.RemoveTags(text)))
          {
            scriptInvokationBuilder.Add("true");
          }
          SheerResponse.Eval(scriptInvokationBuilder.ToString());
        }
      }
      else
      {
        UrlString urlString = new UrlString(Context.Site.XmlControlPage);
        urlString["xmlcontrol"] = "GeneralLink";
        // Define language query string to correctly resolve Display Names.
        urlString["la"] = args.Parameters["language"];
        UrlHandle urlHandle = new UrlHandle();
        urlHandle["va"] = new XmlValue(args.Parameters["fieldValue"], "link").ToString();
        urlHandle.Add(urlString);
        urlString.Append("ro", field.Source);
        Context.ClientPage.ClientResponse.ShowModalDialog(urlString.ToString(), "550", "650", string.Empty, true);
        args.WaitForPostBack();
      }
    }
  }
}