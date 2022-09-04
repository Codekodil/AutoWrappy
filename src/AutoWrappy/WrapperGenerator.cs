using System.Diagnostics.CodeAnalysis;

namespace AutoWrappy
{
	public class WrapperGenerator
	{
		public WrapperGenerator(string searchPath)
		{
			var extensions = new[] { ".h", ".hpp" };
			_parsedClasses = Directory
				.GetFiles(searchPath, "*.*", SearchOption.AllDirectories)
				.Where(file => extensions.Any(file.ToLower().EndsWith))
				.SelectMany(ParseFile)
				.ToDictionary(c => c.Name);

			foreach (var c in _parsedClasses.Values)
				RemoveInvalidFunctions(c);
		}

		private readonly Dictionary<string, ParsedClass> _parsedClasses;

		private const string WrappyPointer = "//WRAPPY_POINTER";
		private const string WrappyShared = "//WRAPPY_SHARED";
		private const string WrappyDelete = "//WRAPPY_DELETE";
		private const string WrappyDispose = "//WRAPPY_DISPOSE";
		private const string WrappyOwner = "//WRAPPY_OWNER";

		private static readonly string[] _buildInTypes = "bool;char;short;int;long;float;double".Split(';');

		private static IEnumerable<ParsedClass> ParseFile(string filePath)
		{
			var all = File.ReadAllText(filePath);

			var pointer = all.Contains(WrappyPointer);
			var shared = all.Contains(WrappyShared);

			if (pointer == shared)
				return new ParsedClass[0];

			var delete = all.Contains(WrappyDelete);
			var dispose = all.Contains(WrappyDispose);
			var owner = all.Contains(WrappyOwner);


			var expanded = "";
			foreach (var c in all)
			{
				switch (c)
				{
					case '\t':
					case '\n':
					case '\r':
						expanded += ' ';
						break;
					case '(':
					case ')':
					case ',':
					case ';':
					case ':':
					case '*':
					case '{':
					case '}':
					case '<':
					case '>':
						expanded += $" {c} ";
						break;
					default:
						expanded += c;
						break;
				}
			}
			var elements = expanded.Split(' ', StringSplitOptions.RemoveEmptyEntries);

			string? currentNamespace = null;
			ParsedClass? currentClass = null;
			var isPublic = false;

			var matches = new List<string>();

			IEnumerable<ParsedClass> Search()
			{
				for (var i = 0; i < elements.Length; i++)
				{
					if (match(i, "namespace", null, "{"))
					{
						currentNamespace = matches[0];
						i += 2;
					}
					else if (match(i, "public", ":"))
						isPublic = true;
					else if (match(i, "private", ":") || match(i, "protected", ":"))
						isPublic = false;
					else
					{
						var isStruct = match(i, "struct", null, "{");
						if (isStruct || match(i, "class", null, "{"))
						{
							isPublic = isStruct;
							if (currentClass != null)
								yield return currentClass;
							currentClass = new ParsedClass
							{
								Name = matches[0],
								Namespace = currentNamespace,
								SourcePath = filePath,
								Shared = shared,
								Delete = delete,
								Dispose = dispose,
								Owner = owner
							};
							Console.WriteLine($"Wrappy: found class {NameWithNamespaceCpp(currentClass)}");
							i += 2;
						}
						else if (currentClass != null && isPublic)
						{
							if (tryParseConstructor(i, out var constructor, out var length))
							{
								currentClass.Constructors.Add(constructor);
								Console.WriteLine($"Wrappy:  with {constructor.Arguments.Count} argument constructor");
								i += length - 1;
							}
							else if (tryParseFunction(i, out var function, out length))
							{
								currentClass.Functions.Add(function);
								Console.WriteLine($"Wrappy:  with function {function.Name}");
								i += length - 1;
							}
						}
					}
				}

				if (currentClass != null)
					yield return currentClass;
			}
			return Search().Select(c =>
			{
				if (!hasConstructor(c.Name) && c.Constructors.Count == 0)
					c.Constructors.Add(new ParsedConstructor());
				return c;
			});

			bool tryParseConstructor(int i, [MaybeNullWhen(false)] out ParsedConstructor constructor, out int elementLength)
			{
				constructor = null;
				elementLength = 0;
				var startingI = i;
				if (match(i, currentClass.Name, "(") && !match(i - 1, "new", currentClass.Name, "("))
				{
					constructor = new ParsedConstructor();
					i += 2;
					while (!match(i, ")"))
					{
						if (!tryParseArgument(i, out var argument, out var aLength))
							break;
						constructor.Arguments.Add(argument);
						i += aLength;
					}
					elementLength = Math.Max(0, i - startingI + 1);
					return true;
				}
				return false;
			}

			bool tryParseFunction(int i, [MaybeNullWhen(false)] out ParsedFunction function, out int elementLength)
			{
				function = null;
				elementLength = 0;
				var startingI = i;
				if (tryParseType(i, out var type, out var tLength))
				{
					i += tLength;
					if (match(i, null, "("))
					{
						function = new ParsedFunction { Name = matches[0], Return = type };
						i += 2;
						while (!match(i, ")"))
						{
							if (!tryParseArgument(i, out var argument, out var aLength))
								return false;
							function.Arguments.Add(argument);
							i += aLength;
						}
						elementLength = Math.Max(0, i - startingI + 1);
						return true;
					}
				}
				return false;
			}

			bool tryParseArgument(int i, [MaybeNullWhen(false)] out ParsedArgument argument, out int elementLength)
			{
				argument = null;
				elementLength = 0;

				if (tryParseType(i, out var type, out var length))
				{
					i += length;
					if (type.Array = match(i, "*"))
						i++;
					var last = match(i, null, ")");
					if (last || match(i, null, ","))
					{
						argument = new ParsedArgument
						{
							Name = matches[0],
							Type = type
						};
						elementLength = length + 1 + (type.Array ? 1 : 0) + (last ? 0 : 1);
						return true;
					}
				}

				return false;
			}

			bool tryParseType(int i, [MaybeNullWhen(false)] out ParsedType type, out int elementLength)
			{
				type = null;
				elementLength = 0;
				if (match(i, "std", ":", ":", "string"))
				{
					type = new ParsedType
					{
						Name = "string"
					};
					elementLength = 4;
					return true;
				}
				if (match(i, "std", ":", ":", "shared_ptr", "<", null, ">"))
				{
					type = new ParsedType
					{
						Name = matches[0],
						Shared = true
					};
					elementLength = 7;
					return true;
				}
				else if (match(i, (string?)null))
				{
					type = new ParsedType
					{
						Name = matches[0],
						Pointer = match(i, null, "*")
					};
					elementLength = type.Pointer ? 2 : 1;
					return true;
				}
				return false;
			}

			bool hasConstructor(string className)
			{
				for (var i = 0; i < elements.Length; i++)
					if (match(i, className, "(") && !match(i - 1, "new", className, "("))
						return true;
				return false;
			}
			bool match(int i, params string?[] required)
			{
				if (i + required.Length > elements.Length) return false;
				for (int j = 0; j < required.Length; j++)
					if (required[j] == null)
					{
						if (!isIdentifier(elements[i + j]))
							return false;
					}
					else
					{
						if (required[j] != elements[i + j])
							return false;
					}
				matches = new List<string>();
				for (int j = 0; j < required.Length; j++)
					if (required[j] == null)
						matches.Add(elements[i + j]);
				return true;
			}
			bool isIdentifier(string name) =>
				name.ToLower().All(c => (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_');
		}

		private void RemoveInvalidFunctions(ParsedClass c)
		{
			c.Functions.RemoveAll(f => !ValidateReturnType(f.Return) || !f.Arguments.All(ValidateArgument));

			bool ValidateArgument(ParsedArgument argument)
			{
				if (argument.Type.Name == "void")
					return !argument.Type.Shared && !argument.Type.Array && argument.Type.Pointer;
				if (argument.Type.Name == "string")
					return !argument.Type.Shared && !argument.Type.Pointer && !argument.Type.Array;
				if (_buildInTypes.Contains(argument.Type.Name))
				{
					if (!argument.Type.Array && argument.Type.Pointer)
					{
						argument.Type.Array = true;
						argument.Type.Pointer = false;
					}
					return !argument.Type.Shared && !argument.Type.Pointer;
				}
				if (_parsedClasses.TryGetValue(argument.Type.Name, out var c))
					return !argument.Type.Array && argument.Type.Pointer != argument.Type.Shared && argument.Type.Shared == c.Shared;
				return false;
			}

			bool ValidateReturnType(ParsedType type)
			{
				if (_buildInTypes.Contains(type.Name) || type.Name == "void")
					return !type.Shared && !type.Array && (!type.Pointer || type.Name == "void" || type.Name == "char");
				if (_parsedClasses.TryGetValue(type.Name, out var c))
					return !type.Array && type.Pointer != type.Shared && type.Shared == c.Shared;
				return false;
			}
		}

		public void GenerateCpp(string path, string? pch)
		{
			using (var file = new StreamWriter(path))
			{
				if (pch != null)
					file.WriteLine(@$"#include""{pch}""");
				foreach (var include in _parsedClasses.Values.Select(c => c.SourcePath).Distinct())
					file.WriteLine(@$"#include""{Path.GetRelativePath(Path.GetDirectoryName(path)!, include)}""");
				file.WriteLine("#include<memory>");
				file.WriteLine("#include<string>");

				file.WriteLine(@"extern ""C""{");
				foreach (var c in _parsedClasses.Values)
				{
					var selfType = c.Shared ? $"std::shared_ptr<{NameWithNamespaceCpp(c)}>" : NameWithNamespaceCpp(c);
					var selfAccess = c.Shared ? "(*self)" : "self";
					foreach (var f in c.Constructors)
					{
						file.Write($"__declspec(dllexport) {TypeToString(new ParsedType { Name = c.Name, Shared = c.Shared, Pointer = !c.Shared })} __stdcall ");
						file.Write($"Wrappy_New_{c.Name}({ExternalArguments(f.Arguments).TrimStart(',')})");
						if (c.Shared)
							file.WriteLine(@$"{{return new {selfType}(new {NameWithNamespaceCpp(c)}({AppliedArguments(f.Arguments)}));}}");
						else
							file.WriteLine(@$"{{return new {selfType}({AppliedArguments(f.Arguments)});}}");
					}
					foreach (var f in c.Functions)
					{
						var returnFormat = "{0}";
						if (_parsedClasses.TryGetValue(f.Return.Name, out var returnC) && returnC.Shared)
							returnFormat = $"return new std::shared_ptr<{NameWithNamespaceCpp(returnC)}>({{0}})";
						else if (f.Return.Name != "void" || f.Return.Pointer)
							returnFormat = "return {0}";

						file.Write($"__declspec(dllexport) {TypeToString(f.Return)} __stdcall ");
						file.Write($"Wrappy_{c.Name}_{f.Name}({selfType}* self{ExternalArguments(f.Arguments)}){{");
						file.Write(String.Format(returnFormat, $"{selfAccess}->{f.Name}({AppliedArguments(f.Arguments)})"));
						file.WriteLine(";}");
					}
					if (c.Delete || c.Dispose)
					{
						file.Write("__declspec(dllexport) void __stdcall ");
						file.Write($"Wrappy_Delete_{c.Name}({selfType}* self)");
						file.WriteLine(@"{{delete self;}}");
					}
				}
				file.Write("}");
			}


			string ExternalArguments(List<ParsedArgument> arg)
			{
				return string.Join("", arg.Select(Apply));
				string Apply(ParsedArgument a)
				{
					return $",{TypeToString(a.Type)} arg_{a.Name.ToLower()}";
				}
			}

			string AppliedArguments(List<ParsedArgument> arg)
			{
				return string.Join(",", arg.Select(Apply));
				string Apply(ParsedArgument a)
				{
					if (a.Type.Name == "string")
						return $"std::string(arg_{a.Name.ToLower()})";
					return (a.Type.Shared ? "*" : "") + "arg_" + a.Name.ToLower();
				}
			}

			string TypeToString(ParsedType t)
			{
				if (t.Name == "string") return "char*";
				var result = _parsedClasses.TryGetValue(t.Name, out var c) ? NameWithNamespaceCpp(c) : t.Name;
				if (t.Shared) result = $"std::shared_ptr<{result}>*";
				if (t.Pointer) result += '*';
				if (t.Array) result += '*';
				return result;
			}
		}

		public void GenerateCs(string path, string dll)
		{
			using (var file = new StreamWriter(path))
			{
				foreach (var c in _parsedClasses.Values)
				{
					var lockStatement = @$"if(!Native.HasValue)throw new ObjectDisposedException(""{c.Name}"");";
					lockStatement = $"{lockStatement}lock(Locker){{{lockStatement}";

					file.Write($"namespace {NamespaceCs(c)}{{");
					file.Write($"internal class {c.Name}");
					if (c.Dispose)
						file.Write(":IDisposable");
					file.Write("{public IntPtr? Native;");
					file.WriteLine($"public {c.Name}(IntPtr? native){{Native=native;}}");
					if (c.Dispose)
						file.WriteLine($"private readonly object Locker=new object();");
					if (c.Owner)
						file.WriteLine("public bool Owner=true;");

					foreach (var f in c.Constructors)
					{
						file.Write(@$"[System.Runtime.InteropServices.DllImport(""{dll}"")]");
						file.Write($"private static extern IntPtr Wrappy_New_{c.Name}");
						file.Write($"({NativeExternalArguments(f.Arguments, true).TrimStart(',')});");

						file.Write($"public {c.Name}({ExternalArguments(f.Arguments, false).TrimStart(',')})");
						file.WriteLine($"{{Native=Wrappy_New_{c.Name}({AppliedArguments(f.Arguments).TrimStart(',')});}}");
					}
					foreach (var f in c.Functions)
					{
						file.Write(@$"[System.Runtime.InteropServices.DllImport(""{dll}"")]");
						file.Write($"private static extern {NativeTypeToString(f.Return)} Wrappy_{c.Name}_{f.Name}");
						file.Write($"(IntPtr self{NativeExternalArguments(f.Arguments, true)});");

						file.Write($"public {TypeToString(f.Return)} {f.Name}");
						file.Write($"({ExternalArguments(f.Arguments, false).TrimStart(',')}){{");
						if (c.Dispose) file.Write(lockStatement);
						var returnFormat = "{0}";
						if (_parsedClasses.TryGetValue(f.Return.Name, out var returnC))
							returnFormat = $"return new {NameWithNamespaceCs(returnC)}((IntPtr?){{0}})";
						else if (f.Return.Name != "void" || f.Return.Pointer)
							returnFormat = "return {0}";
						file.Write(string.Format(returnFormat, $"Wrappy_{c.Name}_{f.Name}(Native??IntPtr.Zero{AppliedArguments(f.Arguments)})") + ";");
						if (c.Dispose) file.Write("}");
						file.WriteLine("}");
					}
					if (c.Delete || c.Dispose)
					{
						file.Write(@$"[System.Runtime.InteropServices.DllImport(""{dll}"")]");
						file.Write($"private static extern void Wrappy_Delete_{c.Name}");
						file.WriteLine($"(IntPtr self);");
						if (c.Dispose)
						{
							file.Write("public void Dispose(){");
							file.Write(lockStatement);
							if (c.Owner)
								file.WriteLine($"if(Owner){{Wrappy_Delete_{c.Name}(Native.Value);Native=null;}}}}}}");
							else
								file.WriteLine($"Wrappy_Delete_{c.Name}(Native.Value);Native=null;}}}}");
							if (c.Delete)
							{
								file.Write($"~{c.Name}(){{");
								if (c.Owner)
									file.WriteLine($"if(Native.HasValue&&Owner)Wrappy_Delete_{c.Name}(Native.Value);}}");
								else
									file.WriteLine($"if(Native.HasValue)Wrappy_Delete_{c.Name}(Native.Value);}}");
							}
						}
						else if (c.Delete)
						{
							file.Write($"~{c.Name}(){{");
							if (c.Owner)
								file.WriteLine($"if(Owner)Wrappy_Delete_{c.Name}(Native??IntPtr.Zero);}}");
							else
								file.WriteLine($"Wrappy_Delete_{c.Name}(Native??IntPtr.Zero);}}");
						}
					}

					file.Write("}}");
				}
			}

			string ExternalArguments(List<ParsedArgument> arg, bool prefix)
			{
				return string.Join("", arg.Select(Apply));
				string Apply(ParsedArgument a)
				{
					return $",{TypeToString(a.Type)} {(prefix ? "arg_" : "")}{a.Name.ToLower()}";
				}
			}

			string AppliedArguments(List<ParsedArgument> arg)
			{
				return string.Join("", arg.Select(Apply));
				string Apply(ParsedArgument a)
				{
					if (_parsedClasses.ContainsKey(a.Type.Name))
						return "," + a.Name.ToLower() + @$".Native??throw new ObjectDisposedException(""{a.Type.Name}"")";
					return "," + a.Name.ToLower();
				}
			}

			string TypeToString(ParsedType t)
			{
				var result = t.Name;
				if (t.Name == "char") result = "byte";
				if (_parsedClasses.TryGetValue(t.Name, out var c))
					result = NameWithNamespaceCs(c);
				else if (t.Shared || t.Pointer)
					result = "IntPtr";
				if (t.Array) result += "[]";
				return result;
			}

			string NativeExternalArguments(List<ParsedArgument> arg, bool prefix)
			{
				return string.Join("", arg.Select(Apply));
				string Apply(ParsedArgument a)
				{
					return $",{NativeTypeToString(a.Type)} {(prefix ? "arg_" : "")}{a.Name.ToLower()}";
				}
			}

			string NativeTypeToString(ParsedType t)
			{
				var result = t.Name;
				if (t.Name == "char") result = "byte";
				if (t.Shared || t.Pointer) result = "IntPtr";
				if (t.Array) result += "[]";
				return result;
			}
		}


		private static string NamespaceCs(ParsedClass c) => c.Namespace ?? "Wrappy";
		private static string NameWithNamespaceCs(ParsedClass c) => $"{NamespaceCs(c)}.{c.Name}";
		private static string NameWithNamespaceCpp(ParsedClass c) => c.Namespace == null ? c.Name : $"{c.Namespace}::{c.Name}";
	}
}