using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Shell.Applications.Layouts.DeviceEditor;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Specialized;

namespace Sitecore.Support.Shell.Applications.WebEdit.Commands
{
  [Serializable]
  public class EditRenderingProperties : Command
  {
    public override void Execute(CommandContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      string arg_50_0 = WebUtil.GetFormValue("scLayout");
      string text = ShortID.Decode(WebUtil.GetFormValue("scDeviceID"));
      string uniqueId = ShortID.Decode(context.Parameters["uniqueid"]);
      string text2 = "PageDesigner" + Guid.NewGuid();
      string text3 = WebEditUtil.ConvertJSONLayoutToXML(arg_50_0);
      Assert.IsNotNull(text3, "xml");
      WebUtil.SetSessionValue(text2, text3);
      int index = LayoutDefinition.Parse(text3).GetDevice(text).GetIndex(uniqueId);
      NameValueCollection expr_81 = new NameValueCollection();
      expr_81["device"] = text;
      expr_81["handle"] = text2;
      expr_81["selectedindex"] = index.ToString();
      ClientPipelineArgs args = new ClientPipelineArgs(expr_81);
      Context.ClientPage.Start(this, "Run", args);
    }

    protected void Run(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      int @int = MainUtil.GetInt(args.Parameters["selectedindex"], -1);
      if (@int < 0)
      {
        return;
      }
      Item clientContentItem = WebEditUtil.GetClientContentItem(Client.ContentDatabase);
      if (new RenderingParameters
      {
        Args = args,
        DeviceId = args.Parameters["device"],
        SelectedIndex = @int,
        HandleName = args.Parameters["handle"],
        Item = clientContentItem
      }.Show())
      {
        if (args.HasResult)
        {
          string text = WebUtil.GetSessionString(args.Parameters["handle"]);
          text = EditRenderingProperties.GetLayout(text);
          SheerResponse.SetAttribute("scLayoutDefinition", "value", text);
          SheerResponse.Eval("window.parent.Sitecore.PageModes.ChromeManager.handleMessage('chrome:rendering:propertiescompleted');");
        }
        else
        {
          SheerResponse.SetAttribute("scLayoutDefinition", "value", string.Empty);
        }
        WebUtil.RemoveSessionValue(args.Parameters["handle"]);
      }
    }

    private static string GetLayout(string layout)
    {
      Assert.ArgumentNotNull(layout, "layout");
      return WebEditUtil.ConvertXMLLayoutToJSON(layout);
    }
  }
}
