namespace Sitecore.Support.Shell.Applications.ContentManager.Galleries.Links
{
    using System.Collections.Generic;
    using System.Text;
    using Sitecore.Collections;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using Sitecore.Links;
    using Sitecore.Resources;
    using Sitecore.Shell;

    public class GalleryLinksForm : Sitecore.Shell.Applications.ContentManager.Galleries.Links.GalleryLinksForm
    {
        protected override void ProcessReferences(Item item, StringBuilder result)
        {
            ItemLink[] references = this.GetReferences(item);
            List<Pair<Item, ItemLink>> list = new List<Pair<Item, ItemLink>>();
            foreach (ItemLink link in references)
            {
                Database database = Factory.GetDatabase(link.TargetDatabaseName, false);
                if (database != null)
                {
                    Item item2 = database.GetItem(link.TargetItemID);
                    if (((item2 == null) || !this.IsHidden(item2)) || UserOptions.View.ShowHiddenItems)
                    {
                        list.Add(new Pair<Item, ItemLink>(item2, link));
                    }
                }
            }
            this.RenderReferences(result, list);
        }

        protected override void ProcessReferrers(Item item, StringBuilder result)
        {
            ItemLink[] refererers = this.GetRefererers(item);
            List<Pair<Item, ItemLink>> referrers = new List<Pair<Item, ItemLink>>();
            foreach (ItemLink link in refererers)
            {
                Database database = Factory.GetDatabase(link.SourceDatabaseName, false);
                if (database != null)
                {
                    Item item2 = database.GetItem(link.SourceItemID);
                    if (((item2 == null) || !this.IsHidden(item2)) || UserOptions.View.ShowHiddenItems)
                    {
                        referrers.Add(new Pair<Item, ItemLink>(item2, link));
                    }
                }
            }
            this.RenderReferrers(result, referrers);
        }

        private void RenderReferences(StringBuilder result, List<Pair<Item, ItemLink>> references)
        {
            result.Append("<div class=\"scMenuHeader\">" + Translate.Text("Items that the selected item refer to:") + "</div>");
            if (references.Count == 0)
            {
                result.Append("<div class=\"scNone\">" + Translate.Text("None") + "</div>");
            }
            else
            {
                foreach (Pair<Item, ItemLink> pair in references)
                {
                    Item reference = pair.Part1;
                    ItemLink link = pair.Part2;
                    if (reference == null)
                    {
                        result.Append(string.Format("<div class=\"scLink\">{0} {1}: {2}, {3}</div>", new object[] { Images.GetImage("Applications/16x16/error.png", 16, 16, "absmiddle", "0px 4px 0px 0px"), Translate.Text("Not found"), link.TargetDatabaseName, link.TargetItemID }));
                    }
                    else
                    {
                        string linkTooltip = this.GetLinkTooltip(reference, link);
                        #region Changed code
                        result.Append(string.Concat(new object[] { "<a target=\"_blank\" href=\"/sitecore/shell/sitecore/content/Applications/Content Editor.aspx?id=", reference.ID, "&la=", reference.Language, "&fo=", reference.ID, "\" class=\"scLink\">", Images.GetImage(reference.Appearance.Icon, 16, 16, "absmiddle", "0px 4px 0px 0px"), reference.GetUIDisplayName(), " - [", reference.Paths.Path, "]</a>" })); // added target=\"_blank\" and changed href
                        #endregion
                    }
                }
            }
        }

        private void RenderReferrers(StringBuilder result, List<Pair<Item, ItemLink>> referrers)
        {
            result.Append(this.ReferrersHeader);
            if (referrers.Count == 0)
            {
                result.Append("<div class=\"scNone\">" + Translate.Text("None") + "</div>");
            }
            else
            {
                result.Append("<div class=\"scRef\">");
                foreach (Pair<Item, ItemLink> pair in referrers)
                {
                    Item item = pair.Part1;
                    ItemLink link = pair.Part2;
                    Item sourceItem = null;
                    if (link != null)
                    {
                        sourceItem = link.GetSourceItem();
                    }
                    if (item == null)
                    {
                        result.Append(string.Format("<div class=\"scLink\">{0} {1}: {2}, {3}</div>", new object[] { Images.GetImage("Applications/16x16/error.png", 16, 16, "absmiddle", "0px 4px 0px 0px"), Translate.Text("Not found"), link.SourceDatabaseName, link.SourceItemID }));
                    }
                    else
                    {
                        string str = item.Language.ToString();
                        string str2 = item.Version.ToString();
                        if (sourceItem != null)
                        {
                            str = sourceItem.Language.ToString();
                            str2 = sourceItem.Version.ToString();
                        }
                        #region Changed code
                        result.Append(string.Concat(new object[] { "<a target=\"_blank\" href=\"/sitecore/shell/sitecore/content/Applications/Content Editor.aspx?id=", item.ID, "&la=", item.Language, "&fo=", item.ID, "\" class=\"scLink\">", Images.GetImage(item.Appearance.Icon, 16, 16, "absmiddle", "0px 4px 0px 0px"), item.GetUIDisplayName() }));// added target=\"_blank\" and changed href
                        #endregion
                        if ((link != null) && !link.SourceFieldID.IsNull)
                        {
                            Field field = item.Fields[link.SourceFieldID];
                            if (!string.IsNullOrEmpty(field.DisplayName))
                            {
                                result.Append(" - ");
                                result.Append(field.DisplayName);
                                if (sourceItem != null)
                                {
                                    Field field2 = sourceItem.Fields[link.SourceFieldID];
                                    if ((field2 != null) && !field2.HasValue)
                                    {
                                        result.Append(" <span style=\"color:#999999\">");
                                        result.Append(Translate.Text("[inherited]"));
                                        result.Append("</span>");
                                    }
                                }
                            }
                        }
                        result.Append(" - [" + item.Paths.Path + "]</a>");
                    }
                }
                result.Append("</div>");
            }
        }

        private bool IsHidden(Item item)
        {
            while (item != null)
            {
                if (item.Appearance.Hidden)
                {
                    return true;
                }
                item = item.Parent;
            }
            return false;
        }
    }
}