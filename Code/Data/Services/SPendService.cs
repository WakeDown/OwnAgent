using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Data.Objects;
using Models.ViewModels;

namespace Data.Services
{
    public class SpendService : BaseService
    {
        public SpendService(string userSid) : base(userSid)
        {
        }
        public static SpendService Instance(string userSid)
        {
            return new SpendService(userSid);
        }

        public void CreateDefaultCategories()
        {
            if (!Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid).Any())
            {
                var cat = new SpendCategory();
                cat.Enabled = true;
                cat.UserSid = UserSid;
                cat.Name = "Еда/Бытовое";
                cat.OrderNum = 10;
                Uow.SpendCategories.Insert(cat);
                Uow.Commit();

                cat.Name = "Услуги";
                cat.OrderNum = 20;
                Uow.SpendCategories.Insert(cat);
                Uow.Commit();

                cat.Name = "Ресторан";
                cat.OrderNum = 30;
                Uow.SpendCategories.Insert(cat);
                Uow.Commit();

                cat.Name = "Транспорт/Бензин";
                cat.OrderNum = 40;
                Uow.SpendCategories.Insert(cat);
                Uow.Commit();

                cat.Name = "Одежда";
                cat.OrderNum = 50;
                Uow.SpendCategories.Insert(cat);
                Uow.Commit();

                cat.Name = "Зарплата";
                cat.OrderNum = 60;
                Uow.SpendCategories.Insert(cat);
                Uow.Commit();

                cat.Name = "Подработка";
                cat.OrderNum = 70;
                Uow.SpendCategories.Insert(cat);
                Uow.Commit();

                cat.Name = "Другое";
                cat.OrderNum = 80;
                Uow.SpendCategories.Insert(cat);
                Uow.Commit();
            }
        }

        public IEnumerable<KeyValuePair<int, string>> GetCategorySelectionList()
        {
            var list = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid, x => x.OrderBy(y => y.OrderNum).ThenBy(y => y.Name));
            var result = new List<KeyValuePair<int, string>>();
            foreach (var category in list)
            {
                result.Add(new KeyValuePair<int, string>(category.CategoryId, category.Name));
            }
            return result;
        }

        public void CreateIncome(Spend spend)
        {
            spend.VectorId = Uow.SpendVectors.GetOne(x => x.SysName == "INC").VectorId;
            CreateSpend(spend);
        }

        public void CreateExpense(Spend spend)
        {
            spend.VectorId = Uow.SpendVectors.GetOne(x => x.SysName == "EXP").VectorId;
            CreateSpend(spend);
        }

        public void CreateSpend(Spend spend)
        {
            spend.CreateDate = DateTime.Now;
            spend.Enabled = true;
            spend.UserSid = UserSid;
            Uow.Spends.Insert(spend);
            Uow.Commit();
        }

        public IEnumerable<Spend> GetSpendList(out int totalCount, int? page = null, int? psize = null)
        {
            var list = Uow.Spends.GetAllWithTotalCount(out totalCount, page, psize, x => x.UserSid == UserSid && x.Enabled
            , x => x.OrderByDescending(y => y.Date), x => x.SpendCategory, x => x.SpendVector);
            return list;
        }

        public IEnumerable<Spend> GetTop(int topRows)
        {
            var limitDate = DateTime.Now.AddMonths(-3).Date;
            var list = (from s in Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.Date >= limitDate)
                        group s by new
                        {
                            s.SpendVector,
                            s.SpendCategory,
                            s.Sum
                        } into gcs
                        let count = gcs.Count()

                        orderby count descending
                        select new
                        {
                            SpendVector = gcs.Key.SpendVector,
                            SpendCategory = gcs.Key.SpendCategory,
                            Sum = gcs.Key.Sum
                        }).AsEnumerable().Select(x => new Spend
                        {
                            SpendVector = x.SpendVector,
                            SpendCategory = x.SpendCategory,
                            Sum = x.Sum
                        }).Take(topRows).ToList();
            return list;
        }

        public IEnumerable<SpendStatViewModel> GetMonthlyCategoryReport(int year, int month)
        {
            var startDate = new DateTime(year, month,1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return GetPeriodReport(startDate, endDate);
        }

        public IEnumerable<SpendStatViewModel> GetQuarterCategoryReport(int endYear, int endMonth)
        {
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startMonth = endDate.AddMonths(-3).Month;
            var startYear = endDate.AddMonths(-3).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetPeriodReport(startDate, endDate);
        }

        public IEnumerable<SpendStatViewModel> GetYearlyCategoryReport(int endYear, int endMonth)
        {
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startMonth = endDate.AddMonths(-12).Month;
            var startYear = endDate.AddMonths(-12).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetPeriodReport(startDate, endDate);
        }

        public IEnumerable<SpendStatViewModel> Get5YearlyCategoryReport(int endYear, int endMonth)
        {
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startMonth = endDate.AddMonths(-60).Month;
            var startYear = endDate.AddMonths(-60).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetPeriodReport(startDate, endDate);
        }

        public IEnumerable<SpendStatViewModel> GetAllTimeCategoryReport()
        {
            int endYear = DateTime.Now.Year + 100;
            int endMonth = DateTime.Now.Month;
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startDate = new DateTime(1900, 1, 1);

            return GetPeriodReport(startDate, endDate);
        }

        public IEnumerable<SpendStatViewModel> GetPeriodReport(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньшще даты начала!");

            var list = new List<SpendStatViewModel>();

            var sDate = startDate.Date;
            var eDate = endDate.Date;

            var report = from s in Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.Date >= sDate && x.Date <= eDate)
                         group s by new
                         {
                             s.SpendCategory,
                             s.SpendVector
                         }
                into gs
                         select new
                         {
                             SpendVectorIconName = gs.Key.SpendVector.IconName,
                             SpendVectorBgColorClass = gs.Key.SpendVector.BgColorClass,
                             SpendVectorSysName = gs.Key.SpendVector.SysName,
                             SpendCategory = gs.Key.SpendCategory.Name,
                             SpendVector = gs.Key.SpendVector.Name,
                             Sum = gs.Sum(x => x.Sum)
                         };
            list = report.AsEnumerable().Select(x => new SpendStatViewModel
            {
                SpendVectorIconName = x.SpendVectorIconName,
                SpendVectorBgColorClass = x.SpendVectorBgColorClass,
                SpendVectorSysName = x.SpendVectorSysName,
                SpendVectorName = x.SpendVector,
                SpendCategoryName = x.SpendCategory,
                Sum = x.Sum
            }).OrderBy(x => x.SpendVectorName).ThenByDescending(x => x.Sum).ToList();


            return list;
        }

        public IEnumerable<SpendChartViewModel> GetIncomeMonthlyChartData(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return GetIncExpChartData("INC", startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetExpenseMonthlyChartData(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return GetIncExpChartData("EXP", startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetIncExpChartData(string vectorSysName, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньшще даты начала!");

            var sDate = startDate.Date;
            var eDate = endDate.Date;

            var data =
                Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.SpendVector.SysName== vectorSysName && x.Date >= sDate && x.Date <= eDate,
                    x => x.OrderBy(y => y.Date)).Select(x=>new SpendChartViewModel() { Date = x.Date, Sum = x.Sum});

            return data;
        }

        public IEnumerable<SpendChartViewModel> GetDifferenceMonthlyChartData(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return GetDifferenceChartData(startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetDifferenceChartData(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньшще даты начала!");

            var sDate = startDate.Date;
            var eDate = endDate.Date;

            var list = from s in Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.Date >= sDate && x.Date <= eDate)
            group s by new
            {
                s.Date
            }
                into gs
            select new
            {
                Date = gs.Key.Date,
                Sum = gs.Sum(x => x.Sum)
            };
            var data = list.AsEnumerable().Select(x => new SpendChartViewModel
            {
                Date = x.Date,
                Sum = x.Sum
            }).OrderBy(x => x.Date).ToList();

            return data;
        }

        public IEnumerable<SpendChartViewModel> GetMonthlyCumulativeTotalChartData(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return GetCumulativeTotalChartData(startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetQuarterCumulativeTotalChartData(int endYear, int endMonth)
        {
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startMonth = endDate.AddMonths(-3).Month;
            var startYear = endDate.AddMonths(-3).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetCumulativeTotalChartData(startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetYearlyCumulativeTotalChartData(int endYear, int endMonth)
        {
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startMonth = endDate.AddMonths(-12).Month;
            var startYear = endDate.AddMonths(-12).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetCumulativeTotalChartData(startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> Get5YearlyCumulativeTotalChartData(int endYear, int endMonth)
        {
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startMonth = endDate.AddMonths(-60).Month;
            var startYear = endDate.AddMonths(-60).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetCumulativeTotalChartData(startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetAllTimeCumulativeTotalChartData()
        {
            int endYear = DateTime.Now.Year + 100;
            int endMonth = DateTime.Now.Month;
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startDate = new DateTime(1900, 1, 1);

            return GetCumulativeTotalChartData(startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetCumulativeTotalChartData(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньшще даты начала!");

            var sDate = startDate.Date;
            var eDate = endDate.Date;

            double currentTotal = 0;

            var data = Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.Date >= sDate && x.Date <= eDate, x => x.OrderBy((y=>y.Date)), x=>x.SpendVector).AsEnumerable()
                  .Select(x => {
                      currentTotal = x.SpendVector.SysName=="INC" ? currentTotal + x.Sum : currentTotal - x.Sum;
                      return new SpendChartViewModel() { Date = x.Date, Sum = x.Sum, CumulativeTotal = currentTotal };
                  });

            return data;
        }

        //public static IEnumerable<SpendStatItem> GetVectorMonthlyReport(string clientId, int year, int month)
        //{		
        //    var list = new List<SpendStatItem>();		
        //    try		
        //    {		
        //        BalanceContext db = new BalanceContext();		

        //        var report = from s in db.Spends
        //            where s.ClientId.Equals(clientId) && s.Date.Year == year && s.Date.Month == month
        //            group s by new
        //            {		
        //                s.SpendVector		
        //            }		
        //            into gs
        //            select new		
        //            {		
        //                SpendVector = gs.Key.SpendVector.Name,		
        //                Sum = gs.Sum(x => x.Sum)		
        //            };		
        //        list = report.AsEnumerable().Select(x => new SpendStatItem		
        //        {		
        //            SpendVectorName = x.SpendVector,		
        //            Sum = x.Sum		
        //        }).OrderBy(x => x.SpendVectorName).ThenByDescending(x => x.Sum).ToList();		
        //    }		
        //    catch { }		

        //    return list;		
        //}		

        //public static IEnumerable<SpendStatItem> GetVectorYearlyReport(string clientId, int year)
        //{		
        //    var list = new List<SpendStatItem>();		

        //    BalanceContext db = new BalanceContext();		

        //    var report = from s in db.Spends
        //                 where s.ClientId.Equals(clientId) && s.Date.Year == year
        //                 group s by new
        //                 {		
        //                     s.SpendVector		
        //                 }		
        //        into gs
        //                 select new		
        //                 {		
        //                     SpendVector = gs.Key.SpendVector.Name,		
        //                     Sum = gs.Sum(x => x.Sum)		
        //                 };		
        //    list = report.AsEnumerable().Select(x => new SpendStatItem		
        //    {		
        //        SpendVectorName = x.SpendVector,		
        //        Sum = x.Sum		
        //    }).OrderBy(x => x.SpendVectorName).ThenByDescending(x => x.Sum).ToList();		


        //    return list;		
        //}
    }
}
