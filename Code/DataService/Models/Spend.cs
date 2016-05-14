using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using OwnAgent.Objects;

namespace OwnAgent.Models
{
    public class Spend
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int VectorId { get; set; }
        public virtual SpendVector SpendVector { get; set; }
        
        public int CategoryId { get; set; }
        public virtual SpendCategory SpendCategory { get; set; }
        public double Sum { get; set; }
        public string Comment { get; set; }
        [MaxLength(50)]
        public string ClientId { get; set; }

        public Spend()
        {
            Date = DateTime.Now;
        }

        public void Save(string clientId)
        {
            if (Sum > 0)
            {
                BalanceContext context = new BalanceContext();
                ClientId = clientId;
                context.Spends.Add(this);
                context.SaveChanges();
            }
        }

        public static IEnumerable<Spend> GetList(string clientId, int topRows = 10)
        {
            BalanceContext context = new BalanceContext();
            
            return context.Spends.Where(x => x.ClientId.Equals(clientId)).OrderByDescending(x=>x.Date).ThenByDescending(x=>x.Id).Take(topRows).ToList();
        }

        public static IEnumerable<Spend> GetTop(string clientId, int topRows = 3)
        {
            BalanceContext context = new BalanceContext();

            var list = (from s in context.Spends
                        where s.ClientId.Equals(clientId)
                       group s by new
                       {
                           s.SpendVector, s.SpendCategory, s.Sum
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
            //return context.Spends.GroupBy(x=> x.VectorId, x=>x.CategoryId).ToList();
        }

        public static IEnumerable<KeyValuePair<int, int>> GetMonthlyReportYears(string clientId)
        {
            var list = new Dictionary<int, int>();
            list.Add(2015, 2015);
            list.Add(2014, 2014);
            return list;
        }

        public static IEnumerable<KeyValuePair<int, string>> GetMonthlyReportMonthes(string clientId)
        {
            var list = new Dictionary<int, string>();
            list.Add(1, "январь");
            list.Add(2, "февраль");
            list.Add(3, "март");
            list.Add(4, "апрель");
            list.Add(5, "май");
            list.Add(6, "июнь");
            list.Add(7, "июль");
            list.Add(8, "август");
            list.Add(9, "сентябрь");
            list.Add(10, "октябрь");
            list.Add(11, "ноябрь");
            list.Add(12, "декабрь");
            return list;
        }

        public static IEnumerable<SpendStatItem> GetMonthlyReport(string clientId, int year, int month)
        {
            var list = new List<SpendStatItem>();

            try
            {

                BalanceContext db = new BalanceContext();

                var report = from s in db.Spends
                    where s.ClientId.Equals(clientId) && s.Date.Year == year && s.Date.Month == month
                    group s by new
                    {
                        s.SpendCategory,
                        s.SpendVector
                    }
                    into gs
                    select new
                    {
                        SpendCategory = gs.Key.SpendCategory.Name,
                        SpendVector = gs.Key.SpendVector.Name,
                        Sum = gs.Sum(x => x.Sum)
                    };
                list = report.AsEnumerable().Select(x => new SpendStatItem
                {
                    SpendVectorName = x.SpendVector,
                    SpendCategoryName = x.SpendCategory,
                    Sum = x.Sum
                }).OrderBy(x => x.SpendVectorName).ThenByDescending(x => x.Sum).ToList();

                //db.Spends.Where(x=>x.ClientId.Equals(clientId) && x.Date.Year==year && x.Date.Month==month).GroupBy(x=>x.Date.Month, x.).Select(x=>new
                //{
                //    x.First().SpendCategory.Name,
                //    x.First().Spe
                //})
            }
            catch
            {
            }
            return list;
        }

        public static IEnumerable<SpendStatItem> GetYearlyReport(string clientId, int year)
        {
            var list = new List<SpendStatItem>();

            BalanceContext db = new BalanceContext();

            var report = from s in db.Spends
                         where s.ClientId.Equals(clientId) && s.Date.Year == year
                         group s by new
                         {
                             s.SpendCategory,
                             s.SpendVector
                         }
                into gs
                         select new
                         {
                             SpendCategory = gs.Key.SpendCategory.Name,
                             SpendVector = gs.Key.SpendVector.Name,
                             Sum = gs.Sum(x => x.Sum)
                         };
            list = report.AsEnumerable().Select(x => new SpendStatItem
            {
                SpendVectorName = x.SpendVector,
                SpendCategoryName = x.SpendCategory,
                Sum = x.Sum
            }).OrderBy(x => x.SpendVectorName).ThenByDescending(x => x.Sum).ToList();

            //db.Spends.Where(x=>x.ClientId.Equals(clientId) && x.Date.Year==year && x.Date.Month==month).GroupBy(x=>x.Date.Month, x.).Select(x=>new
            //{
            //    x.First().SpendCategory.Name,
            //    x.First().Spe
            //})

            return list;
        }

        public static IEnumerable<SpendStatItem> GetVectorMonthlyReport(string clientId, int year, int month)
        {
            var list = new List<SpendStatItem>();
            try
            {
                BalanceContext db = new BalanceContext();

                var report = from s in db.Spends
                    where s.ClientId.Equals(clientId) && s.Date.Year == year && s.Date.Month == month
                    group s by new
                    {
                        s.SpendVector
                    }
                    into gs
                    select new
                    {
                        SpendVector = gs.Key.SpendVector.Name,
                        Sum = gs.Sum(x => x.Sum)
                    };
                list = report.AsEnumerable().Select(x => new SpendStatItem
                {
                    SpendVectorName = x.SpendVector,
                    Sum = x.Sum
                }).OrderBy(x => x.SpendVectorName).ThenByDescending(x => x.Sum).ToList();
            }
            catch { }

            return list;
        }

        public static IEnumerable<SpendStatItem> GetVectorYearlyReport(string clientId, int year)
        {
            var list = new List<SpendStatItem>();

            BalanceContext db = new BalanceContext();

            var report = from s in db.Spends
                         where s.ClientId.Equals(clientId) && s.Date.Year == year
                         group s by new
                         {
                             s.SpendVector
                         }
                into gs
                         select new
                         {
                             SpendVector = gs.Key.SpendVector.Name,
                             Sum = gs.Sum(x => x.Sum)
                         };
            list = report.AsEnumerable().Select(x => new SpendStatItem
            {
                SpendVectorName = x.SpendVector,
                Sum = x.Sum
            }).OrderBy(x => x.SpendVectorName).ThenByDescending(x => x.Sum).ToList();


            return list;
        }
    }
}