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
    public partial class AD_USER: ICloneable
    {
    	public AD_USER () {}
    
    	#region ICloneable Members
        public AD_USER Clone()
        {
            return (AD_USER)this.MemberwiseClone(); 
        }
    
        object ICloneable.Clone()
        {
            return (AD_USER)this.MemberwiseClone();
        }
        #endregion
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public Nullable<bool> LockFlg { get; set; }
        public Nullable<bool> DelFlg { get; set; }
        public string RoleID { get; set; }
        public string PartID { get; set; }
        public string UnitID { get; set; }
        public Nullable<System.DateTime> CreateAt { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string Phone { get; set; }
    }
}
