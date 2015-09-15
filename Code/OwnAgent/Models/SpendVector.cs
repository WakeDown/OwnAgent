using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OwnAgent.Objects;

namespace OwnAgent.Models
{
    public class SpendVector
    {
        [Key]
        public int VectorId { get; set; }
        public string Name { get; set; }
        public int OrderNum { get; set; }
        public bool Selected { get; set; }
        [MaxLength(50)]
        public string ClientId { get; set; }

        public SpendVector()
        {
            Selected = false;
        }

        public SpendVector(string name, int orderNum, string clientId) : this()
        {
            //VectorId = id;
            Name = name;
            OrderNum = orderNum;
            ClientId = clientId;
        }

        public static IEnumerable<SpendVector> GetList(string clientId)
        {
            BalanceContext db = new BalanceContext();
            var list = db.SpendVectors.Where(x => x.ClientId.Equals(clientId)).ToList().OrderBy(c => c.OrderNum);
            if (list.Any())
            {
                list.First().Selected = true;
            }
            return list;

            //var list = new List<SpendVector>();
            //list.Add(new SpendVector(1, "расход", 1) { Selected = true });
            //list.Add(new SpendVector(2, "доход", 2));
            //list.Add(new SpendVector(3, "инвест", 3));
            //return list.OrderBy(c => c.OrderNum);
        }

        public static SelectList GetSelectionList(string clientId)
        {
            var list = GetList(clientId).ToList();
            return new SelectList(list, "VectorId", "Name", list.First(m => m.Selected).VectorId);
        }
    }
}