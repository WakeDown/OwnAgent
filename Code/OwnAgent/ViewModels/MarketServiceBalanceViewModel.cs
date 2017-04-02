using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OwnAgent.ViewModels
{
    public class MarketServiceBalanceViewModel
    {
        public DateTimeOffset? ChangeDate { get; set; }
        public decimal? ServiceSum { get; set; }
        public decimal? PaymentSum { get; set; }
        public decimal? BalanceSum { get; set; }
    }
}