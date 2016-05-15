using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Helpers
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Add, update, or remove parameters from a URL's query string.
        /// </summary>
        /// <param name="helper">UrlHelper instance</param>
        /// <param name="url">The URL to modify. If null, the current URL from the Request object is used.</param>
        /// <param name="updates">Query string parameters to add/overwrite.</param>
        /// <param name="removes">Query string parameters to remove entirely.</param>
        /// <param name="appends">Query string parameters to append additional values to (using delimiter)</param>
        /// <param name="subtracts">Query string parameters to subtract values from (using delimiter)</param>
        /// <param name="delimiter">Character to use to delimit multiple values for a query string parameter (defaults to `|`)</param>
        /// <returns>URL with modified query string</returns>
        public static string ModifyQueryString(this UrlHelper helper,
            string url,
            IDictionary<string, object> updates = null,
            IEnumerable<string> removes = null,
            IDictionary<string, object> appends = null,
            IDictionary<string, object> subtracts = null,
            char delimiter = '|')
        {
            var request = helper.RequestContext.HttpContext.Request;

            if (string.IsNullOrWhiteSpace(url))
            {
                url = request.RawUrl;
            }

            var urlParts = url.Split('?');
            url = urlParts[0];
            var query = urlParts.Length > 1
                ? HttpUtility.ParseQueryString(urlParts[1])
                : new NameValueCollection();

            if (updates != null)
            {
                updates.Keys.ToList().ForEach(key => query[key] = updates[key].ToString());
            }

            if (removes != null)
            {
                removes.ToList().ForEach(key => query.Remove(key));
            }

            if (appends != null)
            {
                foreach (var key in appends.Keys)
                {
                    var values = new List<string>();
                    if (query.AllKeys.Contains(key))
                    {
                        values.Add(query[key]);
                    }
                    if (typeof(IList).IsAssignableFrom(appends[key].GetType()))
                    {
                        foreach (var item in (appends[key] as IList))
                        {
                            values.Add(item.ToString());
                        }
                    }
                    else
                    {
                        values.Add(appends[key].ToString());
                    }
                    query[key] = string.Join(delimiter.ToString(), values);
                }
            }

            if (subtracts != null)
            {
                foreach (var key in subtracts.Keys)
                {
                    if (query.AllKeys.Contains(key))
                    {
                        var queryParts = query[key].Split(new char[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (typeof(IList).IsAssignableFrom(subtracts[key].GetType()))
                        {
                            foreach (var item in (subtracts[key] as IList))
                            {
                                queryParts.Remove(item.ToString());
                            }
                        }
                        else
                        {
                            queryParts.Remove(subtracts[key].ToString());
                        }
                        query[key] = string.Join(delimiter.ToString(), queryParts);
                    }
                }
            }

            var queryString = string.Join("&",
                query.AllKeys.Where(key =>
                    !string.IsNullOrWhiteSpace(query[key])).Select(key =>
                        string.Join("&", query.GetValues(key).Select(val =>
                            string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(val))))));

            return query.HasKeys() ? url + "?" + queryString : url;
        }

        //Builds URL by finding the best matching route that corresponds to the current URL,
        //with given parameters added or replaced.
        public static MvcHtmlString Current(this UrlHelper helper, object substitutes, string action = null, string controller = null)
        {
            //get the route data for the current URL e.g. /Research/InvestmentModelling/RiskComparison
            //this is needed because unlike UrlHelper.Action, UrlHelper.RouteUrl sets includeImplicitMvcValues to false
            //which causes it to ignore current ViewContext.RouteData.Values
            var rd = new RouteValueDictionary(helper.RequestContext.RouteData.Values);
            if (!String.IsNullOrEmpty(action))
            {
                rd["action"] = action;
            }

            if (!String.IsNullOrEmpty(controller))
            {
                rd["controller"] = controller;
            }

            //get the current query string e.g. ?BucketID=17371&amp;compareTo=123
            var qs =   helper.RequestContext.HttpContext.Request.QueryString;

            //add query string parameters to the route value dictionary
            foreach (string param in qs)
                if (!string.IsNullOrEmpty(qs[param]))
                    rd[param] = qs[param];

            //override parameters we're changing
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
            {
                var value = property.GetValue(substitutes);
                if (string.IsNullOrEmpty(value.ToString())) rd.Remove(property.Name);
                else rd[property.Name] = value;
            }
            //UrlHelper will find the first matching route
            //(the routes are searched in the order they were registered).
            //The unmatched parameters will be added as query string.
            var url = helper.RouteUrl(rd);
            return new MvcHtmlString(url);
        }

    }
}
