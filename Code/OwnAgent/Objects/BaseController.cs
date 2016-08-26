using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Services;
using Microsoft.AspNet.Identity;

namespace OwnAgent.Objects
{
    public class BaseController:Controller
    {
        public string UserSid { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UserSid = User.Identity.GetUserId();
            ViewBag.UserSid = UserSid;

            //Создание категорий для нового пользователя
            if (!String.IsNullOrEmpty(UserSid) && (Session["HasDefaultValues"] == null || !Convert.ToBoolean(Session["HasDefaultValues"])))
            {
                SpendService.Instance(UserSid).CreateDefaultBills();
                SpendService.Instance(UserSid).CreateDefaultCategories();
                Session["HasDefaultValues"] = true;
            }

            base.OnActionExecuting(filterContext);

            
        }
    }
}