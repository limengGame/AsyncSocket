using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer
{
    public class AsyncSocketUserTokenPool
    {
        private Stack<AsyncSocketUserToken> m_pool;

        public AsyncSocketUserTokenPool(int capacity)
        {
            m_pool = new Stack<AsyncSocketUserToken>(capacity);
        }

        public void Push(AsyncSocketUserToken token)
        {
            if (token == null)
            {
                throw new Exception("add to AsyncSocketUserToken Pool cannot be null!");
            }
            else
            {
                m_pool.Push(token);
            }
        }

        public AsyncSocketUserToken Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }
        
        public int Count
        {
            get { return m_pool.Count; }
        }
    }

    public class AsyncSocketUserTokenList
    {
        private List<AsyncSocketUserToken> m_list;

        public AsyncSocketUserTokenList()
        {
            m_list = new List<AsyncSocketUserToken>();
        }

        public void Add(AsyncSocketUserToken token)
        {
            m_list.Add(token);
        }

        public void Remove(AsyncSocketUserToken token)
        {
            if (m_list.Contains(token))
            {
                m_list.Remove(token);
            }
        }

        public void CopyList(ref AsyncSocketUserToken[] array)
        {
            lock (m_list)
            {
                array = new AsyncSocketUserToken[m_list.Count];
                m_list.CopyTo(array);
            }
        }
    }
    
}
