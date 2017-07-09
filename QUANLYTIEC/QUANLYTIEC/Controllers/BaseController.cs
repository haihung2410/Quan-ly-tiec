using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QUANLYTIEC.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/
        public  bool CheckPermission()
        {
            return (Session["UserID"] == null) ? false : true;
        }
        public string Encrypte(string pString)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedDataBytes = md5Hasher.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(pString));
            string sEncryptPass = Convert.ToBase64String(hashedDataBytes);
            return sEncryptPass;
        }
    }
}