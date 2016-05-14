using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Objects
{
    public class OwnAgentDbContext : DbContext
    {
        public OwnAgentDbContext()
            : base("Name=OwnAgentEntities")//this is the connection string name
        {
        }
    
    }
}
