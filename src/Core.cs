using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.IO.Compression;

namespace Aster {
 public class Settings {
  public string Destination = "";
  public List<string> Sources = new List<string>();
  public List<string> Pins = new List<string>();
  public List<string> Recent = new List<string>();
  public List<RecentItem> Activity = new List<RecentItem>();
  public List<string> GameRoots = new List<string>();
  public int Version = 2;
  public List<string> Queries = new List<string>();
  public List<SearchPreset> Presets = new List<SearchPreset>();
  public Dictionary<string,string> AppGroups = new Dictionary<string,string>();
  public Dictionary<string,string> SourceBuckets = new Dictionary<string,string>();
  public List<AppShortcut> Shortcuts = new List<AppShortcut>();
  public List<LibraryItem> LibraryItems = new List<LibraryItem>();
  public List<SortRoute> SortRoutes = SortRoute.Defaults();
  public bool KeepSourceFolders = true;
  public string Theme = "Dark";
  public string Font = "Segoe UI";
  public double RowHeight = 48;
  public bool Motion = true;
  public bool Startup = false;
  public bool MinimizeToTray = false;
  public List<Rule> Rules = Rule.Defaults();
 }
 public class AppShortcut {public string Name;public string Path;public string Group;}
 public class SearchPreset {public string Text;public string Location;public int Type;public int Size;public int Date;}
 public class RecentItem {public string Path;public DateTime Opened;}
 public class NavigationHistory {
  readonly List<string> paths=new List<string>();int position=-1;
  public bool CanBack {get{return position>0;}}public bool CanForward {get{return position>=0&&position<paths.Count-1;}}
  public void Visit(string path){path=System.IO.Path.GetFullPath(path);if(position>=0&&paths[position].Equals(path,StringComparison.OrdinalIgnoreCase))return;if(CanForward)paths.RemoveRange(position+1,paths.Count-position-1);paths.Add(path);position=paths.Count-1;}
  public string Back(){return CanBack?paths[--position]:null;}public string Forward(){return CanForward?paths[++position]:null;}
 }
 public class Rule {
  public string Name {get;set;}
  public string Extensions {get;set;}
  public bool Enabled {get;set;}
  public static List<Rule> Defaults() { return new List<Rule> {
   new Rule {Name="Ảnh",Extensions=".jpg .jpeg .png .webp .gif .heic .bmp .tiff .tif",Enabled=true},
   new Rule {Name="Video",Extensions=".mp4 .mov .mkv .avi .webm",Enabled=true},
   new Rule {Name="Word",Extensions=".doc .docx .docm .rtf .odt",Enabled=true},
   new Rule {Name="Excel",Extensions=".xls .xlsx .xlsm .csv .ods",Enabled=true},
   new Rule {Name="PowerPoint",Extensions=".ppt .pptx .pptm .odp",Enabled=true},
   new Rule {Name="PDF",Extensions=".pdf",Enabled=true},
   new Rule {Name="Tài liệu",Extensions=".txt .md",Enabled=true},
   new Rule {Name="Âm thanh",Extensions=".mp3 .wav .flac .aac .ogg .m4a",Enabled=true},
   new Rule {Name="File nén",Extensions=".zip .rar .7z .tar .gz",Enabled=true},
   new Rule {Name="Bộ cài",Extensions=".exe .msi .msix .iso",Enabled=true}
  }; }
 }
 public class MoveRecord { public string From; public string To; public DateTime Time; public bool Undone; }
 public class StateStore {
  public readonly string Root;
  public Settings Settings;
  public bool HasLibraryData;
  public List<MoveRecord> Journal;
  readonly object gate = new object();
  readonly JavaScriptSerializer json = new JavaScriptSerializer {MaxJsonLength=int.MaxValue};
  public StateStore(string root) {
   Root=root; Directory.CreateDirectory(root);
   Settings=Read<Settings>("settings.json") ?? new Settings();
   var savedLibrary=Read<List<LibraryItem>>("library.json");HasLibraryData=savedLibrary!=null;if(savedLibrary!=null)Settings.LibraryItems=savedLibrary;
   Journal=Read<List<MoveRecord>>("history.json") ?? new List<MoveRecord>();
  }
  T Read<T>(string file) where T:class {
   string path=Path.Combine(Root,file);
   if(!File.Exists(path)) return null;
   try { return json.Deserialize<T>(File.ReadAllText(path)); }
   catch { File.Copy(path,path+".corrupt-"+DateTime.UtcNow.Ticks,true); return null; }
  }
  void Write(string file,object data) {
   string path=Path.Combine(Root,file), temp=path+".tmp";
   File.WriteAllText(temp,json.Serialize(data));
   if(File.Exists(path)) File.Replace(temp,path,null); else File.Move(temp,path);
  }
  public void Save() { lock(gate){Write("library.json",Settings.LibraryItems);HasLibraryData=true;Write("settings.json",Settings);} }
  public void Record(string from,string to) {
   lock(gate) { Journal.Add(new MoveRecord {From=from,To=to,Time=DateTime.Now}); Write("history.json",Journal); }
  }
  public void MarkUndone(MoveRecord record) {lock(gate){record.Undone=true;Write("history.json",Journal);}}
  public List<MoveRecord> History() {lock(gate)return Journal.ToList();}
 }
 public partial class Entry {
  public string Name {get;set;}
  public string Path {get;set;}
  public bool IsDirectory {get;set;}
  public long Bytes {get;set;}
  public DateTime Modified {get;set;}
  public string Kind {get;set;}
  public string Context {get;set;}
  public string Collection {get;set;}
  public bool Favorite {get;set;}
  public bool Missing {get;set;}
  public string Availability {get{return Missing?"Không tìm thấy file":"Sẵn sàng";}}
  public string Size {get {return Missing?"—":IsDirectory ? "—" : Fs.Size(Bytes);}}
  public string Date {get {return Modified.ToString("dd/MM/yyyy  HH:mm");}}
  public string Glyph {get {return IsDirectory ? "\uF12B" : Kind=="Ảnh" ? "\uEB9F" : Kind=="Video" ? "\uE714" : Kind=="Âm thanh" ? "\uE8D6" : Kind=="File nén" ? "\uF012" : "\uE8A5";}}
  public string Accent {get {return IsDirectory ? "#7FA9FC" : Kind=="Ảnh" ? "#AE96F0" : Kind=="Video" ? "#F19DAD" : Kind=="Tài liệu" ? "#6ECBC4" : "#D9B77B";}}
 }
 public static class Fs {
  public static string Canonical(string path){if(path.Length==2&&path[1]==':')path+="\\";path=System.IO.Path.GetFullPath(path);return path.Length>System.IO.Path.GetPathRoot(path).Length?path.TrimEnd('\\','/'):path;}
  public static string Size(long n) {double v=n;string[] u={"B","KB","MB","GB","TB"};int i=0;while(v>=1024&&i<4){v/=1024;i++;}return v.ToString(i==0?"0":"0.#")+" "+u[i];}
  public static bool Within(string path,string root) {path=System.IO.Path.GetFullPath(path).TrimEnd('\\','/');root=System.IO.Path.GetFullPath(root).TrimEnd('\\','/');return path.Equals(root,StringComparison.OrdinalIgnoreCase)||path.StartsWith(root+System.IO.Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase);}
  public static bool Reparse(string p) {return (File.GetAttributes(p)&FileAttributes.ReparsePoint)!=0;}
  public static bool HasLinkedAncestor(string path){for(string p=System.IO.Path.GetFullPath(path);!String.IsNullOrEmpty(p);p=System.IO.Path.GetDirectoryName(p)){if((Directory.Exists(p)||File.Exists(p))&&Reparse(p))return true;}return false;}
  public static Entry Get(string p) {
   bool d=Directory.Exists(p);var f=new FileInfo(p);
   return new Entry {Name=System.IO.Path.GetFileName(p.TrimEnd('\\')),Path=p,IsDirectory=d,Bytes=d?0:f.Length,Modified=File.GetLastWriteTime(p),Kind=d?"Thư mục":Category(p,Rule.Defaults())??"Khác"};
  }
  public static List<Entry> List(string p) {
   var rows=new List<Entry>(); foreach(var f in Directory.EnumerateFileSystemEntries(p))try{rows.Add(Get(f));}catch(IOException){}catch(UnauthorizedAccessException){}
   return rows.OrderByDescending(x=>x.IsDirectory).ThenBy(x=>x.Name,StringComparer.CurrentCultureIgnoreCase).ToList();
  }
  public static IEnumerable<string> Walk(string root,CancellationToken ct,Action<string> error=null) {
   var stack=new Stack<string>();stack.Push(root);
   while(stack.Count>0){ct.ThrowIfCancellationRequested();string d=stack.Pop();string[] files,dirs;
    try {if(Reparse(d))continue;files=Directory.GetFiles(d);dirs=Directory.GetDirectories(d);}catch(Exception e){if(error!=null)error(d+": "+e.Message);continue;}
    foreach(string f in files){ct.ThrowIfCancellationRequested();if(!Reparse(f))yield return f;}
    foreach(string sub in dirs)stack.Push(sub);
   }
  }
  public static string Category(string file,List<Rule> rules) {string ext=System.IO.Path.GetExtension(file).ToLowerInvariant();var rule=rules.FirstOrDefault(r=>r.Enabled&&(r.Extensions??"").ToLowerInvariant().Split(new[]{' ',',',';'},StringSplitOptions.RemoveEmptyEntries).Contains(ext));return rule==null?null:rule.Name;}
  public static bool Protected(string dir) {
   string[] roots={Environment.GetFolderPath(Environment.SpecialFolder.Windows),Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)};
   if(roots.Where(r=>!String.IsNullOrEmpty(r)).Any(r=>Within(dir,r)))return true;
   for(string current=System.IO.Path.GetFullPath(dir);!String.IsNullOrEmpty(current);current=System.IO.Path.GetDirectoryName(current)) {
    string n=System.IO.Path.GetFileName(current).ToLowerInvariant();
    if(new[]{"games","game","steamapps","epic games","riot games","gog games","node_modules",".git"}.Contains(n))return true;
    try{if(Directory.Exists(current)&&(Directory.EnumerateFiles(current,"*.dll").Any()||Directory.EnumerateFiles(current,"*.exe").Any(f=>Directory.Exists(System.IO.Path.Combine(current,System.IO.Path.GetFileNameWithoutExtension(f)+"_Data")))||File.Exists(System.IO.Path.Combine(current,"appmanifest.acf"))))return true;}catch{}
   }return false;
  }
  public static string Unique(string path) {if(!File.Exists(path)&&!Directory.Exists(path))return path;string dir=System.IO.Path.GetDirectoryName(path),name=System.IO.Path.GetFileNameWithoutExtension(path),ext=System.IO.Path.GetExtension(path);for(int i=2;;i++){string p=System.IO.Path.Combine(dir,name+" ("+i+")"+ext);if(!File.Exists(p)&&!Directory.Exists(p))return p;}}
 }
 public class PlanItem {public string Source {get;set;} public string Destination {get;set;} public string Category {get;set;} public string Context {get;set;} public string Name {get{return System.IO.Path.GetFileName(Source);}}}
 public static class Organizer {
  public static List<PlanItem> Plan(string source,string dest,List<Rule> rules,CancellationToken ct,string sourceLabel=null) {
   source=System.IO.Path.GetFullPath(source);dest=System.IO.Path.GetFullPath(dest);
   if(Fs.HasLinkedAncestor(source)||Fs.HasLinkedAncestor(dest))throw new InvalidOperationException("Nguồn và đích tự động không được đi qua junction hoặc symbolic link.");
   if(Fs.Within(dest,source)||Fs.Within(source,dest))throw new InvalidOperationException("Thư mục nguồn và đích phải tách biệt, không nằm trong nhau.");
   if(Fs.Protected(source))throw new InvalidOperationException("Nguồn này thuộc dữ liệu hệ thống, ứng dụng hoặc game.");
   if(Fs.Protected(dest))throw new InvalidOperationException("Đích không được nằm trong dữ liệu hệ thống, game hoặc ứng dụng.");
   sourceLabel=sourceLabel??System.IO.Path.GetFileName(source.TrimEnd('\\'));
   if(String.IsNullOrWhiteSpace(sourceLabel)||sourceLabel=="."||sourceLabel==".."||sourceLabel.IndexOfAny(System.IO.Path.GetInvalidFileNameChars())>=0)throw new InvalidOperationException("Nguồn phải là một thư mục có tên, không phải gốc ổ đĩa.");
   var result=new List<PlanItem>();
   var stack=new Stack<string>();stack.Push(source);
   while(stack.Count>0){ct.ThrowIfCancellationRequested();var d=stack.Pop();if(Fs.Reparse(d)||Fs.Protected(d))continue;
    string[] files,dirs;try{files=Directory.GetFiles(d);dirs=Directory.GetDirectories(d);}catch(UnauthorizedAccessException){continue;}catch(IOException){continue;}
    // A folder containing an executable and accompanying assets is treated as an app bundle.
    bool appBundle=files.Any(f=>System.IO.Path.GetExtension(f).Equals(".exe",StringComparison.OrdinalIgnoreCase))&&(dirs.Length>0||files.Any(f=>!System.IO.Path.GetExtension(f).Equals(".exe",StringComparison.OrdinalIgnoreCase)));
    // Downloads can contain standalone installers alongside personal files.
    // Other executable bundles remain intact, including an explicitly chosen bundle root.
    if(appBundle&&(!d.Equals(source,StringComparison.OrdinalIgnoreCase)||!System.IO.Path.GetFileName(source).Equals("Downloads",StringComparison.OrdinalIgnoreCase)))continue;
    foreach(var f in files){ct.ThrowIfCancellationRequested();if(Fs.Reparse(f))continue;string category=Fs.Category(f,rules);if(category==null)continue;
     if(String.IsNullOrWhiteSpace(category)||category=="."||category==".."||category.IndexOfAny(System.IO.Path.GetInvalidFileNameChars())>=0)throw new InvalidOperationException("Tên nhóm phải là tên thư mục hợp lệ.");
     string context=d.Substring(source.TrimEnd('\\').Length).TrimStart('\\');
     string target=new[]{"Word","Excel","PowerPoint","PDF"}.Contains(category)?System.IO.Path.Combine("Office",category):category;
     result.Add(new PlanItem {Source=f,Category=category,Context=context.Length==0?"Gốc thư mục":context,Destination=System.IO.Path.Combine(dest,sourceLabel,context,target,System.IO.Path.GetFileName(f))});
    }foreach(var sub in dirs)stack.Push(sub);
   }return result;
  }
 }
 public class Job : INotifyPropertyChanged {
  public string Id=Guid.NewGuid().ToString("N");
  public string Name {get;set;}
  public string Detail {get;set;}
  public string Group {get;set;}
  public DateTime Started {get;set;}
  string currentFile="";
  public string CurrentFile {get{return currentFile;}set{currentFile=value;Changed("CurrentFile");}}
  string status="Đang chờ";double progress;
  public string Status {get{return status;}set{status=value;Changed("Status");}}
  public double Progress {get{return progress;}set{progress=value;Changed("Progress");}}
  public CancellationTokenSource Cancellation=new CancellationTokenSource();
  public ManualResetEventSlim Pause=new ManualResetEventSlim(true);
  public bool Finished;
  public event PropertyChangedEventHandler PropertyChanged;
  void Changed(string n){var h=PropertyChanged;if(h!=null)h(this,new PropertyChangedEventArgs(n));}
  public void Check(){Cancellation.Token.ThrowIfCancellationRequested();Pause.Wait(Cancellation.Token);}
  public void Toggle(){if(Finished)return;if(Pause.IsSet){Pause.Reset();Status="Tạm dừng";}else{Pause.Set();Status="Đang xử lí";}}
  public void Cancel(){if(!Finished){Cancellation.Cancel();Pause.Set();}}
 }
 public class Operations {
  readonly StateStore store;
  readonly SemaphoreSlim slots=new SemaphoreSlim(2);
  public Operations(StateStore s){store=s;}
  public async Task Run(Job j,Action action){try{await slots.WaitAsync(j.Cancellation.Token);try{j.Check();j.Status="Đang xử lí";await Task.Run(action);j.Status="Hoàn tất";j.Progress=100;}finally{slots.Release();}}catch(OperationCanceledException){j.Status="Đã hủy";}catch(Exception e){j.Status="Lỗi: "+e.Message;}finally{j.Finished=true;}}
  public void CopyFile(string src,string dst,Job j) {
   j.CurrentFile=System.IO.Path.GetFileName(src);
   if(Fs.Reparse(src))throw new IOException("Không xử lí liên kết hệ thống.");
   Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dst));string temp=dst+".aster-"+j.Id+".partial";
   try{using(var input=new FileStream(src,FileMode.Open,FileAccess.Read,FileShare.Read,1048576,FileOptions.SequentialScan))using(var output=new FileStream(temp,FileMode.CreateNew,FileAccess.Write,FileShare.None,1048576,FileOptions.SequentialScan)){
    byte[] b=new byte[1048576];int n;long done=0;while((n=input.Read(b,0,b.Length))>0){j.Check();output.Write(b,0,n);done+=n;j.Progress=input.Length==0?100:done*100.0/input.Length;}output.Flush(true);
   }j.Check();File.SetLastWriteTimeUtc(temp,File.GetLastWriteTimeUtc(src));File.Move(temp,dst);}finally{if(File.Exists(temp))File.Delete(temp);}
  }
  public string Transfer(string src,string requested,bool move,Job j) {
   j.CurrentFile=System.IO.Path.GetFileName(src);
   j.Check();if(Fs.Reparse(src))throw new IOException("Không xử lí junction hoặc symbolic link.");
   if(move&&System.IO.Path.GetFullPath(src).Equals(System.IO.Path.GetFullPath(requested),StringComparison.OrdinalIgnoreCase))return src;
   bool dir=Directory.Exists(src);if(dir&&Fs.Within(requested,src))throw new IOException("Không thể chuyển thư mục vào chính nó.");
   string dst=Fs.Unique(requested);Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dst));
   if(move&&System.IO.Path.GetPathRoot(src).Equals(System.IO.Path.GetPathRoot(dst),StringComparison.OrdinalIgnoreCase)){
    if(dir)Directory.Move(src,dst);else File.Move(src,dst);store.Record(src,dst);return dst;
   }
   if(dir){Directory.CreateDirectory(dst);foreach(var f in Fs.Walk(src,j.Cancellation.Token)){j.Check();CopyFile(f,System.IO.Path.Combine(dst,f.Substring(src.TrimEnd('\\').Length).TrimStart('\\')),j);}
    CopyEmptyDirectories(src,dst,j);if(move){Directory.Delete(src,true);store.Record(src,dst);}
   }else {CopyFile(src,dst,j);if(move){File.Delete(src);store.Record(src,dst);}}
   return dst;
  }
  void CopyEmptyDirectories(string src,string dst,Job j){foreach(var d in Directory.GetDirectories(src)){j.Check();if(Fs.Reparse(d))throw new IOException("Thư mục có liên kết hệ thống; nguồn được giữ lại.");string target=System.IO.Path.Combine(dst,System.IO.Path.GetFileName(d));Directory.CreateDirectory(target);CopyEmptyDirectories(d,target,j);}}
  public void Organize(List<PlanItem> plan,Job j){for(int i=0;i<plan.Count;i++){j.Check();var p=plan[i];if(File.Exists(p.Source))Transfer(p.Source,p.Destination,true,j);j.Progress=(i+1)*100.0/Math.Max(1,plan.Count);}}
  public void Undo(Job j){var records=store.History().Where(r=>!r.Undone).Reverse().ToList();foreach(var r in records){j.Check();if(File.Exists(r.From)||Directory.Exists(r.From))throw new IOException("Vị trí cũ đã có dữ liệu: "+r.From);if(!File.Exists(r.To)&&!Directory.Exists(r.To))throw new IOException("Không tìm thấy file đã chuyển: "+r.To);TransferUndo(r.To,r.From,j);store.MarkUndone(r);}}
  void TransferUndo(string src,string dst,Job j){Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dst));if(System.IO.Path.GetPathRoot(src).Equals(System.IO.Path.GetPathRoot(dst),StringComparison.OrdinalIgnoreCase)){if(Directory.Exists(src))Directory.Move(src,dst);else File.Move(src,dst);}else if(File.Exists(src)){CopyFile(src,dst,j);File.Delete(src);}else throw new IOException("Hoàn tác thư mục khác ổ chưa được hỗ trợ.");}
  public void Zip(IEnumerable<string> paths,string dest,Job j){dest=Fs.Unique(dest);string temp=dest+".partial";try{using(var zip=ZipFile.Open(temp,ZipArchiveMode.Create)){foreach(string p in paths){j.Check();var files=Directory.Exists(p)?Fs.Walk(p,j.Cancellation.Token):new[]{p};foreach(string f in files){j.Check();string name=Directory.Exists(p)?System.IO.Path.GetFileName(p)+"/"+f.Substring(p.TrimEnd('\\').Length).TrimStart('\\').Replace('\\','/'):System.IO.Path.GetFileName(f);var entry=zip.CreateEntry(name,CompressionLevel.Optimal);using(var input=File.OpenRead(f))using(var output=entry.Open()){byte[] b=new byte[1048576];int n;while((n=input.Read(b,0,b.Length))>0){j.Check();output.Write(b,0,n);}}}}}j.Check();File.Move(temp,dest);}finally{if(File.Exists(temp))File.Delete(temp);}}
  public void Unzip(string src,string dest,Job j){dest=Fs.Unique(dest);Directory.CreateDirectory(dest);using(var zip=ZipFile.OpenRead(src)){foreach(var e in zip.Entries){j.Check();string target=System.IO.Path.GetFullPath(System.IO.Path.Combine(dest,e.FullName));if(!Fs.Within(target,dest))throw new IOException("File nén có đường dẫn không hợp lệ.");if(e.FullName.EndsWith("/")){Directory.CreateDirectory(target);continue;}Directory.CreateDirectory(System.IO.Path.GetDirectoryName(target));using(var input=e.Open())using(var output=new FileStream(target,FileMode.CreateNew)){byte[] b=new byte[1048576];int n;while((n=input.Read(b,0,b.Length))>0){j.Check();output.Write(b,0,n);}}}}}
 }
}


