using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Data;
using LNF.Web.Mvc.UI;
using LNF.Web.Mvc.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace LNF.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString SiteMenu(this HtmlHelper helper, ClientItem client)
        {
            if (helper.ViewContext.HttpContext.Session["SiteMenu"] == null)
                helper.ViewContext.HttpContext.Session["SiteMenu"] = WebUtility.GetSiteMenu(client.ClientID);

            return new HtmlString(helper.ViewContext.HttpContext.Session["SiteMenu"].ToString());
        }

        public static IHtmlString BootstrapMenu(this HtmlHelper helper, IEnumerable<DropDownMenuItem> items, string logoUrl = "", object htmlAttributes = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<nav class=\"navbar navbar-default navbar-fixed-top\" role=\"navigation\">");
            sb.AppendLine("<div class=\"container-fluid\">");

            //Brand and toggle get grouped for better mobile display
            sb.AppendLine("<div class=\"navbar-header\">");
            sb.AppendLine("<button type=\"button\" class=\"navbar-toggle collapsed\" data-toggle=\"collapse\" data-target=\"#bs-navbar-collapse-1\">");
            sb.AppendLine("<span class=\"sr-only\">Toggle navigation</span>");
            sb.AppendLine("<span class=\"icon-bar\"></span>");
            sb.AppendLine("<span class=\"icon-bar\"></span>");
            sb.AppendLine("<span class=\"icon-bar\"></span>");
            sb.AppendLine("</button>");
            sb.AppendLine(string.Format("<a class=\"navbar-brand\" href=\"/\">{0}</a>", SendEmail.CompanyName));
            sb.AppendLine("</div>");

            sb.AppendLine("<div class=\"collapse navbar-collapse\" id=\"bs-navbar-collapse-1\">");

            //left items
            sb.AppendLine("<ul class=\"nav navbar-nav\">");
            foreach (DropDownMenuItem item in items)
            {
                if (item.ParentID == 0)
                {
                    sb.AppendLine("<li class=\"dropdown\">");
                    sb.AppendLine(string.Format("<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">{0} <span class=\"caret\"></span></a>", item.Text));
                    sb.AppendLine("<ul class=\"dropdown-menu\" role=\"menu\">");
                    foreach (var child in items.Where(x => x.ParentID == item.ID))
                    {
                        sb.AppendLine(string.Format("<li><a href=\"{0}\">{1}</a></li>", child.URL, child.Text));
                    }
                    sb.AppendLine("</ul>");
                    sb.AppendLine("</li>");
                }
            }
            sb.AppendLine("</ul>");

            //right items
            sb.AppendLine("<ul class=\"nav navbar-nav navbar-right\">");
            if (CacheManager.Current.CurrentUser != null)
            {
                sb.AppendLine("<li class=\"dropdown\">");
                sb.AppendLine(string.Format("<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">{0} {1} <span class=\"caret\"></span></a>", CacheManager.Current.CurrentUser.FName, CacheManager.Current.CurrentUser.LName));
                sb.AppendLine("<ul class=\"dropdown-menu\" role=\"menu\">");
                sb.AppendLine(string.Format("<li><a href=\"{0}\">Preferences</a></li>", VirtualPathUtility.ToAbsolute("~/preferences")));
                sb.AppendLine(string.Format("<li><a href=\"{0}\">Sign Out</a></li>", ServiceProvider.Current.Context.LoginUrl));
                sb.AppendLine("</ul>");
                sb.AppendLine("</li>");
            }
            else
            {
                sb.AppendLine(string.Format("<li><a href=\"{0}\">Sign In</a></li>", ServiceProvider.Current.Context.LoginUrl));
            }
            sb.AppendLine("</ul>");

            sb.AppendLine("</div>"); //<!-- /.navbar-collapse -->
            sb.AppendLine("</div>"); //<!-- /.container-fluid -->
            sb.AppendLine("</nav>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlString DropDownMenu(this HtmlHelper helper, IEnumerable<DropDownMenuItem> items, string logoUrl = "", object htmlAttributes = null)
        {
            DropDownMenu result = new DropDownMenu(items, logoUrl, htmlAttributes);
            return new HtmlString(result.Render());
        }

        public static IHtmlString Table<T>(this HtmlHelper helper, IEnumerable<T> items, object htmlAttributes = null, params TableColumn<T>[] columnSet)
        {
            return new Table<T>(items)
                .Attributes.Add(htmlAttributes)
                .GetHtml(columnSet);
        }

        public static IHtmlString PageMenuLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, string currentPage, ClientPrivilege requiredPriv = 0)
        {
            if (requiredPriv == 0 || CacheManager.Current.CurrentUser.HasPriv(requiredPriv))
                return helper.ActionLink(linkText, actionName, controllerName, null, new { @class = "nav-menu-item" + ((currentPage == controllerName) ? " nav-selected" : string.Empty) });
            else
                return new HtmlString(string.Empty);
        }

        public static IHtmlString CheckBoxList(this HtmlHelper helper, string name, IEnumerable<CheckBoxListItem> items = null, CheckBoxListOptions options = null)
        {
            CheckBoxList cbl = new CheckBoxList(items, options);
            return new HtmlString(cbl.Render(name));
        }

        public static IHtmlString CheckBoxList<T>(this HtmlHelper helper, string name, IEnumerable<T> items, Expression<Func<T, object>> valueExpression, Expression<Func<T, object>> textExpression, CheckBoxListOptions options = null)
        {
            List<CheckBoxListItem> cbliList = new List<CheckBoxListItem>();

            foreach (T item in items)
            {
                CheckBoxListItem cbli = new CheckBoxListItem
                {
                    Value = valueExpression.Compile().Invoke(item).ToString(),
                    Text = textExpression.Compile().Invoke(item).ToString()
                };
                cbliList.Add(cbli);
            }

            CheckBoxList cbl = new CheckBoxList(cbliList, options);
            return new HtmlString(cbl.Render(name));
        }

        public static IHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<CheckBoxListItem> items, CheckBoxListOptions options = null)
        {
            string name = string.Empty;
            TProperty val = helper.EvalExpression(expression, out name);

            TagBuilder builder = new TagBuilder("div");
            string cssClass = options == null ? string.Empty : options.CssClass;
            string colCssClass = options == null || string.IsNullOrEmpty(options.ColumnCssClass) ? "col-sm-1" : options.ColumnCssClass;
            int itemsPerRow = options == null ? 4 : options.ItemsPerRow;
            builder.AddCssClass(("container-fluid checkbox-list " + cssClass).Trim());
            TagBuilder hidden = new TagBuilder("input");
            hidden.Attributes.Add("type", "hidden");
            hidden.Attributes.Add("name", name);
            hidden.Attributes.Add("id", name);
            hidden.Attributes.Add("value", val.ToString());
            builder.InnerHtml = hidden.ToString();
            TagBuilder row = new TagBuilder("div");
            row.AddCssClass("row");
            int index = 0;

            foreach (CheckBoxListItem item in items)
            {
                if (index > 0 && index % itemsPerRow == 0)
                {
                    builder.InnerHtml += row.ToString();
                    row = new TagBuilder("div");
                    row.AddCssClass("row");
                }

                TagBuilder col = new TagBuilder("div");
                col.AddCssClass(colCssClass);

                TagBuilder label = new TagBuilder("label");
                label.AddCssClass("control-label");

                TagBuilder checkbox = new TagBuilder("input");
                checkbox.Attributes.Add("type", "checkbox");
                checkbox.Attributes.Add("value", item.Value);

                label.InnerHtml = string.Format("{0} {1}", checkbox.ToString(), item.Text);

                col.InnerHtml = label.ToString();

                row.InnerHtml += col.ToString();

                index++;
            }

            if (!string.IsNullOrEmpty(row.InnerHtml))
                builder.InnerHtml += row.ToString();

            return new HtmlString(builder.ToString());
        }

        public static IHtmlString MultiSelectList(this HtmlHelper helper, string name, IEnumerable<MultiSelectItem> items = null, MultiSelectOptions options = null)
        {
            MultiSelect ms = new MultiSelect(items, options);
            return new HtmlString(ms.Render(name));
        }

        public static IHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> items, object htmlAttributes = null)
        {
            StringBuilder sb = new StringBuilder();

            TProperty val = helper.EvalExpression(expression, out string name);

            RouteValueDictionary attr = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            sb.AppendLine(string.Format("<div{0}>", attr.ToHtmlAttributesString()));

            foreach (SelectListItem sli in items)
            {
                bool selected = sli.Value == val.ToString();
                sb.AppendLine(string.Format("<label><input type=\"radio\" name=\"{0}\" value=\"{1}\"{2} />{3}</label>", name, sli.Value, (selected ? " checked" : string.Empty), sli.Text));
            }

            sb.AppendLine("</div>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlString CheckBoxFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, bool>> expression, object htmlAttributes, bool useHiddenField, string jshandler = null)
        {
            StringBuilder sb = new StringBuilder();

            bool val = helper.EvalExpression(expression, out string name);

            RouteValueDictionary attr = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            if (useHiddenField)
            {
                sb.AppendLine(string.Format("<input type=\"checkbox\"{0}{1}{2} />",
                    (val ? " checked=\"checked\"" : string.Empty),
                    attr.ToHtmlAttributesString(),
                    (string.IsNullOrEmpty(jshandler) ? string.Empty : " onclick=\"" + jshandler + "(this,{'target_id':'" + name + "'});\"")
                ));
                sb.AppendLine(string.Format("<input type=\"hidden\" id=\"{0}\" name=\"{0}\" value=\"{1}\" />",
                    name, (val ? "true" : "false")
                ));
            }
            else
            {
                sb.AppendLine(string.Format("<input type=\"checkbox\" id=\"{0}\" name=\"{0}\"{1}{2} />",
                    name, (val ? " checked=\"checked\"" : string.Empty), attr.ToHtmlAttributesString()
                ));
            }

            return new HtmlString(sb.ToString());
        }

        public static IHtmlString OptGroupDropDownList(this HtmlHelper helper, string name, IEnumerable<OptGroupSelectListItem> items = null, object htmlAttributes = null)
        {
            OptGroupDropDownList ogddl = new OptGroupDropDownList(name, items, htmlAttributes);
            StringBuilder sb = new StringBuilder();
            ogddl.Render(sb);
            return new HtmlString(sb.ToString());
        }

        public static IHtmlString OptGroupDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<OptGroupSelectListItem> items = null, object htmlAttributes = null)
        {
            Func<TModel, TProperty> func = expression.Compile();
            var selected_value = func(helper.ViewData.Model);

            string id = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
            string name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));

            OptGroupDropDownList ogddl = new OptGroupDropDownList(name, items, htmlAttributes);
            ogddl.SelectedValue = selected_value;

            StringBuilder sb = new StringBuilder();
            ogddl.Render(sb);

            return new HtmlString(sb.ToString());
        }

        public static IHtmlString PeriodPickerFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, DateTime>> exp, int startYear = 2003, int endYear = 0, string monthFormat = "MMMM", bool autoPostback = false, object htmlAttributes = null)
        {
            string name;
            DateTime period = helper.EvalExpression(exp, out name);
            PeriodPicker picker = new PeriodPicker(name, period, startYear, endYear, monthFormat, autoPostback, htmlAttributes);
            return picker.Render();
        }

        public static IHtmlString NavigationLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, object routeValues = null, bool ignoreAction = true, bool ignoreRouteValues = true)
        {
            //http://chrisondotnet.com/2012/08/setting-active-link-twitter-bootstrap-navbar-aspnet-mvc/

            var currentAction = helper.ViewContext.RouteData.GetRequiredString("action");
            var currentController = helper.ViewContext.RouteData.GetRequiredString("controller");
            var currentRouteValues = helper.ViewContext.RouteData.Values;

            var builder = new TagBuilder("li")
            {
                InnerHtml = helper.ActionLink(linkText, actionName, controllerName, routeValues, null).ToHtmlString()
            };

            builder.MergeAttribute("role", "presentation");

            if ((ignoreAction || actionName == currentAction) && controllerName == currentController)
            {
                bool active = true;

                if (routeValues != null && !ignoreRouteValues)
                {
                    var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(routeValues);
                    foreach (var kvp in dict)
                    {
                        if (currentRouteValues.ContainsKey(kvp.Key))
                        {
                            if (!currentRouteValues[kvp.Key].Equals(kvp.Value))
                                active = false;
                        }
                    }
                }

                if (active)
                    builder.AddCssClass("active");
            }

            return new HtmlString(builder.ToString());
        }

        public static IHtmlString CreateSubMenu(this HtmlHelper helper, SubMenu subMenu, string currentAction = null, string currentController = null)
        {
            currentAction = currentAction ?? helper.ViewContext.RouteData.GetRequiredString("action");
            currentController = currentController ?? helper.ViewContext.RouteData.GetRequiredString("controller");
            var activeItem = subMenu.FirstOrDefault(x => x.ActionName == currentAction && x.ControllerName == currentController);
            if (activeItem != null) activeItem.Active = true;
            IHtmlString result = helper.Partial("_SubMenuPartial", subMenu);
            return result;
        }

        public static IHtmlString FormatNullableDate(this HtmlHelper helper, DateTime? value, string format)
        {
            if (value.HasValue)
                return new HtmlString(value.Value.ToString(format));
            else
                return new HtmlString(string.Empty);
        }

        public static IHtmlString Email(this HtmlHelper helper, string name, object value = null, object htmlAttributes = null)
        {
            var email = new UI.Email() { Name = name, Value = value, HtmlAttributes = htmlAttributes };
            return email.Render();
        }

        public static IHtmlString EmailFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            string name;
            TProperty value = helper.EvalExpression(expression, out name);
            return Email(helper, name, value, htmlAttributes);
        }

        public static IHtmlString Calendar(this HtmlHelper helper, string name, DateTime value, object htmlAttributes = null)
        {
            var cal = new Calendar() { Name = name, SelectedMonth = UI.Calendar.GetFirstOfMonth(value), SelectedDate = value, HtmlAttributes = htmlAttributes };
            return cal.Render();
        }

        public static IHtmlString CalendarFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, DateTime>> expression, object htmlAttributes = null)
        {
            string name;
            DateTime value = helper.EvalExpression(expression, out name);
            return Calendar(helper, name, value, htmlAttributes);
        }

        public static IHtmlString GoogleAnalytics(this HtmlHelper helper)
        {
            var ga = new GoogleAnalytics();
            return ga.Render();
        }

        public static string ToHtmlAttributesString(this RouteValueDictionary rvd)
        {
            string result = string.Empty;
            if (rvd != null)
            {
                foreach (string key in rvd.Keys)
                {
                    result += string.Format(" {0}=\"{1}\"", key, rvd[key]);
                }
            }
            return result;
        }

        public static TProperty EvalExpression<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, out string name)
        {
            try
            {
                name = LNF.CommonTools.Utility.PropertyName(expression);
                return expression.Compile().Invoke(helper.ViewData.Model);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Unable to retrieve property data from model", ex);
            }
        }
    }
}