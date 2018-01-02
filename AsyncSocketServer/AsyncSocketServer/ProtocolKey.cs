using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public class ProtocolConst
    {
        public static int InitBufferSize = 1024 * 4; //初始化缓存大小
        public static int ReceiveBufferSize = 1024 * 4; //接收缓存大小
        public static int SocketTimeOutMS = 60 * 1000; //超时时间为60S
    }

    public class ProtocolKey
    {
        public static string Request = "Request";
        public static string Response = "Response";
        public static string LeftBrackets = "[";
        public static string RightBrackets = "]";
        public static string ReturnWrap = "\r\n";
        public static string EqualSign = "=";
        public static string Command = "Command";
        public static string Code = "Code";
        public static string Message = "Message";
        public static string UserName = "UserName";
        public static string Password = "Password";
        public static string FileName = "FileName";
        public static string Item = "Item";
        public static string ParentDir = "ParentDir";
        public static string DirName = "DirName";
        public static char TextSeperator = (char)1;
        public static string FileSize = "FileSize";
        public static string PacketSize = "PacketSize";

        public static string FileExists = "FileExists";
        public static string OpenFile = "OpenFile";
        public static string SetSize = "SetSize";
        public static string GetSize = "GetSize";
        public static string SetPosition = "SetPosition";
        public static string GetPosition = "GetPosition";
        public static string Read = "Read";
        public static string Write = "Write";
        public static string Seek = "Seek";
        public static string CloseFile = "CloseFile";
        public static string Mode = "Mode";
        public static string Size = "Size";
        public static string Position = "Position";
        public static string Count = "Count";
        public static string Offset = "Offset";
        public static string SeekOrigin = "SeekOrigin";
        public static string Login = "Login";
        public static string Active = "Active";
        public static string GetClients = "GetClients";
        public static string Dir = "Dir";
        public static string CreateDir = "CreateDir";
        public static string DeleteDir = "DeleteDir";
        public static string FileList = "FileList";
        public static string DeleteFile = "DeleteFile";
        public static string Upload = "Upload";
        public static string Data = "Data";
        public static string Eof = "Eof";
        public static string Download = "Download";
        public static string SendFile = "SendFile";
        public static string CyclePacket = "CyclePacket";
    }


    public class ProtocolCode
    {
        public static int Success = 0x00000000;
        public static int NotExistCommand = Success + 0x01;
        public static int PacketLengthError = Success + 0x02;
        public static int PacketFormatError = Success + 0x03;
        public static int UnknowError = Success + 0x04;
        public static int CommandNoCompleted = Success + 0x05;
        public static int ParameterError = Success + 0x06;
        public static int UserOrPasswordError = Success + 0x07;
        public static int UserHasLogined = Success + 0x08;
        public static int FileNotExist = Success + 0x09;
        public static int NotOpenFile = Success + 0x0A;
        public static int FileIsInUse = Success + 0x0B;

        public static int DirNotExist = 0x02000001;
        public static int CreateDirError = 0x02000002;
        public static int DeleteDirError = 0x02000003;
        public static int DeleteFileFailed = 0x02000007;
        public static int FileSizeError = 0x02000008;

        public static string GetErrorCodeString(int errorCode)
        {
            string errorString = null;
            if (errorCode == NotExistCommand)
                errorString = "Not Exist Command";
            return errorString;
        }
    }

    public enum DownloadSocketCommand
    {
        None = 0,
        Login = 1,
        Active = 2,
        Dir = 3,
        FileList = 4,
        Download = 5,
    }

    public enum UploadSocketCommand
    {
        None = 0,
        Login = 1,
        Active = 2,
        Dir = 3,
        CreateDir = 4,
        DeleteDir = 5,
        FileList = 6,
        DeleteFile = 7,
        Upload = 8,
        Data = 9,
        Eof = 10,
    }

}
