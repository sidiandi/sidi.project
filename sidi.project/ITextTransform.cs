using Sidi.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi
{
    internal interface ITextTransform
    {
        string Transform(string input);
    }

    internal static class ITextTransformExtensions
    {
        public static void Transform(this ITextTransform transform, TextReader input, TextWriter output)
        {
            for (; ;)
            {
                var line = input.ReadLine();
                if (line == null)
                {
                    break;
                }
                output.WriteLine(transform.Transform(line));
            }
        }

        static bool IsBinaryFile(LPath f)
        {
            if (string.Equals(f.Extension, ".ico"))
            {
                return true;
            }
            return false;
        }

        public static void CreateFromTemplate(this ITextTransform transform, LPath source, LPath destination)
        {
            // Console.WriteLine("{0} -> {1}", source, destination);

            if (source.IsFile)
            {
                destination.EnsureParentDirectoryExists();
                if (IsBinaryFile(source))
                {
                    source.CopyFile(destination);
                }
                else
                {
                    using (var w = destination.WriteText())
                    using (var r = source.ReadText())
                    {
                        transform.Transform(r, w);
                    }
                }
            }
            else
            {
                foreach (var i in source.Info.GetChildren())
                {
                    transform.CreateFromTemplate(i.FullName, destination.CatDir(transform.Transform(i.Name)));
                }
            }
        }
    }


}
