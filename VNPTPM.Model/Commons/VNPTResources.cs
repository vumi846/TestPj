using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;

namespace VNPTPM.Model.Commons
{
    public class VNPTResources
    {
        public static class ID
        {
            public const string MsgAccessDenined = "MsgAccessDenined";
            public const string MsgSaveOk = "MsgSaveOk";
            public const string MsgUploadOk = "MsgUploadOk";
            public const string MsgDeleteOk = "MsgDeleteOk";
            public const string MsgLoginFail = "MsgLoginFail";
            public const string MsgUserLock = "MsgUserLock";
            public const string MsgNotFound = "MsgNotFound";
            public const string MsgRoleDefault = "MsgRoleDefault";
            public const string MsgException = "MsgException";
            public const string MsgInValidData = "MsgInValidData";
            public const string MsgNotFoundSetting = "MsgNotFoundSetting";
            public const string MsgErrorMaximumLength = "MsgErrorMaximumLength";
            public const string MsgErrorMinimumLength = "MsgErrorMinimumLength";
            public const string MsgErrorRequire = "MsgErrorRequire";
            public const string MsgErrorInvalidType = "MsgErrorInvalidType";
            public const string MsgErrorRegularExpressionNumber = "MsgErrorRegularExpressionNumber";
            public const string MsgErrorRegularExpressionInteger = "MsgErrorRegularExpressionInteger";
            public const string MsgErrorPhone = "MsgErrorPhone";
            public const string MsgErrorDateWithFormat = "MsgErrorDateWithFormat";
            public const string MsgErrorRang = "MsgErrorRang";
            public const string MsgErrorStartEndDatetime = "MsgErrorStartEndDatetime";
            public const string MsgErrorDueDatetime = "MsgErrorDueDatetime";
            public const string MsgErrorVideoUrlMaxSize = "MsgErrorVideoUrlMaxSize";
            public const string MsgErrorFileUrlMaxSize = "MsgErrorFileUrlMaxSize";
            public const string MsgErrorImageUrlMaxSize = "MsgErrorImageUrlMaxSize";
            public const string MsgErrorNotFoundPath = "MsgErrorNotFoundPath";
            public const string MsgErrorBase64Incorrect = "MsgErrorBase64Incorrect";
            public const string MsgErrorRequiredDeleteID = "MsgErrorRequiredDeleteID";
            public const string MsgErrorIsExists = "MsgErrorIsExists";
            public const string MsgErrorSignatureIncorrect = "MsgErrorSignatureIncorrect";
            public const string MsgErrorNotFoundConfig = "MsgErrorNotFoundConfig";
            public const string MsgErrorPaymentExisted = "MsgErrorPaymentExisted";
            public const string MsgErrorPaymentNotExpiry = "MsgErrorPaymentNotExpiry";
            public const string MsgErrorHasSpace = "MsgErrorHasSpace";
            public const string MsgErrorPaymentProcess = "MsgErrorPaymentProcess";
            public const string MsgErrorHasException = "MsgErrorHasException";
            public const string MsgErrorDontInputPhone = "MsgErrorDontInputPhone";
            public const string MsgErrorIDIsNotExists = "MsgErrorIDIsNotExists";
            public const string MsgErrorLogHourOverLog = "MsgErrorLogHourOverLog";
            public const string MsgErrorSearchMonthInvalid = "MsgErrorSearchMonthInvalid";
            public const string MsgErrorSearchPeriodInvalid = "MsgErrorSearchPeriodInvalid";
            public const string MsgNoRecordBranch = "MsgNoRecordBranch";
            public const string MsgCreateDataSuccess = "MsgCreateDataSuccess";
            public const string MsgUpdateDataSuccess = "MsgUpdateDataSuccess";
            public const string MsgSuccessPayment = "MsgSuccessPayment";
            public const string MsgApproveDataSuccess = "MsgApproveDataSuccess";
            public const string MsgRejectDataSuccess = "MsgRejectDataSuccess";
            public const string MsgLockDataSuccess = "MsgLockDataSuccess"; 
            public const string MsgUnLockDataSuccess = "MsgUnLockDataSuccess";
            public const string MsgDeleteDataSuccess = "MsgDeleteDataSuccess";
            public const string MsgUploadDataSuccess = "MsgUploadDataSuccess";
            public const string MsgResolveDataSuccess = "MsgResolveDataSuccess";
            public const string MsgRatingDataSuccess = "MsgRatingDataSuccess";
            public const string MsgValidData = "MsgValidData";
            public const string NotFoundByName = "NotFoundByName";
            public const string Login_IDNotNull = "Login_IDNotNull";
            public const string Login_PhoneNotNull = "Login_PhoneNotNull";
            public const string Login_OTPNotNull = "Login_OTPNotNull";
            public const string Login_OTPOutOfSize = "Login_OTPOutOfSize";
            public const string Login_Phone = "Login_Phone";
            public const string Login_OTPNotMatch = "Login_OTPNotMatch";
            public const string Login_DisplayGender = "Login_DisplayGender_{0}";
            public const string Login_AccLock = "Login_AccLock";
            public const string Login_PhoneMinLength = "Login_PhoneMinLength";
            public const string Login_OTPCodeContent = "Login_OTPCodeContent";
            public const string MsgErrorHaveChildDelete = "MsgErrorHaveChildDelete";
            public const string MsgErrorUserRoleDelete = "MsgErrorUserRoleDelete";
            public const string MsgErrorUserDeleteIssue = "MsgErrorUserDeleteIssue";
            public const string MsgErrorUserDeleteUnit = "MsgErrorUserDeleteUnit";
            public const string MsgErrorPartDeleteUser = "MsgErrorPartDeleteUser";
            public const string MsgErrorUnitDeleteUser = "MsgErrorUnitDeleteUser";
            public const string MsgErrorIssueDeleteType = "MsgErrorIssueDeleteType";
            public const string MsgErrorLogDatetime = "MsgErrorLogDatetime";
            public const string MsgErrorSamePerson = "MsgErrorSamePerson";
            public const string MsgErrorWrongOldPassword = "MsgErrorWrongOldPassword";
            public const string MsgErrorMultiTopic = "MsgErrorMultiTopic";
            public const string MsgErrorNoActiveTopic = "MsgErrorNoActiveTopic";
            public const string MsgErrorNoQuestion = "MsgErrorNoQuestion";
            public const string MsgErrorNoChoice = "MsgErrorNoChoice";
            public const string MsgErrorQuestionSameOrder = "MsgErrorQuestionSameOrder";
            public const string MsgErrorChoiceSameOrder = "MsgErrorChoiceSameOrder";
            public const string MsgErrorQuestionNullOrder = "MsgErrorQuestionNullOrder";
            public const string MsgErrorChoiceNullOrder = "MsgErrorChoiceNullOrder";

            public const string LabelFormatDate = "";

            public const string MsgErrorInvalidWorkHour = "MsgErrorInvalidWorkHour";

            //title page
            public const string Label_page_ID = "Label_page_ID";
            public const string Label_page_Nm = "Label_page_Nm";
            public const string Label_obj_page = "Label_obj_page";

            //User
            public const string User_UserName = "User_UserName";
            public const string User_Password = "User_Password";
            public const string User_Description = "User_Description";
            public const string User_RoleID = "User_RoleID";
            public const string User_FullName = "User_FullName";
            public const string User_UnitID = "User_UnitID";
            public const string User_PartID = "User_PartID";

            //Control
            public const string Control_ID = "Control_ID";
            public const string Control_Name = "Control_Name";

            //branch
            public const string Label_Branch = "Label_Branch";

            //deal
            public const string Label_Deal = "Label_Deal";

            //account
            public const string Txt_AccountNm = "Txt_AccountNm";

            //Role
            public const string Label_Role_ID = "Label_Role_ID";
            public const string Label_Role_Name = "Label_Role_Name";

            //Part
            public const string Label_Part_ID = "Label_Part_ID";
            public const string Label_Part_Name = "Label_Part_Name";
            public const string Label_Part_Description = "Label_Part_Description";

            //IssueType
            public const string Label_IssueType_ID = "Label_IssueType_ID";
            public const string Label_IssueType_Name = "Label_IssueType_Name";

            //RolePage
            public const string Label_RolePage_RoleID = "Label_RolePage_RoleID";
            public const string Label_RolePage_PageID = "Label_RolePage_PageID";

            //Project 
            public const string Label_Project_ID = "Label_Project_ID";
            public const string Label_Project_Name = "Label_Project_Name";
            public const string Label_Project_Code = "Label_Project_Code";

            //Unit 
            public const string Label_Unit_ID = "Label_Unit_ID";
            public const string Label_Unit_Name = "Label_Unit_Name";
            public const string Label_Unit_ParentID = "Label_Unit_ParentID";
            public const string Label_Unit_Leader = "Label_Unit_Leader";

            //Issue 
            public const string Label_Issue_ID = "Label_Issue_ID";
            public const string Label_Issue_Code = "Label_Issue_Code";
            public const string Label_Issue_ProjectID = "Label_Issue_ProjectID";
            public const string Label_Issue_TypeID = "Label_Issue_TypeID";
            public const string Label_Issue_Summary = "Label_Issue_Summary";
            public const string Label_Issue_Description = "Label_Issue_Description";
            public const string Label_Issue_Priority = "Label_Issue_Priority";
            public const string Label_Issue_StartDate = "Label_Issue_StartDate";
            public const string Label_Issue_DueDate = "Label_Issue_DueDate";
            public const string Label_Issue_Status = "Label_Issue_Status";
            public const string Label_Issue_ParentID = "Label_Issue_ParentID";
            public const string Label_Issue_RelatedPersonel = "Label_Issue_RelatedPersonel";
            public const string Label_Issue_LinkType = "Label_Issue_LinkType";
            public const string Label_Issue_LinkID = "Label_Issue_LinkID";
            public const string Label_Issue_LinkUrl = "Label_Issue_LinkUrl";
            public const string Label_Issue_ProgressID = "Label_Issue_ProgressID";
            public const string Label_Issue_Reporter = "Label_Issue_Reporter";
            public const string Label_Issue_Assignee = "Label_Issue_Assignee";
            public const string Label_Issue_AssigneeFrom = "Label_Issue_AssigneeFrom";
            public const string MsgRestartIssueOk = "MsgRestartIssueOk";

            //Comment
            public const string Label_Comment_ID = "Label_Comment_ID";
            public const string Label_Comment_IssueID = "Label_Comment_IssueID";
            public const string Label_Comment_Reporter = "Label_Comment_Reporter";
            public const string Label_Comment_Description = "Label_Comment_Description";
            public const string Label_Comment_FileUrls = "Label_Comment_FileUrls";

            //LogWork
            public const string Label_LogWork_ID = "Label_LogWork_ID";
            public const string Label_LogWork_IssueID = "Label_LogWork_IssueID";
            public const string Label_LogWork_Reporter = "Label_LogWork_Reporter";
            public const string Label_LogWork_LogDate = "Label_LogWork_LogDate";
            public const string Label_LogWork_Worked = "Label_LogWork_Worked";
            public const string Label_LogWork_Description = "Label_LogWork_Description";

            // File
            public const string Label_File_Code = "Label_File_Code";
            public const string Label_File_SerialNo = "Label_File_SerialNo";
            public const string Label_File_ID = "Label_File_ID";
            public const string Label_File_Title = "Label_File_Title";
            public const string Label_File_Description = "Label_File_Description";
            public const string Label_File_UserName = "Label_File_UserName";
            public const string Label_File_AssignUnits = "Label_File_AssignUnits";
            public const string Label_File_FileUrls = "Label_File_FileUrls";
            public const string Label_File_DelFlg = "Label_File_DelFlg";
            public const string Label_File_CreateAt = "Label_File_CreateAt";
            public const string Label_File_UpdateAt = "Label_File_UpdateAt";

            // FileLog
            public const string Label_FileLog_ID = "Label_FileLog_ID";
            public const string Label_FileLog_UserName = "Label_FileLog_UserName";
            public const string Label_FileLog_CreateAt = "Label_FileLog_CreateAt";

            // GroupChat
            public const string Label_GroupChat_ID = "Label_GroupChat_ID";
            public const string Label_GroupChat_Name = "Label_GroupChat_Name";
            public const string Label_GroupChat_Member = "Label_GroupChat_Member";

            //report
            
            public const string Label_Report_Unexpired = "Label_Report_Unexpired";
            public const string Label_Report_Expired = "Label_Report_Expired";
            public const string Label_Report_LeaderinMonth = "Label_Report_LeaderinMonth";
            public const string Label_Report_LeaderinNextMonth = "Label_Report_LeaderinNextMonth";
            public const string Label_Report_CompleteinMonth = "Label_Report_CompleteinMonth";
            public const string Label_Report_StillValid = "Label_Report_StillValid";
            public const string Label_Report_OutDate1 = "Label_Report_OutDate1";
            public const string Label_Report_OnTime = "Label_Report_OnTime";
            public const string Label_Report_OutDate2 = "Label_Report_OutDate2";
            public const string Label_Report_ApprovedComplete = "Label_Report_ApprovedComplete";
            public const string Label_Report_NotComplete = "Label_Report_NotComplete";
            public const string Label_Report_NotRating = "Label_Report_NotRating";
            public const string Label_Report_Sum = "Label_Report_Sum";
            public const string Label_Report_SumOut = "Label_Report_SumOut";


        }

        private static VNPTResources instance;
        public static VNPTResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VNPTResources();
                }

                return instance;
            }
        }

        public string Get(ModelStateDictionary modelState)
        {
            var results = new List<string>();
            var msg = "";
            foreach (ModelState model in modelState.Values)
            {
                foreach (ModelError error in model.Errors)
                {
                    msg = error.ErrorMessage;
                    if (string.IsNullOrEmpty(msg) && error.Exception != null && !string.IsNullOrEmpty(error.Exception.Message))
                    {
                        msg = error.Exception.Message;
                    }
                    results.Add(msg);
                }
            }

            return string.Join(";", results);
        }

        public string Get(string key)
        {
            return GlobalResources.Resources.ResourceManager.GetString(key);
        }
    }
}
