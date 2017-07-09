using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models;
using QUANLYTIEC.Models.BUS;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data.SqlClient;
using System.Transactions;

namespace QUANLYTIEC.Controllers
{
    public class PartyController : BaseController
    {
        #region method is used for load page
        public ActionResult PartyScheduler()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            return View();
        }
        public ActionResult ViewProductMaterial(string id)
        {
            //if (!CheckPermission())
            //    return RedirectToAction("Index", "Login");
            if (!string.IsNullOrWhiteSpace(id) && id.All(Char.IsDigit))
            {
                int Id = Convert.ToInt32(id);
                TBL_PARTY item = DA_Party.Instance.GetById(Id);
                if (item != null)
                {
                    ViewBag.ProductParty = DA_PartyProduct.Instance.getEntityBasePartyId(Id);
                    return View(item);
                }
            }
            return RedirectToAction("PartyScheduler", "Party");
        }
        public ActionResult MaterialParty()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            return View();
        }
        public ActionResult RegisterParty()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.ComboboxPartyType = DA_PartyType.Instance.GetAll();
            ViewBag.ComboboxFood = DA_Food.Instance.loadEntityForCombobox(true);
            ViewBag.ComboboxService = DA_Service.Instance.loadEntityForCombobox(true);
            return View();
        }
        public ActionResult EditParty(string id)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            if (!string.IsNullOrWhiteSpace(id) && id.All(Char.IsDigit))
            {
                int Id = Convert.ToInt32(id);
                TBL_PARTY item = DA_Party.Instance.GetById(Id);
                if (item != null)
                {
                    ViewBag.ComboboxPartyType = DA_PartyType.Instance.GetAll();
                    ViewBag.ComboboxFood = DA_Food.Instance.loadEntityForCombobox(false);
                    ViewBag.ComboboxService = DA_Service.Instance.loadEntityForCombobox(false);
                    ViewBag.PartyFood = DA_PartyProduct.Instance.getEntityBasePartyId(Id);
                    ViewBag.PartyService = DA_PartyService.Instance.getEntityBasePartyId(Id);
                    return View(item);
                }
            }
            return RedirectToAction("PartyScheduler", "Party");
        }
        #endregion

        #region method is used for ajax

        #region ajax excute data
        /// <summary>
        /// ajax add or update entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddOrUpdateEntity(TBL_PARTY objectParty, List<TBL_PARTY_PRODUCT> lsObjectMeal, List<TBL_PARTY_SERVICE> lsObjecService, bool isEdit, int[][] objectDate)
        {

            //if (objectParty.BookingDate != null && objectParty.PartyDate != null && objectParty.NegativeDate != null && ((isEdit && objectParty.PartyID > 0) || !isEdit))
            //{
            try
            {
                if (objectDate.Length >= 3)
                {
                    objectParty.BookingDate = new DateTime(objectDate[0][2], objectDate[0][1], objectDate[0][0]);
                    objectParty.PartyDate = new DateTime(objectDate[1][2], objectDate[1][1], objectDate[1][0], objectDate[1][3], objectDate[1][4], 0);
                    objectParty.NegativeDate = new DateTime(objectDate[2][2], objectDate[2][1], objectDate[2][0]);
                    objectParty.DepositDate = ((objectDate.Length > 3) ? new DateTime(objectDate[3][2], objectDate[3][1], objectDate[3][0]) : objectParty.DepositDate);
                    objectParty.UserCreate = Convert.ToInt32(Session["UserID"].ToString().All(Char.IsDigit) ? Session["UserID"] : 0);
                    if (isEdit)
                        return Json(DA_Party.Instance.ProcesseActionUpdateFormAllEntity(objectParty, lsObjectMeal, lsObjecService) ? 1 : 0);
                    else
                        return Json(DA_Party.Instance.ProcesseActionInsertFormAllEntity(objectParty, lsObjectMeal, lsObjecService) ? 1 : 0);
                }

            }
            catch (Exception ex) { }
            //}
            return Json(0);
        }

        /// <summary>
        /// ajax method delete entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteEntity(int id)
        {
            try
            {
                if (id != 0)
                    return Json(DA_Party.Instance.ProcesseActionDeleteFormAllEntity(id) ? 1 : 0);
            }
            catch (Exception ex) { }
            return Json(0);
        }
        [HttpPost]
        public JsonResult AddOrUpdatePartyProductMaterial(List<TBL_PARTY_PRODUCT_MATERIAL> lsPartyProductMaterialInsert, List<TBL_PARTY_PRODUCT_MATERIAL> lsPartyProductMaterialUpdate)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    if (lsPartyProductMaterialInsert != null && lsPartyProductMaterialInsert.Count > 0)
                    {
                        DA_PartyProductMaterial.Instance.Insert(lsPartyProductMaterialInsert);
                    }
                    if (lsPartyProductMaterialUpdate != null && lsPartyProductMaterialUpdate.Count > 0)
                    {
                        DA_PartyProductMaterial.Instance.Update(lsPartyProductMaterialUpdate);
                    }
                    scope.Complete();
                    return Json(1);
                }
            }
            catch (Exception ex) { return Json(0); }
        }
        #endregion

        #region fill data
        /// <summary>
        /// get list entity for calendar
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPartyForFullCalendar()
        {
            try
            {
                var dateFrom = Request.Form.GetValues("start").FirstOrDefault();
                var dateTo = Request.Form.GetValues("end").FirstOrDefault();
                DateTime DateFrom, DateTo;
                if ((dateFrom != null && DateTime.TryParse((dateFrom ?? "").ToString(), out DateFrom)) && (dateTo != null && DateTime.TryParse((dateTo ?? "").ToString(), out DateTo)))
                {
                    return Json(DA_Party.Instance.GetEntityBasePartyDate(DateFrom, DateTo), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex) { }
            return Json(null);
        }
        /// <summary>
        /// ajax for datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPartyDatatableIndex()
        {
            try
            {
                #region get para from view
                var isSearchDate = Request.Form.GetValues("isSearchDate").FirstOrDefault();
                var valueDate = Request.Form.GetValues("valueDate").FirstOrDefault();
                DateTime ValueDate = new DateTime();
                Boolean IsSearchDate = DateTime.TryParse(valueDate, out ValueDate) ? Convert.ToBoolean(isSearchDate) : false;
                //jQuery DataTables Param
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                //Find paging info
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                int orderColumn = Convert.ToInt32(Request.Form.GetValues("order[0][column]").FirstOrDefault());
                //Find order columns info
                var sortColumn = Request.Form.GetValues("columns[" + orderColumn + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //find search columns info
                var search = Request.Form["search[value]"];
                //page
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt16(start) : 0;
                #endregion

                long recordsTotal = 0;

                List<object> data = DA_Party.Instance.getPartyForDatatablePagging(search.ToString(), skip, length != null ? Convert.ToInt32(length) : 0, sortColumn, sortColumnDir, IsSearchDate, ValueDate);
                recordsTotal = DA_Party.Instance.countAllPartyFlowSearch(search.ToString());
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }
        [HttpPost]
        public JsonResult GetUnitCostAndProfitAmountBaseProductID(int id)
        {
            try
            {
                if (id != 0)
                    return Json(DA_Party.Instance.GetUnitCostAndProfitAmountBaseProductID(id));
            }
            catch (Exception ex) { }
            return Json(0);
        }
        [HttpPost]
        public JsonResult MethodFillDataForDatatablesProductMaterial(int productId, int partyId)
        {
            try
            {
                if (productId != 0)
                    return Json(DA_Party.Instance.MethodFillDataForDatatablesProductMaterial(productId, partyId));
            }
            catch (Exception ex) { }
            return Json(0);
        }
        [HttpPost]
        public JsonResult GetIdAndCountProductInParty(int partyId)
        {
            try
            {
                if (partyId != 0)
                    return Json(DA_Party.Instance.GetIdAndCountProductInParty(partyId));
            }
            catch (Exception ex) { }
            return Json(0);
        }
        #endregion

        #endregion
    }
}