using NUnit.Framework;
using Sidi;
using Sidi.IO;
using Sidi.Test;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sidi.project.Test
{
    [TestFixture]
    public class ConsoleToolTest : TestBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void RunDir()
        {
            var cmd = new ConsoleTool("cmd.exe")
            {
                AssertSuccess = true
            };

            Task.WhenAll(
                cmd.Run("/c", "echo", "hello"),
                cmd.Run("/c", "dir")
                ).Wait();
        }
    }
}
