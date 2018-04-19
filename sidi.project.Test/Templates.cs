using NUnit.Framework;
using Sidi;
using Sidi.IO;
using Sidi.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sidi.project.Test
{
    [TestFixture]
    public class Templates : TestBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void CheckIfTemplatesBuild()
        {
            var sourceRoot = Paths.BinDir.Parent.Parent.Parent.Parent;
            var templateDir = sourceRoot.CatDir(@"sidi.project\templates");
            log.Info(templateDir);

            var templates = new[] { "ConsoleExe" }.Select(_ => templateDir.CatDir(_));

            foreach (var template in templates)
            {
                var cmd = new ConsoleTool("cmd.exe") { WorkingDirectory = template };
                var r = cmd.Run("/c", "build.cmd").Result;
                Assert.AreEqual(0, r.ExitCode);
            }
        }
    }
}
