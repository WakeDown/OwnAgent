using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Balance.Models
{
    public class Spend
    {
        public DateTime Date { get; set; }
        public SpendVector Vector { get; set; }
        public SpendCategory Category { get; set; }
        public double Sum { get; set; }
        public string Comment { get; set; }

        public Spend()
        {
            Date=DateTime.Now;
        }

        public void Save()
        {
            
        }
    }
}