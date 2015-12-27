namespace Project_B.CodeServerSide.DataProvider.DataHelper.RawData {
    public class RawTemplateObj<T> where T : new() {
        public RawObject RawObject { get; }
        public T Object { get; }

        public RawTemplateObj() {
            Object = new T();
            RawObject = new RawObject();
        }
    }
}