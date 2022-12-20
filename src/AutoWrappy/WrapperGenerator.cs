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
		private bool _usesGlm;

		private const string WrappyPointer = "//WRAPPY_POINTER";
		private const string WrappyShared = "//WRAPPY_SHARED";
		private const string WrappyDelete = "//WRAPPY_DELETE";
		private const string WrappyDispose = "//WRAPPY_DISPOSE";
		private const string WrappyOwner = "//WRAPPY_OWNER";

		private static readonly string[] _buildInTypes = "bool;char;short;int;long;float;double".Split(';');

		private IEnumerable<ParsedClass> ParseFile(string filePath)
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
						var isStruct = match(i, "struct", null, "{") || match(i, "struct", null, ":");
						if (isStruct || match(i, "class", null, "{") || match(i, "class", null, ":"))
						{
							var name = matches[0];
							isPublic = isStruct;
							if (currentClass != null)
								yield return currentClass;
							currentClass = new ParsedClass
							{
								Name = name,
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
							else if (tryParseEvent(i, out var callback, out length))
							{
								if (delete || dispose)
								{
									currentClass.Events.Add(callback);
									Console.WriteLine($"Wrappy:  with event {callback.Name}");
								}
								else
									Console.WriteLine($"Wrappy:  events require WRAPPY_DELETE or WRAPPY_DISPOSE ({callback.Name})");
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

			bool tryParseEvent(int i, [MaybeNullWhen(false)] out ParsedEvent callback, out int elementLength)
			{
				callback = null;
				elementLength = 0;
				var startingI = i;
				if (tryParseType(i, out var type, out var tLength))
				{
					i += tLength;
					if (match(i, "(", "__stdcall", "*", null, ")", "("))
					{
						callback = new ParsedEvent { Name = matches[0], Return = type };
						i += 6;
						while (!match(i, ")"))
						{
							if (!tryParseArgument(i, out var argument, out var aLength))
								return false;
							callback.Arguments.Add(argument);
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
					var last = match(i, null, ")");
					if (last || match(i, null, ","))
					{
						argument = new ParsedArgument
						{
							Name = matches[0],
							Type = type
						};
						elementLength = length + 1 + (last ? 0 : 1);
						return true;
					}
				}

				return false;
			}

			bool tryParseType(int i, [MaybeNullWhen(false)] out ParsedType type, out int elementLength)
			{
				type = null;
				elementLength = 0;
				bool span;
				if (match(i, "std", ":", ":", "span", "<"))
				{
					span = true;
					i += 5;
				}
				else
					span = false;
				if (match(i, "std", ":", ":", "string"))
				{
					if (span && !match(i + 4, ">"))
						return false;
					type = new ParsedType
					{
						Name = "string",
						Span = span
					};
					elementLength = 4 + (span ? 6 : 0);
					return true;
				}
				if (match(i, "std", ":", ":", "shared_ptr", "<", null, ">"))
				{
					var name = matches[0];
					if (span && !match(i + 7, ">"))
						return false;
					type = new ParsedType
					{
						Name = name,
						Shared = true,
						Span = span
					};
					elementLength = 7 + (span ? 6 : 0);
					return true;
				}
				if (match(i, "glm", ":", ":", null))
				{
					var name = matches[0];
					switch (name)
					{
						case "vec2":
						case "vec3":
						case "vec4":
						case "mat4":
							_usesGlm = true;
							type = new ParsedType
							{
								Name = name,
								Span = span,
								Glm = true
							};
							elementLength = 4 + (span ? 6 : 0);
							return true;
					}
				}
				else if (match(i, (string?)null))
				{
					var name = matches[0];
					var pointer = match(i + 1, "*");
					if (span && !match(i + (pointer ? 2 : 1), ">"))
						return false;
					type = new ParsedType
					{
						Name = name,
						Pointer = match(i, null, "*"),
						Span = span
					};
					elementLength = (pointer ? 2 : 1) + (span ? 6 : 0);
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
			c.Constructors.RemoveAll(c => !c.Arguments.All(ValidateArgument));
			c.Functions.RemoveAll(f => !ValidateReturnType(f.Return) || !f.Arguments.All(ValidateArgument));
			c.Events.RemoveAll(e => !ValidateReturnType(e.Return) || !e.Arguments.All(ValidateArgument) || e.Arguments.Any(a => a.Type.Glm));

			c.Events.RemoveAll(e =>
			{
				if (e.Return.Shared || e.Arguments.Any(a => a.Type.Shared))
				{
					Console.WriteLine($"Wrappy: Events with shared_ptr are currently not supported ({c.Name}::{e.Name})");
					return true;
				}
				return false;
			});

			bool ValidateArgument(ParsedArgument argument)
			{
				if (argument.Type.Name == "void")
					return !argument.Type.Shared && !argument.Type.Span && argument.Type.Pointer;
				if (argument.Type.Name == "string")
					return !argument.Type.Shared && !argument.Type.Pointer;
				if (argument.Type.Glm)
					return !argument.Type.Shared && !argument.Type.Pointer;
				if (_buildInTypes.Contains(argument.Type.Name))
					return !argument.Type.Shared && !argument.Type.Pointer;
				if (_parsedClasses.TryGetValue(argument.Type.Name, out var c))
					return argument.Type.Pointer != argument.Type.Shared && argument.Type.Shared == c.Shared;
				return false;
			}

			bool ValidateReturnType(ParsedType type)
			{
				if (type.Span)
					return false;
				if (type.Glm)
					return false;
				if (_buildInTypes.Contains(type.Name) || type.Name == "void")
					return !type.Shared && (!type.Pointer || type.Name == "void" || type.Name == "char");
				if (_parsedClasses.TryGetValue(type.Name, out var c))
					return type.Pointer != type.Shared && type.Shared == c.Shared;
				return false;
			}
		}

		public void GenerateCpp(string path, string? pch)
		{
			string resultingFile;
			using (var file = new StringWriter())
			{
				if (pch != null)
					file.WriteLine(@$"#include""{pch}""");
				foreach (var include in _parsedClasses.Values.Select(c => c.SourcePath).Distinct())
					file.WriteLine(@$"#include""{Path.GetRelativePath(Path.GetDirectoryName(path)!, include)}""");
				file.WriteLine("#include<memory>");
				file.WriteLine("#include<vector>");
				file.WriteLine("#include<string>");
				if (_usesGlm) file.WriteLine("#include<glm/glm.hpp>");


				file.WriteLine(@"extern ""C""{");
				foreach (var c in _parsedClasses.Values)
				{
					var selfType = c.Shared ? $"std::shared_ptr<{NameWithNamespaceCpp(c)}>" : NameWithNamespaceCpp(c);
					var selfAccess = c.Shared ? "(*self)" : "self";
					foreach (var f in c.Constructors)
					{
						file.Write($"__declspec(dllexport) {TypeToString(new ParsedType { Name = c.Name, Shared = c.Shared, Pointer = !c.Shared })} __stdcall ");
						file.Write($"Wrappy_New_{c.Name}({ExternalArguments(f.Arguments).TrimStart(',')}){{");
						WritePreactions(f.Arguments);
						if (c.Shared)
							file.Write(@$"auto inner_result=new {selfType}(new {NameWithNamespaceCpp(c)}({AppliedArguments(f.Arguments)}));");
						else
							file.Write(@$"auto inner_result=new {selfType}({AppliedArguments(f.Arguments)});");
						WritePostactions(f.Arguments);
						file.WriteLine("return inner_result;}");
					}
					foreach (var f in c.Functions)
					{
						var actionFormat = "{0};";
						var returnFormat = "";
						if (_parsedClasses.TryGetValue(f.Return.Name, out var returnC) && returnC.Shared)
						{
							actionFormat = $"auto inner_result=new std::shared_ptr<{NameWithNamespaceCpp(returnC)}>({{0}});";
							returnFormat = "return inner_result;";
						}
						else if (f.Return.Name != "void" || f.Return.Pointer)
						{
							actionFormat = "auto inner_result={0};";
							returnFormat = "return inner_result;";
						}

						file.Write($"__declspec(dllexport) {TypeToString(f.Return)} __stdcall ");
						file.Write($"Wrappy_{c.Name}_{f.Name}({selfType}* self{ExternalArguments(f.Arguments)}){{");
						WritePreactions(f.Arguments);
						file.Write(string.Format(actionFormat, $"{selfAccess}->{f.Name}({AppliedArguments(f.Arguments)})"));
						WritePostactions(f.Arguments);
						file.Write(returnFormat);
						file.WriteLine("}");
					}
					foreach (var e in c.Events)
					{
						file.Write($"__declspec(dllexport) void __stdcall ");
						file.Write($"Wrappy_{c.Name}_SetEvent_{e.Name}({selfType}* self, {TypeToString(e.Return)}(__stdcall* event)({ExternalArguments(e.Arguments).TrimStart(',')})){{");
						file.Write($"{selfAccess}->{e.Name} = event");
						file.WriteLine(";}");
					}
					if (c.Delete || c.Dispose)
					{
						file.Write("__declspec(dllexport) void __stdcall ");
						file.Write($"Wrappy_Delete_{c.Name}({selfType}* self)");
						file.WriteLine(@"{{delete self;}}");
					}
					void WritePreactions(IEnumerable<ParsedArgument> args)
					{
						foreach (var a in args)
							if (a.Type.Span)
							{
								if (a.Type.Shared)
								{
									file.Write($"std::vector<{TypeToString(a.Type, false)}> vec_arg_{a.Name.ToLower()}(l_arg_{a.Name.ToLower()});");
									file.Write($"std::span<{TypeToString(a.Type, false)}> span_arg_{a.Name.ToLower()}(&vec_arg_{a.Name.ToLower()}[0],l_arg_{a.Name.ToLower()});");
									file.Write($"for(int i=0;i<l_arg_{a.Name.ToLower()};++i)");
									file.Write($"if(p_arg_{a.Name.ToLower()}[i])");
									file.Write($"vec_arg_{a.Name.ToLower()}[i]=*(p_arg_{a.Name.ToLower()}[i]);");
								}
								else
									file.Write($"std::span<{TypeToString(a.Type, false)}> span_arg_{a.Name.ToLower()}(p_arg_{a.Name.ToLower()},l_arg_{a.Name.ToLower()});");
							}
					}
					void WritePostactions(IEnumerable<ParsedArgument> args)
					{
						foreach (var a in args)
							if (a.Type.Span)
							{
								if (a.Type.Shared)
								{
									file.Write($"for(int i=0;i<l_arg_{a.Name.ToLower()};++i)");
									file.Write($"if(vec_arg_{a.Name.ToLower()}[i]){{");
									file.Write($"if(!p_arg_{a.Name.ToLower()}[i]||*(p_arg_{a.Name.ToLower()}[i])!=vec_arg_{a.Name.ToLower()}[i])");
									file.Write($"p_arg_{a.Name.ToLower()}[i]=new {TypeToString(a.Type, false)}(vec_arg_{a.Name.ToLower()}[i]);");
									file.Write("}else ");
									file.Write($"p_arg_{a.Name.ToLower()}[i]=nullptr;");
								}
							}
					}
				}
				file.Write("}");
				resultingFile = file.ToString();
			}

			try
			{
				if (File.ReadAllText(path) != resultingFile)
					throw new Exception();
			}
			catch (Exception)
			{
				File.WriteAllText(path, resultingFile);
			}



			string ExternalArguments(List<ParsedArgument> arg)
			{
				return string.Join("", arg.SelectMany(Apply));
				IEnumerable<string> Apply(ParsedArgument a)
				{
					if (a.Type.Span)
					{
						yield return $",{TypeToString(a.Type)}* p_arg_{a.Name.ToLower()}";
						yield return $",int l_arg_{a.Name.ToLower()}";
					}
					else
						yield return $",{TypeToString(a.Type)} arg_{a.Name.ToLower()}";
				}
			}

			string AppliedArguments(List<ParsedArgument> arg)
			{
				return string.Join(",", arg.Select(Apply));
				string Apply(ParsedArgument a)
				{
					if (a.Type.Span)
						return "span_arg_" + a.Name.ToLower();
					if (a.Type.Name == "string")
						return $"std::string(arg_{a.Name.ToLower()})";
					return string.Format(a.Type.Shared ? "{0}?*{0}:nullptr" : a.Type.Glm && !a.Type.Span ? "*{0}" : "{0}", "arg_" + a.Name.ToLower());
				}
			}

			string TypeToString(ParsedType t, bool sharedHasPointer = true)
			{
				if (t.Name == "string") return "char*";
				if (t.Glm) return $"glm::{t.Name}{(t.Span ? "" : "*")}";
				var result = _parsedClasses.TryGetValue(t.Name, out var c) ? NameWithNamespaceCpp(c) : t.Name;
				if (t.Shared)
				{
					result = $"std::shared_ptr<{result}>";
					if (sharedHasPointer)
						result += "*";
				}
				if (t.Pointer) result += '*';
				return result;
			}
		}

		public void GenerateCs(string path, string dll)
		{
			string resultingFile;
			using (var file = new StringWriter())
			{
				foreach (var c in _parsedClasses.Values)
				{
					var lockStatement = c.Dispose ? @$"if(!Native.HasValue)throw new ObjectDisposedException(""{c.Name}"");" : "";
					lockStatement = $"{lockStatement}lock(Locker){{{lockStatement}";

					var undisposedSet = c.Dispose && !c.Delete;

					file.Write($"namespace {NamespaceCs(c)}{{");
					file.Write($"internal class {c.Name}");
					if (c.Dispose)
						file.Write(":IDisposable");
					file.Write("{public IntPtr? Native;");
					file.WriteLine($"public {c.Name}(IntPtr? native){{Native=native;}}");
					if (c.Dispose || c.Events.Count > 0)
						file.WriteLine($"private readonly object Locker=new object();");
					if (c.Owner)
						file.WriteLine("public bool Owner=true;");

					if (undisposedSet)
						file.WriteLine($"public static HashSet<{c.Name}>Undisposed{{get;}}=new HashSet<{c.Name}>();");

					foreach (var f in c.Constructors)
					{
						var isUnsafe = IsUnsafe(f.Arguments);
						var unsafeKeyword = isUnsafe ? "unsafe " : "";
						file.Write(@$"[System.Runtime.InteropServices.DllImport(""{dll}"")]");
						file.Write($"{unsafeKeyword}private static extern IntPtr Wrappy_New_{c.Name}");
						file.Write($"({NativeExternalArguments(f.Arguments, true).TrimStart(',')});");

						file.Write($"{unsafeKeyword}public {c.Name}({ExternalArguments(f.Arguments, false).TrimStart(',')}){{");
						WriteFixed(f.Arguments);
						file.Write("{");
						WritePreactions(f.Arguments);
						file.Write($"Native=Wrappy_New_{c.Name}({AppliedArguments(f.Arguments).TrimStart(',')});");
						WritePostactions(f.Arguments);
						if (undisposedSet)
							file.Write($"lock(Undisposed)Undisposed.Add(this);");
						file.WriteLine("}}");
					}
					foreach (var f in c.Functions)
					{
						var isUnsafe = IsUnsafe(f.Arguments);
						var unsafeKeyword = isUnsafe ? "unsafe " : "";
						file.Write(@$"[System.Runtime.InteropServices.DllImport(""{dll}"")]");
						file.Write($"{unsafeKeyword}private static extern {NativeTypeToString(f.Return)} Wrappy_{c.Name}_{f.Name}");
						file.Write($"(IntPtr self{NativeExternalArguments(f.Arguments, true)});");

						file.Write($"{unsafeKeyword}public {TypeToString(f.Return)} {f.Name}");
						file.Write($"({ExternalArguments(f.Arguments, false).TrimStart(',')}){{");
						if (c.Dispose) file.Write(lockStatement);

						var actionFormat = "{0};";
						var returnFormat = "";
						if (_parsedClasses.TryGetValue(f.Return.Name, out var returnC))
						{
							actionFormat = $"var inner_result=new {NameWithNamespaceCs(returnC)}((IntPtr?){{0}});";
							returnFormat = "return inner_result;";
						}
						else if (f.Return.Name != "void" || f.Return.Pointer)
						{
							actionFormat = "var inner_result={0};";
							returnFormat = "return inner_result;";
						}

						WriteFixed(f.Arguments);
						file.Write("{");
						WritePreactions(f.Arguments);
						file.Write(string.Format(actionFormat, $"Wrappy_{c.Name}_{f.Name}(Native??IntPtr.Zero{AppliedArguments(f.Arguments)})"));
						WritePostactions(f.Arguments);
						file.Write(returnFormat);
						if (c.Dispose) file.Write("}");
						file.WriteLine("}}");
					}
					foreach (var e in c.Events)
					{
						file.Write($"private delegate {NativeTypeToString(e.Return)} {e.Name}Delegate_Native");
						file.Write($"({NativeExternalArguments(e.Arguments, true).TrimStart(',')});");
						file.Write($"private {e.Name}Delegate_Native? {e.Name}Delegate_Native_Object;");

						file.Write(@$"[System.Runtime.InteropServices.DllImport(""{dll}"",CallingConvention=System.Runtime.InteropServices.CallingConvention.StdCall)]");
						file.Write($"private static extern void Wrappy_{c.Name}_SetEvent_{e.Name}");
						file.Write($"(IntPtr self,{e.Name}Delegate_Native? action);");

						file.Write($"public delegate {TypeToString(e.Return)} {e.Name}Delegate");
						file.Write($"({ExternalArguments(e.Arguments, false).TrimStart(',')});");

						var returnFormat = "{0}";
						if (_parsedClasses.TryGetValue(e.Return.Name, out var returnC))
							returnFormat = @$"return {{0}}?.Native??IntPtr.Zero";
						else if (e.Return.Name != "void" || e.Return.Pointer)
							returnFormat = "return {0}??default";

						file.Write($"private {e.Name}Delegate? {e.Name}Delegate_Object;");
						file.Write($"public event {e.Name}Delegate {e.Name}{{add{{");
						file.Write(lockStatement);
						file.Write($"{e.Name}Delegate_Object+=value;");
						file.Write($"if({e.Name}Delegate_Native_Object==null){{{e.Name}Delegate_Native_Object=({string.Join(",", e.Arguments.Select(a => a.Name.ToLower()))})=>{{");
						file.Write(string.Format(returnFormat, $"{e.Name}Delegate_Object?.Invoke({string.Join(",", e.Arguments.Select(DelegateArgument))})"));
						string DelegateArgument(ParsedArgument a)
						{
							if (_parsedClasses.TryGetValue(a.Type.Name, out var returnC))
								return $"{a.Name}==IntPtr.Zero?null:new {NameWithNamespaceCs(returnC)}((IntPtr?){a.Name})";
							return a.Name.ToLower();
						}
						file.Write($";}};Wrappy_{c.Name}_SetEvent_{e.Name}(Native??IntPtr.Zero,{e.Name}Delegate_Native_Object);}}}}}}");
						file.Write($"remove{{{e.Name}Delegate_Object-=value;}}");
						file.WriteLine($"}}");
					}
					if (c.Events.Count > 0)
					{
						file.Write($"private void ClearDelegates(){{");
						file.Write($"if(!Native.HasValue)return;lock(Locker){{if(!Native.HasValue)return;");
						foreach (var e in c.Events)
							file.Write($"if({e.Name}Delegate_Native_Object!=null){{Wrappy_{c.Name}_SetEvent_{e.Name}(Native??IntPtr.Zero,null);{e.Name}Delegate_Native_Object=null;}}");
						file.WriteLine($"}}}}");
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
							if (undisposedSet)
								file.Write($"lock(Undisposed)Undisposed.Remove(this);");
							if (c.Events.Count > 0)
								file.Write($"ClearDelegates();");
							if (c.Owner)
								file.WriteLine($"if(Owner){{Wrappy_Delete_{c.Name}(Native.Value);Native=null;}}}}}}");
							else
								file.WriteLine($"Wrappy_Delete_{c.Name}(Native.Value);Native=null;}}}}");
							if (c.Delete)
							{
								file.Write($"~{c.Name}(){{");
								if (c.Events.Count > 0)
									file.Write($"ClearDelegates();");
								if (c.Owner)
									file.WriteLine($"if(Native.HasValue&&Owner)Wrappy_Delete_{c.Name}(Native.Value);}}");
								else
									file.WriteLine($"if(Native.HasValue)Wrappy_Delete_{c.Name}(Native.Value);}}");
							}
						}
						else if (c.Delete)
						{
							file.Write($"~{c.Name}(){{");
							if (c.Events.Count > 0)
								file.Write($"ClearDelegates();");
							if (c.Owner)
								file.WriteLine($"if(Owner)Wrappy_Delete_{c.Name}(Native??IntPtr.Zero);}}");
							else
								file.WriteLine($"Wrappy_Delete_{c.Name}(Native??IntPtr.Zero);}}");
						}
					}
					void WriteFixed(IEnumerable<ParsedArgument> args)
					{
						foreach(var a in args)
							if(a.Type.Span && !(a.Type.Shared || a.Type.Pointer))
								file.Write($"fixed(void*native_{a.Name.ToLower()}={a.Name.ToLower()})");
					}
					void WritePreactions(IEnumerable<ParsedArgument> args)
					{
						foreach (var a in args)
							if (a.Type.Span && (a.Type.Shared || a.Type.Pointer))
							{
								file.Write($"IntPtr*native_{a.Name.ToLower()}=stackalloc IntPtr[{a.Name.ToLower()}.Length];");
								file.Write($"for(var i=0;i<{a.Name.ToLower()}.Length;i++)");
								file.Write($"native_{a.Name.ToLower()}[i]={a.Name.ToLower()}[i]?.Native??IntPtr.Zero;");
							}
					}
					void WritePostactions(IEnumerable<ParsedArgument> args)
					{
						foreach (var a in args)
							if (a.Type.Span && (a.Type.Shared || a.Type.Pointer))
							{
								file.Write($"for(var i=0;i<{a.Name.ToLower()}.Length;i++)");
								file.Write($"if(native_{a.Name.ToLower()}[i]==IntPtr.Zero)");
								file.Write($"{a.Name.ToLower()}[i]=null;");
								file.Write("else{");
								file.Write($"if(native_{a.Name.ToLower()}[i]!={a.Name.ToLower()}[i]?.Native)");
								file.Write($"{a.Name.ToLower()}[i]=new {a.Type.Name}((IntPtr?)native_{a.Name.ToLower()}[i]);}}");
							}
					}

					file.WriteLine("}}");
				}
				resultingFile = file.ToString();
			}

			try
			{
				if (File.ReadAllText(path) != resultingFile)
					throw new Exception();
			}
			catch (Exception)
			{
				File.WriteAllText(path, resultingFile);
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
				return string.Join("", arg.SelectMany(Apply));
				IEnumerable<string> Apply(ParsedArgument a)
				{
					if (a.Type.Span)
					{
						yield return ",native_" + a.Name.ToLower();
						yield return "," + a.Name.ToLower() + ".Length";
					}
					else
					{
						if (_parsedClasses.ContainsKey(a.Type.Name))
							yield return "," + a.Name.ToLower() + @$"?.Native??IntPtr.Zero";
						else if (a.Type.Glm)
							yield return ",&" + a.Name.ToLower();
						else
							yield return "," + a.Name.ToLower();
					}
				}
			}

			string TypeToString(ParsedType t, bool ignoreSpan = false)
			{
				var result = t.Name;
				if (t.Name == "char") result = "byte";
				if (_parsedClasses.TryGetValue(t.Name, out var c))
					result = NameWithNamespaceCs(c) + "?";
				else if (t.Shared || t.Pointer)
					result = "IntPtr";
				else if (t.Glm)
					result = "System.Numerics." + t.Name switch
					{
						"vec2" => "Vector2",
						"vec3" => "Vector3",
						"vec4" => "Vector4",
						"mat4" => "Matrix4x4",
						_ => throw new NotImplementedException()
					};
				if (t.Span && !ignoreSpan)
					result = $"Span<{result}>";
				return result;
			}

			string NativeExternalArguments(List<ParsedArgument> arg, bool prefix)
			{
				return string.Join("", arg.SelectMany(Apply));
				IEnumerable<string> Apply(ParsedArgument a)
				{
					if (a.Type.Span)
					{
						yield return $",void* {(prefix ? "p_arg_" : "")}{a.Name.ToLower()}";
						yield return $",int {(prefix ? "l_arg_" : "")}{a.Name.ToLower()}";
					}
					else
						yield return $",{NativeTypeToString(a.Type)} {(prefix ? "arg_" : "")}{a.Name.ToLower()}";
				}
			}

			string NativeTypeToString(ParsedType t)
			{
				var result = t.Name;
				if (t.Name == "char") result = "byte";
				if (t.Shared || t.Pointer) result = "IntPtr";
				if (t.Glm) result = t.Span ? TypeToString(t, true) : "void*";
				return result;
			}

			bool IsUnsafe(List<ParsedArgument> arg) =>
				arg.Any(a => a.Type.Glm || a.Type.Span);
		}


		private static string NamespaceCs(ParsedClass c) => c.Namespace ?? "Wrappy";
		private static string NameWithNamespaceCs(ParsedClass c) => $"{NamespaceCs(c)}.{c.Name}";
		private static string NameWithNamespaceCpp(ParsedClass c) => c.Namespace == null ? c.Name : $"{c.Namespace}::{c.Name}";
	}
}