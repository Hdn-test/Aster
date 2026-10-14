using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.IO.Compression;
using Aster;

class CoreTests {
 static int assertions;
 static void Check(bool condition,string label){if(!condition)throw new Exception("FAIL: "+label);assertions++;Console.WriteLine("PASS: "+label);}
 static void Throws(Action action,string label){bool thrown=false;try{action();}catch{thrown=true;}Check(thrown,label);}
 static string Put(string root,string name,string value){string p=Path.Combine(root,name);Directory.CreateDirectory(Path.GetDirectoryName(p));File.WriteAllText(p,value);return p;}
 public static int Main(string[] args){try{
  string root=Path.Combine(Path.GetFullPath(args.Length>0?args[0]:"artifacts"),"test-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
  var store=new StateStore(Path.Combine(root,"state"));var ops=new Operations(store);var job=new Job();
  string source=Path.Combine(root,"Downloads"),dest=Path.Combine(root,"Organized");
  Put(source,"vacation.jpg","photo");Put(source,"Project A/cover.png","cover");Put(source,"Project B/cover.png","different");Put(source,"notes.docx","document");Put(source,"setup.exe","installer");Put(source,"unknown.xyz","unknown");
  Put(source,"Games/MyGame/texture.png","game image");Put(source,"Games/MyGame/steam_api64.dll","dll");
  Put(source,"Custom Tool/tool.exe","app");Put(source,"Custom Tool/config.json","data");Put(source,"Custom Tool/icon.png","app image");
  Put(source,"Unity Project/play.exe","exe");Put(source,"Unity Project/play_Data/image.png","asset");
  var plan=Organizer.Plan(source,dest,Rule.Defaults(),CancellationToken.None);
  Check(plan.Count==5,"Only personal files and standalone installer selected");
  Check(plan.Any(p=>p.Destination==Path.Combine(dest,"Downloads","Project A","Ảnh","cover.png")),"Source and project context retained");
  Check(plan.Any(p=>p.Destination==Path.Combine(dest,"Downloads","Project B","Ảnh","cover.png")),"Identical names in different projects stay separate");
  Check(!plan.Any(p=>p.Source.Contains("Games")||p.Source.Contains("Custom Tool")||p.Source.Contains("Unity Project")),"Game / app / Unity assets excluded");
  Check(plan.Any(p=>p.Destination==Path.Combine(dest,"Downloads","Office","Word","notes.docx")),"Word files move into Office / Word");
  Check(Catalog.Office("budget.xlsx")&&Catalog.Office("slides.pptx")&&!Catalog.Office("texture.png"),"Office recognizes Excel and PowerPoint separately from images");
  var photos=Catalog.Scan(source,"Ảnh",CancellationToken.None);Check(photos.Count==3&&!photos.Any(p=>p.Path.Contains("Games")||p.Path.Contains("Custom Tool")),"Photo library excludes ingame and application assets");
  string gameRoot=Path.Combine(root,"Game Library");Put(gameRoot,"CoolGame/CoolGame.exe","game");Put(gameRoot,"CoolGame/crashhandler.exe","helper");Put(gameRoot,"CoolGame/texture.png","asset");Put(gameRoot,"CoolGame/engine.dll","asset");Put(gameRoot,"AnotherGame/AnotherGame.exe","game");
  var games=Catalog.Scan(gameRoot,"Game",CancellationToken.None);Check(games.Count==2&&games.All(g=>g.Kind=="Game"&&g.Path.EndsWith(".exe")&&!g.Name.Contains("crash")),"Game catalog exposes only game entry executables");
  var nav=new NavigationHistory();nav.Visit(source);nav.Visit(gameRoot);Check(nav.CanBack&&nav.Back()==source&&nav.CanForward,"Back / forward navigation remembers paths");nav.Visit(dest);Check(!nav.CanForward&&nav.Back()==source,"New navigation discards old forward branch");
  Check(Fs.Canonical("C:")=="C:\\","Drive roots remain absolute");
  var separate=Organizer.Plan(source,dest,Rule.Defaults(),CancellationToken.None,"Downloads (2)");Check(separate.All(p=>Fs.Within(p.Destination,Path.Combine(dest,"Downloads (2)"))),"Different roots sharing a name can use separate buckets");
  string unnamed=Path.Combine(root,"Portable App");Put(unnamed,"app.exe","app");Put(unnamed,"icon.png","icon");Check(Organizer.Plan(unnamed,dest,Rule.Defaults(),CancellationToken.None).Count==0,"Portable app root excluded even without DLL");
  Throws(()=>Organizer.Plan(source,Path.Combine(source,"Output"),Rule.Defaults(),CancellationToken.None),"Nested output rejected");
  Throws(()=>Organizer.Plan(source,root,Rule.Defaults(),CancellationToken.None),"Ancestor output rejected");
  Throws(()=>Organizer.Plan(source,Path.Combine(source,"Games"),Rule.Defaults(),CancellationToken.None),"Game destination rejected");
  Check(Fs.Within(Path.Combine(root,"a","b"),Path.Combine(root,"a")),"Containment matches directory boundaries");
  Check(!Fs.Within(Path.Combine(root,"ab"),Path.Combine(root,"a")),"Prefix lookalike is not containment");
  var disabled=Rule.Defaults();disabled[0].Enabled=false;Check(Organizer.Plan(source,dest,disabled,CancellationToken.None).Count==2,"Disabled rules respected");
  var bad=Rule.Defaults();bad[0].Name="../escape";Throws(()=>Organizer.Plan(source,dest,bad,CancellationToken.None),"Invalid rule folder rejected");
  ops.Organize(plan,job);Check(plan.All(p=>!File.Exists(p.Source)&&File.Exists(p.Destination)),"Plan moves every eligible file");
  Check(File.Exists(Path.Combine(source,"Games/MyGame/texture.png")),"Game source remains untouched");
  Check(store.History().Count==5,"Move journal records completed moves");
  ops.Undo(new Job());Check(plan.All(p=>File.Exists(p.Source)&&!File.Exists(p.Destination)),"Undo restores original locations");
  Check(store.History().All(r=>r.Undone),"Undo state persisted");
  string copySource=Put(root,"copy/input.txt","original"),copyDest=Put(root,"copy/output.txt","existing");
  string copied=ops.Transfer(copySource,copyDest,false,new Job());Check(File.ReadAllText(copyDest)=="existing"&&File.ReadAllText(copied)=="original", "Collision gets unique name without overwrite");
  Check(File.Exists(copySource),"Copy retains source");
  string same=ops.Transfer(copySource,copySource,true,new Job());Check(same==copySource&&File.Exists(copySource),"Move to same location is no-op");
  string folder=Path.Combine(root,"Tree");Put(folder,"nested/file.txt","nested");Directory.CreateDirectory(Path.Combine(folder,"empty"));string treeCopy=ops.Transfer(folder,Path.Combine(root,"TreeCopy"),false,new Job());
  Check(File.Exists(Path.Combine(treeCopy,"nested/file.txt"))&&Directory.Exists(Path.Combine(treeCopy,"empty")),"Folder copy preserves files and empty directories");
  Throws(()=>ops.Transfer(folder,Path.Combine(folder,"child"),true,new Job()),"Cannot move folder into itself");
  string big=Path.Combine(root,"large.bin");File.WriteAllBytes(big,new byte[3*1024*1024]);var canceled=new Job();canceled.PropertyChanged+=(s,e)=>{if(e.PropertyName=="Progress"&&canceled.Progress>0)canceled.Cancel();};string canceledDest=Path.Combine(root,"canceled.bin");
  Throws(()=>ops.Transfer(big,canceledDest,false,canceled),"Cancellation interrupts chunked copy");
  Check(File.Exists(big)&&!File.Exists(canceledDest)&&!Directory.GetFiles(root,"*.partial").Any(),"Cancelled copy cleans partial output and retains source");
  string zip=Path.Combine(root,"archive.zip");ops.Zip(new[]{folder},zip,new Job());string extracted=Path.Combine(root,"extracted");ops.Unzip(zip,extracted,new Job());Check(File.ReadAllText(Path.Combine(extracted,"Tree/nested/file.txt"))=="nested","ZIP round trip preserves file content");
  string evil=Path.Combine(root,"evil.zip");using(var z=ZipFile.Open(evil,ZipArchiveMode.Create)){var entry=z.CreateEntry("../escaped.txt");using(var writer=new StreamWriter(entry.Open()))writer.Write("bad");}
  Throws(()=>ops.Unzip(evil,Path.Combine(root,"evil-output"),new Job()),"ZIP traversal rejected");Check(!File.Exists(Path.Combine(root,"escaped.txt")),"ZIP cannot write outside destination");
  var fresh=new StateStore(Path.Combine(root,"state"));Check(fresh.History().Count==store.History().Count,"History survives restart");
  fresh.Settings.Font="Segoe UI Variable";fresh.Save();Check(new StateStore(fresh.Root).Settings.Font=="Segoe UI Variable","Settings round trip");
  Console.WriteLine("\n"+assertions+" assertions passed. Test sandbox: "+root);return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}

