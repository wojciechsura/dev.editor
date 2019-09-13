﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Data
{
    public class StructData : BaseData
    {
        public StructData(string name, string type, List<BaseData> members)
            : base(name)
        {
            Type = type;
            Children = members;
        }

        public string Type { get; }
        public override IList<BaseData> Children { get; }
    }
}
