﻿using System.Text;
using System.Web;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Xml.Xsl;

namespace Sitecore.Support.Xml.Xsl
{
  public class LinkRenderer : Sitecore.Xml.Xsl.LinkRenderer
  {
    [NotNull]
    readonly char[] _delimiter = new[] { '=', '&' };
    public LinkRenderer(Item item) : base(item)
    {
    }

    [NotNull]
    public override RenderFieldResult Render()
    {
      var attributes = new SafeDictionary<string>();

      attributes.AddRange(Parameters);

      var isEndLink = MainUtil.GetBool(attributes["endlink"], false);

      if (isEndLink)
      {
        return RenderFieldResult.EndLink;
      }

      var ignoreList = Set<string>.Create("field", "select", "text", "haschildren", "before", "after", "enclosingtag", "fieldname", "disable-web-editing");

      var linkField = LinkField;

      if (linkField != null)
      {
        attributes["title"] = HttpUtility.HtmlAttributeEncode(StringUtil.GetString(attributes["title"], linkField.Title));
        attributes["target"] = StringUtil.GetString(attributes["target"], linkField.Target);
        attributes["class"] = StringUtil.GetString(attributes["class"], linkField.Class);
        this.SetRelAttribute(attributes, linkField);
      }

      var text = string.Empty;

      var rawParameters = RawParameters;

      if (!string.IsNullOrEmpty(rawParameters) && rawParameters.IndexOfAny(_delimiter) < 0)
      {
        text = rawParameters;
      }

      if (string.IsNullOrEmpty(text))
      {
        var targetItem = this.ResolveTargetItemWithCorrectLanguage();
        var displayName = targetItem != null ? targetItem.DisplayName : string.Empty;

        var fieldText = linkField != null ? linkField.Text : string.Empty;

        text = StringUtil.GetString(text, attributes["text"], fieldText, displayName);
      }

      var url = GetUrl(linkField);

      switch (LinkType)
      {
        case "javascript":
          attributes["href"] = "#";
          attributes["onclick"] = StringUtil.GetString(attributes["onclick"], url);
          break;

        default:
          attributes["href"] = HttpUtility.HtmlEncode(StringUtil.GetString(attributes["href"], url));
          break;
      }

      var link = new StringBuilder("<a", 47);

      foreach (var pair in attributes)
      {
        var name = pair.Key;
        var value = pair.Value;

        if (!ignoreList.Contains(name.ToLowerInvariant()))
        {
          AddAttribute(link, name, value);
        }
      }

      link.Append('>');

      var hasChildren = MainUtil.GetBool(attributes["haschildren"], false);

      if (!hasChildren)
      {
        if (string.IsNullOrEmpty(text))
        {
          return RenderFieldResult.Empty;
        }

        link.Append(text);
      }

      return new RenderFieldResult { FirstPart = link.ToString(), LastPart = "</a>" };
    }
    protected virtual Item ResolveTargetItemWithCorrectLanguage()
    {
      if (this.LinkField != null)
      {
        ID targetID = this.LinkField.TargetID;

        if (!targetID.IsNull)
        {
          return this.LinkField.InnerField.Database.Items[targetID, this.Item.Language];
        }

        string itemPath = this.LinkField.InternalPath;

        if (itemPath.Length > 0)
        {
          return this.LinkField.InnerField.Database.Items[itemPath, this.Item.Language];
        }

        return null;
      }
      return Item;
    }
  }
}