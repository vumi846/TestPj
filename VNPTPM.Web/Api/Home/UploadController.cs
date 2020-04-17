using MultipartDataMediaFormatter.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using VNPTPM.Model;
using VNPTPM.Model.Commons;
using VNPTPM.Model.Core;
using VNPTPM.Model.VM;
using VNPTPM.Web.Api.Base;

namespace VNPTPM.Web.Api.Home
{
    [RoutePrefix("api/Upload")]
    public class UploadController : BaseController
    {

        [HttpPost]
        [Route("UploadImage")]
        public IHttpActionResult UploadImage([FromBody]UploadVM dataRequest)
        {
            try
            {
                var server = HttpContext.Current.Server;
                var dirName = $"Upload/{dataRequest.AccountID.ToString()}/{dataRequest.BranchID.ToString()}";
                var root = $"{VNPTConfigs.DirRootUpload}/{dirName}";
                //server.MapPath(@"~/" + dirName);
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
                var result = dataRequest.Base64String.Substring(dataRequest.Base64String.LastIndexOf(',') + 1);
                var img = VNPTHelper.Base64ToImage(result);
                img.Save($"{root}/{dataRequest.FileName}");

                return Json(new TResult()
                {
                    Status = (int)EStatus.Ok,
                    Data = $"{dirName}/{dataRequest.FileName}"
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                return Json(new TResult()
                {
                    Status = (short)EStatus.Fail,
                    Msg = e.Message
                });
            }
        }

        [HttpPost]
        [Route("File")]
        public IHttpActionResult UploadFile([FromBody]UploadVM dataRequest)
        {
            try
            {
                var server = HttpContext.Current.Server;
                var dirName = $"Upload/{dataRequest.AccountID.ToString()}/{dataRequest.BranchID.ToString()}";
                // normalize URI string. if exists redundant slash, remove it
                if (dirName.ElementAt(dirName.Length - 1).Equals('/'))
                    dirName = dirName.Substring(0, dirName.Length - 1);

                var root = $"{VNPTConfigs.DirRootUpload}/{dirName}";
                //server.MapPath(@"~/" + dirName);
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }

                Byte[] bytes = Convert.FromBase64String(dataRequest.Base64String);
                File.WriteAllBytes($"{root}/{dataRequest.FileName}", bytes);
                return Json(new TResult()
                {
                    Status = (int)EStatus.Ok,
                    Data = $"{dirName}/{dataRequest.FileName}"
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                return Json(new TResult()
                {
                    Status = (short)EStatus.Fail,
                    Msg = e.Message
                });
            }
        }

        //api/Upload/Image
        [HttpPost]
        [Route("Image")]
        public IHttpActionResult Image([FromBody]UploadVM dataRequest)
        {
            try
            {
                string imageNm = null;
                //convert GUID to string with AccountID
                string AccountIDText = dataRequest.AccountID.ToString();

                //get image
                if (!string.IsNullOrEmpty(dataRequest.Base64String))
                {
                    string pathSaveImage = string.Format(Constants.LBM_PATH_SAVE_FILE_TMP, AccountIDText);
                    imageNm = VNPTHelper.SaveBase64ToImage(dataRequest.Base64String, pathSaveImage);
                    if (imageNm.Equals(EStatusFile.OverWeight.ToString()))
                    {
                        string errorMsg = string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorImageUrlMaxSize), VNPTHelper.ConvertBytesToMegabytes(
                                Constants.LBM_MAX_SIZE_IMAGE));
                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Data = errorMsg
                        });
                    } else if (imageNm.Equals(EStatusFile.Base64Incorrect.ToString())) {
                        string errorMsg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorBase64Incorrect);
                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = errorMsg
                        });
                    }
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = imageNm
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.Repository, e.Message);
                return Json(new TResult()
                {
                    Status = (short)EStatus.Fail,
                    Msg = e.Message
                });
            }
        }

        //api/Upload/Media
        [HttpPost]
        [Route("Media")]
        public async Task<IHttpActionResult> MediaUpload()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Data = StatusCode(HttpStatusCode.UnsupportedMediaType)
                    });
                }

                var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
                List<string> listNameMedia = new List<string>();
                var AccountID = HttpContext.Current.Request.Form.GetValues("AccountID")[0];
                var TypeData = HttpContext.Current.Request.Form.GetValues("TypeData")[0];
                var Extension = HttpContext.Current.Request.Form.GetValues("Extension")[0];

                foreach (var stream in filesReadToProvider.Contents)
                {
                    // Getting of content as byte[], picture name and picture type
                    var fileBytes = await stream.ReadAsByteArrayAsync();
                    var fileName = stream.Headers.ContentDisposition.FileName;
                    if (fileBytes != null && fileName != null)
                    {
                        //var contentType = stream.Headers.ContentType.MediaType;
                        string mediaNm = VNPTHelper.createFileFromBytes(fileBytes, TypeData, AccountID, Extension, fileName);
                        if (mediaNm.Equals(EStatusFile.OverWeight.ToString()))
                        {
                            string errorMsg = "";
                            if (TypeData.Equals(Constants.LBM_TYPE_DETECT_IMAGE))
                                {
                                errorMsg = string.Format(VNPTResources.Instance.Get(
                                VNPTResources.ID.MsgErrorImageUrlMaxSize), VNPTHelper.ConvertBytesToMegabytes(
                                    Constants.LBM_MAX_SIZE_IMAGE));
                            }
                            else if (TypeData.Equals(Constants.LBM_TYPE_DETECT_VIDEO))
                            {
                                errorMsg = string.Format(VNPTResources.Instance.Get(
                                VNPTResources.ID.MsgErrorVideoUrlMaxSize), VNPTHelper.ConvertBytesToMegabytes(
                                    Constants.LBM_MAX_SIZE_VIDEO));
                            }
                            return Json(new TResult()
                            {
                                Status = (short)EStatus.Fail,
                                Msg = errorMsg
                            });
                        } else
                        {
                            listNameMedia.Add(mediaNm);
                        }
                    }
                }
                
                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = new
                    {
                        MediaNm = listNameMedia,
                        DirMedia = string.Format(Constants.LBM_PATH_SAVE_FILE_TMP, AccountID)
                    }
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.Repository, e.Message);
                return Json(new TResult()
                {
                    Status = (short)EStatus.Fail,
                    Msg = e.Message
                });
            }
        }

        [HttpPost]
        [Route("MediaWeb")]
        public async Task<IHttpActionResult> MediaWeb(string accountID, string typeData)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Data = StatusCode(HttpStatusCode.UnsupportedMediaType)
                    });
                }

                var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
                List<string> listNameMedia = new List<string>();
                var Extension = "";

                foreach (var stream in filesReadToProvider.Contents)
                {
                    // Getting of content as byte[], picture name and picture type
                    var fileBytes = await stream.ReadAsByteArrayAsync();
                    var fileName = stream.Headers.ContentDisposition.FileName;
                    if (fileBytes != null && fileName != null)
                    {
                        Extension = Path.GetExtension((string)JsonConvert.DeserializeObject(fileName, typeof(string))).Replace(".", "");
                        fileName = Path.GetFileName((string)JsonConvert.DeserializeObject(fileName, typeof(string)));
                        string mediaNm = VNPTHelper.createFileFromBytes(fileBytes, typeData, accountID, Extension, fileName);
                        if (mediaNm.Equals(EStatusFile.OverWeight.ToString()))
                        {
                            string errorMsg = "";
                            if (typeData.Equals(Constants.WO_TYPE_DETECT_IMAGE))
                            {
                                errorMsg = string.Format(VNPTResources.Instance.Get(
                                VNPTResources.ID.MsgErrorImageUrlMaxSize), VNPTHelper.ConvertBytesToMegabytes(
                                    Constants.LBM_MAX_SIZE_IMAGE));
                            }
                            else if (typeData.Equals(Constants.WO_TYPE_DETECT_VIDEO))
                            {
                                errorMsg = string.Format(VNPTResources.Instance.Get(
                                VNPTResources.ID.MsgErrorVideoUrlMaxSize), VNPTHelper.ConvertBytesToMegabytes(
                                    Constants.LBM_MAX_SIZE_VIDEO));
                            }
                            else if (typeData.Equals(Constants.WO_TYPE_DETECT_FILES))
                            {
                                errorMsg = string.Format(VNPTResources.Instance.Get(
                                VNPTResources.ID.MsgErrorFileUrlMaxSize), VNPTHelper.ConvertBytesToMegabytes(
                                    Constants.LBM_MAX_SIZE_FILES));
                            }
                            return Json(new TResult()
                            {
                                Status = (short)EStatus.Fail,
                                Msg = errorMsg
                            });
                        }
                        else
                        {
                            listNameMedia.Add(mediaNm);
                        }
                    }
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = new
                    {
                        MediaNm = listNameMedia,
                        DirMedia = string.Format(Constants.WO_PATH_SAVE_FILE_TMP, accountID)
                    }
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                return Json(new TResult()
                {
                    Status = (short)EStatus.Fail,
                    Msg = e.Message
                });
            }
        }

        

        [HttpGet]
        [Route("GetDirectory")]
        public IHttpActionResult GetDirectory()
        {
            try
            {
                var result = VNPTConfigs.DirRootUpload;
                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = result
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                return Json(new TResult()
                {
                    Status = (short)EStatus.Fail,
                    Msg = e.Message
                });
            }
        }

    }
}
