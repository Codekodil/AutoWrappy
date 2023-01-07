using AutoWrappy;

var directory = ".";
string? cpp = null;
string? cs = null;
string? dll = null;
string? pch = null;

var customGlm = new Dictionary<string, string>();

for (var i = 0; i < args.Length - 1; i += 2)
{
	switch (args[i])
	{
		case "-d": directory = args[i + 1]; break;
		case "-cpp": cpp = args[i + 1]; break;
		case "-cs": cs = args[i + 1]; break;
		case "-dll": dll = args[i + 1]; break;
		case "-pch": pch = args[i + 1]; break;
		case "-glmVec2": customGlm["vec2"] = args[i + 1]; break;
		case "-glmVec3": customGlm["vec3"] = args[i + 1]; break;
		case "-glmVec4": customGlm["vec4"] = args[i + 1]; break;
		case "-glmMat2": customGlm["mat2"] = args[i + 1]; break;
		case "-glmMat3": customGlm["mat3"] = args[i + 1]; break;
		case "-glmMat4": customGlm["mat4"] = args[i + 1]; break;
		case "-glmQuat": customGlm["quat"] = args[i + 1]; break;
		default:
			Console.WriteLine(@$"Wrappy: invalid command ""{args[i]}""");
			return;
	}
}

if (cpp == null)
	Console.WriteLine("Wrappy: missing -cpp command");
if (cs == null)
	Console.WriteLine("Wrappy: missing -cs command");
if (dll == null)
	Console.WriteLine("Wrappy: missing -dll command");
if (cpp == null || cs == null || dll == null)
	return;

directory = Path.GetFullPath(directory);
cpp = Path.GetFullPath(cpp);
cs = Path.GetFullPath(cs);

var generator = new WrapperGenerator(directory, customGlm);
Console.WriteLine("Wrappy: generating cpp");
generator.GenerateCpp(cpp, pch);
Console.WriteLine("Wrappy: generating cs");
generator.GenerateCs(cs, dll);
Console.WriteLine("Wrappy: done");