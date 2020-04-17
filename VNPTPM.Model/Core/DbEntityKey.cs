using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    [Serializable]
    public class DbEntityKey : ICloneable
    {
        public DbEntityKey() { }

        public DbEntityKey(string p_tblName, DbKeyMember[] p_keyVal)
        {
            tblName = p_tblName;
            keyValues = p_keyVal;
        }

        string tblName = "";
        public string TblName { get { return tblName; } set { tblName = value; } }

        DbKeyMember[] keyValues;
        public DbKeyMember[] KeyValues { get { return keyValues; } set { keyValues = value; } }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
