using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public void SpendDelete(int id)
        {
            var spend = Uow.Spends.GetById(id);
            spend.DeleteDate = DateTime.Now;
            spend.Enabled = false;
            Uow.Spends.Update(spend);
            Uow.Commit();
        }

        public IEnumerable<Spend> GetSpendList(out int totalCount, int? page = null, int? psize = null, DateTime? dateStart = null, DateTime? dateEnd = null, int? categoryId = null, int? vectorId = null)
        {
            var list = Uow.Spends.GetAllWithTotalCount(out totalCount, page, psize, x => x.UserSid == UserSid && x.Enabled
            && (!dateStart.HasValue || (dateStart.HasValue && DbFunctions.TruncateTime(x.Date) >= DbFunctions.TruncateTime(dateStart)))
            && (!dateEnd.HasValue || (dateEnd.HasValue && DbFunctions.TruncateTime(x.Date) <= DbFunctions.TruncateTime(dateEnd)))
            && (!categoryId.HasValue || (categoryId.HasValue && x.CategoryId == categoryId))
            && (!vectorId.HasValue || (vectorId.HasValue && x.VectorId == vectorId))
            , x => x.OrderByDescending(y => y.Date).ThenByDescending(y => y.Id), x => x.SpendCategory, x => x.SpendVector);
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

        public IEnumerable<SpendStatViewModel> GetMonthlyCategoryReport(int year, int month, string vectorSysName = null)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return GetPeriodReport(startDate, endDate, vectorSysName);
        }

        //public IEnumerable<SpendStatViewModel> GetQuarterCategoryReport(int endYear, int endMonth, string vectorSysName = null)
        public IEnumerable<SpendStatViewModel> GetQuarterCategoryReport(int year, int quarter, string vectorSysName = null)
        {
            //var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            //var startMonth = endDate.AddMonths(-3).Month;
            //var startYear = endDate.AddMonths(-3).Year;
            //var startDate = new DateTime(startYear, startMonth, 1);
            int month = quarter * 3;
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            var startMonth = endDate.AddMonths(-2).Month;
            var startYear = endDate.AddMonths(-2).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetPeriodReport(startDate, endDate, vectorSysName);
        }

        //public IEnumerable<SpendStatViewModel> GetYearlyCategoryReport(int endYear, int endMonth, string vectorSysName = null)
        public IEnumerable<SpendStatViewModel> GetYearlyCategoryReport(int year, string vectorSysName = null)
        {
            var endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            var startMonth = endDate.AddMonths(-11).Month;
            var startYear = endDate.AddMonths(-11).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetPeriodReport(startDate, endDate, vectorSysName);
        }

        //public IEnumerable<SpendStatViewModel> Get5YearlyCategoryReport(int endYear, int endMonth, string vectorSysName = null)
        public IEnumerable<SpendStatViewModel> Get5YearlyCategoryReport(int year, string vectorSysName = null)
        {
            var endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            var startMonth = endDate.AddMonths(-59).Month;
            var startYear = endDate.AddMonths(-59).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetPeriodReport(startDate, endDate, vectorSysName);
        }

        public IEnumerable<SpendStatViewModel> GetAllTimeCategoryReport(string vectorSysName = null)
        {
            int endYear = DateTime.Now.Year + 100;
            int endMonth = DateTime.Now.Month;
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startDate = new DateTime(1900, 1, 1);

            return GetPeriodReport(startDate, endDate, vectorSysName);
        }

        public IEnumerable<SpendStatViewModel> GetPeriodReport(DateTime startDate, DateTime endDate, string vectorSysName = null)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньшще даты начала!");

            var list = new List<SpendStatViewModel>();

            var sDate = startDate.Date;
            var eDate = endDate.Date;

            var report = from s in Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.Date >= sDate && x.Date <= eDate
                         && (String.IsNullOrEmpty(vectorSysName) || (!String.IsNullOrEmpty(vectorSysName) && x.SpendVector.SysName == vectorSysName))
                         )
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
                             Sum = gs.Sum(x => x.Sum),
                             SpendCategoryId = gs.Key.SpendCategory.CategoryId,
                             SpendVectorId = gs.Key.SpendVector.VectorId
                         };
            list = report.AsEnumerable().Select(x => new SpendStatViewModel
            {
                SpendVectorIconName = x.SpendVectorIconName,
                SpendVectorBgColorClass = x.SpendVectorBgColorClass,
                SpendVectorSysName = x.SpendVectorSysName,
                SpendVectorName = x.SpendVector,
                SpendCategoryName = x.SpendCategory,
                Sum = x.Sum,
                SpendCategoryId = x.SpendCategoryId,
                SpendVectorId = x.SpendVectorId
            }).OrderBy(x => x.SpendVectorName).ThenByDescending(x => x.Sum).ToList();


            return list;
        }

        public IEnumerable<SpendChartViewModel> GetSpendListMonthlyChartData(int year, int month, string vectorSysName = null)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return GetIncExpChartData(vectorSysName, startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetSpendListQuarterChartData(int year, int quarter, string vectorSysName = null)
        {
            int month = quarter * 3;
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            var startMonth = endDate.AddMonths(-2).Month;
            var startYear = endDate.AddMonths(-2).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetIncExpChartData(vectorSysName, startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetSpendListYearlyChartData(int year, string vectorSysName = null)
        {
            var endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            var startMonth = endDate.AddMonths(-11).Month;
            var startYear = endDate.AddMonths(-11).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetIncExpChartData(vectorSysName, startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetSpendList5YearlyChartData(int year, string vectorSysName = null)
        {
            var endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            var startMonth = endDate.AddMonths(-59).Month;
            var startYear = endDate.AddMonths(-59).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetIncExpChartData(vectorSysName, startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetSpendListAllTimeChartData(string vectorSysName = null)
        {
            int endYear = DateTime.Now.Year + 100;
            int endMonth = DateTime.Now.Month;
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startDate = new DateTime(1900, 1, 1);

            return GetIncExpChartData(vectorSysName, startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetExpenseMonthlyChartData(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return GetIncExpChartData("EXP", startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> GetIncExpChartData(string vectorSysName, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньше даты начала!");

            var sDate = startDate.Date;
            var eDate = endDate.Date;

            var data =
                Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid
                && (String.IsNullOrEmpty(vectorSysName) || (!String.IsNullOrEmpty(vectorSysName) && x.SpendVector.SysName == vectorSysName))
                && x.Date >= sDate && x.Date <= eDate,
                    x => x.OrderBy(y => y.Date)).Select(x => new SpendChartViewModel() { Date = x.Date, Sum = (x.SpendVector.SysName == "EXP" ? -x.Sum : x.Sum) });

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
            var startMonth = endDate.AddMonths(-2).Month;
            var startYear = endDate.AddMonths(-2).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetCumulativeTotalChartData(startDate, endDate);
        }

        //public IEnumerable<SpendChartViewModel> GetYearlyCumulativeTotalChartData(int endYear, int endMonth)
        public IEnumerable<SpendChartViewModel> GetYearlyCumulativeTotalChartData(int year)
        {
            var endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            var startMonth = endDate.AddMonths(-11).Month;
            var startYear = endDate.AddMonths(-11).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetCumulativeTotalChartData(startDate, endDate);
        }

        public IEnumerable<SpendChartViewModel> Get5YearlyCumulativeTotalChartData(int endYear, int endMonth)
        {
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startMonth = endDate.AddMonths(-59).Month;
            var startYear = endDate.AddMonths(-59).Year;
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

        public IEnumerable<SpendChartViewModel> GetCumulativeTotalChartData(DateTime startDate, DateTime endDate, bool calcCumulativeTotal = false)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньшще даты начала!");

            var sDate = startDate.Date;
            var eDate = endDate.Date;


            double currentTotal = 0;
            if (calcCumulativeTotal)
            {
                currentTotal = Uow.Spends.GetAllQuery(
                  x =>
                      x.Enabled && x.UserSid == UserSid &&
                      DbFunctions.TruncateTime(x.Date) < DbFunctions.TruncateTime(sDate))
                  .Sum(x => x.SpendVector.SysName == "INC" ? +x.Sum : -x.Sum);
            }

            var data = Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && DbFunctions.TruncateTime(x.Date) >= DbFunctions.TruncateTime(sDate) && DbFunctions.TruncateTime(x.Date) <= DbFunctions.TruncateTime(eDate), x => x.OrderBy((y => y.Date)), x => x.SpendVector).AsEnumerable()
                  .Select(x =>
                  {
                      currentTotal = x.SpendVector.SysName == "INC" ? currentTotal + x.Sum : currentTotal - x.Sum;
                      return new SpendChartViewModel() { Date = x.Date, Sum = x.Sum, CumulativeTotal = currentTotal };
                  });

            return data;
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<SpendChartViewModel>>> GetCumulativeCategoryChartData(DateTime startDate, DateTime endDate, string vectorSysName, bool calcCumulativeTotal = false)
        {
            var list = new List<KeyValuePair<string, IEnumerable<SpendChartViewModel>>>();

            var cats = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid);

            foreach (var cat in cats)
            {
                var item = new KeyValuePair<string, IEnumerable<SpendChartViewModel>>(cat.Name, GetCumulativeByCategoryChartData(startDate, endDate, cat.CategoryId, vectorSysName, calcCumulativeTotal));
                list.Add(item);
            }

            return list;
        }

        public IEnumerable<SpendChartViewModel> GetCumulativeByCategoryChartData(DateTime startDate, DateTime endDate, int categoryId, string vectorSysName, bool calcCumulativeTotal = false)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньшще даты начала!");

            var sDate = startDate.Date;
            var eDate = endDate.Date;


            double currentTotal = 0;
            if (calcCumulativeTotal)
            {
                currentTotal = Uow.Spends.GetAllQuery(
                  x =>
                      x.Enabled && x.UserSid == UserSid && x.CategoryId== categoryId && 
                      (String.IsNullOrEmpty(vectorSysName) || (!String.IsNullOrEmpty(vectorSysName) && x.SpendVector.SysName==vectorSysName)) &&
                      DbFunctions.TruncateTime(x.Date) < DbFunctions.TruncateTime(sDate))
                  .Sum(x => x.SpendVector.SysName == "INC" ? +x.Sum : -x.Sum);
            }

            var data = Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.CategoryId == categoryId &&
                      (String.IsNullOrEmpty(vectorSysName) || (!String.IsNullOrEmpty(vectorSysName) && x.SpendVector.SysName == vectorSysName))
                    && DbFunctions.TruncateTime(x.Date) >= DbFunctions.TruncateTime(sDate) && DbFunctions.TruncateTime(x.Date) <= DbFunctions.TruncateTime(eDate), x => x.OrderBy((y => y.Date)), x => x.SpendVector).AsEnumerable()
                  .Select(x =>
                  {
                      currentTotal = x.SpendVector.SysName == "INC" ? currentTotal + x.Sum : currentTotal - x.Sum;
                      return new SpendChartViewModel() { Date = x.Date, Sum = x.Sum, CumulativeTotal = currentTotal };
                  });

            return data;
        }

        public IEnumerable<SpendChartViewModel> GetCumulativeByMonthesByCategoryChartData(DateTime startDate, DateTime endDate, int categoryId, string vectorSysName, bool calcCumulativeTotal = false)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньшще даты начала!");

            var sDate = startDate.Date;
            var eDate = endDate.Date;


            double currentTotal = 0;
            if (calcCumulativeTotal)
            {
                currentTotal = Uow.Spends.GetAllQuery(
                  x =>
                      x.Enabled && x.UserSid == UserSid && x.CategoryId == categoryId &&
                      (String.IsNullOrEmpty(vectorSysName) || (!String.IsNullOrEmpty(vectorSysName) && x.SpendVector.SysName == vectorSysName)) &&
                      DbFunctions.TruncateTime(x.Date) < DbFunctions.TruncateTime(sDate))
                  .Sum(x => x.SpendVector.SysName == "INC" ? +x.Sum : -x.Sum);
            }

            var data =
                (from m in
                    Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.CategoryId == categoryId &&
                                                (String.IsNullOrEmpty(vectorSysName) ||
                                                 (!String.IsNullOrEmpty(vectorSysName) &&
                                                  x.SpendVector.SysName == vectorSysName))
                                                && DbFunctions.TruncateTime(x.Date) >= DbFunctions.TruncateTime(sDate) &&
                                                DbFunctions.TruncateTime(x.Date) <= DbFunctions.TruncateTime(eDate),
                        x => x.OrderBy((y => y.Date)), x => x.SpendVector)
                    group m by new
                    {
                        Year = m.Date.Year,
                        Month = m.Date.Month
                    }
                    into g
                    select new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Total = g.Sum(x => x.SpendVector.SysName == "INC" ? +x.Sum : -x.Sum)
                    }).AsEnumerable().Select(x =>
                    {
                        return new SpendChartViewModel() { Date = new DateTime(x.Year, x.Month, 1), Sum = x.Total };
                    });
        //               .AsEnumerable()
        //.Select(g => new {
        //    Period = g.Year + "-" + g.Month,
        //    Total = g.Total,
        //});
            //.Select(x =>
            //{
            //    currentTotal = x.SpendVector.SysName == "INC" ? currentTotal + x.Sum : currentTotal - x.Sum;
            //    return new SpendChartViewModel() { Date = x.Date, Sum = x.Sum, CumulativeTotal = currentTotal };
            //});

            return data;
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<SpendChartViewModel>>> GetYearlyCumulativeCategoryChartData(int year, string vectorSysName)
        {
            var endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            var startMonth = endDate.AddMonths(-11).Month;
            var startYear = endDate.AddMonths(-11).Year;
            var startDate = new DateTime(startYear, startMonth, 1);
            
            return GetCumulativeCategoryChartData(startDate, endDate, vectorSysName);
        }

        public IEnumerable<SpendCategory> GetCategoryList()
        {
            var list = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid, x => x.OrderBy(y => y.OrderNum).ThenBy(y => y.Name), x => x.Spend);

            return list;
        }

        public void SpendCategoryDelete(int id)
        {
            var cat = Uow.SpendCategories.GetOne(x => x.CategoryId == id, x => x.Spend);
            if (cat.Spend.Any(x => x.Enabled)) throw new ArgumentException("Нельзя удалить категорию пока она содержит записи трат! Удалите или переместите записи в другую категорию и посторите попытку!");
            cat.Enabled = false;
            Uow.Commit();
        }

        public SpendCategory SpendCategoryGet(int id)
        {
            var model = Uow.SpendCategories.GetOne(x => x.CategoryId == id);
            return model;
        }

        public void SpendCategoryCreate(SpendCategory model)
        {
            if (String.IsNullOrEmpty(model.Name)) throw new ArgumentException("Необходимо заполнить название категории!");
            var cat = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid && x.Name == model.Name);
            if (cat.Any()) throw new ArgumentException("Такое название категории уже существует!");
            model.Enabled = true;
            model.UserSid = UserSid;
            model.OrderNum = 500;
            Uow.SpendCategories.Insert(model);
            Uow.Commit();
        }

        public void SpendCategoryEdit(SpendCategory model)
        {
            if (String.IsNullOrEmpty(model.Name)) throw new ArgumentException("Необходимо заполнить название категории!");
            var catExists = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid && x.Name == model.Name);
            if (catExists.Any()) throw new ArgumentException("Такое название категории уже существует!");
            var cat = Uow.SpendCategories.GetOne(x => x.CategoryId == model.CategoryId);
            cat.Name = model.Name;
            Uow.SpendCategories.Update(cat);
            Uow.Commit();
        }

        public void SpendCategoryOrderUp(int id)
        {
            var cat = Uow.SpendCategories.GetOne(x => x.CategoryId == id);

            var cats = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid && x.CategoryId != id && x.OrderNum < cat.OrderNum);

            if (cats.Any())
            {
                var upperCat = cats.OrderByDescending(x => x.OrderNum).First();
                int oldOrder = cat.OrderNum;
                cat.OrderNum = upperCat.OrderNum;
                upperCat.OrderNum = oldOrder;
                Uow.SpendCategories.Update(upperCat);
            }
            else
            {
                cat.OrderNum--;
            }

            Uow.SpendCategories.Update(cat);
            Uow.Commit();
        }

        public void SpendCategoryOrderDown(int id)
        {
            var cat = Uow.SpendCategories.GetOne(x => x.CategoryId == id);

            var cats = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid && x.CategoryId != id && x.OrderNum > cat.OrderNum);

            if (cats.Any())
            {
                var upperCat = cats.OrderBy(x => x.OrderNum).First();
                int oldOrder = cat.OrderNum;
                cat.OrderNum = upperCat.OrderNum;
                upperCat.OrderNum = oldOrder;
                Uow.SpendCategories.Update(upperCat);
            }
            else
            {
                cat.OrderNum++;
            }

            Uow.SpendCategories.Update(cat);
            Uow.Commit();
        }

        public void SpendCategoryMerge(int[] catIds, int catNameId)
        {
            var list = Uow.Spends.GetAll(x => x.Enabled && x.UserSid == UserSid && catIds.Contains(x.CategoryId));
            foreach (var item in list)
            {
                item.CategoryId = catNameId;
            }
            var oldCats = Uow.SpendCategories.GetAll(
                x => x.Enabled && x.UserSid == UserSid && catIds.Contains(x.CategoryId) && x.CategoryId != catNameId);
            foreach (var cat in oldCats)
            {
                cat.Enabled = false;
            }
            Uow.Commit();
        }

        public IEnumerable<SpendBills> SpendBillGetList()
        {
            var model = Uow.SpendBills.GetAll(x => x.UserSid == UserSid, x=>x.OrderBy(y=>y.OrderNum).ThenBy(y=>y.Name));
            return model;
        }

        public SpendBills SpendBillGet(int id)
        {
            var model = Uow.SpendBills.GetOne(x => x.Id == id);
            return model;
        }

        public void SpendBillCreate(SpendBills model)
        {
            if (String.IsNullOrEmpty(model.Name)) throw new ArgumentException("Необходимо заполнить название счета!");
            var bill = Uow.SpendBills.GetAll(x => x.Enabled && x.UserSid == UserSid && x.Name == model.Name);
            if (bill.Any()) throw new ArgumentException("Такое название счета уже существует!");
            model.Enabled = true;
            model.UserSid = UserSid;
            model.OrderNum = 500;
            Uow.SpendBills.Insert(model);
            Uow.Commit();
        }

        public IEnumerable<SpendBillTypes> SpendBillTypeGetList()
        {
            var list = Uow.SpendBillTypes.GetAll(null, x=>x.OrderBy(y=>y.OrderNum).ThenBy(y=>y.Name));
            return list;
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
