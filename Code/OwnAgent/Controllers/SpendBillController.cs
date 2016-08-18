using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Services;
using OwnAgent.Objects;

namespace OwnAgent.Controllers
{
    public class SpendBillController : BaseController
    {
        // GET: SpendBill
        public ActionResult Index()
        {
            var list = SpendService.Instance(UserSid).SpendBillGetList();
            return View(list);
        }

        [HttpPost]
        public ActionResult SpendBillOrderDown()
        {

            return Json(new {});
        }

        [HttpPost]
        public ActionResult SpendBillOrderUp()
        {

            return Json(new { });
        }

        [HttpGet]
        public ActionResult Merge()
        {

            return Json(new { });
        }

        [HttpPost]
        public ActionResult BillMerge()
        {

            return Json(new { });
        }

        [HttpGet]
        public ActionResult New()
        {

            return Json(new { });
        }

        [HttpPost]
        public ActionResult Create()
        {

            return Json(new { });
        }

        [HttpPost]
        public ActionResult Edit()
        {

            return Json(new { });
        }

        [HttpPost]
        public ActionResult Delete()
        {

            return Json(new { });
        }
    }
}