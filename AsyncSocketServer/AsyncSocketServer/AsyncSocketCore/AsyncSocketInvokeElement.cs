using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public class AsyncSocketInvokeElement
    {
        protected AsyncSocketServer m_asyncSocketServer;
        protected AsyncSocketUserToken m_asyncSocketUserToken;
        public AsyncSocketUserToken AsyncSocketUserToken
        {
            get { return m_asyncSocketUserToken; }
        }

        private bool m_netByteOrder;
        public bool NetByteOrder
        {
            get { return m_netByteOrder; }
            set { m_netByteOrder = value; }
        }

        protected bool m_sendAsync;

        protected IncomingDataParser m_incomingDataParser;
        protected OutgoingDataAssembler m_outgoingDataAssembler;

        protected DateTime m_connectDateTime;
        public DateTime ConnectDateTime
        {
            get { return m_connectDateTime; }
            set { m_connectDateTime = value; }
        }

        protected DateTime m_activeDateTime;
        public DateTime ActiveDateTime
        {
            get { return m_activeDateTime; }
            set { m_activeDateTime = value; }
        }

        public AsyncSocketInvokeElement(AsyncSocketServer asyncSocketServer, AsyncSocketUserToken asyncSocketUserToken)
        {
            m_asyncSocketServer = asyncSocketServer;
            m_asyncSocketUserToken = asyncSocketUserToken;

            m_incomingDataParser = new IncomingDataParser();
            m_outgoingDataAssembler = new OutgoingDataAssembler();

            m_connectDateTime = DateTime.UtcNow;
            m_activeDateTime = DateTime.UtcNow;
            m_sendAsync = false;
        }

        public virtual bool ProcessReceive(byte[] buffer, int offset, int count)
        {
            m_activeDateTime = DateTime.UtcNow;
            DynamicBufferManager receiveBuffer = m_asyncSocketUserToken.ReceiveBuffer;
            receiveBuffer.WriteBuffer(buffer, offset, count);

            bool result = true;
            while (receiveBuffer.DataCount > sizeof(int))
            {
                int packetLength = BitConverter.ToInt32(receiveBuffer.Buffer, 0);
                if (m_netByteOrder)
                    packetLength = System.Net.IPAddress.NetworkToHostOrder(packetLength);

                if ((packetLength > 10 * 1024 * 1024) | (receiveBuffer.DataCount > 10 * 1024 * 1024))
                    return false;

                if ((receiveBuffer.DataCount - sizeof(int)) >= packetLength)
                {
                    //解析数据
                    result = ProcessPacket(receiveBuffer.Buffer, sizeof(int), packetLength);
                    if (result)
                        receiveBuffer.Clear(sizeof(int) + packetLength);
                    else
                        return result;
                }
                else
                    return true;
            }
            
            return true;
        }

        //处理分包后的数据，对命令和数据分开，并对数据进行解析
        public virtual bool ProcessPacket(byte[] buffer, int offset, int count)
        {
            if (count < sizeof(int))
                return false;
            int commandLength = BitConverter.ToInt32(buffer, offset);
            string commandStr = Encoding.UTF8.GetString(buffer, offset + sizeof(int), commandLength);
            //解析命令
            if (!m_incomingDataParser.DecodeProtocolText(commandStr))
                return false;

            return ProcessCommand(buffer, offset + sizeof(int) + commandLength, count - sizeof(int) - commandLength);
        }

        //子类从此方法继承,处理具体命令
        public virtual bool ProcessCommand(byte[] buffer, int offset, int count)
        {
            return true;
        }

        public virtual bool SendCompleted()
        {
            m_activeDateTime = DateTime.UtcNow;
            m_sendAsync = false;

            AsyncSendBufferManager asyncSendBufferManager = m_asyncSocketUserToken.SendBuffer;
            asyncSendBufferManager.ClearFirstPacket();

            int offset = 0;
            int count = 0;
            if (asyncSendBufferManager.GetFirstPacket(ref offset, ref count))
            {
                m_sendAsync = true;
                return m_asyncSocketServer.SendAsyncEvent(m_asyncSocketUserToken.ConnectSocket, m_asyncSocketUserToken.SendEventArgs,
                    asyncSendBufferManager.DynamicBufferManager.Buffer, offset, count);
            }
            else
                return SendCallback();
        }

        //发送回调函数，用于连续发送数据
        public virtual bool SendCallback()
        {
            return true;
        }

        //写入Command
        public bool DoSendResult()
        {
            string commandText = m_outgoingDataAssembler.GetProtocolText();
            byte[] buffer = Encoding.UTF8.GetBytes(commandText);
            int totalLength = sizeof(int) + buffer.Length;
            AsyncSendBufferManager asyncSendBufferManager = m_asyncSocketUserToken.SendBuffer;
            asyncSendBufferManager.StartPacket();
            asyncSendBufferManager.DynamicBufferManager.WriteInt(totalLength, false); //写入总大小
            asyncSendBufferManager.DynamicBufferManager.WriteInt(buffer.Length, false); //写入命令大小
            asyncSendBufferManager.DynamicBufferManager.WriteBuffer(buffer); //写入命令内容
            asyncSendBufferManager.EndPacket();

            bool result = true;
            if (!m_sendAsync)
            {
                int sendOffset = 0;
                int sendCount = 0;
                if (asyncSendBufferManager.GetFirstPacket(ref sendOffset, ref sendCount))
                {
                    m_sendAsync = true;
                    result = m_asyncSocketServer.SendAsyncEvent(m_asyncSocketUserToken.ConnectSocket, m_asyncSocketUserToken.SendEventArgs,
                        asyncSendBufferManager.DynamicBufferManager.Buffer, sendOffset, sendCount);
                }
            }
            return result;
        }

        //写入Command和二进制数据
        public bool DoSendResult(byte[] buffer, int offset, int count)
        {
            string commandText = m_outgoingDataAssembler.GetProtocolText();
            byte[] sendBuffer = Encoding.UTF8.GetBytes(commandText);
            int totalLength = sizeof(int) + sendBuffer.Length + buffer.Length;
            AsyncSendBufferManager asyncSendBufferManager = m_asyncSocketUserToken.SendBuffer;
            asyncSendBufferManager.StartPacket();
            asyncSendBufferManager.DynamicBufferManager.WriteInt(totalLength, false); //写入总大小
            asyncSendBufferManager.DynamicBufferManager.WriteInt(sendBuffer.Length, false); //写入command命令大小
            asyncSendBufferManager.DynamicBufferManager.WriteBuffer(sendBuffer); //写入命令内容
            asyncSendBufferManager.DynamicBufferManager.WriteBuffer(buffer); //写入二进制内容
            asyncSendBufferManager.EndPacket();

            bool result = true;
            if (!m_sendAsync)
            {
                int sendOffset = 0;
                int sendCount = 0;
                result = m_asyncSocketServer.SendAsyncEvent(m_asyncSocketUserToken.ConnectSocket, m_asyncSocketUserToken.SendEventArgs,
                    asyncSendBufferManager.DynamicBufferManager.Buffer, sendOffset, sendCount);
            }
            return result;
        }

        public bool DoSendBuffer(byte[] buffer, int offset, int count) //不是按包格式下发一个内存块，用于日志这类下发协议
        {
            AsyncSendBufferManager asyncSendBufferManager = m_asyncSocketUserToken.SendBuffer;
            asyncSendBufferManager.StartPacket();
            asyncSendBufferManager.DynamicBufferManager.WriteBuffer(buffer, offset, count);
            asyncSendBufferManager.EndPacket();

            bool result = true;
            if (!m_sendAsync)
            {
                int packetOffset = 0;
                int packetCount = 0;
                if (asyncSendBufferManager.GetFirstPacket(ref packetOffset, ref packetCount))
                {
                    m_sendAsync = true;
                    result = m_asyncSocketServer.SendAsyncEvent(m_asyncSocketUserToken.ConnectSocket, m_asyncSocketUserToken.SendEventArgs,
                        asyncSendBufferManager.DynamicBufferManager.Buffer, packetOffset, packetCount);
                }
            }
            return result;
        }

        public virtual void Close()
        {

        }

    }
}
