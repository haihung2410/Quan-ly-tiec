﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Transactions;
using System.Linq.Dynamic;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_PARTY_PRODUCT_MATERIAL : EfRepositoryBase<TBL_PARTY_PRODUCT_MATERIAL>
    {
        #region para
        private static volatile DA_PARTY_PRODUCT_MATERIAL _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_PARTY_PRODUCT_MATERIAL Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_PARTY_PRODUCT_MATERIAL();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method

        public List<TBL_PARTY_PRODUCT_MATERIAL> getEntityBasePartyId(int partyId)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<TBL_PARTY_PRODUCT_MATERIAL> result = new List<TBL_PARTY_PRODUCT_MATERIAL>();
                    result = context.TBL_PARTY_PRODUCT_MATERIAL.Where(n => n.PartyID == partyId).ToList();
                    return result;
                }
            }
            catch (Exception ex) { return null; }
        }
        public SYS_PARTY_TYPE GetPartyType(int partyId)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    SYS_PARTY_TYPE getData = new SYS_PARTY_TYPE();
                    getData = (from p in context.TBL_PARTY
                               join pt in context.SYS_PARTY_TYPE on p.PartyType equals pt.PartyTypeID
                               where p.PartyID == partyId
                               select pt).FirstOrDefault();
                    return getData;
                }
            }
            catch (Exception ex) { return null; }
        }
        #endregion


        public List<object> GetViewReportForDatatablePagging(int start, int length, string sortColumn, string sortColumnDir, int partyID)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<object> getData = new List<object>();
                    //check data
                    sortColumn = string.IsNullOrWhiteSpace(sortColumn) ? "" : sortColumn;
                    sortColumnDir = string.IsNullOrWhiteSpace(sortColumnDir) ? "" : sortColumnDir;
                    //excute query 
                    getData = (from p in context.TBL_PARTY_PRODUCT_MATERIAL
                               join m in (from ma in context.TBL_MATERIAL
                                          join u in context.TBL_UOM on ma.UOMID equals u.UOMID into lsU
                                          from u in lsU.DefaultIfEmpty() 
                                          select new { ma.MaterialID, ma.MaterialName, u.UOMName })
                                          on p.MaterialID equals m.MaterialID into lsM
                               from m in lsM.DefaultIfEmpty() 
                               join v in context.TBL_VENDOR on p.VendorID equals v.VendorID into lsV
                               from v in lsV.DefaultIfEmpty()
                               where p.PartyID == partyID                            
                                select new { m.MaterialName, p.Quantity, m.UOMName, p.UnitPrice, v.VendorName, p.IsDelivery })
                               .ToList<object>();
                    return getData;
                }
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }

    }
}