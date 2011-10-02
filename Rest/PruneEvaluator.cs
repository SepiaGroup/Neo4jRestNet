using System.Collections.Generic;
using Neo4jRestNet.Core;
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

		private string _Language;
		private string _Name;
		private string _Body;

		private PruneEvaluator(string Builtin)
		{
			_Language = "builtin";
			_Name = Builtin;
			_Body = null;
		}

		public PruneEvaluator(Language Language, string Script)
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

			return new JProperty("prune_evaluator", jo);
        }
    }
}