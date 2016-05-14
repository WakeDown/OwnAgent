using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;

namespace Data.Objects
{
    public class UnitOfWork: IUnitOfWork, IDisposable
    {
        public OwnAgentDbContext DbContext { get; set; }

        public UnitOfWork()
        {
            CreateDbContext();
        }

        //repositories
        #region Repositries

        private IRepository<Spend> _spends;
        private IRepository<SpendCategory> _spendCategories;
        private IRepository<SpendVector> _spendVectors;
        private IRepository<AspNetUsers> _aspNetUsers;

        
        public IRepository<Spend> Spends
        {
            get
            {
                if (_spends == null)
                {
                    _spends = new Repository<Spend>(DbContext);

                }
                return _spends;
            }
        }

        public IRepository<SpendCategory> SpendCategories
        {
            get
            {
                if (_spendCategories == null)
                {
                    _spendCategories = new Repository<SpendCategory>(DbContext);

                }
                return _spendCategories;
            }
        }

        public IRepository<SpendVector> SpendVectors
        {
            get
            {
                if (_spendVectors == null)
                {
                    _spendVectors = new Repository<SpendVector>(DbContext);

                }

                return _spendVectors;
            }
        }

        public IRepository<AspNetUsers> AspNetUsers
        {
            get
            {
                if (_aspNetUsers == null)
                {
                    _aspNetUsers = new Repository<AspNetUsers>(DbContext);

                }

                return _aspNetUsers;
            }
        }

        #endregion

        /// <summary>
        /// Save pending changes to the database
        /// </summary>
        public void Commit()
        {
            DbContext.SaveChanges();
        }

        protected void CreateDbContext()
        {
            DbContext = new OwnAgentDbContext();

            // Do NOT enable proxied entities, else serialization fails.
            //if false it will not get the associated certification and skills when we
            //get the applicants
            DbContext.Configuration.ProxyCreationEnabled = false;

            // Load navigation properties explicitly (avoid serialization trouble)
            DbContext.Configuration.LazyLoadingEnabled = false;

            // Because Web API will perform validation, we don't need/want EF to do so
            DbContext.Configuration.ValidateOnSaveEnabled = false;

            //DbContext.Configuration.AutoDetectChangesEnabled = false;
            // We won't use this performance tweak because we don't need
            // the extra performance and, when autodetect is false,
            // we'd have to be careful. We're not being that careful.
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }

        #endregion
    }
}
