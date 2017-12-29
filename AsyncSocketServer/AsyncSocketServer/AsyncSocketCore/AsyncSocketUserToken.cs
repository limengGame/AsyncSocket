using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace AsyncSocketServer
{
    public class AsyncSocketUserToken
    {
        protected SocketAsyncEventArgs m_receiveEventArgs;
        public SocketAsyncEventArgs ReceiveEventArgs
        {
            get { return m_receiveEventArgs; }
            set { m_receiveEventArgs = value; }
        }

        private byte[] m_asyncReceiveBuffer;

        protected SocketAsyncEventArgs m_sendEventArgs;
        public SocketAsyncEventArgs SendEventArgs
        {
            get { return m_sendEventArgs; }
            set { m_sendEventArgs = value; }
        }

        protected AsyncSendBufferManager m_sendBuffer;
        public AsyncSendBufferManager SendBuffer
        {
            get { return m_sendBuffer; }
            set { m_sendBuffer = value; }
        }
        
        protected DynamicBufferManager m_receiveBuffer;
        public DynamicBufferManager ReceiveBuffer
        {
            get { return m_receiveBuffer; }
            set { m_receiveBuffer = value; }
        }

        protected AsyncSocketInvokeElement m_asyncSocketInvokeElement;
        public AsyncSocketInvokeElement AsyncSocketInvokeElement
        {
            get { return m_asyncSocketInvokeElement; }
            set { m_asyncSocketInvokeElement = value; }
        }
        
        protected Socket m_connectSocket;
        public Socket ConnectSocket
        {
            get { return m_connectSocket; }
            set {
                m_connectSocket = value;
                if (m_connectSocket == null) //清理缓存
                {
                    if (m_asyncSocketInvokeElement != null)
                        m_asyncSocketInvokeElement.Close();
                    m_receiveBuffer.Clear(m_receiveBuffer.DataCount);
                    m_sendBuffer.ClearPacket();
                }
                m_asyncSocketInvokeElement = null;
                m_receiveEventArgs.AcceptSocket = m_connectSocket;
                m_sendEventArgs.AcceptSocket = m_connectSocket;
            }
        }

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

        public AsyncSocketUserToken(int asyncReceiveBufferSize)
        {
            m_asyncReceiveBuffer = new byte[asyncReceiveBufferSize];
            m_asyncSocketInvokeElement = null;
            m_receiveEventArgs = new SocketAsyncEventArgs();
            m_receiveEventArgs.UserToken = this;
            m_receiveEventArgs.SetBuffer(m_asyncReceiveBuffer, 0, m_asyncReceiveBuffer.Length);

            m_sendEventArgs = new SocketAsyncEventArgs();
            m_sendEventArgs.UserToken = this;

            m_receiveBuffer = new DynamicBufferManager(ProtocolConst.InitBufferSize);
            m_sendBuffer = new AsyncSendBufferManager(ProtocolConst.InitBufferSize);
        }

    }
}
