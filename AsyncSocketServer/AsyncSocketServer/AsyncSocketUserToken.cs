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

        protected DynamicBufferManager m_receiveBuffer;
        public DynamicBufferManager ReceiveBuffer
        {
            get { return m_receiveBuffer; }
            set { m_receiveBuffer = value; }
        }


        protected Socket m_connectSocket;
        public Socket ConnectSocket
        {
            get { return m_connectSocket; }
            set {


            }
        }



    }
}
