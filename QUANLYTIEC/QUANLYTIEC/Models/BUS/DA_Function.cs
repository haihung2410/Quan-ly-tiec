using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_Function : EfRepositoryBase<SYS_FUNCTION>
    {
        #region para
        private static volatile DA_Function _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_Function Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_Function();
                    }
                }
                return _instance;
            }
        }
        #endregion
        
    }
}