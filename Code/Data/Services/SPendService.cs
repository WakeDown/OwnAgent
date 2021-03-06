﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public void CreateDefaultBills()
        {
            if (!Uow.SpendBills.GetAll(x => x.Enabled && x.UserSid == UserSid).Any())
            {
                var bill = new SpendBills();
                bill.Name = "Мой кошелек";
                bill.TypeId = Uow.SpendBillTypes.GetOne(x => x.SysName == "STANDART").Id;
                SpendBillCreate(bill);

                bill = new SpendBills();
                bill.Name = "Коплю на отпуск";
                bill.TypeId = Uow.SpendBillTypes.GetOne(x => x.SysName == "SAVING").Id;
                SpendBillCreate(bill);

                bill = new SpendBills();
                bill.Name = "Коплю на квартиру";
                bill.TypeId = Uow.SpendBillTypes.GetOne(x => x.SysName == "SAVING").Id;
                SpendBillCreate(bill);

                bill = new SpendBills();
                bill.Name = "Кредит за машину";
                bill.TypeId = Uow.SpendBillTypes.GetOne(x => x.SysName == "CREDIT").Id;
                SpendBillCreate(bill);
            }
        }

        public void CreateDefaultCategories()
        {
            if (!Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid).Any())
            {
                var cat = new SpendCategory();
                cat.Enabled = true;
                cat.UserSid = UserSid;
                cat.Name = "Еда/Бытовые товары";
                cat.OrderNum = 10;
                cat.IsSystem = false;
                Uow.SpendCategories.Insert(cat);

                cat.Name = "Бытовые услуги";
                cat.OrderNum = 20;
                cat.IsSystem = false;
                Uow.SpendCategories.Insert(cat);

                cat.Name = "Ресторан";
                cat.OrderNum = 30;
                cat.IsSystem = false;
                Uow.SpendCategories.Insert(cat);

                cat.Name = "Транспорт/Бензин";
                cat.OrderNum = 40;
                cat.IsSystem = false;
                Uow.SpendCategories.Insert(cat);

                cat.Name = "Одежда";
                cat.OrderNum = 50;
                cat.IsSystem = false;
                Uow.SpendCategories.Insert(cat);

                cat.Name = "Зарплата";
                cat.OrderNum = 60;
                cat.IsSystem = false;
                Uow.SpendCategories.Insert(cat);

                cat.Name = "Подработка";
                cat.OrderNum = 70;
                cat.IsSystem = false;
                Uow.SpendCategories.Insert(cat);

                cat.Name = "Другое";
                cat.OrderNum = 80;
                cat.IsSystem = false;
                Uow.SpendCategories.Insert(cat);

                cat.Name = "Перевод";
                cat.OrderNum = 90;
                cat.IsSystem = true;
                cat.SysName = "TRANSFER";
                Uow.SpendCategories.Insert(cat);
                Uow.Commit();
            }else if (
                !Uow.SpendCategories.GetAll(
                    x => x.Enabled && x.UserSid == UserSid && x.IsSystem && x.SysName == "TRANSFER").Any())
            {
                var cat = new SpendCategory();
                cat.Enabled = true;
                cat.UserSid = UserSid;
                cat.Name = "Перевод";
                cat.OrderNum = 90;
                cat.IsSystem = true;
                cat.SysName = "TRANSFER";
                Uow.SpendCategories.Insert(cat);
                Uow.Commit();
            }
        }

        public IEnumerable<KeyValuePair<int, string>> GetCategorySelectionList4NewSpend()
        {
            var list = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid && !x.IsSystem, x => x.OrderBy(y => y.OrderNum).ThenBy(y => y.Name)).Take(3);
            var result = new List<KeyValuePair<int, string>>();
            foreach (var category in list)
            {
                result.Add(new KeyValuePair<int, string>(category.CategoryId, category.Name));
            }

            var listOrdered = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid && !x.IsSystem, x => x.OrderBy(y => y.Name));
            foreach (var category in listOrdered)
            {
                result.Add(new KeyValuePair<int, string>(category.CategoryId, category.Name));
            }

            return result;
        }

        public IEnumerable<KeyValuePair<int, string>> GetCategorySelectionList()
        {
            var list = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid && !x.IsSystem, x => x.OrderBy(y => y.OrderNum).ThenBy(y => y.Name));
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

        public void CreateTransfer(SpendTransfer model)
        {
            if (model.BillToId==model.BillFromId) throw new ArgumentException("Необходимо указать разные счета!");

            model.CreateDate = DateTime.Now;
            model.Enabled = true;
            model.UserSid = UserSid;
            Uow.SpendTransfer.Insert(model);
            Uow.Commit();

            var catId = Uow.SpendCategories.GetAll(x => x.UserSid==UserSid && x.IsSystem && x.SysName == "TRANSFER").Single().CategoryId;

            Spend spendExp = new Spend();
            spendExp.Sum = model.Sum;
            spendExp.BillId = model.BillFromId;
            spendExp.Comment = model.Comment;
            spendExp.CategoryId = catId;
            spendExp.Date = model.Date;
            spendExp.TransferId = model.Id;
            CreateExpense(spendExp);

            Spend spendInc = new Spend();
            spendInc.Sum = model.Sum;
            spendInc.BillId = model.BillToId;
            spendInc.Comment = model.Comment;
            spendInc.CategoryId = catId;
            spendInc.Date = model.Date;
            spendExp.TransferId = model.Id;
            CreateIncome(spendInc);
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

        public IEnumerable<Spend> GetSpendList(out int totalCount, int? page = null, int? psize = null, DateTime? dateStart = null, DateTime? dateEnd = null, int? categoryId = null, int? vectorId = null, int? billId = null)
        {
            var list = Uow.Spends.GetAllWithTotalCount(out totalCount, page, psize, x => x.UserSid == UserSid && x.Enabled
            && (!dateStart.HasValue || (dateStart.HasValue && DbFunctions.TruncateTime(x.Date) >= DbFunctions.TruncateTime(dateStart)))
            && (!dateEnd.HasValue || (dateEnd.HasValue && DbFunctions.TruncateTime(x.Date) <= DbFunctions.TruncateTime(dateEnd)))
            && (!categoryId.HasValue || (categoryId.HasValue && x.CategoryId == categoryId))
            && (!vectorId.HasValue || (vectorId.HasValue && x.VectorId == vectorId))
            && (!billId.HasValue || (billId.HasValue && x.BillId == billId))
            , x => x.OrderByDescending(y => y.Date).ThenByDescending(y => y.Id), x => x.SpendCategory, x => x.SpendVector, x => x.SpendBills);
            return list;
        }

        public IEnumerable<Spend> GetTop(int topRows)
        {
            var limitDate = DateTime.Now.AddMonths(-3).Date;
            var list = (from s in Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.TransferId == null && x.Date >= limitDate)
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

        public IEnumerable<SpendStatBillViewModel> GetMonthlyBilleport(int year, int month, string vectorSysName = null, int? billId = null)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return GetPeriodBillReport(startDate, endDate, vectorSysName, billId);
        }

        public IEnumerable<SpendStatBillViewModel> GetQuarterBillReport(int year, int quarter, string vectorSysName = null)
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

            return GetPeriodBillReport(startDate, endDate, vectorSysName);
        }

        //public IEnumerable<SpendStatViewModel> GetYearlyCategoryReport(int endYear, int endMonth, string vectorSysName = null)
        public IEnumerable<SpendStatBillViewModel> GetYearlyBillReport(int year, string vectorSysName = null)
        {
            var endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            var startMonth = endDate.AddMonths(-11).Month;
            var startYear = endDate.AddMonths(-11).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetPeriodBillReport(startDate, endDate, vectorSysName);
        }

        //public IEnumerable<SpendStatViewModel> Get5YearlyCategoryReport(int endYear, int endMonth, string vectorSysName = null)
        public IEnumerable<SpendStatBillViewModel> Get5YearlyBillReport(int year, string vectorSysName = null)
        {
            var endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            var startMonth = endDate.AddMonths(-59).Month;
            var startYear = endDate.AddMonths(-59).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetPeriodBillReport(startDate, endDate, vectorSysName);
        }

        public IEnumerable<SpendStatBillViewModel> GetAllTimeBillReport(string vectorSysName = null)
        {
            int endYear = DateTime.Now.Year + 100;
            int endMonth = DateTime.Now.Month;
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startDate = new DateTime(1900, 1, 1);

            return GetPeriodBillReport(startDate, endDate, vectorSysName);
        }

        public IEnumerable<SpendStatBillViewModel> GetPeriodBillReport(DateTime startDate, DateTime endDate, string vectorSysName = null, int? billId = null)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньше даты начала!");


            var sDate = startDate.Date;
            var eDate = endDate.Date;

            var list =
                Uow.SpendBills.GetAll(x => x.Enabled && x.UserSid == UserSid
                && (!billId.HasValue || (billId.HasValue && x.Id == billId))
                ,
                    x => x.OrderBy(y => y.OrderNum).ThenBy(y => y.Name), x => x.SpendBillTypes)
                    .Select(x => new SpendStatBillViewModel
                    {
                        SpendBillName = x.Name,
                        SpendBillId = x.Id,
                        BillTypeName = x.SpendBillTypes.Name,
                        BillTypeIconName = x.SpendBillTypes.IconName
                    }).ToList();

            foreach (var item in list)
            {
                var report = from s in Uow.Spends.GetAllQuery(x => x.Enabled && x.BillId == item.SpendBillId && x.UserSid == UserSid && x.Date >= sDate && x.Date <= eDate
                         && (String.IsNullOrEmpty(vectorSysName) || (!String.IsNullOrEmpty(vectorSysName) && x.SpendVector.SysName == vectorSysName))
                         )
                             group s by new
                             {
                                 s.SpendBills
                             }
                         into gs
                             select new
                             {
                                 BillTypeIconName = gs.Key.SpendBills.SpendBillTypes.IconName,
                                 Sum = gs.Sum(x => x.SpendVector.SysName == "INC" ? x.Sum : -x.Sum),
                                 IncSum = gs.Sum(x => x.SpendVector.SysName == "INC" ? x.Sum : 0),
                                 ExpSum = gs.Sum(x => x.SpendVector.SysName == "EXP" ? x.Sum : 0),
                             };
                if (report.Any())
                {
                    var rItem = report.First();
                    item.BillTypeIconName = rItem.BillTypeIconName;
                    item.Sum = rItem.Sum;
                    item.IncSum = rItem.IncSum;
                    item.ExpSum = rItem.ExpSum;
                }

            }

            return list;
        }

        public IEnumerable<SpendStatCategoryViewModel> GetMonthlyCategoryStatReport(int year, int month, int? categoryId = null)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return GetPeriodCategoryStatReport(startDate, endDate, categoryId);
        }

        public IEnumerable<SpendStatCategoryViewModel> GetPeriodCategoryStatReport(DateTime startDate, DateTime endDate, int? categoryId = null)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньшще даты начала!");


            var sDate = startDate.Date;
            var eDate = endDate.Date;

            var list = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid
                                                       &&
                                                       (!categoryId.HasValue ||
                                                        (categoryId.HasValue && x.CategoryId == categoryId)),
                                                        x => x.OrderBy(y => y.Name)
                ).Select(x => new SpendStatCategoryViewModel
                {
                    SpendCategoryName = x.Name,
                    SpendCategoryId = x.CategoryId,
                }).ToList();

            foreach (var cat in list)
            {
                var report = from s in Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.TransferId == null && x.Date >= sDate && x.Date <= eDate
                         && (!categoryId.HasValue || (categoryId.HasValue && x.CategoryId == categoryId))
                         )
                             group s by new
                             {
                                 s.SpendCategory
                             }
                         into gs
                             select new
                             {
                                 IncSum = gs.Sum(x => x.SpendVector.SysName == "INC" ? x.Sum : 0),
                                 ExpSum = gs.Sum(x => x.SpendVector.SysName == "EXP" ? x.Sum : 0)
                             };
                if (report.Any())
                {
                    var rItem = report.First();
                    cat.IncSum = rItem.IncSum;
                    cat.ExpSum = rItem.ExpSum;
                }
            }

            return list;
        }

        public IEnumerable<SpendStatViewModel> GetMonthlyCategoryReport(int year, int month, string vectorSysName = null, int? categoryId = null, int? billId = null)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return GetPeriodCategoryReport(startDate, endDate, vectorSysName, categoryId, billId);
        }

        //public IEnumerable<SpendStatViewModel> GetQuarterCategoryReport(int endYear, int endMonth, string vectorSysName = null)
        public IEnumerable<SpendStatViewModel> GetQuarterCategoryReport(int year, int quarter, string vectorSysName = null, int? categoryId = null, int? billId = null)
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

            return GetPeriodCategoryReport(startDate, endDate, vectorSysName, categoryId, billId);
        }

        //public IEnumerable<SpendStatViewModel> GetYearlyCategoryReport(int endYear, int endMonth, string vectorSysName = null)
        public IEnumerable<SpendStatViewModel> GetYearlyCategoryReport(int year, string vectorSysName = null, int? categoryId = null, int? billId = null)
        {
            var endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            var startMonth = endDate.AddMonths(-11).Month;
            var startYear = endDate.AddMonths(-11).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetPeriodCategoryReport(startDate, endDate, vectorSysName, categoryId, billId);
        }

        //public IEnumerable<SpendStatViewModel> Get5YearlyCategoryReport(int endYear, int endMonth, string vectorSysName = null)
        public IEnumerable<SpendStatViewModel> Get5YearlyCategoryReport(int year, string vectorSysName = null, int? categoryId = null, int? billId = null)
        {
            var endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            var startMonth = endDate.AddMonths(-59).Month;
            var startYear = endDate.AddMonths(-59).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetPeriodCategoryReport(startDate, endDate, vectorSysName, categoryId, billId);
        }

        public IEnumerable<SpendStatViewModel> GetAllTimeCategoryReport(string vectorSysName = null, int? categoryId = null, int? billId = null)
        {
            int endYear = DateTime.Now.Year + 100;
            int endMonth = DateTime.Now.Month;
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startDate = new DateTime(1900, 1, 1);

            return GetPeriodCategoryReport(startDate, endDate, vectorSysName, categoryId, billId);
        }

        public IEnumerable<SpendStatViewModel> GetPeriodCategoryReport(DateTime startDate, DateTime endDate, string vectorSysName = null, int? categoryId = null, int? billId = null)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньшще даты начала!");

            var list = new List<SpendStatViewModel>();

            var sDate = startDate.Date;
            var eDate = endDate.Date;

            var report = from s in Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && !x.SpendCategory.IsSystem && x.TransferId == null && x.Date >= sDate && x.Date <= eDate
                         && (String.IsNullOrEmpty(vectorSysName) || (!String.IsNullOrEmpty(vectorSysName) && x.SpendVector.SysName == vectorSysName))
                         && (!categoryId.HasValue || (categoryId.HasValue && x.CategoryId == categoryId))
                         && (!billId.HasValue || (billId.HasValue && x.BillId == billId))
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
                 && x.TransferId == null
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
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньше даты начала!");

            var sDate = startDate.Date;
            var eDate = endDate.Date;

            var list = from s in Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.TransferId == null && x.Date >= sDate && x.Date <= eDate)
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

        public IEnumerable<SpendChartViewModel> GetMonthlyCumulativeTotalChartData(int year, int month, int? categoryId = null, int? billId = null)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return GetCumulativeTotalChartData(startDate, endDate, categoryId: categoryId, billId: billId);
        }

        public IEnumerable<SpendChartViewModel> GetQuarterCumulativeTotalChartData(int endYear, int endMonth, int? categoryId = null, int? billId = null)
        {
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startMonth = endDate.AddMonths(-2).Month;
            var startYear = endDate.AddMonths(-2).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetCumulativeTotalChartData(startDate, endDate, categoryId: categoryId, billId: billId);
        }

        //public IEnumerable<SpendChartViewModel> GetYearlyCumulativeTotalChartData(int endYear, int endMonth)
        public IEnumerable<SpendChartViewModel> GetYearlyCumulativeTotalChartData(int year, int? categoryId = null, int? billId = null)
        {
            var endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            var startMonth = endDate.AddMonths(-11).Month;
            var startYear = endDate.AddMonths(-11).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetCumulativeTotalChartData(startDate, endDate, categoryId: categoryId, billId: billId);
        }

        public IEnumerable<SpendChartViewModel> Get5YearlyCumulativeTotalChartData(int endYear, int endMonth, int? categoryId = null, int? billId = null)
        {
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startMonth = endDate.AddMonths(-59).Month;
            var startYear = endDate.AddMonths(-59).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetCumulativeTotalChartData(startDate, endDate, categoryId: categoryId, billId: billId);
        }

        public IEnumerable<SpendChartViewModel> GetAllTimeCumulativeTotalChartData(int? categoryId = null, int? billId = null)
        {
            int endYear = DateTime.Now.Year + 100;
            int endMonth = DateTime.Now.Month;
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var startDate = new DateTime(1900, 1, 1);

            return GetCumulativeTotalChartData(startDate, endDate, categoryId: categoryId, billId: billId);
        }

        public IEnumerable<SpendChartViewModel> GetCumulativeTotalChartData(DateTime startDate, DateTime endDate, bool calcCumulativeTotal = false, int? categoryId = null, int? billId = null)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньшще даты начала!");

            var sDate = startDate.Date;
            var eDate = endDate.Date;


            double currentTotal = 0;
            if (calcCumulativeTotal)
            {
                currentTotal = Uow.Spends.GetAllQuery(
                  x =>
                      x.Enabled && x.UserSid == UserSid
                       && x.TransferId == null
                      && (!categoryId.HasValue || (categoryId.HasValue && x.CategoryId == categoryId))
                      && (!billId.HasValue || (billId.HasValue && x.BillId == billId))
                      && DbFunctions.TruncateTime(x.Date) < DbFunctions.TruncateTime(sDate))
                  .Sum(x => x.SpendVector.SysName == "INC" ? +x.Sum : -x.Sum);
            }

            var data = Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid
             && x.TransferId == null
            && (!categoryId.HasValue || (categoryId.HasValue && x.CategoryId == categoryId))
                      && (!billId.HasValue || (billId.HasValue && x.BillId == billId))
            && DbFunctions.TruncateTime(x.Date) >= DbFunctions.TruncateTime(sDate)
            && DbFunctions.TruncateTime(x.Date) <= DbFunctions.TruncateTime(eDate)
            , x => x.OrderBy((y => y.Date)), x => x.SpendVector).AsEnumerable()
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
                      x.Enabled && x.UserSid == UserSid && x.TransferId == null && x.CategoryId == categoryId &&
                      (String.IsNullOrEmpty(vectorSysName) || (!String.IsNullOrEmpty(vectorSysName) && x.SpendVector.SysName == vectorSysName)) &&
                      DbFunctions.TruncateTime(x.Date) < DbFunctions.TruncateTime(sDate))
                  .Sum(x => x.SpendVector.SysName == "INC" ? +x.Sum : -x.Sum);
            }

            var data = Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.TransferId == null && x.CategoryId == categoryId &&
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
                      x.Enabled && x.UserSid == UserSid && x.TransferId == null && x.CategoryId == categoryId &&
                      (String.IsNullOrEmpty(vectorSysName) || (!String.IsNullOrEmpty(vectorSysName) && x.SpendVector.SysName == vectorSysName)) &&
                      DbFunctions.TruncateTime(x.Date) < DbFunctions.TruncateTime(sDate))
                  .Sum(x => x.SpendVector.SysName == "INC" ? +x.Sum : -x.Sum);
            }

            var data =
                (from m in
                    Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.TransferId == null && x.CategoryId == categoryId &&
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

        public IEnumerable<KeyValuePair<string, IEnumerable<SpendChartViewModel>>> GetYearlyCategoryChartDataGroupByMonthes(int year, string vectorSysName, int? categoryId = null, int? billId = null)
        {
            var endDate = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            var startMonth = endDate.AddMonths(-11).Month;
            var startYear = endDate.AddMonths(-11).Year;
            var startDate = new DateTime(startYear, startMonth, 1);

            return GetCategoryChartDataGroupByMonthes(startDate, endDate, vectorSysName, categoryId: categoryId, billId: billId);
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<SpendChartViewModel>>> GetCategoryChartDataGroupByMonthes(DateTime startDate, DateTime endDate, string vectorSysName, bool calcCumulativeTotal = false, int? categoryId = null, int? billId = null)
        {
            var list = new List<KeyValuePair<string, IEnumerable<SpendChartViewModel>>>();

            var cats = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid && !x.IsSystem
            && (!categoryId.HasValue || (categoryId.HasValue && x.CategoryId == categoryId))
            //&& (!billId.HasValue || (billId.HasValue == billId))
            );

            foreach (var cat in cats)
            {
                var data = GetChartDataByCategoryGroupByMonthes(startDate, endDate, cat.CategoryId, vectorSysName, calcCumulativeTotal, billId: billId);
                var item = new KeyValuePair<string, IEnumerable<SpendChartViewModel>>(cat.Name, data);
                list.Add(item);
            }

            return list;
        }

        public IEnumerable<SpendChartViewModel> GetChartDataByCategoryGroupByMonthes(DateTime startDate, DateTime endDate, int categoryId, string vectorSysName, bool calcCumulativeTotal = false, int? billId = null)
        {
            if (startDate > endDate) throw new ArgumentException("Дата окончания не может быть меньшще даты начала!");

            var sDate = startDate.Date;
            var eDate = endDate.Date;


            double currentTotal = 0;
            if (calcCumulativeTotal)
            {
                currentTotal = Uow.Spends.GetAllQuery(
                  x =>
                      x.Enabled && x.UserSid == UserSid && x.CategoryId == categoryId && x.TransferId==null
                      && (String.IsNullOrEmpty(vectorSysName) || (!String.IsNullOrEmpty(vectorSysName) && x.SpendVector.SysName == vectorSysName))
                      && (!billId.HasValue || (billId.HasValue && x.BillId == billId))
                      && DbFunctions.TruncateTime(x.Date) < DbFunctions.TruncateTime(sDate))
                  .Sum(x => x.SpendVector.SysName == "INC" ? +x.Sum : -x.Sum);
            }

            var data = Uow.Spends.GetAllQuery(x => x.Enabled && x.UserSid == UserSid && x.CategoryId == categoryId && x.TransferId == null &&
                                                   (String.IsNullOrEmpty(vectorSysName) ||
                                                    (!String.IsNullOrEmpty(vectorSysName) &&
                                                     x.SpendVector.SysName == vectorSysName))
                                                   && (!billId.HasValue || (billId.HasValue && x.BillId == billId))
                                                     && DbFunctions.TruncateTime(x.Date) >= DbFunctions.TruncateTime(sDate)
                                                   && DbFunctions.TruncateTime(x.Date) <= DbFunctions.TruncateTime(eDate),
                x => x.OrderBy((y => y.Date)), x => x.SpendVector).AsEnumerable()
                  .Select(x =>
                  {
                      return new SpendChartViewModel() { Date = x.Date, Sum = x.Sum };
                  });

            var result = from t in data
                         group t by new { t.Year, t.Month }
                into g
                         select new SpendChartViewModel()
                         {
                             Year = g.Key.Year,
                             Month = g.Key.Month,
                             Sum = g.Sum(x => x.Sum)
                         };


            return result;
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
            if (cat.Spend.Any(x => x.Enabled)) throw new ArgumentException("Нельзя удалить категорию пока она содержит записи доходов/расходов! Удалите или переместите записи в другую категорию и повторите попытку!");
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

            int orderNum = 1;
            var cats = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid);
            if (cats.Any())
            {
                orderNum = cats.Max(x => x.OrderNum) + 1;
            }

            model.OrderNum = orderNum;
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

        public void SpendBillOrderUp(int id)
        {
            var bill = Uow.SpendBills.GetOne(x => x.Id == id);

            var bills = Uow.SpendBills.GetAll(x => x.Enabled && x.UserSid == UserSid && x.Id != id && x.OrderNum < bill.OrderNum);

            if (bills.Any())
            {
                var upperBill = bills.OrderByDescending(x => x.OrderNum).First();
                int oldOrder = bill.OrderNum;
                bill.OrderNum = upperBill.OrderNum;
                upperBill.OrderNum = oldOrder;
                Uow.SpendBills.Update(upperBill);
            }
            else
            {
                bill.OrderNum--;
            }

            Uow.SpendBills.Update(bill);
            Uow.Commit();
        }

        public void SpendBillOrderDown(int id)
        {
            var bill = Uow.SpendBills.GetOne(x => x.Id == id);

            var bills = Uow.SpendBills.GetAll(x => x.Enabled && x.UserSid == UserSid && x.Id != id && x.OrderNum > bill.OrderNum);

            if (bills.Any())
            {
                var upperBill = bills.OrderBy(x => x.OrderNum).First();
                int oldOrder = bill.OrderNum;
                bill.OrderNum = upperBill.OrderNum;
                upperBill.OrderNum = oldOrder;
                Uow.SpendBills.Update(upperBill);
            }
            else
            {
                bill.OrderNum++;
            }

            Uow.SpendBills.Update(bill);
            Uow.Commit();
        }

        public IEnumerable<SpendBills> SpendBillGetList()
        {
            var model = Uow.SpendBills.GetAll(x => x.UserSid == UserSid && x.Enabled, x => x.OrderBy(y => y.OrderNum).ThenBy(y => y.Name), x => x.SpendBillTypes, x => x.Spend);
            return model;
        }

        public IEnumerable<KeyValuePair<int, string>> SpendBillGetSelectionList()
        {
            var list = SpendBillGetList();
            var result = new List<KeyValuePair<int, string>>();
            foreach (var category in list)
            {
                result.Add(new KeyValuePair<int, string>(category.Id, category.Name));
            }
            return result;
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

            int orderNum = 1;
            var bills = Uow.SpendBills.GetAll(x => x.Enabled && x.UserSid == UserSid);
            if (bills.Any())
            {
                orderNum = bills.Max(x => x.OrderNum) + 1;
            }

            model.OrderNum = orderNum;
            model.CreateDate = DateTime.Now;
            Uow.SpendBills.Insert(model);
            Uow.Commit();
        }

        public void SpendBillEdit(SpendBills model)
        {
            if (String.IsNullOrEmpty(model.Name)) throw new ArgumentException("Необходимо заполнить название счета!");
            var exists = Uow.SpendBills.GetAll(x => x.Enabled && x.UserSid == UserSid && x.Name == model.Name);
            if (exists.Any()) throw new ArgumentException("Такое название счета уже существует!");
            var bill = Uow.SpendBills.GetOne(x => x.Id == model.Id);
            bill.Name = model.Name;
            bill.TypeId = model.TypeId;
            bill.StartAmount = model.StartAmount;
            bill.StartDate = model.StartDate;
            bill.EndDate = model.EndDate;
            bill.Comment = model.Comment;
            Uow.SpendBills.Update(bill);
            Uow.Commit();
        }

        public IEnumerable<SpendBillTypes> SpendBillTypeGetList()
        {
            var list = Uow.SpendBillTypes.GetAll(null, x => x.OrderBy(y => y.OrderNum).ThenBy(y => y.Name));
            return list;
        }

        public void SpendBillDelete(int id)
        {
            var bill = Uow.SpendBills.GetOne(x => x.Id == id, x => x.Spend);
            if (bill.Spend.Any(x => x.Enabled)) throw new ArgumentException("Нельзя удалить счет пока он содержит записи доходов/расходов! Удалите или переместите записи на другой счет и повторите попытку!");
            bill.Enabled = false;
            Uow.Commit();
        }
        public void SpendBillMerge(int[] billIds, int billNameId)
        {
            var list = Uow.Spends.GetAll(x => x.Enabled && x.UserSid == UserSid && billIds.Contains(x.CategoryId));
            foreach (var item in list)
            {
                item.CategoryId = billNameId;
            }
            var oldBills = Uow.SpendBills.GetAll(
                x => x.Enabled && x.UserSid == UserSid && billIds.Contains(x.Id) && x.Id != billNameId);
            foreach (var bill in oldBills)
            {
                bill.Enabled = false;
            }
            Uow.Commit();
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
