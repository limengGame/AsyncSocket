using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public class OutgoingDataAssembler
    {
        private List<string> m_protocolText;

        public OutgoingDataAssembler()
        {
            m_protocolText = new List<string>();
        }

        public void Clear()
        {
            m_protocolText.Clear();
        }

        public string GetProtocolText()
        {
            string tempStr = "";
            if (m_protocolText.Count > 0)
            {
                tempStr = m_protocolText[0];
                for (int i = 1; i < m_protocolText.Count; i++)
                {
                    tempStr += ProtocolKey.ReturnWrap + m_protocolText[i];
                }
            }
            return tempStr;
        }

        public void AddRequest()
        {
            //[Request]
            m_protocolText.Add(ProtocolKey.LeftBrackets + ProtocolKey.Request + ProtocolKey.RightBrackets);
        }

        public void AddResponse()
        {
            //[Response]
            m_protocolText.Add(ProtocolKey.LeftBrackets + ProtocolKey.Response + ProtocolKey.RightBrackets);
        }

        public void AddCommand(string commandKey)
        {
            //Command=
            m_protocolText.Add(ProtocolKey.Command + ProtocolKey.EqualSign + commandKey);
        }

        public void AddSuccess()
        {
            m_protocolText.Add(ProtocolKey.Code + ProtocolKey.EqualSign + ProtocolCode.Success.ToString());//Code=
        }

        public void AddFailure(int errorCode, string message)
        {
            m_protocolText.Add(ProtocolKey.Code + ProtocolKey.EqualSign + errorCode.ToString());
            m_protocolText.Add(ProtocolKey.Message + ProtocolKey.EqualSign + message);
        }

        public void AddValue(string protocolKey, string value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value);
        }

        public void AddValue(string protocolKey, short value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
        }

        public void AddValue(string protocolKey, int value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
        }

        public void AddValue(string protocolKey, long value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
        }

        public void AddValue(string protocolKey, Single value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
        }

        public void AddValue(string protocolKey, double value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
        }

    }
}
