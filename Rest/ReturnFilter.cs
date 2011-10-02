using System.Collections.Generic;
using Neo4jRestNet.Core;
using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Rest
{
    public class ReturnFilter
    {
		public static readonly ReturnFilter All = new ReturnFilter("all");
		public static readonly ReturnFilter AllButStartNode = new ReturnFilter("all_but_start_node");

		public enum Language
		{
			Javascript = 0
		}

		private string _Language;
		private string _Name;
		private string _Body;

		private ReturnFilter(string builtin)
		{
			_Language = "builtin";
			_Name = builtin;
			_Body = null;
		}

		public ReturnFilter(Language Language, string Script)
		{
			_Language = GetLanguageName(Language);
			_Name = null;
			_Body = Script;
		}

		private string GetLanguageName(Language Language)
		{
			switch (Language)
			{
				case Language.Javascript: return "javascript";
				default: return "javascript";
			}
		}

        public JProperty ToJson()
        {
			JObject jo = new JObject();
			jo.Add("language", _Language);

			if (_Name != null)
				jo.Add("name", _Name);
			else
				jo.Add("body", _Body);

			return new JProperty("return_filter", jo);
        }
    }
}