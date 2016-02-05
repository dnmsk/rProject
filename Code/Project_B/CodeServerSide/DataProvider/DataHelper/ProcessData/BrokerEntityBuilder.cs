using System;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData {
    public class BrokerEntityBuilder<T> where T : class, new() {
        private readonly ProcessStat _processStat;

        public BrokerEntityBuilder(ProcessStat processStat) {
            _processStat = processStat;
            SetupValidateObject(obj => obj != null);
        }

        private Func<T> _funcGetRaw;
        public BrokerEntityBuilder<T> SetupGetRaw(Func<T> funcGetRaw) {
            _funcGetRaw = funcGetRaw;
            return this;
        }

        private Func<T> _funcCreateRaw;
        public BrokerEntityBuilder<T> SetupCreateRaw(Func<T> funcCreateRaw) {
            _funcCreateRaw = funcCreateRaw;
            return this;
        }

        private Func<T, T> _funcMatchRaw;
        public BrokerEntityBuilder<T> SetupTryMatchRaw(GatherBehaviorMode gatherBehaviorMode, Func<T, T> funcMatchRaw) {
            if (gatherBehaviorMode.HasFlag(GatherBehaviorMode.TryDetectAll)) {
                _funcMatchRaw = funcMatchRaw;
            }
            return this;
        }

        private Func<T, T> _funcCreateOriginal;
        public BrokerEntityBuilder<T> SetupCreateOriginal(GatherBehaviorMode gatherBehaviorMode, Func<T, T> funcCreateOriginal) {
            if (gatherBehaviorMode.HasFlag(GatherBehaviorMode.CreateOriginal)) {
                _funcCreateOriginal = funcCreateOriginal;
            }
            return this;
        }

        private Func<T, T> _funcFinallyRaw;
        public BrokerEntityBuilder<T> SetupFinally(Func<T, T> funcFinallyRaw) {
            _funcFinallyRaw = funcFinallyRaw;
            return this;
        }

        private Func<T, bool> _funcValidateSuccess;
        public BrokerEntityBuilder<T> SetupValidateObject(Func<T, bool> funcValidateSuccess) {
            _funcValidateSuccess = funcValidateSuccess;
            return this;
        }

        public T MakeObject() {
            _processStat.TotalCount++;
            var res = default(T);
            if (_funcGetRaw != null) {
                res = _funcGetRaw();
            }
            if (res == null && _funcCreateRaw != null) {
                _processStat.CreateRawCount++;
                res = _funcCreateRaw();
            }
            if (!_funcValidateSuccess(res) && _funcMatchRaw != null) {
                res = _funcMatchRaw(res);
                _processStat.TryMatchedCount++;
            }
            if (!_funcValidateSuccess(res) && _funcCreateOriginal != null) {
                res = _funcCreateOriginal(res);
                _processStat.TryCreateOriginalCount++;
                if (_funcValidateSuccess(res)) {
                    _processStat.CreateOriginalCount++;
                }
            }
            if (_funcFinallyRaw != null) {
                res = _funcFinallyRaw(res);
            }
            if (_funcValidateSuccess(res)) {
                _processStat.FinallySuccessCount++;
            }
            return res;
        }
    }
}