using GroupDocs.Conversion.Config;
using GroupDocs.Conversion.Handler;
using GroupDocs.Conversion.Options.Save;
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using VNPTPM.Model.Core;
using VNPTPM.Model.VM;
using static GroupDocs.Conversion.Options.Save.ImageSaveOptions;

namespace VNPTPM.Model.Commons
{
    public class VNPTHelper
    {
        public static string GetServiceName()
        {
            var request = HttpContext.Current.Request;
            var filePath = request.FilePath;

            return string.IsNullOrEmpty(filePath) ? string.Empty : filePath;
        }
        public static string GetUploadPath()
        {
            var url = "";
            var request = HttpContext.Current.Request;
            if (request.IsSecureConnection)
                url = "https://";
            else
                url = "http://";

            url += $"{request["HTTP_HOST"]}/";

            return url;
        }
        
        public static string GetUnsigned(string str)
        {

            string[] vietNamChar = new string[]
               {
                    "aAeEoOuUiIdDyY",
                    "áàạảãâấầậẩẫăắằặẳẵ",
                    "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                    "éèẹẻẽêếềệểễ",
                    "ÉÈẸẺẼÊẾỀỆỂỄ",
                    "óòọỏõôốồộổỗơớờợởỡ",
                    "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                    "úùụủũưứừựửữ",
                    "ÚÙỤỦŨƯỨỪỰỬỮ",
                    "íìịỉĩ",
                    "ÍÌỊỈĨ",
                    "đ",
                    "Đ",
                    "ýỳỵỷỹ",
                    "ÝỲỴỶỸ"
               };

            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < vietNamChar.Length; i++)
            {
                for (int j = 0; j < vietNamChar[i].Length; j++)
                    str = str.Replace(vietNamChar[i][j], vietNamChar[0][i - 1]);
            }
            return str;
        }

        public static string GetPrefix(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            name = GetUnsigned(name);

            var result = "";
            var array = name.Split(new string[] { " " }, StringSplitOptions.None).ToList();
            if (array.Count > 0)
            {
                foreach (var item in array)
                {
                    if (item.Length > 0)
                        result += item.Substring(0, 1);
                }
            }

            return result.ToLower();
        }

        public static long GetMaxAutoNumber(IRepository repository, string funcName)
        {
            AD_AUTONUMBER ob = repository.GetByKey<AD_AUTONUMBER>(funcName);
            if (ob == null)
            {
                return 0;
            }

            return ob.Current.GetValueOrDefault();
        }

        public static string GetAutoNumber(IRepository repository, string funcName, long? maxNumber)
        {
            string kq = "";
            AD_AUTONUMBER ob = repository.GetByKey<AD_AUTONUMBER>(funcName);

            if (ob != null)
            {
                ob = (AD_AUTONUMBER)ob.Clone();
                //if (ob.Reset == true)
                //{
                //    AD_CONFIG tsNgayDangNhap = repository.GetQuery<AD_CONFIG>().FirstOrDefault(o => funcName.Equals(o.ID));
                //    if (tsNgayDangNhap == null)
                //    {
                //        tsNgayDangNhap = new AD_CONFIG();
                //        tsNgayDangNhap.ID = funcName;
                //        tsNgayDangNhap.Name = DateTime.Today.ToString("yyyyMMdd");

                //        repository.Add(tsNgayDangNhap);

                //        ob.Current = 0;
                //    }
                //    else
                //    {
                //        if (!DateTime.Today.ToString("yyyyMMdd").Equals(tsNgayDangNhap.Name))
                //        {
                //            tsNgayDangNhap.Name = DateTime.Today.ToString("yyyyMMdd");

                //            repository.Update(tsNgayDangNhap);

                //            ob.Current = 0;
                //        }
                //    }
                //}

                if (ob.Step == 0)
                {
                    ob.Step = 1;
                }

                ob.Current = (maxNumber != null ? maxNumber : ob.Current) + ob.Step;
                if (string.IsNullOrEmpty(ob.Format))
                {
                    ob.Format = string.Empty;
                    kq = (ob.Current).ToString();
                }
                else
                {
                    //[BN-][$yyyy][-][00000]
                    List<string> lst = new List<string>();
                    string str = "";
                    for (int i = 0; i < ob.Format.Length; i++)
                    {
                        if (ob.Format[i] == '[') str = "";
                        else if (ob.Format[i] == ']') lst.Add(str);
                        else str += ob.Format[i];
                    }
                    if (lst.Count == 0) lst.Add(str);
                    foreach (string s in lst)
                    {
                        if (s.Contains("$"))
                        {
                            string f = s.Replace("$", "");
                            kq += DateTime.Now.ToString(f);
                        }
                        else if (s.Any(o => o != '0') == false)
                        {
                            kq += ((double)ob.Current).ToString(s);
                        }
                        else kq += s;
                    }
                }

                repository.Update(ob);
            }
            else
            {
                ob = new AD_AUTONUMBER()
                {
                    ID = funcName,
                    Format = "",
                    Current = 1,
                    Step = 1
                };

                repository.Add(ob);

                kq += ((double)ob.Current).ToString(ob.Format);
            }
            return kq;
        }

        public static string GetNumberPixel(string pValue)
        {
            var result = "";
            foreach (Char c in pValue.Trim())
            {
                if (';'.Equals(c))
                {
                    continue;
                }

                if (Char.IsDigit(c))
                {
                    result += c;
                }
            }
            return result.Trim();
        }

        public static int ParseInt(string value)
        {
            int result = 0;
            int.TryParse(value, out result);

            return result;
        }

        public static double ParseDouble(string value)
        {
            //replace "." => "," to convert correctly
            value = value.Replace(".",",");
            double result = 0;
            double.TryParse(value, out result);

            return result;
        }

        public static string DoubleToString(double value)
        {
            //replace "," => "." to convert correctly
            string result = "";
            result = value.ToString("R").Replace(",", ".");

            return result;
        }

        public static decimal ParseDecimal(string value)
        {
            decimal.TryParse(value, out decimal result);

            return result;
        }

        public static string GetUserName()
        {
            var request = HttpContext.Current.Request;
            var userName = request.Headers["UserName"];

            return string.IsNullOrEmpty(userName) ? string.Empty : userName;
        }
        
        public static Image Base64ToImage(string base64String)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);
                return image;
            }
            catch { }
            return null;

        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static string ByteArrayImageToBase64(byte[] byteArrayIn)
        {
            try
            {

                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                using (MemoryStream m = new MemoryStream())
                {
                    returnImage.Save(m, returnImage.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }

            }
            catch
            {
                return "";
            }

        }
                
        public static void Compress(DirectoryInfo directorySelected, string directoryPath, string fileName)
        {
            var zipName = $"{directoryPath}/{fileName}.zip";
            if (File.Exists(zipName))
            {
                File.Delete(zipName);
            }

            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(@"" + directoryPath, @"" + fileName);
                zip.Save(@"" + $"{directoryPath}/{fileName}.zip");
            }
        }

        public static List<int> CommaJsonStringToIntList(string _s)
        {
            List<int> list = (List<int>)JsonConvert.DeserializeObject(_s, typeof(List<int>));

            return (list);
        }

        public static string SaveBase64ToImage(string base64String, string pathSaveImage)
        {
            var server = HttpContext.Current.Server;
            var folderBanner = $"{VNPTConfigs.DirRootUpload}/{pathSaveImage}";
            //server.MapPath(String.Concat("~", "/", pathSaveImage));
            string imageNm = String.Concat("IMG", DateTime.Now.ToString(Constants.LBM_FORMAT_SAVE_NAME_FILE), ".jpeg");

            if(!string.IsNullOrEmpty(base64String))
            {
                Image image = VNPTHelper.Base64ToImage(base64String);
                if (image != null)
                {
                    bool exists = System.IO.Directory.Exists(folderBanner);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(folderBanner);
                    string fullPath = String.Concat(folderBanner, imageNm);
                    image.Save(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    FileInfo file = new FileInfo(fullPath);
                    if (file.Exists == true)
                    {
                        if (file.Length > Constants.LBM_MAX_SIZE_IMAGE)
                        {
                            return EStatusFile.OverWeight.ToString();
                        }
                    }

                    return imageNm;
                }
                else
                {
                    return EStatusFile.Base64Incorrect.ToString();
                }
            } else {
                return null;
            }
        }

        public static void DeleteFile(string pathSaveFile, string fileNm)
        {
            var server = HttpContext.Current.Server;
            var folderFile = $"{VNPTConfigs.DirRootUpload}/{pathSaveFile}";
            //server.MapPath(String.Concat("~", "/", pathSaveFile));
            string fullPath = String.Concat(folderFile, fileNm);
            FileInfo file = new FileInfo(fullPath);
            if(file.Exists == true)
            {
                File.Delete(fullPath);
            }
        }

        public static void DeleteListFileRemove(string pathSaveFile, string listStringOld, string listStringNew)
        {
            var server = HttpContext.Current.Server;
            var folderFile = $"{VNPTConfigs.DirRootUpload}/{pathSaveFile}";
            //server.MapPath(String.Concat("~", "/", pathSaveFile));

            List<string> listOld = new List<string>();
            if (!string.IsNullOrEmpty(listStringOld))
            {
                listOld = (List<string>)JsonConvert.DeserializeObject(listStringOld, typeof(List<string>));
            }
            List<string> listNew = new List<string>();
            if (!string.IsNullOrEmpty(listStringNew) && listStringNew != Constants.LBM_VALUE_DETECT_INPUT_BLANK_STRING)
            {
                listNew = (List<string>)JsonConvert.DeserializeObject(listStringNew, typeof(List<string>));
            }

            foreach (string oldFile in listOld)
            {
                if (!listNew.Contains(oldFile))
                {
                    if (isExistFile(pathSaveFile, oldFile))
                    {
                        DeleteFile(pathSaveFile, oldFile);
                    }
                }
            }
        }

        public static bool isExistFile(string pathSaveFile, string fileNm)
        {
            if (string.IsNullOrEmpty(System.IO.Path.Combine(pathSaveFile, fileNm)))
            {
                var server = HttpContext.Current.Server;
                var folderFile = $"{VNPTConfigs.DirRootUpload}/{pathSaveFile}";
                //server.MapPath(String.Concat("~", "/", pathSaveFile));
                FileInfo file = new FileInfo(System.IO.Path.Combine(folderFile, fileNm));
                if (file.Exists == true)
                {
                    return true;
                } else
                {
                    return false;
                }
            }

            return false;
        }

        public static string CoppyFile(string sourcePath, string targetPath, string fileName, string newFileName = null)
        {
            var server = HttpContext.Current.Server;
            sourcePath = $"{VNPTConfigs.DirRootUpload}/{sourcePath}";
            //server.MapPath(String.Concat("~", "/", sourcePath));
            targetPath = $"{VNPTConfigs.DirRootUpload}/{targetPath}";
            //server.MapPath(String.Concat("~", "/", targetPath));

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            if(!string.IsNullOrEmpty(newFileName))
            {
                fileName = newFileName;
            }
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location:
            // Create a new target folder. 
            // If the directory already exists, this method does not create a new directory.
            System.IO.Directory.CreateDirectory(targetPath);
            
            // To copy all the files in one directory to another directory.
            // Get the files in the source folder. (To recursively iterate through
            // all subfolders under the current directory, see
            // "How to: Iterate Through a Directory Tree.")
            // Note: Check for target path was performed previously
            //       in this code example.
            if (System.IO.Directory.Exists(sourcePath))
            {
                // To copy a file to another location and 
                // overwrite the destination file if it already exists.
                System.IO.File.Copy(sourceFile, destFile, true);
            }
            else
            {
                return EStatusFile.PathNotExist.ToString();
            }

            return fileName;
        }

        public static bool IsNullGuid(Guid? value)
        {
            if (value == null || value == Guid.Empty)
            {
                return true;
            }

            return false;
        }

        public static string sendNotification(string serverKey, string tokenDevice, string SenderID, 
            string titleInput, string bodyInput, Guid dealIDInput, Guid branchIDInput)
        {
            WebRequest tRequest = WebRequest.Create(Constants.LBM_FIREBASE_URL);
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
            //Sender Id - From firebase project setting  
            tRequest.Headers.Add(string.Format("Sender: id={0}", SenderID));
            tRequest.ContentType = "application/json";
            var payload = new
            {
                to = tokenDevice,
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = bodyInput,
                    title = titleInput,
                },
                data = new
                {
                    dealID = dealIDInput,
                    branchID = branchIDInput
                }
            };

            string postbody = JsonConvert.SerializeObject(payload).ToString();
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                return sResponseFromServer;
                            }
                    }
                }
            }

            return "error";
        }

        

        public static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        public static string createFileFromBytes(byte[] fileBytes, string typeData, string AccountID, string extFile, string fileNameOld)
        {
            if (fileBytes != null && AccountID != null)
            {
                var server = HttpContext.Current.Server;
                string pathSaveMedia = string.Format(Constants.WO_PATH_SAVE_FILE_TMP, AccountID);
                var folderMedia = $"{VNPTConfigs.DirRootUpload}/{pathSaveMedia}";
                //    server.MapPath(String.Concat("~", "/", pathSaveMedia));
                string mediaNm = String.Concat(DateTime.Now.ToString(Constants.WO_FORMAT_SAVE_NAME_FILE),"_",fileNameOld);
                string fullPath = String.Concat(folderMedia, mediaNm);
                bool exists = System.IO.Directory.Exists(folderMedia);
                if (!exists)
                    System.IO.Directory.CreateDirectory(folderMedia);
                using (Stream sw = File.OpenWrite(fullPath))
                {
                    sw.Write(fileBytes, 0, fileBytes.Length);
                    sw.Close();
                }
                FileInfo file = new FileInfo(fullPath);
                if (typeData.Equals(Constants.WO_TYPE_DETECT_IMAGE))
                {
                    if (file.Length > Constants.WO_MAX_SIZE_IMAGE)
                    {
                        return EStatusFile.OverWeight.ToString();
                    }
                }
                else if (typeData.Equals(Constants.WO_TYPE_DETECT_VIDEO))
                {
                    if (file.Length > Constants.WO_MAX_SIZE_VIDEO)
                    {
                        return EStatusFile.OverWeight.ToString();
                    }
                }
                else if (typeData.Equals(Constants.WO_TYPE_DETECT_FILES))
                {
                    if (file.Length > Constants.WO_MAX_SIZE_FILES)
                    {
                        return EStatusFile.OverWeight.ToString();
                    }
                }

                return mediaNm;
            }
            else
            {
                return null;
            }
        }

        public static string RD6()
        {
            Random rd = new Random();

            string n1 = rd.Next(0, 9).ToString();
            string n2 = rd.Next(1, 7).ToString();
            string n3 = rd.Next(3, 9).ToString();
            string n4 = rd.Next(0, 7).ToString();
            string n5 = rd.Next(4, 8).ToString();
            string n6 = rd.Next(2, 6).ToString();

            return n1 + n2 + n3 + n4 + n5 + n6;
        }

        public static bool isExistMerchandise(string MerchandiseListInput, string MerchandiseListDB)
        {
            List<int> MerListInput = new List<int>();
            if (!string.IsNullOrEmpty(MerchandiseListInput))
            {
                MerListInput = (List<int>)JsonConvert.DeserializeObject(MerchandiseListInput, typeof(List<int>));
            }
            else
            {
                return true;
            }
            List<int> MerListDB = new List<int>();
            if (!string.IsNullOrEmpty(MerchandiseListDB))
            {
                MerListDB = (List<int>)JsonConvert.DeserializeObject(MerchandiseListDB, typeof(List<int>));
            }
            else
            {
                return true;
            }

            foreach (int mer in MerListInput)
            {
                if (MerListDB.Contains(mer) == true)
                {
                    return true;
                }
            }

            return false;
        }

        public static int getPeriodOfMonth(int month)
        {
            int period = 0;
            switch (month) {
                case 1:
                case 2:
                case 3:
                    period = (int)EStatusPeriodOfYear.season1;
                    break;
                case 4:
                case 5:
                case 6:
                    period = (int)EStatusPeriodOfYear.season2;
                    break;
                case 7:
                case 8:
                case 9:
                    period = (int)EStatusPeriodOfYear.season3;
                    break;
                case 10:
                case 11:
                case 12:
                    period = (int)EStatusPeriodOfYear.season4;
                    break;
                default:
                    break;
            }

            return period;
        }

        public static DateTime getLastDayOfPeriod(int period, int year)
        {
            DateTime lastDate = new DateTime(1000, 1, 1);
            switch (period)
            {
                case (int)EStatusPeriodOfYear.season1:
                    lastDate = new DateTime(year, 3, 31);
                    break;
                case (int)EStatusPeriodOfYear.season2:
                    lastDate = new DateTime(year, 6, 30);
                    break;
                case (int)EStatusPeriodOfYear.season3:
                    lastDate = new DateTime(year, 9, 30);
                    break;
                case (int)EStatusPeriodOfYear.season4:
                    lastDate = new DateTime(year, 12, 31);
                    break;
                default:
                    break;
            }

            return lastDate;
        }
    }

    public class VNPTClone<T> where T : class, new()
    {
        public static T Clone(T param)
        {
            var result = new T();
            var properties = param.GetType().GetProperties();
            if (properties != null)
            {
                foreach (var p in properties)
                {
                    var value = p.GetValue(param);
                    if (p.PropertyType.Name == "String")
                    {
                        if (!"none".Equals(value))
                        {
                            result.GetType().GetProperty(p.Name).SetValue(result, value);
                        }
                    }
                }
            }

            return result;
        }
    }
}
