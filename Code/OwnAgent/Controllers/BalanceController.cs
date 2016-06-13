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
        //    SpendService.Instance(UserSid).CreateSpend(spend);
        //    //model.Save(ClientId);
        //    return Json(new {});
        //    //return RedirectToAction("New");
        //}

        [HttpPost]
        public JsonResult SpendIncome(SpendNewViewModel model)
        {
            var spend = SpendViewModel2Spend.Map(model);
            SpendService.Instance(UserSid).CreateIncome(spend);
            //model.Save(ClientId);
            return Json(new { });
            //return RedirectToAction("New");
        }

        [HttpPost]
        public JsonResult SpendExpense(SpendNewViewModel model)
        {
            var spend = SpendViewModel2Spend.Map(model);
            SpendService.Instance(UserSid).CreateExpense(spend);
            //model.Save(ClientId);
            return Json(new { });
            //return RedirectToAction("New");
        }

        [HttpGet]
        public ActionResult SpendTop()
        {
            //var list = Spend.GetTop(ClientId, 7);
            var list = SpendService.Instance(UserSid).GetTop(4);
            var models = Spend2SpendViewModel.MapList2TopViewModelList(list);
            return View(models);
        }

        [HttpGet]
        public ActionResult SpendLastAdd()
        {
            int totalCount;
            var list = SpendService.Instance(UserSid).GetSpendList(out totalCount, 1, 4);
            var models = Spend2SpendViewModel.MapList2LastAddViewModelList(list);
            return View(models);
        }

        [HttpGet]
        public ActionResult List()
        {
            //var list =Spend.GetList(ClientId);
            int totalCount;
            var list = SpendService.Instance(UserSid).GetSpendList(out totalCount);
            return View(list);
        }
        
        [HttpPost]
        public JsonResult GetTop()
        {
            //var list = Spend.GetTop(ClientId, 7);
            var list = SpendService.Instance(UserSid).GetTop(7);
            var models = Spend2SpendViewModel.MapList2TopViewModelList(list);
            return Json(models);
        }

        

        [HttpPost]
        public JsonResult GetLastadd()
        {
            int totalCount;
            var list = SpendService.Instance(UserSid).GetSpendList(out totalCount, 1, 3);
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
            if (filter=="month")list = SpendService.Instance(UserSid).GetMonthlyCategoryReport(year.Value, month.Value);
            if (filter == "quarter") list = SpendService.Instance(UserSid).GetQuarterCategoryReport(year.Value, month.Value);
            if (filter == "year") list = SpendService.Instance(UserSid).GetYearlyCategoryReport(year.Value, month.Value);
            if (filter == "5year") list = SpendService.Instance(UserSid).Get5YearlyCategoryReport(year.Value, month.Value);
            if (filter == "alltime") list = SpendService.Instance(UserSid).GetAllTimeCategoryReport();

            return View("SpendCategoryReport", list);
        }

        public ActionResult GetIncomeChartData(string filter, int? year, int? month)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("SpendCategoryReport", new { filter = "month" });
            if (!year.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = DateTime.Now.Year, month = month });
            if (!month.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = year, month = DateTime.Now.Month });

            IEnumerable<SpendChartViewModel> list = new List<SpendChartViewModel>();
            if (filter == "month") list = SpendService.Instance(UserSid).GetIncomeMonthlyChartData(year.Value, month.Value);

            return Json(list);
        }

        public ActionResult GetExpenseChartData(string filter, int? year, int? month)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("SpendCategoryReport", new { filter = "month" });
            if (!year.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = DateTime.Now.Year, month = month });
            if (!month.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = year, month = DateTime.Now.Month });

            IEnumerable<SpendChartViewModel> list = new List<SpendChartViewModel>();
            if (filter == "month") list = SpendService.Instance(UserSid).GetExpenseMonthlyChartData(year.Value, month.Value);

            return Json(list);
        }

        public ActionResult GetDifferenceChartData(string filter, int? year, int? month)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("SpendCategoryReport", new { filter = "month" });
            if (!year.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = DateTime.Now.Year, month = month });
            if (!month.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = year, month = DateTime.Now.Month });

            IEnumerable<SpendChartViewModel> list = new List<SpendChartViewModel>();
            if (filter == "month") list = SpendService.Instance(UserSid).GetDifferenceMonthlyChartData(year.Value, month.Value);

            return Json(list);
        }

        public ActionResult GetCumulativeTotalChartData(string filter, int? year, int? month)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("SpendCategoryReport", new { filter = "month" });
            if (!year.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = DateTime.Now.Year, month = month });
            if (!month.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = year, month = DateTime.Now.Month });

            IEnumerable<SpendChartViewModel> list = new List<SpendChartViewModel>();
            if (filter == "month") list = SpendService.Instance(UserSid).GetMonthlyCumulativeTotalChartData(year.Value, month.Value);
            if (filter == "quarter") list = SpendService.Instance(UserSid).GetQuarterCumulativeTotalChartData(year.Value, month.Value);
            if (filter == "year") list = SpendService.Instance(UserSid).GetYearlyCumulativeTotalChartData(year.Value, month.Value);
            if (filter == "5year") list = SpendService.Instance(UserSid).Get5YearlyCumulativeTotalChartData(year.Value, month.Value);
            if (filter == "alltime") list = SpendService.Instance(UserSid).GetAllTimeCumulativeTotalChartData();

            return Json(list);
        }

        public ActionResult SpendDelete(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            SpendService.Instance(UserSid).SpendDelete(id.Value);

            return Json(new {});
        }
    }
}