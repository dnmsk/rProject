﻿using System.Collections.Concurrent;
using System.Threading;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using CommonUtils.WatchfulSloths.WatchfulThreads;

namespace CommonUtils.WatchfulSloths {
    /// <summary>
    /// Класс зоркий ленивец.
    /// Предназначен для обеспечения доступа к данным, актуальность которых не кретична,
    /// однако кретично время доступа.
    /// Процесс обновления данных работает в одном потоке для экономии (кучу не плодить).
    /// </summary>
    /// <remarks>
    /// Для обновлений данных используются правила <see cref="ISlothMoveRule"/>.
    /// <see cref="SlothMoveByTime{T}"/> - если нужно переодическое обновление данных по времени.
    /// <see cref="SlothMoveByTimeEco{T}"/> - если нужно переодическое обновление данных по времени только если были обращения.
    /// <see cref="SlothMoveByFirstSuccess{T}"/> - если можно загрузить один раз.
    /// Для доступа к данным использовать <see cref="Get{T}"/> от typeof(правила), либо сохранить ссылку на правило и 
    /// брать значение из него напрямую.
    /// </remarks>
    public class WatchfulSloth : Singleton<WatchfulSloth>, IWatchfulSloth {
        private readonly ManualResetEvent _stopEvent;

        /// <summary>
        /// Словарь для хранения правил, для ленивца.
        /// </summary>
        private readonly ConcurrentBag<ISlothMoveRule> _rulesMap = new ConcurrentBag<ISlothMoveRule>();
        /// <summary>
        /// Logger для текущего класса
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(WatchfulSloth).FullName);

        private const int _wakeUpInterval = 100;

        public WatchfulSloth() {
            _stopEvent = new ManualResetEvent(false);
            WakeUp();
            new Thread(() => {
                while (!_stopEvent.WaitOne(_wakeUpInterval)) {
                    WakeUp();
                }
            }).Start();
        }
        
        /// <summary>
        /// Установка правила, по которому будет производится контроль данных и их актуальности.
        /// </summary>
        /// <param name="moveRule">Правило.</param>
        /// <returns>Возвращает true, если правило успешно установлено, иначе false.</returns>
        public bool SetMove(ISlothMoveRule moveRule) {
            return SetMove(moveRule, _rulesMap);
        }

        /// <summary>
        /// Установка правила, по которому будет производится контроль данных и их актуальности.
        /// </summary>
        /// <param name="moveRule">Правило.</param>
        /// <returns>Возвращает true, если правило успешно установлено, иначе false.</returns>
        protected bool SetMove(ISlothMoveRule moveRule, ConcurrentBag<ISlothMoveRule> rulesMap) {
            rulesMap.Add(moveRule);
            return true;
        }

        /// <summary>
        /// Возвращает данные, за которыми следит ленивец.
        /// </summary>
        /// <typeparam name="T">Тип данных.</typeparam>
        /// <param name="keyMove">Ключ, по которому храняться данные. Обычно это typeof(правило).</param>
        /// <param name="defValue">Значение по умолчанию.</param>
        /// <returns>Возвращает актуальные данные в случае успеха, либо значени по умолчанию.</returns>
//        public T Get<T>(Type keyMove, T defValue) {
//            ISlothMoveRule obj;
//            _rulesMap.TryGetValue(keyMove, out obj);
//            return (obj as SlothMoveRule<T>).Return(e => e.Result, defValue);
//        }

        private void WakeUp() {
            _rulesMap
                .With(m => m)
                .Each(v => v.If(e => e.IsNeedMove).Do(e => TaskRunner.Instance.AddAction(() => { e.Move(_wakeUpInterval); })));
        }
    }
}
