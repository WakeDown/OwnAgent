using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;

namespace Data.Objects
{
    public abstract class BaseService
    {
        protected UnitOfWork Uow { get; }
        //protected AspNetUsers User { get; set; }
        protected string UserSid { get; }

        public BaseService(string userSid)
        {
            UserSid = userSid;
            Uow = new UnitOfWork();
        }
    }
}
