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
    [RoutePrefix("api/Page")]
    public class PageController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                var result = this.Repository.GetQuery<AD_PAGE>().FirstOrDefault(r => r.ID == id);
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
                var isAllPage = string.IsNullOrEmpty(searchText);
                var pages = this.Repository.GetQuery<AD_PAGE>()
                    .Where(r => isAllPage 
                    || r.Name.Contains(searchText)
                    || searchText.Equals(r.ID));

                var result = pages.GroupJoin(pages.Where(r => r.ParentID == null), a => a.ParentID, b => b.ID, (a, b) => new
                {
                    ID = a.ID,
                    Name = a.Name,
                    ParentID = a.ParentID,
                    OrdinalNumber = a.OrdinalNumber,
                    MenuFlg = a.MenuFlg,
                    ButtonFlg = a.ButtonFlg,
                    ParentName = b.Select(p => p.Name).FirstOrDefault()
                })
                .OrderBy(r=>r.ParentName)
                .ThenBy(r=>r.OrdinalNumber)
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
        public IHttpActionResult Put(string id, [FromBody]AD_PAGE dataRequest)
        {
            try
            {
                //validate custom data input
                string errorMsg = null;
                GetValidate(dataRequest, ref errorMsg);

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

                var dataItem = this.Repository.GetQuery<AD_PAGE>().FirstOrDefault(r => r.ID == id);
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
        public IHttpActionResult Post([FromBody] AD_PAGE dataRequest)
        {
            try
            {
                //validate custom data input
                string errorMsg = null;
                GetValidate(dataRequest, ref errorMsg);

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

                

                if (this.Repository.GetQuery<AD_PAGE>().Any(r => r.ID.Equals(dataRequest.ID)))
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
        public IHttpActionResult Saves(IEnumerable<AD_PAGE> dataRequest)
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

                AD_PAGE dataItem = null;
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

                    dataItem = this.Repository.GetQuery<AD_PAGE>().FirstOrDefault(r => r.ID == item.ID);
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

                var list = this.Repository.GetQuery<AD_PAGE>().Where(r => ids.Any(p => p == r.ID)).ToList();
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

        [HttpGet]
        [Route("GetMenu")]
        public IHttpActionResult GetMenu(string roleID)
        {
            try
            {
                var role = this.Repository.GetQuery<AD_ROLE>().FirstOrDefault(r => r.ID.Equals(roleID));
                if (role == null)
                {
                    return Json(new TResult()
                    {
                        Status = (int)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                var pages = this.Repository.GetQuery<AD_PAGE>();

                var pageItem = pages;
                if (role.DefaultFlg != true)
                {
                    pageItem = pages
                        .Join(this.Repository.GetQuery<AD_ROLE_PAGE>().Where(r => r.RoleID.Equals(roleID) & r.ActiveFlg == true)
                        , a => a.ID, b => b.PageID, (a, b) => a);
                }

                var result = pageItem.Where(r => r.ParentID != null && r.ButtonFlg != true && r.MenuFlg == true)
                    .GroupBy(r => r.ParentID)
                    .Select(r => new
                    {
                        ParentID = r.Key,
                        Childrens = r.OrderBy(p => p.OrdinalNumber).Select(p => new
                        {
                            p.Name,
                            p.ID
                        })
                    })
                    .Join(pages, a => a.ParentID, b => b.ID, (a, b) => new { a, b })
                    .ToList().Select(r => new MenuVM()
                    {
                        Name = r.b.Name,
                        Url = r.b.ID,
                        OrderIndex = r.b.OrdinalNumber,
                        Childrens = r.a.Childrens.Select(p => new MenuVM()
                        {
                            Name = p.Name,
                            Url = p.ID
                        }).ToList(),
                        IsButton = false
                    })
                    .Union(pages.Where(r => r.ButtonFlg == true)
                    .ToList().Select(r => new MenuVM()
                    {
                        Name = r.Name,
                        Url = r.ID,
                        OrderIndex = r.OrdinalNumber,
                        Childrens = new List<MenuVM>(),
                        IsButton = true
                    }))
                    .OrderBy(r => r.OrderIndex)
                    .ToList();

                if (result.Count == 0)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgAccessDenined)
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
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Check")]
        public IHttpActionResult Check(string roleID, string url)
        {
            try
            {
                var role = this.Repository.GetQuery<AD_ROLE>().FirstOrDefault(r => r.ID.Equals(roleID));
                if (role == null)
                {
                    return Json(new TResult()
                    {
                        Status = (int)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                if (role.DefaultFlg == true)
                {
                    return Json(new TResult()
                    {
                        Status = (int)EStatus.Ok
                    });
                }

                if (this.Repository.GetQuery<AD_ROLE_PAGE>()
                    .Any(r => r.RoleID.Equals(roleID) && r.PageID.Equals(url) && r.ActiveFlg == true))
                {
                    return Json(new TResult()
                    {
                        Status = (int)EStatus.Ok
                    });
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Fail,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgAccessDenined)
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                return BadRequest(e.Message);
            }
        }

        private void GetValidate(AD_PAGE item, ref string errorMsg)
        {
            if (string.IsNullOrEmpty(item.ID))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Label_page_ID)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(150, item.ID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Label_page_ID), 150), ". ");
                }
                if (CustomValidation.hasSpace(item.ID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorHasSpace), VNPTResources.Instance.Get(VNPTResources.ID.Label_page_ID)), ". ");
                }
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Label_page_Nm)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(150, item.Name))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Label_page_Nm), 150), ". ");
                }
            }
        }
    }
}
