namespace Neo4jRestNet.GremlinPlugin
{
	public static class JavaStringCommands
	{
		public static JavaBoolean Contains(this JavaString javaString, string value)
		{
			return new JavaBoolean(javaString).Append("contains('{0}')", value);
		}

		public static JavaString CompareToIgnoreCase(this JavaString javaString, string value)
		{
			return javaString.Append("compareToIgnoreCase('{0}')", value);
		}

		public static JavaString CompareToIgnoreCase(this JavaString javaString, JavaString value)
		{
			return javaString.Append("compareToIgnoreCase({0})", value.ToString());
		}

		public static JavaString CompareToIgnoreCase(this JavaObject javaObject, string value)
		{
			return CompareToIgnoreCase(new JavaString(javaObject), value);
		}

		public static JavaString CompareToIgnoreCase(this JavaObject javaObject, JavaString value)
		{
			return CompareToIgnoreCase(new JavaString(javaObject), value);
		}

		public static JavaString ToLowerCase(this IJavaObject javaObject)
		{
			return (new JavaString(javaObject)).Append("toLowerCase()");
		}
		
		public static JavaString ToUpperCase(this IJavaObject javaObject)
		{
			return (new JavaString(javaObject)).Append("toUpperCase()");
		}

		public static JavaString Trim(this IJavaObject javaObject)
		{
			return (new JavaString(javaObject)).Append("trim()");
		}

	}
}
