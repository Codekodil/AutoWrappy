namespace AutoWrappy
{
	internal class ParsedFunction
	{
		public string Name { get; set; } = "";
		public ParsedType Return { get; set; } = new ParsedType();
		public List<ParsedArgument> Arguments { get; } = new List<ParsedArgument>();
	}
}