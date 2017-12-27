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



}
