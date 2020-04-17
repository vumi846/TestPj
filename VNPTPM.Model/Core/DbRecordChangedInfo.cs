using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    [Serializable]
    public class DbRecordChangedInfo : ICloneable
    {
        public DbRecordChangedInfo() { }

        public DbRecordChangedInfo(string P_fieldName, object p_OriginalValue, object p_ChangedValue)
        {
            fieldName = P_fieldName;
            originalValue = p_OriginalValue;
            changedValue = p_ChangedValue;
        }

        string fieldName = "";
        public string FieldName { get { return fieldName; } set { fieldName = value; } }

        object changedValue;
        public object ChangedValue { get { return changedValue; } set { changedValue = value; } }

        object originalValue;
        public object OriginalValue { get { return originalValue; } set { originalValue = value; } }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return FieldName + ": " + (originalValue == null ? "" : originalValue.ToString()) + " - " + (changedValue == null ? "" : changedValue.ToString());
        }
    }
}
