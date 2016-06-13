using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
    }
}