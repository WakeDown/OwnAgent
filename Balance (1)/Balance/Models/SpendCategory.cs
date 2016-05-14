using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Balance.Models
{
    public class SpendCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderNum { get; set; }
        public bool Selected { get; set; }

        public SpendCategory()
        {
            Selected = false;
        }

        public SpendCategory(int id, string name, int orderNum):this()
        {
            Id = id;
            Name = name;
            OrderNum = orderNum;
        }

        public static IEnumerable<SpendCategory> GetList()
        {
            var list = new List<SpendCategory>();
            list.Add(new SpendCategory(1, "Еда/Бытовое", 1) { Selected = true });
            list.Add(new SpendCategory(2, "Ресторан", 2));
            list.Add(new SpendCategory(3, "Бензин", 3));
            list.Add(new SpendCategory(4, "Услуги", 4));
            list.Add(new SpendCategory(5, "Одежда", 5));
            list.Add(new SpendCategory(6, "Зарплата", 6));
            list.Add(new SpendCategory(7, "Подработка", 7));
            list.Add(new SpendCategory(8, "Вклад", 8));
            list.Add(new SpendCategory(9, "Другое", 9));
            return list.OrderBy(c=>c.OrderNum);
        }

        public static SelectList GetSelectionList()
        {
            var list = GetList().ToList();
            return new SelectList(list, "Id", "Name",list.First(m=>m.Selected).Id);
        }
    }
}