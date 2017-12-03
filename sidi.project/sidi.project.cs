using Microsoft.VisualStudio.TextTemplating;
using Sidi;
using Sidi.CommandLine;
using Sidi.Extensions;
using Sidi.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sidi.project
{
    public class Project
    {
        public static int Main(string[] args)
        {
            return GetOpt.Run(new Project(), args);
        }

        [Usage("Specify the project directory. Default is .")]
        public LPath ProjectDirectory
        {
            get
            {
                if (_ProjectDirectory == null)
                {
                    return new LPath(".").GetFullPath();
                }
                return _ProjectDirectory;
            }

            set
            {
                _ProjectDirectory = value;
            }
        }

        LPath _ProjectDirectory;

        [Usage("Initialize the project directory")]
        public void Init()
        {
            var source = Paths.BinDir.CatDir("templates", "solution");
            var destination = ProjectDirectory;

            var d = new Dictionary<string, string>
            {
                { "ProductName", "MyProduct" },
                { "CompanyName", "ACME" },
                { "CopyrightMessage", "Copyright 2017 by ACME" },
            };

            foreach (var k in new[] {
                "UpgradeCodeGuid",
                "AssemblyGuid",
                "TestAssemblyGuid",
                "SolutionProjectGuid",
                "SolutionTestProjectGuid",
                "SolutionGuid"
            })
            {
                d[k] = Guid.NewGuid().ToString();
            }

            d["ProjectName"] = d["ProductName"];
            d["TestProjectName"] = d["ProjectName"] + ".Test";
            d["Namespace"] = "Sidi." + d["ProductName"];
            d["TestNamespace"] = d["Namespace"] + ".Test";
            var transform = new DictionaryTransform(d);

            transform.CreateFromTemplate(source, destination);
            Run(destination, "git", "init");
            Run(destination, "git", "add", ".");
            Run(destination, "git", "commit", ".", "-m", "Initial version");
        }

        static void Run(LPath workingDirectory, LPath fileName, params string[] arguments)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = workingDirectory,
                    FileName = fileName,
                    Arguments = arguments.Select(_ => _.Quote()).Join(" "),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            p.Start();
            p.WaitForExit();
        }
    }
}
