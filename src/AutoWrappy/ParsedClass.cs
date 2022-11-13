namespace AutoWrappy
{
	internal class ParsedClass
	{
		public string Name { get; set; } = "";
		public string? Namespace { get; set; }
		public string SourcePath { get; set; } = "";
		public List<ParsedFunction> Functions { get; } = new List<ParsedFunction>();
		public List<ParsedConstructor> Constructors { get; } = new List<ParsedConstructor>();
		public List<ParsedEvent> Events { get; } = new List<ParsedEvent>();
		public bool Shared { get; set; }
		public bool Delete { get; set; }
		public bool Dispose { get; set; }
		public bool Owner { get; set; }
	}
}