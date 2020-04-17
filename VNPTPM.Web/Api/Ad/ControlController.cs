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
    [RoutePrefix("api/Control")]
    public class ControlController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                var result = this.Repository.GetQuery<AD_CONTROL>().FirstOrDefault(r => r.ID == id);
                if (result is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
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

        [HttpGet]
        [Route("Search")]
        public IHttpActionResult Search(string searchText)
        {
            try
            {
                var isAllControl = string.IsNullOrEmpty(searchText);
                var result = this.Repository.GetQuery<AD_CONTROL>()
                    .Where(r => isAllControl
                    || r.Name.Contains(searchText)
                    || r.ID.Contains(searchText))
                .OrderBy(r => r.Name)
                .ToList();

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

        [HttpPut]
        public IHttpActionResult Put(string id, [FromBody]AD_CONTROL dataRequest)
        {
            try
            {
                //validate custom data input
                string errorMsg = null;
                if (string.IsNullOrEmpty(dataRequest.ID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Control_ID)), ". ");
                }
                else
                {
                    if (!CustomValidation.maxLength(50, dataRequest.ID))
                    {
                        errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Control_ID), 50), ". ");
                    }
                    if (CustomValidation.hasSpace(dataRequest.ID))
                    {
                        errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorHasSpace), VNPTResources.Instance.Get(VNPTResources.ID.Control_ID)), ". ");
                    }
                }

                if (string.IsNullOrEmpty(dataRequest.Name))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Control_Name)), ". ");
                }
                else
                {
                    if (!CustomValidation.maxLength(150, dataRequest.Name))
                    {
                        errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Control_Name), 150), ". ");
                    }
                }
                //validate custom data input
                if (errorMsg != "" && errorMsg != null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = errorMsg
                    });
                }

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

                var dataItem = this.Repository.GetQuery<AD_CONTROL>().FirstOrDefault(r => r.ID == id);
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                dataItem = dataRequest.Clone();
                dataItem.ID = id;

                this.Repository.Update(dataItem);
                this.VNPTLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(dataItem));
                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgUpdateDataSuccess),
                        VNPTResources.Instance.Get(VNPTResources.ID.Label_obj_page)),
                    Data = dataItem
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
        public IHttpActionResult Post([FromBody] AD_CONTROL dataRequest)
        {
            try
            {
                //validate custom data input
                string errorMsg = null;
                if (string.IsNullOrEmpty(dataRequest.ID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Control_ID)), ". ");
                }
                else
                {
                    if (!CustomValidation.maxLength(50, dataRequest.ID))
                    {
                        errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Control_ID), 50), ". ");
                    }
                    if (CustomValidation.hasSpace(dataRequest.ID))
                    {
                        errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorHasSpace), VNPTResources.Instance.Get(VNPTResources.ID.Control_ID)), ". ");
                    }
                }

                if (string.IsNullOrEmpty(dataRequest.Name))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Control_Name)), ". ");
                }
                else
                {
                    if (!CustomValidation.maxLength(150, dataRequest.Name))
                    {
                        errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Control_Name), 150), ". ");
                    }
                }
                //validate custom data input
                if (errorMsg != "" && errorMsg != null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = errorMsg
                    });
                }

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



                if (this.Repository.GetQuery<AD_CONTROL>().Any(r => r.ID.Equals(dataRequest.ID)))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorIsExists), dataRequest.ID)
                    });
                }

                var dataItem = dataRequest.Clone();

                this.Repository.Add(dataItem);
                this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgCreateDataSuccess),
                        VNPTResources.Instance.Get(VNPTResources.ID.Label_obj_page)),
                    Data = dataItem
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
        [Route("Saves")]
        public IHttpActionResult Saves(IEnumerable<AD_CONTROL> dataRequest)
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

                AD_CONTROL dataItem = null;
                foreach (var item in dataRequest)
                {
                    string errorMsg = null;
                    if (string.IsNullOrEmpty(item.ID))
                    {
                        errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                                VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Control_ID)), ". ");
                    }
                    else
                    {
                        if (!CustomValidation.maxLength(50, item.ID))
                        {
                            errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                                VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Control_ID), 50), ". ");
                        }
                        if (CustomValidation.hasSpace(item.ID))
                        {
                            errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                                VNPTResources.ID.MsgErrorHasSpace), VNPTResources.Instance.Get(VNPTResources.ID.Control_ID)), ". ");
                        }
                    }

                    if (string.IsNullOrEmpty(item.Name))
                    {
                        errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                                VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Control_Name)), ". ");
                    }
                    else
                    {
                        if (!CustomValidation.maxLength(150, item.Name))
                        {
                            errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                                VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Control_Name), 150), ". ");
                        }
                    }

                    dataItem = this.Repository.GetQuery<AD_CONTROL>().FirstOrDefault(r => r.ID == item.ID);
                    if (dataItem is null)
                    {
                        dataItem = item.Clone();
                        this.Repository.Add(dataItem);
                        this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
                    }
                    else
                    {
                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorIsExists), item.ID)
                        });
                    }
                }

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgUpdateDataSuccess),
                        VNPTResources.Instance.Get(VNPTResources.ID.Label_obj_page)),
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
        [Route("Deletes")]
        public IHttpActionResult Deletes([FromBody] DeleteVM dataRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(dataRequest.IDList))
                {
                    return Json(new TResult()
                    {
                        Status = (int)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorRequiredDeleteID)
                    });
                }
                string[] ids = (string[])JsonConvert.DeserializeObject(dataRequest.IDList, typeof(string[]));

                var list = this.Repository.GetQuery<AD_CONTROL>().Where(r => ids.Any(p => p == r.ID)).ToList();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var autoNumber = list[i];

                    this.Repository.Delete(autoNumber);
                    this.VNPTLogs.Write(this.Repository, EAction.Delete, JsonConvert.SerializeObject(autoNumber));
                }

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgDeleteOk)
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
        [Route("Check")]
        public IHttpActionResult Check(CheckControlVM dataRequest)
        {
            try
            {
                var page = !string.IsNullOrEmpty(dataRequest.PageUrl)
                    ? this.Repository.GetQuery<AD_PAGE>().FirstOrDefault(r => r.ID.Equals(dataRequest.PageUrl))
                    : null;

                var role = !string.IsNullOrEmpty(dataRequest.RoleID)
                    ? this.Repository.GetQuery<AD_ROLE>().FirstOrDefault(r => r.ID.Equals(dataRequest.RoleID))
                    : null;

                if (page is null || role is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail
                    });
                }

                var controlSQL = this.Repository.GetQuery<AD_CONTROL>();

                SaveRoleControl(controlSQL, dataRequest.Controls, page, role.ID);

                var isDefault = role.DefaultFlg == true;

                var rolePage = this.Repository.GetQuery<AD_ROLE_PAGE>()
                    .FirstOrDefault(r => r.RoleID.Equals(role.ID) && r.PageID.Equals(page.ID));

                string[] controlIDs = null;
                if (rolePage != null && !string.IsNullOrEmpty(rolePage.Value))
                {
                    controlIDs = (string[])JsonConvert.DeserializeObject(rolePage.Value, typeof(string[]));
                }

                if (controlIDs != null && controlIDs.Length > 0)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Ok,
                        Data = controlSQL.GroupJoin(controlIDs, a => a.ID, b => b, (a, b) => new { a, b })
                            .Select(r => new
                            {
                                r.a.ID,
                                ActiveFlg = r.b.FirstOrDefault() != null
                            }).ToList()
                    });
                }

                var result = controlSQL
                    .Select(r => new
                    {
                        r.ID,
                        ActiveFlg = true
                    }).ToList();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = result
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                return BadRequest(e.Message);
            }
        }

        private void SaveRoleControl(IQueryable<AD_CONTROL> controlSQL, List<string> controls, AD_PAGE page, string roleID)
        {
            var flag = false;
            var pageID = page.ID;
            var rolePageValue = JsonConvert.SerializeObject(controls);
            if (string.IsNullOrEmpty(page.Value) || !page.Value.Equals(rolePageValue))
            {
                flag = true;
                page.Value = rolePageValue;
                this.Repository.Update(page);
            }
            var controlDBs = controlSQL.ToList();
            
            foreach (var item in controls)
            {
                if (!controlDBs.Any(r => r.ID.Equals(item)))
                {
                    flag = true;
                    this.Repository.Add(new AD_CONTROL()
                    {
                        ID = item
                    });
                }
            }

            var rolePage = this.Repository.GetQuery<AD_ROLE_PAGE>().FirstOrDefault(r => r.RoleID.Equals(roleID) && r.PageID.Equals(pageID));
            if (rolePage == null)
            {
                rolePage = new AD_ROLE_PAGE()
                {
                    RoleID = roleID,
                    PageID = pageID,
                    Value = rolePageValue,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    ActiveFlg = true
                };

                flag = true;
                this.Repository.Add(rolePage);
            }

            if (flag)
                this.Repository.UnitOfWork.SaveChanges();
        }
    }
}
