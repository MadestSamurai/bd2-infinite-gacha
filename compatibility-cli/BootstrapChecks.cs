using System.Text;
using BD2InfiniteGacha.Compatibility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Mono.Cecil;
public static class BootstrapChecks
{
    public static void Prepare(string managed,string output)
    {
        Directory.CreateDirectory(output);
        var refs=new[]{"mscorlib.dll","System.dll","System.Core.dll"}.Select(f=>MetadataReference.CreateFromFile(Path.Combine(managed,f))).ToArray();
        byte[] Compile(string name,string source,IEnumerable<MetadataReference> references,OutputKind kind=OutputKind.DynamicallyLinkedLibrary,ResourceDescription[]? resources=null)
        {using var bytes=new MemoryStream();var result=CSharpCompilation.Create(name,[CSharpSyntaxTree.ParseText(source)],references,new CSharpCompilationOptions(kind,optimizationLevel:OptimizationLevel.Release)).Emit(bytes,manifestResources:resources);if(!result.Success)throw new Exception(string.Join("\n",result.Diagnostics));return bytes.ToArray();}
        var stub="""
namespace BD2InfiniteGacha {
 public class RuntimeStatus {public string State;public string Error;public int ProcessId;public long ProcessStart;public long At;}
}
namespace BD2InfiniteGacha.Runtime {
 internal static class Storage {
  internal static void Write(string file,BD2InfiniteGacha.RuntimeStatus status){System.IO.File.WriteAllText(System.IO.Path.Combine(System.Environment.GetEnvironmentVariable("GACHA_PROBE_OUTPUT"),file),status.State+"|"+status.Error);}
 }
 internal sealed class RuntimeEngine {
  private HarmonyLib.Harmony harmony;
  internal void Start(){harmony=new HarmonyLib.Harmony("gacha-bootstrap-probe");Loader.Status("active","");}
  internal void Stop(){}
 }
}
""";
        string loader=Encoding.UTF8.GetString(HookCompiler.Resource("Hook.Loader.cs"));
        byte[] harmony=HookCompiler.Resource("BD2InfiniteGacha.Harmony.dll");
        var payload=Compile("BD2InfiniteGacha.Runtime4",loader+stub,refs.Append(MetadataReference.CreateFromImage(harmony)),resources:[new ResourceDescription("BD2InfiniteGacha.Harmony.dll",()=>new MemoryStream(harmony),true)]);
        using(var asm=AssemblyDefinition.ReadAssembly(new MemoryStream(payload)))
        {
            var type=asm.MainModule.GetType("BD2InfiniteGacha.Runtime.Loader");
            if(type.Fields.Any(f=>f.FieldType.FullName.Contains("RuntimeEngine")||f.FieldType.FullName.Contains("Harmony")))throw new Exception("Loader eagerly binds a runtime dependency");
            foreach(var instruction in type.Methods.Where(m=>m.HasBody).SelectMany(m=>m.Body.Instructions))
                if(instruction.Operand is MemberReference m&&(m.DeclaringType?.FullName.Contains("RuntimeEngine")==true||m.DeclaringType?.FullName.Contains("HarmonyLib")==true))throw new Exception("Loader eagerly invokes runtime dependency");
        }
        File.WriteAllBytes(Path.Combine(output,"bootstrap.dll"),payload);
        var runner="""
using System;
using System.IO;
using System.Reflection;
class Runner {static int Main(string[] args){try{Assembly.Load(File.ReadAllBytes(args[0])).GetType("BD2InfiniteGacha.Runtime.Loader",true).GetMethod("Load").Invoke(null,null);return 0;}catch(Exception e){File.WriteAllText(Path.Combine(Environment.GetEnvironmentVariable("GACHA_PROBE_OUTPUT"),"outer-error.txt"),e.ToString());return 1;}}}
""";
        File.WriteAllBytes(Path.Combine(output,"Probe.exe"),Compile("Probe",runner,refs,OutputKind.ConsoleApplication));
    }
}
