using System;
using System.IO;
using System.Security.Permissions;
using System.Threading;

namespace CommonUtils.Code {
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class FileMonitor {
        private readonly string _path;
        private readonly string _file;
        private FileSystemWatcher _watcher;
        private DateTime _lastWrite;

        public bool WaitTimeoutBeforeUpdate { get; set; }

        public delegate void FileChangedDelegate();
        public event FileChangedDelegate Changed;

        public FileMonitor(string path, string file) {
            _path = path;
            _file = file;
        }

        public void Start() {
            if (string.IsNullOrEmpty(_path) || string.IsNullOrEmpty(_file)) {
                throw new ArgumentException("Файл не указан");
            }
            if (!Directory.Exists(_path)) {
                throw new ArgumentException("Папка не существует: " + _path);
            }

            _watcher = new FileSystemWatcher(_path, _file);
            _watcher.Created += OnChanged;
            _watcher.Changed += OnChanged;

            //начинаем слушать
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop() {
            _watcher.EnableRaisingEvents = false;
        }

        private void OnChanged(object source, FileSystemEventArgs e) {
            //перечитываем файл, только если изменилась дата последней записи
            DateTime currentLastWrite = File.GetLastWriteTime(Path.Combine(_path, _file));
            if (_lastWrite != currentLastWrite) {
                if (WaitTimeoutBeforeUpdate) {
                    Thread.Sleep(3000);
                }
                FileChange();
                _lastWrite = currentLastWrite;
            }
        }

        private void FileChange() {
            if (Changed != null) {
                Changed();
            }
        }
    }
}
