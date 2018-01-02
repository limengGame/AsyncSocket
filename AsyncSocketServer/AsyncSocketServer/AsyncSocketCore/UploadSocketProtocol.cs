using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AsyncSocketServer
{
    public class UploadSocketProtocol : BaseSocketProtocol
    {
        private string m_fileName;
        public string FileName { get { return m_fileName; } }
        private FileStream m_fileStream;

        public UploadSocketProtocol(AsyncSocketServer asyncSocketServer, AsyncSocketUserToken asyncSocketUserToken)
            : base(asyncSocketServer, asyncSocketUserToken)
        {
            m_socketFlag = "Upload";
            m_fileName = "";
            m_fileStream = null;
            lock (m_asyncSocketServer.UploadSocketProtocolMgr)
            {
                m_asyncSocketServer.UploadSocketProtocolMgr.Add(this);
            }
        }

        public override void Close()
        {
            base.Close();
            m_fileName = "";
            if (m_fileStream != null)
            {
                m_fileStream.Close();
                m_fileStream = null;
            }
            lock (m_asyncSocketServer.UploadSocketProtocolMgr)
            {
                m_asyncSocketServer.UploadSocketProtocolMgr.Remove(this);
            }
        }

        public override bool ProcessCommand(byte[] buffer, int offset, int count)
        {
            UploadSocketCommand command = StrToCommand(m_incomingDataParser.Command);
            m_outgoingDataAssembler.Clear();
            m_outgoingDataAssembler.AddResponse();
            m_outgoingDataAssembler.AddCommand(m_incomingDataParser.Command);
            if (!CheckLogined(command))
            {
                m_outgoingDataAssembler.AddFailure(ProtocolCode.UserHasLogined, "");
                return DoSendResult();
            }
            if (command == UploadSocketCommand.Login)
                return DoLogin();
            else if (command == UploadSocketCommand.Active)
                return DoActive();
            else if (command == UploadSocketCommand.Dir)
                return DoDir();
            else if (command == UploadSocketCommand.CreateDir)
                return DoCreateDir();
            else if (command == UploadSocketCommand.DeleteDir)
                return DoDeleteDir();
            else if (command == UploadSocketCommand.FileList)
                return DoFileList();
            else if (command == UploadSocketCommand.DeleteFile)
                return DoDeleteFile();
            else if (command == UploadSocketCommand.Upload)
                return DoUpload();
            else if (command == UploadSocketCommand.Data)
                return DoData(buffer, offset, count);
            else if (command == UploadSocketCommand.Eof)
                return DoEof();
            else
            {
                Program.Logger.Error("Unknow command: " + m_incomingDataParser.Command);
                return false;
            }

        }

        public bool DoDir()
        {

            return true;
        }

        public bool DoCreateDir()
        {

            return true;
        }
        public bool DoDeleteDir()
        {

            return true;
        }
        public bool DoFileList()
        {

            return true;
        }

        public bool DoDeleteFile()
        {

            return true;
        }
        public bool DoUpload()
        {

            return true;
        }
        public bool DoData()
        {

            return true;
        }
        public bool DoEof()
        {

            return true;
        }

        public bool CheckLogined(UploadSocketCommand command)
        {
            if ((command == UploadSocketCommand.Login) | (command == UploadSocketCommand.Active))
                return true;
            else
                return m_logined;
        }

        public UploadSocketCommand StrToCommand(string command)
        {
            if (command.Equals(ProtocolKey.Active, StringComparison.CurrentCultureIgnoreCase))
                return UploadSocketCommand.Active;
            else if (command.Equals(ProtocolKey.Login, StringComparison.CurrentCultureIgnoreCase))
                return UploadSocketCommand.Login;
            else if (command.Equals(ProtocolKey.Dir, StringComparison.CurrentCultureIgnoreCase))
                return UploadSocketCommand.Dir;
            else if (command.Equals(ProtocolKey.CreateDir, StringComparison.CurrentCultureIgnoreCase))
                return UploadSocketCommand.CreateDir;
            else if (command.Equals(ProtocolKey.DeleteDir, StringComparison.CurrentCultureIgnoreCase))
                return UploadSocketCommand.DeleteDir;
            else if (command.Equals(ProtocolKey.FileList, StringComparison.CurrentCultureIgnoreCase))
                return UploadSocketCommand.FileList;
            else if (command.Equals(ProtocolKey.DeleteFile, StringComparison.CurrentCultureIgnoreCase))
                return UploadSocketCommand.DeleteFile;
            else if (command.Equals(ProtocolKey.Upload, StringComparison.CurrentCultureIgnoreCase))
                return UploadSocketCommand.Upload;
            else if (command.Equals(ProtocolKey.Data, StringComparison.CurrentCultureIgnoreCase))
                return UploadSocketCommand.Data;
            else if (command.Equals(ProtocolKey.Eof, StringComparison.CurrentCultureIgnoreCase))
                return UploadSocketCommand.Eof;
            else
                return UploadSocketCommand.None;
        }

    }


    public class UploadSocketProtocolMgr : Object
    {
        private List<UploadSocketProtocol> m_list;

        public UploadSocketProtocolMgr()
        {
            m_list = new List<UploadSocketProtocol>();
        }

        public int Count()
        {
            return m_list.Count;
        }

        public UploadSocketProtocol ElementAt(int index)
        {
            return m_list.ElementAt(index);
        }

        public void Add(UploadSocketProtocol value)
        {
            m_list.Add(value);
        }

        public void Remove(UploadSocketProtocol value)
        {
            m_list.Remove(value);
        }
    }


}
