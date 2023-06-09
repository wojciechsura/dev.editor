﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.AnalyzerDefinition.Definitions
{
    abstract class BaseDefinition
    {
        private readonly string name;

        public BaseDefinition(string name)
        {
            this.name = name;
        }

        public string Name => name;
    }
}
