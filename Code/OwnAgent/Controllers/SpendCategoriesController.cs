using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Models;
using Data.Services;
using OwnAgent.Objects;

namespace OwnAgent.Controllers
{
    [Authorize]
    public class SpendCategoriesController : BaseController
    {
        // GET: SpendCategories
        public ActionResult Index()
        {
            var list = SpendService.Instance(UserSid).GetCategoryList();

            return View(list);
        }

        public ActionResult SpendCategoryDelete(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            try
            {
                SpendService.Instance(UserSid).SpendCategoryDelete(id.Value);
            }
            catch (ArgumentException ex)
            {
                return Json(new {responseText=ex.Message });
            }

            return Json(new { });
        }

        [HttpGet]
        public ActionResult New()
        {
            var model = new SpendCategory();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SpendCategory model)
        {
            SpendService.Instance(UserSid).SpendCategoryCreate(model);

            return Json(new{});
        }

        [HttpPost]
        public ActionResult CatMerge(int[] cats, int nameId)
        {
            SpendService.Instance(UserSid).SpendCategoryMerge(cats, nameId);
            return Json(new {});
        }

        public ActionResult Merge()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = SpendService.Instance(UserSid).SpendCategoryGet(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SpendCategory model)
        {
            SpendService.Instance(UserSid).SpendCategoryEdit(model);

            return Json(new { });
        }

        [HttpPost]
        public ActionResult SpendCategoryOrderUp(int id)
        {
            SpendService.Instance(UserSid).SpendCategoryOrderUp(id);

            return Json(new { });
        }
        [HttpPost]
        public ActionResult SpendCategoryOrderDown(int id)
        {
            SpendService.Instance(UserSid).SpendCategoryOrderDown(id);

            return Json(new { });
        }
    }
}