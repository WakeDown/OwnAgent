using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;

namespace Data.Objects
{
    public interface IUnitOfWork
    {
        // Save pending changes to the data store.
        void Commit();

        // Repositories
        IRepository<Spend> Spends { get; }
        IRepository<SpendCategory> SpendCategories { get; }
        IRepository<SpendVector> SpendVectors { get; }
        IRepository<AspNetUsers> AspNetUsers { get; }
        
    }
}
