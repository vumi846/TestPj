using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Core
{
    [Serializable]
    public class DbKeyMember : ICloneable
    {
        public DbKeyMember() { }
        string m_keyName = "";
        object m_Value;

        public DbKeyMember(string keyName, object keyValue)
        {
            m_keyName = keyName;
            m_Value = keyValue;
        }

        public string Key { get { return m_keyName; } set { m_keyName = value; } }
        public object Value { get { return m_Value; } set { m_Value = value; } }

        public override string ToString()
        {
            return m_keyName + " - " + (m_Value == null ? "" : m_Value.ToString());
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
