using SumitQuotation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace SumitQuotation.Controllers
{
    public class QuotationAPIController : ApiController
    {
        QuotationDBEntities ent = new QuotationDBEntities();

        [System.Web.Http.HttpPost]
        public void Post(tbl_Quotation model)
        {
            int quotationId = 0;
            if (model.Quotation_Id > 0)
            {
                ent.Entry(model).State = EntityState.Modified;
                quotationId = model.Quotation_Id;
            }
            else
            {
                var quotation = ent.tbl_Quotation.Add(model);
                ent.SaveChanges();
                quotationId = model.Quotation_Id;
            }
            var details = ent.tbl_QuotationDetail.Where(s => s.QuotationId == model.Quotation_Id).ToList();
            ent.tbl_QuotationDetail.RemoveRange(details);

            if (model.tbl_QuotationDetail != null)
            {
                foreach (var item in model.tbl_QuotationDetail)
                {
                    if (item.IsDeleted == false)
                    {
                        tbl_QuotationDetail modelQC = new tbl_QuotationDetail();
                        modelQC.ItemId = item.ItemId;
                        modelQC.Quantity = item.Quantity;
                        modelQC.Rate = item.Rate;
                        modelQC.TotalAmount = item.TotalAmount;
                        modelQC.QuotationId = quotationId;
                        ent.tbl_QuotationDetail.Add(modelQC);
                    }
                }
            }
            ent.SaveChanges();
        }

        public void Delete(int id)
        {
            var quotation = ent.tbl_Quotation.Where(m => m.Quotation_Id == id).FirstOrDefault();

            var quotationdetail = ent.tbl_QuotationDetail.Where(m => m.QuotationId == id).ToList();

            if (quotation != null)
            {
                ent.tbl_Quotation.Remove(quotation);
            }

            if (quotationdetail != null)
            {
                ent.tbl_QuotationDetail.RemoveRange(quotationdetail);
            }
            ent.SaveChanges();
        }

        public tbl_Quotation Get(int id)
        {
            var quotation = ent.tbl_Quotation.Where(m => m.Quotation_Id == id).FirstOrDefault();
            var quotationdetail = from qd in ent.tbl_QuotationDetail
                                  join I in ent.tbl_Item on qd.ItemId equals I.Item_Id
                                  where qd.QuotationId == id
                                  select new
                                  {
                                      qd.QuotationDetail_Id,
                                      qd.QuotationId,
                                      qd.ItemId,
                                      qd.Rate,
                                      qd.Quantity,
                                      qd.TotalAmount,
                                      I.ItemName
                                  };
            List<tbl_QuotationDetail> lst = new List<tbl_QuotationDetail>();

            if (quotationdetail != null)
            {
                foreach (var item in quotationdetail)
                {
                    tbl_QuotationDetail modelQC = new tbl_QuotationDetail();
                    modelQC.ItemId = item.ItemId;
                    modelQC.Quantity = item.Quantity;
                    modelQC.Rate = item.Rate;
                    modelQC.TotalAmount = item.TotalAmount;
                    modelQC.QuotationId = item.QuotationId;
                    modelQC.QuotationDetail_Id = item.QuotationDetail_Id;
                    modelQC.ItemName = item.ItemName;
                    lst.Add(modelQC);
                }
            }
            if (lst != null)
            {
                quotation.tbl_QuotationDetail = lst;
            }
            return quotation;
        }
    }
}
