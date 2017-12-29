using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public class IncomingDataParser
    {
        private string m_header;
        public string Header { get { return m_header; } }

        private string m_command;
        public string Command { get { return m_command; } }

        private List<string> m_names;
        public List<string> Names { get { return m_names; } }

        private List<string> m_values;
        public List<string> Values { get { return m_values; } }

        public IncomingDataParser()
        {
            m_names = new List<string>();
            m_values = new List<string>();
        }

        public bool DecodeProtocolText(string protocolText)
        {
            m_header = "";
            m_names.Clear();
            m_values.Clear();
            int speIndex = protocolText.IndexOf(ProtocolKey.ReturnWrap);
            if (speIndex < 0)
            {
                return false;
            }
            else
            {
                string[] tempNameValues = protocolText.Split(new string[] { ProtocolKey.ReturnWrap }, StringSplitOptions.RemoveEmptyEntries);
                if (tempNameValues.Length < 2)
                    return false;
                for (int i = 0; i < tempNameValues.Length; i++)
                {
                    string[] tempStr = tempNameValues[i].Split(new string[] { ProtocolKey.EqualSign }, StringSplitOptions.None);
                    if (tempStr.Length > 1)
                    {
                        if (tempStr.Length > 2)
                            return false;
                        if (tempStr[0].Equals(ProtocolKey.Command, StringComparison.CurrentCultureIgnoreCase))
                            m_command = tempStr[0];
                        else
                        {
                            m_names.Add(tempStr[0].ToLower());
                            m_values.Add(tempStr[1]);
                        }
                    }
                }
                return true;
            }
        }

        public bool GetValue(string protocolKey, ref string value)
        {
            int index = m_names.IndexOf(protocolKey.ToLower());
            if (index < 0)
                return false;
            else
                value = m_values[index];
            return true;
        }

        public List<string> GetValue(string protocolKey)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < m_names.Count; i++)
            {
                if (m_names[i].Equals(protocolKey, StringComparison.CurrentCultureIgnoreCase))
                    result.Add(m_names[i]);
            }
            return result;
        }

        public bool GetValue(string protocolKey, ref short value)
        {
            int index = m_names.IndexOf(protocolKey.ToLower());
            if (index < 0)
                return false;
            else
                return short.TryParse(m_values[index], out value);
        }

        public bool GetValue(string protocolKey, ref int value)
        {
            int index = m_names.IndexOf(protocolKey.ToLower());
            if (index < 0)
                return false;
            else
                return int.TryParse(m_values[index], out value);
        }

        public bool GetValue(string protocolKey, ref long value)
        {
            int index = m_names.IndexOf(protocolKey.ToLower());
            if (index < 0)
                return false;
            else
                return long.TryParse(m_values[index], out value);
        }

        public bool GetValue(string protocolKey, ref Single value)
        {
            int index = m_names.IndexOf(protocolKey.ToLower());
            if (index < 0)
                return false;
            else
                return Single.TryParse(m_values[index], out value);
        }
        
        public bool GetValue(string protocolKey, ref Double value)
        {
            int index = m_names.IndexOf(protocolKey.ToLower());
            if (index < 0)
                return false;
            else
                return Double.TryParse(m_values[index], out value);
        }

    }
}
