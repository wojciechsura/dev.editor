using Dev.Editor.BinAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition
{
    class Scope
    {
        private readonly Dictionary<string, BaseData> contents;

        public Scope(Scope parentScope = null)
        {
            ParentScope = parentScope;
            contents = new Dictionary<string, BaseData>();
        }

        public void AddContent(string identifier, BaseData value)
        {
            if (contents.ContainsKey(identifier))
                throw new InvalidOperationException($"Identifier with key {identifier} already exists!");

            contents[identifier] = value;
        }

        public (bool, BaseData) TryGetContent(string identifier)
        {
            if (contents.ContainsKey(identifier))
                return (true, contents[identifier]);
            else
                return (false, null);
        }

        public Scope ParentScope { get; }
    }
}
