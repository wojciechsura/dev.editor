﻿using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration
{
    public class StoredFiles : ConfigCollection<StoredFile>
    {
        internal const string NAME = "StoredFiles";

        public StoredFiles(BaseItemContainer parent) 
            : base(NAME, parent, StoredFile.NAME)
        {

        }
    }
}
