//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MarketServicePayments
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public Nullable<decimal> Sum { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Comment { get; set; }
        public System.DateTimeOffset CreateDate { get; set; }
        public string CreatorId { get; set; }
        public bool Enabled { get; set; }
        public Nullable<System.DateTimeOffset> DeleteDate { get; set; }
        public string DeleterId { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual MarketServices MarketServices { get; set; }
    }
}
