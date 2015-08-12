[assembly: WebActivator.PreApplicationStartMethod(typeof(Project_R.App_Start.SquishItSass), "Start")]

namespace Project_R.App_Start
{
    using SquishIt.Framework;
    using SquishIt.Sass;

    public class SquishItSass
    {
        public static void Start()
        {
            Bundle.RegisterStylePreprocessor(new SassPreprocessor());
        }
    }
}