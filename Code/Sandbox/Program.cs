using System;
using System.Threading;
using CommonUtils.Core.Config;

namespace Sandbox {
    class Program {
        static void Main(string[] args) {
            ConfigHelper.RegisterConfigTarget("Application.xml", arg => {
                Console.WriteLine(arg.ToString());
            });
            while (true) {
                Thread.Sleep(1000);
            }
        }
    }
}
