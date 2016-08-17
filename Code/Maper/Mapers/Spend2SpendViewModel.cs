using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Models.ViewModels;

namespace Models.Mappers
{
    public class Spend2SpendViewModel
    {
        public static SpendLastAddViewModel Map2LastAddViewModel(Spend spend)
        {
            var model = new SpendLastAddViewModel();
            model.Id = spend.Id;
            model.Sum = spend.Sum;
            model.SpendCategoryName = spend.SpendCategory.Name;
            model.SpendVectorName = spend.SpendVector.Name;
            model.SpendVectorBgColorClass = spend.SpendVector.BgColorClass;
            model.SpendVectorIconName = spend.SpendVector.IconName;
            model.SpendVectorSysName = spend.SpendVector.SysName;
            model.Date = spend.Date;
            return model;
        }

        public static IEnumerable<SpendLastAddViewModel> MapList2LastAddViewModelList(IEnumerable<Spend> list)
        {
            var result = new List<SpendLastAddViewModel>();
            foreach (Spend spend in list)
            {
                result.Add(Map2LastAddViewModel(spend));
            }
            return result;
        }

        public static SpendTopViewModel Map2TopViewModel(Spend spend)
        {
            var model = new SpendTopViewModel();
            model.Sum = spend.Sum;
            model.SpendCategoryName = spend.SpendCategory.Name;
            model.SpendVectorName = spend.SpendVector.Name;
            model.SpendCategoryId = spend.CategoryId;
            model.SpendVectorId = spend.VectorId;
            model.SpendVectorSysName = spend.SpendVector.SysName;
            model.SpendVectorBgColorClass = spend.SpendVector.BgColorClass;
            model.SpendVectorIconName = spend.SpendVector.IconName;
            model.Date = spend.Date;
            return model;
        }

        public static IEnumerable<SpendTopViewModel> MapList2TopViewModelList(IEnumerable<Spend> list)
        {
            var result = new List<SpendTopViewModel>();
            foreach (Spend spend in list)
            {
                result.Add(Map2TopViewModel(spend));
            }
            return result;
        }
    }
}
