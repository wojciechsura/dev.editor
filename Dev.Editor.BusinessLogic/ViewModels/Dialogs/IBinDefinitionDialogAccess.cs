﻿using Dev.Editor.BusinessLogic.Models.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Dialogs
{
    public interface IBinDefinitionDialogAccess
    {
        void CloseDialog(BinDefinitionDialogResult model, bool result);
    }
}
