using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductNamePlaceHolder.Test
{
    [TestFixture]
    public class ProgramTest
    {
        [Test]
        public void ShowHelp()
        {
            Sidi.GetOpt.GetOpt.Run(new Program(), new[] { "--help" });
        }
    }
}
