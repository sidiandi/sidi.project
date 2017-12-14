using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sidi.CommandLine;

namespace _Namespace_
{
    [Usage("One line summary of program")]
    class _ProductName_
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        static void Main(string[] args)
        {
            GetOpt.Run(new _ProductName_(), args);
        }
        
        [Usage("dummy option")]
        public bool Dummy { get; set; }

        [ArgumentHandler]
        public void ProcessArguments(string[] args)
        {
        }
    }
}
