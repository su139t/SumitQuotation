using Newtonsoft.Json;
using SumitQuotation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Globalization;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Microsoft.Ajax.Utilities;

namespace SumitQuotation.Controllers
{
    public class QuotationController : Controller
    {
        QuotationDBEntities ent = new QuotationDBEntities();

        string baseaddress = "https://localhost:44363/api/";

        public ActionResult Create()
        {
            TempData["DetailData"] = null;
            var model = new tbl_Quotation();
            if (model.Quotation_Id == 0)
            {
                var date = DateTime.Today.Date;
                model.QuotationDate = date;
            }
            FillDropDown(model);
            GetQuotationNo(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(tbl_Quotation model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseaddress);
                model.tbl_QuotationDetail = (tbl_QuotationDetail[])TempData["DetailData"];
                string tempRes = JsonConvert.SerializeObject(model);
                var postTask = client.PostAsJsonAsync<tbl_Quotation>("QuotationAPI", model);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return Json("success");
                }
            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseaddress);

                var deleteTask = client.DeleteAsync("QuotationAPI/" + id.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            TempData["DetailData"] = null;
            var mdl = ent.tbl_Quotation.Where(a => a.Quotation_Id == id).FirstOrDefault();
            tbl_Quotation quotation = null;

            FillDropDown(mdl);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseaddress);
                
                var responseTask = client.GetAsync("QuotationAPI?id=" + id.ToString());
                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<tbl_Quotation>();
                    readTask.Wait();

                    quotation = readTask.Result;
                }
                TempData["DetailData"] = quotation.tbl_QuotationDetail;
            }

            return View("Create", quotation);
        }

        public ActionResult GetDetailData_FromAray(tbl_QuotationDetail[] detailArray)
        {
            TempData["DetailData"] = detailArray;
            return PartialView("_DetailList", detailArray);
        }

        private void FillDropDown(tbl_Quotation model)
        {
            List<SelectListItem> drpCustomer = ent.tbl_Customer.OrderBy(n => n.CustomerName).Select(n => new SelectListItem
            {
                Value = n.Customer_Id.ToString(),
                Text = n.CustomerName,
            }).ToList();

            if (model != null && model.CustomerId > 0)
            {
                ViewBag.CustomerId = new SelectList(drpCustomer, "Value", "Text", model.CustomerId.ToString());
            }
            else
            {
                ViewBag.CustomerId = new SelectList(drpCustomer, "Value", "Text");
            }

            List<SelectListItem> drpItem = ent.tbl_Item.OrderBy(m => m.ItemName).Select(m => new SelectListItem
            {
                Value = m.Item_Id.ToString(),
                Text = m.ItemName,
            }).ToList();

            ViewBag.ItemId = new SelectList(drpItem, "Value", "Text");
        }

        private void GetQuotationNo(tbl_Quotation model)
        {
            var lst = ent.tbl_Quotation.ToList();
            if (lst.Count == 0)
            {
                ViewBag.QuotationAutoNo = 1;
            }
            else
            {
                ViewBag.QuotationAutoNo = lst.Max(i => Convert.ToInt64(i.QuotationNo)) + 1;
            }
        }

        public ActionResult Index()
        {
            var lst = ent.Quotation_View.ToList();
            return View(lst);
        }

        [HttpPost]
        public ActionResult BulkDelete(int[] ids)
        {
            string result = string.Empty;
            try
            {
                if (ids != null && ids.Count() > 0)
                {
                    foreach (int id in ids)
                    {
                        var data = ent.tbl_Quotation.Where(e => e.Quotation_Id == id).FirstOrDefault();
                        var qdata = ent.tbl_QuotationDetail.Where(e => e.QuotationId == id).ToList();
                        if (data != null)
                        {
                            ent.tbl_Quotation.Remove(data);
                        }
                        if (qdata != null)
                        {
                            ent.tbl_QuotationDetail.RemoveRange(qdata);
                        }
                    }
                    ent.SaveChangesAsync();
                    result = "success";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(result);
        }
    }
}
