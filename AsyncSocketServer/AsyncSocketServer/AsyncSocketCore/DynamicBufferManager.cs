using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public class DynamicBufferManager
    {
        public byte[] Buffer { get; set; }
        public int DataCount;

        public DynamicBufferManager(int bufferSize)
        {
            DataCount = 0;
            Buffer = new byte[bufferSize];
        }

        public int GetReserveCount()
        {
            return Buffer.Length - DataCount;
        }

        public void Clear()
        {
            DataCount = 0;
        }

        public void Clear(int count)
        {
            if (DataCount <= count)
            {
                DataCount = 0;
            }
            else
            {
                for (int i = 0; i < DataCount - count; i++)
                {
                    Buffer[i] = Buffer[count + i];
                }
                DataCount = DataCount - count;
            }
        }

        public void SetBufferSize(int size)
        {
            if (Buffer.Length < size)
            {
                byte[] tempBuffer = new byte[size];
                Array.Copy(Buffer, tempBuffer, DataCount);
                Buffer = tempBuffer;
            }
        }

        public void WriteBuffer(byte[] buffer, int offset, int count)
        {
            if (GetReserveCount() >= count)
            {
                Array.Copy(buffer, offset, Buffer, DataCount, count);
                DataCount = DataCount + count;
            }
            else
            {
                int totalSize = DataCount - GetReserveCount() + count;
                byte[] tempBuffer = new byte[totalSize];
                Array.Copy(Buffer, 0, tempBuffer, 0, DataCount);
                Array.Copy(buffer, offset, tempBuffer, DataCount, count);
                DataCount = DataCount + count;
                Buffer = tempBuffer;
            }
        }

        public void WriteBuffer(byte[] buffer)
        {
            WriteBuffer(buffer, 0, buffer.Length);
        }

        public void WriteShort(short value, bool convert)
        {
            if (convert)
            {
                //Net是小头结构，网络字节是大头结构，需要客户端服务器商量好
                value = System.Net.IPAddress.HostToNetworkOrder(value);
            }
            byte[] buffer = BitConverter.GetBytes(value);
            WriteBuffer(buffer);
        }

        public void WriteInt(int value, bool convert)
        {
            if (convert)
            {
                value = System.Net.IPAddress.HostToNetworkOrder(value);
            }
            byte[] buffer = BitConverter.GetBytes(value);
            WriteBuffer(buffer);
        }

        public void WriteString(string value)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            WriteBuffer(buffer);
        }
        
    }
}
