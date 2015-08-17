using System;

namespace CommonUtils.ExtendedTypes {
    public class DIObjectAccessor<T> {
        private readonly Func<T> _getFunc;

        public DIObjectAccessor(Func<T> getFunc) {
            _getFunc = getFunc;
        }

        public T Get { 
            get { return _getFunc(); } 
        }
    }
}
