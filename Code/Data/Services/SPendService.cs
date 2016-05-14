using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Data.Objects;

namespace Data.Services
{
    public class SpendService:BaseService
    {
        public SpendService(string userSid) : base(userSid)
        {
        }
        public static SpendService Instance(string userSid)
        {
            return new SpendService(userSid);
        }

        public IEnumerable<KeyValuePair<int, string>> GetCategorySelectionList()
        {
            var list = Uow.SpendCategories.GetAll(x => x.Enabled && x.UserSid == UserSid, x=>x.OrderBy(y=>y.OrderNum).ThenBy(y=>y.Name));
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
            spend.CreateDate=DateTime.Now;
            spend.Enabled = true;
            spend.UserSid = UserSid;
            Uow.Spends.Insert(spend);
            Uow.Commit();
        }

        public IEnumerable<Spend> GetSpendList(out int totalCount, int? page = null, int? psize = null)
        {
            var list = Uow.Spends.GetAllWithTotalCount(out totalCount, page, psize, x => x.UserSid == UserSid && x.Enabled
            , x => x.OrderByDescending(y => y.CreateDate), x=>x.SpendCategory, x=>x.SpendVector);
            return list;
        }

        public IEnumerable<Spend> GetTop(int topRows)
        {
            var limitDate = DateTime.Now.AddMonths(-3).Date;
            var list = (from s in Uow.Spends.GetAllQuery(x=>x.Enabled&&x.UserSid==UserSid && x.Date >= limitDate)
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
    }
}
