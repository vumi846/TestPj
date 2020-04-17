using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Commons
{
    public class Constants
    {
        public static readonly string FormatDate = "yyyyMMddHHmmss";
        public static readonly string LBM_FORMAT_SAVE_NAME_FILE = "yyyyMMddHHmmssFFF";
        public static readonly int LBM_SCORE_GET_WITH_KM = 6371;
        public static readonly int LBM_SCORE_GET_WITH_MI = 3959;
        public static readonly int LBM_NUM_RECORDS_GET_LIMIT = 10;
        public static readonly int LBM_NUM_RECORDS_GET_OFFSET = 0;
        public static readonly int LBM_NUM_RECORDS_GET_MIN = 3;
        public static readonly string LBM_SETTING_RADIUS = "0.5";//KM
        public static readonly string LBM_SETTING_MERCHANDISE = null;//select all
        public static readonly string LBM_PREFIX_CODE_BRANCH = "BRA";
        public static readonly string LBM_PREFIX_FILE_COPY = "COPY";
        public static readonly string LBM_TYPE_DETECT_VIDEO = "video";
        public static readonly string LBM_TYPE_DETECT_IMAGE = "image";
        public static readonly string LBM_TYPE_DETECT_FILES = "files";
        public static readonly string LBM_PATH_SAVE_FILES = "Upload/Account/{0}/";
        public static readonly string LBM_PATH_SAVE_VIDEO_DEAL = "Upload/Account/{0}/Video/Deal/";
        public static readonly string LBM_PATH_SAVE_VIDEO_BRANCH = "Upload/Account/{0}/Video/Branch/";
        public static readonly string LBM_PATH_SAVE_IMAGE_DEAL = "Upload/Account/{0}/Image/Deal/";
        public static readonly string LBM_PATH_SAVE_IMAGE_BRANCH = "Upload/Account/{0}/Image/Branch/";
        public static readonly string LBM_PATH_SAVE_IMAGE_ITEM = "Upload/Account/{0}/Image/Item/";
        public static readonly string LBM_PATH_SAVE_IMAGE_ACCOUNT = "Upload/Account/{0}/Image/Avatar/";
        public static readonly string LBM_PATH_SAVE_IMAGE_MERCHANDISE = "Upload/Merchandise/";
        public static readonly string LBM_PATH_SAVE_IMAGE_DISCOUNT = "Upload/Discount/";
        public static readonly string LBM_PATH_SAVE_FILE_TMP = "Upload/Tmp/Account/{0}/";
        public static readonly string LBM_ICON_DEFAUL_WITH_MANY_MER = "icon_all.png";
        public static readonly string LBM_FIREBASE_SERVER_KEY = "AAAAKXl-zlo:APA91bHjlgVmkiYIfuwiMWyCLx2Bj7IhoBNVyQoQXLi5St9HAaEcXrbMIEWIiAjrqjq3pNMfv3qxyAvDdwu2GfRWkEzNLMpws0JWcyS5lQJ43KCancMLDn2zQxB_SiVGBHrcbIh50IFf";
        public static readonly string LBM_FIREBASE_SENDER_ID = "178132012634";
        public static readonly string LBM_FIREBASE_URL = "https://fcm.googleapis.com/fcm/send";
        public static readonly string LBM_HISTORY_ACTION_DEFAULT = null;
        public static readonly string LBM_HISTORY_ACTION_SEEN_DEAL = "Seen deal";
        public static readonly string LBM_HISTORY_ACTION_SEEN_BRANCH = "Seen branch";
        public static readonly string LBM_HISTORY_ACTION_SEARCH = "Search {0}: {1}";
        public static readonly string LBM_NAME_ACCOUNT_DEAFAULT = "Customer";
        public static readonly string LBM_NAME_SYSTEM_PAYMENT = "System LBM";
        public static readonly string LBM_NAME_SYSTEM_PAYMENT_VNPAY = "System VNPAY";
        public static readonly string LBM_VALUE_DETECT_INPUT_BLANK_STRING = "none";
        public static readonly int LBM_VALUE_DETECT_INPUT_BLANK_INT = -1;
        public static readonly DateTime LBM_VALUE_DETECT_INPUT_BLANK_DATE = new DateTime(0001,01,01);
        public static readonly long LBM_MAX_SIZE_FILES = 1073741824; //UNIT: 1073741824 BYTES => 1GB
        public static readonly long LBM_MAX_SIZE_VIDEO = 314572800; //UNIT: 314572800 BYTES => 300MB
        public static readonly long LBM_MAX_SIZE_IMAGE = 3145728; //UNIT: 3145728 BYTES => 3MB

        //PROJECT WORKING
        public static readonly string WO_FORMAT_SAVE_NAME_FILE = "yyyyMMddHHmmssFFF";
        public static readonly DateTime WO_VALUE_DETECT_INPUT_BLANK_DATE = new DateTime(0001, 01, 01);
        public static readonly string WO_PATH_SAVE_FILES = "Upload/Account/{0}/";
        public static readonly string WO_PATH_SAVE_FILE_TMP = "Upload/Tmp/Account/{0}/";
        public static readonly string WO_TYPE_DETECT_VIDEO = "video";
        public static readonly string WO_TYPE_DETECT_IMAGE = "image";
        public static readonly string WO_TYPE_DETECT_FILES = "files";
        public static readonly long WO_MAX_SIZE_FILES = 1073741824; //UNIT: 1073741824 BYTES => 1GB
        public static readonly long WO_MAX_SIZE_VIDEO = 314572800; //UNIT: 314572800 BYTES => 300MB
        public static readonly long WO_MAX_SIZE_IMAGE = 3145728; //UNIT: 3145728 BYTES => 3MB
        public static readonly int WO_ACCTION_TYPE_RESOLVE = 0;
        public static readonly int WO_ACCTION_TYPE_RATING = 1;
    }
}
