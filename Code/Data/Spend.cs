//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Spend
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public int VectorId { get; set; }
        public int CategoryId { get; set; }
        public double Sum { get; set; }
        public string Comment { get; set; }
        public string ClientId { get; set; }
    
        public virtual SpendCategory SpendCategory { get; set; }
        public virtual SpendVector SpendVector { get; set; }
    }
}
