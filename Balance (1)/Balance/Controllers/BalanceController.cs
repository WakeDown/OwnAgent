using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Balance.Models;

namespace Balance.Controllers
{
    public class BalanceController : Controller
    {
        [HttpGet]
        public ActionResult New()
        {
            return View(new Spend());
        }
        [HttpPost]
        public ActionResult New(Spend model)
        {
            model.Save();
            return RedirectToAction("New");
        }
    }
}