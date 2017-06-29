﻿using System;
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

        public struct DBCPackageLifetime
        {
            public const int Month = 12;
            public const int Year = 1;
        }

        public struct DBCDateFormat
        {
            public const string ddMMyyyy = "dd-MM-yyyy";
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
    }
}
