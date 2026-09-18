using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
namespace BD2InfiniteGacha.Runtime
{
    public static class Loader
    {
        private static object engine;private static bool resolver;
        public static void Probe(){lock(typeof(Loader)){
            if(!resolver){AppDomain.CurrentDomain.AssemblyResolve+=Resolve;resolver=true;}
            try{var probe=Activator.CreateInstance(typeof(Loader).Assembly.GetType("BD2InfiniteGacha.Runtime.RuntimeEngine",true),true);probe.GetType().GetMethod("StartProbe",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(probe,null);}
            catch(Exception e){Storage.Write("probe.json",new Snapshot{Runtime=Identity.Runtime,State="error",Message=e.GetBaseException().Message,At=DateTime.UtcNow.Ticks});}
        }}
        public static void Load(){lock(typeof(Loader)){
            if(!resolver){AppDomain.CurrentDomain.AssemblyResolve+=Resolve;resolver=true;}
            try{
                var own=typeof(Loader).Assembly;
                var prefixes=new[]{"BD2Fishing.Runtime","BD2Sichuan.Runtime","BD2Daily.Runtime","BD2InfiniteGacha.Runtime","BD2Rhythm.Runtime","BD2Territory.Runtime","BD2ArenaDefenseWatcher.Active.Runtime","BD2ReplayCaptureHook."};
                if(AppDomain.CurrentDomain.GetAssemblies().Any(a=>a!=own&&prefixes.Any(p=>(a.GetName().Name??"").StartsWith(p,StringComparison.Ordinal))))throw new InvalidOperationException("游戏已连接其他 BD2 组件，请正常重启游戏后再连接。");
                if(engine==null)engine=Activator.CreateInstance(own.GetType("BD2InfiniteGacha.Runtime.RuntimeEngine",true),true);
                engine.GetType().GetMethod("Start",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(engine,null);
            }catch(Exception e){try{Unload();}catch{}Status("error",e.GetBaseException().Message);}
        }}
        public static void Unload(){lock(typeof(Loader)){if(engine!=null)engine.GetType().GetMethod("Stop",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(engine,null);engine=null;Status("inactive","");}}
        internal static void Status(string state,string error){try{using(var p=Process.GetCurrentProcess())Storage.Write("runtime.json",new RuntimeStatus{State=state,Error=error,ProcessId=p.Id,ProcessStart=p.StartTime.ToUniversalTime().Ticks,At=DateTime.UtcNow.Ticks});}catch{}}
        private static Assembly Resolve(object sender,ResolveEventArgs args){if(new AssemblyName(args.Name).Name!="0Harmony")return null;using(var s=typeof(Loader).Assembly.GetManifestResourceStream("BD2InfiniteGacha.Harmony.dll"))using(var b=new MemoryStream()){s.CopyTo(b);return Assembly.Load(b.ToArray());}}
    }
}
