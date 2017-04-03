using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Models;
using Data.Services;
using OwnAgent.Objects;
using OwnAgent.ViewModels;

namespace OwnAgent.Controllers
{
    [Authorize]
    public class MarketController : BaseController
    {
        // GET: Market
        public ActionResult Index(string state)
        {
            bool needRedirect = false;
            if (String.IsNullOrEmpty(state))
            {
                state = "active";
                needRedirect = true;
            }

            if (needRedirect)
                return RedirectToAction("Index", new {state});

            return View();
        }

        public ActionResult List(string state)
        {
            bool? stateIsActive = null;
            if (state == "active")
            {
                stateIsActive = true;
            }
            else if (state == "disabled")
            {
                stateIsActive = false;
            }
            else
            {
                stateIsActive = null;
            }

            var list = MarketService.Instance(UserSid).ServiceGetList(stateIsActive);

            return View(list);
        }

        [HttpGet]
        public ActionResult New()
        {
            ViewBag.TypesList = MarketService.Instance(UserSid).ServiceTypesGetList();
            ViewBag.PayFormsList = MarketService.Instance(UserSid).ServicePayFormsGetList();
            return View();
        }

        [HttpPost]
        public ActionResult New(MarketServices model)
        {
            int id = MarketService.Instance(UserSid).ServiceCreate(model);
            return RedirectToAction("Card", new {id});
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            ViewBag.TypesList = MarketService.Instance(UserSid).ServiceTypesGetList();
            ViewBag.PayFormsList = MarketService.Instance(UserSid).ServicePayFormsGetList();
            var model = MarketService.Instance(UserSid).ServiceGet(id.Value);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(MarketServices model)
        {
            MarketService.Instance(UserSid).ServiceUpdate(model);
            return RedirectToAction("Card", new { id = model.Id });
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            MarketService.Instance(UserSid).ServiceDelete(id.Value);
            return RedirectToAction("Card", new { id = id.Value });
        }

        public ActionResult Card(int? id)
        {
            if (!id.HasValue) return HttpNotFound();

            var model = MarketService.Instance(UserSid).ServiceGet(id.Value);
            return View(model);
        }

        public ActionResult History(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var list = MarketService.Instance(UserSid).ServiceHistoryGetList(id.Value);
            return View(list);
        }

        public ActionResult ConditionChange(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var model = MarketService.Instance(UserSid).ServiceGet(id.Value);
            ViewBag.ConditionsList = MarketService.Instance(UserSid).ServiceConditionsGetList();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConditionChangeSave(int? sid, int? cid, string comment)
        {
            if (!sid.HasValue || !cid.HasValue) return HttpNotFound();
            MarketService.Instance(UserSid).ServiceSetConditionAndSaveHistory(sid.Value, cid.Value, comment);

            return Json(new {});
        }

        [HttpPost]
        public ActionResult Condition(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var service = MarketService.Instance(UserSid).ServiceGet(id.Value);
            //var model = new KeyValuePair<string, string>(service.MarketServiceConditions?.Name, service.ConditionComment);
            var model = new MarketServiceConditionViewModel();
            model.Name = service.MarketServiceConditions?.Name;
            model.Comment = service.ConditionComment;
            return View("Condition", model: model);
        }

        [HttpPost]
        public ActionResult Balance(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var service = MarketService.Instance(UserSid).ServiceGet(id.Value);
            //var model = new KeyValuePair<decimal?, DateTimeOffset?>(service.BalanceSum, service.BalanceSumChangeDate);

            var model = new MarketServiceBalanceViewModel();
            model.ChangeDate = service.BalanceSumChangeDate;
            model.ServiceSum = service.ServiceSum;
            model.PaymentSum = service.MarketServicePayments.Where(x => x.Enabled).Sum(x => x.Sum);
            model.BalanceSum = service.BalanceSum;

            return View("Balance", model: model);
        }

        public ActionResult PaymentList(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var list = MarketService.Instance(UserSid).ServicePaymentsGetList(id.Value);

            return View(list);
        }

        public ActionResult PaymentNew()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult PaymentNewSave(MarketServicePayments model)
        {
            MarketService.Instance(UserSid).ServicePaymentCreate(model);

            return Json(new { });
        }

        [HttpPost]
        public ActionResult PaymentDelete(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            MarketService.Instance(UserSid).ServicePaymentDelete(id.Value);
            return Json(new {});
        }
    }
}