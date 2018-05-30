﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HQ.Common
{
    /// <summary>
    /// 页面基础类
    /// </summary>
    public class PageBaseHelper : Page
    {

        #region 基本判断
        /// <summary>
        /// 是否为POST提交方式   在这里可以判断是否重复提交的  Request.Headers["Accept"] != "*/*"
        /// </summary>
        public bool IsHttpPOST { get { return Request.HttpMethod == "POST"; } }
        /// <summary>
        /// 是否为异步请求
        /// </summary>
        public bool IsAjaxRequest
        {
            get
            {
                string sheader = Request.Headers["X-Requested-With"];
                return (sheader != null && sheader == "XMLHttpRequest");
            }
        }
        #endregion
        
        #region 页面取值
        /// <summary>
        /// 获取Request.QueryString中的指定键的值
        /// </summary>
        /// <param name="sQueryKey">键</param>
        /// <param name="sDefaultValue">默认值</param>
        /// <returns></returns>
        public string GetQueryString(string sQueryKey, string sDefaultValue)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request.QueryString[sQueryKey] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.QueryString[sQueryKey]))
            {
                return HttpContext.Current.Request.QueryString[sQueryKey];
            }
            return sDefaultValue;
        }

        /// <summary>
        /// 判断参数中是否含有某个某个参数键值
        /// </summary>
        /// <param name="key">参数key值</param>
        /// <returns></returns>
        public bool isHasQuery(string key) {
            if (HttpContext.Current.Request.QueryString != null) {
                string[] keys = HttpContext.Current.Request.QueryString.ToString().Split('&');
                if (keys.Contains("back")) {
                    return true;
                } else if (HttpContext.Current.Request.QueryString[key]!=null) {
                    return true;
                }
            }
           return false;
           ////System.Collections.Specialized.NameObjectCollectionBase.KeysCollection keys= HttpContext.Current.Request.QueryString.Keys;
           // if (HttpContext.Current != null && HttpContext.Current.Request.QueryString[key] != null) {
           //     return true;
           // }
           // return false;
        }

        /// <summary>
        /// 获取Request.QueryString中的指定键的值
        /// </summary>
        /// <param name="sQueryKey">键</param>
        /// <param name="iDefaultValue">默认值</param>
        /// <returns></returns>
        public int GetQueryString(string sQueryKey, int iDefaultValue)
        {
            int iValue = 0;
            if (Int32.TryParse(GetQueryString(sQueryKey, iDefaultValue.ToString()), out iValue))
            {
                return iValue;
            }
            return iDefaultValue;
        }

        public long GetQueryString(string sQueryKey, long lDefaultValue)
        {
            long lValue = 0;
            if (long.TryParse(GetQueryString(sQueryKey, lDefaultValue.ToString()), out lValue))
            {
                return lValue;
            }
            return lDefaultValue;
        }

        /// <summary>
        /// 获取Request.QueryString中的指定键的值
        /// </summary>
        /// <param name="sQueryKey">键</param>
        /// <param name="dDefaultValue">默认值</param>
        /// <returns></returns>
        public double GetQueryString(string sQueryKey, double dDefaultValue)
        {
            double dValue = 0;
            if (double.TryParse(GetQueryString(sQueryKey, dDefaultValue.ToString()), out dValue))
            {
                return dValue;
            }
            return dDefaultValue;
        }
        /// <summary>
        /// 获取Request.Form中的指定键的值
        /// </summary>
        /// <param name="sFormName">键</param>
        /// <param name="sDefaultValue">默认值</param>
        /// <returns></returns>
        public string GetFormValue(string sFormName, string sDefaultValue)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request.Form[sFormName] != null)
            {
                return HttpContext.Current.Request.Form[sFormName];
            }
            return sDefaultValue;
        }

        /// <summary>
        /// 获取Request.Form中的指定键的值
        /// </summary>
        /// <param name="sFormName">键</param>
        /// <param name="iDefaultValue">默认值</param>
        /// <returns></returns>
        public int GetFormValue(string sFormName, int iDefaultValue)
        {
            int iValue = 0;
            if (Int32.TryParse(GetFormValue(sFormName, iDefaultValue.ToString()), out iValue))
            {
                return iValue;
            }
            return iDefaultValue;
        }

        /// <summary>
        /// 获取Request.Form中的指定键的值
        /// </summary>
        /// <param name="sFormName">键</param>
        /// <param name="iDefaultValue">默认值</param>
        /// <returns></returns>
        public double GetFormValue(string sFormName, double iDefaultValue) {
            double iValue = 0;
            if (Double.TryParse(GetFormValue(sFormName, iDefaultValue.ToString()), out iValue)) {
                return iValue;
            }
            return iDefaultValue;
        }

        /// <summary>
        /// 转换json格式
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string GetJson(object value)
        {
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            };
            return JsonConvert.SerializeObject(value, timeConverter);
        }
        #endregion

        
        /// <summary>
        /// 获取 get/post 数据
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public JObject GetHttpParams()
        {
            JObject p = new JObject();
            if (Request.HttpMethod.ToLower() == "post") //post 数据请求
            {
                if (Request.InputStream.Length > 0) //判断是否以字节流形式post数据还是以keyValuePair形式
                {
                    //web client提交
                    using (StreamReader sr = new StreamReader(Request.InputStream, Encoding.UTF8))
                    {
                        string respone = sr.ReadToEnd();
                        if (respone != null && !string.IsNullOrEmpty(respone))
                        {
                            respone = HttpContext.Current.Server.UrlDecode(respone);
                            // 开始分析参数对   
                            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
                            MatchCollection mc = re.Matches(respone);
                            foreach (Match m in mc)
                            {
                                p.Add(m.Result("$2"), m.Result("$3"));
                            }
                        }
                    }
                }
                else
                {
                    NameValueCollection values = HttpContext.Current.Request.Form;
                    foreach (string m in values.Keys)
                    {
                        p.Add(m, values[m]);
                    }
                }
            }
            else  //get 数据请求
            {
                string url = Request.Url.ToString();
                if (url == null || url == "")
                    return p;
                int questionMarkIndex = url.IndexOf('?');
                if (questionMarkIndex == url.Length - 1)
                    return p;
                string ps = url.Substring(questionMarkIndex + 1);
                if (ps != null && !string.IsNullOrEmpty(ps))
                {
                    ps = HttpContext.Current.Server.UrlDecode(ps);
                    // 开始分析参数对   
                    Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
                    MatchCollection mc = re.Matches(ps);
                    foreach (Match m in mc)
                    {
                        p.Add(m.Result("$2"), m.Result("$3"));
                    }
                }
            }
            return p;
        }

    }
}
