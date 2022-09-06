using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace gxy
{
    class http
    {

        public static string Get(string url)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "GET";
                webRequest.Timeout = 30000;
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36";
                webRequest.KeepAlive = false;
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                StreamReader sr = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Post(string url, string token, string sign, string roleKey, string jsonStr)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(jsonStr);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Timeout = 30000;
                webRequest.Method = "Post";
                webRequest.UserAgent = "Mozilla/5.0 (Linux; Android 7.0; HTC M9e Build/EZG0TF) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/55.0.1566.54 Mobile Safari/537.36";
                webRequest.ContentType = "application/json; charset=UTF-8";
                webRequest.Host = "api.moguding.net:9000";
                webRequest.Headers["Accept-Language"] = "zh-CN,zh;q=0.8";
                webRequest.Headers["Sign"] = sign;
                webRequest.Headers["Authorization"] = token;
                webRequest.Headers["roleKey"] = roleKey;
                webRequest.Headers["Accept-Encoding"] = "";
                webRequest.Headers["Cache-Control"] = "no-cache";
                webRequest.ContentLength = data.Length;

                using (Stream stream = webRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
