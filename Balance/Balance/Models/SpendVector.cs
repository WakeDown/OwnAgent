using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Balance.Models
{
    //public enum SpendVector
    //{
    //    Minus = 0,
    //    Plus=1,
    //    Invest=2
    //}
    public class SpendVector
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderNum { get; set; }
        public bool Selected { get; set; }

        public SpendVector()
        {
            Selected = false;
        }

        public SpendVector(int id, string name, int orderNum):this()
        {
            Id = id;
            Name = name;
            OrderNum = orderNum;
        }

        public static IEnumerable<SpendVector> GetList()
        {
            var list = new List<SpendVector>();
            list.Add(new SpendVector(1, "расход", 1) { Selected = true });
            list.Add(new SpendVector(2, "доход", 2));
            list.Add(new SpendVector(3, "инвест", 3));
            return list.OrderBy(c => c.OrderNum);
        }

        public static SelectList GetSelectionList()
        {
            var list = GetList().ToList();
            return new SelectList(list, "Id", "Name", list.First(m => m.Selected).Id);
        }
    }
}