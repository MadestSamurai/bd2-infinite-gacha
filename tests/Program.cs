using System.Runtime.Serialization.Json;
using System.Text;
using BD2InfiniteGacha;

if(args.Length==2&&args[0]=="--messages"){File.WriteAllText(args[1],System.Text.Json.JsonSerializer.Serialize(LocalizationTests.Sources(args[0]),new System.Text.Json.JsonSerializerOptions{WriteIndented=true}));return;}
var root=Path.GetFullPath(args.Length==0?Path.Combine(AppContext.BaseDirectory,"test-data","core"):args[0]);Directory.CreateDirectory(root);
var checks=new List<string>();int assertions=0;
void Check(bool value,string message){assertions++;if(!value)throw new Exception(message);}
void Test(string name,Action test){test();checks.Add(name);Console.WriteLine("PASS "+name);}
void Throws(Action action){try{action();}catch{assertions++;return;}throw new Exception("Expected failure");}
Costume[] Stock(params int[] ids)=>ids.Select(id=>new Costume{Id=id,Level=0}).ToArray();
int[] Cards(int a,int b)=>Enumerable.Repeat(1,a).Concat(Enumerable.Repeat(2,b)).Concat(Enumerable.Repeat(3,10-a-b)).ToArray();
RuleDraft Draft()=>new(){ExcludeOverflow=false,Threshold="3",A=[1],B=[2],Interval="1000",Rows=Enumerable.Range(0,10).Select(a=>new RuleRow{A=a}).ToArray()};
Pool Pool()=>new(){Id=10,DrawId=11,Size=10,Key="pool-v1",Name="test",Costumes=Enumerable.Range(1,60).Select(id=>new Costume{Id=id}).ToArray()};
Snapshot Snap(long? now=null)=>new(){Account=new string('a',64),ProcessId=17,ProcessStart=123,PoolId=10,Pools=[Pool()],Result=Cards(0,0),ResultReady=true,At=now??DateTime.UtcNow.Ticks};
Control Command(long now)=>new(){Enabled=true,Owner="run1",Account=new string('a',64),ProcessId=17,ProcessStart=123,PoolId=10,PoolKey="pool-v1",Rules=Draft(),Expires=now+TimeSpan.FromSeconds(10).Ticks};
Test("default requires explicit threshold",()=>{var d=new RuleDraft();Check(Rules.Validate(d,Stock(1,2))!="","blank must block");Check(d.CountCopies&&d.Rows.All(r=>!r.Enabled)&&d.Interval=="1000","defaults");});
Test("copy and distinct semantics",()=>{var d=Draft();Check(Rules.Evaluate(d,Cards(3,2),Pool().Costumes).Matched,"copies");d.CountCopies=false;var result=Rules.Evaluate(d,Cards(3,2),Pool().Costumes);Check(!result.Matched&&result.A==1&&result.B==1,"distinct");d.Rows[1].Enabled=true;d.Rows[1].B="1";Check(Rules.Evaluate(d,Cards(3,2),Pool().Costumes).Matched,"distinct secondary");});
Test("A priority uses inclusive top and exact lower rows",()=>{var d=Draft();d.Rows[1].Enabled=true;d.Rows[1].B="1";Check(Rules.Evaluate(d,Cards(3,0),Pool().Costumes).Matched,"inclusive");Check(Rules.Evaluate(d,Cards(4,0),Pool().Costumes).Matched,"above");Check(!Rules.Evaluate(d,Cards(2,8),Pool().Costumes).Matched,"not lower threshold");Check(Rules.Evaluate(d,Cards(1,1),Pool().Costumes).Matched,"exact");Check(!Rules.Evaluate(d,Cards(0,10),Pool().Costumes).Matched,"disabled zero");});
Test("all feasible count combinations match independent rule oracle",()=>{
 for(int top=1;top<=10;top++)foreach(bool copies in new[]{true,false})for(int a=0;a<=10;a++)for(int b=0;b<=10-a;b++){
  var d=Draft();d.Threshold=top.ToString();d.CountCopies=copies;d.A=Enumerable.Range(1,10).ToArray();d.B=Enumerable.Range(11,10).ToArray();
  foreach(var row in d.Rows){row.Enabled=row.A%2==0;row.B=Math.Min(10-row.A,3).ToString();}
  var cards=(copies?Enumerable.Repeat(1,a):Enumerable.Range(1,a)).Concat(copies?Enumerable.Repeat(11,b):Enumerable.Range(11,b)).Concat(Enumerable.Repeat(31,10-a-b)).ToArray();
  var got=Rules.Evaluate(d,cards,Pool().Costumes);bool expect=a>=top||(a%2==0&&b>=Math.Min(10-a,3));Check(got.A==a&&got.B==b&&got.Matched==expect,$"oracle {copies} {top}/{a}/{b}");
 }
});
Test("invalid values retained and hidden/disabled rows ignored",()=>{var d=Draft();d.Rows[2].Enabled=true;d.Rows[2].B="9";Check(Rules.Validate(d,Stock(1,2,3))!=""&&d.Rows[2].B=="9","sum preserved");d.Rows[2].Enabled=false;Check(Rules.Validate(d,Stock(1,2,3))=="","disabled");d.Rows[9].Enabled=true;d.Rows[9].B="abc";Check(Rules.Validate(d,Stock(1,2,3))=="","hidden");d.Threshold="11";Check(Rules.Validate(d,Stock(1,2,3))!=""&&d.Threshold=="11","top preserved");d.Threshold="3";d.Interval="-2";Check(Rules.Validate(d,Stock(1,2,3))!=""&&d.Interval=="-2","interval preserved");Check(Rules.RowError(2,"999999999999")!="","integer overflow");});
Test("group validation and reachable distinct alternatives",()=>{var d=Draft();d.B=[1];Check(Rules.Validate(d,Stock(1,2))!="","overlap");d.B=[2];Check(Rules.Validate(d,Stock(2))!="","removed pool");d.CountCopies=false;Check(Rules.Validate(d,Stock(1,2))!="","impossible distinct top");d.Rows[1].Enabled=true;d.Rows[1].B="1";Check(Rules.Validate(d,Stock(1,2))=="","reachable alternative");d.A=[];d.Rows[1].Enabled=false;d.Rows[0].Enabled=true;d.Rows[0].B="1";Check(Rules.Validate(d,Stock(1,2))=="","B only explicit");});
Test("incomplete results rejected",()=>{Throws(()=>Rules.Evaluate(Draft(),[1,2],Pool().Costumes));var s=Snap();s.Result=[1,2];var m=new AutomationMachine();Check(!m.Step(Command(s.At),s,0,0,0,s.At)&&m.State=="error","short");s=Snap();s.Result=Enumerable.Repeat(999,10).ToArray();m=new();Check(!m.Step(Command(s.At),s,0,0,0,s.At)&&m.State=="error","outside pool");});
Test("already qualifying result never rerolled",()=>{var s=Snap();s.Result=Cards(3,0);var c=Command(s.At);var m=new AutomationMachine();Check(!m.Step(c,s,0,0,0,s.At)&&m.State=="matched"&&m.Rolls==0,"initial");s.Result=Cards(0,0);Check(!m.Step(c,s,1,1,0,s.At)&&m.State=="matched","terminal");c.Owner="new";Check(m.Step(c,s,1,1,0,s.At),"explicit restart");});
Test("only one request and requires response plus result",()=>{var s=Snap();var c=Command(s.At);var m=new AutomationMachine();Check(m.Step(c,s,7,9,0,s.At),"first");for(int i=0;i<30;i++)Check(!m.Step(c,s,7,9,0,s.At+i*1000000),"pending duplicate");Check(!m.Step(c,s,8,9,0,s.At+10000000),"response alone");Check(m.Step(c,s,8,10,0,s.At+10000000)&&m.Rolls==1,"identical result with epochs advances");});
Test("result alone cannot acknowledge request",()=>{var s=Snap();var c=Command(s.At);var m=new AutomationMachine();m.Step(c,s,0,0,0,s.At);Check(!m.Step(c,s,0,1,0,s.At+10000000)&&m.Rolls==0,"requires response");});
Test("qualifying next result stops before any next request",()=>{var s=Snap();var c=Command(s.At);var m=new AutomationMachine();Check(m.Step(c,s,0,0,0,s.At),"send");s.Result=Cards(3,0);Check(!m.Step(c,s,1,1,0,s.At+20000000)&&m.State=="matched"&&m.Rolls==1,"match stop");});
Test("animation gating and configured interval",()=>{var s=Snap();var c=Command(s.At);var m=new AutomationMachine();s.ResultReady=false;Check(!m.Step(c,s,0,0,0,s.At),"animation before");s.ResultReady=true;Check(m.Step(c,s,0,0,0,s.At),"ready");s.ResultReady=false;Check(!m.Step(c,s,1,1,0,s.At+2000000),"next animation");s.ResultReady=true;Check(!m.Step(c,s,1,1,0,s.At+3000000)&&m.Rolls==1,"interval");Check(m.Step(c,s,1,1,0,s.At+10000000),"interval elapsed");});
Test("skip requires active pending response and native visible button",()=>{
 var s=Snap();var c=Command(s.At);var m=new AutomationMachine();s.ResultReady=false;s.CanSkip=true;
 Check(!m.Step(c,s,0,0,0,s.At)&&!m.SkipAnimation,"no skip of manual initial draw");
 s.ResultReady=true;Check(m.Step(c,s,0,0,0,s.At)&&!m.SkipAnimation,"request first");s.ResultReady=false;
 Check(!m.Step(c,s,0,0,0,s.At+1000000)&&!m.SkipAnimation,"no skip before response");
 s.CanSkip=false;Check(!m.Step(c,s,1,0,0,s.At+2000000)&&!m.SkipAnimation,"hidden button or popup waits");s.CanSkip=true;
 Check(!m.Step(c,s,1,0,0,s.At+3000000)&&m.SkipAnimation&&m.Rolls==0,"native skip before final SetResult");
 Check(!m.Step(c,s,1,0,0,s.At+5000000)&&m.SkipAnimation,"multi-stage reveal can skip again without drawing");
 s.ResultReady=true;Check(!m.Step(c,s,1,1,0,s.At+6000000)&&!m.SkipAnimation&&m.Rolls==1,"result acknowledged once respects interval");
 Check(m.Step(c,s,1,1,0,s.At+10000000)&&!m.SkipAnimation&&m.Rolls==1,"one next request after interval");
 s.Result=Cards(3,0);Check(!m.Step(c,s,2,2,0,s.At+12000000)&&m.State=="matched"&&!m.SkipAnimation,"match stops before next request");
 s.ResultReady=false;Check(!m.Step(c,s,2,2,0,s.At+13000000)&&!m.SkipAnimation,"matched never skips");
});
Test("skip is revoked on every stop or failure boundary",()=>{
 for(int mode=0;mode<10;mode++){
  var s=Snap();var c=Command(s.At);var m=new AutomationMachine();m.Step(c,s,0,0,0,s.At);s.ResultReady=false;s.CanSkip=true;
  m.Step(c,s,1,0,0,s.At+1000000);Check(m.SkipAnimation,"active skip setup");long now=s.At+2000000;int error=0;
  switch(mode){case 0:c.Enabled=false;break;case 1:c.Expires=now;break;case 2:s.Locked=true;break;case 3:s.Account="changed";break;case 4:s.PoolId++;break;case 5:s.ProcessId++;break;case 6:c.Rules.Threshold="11";break;case 7:error=123;break;case 8:now=s.At+TimeSpan.FromSeconds(121).Ticks;c.Expires=now+TimeSpan.FromSeconds(10).Ticks;break;case 9:s.Pools[0].Key="changed";break;}
  Check(!m.Step(c,s,1,0,error,now)&&!m.SkipAnimation&&(m.State=="error"||m.State=="stopped"),"skip revoked "+mode);
 }
});
Test("response error stops without retries",()=>{var s=Snap();var c=Command(s.At);var m=new AutomationMachine();m.Step(c,s,0,0,0,s.At);Check(!m.Step(c,s,1,0,123,s.At+10000000)&&m.State=="error"&&m.Message.Contains("123"),"error");Check(!m.Step(c,s,2,2,0,s.At+20000000),"no retry");});
Test("request timeout no retry while lease renews",()=>{var s=Snap();var c=Command(s.At);var m=new AutomationMachine();m.Step(c,s,0,0,0,s.At);long now=s.At+TimeSpan.FromSeconds(31).Ticks;c.Expires=now+TimeSpan.FromSeconds(10).Ticks;Check(!m.Step(c,s,0,0,0,now)&&m.State=="error","timeout");Check(!m.Step(c,s,1,1,0,now+1000),"late response cannot resume");});
Test("lease expiry and explicit stop retain current result",()=>{foreach(bool expired in new[]{true,false}){var s=Snap();var c=Command(s.At);var m=new AutomationMachine();m.Step(c,s,0,0,0,s.At);if(expired)c.Expires=s.At;else c.Enabled=false;Check(!m.Step(c,s,1,1,0,s.At)&&m.State=="stopped","stop");c.Expires=s.At+10000000;c.Enabled=true;Check(!m.Step(c,s,1,1,0,s.At),"same owner no restart");}});
Test("process account pool lock and invalid rules fail closed",()=>{
 Action<Snapshot,Control>[] mutations=[(s,c)=>s.ProcessId++,(s,c)=>s.ProcessStart++,(s,c)=>s.Account="changed",(s,c)=>s.PoolId++,(s,c)=>s.Pools[0].Key="new",(s,c)=>s.Locked=true,(s,c)=>c.Rules.Threshold="11"];
 foreach(var mutate in mutations){var s=Snap();var c=Command(s.At);var m=new AutomationMachine();m.Step(c,s,0,0,0,s.At);mutate(s,c);Check(!m.Step(c,s,1,1,0,s.At+10000000)&&m.State=="error","boundary changed");}
});
Test("host snapshots rules and does not resume after restart",()=>{var port=new FakePort(Snap());var client=new ClientController(port);var d=Draft();client.Start(port.S,port.S.Pools[0],d);d.Threshold="1";d.A[0]=2;Check(port.Last.Rules.Threshold=="3"&&port.Last.Rules.A[0]==1,"immutable");Check(!new ClientController(port).Running,"new UI no autoresume");port.S.Owner=port.Last.Owner;port.S.State="matched";client.Poll(port.S);Check(!client.Running&&!port.Last.Enabled,"matched drops lease");});
Test("host rejects stale and wrong process snapshots",()=>{var s=Snap();var port=new FakePort(s);var client=new ClientController(port);s.At=DateTime.UtcNow.AddSeconds(-6).Ticks;Throws(()=>client.Start(s,s.Pools[0],Draft()));s.At=DateTime.UtcNow.AddSeconds(3).Ticks;Throws(()=>client.Start(s,s.Pools[0],Draft()));s.At=DateTime.UtcNow.Ticks;s.ProcessStart++;Throws(()=>client.Start(s,s.Pools[0],Draft()));Check(!client.Running,"not started");});
Test("host tolerates transient snapshots then stops on prolonged loss",()=>{
 foreach(bool missing in new[]{true,false}){var s=Snap();long now=s.At;var port=new FakePort(s);var client=new ClientController(port,()=>now);client.Start(s,s.Pools[0],Draft());string owner=port.Last.Owner;s.Account="";client.Poll(missing?null:s);Check(client.Running,"transient read holds task");now+=TimeSpan.FromSeconds(2).Ticks;s.Account=new string('a',64);s.At=now;client.Poll(s);Check(client.Running&&port.Last.Owner==owner,"same owner recovers");client.Poll(null);now+=TimeSpan.FromSeconds(16).Ticks;client.Poll(null);Check(!client.Running&&!port.Last.Enabled&&client.Notice.Contains("15"),"persistent loss stops");}
});
Test("host transient write failure keeps owner and retries",()=>{
 var s=Snap();long now=s.At;var port=new FakePort(s);var client=new ClientController(port,()=>now);client.Start(s,s.Pools[0],Draft());string owner=port.Last.Owner;
 now+=TimeSpan.FromSeconds(3).Ticks;s.At=now;port.FailWrites=true;client.Poll(s);Check(client.Running&&client.Notice.Length>0,"write retry waiting");port.FailWrites=false;now+=TimeSpan.FromSeconds(1).Ticks;s.At=now;client.Poll(s);Check(client.Running&&port.Last.Owner==owner&&client.Notice=="","write recovered without restart");
 s.Account=new string('b',64);client.Poll(s);Check(!client.Running&&!port.Last.Enabled,"known changed identity stops immediately");
});
Test("native temporary interface loss resumes same pending draw",()=>{
 var s=Snap();var c=Command(s.At);var m=new AutomationMachine();Check(m.Step(c,s,0,0,0,s.At),"send once");s.PoolId=0;s.ResultReady=false;
 Check(!m.Step(c,s,1,0,0,s.At+1000000)&&m.State=="pending","missing UI waits");s.PoolId=10;s.CanSkip=true;
 Check(!m.Step(c,s,1,0,0,s.At+2000000)&&m.SkipAnimation,"UI resumes pending animation");s.ResultReady=true;s.Result=Cards(3,0);
 Check(!m.Step(c,s,1,1,0,s.At+3000000)&&m.State=="matched"&&m.Rolls==1,"result evaluated once");
});
Test("native popup and long reveal do not prematurely interrupt",()=>{
 var s=Snap();var c=Command(s.At);var m=new AutomationMachine();m.Step(c,s,0,0,0,s.At);s.ResultReady=false;s.CanSkip=true;s.PopupOpen=true;
 for(int sec=1;sec<=90;sec++){long now=s.At+TimeSpan.FromSeconds(sec).Ticks;c.Expires=now+TimeSpan.FromSeconds(10).Ticks;Check(!m.Step(c,s,1,0,0,now)&&m.State=="pending"&&!m.SkipAnimation,"popup waits without drawing");}
 s.PopupOpen=false;long resumed=s.At+TimeSpan.FromSeconds(91).Ticks;c.Expires=resumed+TimeSpan.FromSeconds(10).Ticks;
 Check(!m.Step(c,s,1,0,0,resumed)&&m.SkipAnimation,"skip resumes after popup");s.ResultReady=true;s.Result=Cards(3,0);
 Check(!m.Step(c,s,1,1,0,resumed+1000000)&&m.State=="matched","retains matching draw");
});
Test("settings preserve malformed user text and account/pool scopes",()=>{var store=new Preferences(root);var a=new string('a',64);var b=new string('b',64);var d=Draft();d.Threshold="11";d.Rows[0].B="typing...";store.Save(a,10,d);store.Save(b,10,new RuleDraft{Threshold="7"});store.Save(a,11,new RuleDraft{Threshold="2"});var got=store.Load(a,10);Check(got.Threshold=="11"&&got.Rows[0].B=="typing...","preserved");Check(store.Load(b,10).Threshold=="7"&&store.Load(a,11).Threshold=="2","scoped");var path=Path.Combine(root,"settings.json");File.WriteAllText(path,"not-json");Throws(()=>store.Save(a,10,d));Check(File.ReadAllText(path)=="not-json","corrupt not overwritten");File.Move(path,Path.Combine(root,"corrupt-settings-fixture.txt"),true);});
Test("Mono DataContract and host JSON agree both directions",()=>{var c=Command(DateTime.UtcNow.Ticks);c.Rules.Rows[2].B="3";var ser=new DataContractJsonSerializer(typeof(Control));using var bytes=new MemoryStream();ser.WriteObject(bytes,c);File.WriteAllBytes(Path.Combine(root,"mono-wire.json"),bytes.ToArray());var host=JsonFiles.Read<Control>(Path.Combine(root,"mono-wire.json"))!;Check(host.Rules.Rows[2].B=="3"&&host.Account==c.Account,"Mono to host");JsonFiles.Write(Path.Combine(root,"host-wire.json"),c);using var from=File.OpenRead(Path.Combine(root,"host-wire.json"));var mono=(Control)ser.ReadObject(from)!;Check(mono.Rules.CountCopies&&mono.Rules.A.SequenceEqual(c.Rules.A),"host to Mono");var snapshot=Snap();var sser=new DataContractJsonSerializer(typeof(Snapshot));using var sb=new MemoryStream();sser.WriteObject(sb,snapshot);File.WriteAllBytes(Path.Combine(root,"snapshot-wire.json"),sb.ToArray());Check(JsonFiles.Read<Snapshot>(Path.Combine(root,"snapshot-wire.json"))!.Pools[0].Name=="test","pool name serializes");});
Test("overflow limit for every enhancement and duplicate combination",()=>{
 for(int la=-1;la<=5;la++)for(int lb=-1;lb<=5;lb++)for(int a=0;a<=10;a++)for(int b=0;b<=10-a;b++){
  var d=Draft();d.ExcludeOverflow=true;d.Rows[1].Enabled=true;d.Rows[1].B="2";
  Costume[] stock=[new(){Id=1,Level=la},new(){Id=2,Level=lb},new(){Id=3,Level=-1}];
  var got=Rules.Evaluate(d,Cards(a,b),stock);int ea=Math.Min(a,5-la),eb=Math.Min(b,5-lb);
  Check(got.A==ea&&got.B==eb&&got.Matched==(ea>=3||(ea==1&&eb>=2)),"overflow oracle");
 }
});
Test("overflow can be disabled and is inactive for distinct mode",()=>{var d=Draft();d.ExcludeOverflow=true;Costume[] stock=[new(){Id=1,Level=4},new(){Id=2,Level=5},new(){Id=3,Level=-1}];var got=Rules.Count(d,Cards(3,2),stock);Check(got.A==1&&got.B==0&&got.Overflow==4&&got.OverflowSlots.Count(x=>x)==4,"cap each costume");d.ExcludeOverflow=false;Check(Rules.Count(d,Cards(3,2),stock).A==3,"disabled");d.ExcludeOverflow=true;d.CountCopies=false;got=Rules.Count(d,Cards(3,2),stock);Check(got.A==1&&got.B==1&&got.Overflow==0,"distinct unchanged");});
Test("overflow validation accounts for available upgrades and unknown inventory",()=>{var d=Draft();d.ExcludeOverflow=true;Costume[] stock=[new(){Id=1,Level=4},new(){Id=2,Level=5},new(){Id=3,Level=0}];Check(Rules.Validate(d,stock)!="","only one useful A cannot meet three");d.Rows[1].Enabled=true;d.Rows[1].B="0";Check(Rules.Validate(d,stock)=="","one A condition reachable");stock[0].Level=-2;Check(Rules.Validate(d,stock)!="","unknown blocks");Throws(()=>Rules.Count(d,Cards(1,0),stock));Check(Rules.Capacity(new Costume{Level=-1})==6&&Rules.Capacity(new Costume{Level=0})==5&&Rules.Capacity(new Costume{Level=5})==0,"unlock and upgrade capacity");});
Test("old settings enable overflow by default without losing groups",()=>{var path=Path.Combine(root,"old-rule.json");File.WriteAllText(path,"{\"Threshold\":\"3\",\"CountCopies\":true,\"A\":[1],\"B\":[2]}");var host=JsonFiles.Read<RuleDraft>(path)!;Check(host.ExcludeOverflow&&host.A.SequenceEqual(new[]{1}),"host upgrade");using var ms=File.OpenRead(path);var wire=(RuleDraft)new DataContractJsonSerializer(typeof(RuleDraft)).ReadObject(ms)!;Check(wire.ExcludeOverflow&&wire.A.SequenceEqual(new[]{1}),"Mono upgrade");});
Test("machine evaluates useful copies before allowing a stop",()=>{var s=Snap();s.Pools[0].Costumes.First(x=>x.Id==1).Level=4;s.Result=Cards(3,0);var c=Command(s.At);c.Rules.ExcludeOverflow=true;c.Rules.Rows[1].Enabled=true;c.Rules.Rows[1].B="1";s.Pools[0].Costumes.First(x=>x.Id==2).Level=0;var m=new AutomationMachine();Check(m.Step(c,s,0,0,0,s.At)&&m.A==1,"three copies only one upgrade not matched");s.Result=Cards(3,1);Check(!m.Step(c,s,1,1,0,s.At+10000000)&&m.State=="matched"&&m.A==1&&m.B==1,"useful secondary stops");});
Test("publication fault pauses and retries the identical result without dropping it",()=>{
 var gate=new PublicationBuffer();var s=Snap();int writes=0;Snapshot? delivered=null;
 Check(!gate.Publish(s,_=>throw new IOException("temporary lock"))&&gate.Blocked,"lock retains snapshot");
 Check(!gate.Retry(_=>throw new UnauthorizedAccessException("scanner"))&&gate.Blocked,"permission transient still waits");
 Check(gate.Retry(value=>{writes++;delivered=value;})&&!gate.Blocked&&ReferenceEquals(delivered,s),"same data delivered after recovery");
 Check(gate.Retry(_=>writes++)&&writes==1,"no duplicate write after recovery");
});
Test("long run survives missing polls and transient writes without extra requests",()=>{
 var s=Snap();long now=s.At;var port=new FakePort(s);var client=new ClientController(port,()=>now);var draft=Draft();draft.Interval="100";client.Start(s,s.Pools[0],draft);
 var machine=new AutomationMachine();long response=0,result=0;int requests=0;bool outstanding=false;
 for(int tick=0;tick<2000;tick++){
  now+=TimeSpan.FromMilliseconds(100).Ticks;s.At=now;port.FailWrites=tick%47==0;
  client.Poll(tick%31==0?null:s);port.FailWrites=false;
  if(outstanding&&tick%3==0){response++;result++;outstanding=false;}
  bool draw=machine.Step(port.Last,s,response,result,0,now);
  if(draw){Check(!outstanding,"no duplicate request during recovery");outstanding=true;requests++;}
  Check(client.Running&&machine.State=="pending","run not interrupted by transient host I/O");
 }
 Check(requests>500&&machine.Rolls==requests-1,"long run keeps one outstanding draw");client.Stop();Check(!machine.Step(port.Last,s,response,result,0,now)&&machine.State=="stopped","stop still wins");
});
Test("persistent missing UI and unrecovered result animation stop with reasons",()=>{
 var s=Snap();var c=Command(s.At);var m=new AutomationMachine();m.Step(c,s,0,0,0,s.At);s.PoolId=0;m.Step(c,s,0,0,0,s.At+1000000);
 long now=s.At+TimeSpan.FromSeconds(16).Ticks;c.Expires=now+TimeSpan.FromSeconds(10).Ticks;
 Check(!m.Step(c,s,0,0,0,now)&&m.State=="error"&&m.Message.Contains("15"),"UI missing deadline");
 s=Snap();c=Command(s.At);m=new();m.Step(c,s,0,0,0,s.At);s.ResultReady=false;m.Step(c,s,1,0,0,s.At+1000000);
 now=s.At+TimeSpan.FromSeconds(121).Ticks;c.Expires=now+TimeSpan.FromSeconds(10).Ticks;
 Check(!m.Step(c,s,1,0,0,now)&&m.State=="error"&&m.Message.Contains("120"),"animation deadline separate from server timeout");
});
Test("bilingual catalogs and source coverage",()=>{assertions+=LocalizationTests.Run();});
JsonFiles.Write(Path.Combine(root,"checks.json"),new{status="passed",scenarios=checks.Count,assertions,checks,realGameTouched=false});Console.WriteLine($"{checks.Count} scenarios / {assertions} assertions passed");
sealed class FakePort(Snapshot s):IClientPort{
 public Snapshot S=s;public Control Last=new();public bool FailWrites;private readonly GameProcess process=new(s.ProcessId,s.ProcessStart,"fake.exe");
 public GameProcess? Find()=>process;public Snapshot? Read()=>S;public void Write(Control command){if(FailWrites)throw new IOException("fixture lock");Last=JsonFiles.Clone(command);}
 public Task ConnectAsync(Action<string> progress,CancellationToken cancellation)=>Task.CompletedTask;
}
