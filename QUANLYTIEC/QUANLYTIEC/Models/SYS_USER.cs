//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QUANLYTIEC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SYS_USER
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string CardNumber { get; set; }
        public Nullable<int> RoleID { get; set; }
        public Nullable<bool> IsAdmin { get; set; }
    }
}
