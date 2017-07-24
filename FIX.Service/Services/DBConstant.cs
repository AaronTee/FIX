using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIX.Service
{
    public static class DBConstant
    {
        public struct DBCGender
        {
            public const string Male = "M";
            public const string Female = "F";
            public const string Other = "O";
        }

        public struct DBCRole
        {
            public enum Id
            {
                Admin = 3,
                User = 4,
                SuperAdmin = 5
            }

            public const string Admin = "Admin";
            public const string User = "User";
            public const string SuperAdmin = "SuperAdmin";
        }

        public struct DBCPackageLifetime
        {
            public const int Month = 12;
            public const int Year = 1;
        }

        public struct DBCDateFormat
        {
            public const string ddMMyyyy = "dd-MM-yyyy";
            public const string ddMMMyyyy = "dd-MMM-yyyy";
            public const string MMMyyyy = "MMM/yyyy";
            public const string ddMMMyyyyHHmmsstt = "dd-MMM-yyyy hh:mm:ss tt";
        }

        public struct EventType
        {
            public const string Added = "Added";
            public const string Modified = "Modified";
            public const string Deleted = "Deleted";
        }

        //without going to db retrieve status.
        public enum EStatus
        {
            Active = 1,
            Deactivated = 2,
            Pending = 3,
            Approved = 4,
            Void = 5,
            Activated = 6,
        }

        public static string GetDescription(this EStatus s){
            return Enum.GetName(typeof(EStatus), s);
        }

        public const int MAX_REFERRAL_TREE_LEVEL = 5;
        public const int MAX_REFERRAL_TREE_SEARCH_LEVEL = 25;
        public const string DEFAULT_TIMEZONEID = "Singapore Standard Time";
    }
}
