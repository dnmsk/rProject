namespace Project_B.Models {
    public class WithFilterModel<T> {
        public readonly FilterModelBase Filter;
        public T Data { get; set; }

        public WithFilterModel(FilterModelBase filter) {
            Filter = filter;
        }
    }
    public class WithFilterModel<F, T> {
        public readonly FilterModel<F> Filter;
        public T Data { get; set; }

        public WithFilterModel(FilterModel<F> filter) {
            Filter = filter;
        }
    }
}