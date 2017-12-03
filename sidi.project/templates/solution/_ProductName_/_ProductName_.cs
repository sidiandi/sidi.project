using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sidi.CommandLine;

namespace _Namespace_
{
    class _ProductName_ : IArgumentHandler
    {
        static void Main(string[] args)
        {
			GetOpt.Run(new _ProductName_(), args);
        }
		
		[Usage("dummy option")]
		public bool Dummy { get; set; }

        public void ProcessArguments(string[] args)
        {
        }
    }
}
