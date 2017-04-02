using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Models;
using Data.Services;
using OwnAgent.Objects;

namespace OwnAgent.Controllers
{
    public class MarketController : BaseController
    {
        // GET: Market
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult New()
        {
            ViewBag.TypesList = MarketService.Instance(UserSid).ServiceTypesGetList();
            ViewBag.PayFormsList = MarketService.Instance(UserSid).ServicePayFormsGetList();
            return View();
        }

        [HttpPost]
        public ActionResult New(MarketServices model)
        {
            int id = MarketService.Instance(UserSid).ServiceCreate(model);
            return RedirectToAction("Card", new {id});
        }

        public ActionResult Card(int? id)
        {
            if (!id.HasValue) return HttpNotFound();

            var model = MarketService.Instance(UserSid).ServiceGet(id.Value);
            return View(model);
        }
    }
}