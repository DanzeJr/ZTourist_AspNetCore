using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Infrastructure
{
    [HtmlTargetElement("nav", Attributes = "page-info")]
    public class PaginationButtonTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PaginationButtonTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            this.urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PageInfo PageInfo { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();

        public string PageClass { get; set; } = "";
        public string PageClassNormal { get; set; } = "";
        public string PageClassSelected { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder ul = new TagBuilder("ul");

            for (int i = 0; i <= PageInfo.TotalPages + 1; i++)
            {
                if (i == 0 && PageInfo.CurrentPage == 1) // if current page is 1 then does not display prev button
                    continue;
                else if (i == (PageInfo.TotalPages + 1) && PageInfo.CurrentPage == PageInfo.TotalPages) // if current page is end of pages then does not display next button
                    continue;
                TagBuilder li = new TagBuilder("li");
                TagBuilder a = new TagBuilder("a");

                li.AddCssClass(PageClass);
                if (i == 0)
                {
                    li.AddCssClass("tg-prevpage");
                    PageUrlValues["page"] = PageInfo.CurrentPage - 1;
                }
                else if (i == (PageInfo.TotalPages + 1))
                {
                    li.AddCssClass("tg-nextpage");
                    PageUrlValues["page"] = PageInfo.CurrentPage + 1;
                }
                else
                {
                    li.AddCssClass(PageInfo.CurrentPage == i ? PageClassSelected : PageClassNormal);
                    PageUrlValues["page"] = i;
                }

                a.Attributes["href"] = PageInfo.CurrentPage == i ? "javascript:void(0);" : urlHelper.Action(PageInfo.PageAction, PageUrlValues);

                if (i == 0)
                {
                    TagBuilder iTag = new TagBuilder("i");
                    iTag.Attributes["class"] = "fa fa-angle-left";
                    a.InnerHtml.AppendHtml(iTag);
                }
                else if (i == (PageInfo.TotalPages + 1))
                {
                    TagBuilder iTag = new TagBuilder("i");
                    iTag.Attributes["class"] = "fa fa-angle-right";
                    a.InnerHtml.AppendHtml(iTag);
                }
                else
                {
                    a.InnerHtml.Append(i.ToString());
                }
                li.InnerHtml.AppendHtml(a);
                ul.InnerHtml.AppendHtml(li);
            } //end of for

            output.Content.AppendHtml(ul);
        }
    }
}
