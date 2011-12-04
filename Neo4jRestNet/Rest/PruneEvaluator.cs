using Newtonsoft.Json.Linq;

namespace Neo4jRestNet.Rest
{
    public class PruneEvaluator
    {
		public static readonly PruneEvaluator None = new PruneEvaluator("none");

		public enum Language
		{
			Javascript = 0
		}

		private readonly string _language;
		private readonly string _name;
		private readonly string _body;

		private PruneEvaluator(string builtin)
		{
			_language = "builtin";
			_name = builtin;
			_body = null;
		}

		public PruneEvaluator(Language language, string script)
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

			return new JProperty("prune_evaluator", jo);
        }
    }
}