using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sidi.CommandLine;

namespace Sidi.FooBarTest
{
    [Usage("One line summary of program")]
    class FooBarTest
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        static void Main(string[] args)
        {
            GetOpt.Run(new FooBarTest(), args);
        }
        
        [Usage("dummy option")]
        public bool Dummy { get; set; }

        [ArgumentHandler]
        public void ProcessArguments(string[] args)
        {
        }
    }
}
