namespace CommonUtils.ExtendedTypes {
    public class ObjectWrapper<T> {
        public T Obj { get; set; }

        public ObjectWrapper(T val) {
            Obj = val;
        } 
    }
}
