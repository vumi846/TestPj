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
    public partial class AD_LOG: ICloneable
    {
    	public AD_LOG () {}
    
    	#region ICloneable Members
        public AD_LOG Clone()
        {
            return (AD_LOG)this.MemberwiseClone(); 
        }
    
        object ICloneable.Clone()
        {
            return (AD_LOG)this.MemberwiseClone();
        }
        #endregion
        public System.Guid ID { get; set; }
        public string UserName { get; set; }
        public string ServiceName { get; set; }
        public Nullable<int> ActionRec { get; set; }
        public string Data { get; set; }
        public Nullable<System.DateTime> CreateAt { get; set; }
        public string IpAddress { get; set; }
    }
}