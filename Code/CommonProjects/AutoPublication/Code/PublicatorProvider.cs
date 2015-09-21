using System;
using System.IO;
using MainLogic.WebFiles;

namespace AutoPublication.Code {
    public static class PublicatorProvider {
        private const string _backupFolderInProject = "_Backup";
        private static readonly string _archivatorPath;
        private static readonly string _tempFolder;
        static PublicatorProvider() {
            _archivatorPath = SiteConfiguration.GetConfigurationProperty("ArchivatorPath");
            _tempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SiteConfiguration.GetConfigurationProperty("TempPath"));
        }

        public static string Publish(string targetZipLocation, string projectName, string targetPublishLocation) {
            CmdCommandExecutor.Run("cmd.exe", "/c whoami");
            var targetZipCopied = CopyToTempFolder(targetZipLocation);
            UnpuckBuild(Path.Combine(_tempFolder, targetZipCopied));
            CopyToBackupFolder(targetPublishLocation);
            CopyToDestination(projectName, targetPublishLocation, targetZipCopied.Substring(0, targetZipCopied.IndexOf('.')));
            ClearTempFolder();
            return targetZipCopied;
        }

        private static void CopyToBackupFolder(string targetPublishLocation) {
            var backupFolder = Path.Combine(targetPublishLocation, _backupFolderInProject);
            if (Directory.Exists(backupFolder)) {
                CmdCommandExecutor.Run("cmd.exe", string.Format("/c rmdir {0} /S /Q", backupFolder));
            }
            var tempBackupFolder = Path.Combine(_tempFolder, _backupFolderInProject);
            CmdCommandExecutor.Run("cmd.exe", string.Format("/c mkdir {0}", tempBackupFolder));
            CopyDirectory(targetPublishLocation, tempBackupFolder);
            //CmdCommandExecutor.Run("robocopy.exe", string.Format("{0} {1} /IS /S /E", targetPublishLocation, tempBackupFolder));
            CmdCommandExecutor.Run("cmd.exe", string.Format("/c move \"{0}\" \"{1}\"", tempBackupFolder, targetPublishLocation));
        }

        private static string CopyToTempFolder(string targetZipLocation) {
            if (Directory.Exists(_tempFolder)) {
                ClearTempFolder();
            }
            CmdCommandExecutor.Run("cmd.exe", string.Format("/c mkdir {0}", _tempFolder));
            CmdCommandExecutor.Run("cmd.exe", string.Format("/c copy \"{0}\" {1} /Y /Z", targetZipLocation, _tempFolder));
            return targetZipLocation.Substring(targetZipLocation.LastIndexOf('\\') + 1);
        }

        private static void UnpuckBuild(string pathToArchive) {
            CmdCommandExecutor.Run(_archivatorPath, string.Format("x -o{0}\\* {1} -y", _tempFolder, pathToArchive));
        }

        private static void CopyToDestination(string projectName, string targetPublishLocation, string unpackedSubdirectory) {
            CopyDirectory(Path.Combine(_tempFolder, unpackedSubdirectory, projectName), targetPublishLocation);
            //CmdCommandExecutor.Run("robocopy.exe", string.Format("{0} {1} /IS /S /E", Path.Combine(_tempFolder, unpackedSubdirectory, projectName), targetPublishLocation));
        }

        private static void ClearTempFolder() {
            CmdCommandExecutor.Run("cmd.exe", string.Format("/c rmdir {0} /S /Q", _tempFolder));
        }
        private static void CopyDirectory(string sourcePath, string destPath) {
            if(!Directory.Exists(destPath)) {
                //CmdCommandExecutor.Run("cmd.exe", string.Format("/c mkdir {0}", destPath));
                Directory.CreateDirectory(destPath);
            }
            if (sourcePath.Trim('\\').EndsWith("log", StringComparison.InvariantCultureIgnoreCase)) {
                return;
            }

            //CmdCommandExecutor.Run("cmd.exe", string.Format("/c copy \"{0}\" {1} /Y /Z", sourcePath, destPath));
            foreach(string file in Directory.GetFiles(sourcePath)) {
                string dest = Path.Combine(destPath, Path.GetFileName(file));
                File.Copy(file, dest, true);
            }

            foreach(string folder in Directory.GetDirectories(sourcePath)) {
                string dest = Path.Combine(destPath, Path.GetFileName(folder));
                CopyDirectory(folder, dest);
            }
        }
    }
}