using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VNPTPM.Model;
using VNPTPM.Model.Commons;
using VNPTPM.Model.Core;
using VNPTPM.Model.Validate;
using VNPTPM.Model.VM;
using VNPTPM.Web.Api.Base;

namespace VNPTPM.Web.Api.Ad
{
    [RoutePrefix("api/RolePage")]
    public class RolePageController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                var role = this.Repository.GetQuery<AD_ROLE>().FirstOrDefault(r => r.ID.Equals(id));
                if (role is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                var rolePages = this.Repository.GetQuery<AD_PAGE>()
                    .GroupJoin(this.Repository.GetQuery<AD_ROLE_PAGE>().Where(r => r.RoleID.Equals(id)),
                    a => a.ID, b => b.PageID, (a, b) => new
                    {
                        AD_PAGE = a,
                        AD_ROLE_PAGE = b.FirstOrDefault()
                    })
                    .ToList();

                var controlSQL = this.Repository.GetQuery<AD_CONTROL>();
                var result = new UserRoleVM
                {
                    RoleID = id,
                    Items = new List<UserRole>()
                };

                List<UserRole> controls = null;
                string[] controlByPages = null;
                IQueryable<AD_CONTROL> controlPageSQL = null;
                string[] controlIDs = null;
                UserRole item = null;
                foreach (var rolePage in rolePages)
                {
                    controls = null;
                    controlByPages = null;
                    controlPageSQL = null;
                    controlIDs = null;
                    if (rolePage != null && !string.IsNullOrEmpty(rolePage.AD_PAGE?.Value))
                    {
                        controlByPages = (string[])JsonConvert.DeserializeObject(rolePage.AD_PAGE?.Value, typeof(string[]));
                        if (controlByPages != null)
                        {
                            controlPageSQL = controlSQL.Join(controlByPages, a => a.ID, b => b, (a, b) => a);
                        }
                    }

                    if (controlByPages != null)
                    {
                        if (rolePage != null && !string.IsNullOrEmpty(rolePage.AD_ROLE_PAGE?.Value))
                        {
                            controlIDs = (string[])JsonConvert.DeserializeObject(rolePage.AD_ROLE_PAGE?.Value, typeof(string[]));

                            if (controlIDs != null)
                            {
                                controls = controlPageSQL.GroupJoin(controlIDs, a => a.ID, b => b, (a, b) => new
                                {
                                    a,
                                    b
                                })
                                .Select(r => new UserRole()
                                {
                                    ID = r.a.ID,
                                    Name = r.a.Name,
                                    ActiveFlg = r.b.FirstOrDefault() != null
                                }).ToList();
                            }
                        }
                        else
                        {
                            controls = controlPageSQL
                                .Select(r => new UserRole()
                                {
                                    ID = r.ID,
                                    Name = r.Name,
                                    ActiveFlg = false
                                }).ToList();
                        }

                    }

                    item = new UserRole()
                    {
                        ID = rolePage.AD_PAGE.ID,
                        Name = rolePage.AD_PAGE.Name,
                        ActiveFlg = rolePage.AD_ROLE_PAGE != null && rolePage.AD_ROLE_PAGE.ActiveFlg.GetValueOrDefault(),
                        Items = controls
                    };

                    result.Items.Add(item);
                }

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

        [HttpPost]
        public IHttpActionResult Saves(IEnumerable<AD_ROLE_PAGE> dataRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var model = ModelState;
                    string msg = VNPTResources.Instance.Get(model);

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = msg
                    });
                }

                AD_ROLE_PAGE dataItem = null;
                foreach (var item in dataRequest)
                {
                    string errorMsg = null;
                    GetValidate(item, ref errorMsg);
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        return Json(new TResult()
                        {
                            Status = (int)EStatus.Fail,
                            Msg = errorMsg
                        });
                    }

                    dataItem = this.Repository.GetQuery<AD_ROLE_PAGE>().FirstOrDefault(r => r.RoleID.Equals(item.RoleID) && r.PageID.Equals(item.PageID));
                    if (dataItem is null)
                    {
                        dataItem = item.Clone();

                        this.Repository.Add(dataItem);
                        this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
                    }
                    else
                    {
                        dataItem = item.Clone();

                        this.Repository.Update(dataItem);
                        this.VNPTLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(dataItem));
                    }
                }

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgSaveOk),
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

        private void GetValidate(AD_ROLE_PAGE item, ref string errMsg)
        {
            if (string.IsNullOrEmpty(item.RoleID))
            {
                errMsg = string.Concat(errMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Label_RolePage_RoleID)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(50, item.RoleID))
                {
                    errMsg = string.Concat(errMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Label_RolePage_RoleID), 50), ". ");
                }
            }

            if (string.IsNullOrEmpty(item.PageID))
            {
                errMsg = string.Concat(errMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Label_RolePage_PageID)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(50, item.PageID))
                {
                    errMsg = string.Concat(errMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Label_RolePage_PageID), 50), ". ");
                }
            }
        }
    }
}
