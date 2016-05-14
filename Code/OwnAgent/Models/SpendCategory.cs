using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OwnAgent.Objects;

namespace OwnAgent.Models
{
    public class SpendCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int OrderNum { get; set; }
        public bool Selected { get; set; }
        [MaxLength(50)]
        public string ClientId { get; set; }

        public SpendCategory()
        {
            Selected = false;
        }

        public SpendCategory(string name, int orderNum, string clientId) : this()
        {
            //CategoryId = id;
            Name = name;
            OrderNum = orderNum;
            ClientId = clientId;
        }

        public static IEnumerable<SpendCategory> GetList(string clientId)
        {
            BalanceContext db = new BalanceContext();
            var list = db.SpendCategories.Where(x=>x.ClientId.Equals(clientId)).ToList().OrderBy(c => c.OrderNum);
            if (list.Any())
            {
                list.First().Selected = true;
            }
            return list;

            //var list = new List<SpendCategory>();
            //list.Add(new SpendCategory(1, "Еда/Бытовое", 1) { Selected = true });
            //list.Add(new SpendCategory(2, "Ресторан", 2));
            //list.Add(new SpendCategory(3, "Бензин", 3));
            //list.Add(new SpendCategory(4, "Услуги", 4));
            //list.Add(new SpendCategory(5, "Одежда", 5));
            //list.Add(new SpendCategory(6, "Зарплата", 6));
            //list.Add(new SpendCategory(7, "Подработка", 7));
            //list.Add(new SpendCategory(8, "Вклад", 8));
            //list.Add(new SpendCategory(9, "Другое", 9));
            //return list.OrderBy(c => c.OrderNum);
        }

        public static SelectList GetSelectionList(string clientId)
        {
            var list = GetList(clientId).ToList();
            return new SelectList(list, "CategoryId", "Name", list.Any() ? list.First(m => m.Selected).CategoryId:0);
        }
    }
}