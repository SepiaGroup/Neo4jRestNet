using System;
using System.Net;
using System.Text;
using Neo4jRestNet.Core;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Rest
{
    public class HttpRest
    {
		private static HttpStatusCode BaseRestRequest(string url, string method, string body)
		{

			HttpWebRequest w = (HttpWebRequest)WebRequest.Create(url);
			w.Proxy = null;
			w.Method = method;
			w.ContentType = "application/json";

			if (!string.IsNullOrEmpty(body))
			{
				Stream dataStream = w.GetRequestStream();

				byte[] b = Encoding.UTF8.GetBytes(body);
				dataStream.Write(b, 0, b.Length);
				dataStream.Close();
			}

			WebResponse resp = w.GetResponse();
			resp.Close();

			HttpStatusCode StatusCode = ((HttpWebResponse)resp).StatusCode;

			return StatusCode;
		}

		private static HttpStatusCode BaseRestRequest(string url, string method, string body, out string response)
        {

			HttpWebRequest w = (HttpWebRequest)WebRequest.Create(url);
			w.Proxy = null;
			w.Method = method;
			w.ContentType = "application/json";
			w.Accept = "application/json";

			if (!string.IsNullOrEmpty(body))
			{
				Stream dataStream = w.GetRequestStream();

				byte[] b = Encoding.UTF8.GetBytes(body);
				dataStream.Write(b, 0, b.Length);
				dataStream.Close();
			}

			WebResponse resp = w.GetResponse();

			StreamReader reader = new StreamReader(resp.GetResponseStream());

			response = reader.ReadToEnd();

			reader.Close();
			resp.Close();

			HttpStatusCode StatusCode = ((HttpWebResponse)resp).StatusCode;

			return StatusCode;
        }

        /// <summary>
        /// Http GET Request
        /// </summary>
        /// <param name="url">What to get</param>
        /// <returns>Response body from server</returns>
		public static HttpStatusCode Get(string url, out string response)
        {
            return BaseRestRequest(url, "GET", null, out response);
        }

        /// <summary>
        /// Http POST Request
        /// </summary>
        /// <param name="url">Where to post</param>
        /// <param name="body">Data to post</param>
        /// <returns>Response body from server</returns>
		public static HttpStatusCode Post(string url, string body, out string response)
        {
            return BaseRestRequest(url, "POST", body, out response);
        }

        /// <summary>
        /// Http PUT Request
        /// </summary>
        /// <param name="url">Where to put</param>
        /// <param name="body">Data to put</param>
        /// <returns></returns>
		public static HttpStatusCode Put(string url, string body)
        {
            return BaseRestRequest(url, "PUT", body);
        }

        /// <summary>
        /// Http DELETE Request
        /// </summary>
        /// <param name="url">What to delete</param>
        /// <returns>Response body from server</returns>
		public static HttpStatusCode Delete(string url)
        {
            return BaseRestRequest(url, "DELETE", null);
        }
    }
}