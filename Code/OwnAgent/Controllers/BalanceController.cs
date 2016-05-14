using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Data.Models;
using Data.Services;
using Microsoft.AspNet.Identity;
using Models.Mappers;
using Models.ViewModels;
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
            var spend = new SpendNewViewModel() {Date = DateTime.Now};
            return View(spend);
        }
        //[HttpPost]
        //public JsonResult New(SpendNewViewModel model)
        //{
        //    var spend = SpendViewModel2Spend.Map(model);
        //    SpendService.Instance(User.Identity.GetUserId()).CreateSpend(spend);
        //    //model.Save(ClientId);
        //    return Json(new {});
        //    //return RedirectToAction("New");
        //}

        [HttpPost]
        public JsonResult SpendIncome(SpendNewViewModel model)
        {
            var spend = SpendViewModel2Spend.Map(model);
            SpendService.Instance(User.Identity.GetUserId()).CreateIncome(spend);
            //model.Save(ClientId);
            return Json(new { });
            //return RedirectToAction("New");
        }

        [HttpPost]
        public JsonResult SpendExpense(SpendNewViewModel model)
        {
            var spend = SpendViewModel2Spend.Map(model);
            SpendService.Instance(User.Identity.GetUserId()).CreateExpense(spend);
            //model.Save(ClientId);
            return Json(new { });
            //return RedirectToAction("New");
        }

        [HttpGet]
        public ActionResult SpendTop()
        {
            //var list = Spend.GetTop(ClientId, 7);
            var list = SpendService.Instance(User.Identity.GetUserId()).GetTop(4);
            var models = Spend2SpendViewModel.MapList2TopViewModelList(list);
            return View(models);
        }

        [HttpGet]
        public ActionResult SpendLastAdd()
        {
            int totalCount;
            var list = SpendService.Instance(User.Identity.GetUserId()).GetSpendList(out totalCount, 1, 3);
            var models = Spend2SpendViewModel.MapList2LastAddViewModelList(list);
            return View(models);
        }

        [HttpGet]
        public ActionResult List()
        {
            //var list =Spend.GetList(ClientId);
            int totalCount;
            var list = SpendService.Instance(User.Identity.GetUserId()).GetSpendList(out totalCount);
            return View(list);
        }
        
        [HttpPost]
        public JsonResult GetTop()
        {
            //var list = Spend.GetTop(ClientId, 7);
            var list = SpendService.Instance(User.Identity.GetUserId()).GetTop(7);
            var models = Spend2SpendViewModel.MapList2TopViewModelList(list);
            return Json(models);
        }

        

        [HttpPost]
        public JsonResult GetLastadd()
        {
            int totalCount;
            var list = SpendService.Instance(User.Identity.GetUserId()).GetSpendList(out totalCount, 1, 3);
            var models = Spend2SpendViewModel.MapList2LastAddViewModelList(list);
            return Json(models);
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