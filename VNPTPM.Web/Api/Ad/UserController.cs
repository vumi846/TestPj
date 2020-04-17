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
    [RoutePrefix("api/User")]
    public class UserController : BaseController
    {
        public UserInformationVM Login(string userName, string password)
        {
            var passWord = new VNPTCrypto().Encrypt(password);
            var listuser = this.Repository.GetQuery<AD_USER>()
                .Where(r => r.DelFlg != true
                && r.UserName.Equals(userName)).ToList();
            var user = new AD_USER();

            if (listuser.Count < 1)
            {
                return new UserInformationVM()
                {
                    Msg = VNPTResources.ID.MsgLoginFail
                };
            }
            else
            {
                for (int i = 0; i < listuser.Count; i++)
                {
                    if (listuser[i].UserName.Equals(userName) && listuser[i].Password.Equals(passWord))
                    {
                        user = listuser[i];
                    }
                }

            }

            if (user.UserName is null)
            {
                return new UserInformationVM()
                {
                    Msg = VNPTResources.ID.MsgLoginFail
                };
            }

            if (user.LockFlg.GetValueOrDefault())
            {
                return new UserInformationVM()
                {
                    Msg = VNPTResources.ID.MsgUserLock
                };
            }

            var role = this.Repository.GetQuery<AD_ROLE>()
                .FirstOrDefault(r => r.ID.Equals(user.RoleID));


            return new UserInformationVM()
            {
                UserName = userName,
                FullName = string.IsNullOrEmpty(user.FullName) ? string.Empty : user.FullName,
                RoleID = role != null ? role.ID : string.Empty,
                RoleName = role != null ? role.Name : string.Empty
            };
        }

        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                var result = this.Repository.GetQuery<AD_USER>().FirstOrDefault(r => r.UserName.Equals(id));
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
                var isAllUser = string.IsNullOrEmpty(searchText);
                var result = this.Repository.GetQuery<AD_USER>()
                    .Where(r => isAllUser
                    || r.UserName.Contains(searchText)
                    || r.FullName.Contains(searchText))
                    .Join(this.Repository.GetQuery<AD_ROLE>(), a => a.RoleID, b => b.ID,
                    (a, b) => new
                    {
                        AD_USER = a,
                        AD_ROLE = b
                    })
                    .Select(r => new
                    {
                        r.AD_USER.UserName,
                        r.AD_USER.RoleID,
                        r.AD_USER.CreateAt,
                        r.AD_USER.DelFlg,
                        r.AD_USER.Description,
                        r.AD_USER.LockFlg,
                        r.AD_USER.UpdateAt,
                        Password = Guid.Empty.ToString(),
                        RoleName = r.AD_ROLE.Name,
                        r.AD_USER.FullName,
                        r.AD_USER.UnitID,
                        r.AD_USER.PartID,
                        r.AD_USER.Phone
                    })
                    .Join(this.Repository.GetQuery<M_PART>(), a => a.PartID, b => b.ID,
                    (a, b) => new
                    {
                        AD_USER = a,
                        M_PART = b
                    })
                    .Select(r => new
                    {
                        r.AD_USER.UserName,
                        r.AD_USER.RoleID,
                        r.AD_USER.CreateAt,
                        r.AD_USER.DelFlg,
                        r.AD_USER.Description,
                        r.AD_USER.LockFlg,
                        r.AD_USER.UpdateAt,
                        Password = Guid.Empty.ToString(),
                        r.AD_USER.RoleName,
                        r.AD_USER.FullName,
                        r.AD_USER.UnitID,
                        r.AD_USER.PartID,
                        r.AD_USER.Phone,
                        PartName = r.M_PART.Name
                    })
                    .Join(this.Repository.GetQuery<M_UNIT>(), a => a.UnitID, b => b.ID,
                    (a, b) => new
                    {
                        AD_USER = a,
                        M_UNIT = b
                    })
                    .Select(r => new
                    {
                        r.AD_USER.UserName,
                        r.AD_USER.RoleID,
                        r.AD_USER.CreateAt,
                        r.AD_USER.DelFlg,
                        r.AD_USER.Description,
                        r.AD_USER.LockFlg,
                        r.AD_USER.UpdateAt,
                        Password = Guid.Empty.ToString(),
                        r.AD_USER.RoleName,
                        r.AD_USER.FullName,
                        r.AD_USER.UnitID,
                        r.AD_USER.PartID,
                        r.AD_USER.PartName,
                        r.AD_USER.Phone,
                        UnitName = r.M_UNIT.Name
                    })
                .OrderBy(r => r.UserName)
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
        [Route("GetAssignees")]
        public IHttpActionResult GetAssignees(string searchText)
        {
            try
            {
                var isAllUser = string.IsNullOrEmpty(searchText);
                var result = this.Repository.GetQuery<AD_USER>()
                    .Where(r => isAllUser
                    || r.UserName.Contains(searchText)
                    || r.FullName.Contains(searchText))
                    .Join(this.Repository.GetQuery<AD_ROLE>(), a => a.RoleID, b => b.ID,
                    (a, b) => new
                    {
                        AD_USER = a,
                        AD_ROLE = b
                    })
                    .Select(r => new
                    {
                        r.AD_USER.UserName,
                        r.AD_USER.RoleID,
                        r.AD_USER.CreateAt,
                        r.AD_USER.DelFlg,
                        r.AD_USER.Description,
                        r.AD_USER.LockFlg,
                        r.AD_USER.UpdateAt,
                        Password = Guid.Empty.ToString(),
                        RoleName = r.AD_ROLE.Name,
                        r.AD_USER.FullName,
                        r.AD_USER.UnitID,
                        r.AD_USER.PartID,
                        r.AD_USER.Phone
                    })
                    .Join(this.Repository.GetQuery<M_PART>(), a => a.PartID, b => b.ID,
                    (a, b) => new
                    {
                        AD_USER = a,
                        M_PART = b
                    })
                    .Select(r => new
                    {
                        r.AD_USER.UserName,
                        r.AD_USER.RoleID,
                        r.AD_USER.CreateAt,
                        r.AD_USER.DelFlg,
                        r.AD_USER.Description,
                        r.AD_USER.LockFlg,
                        r.AD_USER.UpdateAt,
                        Password = Guid.Empty.ToString(),
                        r.AD_USER.RoleName,
                        r.AD_USER.FullName,
                        r.AD_USER.UnitID,
                        r.AD_USER.Phone,
                        r.AD_USER.PartID,
                        PartName = r.M_PART.Name
                    })
                    .Join(this.Repository.GetQuery<M_UNIT>(), a => a.UnitID, b => b.ID,
                    (a, b) => new
                    {
                        AD_USER = a,
                        M_UNIT = b
                    })
                    .Select(r => new
                    {
                        r.AD_USER.UserName,
                        r.AD_USER.RoleID,
                        r.AD_USER.CreateAt,
                        r.AD_USER.DelFlg,
                        r.AD_USER.Description,
                        r.AD_USER.LockFlg,
                        r.AD_USER.UpdateAt,
                        r.AD_USER.Phone,
                        Password = Guid.Empty.ToString(),
                        r.AD_USER.RoleName,
                        r.AD_USER.FullName,
                        r.AD_USER.UnitID,
                        r.AD_USER.PartID,
                        r.AD_USER.PartName,
                        UnitName = r.M_UNIT.Name
                    })
                .OrderBy(r => r.UserName)
                .ToList();

                List<string> notDisplayEmpsIDs = new List<string>();
                var notDisplayEmps = this.Repository.GetQuery<AD_CONFIG>()
                    .Where(r => r.ID.Equals("NotDisplayEmps")).FirstOrDefault();
                if (notDisplayEmps != null && !string.IsNullOrEmpty(notDisplayEmps.Value))
                {
                    notDisplayEmpsIDs = notDisplayEmps.Value.Split(';').ToList();
                }

                if (notDisplayEmpsIDs.Count > 0)
                {
                    result = result.FindAll(r => notDisplayEmpsIDs.Any(a => a == r.RoleID) != true);
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
        [Route("GetFullName")]
        public IHttpActionResult GetFullName(string username)
        {
            try
            {
                
                var result = this.Repository.GetQuery<AD_USER>().FirstOrDefault(r => r.UserName.Equals(username));
                if(result != null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Ok,
                        Data = result
                    });
                }
                else
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail
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

        [HttpGet]
        [Route("Searchcombo")]
        public IHttpActionResult Searchcombo()
        {
            try
            {
                var listuser = this.Repository.GetQuery<AD_USER>()
                    .Select(a => new{
                        a.UserName,
                        a.FullName
                    }).ToList();

                if(listuser.Count > 0)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Ok,
                        Data = listuser
                    });
                }
                else
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
        public IHttpActionResult Put(string id, [FromBody]AD_USER dataRequest)
        {
            try
            {
                //validate custom data input
                string errorMsg = this.validateUpdateUser(dataRequest);

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

                var dataItem = this.Repository.GetQuery<AD_USER>().FirstOrDefault(r => r.UserName.Equals(id));
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                var oldPassword = dataItem.Password;
                dataItem = dataRequest.Clone();
                dataItem.UserName = id;
                dataItem.UpdateAt = DateTime.Now;
                if (!string.IsNullOrEmpty(dataRequest.Password) && !oldPassword.Equals(dataRequest.Password) && !Guid.Empty.ToString().Equals(dataRequest.Password))
                {
                    dataItem.Password = (new VNPTCrypto()).Encrypt(dataRequest.Password);
                }
                else
                {
                    dataItem.Password = oldPassword;
                }

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


        [HttpPut]
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword(string newpassword, [FromBody]AD_USER dataRequest)
        {
            try
            {
                var dataItem = this.Repository.GetQuery<AD_USER>().FirstOrDefault(r => r.UserName.Equals(dataRequest.UserName));
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                if((new VNPTCrypto()).Encrypt(dataRequest.Password) != dataItem.Password)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorWrongOldPassword)
                    });
                }
                dataItem.Password = newpassword;

                //validate custom data input
                string errorMsg = this.validateUpdateUser(dataItem);

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

                
                dataItem.UpdateAt = DateTime.Now;
                dataItem.Password = (new VNPTCrypto()).Encrypt(dataItem.Password);
                

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
        public IHttpActionResult Post([FromBody] AD_USER dataRequest)
        {
            try
            {
                //validate custom data input
                string errorMsg = null;
                errorMsg = this.validateAddUser(dataRequest);

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

                if (this.Repository.GetQuery<AD_USER>().Any(r => r.UserName.Equals(dataRequest.UserName)))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorIsExists), dataRequest.UserName)
                    });
                }

                var dataItem = dataRequest.Clone();
                dataItem.CreateAt = DateTime.Now;
                dataItem.UpdateAt = DateTime.Now;

                if (!string.IsNullOrEmpty(dataRequest.Password))
                {
                    dataItem.Password = (new VNPTCrypto()).Encrypt(dataRequest.Password);
                }

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
        public IHttpActionResult Saves(IEnumerable<AD_USER> dataRequest)
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

                AD_USER dataItem = null;
                foreach (AD_USER item in dataRequest)
                {
                    var errorMsg = "";
                    errorMsg = this.validateAddUser(item);
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = errorMsg
                        });
                    }

                    dataItem = this.Repository.GetQuery<AD_USER>().FirstOrDefault(r => r.UserName.Equals(item.UserName));
                    if (dataItem is null)
                    {
                        dataItem = item.Clone();
                        dataItem.CreateAt = (dataItem.CreateAt != Constants.WO_VALUE_DETECT_INPUT_BLANK_DATE && dataItem.CreateAt != null) ? dataItem.CreateAt : DateTime.Now;
                        dataItem.UpdateAt = (dataItem.UpdateAt != Constants.WO_VALUE_DETECT_INPUT_BLANK_DATE && dataItem.UpdateAt != null) ? dataItem.UpdateAt : DateTime.Now;
                        if (!string.IsNullOrEmpty(item.Password))
                        {
                            dataItem.Password = (new VNPTCrypto()).Encrypt(item.Password);
                        }

                        this.Repository.Add(dataItem);
                        this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
                    }
                    else
                    {
                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorIsExists), item.UserName)
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

                var list = this.Repository.GetQuery<AD_USER>().Where(r => ids.Any(p => p == r.UserName)).ToList();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var dataItem = list[i];
                    var unitcount = this.Repository.GetQuery<M_UNIT>().Where(r => r.Leader.Equals(dataItem.UserName)).ToList();
                    if (unitcount.Count > 0)
                    {
                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorUserDeleteUnit), dataItem.FullName, unitcount[0].Name)
                        });
                    }
                    this.Repository.Delete(dataItem);
                    this.VNPTLogs.Write(this.Repository, EAction.Delete, JsonConvert.SerializeObject(dataItem));
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

        private string validateAddUser(AD_USER dataRequest)
        {
            var errorMsg = "";

            //USERNAME
            if (string.IsNullOrEmpty(dataRequest.UserName))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_UserName)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(50, dataRequest.UserName))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.User_UserName), 50), ". ");
                }
                if (CustomValidation.hasSpace(dataRequest.UserName))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorHasSpace), VNPTResources.Instance.Get(VNPTResources.ID.User_UserName)), ". ");
                }
            }
            
            //PASSWORD
            if (string.IsNullOrEmpty(dataRequest.Password))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_Password)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(150, dataRequest.Password))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.User_Password), 150), ". ");
                }
                if (CustomValidation.hasSpace(dataRequest.Password))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorHasSpace), VNPTResources.Instance.Get(VNPTResources.ID.User_Password)), ". ");
                }
            }

            //FULLNAME
            if (string.IsNullOrEmpty(dataRequest.FullName))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_FullName)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(250, dataRequest.FullName))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.User_FullName), 250), ". ");
                }
            }

            //PHONE
            if (string.IsNullOrEmpty(dataRequest.Phone))
            {
                //errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                    //    VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_Phone)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(50, dataRequest.Phone))
                {
                   // errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                     //   VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.User_Phone), 50), ". ");
                }
                if (this.Repository.GetQuery<AD_USER>().Any(r => r.Phone.Equals(dataRequest.Phone)) == true)
                {
                    //errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                    //    VNPTResources.ID.MsgErrorIsExists), VNPTResources.Instance.Get(VNPTResources.ID.User_Phone)), ". ");
                }
            }

            //DES
            if (!string.IsNullOrEmpty(dataRequest.Description))
            {
                if (!CustomValidation.maxLength(300, dataRequest.Description))
                {
                    //errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                    //    VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.User_Description), 300), ". ");
                }
            }

            //ROLE
            if (string.IsNullOrEmpty(dataRequest.RoleID))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_RoleID)), ". ");
            }
            else
            {
                if (this.Repository.GetQuery<AD_ROLE>().Any(r => r.ID.Equals(dataRequest.RoleID)) != true)
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorIDIsNotExists), VNPTResources.Instance.Get(VNPTResources.ID.User_RoleID)), ". ");
                }
                if (!CustomValidation.maxLength(50, dataRequest.RoleID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.User_RoleID), 50), ". ");
                }
            }

            //PART
            if (string.IsNullOrEmpty(dataRequest.PartID))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_PartID)), ". ");
            }
            else
            {
                if (this.Repository.GetQuery<M_PART>().Any(r => r.ID.Equals(dataRequest.PartID)) != true)
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorIDIsNotExists), VNPTResources.Instance.Get(VNPTResources.ID.User_PartID)), ". ");
                }
            }

            //UNIT
            if (string.IsNullOrEmpty(dataRequest.UnitID))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_UnitID)), ". ");
            }
            else
            {
                if (this.Repository.GetQuery<M_UNIT>().Any(r => r.ID.Equals(dataRequest.UnitID)) != true)
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorIDIsNotExists), VNPTResources.Instance.Get(VNPTResources.ID.User_UnitID)), ". ");
                }
            }

            return errorMsg;
        }

        private string validateUpdateUser(AD_USER dataRequest)
        {
            var errorMsg = "";

            //USERNAME
            if (string.IsNullOrEmpty(dataRequest.UserName))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_UserName)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(50, dataRequest.UserName))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.User_UserName), 50), ". ");
                }
                if (CustomValidation.hasSpace(dataRequest.UserName))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorHasSpace), VNPTResources.Instance.Get(VNPTResources.ID.User_UserName)), ". ");
                }
            }

            //PASSWORD
            if (string.IsNullOrEmpty(dataRequest.Password))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_Password)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(150, dataRequest.Password))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.User_Password), 250), ". ");
                }
                if (CustomValidation.hasSpace(dataRequest.Password))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorHasSpace), VNPTResources.Instance.Get(VNPTResources.ID.User_Password)), ". ");
                }
            }

            //FULLNAME
            if (string.IsNullOrEmpty(dataRequest.FullName))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_FullName)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(250, dataRequest.FullName))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.User_FullName), 250), ". ");
                }
            }

            //PHONE
            if (string.IsNullOrEmpty(dataRequest.Phone))
            {
                //errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        //VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_Phone)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(50, dataRequest.Phone))
                {
                    //errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                     //   VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.User_Phone), 50), ". ");
                }
                if (this.Repository.GetQuery<AD_USER>().Any(r => r.Phone.Equals(dataRequest.Phone)) == true)
                {
                    //errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                    //    VNPTResources.ID.MsgErrorIsExists), VNPTResources.Instance.Get(VNPTResources.ID.User_Phone)), ". ");
                }
            }

            //DES
            if (!string.IsNullOrEmpty(dataRequest.Description))
            {
                if (!CustomValidation.maxLength(300, dataRequest.Description))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.User_Description), 300), ". ");
                }
            }

            //ROLE
            if (string.IsNullOrEmpty(dataRequest.RoleID))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_RoleID)), ". ");
            }
            else
            {
                if (this.Repository.GetQuery<AD_ROLE>().Any(r => r.ID.Equals(dataRequest.RoleID)) != true)
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorIDIsNotExists), VNPTResources.Instance.Get(VNPTResources.ID.User_RoleID)), ". ");
                }
                if (!CustomValidation.maxLength(50, dataRequest.RoleID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.User_RoleID), 50), ". ");
                }
            }

            //PART
            if (string.IsNullOrEmpty(dataRequest.PartID))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_PartID)), ". ");
            }
            else
            {
                if (this.Repository.GetQuery<M_PART>().Any(r => r.ID.Equals(dataRequest.PartID)) != true)
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorIDIsNotExists), VNPTResources.Instance.Get(VNPTResources.ID.User_PartID)), ". ");
                }
            }

            //UNIT
            if (string.IsNullOrEmpty(dataRequest.UnitID))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.User_UnitID)), ". ");
            }
            else
            {
                if (this.Repository.GetQuery<M_UNIT>().Any(r => r.ID.Equals(dataRequest.UnitID)) != true)
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorIDIsNotExists), VNPTResources.Instance.Get(VNPTResources.ID.User_UnitID)), ". ");
                }
            }

            return errorMsg;
        }
    }
}
