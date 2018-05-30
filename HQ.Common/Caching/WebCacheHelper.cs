using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Collections;

namespace HQ.Common.Caching
{
    /// <summary>
    /// ��װ��system.web.caching.cache�Ļ���������
    /// </summary>
    public class WebCacheHelper
    {
        public delegate object CacheLoaderDelegate();

        public delegate void CacheLoaderErrorDeletegate(string key, Exception e);

        public static CacheLoaderErrorDeletegate _cacheLoaderErrorDelegate = null;

        private static readonly System.Web.Caching.Cache _cache = HttpRuntime.Cache;

        private WebCacheHelper()
        {         
        }

        #region �������

        /// <summary>
        /// ��ջ���
        /// </summary>
        public static void Clear()
        {
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            ArrayList list = new ArrayList();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Key);
            }

            foreach (string str in list)
            {
                Remove(str);
            }
        }

        /// <summary>
        /// �Ƴ���������
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// ���������Ƴ�����
        /// </summary>
        /// <param name="pattern"></param>
        public static void RemoveByPattern(string pattern)
        {

        }
        #endregion

        #region ���뻺��
        /// <summary>
        /// ���뻺�� δʹ���ļ������ڹ���ʱ��
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Insert(string key, object obj)
        {
            if (obj!=null)
            {
                BaseInsert(key, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.AboveNormal, null);
            }
        }

        /// <summary>
        /// ���뻺�� ��ʹ�ù���ʱ��
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="absoluteExprition"></param>
        public static void Insert(string key, object obj, TimeSpan absoluteExprition)
        {
            Insert(key, obj, absoluteExprition, null);
        }

        /// <summary>
        /// ���뻺�� ��ʹ���ļ�����
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="dep"></param>
        public static void Insert(string key, object obj, CacheDependency dep)
        {
            Insert(key, obj, new TimeSpan(100,0,0,0), dep);
        }

        /// <summary>
        /// ���뻺�� ʹ���ļ����������ʱ��
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="absoluteExprition"></param>
        /// <param name="dep"></param>
        public static void Insert(string key, object obj, TimeSpan absoluteExprition, CacheDependency dep)
        {
            Insert(key, obj, absoluteExprition, dep, System.Web.Caching.CacheItemPriority.Normal, null);
        }

        /// <summary>
        /// ���뻺�� ʹ���ļ����������ʱ�䣬������û������ȼ��뻺��ʧЧ�ص�����
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="absoluteExprition"></param>
        /// <param name="dep"></param>
        /// <param name="priority"></param>
        /// <param name="onRemoveCallback"></param>
        public static void Insert(string key, object obj, TimeSpan absoluteExprition, CacheDependency dep, System.Web.Caching.CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            if (obj != null)
            {
                BaseInsert(key, obj, dep, DateTime.Now.Add(absoluteExprition), System.Web.Caching.Cache.NoSlidingExpiration, priority, onRemoveCallback);
            }
        }

        private static void BaseInsert(string key, object value, CacheDependency dep, DateTime absoluteExpiration, TimeSpan slidingExpiration, System.Web.Caching.CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            if (value != null)
            {
                _cache.Insert(key, value, dep, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
            }
        }

        #endregion

        #region ��ȡ����
        public static object Get(string key)
        {
            return _cache[key];
        }

        public static object Get(string key, TimeSpan absoluteExprition, CacheLoaderDelegate cacheLoader)
        {
            object obj = _cache[key];
            if (obj == null)
            {
                if (cacheLoader != null)
                {
                    try
                    {
                        obj = cacheLoader();
                        Insert(key, obj, absoluteExprition);
                    }
                    catch (Exception ex)
                    {
                        if (_cacheLoaderErrorDelegate != null)
                        {
                            _cacheLoaderErrorDelegate(key, ex);
                        }
                    }
                }
            }
            return obj;
        }

        public static object Get(string key, CacheDependency dep, CacheLoaderDelegate cacheLoader)
        {
            object obj = _cache[key];
            if (obj == null)
            {
                if (cacheLoader != null)
                {
                    try
                    {
                        obj = cacheLoader();
                        Insert(key, obj, dep);
                    }
                    catch (Exception ex)
                    {
                        if (_cacheLoaderErrorDelegate != null)
                        {
                            _cacheLoaderErrorDelegate(key, ex);
                        } 
                    }
                }
            }
            return obj;
        }

        public static object Get(string key, TimeSpan absoluteExprition, CacheDependency dep, WebCacheHelper.CacheLoaderDelegate cacheLoader)
        {
            object obj = _cache[key];
            if (obj == null)
            {
                if (cacheLoader != null)
                {
                    try
                    {
                        obj = cacheLoader();
                        Insert(key, obj, absoluteExprition, dep);
                    }
                    catch (Exception ex)
                    {
                        if (_cacheLoaderErrorDelegate != null)
                        {
                            _cacheLoaderErrorDelegate(key, ex);
                        }
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// �õ����л���
        /// </summary>
        public static System.Web.Caching.Cache CacheList
        {
            get
            {
                return _cache;
            }
        }
        #endregion

        #region ����Ƿ����
        /// <summary>
        /// ����Ƿ����
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Contains(string key)
        {
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().Equals(key, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        public static void SetCacheLoaderErrorHandler(CacheLoaderErrorDeletegate handler)
        {
            _cacheLoaderErrorDelegate = handler;
        }
    }

    /// <summary>
    /// �������֣��ڲ��ѽ�������ת��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebCacheHelper<T> where T : class
    {
        /// <summary>
        /// ��ȡ����ֵ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get(string key)
        {
            T obj = WebCacheHelper.Get(key) as T;
            return obj;
        }

        /// <summary>
        /// ��ȡ����ֵ����������ڣ������cacheLoader��absoluteExpritionʱ��κ�ʧЧ
        /// </summary>
        /// <param name="key"></param>
        /// <param name="absoluteExprition"></param>
        /// <param name="cacheLoader"></param>
        /// <returns></returns>
        public static T Get(string key, TimeSpan absoluteExprition, WebCacheHelper.CacheLoaderDelegate cacheLoader)
        {
            return WebCacheHelper.Get(key, absoluteExprition, cacheLoader) as T;
        }

        /// <summary>
        /// ��ȡ����ֵ����������ڣ������cacheLoader��ʹ���ļ���������
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dep"></param>
        /// <param name="cacheLoader"></param>
        /// <returns></returns>
        public static T Get(string key, CacheDependency dep, WebCacheHelper.CacheLoaderDelegate cacheLoader)
        {
            return WebCacheHelper.Get(key, dep, cacheLoader) as T;
        }

        /// <summary>
        /// ��ȡ����ֵ����������ڣ������cacheLoader��ʹ��ʱ��κ��ļ�������������
        /// </summary>
        /// <param name="key"></param>
        /// <param name="absoluteExprition"></param>
        /// <param name="dep"></param>
        /// <param name="cacheLoader"></param>
        /// <returns></returns>
        public static T Get(string key, TimeSpan absoluteExprition,CacheDependency dep, WebCacheHelper.CacheLoaderDelegate cacheLoader)
        {
            return WebCacheHelper.Get(key, absoluteExprition, dep, cacheLoader) as T;
        }
    }
}
