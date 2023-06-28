
using RestSharp;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Admin.NET.Core
{
    public static class WebUtils
    {
        /// <summary>
        /// 对 URL 字符串进行编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str, Encoding e)
        {
            return HttpUtility.UrlEncode(str, e);
        }

        /// <summary>
        /// 将已经为在 URL 中传输而编码的字符串转换为解码的字符串
        /// </summary> 
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str, Encoding e)
        {
            return HttpUtility.UrlDecode(str, e);
        }

		public static string TrimAll(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return "";
			}
			return input.Trim(new char[] { ' ', '\t', '\r', '\n' });
		}


		#region Get

		/// <summary>
		/// RestSharper Get
		/// </summary>
		/// <param name="host">域名</param>
		/// <param name="action">方法</param>
		/// <returns></returns>
		/// <remarks>
		/// host 为空或空字符串，则返回 string.Empty
		/// </remarks>
		public static string Get(string host, string action)
        {
            if (host.IsNullOrEmpty())
                return string.Empty;
            var client = new RestClient(host);
            var request = new RestRequest(action, Method.Get);
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            var response = client.Execute(request);
            var content = response.Content;
            Console.WriteLine("======request:"+host+action);
            return content;
        }

       
        #endregion

        /// <summary>
        /// 将键值对转换成URL参数
        /// </summary>
        /// <param name="nvc"></param>
        /// <returns></returns>
        public static string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return string.Join("&", array);
        }
    }
}
