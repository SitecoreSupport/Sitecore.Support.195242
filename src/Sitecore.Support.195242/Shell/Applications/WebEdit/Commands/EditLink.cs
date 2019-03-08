using System;
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
using System.Reflection;

namespace Sitecore.Support.Shell.Applications.WebEdit.Commands
{
  [Serializable]
  public class EditLink : Sitecore.Shell.Applications.WebEdit.Commands.EditLink
  {
    public override void Execute([NotNull] CommandContext context)
    {
      Assert.ArgumentNotNull(context, "context");

      string fieldValue = WebUtil.GetFormValue("scPlainValue");
      context.Parameters.Add("fieldValue", fieldValue);

      Context.ClientPage.Start(this, "Run", context.Parameters);
    }

    private new static void Run([NotNull] ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Item item = Context.ContentDatabase.GetItem(args.Parameters["itemid"]);
      Assert.IsNotNull(item, typeof(Item));

      Field field = item.Fields[args.Parameters["fieldid"]];
      Assert.IsNotNull(field, typeof(Field));

      string controlID = args.Parameters["controlid"];

      if (args.IsPostBack)
      {
        if (args.HasResult)
        {
          string htmlValue = (typeof(Sitecore.Shell.Applications.WebEdit.Commands.EditLink)
              .GetMethod("RenderLink", BindingFlags.NonPublic | BindingFlags.Static)
              .Invoke(null, new object[] {args})
            as RenderFieldResult).ToString();

          SheerResponse.SetAttribute(
            "scHtmlValue", "value", string.IsNullOrEmpty(htmlValue) ? WebEditLinkCommand.GetDefaultText() : htmlValue);

          SheerResponse.SetAttribute("scPlainValue", "value", args.Result);
          var builder = new ScriptInvokationBuilder("scSetHtmlValue");
          builder.AddString(controlID);
          if (!string.IsNullOrEmpty(htmlValue) && string.IsNullOrEmpty(StringUtil.RemoveTags(htmlValue)))
          {
            builder.Add("true");
          }

          SheerResponse.Eval(builder.ToString());
        }
      }
      else
      {
        var url = new UrlString(Context.Site.XmlControlPage);
        url["xmlcontrol"] = "GeneralLink";
        // Define language query string to correctly resolve Display Names.
        url["la"] = args.Parameters["language"];
        var urlHandle = new UrlHandle();
        urlHandle["va"] = new XmlValue(args.Parameters["fieldValue"], "link").ToString();
        urlHandle.Add(url);

        url.Append("ro", field.Source);
        Context.ClientPage.ClientResponse.ShowModalDialog(url.ToString(), "550", "650", string.Empty, true);

        args.WaitForPostBack();
      }
    }
  }
}