using System;
using System.Collections.Generic;

namespace CommonUtils.ExtendedTypes {
    /// <summary>
    /// Обратный кеш
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    public abstract class BackwardCache<TK, TV>
    where TV : IComparable {
        private readonly object _lockObj = new object();
        private volatile Dictionary<TK, TV> _map;
        private volatile Dictionary<TV, TK> _backMap;

        protected BackwardCache() {
            _map = new Dictionary<TK, TV>();
            _backMap = new Dictionary<TV, TK>();
        }

        /// <summary>
        /// Взятие значения
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TV GetValue(TK value) {
            var result = default (TV);
            if (IsFirstValid(value)) {
                value = ProcessFirstValue(value);
                if (!_map.TryGetValue(value, out result)) {
                    var secondValue = GetSecondValue(value);
                    //NOTE: Решение проблемы инициализации кеша неверным значением
                    if (secondValue.CompareTo(result) != 0) {
                        value = GetFirstValue(secondValue);
                    }
                    result = secondValue;
//                    if (IsSecondValid(result)) {
                        lock (_lockObj) {
                            _map[value] = result;
                            if (!_backMap.ContainsKey(result)) {
                                _backMap[result] = value;
                            }
                        }
//                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Взятие значения
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TK GetValue(TV value) {
            var result = default(TK);
            if (IsSecondValid(value)) {
                value = ProcessSecondValue(value);
                if (!_backMap.TryGetValue(value, out result)) {
                    result = GetFirstValue(value);
//                    if (IsFirstValid(result)) {
                        lock (_lockObj) {
                            _backMap[value] = result;
                            if (!_map.ContainsKey(result)) {
                                _map[result] = value;
                            }
                        }
//                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Удалить запись
        /// </summary>
        /// <param name="val"></param>
        public void DropCacheRecord(TK val) {
            if (IsFirstValid(val) && _map.ContainsKey(val)) {
                lock (_lockObj) {
                    TV res;
                    if (_map.TryGetValue(val, out res)) {
                        _map.Remove(val);
                        _backMap.Remove(res);
                    }
                }
            }
        }

        /// <summary>
        /// Удалить запись
        /// </summary>
        /// <param name="val"></param>
        public void DropCacheRecord(TV val) {
            if (IsSecondValid(val) && _backMap.ContainsKey(val)) {
                lock (_lockObj) {
                    TK res;
                    if (_backMap.TryGetValue(val, out res)) {
                        _map.Remove(res);
                        _backMap.Remove(val);
                    }
                }
            }
        }

        protected abstract TK GetFirstValue(TV val);
        protected abstract TV GetSecondValue(TK val);

        protected abstract bool IsSecondValid(TV val);
        protected abstract bool IsFirstValid(TK val);

        protected virtual TK ProcessFirstValue(TK val) {
            return val;
        }

        protected virtual TV ProcessSecondValue(TV val) {
            return val;
        }
    }
}