using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Data.Objects;

namespace Data.Services
{
    public class MarketService : BaseService
    {
        protected OwnAgentEntities Db { get; }
        public MarketService(string userSid) : base(userSid)
        {
            Db = new OwnAgentEntities();
        }

        public static MarketService Instance(string userSid)
        {
            return new MarketService(userSid);
        }

        public int ServiceCreate(MarketServices model)
        {
            model.CreateDate=DateTimeOffset.Now;
            model.CreatorId = UserSid;
            model.Enabled = true;

            Db.MarketServices.Add(model);
            Db.SaveChanges();

            ServiceSetStateAndSaveStateHistory(model.Id, "NEW");

            return model.Id;
        }

        public void ServiceSetStateAndSaveStateHistory(int serviceId, string stateSysName, string comment = null)
        {
            var state = Db.MarketServiceStates.Single(x => x.SysName == stateSysName);
            var service = Db.MarketServices.Single(x => x.Id == serviceId);

            service.StateId = state.Id;

            var histModel = new MarketServiceStateHistory();
            histModel.ServiceId = service.Id;
            histModel.StateId = service.StateId.Value;
            histModel.Comment = comment;
            histModel.CreateDate = DateTimeOffset.Now;
            histModel.CreatorId = UserSid;
            Db.MarketServiceStateHistory.Add(histModel);

            Db.SaveChanges();
        }

        public MarketServices ServiceGet(int id)
        {
            var model = Db.MarketServices.Where(x => x.Id == id)
                .Include(x => x.MarketServiceTypes)
                .Include(x=>x.MarketServicePayForms)
                .Single();

            return model;
        }

        public IEnumerable<MarketServiceTypes> ServiceTypesGetList()
        {
            var list = Db.MarketServiceTypes.OrderBy(x=>x.Name);

            return list;
        }

        public IEnumerable<MarketServicePayForms> ServicePayFormsGetList()
        {
            var list = Db.MarketServicePayForms.OrderBy(x => x.Name);

            return list;
        }
    }
}
