using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using VNPTPM.Model;
using VNPTPM.Model.Commons;
using VNPTPM.Model.Core;
using VNPTPM.Model.Validate;
using VNPTPM.Model.VM;
using VNPTPM.Web.Api.Base;

namespace VNPTPM.Web.Api.M
{
    [RoutePrefix("api/Unit")]
    public class UnitController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var result = this.Repository.GetQuery<M_UNIT>()
                .OrderBy(r => r.ID)
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

        [HttpGet]
        [Route("Search")]
        public IHttpActionResult Search(string searchText)
        {
            try
            {
                var isAll = string.IsNullOrEmpty(searchText);
                var result = this.Repository.GetQuery<M_UNIT>()
                    .Where(r => (isAll
                    || r.Name.Contains(searchText)
                    || r.ID.Contains(searchText))
                    && (r.Leader == null || r.Leader == ""))
                    .Select(r => new
                    {
                        r.ID,
                        r.Leader,
                        r.Name,
                        r.ParentID,
                        r.DelFlg,
                        r.CreateAt,
                        LeaderName = "",
                    })
                    .OrderBy(r => r.ID).ToList();

                var result2 = this.Repository.GetQuery<M_UNIT>()
                    .Where(r => isAll
                    || r.Name.Contains(searchText)
                    || r.ID.Contains(searchText)
                    && r.Leader != null)
                    .Join(this.Repository.GetQuery<AD_USER>(), a => a.Leader, b => b.UserName, (a, b)
                     => new
                     {
                         a.ID,
                         a.Leader,
                         a.Name,
                         a.ParentID,
                         a.DelFlg,
                         a.CreateAt,
                         LeaderName = b.FullName
                     })
                    .OrderBy(r => r.ID).ToList();

                var allResult = result.Union(result2).ToList();

                if (allResult.Count > 0)
                {
                    
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Ok,
                        Data = allResult
                    });
                } else
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }
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
        public IHttpActionResult Put(string id, [FromBody]M_UNIT dataRequest)
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

                var dataItem = this.Repository.GetQuery<M_UNIT>().FirstOrDefault(r => r.ID == id);
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
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgSaveOk),
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
        public IHttpActionResult Post([FromBody] M_UNIT dataRequest)
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

                if (this.Repository.GetQuery<M_UNIT>().Any(r => r.ID.Equals(dataRequest.ID)))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorIsExists), dataRequest.ID)
                    });
                }

                var dataItem = dataRequest.Clone();
                dataItem.CreateAt = DateTime.Now;

                this.Repository.Add(dataItem);
                this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgSaveOk),
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
        public IHttpActionResult Saves(IEnumerable<M_UNIT> dataRequest)
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

                M_UNIT dataItem = null;
                foreach (var item in dataRequest)
                {
                    

                    dataItem = this.Repository.GetQuery<M_UNIT>().FirstOrDefault(r => r.ID == item.ID);
                    if (dataItem is null)
                    {
                        dataItem = item.Clone();
                        dataItem.CreateAt = DateTime.Now;
                        string errorMsg = null;
                        GetValidate(dataItem, ref errorMsg);
                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            return Json(new TResult()
                            {
                                Status = (int)EStatus.Fail,
                                Msg = errorMsg
                            });
                        }

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

        [HttpGet]
        [Route("Single")]
        public IHttpActionResult Single(string id)
        {
            try
            {
                var dataItem = this.Repository.GetQuery<M_UNIT>().FirstOrDefault(r => r.ID == id);
                if (dataItem is null)
                {

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound))
                    });
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = dataItem
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

                var list = this.Repository.GetQuery<M_UNIT>().Where(r => ids.Any(p => p == r.ID)).ToList();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var temp = list[i];
                    var condition = this.Repository.GetQuery<M_UNIT>().Where(r => r.ID.ToString().Equals(temp.ID))
                    .Join(this.Repository.GetQuery<AD_USER>(), b => b.ID, a => a.UnitID, (b, a)
                     => new
                     {
                         a.FullName
                     }).ToList();
                    if (condition.Count > 0)
                    {
                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorUnitDeleteUser), temp.Name, condition[0].FullName)
                        });
                    }
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
        [Route("GetCombo")]
        public IHttpActionResult GetCombo(string id)
        {

            try
            {
                var result = this.Repository.GetQuery<M_UNIT>()
                .Where(r => r.ID != id)
                .OrderBy(r => r.ID)
                .ToList();

                var childlist = this.Repository.GetQuery<M_UNIT>()
                .Where(r => r.ParentID == id)
                .OrderBy(r => r.ID)
                .ToList();

                result.RemoveAll(x => childlist.Exists(y => y.ParentID == id));


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

        private void GetValidate(M_UNIT item, ref string errorMsg)
        {
            if (string.IsNullOrEmpty(item.ID))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Label_Part_ID)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(50, item.ID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Label_Part_ID), 50), ". ");
                }
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Label_Part_Name)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(250, item.Name))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Label_Part_Name), 250), ". ");
                }
            }
        }
    }
}
