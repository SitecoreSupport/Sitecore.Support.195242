using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;
using Sitecore.Xml.Xsl;

namespace Sitecore.Support.Pipelines.RenderField
{
  public class GetLinkFieldValue
  {
    [AllowNull("args")]
    public void Process(RenderFieldArgs args)
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

      LinkRenderer linkRenderer = CreateRenderer(args.Item);

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

    static void SetWebEditParameters([NotNull] RenderFieldArgs args, [NotNull] params string[] parameterNames)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(parameterNames, "parameterNames");

      foreach (var parameterName in parameterNames)
      {
        if (string.IsNullOrEmpty(args.Parameters[parameterName]))
        {
          continue;
        }

        args.WebEditParameters[parameterName] = args.Parameters[parameterName];
      }
    }

    protected virtual LinkRenderer CreateRenderer(Item item)
    {
      return new LinkRenderer(item);
    }

    protected virtual bool SkipProcessor([CanBeNull] RenderFieldArgs args)
    {
      if (args == null)
      {
        return true;
      }

      var fieldTypeKey = args.FieldTypeKey;
      return fieldTypeKey != "link" && fieldTypeKey != "general link";
    }
  }
}