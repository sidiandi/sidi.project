using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Namespace_.Test
{
    [TestFixture]
    public class _MainClass_Test
    {
        [Test]
        public void Construct()
        {
            var x = new _MainClass_();
        }

        [Test]
        public void ShowHelp()
        {
            Sidi.GetOpt.GetOpt.Run(new _MainClass_(), new[]{"--help"});
        }
    }
}
