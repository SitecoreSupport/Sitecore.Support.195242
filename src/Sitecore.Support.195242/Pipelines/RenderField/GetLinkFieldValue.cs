using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;
using Sitecore.Support.Xml.Xsl;
using Sitecore.Xml.Xsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Sitecore.Support.Pipelines.RenderField
{
  public class GetLinkFieldValue : Sitecore.Pipelines.RenderField.GetLinkFieldValue
  {
    public new void Process(RenderFieldArgs args)
    {
      if (this.SkipProcessor(args))
      {
        return;
      }

      SetWebEditParameters(args, "class", "text", "target", "haschildren");

      if (!string.IsNullOrEmpty(args.Parameters["text"]))
      {
        args.WebEditParameters["text"] = args.Parameters["text"];
      }

      Sitecore.Support.Xml.Xsl.LinkRenderer linkRenderer = (Sitecore.Support.Xml.Xsl.LinkRenderer)CreateRenderer(args.Item);

      linkRenderer.FieldName = args.FieldName;
      linkRenderer.FieldValue = args.FieldValue;
      linkRenderer.Parameters = args.Parameters;
      linkRenderer.RawParameters = args.RawParameters;

      args.DisableWebEditFieldWrapping = true;
      args.DisableWebEditContentEditing = true;

      var result = linkRenderer.Render();

      args.Result.FirstPart = result.FirstPart;
      args.Result.LastPart = result.LastPart;
    }

    private static void SetWebEditParameters(RenderFieldArgs args, params string[] parameterNames)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(parameterNames, "parameterNames");
      foreach (string key in parameterNames)
      {
        if (!string.IsNullOrEmpty(args.Parameters[key]))
        {
          args.WebEditParameters[key] = args.Parameters[key];
        }
      }
    }


    protected override Sitecore.Xml.Xsl.LinkRenderer CreateRenderer(Item item)
    {
      return new Sitecore.Support.Xml.Xsl.LinkRenderer(item);
    }
  }
}