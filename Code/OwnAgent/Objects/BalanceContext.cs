using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using OwnAgent.Models;

namespace OwnAgent.Objects
{
    public class BalanceContext : DbContext
    {
        public BalanceContext() : base("BalanceContext")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }

        //public DbSet<Client> Clients { get; set; }
        //public DbSet<Spend> Spends { get; set; }
        //public DbSet<SpendCategory> SpendCategories { get; set; }
        //public DbSet<SpendVector> SpendVectors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}