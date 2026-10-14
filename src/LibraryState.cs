using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Aster {
 public class LibraryItem {public string Category;public string Path;public string Title;public string Collection="Chung";public bool Favorite;public DateTime Added;}
 public static class LibraryState {
  public static bool Accepts(string category,string path){string ext=System.IO.Path.GetExtension(path).ToLowerInvariant();return category=="Game"?new[]{".exe",".lnk",".url"}.Contains(ext):category=="Office"?Catalog.Office(path):new[]{".jpg",".jpeg",".png",".bmp",".gif",".webp",".heic",".tiff",".tif"}.Contains(ext);}
  public static int Add(Settings settings,string category,IEnumerable<string> paths){int count=0;foreach(string raw in paths){string path=Fs.Canonical(raw);if(!File.Exists(path)||!Accepts(category,path))continue;if(settings.LibraryItems.Any(x=>x.Category==category&&x.Path.Equals(path,StringComparison.OrdinalIgnoreCase)))continue;settings.LibraryItems.Add(new LibraryItem {Category=category,Path=path,Title=System.IO.Path.GetFileNameWithoutExtension(path),Added=DateTime.Now});count++;}return count;}
  public static void Remove(Settings settings,string category,IEnumerable<string> paths){var selected=paths.ToList();settings.LibraryItems.RemoveAll(x=>x.Category==category&&selected.Contains(x.Path,StringComparer.OrdinalIgnoreCase));}
  public static List<Entry> Entries(Settings settings,string category){var result=new List<Entry>();foreach(var saved in settings.LibraryItems.Where(x=>x.Category==category)){Entry entry;try{entry=Fs.Get(saved.Path);entry.Missing=!File.Exists(saved.Path);}catch{entry=new Entry {Path=saved.Path,Name=System.IO.Path.GetFileName(saved.Path),Missing=true,Kind=category};}entry.Name=String.IsNullOrWhiteSpace(saved.Title)?System.IO.Path.GetFileName(saved.Path):saved.Title;entry.Context=System.IO.Path.GetDirectoryName(saved.Path);entry.Collection=saved.Collection??"Chung";entry.Favorite=saved.Favorite;if(category=="Game")entry.Kind="Game";result.Add(entry);}return result;}
 }
 public class SortRoute {public string Category;public string Destination="";public bool Enabled=true;
  public static List<SortRoute> Defaults(){return new[]{"Ảnh","Office","File nén","Video & âm thanh","Bộ cài"}.Select(x=>new SortRoute {Category=x}).ToList();}
 }
 public static class FolderSorting {
  public static string Category(string path){if(Catalog.Office(path))return "Office";string type=Fs.Category(path,Rule.Defaults());return type=="Video"||type=="Âm thanh"?"Video & âm thanh":new[]{"Ảnh","File nén","Bộ cài"}.Contains(type)?type:null;}
  public static List<PlanItem> Plan(string source,List<SortRoute> routes,string sourceLabel,IEnumerable<string> gameRoots,IEnumerable<string> gameFiles,bool keepSources,CancellationToken token){
   source=Fs.Canonical(source);if(!Directory.Exists(source))throw new DirectoryNotFoundException("Không tìm thấy thư mục nguồn.");
   if(!routes.Any(r=>r.Enabled))throw new InvalidOperationException("Bật ít nhất một nhóm để phân loại.");
   var protectedRoots=gameRoots.Where(Directory.Exists).ToList();var excludedFiles=gameFiles.ToList();
   if(Fs.HasLinkedAncestor(source)||Fs.Protected(source)||protectedRoots.Any(p=>Fs.Within(source,p)))throw new InvalidOperationException("Nguồn thuộc thư mục game, ứng dụng hoặc hệ thống; dữ liệu được giữ nguyên.");
   if(String.IsNullOrEmpty(sourceLabel)||sourceLabel.IndexOfAny(System.IO.Path.GetInvalidFileNameChars())>=0||sourceLabel=="."||sourceLabel=="..")throw new InvalidOperationException("Chọn một thư mục nguồn có tên.");
   foreach(var route in routes.Where(r=>r.Enabled)){
    if(String.IsNullOrWhiteSpace(route.Destination))throw new InvalidOperationException("Chưa chọn nơi lưu cho nhóm "+route.Category+".");
    string dest=Fs.Canonical(route.Destination);if(dest.Equals(source,StringComparison.OrdinalIgnoreCase)||Fs.HasLinkedAncestor(dest)||Fs.Protected(dest)||protectedRoots.Any(p=>Fs.Within(dest,p)))throw new InvalidOperationException("Nơi lưu nhóm "+route.Category+" không được trùng nguồn hoặc thuộc game / ứng dụng / hệ thống.");
    if(routes.Count(r=>r.Enabled&&!String.IsNullOrWhiteSpace(r.Destination)&&Fs.Canonical(r.Destination).Equals(dest,StringComparison.OrdinalIgnoreCase))>1)throw new InvalidOperationException("Mỗi nhóm cần một thư mục đích riêng để tránh trộn dữ liệu.");
   }
   var files=Directory.GetFiles(source);bool bundled=files.Any(f=>f.EndsWith(".exe",StringComparison.OrdinalIgnoreCase))&&(Directory.GetDirectories(source).Any()||files.Any(f=>!f.EndsWith(".exe",StringComparison.OrdinalIgnoreCase)));
   if(bundled&&!new[]{"Downloads","Desktop"}.Contains(System.IO.Path.GetFileName(source),StringComparer.OrdinalIgnoreCase))throw new InvalidOperationException("Nguồn có dấu hiệu gói game / ứng dụng. Aster sẽ không tách tài nguyên khỏi gói.");
   var plan=new List<PlanItem>();
   // Only files directly inside the selected source are eligible. Never traverse game/project folders.
   foreach(string file in files){token.ThrowIfCancellationRequested();if(Fs.Reparse(file)||excludedFiles.Contains(file,StringComparer.OrdinalIgnoreCase))continue;string category=Category(file);var route=routes.FirstOrDefault(r=>r.Enabled&&r.Category==category);if(route==null)continue;
    string destination=Fs.Canonical(route.Destination);if(keepSources)destination=System.IO.Path.Combine(destination,sourceLabel);
    if(category=="Office")destination=System.IO.Path.Combine(destination,Fs.Category(file,Rule.Defaults())??"Khác");
    plan.Add(new PlanItem {Source=file,Category=category,Context=System.IO.Path.GetFileName(source),Destination=System.IO.Path.Combine(destination,System.IO.Path.GetFileName(file))});
   }return plan;
  }
 }
}

