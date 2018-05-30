using System;
using System.Collections.Generic;
using System.Text;

namespace HQ.Common.Caching
{
    /// <summary>
    /// CommonCacheHelper�л���ʵ��
    /// </summary>
    public class CacheEntry
    {
        private DateTime _createTime = DateTime.Now;
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }


        private DateTime _lastUpdate;
        /// <summary>
        /// ��һ�θ���ʱ��
        /// </summary>
        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; }
        }

        private object _content;
        /// <summary>
        /// �������ֵ
        /// </summary>
        public object Content
        {
            get { return _content; }
            set { _content = value; }
        }

        private CacheLoaderDelegate loader;
        /// <summary>
        /// ����ί��
        /// </summary>
        public CacheLoaderDelegate CacheLoader
        {
            get { return loader; }
            set { loader = value; }
        }
	
	
    }
}
