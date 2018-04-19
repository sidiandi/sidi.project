using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace sidi.project.Test
{
    [TestFixture]
    public class DictionaryTransformTest
    {
        [Test]
        public void Replace()
        {
            var d = new Dictionary<string, string>
            {
                { "ProductName", "MyProduct" },
                { "CompanyName", "ACME" },
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
                d[k] = Guid.Parse("3789d247-33f6-4de0-bf67-02e70134c079").ToString();
            }

            var t = new DictionaryTransform(d);

            Assert.AreEqual("<ProjectGuid>{3789d247-33f6-4de0-bf67-02e70134c079}</ProjectGuid>", t.Transform("<ProjectGuid>{_SolutionTestProjectGuid_}</ProjectGuid>"));
            Assert.AreEqual("This is the MyProduct solution from ACME.", t.Transform("This is the _ProductName_ solution from _CompanyName_."));
            
        }
    }
}