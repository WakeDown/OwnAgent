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

        public IEnumerable<MarketServices> ServiceGetList(bool? stateIsActive)
        {
            var list = Db.MarketServices
                .Where(
                x=>x.Enabled
                && (!stateIsActive.HasValue || (stateIsActive.HasValue && (stateIsActive.Value && x.MarketServiceStates.SysName=="ACTIVE") || (stateIsActive.HasValue && (!stateIsActive.Value && x.MarketServiceStates.SysName == "DISABLED"))))
                )
                .OrderByDescending(x => x.CreateDate);
            return list;
        }

        public int ServiceCreate(MarketServices model)
        {
            var state = Db.MarketServiceStates.Single(x => x.SysName == "ACTIVE");

            model.StateId = state.Id;
            model.CreateDate=DateTimeOffset.Now;
            model.CreatorId = UserSid;
            model.Enabled = true;

            Db.MarketServices.Add(model);
            Db.SaveChanges();

            ServiceBalanceCumCalculate(model.Id);

            ServiceSaveHistory(model.Id, "CREATED");
            return model.Id;
        }

        public void ServiceUpdate(MarketServices newModel)
        {
            var model = Db.MarketServices.Single(x => x.Id == newModel.Id);

            model.TypeId = newModel.TypeId;
            model.ClientName = newModel.ClientName;
            model.RecipientName = newModel.RecipientName;
            model.ManagerName = newModel.ManagerName;
            model.ClaimNumber = newModel.ClaimNumber;
            model.TenderNumber = newModel.TenderNumber;
            model.TenderDate = newModel.TenderDate;
            model.ContractSum = newModel.ContractSum;
            model.BudgetSum = newModel.BudgetSum;
            model.ServiceSum = newModel.ServiceSum;
            model.ServiceComment = newModel.ServiceComment;
            model.ServicePayFormId = newModel.ServicePayFormId;
            model.Comment = newModel.Comment;
            Db.SaveChanges();

            ServiceBalanceCumCalculate(model.Id);

            ServiceSaveHistory(model.Id, "CHANGED");
        }

        public void ServiceDelete(int id)
        {
            var state = Db.MarketServiceStates.Single(x => x.SysName == "DISABLED");

            var model = Db.MarketServices.Single(x => x.Id == id);
            model.StateId = state.Id;
            model.DeleteDate = DateTimeOffset.Now;
            model.DeleterId = UserSid;
            model.Enabled = false;

            Db.SaveChanges();

            ServiceSaveHistory(model.Id, "DELETED");
        }

        public void ServiceSaveHistory(int serviceId, string actionSysName, string comment = null)
        {
            var action = Db.MarketServiceHistoryActions.Single(x => x.SysName == actionSysName);
            ServiceSaveHistory(serviceId, action.Id, comment);
        }

        public void ServiceSaveHistory(int serviceId, int actionId, string comment = null)
        {
            var histModel = new MarketServiceHistory();
            histModel.ServiceId = serviceId;
            histModel.ActionId = actionId;
            histModel.Comment = comment;
            histModel.CreateDate = DateTimeOffset.Now;
            histModel.CreatorId = UserSid;
            Db.MarketServiceHistory.Add(histModel);

            Db.SaveChanges();
        }

        public void ServiceSetConditionAndSaveHistory(int serviceId, int conditionId, string comment = null)
        {
            var service = ServiceGet(serviceId);
            var condition = Db.MarketServiceConditions.Single(x => x.Id == conditionId);

            string changeComment = (service.ConditionId.HasValue ? service.MarketServiceConditions.Name : "Не определено") + " -> ";
            service.ConditionId = conditionId;
            service.ConditionChangeDate = DateTimeOffset.Now;
            service.ConditionComment = comment;
            Db.SaveChanges();

            service = ServiceGet(serviceId);
            changeComment += service.ConditionId.HasValue ? service.MarketServiceConditions.Name : "Не определено";
            if (!String.IsNullOrEmpty(comment))
            {
                changeComment += "\r\nКомментарий: " + comment;
            }

            if (condition.IsFinish && !service.MarketServiceConditions.IsFinish)
            {
                var state = Db.MarketServiceStates.Single(x => x.SysName == "ACTIVE");
                service.StateId = state.Id;
                Db.SaveChanges();
                ServiceSaveHistory(serviceId, "REACTIVATE", changeComment);
            }

            ServiceSaveHistory(serviceId, "CONDITIONCHANGED", changeComment);

            if (!condition.IsFinish && service.MarketServiceConditions.IsFinish)
            {
                var state = Db.MarketServiceStates.Single(x => x.SysName == "DISABLED");
                service.StateId = state.Id;
                Db.SaveChanges();
                ServiceSaveHistory(serviceId, "FINISH", changeComment);
            }
        }

        public MarketServices ServiceGet(int id)
        {
            var model = Db.MarketServices.Where(x => x.Id == id)
                .Include(x => x.MarketServiceTypes)
                .Include(x=>x.MarketServicePayForms)
                .Include(x => x.MarketServiceConditions)
                .Include(x => x.MarketServicePayments)
                .Single();

            return model;
        }

        public IEnumerable<MarketServiceHistory> ServiceHistoryGetList(int serviceId)
        {
            var list = Db.MarketServiceHistory
                .Where(x => x.ServiceId== serviceId)
                .Include(x=>x.MarketServiceHistoryActions)
                .OrderByDescending(x=>x.CreateDate);

            return list;
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

        public IEnumerable<MarketServiceConditions> ServiceConditionsGetList()
        {
            var list = Db.MarketServiceConditions.OrderBy(x => x.Name);
            return list;
        }

        public IEnumerable<MarketServicePayments> ServicePaymentsGetList(int serviceId)
        {
            var list = Db.MarketServicePayments.Where(x => x.Enabled && x.ServiceId == serviceId)
                .OrderByDescending(x => x.Date);

            return list;
        }

        public void ServicePaymentCreate(MarketServicePayments model)
        {
            model.CreateDate = DateTimeOffset.Now;
            model.CreatorId = UserSid;
            model.Enabled = true;
            Db.MarketServicePayments.Add(model);
            Db.SaveChanges();

            ServiceBalanceCumCalculate(model.ServiceId);

            ServiceSaveHistory(model.ServiceId, "PAYMENTCREATED");
        }

        public void ServicePaymentDelete(int id)
        {
            var model = Db.MarketServicePayments.Single(x => x.Id == id);
            model.DeleteDate = DateTimeOffset.Now;
            model.DeleterId = UserSid;
            model.Enabled = false;

            Db.SaveChanges();

            ServiceBalanceCumCalculate(model.ServiceId);

            ServiceSaveHistory(model.ServiceId, "PAYMENTDELETED");
        }

        public void ServiceBalanceCumCalculate(int serviceId)
        {
            var service = Db.MarketServices.Single(x => x.Id == serviceId);

            var payments = Db.MarketServicePayments.Where(x => x.Enabled && x.ServiceId == serviceId);

            decimal? balanceSum = service.ServiceSum;

            if (payments.Any())
            {
                balanceSum -= payments.Sum(x => x.Sum);
            }

            service.BalanceSum = balanceSum;
            service.BalanceSumChangeDate = DateTimeOffset.Now;

            Db.SaveChanges();
        }
    }
}
