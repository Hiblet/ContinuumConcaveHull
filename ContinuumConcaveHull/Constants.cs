using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuumConcaveHull
{
    public class Constants
    {
        // NOTE: THESE STRINGS SHOULD MATCH THE ACCESSORS IN XmlInputConfiguration.cs
        public static string GROUPFIELDKEY = "ContinuumConcaveHullGroupField";
        public static string GROUPFIELDNAMESKEY = "ContinuumConcaveHullGroupFieldNames"; // Last seen field

        public static string XFIELDKEY = "ContinuumConcaveHullXField";
        public static string XFIELDNAMESKEY = "ContinuumConcaveHullXFieldNames";

        public static string YFIELDKEY = "ContinuumConcaveHullYField";
        public static string YFIELDNAMESKEY = "ContinuumConcaveHullYFieldNames";

        public static string CONCAVITYKEY = "ContinuumConcaveHullConcavity";
        public static string SCALEFACTORKEY = "ContinuumConcaveHullScaleFactor";


        // Default Values
        public static string DEFAULTGROUPFIELD = "";
        public static string DEFAULTGROUPFIELDNAMES = "";

        public static string DEFAULTXFIELD = "X_Field";
        public static string DEFAULTXFIELDNAMES = "X_Field";

        public static string DEFAULTYFIELD = "Y_Field";
        public static string DEFAULTYFIELDNAMES = "Y_Field";

        public static string DEFAULTCONCAVITY = "90";
        public static string DEFAULTSCALEFACTOR = "NULL";

        public static int LARGEOUTPUTFIELDSIZE = 4096;
        public static int INT64_FIELDSIZE = 8;
        public static int INT32_FIELDSIZE = 4;
        public static int DOUBLE_FIELDSIZE = 8; // 8 Bytes
    }
}
