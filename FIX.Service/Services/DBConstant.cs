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
                Admin = 1,
                User = 2,
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
            public const string PS_yyyyMMddhhmmss = "yyyyMMddhhmmss";
        }

        public struct DBCEventType
        {
            public const string Added = "Added";
            public const string Modified = "Modified";
            public const string Deleted = "Deleted";
        }

        public struct DBCCurrency
        {
            public const string USD = "USD";
        }

        public struct MatchingBonusSetting
        {
            public const int Level = 3;
            public const decimal StartingRate = 0.3M;
            public const decimal DecreaseRate = 0.1M;
        }

        public enum EOperator
        {
            ADD, DEDUCT, MULTIPLY, DIVIDE
        }

        public struct DBCDocSequence
        {
            public enum EDocSequenceId
            {
                Interest_Return = 1,
                Matching_Bonus = 2,
                Withdrawal = 3
            }
        }

        //without going to db retrieve status.
        public enum EStatus
        {
            Void = 0,
            Approved = 1,
            Deactivated = 2,
            Pending = 3,
            Active = 4,
            Expired = 5
        }

        public enum EJState
        {
            Unknown, Success, Failed, NoWallet
        }

        public enum ETransactionType
        {
            Interest_Return, Matching_Bonus, Withdrawal
        }

        public static string GetDescription(this ETransactionType type)
        {
            return Enum.GetName(type.GetType(), type);
        }

        public static string GetDescription(this EStatus s){
            return Enum.GetName(typeof(EStatus), s);
        }

        public const int SUPER_ADMIN_REFERRAL_ID = 0;
        public const int MAX_REFERRAL_TREE_LEVEL = 5;
        public const int MAX_REFERRAL_TREE_SHOW_PACKAGE = 1;
        public const int MAX_REFERRAL_TREE_SEARCH_LEVEL = 25;
        public const string DEFAULT_TIMEZONEID = "Singapore Standard Time";

        //Images
        public const string UploadReceiptPrefix = "RCP_";

        public const int MAX_CONCURRENCY_ITERATION = 5;

    }
}
