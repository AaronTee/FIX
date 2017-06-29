using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIX.Web.Models
{
    public class AlertModel
    {
        public struct AlertStyles
        {
            public const string Success = "success";
            public const string Information = "info";
            public const string Warning = "warning";
            public const string Danger = "danger";
        }

        public const string TempDataKey = "TempDataAlerts";
        public const string TempDataKeyAutoDisappear = "AutoDisappear";
        public string AlertStyle { get; set; }
        public string Message { get; set; }
        public bool Dismissable { get; set; }
        public bool AutoDisappear { get; set; }
        public AlertStyles Style { get; }

    }//end class

    public class RequiredActionModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string SubmitButtonDescription { get; set; }
        public string DataName { get; set; }
        public object Data { get; set; }
        public FormMethod FormMethod { get; set; }
    }
}