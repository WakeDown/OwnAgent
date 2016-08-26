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
    public class SpendBillController : BaseController
    {
        // GET: SpendBill
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BillList()
        {
            var list = SpendService.Instance(UserSid).SpendBillGetList();

            return View(list);
        }

        [HttpPost]
        public ActionResult SpendBillOrderDown(int id)
        {
            SpendService.Instance(UserSid).SpendBillOrderDown(id);
            return Json(new {});
        }

        [HttpPost]
        public ActionResult SpendBillOrderUp(int id)
        {
            SpendService.Instance(UserSid).SpendBillOrderUp(id);
            return Json(new { });
        }

        [HttpGet]
        public ActionResult Merge()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult BillMerge(int[] bills, int nameId)
        {
            SpendService.Instance(UserSid).SpendBillMerge(bills, nameId);
            return Json(new { });
        }

        [HttpGet]
        public ActionResult New()
        {
            var model = new SpendBills();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SpendBills model)
        {
            SpendService.Instance(UserSid).SpendBillCreate(model);
            return Json(new { });
        }

        public ActionResult Edit(int id)
        {
            var model = SpendService.Instance(UserSid).SpendBillGet(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(SpendBills model)
        {
            SpendService.Instance(UserSid).SpendBillEdit(model);
            return Json(new { });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            SpendService.Instance(UserSid).SpendBillDelete(id);
            return Json(new { });
        }
    }
}