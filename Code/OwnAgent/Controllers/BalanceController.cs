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
        public ActionResult Index(string filter, int? year, int? month, int? quarter = null)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("Index", new { filter = "month", year = year, month = month, quarter = quarter });
            if (!year.HasValue) return RedirectToAction("Index", new { filter = filter, year = DateTime.Now.Year, month = month, quarter = quarter });
            if (!month.HasValue) return RedirectToAction("Index", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            if (!quarter.HasValue)
            {
                var quarterD = (double)month / 3;
                if (quarterD < 1) quarter = 1;
                else if (quarterD > 1 && quarterD < 2) quarter = 2;
                else if (quarterD > 2 && quarterD < 3) quarter = 3;
                else if (quarterD > 3 && quarterD < 4) quarter = 4;
                return RedirectToAction("Index", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            }

            return View();
        }

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
        public ActionResult List(string filter = null, int? year = null, int? month = null, int? quarter = null, int? categoryId = null, int? vectorId = null)
        {
            DateTime? dateStart = null;
            DateTime? dateEnd = null;
            if (!year.HasValue) year = DateTime.Now.Year;
            if (!month.HasValue) month = DateTime.Now.Month;
            if (filter == "month")
            {
                dateStart = new DateTime(year.Value, month.Value, 1);
                dateEnd = new DateTime(year.Value, month.Value, DateTime.DaysInMonth(year.Value, month.Value));
            }
            if (filter == "quarter")
            {
                dateEnd = new DateTime(year.Value, month.Value, DateTime.DaysInMonth(year.Value, month.Value));
                var startMonth = dateEnd.Value.AddMonths(-3).Month;
                var startYear = dateEnd.Value.AddMonths(-3).Year;
                dateStart = new DateTime(startYear, startMonth, 1);
            }
            if (filter == "year")
            {
                dateEnd = new DateTime(year.Value, month.Value, DateTime.DaysInMonth(year.Value, month.Value));
                var startMonth = dateEnd.Value.AddMonths(-12).Month;
                var startYear = dateEnd.Value.AddMonths(-12).Year;
                dateStart = new DateTime(startYear, startMonth, 1);
            }
            if (filter == "5year")
            {
                dateEnd = new DateTime(year.Value, month.Value, DateTime.DaysInMonth(year.Value, month.Value));
                var startMonth = dateEnd.Value.AddYears(-5).Month;
                var startYear = dateEnd.Value.AddYears(-5).Year;
                dateStart = new DateTime(startYear, startMonth, 1);
            }

            //var list =Spend.GetList(ClientId);
            int totalCount;
            var list = SpendService.Instance(UserSid).GetSpendList(out totalCount, dateStart: dateStart, dateEnd: dateEnd, categoryId: categoryId, vectorId: vectorId);
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
        public ActionResult Stat(string filter, int? year, int? month, int? quarter = null)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("Stat", new { filter = "month", year = year, month = month, quarter = quarter });
            if (!year.HasValue) return RedirectToAction("Stat", new { filter = filter, year = DateTime.Now.Year, month = month, quarter = quarter });
            if (!month.HasValue) return RedirectToAction("Stat", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            if (!quarter.HasValue)
            {
                var quarterD = (double)month / 3;
                if (quarterD < 1) quarter = 1;
                else if (quarterD > 1 && quarterD < 2) quarter = 2;
                else if (quarterD > 2 && quarterD < 3) quarter = 3;
                else if (quarterD > 3 && quarterD < 4) quarter = 4;
                return RedirectToAction("Stat", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            }

            //if (!y.HasValue || !m.HasValue)
            //   return RedirectToAction("Stat", new {y = DateTime.Now.Year, m = DateTime.Now.Month});

            return View();
        }

        public ActionResult SpendBillReport(string filter, int? year, int? month, int? quarter = null)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("SpendBillReport", new { filter = "month", year = year, month = month, quarter = quarter });
            if (!year.HasValue) return RedirectToAction("SpendBillReport", new { filter = filter, year = DateTime.Now.Year, month = month, quarter = quarter });
            if (!month.HasValue) return RedirectToAction("SpendBillReport", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            if (!quarter.HasValue)
            {
                var quarterD = (double)month / 3;
                if (quarterD < 1) quarter = 1;
                else if (quarterD > 1 && quarterD < 2) quarter = 2;
                else if (quarterD > 2 && quarterD < 3) quarter = 3;
                else if (quarterD > 3 && quarterD < 4) quarter = 4;
                return RedirectToAction("SpendBillReport", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            }

            IEnumerable<SpendStatBillViewModel> list = new List<SpendStatBillViewModel>();
            if (filter == "month") list = SpendService.Instance(UserSid).GetMonthlyBilleport(year.Value, month.Value);
            if (filter == "quarter") list = SpendService.Instance(UserSid).GetQuarterBillReport(year.Value, quarter.Value);
            if (filter == "year") list = SpendService.Instance(UserSid).GetYearlyBillReport(year.Value);
            if (filter == "5year") list = SpendService.Instance(UserSid).Get5YearlyBillReport(year.Value);
            if (filter == "alltime") list = SpendService.Instance(UserSid).GetAllTimeBillReport();

            return View("SpendBillReport", list);
        }

        public ActionResult SpendCategoryReport(string filter, int? year, int? month, int? quarter = null)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("SpendCategoryReport", new { filter = "month", year= year, month= month,quarter=quarter });
            if (!year.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = DateTime.Now.Year, month = month, quarter = quarter });
            if (!month.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            if (!quarter.HasValue)
            {
                var quarterD = (double)month / 3;
                if (quarterD < 1) quarter = 1;
                else if (quarterD > 1 && quarterD < 2) quarter = 2;
                else if (quarterD > 2 && quarterD < 3) quarter = 3;
                else if (quarterD > 3 && quarterD < 4) quarter = 4;
                return RedirectToAction("SpendCategoryReport", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            }

            IEnumerable<SpendStatViewModel> list = new List<SpendStatViewModel>();
            if (filter=="month")list = SpendService.Instance(UserSid).GetMonthlyCategoryReport(year.Value, month.Value);
            if (filter == "quarter") list = SpendService.Instance(UserSid).GetQuarterCategoryReport(year.Value, quarter.Value);
            if (filter == "year") list = SpendService.Instance(UserSid).GetYearlyCategoryReport(year.Value);
            if (filter == "5year") list = SpendService.Instance(UserSid).Get5YearlyCategoryReport(year.Value);
            if (filter == "alltime") list = SpendService.Instance(UserSid).GetAllTimeCategoryReport();

            return View("SpendCategoryReport", list);
        }

        public ActionResult SpendCategoryReportData(string filter, int? year, int? month, int? quarter = null, string vectorSysName = null)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("SpendCategoryReportData", new { filter = "month", year = year, month = month, quarter = quarter });
            if (!year.HasValue) return RedirectToAction("SpendCategoryReportData", new { filter = filter, year = DateTime.Now.Year, month = month, quarter = quarter });
            if (!month.HasValue) return RedirectToAction("SpendCategoryReportData", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            if (!quarter.HasValue)
            {
                var quarterD = (double)month / 3;
                if (quarterD < 1) quarter = 1;
                else if (quarterD > 1 && quarterD < 2) quarter = 2;
                else if (quarterD > 2 && quarterD < 3) quarter = 3;
                else if (quarterD > 3 && quarterD < 4) quarter = 4;
                return RedirectToAction("SpendCategoryReportData", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            }

            IEnumerable<SpendStatViewModel> list = new List<SpendStatViewModel>();
            if (filter == "month") list = SpendService.Instance(UserSid).GetMonthlyCategoryReport(year.Value, month.Value, vectorSysName);
            if (filter == "quarter") list = SpendService.Instance(UserSid).GetQuarterCategoryReport(year.Value, quarter.Value, vectorSysName);
            if (filter == "year") list = SpendService.Instance(UserSid).GetYearlyCategoryReport(year.Value, vectorSysName);
            if (filter == "5year") list = SpendService.Instance(UserSid).Get5YearlyCategoryReport(year.Value, vectorSysName);
            if (filter == "alltime") list = SpendService.Instance(UserSid).GetAllTimeCategoryReport(vectorSysName);

            return Json(list);
        }

        public ActionResult GetSpendListChartData(string filter, int? year, int? month, int? quarter = null, string vectorSysName = null)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("GetSpendListChartData", new { filter = "month", year = year, month = month, quarter = quarter });
            if (!year.HasValue) return RedirectToAction("GetSpendListChartData", new { filter = filter, year = DateTime.Now.Year, month = month, quarter = quarter });
            if (!month.HasValue) return RedirectToAction("GetSpendListChartData", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            if (!quarter.HasValue)
            {
                var quarterD = (double)month / 3;
                if (quarterD < 1) quarter = 1;
                else if (quarterD > 1 && quarterD < 2) quarter = 2;
                else if (quarterD > 2 && quarterD < 3) quarter = 3;
                else if (quarterD > 3 && quarterD < 4) quarter = 4;
                return RedirectToAction("GetSpendListChartData", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            }

            IEnumerable<SpendChartViewModel> list = new List<SpendChartViewModel>();
            if (filter == "month") list = SpendService.Instance(UserSid).GetSpendListMonthlyChartData(year.Value, month.Value, vectorSysName);
            if (filter == "quarter") list = SpendService.Instance(UserSid).GetSpendListQuarterChartData(year.Value, quarter.Value, vectorSysName);
            if (filter == "year") list = SpendService.Instance(UserSid).GetSpendListYearlyChartData(year.Value, vectorSysName);
            if (filter == "5year") list = SpendService.Instance(UserSid).GetSpendList5YearlyChartData(year.Value, vectorSysName);
            if (filter == "alltime") list = SpendService.Instance(UserSid).GetSpendListAllTimeChartData(vectorSysName);

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

        public ActionResult GetCumulativeTotalChartData(string filter, int? year, int? month, int? quarter = null)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("SpendCategoryReport", new { filter = "month" });
            if (!year.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = DateTime.Now.Year, month = month });
            if (!month.HasValue) return RedirectToAction("SpendCategoryReport", new { filter = filter, year = year, month = DateTime.Now.Month });
            if (!quarter.HasValue)
            {
                var quarterD = (double)month / 3;
                if (quarterD < 1) quarter = 1;
                else if (quarterD > 1 && quarterD < 2) quarter = 2;
                else if (quarterD > 2 && quarterD < 3) quarter = 3;
                else if (quarterD > 3 && quarterD < 4) quarter = 4;
                return RedirectToAction("SpendCategoryReportData", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            }

            IEnumerable<SpendChartViewModel> list = new List<SpendChartViewModel>();
            if (filter == "month") list = SpendService.Instance(UserSid).GetMonthlyCumulativeTotalChartData(year.Value, month.Value);
            if (filter == "quarter") list = SpendService.Instance(UserSid).GetQuarterCumulativeTotalChartData(year.Value, quarter.Value);
            if (filter == "year") list = SpendService.Instance(UserSid).GetYearlyCumulativeTotalChartData(year.Value);
            if (filter == "5year") list = SpendService.Instance(UserSid).Get5YearlyCumulativeTotalChartData(year.Value, month.Value);
            if (filter == "alltime") list = SpendService.Instance(UserSid).GetAllTimeCumulativeTotalChartData();

            return Json(list);
        }

        public ActionResult GetCumulativeCategoryChartData(string filter, int? year, int? quarter = null, string vectorSysName = null)
        {
            IEnumerable<KeyValuePair<string, IEnumerable<SpendChartViewModel>>> list = new List<KeyValuePair<string, IEnumerable<SpendChartViewModel>>>();
            if (filter == "year") list = SpendService.Instance(UserSid).GetYearlyCategoryChartDataGroupByMonthes(year.Value, vectorSysName);
            //if (filter == "5year") list = SpendService.Instance(UserSid).Get5YearlyCumulativeTotalChartData(year.Value);
            //if (filter == "alltime") list = SpendService.Instance(UserSid).GetAllTimeCumulativeTotalChartData();

            return Json(list);
        }

        public ActionResult SpendDelete(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            SpendService.Instance(UserSid).SpendDelete(id.Value);

            return Json(new {});
        }

        public ActionResult Charts(string filter, int? year, int? month, int? quarter = null)
        {
            if (String.IsNullOrEmpty(filter)) return RedirectToAction("Charts", new { filter = "year", year = year, month = month, quarter = quarter });
            if (!year.HasValue) return RedirectToAction("Charts", new { filter = filter, year = DateTime.Now.Year, month = month, quarter = quarter });
            if (!month.HasValue) return RedirectToAction("Charts", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            if (!quarter.HasValue)
            {
                var quarterD = (double)month / 3;
                if (quarterD < 1) quarter = 1;
                else if (quarterD > 1 && quarterD < 2) quarter = 2;
                else if (quarterD > 2 && quarterD < 3) quarter = 3;
                else if (quarterD > 3 && quarterD < 4) quarter = 4;
                return RedirectToAction("Charts", new { filter = filter, year = year, month = DateTime.Now.Month, quarter = quarter });
            }

            return View();
        }
    }
}