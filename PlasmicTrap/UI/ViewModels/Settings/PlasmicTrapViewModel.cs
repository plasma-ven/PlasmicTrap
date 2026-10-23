using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;

namespace PlasmicTrap.UI.ViewModels.Settings
{
    public class PlasmicTrapViewModel : NotifyPropertyChangedViewModel
    {
        public WebEnvironment[] WebEnvironments => Enum.GetValues<WebEnvironment>();

        public bool UpdateCheckingEnabled
        {
            get => App.Settings.Prop.CheckForUpdates;
            set => App.Settings.Prop.CheckForUpdates = value;
        }

        public bool AnalyticsEnabled
        {
            get => App.Settings.Prop.EnableAnalytics;
            set => App.Settings.Prop.EnableAnalytics = value;
        }

        public WebEnvironment WebEnvironment
        {
            get => App.Settings.Prop.WebEnvironment;
            set => App.Settings.Prop.WebEnvironment = value;
        }

        public Visibility WebEnvironmentVisibility => App.Settings.Prop.DeveloperMode ? Visibility.Visible : Visibility.Collapsed;

        public bool ShouldExportConfig { get; set; } = true;

        public bool ShouldExportLogs { get; set; } = true;

        public ICommand ExportDataCommand => new RelayCommand(ExportData);

        private void ExportData()
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");

            var dialog = new SaveFileDialog 
            { 
                FileName = $"PlasmicTrap-export-{timestamp}.zip",
                Filter = $"{Strings.FileTypes_ZipArchive}|*.zip" 
            };

            if (dialog.ShowDialog() != true)
                return;

            using var memStream = new MemoryStream();
            using var zipStream = new ZipOutputStream(memStream);

            if (ShouldExportConfig)
            {
                var files = new List<string>()
                {
                    App.Settings.FileLocation,
                    App.State.FileLocation,
                    App.FastFlags.FileLocation
                };

                AddFilesToZipStream(zipStream, files, "Config/");
            }

            if (ShouldExportLogs && Directory.Exists(Paths.Logs))
            {
                var files = Directory.GetFiles(Paths.Logs)
                    .Where(x => !x.Equals(App.Logger.FileLocation, StringComparison.OrdinalIgnoreCase));

                AddFilesToZipStream(zipStream, files, "Logs/");
            }

            zipStream.CloseEntry();
            zipStream.Finish();
            memStream.Position = 0;

            using var outputStream = File.OpenWrite(dialog.FileName);
            memStream.CopyTo(outputStream);

            Process.Start("explorer.exe", $"/select,\"{dialog.FileName}\"");
        }

        public ICommand ClearCacheCommand => new RelayCommand(ClearCache);

        private void ClearCache()
        {
            const string LOG_IDENT = "PlasmicTrapViewModel::ClearCache";

            bool robloxRunning = Process.GetProcessesByName(App.RobloxPlayerAppName).Length > 0
                || Process.GetProcessesByName(App.RobloxStudioAppName).Length > 0;

            if (robloxRunning)
            {
                Frontend.ShowMessageBox("Close Roblox before clearing the cache.", MessageBoxImage.Information);
                return;
            }

            string[] targets =
            {
                Path.Combine(Paths.LocalAppData, "Roblox", "Downloads"),
                Path.Combine(Paths.LocalAppData, "Roblox", "rbx-storage"),
                Paths.Downloads
            };

            long freedBytes = 0;

            foreach (string target in targets)
            {
                if (!Directory.Exists(target))
                    continue;

                App.Logger.WriteLine(LOG_IDENT, $"Clearing '{target}'");

                freedBytes += DeleteDirectoryContents(new DirectoryInfo(target));
            }

            App.Logger.WriteLine(LOG_IDENT, $"Freed {freedBytes} bytes");

            Frontend.ShowMessageBox($"Cleared {FormatBytes(freedBytes)} of cached files.", MessageBoxImage.Information);
        }

        private static long DeleteDirectoryContents(DirectoryInfo directory)
        {
            const string LOG_IDENT = "PlasmicTrapViewModel::DeleteDirectoryContents";

            long freedBytes = 0;

            foreach (FileInfo file in directory.EnumerateFiles())
            {
                try
                {
                    long length = file.Length;
                    file.Delete();
                    freedBytes += length;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Failed to delete '{file.FullName}'");
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }

            foreach (DirectoryInfo subDirectory in directory.EnumerateDirectories())
            {
                freedBytes += DeleteDirectoryContents(subDirectory);

                try
                {
                    subDirectory.Delete();
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Failed to delete '{subDirectory.FullName}'");
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }

            return freedBytes;
        }

        private static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB" };
            double size = bytes;
            int unit = 0;

            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }

            return $"{size:0.#} {units[unit]}";
        }

        private void AddFilesToZipStream(ZipOutputStream zipStream, IEnumerable<string> files, string directory)
        {
            const string LOG_IDENT = "PlasmicTrapViewModel::AddFilesToZipStream";

            foreach (string file in files)
            {
                if (!File.Exists(file))
                    continue;

                try
                {
                    using FileStream fileStream = File.OpenRead(file);

                    var entry = new ZipEntry(directory + Path.GetFileName(file));
                    entry.DateTime = DateTime.Now;

                    zipStream.PutNextEntry(entry);

                    fileStream.CopyTo(zipStream);
                }
                catch (IOException ex)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Failed to open '{file}'");
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }
        }
    }
}
