namespace AutoWrappy
{
	internal class ParsedType
	{
		public string Name { get; set; } = "";
		public bool Pointer { get; set; }
		public bool Span { get; set; }
		public bool Shared { get; set; }
	}
}