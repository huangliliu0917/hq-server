using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HQ.Common.Caching
{
    /// <summary>
    /// ���������
    /// </summary>
    /// <returns></returns>
    public delegate object CacheLoaderDelegate();

    /// <summary>
    /// ͨ�û����࣬������win/web
    /// </summary>
    public class CommonCacheHelper
    {
        private static readonly Dictionary<string, CacheEntry> _dicCache = new Dictionary<string, CacheEntry>(); //dictionary �����̰߳�ȫ 
        private static ReaderWriterLock _rwLock = new ReaderWriterLock(); //��д��������һ������¶�Ϊ��������д�������٣�����ѡ���

        /// <summary>
        /// ���ػ������������ʱ�������
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static CacheEntry GetCacheEntry(string key)
        {
            CacheEntry entry = null;
            try
            {
                _rwLock.AcquireReaderLock(1000);
                if (Contains(key))
                {
                    entry = Instance[key];
                }
                return entry;
                //return new Exception("CacheEntry�����ڣ���ʹ�� Get(string key, CacheLoaderDelegate loader)����");
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }
        }


        /// <summary>
        /// �Ƿ����
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Contains(string key)
        {
            return Instance.ContainsKey(key);
        }

        /// <summary>
        /// ���뻺��
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        public static void Insert(string key, object content)
        {
            try
            {
                _rwLock.AcquireWriterLock(500);
                if (!Contains(key))
                {
                    CacheEntry entry = new CacheEntry();
                    entry.Content = content;
                    DateTime dtNow = DateTime.Now;
                    entry.CreateTime = dtNow;
                    entry.LastUpdate = dtNow;
                    entry.CacheLoader = null;                    
                    _dicCache.Add(key, entry);
                }
            }
            finally
            {
                 _rwLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// ��ȡ����ֵ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            CacheEntry entry = null;
            try
            {
                _rwLock.AcquireReaderLock(1000);
                if (Contains(key))
                {
                    entry = Instance[key];
                }
                if (entry != null)
                {
                    return entry.Content;
                }
                return new Exception("CacheEntry�����ڣ���ʹ�� Get(string key, CacheLoaderDelegate loader)����");
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }
        }


        /// <summary>
        /// ��ȡ����ֵ����
        /// </summary>
        /// <param name="key"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        public static object Get(string key, CacheLoaderDelegate loader)
        {
            CacheEntry entry = null;
            try
            {
                
                if (Contains(key))
                {
                    _rwLock.AcquireReaderLock(1000);
                    entry = Instance[key];
                }
                else
                {
                    entry = new CacheEntry();
                    if (loader != null)
                    {
                        _rwLock.AcquireWriterLock(500);
                        entry.Content = loader();
                        DateTime dtNow = DateTime.Now;
                        entry.CreateTime = dtNow;
                        entry.LastUpdate = dtNow;
                        entry.CacheLoader = loader;
                        _dicCache.Add(key,entry);
                    }
                }
                return entry.Content;
            }
            finally
            {
                if(_rwLock.IsReaderLockHeld)
                    _rwLock.ReleaseReaderLock();
                if (_rwLock.IsWriterLockHeld)
                    _rwLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// ���ݼ�ֵ���»���
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        public static void Update(string key, object content)
        {
            try
            {
                if(Contains(key))
                {
                    _rwLock.AcquireWriterLock(500);
                    _dicCache[key].Content = content;
                    _dicCache[key].LastUpdate = DateTime.Now;
                }
            }
            finally
            {
                if (_rwLock.IsWriterLockHeld)
                {
                    _rwLock.ReleaseWriterLock();
                }
            }
        }

        /// <summary>
        /// ���ݼ�ֵ�Ƴ�����
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            try
            {
                if (Contains(key))
                {
                    _rwLock.AcquireWriterLock(500);
                    _dicCache.Remove(key);
                }
            }
            finally
            {
                if (_rwLock.IsWriterLockHeld)
                {
                    _rwLock.ReleaseWriterLock();
                }
            }
            
        }

        /// <summary>
        /// ��ջ���
        /// </summary>
        /// <returns></returns>
        public static bool Clear()
        {
            if (_dicCache.Count == 0)
            {
                try
                {
                    _rwLock.AcquireWriterLock(500);
                    _dicCache.Clear();
                    return true;
                }
                finally
                {
                    _rwLock.ReleaseWriterLock();
                }
            }
            return false;

        }

        /// <summary>
        /// ���ػ����ֵ�
        /// </summary>
        public static Dictionary<string, CacheEntry> Instance
        {
            get
            {
                return _dicCache;
            }
        }

        

    }
}
