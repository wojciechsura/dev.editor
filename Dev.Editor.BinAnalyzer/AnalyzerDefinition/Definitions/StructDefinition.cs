﻿using Dev.Editor.BinAnalyzer.AnalyzerDefinition.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions
{
    class StructDefinition : BaseDefinition
    {
        private readonly List<BaseStatement> statements;

        public StructDefinition(string name, List<BaseStatement> statements)
            : base(name)
        {
            this.statements = statements;
        }

        public IList<BaseStatement> Statements => statements;
    }
}
