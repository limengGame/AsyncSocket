using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AsyncSocketServer
{
    public class DownloadSocketProtocol : BaseSocketProtocol
    {
        private string m_fileName;
        public string FileName { get { return this.m_fileName; } }

        private FileStream m_fileStream;
        private bool m_sendFile;
        private int m_packetSize;
        private byte[] readBuffer;
        
        public DownloadSocketProtocol(AsyncSocketServer asyncSocketServer, AsyncSocketUserToken asyncSocketUserToken)
            : base(asyncSocketServer, asyncSocketUserToken)
        {
            m_fileName = "";
            m_socketFlag = "Download";
            m_fileStream = null;
            m_sendFile = false;
            m_packetSize = 64 * 124;
            lock (m_asyncSocketServer.DownloadSocketProtocolMgr)
            {
                m_asyncSocketServer.DownloadSocketProtocolMgr.Add(this);
            }
        }

        public override void Close()
        {
            base.Close();
            m_fileName = "";
            m_sendFile = false;
            if (m_fileStream != null)
            {
                m_fileStream = null;
            }
            lock (m_asyncSocketServer.DownloadSocketProtocolMgr)
            {
                m_asyncSocketServer.DownloadSocketProtocolMgr.Remove(this);
            }
        }

        public override bool ProcessCommand(byte[] buffer, int offset, int count)
        {
            DownloadSocketCommand command = StrToCommand(m_incomingDataParser.Command);
            m_outgoingDataAssembler.Clear();
            m_outgoingDataAssembler.AddResponse();
            m_outgoingDataAssembler.AddCommand(m_incomingDataParser.Command);
            if (!CheckLogined(command))
            {
                m_outgoingDataAssembler.AddFailure(ProtocolCode.UserHasLogined, "");
                return DoSendResult();
            }
            if (command == DownloadSocketCommand.Active)
                return DoActive();
            else if (command == DownloadSocketCommand.Login)
                return DoLogin();
            else if (command == DownloadSocketCommand.Dir)
                return DoDir();
            else if (command == DownloadSocketCommand.FileList)
                return DoFileList();
            else if (command == DownloadSocketCommand.Download)
                return DoDownload();
            else
            {
                Program.Logger.ErrorFormat("Unknown Command : ", m_incomingDataParser.Command);
                return false;
            }
        }

        public DownloadSocketCommand StrToCommand(string command)
        {
            if (command.Equals(ProtocolKey.Active, StringComparison.CurrentCultureIgnoreCase))
                return DownloadSocketCommand.Active;
            else if (command.Equals(ProtocolKey.Login, StringComparison.CurrentCultureIgnoreCase))
                return DownloadSocketCommand.Login;
            else if (command.Equals(ProtocolKey.Dir, StringComparison.CurrentCultureIgnoreCase))
                return DownloadSocketCommand.Dir;
            else if (command.Equals(ProtocolKey.FileList, StringComparison.CurrentCultureIgnoreCase))
                return DownloadSocketCommand.FileList;
            else if (command.Equals(ProtocolKey.Download, StringComparison.CurrentCultureIgnoreCase))
                return DownloadSocketCommand.Download;
            else
                return DownloadSocketCommand.None;
        }

        public bool CheckLogined(DownloadSocketCommand command)
        {
            if ((command == DownloadSocketCommand.Login) | (command == DownloadSocketCommand.Active))
                return true;
            else
                return m_logined;
        }

        public bool DoDir()
        {

            return true;
        }

        public bool DoFileList()
        {

            return true;
        }

        public bool DoDownload()
        {

            return true;
        }


    }



    public class DownloadSocketProtocolMgr : Object
    {
        private List<DownloadSocketProtocol> m_list;

        public DownloadSocketProtocolMgr()
        {
            this.m_list = new List<DownloadSocketProtocol>();
        }

        public int Count()
        {
            return m_list.Count;
        }

        public DownloadSocketProtocol ElementAt(int index)
        {
            return m_list.ElementAt(index);
        }

        public void Add(DownloadSocketProtocol value)
        {
            m_list.Add(value);
        }

        public void Remove(DownloadSocketProtocol value)
        {
            m_list.Remove(value);
        }

    }

}
