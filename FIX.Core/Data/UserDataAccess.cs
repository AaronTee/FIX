using System;
using System.Collections.Generic;

namespace FIX.Core.Data
{
    public class UserDataAccess : BaseEntity
    {
        public bool AllowView { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowApprove { get; set; }
        public string UserRoleID { get; set; }
        public string MenuID { get; set; }
    }
}
