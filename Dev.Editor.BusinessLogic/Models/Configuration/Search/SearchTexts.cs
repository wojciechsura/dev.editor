﻿using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Search
{
    public class SearchTexts : BaseTypedItemCollection<SearchText>
    {
        public const string NAME = "SearchTexts";

        public SearchTexts(BaseItemContainer parent) 
            : base(NAME, parent)
        {
            ChildInfos = new List<BaseChildInfo>
            {
                new ChildInfo<SearchText>(SearchText.NAME, () => new SearchText())
            };
        }

        protected override IEnumerable<BaseChildInfo> ChildInfos { get; }
    }
}
