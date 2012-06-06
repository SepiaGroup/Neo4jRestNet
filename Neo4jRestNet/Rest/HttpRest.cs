using System.Net;
using System.Text;
using System.IO;

namespace Neo4jRestNet.Rest
{
    public class HttpRest
    {
		public enum Method
		{
			Delete,
			Get,
			Post,
			Put
		}

		private static HttpStatusCode BaseRestRequest(string url, Method method, string body)
		{

			var w = (HttpWebRequest)WebRequest.Create(url);
			w.Proxy = null;
			w.Method = method.ToString();
			w.ContentType = "application/json";

			if (!string.IsNullOrEmpty(body))
			{
				var dataStream = w.GetRequestStream();

				byte[] b = Encoding.UTF8.GetBytes(body);
				dataStream.Write(b, 0, b.Length);
				dataStream.Close();
			}

			HttpStatusCode statusCode;
			try
			{
				var resp = w.GetResponse();

				resp.Close();

				statusCode = ((HttpWebResponse) resp).StatusCode;
			}
			catch (WebException ex)
			{
				var webResponse = ex.Response;
				
				webResponse.Close();
				
				statusCode = ((HttpWebResponse)webResponse).StatusCode;
			}

			return statusCode;
		}

		private static HttpStatusCode BaseRestRequest(string url, Method method, string body, out string response)
        {

			var w = (HttpWebRequest)WebRequest.Create(url);
			w.Proxy = null;
			w.Method = method.ToString();
			w.ContentType = "application/json";
			w.Accept = "application/json";

			if (!string.IsNullOrEmpty(body))
			{
				Stream dataStream = w.GetRequestStream();

				byte[] b = Encoding.UTF8.GetBytes(body);
				dataStream.Write(b, 0, b.Length);
				dataStream.Close();
			}

			HttpStatusCode statusCode;
			try
			{
				var resp = w.GetResponse();

				var reader = new StreamReader(resp.GetResponseStream());

				response = reader.ReadToEnd();

				reader.Close();
				resp.Close();

				statusCode = ((HttpWebResponse) resp).StatusCode;
			}
			catch (WebException ex)
			{
				
				var webResponse = ex.Response;

				var reader = new StreamReader(webResponse.GetResponseStream());

				response = reader.ReadToEnd();

				reader.Close();
				webResponse.Close();

				statusCode = ((HttpWebResponse)webResponse).StatusCode;
			}

			return statusCode;
        }

    	/// <summary>
    	/// Http GET Request
    	/// </summary>
    	/// <param name="url">What to get</param>
    	/// <param name="response">body</param>
    	/// <returns>Response body from server</returns>
    	public static HttpStatusCode Get(string url, out string response)
        {
            return BaseRestRequest(url, Method.Get, null, out response);
        }

    	/// <summary>
    	/// Http POST Request
    	/// </summary>
    	/// <param name="url">Where to post</param>
    	/// <param name="body">Data to post</param>
    	/// <param name="response"></param>
    	/// <returns>Response body from server</returns>
    	public static HttpStatusCode Post(string url, string body, out string response)
        {
            return BaseRestRequest(url, Method.Post, body, out response);
        }

        /// <summary>
        /// Http PUT Request
        /// </summary>
        /// <param name="url">Where to put</param>
        /// <param name="body">Data to put</param>
        /// <returns></returns>
		public static HttpStatusCode Put(string url, string body)
        {
            return BaseRestRequest(url, Method.Put, body);
        }

        /// <summary>
        /// Http DELETE Request
        /// </summary>
        /// <param name="url">What to delete</param>
        /// <returns>Response body from server</returns>
		public static HttpStatusCode Delete(string url)
        {
            return BaseRestRequest(url, Method.Delete, null);
        }
    }
}