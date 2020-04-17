using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using VNPTPM.Model.Commons;
using VNPTPM.Model.Core;

namespace VNPTPM.Web.Api.Base
{
    [Authorize]
    public class BaseController : ApiController, IDisposable
    {
        private IDALContainer DALContainer = null;
        private IDALContainer DALContainerLog = null;
        private VNPTLogs vNPTLogs = null;
        public BaseController()
        {
            DALContainer = new EFDALContainer();
            DALContainerLog = new EFDALContainer();
        }

        public IRepository Repository
        {
            get
            {
                return DALContainer.Repository;
            }
        }

        public IRepository RepositoryLog
        {
            get
            {
                return DALContainerLog.Repository;
            }
        }
        public VNPTLogs VNPTLogs
        {
            get
            {
                if (vNPTLogs == null)
                {
                    vNPTLogs = new VNPTLogs();
                }

                return vNPTLogs;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (DALContainer != null)
            {
                DALContainer.Close();
                DALContainer.Dispose();
                DALContainer = null;
            }
            base.Dispose(disposing);
        }

        //Common
        public bool getForward(int CreateMonth, int SearchMonth, int CurrentMonth,
            int CreateYear, int SearchYear, int CurrentYear, short? StatusTask,
            DateTime CurrentDt, DateTime? DKHTDt, DateTime? HTDt, int TypeReport)
        {
            bool isForward = false;
            int LastDtInMonth = DateTime.DaysInMonth(SearchYear, SearchMonth);
            DateTime LastSearchDtInMonth = new DateTime(SearchYear, SearchMonth, LastDtInMonth);
            if (((CreateMonth < SearchMonth && CreateYear == SearchYear) || (CreateYear < SearchYear)) && (CreateMonth >= 1 && CreateMonth <= 12))
            {
                if ((SearchMonth == CurrentMonth && SearchYear == CurrentYear) || (SearchYear > CurrentYear))
                {
                    if (StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess && StatusTask != (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        if ((DKHTDt > CurrentDt && TypeReport == (int)ETypeReport.Unexpired)
                            || (DKHTDt <= CurrentDt && TypeReport == (int)ETypeReport.Expired))
                        {
                            isForward = true;
                        }
                    }
                    else if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        if ((HTDt <= DKHTDt && TypeReport == (int)ETypeReport.Unexpired)
                            || (HTDt > DKHTDt && TypeReport == (int)ETypeReport.Expired))
                        {
                            isForward = true;
                        }
                    }
                }
                else if ((SearchMonth < CurrentMonth && SearchYear == CurrentYear) || (SearchYear < CurrentYear))
                {
                    if ((StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess && StatusTask != (int)EStatusIssueResolve.EndWithNotProcess)
                        || (HTDt > DKHTDt))
                    {
                        if ((DKHTDt > LastSearchDtInMonth && TypeReport == (int)ETypeReport.Unexpired)
                            || (DKHTDt <= LastSearchDtInMonth && TypeReport == (int)ETypeReport.Expired))
                        {
                            isForward = true;
                        }
                    }
                    else if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        if ((HTDt <= DKHTDt && TypeReport == (int)ETypeReport.Unexpired)
                            || (HTDt > DKHTDt && TypeReport == (int)ETypeReport.Expired))
                        {
                            isForward = true;
                        }
                    }
                }

            }
            return isForward;
        }

        public bool getLeaderinMonth(int CreateMonth, int SearchMonth, int CurrentMonth,
            int CreateYear, int SearchYear, int CurrentYear, short? StatusTask,
            DateTime? DKHTDt, DateTime? HTDt, int TypeReport)
        {
            bool isLeaderinMonth = false;
            int DKHTMonth = 0;
            if (DKHTDt != null)
            {
                DateTime DKHTDt2 = (DateTime)DKHTDt;
                DKHTMonth = DKHTDt2.Month;
            }
            if (CreateMonth == SearchMonth && CreateYear == SearchYear)
            {
                if ((SearchMonth == CurrentMonth) && (SearchYear == CurrentYear))
                {
                    if (TypeReport == (int)ETypeReport.LeaderinMonth)
                    {
                        if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                        {
                            isLeaderinMonth = true;
                        }
                    }
                    else if (TypeReport == (int)ETypeReport.LeaderinNextMonth)
                    {
                        if (StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess && StatusTask != (int)EStatusIssueResolve.EndWithNotProcess)
                        {
                            if (DKHTMonth > CurrentMonth)
                            {
                                isLeaderinMonth = true;
                            }
                        }
                    }

                }
                else if (((SearchMonth < CurrentMonth) && (SearchYear == CurrentYear)) || (SearchYear < CurrentYear))
                {
                    if (TypeReport == (int)ETypeReport.LeaderinMonth)
                    {
                        if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                        {
                            if (HTDt != null && HTDt.Value.Month == SearchMonth)
                            {
                                isLeaderinMonth = true;
                            }
                        }
                    }
                    else if (TypeReport == (int)ETypeReport.LeaderinNextMonth)
                    {
                        if (StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess && StatusTask != (int)EStatusIssueResolve.EndWithNotProcess)
                        {
                            isLeaderinMonth = true;
                        }
                        else if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                        {
                            if (HTDt != null &&  HTDt.Value.Month > SearchMonth)
                            {
                                isLeaderinMonth = true;
                            }
                        }
                    }

                }

            }
            return isLeaderinMonth;
        }


        public bool getCompleteinMonth(int CreateMonth, int SearchMonth, int CurrentMonth,
            int CreateYear, int SearchYear, int CurrentYear, short? StatusTask,
            DateTime? HTDt)
        {
            bool isCompleteinMonth = false;
            if ((CreateMonth <= SearchMonth && CreateYear == SearchYear) || (CreateYear < SearchYear))
            {
                if (SearchMonth == CurrentMonth && SearchYear == CurrentYear)
                {
                    if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        isCompleteinMonth = true;
                    }
                }
                else if ((SearchMonth < CurrentMonth && SearchYear == CurrentYear) || (SearchYear < CurrentYear))
                {
                    if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        if (HTDt != null && HTDt.Value.Month == SearchMonth)
                        {
                            isCompleteinMonth = true;
                        }
                    }
                }

            }
            return isCompleteinMonth;
        }

        public bool getStatusProcessIssue(int CreateMonth, int SearchMonth, int CurrentMonth,
            int CreateYear, int SearchYear, int CurrentYear, short? StatusTask,
            DateTime CurrentDt, DateTime? DKHTDt, DateTime? HTDt, int TypeReport)
        {
            bool isStatus = false;
            if ((CreateMonth <= SearchMonth && CreateYear == SearchYear) || (CreateYear < SearchYear))
            {
                if (SearchMonth == CurrentMonth && SearchYear == CurrentYear)
                {
                    if (StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess && StatusTask != (int)EStatusIssueResolve     .EndWithNotProcess)
                    {
                        if ((DKHTDt >= CurrentDt && TypeReport == (int)ETypeReport.StillValid)
                            || (DKHTDt < CurrentDt && TypeReport == (int)ETypeReport.OutDate1))
                        {
                            isStatus = true;
                        } 
                    }
                    else if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        if ((HTDt <= DKHTDt && TypeReport == (int)ETypeReport.OnTime)
                            || (HTDt > DKHTDt && TypeReport == (int)ETypeReport.OutDate2))
                        {
                            isStatus = true;
                        }
                    }
                }
                else if ((SearchMonth < CurrentMonth && SearchYear == CurrentYear) || (SearchYear < CurrentYear))
                {
                    //if ((StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess && StatusTask != (int)EStatusIssueResolve.EndWithNotProcess)
                    //    || (HTDt > DKHTDt))
                    //{
                    //    int LastDtInMonth = DateTime.DaysInMonth(SearchYear, SearchMonth);
                    //    DateTime LastSearchDtInMonth = new DateTime(SearchYear, SearchMonth, LastDtInMonth);
                    //    if ((DKHTDt >= LastSearchDtInMonth && TypeReport == (int)ETypeReport.StillValid)
                    //        || (DKHTDt < LastSearchDtInMonth && TypeReport == (int)ETypeReport.OutDate1))
                    //    {
                    //        isStatus = true;
                    //    }
                    //}
                    //else if ((StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                    //    && (HTDt <= DKHTDt))
                    //{
                    //    if ((HTDt <= DKHTDt && TypeReport == (int)ETypeReport.OnTime)
                    //        || (HTDt > DKHTDt && TypeReport == (int)ETypeReport.OutDate2))
                    //    {
                    //        isStatus = true;
                    //    }
                    //}

                    if (HTDt == null)
                    {
                        if ((StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess
                            && StatusTask != (int)EStatusIssueResolve.EndWithNotProcess))
                        {
                            int LastDtInMonth = DateTime.DaysInMonth(SearchYear, SearchMonth);
                            DateTime LastSearchDtInMonth = new DateTime(SearchYear, SearchMonth, LastDtInMonth);
                            if ((DKHTDt >= LastSearchDtInMonth && TypeReport == (int)ETypeReport.StillValid)
                                || (DKHTDt < LastSearchDtInMonth && TypeReport == (int)ETypeReport.OutDate1))
                            {
                                isStatus = true;
                            }
                        }
                    }
                    else
                    {
                        DateTime HTMonth2 = (DateTime)HTDt;
                        int HTMonth = HTMonth2.Month;
                        if (HTDt <= DKHTDt)
                        {
                            if ((HTMonth > SearchMonth && TypeReport == (int)ETypeReport.StillValid)
                                || (HTMonth <= SearchMonth && TypeReport == (int)ETypeReport.OnTime))
                            {
                                isStatus = true;
                            }
                        }
                        else
                        {
                            if ((HTMonth > SearchMonth && TypeReport == (int)ETypeReport.OutDate1)
                                || (HTMonth <= SearchMonth && TypeReport == (int)ETypeReport.OutDate2))
                            {
                                isStatus = true;
                            }
                        }
                    }
                }
            }
            return isStatus;
        }

        public bool getRatting(int CreateMonth, int SearchMonth, int CurrentMonth,
            int CreateYear, int SearchYear, int CurrentYear, short? StatusTask, short? StatusRatting,
            DateTime? HTDt, DateTime? RatingDt, int TypeReport)
        {
            bool isRatting = false;
            int HTMonth = 0;
            int HTYear = 0;
            if (HTDt != null)
            {
                DateTime HTDt2 = (DateTime)HTDt;
                HTMonth = HTDt2.Month;
                HTYear = HTDt2.Year;
            }
            if (((CreateMonth <= SearchMonth && CreateYear == SearchYear) || (CreateYear < SearchYear))
                && (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                && (HTMonth == SearchMonth && HTYear == SearchYear))
            {
                if (SearchMonth == CurrentMonth && SearchYear == CurrentYear)
                {
                    if (((StatusRatting == (int)EStatusIssueRating.ProcessDone) && (TypeReport == (int)ETypeReport.ApprovedComplete))
                        || ((StatusRatting == (int)EStatusIssueRating.ProcessNotDone) && (TypeReport == (int)ETypeReport.NotComplete))
                        || ((StatusRatting == null) && (TypeReport == (int)ETypeReport.NotRating)))
                    {
                        isRatting = true;
                    }
                }
                else if ((SearchMonth < CurrentMonth && SearchYear == CurrentYear) || (SearchYear < CurrentYear))
                {
                    //get RatingMonth and RatingYear
                    int RatingMonth = 0;
                    int RatingYear = 0;
                    if (RatingDt != null)
                    {
                        DateTime dtValue = (DateTime)RatingDt;
                        RatingMonth = dtValue.Month;
                        RatingYear = dtValue.Year;
                    }
                    if (((StatusRatting == (int)EStatusIssueRating.ProcessDone)
                            && (SearchMonth == RatingMonth && SearchYear == RatingYear)
                            && (TypeReport == (int)ETypeReport.ApprovedComplete)
                         )
                     || (
                            (StatusRatting == (int)EStatusIssueRating.ProcessNotDone)
                            && (SearchMonth == RatingMonth && SearchYear == RatingYear)
                            && (TypeReport == (int)ETypeReport.NotComplete)
                         )
                     || (
                            (
                                (StatusRatting == null)
                                || (SearchMonth > RatingMonth && SearchYear == RatingYear)
                                || (SearchYear > RatingYear)
                            )
                            && (TypeReport == (int)ETypeReport.NotRating)
                         )
                    )
                    {
                        isRatting = true;
                    }
                }
            }

            return isRatting;
        }

        public string getTitleSubWithTypeReport(int type)
        {
            string titleSub = "";
            switch (type)
            {
                case (int)ETypeReport.Unexpired:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_Unexpired);
                    //titleSub = "Chuyển tiếp từ tháng trước/Còn hạn";
                    break;
                case (int)ETypeReport.Expired:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_Expired);
                    //titleSub = "Chuyển tiếp từ tháng trước/Hết hạn";
                    break;
                case (int)ETypeReport.LeaderinMonth:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_LeaderinMonth);
                    //titleSub = "Lãnh đạo giao trong tháng/Hoàn thành trong tháng";
                    break;
                case (int)ETypeReport.LeaderinNextMonth:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_LeaderinNextMonth);
                    //titleSub = "Lãnh đạo giao trong tháng/Gối sang tháng khác";
                    break;
                case (int)ETypeReport.CompleteinMonth:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_CompleteinMonth);
                    //titleSub = "Tổng số việc/Hoàn thành trong tháng";
                    break;
                case (int)ETypeReport.StillValid:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_StillValid);
                    //titleSub = "Chưa xử lý xong/Còn hạn";
                    break;
                case (int)ETypeReport.OutDate1:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_OutDate1);
                    //titleSub = "Chưa xử lý xong/Quá hạn";
                    break;
                case (int)ETypeReport.OnTime:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_OnTime);
                    //titleSub = "Xử lý xong/Đúng hạn";
                    break;
                case (int)ETypeReport.OutDate2:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_OutDate2);
                    //titleSub = "Xử lý xong/Quá hạn";
                    break;
                case (int)ETypeReport.ApprovedComplete:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_ApprovedComplete);
                    //titleSub = "Lãnh đạo đánh giá/Duyệt xử lý xong";
                    break;
                case (int)ETypeReport.NotComplete:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_NotComplete);
                    //titleSub = "Lãnh đạo đánh giá/Chưa xử lý xong";
                    break;
                case (int)ETypeReport.NotRating:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_NotRating);
                    //titleSub = "Lãnh đạo đánh giá/Chưa đánh giá";
                    break;
                case (int)ETypeReport.Sum:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_Sum);
                    //titleSub = "Tổng số việc/Tổng cộng";
                    break;
                case (int)ETypeReport.SumOut:
                    titleSub = VNPTResources.Instance.Get(VNPTResources.ID.Label_Report_SumOut);
                    //titleSub = "Tổng số công việc quá hạn";
                    break;
                default:
                    break;
            }

            return titleSub;
        }

        public int getTypeReportWithColumnNm(string colName)
        {
            int type = (int)ETypeReport.Default;
            switch (colName)
            {
                case nameof(ETypeReport.Unexpired):
                    type = (int)ETypeReport.Unexpired;
                    break;
                case nameof(ETypeReport.Expired):
                    type = (int)ETypeReport.Expired;
                    break;
                case nameof(ETypeReport.LeaderinMonth):
                    type = (int)ETypeReport.LeaderinMonth;
                    break;
                case nameof(ETypeReport.LeaderinNextMonth):
                    type = (int)ETypeReport.LeaderinNextMonth;
                    break;
                case nameof(ETypeReport.CompleteinMonth):
                    type = (int)ETypeReport.CompleteinMonth;
                    break;
                case nameof(ETypeReport.StillValid):
                    type = (int)ETypeReport.StillValid;
                    break;
                case nameof(ETypeReport.OutDate1):
                    type = (int)ETypeReport.OutDate1;
                    break;
                case nameof(ETypeReport.OnTime):
                    type = (int)ETypeReport.OnTime;
                    break;
                case nameof(ETypeReport.OutDate2):
                    type = (int)ETypeReport.OutDate2;
                    break;
                case nameof(ETypeReport.ApprovedComplete):
                    type = (int)ETypeReport.ApprovedComplete;
                    break;
                case nameof(ETypeReport.NotComplete):
                    type = (int)ETypeReport.NotComplete;
                    break;
                case nameof(ETypeReport.NotRating):
                    type = (int)ETypeReport.NotRating;
                    break;
                case nameof(ETypeReport.Sum):
                    type = (int)ETypeReport.Sum;
                    break;
                case nameof(ETypeReport.SumOut):
                    type = (int)ETypeReport.SumOut;
                    break;
                default:
                    break;
            }

            return type;
        }

        public bool getDetailWithTypeInfo(int CreateMonth, int SearchMonth, int CurrentMonth,
            int CreateYear, int SearchYear, int CurrentYear, short? StatusTask, short? StatusRatting,
            DateTime CurrentDt, DateTime? DKHTDt, DateTime? HTDt, DateTime? RatingDt, int TypeReport)
        {
            bool validDataFlg = false;
            switch (TypeReport)
            {
                case (int)ETypeReport.Unexpired:
                case (int)ETypeReport.Expired:
                    validDataFlg = this.getForward(CreateMonth, SearchMonth, CurrentMonth,
                        CreateYear, SearchYear, CurrentYear, StatusTask,
                        CurrentDt, DKHTDt, HTDt, TypeReport);
                    break;
                case (int)ETypeReport.LeaderinMonth:
                case (int)ETypeReport.LeaderinNextMonth:
                    validDataFlg = this.getLeaderinMonth(CreateMonth, SearchMonth, CurrentMonth,
                        CreateYear, SearchYear, CurrentYear, StatusTask,
                        DKHTDt, HTDt, TypeReport);
                    break;
                case (int)ETypeReport.CompleteinMonth:
                    validDataFlg = this.getCompleteinMonth(CreateMonth, SearchMonth, CurrentMonth,
                        CreateYear, SearchYear, CurrentYear, StatusTask, HTDt);
                    break;
                case (int)ETypeReport.StillValid:
                case (int)ETypeReport.OutDate1:
                case (int)ETypeReport.OnTime:
                case (int)ETypeReport.OutDate2:
                    validDataFlg = this.getStatusProcessIssue(CreateMonth, SearchMonth, CurrentMonth,
                        CreateYear, SearchYear, CurrentYear, StatusTask,
                        CurrentDt, DKHTDt, HTDt, TypeReport);
                    break;
                case (int)ETypeReport.ApprovedComplete:
                case (int)ETypeReport.NotComplete:
                case (int)ETypeReport.NotRating:
                    validDataFlg = this.getRatting(CreateMonth, SearchMonth, CurrentMonth,
                        CreateYear, SearchYear, CurrentYear, StatusTask, StatusRatting,
                        HTDt, RatingDt, TypeReport);
                    break;
                case (int)ETypeReport.Sum:
                    validDataFlg =
                        this.getForward(CreateMonth, SearchMonth, CurrentMonth,
                            CreateYear, SearchYear, CurrentYear, StatusTask,
                            CurrentDt, DKHTDt, HTDt, (int)ETypeReport.Unexpired)
                        ||
                        this.getForward(CreateMonth, SearchMonth, CurrentMonth,
                            CreateYear, SearchYear, CurrentYear, StatusTask,
                            CurrentDt, DKHTDt, HTDt, (int)ETypeReport.Expired)
                        ||
                        this.getLeaderinMonth(CreateMonth, SearchMonth, CurrentMonth,
                            CreateYear, SearchYear, CurrentYear, StatusTask,
                            DKHTDt, HTDt, (int)ETypeReport.LeaderinMonth)
                        ||
                        this.getLeaderinMonth(CreateMonth, SearchMonth, CurrentMonth,
                            CreateYear, SearchYear, CurrentYear, StatusTask,
                            DKHTDt, HTDt, (int)ETypeReport.LeaderinNextMonth);
                    break;
                case (int)ETypeReport.SumOut:
                    validDataFlg =
                        this.getStatusProcessIssue(CreateMonth, SearchMonth, CurrentMonth,
                            CreateYear, SearchYear, CurrentYear, StatusTask,
                            CurrentDt, DKHTDt, HTDt, (int)ETypeReport.OutDate1)
                        ||
                        this.getStatusProcessIssue(CreateMonth, SearchMonth, CurrentMonth,
                            CreateYear, SearchYear, CurrentYear, StatusTask,
                            CurrentDt, DKHTDt, HTDt, (int)ETypeReport.OutDate2);

                    break;
                default:
                    break;
            }

            return validDataFlg;
        }
        public EStatusTask getStatusOfTask(DateTime issueDate, EStatusIssueResolve status)
        {
            var intervalToDueValues = Enum.GetValues(typeof(VNPTPM.Model.Commons.EIntervalToDue));
            int intervalToDue = -1;
            foreach (int interval in intervalToDueValues) { intervalToDue = interval; }
            if (status == EStatusIssueResolve.Started || status == EStatusIssueResolve.NotStart)
            {
                if ((issueDate - DateTime.Now).Days <= intervalToDue && (issueDate - DateTime.Now).Days >= 0)
                {
                    return EStatusTask.IncomingDue;
                }
                else if ((issueDate - DateTime.Now).Days <= 0)
                {
                    return EStatusTask.OverDue;
                }
            }
            return EStatusTask.Normal;
        }

        //get data in case search with period of year
        public bool getForwardWithPeriod(int CreateMonth, int SearchPeriod, int CurrentMonth,
            int CreateYear, int SearchYear, int CurrentYear, short? StatusTask,
            DateTime CurrentDt, DateTime? DKHTDt, DateTime? HTDt, int TypeReport)
        {
            bool isForward = false;

            if (((VNPTHelper.getPeriodOfMonth(CreateMonth) < SearchPeriod && CreateYear == SearchYear) 
                || (CreateYear < SearchYear)) && (CreateMonth >= 1 && CreateMonth <= 12))
            {
                if ((SearchPeriod == VNPTHelper.getPeriodOfMonth(CurrentMonth) && SearchYear == CurrentYear) 
                    || (SearchYear > CurrentYear))
                {
                    if (StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess && StatusTask != (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        if ((DKHTDt > CurrentDt && TypeReport == (int)ETypeReport.Unexpired)
                            || (DKHTDt <= CurrentDt && TypeReport == (int)ETypeReport.Expired))
                        {
                            isForward = true;
                        }
                    }
                    else if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        if ((HTDt <= DKHTDt && TypeReport == (int)ETypeReport.Unexpired)
                            || (HTDt > DKHTDt && TypeReport == (int)ETypeReport.Expired))
                        {
                            isForward = true;
                        }
                    }
                }
                else if ((SearchPeriod < VNPTHelper.getPeriodOfMonth(CurrentMonth) && SearchYear == CurrentYear) 
                    || (SearchYear < CurrentYear))
                {
                    if ((StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess && StatusTask != (int)EStatusIssueResolve.EndWithNotProcess)
                        || (VNPTHelper.getPeriodOfMonth(HTDt.Value.Month) > SearchPeriod))
                    {
                        if ((DKHTDt >= VNPTHelper.getLastDayOfPeriod(SearchPeriod, SearchYear) && TypeReport == (int)ETypeReport.Unexpired)
                            || (DKHTDt < VNPTHelper.getLastDayOfPeriod(SearchPeriod, SearchYear) && TypeReport == (int)ETypeReport.Expired))
                        {
                            isForward = true;
                        }
                    }
                    else if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        if ((HTDt <= DKHTDt && TypeReport == (int)ETypeReport.Unexpired)
                            || (HTDt > DKHTDt && TypeReport == (int)ETypeReport.Expired))
                        {
                            isForward = true;
                        }
                    }
                }

            }
            return isForward;
        }

        public bool getLeaderinMonthWithPeriod(int CreateMonth, int SearchPeriod, int CurrentMonth,
            int CreateYear, int SearchYear, int CurrentYear, short? StatusTask,
            DateTime? DKHTDt, DateTime? HTDt, int TypeReport)
        {
            bool isLeaderinMonth = false;
            int DKHTMonth = 0;
            if (DKHTDt != null)
            {
                DateTime DKHTDt2 = (DateTime)DKHTDt;
                DKHTMonth = DKHTDt2.Month;
            }
            if (VNPTHelper.getPeriodOfMonth(CreateMonth) == SearchPeriod && CreateYear == SearchYear)
            {
                if ((SearchPeriod == VNPTHelper.getPeriodOfMonth(CurrentMonth)) && (SearchYear == CurrentYear))
                {
                    if (TypeReport == (int)ETypeReport.LeaderinMonth)
                    {
                        if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                        {
                            isLeaderinMonth = true;
                        }
                    }
                    else if (TypeReport == (int)ETypeReport.LeaderinNextMonth)
                    {
                        if (StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess && StatusTask != (int)EStatusIssueResolve.EndWithNotProcess)
                        {
                            if (VNPTHelper.getPeriodOfMonth(DKHTMonth) > SearchPeriod)
                            {
                                isLeaderinMonth = true;
                            }
                        }
                    }

                }
                else if (((SearchPeriod < VNPTHelper.getPeriodOfMonth(CurrentMonth)) && (SearchYear == CurrentYear)) || (SearchYear < CurrentYear))
                {
                    if (TypeReport == (int)ETypeReport.LeaderinMonth)
                    {
                        if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess 
                            || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                        {
                            if (HTDt != null && VNPTHelper.getPeriodOfMonth(HTDt.Value.Month) == SearchPeriod)
                            {
                                isLeaderinMonth = true;
                            }
                        }
                    }
                    else if (TypeReport == (int)ETypeReport.LeaderinNextMonth)
                    {
                        if (StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess && StatusTask != (int)EStatusIssueResolve.EndWithNotProcess)
                        {
                            isLeaderinMonth = true;
                        }
                        else if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                        {
                            if (HTDt != null && VNPTHelper.getPeriodOfMonth(HTDt.Value.Month) > SearchPeriod)
                            {
                                isLeaderinMonth = true;
                            }
                        }
                    }

                }

            }
            return isLeaderinMonth;
        }

        public bool getCompleteinMonthWithPeriod(int CreateMonth, int SearchPeriod, int CurrentMonth,
            int CreateYear, int SearchYear, int CurrentYear, short? StatusTask,
            DateTime? HTDt)
        {
            bool isCompleteinMonth = false;
            if ((VNPTHelper.getPeriodOfMonth(CreateMonth) <= SearchPeriod && CreateYear == SearchYear) || (CreateYear < SearchYear))
            {
                if (SearchPeriod == VNPTHelper.getPeriodOfMonth(CurrentMonth) && SearchYear == CurrentYear)
                {
                    if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        isCompleteinMonth = true;
                    }
                }
                else if ((SearchPeriod < VNPTHelper.getPeriodOfMonth(CurrentMonth) && SearchYear == CurrentYear) || (SearchYear < CurrentYear))
                {
                    if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        if (HTDt != null && VNPTHelper.getPeriodOfMonth(HTDt.Value.Month) == SearchPeriod)
                        {
                            isCompleteinMonth = true;
                        }
                    }
                }

            }
            return isCompleteinMonth;
        }

        public bool getStatusProcessIssueWithPeriod(int CreateMonth, int SearchPeriod, int CurrentMonth,
            int CreateYear, int SearchYear, int CurrentYear, short? StatusTask,
            DateTime CurrentDt, DateTime? DKHTDt, DateTime? HTDt, int TypeReport)
        {
            bool isStatus = false;
            if ((VNPTHelper.getPeriodOfMonth(CreateMonth) <= SearchPeriod && CreateYear == SearchYear) || (CreateYear < SearchYear))
            {
                if (SearchPeriod == VNPTHelper.getPeriodOfMonth(CurrentMonth) && SearchYear == CurrentYear)
                {
                    if (StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess && StatusTask != (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        if ((DKHTDt >= CurrentDt && TypeReport == (int)ETypeReport.StillValid)
                            || (DKHTDt < CurrentDt && TypeReport == (int)ETypeReport.OutDate1))
                        {
                            isStatus = true;
                        }
                    }
                    else if (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                    {
                        if ((HTDt <= DKHTDt && TypeReport == (int)ETypeReport.OnTime)
                            || (HTDt > DKHTDt && TypeReport == (int)ETypeReport.OutDate2))
                        {
                            isStatus = true;
                        }
                    }
                }
                else if ((SearchPeriod < VNPTHelper.getPeriodOfMonth(CurrentMonth) && SearchYear == CurrentYear) || (SearchYear < CurrentYear))
                {
                    if (HTDt == null)
                    {
                        if ((StatusTask != (int)EStatusIssueResolve.EndWithDoneProcess
                            && StatusTask != (int)EStatusIssueResolve.EndWithNotProcess))
                        {
                            if ((DKHTDt >= VNPTHelper.getLastDayOfPeriod(SearchPeriod, SearchYear) && TypeReport == (int)ETypeReport.StillValid)
                                || (DKHTDt < VNPTHelper.getLastDayOfPeriod(SearchPeriod, SearchYear) && TypeReport == (int)ETypeReport.OutDate1))
                            {
                                isStatus = true;
                            }
                        }
                    }
                    else
                    {
                        DateTime HTMonth2 = (DateTime)HTDt;
                        int HTMonth = HTMonth2.Month;
                        if (HTDt <= DKHTDt)
                        {
                            if ((VNPTHelper.getPeriodOfMonth(HTMonth) > SearchPeriod && TypeReport == (int)ETypeReport.StillValid)
                                || (VNPTHelper.getPeriodOfMonth(HTMonth) <= SearchPeriod && TypeReport == (int)ETypeReport.OnTime))
                            {
                                isStatus = true;
                            }
                        }
                        else
                        {
                            if ((VNPTHelper.getPeriodOfMonth(HTMonth) > SearchPeriod && TypeReport == (int)ETypeReport.OutDate1)
                                || (VNPTHelper.getPeriodOfMonth(HTMonth) <= SearchPeriod && TypeReport == (int)ETypeReport.OutDate2))
                            {
                                isStatus = true;
                            }
                        }
                    }
                }
            }
            return isStatus;
        }

        public bool getRattingWithPeriod(int CreateMonth, int SearchPeriod, int CurrentMonth,
            int CreateYear, int SearchYear, int CurrentYear, short? StatusTask, short? StatusRatting,
            DateTime? HTDt, DateTime? RatingDt, int TypeReport)
        {
            bool isRatting = false;
            int HTMonth = 0;
            int HTYear = 0;
            if (HTDt != null)
            {
                DateTime HTDt2 = (DateTime)HTDt;
                HTMonth = HTDt2.Month;
                HTYear = HTDt2.Year;
            }
            if (((VNPTHelper.getPeriodOfMonth(CreateMonth) <= SearchPeriod && CreateYear == SearchYear) || (CreateYear < SearchYear))
                && (StatusTask == (int)EStatusIssueResolve.EndWithDoneProcess || StatusTask == (int)EStatusIssueResolve.EndWithNotProcess)
                && ((VNPTHelper.getPeriodOfMonth(HTMonth) == SearchPeriod && HTYear == SearchYear)))
            {
                if (SearchPeriod == VNPTHelper.getPeriodOfMonth(CurrentMonth) && SearchYear == CurrentYear)
                {
                    if (((StatusRatting == (int)EStatusIssueRating.ProcessDone) && (TypeReport == (int)ETypeReport.ApprovedComplete))
                        || ((StatusRatting == (int)EStatusIssueRating.ProcessNotDone) && (TypeReport == (int)ETypeReport.NotComplete))
                        || ((StatusRatting == null) && (TypeReport == (int)ETypeReport.NotRating)))
                    {
                        isRatting = true;
                    }
                }
                else if ((SearchPeriod < VNPTHelper.getPeriodOfMonth(CurrentMonth) && SearchYear == CurrentYear) || (SearchYear < CurrentYear))
                {
                    //get RatingMonth and RatingYear
                    int RatingMonth = 0;
                    int RatingYear = 0;
                    if (RatingDt != null)
                    {
                        DateTime dtValue = (DateTime)RatingDt;
                        RatingMonth = dtValue.Month;
                        RatingYear = dtValue.Year;
                    }
                    if (((StatusRatting == (int)EStatusIssueRating.ProcessDone)
                            && (SearchPeriod == VNPTHelper.getPeriodOfMonth(RatingMonth) && SearchYear == RatingYear)
                            && (TypeReport == (int)ETypeReport.ApprovedComplete)
                         )
                     || (
                            (StatusRatting == (int)EStatusIssueRating.ProcessNotDone)
                            && (SearchPeriod == VNPTHelper.getPeriodOfMonth(RatingMonth) && SearchYear == RatingYear)
                            && (TypeReport == (int)ETypeReport.NotComplete)
                         )
                     || (
                            (
                                (StatusRatting == null)
                                || (SearchPeriod > VNPTHelper.getPeriodOfMonth(RatingMonth) && SearchYear == RatingYear)
                                || (SearchYear > RatingYear)
                            )
                            && (TypeReport == (int)ETypeReport.NotRating)
                         )
                    )
                    {
                        isRatting = true;
                    }
                }
            }

            return isRatting;
        }

    }
}