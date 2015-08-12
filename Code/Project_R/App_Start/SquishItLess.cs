using Project_R.App_Start;
using SquishIt.Framework;
using SquishIt.Less;
using WebActivator;

[assembly: PreApplicationStartMethod(typeof (SquishItLess), "Start")]

namespace Project_R.App_Start {
    public class SquishItLess {
        public static void Start() {
            Bundle.RegisterStylePreprocessor(new LessPreprocessor());
        }
    }
}