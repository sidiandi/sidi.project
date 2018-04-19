using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sidi.GetOpt;

namespace _Namespace_
{
    class _MainClass_
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        static void Main(string[] args)
        {
            Sidi.GetOpt.GetOpt.Run(new _MainClass_(), args);
        }
        
        [Usage("dummy option")]
        public bool Dummy { get; set; }

        [Usage("One line summary of program")]
        public void ProcessArguments(string[] args)
        {
        }
    }
}
