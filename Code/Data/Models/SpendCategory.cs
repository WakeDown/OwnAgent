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
    
    public partial class SpendCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SpendCategory()
        {
            this.Spend = new HashSet<Spend>();
        }
    
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int OrderNum { get; set; }
        public bool Selected { get; set; }
        public bool Enabled { get; set; }
        public string UserSid { get; set; }
        public bool IsSystem { get; set; }
        public string SysName { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Spend> Spend { get; set; }
    }
}
