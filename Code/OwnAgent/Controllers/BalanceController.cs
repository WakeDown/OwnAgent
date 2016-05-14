using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using OwnAgent.Models;
using OwnAgent.Objects;

namespace OwnAgent.Controllers
{
    [Authorize]
    public class BalanceController:BaseController
    {
        [HttpGet]
        public ActionResult New()
        {
            string s = ClientId;
            return View(new Spend());
        }
        [HttpPost]
        public JsonResult New(Spend model)
        {
            model.Save(ClientId);
            return Json(new {});
            //return RedirectToAction("New");
        }

        [HttpGet]
        public ActionResult List()
        {
            var list =Spend.GetList(ClientId);

            return View(list);
        }
        
        [HttpPost]
        public JsonResult GetTop()
        {
            var list = Spend.GetTop(ClientId, 7);
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetLastadd()
        {
            var list = Spend.GetList(ClientId, 3);
            return Json(list);
        }

        //public ActionResult ShortList()
        //{
        //    var list = Spend.GetList(3);

        //    return View(list);
        //}
        public ActionResult Stat(int? y, int? m)
        {
            if (!y.HasValue || !m.HasValue)
               return RedirectToAction("Stat", new {y = DateTime.Now.Year, m = DateTime.Now.Month});

            return View();
        }
    }
}