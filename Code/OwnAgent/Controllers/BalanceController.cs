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
        public ActionResult Stat(string filter, int? year, int? month)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("Stat", new { filter = "month", year = year, month = month });
            if (!year.HasValue) return RedirectToAction("Stat", new { filter = filter, year = DateTime.Now.Year, month = month });
            if (!month.HasValue) return RedirectToAction("Stat", new { filter = filter, year = year, month = DateTime.Now.Month });

            //if (!y.HasValue || !m.HasValue)
            //   return RedirectToAction("Stat", new {y = DateTime.Now.Year, m = DateTime.Now.Month});

            return View();
        }

        public ActionResult SpendCategoryReport(string filter, int? year, int? month)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("SpendCategoryReport", new { filter = "month" });
            if (!year.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = DateTime.Now.Year, month = month });
            if (!month.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = year, month = DateTime.Now.Month });

            IEnumerable<SpendStatViewModel> list = new List<SpendStatViewModel>();
            if (filter=="month")list = SpendService.Instance(User.Identity.GetUserId()).GetMonthlyCategoryReport(year.Value, month.Value);
            if (filter == "quarter") list = SpendService.Instance(User.Identity.GetUserId()).GetQuarterCategoryReport(year.Value, month.Value);
            if (filter == "year") list = SpendService.Instance(User.Identity.GetUserId()).GetYearlyCategoryReport(year.Value, month.Value);
            if (filter == "5year") list = SpendService.Instance(User.Identity.GetUserId()).Get5YearlyCategoryReport(year.Value, month.Value);
            if (filter == "alltime") list = SpendService.Instance(User.Identity.GetUserId()).GetAllTimeCategoryReport();

            return View("SpendCategoryReport", list);
        }
    }
}