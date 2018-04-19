using Sidi.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sidi
{
    internal class ConsoleTool
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly LPath executable;

        public ConsoleTool(LPath executable)
        {
            this.executable = executable ?? throw new ArgumentNullException(nameof(executable));
        }

        public LPath WorkingDirectory { get; set; }
        public bool AssertSuccess { get; set; }

        public static string GetArgumentString(string[] arguments)
        {
            return String.Join(" ", arguments.Select(QuoteIfRequired));
        }

        public static string QuoteIfRequired(string x)
        {
            if (Regex.IsMatch(x, @"\s"))
            {
                return Quote(x);
            }
            else
            {
                return x;
            }
        }

        public static string Quote(string x)
        {
            return "\"" + Regex.Replace(x, "\"", @"\""") + "\"";
        }

        public async Task<Result> Run(params string[] arguments)
        {
            var argumentString = GetArgumentString(arguments);
            log.InfoFormat("start: {0} {1}", this.executable, argumentString);

            var startInfo = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = argumentString,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            if (this.WorkingDirectory != null)
            {
                startInfo.WorkingDirectory = WorkingDirectory;
            }

            var p = new Process { StartInfo = startInfo };

            var result = new Result();
            p.Start();
            result.Begin = DateTime.UtcNow;

            var output = new StringWriter();
            var error = new StringWriter();

            log.InfoFormat("started: {0} {1}, pid={2}", p.StartInfo.FileName, p.StartInfo.Arguments, p.Id);

            await Task.WhenAll(
                CopyToAsync(p.StandardOutput, output, Console.Out),
                CopyToAsync(p.StandardError, error, Console.Out));

            result.End = DateTime.UtcNow;
            result.ExitCode = p.ExitCode;
            result.Output = output.ToString();
            result.Error = error.ToString();

            log.InfoFormat("exit with {3}: {0} {1}, pid={2}", p.StartInfo.FileName, p.StartInfo.Arguments, p.Id, p.ExitCode);

            if (p.ExitCode != 0)
            {
                throw new Exception(String.Format("exit with non-zero exit code {3}: {0} {1}, pid={2}", p.StartInfo.FileName, p.StartInfo.Arguments, p.Id, p.ExitCode));
            }

            log.Info(result);
            return result;
        }

        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        static async Task CopyToAsync(TextReader source, params TextWriter[] dest)
        {
            var buf = new char[4096];
            for (; ; )
            {
                var count = await source.ReadBlockAsync(buf, 0, buf.Length);
                if (count <= 0)
                {
                    break;
                }
                await Task.WhenAll(dest.Select(_ => _.WriteAsync(buf, 0, count)).ToArray());
            }
        }

        public class Result
        {
            public DateTime Begin { get; set; }
            public DateTime End { get; set; }
            public int ExitCode { get; set; }
            public string Output { get; set; }
            public string Error { get; set; }

            public override string ToString()
            {
                return
@"Begin: " + Begin.ToString("o") + @"
End: " + End.ToString("o") + @"
ExitCode: " + ExitCode.ToString() + @"
Output
====
" + Output + @"
====
Error
====
" + Error + @"====";
            }
        }
    }
}
