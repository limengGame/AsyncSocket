using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    struct SendBufferPacket
    {
        public int Offset;
        public int Count;
    }
    
    public class AsyncSendBufferManager
    {
        private DynamicBufferManager m_dynamicBufferManager;
        public DynamicBufferManager DynamicBufferManager
        {
            get { return m_dynamicBufferManager; }
        }
        private List<SendBufferPacket> m_sendBufferPacketList;
        private SendBufferPacket m_sendBufferPaclet;


        public AsyncSendBufferManager(int bufferSize)
        {
            m_dynamicBufferManager = new DynamicBufferManager(bufferSize);
            m_sendBufferPacketList = new List<SendBufferPacket>();
            m_sendBufferPaclet.Offset = 0;
            m_sendBufferPaclet.Count = 0;
        }

        public void StartPacket()
        {
            m_sendBufferPaclet.Offset = m_dynamicBufferManager.DataCount;
            m_sendBufferPaclet.Count = 0;
        }

        public void EndPacket()
        {
            m_sendBufferPaclet.Count = m_dynamicBufferManager.DataCount - m_sendBufferPaclet.Offset;
            m_sendBufferPacketList.Add(m_sendBufferPaclet);
        }

        public bool GetFirstPacket(ref int offset, ref int count)
        {
            if (m_sendBufferPacketList.Count <= 0)
                return false;
            offset = 0;
            count = m_sendBufferPacketList[0].Count;
            return true;
        }

        public bool ClearFirstPacket()
        {
            if (m_sendBufferPacketList.Count <= 0)
                return false;
            int count = m_sendBufferPacketList[0].Count;
            m_dynamicBufferManager.Clear(count);
            m_sendBufferPacketList.RemoveAt(0);
            return true;
        }

        public void ClearPacket()
        {
            m_sendBufferPacketList.Clear();
            m_dynamicBufferManager.Clear(m_dynamicBufferManager.DataCount);
        }

    }
}
