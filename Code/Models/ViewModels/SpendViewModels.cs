using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class SpendNewViewModel
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public int VectorId { get; set; }
        public int CategoryId { get; set; }
        public double Sum { get; set; }
        public string Comment { get; set; }
    }

    public class SpendTopViewModel
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public string SpendVectorBgColorClass { get; set; }
        public string SpendVectorIconName { get; set; }
        public string SpendVectorSysName { get; set; }
        public string SpendVectorName { get; set; }
        public string SpendCategoryName { get; set; }
        public int SpendCategoryId { get; set; }
        public int SpendVectorId { get; set; }
        public double Sum { get; set; }
    }

    public class SpendLastAddViewModel
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public string SpendVectorName { get; set; }
        public string SpendCategoryName { get; set; }
        public double Sum { get; set; }
        public string SpendVectorBgColorClass { get; set; }
        public string SpendVectorIconName { get; set; }
    }

    public class SpendStatViewModel
    {
        public string SpendVectorSysName { get; set; }
        public string SpendVectorName { get; set; }
        public string SpendCategoryName { get; set; }
        public int SpendCategoryId { get; set; }
        public int SpendVectorId { get; set; }
        public double Sum { get; set; }
        public string SpendVectorBgColorClass { get; set; }
        public string SpendVectorIconName { get; set; }
        public double Percent { get; set; }
    }

    public class SpendChartViewModel
    {
        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                Year = date.Year;
                Month = date.Month;
                Day = date.Day;
                DateStr = Date.ToString("dd.MM.yyyy");
            }
        }
        public string DateStr { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public double Sum { get; set; }
        public double CumulativeTotal { get; set; }
    }
}
