using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Aster;

class UITests {
 static int count;
 static void Check(bool okay,string text){if(!okay)throw new Exception(text);count++;Console.WriteLine("PASS: "+text);}
 static T Find<T>(Window w,string name) where T:class {return w.FindName(name) as T;}
 static void Click(Window w,string name){Find<Button>(w,name).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));}
 static void Wait(Func<bool> done){DateTime until=DateTime.UtcNow.AddSeconds(8);while(!done()&&DateTime.UtcNow<until){var frame=new DispatcherFrame();var timer=new DispatcherTimer(DispatcherPriority.Background) {Interval=TimeSpan.FromMilliseconds(20)};timer.Tick+=(s,e)=>{timer.Stop();frame.Continue=false;};timer.Start();Dispatcher.PushFrame(frame);}if(!done())throw new Exception("UI wait timed out");}
 [STAThread] public static int Main(string[] args){try{
  string root=Path.GetFullPath(args[0]);string sandbox=Path.Combine(root,"ui-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(sandbox);
  string source=Path.Combine(sandbox,"Downloads"),dest=Path.Combine(sandbox,"Organized");Directory.CreateDirectory(source);Directory.CreateDirectory(Path.Combine(source,"Project A"));
  File.WriteAllText(Path.Combine(source,"Proposal.docx"),"fixture");var png=new PngBitmapEncoder();png.Frames.Add(BitmapFrame.Create(BitmapSource.Create(2,2,96,96,PixelFormats.Bgra32,null,new byte[]{255,100,70,255,255,100,70,255,255,100,70,255,255,100,70,255},8)));using(var output=File.Create(Path.Combine(source,"cover.png")))png.Save(output);File.WriteAllText(Path.Combine(source,"Project A","notes.pdf"),"fixture");
  var settings=new StateStore(Path.Combine(sandbox,"state"));settings.Settings.Sources.Add(source);settings.Save();
  var application=new Application();SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));var desktop=new Desktop(settings.Root);var window=desktop.Create();
  foreach(string page in new[]{"Home","Explorer","Search","Storage","Transfers","Organize","Apps","Settings"}){Click(window,"Nav"+page);Check(Find<FrameworkElement>(window,page+"Page").Visibility==Visibility.Visible,page+" navigation");desktop.Render(Path.Combine(root,page.ToLowerInvariant()+".png"),page);}
  typeof(Desktop).GetMethod("OpenFolder",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(desktop,new object[]{source});
  Wait(()=>Find<DataGrid>(window,"FileList").Items.Count==3);Check(Find<TextBox>(window,"PathBox").Text==source,"Explorer loads real sandbox folder");
  Find<DataGrid>(window,"FileList").SelectedIndex=1;Check(Find<Button>(window,"Reveal").IsEnabled,"Selected file details shown");
  Wait(()=>Find<Image>(window,"DetailImage").Visibility==Visibility.Visible);Check(true,"Image preview decodes without retaining file lock");using(var exclusive=new FileStream(Path.Combine(source,"cover.png"),FileMode.Open,FileAccess.ReadWrite,FileShare.None))Check(true,"Preview releases image file");
  Click(window,"ViewToggle");Check(Find<ListBox>(window,"FileGrid").Visibility==Visibility.Visible,"Grid view switches");Click(window,"ViewToggle");
  desktop.Render(Path.Combine(root,"explorer.png"),"Explorer");
  typeof(Desktop).GetMethod("OpenFolder",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(desktop,new object[]{Path.Combine(source,"Project A")});Wait(()=>Find<DataGrid>(window,"FileList").Items.Count==1);Check(Find<Button>(window,"Back").IsEnabled,"Back button enabled after folder navigation");Click(window,"Back");Wait(()=>Find<TextBox>(window,"PathBox").Text==source&&Find<DataGrid>(window,"FileList").Items.Count==3);Check(Find<Button>(window,"Forward").IsEnabled,"Back restores parent and enables forward");Click(window,"Forward");Wait(()=>Find<TextBox>(window,"PathBox").Text==Path.Combine(source,"Project A"));Click(window,"Back");Wait(()=>Find<DataGrid>(window,"FileList").Items.Count==3);
  typeof(Desktop).GetMethod("Recent",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(desktop,new object[]{source.ToUpperInvariant()});typeof(Desktop).GetMethod("Recent",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(desktop,new object[]{source});var recents=new StateStore(settings.Root).Settings.Activity;Check(recents.Count(r=>r.Path.Equals(source,StringComparison.OrdinalIgnoreCase))==1,"Recent paths deduplicate ignoring case");
  Click(window,"NavOffice");Check(Find<FrameworkElement>(window,"LibraryPage").Visibility==Visibility.Visible,"Office is a prominent library page");Click(window,"LibraryRefresh");Wait(()=>Find<DataGrid>(window,"LibraryList").Items.Count==2);Click(window,"OfficeWord");Wait(()=>Find<DataGrid>(window,"LibraryList").Items.Count==1);Check(((Entry)Find<DataGrid>(window,"LibraryList").Items[0]).Kind=="Word","Office Word filter");desktop.Render(Path.Combine(root,"office.png"),"Library");
  Check(((Entry)Find<DataGrid>(window,"LibraryList").Items[0]).Icon!=null,"Associated Office app icon is available");
  var storeField=typeof(Desktop).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var actualStore=(StateStore)storeField.GetValue(desktop);string gameExe=Path.Combine(sandbox,"Game Library","Nebula","Nebula.exe");Directory.CreateDirectory(Path.GetDirectoryName(gameExe));File.WriteAllText(gameExe,"fixture exe; never executed");File.WriteAllText(Path.Combine(Path.GetDirectoryName(gameExe),"texture.png"),"asset");actualStore.Settings.GameRoots.Add(Path.Combine(sandbox,"Game Library"));actualStore.Settings.Shortcuts.Add(new AppShortcut {Name="Nebula",Path=gameExe,Group="Game"});actualStore.Settings.Shortcuts.Add(new AppShortcut {Name="Non-exe",Path=Path.Combine(source,"cover.png"),Group="Ứng dụng"});actualStore.Save();
  Click(window,"NavGames");Click(window,"LibraryRefresh");Wait(()=>Find<DataGrid>(window,"LibraryList").Items.Count==1);Check(((Entry)Find<DataGrid>(window,"LibraryList").Items[0]).Name=="Nebula.exe","Game library hides ingame assets");desktop.Render(Path.Combine(root,"games.png"),"Library");
  Click(window,"NavVault");Find<ComboBox>(window,"VaultGroup").SelectedIndex=1;Check(Find<ListBox>(window,"VaultItems").Items.Count==1,"EXE-only game vault groups entries");desktop.Render(Path.Combine(root,"vault.png"),"Vault");Find<ListBox>(window,"VaultItems").SelectedIndex=0;Click(window,"VaultRemove");Check(File.Exists(gameExe)&&Find<ListBox>(window,"VaultItems").Items.Count==0,"Removing from vault keeps original EXE");
  Click(window,"NavSearch");Click(window,"BuildIndex");Wait(()=>Find<DataGrid>(window,"SearchResults").Items.Count==3);Check(true,"Index loads selected roots");
  Find<TextBox>(window,"SearchText").Text="Proposal";Wait(()=>Find<DataGrid>(window,"SearchResults").Items.Count==1);Check(true,"Debounced instant name search");
  Find<TextBox>(window,"SearchText").Text="";Find<ComboBox>(window,"TypeFilter").SelectedIndex=1;Wait(()=>Find<DataGrid>(window,"SearchResults").Items.Count==1&&((Entry)Find<DataGrid>(window,"SearchResults").Items[0]).Name=="cover.png");Check(true,"Type filter works");
  Click(window,"SaveQuery");Find<ComboBox>(window,"TypeFilter").SelectedIndex=0;((Button)Find<WrapPanel>(window,"SavedQueries").Children[0]).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));Check(Find<ComboBox>(window,"TypeFilter").SelectedIndex==1,"Saved search restores filters");
  Click(window,"NavOrganize");Find<TextBox>(window,"SourcePath").Text=source;Find<TextBox>(window,"DestPath").Text=dest;Click(window,"PreviewRules");Wait(()=>Find<Button>(window,"ApplyRules").IsEnabled);Check(Find<DataGrid>(window,"PlanList").Items.Count==3,"Rules preview displays real plan");
  desktop.Render(Path.Combine(root,"organize.png"),"Organize");Find<TextBox>(window,"DestPath").Text=Path.Combine(sandbox,"Changed");Check(!Find<Button>(window,"ApplyRules").IsEnabled,"Path changes invalidate preview");
  Click(window,"NavSettings");Find<ComboBox>(window,"DensityChoice").SelectedIndex=0;Find<CheckBox>(window,"MotionChoice").IsChecked=false;Click(window,"SaveSettings");Check(Find<DataGrid>(window,"FileList").RowHeight==36,"Row density applies");
  Find<ComboBox>(window,"ThemeChoice").SelectedIndex=1;Click(window,"SaveSettings");Check(new StateStore(settings.Root).Settings.Theme=="Light","Theme saved");desktop.Render(Path.Combine(root,"settings-light.png"),"Settings");
  Find<CheckBox>(window,"StartupChoice").IsChecked=true;Find<CheckBox>(window,"TrayChoice").IsChecked=true;Click(window,"SaveSettings");var prefs=new StateStore(settings.Root).Settings;Check(prefs.Startup&&prefs.MinimizeToTray,"Startup and tray preferences persist without touching real user startup in tests");
  Check(!Directory.Exists(Path.Combine(sandbox,"Organized")),"Rendering / preview never moves files");
  foreach(var f in Directory.GetFiles(source,"*",SearchOption.AllDirectories))File.SetLastWriteTimeUtc(f,DateTime.UtcNow.AddMinutes(-1));
  Click(window,"WatchRules");var tick=typeof(Desktop).GetMethod("WatchTick",BindingFlags.NonPublic|BindingFlags.Instance);var busy=typeof(Desktop).GetField("watchBusy",BindingFlags.NonPublic|BindingFlags.Instance);tick.Invoke(desktop,null);Wait(()=>!(bool)busy.GetValue(desktop));Check(Directory.GetFiles(source,"*",SearchOption.AllDirectories).Length==3,"Watcher waits for stable second observation");
  tick.Invoke(desktop,null);Wait(()=>!(bool)busy.GetValue(desktop));Check(Directory.GetFiles(source,"*",SearchOption.AllDirectories).Length==0&&Directory.GetFiles(Path.Combine(sandbox,"Changed"),"*",SearchOption.AllDirectories).Length==3,"Automatic watcher moves stable files into separate context buckets");Click(window,"WatchRules");
  Console.WriteLine(count+" UI checks passed. Sandbox: "+sandbox);return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}

