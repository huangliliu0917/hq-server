﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using HQ.PddOpen.Core;
using HQ.PddOpen.Core.Entities;
using HQ.PddOpen.Core.Exceptions;

namespace Senparc.Weixin.HttpUtility
{
    public static class Post
    {
        /// <summary>
        /// 获取Post结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="returnText"></param>
        /// <returns></returns>
        public static T GetResult<T>(string returnText)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            if (returnText.Contains("error_code"))
            {
                //可能发生错误
                ErrorJsonResult errorResult = js.Deserialize<ErrorJsonResult>(returnText);
                //发生错误
                throw new ErrorJsonResultException(
                    string.Format("拼多多Post请求发生错误！错误代码：{0}，说明：{1}",
                                  (int)errorResult.error_code,
                                  errorResult.error_msg),
                    null, errorResult);
            }

            T result = js.Deserialize<T>(returnText);
            return result;
        }


        #region 同步方法

        /// <summary>
        /// 发起Post请求
        /// </summary>
        /// <typeparam name="T">返回数据类型（Json对应的实体）</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="cookieContainer">CookieContainer，如果不需要则设为null</param>
        /// <param name="encoding"></param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <param name="fileDictionary"></param>
        /// <param name="postDataDictionary"></param>
        /// <returns></returns>
        public static T PostFileGetJson<T>(string url, CookieContainer cookieContainer = null, Dictionary<string, string> fileDictionary = null, Dictionary<string, string> postDataDictionary = null, Encoding encoding = null, int timeOut = Config.TIME_OUT)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                postDataDictionary.FillFormDataStream(ms); //填充formData
                string returnText = RequestUtility.HttpPost(url, cookieContainer, ms, fileDictionary, null, encoding, timeOut: timeOut);
                var result = GetResult<T>(returnText);
                return result;
            }
        }

        /// <summary>
        /// 发起Post请求
        /// </summary>
        /// <typeparam name="T">返回数据类型（Json对应的实体）</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="cookieContainer">CookieContainer，如果不需要则设为null</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="encoding"></param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <param name="checkValidationResult">验证服务器证书回调自动验证</param>
        /// <returns></returns>
        public static T PostGetJson<T>(string url, CookieContainer cookieContainer = null, Stream fileStream = null, Encoding encoding = null, int timeOut = Config.TIME_OUT, bool checkValidationResult = false)
        {
            string returnText = RequestUtility.HttpPost(url, cookieContainer, fileStream, null, null, encoding, timeOut: timeOut, checkValidationResult: checkValidationResult);

            //WeixinTrace.SendLog(url, returnText);

            var result = GetResult<T>(returnText);
            return result;
        }

        /// <summary>
        /// PostGetJson
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="formData"></param>
        /// <param name="encoding"></param>
        /// <param name="timeOut"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T PostGetJson<T>(string url, CookieContainer cookieContainer = null, Dictionary<string, string> formData = null, Encoding encoding = null, int timeOut = Config.TIME_OUT)
        {
            string returnText = RequestUtility.HttpPost(url, cookieContainer, formData, encoding, timeOut: timeOut);
            var result = GetResult<T>(returnText);
            return result;
        }

        /// <summary>
        /// 使用Post方法上传数据并下载文件或结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="stream"></param>
        public static void Download(string url, string data, Stream stream)
        {
            WebClient wc = new WebClient();
            var file = wc.UploadData(url, "POST", Encoding.UTF8.GetBytes(string.IsNullOrEmpty(data) ? "" : data));
            foreach (var b in file)
            {
                stream.WriteByte(b);
            }
        }

        #endregion

        #region 异步方法

        /// <summary>
        /// PostFileGetJson的异步版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="fileDictionary"></param>
        /// <param name="encoding"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static async Task<T> PostFileGetJsonAsync<T>(string url, CookieContainer cookieContainer = null, Dictionary<string, string> fileDictionary = null, Encoding encoding = null, int timeOut = Config.TIME_OUT)
        {
            string returnText = await RequestUtility.HttpPostAsync(url, cookieContainer, null, fileDictionary, null, encoding, timeOut);
            var result = GetResult<T>(returnText);
            return result;
        }

        /// <summary>
        /// PostGetJson的异步版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="fileStream"></param>
        /// <param name="encoding"></param>
        /// <param name="timeOut"></param>
        /// <param name="checkValidationResult"></param>
        /// <returns></returns>
        public static async Task<T> PostGetJsonAsync<T>(string url, CookieContainer cookieContainer = null, Stream fileStream = null, Encoding encoding = null, int timeOut = Config.TIME_OUT, bool checkValidationResult = false)
        {
            string returnText = await RequestUtility.HttpPostAsync(url, cookieContainer, fileStream, null, null, encoding, timeOut, checkValidationResult: checkValidationResult);
            var result = GetResult<T>(returnText);
            return result;
        }

        /// <summary>
        /// PostGetJson的异步版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="formData"></param>
        /// <param name="encoding"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static async Task<T> PostGetJsonAsync<T>(string url, CookieContainer cookieContainer = null, Dictionary<string, string> formData = null, Encoding encoding = null, int timeOut = Config.TIME_OUT)
        {
            string returnText = await RequestUtility.HttpPostAsync(url, cookieContainer, formData, encoding, timeOut);
            var result = GetResult<T>(returnText);
            return result;
        }

        /// <summary>
        /// 【异步方法】发起Post请求
        /// </summary>
        /// <typeparam name="T">返回数据类型（Json对应的实体）</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="cookieContainer">CookieContainer，如果不需要则设为null</param>
        /// <param name="encoding"></param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <param name="fileDictionary"></param>
        /// <param name="postDataDictionary"></param>
        /// <returns></returns>
        public static async Task<T> PostFileGetJsonAsync<T>(string url, CookieContainer cookieContainer = null, Dictionary<string, string> fileDictionary = null, Dictionary<string, string> postDataDictionary = null, Encoding encoding = null, int timeOut = Config.TIME_OUT)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                postDataDictionary.FillFormDataStream(ms); //填充formData
                string returnText = await RequestUtility.HttpPostAsync(url, cookieContainer, ms, fileDictionary, null, encoding, timeOut: timeOut);
                var result = GetResult<T>(returnText);
                return result;
            }
        }

        /// <summary>
        /// 使用Post方法上传数据并下载文件或结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="stream"></param>
        public static async Task DownloadAsync(string url, string data, Stream stream)
        {
            WebClient wc = new WebClient();

            var fileBytes = await wc.UploadDataTaskAsync(url, "POST", Encoding.UTF8.GetBytes(string.IsNullOrEmpty(data) ? "" : data));
            await stream.WriteAsync(fileBytes, 0, fileBytes.Length);//也可以分段写入
        }



        #endregion
    }
}
