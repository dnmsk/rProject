using System;

namespace CommonUtils.WatchfulSloths {
    /// <summary>
    /// Подтюненный будильник, который говорит сколько времени сейчас, если не указано иное
    /// </summary>
    public class WatchTuned : IWatch {
        private readonly DateTime? _now;

        public WatchTuned(DateTime? now = null) {
            _now = now;
        }

        public DateTime Now() {
            return _now ?? DateTime.Now;
        }
    }
}
