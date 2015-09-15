using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace OwnAgent.Objects
{
    public class BaseController:Controller
    {
        public string ClientId { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ClientId = User.Identity.GetUserId();
            ViewBag.ClientId = ClientId;

            base.OnActionExecuting(filterContext);
        }
    }
}