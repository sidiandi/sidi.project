using Sidi;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace sidi.project
{
    internal class DictionaryTransform : ITextTransform
    {
        private Dictionary<string, string> dictionary;

        public DictionaryTransform(Dictionary<string, string> dictionary)
        {
            this.dictionary = dictionary;
        }

        public string Transform(string input)
        {
            return Regex.Replace(input, @"(\w+)", new MatchEvaluator((m) =>
            {
                if (dictionary.TryGetValue(m.Groups[1].Value, out var r))
                {
                    return r;
                }
                else
                {
                    return m.Value;
                }
            }));
        }
    }
}