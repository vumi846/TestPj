//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VNPTPM.Model
{
    using System;
    using System.Collections.Generic;
    
    [Serializable]
    public partial class DATA_MEETING: ICloneable
    {
    	public DATA_MEETING () {}
    
    	#region ICloneable Members
        public DATA_MEETING Clone()
        {
            return (DATA_MEETING)this.MemberwiseClone(); 
        }
    
        object ICloneable.Clone()
        {
            return (DATA_MEETING)this.MemberwiseClone();
        }
        #endregion
        public System.Guid ID { get; set; }
        public string Name { get; set; }
        public string Admin { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> MeetingDate { get; set; }
        public Nullable<System.DateTime> StartAt { get; set; }
        public Nullable<System.DateTime> EndAt { get; set; }
        public Nullable<bool> ApproveFlg { get; set; }
        public string ApproveBy { get; set; }
        public Nullable<System.DateTime> ApproveAt { get; set; }
        public Nullable<System.DateTime> CreateAt { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<bool> DelFlg { get; set; }
        public Nullable<System.DateTime> DelAt { get; set; }
        public string DelBy { get; set; }
    }
}