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

		private readonly string _language;
		private readonly string _name;
		private readonly string _body;

		private ReturnFilter(string builtin)
		{
			_language = "builtin";
			_name = builtin;
			_body = null;
		}

		public ReturnFilter(Language language, string script)
		{
			_language = GetLanguageName(language);
			_name = null;
			_body = script;
		}

		private string GetLanguageName(Language language)
		{
			switch (language)
			{
				case Language.Javascript: return "javascript";
				default: return "javascript";
			}
		}

        public JProperty ToJson()
        {
			var jo = new JObject {{"language", _language}};

        	if (_name != null)
				jo.Add("name", _name);
			else
				jo.Add("body", _body);

			return new JProperty("return_filter", jo);
        }
    }
}