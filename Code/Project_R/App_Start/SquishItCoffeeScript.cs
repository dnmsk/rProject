[assembly: WebActivator.PreApplicationStartMethod(typeof(Project_R.App_Start.SquishItCoffeeScript), "Start")]

namespace Project_R.App_Start
{
    using SquishIt.Framework;
    using SquishIt.CoffeeScript;

    public class SquishItCoffeeScript
    {
        public static void Start()
        {
            Bundle.RegisterScriptPreprocessor(new CoffeeScriptPreprocessor());
        }
    }
}