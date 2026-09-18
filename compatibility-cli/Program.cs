using BD2InfiniteGacha.Compatibility;
using System.Text.Json;
if(args.Length==3&&args[0]=="bootstrap"){BootstrapChecks.Prepare(args[1],args[2]);Console.WriteLine("Bootstrap fixture ready");return;}
if(args.Length==3&&args[0]=="generate"){File.WriteAllText(args[2],JsonSerializer.Serialize(HookCompiler.Generate(args[1]),new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine("Generated contract");return;}
if(args.Length==3&&args[0]=="check"){var p=HookCompiler.Prepare(args[1]);Directory.CreateDirectory(args[2]);File.WriteAllBytes(Path.Combine(args[2],"BD2InfiniteGacha.Runtime4.dll"),p.Payload);File.WriteAllText(Path.Combine(args[2],"compatibility.json"),JsonSerializer.Serialize(p.Report));Console.WriteLine(JsonSerializer.Serialize(p.Report));return;}
Console.Error.WriteLine("Usage: generate|check|bootstrap <client Managed directory> <output>");Environment.ExitCode=2;
