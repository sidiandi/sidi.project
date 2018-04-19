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
    public class EndToEnd : TestBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static ConsoleTool GetTool()
        {
            var exe = Paths.BinDir.Parent.Parent.CatDir("sidi.project", "bin", "sidi.project.exe");
            return new ConsoleTool(exe);
        }

        [Test]
        public void ShowHelp()
        {
            var r = GetTool().Run("--help").Result;
        }

        [Test]
        public void CreateLibraryProject()
        {
            var testProjectRoot = TestFile("name_074244f56b5344c4931f406de990e26f");
            var project = new Project()
            {
                TemplateDirectory = Paths.BinDir.Parent.Parent.CatDir("sidi.project", "bin", "templates")
            };
            testProjectRoot.EnsureNotExists();
            project.CreateProject(new[] { testProjectRoot });
        }
    }
}
