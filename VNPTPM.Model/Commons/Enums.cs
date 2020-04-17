using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Commons
{
    public enum EStatus
    {
        Fail = 0,
        Ok = 1
    }

    public enum EStatusFile
    {
        OverWeight = 0,
        PathNotExist = 1,
        Base64Incorrect = 2,
    }

    public enum EAction
    {
        Get,
        Insert,
        Update,
        Delete
    }

    public enum EAutoNumberName
    {
    }

    public enum EAccSetting
    {
        RAD,
        MED
    }

    public enum EApprovedFlg
    {
        Waiting,
        Approved,
        Reject
    }

    public enum EUnitAmount
    {
        VNĐ = 1,
        USD = 2,
    }

    public enum EIntervalToDue
    {
        Interval = 3    // 3 days to due
    }

    public enum ETypeHistory
    {
        Move = 0,
        SeenDeal = 1,
        SeenBranch = 2,
    }

    public enum EMethodPay
    {
        CashPayment = 1,
        OnlinePayment = 2,
    }

    public enum EStatusPay
    {
        Unpaid = 0,
        Running = 1,
        Finish = 2,
    }
    public enum EStatusTask
    {
        OverDue = 0,
        IncomingDue = 1,
        Normal = -1
    }
    public enum EStatusIssueResolve
    {
        NotStart = 0,
        Started = 1,
        EndWithDoneProcess = 2,
        EndWithNotProcess = 3
    }
    public enum EStatusIssueRating
    {
        ProcessDone = 4,
        ProcessNotDone = 5
    }
    public enum EStatusIssueComment
    {
        CreateWithFileAtach = 0,
        CommentNormal = 1,
        CommentRating = 2,
        ReopendIssue = 3
    }
    public enum ETypeReport
    {
        Default = 0,
        Unexpired = 1,
        Expired = 2,
        LeaderinMonth = 3,
        LeaderinNextMonth = 4,
        CompleteinMonth = 5,
        StillValid = 6,
        OutDate1 = 7,
        OnTime = 8,
        OutDate2 = 9,
        ApprovedComplete = 10,
        NotComplete = 11,
        NotRating = 12,
        Sum = 13,
        SumOut = 14
    }
    public enum EStatusSearchReport
    {
        SearchPeriod = 0,
        SearchMonth = 1
    }
    public enum EStatusPeriodOfYear
    {
        season1 = 1,
        season2 = 2,
        season3 = 3,
        season4 = 4
    }
}
