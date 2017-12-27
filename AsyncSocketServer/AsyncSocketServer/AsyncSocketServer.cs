using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AsyncSocketServer
{
    class AsyncSocketServer
    {
        private Socket listenSocket;
        private int m_numConnections; //最大连接数
        private int m_receiveBufferSize; //最大接收缓存大小
        private Semaphore m_maxNumberAcceptedClients;

        private int m_socketTimeOutMS;
        public int SocketTimeOutMS {
            get { return this.m_socketTimeOutMS; }
            set { SocketTimeOutMS = value; }
        }

        private AsyncSocketUserTokenPool m_AsyncSocketUserTokenPool;
        private AsyncSocketUserTokenList m_AsyncSocketUserTokenList;
        public AsyncSocketUserTokenList AsyncSocketUserTokenList
        {
            get { return m_AsyncSocketUserTokenList; }
        }



        public AsyncSocketServer(int numConnections)
        {
            m_numConnections = numConnections;
            m_receiveBufferSize = ProtocolConst.ReceiveBufferSize;

            m_AsyncSocketUserTokenPool = new AsyncSocketUserTokenPool(m_numConnections);
            m_AsyncSocketUserTokenList = new AsyncSocketUserTokenList();
            m_maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);
            

        }




    }
}
