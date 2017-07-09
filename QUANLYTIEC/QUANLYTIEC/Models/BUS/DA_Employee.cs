﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_Employee : EfRepositoryBase<TBL_EMPLOYEE>
    {
        #region para
        private static volatile DA_Employee _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_Employee Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_Employee();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method

        #region For datatable
        /// <summary>
        /// get Employee for datatable flow pagging
        /// </summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortColumnDir"></param>
        /// <returns></returns>
        public List<object> getDEmployeeForDatatablePagging(string search, int start, int length, string sortColumn, string sortColumnDir)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<object> getData = new List<object>();
                    //check data
                    search = String.IsNullOrWhiteSpace(search) ? "" : search;
                    sortColumn = String.IsNullOrWhiteSpace(sortColumn) ? "" : sortColumn;
                    sortColumnDir = String.IsNullOrWhiteSpace(sortColumnDir) ? "" : sortColumnDir;
                    //excute query 
                    getData = (from u in context.TBL_EMPLOYEE
                               join d in context.TBL_DEPARTMENT on u.DepartmentID equals d.DepartmentID into ds
                               from d in ds.DefaultIfEmpty()
                               where search == "" || u.FullName.Contains(search) || d.DepartmentName.Contains(search)
                               select new { u.EmployeeID, u.FullName, u.PhoneNumber, DepartmentName = d.DepartmentName, u.IsCalculatePayroll, u.IsCalAttendance, u.IsLeaveOff }).OrderBy((sortColumn == "" && sortColumnDir == "") ? "EmployeeID asc" : sortColumn + " " + sortColumnDir).Skip(start).Take(length).ToList<object>();
                    return getData;
                }
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        /// <summary>
        /// get all Department flow search
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int countAllEmployeeFlowSearch(string search)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    int result = 0;
                    //check data
                    search = String.IsNullOrWhiteSpace(search) ? "" : search;
                    //excute query 
                    result = (from u in context.TBL_EMPLOYEE
                              join d in context.TBL_DEPARTMENT on u.DepartmentID equals d.DepartmentID into ds
                              from d in ds.DefaultIfEmpty()
                              where search == "" || u.FullName.Contains(search) || d.DepartmentName.Contains(search)
                              select u).Count();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion

        #endregion
    }
}