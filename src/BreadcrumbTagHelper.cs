using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SmartBreadcrumbs
{
    [HtmlTargetElement("breadcrumb")]
    public class BreadcrumbTagHelper : TagHelper
    {
        #region Fields

        private readonly BreadcrumbsManager _breadcrumbsManager;
        private readonly IUrlHelper _urlHelper;

        #endregion

        #region Properties

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        #endregion

        public BreadcrumbTagHelper(BreadcrumbsManager breadcrumbsManager, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _breadcrumbsManager = breadcrumbsManager;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var child = await output.GetChildContentAsync();

            string action = ViewContext.ActionDescriptor.RouteValues["action"];
            string controller = ViewContext.ActionDescriptor.RouteValues["controller"];

            var nodeKey = $"{controller}.{action}";
            var node = _breadcrumbsManager.GetNode(nodeKey);
          

            output.TagName = _breadcrumbsManager.Options.TagName;
            
            // Tag Classes
            if (!string.IsNullOrEmpty(_breadcrumbsManager.Options.TagClasses))
            {
                output.Attributes.Add("class", _breadcrumbsManager.Options.TagClasses);
            }

            output.Content.AppendHtml($"<ol class=\"{_breadcrumbsManager.Options.OlClasses}\">");

            var sb = new StringBuilder();

            if (node != null)
            {
                  node.Title = ExtractTitleFromModel(node.GetOriginTitle());
                

                sb.Insert(0, GetLi(node.Title, node.GetUrl(_urlHelper),true));

                while (node.Parent != null)
                {
                    node = node.Parent;

                    // Separator
                    if (_breadcrumbsManager.Options.HasSeparatorElement)
                    {
                        sb.Insert(0, _breadcrumbsManager.Options.SeparatorElement);
                    }

                    sb.Insert(0, GetLi(node.Title, node.GetUrl(_urlHelper),false));
                }
            }

            // If the node was custom and it had no defaultnode
            if (node != _breadcrumbsManager.DefaultNode)
            {
                // Separator
                if (_breadcrumbsManager.Options.HasSeparatorElement)
                {
                    sb.Insert(0, _breadcrumbsManager.Options.SeparatorElement);
                }

                sb.Insert(0, GetLi(_breadcrumbsManager.DefaultNode.Title, 
                    _breadcrumbsManager.DefaultNode.GetUrl(_urlHelper), 
                    false));
            }

            output.Content.AppendHtml(sb.ToString());
            output.Content.AppendHtml(child);
            output.Content.AppendHtml("</ol>");

        }



        private string ExtractTitleFromModel(string title)
        {
            var type = ViewContext.ViewData.Model?.GetType();

            if (type == null)
            {
                return title;
            }

            var property = type.GetProperty(title);
            if (property != null)
            {
                return (string) property.GetValue(ViewContext.ViewData.Model);
            }

            var method = type.GetMethod(title);
            if (method != null)
            {
                return (string) method.Invoke(ViewContext.ViewData.Model,null);
            }

            return title;
        }

        private string GetClass(string classes)
        {
            if (string.IsNullOrEmpty(classes))
                return "";

            return $" class=\"{classes}\"";
        }

        private string GetLi(string title, string link, bool isActive)
        {
            var normalTemplate = _breadcrumbsManager.Options.LiTemplate;
            var activeTemplate = _breadcrumbsManager.Options.ActiveLiTemplate;

            if (!isActive && string.IsNullOrEmpty(normalTemplate))
                return $"<li{GetClass(_breadcrumbsManager.Options.LiClasses)}><a href=\"{link}\">{title}</a></li>";

            if (isActive && string.IsNullOrEmpty(activeTemplate))
                return $"<li{GetClass(_breadcrumbsManager.Options.LiClasses)}>{title}</li>";

            return isActive
                ? string.Format(activeTemplate, title, link)
                : string.Format(normalTemplate, title, link);
        }
    }
}