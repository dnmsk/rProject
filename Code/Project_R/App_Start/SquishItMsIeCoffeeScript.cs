[assembly: WebActivator.PreApplicationStartMethod(typeof(Project_R.App_Start.SquishItMsIeCoffeeScript), "Start")]

namespace Project_R.App_Start
{
    using SquishIt.Framework;
    using SquishIt.MsIeCoffeeScript;

    public class SquishItMsIeCoffeeScript
    {
        public static void Start()
        {
            Bundle.RegisterScriptPreprocessor(new CoffeeScriptPreprocessor());
        }
    }
}