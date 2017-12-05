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
    public class Project : IArgumentHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        [Usage("Specify the product name. ")]
        public string Product { get; set; }

        [Usage("Specify the company.")]
        public string Company { get; set; }

        LPath _ProjectDirectory;

        static void Init(LPath destination, string Product, string Company)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (string.IsNullOrEmpty(Product))
            {
                throw new ArgumentException("message", nameof(Product));
            }

            if (Company == null)
            {
                throw new ArgumentNullException(nameof(Company));
            }

            var source = Paths.BinDir.CatDir("templates", "solution");

            log.InfoFormat("Create project from template {0} in {1}", source, destination);

            destination.EnsureDirectoryExists();

            if (destination.Children.Any())
            {
                throw new Exception("Cannot create project in non-empty directory");
            }

            var d = new Dictionary<string, string>
            {
                { "ProductName", Product },
                { "CompanyName", Company },
                { "CopyrightMessage", String.Format("Copyright {0} by {1}", DateTime.Now.Year, Company) },
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

        string GuessProduct(string productProperty, LPath dest)
        {
            if (String.IsNullOrEmpty(productProperty))
            {
                return dest.FileNameWithoutExtension;
            }
            return productProperty;
        }

        string GuessCompany(string companyProperty)
        {
            if (String.IsNullOrEmpty(companyProperty))
            {
                return LPath.GetValidFilename(Environment.UserName);
            }
            return companyProperty;
        }

        public void ProcessArguments(string[] args)
        {
            foreach (var d in args.Select(_ => new LPath(_).GetFullPath()))
            {
                Init(d, GuessProduct(Product,d), GuessCompany(Company));
            }
        }
    }
}
