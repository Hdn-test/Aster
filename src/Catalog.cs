using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Aster {
 public static class Catalog {
  public static bool Office(string path){return new[]{".doc",".docx",".docm",".rtf",".odt",".xls",".xlsx",".xlsm",".csv",".ods",".ppt",".pptx",".pptm",".odp",".pdf"}.Contains(Path.GetExtension(path).ToLowerInvariant());}
  public static bool Helper(string path){string n=Path.GetFileNameWithoutExtension(path).ToLowerInvariant();return new[]{"unins","uninstall","setup","install","crash","redist","dxsetup","vcredist","vc_redist","update","helper","reporter","launcherhelper"}.Any(s=>n.Contains(s));}
  public static List<Entry> Scan(string root,string type,CancellationToken token){
   var result=new List<Entry>();var stack=new Stack<Tuple<string,int>>();stack.Push(Tuple.Create(root,0));
   while(stack.Count>0&&result.Count<50000){token.ThrowIfCancellationRequested();var node=stack.Pop();string dir=node.Item1;string[] files,dirs;try{if(Fs.Reparse(dir))continue;files=Directory.GetFiles(dir);dirs=Directory.GetDirectories(dir);}catch(IOException){continue;}catch(UnauthorizedAccessException){continue;}
    if(type=="Game"){
     var launch=files.Where(f=>new[]{".exe",".lnk",".url"}.Contains(Path.GetExtension(f).ToLowerInvariant())&&!Helper(f)).OrderByDescending(f=>Path.GetFileNameWithoutExtension(f).Equals(Path.GetFileName(dir),StringComparison.OrdinalIgnoreCase)).ThenByDescending(f=>SafeLength(f)).FirstOrDefault();
     if(launch!=null){var item=Fs.Get(launch);item.Kind="Game";item.Context=dir;result.Add(item);continue;}
     if(node.Item2<8)foreach(var sub in dirs){string n=Path.GetFileName(sub).ToLowerInvariant();if(!new[]{"textures","assets","content","data","movies","sound","audio","redist",".git","node_modules"}.Contains(n)&&!n.EndsWith("_data"))stack.Push(Tuple.Create(sub,node.Item2+1));}
    }else{
     bool bundle=files.Any(f=>Path.GetExtension(f).Equals(".exe",StringComparison.OrdinalIgnoreCase))||files.Any(f=>Path.GetExtension(f).Equals(".dll",StringComparison.OrdinalIgnoreCase));
     if(Fs.Protected(dir)||(bundle&&!Path.GetFileName(dir).Equals("Downloads",StringComparison.OrdinalIgnoreCase)))continue;
     foreach(var f in files){token.ThrowIfCancellationRequested();try{if(Fs.Reparse(f))continue;var entry=Fs.Get(f);if(type=="Ảnh"?entry.Kind=="Ảnh":Office(f)){entry.Context=dir;result.Add(entry);}}catch(IOException){}catch(UnauthorizedAccessException){}}
     foreach(var sub in dirs)stack.Push(Tuple.Create(sub,node.Item2+1));
    }
   }return result.OrderBy(e=>e.Name,StringComparer.CurrentCultureIgnoreCase).ToList();
  }
  static long SafeLength(string f){try{return new FileInfo(f).Length;}catch{return 0;}}
 }
}

