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
    public partial class DATA_IDEA_DETAIL: ICloneable
    {
    	public DATA_IDEA_DETAIL () {}
    
    	#region ICloneable Members
        public DATA_IDEA_DETAIL Clone()
        {
            return (DATA_IDEA_DETAIL)this.MemberwiseClone(); 
        }
    
        object ICloneable.Clone()
        {
            return (DATA_IDEA_DETAIL)this.MemberwiseClone();
        }
        #endregion
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> IdeaID { get; set; }
        public string UserName { get; set; }
        public Nullable<bool> ApproveFlg { get; set; }
        public Nullable<System.DateTime> ApproveAt { get; set; }
        public Nullable<System.DateTime> CreateAt { get; set; }
    }
}