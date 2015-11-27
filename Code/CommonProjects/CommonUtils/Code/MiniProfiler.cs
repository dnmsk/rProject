using System;
using System.Diagnostics;
using CommonUtils.Core.Logger;

namespace CommonUtils.Code {
    public class MiniProfiler : IDisposable {
        private readonly string _name;
        private readonly int _minTimeToSkip;
        private readonly Stopwatch _sw;

        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (MiniProfiler).FullName);
        
        public MiniProfiler(string name, int minTimeToSkip = 100) {
            _name = name;
            _minTimeToSkip = minTimeToSkip;
            _sw = Stopwatch.StartNew();
        }

        public void Dispose() {
            _sw.Stop();
            if (_minTimeToSkip < _sw.ElapsedMilliseconds) {
                _logger.Info("{0}: {1}ms", _name, _sw.ElapsedMilliseconds);
            }
        }
    }
}
