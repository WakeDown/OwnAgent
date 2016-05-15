using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Models.ViewModels;

namespace Models.Mappers
{
    public class SpendViewModel2Spend
    {
        public static Spend Map(SpendNewViewModel model)
        {
            var spend = new Spend();
            spend.Sum = model.Sum;
            spend.Comment = model.Comment;
            spend.VectorId = model.VectorId;
            spend.CategoryId = model.CategoryId;
            spend.Date = model.Date;
            return spend;
        }
    }
}
