using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OwnAgent.Models;

namespace OwnAgent.Objects
{
    public class BalanceInitializer : CreateDatabaseIfNotExists<BalanceContext>
    {
        protected override void Seed(BalanceContext context)
        {
            base.Seed(context);

            ////if (!context.SpendCategories.Any())
            ////{
            //    var spendCats = new List<SpendCategory>();
            //    spendCats.Add(new SpendCategory(1, "Еда/Бытовое", 1) { Selected = true });
            //    spendCats.Add(new SpendCategory(2, "Ресторан", 2));
            //    spendCats.Add(new SpendCategory(3, "Бензин", 3));
            //    spendCats.Add(new SpendCategory(4, "Услуги", 4));
            //    spendCats.Add(new SpendCategory(5, "Одежда", 5));
            //    spendCats.Add(new SpendCategory(6, "Зарплата", 6));
            //    spendCats.Add(new SpendCategory(7, "Подработка", 7));
            //    spendCats.Add(new SpendCategory(8, "Вклад", 8));
            //    spendCats.Add(new SpendCategory(9, "Другое", 9));
            //    spendCats.ForEach(s => context.SpendCategories.Add(s));
            //    context.SaveChanges();
            ////}

            ////if (!context.SpendVectors.Any())
            ////{
            //    var spendVectors = new List<SpendVector>();
            //    spendVectors.Add(new SpendVector(1, "расход", 1) { Selected = true });
            //    spendVectors.Add(new SpendVector(2, "доход", 2));
            //    spendVectors.Add(new SpendVector(3, "инвест", 3));
            //    spendVectors.ForEach(s => context.SpendVectors.Add(s));
            //    context.SaveChanges();
            ////}
        }
    }
}
