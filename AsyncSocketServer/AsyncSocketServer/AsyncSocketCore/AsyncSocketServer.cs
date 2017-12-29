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
    public class AsyncSocketServer
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

        public void Init()
        {
            AsyncSocketUserToken userToken = null;
            for (int i = 0; i < m_numConnections; i++)
            {
                userToken = new AsyncSocketUserToken(m_receiveBufferSize);
                userToken.ReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                userToken.SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                m_AsyncSocketUserTokenPool.Push(userToken);
            }
        }

        public void Start(IPEndPoint endPoint)
        {
            listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(endPoint);
            listenSocket.Listen(m_numConnections);
            Program.Logger.InfoFormat("Start listen endPoint: {0}", endPoint.ToString());
            StartAccept(null);
        }

        public void StartAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs == null)
            {
                acceptEventArgs = new SocketAsyncEventArgs();
                acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
                acceptEventArgs = null;

            m_maxNumberAcceptedClients.WaitOne();
            bool waitAsync = listenSocket.AcceptAsync(acceptEventArgs);
            if (!waitAsync)
            {
                ProcessAccept(acceptEventArgs);
            }

        }

        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            try
            {
                ProcessAccept(asyncEventArgs);
            }
            catch (Exception ex)
            {
                Program.Logger.ErrorFormat("Accept client {0}, error message {1}", asyncEventArgs.AcceptSocket, ex.Message);
                Program.Logger.Error(ex.StackTrace);
            }
        }

        void IO_Completed(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            AsyncSocketUserToken userToken = asyncEventArgs.UserToken as AsyncSocketUserToken;
            userToken.ActiveDateTime = DateTime.Now;
            try
            {
                lock (userToken)
                {
                    if (asyncEventArgs.LastOperation == SocketAsyncOperation.Receive)
                    {
                        ProcessReceive(asyncEventArgs);
                    }
                    else if (asyncEventArgs.LastOperation == SocketAsyncOperation.Send)
                    {
                        ProcessSend(asyncEventArgs);
                    }
                    else
                        throw new ArgumentException("The last operation on the socket was not a receive or a send.");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.ErrorFormat("IOCompleted client {0}, error message {1}", userToken.ConnectSocket, ex.Message);
                Program.Logger.Error(ex.StackTrace);
            }
        }

        void ProcessAccept(SocketAsyncEventArgs asyncEventArgs)
        {
            Program.Logger.InfoFormat("Client connection accepted, LocalEndPoint:{0}, RemoteEndPoint:{1}",
                asyncEventArgs.AcceptSocket.LocalEndPoint.ToString(), asyncEventArgs.AcceptSocket.RemoteEndPoint.ToString());

            AsyncSocketUserToken userToken = m_AsyncSocketUserTokenPool.Pop();
            m_AsyncSocketUserTokenList.Add(userToken);
            userToken.ConnectSocket = asyncEventArgs.AcceptSocket;
            userToken.ConnectDateTime = DateTime.Now;

            try
            {
                bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(asyncEventArgs);
                if (!willRaiseEvent)
                {
                    lock (userToken)
                    {
                        ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.ErrorFormat("Accept client {0}, error message {1}", userToken.ConnectSocket, ex.Message);
                Program.Logger.Error(ex.StackTrace);
            }
            //把当前异步释放，等待下次连接
            StartAccept(asyncEventArgs);
        }

        void ProcessReceive(SocketAsyncEventArgs asyncEventArgs)
        {
            AsyncSocketUserToken userToken = asyncEventArgs.UserToken as AsyncSocketUserToken;
            if (userToken.ConnectSocket == null)
                return;
            userToken.ActiveDateTime = DateTime.Now;

            if (userToken.ReceiveEventArgs.BytesTransferred > 0 && userToken.ReceiveEventArgs.SocketError == SocketError.Success)
            {
                int offset = userToken.ReceiveEventArgs.Offset;
                int count = userToken.ReceiveEventArgs.Count;
                //存在socket对象，并且没有绑定协议对象，则进行协议对象绑定
                if (userToken.AsyncSocketInvokeElement == null & userToken.ConnectSocket != null)
                {
                    BuildingSocketInvokeElement(userToken);
                    offset = offset + 1;
                    count = count - 1;
                }
                if (userToken.AsyncSocketInvokeElement == null)
                {
                    Program.Logger.WarnFormat("Illegal Client connection, LocalEndPoint:{0}, RemoteEndPoint:{1}",
                        asyncEventArgs.AcceptSocket.LocalEndPoint.ToString(), asyncEventArgs.AcceptSocket.RemoteEndPoint.ToString());
                    CloseClientSocket(userToken);
                }
                else
                {
                    if (count > 0)
                    {
                        if (!userToken.AsyncSocketInvokeElement.ProcessReceive(userToken.ReceiveEventArgs.Buffer, offset, count))
                        {
                            CloseClientSocket(userToken);
                        }
                        else
                        {
                            bool willRaiseEvenet = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs);
                            if (!willRaiseEvenet)
                            {
                                ProcessReceive(userToken.ReceiveEventArgs);
                            }
                        }
                    }
                    else
                    {
                        bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs);
                        if (!willRaiseEvent)
                        {
                            ProcessReceive(userToken.ReceiveEventArgs);
                        }
                    }
                }
            }
            else
            {
                CloseClientSocket(userToken);
            }

        }

        private void BuildingSocketInvokeElement(AsyncSocketUserToken userToken)
        {

        }

        private bool ProcessSend(SocketAsyncEventArgs sendAsyncArgs)
        {
            AsyncSocketUserToken userToken = sendAsyncArgs.UserToken as AsyncSocketUserToken;
            if (userToken.AsyncSocketInvokeElement == null)
                return false;
            userToken.ActiveDateTime = DateTime.Now;
            if (sendAsyncArgs.SocketError == SocketError.Success)
                return userToken.AsyncSocketInvokeElement.SendCompleted();
            else
            {
                CloseClientSocket(userToken);
                return false;
            }
        }

        public bool SendAsyncEvent(Socket connectSocket, SocketAsyncEventArgs asyncEventArgs, byte[] buffer, int offset, int count)
        {
            if (connectSocket == null)
                return false;
            asyncEventArgs.SetBuffer(buffer, offset, count);
            bool willRaiseEvent = connectSocket.SendAsync(asyncEventArgs);
            if (!willRaiseEvent)
            {
                ProcessSend(asyncEventArgs);
            }
            return true;
        }


        public void CloseClientSocket(AsyncSocketUserToken userToken)
        {
            if (userToken.ConnectSocket == null)
                return;
            string socketInfo = string.Format("Local Address: {0} Remote Address: {1}", userToken.ConnectSocket.LocalEndPoint,
                userToken.ConnectSocket.RemoteEndPoint);
            Program.Logger.InfoFormat("Client connection disconnected. {0}", socketInfo);
            try
            {
                userToken.ConnectSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Program.Logger.ErrorFormat("CloseClientSocket Disconnect client {0} error, message: {1}", socketInfo, ex.Message);
            }
            userToken.ConnectSocket.Close();
            userToken.ConnectSocket = null;

            m_maxNumberAcceptedClients.Release();
            m_AsyncSocketUserTokenPool.Push(userToken);
            m_AsyncSocketUserTokenList.Remove(userToken);
        }

    }
}
