using Microsoft.VisualStudio.TextTemplating;
using Sidi;
using Sidi.GetOpt;
using Sidi.Extensions;
using Sidi.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace sidi.project
{
    [Usage("Create new C# projects from templates")]
    public class Project
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static int Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();
            return GetOpt.Run(new Project(), args);
        }

        internal Project()
        {
            TemplateDirectory = Paths.BinDir.CatDir("templates", "ConsoleExe");
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

        public LPath TemplateDirectory { get; set; }

        static async Task Init(LPath destination, string Product, string Company, LPath templateDirectory)
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

            var source = templateDirectory.CatDir("ConsoleExe");

            log.InfoFormat("Create project from template {0} in {1}", source, destination);
            if (!source.IsDirectory)
            {
                throw new ArgumentOutOfRangeException(nameof(source), source, "not a directory");
            }

            destination.EnsureDirectoryExists();

            if (destination.Children.Any())
            {
                throw new Exception("Cannot create project in non-empty directory");
            }

            var d = new Dictionary<string, string>
            {
                { "ProductNamePlaceHolder", Product },
                { "CompanyNamePlaceHolder", Company },
                { "CopyrightMessagePlaceHolder", String.Format("Copyright {0} by {1}", DateTime.Now.Year, Company) },
                { "LicenseHeader", @"This file is part of _ProductName_.

ProductNamePlaceHolder is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ProductNamePlaceHolder is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with hagen. If not, see <http://www.gnu.org/licenses/>.
" }
            };

            d["CommentHeader"] = String.Format("{0}\r\n{1}\r\n", d["CopyrightMessagePlaceHolder"], d["LicenseHeader"]);

            foreach (var k in new[] {
                "99890FA4-3413-4B88-9561-F16DB96C2F64",
                "4BFAC30D-938A-404A-9C35-ABD085D3C19D"
            })
            {
                d[k] = Guid.NewGuid().ToString();
            }

            var transform = new DictionaryTransform(d);

            transform.CreateFromTemplate(source, destination);
            destination.Parent.CatDir("packages").EnsureDirectoryExists();
            var sn = new ConsoleTool(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\sn.exe") { WorkingDirectory = destination };
            await sn.Run("-k", "key.snk");
            var nuget = new ConsoleTool("nuget") { WorkingDirectory = destination };
            await nuget.Run("update", LPath.GetValidFilename(d["ProductNamePlaceHolder"]) + ".sln");
            var git = new ConsoleTool("git.exe") { WorkingDirectory = destination };
            await git.Run("init");
            await git.Run("add", ".");
            await git.Run("commit", ".", "-m", "Initial version");
            var cmd = new ConsoleTool("cmd.exe") { WorkingDirectory = destination };
            await cmd.Run("/c", "build.cmd test");
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

        [Usage("Create new C# projects from templates")]
        public void CreateProject(LPath[] projectRoot)
        {
            foreach (var r in projectRoot.Select(_ => new LPath(_).GetFullPath()))
            {
                Init(
                    r, 
                    GuessProduct(Product, r), 
                    GuessCompany(Company),
                    TemplateDirectory
                    ).Wait();
            }
        }
    }
}
